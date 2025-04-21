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
                string tabPageName = $"MeasuredValueP{i}TabPage";
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
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (A)";
                }

                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
                chart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;

                if (GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (V)";
                }
                else
                {
                    chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (A)";
                }

                chart.ChartAreas[0].AxisY.LabelStyle.Angle = 0;
                chart.ChartAreas[0].AxisY.IsLabelAutoFit= false;
                chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

                chart.ChartAreas[0].AxisX.LabelStyle.Format = "0.#####";
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "0.#####";

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

            if (TabcontrolVdPTotalCharts != null && TabcontrolVdPTotalCharts.TabPages.ContainsKey("TotalMeasuredValuesTabPage"))
            {
                TabPage totalTabPage = TabcontrolVdPTotalCharts.TabPages["TotalMeasuredValuesTabPage"];
                if (totalTabPage != null && totalTabPage.Controls.ContainsKey("ChartTotalPositions"))
                {
                    Chart TotalChart = (Chart)totalTabPage.Controls["ChartTotalPositions"];

                    if (TotalChart != null && TotalChart.Series != null)
                    {
                        for (int i = 1; i <= 8; i++)
                        {
                            string seriesName = $"Position {i}";

                            if (AllMeasurements.ContainsKey(i) && AllMeasurements[i] != null && AllMeasurements[i].Count > 0)
                            {
                                Series series = TotalChart.Series[seriesName];

                                if (series != null)
                                {
                                    series.XValueMember = "Source";
                                    series.YValueMembers = "Reading";
                                    series.Points.DataBind(AllMeasurements[i].Select(data => new {data.Source,data.Reading }).ToList(), "Source", "Reading", null);
                                }
                                else
                                {
                                    Debug.WriteLine($"[WARNING] LoadMeasurementDataForCharts - Series '{seriesName}' is null.");
                                }
                            }
                            else
                            {
                                Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - No data or Series key not found for Position {i}. AllMeasurements.ContainsKey({i}) = {AllMeasurements.ContainsKey(i)}");
                            }
                        }

                        if (TotalChart.Legends.Count > 0)
                        {
                            TotalChart.Legends[0].Enabled = true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - TotalChart or TotalChart.Series is null");
                    }
                }
                else
                {
                    Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - ChartTotalPositions not found");
                }
            }
            else
            {
                Debug.WriteLine("[WARNING] LoadMeasurementDataForCharts - TotalMeasuredValuesTabPage not found");
            }

            for (int i = 1; i <= 8; i++)
            {
                string tabPageName = $"MeasuredValueP{i}TabPage";
                string chartName = $"ChartPosition{i}";

                if (TabPageExists(TabcontrolVdPTotalCharts, tabPageName) && TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls.ContainsKey(chartName) && AllMeasurements.ContainsKey(i))
                {
                    Chart measuredChart = (Chart)TabcontrolVdPTotalCharts.TabPages[tabPageName].Controls[chartName];
                    if (measuredChart != null && measuredChart.Series.Count > 0 && AllMeasurements[i] != null && AllMeasurements[i].Count > 0)
                    {
                        Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - Binding data for Position {i} to {chartName}");
                        measuredChart.DataSource = AllMeasurements[i].Select(data => new { data.Source, data.Reading }).ToList();
                        measuredChart.DataBind();
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] LoadMeasurementDataForCharts - No data or Chart/Series issue for Position {i} in individual chart.");
                        if (measuredChart != null && measuredChart.Series.Count > 0)
                        {
                            measuredChart.Series[0].Points.Clear();
                        }
                    }
                }
            }
        }

        private bool TabPageExists(TabControl tabControl, string tabPageName)
        {
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                if (tabPage.Name == tabPageName)
                {
                    return true;
                }
            }
            return false;
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