using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class HallMeasurementResultsForm : Form
    {
        private ToolTip chartToolTip = new ToolTip();
        private readonly string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
        private readonly string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";
        private const int NumberOfHallPositions = 4;
        private Dictionary<string, Chart> measurementCharts = new Dictionary<string, Chart>();


        public HallMeasurementResultsForm()
        {
            InitializeComponent();
            RichTextBoxSettings();
            LoadMeasurementResults();
            InitializeMeasurementCharts();
            UpdateButtonsUI();

            CollectAndCalculateHallMeasured.Instance.AllHallDataUpdated += OnAllHallDataUpdatedHandler;
            Debug.WriteLine("[DEBUG - HallResultsForm] Subscribed to AllHallDataUpdated event.");

            // *** ดึงข้อมูลและอัปเดตกราฟเมื่อ Form ถูกสร้าง ***
            var lastData = CollectAndCalculateHallMeasured.Instance.GetLastMeasurements();
            if (lastData != null)
            {
                Debug.WriteLine("[DEBUG - HallResultsForm] Last measurement data found, updating chart.");
                UpdateMeasurementCharts(lastData);
            }
            CollectAndCalculateHallMeasured.Instance.HallVoltageCalculated += OnHallVoltageCalculatedHandler;
            CollectAndCalculateHallMeasured.Instance.HallPropertiesCalculated += OnHallPropertiesCalculatedHandler;
            ChartHallVoltageResults.MouseMove += ChartHallVoltageResults_MouseMove;
        }

        private void InitializeMeasurementCharts()
        {
            Debug.WriteLine("[DEBUG] Starting InitializeMeasurementCharts");
            measurementCharts.Clear();

            // ค้นหา ChartHallVoltageResults โดยตรง
            ChartHallVoltageResults = Controls.Find("ChartHallVoltageResults", true).FirstOrDefault() as Chart;
            if (ChartHallVoltageResults != null)
            {
                measurementCharts["HallVoltageResults"] = ChartHallVoltageResults;
                Debug.WriteLine($"[DEBUG] Found single chart '{ChartHallVoltageResults.Name}'");
            }
            else
            {
                Debug.WriteLine("[DEBUG] Warning: ChartHallVoltageResults not found!");
            }
            Debug.WriteLine("[DEBUG] Ending InitializeMeasurementCharts");
        }

        private string GetStatePrefix(HallMeasurementState state)
        {
            switch (state)
            {
                case HallMeasurementState.NoMagneticField:
                    return "Out";
                case HallMeasurementState.OutwardOrSouthMagneticField:
                    return "South";
                case HallMeasurementState.InwardOrNorthMagneticField:
                    return "North";
                default:
                    return state.ToString();
            }
        }

        private void RichTextBoxSettings()
        {
            RichTextboxHallOutPos1.Text = "VOut1 :";
            RichTextboxHallOutPos2.Text = "VOut2 :";
            RichTextboxHallOutPos3.Text = "VOut3 :";
            RichTextboxHallOutPos4.Text = "VOut4 :";

            RichTextboxHallInSouthPos1.Text = "VSouth1 :";
            RichTextboxHallInSouthPos2.Text = "VSouth2 :";
            RichTextboxHallInSouthPos3.Text = "VSouth3 :";
            RichTextboxHallInSouthPos4.Text = "VSouth4 :";
            RichTextboxHallInNorthPos1.Text = "VNorth1 :";
            RichTextboxHallInNorthPos2.Text = "VNorth2 :";
            RichTextboxHallInNorthPos3.Text = "VNorth3 :";
            RichTextboxHallInNorthPos4.Text = "VNorth4 :";

            RichTextboxHallVoltage.Text = "VH :";
            RichTextboxHallRes.Text = "RHall :";
            RichTextboxHallCoefficient.Text = "RH :";
            RichTextboxSheetConcentration.Text = "nSheet :";
            RichTextboxBulkConcentration.Text = "nBulk :";
            RichTextboxMobility.Text = "µ :";

            RichTextboxHallCoefficientUnit.Text = "m3 / C";
            RichTextboxSheetConcentrationUnit.Text = "m-2";
            RichTextboxBulkConcentrationUnit.Text = "m-3";
            RichTextboxMobilityUnit.Text = "m2 / V⋅s";

            RichTextboxHallCoefficientUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxSheetConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxBulkConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxMobilityUnit.SelectionAlignment = HorizontalAlignment.Center;

            RichTextboxHallOutPos1.Location = new Point(120, 40);
            RichTextboxHallOutPos2.Location = new Point(120, 75);
            RichTextboxHallOutPos3.Location = new Point(120, 110);
            RichTextboxHallOutPos4.Location = new Point(120, 145);

            RichTextboxHallInSouthPos1.Location = new Point(110, 215);
            RichTextboxHallInSouthPos2.Location = new Point(110, 250);
            RichTextboxHallInSouthPos3.Location = new Point(110, 285);
            RichTextboxHallInSouthPos4.Location = new Point(110, 320);
            RichTextboxHallInNorthPos1.Location = new Point(110, 355);
            RichTextboxHallInNorthPos2.Location = new Point(110, 390);
            RichTextboxHallInNorthPos3.Location = new Point(110, 425);
            RichTextboxHallInNorthPos4.Location = new Point(110, 460);

            RichTextboxHallVoltage.Location = new Point(138, 535);
            RichTextboxHallRes.Location = new Point(125, 570);
            RichTextboxHallCoefficient.Location = new Point(138, 605);
            RichTextboxSheetConcentration.Location = new Point(118, 640);
            RichTextboxBulkConcentration.Location = new Point(124, 675);
            RichTextboxMobility.Location = new Point(146, 710);


            RichTextboxHallOutPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallInSouthPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallVoltage.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallRes.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallCoefficient.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxSheetConcentration.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxBulkConcentration.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxMobility.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallOutPos1.Select(1, 4);
            RichTextboxHallOutPos2.Select(1, 4);
            RichTextboxHallOutPos3.Select(1, 4);
            RichTextboxHallOutPos4.Select(1, 4);

            RichTextboxHallInSouthPos1.Select(1, 6);
            RichTextboxHallInSouthPos2.Select(1, 6);
            RichTextboxHallInSouthPos3.Select(1, 6);
            RichTextboxHallInSouthPos4.Select(1, 6);
            RichTextboxHallInNorthPos1.Select(1, 6);
            RichTextboxHallInNorthPos2.Select(1, 6);
            RichTextboxHallInNorthPos3.Select(1, 6);
            RichTextboxHallInNorthPos4.Select(1, 6);

            RichTextboxHallVoltage.Select(1, 1);
            RichTextboxHallRes.Select(1, 4);
            RichTextboxHallCoefficient.Select(1, 1);
            RichTextboxSheetConcentration.Select(1, 5);
            RichTextboxBulkConcentration.Select(1, 4);

            RichTextboxHallCoefficientUnit.Select(1, 1);
            RichTextboxSheetConcentrationUnit.Select(1, 2);
            RichTextboxBulkConcentrationUnit.Select(1, 2);
            RichTextboxMobilityUnit.Select(1, 1);

            RichTextboxHallOutPos1.SelectionCharOffset -= 8;
            RichTextboxHallOutPos2.SelectionCharOffset -= 8;
            RichTextboxHallOutPos3.SelectionCharOffset -= 8;
            RichTextboxHallOutPos4.SelectionCharOffset -= 8;

            RichTextboxHallInSouthPos1.SelectionCharOffset -= 8;
            RichTextboxHallInSouthPos2.SelectionCharOffset -= 8;
            RichTextboxHallInSouthPos3.SelectionCharOffset -= 8;
            RichTextboxHallInSouthPos4.SelectionCharOffset -= 8;
            RichTextboxHallInNorthPos1.SelectionCharOffset -= 8;
            RichTextboxHallInNorthPos2.SelectionCharOffset -= 8;
            RichTextboxHallInNorthPos3.SelectionCharOffset -= 8;
            RichTextboxHallInNorthPos4.SelectionCharOffset -= 8;

            RichTextboxHallVoltage.SelectionCharOffset -= 8;
            RichTextboxHallRes.SelectionCharOffset -= 8;
            RichTextboxHallCoefficient.SelectionCharOffset -= 8;
            RichTextboxSheetConcentration.SelectionCharOffset -= 8;
            RichTextboxBulkConcentration.SelectionCharOffset -= 8;

            RichTextboxHallCoefficientUnit.SelectionCharOffset += 8;
            RichTextboxSheetConcentrationUnit.SelectionCharOffset += 8;
            RichTextboxBulkConcentrationUnit.SelectionCharOffset += 8;
            RichTextboxMobilityUnit.SelectionCharOffset += 8;

            RichTextboxHallOutPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallInSouthPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallVoltage.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallRes.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallCoefficient.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetConcentration.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxBulkConcentration.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallCoefficientUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxBulkConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxMobilityUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void LoadMeasurementResults()
        {
            TextboxSourceMode.Text = GlobalSettings.Instance.SourceModeUI;
            TextboxMeasureMode.Text = GlobalSettings.Instance.MeasureModeUI;

            var avgNoFieldVoltages = CollectAndCalculateHallMeasured.Instance.GetAverageNoFieldVoltagesByPosition();
            var avgSouthVoltages = CollectAndCalculateHallMeasured.Instance.GetAverageSouthFieldVoltagesByPosition();
            var avgNorthVoltages = CollectAndCalculateHallMeasured.Instance.GetAverageNorthFieldVoltagesByPosition();

            void SetText(System.Windows.Forms.TextBox box, int index, Dictionary<int, double> data)
            {
                if (data.TryGetValue(index, out double value))
                    box.Text = value.ToString("E5");
                else
                    box.Text = "N/A";
            }

            // แสดง Hall Out Voltages
            SetText(TextboxHallOut1, 1, avgNoFieldVoltages);
            SetText(TextboxHallOut2, 2, avgNoFieldVoltages);
            SetText(TextboxHallOut3, 3, avgNoFieldVoltages);
            SetText(TextboxHallOut4, 4, avgNoFieldVoltages);

            // แสดงค่าเฉลี่ย Hall In Voltages จาก South Field
            SetText(TextboxHallInSouth1, 1, avgSouthVoltages);
            SetText(TextboxHallInSouth2, 2, avgSouthVoltages);
            SetText(TextboxHallInSouth3, 3, avgSouthVoltages);
            SetText(TextboxHallInSouth4, 4, avgSouthVoltages);

            // แสดงค่าเฉลี่ย Hall In Voltages จาก North Field
            SetText(TextboxHallInNorth1, 1, avgNorthVoltages);
            SetText(TextboxHallInNorth2, 2, avgNorthVoltages);
            SetText(TextboxHallInNorth3, 3, avgNorthVoltages);
            SetText(TextboxHallInNorth4, 4, avgNorthVoltages);

            TextboxHallVoltage.Text = GlobalSettings.Instance.TotalHallVoltage.ToString("E5");
            TextboxHallRes.Text = GlobalSettings.Instance.HallResistance.ToString("E5");
            TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("E5");
            TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E5");
            TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E5");
            TextboxMobility.Text = GlobalSettings.Instance.Mobility.ToString("E5");

            SetUnit("TextboxHallOut1Unit", "V");
            SetUnit("TextboxHallOut2Unit", "V");
            SetUnit("TextboxHallOut3Unit", "V");
            SetUnit("TextboxHallOut4Unit", "V");
            SetUnit("TextboxHallInSouth1Unit", "V");
            SetUnit("TextboxHallInSouth2Unit", "V");
            SetUnit("TextboxHallInSouth3Unit", "V");
            SetUnit("TextboxHallInSouth4Unit", "V");
            SetUnit("TextboxHallInNorth1Unit", "V");
            SetUnit("TextboxHallInNorth2Unit", "V");
            SetUnit("TextboxHallInNorth3Unit", "V");
            SetUnit("TextboxHallInNorth4Unit", "V");

            SetUnit("TextboxHallVoltageUnit", "V");
            SetUnit("TextboxHallResUnit", "Ω");

            UpdateSemiconductorTypeButtons();
        }

        private void SetUnit(string controlName, string unit)
        {
            if (Controls.Find(controlName, true).FirstOrDefault() is TextBox unitBox)
            {
                unitBox.Text = unit;
            }
        }

        private void OnAllHallDataUpdatedHandler(object sender, Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>>>(OnAllHallDataUpdatedHandler), sender, allHallData);
                return;
            }

            Debug.WriteLine("[DEBUG - HallResultsForm] OnAllHallDataUpdatedHandler called.");
            UpdateMeasurementCharts(allHallData);
        }

        private void UpdateMeasurementCharts(Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData)
        {
            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[DEBUG - HallResultsForm] ChartHallVoltageResults is null in UpdateMeasurementCharts!");
                return;
            }

            Debug.WriteLine("[DEBUG - HallResultsForm] UpdateMeasurementCharts called with data. Number of states: {allHallData?.Count}");

            ChartHallVoltageResults.Series.Clear();
            ChartHallVoltageResults.ChartAreas.Clear();
            ChartHallVoltageResults.ChartAreas.Add(new ChartArea("DefaultArea"));
            ChartArea area = ChartHallVoltageResults.ChartAreas["DefaultArea"];

            area.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
            area.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})";
            area.AxisX.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);
            area.AxisY.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);

            area.AxisX.LabelStyle.Format = "0.000";
            area.AxisY.LabelStyle.Format = "E2";

            area.BackColor = Color.Black;
            area.BackSecondaryColor = Color.Silver;
            area.BackGradientStyle = GradientStyle.TopBottom;

            area.AxisX.MinorGrid.Enabled = true;
            area.AxisX.MinorGrid.LineColor = Color.LightGray;
            area.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            ChartHallVoltageResults.Legends.Clear();
            ChartHallVoltageResults.Legends.Add(new Legend("DefaultLegend"));
            ChartHallVoltageResults.Legends["DefaultLegend"].Docking = Docking.Bottom;
            ChartHallVoltageResults.Legends["DefaultLegend"].Alignment = System.Drawing.StringAlignment.Center;

            foreach (var statePair in allHallData)
            {
                HallMeasurementState state = statePair.Key;
                var dataByPosition = statePair.Value;
                string statePrefix = GetStatePrefix(state);
                Color seriesColor = Color.Black;

                switch (state)
                {
                    case HallMeasurementState.NoMagneticField:
                        seriesColor = Color.GreenYellow;
                        break;
                    case HallMeasurementState.OutwardOrSouthMagneticField:
                        seriesColor = Color.OrangeRed;
                        break;
                    case HallMeasurementState.InwardOrNorthMagneticField:
                        seriesColor = Color.DeepSkyBlue;
                        break;
                }

                for (int position = 1; position <= NumberOfHallPositions; position++)
                {
                    if (dataByPosition.ContainsKey(position))
                    {
                        var dataList = dataByPosition[position];
                        string seriesName = $"{statePrefix} Pos {position}";
                        Debug.WriteLine($"[DEBUG - HallResultsForm] Creating Series: {seriesName} with {dataList.Count} data points and color {seriesColor}");

                        var series = new Series(seriesName)
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            MarkerStyle = MarkerStyle.Circle,
                            MarkerSize = 5,
                            Color = seriesColor
                        };

                        foreach (var dataPoint in dataList)
                        {
                            series.Points.AddXY(dataPoint.Source, dataPoint.Reading);
                        }

                        ChartHallVoltageResults.Series.Add(series);
                        Debug.WriteLine($"[DEBUG - HallResultsForm] Added Series '{seriesName}' to Chart");
                    }
                }
            }

            area.RecalculateAxesScale();
            ChartHallVoltageResults.Invalidate();
            Debug.WriteLine("[DEBUG - HallResultsForm] Chart updated.");
        }

        private void ChartHallVoltageResults_MouseMove(object sender, MouseEventArgs e)
        {
            var hitResult = ChartHallVoltageResults.HitTest(e.X, e.Y);

            if (hitResult.ChartElementType == ChartElementType.DataPoint)
            {
                DataPoint dataPoint = hitResult.Object as DataPoint;
                if (dataPoint != null && hitResult.Series != null)
                {
                    string toolTipText = $"Series: {hitResult.Series.Name}\n" +
                                         $"Current: {dataPoint.XValue:E3} A\n" +
                                         $"Hall Voltage: {dataPoint.YValues[0]:E3} V";
                    chartToolTip.Show(toolTipText, ChartHallVoltageResults, e.X, e.Y - 20);
                }
                else
                {
                    chartToolTip.Hide(ChartHallVoltageResults);
                }
            }
            else
            {
                chartToolTip.Hide(ChartHallVoltageResults);
            }
        }

        private void UpdateSemiconductorTypeButtons()
        {
            if (GlobalSettings.Instance.HallCoefficient < 0)
            {
                GlobalSettings.Instance.SemiconductorType = CollectAndCalculateHallMeasured.SemiconductorType.N;
            }
            else if (GlobalSettings.Instance.HallCoefficient > 0)
            {
                GlobalSettings.Instance.SemiconductorType = CollectAndCalculateHallMeasured.SemiconductorType.P;
            }
            else
            {
                GlobalSettings.Instance.SemiconductorType = CollectAndCalculateHallMeasured.SemiconductorType.Unknown;
            }
            Debug.WriteLine($"[DEBUG] Semiconductor Type set to: {GlobalSettings.Instance.SemiconductorType}-Type");
            UpdateButtonsUI(); 
        }

        private void UpdateButtonsUI()
        {
            if (IconbuttonNType != null)
            {
                IconbuttonNType.BackColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.N) ? Color.LightGreen : Color.Snow;
                //IconbuttonNType.IconColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.N) ? Color.Blue : Color.LightGray;
            }

            if (IconbuttonPType != null)
            {
                IconbuttonPType.BackColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.P) ? Color.LightGreen : Color.Snow;
                //IconbuttonPType.IconColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.P) ? Color.Red : Color.LightGray;
            }
        }

        private void OnHallVoltageCalculatedHandler(object sender, Dictionary<int, double> hallVoltages)
        {
            LoadMeasurementResults();
        }

        private void OnHallPropertiesCalculatedHandler(object sender, (double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility) properties)
        {
            TextboxHallCoefficient.Text = properties.HallCoefficient.ToString("E3");
            TextboxBulkConcentration.Text = properties.BulkConcentration.ToString("E3");
            TextboxMobility.Text = properties.Mobility.ToString("E3");
            UpdateSemiconductorTypeButtons();
        }
    }
}

