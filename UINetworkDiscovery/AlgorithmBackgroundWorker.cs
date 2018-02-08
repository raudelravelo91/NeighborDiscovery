using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Utils;
using NeighborDiscovery.DataGeneration;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Statistics;

namespace UINetworkDiscovery
{
    public class AlgorithmBackgroundWorker
    {
        public NodeType DeviceProtocol { get; private set; }
        public BackgroundWorker Worker { get; private set; }
        public bool IsReading { get; private set; }
        public int LatencyLimit{ get;set;}

        public AlgorithmBackgroundWorker(NodeType type)
        {
            DeviceProtocol = type;
            Worker = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true, };
            Worker.DoWork += Start;
            IsReading = false;
            LatencyLimit = 1000;
        }

        public void RunWorkerAsync(TestSuite suite)
        {
            Worker.RunWorkerAsync(suite);
        }

        public void CancelAsync()
        {
            Worker.CancelAsync();
        }

        private IDiscoveryProtocol NodeFactoryFunc(int id, int dutyCycle)
        {
            switch (DeviceProtocol)
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
            lock (MainWindow.RunningInfo)
            {
                MainWindow.RunningInfo.AddRunningAlgorithm();
            }
            IsReading = true;
            Worker.ReportProgress(0);
            if(!(e.Argument is TestSuite suite))
                throw new Exception("Wrong parameter argument, expected TestSuite");
            
            var environment = new NeighborDiscoveryEnvironment();
            var statisticResults = new StatisticsResult(DeviceProtocol);
            
            var cnt = 0;
            foreach (var n in suite.Tests)
            {
                if (Worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    lock (MainWindow.RunningInfo)
                    {
                        MainWindow.RunningInfo.RemoveRunningAlgorithm(true);
                    }
                    return;
                }
                
                var test = environment.RunSingleSimulation(n.Data, DeviceProtocol, 0);
                statisticResults.AddStatisticTest(test);
                cnt++;
                Worker.ReportProgress(cnt * 100 / suite.NumberOfTests);
            }
            
            statisticResults.BuildAverageFractionOfDiscovey(LatencyLimit);
            lock (MainWindow.RunningInfo)
            {
                MainWindow.RunningInfo.RemoveRunningAlgorithm(false);
            }
            e.Result = statisticResults;
        }
    }
}
