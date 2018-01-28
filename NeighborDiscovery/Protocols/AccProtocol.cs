using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public abstract class AccProtocol:BoundedProtocol
    {
        

        public abstract double SlotGain(int slot);

        protected AccProtocol(int id) : base(id)
        {
            

        }

        protected double SpatialSimilarity(int t0, int tn, IDiscoveryProtocol neighbor)
        {
            var iNeighbors = Get2HopNeighborsFromNeighbor(neighbor);
            var jNeighbors = Neighbors().Where(n => !n.Equals(neighbor));
            double num = IntersectionOfKnownNeighbors(iNeighbors, jNeighbors);
            double den = IntersectionOfKnownNeighbors(jNeighbors, jNeighbors);

            return num/den;
        }

        protected double TemporalDiversity(int t0, int tn, IDiscoveryProtocol neighbor)//how good is i(the neighbor) to j(S)
        {
            var iSlots = GetDeviceNextTransmissions(t0,tn, neighbor);
            var jSlots = GetDeviceNextTransmissions(t0,tn, this);
            double num = IntersectionOfListeningSlots(iSlots,iSlots) - IntersectionOfListeningSlots(iSlots, jSlots);
            double den = tn - t0;
            
            return num/den;
        }

        protected int IntersectionOfKnownNeighbors(IEnumerable<IDiscoveryProtocol> iNeighbors,
            IEnumerable<IDiscoveryProtocol> jNeighbors)
        {
            return iNeighbors.Intersect(jNeighbors).Count();
        }

        protected int IntersectionOfListeningSlots(IEnumerable<int> p1, IEnumerable<int> p2)
        {
            return p1.Intersect(p2).Count();
        }

        protected IEnumerable<IDiscoveryProtocol> Get2HopNeighborsFromNeighbor(IDiscoveryProtocol neighbor)
        {
            var lastContact = ((ContactInfo)GetContactInfo(neighbor)).LastContact;
            int slotsPassed = InternalTimeSlot - lastContact;//how many slots since I listened to it the last time
            int myKey = neighbor.InternalTimeSlot - slotsPassed;//internal time slot of the neighbor the last time I listened to it
            //this can access 2 hop neighbors via a neighbor when they were discovered before myKey
            foreach (var neighbor2Hop in neighbor.Neighbors())
            {
                if(neighbor2Hop.Equals(this))//skip yourself
                    continue;
                var info2Hop = neighbor.GetContactInfo(neighbor2Hop);
                if (info2Hop.FirstContact < myKey)
                    yield return neighbor2Hop;
            }
        }

        protected IEnumerable<int> GetDeviceNextTransmissions(int t0, int tn, IDiscoveryProtocol device)
        {
            var clone = device.Clone();
            clone.MoveNext(device.InternalTimeSlot + t0);
            int cnt = t0;
            while (cnt <= tn)
            {
                clone.MoveNext();
                if (clone.IsTransmitting())
                    yield return cnt;
                cnt++;
            }
        }
    }
}
