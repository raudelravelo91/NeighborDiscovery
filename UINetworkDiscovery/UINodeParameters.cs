using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace UINetworkDiscovery
{
    public class UiNodeParameters
    {
        public NodeType NodeType { get; private set; }

        public UiNodeParameters(NodeType nodeType)
        {
            NodeType = nodeType;
        }

    }

    public class UiDiscoParameters : UiNodeParameters
    {
        public bool Balanced { get; private set; }
        public UiDiscoParameters(bool balanced) : base(NodeType.Disco)
        {
            Balanced = balanced;
        }
    }
}
