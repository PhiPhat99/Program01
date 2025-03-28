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

        // เมธอดในการอัปเดตข้อมูลทั้งใน Chart และ DataGridView
        public void UpdateChartAndDataGridView(List<double> XData, List<double> YData)
        {
            if (InvokeRequired)
            {
                // เรียกใช้งานใน Thread ของ UI
                BeginInvoke(new Action(() => UpdateChartAndDataGridView(XData, YData)));
                return;
            }

            // อัปเดตข้อมูลใน Chart
            UpdateChart(XData, YData);

            // อัปเดตข้อมูลใน DataGridView
            UpdateDataGridView(YData);
        }

        // เมธอดสำหรับอัปเดตข้อมูลใน Chart
        private void UpdateChart(List<double> XData, List<double> YData)
        {
            // ค้นหา chart ในทุกๆ TabPage ของ TabControl
            foreach (TabPage tabPage in TabcontrolVdPTotalCharts.TabPages)
            {
                if (tabPage.Controls.ContainsKey("chartVdP"))
                {
                    var chart = tabPage.Controls["chartVdP"] as Chart;
                    if (chart != null)
                    {
                        var series = chart.Series[0];
                        series.Points.Clear(); // เคลียร์ข้อมูลเก่า

                        // เพิ่มข้อมูลใหม่ใน Chart
                        for (int i = 0; i < XData.Count; i++)
                        {
                            series.Points.AddXY(XData[i], YData[i]);
                        }
                    }
                }
            }
        }

        // เมธอดสำหรับอัปเดตข้อมูลใน DataGridView
        private void UpdateDataGridView(List<double> YData)
        {
            DatagridviewVdPTotalMesure.Rows.Clear(); // เคลียร์ข้อมูลเก่า

            // เพิ่มข้อมูลใหม่ใน DataGridView
            foreach (var value in YData)
            {
                int rowIndex = DatagridviewVdPTotalMesure.Rows.Add();
                DatagridviewVdPTotalMesure.Rows[rowIndex].Cells["MeasuredValue1"].Value = value.ToString("F5");
            }
        }

        // เมธอดโหลดข้อมูลการวัดจากแหล่งข้อมูล (อาจใช้สำหรับโหลดข้อมูลเริ่มต้น)
        private void LoadVdPTotalMeasurementData()
        {
            var VdPData = CollectVdPMeasuredValue.Instance;
            DatagridviewVdPTotalMesure.Rows.Clear(); // เคลียร์ข้อมูลเก่า

            if (DatagridviewVdPTotalMesure.Columns.Count == 0)
            {
                // เพิ่มคอลัมน์หากยังไม่มี
                AddColumnsToDataGridView();
            }

            // เติมข้อมูลการวัดจาก VdPData
            foreach (var readings in VdPData.VdPMeasured)
            {
                int rowIndex = DatagridviewVdPTotalMesure.Rows.Add();
                for (int Cols = 0; Cols < DatagridviewVdPTotalMesure.Columns.Count; Cols++)
                {
                    DatagridviewVdPTotalMesure.Rows[rowIndex].Cells[Cols].Value = readings;
                }
            }
        }

        // เมธอดสำหรับเพิ่มคอลัมน์ลงใน DataGridView
        private void AddColumnsToDataGridView()
        {
            var columns = new[]
            {
        "MeasuredValue1", "MeasuredValue2", "MeasuredValue3", "MeasuredValue4",
        "MeasuredValue5", "MeasuredValue6", "MeasuredValue7", "MeasuredValue8"
    };

            foreach (var col in columns)
            {
                DatagridviewVdPTotalMesure.Columns.Add(col, col);
            }
        }

        // เมธอดที่จะถูกเรียกเมื่อฟอร์มโหลด
        private void VdPTotalMeasureValueForm_Load(object sender, EventArgs e)
        {
            LoadVdPTotalMeasurementData();
        }

        // เมธอดที่ใช้ในการจัดรูปแบบข้อมูลใน DataGridView
        private void DatagridviewVdPTotalMesure_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name.StartsWith("MeasuredValue"))
            {
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double result))
                {
                    e.Value = result.ToString("F5");  // แสดงผล 5 ตำแหน่งหลังจุดทศนิยม
                }
            }
        }

        // เมธอดในการสมัครสมาชิก (subscribe) กับเหตุการณ์จากฟอร์ม MeasurementSettingsForm
        public void SubscribeToMeasurement(MeasurementSettingsForm SettingsForm)
        {
            // ตรวจสอบว่าเหตุการณ์ในฟอร์ม MeasurementSettingsForm ถูกต้อง
            SettingsForm.MeasurementCompleted += UpdateChartAndDataGridView;
        }
    }
}
