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
        private List<DataGridView> _hallDataGridViews;
        private Dictionary<string, BindingSource> _hallBindingSources = new Dictionary<string, BindingSource>();
        private TabControl _mainTabControlHallInTotalChart; // เพิ่ม Field สำหรับ Main TabControl

        private string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
        private string measureUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";

        public HallTotalMeasureValuesForm()
        {
            InitializeComponent();
            _mainTabControlHallInTotalChart = this.Controls.Find("TabcontrolHallInTotalChart", true).OfType<TabControl>().FirstOrDefault(); // Initialize Main TabControl
            InitializeHallBindingSources();
            InitializeHallDataGridViewList();
            InitializeAllDataGridViewColumns();
            InitializeCharts();
            CollectAndCalculateHallMeasured.Instance.DataUpdated += CollectAndCalculateHallMeasured_DataUpdated;
            Load += HallTotalMeasureValuesForm_Load;
            FormClosing += HallTotalMeasureValuesForm_FormClosing;
        }

        private void InitializeHallBindingSources()
        {
            _hallBindingSources["OutVoltage"] = CreateAndSetDataSource();
            BindingSourceHallOutVoltage.DataSource = _hallBindingSources["OutVoltage"];

            _hallBindingSources["InSouthVoltage"] = CreateAndSetDataSource();
            BindingSourceHallInSouthVoltage.DataSource = _hallBindingSources["InSouthVoltage"];

            _hallBindingSources["InNorthVoltage"] = CreateAndSetDataSource();
            BindingSourceHallInNorthVoltage.DataSource = _hallBindingSources["InNorthVoltage"];
        }

        private BindingSource CreateAndSetDataSource()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = CreateHallDataTable();
            return bs;
        }

        private DataTable CreateHallDataTable()
        {
            DataTable dataTable = new DataTable();
            for (int i = 1; i <= 4; i++)
            {
                dataTable.Columns.Add($"Source{i}", typeof(double));
                dataTable.Columns.Add($"Measured{i}", typeof(double));
            }
            return dataTable;
        }

        private void InitializeHallDataGridViewList()
        {
            _hallDataGridViews = new List<DataGridView>
            {
                DatagridviewHallOutVoltageTotalMeasured,
                DatagridviewHallSouthVoltageTotalMeasured,
                DatagridviewHallNorthVoltageTotalMeasured
            };
        }

        private string GetSourceColumnHeaderText(int index)
        {
            string currentUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
            return $"Source {index} ({currentUnit})";
        }

        private string GetMeasuredColumnHeaderText(int index)
        {
            string currentUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
            return $"Measured {index} ({currentUnit})";
        }

        private void InitializeAllDataGridViewColumns()
        {
            foreach (var dataGridView in _hallDataGridViews)
            {
                InitializeDataGridViewColumns(dataGridView);
            }
        }

        private void InitializeDataGridViewColumns(DataGridView dataGridView)
        {
            dataGridView.Columns.Clear();

            for (int i = 1; i <= 4; i++)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Source{i}",
                    HeaderText = GetSourceColumnHeaderText(i),
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6" }
                });

                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Measured{i}",
                    HeaderText = GetMeasuredColumnHeaderText(i),
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6" }
                });
            }

            dataGridView.AutoGenerateColumns = false;
        }

        private void InitializeCharts()
        {
            // Initialize Individual Charts
            for (int i = 1; i <= 4; i++)
            {
                string southTabPageName = $"TabpageV_hiSouthMP{i}";
                string southChartName = $"ChartHallInSouthMeasurementPosition{i}";
                string northTabPageName = $"TabpageV_hiNorthMP{i}";
                string northChartName = $"ChartHallInNorthMeasurementPosition{i}";

                InitializeIndividualChart(southTabPageName, southChartName, i);
                InitializeIndividualChart(northTabPageName, northChartName, i);
            }
        }

        private void InitializeIndividualChart(string tabPageName, string chartName, int position)
        {
            if (_mainTabControlHallInTotalChart != null && _mainTabControlHallInTotalChart.TabPages.ContainsKey(tabPageName) &&
                _mainTabControlHallInTotalChart.TabPages[tabPageName].Controls.ContainsKey(chartName))
            {
                Chart measuredChart = (Chart)_mainTabControlHallInTotalChart.TabPages[tabPageName].Controls[chartName];
                if (measuredChart != null)
                {
                    measuredChart.Series.Clear();
                    Series series = new Series($"Position {position}");
                    series.ChartType = SeriesChartType.Line;
                    measuredChart.Series.Add(series);
                    series.XValueMember = "Reading";
                    series.YValueMembers = "Source";

                    // Copy Axis settings from the first South chart.
                    if (_mainTabControlHallInTotalChart != null && _mainTabControlHallInTotalChart.TabPages.ContainsKey("TabpageV_hiSouthMP1") &&
                        _mainTabControlHallInTotalChart.TabPages["TabpageV_hiSouthMP1"].Controls.ContainsKey("ChartHallInSouthMeasurementPosition1"))
                    {
                        Chart firstSouthChart = (Chart)_mainTabControlHallInTotalChart.TabPages["TabpageV_hiSouthMP1"].Controls["ChartHallInSouthMeasurementPosition1"];
                        if (firstSouthChart != null && firstSouthChart.ChartAreas.Count > 0)
                        {
                            CopyAxisSettings(firstSouthChart.ChartAreas[0].AxisX, measuredChart.ChartAreas[0].AxisX);
                            CopyAxisSettings(firstSouthChart.ChartAreas[0].AxisY, measuredChart.ChartAreas[0].AxisY);
                        }
                    }
                    //Or Copy Axis settings from the first North Chart.
                    else if (_mainTabControlHallInTotalChart != null && _mainTabControlHallInTotalChart.TabPages.ContainsKey("TabpageV_hiNorthMP1") &&
                             _mainTabControlHallInTotalChart.TabPages["TabpageV_hiNorthMP1"].Controls.ContainsKey("ChartHallInNorthMeasurementPosition1"))
                    {
                        Chart firstNorthChart = (Chart)_mainTabControlHallInTotalChart.TabPages["TabpageV_hiNorthMP1"].Controls["ChartHallInNorthMeasurementPosition1"];
                        if (firstNorthChart != null && firstNorthChart.ChartAreas.Count > 0)
                        {
                            CopyAxisSettings(firstNorthChart.ChartAreas[0].AxisX, measuredChart.ChartAreas[0].AxisX);
                            CopyAxisSettings(firstNorthChart.ChartAreas[0].AxisY, measuredChart.ChartAreas[0].AxisY);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[WARNING] InitializeIndividualChart: Chart {chartName} not found");
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] InitializeIndividualChart: TabPage {tabPageName} not found");
            }
        }

        private void LoadHallDataForIndividualCharts(Dictionary<int, List<(double Source, double Reading)>> measurements, string state)
        {
            if (state != "No Magnetic Field" && measurements != null)
            {
                for (int i = 1; i <= 4; i++)
                {
                    string southTabPageName = $"TabpageV_hiSouthMP{i}";
                    string southChartName = $"ChartHallInSouthMeasurementPosition{i}";
                    string northTabPageName = $"TabpageV_hiNorthMP{i}";
                    string northChartName = $"ChartHallInNorthMeasurementPosition{i}";

                    UpdateIndividualChartData(measurements, i, southTabPageName, southChartName);
                    UpdateIndividualChartData(measurements, i, northTabPageName, northChartName);
                }
            }
        }

        public void LoadAllHallData(Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData)
        {
            Debug.WriteLine("[DEBUG] LoadAllHallData - Received all Hall measurement data.");

            LoadHallDataForState(HallMeasurementState.NoMagneticField, allHallData, BindingSourceHallOutVoltage);
            LoadHallDataForState(HallMeasurementState.OutwardOrSouthMagneticField, allHallData, BindingSourceHallInSouthVoltage);
            LoadHallDataForState(HallMeasurementState.InwardOrNorthMagneticField, allHallData, BindingSourceHallInNorthVoltage);
        }

        private void LoadHallDataForState(HallMeasurementState state, Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData, BindingSource bindingSource)
        {
            if (allHallData.TryGetValue(state, out var data))
            {
                DataTable dataTable = ConvertHallDataToDataTable(data);
                UpdateHallDataGridView(dataTable, bindingSource);

                string mainTabControlName = "TabcontrolHallInTotalChart"; // ใช้ TabControl หลัก
                                                                          // กำหนดชื่อ TabPage และ Chart Prefix สำหรับ South และ North
                string southTabPageName = "TabpageHallInSouthTotalChart";
                string southChartPrefix = "ChartHallInSouthMeasurementPosition";
                string northTabPageName = "TabpageHallInNorthTotalChart";
                string northChartPrefix = "ChartHallInNorthMeasurementPosition";

                if (state == HallMeasurementState.OutwardOrSouthMagneticField)
                {
                    UpdateHallInChart(data, bindingSource, mainTabControlName, southTabPageName, "TabcontrolHallInSouthTotalChart", southChartPrefix);
                    Debug.WriteLine("[DEBUG] LoadHallDataForState - Loaded and updated South Magnetic Field data and charts.");
                }
                else if (state == HallMeasurementState.InwardOrNorthMagneticField)
                {
                    UpdateHallInChart(data, bindingSource, mainTabControlName, northTabPageName, "TabcontrolHallInNorthTotalChart", northChartPrefix);
                    Debug.WriteLine("[DEBUG] LoadHallDataForState - Loaded and updated North Magnetic Field data and charts.");
                }
                else if (state == HallMeasurementState.NoMagneticField)
                {
                    Debug.WriteLine("[DEBUG] LoadHallDataForState - Loaded and updated No Magnetic Field data (Chart update skipped).");
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] LoadHallDataForState - No data found for state: {state}");
                UpdateHallDataGridView(CreateHallDataTable(), bindingSource);
                ClearHallInCharts(bindingSource);
            }
        }

        private DataTable ConvertHallDataToDataTable(Dictionary<int, List<(double Source, double Reading)>> data)
        {
            DataTable dataTable = CreateHallDataTable().Clone(); // Create a new DataTable with the same structure

            if (data != null && data.Any())
            {
                int maxRows = data.Max(kvp => kvp.Value.Count);
                for (int i = 0; i < maxRows; i++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int j = 1; j <= 4; j++)
                    {
                        if (data.ContainsKey(j) && i < data[j].Count)
                        {
                            row[$"Source{j}"] = Math.Round(data[j][i].Source, 5);
                            row[$"Measured{j}"] = Math.Round(data[j][i].Reading, 5);
                        }
                        else
                        {
                            row[$"Source{j}"] = DBNull.Value;
                            row[$"Measured{j}"] = DBNull.Value;
                        }
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }

        private void UpdateIndividualChartData(Dictionary<int, List<(double Source, double Reading)>> measurements, int position, string tabPageName, string chartName)
        {
            if (_mainTabControlHallInTotalChart != null && _mainTabControlHallInTotalChart.TabPages.ContainsKey(tabPageName) &&
                _mainTabControlHallInTotalChart.TabPages[tabPageName].Controls.ContainsKey(chartName))
            {
                Chart measuredChart = (Chart)_mainTabControlHallInTotalChart.TabPages[tabPageName].Controls[chartName];
                if (measuredChart != null && measuredChart.Series.Count > 0)
                {
                    Series series = measuredChart.Series[0];
                    if (measurements.ContainsKey(position) && measurements[position] != null && measurements[position].Count > 0)
                    {
                        Debug.WriteLine($"[DEBUG] LoadHallDataForState - Binding data for Position {position} to {chartName} in {tabPageName}");
                        series.Points.DataBind(measurements[position].Select(data => new { Reading = data.Reading, Source = data.Source }).ToList(), "Reading", "Source", null);
                    }
                    else
                    {
                        series.Points.Clear();
                        Debug.WriteLine($"[DEBUG] LoadHallDataForState - No data for Position {position} in Chart: {chartName}");
                    }
                }
                else
                {
                    Debug.WriteLine($"[WARNING] UpdateIndividualChartData: Chart {chartName} not found or no series");
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] UpdateIndividualChartData: TabPage {tabPageName} or Chart {chartName} not found");
            }
        }

        private void SetupAxis(Axis axis, string title, string format, double interval)
        {
            axis.Title = title;
            axis.LabelStyle.Format = format;
            axis.Interval = interval;
        }

        private void CopyAxisSettings(Axis source, Axis destination)
        {
            destination.Title = source.Title;
            destination.LabelStyle.Format = source.LabelStyle.Format;
            destination.Interval = source.Interval;
            destination.Minimum = source.Minimum;
            destination.Maximum = source.Maximum;
            destination.Enabled = source.Enabled;
        }

        

        private void UpdateHallDataGridView(DataTable dataTable, BindingSource bindingSource)
        {
            bindingSource.DataSource = dataTable;
            bindingSource.ResetBindings(false);
        }

        private void UpdateHallInChart(Dictionary<int, List<(double Source, double Reading)>> measurements, BindingSource bindingSource, string mainTabControlName, string mainTabPageName, string subTabControlName, string chartNamePrefix)
        {
            TabControl MainTbCtrl = this.Controls.Find(mainTabControlName, true).OfType<TabControl>().FirstOrDefault();
            if (MainTbCtrl == null) return;

            TabPage MainTbPgs = MainTbCtrl.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Name == mainTabPageName);
            if (MainTbPgs == null) return;

            TabControl SubTbCtrl = MainTbPgs.Controls.Find(subTabControlName, true).OfType<TabControl>().FirstOrDefault();
            if (SubTbCtrl == null) return;

            int index = 1;
            foreach (TabPage subTabPage in SubTbCtrl.TabPages)
            {
                string chartName = $"{chartNamePrefix}{index}";
                Chart chart = subTabPage.Controls.Find(chartName, true).OfType<Chart>().FirstOrDefault();

                Debug.WriteLine($"[DEBUG] UpdateHallInChart - Processing Chart: {chartName}, Position: {index}");

                if (chart != null)
                {
                    chart.Series.Clear();
                    Series series = new Series($"Position {index}");
                    series.ChartType = SeriesChartType.Line;
                    
                    Debug.WriteLine($"[DEBUG] UpdateHallInChart - Processing Chart: {chartName}, Position Index: {index}");
                    if (measurements.TryGetValue(index, out var positionData) && positionData.Any())
                    {
                        DataTable chartDataTable = new DataTable();
                        chartDataTable.Columns.Add("Measured", typeof(double));
                        chartDataTable.Columns.Add("Source", typeof(double));

                        foreach (var dataPoint in positionData)
                        {
                            chartDataTable.Rows.Add(dataPoint.Reading, dataPoint.Source);
                        }

                        Debug.WriteLine($"[DEBUG] UpdateHallInChart - Data in chartDataTable for Position {index}:");
                        
                        foreach (DataRow row in chartDataTable.Rows)
                        {
                            Debug.WriteLine($"[DEBUG] Measured: {row["Measured"]}, Source: {row["Source"]}");
                        }

                        chart.DataSource = chartDataTable;
                        series.XValueMember = "Measured";
                        series.YValueMembers = "Source";
                        chart.DataBind();
                        chart.Series.Add(series);
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] UpdateHallInChart - No data or empty data for Position: {index}");
                        chart.DataSource = null;
                        chart.DataBind();
                    }
                }
                else
                {
                    Debug.WriteLine($"[WARNING] UpdateHallInChart - Chart '{chartName}' not found.");
                }
                index++;
            }
        }

        private void ClearHallInCharts(BindingSource bindingSource)
        {
            string mainTabControlName = "TabcontrolHallInTotalChart";
            string[] mainTabPageNames = { "TabpageHallInSouthTotalChart", "TabpageHallInNorthTotalChart" };
            string[] subTabControlNames = { "TabcontrolHallInSouthTotalChart", "TabcontrolHallInNorthTotalChart" };
            string[] chartNamePrefixes = { "ChartHallInSouthMeasurementPosition", "ChartHallInNorthMeasurementPosition" };

            TabControl MainTbCtrl = this.Controls.Find(mainTabControlName, true).OfType<TabControl>().FirstOrDefault();
            if (MainTbCtrl == null) return;

            for (int i = 0; i < mainTabPageNames.Length; i++)
            {
                TabPage MainTbPgs = MainTbCtrl.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Name == mainTabPageNames[i]);
                if (MainTbPgs == null) continue;

                TabControl SubTbCtrl = MainTbPgs.Controls.Find(subTabControlNames[i], true).OfType<TabControl>().FirstOrDefault();
                if (SubTbCtrl == null) continue;

                foreach (TabPage subTabPage in SubTbCtrl.TabPages)
                {
                    string chartName = $"{chartNamePrefixes[i]}{subTabPage.TabIndex + 1}";
                    Chart chart = subTabPage.Controls.Find(chartName, true).OfType<Chart>().FirstOrDefault();
                    if (chart != null)
                    {
                        chart.DataSource = null;
                        chart.DataBind();
                        chart.Series.Clear();
                    }
                }
            }
        }

        private void CollectAndCalculateHallMeasured_DataUpdated(object sender, EventArgs e)
        {
        }

        private void HallTotalMeasureValuesForm_Load(object sender, EventArgs e)
        {
            if (CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurements().Any())
            {
                LoadAllHallData(CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurements());
            }

            InitializeCharts();
        }

        private void HallTotalMeasureValuesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectAndCalculateHallMeasured.Instance.DataUpdated -= CollectAndCalculateHallMeasured_DataUpdated;
        }
    }
}