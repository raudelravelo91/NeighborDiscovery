using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Nodes;
using NeighborDiscovery.Utils;
using NeighborDiscovery;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Statistics;

namespace UINetworkDiscovery
{
    public class AlgorithmBackgroundWorker
    {
        public NodeType Type { get; private set; }
        public BackgroundWorker Worker { get; private set; }

        public bool IsReading { get; private set; }

        private UINodeParameters NodeParameters { get; set; }

        public AlgorithmBackgroundWorker(NodeType type)
        {
            Type = type;
            Worker = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true, };
            Worker.DoWork += Start;
            IsReading = false;
        }

        public void RunWorkerAsync(string fileName, UINodeParameters nodeParameters = null)
        {
            if (nodeParameters != null && Type != nodeParameters.NodeType)
                throw new ArgumentException("Wrong UINodeParameters type, expected: " + Type.ToString());
            NodeParameters = nodeParameters;
            Worker.RunWorkerAsync(fileName);
        }

        public void CancelAsync()
        {
            Worker.CancelAsync();
        }

        private Node NodeFactoryFunc(BasicParameters basicParameters)
        {
            Node n;
            switch (Type)
            {
                //case NodeType.Birthday:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                //case NodeType.Disco:
                //    //var uiParameters = NodeParameters as UIDiscoParameters;
                //    n = NodeFactory.CreateNode(Type, new DiscoParameters(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, BasicParameters.numberOfDutyCyclesToHandle == 1));
                //    break;
                case NodeType.UConnect:
                    n = new UConnectNode( basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, basicParameters.StartUpTime);
                    break;
                //case NodeType.Searchlight:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                //case NodeType.Hello:
                //    n = NodeFactory.CreateNode(Type, new HelloParameters(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, BasicParameters.numberOfDutyCyclesToHandle == 1));
                //    break;
                //case NodeType.TestAlgorithm:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                case NodeType.GNihao:
                    n = new GNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime, false);
                    break;
                case NodeType.PNihao:
                    n = new PNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime, false);
                    break;
                case NodeType.AccGossipGNihao:
                    n = new AccGossipGNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime ,false);
                    break;
                default:
                case NodeType.AccGossipPNihao:
                    n = new AccGossipPNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime, false);
                    break;
                    throw new Exception("Can not create the given type of node");
            }
            return n;
        }

        private void Start(object sender, DoWorkEventArgs e)
        {
            lock (MainWindow.RunningInfo)
            {
                MainWindow.RunningInfo.AddRunningAlgorithm();
            }
            IsReading = true;
            Worker.ReportProgress(0);
            string fileName = e.Argument.ToString();
            var reader = new NetworkGenerator();
            var networks = reader.CreateFromFile(fileName, NodeFactoryFunc).ToList();
            var environment = new FullDiscoveryEnvironment();
            StatisticsResult statisticResults = new StatisticsResult(Type);
            IsReading = false;
            int cnt = 0;
            foreach (var n in networks)
            {
                if (Worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    lock(MainWindow.RunningInfo)
                    {
                        MainWindow.RunningInfo.RemoveRunningAlgorithm(true);
                    }
                    return;
                }
                //int trackNode = (Type.Equals(NodeType.GNihao)) ? -1 : -1; 
                var test = environment.RunSingleSimulation(n);
                statisticResults.AddStatisticTest(test);
                cnt++;
                Worker.ReportProgress(cnt*100/networks.Count);
            }
            statisticResults.BuildAverageFractionOfDiscovey();
            lock(MainWindow.RunningInfo)
            {
                MainWindow.RunningInfo.RemoveRunningAlgorithm(false);
                
            }
            e.Result = statisticResults;
        }

    }
}
