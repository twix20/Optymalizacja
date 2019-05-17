package canjscip

import jscip._

import scala.collection.immutable

object Solver {

  System.loadLibrary("lib/jscip")

  type Solution = Map[(UserIndex, ObjectIndex, ServerIndex), (Double, Double)]

  def solve(instance: ProblemInstance): immutable.Seq[(Double, Map[(LinkIndex, LinkIndex, LinkIndex), (Double, Double)])] = {
    val scip = new Scip()

    scip.create(instance.name + "-model")

    val userIndices = 0 to instance.numberOfUsers
    val objectIndices = 1 to instance.numberOfObjects
    val serverIndices = 1 to instance.numberOfServers
    val linkIndices = 1 to instance.numberOfLinks


    // variables

    val x: Map[(UserIndex, ObjectIndex, ServerIndex), Variable] = (for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      serverIndex <- serverIndices
      index = (userIndex, objectIndex, serverIndex)
      objectiveCoef = if (userIndex == 0) instance.serverStorageCosts(objectIndex, serverIndex) else 0
    } yield index -> scip.createVar(s"x_$index", 0, 1, objectiveCoef, SCIP_Vartype.SCIP_VARTYPE_BINARY)).toMap

    val y: Map[(UserIndex, ObjectIndex, ServerIndex), Variable] = (for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      serverIndex <- serverIndices
      index = (userIndex, objectIndex, serverIndex)
      pathCapacity = instance.routingVariable.keys                  // path capacity can't be larger than minimum link capacity
        .filter { case (ui, oi, si, _) => ui == userIndex && oi == objectIndex && si == serverIndex }
        .map { case (_, _, _, li) => instance.linkCapacities(li) }
        .min
      _=println(pathCapacity)
      objectiveCoef = (if (userIndex == 0) instance.publisherSatisfaction else instance.clientSatisfaction) - linkIndices
        .map(linkIndex => instance.linkCosts(linkIndex)
          * instance.routingVariable((userIndex, objectIndex, serverIndex, linkIndex))).sum
    } yield index -> scip.createVar(s"y_$index", 0, pathCapacity, objectiveCoef, SCIP_Vartype.SCIP_VARTYPE_CONTINUOUS)).toMap


    // constraints

    val cons_2 = for {
      userIndex <- userIndices.drop(1)
      objectIndex <- objectIndices
      xs = serverIndices.map(serverIndex => x((userIndex, objectIndex, serverIndex)))
    } yield scip.createConsLinear(s"User $userIndex downloads object $objectIndex from exactly one server.",
      xs.toArray, Array.fill(xs.size)(1.0), 1.0, 1.0)

    val cons_3 = for {
      userIndex <- userIndices.drop(1)
      objectIndex <- objectIndices
      serverIndex <- serverIndices
    } yield scip.createConsLinear(s"There must be a copy of object $objectIndex on server $serverIndex if user $userIndex is assigned to it.",
      Array(x((userIndex, objectIndex, serverIndex)), x((0, objectIndex, serverIndex))), Array(1.0, -1.0), -scip.infinity(), 0)

    val cons_4 = for {
      serverIndex <- serverIndices
      xs = objectIndices.map(objectIndex => x((0, objectIndex, serverIndex)))
      serverCapacities = objectIndices.map(objectIndex => instance.serverCapacities(objectIndex).toDouble).toArray
    } yield scip.createConsLinear(s"Capacity of server $serverIndex cannot be exceeded.",
      xs.toArray, serverCapacities, -scip.infinity(), instance.serverCapacities(serverIndex))

    val cons_5 = for {
      linkIndex <- linkIndices
      routingVariables = for {
        userIndex <- userIndices
        objectIndex <- objectIndices
        serverIndex <- serverIndices
      } yield instance.routingVariable((userIndex, objectIndex, serverIndex, linkIndex)).toDouble
      xs = for {
        userIndex <- userIndices
        objectIndex <- objectIndices
        serverIndex <- serverIndices
      } yield x((userIndex, objectIndex, serverIndex))
      ys = for {
        userIndex <- userIndices
        objectIndex <- objectIndices
        serverIndex <- serverIndices
      } yield y((userIndex, objectIndex, serverIndex))
    } yield scip.createConsQuadratic(s"Capacity of link $linkIndex cannot be exceeded.",
      xs.toArray, ys.toArray, routingVariables.toArray, Array.empty, Array.empty,
      -scip.infinity(), instance.linkCapacities(linkIndex))

    val cons_6 = for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      ys = serverIndices.map(serverIndex => y((userIndex, objectIndex, serverIndex)))
    } yield scip.createConsLinear(s"Minimal required rates for user $userIndex to " +
      s"download object $objectIndex must be guaranteed.",
      ys.toArray, Array.fill(ys.size)(1.0), instance.minRates((userIndex, objectIndex)), scip.infinity())

    val cons_7 = for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      ys = serverIndices.map(serverIndex => y((userIndex, objectIndex, serverIndex)))
    } yield scip.createConsLinear(s"Minimal required rates for user $userIndex to " +
      s"download object $objectIndex must be guaranteed.",
      ys.toArray, Array.fill(ys.size)(1.0), -scip.infinity(), instance.maxRates((userIndex, objectIndex)))

    val cons_additional = for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      serverIndex <- serverIndices
      pathCapacity = instance.routingVariable.keys                  // path capacity can't be larger than minimum link capacity
        .filter { case (ui, oi, si, _) => ui == userIndex && oi == objectIndex && si == serverIndex }
        .map { case (_, _, _, li) => instance.linkCapacities(li) }
        .min
    } yield scip.createConsLinear(s"Capacity of server $serverIndex cannot be exceeded.",
      Array(y((userIndex, objectIndex, serverIndex)), x((userIndex, objectIndex, serverIndex))),
      Array(1.0, -pathCapacity), -scip.infinity(), 0)

    cons_2.foreach(scip.addCons)
    cons_3.foreach(scip.addCons)
    cons_4.foreach(scip.addCons)
    cons_5.foreach(scip.addCons)
    cons_6.foreach(scip.addCons)
    cons_7.foreach(scip.addCons)
    cons_additional.foreach(scip.addCons)

    cons_2.foreach(scip.releaseCons)
    cons_3.foreach(scip.releaseCons)
    cons_4.foreach(scip.releaseCons)
    cons_5.foreach(scip.releaseCons)
    cons_6.foreach(scip.releaseCons)
    cons_7.foreach(scip.releaseCons)
    cons_additional.foreach(scip.releaseCons)

    scip.setRealParam("limits/time", 100.0)
    scip.setRealParam("limits/memory", 10000.0)
    scip.setLongintParam("limits/totalnodes", 1000)

    scip.setMaximize()
    scip.solve()



    val result = scip.getSols
      .map { sol =>
        val res = for {
          userIndex <- userIndices
          objectIndex <- objectIndices
          serverIndex <- serverIndices
        } yield (userIndex, objectIndex, serverIndex) -> (scip.getSolVal(sol, x((userIndex, objectIndex, serverIndex))),
          scip.getSolVal(sol, y((userIndex, objectIndex, serverIndex))))
        (scip.getSolOrigObj(sol), res.toMap)
      }

    for {
      userIndex <- userIndices
      objectIndex <- objectIndices
      serverIndex <- serverIndices
    } yield {
      scip.releaseVar(y((userIndex, objectIndex, serverIndex)))
      scip.releaseVar(x((userIndex, objectIndex, serverIndex)))
    }

    scip.free()

    result.toVector
  }
}
