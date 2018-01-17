using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NeighborDiscovery.Nodes;

namespace NeighborDiscovery.Utils
{
    public class RandomGenerator
    {
        public Random Random { get; }

        public RandomGenerator()
        {
            Random = new Random();
        }

        public IEnumerable<double> GetRandomValues(double max)
        {
            while (true)
            {
                yield return Random.NextDouble()*max;
            }
        }


        public IEnumerable<MyPair> GetRandomPairs(double maxX, double maxY)
        {
            while (true)
            {
                yield return new MyPair(Random.NextDouble()*maxX, Random.NextDouble()*maxY);
            }
        }
    }
}
