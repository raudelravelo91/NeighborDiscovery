using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Nodes;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public class TwoNodesEnvironment
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public NodeType NodeType { get; set; }

        private List<int> node1Transmissions;
        private List<int> node2Transmissions;

        public TwoNodesEnvironment(NodeType nodeType, Node node1, Node node2)
        {
            NodeType = nodeType;
            Node1 = node1;
            Node2 = node2;
        }

        private void GenerateTransmissions()
        {
            node1Transmissions = new List<int>();
            node2Transmissions = new List<int>();

            GenerateUntil(LimitByDutyCycle(Node1), Node1, node1Transmissions);
            GenerateUntil(LimitByDutyCycle(Node2), Node2, node2Transmissions);
        }

        private void GenerateUntil(int limit, Node node, List<int> nodeTransmissionList)
        {
            var nextTransmission = node.NextTransmission();

            while (nextTransmission.Slot < limit)
            {
                nodeTransmissionList.Add(nextTransmission.Slot);
                nextTransmission = node.NextTransmission();
            }
        }

        private int LimitByDutyCycle(Node node)
        {
            var duty = (node.GetDutyCycle() * 100);
            if (duty < 1)
                return 2000000;
            else return 1000000;
        }

        public StatisticTestResult RunSimulation(Func<double, double, bool> checker, bool skew = false)
        {
            GenerateTransmissions();
            var fromLimit = 4000;
            var offSetLimit = 200;
            var clockSkew = (skew) ? 0.5 : 0;

            if (NodeType.Equals(NodeType.UConnect))
            {
                fromLimit = 2000;
                offSetLimit = 150;
            }
            else if (NodeType.Equals(NodeType.Disco))
            {
                fromLimit = 2000;
                offSetLimit = 200;
            }
            else if (NodeType.Equals(NodeType.StripedSearchlight))
            {
                fromLimit = 1700;
                offSetLimit = 200;
            }

            var result = new StatisticTestResult(offSetLimit * fromLimit);

            for (var offSet = 0; offSet < offSetLimit; offSet++)
            {
                for (var from = offSet; from < offSet + fromLimit; from++)
                {
                    var cnt1 = 0;
                    var cnt2 = 0;

                    var node1Trans = clockSkew + node1Transmissions[0];
                    //if (skew) node1Trans += clockSkew;
                    while (node1Trans < from)
                    {
                        cnt1++;
                        node1Trans = clockSkew + node1Transmissions[cnt1];
                        //if (skew) node1Trans += clockSkew;
                    }

                    double node2Trans = offSet + node2Transmissions[0];
                    while (node2Trans < from)
                    {
                        cnt2++;
                        node2Trans = offSet + node2Transmissions[cnt2];
                    }

                    while (!checker(node1Trans, node2Trans))
                    {
                        if (node1Trans < node2Trans)
                        {
                            cnt1++;
                        }
                        else if (node1Trans > node2Trans)
                        {
                            cnt2++;
                        }
                        node1Trans = clockSkew + node1Transmissions[cnt1];
                        //if (skew) node1Trans += clockSkew;
                        node2Trans = offSet + node2Transmissions[cnt2];
                    }
                    var latency = (int)node2Trans - from + 1;
                    //Console.WriteLine();
                    result.AddDiscovery(latency);
                }
            }

            return result;
        }


    }

}
