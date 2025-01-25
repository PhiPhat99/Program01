using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class MeasurementSettingsDataChildForm : Form
    {
        public MeasurementSettingsDataChildForm()
        {
            InitializeComponent();
        }

        public void UpdateChartData(int[] data)
        {
            try
            {
                if (chartTunerTesting.Series.Count == 0)
                {
                    chartTunerTesting.Series.Add("Series 1"); // เพิ่ม Series ใหม่ถ้ายังไม่มี
                }

                var series = chartTunerTesting.Series["I-V Curve"];
                series.Points.Clear(); // ลบข้อมูลเก่า
                foreach (var value in data)
                {
                    series.Points.Add(value); // เพิ่มข้อมูลใหม่ใน Chart
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UpdateChartData: {ex.Message}");
            }
        }
    }
}
