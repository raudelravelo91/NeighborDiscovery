using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Statistics;

namespace NeighborDiscovery.Environment
{
    public class TwoNodesEnvironmentTmll
    {
        public DiscoverableDevice Node1 { get; }
        public DiscoverableDevice Node2 { get; }

        public TwoNodesEnvironmentTmll(DiscoverableDevice node1, DiscoverableDevice node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Tuple<int,int> RunTwoNodesSimulationSlow(int node2OffSet, int gotInRange, int maxLatencyExpected)
        {
            var curNode1State = node2OffSet + gotInRange;
            var curNode2State = gotInRange;
            var latencyToListenNode1 = -1;
            var latencyToListenNode2 = -1;
            var listendNode1 = false;
            var listendNode2 = false;
            var infinite = false;
            while (!infinite && (!listendNode1 || !listendNode2))
            {
                var latency = curNode2State - gotInRange + 1;
                if (!listendNode2 && Node1.IsListening(curNode1State) && Node2.IsTransmitting(curNode2State))
                {
                    listendNode2 = true;
                    latencyToListenNode1 = latency;
                }
                if (!listendNode1 && Node2.IsListening(curNode2State) && Node1.IsTransmitting(curNode1State))
                {
                    listendNode1 = true;
                    latencyToListenNode2 = latency;
                }
                curNode1State++;
                curNode2State++;
                if (latency > maxLatencyExpected)
                {
                    infinite = true;
                }
            }
            if(infinite)
                throw new Exception("Maximum Latency detected: " + latencyToListenNode1);

            return new Tuple<int, int>(latencyToListenNode1, latencyToListenNode2);
        }

        private int LatencyToListen(DiscoverableDevice nodeTrasmitter, DiscoverableDevice nodeListener, int gotInRange, int latencyLimit)
        {
            var curNodeTransmission = nodeTrasmitter.GetFirstTransmissionAfter(gotInRange).Slot;
            var latencyToListen = curNodeTransmission - gotInRange + 1;

            while (!nodeListener.IsListening(curNodeTransmission))
            {
                curNodeTransmission = nodeTrasmitter.NextTransmission().Slot;
                latencyToListen = curNodeTransmission - gotInRange + 1;
                if (latencyToListen > latencyLimit)
                {
                    break;
                }
            }
            
            return latencyToListen;
        }

        public Tuple<int, int> RunTwoNodesSimulation(DiscoverableDevice node1, DiscoverableDevice node2, int offSet, int gotInRange, int latencyLimit)
        {
            ResetNodes(node1, node2, offSet);
            var latency1 = LatencyToListen(node1, node2, gotInRange, latencyLimit);
            ResetNodes(node2, node1, offSet);
            var latency2 = LatencyToListen(node2, node1, gotInRange, latencyLimit);
            return new Tuple<int, int>(latency1, latency2);
        }

        private void ResetNodes(DiscoverableDevice node1, DiscoverableDevice node2, int offSet)
        {
            node1.Reset();
            node1.StartUpTime = 0;
            node2.Reset();
            node2.StartUpTime = offSet;
        }

        /// <summary>
        /// runs in paralell
        /// </summary>
        /// <returns></returns>
        public StatisticTestResult RunSimulation(int latencyLimit)
        {
            //Console.WriteLine("CONSOLE HERE!!");
            var offSetLimit = Node1.T / 2;
            var t1 = Math.Min(Node1.T, Node2.T);
            var latencies = new int[offSetLimit][];
            var size = t1 * 2;
            for (var  i = 0;  i < offSetLimit;  i++)
            {
                latencies[i] = new int[size];
            }

            //Parallel.For(0, offSetLimit,
            //(offSet) =>
            for (int offSet = 0; offSet < offSetLimit; offSet++)//node2 hyper period is offset slots relative to node1 (node2 always starts at the same time or after node1 but never before)
            {
                var node1 = Node1.Clone();
                var node2 = Node2.Clone();
                for (var gotInRange = 0; gotInRange < t1; gotInRange++)//after gotInRange slots they got in range and the latency starts from that point and on
                {
                    //var simulation2 = RunTwoNodesSimulationSlow(offSet, gotInRange, 100000);
                    var simulation1 = RunTwoNodesSimulation(node1, node2, offSet, offSet + gotInRange, latencyLimit);

                    //if (simulation1.Item1 != simulation2.Item1 || simulation1.Item2 != simulation2.Item2)
                    //{//checking up
                    //    Console.WriteLine("Fast: " + simulation1 + " Normal: " + simulation2 + " OffSet: " + offSet + " Range: " + gotInRange);
                    //    int theBug = 0;
                    //}
                    latencies[offSet][2*gotInRange] = simulation1.Item1;
                    latencies[offSet][2*gotInRange + 1] = simulation1.Item2;
                }
            }
            //);

            var result = new StatisticTestResult(offSetLimit * t1 * 2);
            for (var i = 0; i < latencies.Length; i++)
                for (var j = 0; j < latencies[0].Length; j++)
                    result.AddDiscovery(latencies[i][j]);
            return result;
        }
    }
}
