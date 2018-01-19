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
        public IDiscoveryProtocol Node1 { get; }
        public IDiscoveryProtocol Node2 { get; }

        public TwoNodesEnvironmentTmll(IDiscoveryProtocol node1, IDiscoveryProtocol node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        private Tuple<int, int> RunTwoNodesSimulation(int node1State, int node2State, int latencyLimit)
        {
            Node1.Reset();
            Node2.Reset();
            Node1.MoveNext(node1State);
            Node2.MoveNext(node2State);
            int latency1 = -1;
            int latency2 = -1;
            int slotCnt = 1;
            if (node2State == 10)
                Console.WriteLine("debug");
            while (latency1 < 0 || latency2 < 0)
            {
                if (latency1 < 0 && Node1.IsListening() && Node2.IsTransmitting())
                    latency1 = slotCnt;
                if (latency2 < 0 && Node2.IsListening() && Node1.IsTransmitting())
                    latency2 = slotCnt;
                
                Node1.MoveNext();
                Node2.MoveNext();
                
                if(slotCnt > latencyLimit)
                    throw new Exception("Latency limit reached. Either there is a flaw in the protocol or the latency limit was set to short.");
                slotCnt++;
            }
            

            return new Tuple<int,int>(latency1, latency2);
        }

        
        public StatisticTestResult RunSimulation(int latencyLimit)
        {
            //Console.WriteLine("CONSOLE HERE!!");
            var latencies = new int[latencyLimit][];
            
            for (var i = 0; i < latencies.Length; i++)
                latencies[i] = new int[latencyLimit * 2];

            //Parallel.For(0, halfPeriod,
            //(node1State) =>
            for (int node1State = 0; node1State < latencies.Length; node1State++)//node2 hyper period is offset slots relative to node1 (node2 always starts at the same time or after node1 but never before)
            {
                for (var node2State = 0; node2State < latencyLimit; node2State++)//after gotInRange slots they got in range and the latency starts from that point and on
                {
                    var simulation1 = RunTwoNodesSimulation(node1State, node2State, latencyLimit);
                    latencies[node1State][node2State*2] = simulation1.Item1;
                    latencies[node1State][node2State*2 + 1] = simulation1.Item2;
                }
            }
            //);

            var expectedDiscoveries = latencies.Length * latencies[0].Length;
            var result = new StatisticTestResult(expectedDiscoveries);
            for (var i = 0; i < latencies.Length; i++)
                for (var j = 0; j < latencies[0].Length; j++)
                    result.AddDiscovery(latencies[i][j]);
            return result;
        }
    }
}
