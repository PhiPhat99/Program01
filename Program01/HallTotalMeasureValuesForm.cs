using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class HallTotalMeasureValuesForm : Form
    {
        private readonly BindingSource HallOutVoltageBindingSource = new BindingSource();
        private readonly BindingSource HallInSouthVoltageBindingSource = new BindingSource();
        private readonly BindingSource HallInNorthVoltageBindingSource = new BindingSource();

        public HallTotalMeasureValuesForm()
        {
            InitializeComponent();
            InitializeAllDataGridViewColumns();
            InitializeChartsInTabControls();

            CollectAndCalculateHallMeasured.Instance.DataUpdated += CollectAndCalculateHallMeasured_DataUpdated;
            this.Load += HallTotalMeasureValuesForm_Load;
            this.FormClosing += HallTotalMeasureValuesForm_FormClosing;

            // Initialize DataTables for BindingSources
            HallOutVoltageBindingSource.DataSource = CreateHallOutDataTable();
            DatagridviewHallOutVoltageTotalMeasured.DataSource = HallOutVoltageBindingSource;

            HallInSouthVoltageBindingSource.DataSource = CreateHallInSouthDataTable();
            DatagridviewHallSouthVoltageTotalMeasured.DataSource = HallInSouthVoltageBindingSource;

            HallInNorthVoltageBindingSource.DataSource = CreateHallInNorthDataTable();
            DatagridviewHallNorthVoltageTotalMeasured.DataSource = HallInNorthVoltageBindingSource;
        }

        private DataTable CreateHallOutDataTable()
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < 8; i++)
            {
                dataTable.Columns.Add($"Source{i}", typeof(double));
                dataTable.Columns.Add($"Measured{i}", typeof(double));
            }
            return dataTable;
        }

        private DataTable CreateHallInSouthDataTable()
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < 8; i++)
            {
                dataTable.Columns.Add($"Source{i}", typeof(double));
                dataTable.Columns.Add($"Measured{i}", typeof(double));
            }
            return dataTable;
        }

        private DataTable CreateHallInNorthDataTable()
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < 8; i++)
            {
                dataTable.Columns.Add($"Source{i}", typeof(double));
                dataTable.Columns.Add($"Measured{i}", typeof(double));
            }
            return dataTable;
        }

        private void InitializeAllDataGridViewColumns()
        {
            InitializeDataGridViewColumns(DatagridviewHallOutVoltageTotalMeasured);
            InitializeDataGridViewColumns(DatagridviewHallSouthVoltageTotalMeasured);
            InitializeDataGridViewColumns(DatagridviewHallNorthVoltageTotalMeasured);
        }

        private void InitializeDataGridViewColumns(DataGridView dataGridView)
        {
            dataGridView.AutoGenerateColumns = false;
        }

        private void InitializeChartsInTabControls()
        {
            InitializeChartsForTab(TabcontrolHallTotalChart, "TabpageHallOutTotalChart", "ChartHallOutMeasuredPosition");
            InitializeChartsForTab(TabcontrolHallTotalChart, "TabpageHallInTotalChart", "ChartHallInSouthMeasuredPosition", "TabpageHallInSouthTotalChart");
            InitializeChartsForTab(TabcontrolHallTotalChart, "TabpageHallInTotalChart", "ChartHallInNorthMeasuredPosition", "TabpageHallInNorthTotalChart");

            Debug.WriteLine("[DEBUG] InitializeChartsInTabControl() - Charts in TabControl initialized (using Designer settings).");
        }

        private void InitializeChartsForTab(TabControl mainTabControl, string parentTabPageName, string chartNamePrefix, string childTabPageName = null)
        {
            if (mainTabControl.TabPages.ContainsKey(parentTabPageName))
            {
                TabControl targetTabControl = mainTabControl.TabPages[parentTabPageName].Controls.OfType<TabControl>().FirstOrDefault();

                if (targetTabControl != null && childTabPageName != null && targetTabControl.TabPages.ContainsKey(childTabPageName))
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        string tabPageName = $"TabpageV_{(childTabPageName.Contains("South") ? "hiSouth" : "hiNorth")}TotalMP{i}";
                        string chartName = $"{chartNamePrefix}{i}";
                        Chart measuredChart = targetTabControl.TabPages[childTabPageName].Controls.Find(chartName, true).OfType<Chart>().FirstOrDefault();

                        if (measuredChart != null)
                        {
                            SetupIVChart(measuredChart, $"I-V Graph of Positions {(i * 2 - 1)} & {i * 2}");
                        }
                        else
                        {
                            Debug.WriteLine($"[WARNING] InitializeChartsForTab - {chartName} not found in {tabPageName} ({childTabPageName})!");
                        }
                    }
                }
                else if (targetTabControl == null && childTabPageName == null) // กรณี Hall Out ที่ไม่มี TabControl ซ้อน
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        string tabPageName = $"TabpageV_hoTotalMP{i}";
                        string chartName = $"{chartNamePrefix}{i}";
                        Chart measuredChart = mainTabControl.TabPages[parentTabPageName].Controls.Find(chartName, true).OfType<Chart>().FirstOrDefault();

                        if (measuredChart != null)
                        {
                            SetupIVChart(measuredChart, $"I-V Graph of Positions {(i * 2 - 1)} & {i * 2}");
                        }
                        else
                        {
                            Debug.WriteLine($"[WARNING] InitializeChartsForTab - {chartName} not found in {tabPageName} ({parentTabPageName})!");
                        }
                    }
                }
                else if (childTabPageName != null)
                {
                    Debug.WriteLine($"[WARNING] InitializeChartsForTab - Child TabPage '{childTabPageName}' not found in '{parentTabPageName}'!");
                }
                else if (targetTabControl == null)
                {
                    Debug.WriteLine($"[WARNING] InitializeChartsForTab - Inner TabControl not found in '{parentTabPageName}'!");
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] InitializeChartsForTab - Parent TabPage '{parentTabPageName}' not found!");
            }
        }

        private void SetupIVChart(Chart chart, string title)
        {
            if (chart != null && chart.ChartAreas.Count > 0)
            {
                chart.Titles.Clear();
                chart.Titles.Add(title);

                string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
                string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";

                chart.ChartAreas[0].AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
                chart.ChartAreas[0].AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})";

                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                chart.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";

                chart.ChartAreas[0].AxisY.LabelStyle.Angle = 0;
                chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
                chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "0.###";

                chart.Invalidate();
            }
            else
            {
                Debug.WriteLine($"[WARNING] SetupIVChart - Chart or ChartArea is null for {title}!");
            }
        }

        public void LoadHallMeasurementData()
        {
            var hallOutMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("HallOut");
            UpdateHallDataGridView(hallOutMeasurements, HallOutVoltageBindingSource);

            var hallInSouthMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("HallInSouth");
            UpdateHallDataGridView(hallInSouthMeasurements, HallInSouthVoltageBindingSource);

            var hallInNorthMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("HallInNorth");
            UpdateHallDataGridView(hallInNorthMeasurements, HallInNorthVoltageBindingSource);

            // Update Charts ตามข้อมูลแต่ละประเภท (ถ้ามี)
        }

        private void UpdateHallDataGridView(Dictionary<int, List<(double Source, double Reading, string MeasurementType)>> measurements, BindingSource bindingSource)
        {
            DataTable dataTable = bindingSource.DataSource as DataTable;
            dataTable.Rows.Clear();

            foreach (var PositionData in measurements)
            {
                int PositionNumber = PositionData.Key;

                if (PositionNumber >= 1 && PositionNumber <= 8)
                {
                    foreach (var measurement in PositionData.Value)
                    {
                        DataRow rowData = dataTable.NewRow();
                        rowData[$"Source{PositionNumber - 1}"] = measurement.Source;
                        rowData[$"Measured{PositionNumber - 1}"] = measurement.Reading;
                        dataTable.Rows.Add(rowData);
                    }
                }
            }

            bindingSource.ResetBindings(false);
        }

        private void CollectAndCalculateHallMeasured_DataUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(LoadHallMeasurementData));
            }
            else
            {
                LoadHallMeasurementData();
            }
        }

        private void HallTotalMeasureValuesForm_Load(object sender, EventArgs e)
        {
            LoadHallMeasurementData();
        }

        private void HallTotalMeasureValuesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectAndCalculateHallMeasured.Instance.DataUpdated -= CollectAndCalculateHallMeasured_DataUpdated;
        }
    }
}