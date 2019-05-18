using System;

namespace Solver
{
    public class CanSolver
    {
        public SolverInstance Instance { get; }

        private Result CurrentBestResult { get; set; }

        public CanSolver(SolverInstance instance)
        {
            Instance = instance;
        }

        public Result Solve(int iterations, int populationSize)
        {
            var population = new Population(populationSize, Instance);
            population.Initialize();

            CurrentBestResult = DeepCloneResult(population.TheBestResult);
            Console.WriteLine(CurrentBestResult.Fitness);

            for (var i = 0; i < iterations; i++)
            {
                population.Evaluate();
                population.Selection();
                population.Mutate();

                var populationBest = population.TheBestResult;

                if (populationBest.Fitness < CurrentBestResult.Fitness)
                {
                    CurrentBestResult = DeepCloneResult(population.TheBestResult);
                    Console.WriteLine(CurrentBestResult.Fitness);
                }

            }

            return CurrentBestResult;
        }

        private Result DeepCloneResult(Result toClone)
        {
            var deepX = toClone.X.DeepClone();
            var deepY = toClone.Y.DeepClone();

            var r = new Result(toClone.Instance, deepX, deepY)
            {
                Fitness = toClone.Fitness
            };

            return r;
        }
    }
}
