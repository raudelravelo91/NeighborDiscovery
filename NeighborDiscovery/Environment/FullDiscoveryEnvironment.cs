using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Statistics;
using Priority_Queue;
using NeighborDiscovery.Nodes;

namespace NeighborDiscovery.Environment
{
    public class FullDiscoveryEnvironment
    {
        private void TransmitToNeighbors(Transmission transmission, Network2D network, StatisticTestResult statisticTest, int trackNodeId)
        {
            int listLen = network.NeighborsOf(transmission.Sender.ID).Count;
            
            for (int neighborIndex = 0; neighborIndex < listLen; neighborIndex++)
            {
                var sender = transmission.Sender;
                var receiver = network.NeighborsOf(transmission.Sender.ID)[neighborIndex];
                int gotInRange = network.GotInRangeWith(sender.ID, receiver.ID);
                
                var discoveries = new List<IDiscovery>();
                if (transmission.Slot >= gotInRange && receiver.ListenTo(transmission, out discoveries))
                {
                    //add statistics about listended transmissions (if you want :)

                    foreach (var node in discoveries)
                    {
                        gotInRange = network.GotInRangeWith(node.ID, receiver.ID);
                        int discoveryLatency = transmission.Slot - gotInRange + 1;
                        if (discoveryLatency > 500)
                        {
                            int debug = 0;
                        }
                        statisticTest.AddDiscovery(discoveryLatency);
                    }
                }
            }
        }

        public StatisticTestResult RunSingleSimulation(Network2D network, int tranckNodeId = -1)
        {
            int discoveriesExpected = (tranckNodeId < 0) ? network.NumberOfLinks : network.DegreeOf(tranckNodeId) * 2;
            StatisticTestResult statisticTest = new StatisticTestResult(discoveriesExpected);
            var minQ = new SimplePriorityQueue<Transmission>();
            for (int i = 0; i < network.NetworkSize; i++)
            {
                var firstTransm = network.GetDevice(i).NextTransmission();
                minQ.Enqueue(firstTransm, firstTransm.Slot);
            }
            int timeSlot = 0;
            var nextTransmissionRound = new List<Transmission>();

            while (statisticTest.TotalDiscoveries < discoveriesExpected)
            {
                if (minQ.Count != network.NetworkSize)
                    throw new Exception("The number of transmission in the priority queue does not match the number of nodes in the network.");
                timeSlot = minQ.First.Slot;
                int cnt = 0;
                nextTransmissionRound.Clear();//setting up the next transmission round
                while (minQ.Count > 0 && timeSlot == minQ.First.Slot)
                {
                    cnt++;
                    var transmission = minQ.Dequeue();
                    nextTransmissionRound.Add(transmission);
                }

                //transmiting and updating values
                for (int i = 0; i < nextTransmissionRound.Count; i++)
                {
                    var transmission = nextTransmissionRound[i];
                    TransmitToNeighbors(transmission, network, statisticTest, tranckNodeId);
                    statisticTest.NumberOfWakesUp++;
                }

                //add to the minQ the nextTransmission round for those nodes that have transmitted in this round
                for (int i = 0; i < nextTransmissionRound.Count; i++)
                {
                    var nextTransm = nextTransmissionRound[i].Sender.NextTransmission();
                    minQ.Enqueue(nextTransm, nextTransm.Slot);
                }
            }
            if (statisticTest.TotalDiscoveries > statisticTest.ExpectedDiscoveries)
            {
                Console.WriteLine(statisticTest.MaxLatency);
            }
            return statisticTest;
        }
    }
}
