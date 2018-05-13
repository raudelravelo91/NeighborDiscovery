using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using NeighborDiscovery.DataGeneration;
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
        
        private readonly string _fileName;
        private readonly PlotModel _model;
        private readonly AlgorithmBackgroundWorker _workerDisco;
        private readonly AlgorithmBackgroundWorker _workerUConnect;
        private readonly AlgorithmBackgroundWorker _workerSearchLight;
        private readonly AlgorithmBackgroundWorker _workerBirthday;
        private readonly AlgorithmBackgroundWorker _workerStripedSearchlight;
        private readonly AlgorithmBackgroundWorker _workerTestAlgorithm;
        private readonly AlgorithmBackgroundWorker _workerGNihao;
        private readonly AlgorithmBackgroundWorker _workerBalancedNihao;
        private readonly AlgorithmBackgroundWorker _workerAccBalancedBNihao;
        private List<int> _threads = new List<int>();

        public MainWindow()
        {
            InitializeComponent();

            _fileName = "testcases.txt";
            _model = new PlotModel();
            SetXAxes(400, 8);
            SetYAxes(1, 10);
            var currentMargins = oxyplot.PlotMargins;
            _model.PlotMargins = new OxyThickness(currentMargins.Left, btClear.Height, currentMargins.Right,
                currentMargins.Bottom);
            oxyplot.PlotMargins = new System.Windows.Thickness(currentMargins.Left, btClear.Height,
                currentMargins.Right, currentMargins.Bottom);
            RunningInfo = new RunningInfo();
            
            SetDefaultSettings();

            _workerDisco = new AlgorithmBackgroundWorker(NodeType.Disco);
            _workerDisco.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerDisco.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerUConnect = new AlgorithmBackgroundWorker(NodeType.UConnect);
            _workerUConnect.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerUConnect.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerSearchLight = new AlgorithmBackgroundWorker(NodeType.Searchlight);
            _workerSearchLight.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerSearchLight.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerBirthday = new AlgorithmBackgroundWorker(NodeType.Birthday);
            _workerBirthday.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerBirthday.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerStripedSearchlight = new AlgorithmBackgroundWorker(NodeType.StripedSearchlight);
            _workerStripedSearchlight.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerStripedSearchlight.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerTestAlgorithm = new AlgorithmBackgroundWorker(NodeType.TestAlgorithm);
            _workerTestAlgorithm.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerTestAlgorithm.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerGNihao = new AlgorithmBackgroundWorker(NodeType.THL2H);
            _workerGNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerGNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerBalancedNihao = new AlgorithmBackgroundWorker(NodeType.GNihao);
            _workerBalancedNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerBalancedNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerAccBalancedBNihao = new AlgorithmBackgroundWorker(NodeType.THL2HExtended);
            _workerAccBalancedBNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerAccBalancedBNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;

        }

        public void SetXAxes(double x, double xMajorStep)
        {
            if (x / xMajorStep > 0)
            {
                var xAxis = new LinearAxis(AxisPosition.Bottom, 0, x, "Discovery latency (slots)")
                {
                    MajorStep = x / xMajorStep,
                    MajorGridlineThickness = 1,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorTickSize = 0,
                    TitleFontSize = 14,
                    TickStyle = TickStyle.Inside
                };
                if (_model.Axes.Count > 0)
                    _model.Axes[0] = xAxis;
                else _model.Axes.Add(xAxis);

                oxyplot.Model = _model;
                oxyplot.RefreshPlot(true);
            }
        }

        public void SetYAxes(double y, double yMajorStep)
        {
            if (y / yMajorStep > 0)
            {
                var yAxis = new LinearAxis(AxisPosition.Left, 0, y, "Fraction of Discoveries")
                {
                    MajorStep = y / yMajorStep,
                    MajorGridlineThickness = 1,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorTickSize = 0,
                    TitleFontSize = 14,
                    TickStyle = TickStyle.Inside
                };
                if (_model.Axes.Count > 1)
                    _model.Axes[1] = yAxis;
                else _model.Axes.Add(yAxis);
                oxyplot.Model = _model;
                oxyplot.RefreshPlot(true);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (((BackgroundWorker)sender).CancellationPending)
            {
                return;
            }
            
            if (sender.Equals(_workerDisco.Worker))
            {
                progressBarDisco.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerUConnect.Worker))
            {
                progressBarUConnect.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerSearchLight.Worker))
            {
                progressBarSearchlight.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerBirthday.Worker))
            {
                progressBarBirthday.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerStripedSearchlight.Worker))
            {
                //progressBarStripedSearchlight.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerTestAlgorithm.Worker))
            {
                //progressBarTestAlg.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerGNihao.Worker))
            {
                progressBarGNihao.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerBalancedNihao.Worker))
            {
                progressBarBalancedNihao.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerAccBalancedBNihao.Worker))
            {
                progressBarAccGreedyBNihao.Value = e.ProgressPercentage;
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
                else throw new ArgumentException("Invalid result");

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
                    RunningInfo.CancelationPending = false;
                    btGenerate.IsEnabled = true;
                    btPlot.Content = "Run";
                    testCaseIcon.Fill = Brushes.Red;
                    testCaseMessage.Text = "Cancelled by User.";
                }
            }
        }

        public bool GetDutyCycle(out double[] duties)
        {
            var dutyList = new List<double>();
            if (cb1p.IsChecked != null && cb1p.IsChecked == true)
                dutyList.Add(1);
            if (cb5p.IsChecked != null && cb5p.IsChecked == true)
                dutyList.Add(5);
            if (cb10p.IsChecked != null && cb10p.IsChecked == true)
                dutyList.Add(10);
            duties = dutyList.ToArray();
            return duties.Length > 0;
        }

        public bool GetTestSettings(out int numberOfTests, out int startUpLimit, out int posRange, out int networkSize,
            out int minCRange, out int maxCRange)
        {
            numberOfTests = startUpLimit = posRange = networkSize = minCRange = maxCRange = 0;
            return int.TryParse(tbNumberOfTestCases.Text, out numberOfTests) &&
                   int.TryParse(tbStartUpLimit.Text, out startUpLimit) &&
                   int.TryParse(tbPosRange.Text, out posRange) &&
                   int.TryParse(tbMinCommRange.Text, out minCRange) &&
                   int.TryParse(tbMaxCommRange.Text, out maxCRange) &&
                   int.TryParse(tbnetworkSize.Text, out networkSize);
        }

        private void SetDefaultSettings()
        {
            //algorithms
            cbDisco.IsChecked = false;
            cbUConnect.IsChecked = false;
            cbSearchlight.IsChecked = false;
            cbBirthday.IsChecked = false;
            //cbStripedSearchlight.IsChecked = false;
            //cbTestAlgorithm.IsChecked = false;
            cbGNihao.IsChecked = false;
            cbBalancedNihao.IsChecked = true;
            cbAccGreedyBNihao.IsChecked = false;
            //duty cycle
            cb1p.IsChecked = false;
            cb5p.IsChecked = true;
            cb10p.IsChecked = false;
            tbFixAsymmetricNetwork.Text = "0";
            //generate settings
            tbNumberOfTestCases.Text = "100";
            tbnetworkSize.Text = "40";
            tbMinCommRange.Text = "20";
            tbMaxCommRange.Text = "40";
            tbPosRange.Text = "100";
            tbStartUpLimit.Text = "400";
        }

        private void btGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!RunningInfo.IsRunning)
            {
                int numberOfTests, startUpLimit, posRange, networkSize, minCRange, maxCRange;
                if (GetTestSettings(out numberOfTests, out startUpLimit, out posRange, out networkSize, out minCRange,
                    out maxCRange))
                {
                    double[] duties;
                    if (GetDutyCycle(out duties))
                    {
                        var suite = TestCasesGenerator.GenerateTestSuite(numberOfTests, networkSize, startUpLimit, posRange,
                            minCRange, maxCRange, duties);
                        if (TestCasesGenerator.SaveTestSuite(_fileName, suite))
                        {
                            testCaseMessage.Text = "Test cases generated";
                            testCaseMessage.Visibility = Visibility.Visible;
                            testCaseIcon.Fill = Brushes.Lime;
                            testCaseIcon.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            testCaseMessage.Text = "An error ocurred while generating the test cases";
                            testCaseMessage.Visibility = Visibility.Visible;
                            testCaseIcon.Fill = Brushes.Red;
                            testCaseIcon.Visibility = Visibility.Visible;
                        }
                    }
                    else
                        MessageBox.Show("At least 1 Duty Cycle must be checked", "Duty Cycle Info", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Invalid Tests Setting Parameters (Ex: check all are numbers)",
                        "Tests Settings Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancellAll()
        {
            _workerDisco.CancelAsync();
            _workerUConnect.CancelAsync();
            _workerSearchLight.CancelAsync();
            _workerBirthday.CancelAsync();
            _workerStripedSearchlight.CancelAsync();
            _workerTestAlgorithm.CancelAsync();
            _workerBalancedNihao.CancelAsync();
            _workerGNihao.CancelAsync();
            _workerAccBalancedBNihao.CancelAsync();
            
        }

        private void ResetAlgorithmProperties()
        {
            //progressBarDisco.Value = 0;
            //discoAvg.Text = "";
            //discoCnt.Text = "";

            //progressBarUConnect.Value = 0;
            //uconnectAvg.Text = "";
            //uconnectCnt.Text = "";

            //progressBarBalancedNihao.Value = 0;
            //uconnectAvg.Text = "";
            //uconnectCnt.Text = "";

            //progressBarBalancedNihao.Value = 0;
            //uconnectAvg.Text = "";
            //uconnectCnt.Text = "";

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
                if (File.Exists(_fileName))
                {
                    var selectedAlgs = cbDisco.IsChecked.Value || cbUConnect.IsChecked.Value ||
                                       cbSearchlight.IsChecked.Value || cbBirthday.IsChecked.Value ||
                                       //cbStripedSearchlight.IsChecked.Value || cbTestAlgorithm.IsChecked.Value ||
                                       cbGNihao.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||
                                       cbAccGreedyBNihao.IsChecked.Value;

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
                            _workerDisco.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbUConnect.IsChecked == true)
                        {
                            _workerUConnect.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbSearchlight.IsChecked == true)
                        {
                            _workerSearchLight.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbBirthday.IsChecked == true)
                        {
                            _workerBirthday.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        //if (cbStripedSearchlight.IsChecked == true)
                        //{
                        //    _workerStripedSearchlight.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        //}

                        //if (cbTestAlgorithm.IsChecked == true)
                        //{
                        //    _workerTestAlgorithm.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        //}

                        if (cbGNihao.IsChecked == true)
                        {
                            _workerGNihao.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbBalancedNihao.IsChecked == true)
                        {
                            _workerBalancedNihao.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbAccGreedyBNihao.IsChecked == true)
                        {
                            _workerAccBalancedBNihao.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
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
            var v = _model.Series.Where(s => s.Title.Equals(type.ToString()));
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
            avgNoNeighbors.Text =  Math.Round(result.AvgNoNeighbors,2).ToString();

            switch (type)
            {
                case NodeType.Birthday:
                    markerType = MarkerType.None;
                    oxyColor = OxyColors.Gray;
                    if (ModelContainsAlgorithm(NodeType.Birthday))
                    {
                        oxyColor = OxyColors.DarkGray;
                    }

                    lineStyle = LineStyle.Solid;
                    birthdayAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.Disco:
                    markerType = MarkerType.Triangle;
                    oxyColor = OxyColors.Magenta;
                    if (ModelContainsAlgorithm(NodeType.Disco))
                    {
                        oxyColor = OxyColors.DeepPink;
                    }

                    lineStyle = LineStyle.Dash;
                    discoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.UConnect:
                    markerType = MarkerType.Square;
                    oxyColor = OxyColors.Blue;
                    if (ModelContainsAlgorithm(NodeType.UConnect))
                    {
                        oxyColor = OxyColors.DarkBlue;
                    }

                    lineStyle = LineStyle.Dot;
                    uconnectAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.Searchlight:
                    markerType = MarkerType.Star;
                    oxyColor = OxyColors.LimeGreen;
                    if (ModelContainsAlgorithm(NodeType.Searchlight))
                    {
                        oxyColor = OxyColors.Green;
                    }

                    lineStyle = LineStyle.DashDot;
                    searchlightAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.StripedSearchlight:
                    markerType = MarkerType.Star;
                    oxyColor = OxyColors.Blue;
                    if (ModelContainsAlgorithm(NodeType.StripedSearchlight))
                    {
                        oxyColor = OxyColors.DarkBlue;
                    }

                    lineStyle = LineStyle.LongDashDotDot;
                    //stripedSearchlighAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.TestAlgorithm:
                    markerType = MarkerType.Cross;
                    oxyColor = OxyColors.Pink;
                    if (ModelContainsAlgorithm(NodeType.TestAlgorithm))
                    {
                        oxyColor = OxyColors.Magenta;
                    }

                    lineStyle = LineStyle.LongDashDotDot;
                    //testAlgAvg.Text = result.AverageDiscoveryLatency.ToString();
                    break;
                case NodeType.GNihao:
                    markerType = MarkerType.Square;
                    oxyColor = OxyColors.Black;
                    if (ModelContainsAlgorithm(NodeType.GNihao))
                    {
                        oxyColor = OxyColors.DarkGray;
                    }

                    lineStyle = LineStyle.DashDashDot;
                    balanceNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    BalancedNihaoCnt.Text = Math.Round(result.AvgTransmissionsSentPerPeriod, 2).ToString();
                    break;
                case NodeType.THL2H:
                    markerType = MarkerType.Cross;
                    oxyColor = OxyColors.Red;
                    if (ModelContainsAlgorithm(NodeType.THL2H))
                    {
                        oxyColor = OxyColors.DarkRed;
                    }

                    lineStyle = LineStyle.LongDashDot;
                    gNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    gNihaoCnt.Text = Math.Round(result.AvgTransmissionsSentPerPeriod, 2).ToString();
                    break;
                case NodeType.THL2HExtended:
                    markerType = MarkerType.Circle;
                    oxyColor = OxyColors.Blue;
                    if (ModelContainsAlgorithm(NodeType.THL2HExtended))
                    {
                        oxyColor = OxyColors.DarkBlue;
                    }

                    lineStyle = LineStyle.Dot;
                    AccGreedyBNihaoAvg.Text = result.AverageDiscoveryLatency.ToString();
                    AccGreedyBNihaoCnt.Text = Math.Round(result.AvgTransmissionsSentPerPeriod, 2).ToString();
                    break;
                default:
                    oxyColor = OxyColors.Black;
                    markerType = MarkerType.None;
                    lineStyle = LineStyle.Solid;
                    break;
            }


            var points = new List<DataPoint>();
            var x = 0;
            double y = 0;
            while (x <= result.GetMaxLatency() && (y = result.GetAverageFractionOfDiscoveryAtLatency(x)) < 1)
            {
                points.Add(new DataPoint(x, y));
                x++;
            }

            points.Add(new DataPoint(x, y));
            //var serie = new FunctionSeries(result.GetAverageFractionOfDiscoveryAtLatency, 1, max, max, type.ToString());
            //model.Series.Add(serie);

            var lineserie = new LineSeries
            {
                Title = type.ToString(),
                ItemsSource = points,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = lineStyle,
                Color = oxyColor,
                MarkerType = markerType
            };

            lock (_model)
            {
                _model.Series.Add(lineserie);
                lock (oxyplot)
                {
                    oxyplot.Model = _model;
                    oxyplot.RefreshPlot(true);
                }
            }


        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            if (_model.Series.Count > 0)
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
            var testResult = environment.RunSimulation();
            var statisticsResult = new StatisticsResult(type);
            statisticsResult.AddStatisticTest(testResult);
            statisticsResult.BuildAverageFractionOfDiscovey(2*node1.Bound);
            worker_RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(statisticsResult, null, false));
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
                                   //cbStripedSearchlight.IsChecked.Value || cbTestAlgorithm.IsChecked.Value ||
                                   cbGNihao.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||cbAccGreedyBNihao.IsChecked.Value;

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
                            var node1 = new BalancedNihao(0, duties[0]);
                            var node2 = new BalancedNihao(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.GNihao);
                        }
                    }

                    if (cbGNihao.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new AccBalancedNihao(0, duties[0]);
                            var node2 = new AccBalancedNihao(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.THL2H);
                        }
                    }

                    if (cbAccGreedyBNihao.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new AccBalancedNihaoExtended(0, duties[0]);
                            var node2 = new AccBalancedNihaoExtended(1, duties[duties.Length - 1]);
                            RunTwoNodesSimulation(node1, node2, NodeType.THL2HExtended);
                        }
                    }

                    if (cbSearchlight.IsChecked == true)
                    {

                    }

                    if (cbBirthday.IsChecked == true)
                    {

                    }

                    //if (cbStripedSearchlight.IsChecked == true)
                    //{

                    //}

                    //if (cbTestAlgorithm.IsChecked == true)
                    //{

                    //}

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
