﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Statistics
{
    public class StatisticsResult
    {
        public NodeType NodeType { get; private set; }
        private double[] averageFractionOfDiscovery;
        private List<StatisticTestResult> tests;
        private double avgContactByWakeUp;
        private double avgDiscoveryLatency;
        public double AverageContactByWakesUp { get { if (avgContactByWakeUp == 0)
                {
                    avgContactByWakeUp = Math.Round(tests.Average(t => t.GetContactsByWakeUpRatio()), 2);
                }
                return avgContactByWakeUp;
            } }
        public double AverageDiscoveryLatency {
            get {
                if (avgDiscoveryLatency == 0)
                {
                    avgDiscoveryLatency = Math.Round(tests.Average(t => t.AverageDiscoveryLatency));
                }
                return avgDiscoveryLatency;
            }
        }

        public StatisticsResult(NodeType nodeType)
        {
            tests = new List<StatisticTestResult>();
            NodeType = nodeType;
        }

        public void AddStatisticTest(StatisticTestResult test)
        {
            tests.Add(test);
        }

        public void BuildAverageFractionOfDiscovey()
        {
            int highestLatency = tests.Max(test => test.MaxLatency);
            averageFractionOfDiscovery = new double[highestLatency +1];

            foreach (var testResult in tests)
            {
                foreach (var pair in testResult.GetCummulativeFractionOfDiscovery(highestLatency))
                {
                    averageFractionOfDiscovery[pair.Key] += pair.Value;
                }
            }

            for (int i = 0; i < averageFractionOfDiscovery.Length; i++)
            {
                averageFractionOfDiscovery[i] /= tests.Count;
            }
        }

        public double GetAverageFractionOfDiscoveryAtLatency(double latency)
        {
            int index = (int)latency;
            if (index > averageFractionOfDiscovery.Length)//above maxLatency is always 100%
                return averageFractionOfDiscovery[averageFractionOfDiscovery.Length - 1];
            return averageFractionOfDiscovery[index];
        }

        public int GetMaxLatency()
        {
            return averageFractionOfDiscovery.Length - 1;
        }

    }
}