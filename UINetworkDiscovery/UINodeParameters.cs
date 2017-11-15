using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace UINetworkDiscovery
{
    public class UINodeParameters
    {
        public NodeType NodeType { get; private set; }

        public UINodeParameters(NodeType nodeType)
        {
            NodeType = nodeType;
        }

    }

    public class UIDiscoParameters : UINodeParameters
    {
        public bool Balanced { get; private set; }
        public UIDiscoParameters(bool balanced) : base(NodeType.Disco)
        {
            Balanced = balanced;
        }
    }
}
