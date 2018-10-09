using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public class NodeFactory
    {
        public static BoundedProtocol CreateNew(NodeType type, int ID, int DC, int COR)
        {
            switch (type)
            {
                case NodeType.Disco:
                    return new Disco(ID, DC);
                case NodeType.UConnect:
                    return new UConnect(ID, DC);
                case NodeType.GNihao:
                    return  new GNihao(ID, DC, COR);
                case NodeType.THL2H:
                    return new THL2H(ID, DC, COR);
                case NodeType.THL2HExtended:
                    return new THL2HExtended(ID, DC, COR);
                default:
                    throw new InvalidEnumArgumentException(type.ToString());
            }
        }
    }
}
