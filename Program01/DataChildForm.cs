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
            GlobalSettings.Instance.OnSettingsChanged += UpdateFromGlobalSettings;
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

            if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Voltage")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceMode} (Source) [{GlobalSettings.Instance.StepUnit}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureMode} (Source) [{GlobalSettings.Instance.StepUnit}]";
            }
            else if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Current")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceMode} (Source) [V]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.Instance.SourceLimitLevelUnit}]";
            }
            else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Voltage")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.Instance.StepUnit}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureMode} (Measure) [V]";
            }
            else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Current")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.Instance.StepUnit}]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.Instance.StepUnit}]";
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

            if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "";
            }
            else if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Current")
            {
                LabelMaxMeasureUnit.Text = "A";
                LabelMinMeasureUnit.Text = "A";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "Ω⁻¹";
            }
            else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "A";
                LabelMinSourceUnit.Text = "A";
                LabelSlopeUnit.Text = "Ω";
            }
            else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Current")
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
            if (GlobalSettings.Instance.XDataBuffer.Count > 0 && GlobalSettings.Instance.YDataBuffer.Count > 0)
            {
                UpdateChart(GlobalSettings.Instance.XDataBuffer, GlobalSettings.Instance.YDataBuffer);
                UpdateMeasurementData(GlobalSettings.Instance.MaxMeasure, GlobalSettings.Instance.MinMeasure, GlobalSettings.Instance.MaxSource, GlobalSettings.Instance.MinSource, GlobalSettings.Instance.Slope);
            }
        }

        private void DataChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalSettings.Instance.OnSettingsChanged -= UpdateFromGlobalSettings;
        }
    }
}
