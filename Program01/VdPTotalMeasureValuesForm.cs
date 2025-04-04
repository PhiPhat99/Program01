using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class VdPTotalMeasureValuesForm : Form
    {
        public VdPTotalMeasureValuesForm()
        {
            InitializeComponent();
        }

        public void UpdateMeasurementData(List<double[]> MeasuredValue)
        {
            UpdateDataGridView(MeasuredValue);
            UpdateChartsInTabs(MeasuredValue);
        }

        private void UpdateDataGridView(List<double[]> MeasuredValue)
        {
            DatagridviewVdPTotalMesure.Rows.Clear();
            DatagridviewVdPTotalMesure.Columns.Clear();

            if (MeasuredValue != null && MeasuredValue.Count > 0 && MeasuredValue[0].Length == 2)
            {
                // เพิ่ม Columns แบบไดนามิก
                for (int i = 0; i < MeasuredValue[0].Length; i++)
                {
                    DatagridviewVdPTotalMesure.Columns.Add($"Column{i}", $"Column {i}");
                }

                // เพิ่ม Rows โดยสลับ X และ Y
                for (int i = 0; i < MeasuredValue.Count; i++)
                {
                    DatagridviewVdPTotalMesure.Rows.Add(MeasuredValue[i]);
                }
            }
            else
            {
                MessageBox.Show("ไม่มีข้อมูลการวัด หรือข้อมูลไม่ถูกต้อง!", "Warning", MessageBoxButtons.OK);
            }
        }

        private void UpdateChartsInTabs(List<double[]> MeasuredValue)
        {
            ChartTotalMeasured.Series.Clear();
            TabcontrolVdPTotalCharts.TabPages.Clear();

            if (MeasuredValue != null && MeasuredValue.Count > 0 && MeasuredValue[0].Length == 2)
            {
                // เพิ่ม Tabs แบบไดนามิก
                TabcontrolVdPTotalCharts.TabPages.Add("Total");

                // เพิ่ม Chart "Total"
                ChartTotalMeasured.Titles.Clear();
                ChartTotalMeasured.Titles.Add("All Positions - I-V Curve");
                ChartTotalMeasured.ChartAreas[0].AxisX.Title = GlobalSettings.Instance.StepUnit;
                ChartTotalMeasured.ChartAreas[0].AxisY.Title = GlobalSettings.Instance.SourceLimitLevelUnit;

                for (int i = 0; i < MeasuredValue.Count; i++)
                {
                    var series = new Series($"Position {i + 1}");
                    series.ChartType = SeriesChartType.Line;
                    series.XValueType = ChartValueType.Double;
                    series.YValueType = ChartValueType.Double;

                    series.Points.AddXY(MeasuredValue[i][1], MeasuredValue[i][0]); // สลับ X และ Y
                    ChartTotalMeasured.Series.Add(series);
                }

                // เพิ่ม Charts ในแต่ละ Tab
                for (int i = 0; i < MeasuredValue.Count; i++)
                {
                    TabcontrolVdPTotalCharts.TabPages.Add($"Position {i + 1}");
                    var chart = new Chart();
                    chart.Dock = DockStyle.Fill;
                    TabcontrolVdPTotalCharts.TabPages[i + 1].Controls.Add(chart);

                    chart.Series.Clear();
                    var series = new Series("I-V Curve");
                    series.ChartType = SeriesChartType.Line;
                    series.XValueType = ChartValueType.Double;
                    series.YValueType = ChartValueType.Double;

                    series.Points.AddXY(MeasuredValue[i][1], MeasuredValue[i][0]); // สลับ X และ Y
                    chart.Series.Add(series);

                    chart.Titles.Clear();
                    chart.Titles.Add($"I-V Curve - Position {i + 1}");
                    chart.ChartAreas[0].AxisX.Title = GlobalSettings.Instance.StepUnit;
                    chart.ChartAreas[0].AxisY.Title = GlobalSettings.Instance.SourceLimitLevelUnit;
                }
            }
            else
            {
                MessageBox.Show("ไม่มีข้อมูลการวัด หรือข้อมูลไม่ถูกต้อง!", "Warning", MessageBoxButtons.OK);
            }
        }
    }
}
