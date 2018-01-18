using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
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

        public AlgorithmBackgroundWorker(NodeType type)
        {
            Type = type;
            Worker = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true, };
            Worker.DoWork += Start;
            IsReading = false;
        }

        public void RunWorkerAsync(string fileName)
        {
            Worker.RunWorkerAsync(fileName);
        }

        public void CancelAsync()
        {
            Worker.CancelAsync();
        }

        private IDiscoveryProtocol NodeFactoryFunc(int id, int dutyCycle)
        {
            switch (Type)
            {
                case NodeType.Disco:
                    return new Disco(id, dutyCycle);
                case NodeType.UConnect:
                    return new UConnect( id, dutyCycle);
                //case NodeType.Birthday:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                //case NodeType.Searchlight:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                //case NodeType.Hello:
                //    n = NodeFactory.CreateNode(Type, new HelloParameters(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, BasicParameters.numberOfDutyCyclesToHandle == 1));
                //    break;
                //case NodeType.TestAlgorithm:
                //    n = NodeFactory.CreateNode(Type, new NodeParameters(Type, basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange));
                //    break;
                //case NodeType.GNihao:
                //    n = new BNihaoR(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime);
                //    break;
                //case NodeType.BNihao:
                //    n = new BNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime);
                //    break;
                //case NodeType.AccGossipGNihao:
                //    n = new AccGossipGNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime);
                //    break;
                //case NodeType.AccGossipPNihao:
                    //n = new AccGossipPNihao(basicParameters.Id, basicParameters.DutyCyclePercentage, basicParameters.CommunicationRange, 20, basicParameters.StartUpTime, false);
                    //break;
                default:
                    throw new Exception("Can not create the given type of node");
            }
        }

        private void Start(object sender, DoWorkEventArgs e)
        {

        }

        //private void Start(object sender, DoWorkEventArgs e)
        //{
        //    lock (MainWindow.RunningInfo)
        //    {
        //        MainWindow.RunningInfo.AddRunningAlgorithm();
        //    }
        //    IsReading = true;
        //    Worker.ReportProgress(0);
        //    var fileName = e.Argument.ToString();
        //    var reader = new RandomGenerator();
        //    var networks = reader.CreateFromFile(fileName, NodeFactoryFunc).ToList();
        //    var environment = new FullDiscoveryEnvironment();
        //    var statisticResults = new StatisticsResult(Type);
        //    IsReading = false;
        //    var cnt = 0;
        //    foreach (var n in networks)
        //    {
        //        if (Worker.CancellationPending == true)
        //        {
        //            e.Cancel = true;
        //            lock(MainWindow.RunningInfo)
        //            {
        //                MainWindow.RunningInfo.RemoveRunningAlgorithm(true);
        //            }
        //            return;
        //        }
        //        //int trackNode = (Type.Equals(NodeType.GNihao)) ? -1 : -1; 
        //        var test = environment.RunSingleSimulation(n);
        //        statisticResults.AddStatisticTest(test);
        //        cnt++;
        //        Worker.ReportProgress(cnt*100/networks.Count);
        //    }
        //    var latencyLimit = 1000;
        //    statisticResults.BuildAverageFractionOfDiscovey(latencyLimit);
        //    lock(MainWindow.RunningInfo)
        //    {
        //        MainWindow.RunningInfo.RemoveRunningAlgorithm(false);
                
        //    }
        //    e.Result = statisticResults;
        //}
    }
}
