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
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm Constructor - Start");
            InitializeComponent();
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm Constructor - After InitializeComponent()");

            // ตรวจสอบ ChartHallVoltageResults ทันทีหลัง InitializeComponent()
            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[CRITICAL ERROR] Constructor: ChartHallVoltageResults is NULL immediately after InitializeComponent().");
                MessageBox.Show("ข้อผิดพลาดร้ายแรง: ChartHallVoltageResults ไม่ได้ถูก Initialize ใน Designer. โปรดตรวจสอบ HallMeasurementResultsForm.Designer.cs", "ข้อผิดพลาดการโหลดฟอร์ม", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RichTextBoxSettings();
            InitializeMeasurementCharts();
            LoadMeasurementResults(); // เมธอดนี้จะเรียก UpdateHallVoltageResultsChart()
            UpdateSemiconductorTypeButtons();

            CollectAndCalculateHallMeasured.Instance.CalculationCompleted += CollectAndCalculateHallMeasured_CalculationCompleted;
            Debug.WriteLine("[DEBUG - HallResultsForm] Subscribed to CalculationCompleted event for numeric updates.");
            ChartHallVoltageResults.MouseMove += ChartHallVoltageResults_MouseMove;

        }

        private void InitializeMeasurementCharts()
        {
            Debug.WriteLine("[DEBUG] Starting InitializeMeasurementCharts");

            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[ERROR] ChartHallVoltageResults control is NULL. Chart initialization failed. Please ensure the 'ChartHallVoltageResults' control exists and is correctly named in the Form Designer.");
                MessageBox.Show("เกิดข้อผิดพลาด: ไม่พบ ChartHallVoltageResults บนฟอร์ม กรุณาตรวจสอบการตั้งค่าฟอร์มใน Designer.", "ข้อผิดพลาดการแสดงผลกราฟ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ChartHallVoltageResults.Series.Clear(); // Clear all existing series from the Chart
            Debug.WriteLine("[DEBUG] Cleared all existing series points.");

            // Ensure there is at least one ChartArea before trying to access it
            if (ChartHallVoltageResults.ChartAreas.Count == 0)
            {
                ChartHallVoltageResults.ChartAreas.Add(new ChartArea("DefaultArea"));
                Debug.WriteLine("[DEBUG] Added 'DefaultArea' to ChartHallVoltageResults because it was missing.");
            }
            else
            {
                Debug.WriteLine($"[DEBUG] Chart already has {ChartHallVoltageResults.ChartAreas.Count} ChartAreas. Using the first one.");
            }

            // Set Chart Area name and customize axes
            ChartHallVoltageResults.ChartAreas[0].Name = "DefaultArea";
            ChartHallVoltageResults.ChartAreas[0].AxisX.Title = "Current (A)";
            ChartHallVoltageResults.ChartAreas[0].AxisY.Title = "Voltage (V)";

            // **** Start of axis and scale balance adjustments ****

            // Adjust LabelStyle.Format (already E2, which is good for scientific notation)
            ChartHallVoltageResults.ChartAreas[0].AxisX.LabelStyle.Format = "E2"; // Scientific notation
            ChartHallVoltageResults.ChartAreas[0].AxisY.LabelStyle.Format = "E2"; // Scientific notation

            // Improve Label Spacing and Overlap
            ChartHallVoltageResults.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep90; // Stagger labels and rotate if needed
            ChartHallVoltageResults.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep90; // Stagger labels and rotate if needed
            ChartHallVoltageResults.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Auto; // Ensures automatic interval offset
            ChartHallVoltageResults.ChartAreas[0].AxisY.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Auto; // Ensures automatic interval offset
            ChartHallVoltageResults.ChartAreas[0].AxisX.Minimum = double.NaN; // Reset min/max to auto-calculate
            ChartHallVoltageResults.ChartAreas[0].AxisX.Maximum = double.NaN; // Reset min/max to auto-calculate
            ChartHallVoltageResults.ChartAreas[0].AxisY.Minimum = double.NaN; // Reset min/max to auto-calculate
            ChartHallVoltageResults.ChartAreas[0].AxisY.Maximum = double.NaN; // Reset min/max to auto-calculate


            // **** การปรับปรุงหลักเพื่อแก้ไขแกน Y (เพิ่ม InnerPlotPosition.X อีก) ****
            // ปรับ InnerPlotPosition ของ ChartArea เพื่อให้มีพื้นที่สำหรับแกน Y และชื่อแกน
            // InnerPlotPosition กำหนดพื้นที่สำหรับข้อมูลกราฟจริง (ไม่รวม axis labels, axis titles, legend)
            // ค่าเป็นเปอร์เซ็นต์ของ ChartArea ทั้งหมด (X, Y, Width, Height)
            ChartHallVoltageResults.ChartAreas[0].InnerPlotPosition.Auto = false;
            // เพิ่มค่า X เพื่อขยับพื้นที่กราฟจริงไปทางขวามากขึ้น ทำให้มีพื้นที่ว่างด้านซ้ายสำหรับแกน Y
            // ลองปรับเป็น 12-15% หรือมากกว่า หากชื่อแกน Y ยังหายไป
            ChartHallVoltageResults.ChartAreas[0].InnerPlotPosition.X = 14;    // <-- ปรับค่านี้ให้มากขึ้น
            ChartHallVoltageResults.ChartAreas[0].InnerPlotPosition.Y = 8;    // คงเดิม
                                                                              // ลดค่า Width เพื่อให้พื้นที่กราฟจริงไม่ขยายไปทางขวามากเกินไปหลังจากขยับ X
            ChartHallVoltageResults.ChartAreas[0].InnerPlotPosition.Width = 83; // <-- ปรับค่านี้ให้ลดลง (100 - X - (100 - Width - X) = 100 - 12 - (100 - 83) = 100 - 12 - 17 = 71%)
                                                                                // หรือคิดง่ายๆ คือ 100 - InnerPlotPosition.X - (พื้นที่ว่างด้านขวาที่คุณต้องการ)
                                                                                // เช่น ถ้า InnerPlotPosition.X = 12 และต้องการเหลือพื้นที่ขวา 5% -> Width = 100 - 12 - 5 = 83
            ChartHallVoltageResults.ChartAreas[0].InnerPlotPosition.Height = 75; // คงเดิม (สำหรับพื้นที่ด้านล่าง)


            // Grid lines (already good)
            ChartHallVoltageResults.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.DimGray;
            ChartHallVoltageResults.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.DimGray;

            // Interval Auto Mode (already good)
            ChartHallVoltageResults.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            ChartHallVoltageResults.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

            // **** End of axis and scale balance adjustments ****

            Debug.WriteLine("[DEBUG] Chart Area and Axis settings applied.");

            // Create Series for South Field (+B) and North Field (-B) for each Tuner
            for (int i = 1; i <= NumberOfHallPositions; i++)
            {
                // Series for South Field (+B)
                string southSeriesName = $"South{i}";
                if (!ChartHallVoltageResults.Series.Any(s => s.Name == southSeriesName))
                {
                    Series southSeries = new Series(southSeriesName);
                    southSeries.ChartType = SeriesChartType.Line;
                    southSeries.BorderWidth = 2;
                    southSeries.Color = Color.OrangeRed; // Orange for +B (South Field)
                    southSeries.LegendText = $"South Field (Position {i})"; // Legend text
                    southSeries.Legend = "Default"; // Use the "Default" Legend
                    southSeries.MarkerStyle = MarkerStyle.Triangle; // Add Marker
                    southSeries.MarkerSize = 6;
                    ChartHallVoltageResults.Series.Add(southSeries);
                    Debug.WriteLine($"[DEBUG] Initialized Chart Series: {southSeriesName} with color {southSeries.Color.Name}. Series Count: {ChartHallVoltageResults.Series.Count}");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] Series '{southSeriesName}' already exists.");
                }

                // Series for North Field (-B)
                string northSeriesName = $"North{i}";
                if (!ChartHallVoltageResults.Series.Any(s => s.Name == northSeriesName))
                {
                    Series northSeries = new Series(northSeriesName);
                    northSeries.ChartType = SeriesChartType.Line;
                    northSeries.BorderWidth = 2;
                    northSeries.Color = Color.DeepSkyBlue; // Blue for -B (North Field)
                    northSeries.LegendText = $"North Field (Position {i})"; // Legend text
                    northSeries.Legend = "Default"; // Use the "Default" Legend
                    northSeries.MarkerStyle = MarkerStyle.Square; // Add Marker
                    northSeries.MarkerSize = 6;
                    ChartHallVoltageResults.Series.Add(northSeries);
                    Debug.WriteLine($"[DEBUG] Initialized Chart Series: {northSeriesName} with color {northSeries.Color.Name}. Series Count: {ChartHallVoltageResults.Series.Count}");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] Series '{northSeriesName}' already exists.");
                }
            }

            ChartHallVoltageResults.Legends.Clear();
            ChartHallVoltageResults.Legends.Add("Default"); // This is the Legend we want the Series to use
            ChartHallVoltageResults.Legends["Default"].Docking = Docking.Bottom; // Place Legend at the bottom
            ChartHallVoltageResults.Legends["Default"].Alignment = StringAlignment.Center; // Center align
            ChartHallVoltageResults.Legends["Default"].IsTextAutoFit = true;
            ChartHallVoltageResults.Legends["Default"].Font = new Font("Segoe UI", 8F);
            ChartHallVoltageResults.Legends["Default"].BackColor = Color.Transparent; // Transparent background
            ChartHallVoltageResults.Legends["Default"].BorderColor = Color.Transparent; // Transparent border

            Debug.WriteLine("[DEBUG] Chart Legend settings applied.");
            Debug.WriteLine("[DEBUG] Ending InitializeMeasurementCharts");
        }

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
            var measurements = CollectAndCalculateHallMeasured.Instance.Measurements;

            Debug.WriteLine("[DEBUG-LOAD] Calling UpdateHallVoltageResultsChart()...");
            UpdateHallVoltageResultsChart(); // นี่คือเมธอดที่น่าจะพล็อตข้อมูลลง Chart
            Debug.WriteLine("[DEBUG-LOAD] UpdateHallVoltageResultsChart() completed.");

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

            SetTextBoxText(TextboxHallInSouth1, "VHS", 1, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth2, "VHS", 2, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth3, "VHS", 3, avgSouthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInSouth4, "VHS", 4, avgSouthHallVoltagesByPos);

            SetTextBoxText(TextboxHallInNorth1, "VHN", 1, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth2, "VHN", 2, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth3, "VHN", 3, avgNorthHallVoltagesByPos);
            SetTextBoxText(TextboxHallInNorth4, "VHN", 4, avgNorthHallVoltagesByPos);

            if (!double.IsNaN(GlobalSettings.Instance.TotalHallVoltage_Average) && !double.IsInfinity(GlobalSettings.Instance.TotalHallVoltage_Average))
            {
                double hallVoltage_mV = GlobalSettings.Instance.TotalHallVoltage_Average * 1000;
                TextboxHallVoltage.Text = $"{hallVoltage_mV:N3}";
            }
            else
            {
                TextboxHallVoltage.Text = "N/A";
            }

            if (!double.IsNaN(GlobalSettings.Instance.HallResistance) && !double.IsInfinity(GlobalSettings.Instance.HallResistance))
            {
                TextboxHallRes.Text = GlobalSettings.Instance.HallResistance.ToString("N3");
            }
            else
            {
                TextboxHallRes.Text = "N/A";
            }

            if (!double.IsNaN(GlobalSettings.Instance.HallCoefficient) && !double.IsInfinity(GlobalSettings.Instance.HallCoefficient))
            {
                TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("N3");
            }
            else
            {
                TextboxHallCoefficient.Text = "N/A";
            }

            if (!double.IsNaN(GlobalSettings.Instance.SheetConcentration) && !double.IsInfinity(GlobalSettings.Instance.SheetConcentration))
            {
                TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E3");
            }
            else
            {
                TextboxSheetConcentration.Text = "N/A";
            }

            if (!double.IsNaN(GlobalSettings.Instance.BulkConcentration) && !double.IsInfinity(GlobalSettings.Instance.BulkConcentration))
            {
                TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E3");
            }
            else
            {
                TextboxBulkConcentration.Text = "N/A";
            }

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
            TextboxHallVoltageUnit.Text = "mV";

            UpdateSemiconductorTypeButtons();
            //CalculateAndDisplayVhs1MinusVhn4();

            Debug.WriteLine("[DEBUG] LoadMeasurementResults - End");
        }

        private void UpdateHallVoltageResultsChart()
        {
            Debug.WriteLine("[DEBUG-CHART] UpdateHallVoltageResultsChart - Start for RAW DATA plotting");

            if (ChartHallVoltageResults == null)
            {
                Debug.WriteLine("[ERROR-CHART] ChartHallVoltageResults is NULL. Cannot update chart.");
                return;
            }

            // 1. Clear existing points from all series
            foreach (var series in ChartHallVoltageResults.Series)
            {
                series.Points.Clear();
                Debug.WriteLine($"[DEBUG-CHART] Cleared points for series: {series.Name}");
            }

            for (int i = 1; i <= NumberOfHallPositions; i++)
            {
                Debug.WriteLine($"[DEBUG-CHART] Plotting raw data for Tuner Position: {i}");

                // --- No Magnetic Field (Out) ---
                /*string outSeriesName = $"Out{i}";
                Series outSeries = ChartHallVoltageResults.Series.FirstOrDefault(s => s.Name == outSeriesName);
                if (outSeries != null && CollectAndCalculateHallMeasured.Instance.Measurements.ContainsKey(HallMeasurementState.NoMagneticField) &&
                    CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.NoMagneticField].ContainsKey(i))
                {
                    foreach (var dataPoint in CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.NoMagneticField][i])
                    {
                        outSeries.Points.AddXY(dataPoint.Item1, dataPoint.Item2); // Item1 = Current, Item2 = Voltage
                        // Debug.WriteLine($"[DEBUG-CHART] Added point to {outSeriesName}: X={dataPoint.Item1:E2}, Y={dataPoint.Item2:E2}");
                    }
                    Debug.WriteLine($"[DEBUG-CHART] Series '{outSeriesName}' updated with {outSeries.Points.Count} points.");
                }
                else
                {
                    Debug.WriteLine($"[WARNING-CHART] Series '{outSeriesName}' not found or NoMagneticField data for Tuner {i} is missing.");
                }*/

                // --- South Magnetic Field ---
                string southSeriesName = $"South{i}";
                Series southSeries = ChartHallVoltageResults.Series.FirstOrDefault(s => s.Name == southSeriesName);
                if (southSeries != null && CollectAndCalculateHallMeasured.Instance.Measurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField) &&
                    CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(i))
                {
                    foreach (var dataPoint in CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.OutwardOrSouthMagneticField][i])
                    {
                        southSeries.Points.AddXY(dataPoint.Item1, dataPoint.Item2);
                    }
                    Debug.WriteLine($"[DEBUG-CHART] Series '{southSeriesName}' updated with {southSeries.Points.Count} points.");
                }
                else
                {
                    Debug.WriteLine($"[WARNING-CHART] Series '{southSeriesName}' not found or SouthMagneticField data for Tuner {i} is missing.");
                }

                // --- North Magnetic Field ---
                string northSeriesName = $"North{i}";
                Series northSeries = ChartHallVoltageResults.Series.FirstOrDefault(s => s.Name == northSeriesName);
                if (northSeries != null && CollectAndCalculateHallMeasured.Instance.Measurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField) &&
                    CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(i))
                {
                    foreach (var dataPoint in CollectAndCalculateHallMeasured.Instance.Measurements[HallMeasurementState.InwardOrNorthMagneticField][i])
                    {
                        northSeries.Points.AddXY(dataPoint.Item1, dataPoint.Item2);
                    }
                    Debug.WriteLine($"[DEBUG-CHART] Series '{northSeriesName}' updated with {northSeries.Points.Count} points.");
                }
                else
                {
                    Debug.WriteLine($"[WARNING-CHART] Series '{northSeriesName}' not found or NorthMagneticField data for Tuner {i} is missing.");
                }
            }

            // Refresh the chart to display changes
            ChartHallVoltageResults.Invalidate();
            ChartHallVoltageResults.Update();
            Debug.WriteLine("[DEBUG-CHART] Chart Invalidated and Updated.");

            Debug.WriteLine("[DEBUG-CHART] UpdateHallVoltageResultsChart - End for RAW DATA plotting");
        }

        private void OnAllHallDataUpdatedHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("[DEBUG] HallMeasurementResultsForm: Received AllHallDataUpdated event. Updating charts.");
            UpdateHallVoltageResultsChart();
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

        /*private void CalculateAndDisplayVhs1MinusVhn4()
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

            if (Controls.Find("lblVhs1MinusVhn4", true).FirstOrDefault() is Label lblVhs1MinusVhn4)
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
        }*/

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