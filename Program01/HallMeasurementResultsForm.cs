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
            RichTextBoxSettings(); // ยังคงเรียกใช้งาน RichTextBoxSettings()
            InitializeMeasurementCharts();
            LoadMeasurementResults();
            UpdateSemiconductorTypeButtons();

            CollectAndCalculateHallMeasured.Instance.CalculationCompleted += CollectAndCalculateHallMeasured_CalculationCompleted;
            Debug.WriteLine("[DEBUG - HallResultsForm] Subscribed to CalculationCompleted event for numeric updates.");
            ChartHallVoltageResults.MouseMove += ChartHallVoltageResults_MouseMove;
        }

        private void InitializeMeasurementCharts()
        {
            Debug.WriteLine("[DEBUG] Starting InitializeMeasurementCharts");
            measurementCharts.Clear();

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
                    return "NoField";
                case HallMeasurementState.OutwardOrSouthMagneticField:
                    return "South";
                case HallMeasurementState.InwardOrNorthMagneticField:
                    return "North";
                default:
                    return state.ToString();
            }
        }

        // เมธอด RichTextBoxSettings() ที่ถูกนำกลับมา
        private void RichTextBoxSettings()
        {
            RichTextboxHallInSouthPos1.Text = "VHS1 :";
            RichTextboxHallInSouthPos2.Text = "VHS2 :";
            RichTextboxHallInSouthPos3.Text = "VHS3 :";
            RichTextboxHallInSouthPos4.Text = "VHS4 :";
            RichTextboxHallInNorthPos1.Text = "VHN1 :";
            RichTextboxHallInNorthPos2.Text = "VHN2 :";
            RichTextboxHallInNorthPos3.Text = "VHN3 :";
            RichTextboxHallInNorthPos4.Text = "VHN4 :";

            RichTextboxHallVoltage.Text = "VH :";
            RichTextboxHallRes.Text = "RHall :";
            RichTextboxHallCoefficient.Text = "RH :";
            RichTextboxSheetConcentration.Text = "nSheet :";
            RichTextboxBulkConcentration.Text = "nBulk :";
            RichTextboxMobility.Text = "µ :";

            RichTextboxHallCoefficientUnit.Text = "cm3/C";
            RichTextboxSheetConcentrationUnit.Text = "cm-2";
            RichTextboxBulkConcentrationUnit.Text = "cm-3";
            RichTextboxMobilityUnit.Text = "cm2/V⋅s";

            RichTextboxHallCoefficientUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxSheetConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxBulkConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxMobilityUnit.SelectionAlignment = HorizontalAlignment.Center;

            RichTextboxHallInSouthPos1.Location = new Point(132, 105);
            RichTextboxHallInSouthPos2.Location = new Point(132, 150);
            RichTextboxHallInSouthPos3.Location = new Point(132, 195);
            RichTextboxHallInSouthPos4.Location = new Point(132, 240);
            RichTextboxHallInNorthPos1.Location = new Point(129, 285);
            RichTextboxHallInNorthPos2.Location = new Point(129, 330);
            RichTextboxHallInNorthPos3.Location = new Point(129, 375);
            RichTextboxHallInNorthPos4.Location = new Point(129, 420);
            RichTextboxHallVoltage.Location = new Point(144, 510);
            RichTextboxHallRes.Location = new Point(132, 555);
            RichTextboxHallCoefficient.Location = new Point(144, 600);
            RichTextboxBulkConcentration.Location = new Point(131, 645);
            RichTextboxSheetConcentration.Location = new Point(125, 690);
            RichTextboxMobility.Location = new Point(154, 735);

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

            RichTextboxHallInSouthPos1.Select(1, 3);
            RichTextboxHallInSouthPos2.Select(1, 3);
            RichTextboxHallInSouthPos3.Select(1, 3);
            RichTextboxHallInSouthPos4.Select(1, 3);
            RichTextboxHallInNorthPos1.Select(1, 3);
            RichTextboxHallInNorthPos2.Select(1, 3);
            RichTextboxHallInNorthPos3.Select(1, 3);
            RichTextboxHallInNorthPos4.Select(1, 3);

            RichTextboxHallVoltage.Select(1, 1);
            RichTextboxHallRes.Select(1, 4);
            RichTextboxHallCoefficient.Select(1, 1);
            RichTextboxSheetConcentration.Select(1, 5);
            RichTextboxBulkConcentration.Select(1, 4);

            RichTextboxHallCoefficientUnit.Select(2, 1);
            RichTextboxSheetConcentrationUnit.Select(2, 2);
            RichTextboxBulkConcentrationUnit.Select(2, 2);
            RichTextboxMobilityUnit.Select(2, 2); // แก้เป็น 2 เพื่อเลือก "cm"

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
            RichTextboxMobility.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallCoefficientUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxBulkConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxMobilityUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void LoadMeasurementResults()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(LoadMeasurementResults));
                return;
            }

            Debug.WriteLine("[DEBUG] LoadMeasurementResults - Start");

            TextboxSourceMode.Text = GlobalSettings.Instance.SourceModeUI;
            TextboxMeasureMode.Text = GlobalSettings.Instance.MeasureModeUI;
            
            var avgSouthHallVoltagesByPos = CollectAndCalculateHallMeasured.Instance.AverageVhsByPosition;
            var avgNorthHallVoltagesByPos = CollectAndCalculateHallMeasured.Instance.AverageVhnByPosition;
            
            void SetTextBoxText(TextBox tbx, string prefix, int index, Dictionary<int, double> data)
            {
                if (data != null && data.TryGetValue(index, out double value) && !double.IsNaN(value) && !double.IsInfinity(value))
                {
                    tbx.Text = $"{value:N5}";
                }
                else
                {
                    tbx.Text = $"N/A";
                }
            }

            // แสดงค่าเฉลี่ย Hall Voltages จาก South Field (VHS) ใน TextBox ที่เกี่ยวข้อง
            SetTextBoxText(TextboxHallInSouth1, "VHS", 1, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth2, "VHS", 2, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth3, "VHS", 3, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth4, "VHS", 4, avgSouthHallVoltagesByPos);

            // แสดงค่าเฉลี่ย Hall Voltages จาก North Field (VHN) ใน TextBox ที่เกี่ยวข้อง
            SetTextBoxText(TextboxHallInNorth1, "VHN", 1, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth2, "VHN", 2, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth3, "VHN", 3, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth4, "VHN", 4, avgNorthHallVoltagesByPos);

            // ตรวจสอบและแสดงผลค่า Hall Voltage
            if (!double.IsNaN(GlobalSettings.Instance.TotalHallVoltage_Average) && !double.IsInfinity(GlobalSettings.Instance.TotalHallVoltage_Average))
            {
                double hallVoltage_V = GlobalSettings.Instance.TotalHallVoltage_Average;
                TextboxHallVoltage.Text = $"{hallVoltage_V:N3}";
            }
            else
            {
                TextboxHallVoltage.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Hall Resistance
            if (!double.IsNaN(GlobalSettings.Instance.HallResistance) && !double.IsInfinity(GlobalSettings.Instance.HallResistance))
            {
                TextboxHallRes.Text = GlobalSettings.Instance.HallResistance.ToString("N3");
            }
            else
            {
                TextboxHallRes.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Hall Coefficient
            if (!double.IsNaN(GlobalSettings.Instance.HallCoefficient) && !double.IsInfinity(GlobalSettings.Instance.HallCoefficient))
            {
                TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("N3");
            }
            else
            {
                TextboxHallCoefficient.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Sheet Concentration
            if (!double.IsNaN(GlobalSettings.Instance.SheetConcentration) && !double.IsInfinity(GlobalSettings.Instance.SheetConcentration))
            {
                TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E3");
            }
            else
            {
                TextboxSheetConcentration.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Bulk Concentration
            if (!double.IsNaN(GlobalSettings.Instance.BulkConcentration) && !double.IsInfinity(GlobalSettings.Instance.BulkConcentration))
            {
                TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E3");
            }
            else
            {
                TextboxBulkConcentration.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Mobility
            if (!double.IsNaN(GlobalSettings.Instance.Mobility) && !double.IsInfinity(GlobalSettings.Instance.Mobility))
            {
                TextboxMobility.Text = GlobalSettings.Instance.Mobility.ToString("N3");
            }
            else
            {
                TextboxMobility.Text = "N/A";
            }
            TextboxHallInSouth1Unit.Text = "V";
            TextboxHallInSouth2Unit.Text = "V";
            TextboxHallInSouth3Unit.Text = "V";
            TextboxHallInSouth4Unit.Text = "V";
            TextboxHallInNorth1Unit.Text = "V";
            TextboxHallInNorth2Unit.Text = "V";
            TextboxHallInNorth3Unit.Text = "V";
            TextboxHallInNorth4Unit.Text = "V";
            TextboxHallResUnit.Text = "Ω";
            TextboxHallVoltageUnit.Text = "V";
            UpdateSemiconductorTypeButtons();

            // เรียกเมธอดใหม่เพื่อคำนวณและแสดงผล V_HS1 - V_HN4
            CalculateAndDisplayVhs1MinusVhn4();

            Debug.WriteLine("[DEBUG] LoadMeasurementResults - End");
        }

        private void UpdateMeasurementCharts(IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> allRawMeasurements)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateMeasurementCharts(allRawMeasurements)));
                return;
            }
            Debug.WriteLine("[DEBUG] UpdateMeasurementCharts (Raw Data) - Start");
            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[DEBUG] ChartHallVoltageResults is null, cannot update charts.");
                return;
            }
            ChartHallVoltageResults.Titles.Clear();
            ChartHallVoltageResults.Titles.Add("Hall Voltage (Raw Data) vs. Current at Each Position");
            ChartHallVoltageResults.Series.Clear();
            ChartHallVoltageResults.ChartAreas[0].AxisX.Title = $"Source ({sourceUnit})";
            ChartHallVoltageResults.ChartAreas[0].AxisY.Title = $"Voltage ({measureUnit})";
            ChartHallVoltageResults.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            ChartHallVoltageResults.ChartAreas[0].AxisY.MajorGrid.Enabled = true;

            Color[] noFieldColors = { Color.Gray, Color.DarkGray, Color.DimGray, Color.Black };
            Color[] southColors = { Color.OrangeRed, Color.Red, Color.DarkRed, Color.Brown };
            Color[] northColors = { Color.DeepSkyBlue, Color.Blue, Color.DarkBlue, Color.MediumBlue };

            foreach (var state in new[] { HallMeasurementState.NoMagneticField, HallMeasurementState.OutwardOrSouthMagneticField, HallMeasurementState.InwardOrNorthMagneticField })
            {
                if (allRawMeasurements.ContainsKey(state))
                {
                    var tunerData = allRawMeasurements[state];
                    for (int i = 1; i <= NumberOfHallPositions; i++)
                    {
                        if (tunerData.ContainsKey(i))
                        {
                            string seriesName = $"{GetStatePrefix(state)}{i}";
                            Series series = new Series(seriesName);
                            series.ChartType = SeriesChartType.Line;
                            series.BorderWidth = 2;
                            series.MarkerStyle = MarkerStyle.Circle;
                            series.MarkerSize = 6;
                            series.Points.Clear();

                            Color seriesColor;
                            switch (state)
                            {
                                case HallMeasurementState.NoMagneticField:
                                    seriesColor = noFieldColors[i - 1];
                                    break;
                                case HallMeasurementState.OutwardOrSouthMagneticField:
                                    seriesColor = southColors[i - 1];
                                    break;
                                case HallMeasurementState.InwardOrNorthMagneticField:
                                    seriesColor = northColors[i - 1];
                                    break;
                                default:
                                    seriesColor = Color.Black;
                                    break;
                            }
                            series.Color = seriesColor;


                            foreach (var dataPoint in tunerData[i])
                            {
                                series.Points.AddXY(dataPoint.Item1, dataPoint.Item2);
                            }
                            ChartHallVoltageResults.Series.Add(series);
                            Debug.WriteLine($"[DEBUG] Chart '{seriesName}' updated with {series.Points.Count} points and color {seriesColor.Name}");
                        }
                    }
                }
            }
            Debug.WriteLine("[DEBUG] UpdateMeasurementCharts (Raw Data) - End");
        }

        private void UpdateCalculatedHallVoltageChart(IReadOnlyDictionary<double, HallCalculationResultPerCurrent> detailedResults)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateCalculatedHallVoltageChart(detailedResults)));
                return;
            }

            Debug.WriteLine("[DEBUG] UpdateCalculatedHallVoltageChart (Calculated) - Start");
            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[DEBUG] ChartHallVoltageResults is null, cannot update calculated charts.");
                return;
            }

            ChartHallVoltageResults.Titles.Clear();
            ChartHallVoltageResults.Titles.Add("True Hall Voltage (VH) vs. Current (Calculated)");
            ChartHallVoltageResults.Series.Clear();

            ChartHallVoltageResults.ChartAreas[0].AxisX.Title = $"Current (A)";
            ChartHallVoltageResults.ChartAreas[0].AxisY.Title = $"True Hall Voltage (V)";
            ChartHallVoltageResults.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            ChartHallVoltageResults.ChartAreas[0].AxisY.MajorGrid.Enabled = true;

            Series vhSeries = new Series("True Hall Voltage (VH)");
            vhSeries.ChartType = SeriesChartType.Line;
            vhSeries.BorderWidth = 2;
            vhSeries.MarkerStyle = MarkerStyle.Circle;
            vhSeries.MarkerSize = 6;
            vhSeries.Color = Color.Blue;

            foreach (var entry in detailedResults.OrderBy(e => e.Key))
            {
                double current = entry.Key;
                double vhAverage = entry.Value.Vh_Average;

                if (!double.IsNaN(vhAverage) && !double.IsInfinity(vhAverage))
                {
                    vhSeries.Points.AddXY(current, vhAverage);
                    Debug.WriteLine($"[DEBUG] Added point to VH Chart: Current={current}, Vh_Average={vhAverage}");
                }
            }
            ChartHallVoltageResults.Series.Add(vhSeries);
            Debug.WriteLine("[DEBUG] UpdateCalculatedHallVoltageChart (Calculated) - End");
        }

        private void OnAllHallDataUpdatedHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Received AllHallDataUpdated event. Updating charts.");
            UpdateMeasurementCharts(CollectAndCalculateHallMeasured.Instance.AllRawMeasurements);
        }

        private void ChartHallVoltageResults_MouseMove(object sender, MouseEventArgs e)
        {
            HitTestResult result = ChartHallVoltageResults.HitTest(e.X, e.Y);

            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                Series series = result.Series;
                DataPoint point = series.Points[result.PointIndex];

                string tooltipText = $"Series: {series.Name}\n";
                tooltipText += $"Current: {point.XValue:E3} {sourceUnit}\n";
                tooltipText += $"Voltage: {point.YValues:E3} {measureUnit}";

                chartToolTip.Show(tooltipText, ChartHallVoltageResults, e.X + 10, e.Y + 10);
            }
            else
            {
                chartToolTip.Hide(ChartHallVoltageResults);
            }
        }

        private void UpdateSemiconductorTypeButtons()
        {
            if (IconbuttonNType != null)
            {
                IconbuttonNType.BackColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.N) ? Color.LightGreen : Color.Snow;
            }

            if (IconbuttonPType != null)
            {
                IconbuttonPType.BackColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.P) ? Color.LightGreen : Color.Snow;
            }
        }

        private void CalculateAndDisplayVhs1MinusVhn4()
        {
            double vhs1 = double.NaN;
            double vhn4 = double.NaN;

            if (CollectAndCalculateHallMeasured.Instance.AverageVhsByPosition != null &&
                CollectAndCalculateHallMeasured.Instance.AverageVhsByPosition.TryGetValue(1, out double valVhs1))
            {
                vhs1 = valVhs1;
            }

            if (CollectAndCalculateHallMeasured.Instance.AverageVhnByPosition != null &&
                CollectAndCalculateHallMeasured.Instance.AverageVhnByPosition.TryGetValue(4, out double valVhn4))
            {
                vhn4 = valVhn4;
            }

            double result = double.NaN;
            if (!double.IsNaN(vhs1) && !double.IsNaN(vhn4))
            {
                result = vhs1 - vhn4;
            }

            Label lblVhs1MinusVhn4 = Controls.Find("lblVhs1MinusVhn4", true).FirstOrDefault() as Label;
            if (lblVhs1MinusVhn4 != null)
            {
                if (!double.IsNaN(result) && !double.IsInfinity(result))
                {
                    lblVhs1MinusVhn4.Text = result.ToString("N3") + " V";
                }
                else
                {
                    lblVhs1MinusVhn4.Text = "N/A";
                }
            }
            else
            {
                Debug.WriteLine("[DEBUG] Warning: lblVhs1MinusVhn4 not found on the form.");
            }
        }

        private void CollectAndCalculateHallMeasured_CalculationCompleted(object sender, EventArgs e)
        {
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Received CalculationCompleted event. Reloading numeric results.");
            LoadMeasurementResults();
        }

        private void HallMeasurementResultsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectAndCalculateHallMeasured.Instance.CalculationCompleted -= CollectAndCalculateHallMeasured_CalculationCompleted;
            ChartHallVoltageResults.MouseMove -= ChartHallVoltageResults_MouseMove;
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Unsubscribed from events.");
        }
    }
}