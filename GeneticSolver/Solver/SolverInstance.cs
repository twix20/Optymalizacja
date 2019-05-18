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
            a_mnsl = new int[m, n + 1, s + 1, l + 1];
        }
    }
}
