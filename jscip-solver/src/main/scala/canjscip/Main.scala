package canjscip

object Main extends App {

  val problemInstance = ProblemInstance(
    name = "CAN",
    numberOfUsers = 2,
    numberOfServers = 2,
    numberOfObjects = 2,
    numberOfLinks = 8,
    routingVariable = (for {
      ((userIndex, serverIndex), path) <- Vector(
        (0, 1) -> Vector(5, 1),
        (0, 2) -> Vector(6, 2),
        (1, 1) -> Vector(3, 1),
        (1, 2) -> Vector(4, 2),
        (2, 1) -> Vector(7, 1),
        (2, 2) -> Vector(8, 2),
      )
      linkIndex <- path
      objectIndex <- 1 to 2
    } yield (userIndex, objectIndex, serverIndex, linkIndex))
      .map(_ -> 1)
      .toMap
      .withDefaultValue(0),
    linkCapacities = Map(
      1 -> 1,
      2 -> 2,
      3 -> 1000,
      4 -> 1000,
      5 -> 1000,
      6 -> 1000,
      7 -> 1000,
      8 -> 1000,
    ),
    objectSizes = Map(
      1 -> 1,
      2 -> 1,
    ),
    serverCapacities = Map(
      1 -> 1,
      2 -> 2,
    ),
    linkCosts = Map(
      1 -> 5,
      2-> 5,
      3 -> 10,
      4 -> 10,
      5 -> 10,
      6 -> 10,
      7 -> 10,
      8 -> 10,
    ),
    serverStorageCosts = Map(
      (1, 1) -> 10,
      (1, 2) -> 10,
      (2, 1) -> 15,
      (2, 2) -> 15
    ),
    clientSatisfaction = 500.0,
    publisherSatisfaction = 800.0,
    minRates = Map(
    (0, 1) -> 0.2,
    (0, 2) -> 0.3,
    (1, 1) -> 0.2,
    (1, 2) -> 0.3,
    (2, 1) -> 0.4,
    (2, 2) -> 0.3
    ),
    maxRates = Map(
    (0, 1) -> 0.7,
    (0, 2) -> 0.9,
    (1, 1) -> 0.8,
    (1, 2) -> 0.9,
    (2, 1) -> 0.7,
    (2, 2) -> 0.8
    ),
  )

  Solver.solve(problemInstance).foreach { case (objective, solution) =>
    println(s"Objective: $objective")
    println(solution.mkString("\n"))
  }
}
