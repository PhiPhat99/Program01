using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class VdPTotalMeasureValueForm : Form
    {
        public VdPTotalMeasureValueForm()
        {
            InitializeComponent();
        }

        public void UpdateChartAndDataGridView(List<double> XData, List<double> YData)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateChartAndDataGridView(XData, YData)));
                return;
            }

            // อัปเดตข้อมูลใน Chart
            foreach (TabPage tabPage in TabcontrolVdPTotalCharts.TabPages)
            {
                if (tabPage.Controls.ContainsKey("chartVdP"))
                {
                    var chart = tabPage.Controls["chartVdP"] as Chart;
                    if (chart != null)
                    {
                        var series = chart.Series[0];
                        series.Points.Clear();

                        for (int i = 0; i < XData.Count; i++)
                        {
                            series.Points.AddXY(XData[i], YData[i]);
                        }
                    }
                }
            }

            // อัปเดตข้อมูลใน DataGridView
            DatagridviewVdPTotalMesure.Rows.Clear();
            for (int i = 0; i < YData.Count; i++)
            {
                int rowIndex = DatagridviewVdPTotalMesure.Rows.Add();
                DatagridviewVdPTotalMesure.Rows[rowIndex].Cells["MeasuredValue1"].Value = YData[i];
            }
        }

        private void LoadVdPTotalMeasurementData()
        {
            var VdPData = CollectVdPMeasuredValue.Instance;
            DatagridviewVdPTotalMesure.Rows.Clear();

            if (DatagridviewVdPTotalMesure.Columns.Count == 0)
            {
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue1", "Measured 1");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue2", "Measured 2");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue3", "Measured 3");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue4", "Measured 4");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue5", "Measured 5");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue6", "Measured 6");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue7", "Measured 7");
                DatagridviewVdPTotalMesure.Columns.Add("MeasuredValue8", "Measured 8");
            }

            foreach (var readings in VdPData.VdPMeasured)
            {
                int rowIndex = DatagridviewVdPTotalMesure.Rows.Add();

                for (int Cols = 0; Cols < DatagridviewVdPTotalMesure.Columns.Count; Cols++)
                {
                    DatagridviewVdPTotalMesure.Rows[rowIndex].Cells[Cols].Value = readings;
                }
            }
        }

        private void VdPTotalMeasureValueForm_Load(object sender, EventArgs e)
        {
            LoadVdPTotalMeasurementData();
        }

        private void DatagridviewVdPTotalMesure_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue1" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue2" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue3" || 
                DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue4" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue5" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue6" || 
                DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue7" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "MeasuredValue8")
            {
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double results))
                {
                    e.Value = results.ToString("F5");
                }
            }
        }

        public void SubscribeToMeasurement(MeasurementSettingsForm SettingsForm)
        {
            SettingsForm.OnMeasurementCompleted += UpdateChartAndDataGridView;
        }

    }
}
