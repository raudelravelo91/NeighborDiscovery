using System;

namespace NeighborDiscovery.Protocols
{
    public class NeighborInfo
    {
        public int NeighborId { get; set; }
        public int Hops { get; set; }
        public int LastSlotRendevouz { get; set; }
    }
}