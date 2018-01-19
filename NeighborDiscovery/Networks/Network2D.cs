using System;
using System.Collections.Generic;
using System.Linq;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Networks
{
    public class Network2D
    {
        private readonly HashSet<Network2DNode> _nodes;
        public int CurrentSize => _nodes.Count;
        public int NumberOfLinks { get; private set; }
        
        public Network2D()
        {
            _nodes = new HashSet<Network2DNode>();
        }

        public void RemoveNode(Network2DNode node)
        {
            if(!_nodes.Remove(node))
                throw new Exception("Node does not exists");
        }

        public void AddNode(Network2DNode newNode2D)
        {
            if(_nodes.Contains(newNode2D))
                throw new Exception("Node already exists");
            
            //update number of links   
            foreach(var possibleNeighbor in _nodes)
            {
                if (newNode2D.NodeIsInRange(possibleNeighbor))
                {
                    NumberOfLinks++;
                }
                if (possibleNeighbor.NodeIsInRange(newNode2D))
                {
                    NumberOfLinks++;
                }
            }

            _nodes.Add(newNode2D);
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
