using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Nodes;


namespace NeighborDiscovery.Utils
{
    public enum NodeType
    {
        Birthday,
        Disco,
        UConnect,
        Searchlight,
        StripedSearchlight,
        Hello,
        TestAlgorithm,
        GNihao,
        AccGossipGNihao,
        PNihao,
        AccGossipPNihao
    }

    //public class NodeFactory
    //{
    //    public static Node CreateNode(NodeType type, NodeParameters parameters)
    //    {
    //        Node n;
    //        switch (type)
    //        {
    //            case NodeType.GNihao:
    //                GNihaoParameters gp = parameters as GNihaoParameters;
    //                if (gp == null)
    //                {
    //                    throw new Exception("Bad parameters for node of type: " + type.ToString());
    //                }
                    
    //                n = new GNihao(parameters.Id, (int)parameters.DutyCyclePercentage, parameters.CommunicationRange , gp.M, gp.RandomInitialState);
    //                break;
    //            case NodeType.PNihao:
    //                PNihaoParameters pp = parameters as PNihaoParameters;
    //                if (pp == null)
    //                {
    //                    throw new Exception("Bad parameters for node of type: " + type.ToString());
    //                }

    //                n = new PNihao(parameters.Id, (int)parameters.DutyCyclePercentage, parameters.CommunicationRange, pp.M, pp.RandomInitialState);
    //                break;
    //            case NodeType.Birthday:
    //                n = new BirthdayNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //            case NodeType.Disco:
    //                DiscoParameters dp = parameters as DiscoParameters;
    //                if(dp == null)
    //                {
    //                    throw new Exception("Bad parameters for node of type: " + type.ToString());
    //                }
    //                n = new DiscoNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange, dp.Balanced);
    //                break;
    //            case NodeType.UConnect:
    //                n = new UConnectNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //            case NodeType.Searchlight:
    //                n = new SearchlightNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //            case NodeType.StripedSearchlight:
    //                n = new StripedSearchlightNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //            case NodeType.Hello:
    //                HelloParameters hp = parameters as HelloParameters;
    //                n = new HelloNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange, hp.Symmetric);//fix the true
    //                break;
    //            case NodeType.TestAlgorithm:
    //                n = new TestAlgorithm(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //            default:
    //                n = new BirthdayNode(parameters.Id, parameters.DutyCyclePercentage, parameters.CommunicationRange);
    //                break;
    //        }

    //        return n;
    //    }
    //}
}
