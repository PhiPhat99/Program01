using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace Program01
{
    public partial class VdPTotalMeasureValuesForm : Form
    {
        public VdPTotalMeasureValuesForm()
        {
            InitializeComponent();
            InitializeDataGridViewColumns();
            InitializeChartsInTabControl();

            CollectAndCalculateVdPMeasured.Instance.DataUpdated += CollectAndCalculateVdPMeasured_DataUpdated;
            this.Load += VdPTotalMeasureValuesForm_Load;
        }

        #region Initialization
        private void InitializeDataGridViewColumns()
        {
            if (DatagridviewVdPTotalMesure.Columns.Count == 0)
            {
                for (int i = 1; i <= 8; i++)
                {
                    DatagridviewVdPTotalMesure.Columns.Add($"SourceValue{i - 1}", $"Source {i}");
                    DatagridviewVdPTotalMesure.Columns.Add($"MeasuredValue{i - 1}", $"Measured {i}");
                }
            }
        }

        private void InitializeChartsInTabControl()
        {
            if (TabcontrolVdPTotalCharts.TabPages.ContainsKey("TotalMeasuredValuesTabPage") &&
                TabcontrolVdPTotalCharts.TabPages["TotalMeasuredValuesTabPage"].Controls.ContainsKey("ChartTotalPositions"))
            {
                Chart totalChart = (Chart)TabcontrolVdPTotalCharts.TabPages["TotalMeasuredValuesTabPage"].Controls["ChartTotalPositions"];
                SetupIVChart(totalChart, "I-V Graph of Total Positions");
            }
            else
            {
                Debug.WriteLine("[WARNING] InitializeChartsInTabControl - ChartTotalPositions not found in TotalMeasuredValuesTabPage!");
            }

            for (int i = 1; i <= 8; i++)
            {
                string tabPageName = $"TabpageMeasuredPosition{i}";
                string chartName = $"ChartPosition{i}";

                if (TabcontrolVdPTotalCharts.TabPages.ContainsKey(tabPageName) &&
                    TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls.ContainsKey(chartName))
                {
                    Chart measuredChart = (Chart)TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls[chartName];
                    SetupIVChart(measuredChart, $"I-V Graph of Position {i}");
                }
                else
                {
                    Debug.WriteLine($"[WARNING] InitializeChartsInTabControl - {chartName} not found in {tabPageName}!");
                }
            }

            Debug.WriteLine("[DEBUG] InitializeChartsInTabControl() - Charts in TabControl initialized (using Designer settings).");
        }

        private void SetupIVChart(Chart chart, string title)
        {
            if (chart != null && chart.ChartAreas.Count > 0)
            {
                chart.Titles.Clear();
                chart.Titles.Add(title);

                if (GlobalSettings.Instance.SourceModeUI == "Voltage")
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.SourceModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.SourceModeUI} (A)";
                }

                chart.ChartAreas[0].AxisY.IsLabelAutoFit = false;
                chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N6";

                if (GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.MeasureModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.MeasureModeUI} (A)";
                }

                chart.ChartAreas[0].AxisX.IsLabelAutoFit= false;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "N6";

                chart.Invalidate();
            }
            else
            {
                Debug.WriteLine($"[WARNING] SetupIVChart - Chart or ChartArea is null for {title}!");
            }
        }

        private void VdPTotalMeasureValuesForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("[DEBUG] VdPTotalMeasureValuesForm_Load called");
            LoadMeasurementData();
            LoadMeasurementDataForCharts();
        }
        #endregion

        #region Data Loading and Display
        public void LoadMeasurementData()
        {
            LoadMeasurementData(null);
        }

        public void LoadMeasurementData(CollectAndCalculateVdPMeasured measurementData = null)
        {
            DatagridviewVdPTotalMesure.Rows.Clear();

            Dictionary<int, List<(double Source, double Reading)>> dataToDisplay = CollectAndCalculateVdPMeasured.Instance.GetAllMeasurementsByTuner();

            Debug.WriteLine("[DEBUG] LoadMeasurementData - Data from CollectVdPMeasured:");
            foreach (var kvp in dataToDisplay)
            {
                Debug.WriteLine($"[DEBUG]    Tuner {kvp.Key}: {kvp.Value.Count} measurements");
            }

            int maxSteps = 0;
            if (dataToDisplay != null && dataToDisplay.Count > 0)
            {
                for (int i = 1; i <= 8; i++)
                {
                    maxSteps = Math.Max(maxSteps, dataToDisplay.ContainsKey(i) ? dataToDisplay[i].Count : 0);
                }

                Debug.WriteLine($"[DEBUG] LoadMeasurementData - maxSteps: {maxSteps}");

                DatagridviewVdPTotalMesure.ColumnCount = 16;

                for (int i = 0; i < maxSteps; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(DatagridviewVdPTotalMesure);

                    for (int tunerIndex = 1; tunerIndex <= 8; tunerIndex++)
                    {
                        if (dataToDisplay.ContainsKey(tunerIndex) && dataToDisplay[tunerIndex].Count > i)
                        {
                            row.Cells[(tunerIndex - 1) * 2].Value = dataToDisplay[tunerIndex][i].Source;
                            row.Cells[(tunerIndex - 1) * 2 + 1].Value = dataToDisplay[tunerIndex][i].Reading;
                        }
                        else
                        {
                            row.Cells[(tunerIndex - 1) * 2].Value = "";
                            row.Cells[(tunerIndex - 1) * 2 + 1].Value = "";
                        }
                    }

                    DatagridviewVdPTotalMesure.Rows.Add(row);
                }
            }
            else
            {
                Debug.WriteLine("[DEBUG] LoadMeasurementData - No data to display in DataGridView");
            }
        }

        private void LoadMeasurementDataForCharts()
        {
            Debug.WriteLine("[DEBUG] LoadMeasurementDataForCharts called");
            Dictionary<int, List<(double Source, double Reading)>> AllMeasurements = CollectAndCalculateVdPMeasured.Instance.GetAllMeasurementsByTuner();
            Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - Total Measurements Count: {AllMeasurements.Count}");

            // --- ส่วนการจัดการ Chart รวม (Total Chart) ---
            if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey("TabpageTotalVdPMeasuredPosition"))
            {
                TabPage totalTabPage = TabcontrolVdPTotalCharts.TabPages["TabpageTotalVdPMeasuredPosition"];
                if (totalTabPage != null && totalTabPage.Controls.ContainsKey("ChartTotalPositions"))
                {
                    Chart TotalChart = (Chart)totalTabPage.Controls["ChartTotalPositions"];

                    if (TotalChart != null && TotalChart.Series != null && TotalChart.Series.Count == 8) // ตรวจสอบว่ามี Series ครบ 8 หรือไม่
                    {
                        for (int i = 1; i <= 8; i++)
                        {
                            if (AllMeasurements.ContainsKey(i) && AllMeasurements[i] != null && AllMeasurements[i].Count > 0)
                            {
                                Series series = TotalChart.Series[i - 1];
                                series.XValueMember = "Reading"; // Reading จะอยู่บนแกน X
                                series.YValueMembers = "Source"; // Source จะอยู่บนแกน Y
                                series.Points.DataBind(AllMeasurements[i].Select(data => new { data.Reading, data.Source }).ToList(), "Reading", "Source", null);
                            }
                            else
                            {
                                Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - No data for Position {i}. Clearing Series at index {i - 1}.");
                                TotalChart.Series[i - 1].Points.Clear();
                            }
                        }

                        if (TotalChart.Legends.Count > 0)
                        {
                            TotalChart.Legends[0].Enabled = true;
                        }

                        string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
                        string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";

                        // ตัวอย่างการปรับแต่งแกน X ของ Chart รวม
                        TotalChart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})"; // กำหนดชื่อแกน Y
                        TotalChart.ChartAreas[0].AxisY.LabelStyle.Format = "N5";
                        TotalChart.ChartAreas[0].AxisY.Interval = 0;

                        // ตัวอย่างการปรับแต่งแกน Y ของ Chart รวม
                        TotalChart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})"; // กำหนดชื่อแกน X
                        TotalChart.ChartAreas[0].AxisX.Interval = 0;
                        TotalChart.ChartAreas[0].AxisX.LabelStyle.Format = "N5";
                    }
                    else
                    {
                        Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - TotalChart or TotalChart.Series is null or does not have 8 Series.");
                    }
                }
                else
                {
                    Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - ChartTotalPositions not found");
                }
            }
            else
            {
                Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - TabpageTotalVdPMeasuredPosition not found");
            }

            // --- ส่วนการจัดการ Chart แยกตาม Position และ TabPage ---
            if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey("TabpageTotalVdPMeasuredPosition") &&
                TabcontrolVdPTotalCharts.TabPages["TabpageTotalVdPMeasuredPosition"].Controls.ContainsKey("ChartTotalPositions"))
            {
                Chart TotalChart = (Chart)TabcontrolVdPTotalCharts.TabPages["TabpageTotalVdPMeasuredPosition"].Controls["ChartTotalPositions"];
                if (TotalChart != null && TotalChart.ChartAreas.Count > 0)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        string tabPageName = $"TabpageVdPMeasuredPosition{i}";
                        string chartName = $"ChartPosition{i}";

                        if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey(tabPageName) &&
                            TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls.ContainsKey(chartName) && AllMeasurements.ContainsKey(i))
                        {
                            Chart measuredChart = (Chart)TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls[chartName];

                            if (measuredChart != null && measuredChart.ChartAreas.Count > 0)
                            {
                                // คัดลอกการตั้งค่าแกน X จาก TotalChart
                                measuredChart.ChartAreas[0].AxisX.Title = TotalChart.ChartAreas[0].AxisX.Title;
                                measuredChart.ChartAreas[0].AxisX.LabelStyle.Format = TotalChart.ChartAreas[0].AxisX.LabelStyle.Format;
                                measuredChart.ChartAreas[0].AxisX.Interval = TotalChart.ChartAreas[0].AxisX.Interval;
                                // คัดลอก Properties อื่นๆ ของแกน X ที่คุณต้องการให้เหมือนกัน

                                // คัดลอกการตั้งค่าแกน Y จาก TotalChart
                                measuredChart.ChartAreas[0].AxisY.Title = TotalChart.ChartAreas[0].AxisY.Title;
                                measuredChart.ChartAreas[0].AxisY.LabelStyle.Format = TotalChart.ChartAreas[0].AxisY.LabelStyle.Format;
                                measuredChart.ChartAreas[0].AxisY.Interval = TotalChart.ChartAreas[0].AxisY.Interval;
                                // คัดลอก Properties อื่นๆ ของแกน Y ที่คุณต้องการให้เหมือนกัน
                            }

                            if (measuredChart != null && measuredChart.Series.Count > 0 && AllMeasurements[i] != null && AllMeasurements[i].Count > 0)
                            {
                                Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - Binding data for Position {i} to {chartName} in {tabPageName}");
                                measuredChart.DataSource = AllMeasurements[i].Select(data => new { data.Source, data.Reading }).ToList();
                                measuredChart.DataBind();
                            }
                            else
                            {
                                Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - No data or Chart/Series issue for Position {i} in {tabPageName}/{chartName}.");
                                if (measuredChart != null && measuredChart.Series.Count > 0)
                                {
                                    measuredChart.Series[0].Points.Clear();
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - TabPage '{tabPageName}' or Chart '{chartName}' not found for Position {i}. AllMeasurements.ContainsKey({i}) = {AllMeasurements.ContainsKey(i)}");
                        }
                    }
                }
            }
        }
        #endregion

        #region Event Handlers

        private void CollectAndCalculateVdPMeasured_DataUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)LoadMeasurementDataForCharts); // เรียกเมธอดแยกสำหรับโหลด Chart
            }
            else
            {
                LoadMeasurementDataForCharts(); // เรียกเมธอดแยกสำหรับโหลด Chart
            }
        }

        #endregion
    }
}