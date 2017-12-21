using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Networks
{
    public class Network2D
    {
        public Network2D(int networkSize, int[,] gotInRange)
        {
            nodes = new List<Network2DNode>();
            this.gotInRange = gotInRange;
            neighbors = new List<Network2DNode>[networkSize];
            for (var i = 0; i < networkSize; i++)
            {
                neighbors[i] = new List<Network2DNode>();
            }
            dutyCycles = new HashSet<double>();
        }

        private class Network2DNode
        {
            public IDiscovery Device { get; private set; }

            public int CommunicationRange
            {
                get; private set;
            }

            public Pair Position { get; private set; }

            public int Degree { get; set; }

            public int StartUpTime => Device.StartUpTime;

            public Network2DNode(IDiscovery device, double xPos, double yPos, int commRange)
            {
                Device = device;
                Position = new Pair(xPos, yPos);
                CommunicationRange = commRange;
            }

            public bool IsNeighborOf(Network2DNode other)
            {
                return this.Position.DistanceTo(other.Position) <= CommunicationRange;
            }

            public override string ToString()
            {
                return Device.ToString() + " Deg: " + Degree + " Position: (" + Position.ToString() + ")";
            }
        }
        private List<Network2DNode> nodes;
        private int[,] gotInRange;
        private List<Network2DNode>[] neighbors;
        
        public int NetworkSize => nodes.Count;
        public int NumberOfLinks { get; private set; }
        public int NumberOfSymmetricNeighbors { get; private set; }

        private HashSet<double> dutyCycles;
        public int DifDutyCyclesCount => dutyCycles.Count;
        public bool IsAsymmetric => DifDutyCyclesCount > 1;

        public void BuildNetwork(double fixAsymmetricCase)
        {
            BuildNeighborsList(fixAsymmetricCase);
            
        }
        //methods
        public IDiscovery GetDevice(int index)
        {
            return nodes[index].Device;
        }

        public int GetDeviceStartUpSlot(int id)
        {
            return nodes[id].StartUpTime;
        }

        public void AddNode(Node node, double xPos, double yPos)
        {
            nodes.Add(new Network2DNode(node, xPos, yPos, node.CommunicationRange));
            
            if (!dutyCycles.Contains(node.GetDutyCycle()))
                dutyCycles.Add(node.GetDutyCycle());
        }

        public List<IDiscovery> NeighborsOf(int index)
        {
            return neighbors[index].Select(x => x.Device).ToList();
        }

        public int GotInRangeWith(int nodeIndex, int neighborIndex)
        {
            return gotInRange[nodeIndex,neighborIndex];
        }

        public int DegreeOf(int index)
        {
            return nodes[index].Degree;
        }

        private void BuildNeighborsList(double symmetricPercentageAllowed)
        {
            
            for (var i = 0; i < NetworkSize; i++)
            {
                var node1 = nodes[i];
                for (var j = i+1; j < NetworkSize; j++)
                {
                    var node2 = nodes[j];
                    if (node1.IsNeighborOf(node2))
                    {
                        if (dutyCycles.Count > 1 && node1.Device.GetDutyCycle() == node2.Device.GetDutyCycle())
                        {
                            if((NumberOfLinks == 0) || ((NumberOfSymmetricNeighbors * 1.0 / NumberOfLinks) >= symmetricPercentageAllowed))
                                continue;
                            NumberOfSymmetricNeighbors += 2;
                        }
                        neighbors[i].Add(node2);
                        node1.Degree++;
                        neighbors[j].Add(node1);
                        node2.Degree++;
                        NumberOfLinks +=2;
                        //int minPeriod = Math.Min(node1.Device.T, node2.Device.T);
                        //int maxStart = Math.Max(node1.StartUpTime, node2.StartUpTime);
                        //int gRange = maxStart + Shuffle.random.Next(minPeriod);
                        //gotInRange[i,j] = gRange;
                        //gotInRange[j,i] = gRange;
                    }
                    
                }
            }
        }
    }

    public class Pair
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Pair(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Pair p)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(X - p.X),2)+Math.Pow(Y - p.Y,2));
        }

        public override string ToString()
        {
            return Math.Round(X,2) + " ; " + Math.Round(Y,2);
        }
    }
}
