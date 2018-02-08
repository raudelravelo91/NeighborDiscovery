using NeighborDiscovery.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Statistics
{
    public class NodeResult : INodeResult
    {
        public IEnumerable<IContact> NewDiscoveries => _discoveries;
        public List<IContact> _discoveries;

        public NodeResult()
        { 
            _discoveries = new List<IContact>();
        }

        public void AddDiscovery(IContact newDiscovery)
        {
            _discoveries.Add(newDiscovery);
        }
    }
}
