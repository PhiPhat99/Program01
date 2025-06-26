﻿using System;
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
        private IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> _currentRawHallData;
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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "E5", Alignment = DataGridViewContentAlignment.MiddleCenter }
                });

                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = $"Reading{i}",
                    HeaderText = $"{GlobalSettings.Instance.MeasureModeUI} {i} ({measureUnit})",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "E5", Alignment = DataGridViewContentAlignment.MiddleCenter }
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
            targetArea.AxisX.LabelStyle.Format = "E5";
            targetArea.AxisY.LabelStyle.Format = "E5";
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

        public void LoadAllHallData(IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> allRawData)
        {
            Debug.WriteLine("[DEBUG] LoadAllHallData - ได้รับข้อมูลการวัด Hall ทั้งหมดแล้ว");
            _currentRawHallData = allRawData; // *** แก้ไข: ใช้ allRawData แทน rawHallData ***

            // --- อัปเดต DataGridViews สำหรับแต่ละสถานะการวัด ---
            // BindingSourceHallOutVoltage สำหรับ NoMagneticField
            UpdateDataGridViewForState(HallMeasurementState.NoMagneticField, (IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>>)_currentRawHallData, BindingSourceHallOutVoltage);
            // BindingSourceHallInSouthVoltage สำหรับ OutwardOrSouthMagneticField
            UpdateDataGridViewForState(HallMeasurementState.OutwardOrSouthMagneticField, (IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>>)_currentRawHallData, BindingSourceHallInSouthVoltage);
            // BindingSourceHallInNorthVoltage สำหรับ InwardOrNorthMagneticField
            UpdateDataGridViewForState(HallMeasurementState.InwardOrNorthMagneticField, (IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>>)_currentRawHallData, BindingSourceHallInNorthVoltage);


            // --- เตรียมข้อมูลสำหรับ Charts โดยการแปลง Tuple เป็น named tuple (Source, Reading) ---
            Dictionary<int, List<(double Source, double Reading)>> noMagChartData = new Dictionary<int, List<(double Source, double Reading)>>();
            Dictionary<int, List<(double Source, double Reading)>> southChartData = new Dictionary<int, List<(double Source, double Reading)>>();
            Dictionary<int, List<(double Source, double Reading)>> northChartData = new Dictionary<int, List<(double Source, double Reading)>>();

            if (_currentRawHallData.ContainsKey(HallMeasurementState.NoMagneticField))
            {
                noMagChartData = _currentRawHallData[HallMeasurementState.NoMagneticField]
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value.Select(t => (Source: t.Item1, Reading: t.Item2)).ToList()
                    );
            }

            if (_currentRawHallData.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField))
            {
                southChartData = _currentRawHallData[HallMeasurementState.OutwardOrSouthMagneticField]
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value.Select(t => (Source: t.Item1, Reading: t.Item2)).ToList()
                    );
            }

            if (_currentRawHallData.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
            {
                northChartData = _currentRawHallData[HallMeasurementState.InwardOrNorthMagneticField]
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value.Select(t => (Source: t.Item1, Reading: t.Item2)).ToList()
                    );
            }

            // รวมข้อมูล Chart ทั้งหมดไว้ใน Dictionary เดียวกันเพื่อวนลูปง่ายขึ้น
            var allChartData = new Dictionary<string, Dictionary<int, List<(double Source, double Reading)>>>()
            {
                { "NoMagnetic", noMagChartData },
                { "South", southChartData },
                { "North", northChartData }
            };

            // --- อัปเดต Charts แต่ละตัว ---
            foreach (var stateEntry in allChartData)
            {
                string stateKey = stateEntry.Key;
                string chartPrefix = null; // คำนำหน้าสำหรับ Chart Key (เช่น "Out", "South", "North")

                switch (stateKey)
                {
                    case "NoMagnetic":
                        chartPrefix = "Out"; // สำหรับกราฟ 'Out' ที่แสดงผลไม่มีสนามแม่เหล็ก
                        break;
                    case "South":
                        chartPrefix = "South"; // สำหรับกราฟ 'South' ที่แสดงผลสนามแม่เหล็กทิศใต้
                        break;
                    case "North":
                        chartPrefix = "North"; // สำหรับกราฟ 'North' ที่แสดงผลสนามแม่เหล็กทิศเหนือ
                        break;
                }

                if (chartPrefix == null)
                {
                    continue; // ข้ามหากไม่มี chartPrefix ที่ตรงกัน
                }

                var stateDataForCharts = stateEntry.Value; // ข้อมูลสำหรับสถานะปัจจุบัน

                foreach (var positionData in stateDataForCharts)
                {
                    int position = positionData.Key;
                    var dataList = positionData.Value; // นี่คือ List<(double Source, double Reading)> แล้ว
                    string chartKey = chartPrefix + position; // สร้างคีย์สำหรับค้นหากราฟ (เช่น "Out1", "South2")

                    if (chartDictionary.TryGetValue(chartKey, out Chart chart))
                    {
                        // เรียกเมธอด helper เพื่ออัปเดตกราฟแต่ละตัว
                        UpdateIndividualChart(chart, dataList, chartKey);
                    }
                    else
                    {
                        Debug.WriteLine($"[WARNING] ไม่พบ Chart key '{chartKey}' ใน chartDictionary. ไม่สามารถอัปเดตกราฟได้.");
                    }
                }
            }

            Debug.WriteLine("[DEBUG] LoadAllHallData - การอัปเดต DataGridViews และ Charts เสร็จสมบูรณ์.");
        }

        private void UpdateDataGridViewForState(HallMeasurementState state, IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> hallData, BindingSource bindingSource)
        {
            if (hallData.TryGetValue(state, out var rawStateData))
            {
                // แปลง Dictionary<int, List<Tuple<double, double>>> เป็น Dictionary<int, List<(double Source, double Reading)>>
                var convertedDataForDataGridView = rawStateData.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.Select(t => (Source: t.Item1, Reading: t.Item2)).ToList()
                );

                DataTable dataTable = ConvertHallDataToDataTable(convertedDataForDataGridView);
                UpdateHallDataGridView(dataTable, bindingSource);
                Debug.WriteLine($"[DEBUG] UpdateDataGridViewForState - Updated DataGridView for state: {state}");
            }
            else
            {
                Debug.WriteLine($"[WARNING] UpdateDataGridViewForState - ไม่พบข้อมูลสำหรับ state: {state}. กำลังเคลียร์ DataGridView.");
                // หากไม่มีข้อมูลสำหรับ state นี้ ให้เคลียร์ DataGridView โดยส่ง DataTable เปล่าไป
                UpdateHallDataGridView(CreateHallDataTable(), bindingSource);
            }
        }

        // ** เมธอด helper สำหรับแปลงข้อมูล Hall เป็น DataTable เพื่อผูกกับ DataGridView **
        private DataTable ConvertHallDataToDataTable(Dictionary<int, List<(double Source, double Reading)>> data)
        {
            // ใช้ Clone() เพื่อให้ได้โครงสร้างคอลัมน์เหมือนเดิม แต่ไม่มีข้อมูลเก่า
            DataTable dataTable = CreateHallDataTable().Clone();

            if (data != null && data.Any())
            {
                int maxRows = 0;
                // ค้นหาจำนวนแถวสูงสุดในบรรดาข้อมูลทุกตำแหน่ง
                foreach (var kvp in data)
                {
                    if (kvp.Value != null)
                    {
                        maxRows = Math.Max(maxRows, kvp.Value.Count);
                    }
                }

                // เพิ่มข้อมูลลงใน DataTable ทีละแถว
                for (int i = 0; i < maxRows; i++)
                {
                    DataRow row = dataTable.NewRow();

                    for (int j = 1; j <= NumberOfHallPositions; j++)
                    {
                        // ตรวจสอบว่ามีข้อมูลสำหรับตำแหน่งและแถวนั้นๆ หรือไม่
                        if (data.ContainsKey(j) && data[j] != null && i < data[j].Count)
                        {
                            row[$"Source{j}"] = data[j][i].Source;
                            row[$"Reading{j}"] = data[j][i].Reading;
                        }
                        else
                        {
                            // ถ้าไม่มีข้อมูล ให้ใส่ DBNull.Value (ค่าว่างสำหรับฐานข้อมูล)
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

                LoadAllHallData((IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>>)_currentRawHallData);

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

            if (CollectAndCalculateHallMeasured.Instance.AllRawMeasurements != null && CollectAndCalculateHallMeasured.Instance.AllRawMeasurements.Any())
            {
                LoadAllHallData(CollectAndCalculateHallMeasured.Instance.AllRawMeasurements);
            }
        }

        private void HallTotalMeasureValuesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectAndCalculateHallMeasured.Instance.DataUpdated -= CollectAndCalculateHallMeasured_DataUpdated;
        }
    }
}