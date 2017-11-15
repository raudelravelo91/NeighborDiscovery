using NeighborDiscovery.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Nodes
{
    public class AccGossipPNihao : PNihao
    {
        public AccGossipPNihao(int id, int duty, int communicationRange, int channelOccupancyRate, int startUpTime, bool randomInitialState = false) : base(id, duty, communicationRange, channelOccupancyRate, startUpTime, randomInitialState)
        {

        }

        public override bool ListenTo(Transmission transmission, out List<IDiscovery> discoveredNodes)
        {
            discoveredNodes = new List<IDiscovery>();
            if (transmission == null)
                throw new Exception("Null transmission received.");
            if (IsListening(transmission.Slot))
            {
                if (!WasDiscovered(transmission.Sender))
                {
                    discoveredNodes.Add(transmission.Sender);
                    neighbors[transmission.Sender] = new ContactInfo(transmission.Slot);
                }
                else
                {
                    //update
                    if (transmission.Slot > neighbors[transmission.Sender].LastMet)
                    {
                        neighbors[transmission.Sender].Update(transmission.Slot);
                    }
                }
                //gossip
                foreach (var twoHopTuple in transmission.Sender.NeighborsDiscovered())
                {
                    IDiscovery twoHopNeighbor = twoHopTuple.Item1;//two hop neighbor
                    int lastMet = twoHopTuple.Item2.LastMet;
                    int firstDiscovered = twoHopTuple.Item2.FirstDiscovered;
                    if (!WasDiscovered(twoHopNeighbor) && firstDiscovered < transmission.Slot)//I havn't discovered this 2-hop neighbor
                    {
                        //look up for me
                        foreach (var threeHopTuple in twoHopNeighbor.NeighborsDiscovered())//add this later: .Where(x => x.Item2 < lastMet)
                        {
                            IDiscovery threeHopNeighbor = threeHopTuple.Item1;
                            if (threeHopNeighbor.Equals(this))//he already discovered me
                            {
                                int discoveredMe = threeHopTuple.Item2.FirstDiscovered;
                                if (discoveredMe < lastMet)
                                {
                                    neighbors[twoHopNeighbor] = new ContactInfo(transmission.Slot);
                                    neighbors[twoHopNeighbor].Update(lastMet);
                                    discoveredNodes.Add(twoHopNeighbor);
                                }
                            }
                        }
                    }
                    else if (firstDiscovered < transmission.Slot)
                    {
                        //update
                        if (lastMet > neighbors[twoHopNeighbor].LastMet)
                        {
                            neighbors[twoHopNeighbor].Update(lastMet);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Acc " + base.ToString();
        }
    }
}
