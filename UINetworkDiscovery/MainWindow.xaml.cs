using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
        private readonly AlgorithmBackgroundWorker _workerTHL2H;
        private readonly AlgorithmBackgroundWorker _workerTHL2HExtended;
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
            _workerGNihao = new AlgorithmBackgroundWorker(NodeType.GNihao);
            _workerGNihao.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerGNihao.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerTHL2H = new AlgorithmBackgroundWorker(NodeType.THL2H);
            _workerTHL2H.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerTHL2H.Worker.ProgressChanged += backgroundWorker_ProgressChanged;
            _workerTHL2HExtended = new AlgorithmBackgroundWorker(NodeType.THL2HExtended);
            _workerTHL2HExtended.Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _workerTHL2HExtended.Worker.ProgressChanged += backgroundWorker_ProgressChanged;

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
            else if (sender.Equals(_workerTHL2H.Worker))
            {
                progressBarTHL2H.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerGNihao.Worker))
            {
                progressBarBalancedNihao.Value = e.ProgressPercentage;
            }
            else if (sender.Equals(_workerTHL2HExtended.Worker))
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
                    PlotFractionOfDiscoveries(results);
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
            cbTHL2H.IsChecked = false;
            cbBalancedNihao.IsChecked = true;
            cbAccGreedyBNihao.IsChecked = false;
            //duty cycle
            cb1p.IsChecked = false;
            cb5p.IsChecked = true;
            cb10p.IsChecked = false;
            tbFixAsymmetricNetwork.Text = "0";
            //generate settings
            tbNumberOfTestCases.Text = "100";
            tbnetworkSize.Text = "400";
            tbMinCommRange.Text = "20";
            tbMaxCommRange.Text = "50";
            tbPosRange.Text = "100";
            tbStartUpLimit.Text = "4000";
            //COR
            CorValue.Value = 20;
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
            _workerGNihao.CancelAsync();
            _workerTHL2H.CancelAsync();
            _workerTHL2HExtended.CancelAsync();
            
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
                                       cbTHL2H.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||
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
                        DeviceData.COR = (int)CorValue.Value;

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

                        if (cbTHL2H.IsChecked == true)
                        {
                            _workerTHL2H.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbBalancedNihao.IsChecked == true)
                        {
                            _workerGNihao.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
                        }

                        if (cbAccGreedyBNihao.IsChecked == true)
                        {
                            _workerTHL2HExtended.RunWorkerAsync(TestCasesGenerator.LoadTestSuite(_fileName));
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

        private OxyColor GetColorByNodeType(NodeType type)
        {
            switch (type)
            {
                case NodeType.Birthday:
                    if (ModelContainsAlgorithm(NodeType.Birthday))
                        return OxyColors.DarkGray;
                    return OxyColors.Gray;
                case NodeType.Disco:
                    if (ModelContainsAlgorithm(NodeType.Disco))
                        return OxyColors.DeepPink;
                    return OxyColors.Magenta;
                case NodeType.UConnect:
                    if (ModelContainsAlgorithm(NodeType.UConnect))
                        return OxyColors.DarkGreen;
                    return OxyColors.Green;
                case NodeType.Searchlight:
                    if (ModelContainsAlgorithm(NodeType.Searchlight))
                        return OxyColors.DarkOrange;
                    return OxyColors.Orange;
                case NodeType.StripedSearchlight:
                    break;
                case NodeType.TestAlgorithm:
                    break;
                case NodeType.GNihao:
                    if (ModelContainsAlgorithm(NodeType.GNihao))
                        return OxyColors.DarkGray;
                    return OxyColors.Black;
                case NodeType.THL2H:
                    if (ModelContainsAlgorithm(NodeType.THL2H))
                        return OxyColors.DarkRed;
                    return OxyColors.Red;
                case NodeType.THL2HExtended:
                    if (ModelContainsAlgorithm(NodeType.THL2HExtended))
                        return OxyColors.DarkBlue;
                    return OxyColors.Blue;
                //default:
                    //return OxyColors.Black;
            }
            return OxyColors.Black;
        }

        private void SetInfoByNodeType(NodeType type, StatisticsResult result)
        {
            switch (type)
            {
                case NodeType.Birthday:
                    break;
                case NodeType.Disco:
                    break;
                case NodeType.UConnect:
                    uconnectAvg.Text = result.AverageDiscoveryLatency.ToString("f2");
                    break;
                case NodeType.Searchlight:
                    break;
                case NodeType.StripedSearchlight:
                    break;
                case NodeType.TestAlgorithm:
                    break;
                case NodeType.GNihao:
                    balanceNihaoAvg.Text = result.AverageDiscoveryLatency.ToString("f2");
                    BalancedNihaoCnt.Text = result.AvgTransmissionsSentPerPeriod.ToString("f2");
                    break;
                case NodeType.THL2H:
                    THL2HAvg.Text = result.AverageDiscoveryLatency.ToString("f2");
                    THL2HCnt.Text = result.AvgTransmissionsSentPerPeriod.ToString("f2");
                    break;
                case NodeType.THL2HExtended:
                    AccGreedyBNihaoAvg.Text = result.AverageDiscoveryLatency.ToString("f2");
                    AccGreedyBNihaoCnt.Text = result.AvgTransmissionsSentPerPeriod.ToString("f2");
                    break;
                default:
                    throw new NotImplementedException("Not showing AvgDiscoveryLatency for NodeType " + type);
            }
        }

        private void PlotFractionOfDiscoveries(StatisticsResult result)
        {
            var max = result.GetMaxLatency();
            avgNoNeighbors.Text =  Math.Round(result.AvgNoNeighbors,2).ToString();
            
            SetInfoByNodeType(result.NodeType, result);

            var points = new List<DataPoint>();
            double x = 0;
            double y = 0;
            while (x <= result.GetMaxLatency() && (y = result.GetAverageFractionOfDiscoveryAtLatency(x)) < 1)
            {
                points.Add(new DataPoint(x, y));
                x++;
            }
            points.Add(new DataPoint(x, y));

            var lineserie = new LineSeries
            {
                Title = result.NodeType.ToString(),
                ItemsSource = points,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = 3,
                MarkerSize = 0,
                LineStyle = LineStyle.Dot,
                Color = GetColorByNodeType(result.NodeType),
                MarkerType = MarkerType.Circle
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

        private void PlotAvgDiscoveryLatency()
        {
            var model = new PlotModel
            {
                Title = "",//title here
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0//no border
            };
            var s1 = new ColumnSeries { Title = "G-Nihao", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Black};
            s1.Items.Add(new ColumnItem { Value = double.Parse(balanceNihaoAvg.Text), Color = OxyColors.Black});

            var s2 = new ColumnSeries { Title = "THL2H", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Red };
            s2.Items.Add(new ColumnItem { Value = double.Parse(THL2HAvg.Text), Color = OxyColors.Red });

            var s3 = new ColumnSeries { Title = "THL2H Extended", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Blue };
            s3.Items.Add(new ColumnItem { Value = double.Parse(AccGreedyBNihaoAvg.Text), Color = OxyColors.Blue });

            var categoryAxis = new CategoryAxis();
            categoryAxis.Labels.Add("Avg. Discovery Latency");
            var valueAxis = new LinearAxis { MinimumPadding = 0, MaximumPadding = 0.25, AbsoluteMinimum = 0 };
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(s1);
            model.Series.Add(s2);
            model.Series.Add(s3);

            oxyplot.Model = model;
            oxyplot.RefreshPlot(true);
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
                                   cbTHL2H.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||cbAccGreedyBNihao.IsChecked.Value;

                if (selectedAlgs)
                {
                    ResetAlgorithmProperties();
                    btPlot.Content = "Cancel";
                    btGenerate.IsEnabled = false;
                    testCaseMessage.Text = "Running Algorithm(s)...";
                    testCaseMessage.Visibility = Visibility.Visible;
                    testCaseIcon.Fill = Brushes.Yellow;
                    testCaseIcon.Visibility = Visibility.Visible;
                    int COR = (int) CorValue.Value;

                    if (cbBirthday.IsChecked == true)
                    {

                    }

                    if (cbSearchlight.IsChecked == true)
                    {

                    }

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
                            var node1 = new GNihao(0, duties[0], COR);
                            var node2 = new GNihao(1, duties[duties.Length - 1], COR);
                            RunTwoNodesSimulation(node1, node2, NodeType.GNihao);
                        }
                    }

                    if (cbTHL2H.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new THL2H(0, duties[0], COR);
                            var node2 = new THL2H(1, duties[duties.Length - 1], COR);
                            RunTwoNodesSimulation(node1, node2, NodeType.THL2H);
                        }
                    }

                    if (cbAccGreedyBNihao.IsChecked == true)
                    {
                        double[] duties;
                        if (GetDutyCycle(out duties))
                        {
                            var node1 = new THL2HExtended(0, duties[0], COR);
                            var node2 = new THL2HExtended(1, duties[duties.Length - 1], COR);
                            RunTwoNodesSimulation(node1, node2, NodeType.THL2HExtended);
                        }
                    }
                }
                else
                    MessageBox.Show("No algorithm has been selected!", "Select Algorithms to Run", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        private void btPlotAvg_Click(object sender, RoutedEventArgs e)
        {
            PlotAvgDiscoveryLatency();
        }
    }
}
