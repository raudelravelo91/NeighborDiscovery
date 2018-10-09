using System;
using System.Threading;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public class PairwiseEnvironmentTmll
    {

        public async Task<StatisticTestResult> RunPairwiseSimulation(BoundedProtocol node1, BoundedProtocol node2, int latencyLimit,
            CancellationToken cancellationToken, IProgress<int> progress)
        {
            var statistics = new StatisticTestResult();

            await Task.Run(() =>
            {
                //Parallel.For(0, node1.T,
                //    new ParallelOptions() {MaxDegreeOfParallelism = System.Environment.ProcessorCount}, node1State =>

                for (int node1State = 0; node1State < node1.T; node1State++)
                {
                    for (var node2State = 0; node2State < node2.T; node2State++)
                    {
                        ProtocolController.SetToState(node1, node1State);
                        ProtocolController.SetToState(node2, node2State);
                        var simulation =
                            RunSimulation(node1, node2, latencyLimit);
                        statistics.AddDiscovery(simulation.Node1Latency);
                        statistics.AddDiscovery(simulation.Node2Latency);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            //Thread.Sleep(2000);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                    progress.Report((node1State+1)*100/node1.T);
                }
            });

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
