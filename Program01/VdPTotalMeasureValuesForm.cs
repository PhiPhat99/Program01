using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class VdPTotalMeasureValuesForm : Form
    {
        public VdPTotalMeasureValuesForm()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            // ตั้งค่าตารางข้อมูล
            DatagridviewVdPTotalMesure.Columns.Clear();
            for (int i = 1; i <= 8; i++)
            {
                DatagridviewVdPTotalMesure.Columns.Add($"Measured{i}", $"Measured {i}");
            }
        }

        public void UpdateMeasurementData(List<double[]> measuredValues)
        {
            // อัปเดต DataGridView
            DatagridviewVdPTotalMesure.Rows.Clear();
            foreach (var row in measuredValues)
            {
                DatagridviewVdPTotalMesure.Rows.Add(row.Cast<object>().ToArray());
            }

            // อัปเดต Chart
            //UpdateChart(measuredValues);
        }

        /*private void UpdateChart(List<double[]> measuredValues)
        {
            .Series.Clear();
            for (int i = 0; i < 8; i++) // 8 ชุดข้อมูล
            {
                var series = new Series($"Measured {i + 1}");
                series.ChartType = SeriesChartType.Line;

                for (int j = 0; j < measuredValues.Count; j++)
                {
                    series.Points.AddXY(j + 1, measuredValues[j][i]);
                }

                chart.Series.Add(series);
            }
        }*/
    }
}
