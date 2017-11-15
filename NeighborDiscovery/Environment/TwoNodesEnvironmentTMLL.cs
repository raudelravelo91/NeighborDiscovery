using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Statistics;

namespace NeighborDiscovery.Environment
{
    public class TwoNodesEnvironmentTMLL
    {
        public IDiscovery Node1 { get; private set; }
        public IDiscovery Node2 { get; private set; }

        public TwoNodesEnvironmentTMLL(IDiscovery node1, IDiscovery node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Tuple<int,int> RunTwoNodesSimulationSlow(int node2OffSet, int gotInRange, int maxLatencyExpected)
        {
            int curNode1State = node2OffSet + gotInRange;
            int curNode2State = gotInRange;
            int latencyToListenNode1 = -1;
            int latencyToListenNode2 = -1;
            bool listendNode1 = false;
            bool listendNode2 = false;
            bool infinite = false;
            while (!infinite && (!listendNode1 || !listendNode2))
            {
                int latency = curNode2State - gotInRange + 1;
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

        public Tuple<int, int> RunTwoNodesSimulation(IDiscovery node1, IDiscovery node2, int gotInRange, int maxLatencyExpected)
        {
            //if(offSet == 1 && gotInRange == 7)
            //    Console.WriteLine("debugging");
            int curNode1State = node1.FirstTransmissionAfter(gotInRange).Slot;
            int curNode2State = node2.FirstTransmissionAfter(gotInRange).Slot;
            int latencyToListenNode1 = -1;
            int latencyToListenNode2 = -1;

            while (!Node1.IsListening(curNode2State))
            {
                curNode2State = node2.NextTransmission().Slot;
            }
            latencyToListenNode1 = curNode2State - gotInRange + 1;

            while (!Node2.IsListening(curNode1State))
            {
                curNode1State = node1.NextTransmission().Slot;
            }
            latencyToListenNode2 = curNode1State - gotInRange + 1;

            if (latencyToListenNode1 > maxLatencyExpected || latencyToListenNode2 > maxLatencyExpected)
            {
                throw new Exception("Maximum Latency detected: " + latencyToListenNode1);
                //return new Tuple<int, int>(-1, -1);
            }

            return new Tuple<int, int>(latencyToListenNode1, latencyToListenNode2);
        }

       

        /// <summary>
        /// runs in paralell
        /// </summary>
        /// <returns></returns>
        public StatisticTestResult RunSimulation()
        {
            //Console.WriteLine("CONSOLE HERE!!");
            int offSetLimit = Node1.T / 2;
            int t1 = Math.Min(Node1.T, Node2.T);
            int[][] latencies = new int[offSetLimit][];
            int size = t1 * 2;
            for (int  i = 0;  i < offSetLimit;  i++)
            {
                latencies[i] = new int[size];
            }

            Parallel.For(0, offSetLimit,
            (offSet) =>
            //for (int offSet = 0; offSet < offSetLimit; offSet++)//node2 hyper period is offset slots relative to node1 (node2 always starts at the same time or after node1 but never before)
            {
                var node1 = Node1.Clone();
                var node2 = Node2.Clone();
                for (int gotInRange = 0; gotInRange < t1; gotInRange++)//after gotInRange slots they got in range and the latency starts from that point and on
                {
                    node1.Reset(0);
                    node2.Reset(offSet);
                    //var simulation1 = RunTwoNodesSimulationSlow(offSet, gotInRange, 100000);
                    var simulation1 = RunTwoNodesSimulation(node1, node2, offSet + gotInRange, 100000);
                    
                    //if (simulation1.Item1 != simulation2.Item1 || simulation1.Item2 != simulation2.Item2)
                    //{//checking up
                    //    Console.WriteLine("Fast: " + simulation2 + " Normal: " + simulation1 + " OffSet: " + offSet + " Range: " + gotInRange);
                    //    int p = 0;
                    //}
                    latencies[offSet][2*gotInRange] = simulation1.Item1;
                    latencies[offSet][2*gotInRange + 1] = simulation1.Item2;
                }
            }
            );

            StatisticTestResult result = new StatisticTestResult(offSetLimit * t1 * 2);
            for (int i = 0; i < latencies.Length; i++)
                for (int j = 0; j < latencies[0].Length; j++)
                    result.AddDiscovery(latencies[i][j]);
            return result;
        }
    }
}
