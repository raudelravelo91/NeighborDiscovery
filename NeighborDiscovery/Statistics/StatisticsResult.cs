using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Statistics
{
    public class StatisticsResult
    {
        public NodeType NodeType { get; }
        private double[] _averageFractionOfDiscovery;
        private readonly List<StatisticTestResult> _tests;
        //private double _avgContactByWakeUp;
        private double _avgDiscoveryLatency;
        public double AverageDiscoveryLatency {
            get {
                if (_avgDiscoveryLatency == 0)
                {
                    _avgDiscoveryLatency = Math.Round(_tests.Average(t => t.AverageDiscoveryLatency));
                }
                return _avgDiscoveryLatency;
            }
        }
        public double AvgNoNeighbors => _tests.Average(t => t.AvgNoNeighbors);
        public double AvgNoNeihborsPerSlot => _tests.Average(t => t.AvgNoNeighborsPerSlot);
        public double AvgTransmissionsSentPerPeriod => _tests.Average(t => t.AvgTransmissionsPerPeriod);

        public StatisticsResult(NodeType nodeType)
        {
            _tests = new List<StatisticTestResult>();
            NodeType = nodeType;
        }

        public void AddStatisticTest(StatisticTestResult test)
        {
            _tests.Add(test);
        }

        public void BuildAverageFractionOfDiscovey(int maxLatency)
        {
            //var highestLatency = tests.Max(test => test.MaxLatency);
            _averageFractionOfDiscovery = new double[maxLatency + 1];

            foreach (var testResult in _tests)
            {
                foreach (var pair in testResult.GetCummulativeFractionOfDiscovery(maxLatency))
                {
                    var index = (pair.Key < maxLatency)?pair.Key:maxLatency;
                    _averageFractionOfDiscovery[index] += pair.Value;
                }
            }

            for (var i = 0; i < _averageFractionOfDiscovery.Length; i++)
            {
                _averageFractionOfDiscovery[i] /= _tests.Count;
            }
        }

        public double GetAverageFractionOfDiscoveryAtLatency(double latency)
        {
            var index = (int)latency;
            if (index > _averageFractionOfDiscovery.Length)//above maxLatency is always 100%
                return _averageFractionOfDiscovery[_averageFractionOfDiscovery.Length - 1];
            return _averageFractionOfDiscovery[index];
        }

        public int GetMaxLatency()
        {
            return _averageFractionOfDiscovery.Length - 1;
        }

        
    }
}
