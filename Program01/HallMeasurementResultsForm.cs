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
            InitializeMeasurementCharts();
            LoadMeasurementResults();
            UpdateButtonsUI();

            //CollectAndCalculateHallMeasured.Instance.AllHallDataUpdated += OnAllHallDataUpdatedHandler;
            //Debug.WriteLine("[DEBUG - HallResultsForm] Subscribed to AllHallDataUpdated event for chart updates.");

            CollectAndCalculateHallMeasured.Instance.CalculationCompleted += CollectAndCalculateHallMeasured_CalculationCompleted;
            Debug.WriteLine("[DEBUG - HallResultsForm] Subscribed to CalculationCompleted event for numeric updates.");
            ChartHallVoltageResults.MouseMove += ChartHallVoltageResults_MouseMove;
        }

        private void InitializeMeasurementCharts()
        {
            Debug.WriteLine("[DEBUG] Starting InitializeMeasurementCharts");
            measurementCharts.Clear();

            // ตรวจสอบให้แน่ใจว่า ChartHallVoltageResults ถูกสร้างขึ้นใน Designer และเข้าถึงได้
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
                    return "NoField"; // เปลี่ยนเป็น NoField เพื่อความชัดเจนในการแสดงผลกราฟ
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
            // ตรวจสอบชื่อ RichTextBoxes ของคุณให้ตรงกัน (จากโค้ดเดิมที่คุณส่งมา)
            // เช่น RichTextboxHallCoefficient แทน TextboxHallCoefficient
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
            RichTextboxHallCoefficient.Text = "RH :"; // ตรวจสอบชื่อ RichTextBox ให้ตรงกัน
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

            // Existing RichTextBox location and font settings...
            RichTextboxHallInSouthPos1.Location = new Point(132, 105);
            RichTextboxHallInSouthPos2.Location = new Point(132, 150);
            RichTextboxHallInSouthPos3.Location = new Point(132, 195);
            RichTextboxHallInSouthPos4.Location = new Point(132, 240);
            RichTextboxHallInNorthPos1.Location = new Point(132, 285);
            RichTextboxHallInNorthPos2.Location = new Point(132, 330);
            RichTextboxHallInNorthPos3.Location = new Point(132, 375);
            RichTextboxHallInNorthPos4.Location = new Point(132, 420);
            RichTextboxHallVoltage.Location = new Point(135, 510);
            RichTextboxHallRes.Location = new Point(125, 555);
            RichTextboxHallCoefficient.Location = new Point(138, 600);
            RichTextboxBulkConcentration.Location = new Point(124, 645);
            RichTextboxSheetConcentration.Location = new Point(120, 690);
            RichTextboxMobility.Location = new Point(146, 735);

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
            RichTextboxHallCoefficient.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point); // แก้ไขชื่อ
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
            RichTextboxMobilityUnit.Select(2, 1);

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
            RichTextboxHallCoefficient.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point); // แก้ไขชื่อ
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
            var avgSouthVoltagesByPos = CollectAndCalculateHallMeasured.Instance.AverageVhsByPosition;
            var avgNorthVoltagesByPos = CollectAndCalculateHallMeasured.Instance.AverageVhnByPosition;

            void SetTextBoxText(TextBox tbx, string prefix, int index, Dictionary<int, double> data)
            {
                if (data != null && data.TryGetValue(index, out double value) && !double.IsNaN(value) && !double.IsInfinity(value))
                {
                    tbx.Text = $"{value:E5}";
                }
                else
                {
                    tbx.Text = $"N/A";
                }
            }

            // Display Average Hall Voltages from South Field (VHS)
            SetTextBoxText(TextboxHallInSouth1, "VHS", 1, avgSouthVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth2, "VHS", 2, avgSouthVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth3, "VHS", 3, avgSouthVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth4, "VHS", 4, avgSouthVoltagesByPos);

            // Display Average Hall Voltages from North Field (VHN)
            SetTextBoxText(TextboxHallInNorth1, "VHN", 1, avgNorthVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth2, "VHN", 2, avgNorthVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth3, "VHN", 3, avgNorthVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth4, "VHN", 4, avgNorthVoltagesByPos);

            // ตรวจสอบและแสดงผลค่า Hall Voltage
            if (!double.IsNaN(GlobalSettings.Instance.TotalHallVoltage_Average) && !double.IsInfinity(GlobalSettings.Instance.TotalHallVoltage_Average))
            {
                TextboxHallVoltage.Text = GlobalSettings.Instance.TotalHallVoltage_Average.ToString("E5");
            }
            else
            {
                TextboxHallVoltage.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Hall Resistance
            if (!double.IsNaN(GlobalSettings.Instance.HallResistance) && !double.IsInfinity(GlobalSettings.Instance.HallResistance))
            {
                TextboxHallRes.Text = GlobalSettings.Instance.HallResistance.ToString("E5");
            }
            else
            {
                TextboxHallRes.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Hall Coefficient
            if (!double.IsNaN(GlobalSettings.Instance.HallCoefficient) && !double.IsInfinity(GlobalSettings.Instance.HallCoefficient))
            {
                TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("E5");
            }
            else
            {
                TextboxHallCoefficient.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Sheet Concentration
            if (!double.IsNaN(GlobalSettings.Instance.SheetConcentration) && !double.IsInfinity(GlobalSettings.Instance.SheetConcentration))
            {
                TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E5");
            }
            else
            {
                TextboxSheetConcentration.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Bulk Concentration
            if (!double.IsNaN(GlobalSettings.Instance.BulkConcentration) && !double.IsInfinity(GlobalSettings.Instance.BulkConcentration))
            {
                TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E5");
            }
            else
            {
                TextboxBulkConcentration.Text = "N/A";
            }

            // ตรวจสอบและแสดงผลค่า Mobility
            if (!double.IsNaN(GlobalSettings.Instance.Mobility) && !double.IsInfinity(GlobalSettings.Instance.Mobility))
            {
                TextboxMobility.Text = GlobalSettings.Instance.Mobility.ToString("E5");
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
            ChartHallVoltageResults.ChartAreas[0].AxisY.Title = $"Voltage ({measureUnit})"; // เปลี่ยนเป็น Voltage เพราะเป็นค่าดิบ
            ChartHallVoltageResults.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            ChartHallVoltageResults.ChartAreas[0].AxisY.MajorGrid.Enabled = true;

            Color[] noFieldColors = { Color.Gray, Color.DarkGray, Color.DimGray, Color.Black }; // สำหรับ No Magnetic Field (Out)
            Color[] southColors = { Color.OrangeRed, Color.Red, Color.DarkRed, Color.Brown };    // สำหรับ South Field (Outward)
            Color[] northColors = { Color.DeepSkyBlue, Color.Blue, Color.DarkBlue, Color.MediumBlue }; // สำหรับ North Field (Inward)

            foreach (var state in new[] { HallMeasurementState.NoMagneticField, HallMeasurementState.OutwardOrSouthMagneticField, HallMeasurementState.InwardOrNorthMagneticField })
            {
                if (allRawMeasurements.ContainsKey(state))
                {
                    var tunerData = allRawMeasurements[state];

                    // วนลูปผ่านแต่ละตำแหน่ง Tuner (1 ถึง 4)
                    for (int pos = 1; pos <= NumberOfHallPositions; pos++)
                    {
                        if (tunerData.ContainsKey(pos) && tunerData[pos] != null && tunerData[pos].Any())
                        {
                            string seriesName = $"{GetStatePrefix(state)}{pos}"; // เช่น "South1", "North2", "NoField3"
                            Series series = new Series(seriesName)
                            {
                                ChartType = SeriesChartType.Line,
                                BorderWidth = 2,
                                MarkerStyle = MarkerStyle.Circle,
                                MarkerSize = 5
                            };

                            // กำหนดสีตามสถานะและตำแหน่ง
                            if (state == HallMeasurementState.OutwardOrSouthMagneticField)
                            {
                                series.Color = southColors[pos - 1];
                            }
                            else if (state == HallMeasurementState.InwardOrNorthMagneticField)
                            {
                                series.Color = northColors[pos - 1];
                            }
                            else if (state == HallMeasurementState.NoMagneticField)
                            {
                                series.Color = noFieldColors[pos - 1];
                                series.BorderDashStyle = ChartDashStyle.Dash; // ใช้เส้นประสำหรับ No Field เพื่อแยกความแตกต่าง
                            }

                            ChartHallVoltageResults.Series.Add(series);

                            // เพิ่มจุดข้อมูลลงใน Series โดยเรียงตามกระแส (Item1)
                            foreach (var dataPoint in tunerData[pos].OrderBy(dp => dp.Item1))
                            {
                                series.Points.AddXY(dataPoint.Item1, dataPoint.Item2);
                            }
                        }
                    }
                }
            }

            ChartHallVoltageResults.Update();
            Debug.WriteLine("[DEBUG] UpdateMeasurementCharts (Raw Data) - End");
        }

        private void ChartHallVoltageResults_MouseMove(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;
            HitTestResult hitTestResult = chart.HitTest(e.X, e.Y);

            if (hitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                DataPoint point = hitTestResult.Series.Points[hitTestResult.PointIndex];
                chartToolTip.Show($"Series: {hitTestResult.Series.Name}\nCurrent: {point.XValue:E3} {sourceUnit}\nVoltage: {point.YValues:E5} {measureUnit}", chart, e.Location.X, e.Location.Y - 15);
            }
            else
            {
                chartToolTip.Hide(chart);
            }
        }

        private void UpdateSemiconductorTypeButtons()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateSemiconductorTypeButtons));
                return;
            }

            // ใช้ค่าจาก GlobalSettings ซึ่งถูกกำหนดโดย CollectAndCalculateHallMeasured
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
            }

            if (IconbuttonPType != null)
            {
                IconbuttonPType.BackColor = (GlobalSettings.Instance.SemiconductorType == CollectAndCalculateHallMeasured.SemiconductorType.P) ? Color.LightGreen : Color.Snow;
            }
        }

        private void CollectAndCalculateHallMeasured_CalculationCompleted(object sender, EventArgs e)
        {
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Received CalculationCompleted event. Reloading numeric results.");
            LoadMeasurementResults(); // Reload all textboxes with updated overall properties
        }

        private void HallMeasurementResultsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //CollectAndCalculateHallMeasured.Instance.AllHallDataUpdated -= OnAllHallDataUpdatedHandler; // ยกเลิกการสมัครสมาชิก
            CollectAndCalculateHallMeasured.Instance.CalculationCompleted -= CollectAndCalculateHallMeasured_CalculationCompleted; // ยกเลิกการสมัครสมาชิก

            ChartHallVoltageResults.MouseMove -= ChartHallVoltageResults_MouseMove;
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Unsubscribed from events.");
        }
    }
}