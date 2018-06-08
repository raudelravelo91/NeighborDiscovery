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
            LatencyLimit = 10000;
        }

        public void RunWorkerAsync(TestSuite suite)
        {
            Worker.RunWorkerAsync(suite);
        }

        public void CancelAsync()
        {
            Worker.CancelAsync();
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
                
                var test = environment.RunSingleSimulation(n.Data, DeviceProtocol);
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
