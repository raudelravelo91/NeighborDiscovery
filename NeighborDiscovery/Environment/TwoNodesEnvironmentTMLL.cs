using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;

namespace NeighborDiscovery.Environment
{
    public class TwoNodesEnvironmentTmll
    {
        public BoundedProtocol Node1 { get; }
        public BoundedProtocol Node2 { get; }

        public TwoNodesEnvironmentTmll(BoundedProtocol node1, BoundedProtocol node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Tuple<int, int> RunTwoNodesSimulation(IDiscoveryProtocol node1, IDiscoveryProtocol node2, int node1State, int node2State, int latencyLimit)
        {
            node1.MoveNext(node1State);
            node2.MoveNext(node2State);
            int latency1 = -1;
            int latency2 = -1;
            int slotCnt = 0;
            while (latency1 < 0 || latency2 < 0)
            {
                if (node1.IsListening() && node2.IsTransmitting() && latency1 < 0)
                    latency1 = slotCnt;
                if (node2.IsListening() && node1.IsTransmitting() && latency2 < 0)
                    latency2 = slotCnt;
                slotCnt++;
                if(slotCnt == latencyLimit)
                    throw new Exception("Latency limit reached. Either there is a flaw in the protocol or the latency limit was set to short.");
            }

            return new Tuple<int,int>(latency1, latency2);
        }

        
        public StatisticTestResult RunSimulation(int latencyLimit)
        {
            //Console.WriteLine("CONSOLE HERE!!");
            var halfPeriod = Node1.T / 2;
            var t1 = Math.Min(Node1.T, Node2.T);
            var latencies = new int[halfPeriod][];
            var size = t1 * 2;
            for (var i = 0; i < halfPeriod; i++)
            {
                latencies[i] = new int[size];
            }

            //Parallel.For(0, halfPeriod,
            //(node1State) =>
            for (int node1State = 0; node1State < halfPeriod; node1State++)//node2 hyper period is offset slots relative to node1 (node2 always starts at the same time or after node1 but never before)
            {
                var node1 = new Disco(Node1.Id, Node1.GetDutyCycle());
                var node2 = new Disco(Node2.Id, Node2.GetDutyCycle());
                for (var node2State = 0; node2State < t1; node2State++)//after gotInRange slots they got in range and the latency starts from that point and on
                {
                    var simulation1 = RunTwoNodesSimulation(node1, node2, node1State, node2State, latencyLimit);
                    latencies[node1State][2 * node2State] = simulation1.Item1;
                    latencies[node1State][2 * node2State + 1] = simulation1.Item2;
                }
            }
            //);

            var result = new StatisticTestResult(halfPeriod * t1 * 2);
            for (var i = 0; i < latencies.Length; i++)
                for (var j = 0; j < latencies[0].Length; j++)
                    result.AddDiscovery(latencies[i][j]);
            return result;
        }
    }
}
