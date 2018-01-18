using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public interface IContact
    {
        IDiscoveryProtocol Device { get; }

        int FirstContact { get; }
    }
}
