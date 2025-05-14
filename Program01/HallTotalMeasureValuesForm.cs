using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System;

namespace Program01
{
    public partial class HallTotalMeasureValuesForm : Form
    {
        private Dictionary<string, Chart> chartDictionary = new Dictionary<string, Chart>();
        private string[] directions = new string[] { "Hall In North", "Hall In South" };
        private const int NumberOfHallPositions = 4;
        private List<DataGridView> _hallDataGridViews;
        private Dictionary<string, BindingSource> _hallBindingSources = new Dictionary<string, BindingSource>();
        private TabControl tabControlHallTotalMeasuredCharts;
        private readonly string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
        private readonly string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";

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

        private void UpdateIndividualChart(Chart chart, List<(double Source, double Reading)> data, string seriesName)
        {
            if (chart == null || data == null || data.Count == 0)
            {
                Debug.WriteLine($"[ERROR] UpdateIndividualChart aborted - chart is null or data is empty for {seriesName}");
                return;
            }

            chart.Series.Clear();
            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };

            foreach (var (Source, Reading) in data)
            {
                series.Points.AddXY(Source, Reading);
            }

            chart.Series.Add(series);
            chart.ChartAreas[0].RecalculateAxesScale();
            chart.Invalidate();

            Debug.WriteLine($"[DEBUG] Chart '{seriesName}' updated with {data.Count} points.");
        }

        private void UpdateTotalChart(Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>> allChartData)
        {
            if (!chartDictionary.TryGetValue("ChartTotalMeasPos", out var totalChart))
            {
                return;
            }

            totalChart.Series.Clear();

            // 🔁 แทน var (stateKey, dataDict) ➜ ใช้ KeyValuePair ตรง ๆ
            foreach (var kvp in allChartData)
            {
                string stateKey = kvp.Key;
                var dataDict = kvp.Value;

                foreach (var dataEntry in dataDict)
                {
                    int position = dataEntry.Key;
                    var dataList = dataEntry.Value;

                    string label = $"Pos {position} ({stateKey})";
                    var series = new Series(label)
                    {
                        ChartType = SeriesChartType.Line
                    };

                    foreach (var (Source, Reading) in dataList)
                    {
                        series.Points.AddXY(Source, Reading);
                    }

                    totalChart.Series.Add(series);
                }
            }

            totalChart.ChartAreas[0].RecalculateAxesScale();

            totalChart.Invalidate(); // บังคับวาดใหม่

            Debug.WriteLine($"[DEBUG] Total Chart updated with {totalChart.Series.Count} series.");
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