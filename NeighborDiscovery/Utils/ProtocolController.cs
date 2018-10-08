using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Utils
{
    public class ProtocolController
    {
        public static void SetToState(IDiscoveryProtocol protocol, int state)
        {
            protocol.Reset();
            int i = 0;
            while (i < state)
            {
                protocol.MoveNext();
                i++;
            }
        }
    }
}
