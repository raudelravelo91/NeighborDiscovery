using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public interface INodeResult
    {
        IEnumerable<IContact> NewDiscoveries{ get;}

        int Count{ get;}

        void AddDiscovery(IContact newDiscovery);
    }
}
