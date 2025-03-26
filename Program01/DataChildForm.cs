using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;
using System.Windows.Markup;
using System.Xml.Linq;
using static Program01.MeasurementSettingsForm;

namespace Program01
{
    public partial class DataChildForm : Form
    {
        public DataChildForm()
        {
            InitializeComponent();
            GlobalSettings.OnSettingsChanged += UpdateFromGlobalSettings;
        }

        private void DataChildForm_Load(object sender, EventArgs e)
        {
            UpdateFromGlobalSettings();
        }

        private void UpdateFromGlobalSettings()
        {
            UpdateChart(GlobalSettings.XDataBuffer, GlobalSettings.YDataBuffer);
            UpdateMeasurementData(GlobalSettings.MaxMeasure, GlobalSettings.MinMeasure, GlobalSettings.MaxSource, GlobalSettings.MinSource, GlobalSettings.Slope);
        }

        public void UpdateChart(List<double> XData, List<double> YData)
        {
            if (ChartTunerTesting.InvokeRequired)
            {
                ChartTunerTesting.Invoke(new Action(() => UpdateChart(XData, YData)));
            }
            else
            {
                ChartTunerTesting.Series["MeasurementData"].Points.Clear();  //*** เกิด Runtime Error

                var chartArea = ChartTunerTesting.ChartAreas[0];

                // ตั้งค่าแกน X และ Y ตามหน่วยที่ตั้งไว้
                if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                {
                    chartArea.AxisX.Title = $"{GlobalSettings.SourceMode} (Source) [{GlobalSettings.StepUnit}]";
                    chartArea.AxisY.Title = $"{GlobalSettings.MeasureMode} (Source) [{GlobalSettings.StepUnit}]";
                }
                else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                {
                    chartArea.AxisX.Title = $"{GlobalSettings.SourceMode} (Source) [V]";
                    chartArea.AxisY.Title = $"Current (Measure) [{GlobalSettings.SourceLimitLevelUnit}]";
                }
                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                {
                    chartArea.AxisX.Title = $"Current (Source) [{GlobalSettings.StepUnit}]";
                    chartArea.AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) [V]";
                }
                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                {
                    chartArea.AxisX.Title = $"Current (Source) [{GlobalSettings.StepUnit}]";
                    chartArea.AxisY.Title = $"Current (Measure) [{GlobalSettings.StepUnit}]";
                }

                // ป้องกันตัวเลขแกนเอียง
                chartArea.AxisX.LabelStyle.Angle = 90;
                chartArea.AxisY.LabelStyle.Angle = 0;
                chartArea.AxisX.IsLabelAutoFit = false;
                chartArea.AxisY.IsLabelAutoFit = false;

                // ตั้งค่าขนาดของ Label และ Title
                chartArea.AxisX.TitleFont = new Font("Angsana New", 12, FontStyle.Bold);
                chartArea.AxisY.TitleFont = new Font("Angsana New", 12, FontStyle.Bold);
                chartArea.AxisX.LabelStyle.Font = new Font("Angsana New", 10);
                chartArea.AxisY.LabelStyle.Font = new Font("Angsana New", 10);

                // ป้องกันการซ้อนทับของ Label
                chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chartArea.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

                // เพิ่มจุดข้อมูลลงกราฟ
                for (int i = 0; i < XData.Count; i++)
                {
                    ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                }

                // ตั้งค่าการจัดรูปแบบตัวเลข
                chartArea.AxisX.LabelStyle.Format = "0.###";
                chartArea.AxisY.LabelStyle.Format = "0.###";
                ChartTunerTesting.Invalidate();
            }
        }

        public void UpdateMeasurementData(double MaxMeasure, double MinMeasure, double MaxSource, double MinSource, double Slope)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateMeasurementData(MaxMeasure, MinMeasure, MaxSource, MinSource, Slope)));
            }
            else
            {
                try
                {
                    UpdateMeasurementValues(MaxMeasure, MinMeasure, MaxSource, MinSource, Slope);

                    // ตั้งค่าหน่วยให้ถูกต้อง
                    if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        LabelMaxMeasureUnit.Text = "V";
                        LabelMinMeasureUnit.Text = "V";
                        LabelMaxSourceUnit.Text = "V";
                        LabelMinSourceUnit.Text = "V";
                        LabelSlopeUnit.Text = "";
                    }
                    else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                    {
                        LabelMaxMeasureUnit.Text = "A";
                        LabelMinMeasureUnit.Text = "A";
                        LabelMaxSourceUnit.Text = "V";
                        LabelMinSourceUnit.Text = "V";
                        LabelSlopeUnit.Text = "Ω⁻¹";
                    }
                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        LabelMaxMeasureUnit.Text = "V";
                        LabelMinMeasureUnit.Text = "V";
                        LabelMaxSourceUnit.Text = "A";
                        LabelMinSourceUnit.Text = "A";
                        LabelSlopeUnit.Text = "Ω";
                    }
                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                    {
                        LabelMaxMeasureUnit.Text = "A";
                        LabelMinMeasureUnit.Text = "A";
                        LabelMaxSourceUnit.Text = "A";
                        LabelMinSourceUnit.Text = "A";
                        LabelSlopeUnit.Text = "";
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show($"Error: {Ex.Message}");
                }
            }
        }

        private void UpdateMeasurementValues(double MaxMeasure, double MinMeasure, double MaxSource, double MinSource, double Slope)
        {
            TextboxMaxMeasureValue.Text = MaxMeasure.ToString("0.######");
            TextboxMinMeasureValue.Text = MinMeasure.ToString("0.######");
            TextboxMaxSourceValue.Text = MaxSource.ToString("0.######");
            TextboxMinSourceValue.Text = MinSource.ToString("0.######");
            TextboxSlopeValue.Text = Slope.ToString("0.######");
        }
    }
}
