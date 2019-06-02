using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public class Population
    {
        public static Random Random = new Random();

        public int Size { get; }
        public SolverInstance Instance { get; }
        public Result[] Results { get; private set; }
        public ICollection<Result> MattingPool { get; private set; }

        public Result TheBestResult => Results
            .Select(r => new {result = r, fitness = r.Fitness})
            .OrderByDescending(vi => vi.fitness)
            .First().result; 

        public Population(int size, SolverInstance instance)
        {
            Size = size;
            Instance = instance;
            Results = new Result[size];
        }

        public void Initialize()
        {
            for (int i = 0; i < Size; i++)
            {
                Results[i] = Result.CreateRandomInstance(Instance);
            }
        }

        public void Evaluate()
        {
            var maxFit = 0.0f;
            for (var i = 0; i < Size; i++)
            {
                Results[i].CalculateFitness();
                if (Results[i].Fitness > maxFit)
                {
                    maxFit = Results[i].Fitness;
                }
            }

            // Normalises fitnesses
            for (var i = 0; i < Size; i++)
            {
                Results[i].Fitness /= maxFit;
            }

            MattingPool = new List<Result>();
            for (var i = 0; i < Size; i++)
            {
                var result = Results[i];

                var n = result.Fitness * 100; // Reward higher fitness
                for (var j = 0; j < n; j++)
                {
                    MattingPool.Add(result);
                }
            }


        }

        public void Selection()
        {
            var newResults = new Result[Size];

            for (int i = 0; i < Results.Length; i++)
            {
                var randomAIndex = Random.Next(0, MattingPool.Count);
                var randomBIndex = Random.Next(0, MattingPool.Count);

                var parentA = MattingPool.ElementAt(randomAIndex);
                var parentB = MattingPool.ElementAt(randomBIndex);

                var child = parentA.Crossover(parentB);

                if (!child.IsValid())
                {
                    i--;
                }
                else
                {
                    child.CalculateFitness();
                    newResults[i] = child;
                }
            }

            this.Results = newResults;
        }

        public void Mutate()
        {
            foreach (var result in Results)
            {
                if (Random.NextDouble() > 0.1)
                {
                    result.Mutate();
                    result.CalculateFitness();
                }
            }
        }
    }
}
