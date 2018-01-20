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
        private readonly Dictionary<Network2DNode, Dictionary<Network2DNode, int>> _neighbors;
        public int CurrentTimeSlot { get; set; }

        public bool IsStatic { get; private set; }
        public int CurrentSize => _nodes.Count;
        public int NumberOfLinks { get; private set; }
        
        public Network2D(bool isStatic = false)
        {
            _nodes = new HashSet<Network2DNode>();
            IsStatic = isStatic;
            if (IsStatic)
            {
                _neighbors = new Dictionary<Network2DNode, Dictionary<Network2DNode, int>>();
            }
        }

        public int GotInRange(Network2DNode node1, Network2DNode node2)
        {
            if(!_nodes.Contains(node1) || !_nodes.Contains(node2))
                throw new ArgumentNullException(string.Format("{0} or {1} not founded", node1, node2));
            return _neighbors[node1][node2];
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
            _neighbors.Add(newNode2D, new Dictionary<Network2DNode, int>());
            

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
                            _neighbors[newNode2D].Add(possibleNeighbor, CurrentTimeSlot);
                        }
                    }
                }
                if (possibleNeighbor.NodeIsInRange(newNode2D))
                {
                    NumberOfLinks++;
                    if (IsStatic)
                    {
                        _neighbors[possibleNeighbor].Add(newNode2D, CurrentTimeSlot);
                    }
                }
            }

            _nodes.Add(newNode2D);
        }

        public IEnumerable<Network2DNode> NeighborsOf(Network2DNode node)
        {
            return IsStatic ? _neighbors[node].Select(x => x.Key) : _nodes.Where(node.NodeIsInRange);
        }

        public void MoveAllNodes()
        {
            foreach (var node in _nodes)
            {
                node.Move();
            }

            CurrentTimeSlot++;
        }
    }
}
