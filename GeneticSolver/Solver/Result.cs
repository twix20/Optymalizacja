using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public class Result
    {
        public static Random Random = new Random();

        public SolverInstance Instance { get; }
        public int[,,] X { get; }
        public float[,,] Y { get; }

        private static float GetRandomNumber(float minimum, float maximum)
        {
            return (float)(Random.NextDouble() * (maximum - minimum) + minimum);
        }

        public static Result CreateRandomInstance(SolverInstance instance)
        {
            var newX = new int[instance.MCount, instance.NCount + 1, instance.SCount + 1];
            var newY = new float[instance.MCount, instance.NCount + 1, instance.SCount + 1];

            var freeSpaceOnServersLookup = new Dictionary<int, uint>();
            foreach (var s in instance.S)
            {
                freeSpaceOnServersLookup.Add(s, instance.B_s[s]);
            }

            // For each object random server to upload on
            // only if the server has capacity left
            foreach (var n in instance.N)
            {
                while (true)
                {
                    var randomServer = Random.Next(1, instance.SCount + 1);

                    if (freeSpaceOnServersLookup[randomServer] >= instance.b_n[n])
                    {
                        freeSpaceOnServersLookup[randomServer] -= instance.b_n[n];

                        foreach (var m in instance.M)
                        {
                            var minY = instance.y_mn_min[m, n];
                            var maxY = instance.y_mn_max[m, n];

                            newX[m, n, randomServer] = 1;
                            newY[m, n, randomServer] = GetRandomNumber(minY, maxY);
                        }

                        break;
                    }
                }
            }

            var potentialResult = new Result(instance, newX, newY);
            potentialResult.CalculateFitness();

            return potentialResult;
        }


        public float Fitness { get; set; }
        public long ExecutionTime { get; set; }

        public Result(SolverInstance instance, int[,,] x, float[,,] y)
        {
            Instance = instance;
            X = x;
            Y = y;
        }

        public Result Crossover(Result parentB)
        {
            var newX = new int[Instance.MCount, Instance.NCount + 1, Instance.SCount + 1];
            var newY = new float[Instance.MCount, Instance.NCount + 1, Instance.SCount + 1];

            // Take half DNA from parentA and the rest from parentB
            // M/2 * N/2 * S/2   =  MNS / 8
            // cubeRootOfTwo ^ 3 = 1/2
            var cubeRootOfTwo = Math.Pow(2, 1.0 / 3.0);

            var halfM = Instance.MCount / cubeRootOfTwo;
            var halfN = Instance.NCount / cubeRootOfTwo;
            var halfS = Instance.SCount / cubeRootOfTwo;

            foreach (var m in Instance.M)
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        var moreThanHalf = m > halfM && n > halfN && s > halfS;
                        var newParent = moreThanHalf ? this : parentB;

                        newX[m, n, s] = newParent.X[m, n, s];
                        newY[m, n, s] = newParent.Y[m, n, s];
                    }
                }
            }

            return new Result(Instance, newX, newY);
        }

        private float y_hat(float[,,] y, int m, int n)
        {
            return Instance.S.Select(s => y[m, n, s]).Sum();
        }

        public void CalculateFitness()
        {
            float H(int[,,] x)
            {
                var sum = 0.0f;
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        sum += Instance.d_ns[n, s] * x[0, n, s];
                    }
                }

                return sum;
            }

            float U(float[,,] y)
            {
                var sum = 0.0f;
                foreach (var n in Instance.N)
                {
                    var leftSum = Instance.S.Select(s => Instance.u_0 * y[0, n, s]).Sum();
                    var rightSum = Instance.M.Skip(1).Select(m => Instance.u_m * y_hat(y, m, n)).Sum();

                    sum += leftSum + rightSum;
                }

                return sum;
            }


            float G(int[,,] x, float[,,] y)
            {
                var sum = 0.0f;
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        foreach (var l in Instance.L)
                        {
                            var mSum = Instance.M.Skip(1).Select(m => Instance.a_mnsl[m, n, s, l] * x[m, n, s] * y_hat(Y, m, n)).Sum();
                            
                            sum += Instance.k_l[l] * (Instance.a_mnsl[0,n,s,l] * x[0,n,s]*y[0,n,s] + mSum);
                        }
                    }
                }

                return sum;
            }

            var HResult = H(X);
            var UResult = U(Y);
            var GResult = G(X, Y);

            Fitness = UResult - GResult - HResult;
        }

        public bool IsValid()
        {
            var constraints = new Func<bool>[]
            {
                Cons1,
                Cons2,
                Cons3,
                Cons4,
                Cons5,
                Cons6
            };

            return constraints.All(c => c());
        }

        /// <summary>
        /// Każdy użytkownik musi być w stanie pobrać każdy obiekt z dokładnie jednej lokalizacji
        /// </summary>
        /// <returns></returns>
        private bool Cons1()
        {
            foreach (var m in Instance.M.Skip(1))
            {
                foreach (var n in Instance.N)
                {
                    var sum = Instance.S.Select(s => (int)X[m, n, s]).Sum();
                    if (sum != 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Musi istnieć kopia obiektu zlokalizowana na serwerze, do którego przypisany jest użytkownik
        /// </summary>
        /// <returns></returns>
        private bool Cons2()
        {
            foreach (var m in Instance.M.Skip(1))
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        var isTrue = X[m, n, s] <= X[0, n, s];
                        if (!isTrue)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Każdy serwer ma ograniczone zasoby dyskowe
        /// </summary>
        /// <returns></returns>
        private bool Cons3()
        {
            foreach (var s in Instance.S)
            {
                var sum = Instance.N.Select(n => X[0, n, s] * Instance.b_n[n]).Sum();

                var isTrue = sum <= Instance.B_s[s];
                if (!isTrue)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Ograniczenia łącza transmisyjnego nie mogą być przekroczone
        /// </summary>
        /// <returns></returns>
        /// 
        private bool Cons4()
        {
            foreach (var l in Instance.L)
            {
                var sum = 0.0f;

                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        var innerSum = Instance.M.Skip(1)
                            .Select(m => Instance.a_mnsl[m,n,s,l] * X[m,n,s] * y_hat(Y,m,n))
                            .Sum();

                        sum += Instance.a_mnsl[0, n, s, l] * X[0, n, s] * Y[0, n, s] + innerSum;
                    }
                }

                var isTrue = sum <= Instance.C_l[l];
                if (!isTrue)
                {
                    return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Wymagane minimalne prędkości muszą zostać zapewnione
        /// </summary>
        /// <returns></returns>
        private bool Cons5()
        {
            foreach (var m in Instance.M)
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        var isTrue = Y[m, n, s] >= X[m, n, s] * Instance.y_mn_min[m, n];
                        if (!isTrue)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Prędkości od użytkownika do serwera mogą być niezerowe tylko gdy użytkownik jest przypisany
        /// do tego serwera i maksymalne prędkości fizycznego łącza nie mogą zostać przekroczone
        /// </summary>
        /// <returns></returns>
        private bool Cons6()
        {
            foreach (var m in Instance.M)
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        var isTrue = Y[m, n, s] <= X[m, n, s] * Instance.y_mn_max[m, n];
                        if (!isTrue)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Pick random link transmission speed and randomly increase/decrease it
        /// If mutated result is not valid, Reject mutation 
        /// </summary>
        public void Mutate()
        {
            var usedTransitionLinks = GetUsedTransmissionLinks();

            var (m, n, s) = usedTransitionLinks.ElementAt(Random.Next(usedTransitionLinks.Count()));

            var maxSpeed = Instance.y_mn_max[m, n];
            var minSpeed = Instance.y_mn_min[m, n];
            var currentSpeed = Y[m, n, s];
            var oldValue = currentSpeed;

            if (Random.NextDouble() > 0.5)
            {
                var leftSpeed = maxSpeed - currentSpeed;
                Y[m, n, s] += leftSpeed * 0.1f;
            }
            else
            {
                var leftSpeed = currentSpeed - minSpeed;
                Y[m, n, s] -= leftSpeed * 0.1f;
            }
    

            if (!IsValid())
            {
                Y[m, n, s] = oldValue;
            }
        }
        List<(int, int, int)> GetUsedTransmissionLinks()
        {
            var valueTuples = new List<(int, int, int)>();
            foreach (var m in Instance.M)
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (Y[m, n, s] != 0.0f)
                        {
                            valueTuples.Add((m, n, s));
                        }
                    }
                }
            }

            return valueTuples;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Fitness: {Fitness} ExecutionTime: {ExecutionTime}ms");
            sb.AppendLine($"(m,n,s) -> (x,y)");

            foreach (var m in Instance.M)
            {
                foreach (var n in Instance.N)
                {
                    foreach (var s in Instance.S)
                    {
                        sb.AppendLine($"({m},{n},{s}) -> ({X[m, n, s]}, {Y[m, n, s]})");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
