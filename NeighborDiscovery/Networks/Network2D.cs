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
        private readonly Dictionary<Network2DNode, List<Network2DNode>> _neighbors;
        
        public bool IsStatic { get; private set; }
        public int CurrentSize => _nodes.Count;
        public int NumberOfLinks { get; private set; }
        
        
        public Network2D(bool isStatic = false)
        {
            _nodes = new HashSet<Network2DNode>();
            IsStatic = isStatic;
            if (IsStatic)
            {
                _neighbors = new Dictionary<Network2DNode, List<Network2DNode>>();
            }
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
            _neighbors.Add(newNode2D, new List<Network2DNode>());
            

            //update number of links and adjacency list when the network is static
            foreach(var possibleNeighbor in _nodes)
            {
                if (newNode2D.NodeIsInRange(possibleNeighbor))
                {
                    NumberOfLinks++;
                    if (IsStatic)
                    {
                        if (IsStatic)
                        {
                            _neighbors[newNode2D].Add(possibleNeighbor);
                        }
                    }
                }
                if (possibleNeighbor.NodeIsInRange(newNode2D))
                {
                    NumberOfLinks++;
                    if (IsStatic)
                    {
                        _neighbors[possibleNeighbor].Add(newNode2D);
                    }
                }
            }

            _nodes.Add(newNode2D);
        }

        public IEnumerable<Network2DNode> NeighborsOf(Network2DNode node)
        {
            return IsStatic ? _neighbors[node] : _nodes.Where(node.NodeIsInRange);
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
