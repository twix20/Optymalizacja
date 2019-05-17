package canjscip

case class ProblemInstance(name: String,
                           numberOfUsers: Int,      // M
                           numberOfServers: Int,    // S
                           numberOfObjects: Int,    // N
                           numberOfLinks: Int,      // L
                           routingVariable: Map[(UserIndex, ObjectIndex, ServerIndex, LinkIndex), Int],   // a_mnsl
                           linkCapacities: Map[LinkIndex, Int],       // C_l (Memory Unit/s)
                           objectSizes: Map[ObjectIndex, Int],        // b_n (Memory Unit)
                           serverCapacities: Map[ServerIndex, Int],   // B_s (Memory Unit)
                           linkCosts: Map[LinkIndex, Double],         // k_l (Cost Unit/Memory Unit/s)
                           serverStorageCosts: Map[(ObjectIndex, ServerIndex), Int], // d_ns (Cost Unit/Memory Unit)
                           clientSatisfaction: Double,                // U_m
                           publisherSatisfaction: Double,             // U_0
                           minRates: Map[(UserIndex, ObjectIndex), Double], // y_mn_min
                           maxRates: Map[(UserIndex, ObjectIndex), Double])             // y_mn_max (Memory Unit/s)