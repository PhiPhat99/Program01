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
            InitializeHallBindingSources();
            InitializeHallDataGridViewList();
            InitializeAllDataGridViewColumns();
            tabControlHallTotalMeasuredCharts = TabcontrolHallTotalMeasCharts;
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
                dataTable.Columns.Add($"Reading{i}", typeof(double));
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
                    HeaderText = $"{GlobalSettings.Instance.SourceModeUI} {i} ({sourceUnit})",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6", Alignment = DataGridViewContentAlignment.MiddleCenter }
                });

                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Reading{i}",
                    HeaderText = $"{GlobalSettings.Instance.MeasureModeUI} {i} ({measureUnit})",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F6", Alignment = DataGridViewContentAlignment.MiddleCenter }
                });
            }

            dataGridView.AutoGenerateColumns = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void InitializeChartDictionary()
        {
            chartDictionary.Clear();
            Debug.WriteLine("[DEBUG] Starting InitializeChartDictionary");

            // Map Out charts (TabPages 0 to 3)
            for (int i = 0; i < 4; i++)
            {
                Debug.WriteLine($"[DEBUG] Processing Out TabPage index: {i}");
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage?.Controls.OfType<Chart>().FirstOrDefault();

                if (chart != null)
                {
                    int position = i + 1;
                    chartDictionary[$"Out{position}"] = chart;
                    Debug.WriteLine($"[DEBUG] Found Out chart at position {position}, Key: Out{position}");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] No Out chart found in TabPage index: {i}");
                }
            }

            // Map South charts (TabPages 4 to 7)
            for (int i = 4; i < 8; i++)
            {
                Debug.WriteLine($"[DEBUG] Processing South TabPage index: {i}");
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage?.Controls.OfType<Chart>().FirstOrDefault();

                if (chart != null)
                {
                    int position = i - 3;
                    chartDictionary[$"South{position}"] = chart;
                    Debug.WriteLine($"[DEBUG] Found South chart at position {position}, Key: South{position}");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] No South chart found in TabPage index: {i}");
                }
            }

            // Map North charts (TabPages 8 to 11)
            for (int i = 8; i < 12; i++)
            {
                Debug.WriteLine($"[DEBUG] Processing North TabPage index: {i}");
                var tabPage = tabControlHallTotalMeasuredCharts.TabPages[i];
                var chart = tabPage?.Controls.OfType<Chart>().FirstOrDefault();

                if (chart != null)
                {
                    int position = i - 7;
                    chartDictionary[$"North{position}"] = chart;
                    Debug.WriteLine($"[DEBUG] Found North chart at position {position}, Key: North{position}");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] No North chart found in TabPage index: {i}");
                }
            }

            foreach (var key in chartDictionary.Keys)
            {
                Debug.WriteLine($"[DEBUG] ChartDictionary key: {key}");
            }

            Debug.WriteLine($"[INFO] Chart mapping complete. Total charts mapped: {chartDictionary.Count}");
            Debug.WriteLine("[DEBUG] Ending InitializeChartDictionary");
        }

        private void UpdateIndividualChart(Chart chart, List<(double Source, double Reading)> data, string seriesName)
        {
            if (chart == null || data == null || data.Count == 0)
            {
                Debug.WriteLine($"[ERROR] UpdateIndividualChart aborted - chart is null or data is empty for {seriesName}");
                return;
            }

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
                newSeries.Points.AddXY(pair.Source, pair.Reading);
            }

            chart.Series.Add(newSeries);
            ChartArea targetArea = chart.ChartAreas.Count > 0 ? chart.ChartAreas[0] : new ChartArea("Default");

            if (chart.ChartAreas.Count == 0)
            {
                chart.ChartAreas.Add(targetArea);
            }

            targetArea.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
            targetArea.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})";

            targetArea.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} ({sourceUnit})";
            targetArea.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} ({measureUnit})";

            targetArea.AxisX.LabelStyle.Angle = 90;
            targetArea.AxisY.LabelStyle.Angle = 0;
            targetArea.AxisX.LabelStyle.Format = "E3";
            targetArea.AxisY.LabelStyle.Format = "E3";
            targetArea.AxisX.LabelStyle.Font = new Font("Segoe UI, Bold", 9);
            targetArea.AxisY.LabelStyle.Font = new Font("Segoe UI, Bold", 9);

            targetArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            targetArea.AxisX.MinorGrid.Enabled = true;
            targetArea.AxisX.MinorGrid.LineColor = Color.DimGray;
            targetArea.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            targetArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            targetArea.AxisY.MinorGrid.Enabled = true;
            targetArea.AxisY.MinorGrid.LineColor = Color.DimGray;
            targetArea.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            targetArea.RecalculateAxesScale();
            chart.Invalidate();
            Debug.WriteLine($"[DEBUG] Chart '{seriesName}' updated with {data.Count} points and color {lineColor.Name}");
        }

        public void LoadAllHallData(Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallData)
        {
            Debug.WriteLine("[DEBUG] LoadAllHallData - Received all Hall measurement data");

            UpdateDataGridViewForState(HallMeasurementState.NoMagneticField, allHallData, BindingSourceHallOutVoltage);
            UpdateDataGridViewForState(HallMeasurementState.OutwardOrSouthMagneticField, allHallData, BindingSourceHallInSouthVoltage);
            UpdateDataGridViewForState(HallMeasurementState.InwardOrNorthMagneticField, allHallData, BindingSourceHallInNorthVoltage);

            Dictionary<int, List<(double Source, double Reading)>> noMagData = new Dictionary<int, List<(double, double)>>();
            Dictionary<int, List<(double Source, double Reading)>> southData = new Dictionary<int, List<(double, double)>>();
            Dictionary<int, List<(double Source, double Reading)>> northData = new Dictionary<int, List<(double, double)>>();

            if (allHallData.ContainsKey(HallMeasurementState.NoMagneticField))
            {
                noMagData = allHallData[HallMeasurementState.NoMagneticField];
            }

            if (allHallData.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField))
            {
                southData = allHallData[HallMeasurementState.OutwardOrSouthMagneticField];
            }

            if (allHallData.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
            {
                northData = allHallData[HallMeasurementState.InwardOrNorthMagneticField];
            }

            var allChartData = new Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>>()
            {
                { "NoMagnetic", noMagData },
                { "South", southData },
                { "North", northData }
            };

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

                if (statePrefix == null)
                {
                    continue;
                }

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
                            row[$"Reading{j}"] = data[j][i].Reading;
                        }
                        else
                        {
                            row[$"Source{j}"] = DBNull.Value;
                            row[$"Reading{j}"] = DBNull.Value;
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
                var allChartData = new Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>>()
                {
                    { "NoMagnetic", args.NoMagneticMeasurements },
                    { "South", args.SouthVoltageMeasurements },
                    { "North", args.NorthVoltageMeasurements }
                };

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

            public HallVoltageDataUpdatedEventArgs(Dictionary<int, List<(double Source, double Reading)>> noMagneticData, Dictionary<int, List<(double Source, double Reading)>> southData, Dictionary<int, List<(double Source, double Reading)>> northData)
            {
                NoMagneticMeasurements = noMagneticData;
                SouthVoltageMeasurements = southData;
                NorthVoltageMeasurements = northData;
                IndividualChartState = null;
                IndividualChartPosition = 0;
                IndividualChartData = null;
            }

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