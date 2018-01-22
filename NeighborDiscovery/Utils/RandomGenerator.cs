using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Utils
{
    public class RandomGenerator
    {
        private static Random Random = new Random();

        public static IEnumerable<double> GetRandomDoubleValues(double max)
        {
            while (true)
            {
                yield return Random.NextDouble()*max;
            }
        }

        public static IEnumerable<double> GetRandomIntValues(int max)
        {
            while (true)
            {
                yield return Random.Next(max);
            }
        }

        public static IEnumerable<MyPair> GetRandomPairs(double maxX, double maxY)
        {
            while (true)
            {
                yield return new MyPair(Random.NextDouble()*maxX, Random.NextDouble()*maxY);
            }
        }

        public static int GetRandomInteger(int limit)
        {
            return Random.Next(limit);
        }

        public static double GetRandomDouble()
        {
            return Random.NextDouble();
        }
    }
}
