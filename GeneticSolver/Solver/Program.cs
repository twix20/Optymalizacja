using System;
using System.Linq;
using System.Threading.Tasks;

namespace Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please enter iterations and populationSize, eg: .\\Solver.exe 500 200");
                return;
            }

            var iterations = int.Parse(args[0]);
            var populationSize = int.Parse(args[1]);

            //var instance = SolverInstance.Small;
            //var instance = SolverInstance.Medium;
            var instance = SolverInstance.Big;

            Result bestResult = null;

            var tasks = Enumerable.Range(0, 1).Select(i => { return Task.Run(() =>
            {
                var solver = new CanSolver(instance);
                var result = solver.Solve(iterations, populationSize);

                if (bestResult == null || result.Fitness > bestResult.Fitness)
                {
                    bestResult = solver.DeepCloneResult(result);
                    Console.WriteLine(bestResult.Fitness);
                }
            }); });

            Task.WhenAll(tasks).Wait();

            Console.WriteLine(bestResult.ToString());
            Console.ReadKey();
        }
    }
}
