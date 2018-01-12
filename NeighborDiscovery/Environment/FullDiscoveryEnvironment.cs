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
            foreach(var receiver in network.NeighborsOf(transmission.Sender.Id))
            {
                var sender = transmission.Sender;
                var gotInRange = network.GotInRangeWith(sender.Id, receiver.Id);

                List<IDiscovery> discoveries;//when 
                if (transmission.Slot < gotInRange || !receiver.ListenTo(transmission, out discoveries))
                    continue;
                //add statistics about listended transmissions here (if you want :)
                foreach (var node in discoveries)
                {
                    gotInRange = network.GotInRangeWith(node.Id, receiver.Id);
                    var discoveryLatency = transmission.Slot - gotInRange + 1;
                    statisticTest.AddDiscovery(discoveryLatency);
                }
            }
        }

        public StatisticTestResult RunSingleSimulation(Network2D network, int tranckNodeId = -1)
        {
            var discoveriesExpected = (tranckNodeId < 0) ? network.NumberOfLinks : network.DegreeOf(tranckNodeId) * 2;
            var statisticTest = new StatisticTestResult(discoveriesExpected);
            var minQ = new SimplePriorityQueue<Transmission>();
            for (var i = 0; i < network.NetworkSize; i++)
            {
                var firstTransm = network.GetDevice(i).NextTransmission();
                minQ.Enqueue(firstTransm, firstTransm.Slot);
            }
            var nextTransmissionRound = new List<Transmission>();

            while (statisticTest.TotalDiscoveries < discoveriesExpected)
            {
                if (minQ.Count != network.NetworkSize)
                    throw new Exception("The number of transmission in the priority queue does not match the number of nodes in the network.");
                var timeSlot = minQ.First.Slot;
                var cnt = 0;
                nextTransmissionRound.Clear();//setting up the next transmission round
                while (minQ.Count > 0 && timeSlot == minQ.First.Slot)
                {
                    cnt++;
                    var transmission = minQ.Dequeue();
                    nextTransmissionRound.Add(transmission);
                }

                //transmiting and updating values
                foreach (var transmission in nextTransmissionRound)
                {
                    TransmitToNeighbors(transmission, network, statisticTest, tranckNodeId);
                    statisticTest.NumberOfWakesUp++;
                }

                //add to the minQ the nextTransmission round for those nodes that have transmitted in this round
                foreach (var t in nextTransmissionRound)
                {
                    var nextTransm = t.Sender.NextTransmission();
                    minQ.Enqueue(nextTransm, nextTransm.Slot);
                }
            }
            if (statisticTest.TotalDiscoveries > statisticTest.ExpectedDiscoveries)
            {
                Console.WriteLine(statisticTest.MaxLatency);
                throw new Exception("total dicoveries greater than expected discoveries");
            }
            return statisticTest;
        }
    }
}
