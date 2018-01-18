using System.Collections.Generic;
using System.Linq;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Networks
{
    public class Network2D
    {
        private readonly List<Network2DNode> _nodes;
        public int CurrentSize => _nodes.Count;
        public int NumberOfLinks { get; private set; }
        
        public Network2D()
        {
            _nodes = new List<Network2DNode>();
        }

        public Network2DNode AddNode(double xPos, double yPos, int communicationRange)
        {
            var newNode2D = new Network2DNode(_nodes.Count, xPos, yPos, communicationRange);
            //update number of links   
            for (int i = 0; i < _nodes.Count; i++)
            {
                var possibleNeighbor = _nodes[i];
                if (newNode2D.NodeIsInRange(possibleNeighbor))
                {
                    NumberOfLinks++;
                }
                if (possibleNeighbor.NodeIsInRange(newNode2D))
                {
                    NumberOfLinks++;
                }
            }
            return newNode2D;
        }

        public IEnumerable<Network2DNode> NeighborsOf(Network2DNode node)
        {
            return _nodes.Where(node.NodeIsInRange);
        }

        public void MoveAllNodes()
        {
            foreach (var node in _nodes)
            {
                node.Move();
            }
        }
        
    }
}
