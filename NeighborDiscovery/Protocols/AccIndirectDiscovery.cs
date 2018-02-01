using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public abstract class AccIndirectDiscovery:AccProtocol
    {
        protected AccIndirectDiscovery(int id) : base(id)
        {

        }


        protected virtual double SpatialSimilarity(int t0, int tn, IDiscoveryProtocol neighbor)
        {
            var iNeighbors = Get2HopNeighborsFromDirectNeighbor(neighbor);//the neighbor
            var jNeighbors = Neighbors().Where(n => !n.Equals(neighbor));//S (this)
            double num = iNeighbors.Intersect(jNeighbors).Count();
            double den = jNeighbors.Count();

            return num/den;
        }

        protected virtual double TemporalDiversity(int t0, int tn, IDiscoveryProtocol neighbor)//i is the neighbor and j is S(this)
        {
            var iSlots = GetDeviceNextTransmissionSlot(t0,tn, neighbor);
            var jSlots = GetDeviceNextTransmissionSlot(t0,tn, this);
            double num = iSlots.Count() - iSlots.Intersect(jSlots).Count();
            double den = tn - t0;
            
            return num/den;
        }

        
    }
}
