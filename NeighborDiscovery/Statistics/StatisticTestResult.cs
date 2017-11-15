﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Statistics
{
    public class StatisticTestResult
    {
        public int ExpectedDiscoveries { get; private set; }
        public int MaxLatency { get; private set; }
        public int NumberOfWakesUp { get; set; }
        public int NumberOfContactMade { get; set; }
        public double AverageDiscoveryLatency { get { return 1.0 * sumOfLatency / TotalDiscoveries; }  }
        public int TotalDiscoveries { get; private set; }
        private Dictionary<int, int> discoveryByLatency { get; set; }
        private long sumOfLatency;


        public StatisticTestResult(int expectedDiscoveries)
        {
            //NumberOfNodes = networkSize;
            ExpectedDiscoveries = expectedDiscoveries;
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
                int max = discoveryByLatency.Max(x => x.Key);
                int latency = 0;
                double lastFractionValue = 0;
                int cumul = 0;
                var values = discoveryByLatency.OrderBy(x => x.Key).ToArray();
                foreach (var pair in values)
                {
                    while (latency < pair.Key)
                    {
                        yield return new KeyValuePair<int, double>(latency, lastFractionValue);
                        latency++;
                    }
                    cumul += pair.Value;
                    lastFractionValue = 1.0 * cumul / ExpectedDiscoveries;
                    yield return new KeyValuePair<int, double>(latency, lastFractionValue);
                    latency++;
                }
                while (latency <= latencyLimit)
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
        

        public bool IsDone { get { return TotalDiscoveries == ExpectedDiscoveries; } }
        
    }

    
}