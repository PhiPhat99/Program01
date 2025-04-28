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

            HallOutVoltageBindingSource.DataSource = CreateHallDataTable();
            DatagridviewHallOutVoltageTotalMeasured.DataSource = HallOutVoltageBindingSource;

            HallInSouthVoltageBindingSource.DataSource = CreateHallDataTable();
            DatagridviewHallSouthVoltageTotalMeasured.DataSource = HallInSouthVoltageBindingSource;

            HallInNorthVoltageBindingSource.DataSource = CreateHallDataTable();
            DatagridviewHallNorthVoltageTotalMeasured.DataSource = HallInNorthVoltageBindingSource;
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
            InitializeChartsForTabPage(
                MainTabControlName: "TabcontrolHallInTotalChart",
                MainTabPagesName: "TabpageHallInSouthTotalChart",
                SubTabControlName: "TabcontrolHallInSouthTotalChart",
                SubTabPagesName: "TabpageV_hiSouthMP",
                ChartNamePrefix: "ChartHallInSouthMeasuredPosition",
                IsSouth: true
            );

            InitializeChartsForTabPage(
                MainTabControlName: "TabcontrolHallInTotalChart",
                MainTabPagesName: "TabpageHallInNorthTotalChart",
                SubTabControlName: "TabcontrolHallInNorthTotalChart",
                SubTabPagesName: "TabpageV_hiNorthMP",
                ChartNamePrefix: "ChartHallInNorthMeasuredPosition",
                IsSouth: false
            );

            Debug.WriteLine("[DEBUG] InitializeChartsInTabControls() - All charts initialized successfully.");
        }

        private void InitializeChartsForTabPage(string MainTabControlName, string MainTabPagesName, string SubTabControlName, string SubTabPagesName, string ChartNamePrefix, bool IsSouth)
        {
            // Step 1: Find Main TabControl
            TabControl MainTbCtrl = this.Controls.Find(MainTabControlName, true).OfType<TabControl>().FirstOrDefault();
            if (MainTbCtrl == null)
            {
                Debug.WriteLine($"[WARNING] Main TabControl '{MainTabControlName}' not found!");
                return;
            }

            // Step 2: Find Main TabPage inside Main TabControl
            TabPage MainTbPgs = MainTbCtrl.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Name == MainTabPagesName);
            if (MainTbPgs == null)
            {
                Debug.WriteLine($"[WARNING] Main TabPage '{MainTabPagesName}' not found inside '{MainTabControlName}'!");
                return;
            }

            // Step 3: Find Sub TabControl inside Main TabPage
            TabControl SubTbCtrl = MainTbPgs.Controls.Find(SubTabControlName, true).OfType<TabControl>().FirstOrDefault();
            if (SubTbCtrl == null)
            {
                Debug.WriteLine($"[WARNING] Sub TabControl '{SubTabControlName}' not found inside '{MainTabPagesName}'!");
                return;
            }

            // Step 4: Find the specific SubTabPage
            TabPage TargetSubTabPage = SubTbCtrl.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Name == SubTabPagesName);
            if (TargetSubTabPage == null)
            {
                Debug.WriteLine($"[WARNING] SubTabPage '{SubTabPagesName}' not found inside '{SubTabControlName}'!");
                return;
            }

            // Step 5: Find and setup Charts inside the specific SubTabPage
            for (int i = 1; i <= 4; i++)
            {
                string ChartName = $"{ChartNamePrefix}{i}";
                Chart chart = TargetSubTabPage.Controls.Find(ChartName, true).OfType<Chart>().FirstOrDefault();

                if (chart != null)
                {
                    string location = IsSouth ? "South" : "North";
                    SetupIVChart(chart, $"I-V Graph of Position {i} ({location})");
                }
                else
                {
                    Debug.WriteLine($"[WARNING] Chart '{ChartName}' not found in SubTabPage '{TargetSubTabPage.Name}'!");
                }
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
            // ดึงข้อมูลการวัด Hall Out (Without Field) และอัปเดต DataGridView
            var hallOutMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("withoutfield");
            UpdateHallDataGridView(hallOutMeasurements, HallOutVoltageBindingSource);

            // ดึงข้อมูลการวัด Hall In (South Field) และอัปเดต DataGridView
            var hallInSouthMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("southfield");
            UpdateHallDataGridView(hallInSouthMeasurements, HallInSouthVoltageBindingSource);

            // ดึงข้อมูลการวัด Hall In (North Field) และอัปเดต DataGridView
            var hallInNorthMeasurements = CollectAndCalculateHallMeasured.Instance.GetAllHallMeasurementsByType("northfield");
            UpdateHallDataGridView(hallInNorthMeasurements, HallInNorthVoltageBindingSource);
        }

        private void UpdateHallDataGridView(Dictionary<int, List<(double Source, double Reading)>> measurements, BindingSource bindingSource)
        {
            if (bindingSource.DataSource is DataTable dataTable)
            {
                dataTable.Rows.Clear();

                foreach (var PositionData in measurements)
                {
                    int PositionNumber = PositionData.Key;

                    if (PositionNumber >= 1 && PositionNumber <= 4)
                    {
                        DataRow rowData = dataTable.NewRow();
                        for (int i = 0; i < PositionData.Value.Count; i++)
                        { 
                            if (i < 4)
                            {
                                rowData[$"Source{PositionNumber}"] = PositionData.Value[i].Source;
                                rowData[$"Measured{PositionNumber}"] = PositionData.Value[i].Reading;
                                break;
                            }
                        }
                        dataTable.Rows.Add(rowData);
                    }
                }

                bindingSource.ResetBindings(false);
            }
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