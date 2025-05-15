using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class HallTotalMeasureValuesForm : Form
    {
        private Dictionary<string, Chart> chartDictionary = new Dictionary<string, Chart>();
        private const int NumberOfHallPositions = 4;
        private List<DataGridView> _hallDataGridViews;
        private Dictionary<string, BindingSource> _hallBindingSources = new Dictionary<string, BindingSource>();
        private TabControl tabControlHallTotalMeasuredCharts;
        private readonly string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
        private readonly string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";
        private Dictionary<string, Color> seriesColorMap = new Dictionary<string, Color>()
{
    { "Out1", Color.GreenYellow }, { "Out2", Color.GreenYellow }, { "Out3", Color.GreenYellow }, { "Out4", Color.GreenYellow },
    { "South1", Color.OrangeRed }, { "South2", Color.OrangeRed }, { "South3", Color.OrangeRed }, { "South4", Color.OrangeRed },
    { "North1", Color.DeepSkyBlue }, { "North2", Color.DeepSkyBlue }, { "North3", Color.DeepSkyBlue }, { "North4", Color.DeepSkyBlue }
};

        public HallTotalMeasureValuesForm()
        {
            InitializeComponent();
            tabControlHallTotalMeasuredCharts = TabcontrolHallTotalMeasCharts;
            InitializeHallBindingSources();
            InitializeHallDataGridViewList();
            InitializeAllDataGridViewColumns();
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
            BindingSource bs = new BindingSource
            {
                DataSource = CreateHallDataTable()
            };

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

            for (int i = 1; i <= NumberOfHallPositions; i++)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Source{i}",
                    HeaderText = $"Source {i} ({sourceUnit})",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6", Alignment = DataGridViewContentAlignment.MiddleCenter }
                });

                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Measured{i}",
                    HeaderText = $"Measured {i} ({measureUnit})",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6", Alignment = DataGridViewContentAlignment.MiddleCenter }
                });
            }

            dataGridView.AutoGenerateColumns = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void InitializeChartDictionary()
        {
            chartDictionary.Clear();

            // ✅ 1. Total Chart (TabPage 0)
            var totalTabPage = tabControlHallTotalMeasuredCharts.TabPages[0];
            var totalChart = totalTabPage.Controls.OfType<Chart>().FirstOrDefault();
            if (totalChart != null)
            {
                chartDictionary["ChartTotalMeasPos"] = totalChart;
            }

            // ✅ 2. NoMagneticField (ตำแหน่ง 1-4) => ใช้ prefix: "Out"
            for (int i = 1; i <= 4; i++)
            {
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage.Controls.OfType<Chart>().FirstOrDefault();
                if (chart != null)
                {
                    chartDictionary[$"Out{i}"] = chart;
                }
            }

            // ✅ 3. South (ตำแหน่ง 1-4) => prefix: "South"
            for (int i = 5; i <= 8; i++)
            {
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage.Controls.OfType<Chart>().FirstOrDefault();
                if (chart != null)
                {
                    int position = i - 4; // เปลี่ยนจาก 5–8 → 1–4
                    chartDictionary[$"South{position}"] = chart;
                }
            }

            // ✅ 4. North (ตำแหน่ง 1-4) => prefix: "North"
            for (int i = 9; i <= 12; i++)
            {
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage.Controls.OfType<Chart>().FirstOrDefault();
                if (chart != null)
                {
                    int position = i - 8; // เปลี่ยนจาก 9–12 → 1–4
                    chartDictionary[$"North{position}"] = chart;
                }
            }

            foreach (var key in chartDictionary.Keys)
            {
                Debug.WriteLine($"[DEBUG] ChartDictionary key: {key}");
            }

            Debug.WriteLine($"[INFO] Chart mapping complete. Total charts mapped: {chartDictionary.Count}");
        }

        private void UpdateTotalChart(Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>> allChartData)
        {
            Chart totalChart;
            if (!chartDictionary.TryGetValue("ChartTotalMeasPos", out totalChart)) return;
            totalChart.Series.Clear();

            ChartArea area = totalChart.ChartAreas[0];
            area.AxisX.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})";
            area.AxisY.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
            area.AxisX.LabelStyle.Format = "F6";
            area.AxisY.LabelStyle.Format = "F6";
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MinorGrid.Enabled = true;
            area.AxisX.MinorGrid.LineColor = Color.DimGray;
            area.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.LineColor = Color.DimGray;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            foreach (var statePair in allChartData)
            {
                string stateKey = statePair.Key;
                var dataDict = statePair.Value;

                foreach (var dataEntry in dataDict)
                {
                    int position = dataEntry.Key;
                    var dataList = dataEntry.Value;
                    string chartKey = stateKey == "NoMagnetic" ? $"Out{position}" : $"{stateKey}{position}";
                    string seriesName = chartKey;

                    Color colorToUse = seriesColorMap.ContainsKey(chartKey) ? seriesColorMap[chartKey] : Color.Black;

                    var series = new Series(seriesName)
                    {
                        ChartType = SeriesChartType.Line,
                        BorderWidth = 2,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 6,
                        Color = colorToUse
                    };

                    foreach (var pair in dataList)
                    {
                        series.Points.AddXY(pair.Reading, pair.Source);
                    }

                    totalChart.Series.Add(series);
                }
            }

            area.RecalculateAxesScale();
            totalChart.Invalidate();
            Debug.WriteLine($"[DEBUG] Total Chart updated with {totalChart.Series.Count} series.");
        }

        private void UpdateIndividualChart(Chart chart, List<(double Source, double Reading)> data, string seriesName)
        {
            if (chart == null || data == null || data.Count == 0)
            {
                Debug.WriteLine($"[ERROR] UpdateIndividualChart aborted - chart is null or data is empty for {seriesName}");
                return;
            }

            Chart totalChart;
            if (!chartDictionary.TryGetValue("ChartTotalMeasPos", out totalChart))
            {
                Debug.WriteLine("[WARNING] TotalChart not found. Skipping individual formatting.");
                return;
            }

            ChartArea totalArea = totalChart.ChartAreas[0];
            chart.Series.Clear();

            Color lineColor = seriesColorMap.ContainsKey(seriesName) ? seriesColorMap[seriesName] : Color.Black;

            Series newSeries = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Diamond,
                MarkerSize = 6,
                Color = lineColor
            };

            foreach (var pair in data)
            {
                newSeries.Points.AddXY(pair.Reading, pair.Source);
            }

            chart.Series.Add(newSeries);

            ChartArea targetArea = chart.ChartAreas.Count > 0 ? chart.ChartAreas[0] : new ChartArea("Default");
            if (chart.ChartAreas.Count == 0)
                chart.ChartAreas.Add(targetArea);

            targetArea.AxisX.Title = totalArea.AxisX.Title;
            targetArea.AxisY.Title = totalArea.AxisY.Title;
            targetArea.AxisX.LabelStyle.Format = totalArea.AxisX.LabelStyle.Format;
            targetArea.AxisY.LabelStyle.Format = totalArea.AxisY.LabelStyle.Format;
            targetArea.AxisX.LabelStyle.Font = totalArea.AxisX.LabelStyle.Font;
            targetArea.AxisY.LabelStyle.Font = totalArea.AxisY.LabelStyle.Font;
            targetArea.AxisX.MajorGrid.LineColor = totalArea.AxisX.MajorGrid.LineColor;
            targetArea.AxisX.MinorGrid.Enabled = totalArea.AxisX.MinorGrid.Enabled;
            targetArea.AxisX.MinorGrid.LineColor = totalArea.AxisX.MinorGrid.LineColor;
            targetArea.AxisX.MinorGrid.LineDashStyle = totalArea.AxisX.MinorGrid.LineDashStyle;
            targetArea.AxisY.MajorGrid.LineColor = totalArea.AxisY.MajorGrid.LineColor;
            targetArea.AxisY.MinorGrid.Enabled = totalArea.AxisY.MinorGrid.Enabled;
            targetArea.AxisY.MinorGrid.LineColor = totalArea.AxisY.MinorGrid.LineColor;
            targetArea.AxisY.MinorGrid.LineDashStyle = totalArea.AxisY.MinorGrid.LineDashStyle;

            targetArea.RecalculateAxesScale();
            chart.Invalidate();

            Debug.WriteLine($"[DEBUG] Chart '{seriesName}' updated with {data.Count} points and color {lineColor.Name}.");
        }

        public void LoadAllHallData(Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData)
        {
            Debug.WriteLine("[DEBUG] LoadAllHallData - Received all Hall measurement data");

            // ✅ 1. อัปเดต DataGridView แต่ละชุด
            UpdateDataGridViewForState(HallMeasurementState.NoMagneticField, allHallData, BindingSourceHallOutVoltage);
            UpdateDataGridViewForState(HallMeasurementState.OutwardOrSouthMagneticField, allHallData, BindingSourceHallInSouthVoltage);
            UpdateDataGridViewForState(HallMeasurementState.InwardOrNorthMagneticField, allHallData, BindingSourceHallInNorthVoltage);

            // ✅ 2. เตรียมข้อมูลสำหรับ Total Chart (รองรับ C# 7.3)
            Dictionary<int, List<(double Source, double Reading)>> noMagData = new Dictionary<int, List<(double, double)>>();
            Dictionary<int, List<(double Source, double Reading)>> southData = new Dictionary<int, List<(double, double)>>();
            Dictionary<int, List<(double Source, double Reading)>> northData = new Dictionary<int, List<(double, double)>>();

            if (allHallData.ContainsKey(HallMeasurementState.NoMagneticField))
                noMagData = allHallData[HallMeasurementState.NoMagneticField];

            if (allHallData.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField))
                southData = allHallData[HallMeasurementState.OutwardOrSouthMagneticField];

            if (allHallData.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
                northData = allHallData[HallMeasurementState.InwardOrNorthMagneticField];

            var allChartData = new Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>>()
    {
        { "NoMagnetic", noMagData },
        { "South", southData },
        { "North", northData }
    };

            UpdateTotalChart(allChartData);

            foreach (var state in allChartData)
            {
                string stateKey = state.Key;
                string statePrefix = null;

                switch (stateKey)
                {
                    case "NoMagnetic":
                        statePrefix = "Out";
                        break;
                    case "South":
                        statePrefix = "South";
                        break;
                    case "North":
                        statePrefix = "North";
                        break;
                }

                if (statePrefix == null) continue;

                var stateData = state.Value;
                foreach (var entry in stateData)
                {
                    int position = entry.Key;
                    var dataList = entry.Value;

                    string chartKey = statePrefix + position;

                    if (chartDictionary.TryGetValue(chartKey, out Chart chart))
                    {
                        UpdateIndividualChart(chart, dataList, chartKey);
                    }
                    else
                    {
                        Debug.WriteLine($"[WARNING] Chart key '{chartKey}' not found in dictionary.");
                    }
                }
            }
        }

        private void UpdateDataGridViewForState(HallMeasurementState state, Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData, BindingSource bindingSource)
        {
            if (allHallData.TryGetValue(state, out var data))
            {
                DataTable dataTable = ConvertHallDataToDataTable(data);
                UpdateHallDataGridView(dataTable, bindingSource);
                Debug.WriteLine($"[DEBUG] UpdateDataGridViewForState - Updated DataGridView for state: {state}");
            }
            else
            {
                Debug.WriteLine($"[WARNING] UpdateDataGridViewForState - No data found for state: {state}");
                UpdateHallDataGridView(CreateHallDataTable(), bindingSource);
            }
        }

        private DataTable ConvertHallDataToDataTable(Dictionary<int, List<(double Source, double Reading)>> data)
        {
            DataTable dataTable = CreateHallDataTable().Clone();
            if (data != null && data.Any())
            {
                int maxRows = data.Max(kvp => kvp.Value.Count);
                for (int i = 0; i < maxRows; i++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int j = 1; j <= NumberOfHallPositions; j++)
                    {
                        if (data.ContainsKey(j) && i < data[j].Count)
                        {
                            row[$"Source{j}"] = data[j][i].Source;
                            row[$"Measured{j}"] = data[j][i].Reading;
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

        private void UpdateHallDataGridView(DataTable dataTable, BindingSource bindingSource)
        {
            bindingSource.DataSource = dataTable;
            bindingSource.ResetBindings(false);
        }

        private void CollectAndCalculateHallMeasured_DataUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(CollectAndCalculateHallMeasured_DataUpdated), sender, e);
                return;
            }

            if (e is HallVoltageDataUpdatedEventArgs args)
            {
                // ✅ เตรียม Total Chart data
                var allChartData = new Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>>()
        {
            { "NoMagnetic", args.NoMagneticMeasurements },
            { "South", args.SouthVoltageMeasurements },
            { "North", args.NorthVoltageMeasurements }
        };
                UpdateTotalChart(allChartData);

                Debug.WriteLine($"[DEBUG] Event args: State={args.IndividualChartState}, Pos={args.IndividualChartPosition}, DataCount={args.IndividualChartData?.Count ?? 0}");

                if (args.IndividualChartData != null && !string.IsNullOrEmpty(args.IndividualChartState))
                {
                    string chartKey = $"{args.IndividualChartState}{args.IndividualChartPosition}";

                    if (chartDictionary.TryGetValue(chartKey, out Chart targetChart))
                    {
                        Debug.WriteLine($"[DEBUG] Chart '{chartKey}' found in dictionary, updating...");
                        UpdateIndividualChart(targetChart, args.IndividualChartData, chartKey);
                    }
                    else
                    {
                        Debug.WriteLine($"[ERROR] Chart key '{chartKey}' not found in chartDictionary!");
                    }
                }
                else
                {
                    Debug.WriteLine("[ERROR] IndividualChartData is null or IndividualChartState is missing");
                }

                var allData = new Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>>()
        {
            { HallMeasurementState.NoMagneticField, args.NoMagneticMeasurements },
            { HallMeasurementState.OutwardOrSouthMagneticField, args.SouthVoltageMeasurements },
            { HallMeasurementState.InwardOrNorthMagneticField, args.NorthVoltageMeasurements }
        };
                LoadAllHallData(allData);

                Debug.WriteLine($"[DEBUG] Received DataUpdated Event");
                Debug.WriteLine($"[DEBUG] NoMagnetic: {args.NoMagneticMeasurements?.Sum(x => x.Value.Count) ?? 0} points");
                Debug.WriteLine($"[DEBUG] South: {args.SouthVoltageMeasurements?.Sum(x => x.Value.Count) ?? 0} points");
                Debug.WriteLine($"[DEBUG] North: {args.NorthVoltageMeasurements?.Sum(x => x.Value.Count) ?? 0} points");
            }
        }


        public class HallVoltageDataUpdatedEventArgs : EventArgs
        {
            public Dictionary<int, List<(double Source, double Reading)>> NoMagneticMeasurements { get; set; }
            public Dictionary<int, List<(double Source, double Reading)>> SouthVoltageMeasurements { get; set; }
            public Dictionary<int, List<(double Source, double Reading)>> NorthVoltageMeasurements { get; set; }

            public string IndividualChartState { get; set; }
            public int IndividualChartPosition { get; set; }
            public List<(double Source, double Reading)> IndividualChartData { get; set; }

            // ✅ Constructor สำหรับข้อมูลรวม (Total Chart)
            public HallVoltageDataUpdatedEventArgs(
                Dictionary<int, List<(double Source, double Reading)>> noMagneticData,
                Dictionary<int, List<(double Source, double Reading)>> southData,
                Dictionary<int, List<(double Source, double Reading)>> northData)
            {
                NoMagneticMeasurements = noMagneticData;
                SouthVoltageMeasurements = southData;
                NorthVoltageMeasurements = northData;
                IndividualChartState = null;
                IndividualChartPosition = 0;
                IndividualChartData = null;
            }

            // ✅ Constructor สำหรับ Individual Chart
            public HallVoltageDataUpdatedEventArgs(string state, int position, List<(double Source, double Reading)> data)
            {
                IndividualChartState = state;
                IndividualChartPosition = position;
                IndividualChartData = data;
                NoMagneticMeasurements = null;
                SouthVoltageMeasurements = null;
                NorthVoltageMeasurements = null;
            }
        }

        private void HallTotalMeasureValuesForm_Load(object sender, EventArgs e)
        {
            InitializeChartDictionary();

            if (CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurements().Any())
            {
                LoadAllHallData(CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurements());
            }
        }

        private void HallTotalMeasureValuesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectAndCalculateHallMeasured.Instance.DataUpdated -= CollectAndCalculateHallMeasured_DataUpdated;
        }
    }
}