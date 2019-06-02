using System.Collections.Generic;
using System.Linq;

namespace Solver
{
    public class SolverInstance
    {
        public int MCount { get; set; }
        public int NCount { get; set; }
        public int SCount { get; set; }
        public int LCount { get; set; }

        public IEnumerable<int> M { get; set; }
        public IEnumerable<int> N { get; set; }
        public IEnumerable<int> S { get; set; }
        public IEnumerable<int> L { get; set; }

        public uint[] C_l { get; set; }
        public uint[] b_n { get; set; }
        public uint[] B_s { get; set; }
        public uint[] k_l { get; set; }
        public uint[,] d_ns { get; set; }

        /// <summary>
        /// publisherSatisfaction
        /// </summary>
        public uint u_0 { get; set; }

        /// <summary>
        /// clientSatisfaction
        /// </summary>
        public uint u_m { get; set; }
        public float[,] y_mn_min { get; set; }
        public float[,] y_mn_max { get; set; }
        public int[,,,] a_mnsl { get; set; }

        public SolverInstance(int m, int n, int s, int l)
        {
            MCount = m;
            NCount = n;
            SCount = s;
            LCount = l;

            M = Enumerable.Range(0, m);
            N = Enumerable.Range(1, n);
            S = Enumerable.Range(1, s);
            L = Enumerable.Range(1, l);
            a_mnsl = new int[m + 1, n + 1, s + 1, l + 1];
        }

