using System;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public class PairwiseEnvironmentTmll
    {

        public StatisticTestResult RunPairwiseSimulation(BoundedProtocol node1, BoundedProtocol node2, int latencyLimit)
        {
            var statistics = new StatisticTestResult();
            var protocolController = new ProtocolController();

            for (int node1State = 0; node1State < node1.T; node1State++)
            {
                for (var node2State = 0; node2State < node2.T; node2State++)
                {
                    protocolController.SetToState(node1, node1State);
                    protocolController.SetToState(node2, node2State);
                    var simulation =
                        RunSimulation(node1, node2, latencyLimit);
                    statistics.AddDiscovery(Math.Abs(simulation.Node1Latency - simulation.Node2Latency));
                    //statistics.AddDiscovery(Math.Min(simulation.Node1Latency, simulation.Node2Latency));

                    //if (Math.Abs(simulation.Node1Latency - simulation.Node2Latency) == 200)
                    //{
                    //    Console.WriteLine("here");
                    //}

                    //statistics.AddDiscovery(simulation.Node1Latency);
                    //statistics.AddDiscovery(simulation.Node2Latency);
                }
            }

            return statistics;
        }

        private DiscoveryLatencyTuple RunSimulation(BoundedProtocol node1, BoundedProtocol node2, int latencyLimit)
        {
            int latency1 = -1;
            int latency2 = -1;
            int slotCnt = 1;

            while (latency1 < 0 || latency2 < 0)
            {
                if (latency1 < 0 && node1.IsListening() && node2.IsTransmitting())
                {
                    latency1 = slotCnt;
                    node1.ListenTo(node2.GetTransmission());
                }

                if (latency2 < 0 && node2.IsListening() && node1.IsTransmitting())
                {
                    latency2 = slotCnt;
                    node2.ListenTo(node1.GetTransmission());
                }

                node1.MoveNext();
                node2.MoveNext();

                if (slotCnt > latencyLimit)
                    throw new Exception("Latency limit reached. Either there is a flaw in the protocol or the latency limit was set to short.");
                slotCnt++;
            }


            return new DiscoveryLatencyTuple(latency1, latency2);
        }

        
    }
}
