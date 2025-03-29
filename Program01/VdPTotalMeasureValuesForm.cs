using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class VdPTotalMeasureValuesForm : Form
    {
        private DataGridView dataGridView;
        private TabControl tabControl;
        private BindingSource bindingSource;
        private CollectVdPMeasuredValue measuredValues;

        public VdPTotalMeasureValuesForm(CollectVdPMeasuredValue measuredValues)
        {
            InitializeComponent();
            this.measuredValues = measuredValues;
            bindingSource = new BindingSource();
            bindingSource.DataSource = measuredValues.GetData(); // ใช้ GetData() จาก CollectVdPMeasuredValue

            InitializeDataGridView();
            InitializeTabControl();
        }

        private void InitializeDataGridView()
        {
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Top,
                DataSource = bindingSource,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            for (int i = 1; i <= 8; i++)
            {
                dataGridView.Columns.Add($"Measured {i}", $"Measured {i}");
                dataGridView.Columns[$"Measured {i}"].DefaultCellStyle.Format = "F5";
            }

            Controls.Add(dataGridView);
        }

        private void InitializeTabControl()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            for (int i = 0; i <= 8; i++)
            {
                var tabPage = new TabPage(i == 0 ? "Total" : i.ToString());
                var chart = new Chart { Dock = DockStyle.Fill };
                var chartArea = new ChartArea();
                chart.ChartAreas.Add(chartArea);
                chart.Series.Add(new Series { ChartType = SeriesChartType.Line });
                tabPage.Controls.Add(chart);
                tabControl.TabPages.Add(tabPage);
            }

            Controls.Add(tabControl);
        }

        public void UpdateData()
        {
            bindingSource.ResetBindings(false);
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            // การอัปเดตกราฟ
            for (int i = 0; i <= 8; i++)
            {
                var chart = (Chart)tabControl.TabPages[i].Controls[0];
                chart.Series[0].Points.Clear();
                var data = measuredValues.GetIVCurveData(i);
                
                foreach (var point in data)
                {
                    chart.Series[0].Points.AddXY(point.Source, point.Reading);
                }
            }
        }
    }
}