        public static SolverInstance Small
        {
            get
            {
                var instance = new SolverInstance(3, 2, 2, 8)
                {
                    C_l = new uint[] {0, 1, 2, 1000, 1000, 1000, 1000, 1000, 1000},
                    b_n = new uint[] {0, 1, 1},
                    B_s = new uint[] {0, 1, 2},
                    k_l = new uint[] {0, 5, 5, 10, 10, 10, 10, 10, 10},
                    d_ns = new uint[,]
                    {
                        {0, 0, 0},
                        {0, 10, 15},
                        {0, 10, 15}
                    },
                    u_0 = 800,
                    u_m = 500,
                    y_mn_min = new[,]
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

                foreach (var n in instance.N)
                {
                    instance.a_mnsl[0, n, 1, 5] = 1;
                    instance.a_mnsl[0, n, 1, 1] = 1;

                    instance.a_mnsl[0, n, 2, 6] = 1;
                    instance.a_mnsl[0, n, 2, 2] = 1;

                    instance.a_mnsl[1, n, 1, 3] = 1;
                    instance.a_mnsl[1, n, 1, 1] = 1;

                    instance.a_mnsl[1, n, 2, 4] = 1;
                    instance.a_mnsl[1, n, 2, 2] = 1;

                    instance.a_mnsl[2, n, 1, 7] = 1;
                    instance.a_mnsl[2, n, 1, 1] = 1;

                    instance.a_mnsl[2, n, 2, 8] = 1;
                    instance.a_mnsl[2, n, 2, 2] = 1;
                }

                return instance;
            }
        }

        public static SolverInstance Medium
        {
            get
            {
                var instance = new SolverInstance(3, 3, 3, 12)
                {
                    C_l = new uint[] {0, 1, 2, 3, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
                    b_n = new uint[] {0, 1, 2, 4},
                    B_s = new uint[] {0, 2, 3, 7},
                    k_l = new uint[] {0, 5, 6, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10},
                    d_ns = new uint[,]
                    {
                        {0, 0, 0, 0},
                        {0, 10, 15, 15},
                        {0, 15, 20, 20},
                        {0, 20, 25, 35}
                    },
                    u_0 = 800,
                    u_m = 500,
                    y_mn_min = new[,]
                    {
                        {0, 0.2f, 0.3f, 0.2f},
                        {0, 0.1f, 0.3f, 0.1f},
                        {0, 0.4f, 0.3f, 0.2f},
                    },
                    y_mn_max = new[,]
                    {
                        {0, 0.7f, 0.9f, 0.9f},
                        {0, 0.8f, 0.9f, 0.9f},
                        {0, 0.7f, 0.8f, 0.9f},
                    },
                    a_mnsl =
                    {
                        [0, 1, 1, 7] = 1,
                        [0, 2, 2, 8] = 1,
                        [0, 3, 3, 9] = 1,
                        [1, 1, 1, 4] = 1,
                        [1, 2, 2, 5] = 1,
                        [1, 3, 3, 6] = 1,
                        [2, 1, 1, 10] = 1,
                        [2, 2, 2, 11] = 1,
                        [2, 3, 3, 12] = 1,
                    },
                };

                foreach (var n in instance.N)
                {
                    instance.a_mnsl[0, n, 1, 7] = 1;
                    instance.a_mnsl[0, n, 1, 1] = 1;
                    instance.a_mnsl[0, n, 2, 8] = 1;
                    instance.a_mnsl[0, n, 2, 2] = 1;
                    instance.a_mnsl[0, n, 3, 9] = 1;
                    instance.a_mnsl[0, n, 3, 3] = 1;

                    instance.a_mnsl[1, n, 1, 4] = 1;
                    instance.a_mnsl[1, n, 1, 1] = 1;
                    instance.a_mnsl[1, n, 2, 5] = 1;
                    instance.a_mnsl[1, n, 2, 2] = 1;
                    instance.a_mnsl[1, n, 3, 6] = 1;
                    instance.a_mnsl[1, n, 3, 3] = 1;

                    instance.a_mnsl[2, n, 1, 10] = 1;
                    instance.a_mnsl[2, n, 1, 1] = 1;
                    instance.a_mnsl[2, n, 2, 11] = 1;
                    instance.a_mnsl[2, n, 2, 2] = 1;
                    instance.a_mnsl[2, n, 3, 12] = 1;
                    instance.a_mnsl[2, n, 3, 3] = 1;
                }

                return instance;
            }
        }

        public static SolverInstance Big
        {
            get
            {
                var instance = new SolverInstance(5, 5, 3, 18)
                {
                    C_l = new uint[]
                    {
                        0, 3, 4, 8, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000,
                        1000, 1000
                    },
                    b_n = new uint[] {0, 1, 1, 2, 1, 3},
                    B_s = new uint[] {0, 2, 3, 8},
                    k_l = new uint[] {0, 5, 5, 10, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
                    d_ns = new uint[,]
                    {
                        {0, 0, 0, 0},
                        {0, 10, 10, 15},
                        {0, 15, 15, 20},
                        {0, 25, 25, 30},
                        {0, 10, 15, 20},
                        {0, 30, 30, 35},
                    },
                    u_0 = 800,
                    u_m = 500,
                    y_mn_min = new[,]
                    {
                        {0, 0.2f, 0.3f, 0.1f, 0.3f, 0.4f},
                        {0, 0.2f, 0.3f, 0.3f, 0.2f, 0.3f},
                        {0, 0.4f, 0.1f, 0.4f, 0.2f, 0.4f},
                        {0, 0.3f, 0.1f, 0.4f, 0.1f, 0.3f},
                        {0, 0.1f, 0.3f, 0.5f, 0.1f, 0.3f},
                    },
                    y_mn_max = new[,]
                    {
                        {0, 0.8f, 0.9f, 0.7f, 0.9f, 0.6f},
                        {0, 0.7f, 0.9f, 0.9f, 0.8f, 0.7f},
                        {0, 0.8f, 0.6f, 0.9f, 0.8f, 0.9f},
                        {0, 0.8f, 0.9f, 0.7f, 0.9f, 0.8f},
                        {0, 0.7f, 0.8f, 0.7f, 0.9f, 0.9f},
                    },
                };


                foreach (var n in instance.N)
                {
                    instance.a_mnsl[0, n, 1, 10] = 1;
                    instance.a_mnsl[0, n, 1, 1] = 1;
                    instance.a_mnsl[0, n, 2, 11] = 1;
                    instance.a_mnsl[0, n, 2, 2] = 1;
                    instance.a_mnsl[0, n, 3, 12] = 1;
                    instance.a_mnsl[0, n, 3, 3] = 1;

                    instance.a_mnsl[1, n, 1, 4] = 1;
                    instance.a_mnsl[1, n, 1, 1] = 1;
                    instance.a_mnsl[1, n, 2, 5] = 1;
                    instance.a_mnsl[1, n, 2, 2] = 1;
                    instance.a_mnsl[1, n, 3, 6] = 1;
                    instance.a_mnsl[1, n, 3, 3] = 1;

                    instance.a_mnsl[2, n, 1, 7] = 1;
                    instance.a_mnsl[2, n, 1, 1] = 1;
                    instance.a_mnsl[2, n, 2, 8] = 1;
                    instance.a_mnsl[2, n, 2, 2] = 1;
                    instance.a_mnsl[2, n, 3, 9] = 1;
                    instance.a_mnsl[2, n, 3, 3] = 1;

                    instance.a_mnsl[3, n, 1, 13] = 1;
                    instance.a_mnsl[3, n, 1, 1] = 1;
                    instance.a_mnsl[3, n, 2, 14] = 1;
                    instance.a_mnsl[3, n, 2, 2] = 1;
                    instance.a_mnsl[3, n, 3, 15] = 1;
                    instance.a_mnsl[3, n, 3, 3] = 1;

                    instance.a_mnsl[4, n, 1, 16] = 1;
                    instance.a_mnsl[4, n, 1, 1] = 1;
                    instance.a_mnsl[4, n, 2, 17] = 1;
                    instance.a_mnsl[4, n, 2, 2] = 1;
                    instance.a_mnsl[4, n, 3, 18] = 1;
                    instance.a_mnsl[4, n, 3, 3] = 1;
                }

                return instance;
            }
        }
    }
}
