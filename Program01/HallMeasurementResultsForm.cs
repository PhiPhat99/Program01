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
        private readonly string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
        private readonly string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";

        public HallMeasurementResultsForm()
        {
            InitializeComponent();
            RichTextBoxSettings();
            LoadMeasurementResults();
            UpdateSemiconductorTypeButtons();

            CollectAndCalculateHallMeasured.Instance.HallVoltageCalculated += OnHallVoltageCalculatedHandler;
            CollectAndCalculateHallMeasured.Instance.HallPropertiesCalculated += OnHallPropertiesCalculatedHandler;
            CollectAndCalculateHallMeasured.Instance.HallIVHDataCalculated += OnHallIVHDataCalculatedHandler; // Subscribe Event ใหม่
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

            RichTextboxHallVoltage.Location = new Point(140, 535);
            RichTextboxHallCoefficient.Location = new Point(140, 570);
            RichTextboxSheetConcentration.Location = new Point(120, 605);
            RichTextboxBulkConcentration.Location = new Point(125, 640);
            RichTextboxMobility.Location = new Point(148, 675);


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
            RichTextboxHallCoefficient.Select(1, 1);
            RichTextboxSheetConcentration.Select(1, 5);
            RichTextboxBulkConcentration.Select(1, 4);

            RichTextboxHallCoefficientUnit.Select(1, 1);
            RichTextboxSheetConcentrationUnit.Select(1, 2);
            RichTextboxBulkConcentrationUnit.Select(1, 2);
            RichTextboxMobilityUnit.Select(1, 1);

            RichTextboxHallOutPos1.SelectionCharOffset = -8;
            RichTextboxHallOutPos2.SelectionCharOffset = -8;
            RichTextboxHallOutPos3.SelectionCharOffset = -8;
            RichTextboxHallOutPos4.SelectionCharOffset = -8;

            RichTextboxHallInSouthPos1.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos2.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos3.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos4.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos1.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos2.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos3.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos4.SelectionCharOffset = -8;

            RichTextboxHallVoltage.SelectionCharOffset = -8;
            RichTextboxHallCoefficient.SelectionCharOffset = -8;
            RichTextboxSheetConcentration.SelectionCharOffset = -8;
            RichTextboxBulkConcentration.SelectionCharOffset = -8;

            RichTextboxHallCoefficientUnit.SelectionCharOffset = +8;
            RichTextboxSheetConcentrationUnit.SelectionCharOffset = +8;
            RichTextboxBulkConcentrationUnit.SelectionCharOffset = +8;
            RichTextboxMobilityUnit.SelectionCharOffset = +8;

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

            var voltages = GlobalSettings.Instance.HallVoltagesByPosition;

            void SetText(TextBox box, int index)
            {
                if (voltages.TryGetValue(index, out double value))
                    box.Text = value.ToString("E3");
                else
                    box.Text = "N/A";
            }

            SetText(TextboxHallOut1, 1);
            SetText(TextboxHallOut2, 2);
            SetText(TextboxHallOut3, 3);
            SetText(TextboxHallOut4, 4);

            SetText(TextboxHallInSouth1, 5);
            SetText(TextboxHallInSouth2, 6);
            SetText(TextboxHallInSouth3, 7);
            SetText(TextboxHallInSouth4, 8);

            SetText(TextboxHallInNorth1, 9);
            SetText(TextboxHallInNorth2, 10);
            SetText(TextboxHallInNorth3, 11);
            SetText(TextboxHallInNorth4, 12);

            TextboxHallVoltage.Text = GlobalSettings.Instance.TotalHallVoltage.ToString("E3");
            TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("E3");
            TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E3");
            TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E3");
            TextboxMobility.Text = GlobalSettings.Instance.Mobility.ToString("E3");

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

            SetUnit("TextboxHallVoltage1Unit", "V");
            SetUnit("TextboxHallVoltage2Unit", "V");
            SetUnit("TextboxHallVoltage3Unit", "V");
            SetUnit("TextboxHallVoltage4Unit", "V");
            SetUnit("TextboxHallVoltageUnit", "V");
        }

        private void SetUnit(string controlName, string unit)
        {
            if (Controls.Find(controlName, true).FirstOrDefault() is TextBox unitBox)
            {
                unitBox.Text = unit;
            }
        }

        /*private void LoadHallVoltageResultsChart()
        {
            // ตรวจสอบและสร้าง Chart Control (สมมติว่าชื่อ chartHallVoltageResults)
            Chart chart = this.Controls.Find("chartHallVoltageResults", true).FirstOrDefault() as Chart;
            if (chart == null)
            {
                chart = new Chart();
                chart.Name = "chartHallVoltageResults";
                this.Controls.Add(chart);

                // ตั้งค่า Area ให้กับ Chart
                ChartArea chartArea = new ChartArea("HallVoltageArea");
                chart.ChartAreas.Add(chartArea);

                // ตั้งค่า Title ให้กับ Chart
                Title title = new Title($"Hall Voltage vs. {GlobalSettings.Instance.SourceModeUI}");
                chart.Titles.Add(title);

                // ตั้งค่าแกน X
                chartArea.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
                chartArea.AxisX.LabelStyle.Format = "E2"; // รูปแบบตัวเลขวิทยาศาสตร์

                // ตั้งค่าแกน Y
                chartArea.AxisY.Title = "Hall Voltage (V)";
                chartArea.AxisY.LabelStyle.Format = "E2"; // รูปแบบตัวเลขวิทยาศาสตร์
            }
            else
            {
                // เคลียร์ Series เก่าออกก่อน (ถ้ามี)
                chart.Series.Clear();
            }

            // ดึงข้อมูลการวัดทั้งหมดจาก Singleton Instance
            var measurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurements();

            // วนลูปผ่านแต่ละตำแหน่ง Tuner (สมมติว่ามีตำแหน่ง 1 ถึง 4)
            for (int tunerPosition = 1; tunerPosition <= 4; tunerPosition++)
            {
                // ตรวจสอบว่ามีข้อมูลสำหรับทุกสถานะแม่เหล็กและตำแหน่ง Tuner นี้หรือไม่
                if (measurements.ContainsKey(HallMeasurementState.NoMagneticField) &&
                    measurements[HallMeasurementState.NoMagneticField].ContainsKey(tunerPosition) &&
                    measurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField) &&
                    measurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(tunerPosition) &&
                    measurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField) &&
                    measurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(tunerPosition))
                {
                    var noFieldData = measurements[HallMeasurementState.NoMagneticField][tunerPosition];
                    var northFieldData = measurements[HallMeasurementState.InwardOrNorthMagneticField][tunerPosition];
                    var southFieldData = measurements[HallMeasurementState.OutwardOrSouthMagneticField][tunerPosition];

                    // สร้าง Series สำหรับ Hall Voltage ที่เกิดจากสนามแม่เหล็กเหนือ (North)
                    Series northHallSeries = new Series($"North (Pos {tunerPosition})");
                    northHallSeries.ChartType = SeriesChartType.Line;

                    // สร้าง Series สำหรับ Hall Voltage ที่เกิดจากสนามแม่เหล็กใต้ (South)
                    Series southHallSeries = new Series($"South (Pos {tunerPosition})");
                    southHallSeries.ChartType = SeriesChartType.Line;

                    // คำนวณและเพิ่ม Data Point ให้กับแต่ละ Series
                    for (int i = 0; i < Math.Max(northFieldData.Count, southFieldData.Count); i++)
                    {
                        double sourceNorth = (i < northFieldData.Count) ? northData[i].Source : double.NaN;
                        double readingNorth = (i < northFieldData.Count) ? northData[i].Reading : double.NaN;
                        double vHallNorth = (i < noFieldData.Count && i < northFieldData.Count) ? readingNorth - noFieldData[i].Reading : double.NaN;

                        double sourceSouth = (i < southFieldData.Count) ? southData[i].Source : double.NaN;
                        double readingSouth = (i < southFieldData.Count) ? southData[i].Reading : double.NaN;
                        double vHallSouth = (i < noFieldData.Count && i < southFieldData.Count) ? readingSouth - noFieldData[i].Reading : double.NaN;

                        if (!double.IsNaN(sourceNorth) && !double.IsNaN(vHallNorth))
                        {
                            northHallSeries.Points.AddXY(sourceNorth, vHallNorth);
                        }

                        if (!double.IsNaN(sourceSouth) && !double.IsNaN(vHallSouth))
                        {
                            southHallSeries.Points.AddXY(sourceSouth, vHallSouth);
                        }
                    }

                    // เพิ่ม Series ลงใน Chart
                    chart.Series.Add(northHallSeries);
                    chart.Series.Add(southHallSeries);
                }
            }
        }*/

        private void OnHallVoltageCalculatedHandler(object sender, Dictionary<int, double> hallVoltages)
        {
            LoadMeasurementResults();
        }

        private void OnHallIVHDataCalculatedHandler(object sender, Dictionary<int, List<(double Current, double HallVoltage)>> ivhDataByPosition)
        {
            ChartHallVoltageResults.Series.Clear();

            foreach (var positionData in ivhDataByPosition)
            {
                int position = positionData.Key;
                List<(double Current, double HallVoltage)> dataPoints = positionData.Value;

                Series series = new Series();
                series.ChartType = SeriesChartType.Line;
                series.Name = $"Position {position}";
                series.XValueMember = "Current";
                series.YValueMembers = "HallVoltage";
                series.IsVisibleInLegend = true;
                series.MarkerStyle = MarkerStyle.Circle;

                series.Points.DataBind(dataPoints, "Current", "HallVoltage", null);
                ChartHallVoltageResults.Series.Add(series);
            }

            ChartHallVoltageResults.ChartAreas[0].AxisX.Title = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "Voltage (V)" : "Current (A)";
            ChartHallVoltageResults.ChartAreas[0].AxisY.Title = "Hall Voltage (V)"; // หรือหน่วยที่เหมาะสม

            // อาจมีการตั้งค่าอื่นๆ เพิ่มเติม เช่น Scale ของแกน, Legend, ฯลฯ
        }

        private void OnHallPropertiesCalculatedHandler(object sender, ( double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility) properties)
        {
            TextboxHallCoefficient.Text = properties.HallCoefficient.ToString("E3");
            TextboxSheetConcentration.Text = properties.SheetConcentration.ToString("E3");
            TextboxBulkConcentration.Text = properties.BulkConcentration.ToString("E3");
            TextboxMobility.Text = properties.Mobility.ToString("E3");
        }

        private void UpdateSemiconductorTypeButtons(CollectAndCalculateHallMeasured.SemiconductorType type = CollectAndCalculateHallMeasured.SemiconductorType.Unknown)
        {
            Color nTypeBackColor = Color.Snow;
            Color pTypeBackColor = Color.Snow;
            Color nTypeCharColor = Color.LightGray;
            Color pTypeCharColor = Color.LightGray;

            if (type == CollectAndCalculateHallMeasured.SemiconductorType.NType)
            {
                nTypeCharColor = Color.LightSteelBlue;
                nTypeBackColor = Color.Navy;
            }
            else if (type == CollectAndCalculateHallMeasured.SemiconductorType.PType)
            {
                pTypeCharColor = Color.LightCoral;
                pTypeBackColor = Color.DarkRed;
            }

            IconbuttonNType.BackColor = nTypeBackColor;
            IconbuttonPType.BackColor = pTypeBackColor;
            IconbuttonNType.IconColor = nTypeCharColor;
            IconbuttonPType.IconColor = pTypeCharColor;
        }

        private void OnSemiconductorTypeCalculatedHandler(object sender, CollectAndCalculateHallMeasured.SemiconductorType type)
        {
            // ทำงานบน UI Thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateSemiconductorTypeButtons(type)));
                return;
            }

            UpdateSemiconductorTypeButtons(type);
        }
    }
}

