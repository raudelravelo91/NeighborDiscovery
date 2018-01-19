using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Protocols;
using OxyPlot;

namespace UINetworkDiscovery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RunningInfo RunningInfo { get; private set; }
        private string fileName;
        private PlotModel Model;
        AlgorithmBackgroundWorker workerDisco;
        AlgorithmBackgroundWorker workerUConnect;
        AlgorithmBackgroundWorker workerSearchLight;
        AlgorithmBackgroundWorker workerBirthday;
        AlgorithmBackgroundWorker workerStripedSearchlight;
        AlgorithmBackgroundWorker workerTestAlgorithm;
        AlgorithmBackgroundWorker workerGNihao;
        AlgorithmBackgroundWorker workerAccGossipGNihao;
        AlgorithmBackgroundWorker workerAccGossipPNihao;
        List<int> threads = new List<int>();

        public MainWindow()
        {
            InitializeComponent();

            fileName = "testcases.txt";
            Model = new PlotModel();
            SetXAxes(400, 8);
            SetYAxes(1, 10);
            var currentMargins = oxyplot.PlotMargins;
            Model.PlotMargins = new OxyThickness(currentMargins.Left, btClear.Height, currentMargins.Right,
                currentMargins.Bottom);
            oxyplot.PlotMargins = new System.Windows.Thickness(currentMargins.Left, btClear.Height,
                currentMargins.Right, currentMargins.Bottom);
            RunningInfo = new RunningInfo();

            SetDefaultSettings();

            workerDisco = new AlgorithmBackgroundWorker(NodeType.Disco);
            workerDisco.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerDisco.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerUConnect = new AlgorithmBackgroundWorker(NodeType.UConnect);
            workerUConnect.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerUConnect.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerSearchLight = new AlgorithmBackgroundWorker(NodeType.Searchlight);
            workerSearchLight.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerSearchLight.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerBirthday = new AlgorithmBackgroundWorker(NodeType.Birthday);
            workerBirthday.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerBirthday.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerStripedSearchlight = new AlgorithmBackgroundWorker(NodeType.StripedSearchlight);
            workerStripedSearchlight.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerStripedSearchlight.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerTestAlgorithm = new AlgorithmBackgroundWorker(NodeType.TestAlgorithm);
            workerTestAlgorithm.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerTestAlgorithm.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerGNihao = new AlgorithmBackgroundWorker(NodeType.GNihao);
            workerGNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerGNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerAccGossipGNihao = new AlgorithmBackgroundWorker(NodeType.AccGossipGNihao);
            workerAccGossipGNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerAccGossipGNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            workerAccGossipPNihao = new AlgorithmBackgroundWorker(NodeType.AccGossipPNihao);
            workerAccGossipPNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerAccGossipPNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;

        }

        public void SetXAxes(double x, double xMajorStep)
        {
            if (x / xMajorStep > 0)
            {
                var X = new LinearAxis(AxisPosition.Bottom, 0, x, "Discovery latency (slots)")
                {
                    MajorStep = x / xMajorStep,
                    MajorGridlineThickness = 1,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorTickSize = 0,
                    TitleFontSize = 14,
                    TickStyle = TickStyle.Inside
                };
                if (Model.Axes.Count > 0)
                    Model.Axes[0] = X;
                else Model.Axes.Add(X);

                oxyplot.Model = Model;
                oxyplot.RefreshPlot(true);
            }
        }

        public void SetYAxes(double y, double yMajorStep)
        {
            if (y / yMajorStep > 0)
            {
                var Y = new LinearAxis(AxisPosition.Left, 0, y, "Fraction of Discoveries")
                {
                    MajorStep = y / yMajorStep,
                    MajorGridlineThickness = 1,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorTickSize = 0,
                    TitleFontSize = 14,
                    TickStyle = TickStyle.Inside
                };
                if (Model.Axes.Count > 1)
                    Model.Axes[1] = Y;
                else Model.Axes.Add(Y);
                oxyplot.Model = Model;
                oxyplot.RefreshPlot(true);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (((BackgroundWorker) sender).CancellationPending)
            {
                return;
            }

            //if (!btGenerate.IsEnabled)
            //{
            //    btGenerate.IsEnabled = !workerBirthday.IsReading && !workerDisco.IsReading 
            //        && !workerQuorum.IsReading && !workerSearchLight.IsReading && !workerUConnect.IsReading;
            //}
            if (sender.Equals(workerDisco.Worker))
            {
                progressBarDisco.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerUConnect.Worker))
            {
                progressBarUConnect.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerSearchLight.Worker))
            {
                progressBarSearchlight.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerBirthday.Worker))
            {
                progressBarBirthday.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerStripedSearchlight.Worker))
            {
                progressBarStripedSearchlight.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerTestAlgorithm.Worker))
            {
                progressBarTestAlg.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerGNihao.Worker))
            {
                progressBarGNihao.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerAccGossipGNihao.Worker))
            {
                progressBarBalancedNihao.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(workerAccGossipPNihao.Worker))
            {
                progressBarAccGossipPNihao.Value = e.ProgressPercentage;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!RunningInfo.CancelationPending)
            {
                if (e.Result is StatisticsResult results)
                {
                    Plot(results);
                }
                else throw new ArgumentException("Invalid Task type");

                if (!RunningInfo.IsRunning)
                {
                    btGenerate.IsEnabled = true;
                    btPlot.Content = "Run";
                    testCaseMessage.Text = "All done.";
                    testCaseMessage.Visibility = Visibility.Visible;
                    testCaseIcon.Fill = Brushes.Lime;
                    testCaseIcon.Visibility = Visibility.Visible;
                }
            }
            else //was canceled
            {
                if (!RunningInfo.IsRunning)
                {
                    btGenerate.IsEnabled = true;
                    btPlot.Content = "Run";
                    RunningInfo.CancelationPending = false;
                    testCaseIcon.Fill = Brushes.Red;
                    testCaseMessage.Text = "Cancelled by User.";
                }
            }
        }

        public bool GetDutyCycle(out double[] duties)
        {
            var dutyList = new List<double>();
            if (cb05p.IsChecked != null && cb05p.IsChecked == true)
                dutyList.Add(0.5);
            if (cb1p.IsChecked != null && cb1p.IsChecked == true)
                dutyList.Add(1);
            if (cb2p.IsChecked != null && cb2p.IsChecked == true)
                dutyList.Add(2);
            if (cb5p.IsChecked != null && cb5p.IsChecked == true)
                dutyList.Add(5);
            if (cb10p.IsChecked != null && cb10p.IsChecked == true)
                dutyList.Add(10);
            duties = dutyList.ToArray();
            return duties.Length > 0;
        }

        public bool GetTestSettings(out int numberOfTests, out int startUpLimit, out int posRange, out int networkSize,
            out int minCRange, out int maxCRange, out int gotInRange)
        {

            numberOfTests = startUpLimit = posRange = networkSize = minCRange = maxCRange = gotInRange = 0;
            return int.TryParse(tbNumberOfTestCases.Text, out numberOfTests) &&
                   int.TryParse(tbStartUpLimit.Text, out startUpLimit) &&
                   int.TryParse(tbPosRange.Text, out posRange) &&
                   int.TryParse(tbMinCommRange.Text, out minCRange) &&
                   int.TryParse(tbMaxCommRange.Text, out maxCRange) &&
                   int.TryParse(tbnetworkSize.Text, out networkSize) &&
                   int.TryParse(tbGotInRangeLimit.Text, out gotInRange);

        }

        private void SetDefaultSettings()
        {
            //algorithms
            cbDisco.IsChecked = true;
            cbUConnect.IsChecked = true;
            cbSearchlight.IsChecked = false;
            cbBirthday.IsChecked = false;
            cbStripedSearchlight.IsChecked = false;
            cbTestAlgorithm.IsChecked = false;
            cbGNihao.IsChecked = false;
            cbBalancedNihao.IsChecked = true;
            cbAccGossipPNihao.IsChecked = false;
            //duty cycle
            cb05p.IsChecked = false;
            cb1p.IsChecked = false;
            cb2p.IsChecked = false;
            cb5p.IsChecked = false;
            cb10p.IsChecked = true;
            tbFixAsymmetricNetwork.Text = "0";
            //generate settings
            tbNumberOfTestCases.Text = "100";
            tbnetworkSize.Text = "40";
            tbMinCommRange.Text = "20";
            tbMaxCommRange.Text = "100";
            tbPosRange.Text = "50";
            tbStartUpLimit.Text = "200";
            tbGotInRangeLimit.Text = "400";
        }

        private void btGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!RunningInfo.IsRunning)
            {
                int numberOfTests, startUpLimit, posRange, networkSize, minCRange, maxCRange, gotInRange;
                if (GetTestSettings(out numberOfTests, out startUpLimit, out posRange, out networkSize, out minCRange,
                    out maxCRange, out gotInRange))
                {
                    double[] duties;
                    if (GetDutyCycle(out duties))
                    {
                        //RandomGenerator.GenerateTestCasesToFile(this.fileName, numberOfTests, startUpLimit, posRange, networkSize, minCRange, maxCRange, duties, gotInRange);
                    }
                    else
                        MessageBox.Show("At least 1 Duty Cycle must be checked", "Duty Cycle Info", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Invalid Tests Setting Parameters (Ex: check all are numbers)",
                        "Tests Settings Info", MessageBoxButton.OK, MessageBoxImage.Information);

                testCaseMessage.Text = "Test Cases Generated.";
                testCaseMessage.Visibility = Visibility.Visible;
                testCaseIcon.Fill = Brushes.Lime;
                testCaseIcon.Visibility = Visibility.Visible;
            }
        }

        private void CancellAll()
        {
            workerDisco.CancelAsync();
            workerUConnect.CancelAsync();
            workerSearchLight.CancelAsync();
            workerBirthday.CancelAsync();
            workerStripedSearchlight.CancelAsync();
            workerTestAlgorithm.CancelAsync();
        }

        private void ResetAlgorithmProperties()
        {
            progressBarDisco.Value = 0;
            discoAvg.Text = "";
            discoCnt.Text = "";

            progressBarUConnect.Value = 0;
            uconnectAvg.Text = "";
            uconnectCnt.Text = "";

            progressBarBalancedNihao.Value = 0;
            uconnectAvg.Text = "";
            uconnectCnt.Text = "";

            //progressBarSearchlight.Value = 0;
            //searchlightAvg.Text = "";
            //searchlightCnt.Text = "";

            //progressBarBirthday.Value = 0;
            //birthdayAvg.Text = "";
            //birthdayCnt.Text = "";

            //progressBarHello.Value = 0;
            //stripedSearchlighAvg.Text = "";
            //stripedSearchlighCnt.Text = "";

            //progressBarTestAlg.Value = 0;
            //testAlgAvg.Text = "";
            //testAlgCnt.Text = "";
        }

        private void btPlot_Click(object sender, RoutedEventArgs e)
        {
            if (RunningInfo.IsRunning)
            {
                var result = MessageBox.Show("Do you want to cancel?", "Cancel", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    CancellAll();
                }
            }
            else
            {
                if (File.Exists(fileName))
                {
                    //RandomGenerator.PercentageToFix = double.Parse(tbFixAsymmetricNetwork.Text);
                    var selectedAlgs = cbDisco.IsChecked.Value || cbUConnect.IsChecked.Value ||
                                       cbSearchlight.IsChecked.Value || cbBirthday.IsChecked.Value ||
                                       cbStripedSearchlight.IsChecked.Value || cbTestAlgorithm.IsChecked.Value ||
                                       cbGNihao.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||
                                       cbAccGossipPNihao.IsChecked.Value;

                    if (selectedAlgs)
                    {
                        ResetAlgorithmProperties();
                        btPlot.Content = "Cancel";
                        btGenerate.IsEnabled = false;
                        testCaseMessage.Text = "Running Algorithm(s)...";
                        testCaseMessage.Visibility = Visibility.Visible;
                        testCaseIcon.Fill = Brushes.Yellow;
                        testCaseIcon.Visibility = Visibility.Visible;

                        if (cbDisco.IsChecked == true)
                        {
                            //var parameters = new DiscoParameters(-1, -1, -1, false);
                            //RunningInfo.AddRunningAlgorithm();
                            workerDisco.RunWorkerAsync(fileName);
                        }

                        if (cbUConnect.IsChecked == true)
                        {
                            //RunningInfo.AddRunningAlgorithm();
                            workerUConnect.RunWorkerAsync(fileName);
                        }

                        if (cbSearchlight.IsChecked == true)
                        {
                            //RunningInfo.AddRunningAlgorithm();
                            workerSearchLight.RunWorkerAsync(fileName);
                        }

                        if (cbBirthday.IsChecked == true)
                        {
                            //RunningInfo.AddRunningAlgorithm();
                            workerBirthday.RunWorkerAsync(fileName);
                        }

                        if (cbStripedSearchlight.IsChecked == true)
                        {
                            //RunningInfo.AddRunningAlgorithm();
                            workerStripedSearchlight.RunWorkerAsync(fileName);
                        }

                        if (cbTestAlgorithm.IsChecked == true)
                        {
                            //RunningInfo.AddRunningAlgorithm();
                            workerTestAlgorithm.RunWorkerAsync(fileName);
                        }

                        if (cbGNihao.IsChecked == true)
                        {
                            workerGNihao.RunWorkerAsync(fileName);
                        }

                        if (cbBalancedNihao.IsChecked == true)
                        {
                            workerAccGossipGNihao.RunWorkerAsync(fileName);
                        }

                        if (cbAccGossipPNihao.IsChecked == true)
                        {
                            workerAccGossipPNihao.RunWorkerAsync(fileName);
                        }
                    }
                    else
                        MessageBox.Show("No algorithm has been selected!", "Select Algorithms to Run",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("The program could not find the Tests file. You should generate the test first",
                        "Test File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ModelContainsAlgorithm(NodeType type)
        {
            var v = Model.Series.Where(s => s.Title.Equals(type.ToString()));
            return (v.Any());
        }

        private void Plot(StatisticsResult result)
        {
            var max = result.GetMaxLatency();
            var type = result.NodeType;
            OxyColor oxyColor;
            MarkerType markerType;
            LineStyle lineStyle;
            oxyColor = OxyColors.Black;
            markerType = MarkerType.None;
            lineStyle = LineStyle.Solid;

            switch (type)
            {
                case NodeType.Birthday:
                    //cbBirthday.Content = "Birthday (E: " + result.AverageContactByWakesUp * 100 + "%) " + "(Avg: " + result.AverageDiscoveryLatency + ")";
                    markerType = MarkerType.None;
                    oxyColor = OxyColors.Gray;
                    if (ModelContainsAlgorithm(NodeType.Birthday))
                    {
                        oxyColor = OxyColors.DarkGray;
                    }

                    lineStyle = LineStyle.Solid;
                    birthdayAvg.Text = result.AverageDiscoveryLatency.ToString();
                    birthdayCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.Disco:
                    //cbDisco.Content = "Disco (E: " + result.AverageContactByWakesUp * 100 + "%)" + "(Avg: " + result.AverageDiscoveryLatency + ")";
                    markerType = MarkerType.Triangle;
                    oxyColor = OxyColors.Magenta;
                    if (ModelContainsAlgorithm(NodeType.Disco))
                    {
                        oxyColor = OxyColors.DeepPink;
                    }

                    lineStyle = LineStyle.Dash;
                    discoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    discoCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.UConnect:
                    //cbUConnect.Content = "UConnect (E: " + result.AverageContactByWakesUp * 100 + "%)" + "(Avg: " + result.AverageDiscoveryLatency + ")";
                    markerType = MarkerType.Square;
                    oxyColor = OxyColors.Blue;
                    if (ModelContainsAlgorithm(NodeType.UConnect))
                    {
                        oxyColor = OxyColors.DarkBlue;
                    }

                    lineStyle = LineStyle.Dot;
                    uconnectAvg.Text = result.AverageDiscoveryLatency.ToString();
                    uconnectCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.Searchlight:
                    //cbSearchlightR.Content = "SearchlightR (E: " + result.AverageContactByWakesUp * 100 + "%)" + "(Avg: " + result.AverageDiscoveryLatency + ")";
                    markerType = MarkerType.Star;
                    oxyColor = OxyColors.LimeGreen;
                    if (ModelContainsAlgorithm(NodeType.Searchlight))
                    {
                        oxyColor = OxyColors.Green;
                    }

                    lineStyle = LineStyle.DashDot;
                    searchlightAvg.Text = result.AverageDiscoveryLatency.ToString();
                    searchlightCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.StripedSearchlight:
                    markerType = MarkerType.Star;
                    oxyColor = OxyColors.Blue;
                    if (ModelContainsAlgorithm(NodeType.StripedSearchlight))
                    {
                        oxyColor = OxyColors.DarkBlue;
                    }

                    lineStyle = LineStyle.LongDashDotDot;
                    stripedSearchlighAvg.Text = result.AverageDiscoveryLatency.ToString();
                    stripedSearchlighCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.TestAlgorithm:
                    markerType = MarkerType.Cross;
                    oxyColor = OxyColors.Pink;
                    if (ModelContainsAlgorithm(NodeType.TestAlgorithm))
                    {
                        oxyColor = OxyColors.Magenta;
                    }

                    lineStyle = LineStyle.LongDashDotDot;
                    testAlgAvg.Text = result.AverageDiscoveryLatency.ToString();
                    testAlgCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.BalancedNihao:
                    markerType = MarkerType.Circle;
                    oxyColor = OxyColors.Black;
                    if (ModelContainsAlgorithm(NodeType.BalancedNihao))
                    {
                        oxyColor = OxyColors.DarkGray;
                    }

                    lineStyle = LineStyle.Dot;
                    gNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    gNihaoCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.AccGossipGNihao:
                    markerType = MarkerType.Circle;
                    oxyColor = OxyColors.Red;
                    if (ModelContainsAlgorithm(NodeType.AccGossipGNihao))
                    {
                        oxyColor = OxyColors.DarkRed;
                    }

                    lineStyle = LineStyle.Dot;
                    AccGossipGNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    BalancedNihaoCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                case NodeType.AccGossipPNihao:
                    markerType = MarkerType.Circle;
                    oxyColor = OxyColors.Green;
                    if (ModelContainsAlgorithm(NodeType.AccGossipPNihao))
                    {
                        oxyColor = OxyColors.DarkGreen;
                    }

                    lineStyle = LineStyle.Dot;
                    AccGossipPNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    AccGossipPNihaoCnt.Text = result.AverageContactByWakesUp.ToString();
                    break;
                default:
                    oxyColor = OxyColors.Black;
                    markerType = MarkerType.None;
                    lineStyle = LineStyle.Solid;
                    break;
            }


            var Points = new List<DataPoint>();
            var x = 0;
            double y = 0;
            while (x <= result.GetMaxLatency() && (y = result.GetAverageFractionOfDiscoveryAtLatency(x)) < 1)
            {
                Points.Add(new DataPoint(x, y));
                x++;
            }

            Points.Add(new DataPoint(x, y));
            //var serie = new FunctionSeries(result.GetAverageFractionOfDiscoveryAtLatency, 1, max, max, type.ToString());
            //model.Series.Add(serie);

            var lineserie = new LineSeries
            {
                Title = type.ToString(),
                ItemsSource = Points,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = lineStyle,
                Color = oxyColor,
                MarkerType = markerType
            };

            lock (Model)
            {
                Model.Series.Add(lineserie);
                lock (oxyplot)
                {
                    oxyplot.Model = Model;
                    oxyplot.RefreshPlot(true);
                }
            }


        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Series.Count > 0)
            {
                oxyplot.Model.Series.Clear();
                oxyplot.RefreshPlot(true);
            }
        }

        private void tbXAxes_KeyUp(object sender, KeyEventArgs e)
        {
            var s = tbXAxes.Text;
            var xStep = 5;
            if (s.Contains('/'))
            {
                var split = s.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1 && int.TryParse(split[1], out xStep))
                {
                    s = split[0];
                }
                else xStep = 5;
            }

            int x;
            if (int.TryParse(s, out x))
            {
                SetXAxes(x, xStep);
            }
        }

        private void tbYAxes_KeyUp(object sender, KeyEventArgs e)
        {
            var s = tbYAxes.Text;
            var yStep = 5;
            if (s.Contains('/'))
            {
                var split = s.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1 && int.TryParse(split[1], out yStep))
                {
                    s = split[0];
                }
                else yStep = 5;
            }

            double y;
            if (double.TryParse(s, out y))
            {
                SetYAxes(y, yStep);
            }
        }

        private void RunTwoNodesSimulation(BoundedProtocol node1, BoundedProtocol node2, NodeType type)
        {
            var environment = new TwoNodesEnvironmentTmll(node1, node2);
            var testResult = environment.RunSimulation(node1.T);
            var statistics = new StatisticsResult(type);
            statistics.AddStatisticTest(testResult);
            statistics.BuildAverageFractionOfDiscovey(node1.T);
            worker_RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(statistics, null, false));
        }

        private void btTwoNodesSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (RunningInfo.IsRunning)
            {
                var result = MessageBox.Show("Do you want to cancel?", "Cancel", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    CancellAll();
                }
            }
            else
            {
                var selectedAlgs = cbDisco.IsChecked.Value || cbUConnect.IsChecked.Value ||
                                   cbSearchlight.IsChecked.Value || cbBirthday.IsChecked.Value ||
                                   cbStripedSearchlight.IsChecked.Value || cbTestAlgorithm.IsChecked.Value ||
                                   cbGNihao.IsChecked.Value || cbBalancedNihao.IsChecked.Value;

                if (selectedAlgs)
                {
                    ResetAlgorithmProperties();
                    btPlot.Content = "Cancel";
                    btGenerate.IsEnabled = false;
                    testCaseMessage.Text = "Running Algorithm(s)...";
                    testCaseMessage.Visibility = Visibility.Visible;
                    testCaseIcon.Fill = Brushes.Yellow;
                    testCaseIcon.Visibility = Visibility.Visible;

                    if (cbDisco.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new Disco(0, duties[0]);
                            var node2 = new Disco(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.Disco);
                        }
                    }

                    if (cbUConnect.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new UConnect(0, duties[0]);
                            var node2 = new UConnect(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.UConnect);
                        }
                    }

                    if (cbBalancedNihao.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new BalancedNihaoTmll(0, duties[0]);
                            var node2 = new BalancedNihaoTmll(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.BalancedNihao);
                        }
                    }

                    if (cbSearchlight.IsChecked == true)
                    {

                    }

                    if (cbBirthday.IsChecked == true)
                    {

                    }

                    if (cbStripedSearchlight.IsChecked == true)
                    {

                    }

                    if (cbTestAlgorithm.IsChecked == true)
                    {

                    }

                    if (cbGNihao.IsChecked == true)
                    {

                    }
                }
                else
                    MessageBox.Show("No algorithm has been selected!", "Select Algorithms to Run", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }
    }
}
