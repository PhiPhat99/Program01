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
        private const int NumberOfVdPPositions = 8;

        public VdPTotalMeasureValuesForm()
        {
            InitializeComponent();
            InitializeDataGridViewColumns();
            InitializeChartsInTabControl();
            CollectAndCalculateVdPMeasured.Instance.DataUpdated += CollectAndCalculateVdPMeasured_DataUpdated;

            Load += VdPTotalMeasureValuesForm_Load;
            FormClosing += VdPTotalMeasureValuesForm_FormClosing;
        }

        #region Initialization
        private void InitializeDataGridViewColumns()
        {
            if (DatagridviewVdPTotalMesure.Columns.Count == 0)
            {
                for (int i = 1; i <= NumberOfVdPPositions; i++)
                {
                    DatagridviewVdPTotalMesure.Columns.Add($"SourceValue{i - 1}", $"Source {i}");
                    DatagridviewVdPTotalMesure.Columns.Add($"MeasuredValue{i - 1}", $"Measured {i}");
                }
            }
        }

        private Chart GetChartControl(string tabPageName, string chartName)
        {
            if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey(tabPageName) &&
                TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls.ContainsKey(chartName))
            {
                return (Chart)TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls[chartName];
            }
            else
            {
                Debug.WriteLine($"[WARNING] GetChartControl - Chart '{chartName}' not found in TabPage '{tabPageName}'!");
                return null;
            }
        }

        private void InitializeChartsInTabControl()
        {
            Chart totalChart = GetChartControl("TotalMeasuredValuesTabPage", "ChartTotalPositions");
            if (totalChart != null)
            {
                SetupIVChart(totalChart, "I-V Graph of Total Positions");
            }

            for (int i = 1; i <= NumberOfVdPPositions; i++)
            {
                string tabPageName = $"TabpageMeasuredPosition{i}";
                string chartName = $"ChartPosition{i}";
                Chart measuredChart = GetChartControl(tabPageName, chartName);
                if (measuredChart != null)
                {
                    SetupIVChart(measuredChart, $"I-V Graph of Position {i}");
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
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (A)";
                }

                chart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "N6";
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;

                if (GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (A)";
                }

                chart.ChartAreas[0].AxisY.IsLabelAutoFit= false;
                chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N6";

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

            CollectAndCalculateVdPMeasured.Instance.DataUpdated += CollectAndCalculateVdPMeasured_DataUpdated;
        }

        private void VdPTotalMeasureValuesForm_FormClosing(object sender, EventArgs e)
        {
            CollectAndCalculateVdPMeasured.Instance.DataUpdated -= CollectAndCalculateVdPMeasured_DataUpdated;
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
                Debug.WriteLine($"[DEBUG]     Tuner {kvp.Key}: {kvp.Value.Count} measurements");
            }

            int maxSteps = dataToDisplay?.Values.Max(list => list?.Count ?? 0) ?? 0;
            Debug.WriteLine($"[DEBUG] LoadMeasurementData - maxSteps: {maxSteps}");

            DatagridviewVdPTotalMesure.ColumnCount = NumberOfVdPPositions * 2;

            for (int i = 0; i < maxSteps; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(DatagridviewVdPTotalMesure);

                for (int tunerIndex = 1; tunerIndex <= NumberOfVdPPositions; tunerIndex++)
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

            if (dataToDisplay == null || dataToDisplay.Count == 0)
            {
                Debug.WriteLine("[DEBUG] LoadMeasurementData - No data to display in DataGridView");
            }
        }

        private void LoadMeasurementDataForCharts()
        {
            Debug.WriteLine("[DEBUG] LoadMeasurementDataForCharts called");
            Dictionary<int, List<(double Source, double Reading)>> AllMeasurements = CollectAndCalculateVdPMeasured.Instance.GetAllMeasurementsByTuner();
            Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - Total Measurements Count: {AllMeasurements.Count}");

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
                                series.XValueMember = "Source";
                                series.YValueMembers = "Reading";
                                series.Points.DataBind(AllMeasurements[i].Select(data => new { data.Source, data.Reading }).ToList(), "Source", "Reading", null);
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

                        TotalChart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})"; // กำหนดชื่อแกน Y
                        TotalChart.ChartAreas[0].AxisX.LabelStyle.Format = "N6";
                        TotalChart.ChartAreas[0].AxisX.Interval = 0;
                        TotalChart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;

                        TotalChart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})"; // กำหนดชื่อแกน X
                        TotalChart.ChartAreas[0].AxisY.Interval = 0;
                        TotalChart.ChartAreas[0].AxisY.LabelStyle.Format = "N6";
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

            if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey("TabpageTotalVdPMeasuredPosition") && TabcontrolVdPTotalCharts.TabPages["TabpageTotalVdPMeasuredPosition"].Controls.ContainsKey("ChartTotalPositions"))
            {
                Chart TotalChart = (Chart)TabcontrolVdPTotalCharts.TabPages["TabpageTotalVdPMeasuredPosition"].Controls["ChartTotalPositions"];
                
                if (TotalChart != null && TotalChart.ChartAreas.Count > 0)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        string tabPageName = $"TabpageVdPMeasuredPosition{i}";
                        string chartName = $"ChartPosition{i}";

                        if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey(tabPageName) && TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls.ContainsKey(chartName) && AllMeasurements.ContainsKey(i))
                        {
                            Chart measuredChart = (Chart)TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls[chartName];

                            if (measuredChart != null && measuredChart.ChartAreas.Count > 0)
                            {
                                measuredChart.ChartAreas[0].AxisX.Title = TotalChart.ChartAreas[0].AxisX.Title;
                                measuredChart.ChartAreas[0].AxisX.LabelStyle.Format = TotalChart.ChartAreas[0].AxisX.LabelStyle.Format;
                                measuredChart.ChartAreas[0].AxisX.Interval = TotalChart.ChartAreas[0].AxisX.Interval;
                                measuredChart.ChartAreas[0].AxisX.LabelStyle.Angle = TotalChart.ChartAreas[0].AxisX.LabelStyle.Angle;

                                measuredChart.ChartAreas[0].AxisY.Title = TotalChart.ChartAreas[0].AxisY.Title;
                                measuredChart.ChartAreas[0].AxisY.LabelStyle.Format = TotalChart.ChartAreas[0].AxisY.LabelStyle.Format;
                                measuredChart.ChartAreas[0].AxisY.Interval = TotalChart.ChartAreas[0].AxisY.Interval;
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
                Invoke((MethodInvoker)LoadMeasurementDataForCharts);
            }
            else
            {
                LoadMeasurementDataForCharts();
            }
        }
        #endregion
    }
}