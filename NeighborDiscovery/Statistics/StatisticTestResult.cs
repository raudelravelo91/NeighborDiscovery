using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Statistics
{
    public class StatisticTestResult
    {
        public int MaxLatency { get; private set; }
        public int NumberOfWakesUp { get; set; }
        public int NumberOfContactMade { get; set; }
        public double AverageDiscoveryLatency => 1.0 * sumOfLatency / TotalDiscoveries;
        public int TotalDiscoveries { get; private set; }
        private Dictionary<int, int> discoveryByLatency { get; set; }
        private long sumOfLatency;


        public StatisticTestResult(int expectedDiscoveries)
        {
            //NumberOfNodes = networkSize;
            discoveryByLatency = new Dictionary<int, int>();
            TotalDiscoveries = 0;
        }

        public void AddDiscovery(int latency)
        {
            if (discoveryByLatency.ContainsKey(latency))
            {
                discoveryByLatency[latency]++;
            }
            else discoveryByLatency[latency] = 1;
            TotalDiscoveries++;
            MaxLatency = Math.Max(MaxLatency, latency);
            sumOfLatency += latency;
        }

        public IEnumerable<KeyValuePair<int, double>> GetCummulativeFractionOfDiscovery(int latencyLimit = -1)
        {
            if (discoveryByLatency.Count == 0)
                yield return new KeyValuePair<int, double>(0, 1);
            else
            {
                var max = discoveryByLatency.Max(x => x.Key);
                var latency = 1;
                double lastFractionValue = 0;
                var cumul = 0;
                var values = discoveryByLatency.OrderBy(x => x.Key).ToArray();
                foreach (var pair in values)
                {
                    while (latency < pair.Key)
                    {
                        yield return new KeyValuePair<int, double>(latency, lastFractionValue);
                        latency++;
                    }
                    cumul += pair.Value;
                    lastFractionValue = 1.0 * cumul / TotalDiscoveries;
                    yield return new KeyValuePair<int, double>(latency, lastFractionValue);
                    latency++;
                }
                while (latency < latencyLimit)
                {
                    yield return new KeyValuePair<int, double>(latency, 1.0);
                    latency++;
                }
            }
        }

        public double GetContactsByWakeUpRatio()
        {
            return 1.0 * NumberOfContactMade / NumberOfWakesUp;
        }
    }
}
