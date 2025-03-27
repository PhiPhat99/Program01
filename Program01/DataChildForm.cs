using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Program01;
using static Program01.MeasurementSettingsForm;

namespace Program01
{
    public partial class DataChildForm : Form
    {
        public DataChildForm()
        {
            InitializeComponent();
        }

        private void DataChildForm_Load(object sender, EventArgs e)
        {
            GlobalSettings.OnSettingsChanged += UpdateFromGlobalSettings;
            UpdateFromGlobalSettings();
        }

        public void UpdateChart(List<double> XData, List<double> YData)
        {
            if (ChartTunerTesting == null)
            {
                MessageBox.Show("Error: ChartTunerTesting is not initialized.", "Error", MessageBoxButtons.OK);
                return;
            }

            ChartTunerTesting.Series["MeasurementData"].Points.Clear();
            var ChartAreas = ChartTunerTesting.ChartAreas[0];

            if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettings.SourceMode} (Source) [{GlobalSettings.StepUnit}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.MeasureMode} (Source) [{GlobalSettings.StepUnit}]";
            }
            else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettings.SourceMode} (Source) [V]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.SourceLimitLevelUnit}]";
            }
            else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.StepUnit}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) [V]";
            }
            else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.StepUnit}]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.StepUnit}]";
            }

            ChartAreas.AxisX.LabelStyle.Angle = 90;
            ChartAreas.AxisY.LabelStyle.Angle = 0;
            ChartAreas.AxisX.IsLabelAutoFit = false;
            ChartAreas.AxisY.IsLabelAutoFit = false;

            ChartAreas.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            ChartAreas.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

            for (int i = 0; i < XData.Count; i++)
            {
                ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
            }

            ChartAreas.AxisX.LabelStyle.Format = "0.#####";
            ChartAreas.AxisY.LabelStyle.Format = "0.#####";
            ChartTunerTesting.Invalidate();

        }

        public void UpdateMeasurementData(double MaxMeasure, double MinMeasure, double MaxSource, double MinSource, double Slope)
        {
            if (TextboxMaxMeasureValue == null || TextboxMinMeasureValue == null || TextboxMaxSourceValue == null || TextboxMinSourceValue == null || TextboxSlopeValue == null)
            {
                MessageBox.Show("Error: Measurement textboxes are not initialized.", "Error", MessageBoxButtons.OK);
                return;
            }

            TextboxMaxMeasureValue.Text = MaxMeasure.ToString("0.######");
            TextboxMinMeasureValue.Text = MinMeasure.ToString("0.######");
            TextboxMaxSourceValue.Text = MaxSource.ToString("0.######");
            TextboxMinSourceValue.Text = MinSource.ToString("0.######");
            TextboxSlopeValue.Text = Slope.ToString("0.######");

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

        private void UpdateFromGlobalSettings()
        {
            if (GlobalSettings.XDataBuffer.Count > 0 && GlobalSettings.YDataBuffer.Count > 0)
            {
                UpdateChart(GlobalSettings.XDataBuffer, GlobalSettings.YDataBuffer);
                UpdateMeasurementData(GlobalSettings.MaxMeasure, GlobalSettings.MinMeasure, GlobalSettings.MaxSource, GlobalSettings.MinSource, GlobalSettings.Slope);
            }
        }

        private void DataChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalSettings.OnSettingsChanged -= UpdateFromGlobalSettings;
        }
    }
}
