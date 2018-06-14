using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Statistics
{
    public class DiscoveryLatencyTuple
    {
        public int Node1Latency { get; }
        public int Node2Latency { get; }

        public DiscoveryLatencyTuple(int node1Latency, int node2Latency)
        {
            Node1Latency = node1Latency;
            Node2Latency = node2Latency;
        }
    }
}
