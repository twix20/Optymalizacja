using System;
using System.Linq;

namespace Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new SolverInstance(3, 2, 2, 8)
            {
                C_l = new uint[]{0, 5, 10, 1000, 1000, 1000, 1000, 1000, 1000},
                b_n = new uint[]{0, 1,1},
                B_s = new uint[] { 0, 1, 2},
                k_l = new uint[] { 0,5,5, 10, 10, 10, 10, 10, 10 },
                d_ns = new uint[,]
                {
                    { 0, 0, 0 },
                    { 0, 10, 15 },
                    { 0, 10, 15 }
                },
                u_0 = 500,
                u_m = 800,
                y_mn_min = new [,]
                {
                    {0, 0.2f, 0.3f},
                    {0, 0.2f, 0.3f},
                    {0, 0.4f, 0.3f},
                },
                y_mn_max = new[,]
                {
                    {0, 0.7f, 0.9f},
                    {0, 0.8f, 0.9f},
                    {0, 0.7f, 0.8f},
                },
            };

            instance.a_mnsl[0, 1, 1, 5] = 1;
            instance.a_mnsl[0, 2, 2, 6] = 1;
            instance.a_mnsl[1, 1, 1, 3] = 1;
            instance.a_mnsl[1, 2, 2, 4] = 1;
            instance.a_mnsl[2, 1, 1, 7] = 1;
            instance.a_mnsl[2, 2, 2, 8] = 1;

            var solver = new CanSolver(instance);

            var result = solver.Solve(500, 200);

            Console.WriteLine(result.ToString());
            Console.ReadKey();
        }
    }
}
