using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
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
        private CancellationTokenSource _cts;
        private Progress<int> _progress;
        private bool _isRunning;
        private InfoBarController _infoBar;



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
            _infoBar = new InfoBarController(infoMessage, infoIcon);
            _cts = new CancellationTokenSource();

            SetDefaultSettings();
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
                    TitleFontSize = 20,
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
                    TitleFontSize = 20,
                    TickStyle = TickStyle.Inside
                };
                if (_model.Axes.Count > 1)
                    _model.Axes[1] = yAxis;
                else _model.Axes.Add(yAxis);
                oxyplot.Model = _model;
                oxyplot.RefreshPlot(true);
            }
        }

        public bool GetDutyCycle(out double[] duties)
        {
            var dutyList = new List<double>();
            if (cb1p.IsChecked ?? false)
                dutyList.Add(1);
            if (cb5p.IsChecked.HasValue && cb5p.IsChecked.Value)
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
                            infoMessage.Text = "Test cases generated";
                            infoMessage.Visibility = Visibility.Visible;
                            infoIcon.Fill = Brushes.Lime;
                            infoIcon.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            infoMessage.Text = "An error ocurred while generating the test cases";
                            infoMessage.Visibility = Visibility.Visible;
                            infoIcon.Fill = Brushes.Red;
                            infoIcon.Visibility = Visibility.Visible;
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

        private async void btPlot_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                var result = MessageBox.Show("Do you want to cancel?", "Cancel", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    btTwoNodesSimulation.IsEnabled = false;
                    _infoBar.ShowMessage("Cancelling...", Brushes.Yellow);
                    _cts.Cancel();
                }
            }
            else
            {
                _isRunning = true;
                var selectedAlgs = cbDisco.IsChecked.Value || cbUConnect.IsChecked.Value ||
                                   cbSearchlight.IsChecked.Value || cbBirthday.IsChecked.Value ||
                                   cbTHL2H.IsChecked.Value || cbBalancedNihao.IsChecked.Value || cbAccGreedyBNihao.IsChecked.Value;

                if (selectedAlgs)
                {
                    if (cbDisco.IsChecked == true)
                    {

                    }
                    if (cbUConnect.IsChecked == true)
                    {

                    }

                    if (cbBalancedNihao.IsChecked == true)
                    {

                    }
                    if (cbTHL2H.IsChecked == true)
                    {
                     
                    }
                    if (cbAccGreedyBNihao.IsChecked == true)
                    {
                        
                    }
                }
                else
                    MessageBox.Show("No algorithm has been selected!", "Select Algorithms to Run", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        private bool ModelContainsAlgorithm(NodeType type)
        {
            var v = _model.Series.Where(s => s.Title.Contains(type.ToString()));
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
            avgNoNeighbors.Text =  result.AvgNoNeighbors.ToString("f2") +"(" + result.AvgNoNeihborsPerSlot.ToString("f4") +"/slot)";
            
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

            //string dcs = result.GetDutyCyclesUsed().First() + "%";
            //foreach (var dc in result.GetDutyCyclesUsed().Skip(1))
            //{
            //    dcs += "," + dc + "%";
            //}

            var lineserie = new LineSeries
            {
                Title = result.NodeType.ToString() + " (DC: "   + "5%)"  ,
                FontSize = 20,
                ItemsSource = points,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = 4,
                MarkerSize = 0,
                LineStyle = LineStyle.Dot,
                Color = GetColorByNodeType(result.NodeType),
                MarkerType = MarkerType.Circle
            };

            lock (_model)
            {
                lineserie.FontSize = 20;
                _model.Series.Add(lineserie);
                _model.LegendPlacement = LegendPlacement.Outside;
                _model.LegendFontSize = 20;
                _model.TitleFontSize = 20;
                _model.SubtitleFontSize = 20;
                _model.DefaultFontSize = 20;
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
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0,//no border
                LegendFontSize = 20,
                TitleFontSize = 20,
                SubtitleFontSize = 20,
                DefaultFontSize = 20,
        };
            var s1 = new ColumnSeries { Title = "G-Nihao", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Black};
            s1.Items.Add(new ColumnItem { Value = double.Parse(balanceNihaoAvg.Text), Color = OxyColors.Black});

            var s2 = new ColumnSeries { Title = "THL2H", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Red };
            s2.Items.Add(new ColumnItem { Value = double.Parse(THL2HAvg.Text), Color = OxyColors.Red });

            var s3 = new ColumnSeries { Title = "THL2H Extended", StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Blue };
            s3.Items.Add(new ColumnItem { Value = double.Parse(AccGreedyBNihaoAvg.Text), Color = OxyColors.Blue });

            var categoryAxis = new CategoryAxis();
            categoryAxis.Labels.Add("Avg. Discovery Latency");
            var valueAxis = new LinearAxis { MinimumPadding = 0, MaximumPadding = 0.25, AbsoluteMinimum = 0};
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
            ClearChart();
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

        private async void btTwoNodesSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                btTwoNodesSimulation.IsEnabled = false;
                _infoBar.ShowMessage("Cancelling...", Brushes.Yellow);
                _cts.Cancel();
            }
            else
            {
                _isRunning = true;
                
                var selectedAlgs = cbDisco.IsChecked.Value || cbUConnect.IsChecked.Value ||
                                   cbSearchlight.IsChecked.Value || cbBirthday.IsChecked.Value ||
                                   cbTHL2H.IsChecked.Value || cbBalancedNihao.IsChecked.Value ||cbAccGreedyBNihao.IsChecked.Value;
                if (selectedAlgs)
                {
                    foreach (var protocolType in GetProtocolsToRun())
                    {
                        UpdateViewToRunningAlgorithm(protocolType);
                        try
                        {
                            _cts = new CancellationTokenSource();
                            _progress = new Progress<int>();
                            _progress.ProgressChanged += ReportProgress;
                            await RunTwoNodesSimulation(protocolType, _cts, _progress);
                        }
                        catch (OperationCanceledException)
                        {
                            UpdateViewToOperationCanceled();
                            ClearChart();
                            return;
                        }
                    }

                    UpdateViewToAllDone();
                }
                else
                    MessageBox.Show("No algorithm has been selected!", "Select Algorithms to Run", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        public List<NodeType> GetProtocolsToRun()
        {
            var protocolsToRun = new List<NodeType>();

            if (cbDisco.IsChecked == true)
            {
                protocolsToRun.Add(NodeType.Disco);
            }
            if (cbUConnect.IsChecked == true)
            {
                protocolsToRun.Add(NodeType.UConnect);
            }
            if (cbBalancedNihao.IsChecked == true)
            {
                protocolsToRun.Add(NodeType.GNihao);
            }
            if (cbTHL2H.IsChecked == true)
            {
                protocolsToRun.Add(NodeType.THL2H);
            }
            if (cbAccGreedyBNihao.IsChecked == true)
            {
                protocolsToRun.Add(NodeType.THL2HExtended);
            }

            return protocolsToRun;
        }

        private async Task RunTwoNodesSimulation(NodeType type, CancellationTokenSource cts, Progress<int> progress)
        {
            int COR = (int)CorValue.Value;
            double[] duties;
            GetDutyCycle(out duties);
            var node1 = NodeFactory.CreateNew(type,0, (int)duties[0], COR);
            var node2 = NodeFactory.CreateNew(type, 1, (int)duties[duties.Length - 1], COR);
            var environment = new PairwiseEnvironmentTmll();
            var testResult = await environment.RunPairwiseSimulation(node1, node2, Math.Max(node1.T, node2.T), _cts.Token, _progress);
            
            var statisticsResult = new StatisticsResult(type);
            statisticsResult.AddStatisticTest(testResult);
            statisticsResult.BuildAverageFractionOfDiscovey(2 * node1.T);

            PlotFractionOfDiscoveries(statisticsResult);
        }

        private void ReportProgress(object sender, int e)
        {
            progressBar.Value = e;
        }

        private void btPlotAvg_Click(object sender, RoutedEventArgs e)
        {
            PlotAvgDiscoveryLatency();
        }

        private void UpdateViewToOperationCanceled()
        {
            btTwoNodesSimulation.Content = "1 vs 1";
            btTwoNodesSimulation.IsEnabled = true;
            btGenerate.IsEnabled = true;
            _infoBar.ShowMessage("Operation was cancelled.", Brushes.Red);
            _isRunning = false;
        }

        private void UpdateViewToAllDone()
        {
            btTwoNodesSimulation.Content = "1 vs 1";
            btGenerate.IsEnabled = true;
            _infoBar.ShowMessage("All done.", Brushes.Lime);
            _isRunning = false;
        }

        private void UpdateViewToRunningAlgorithm(NodeType type)
        {
            btTwoNodesSimulation.Content = "Cancel";
            btGenerate.IsEnabled = false;
            _infoBar.ShowMessage($"Running {type.ToString()}...", Brushes.Yellow);
        }

        private void ClearChart()
        {
            if (_model.Series.Count > 0)
            {
                oxyplot.Model.Series.Clear();
                oxyplot.RefreshPlot(true);
            }

            THL2HAvg.Text = "";
            AccGreedyBNihaoAvg.Text = "";
            balanceNihaoAvg.Text = "";
            uconnectAvg.Text = "";
            discoAvg.Text = "";
        }

    }
}
