using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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

            if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
            {
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Measure) [V]";
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [V]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Measure) [A]";
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [V]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
            {
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Measure) [V]";
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [A]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Measure) [A]";
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [A]";
            }

            ChartAreas.AxisX.IsLabelAutoFit = false;
            ChartAreas.AxisY.IsLabelAutoFit = false;
            ChartAreas.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            ChartAreas.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

            for (int i = 0; i < XData.Count; i++)
            {
                ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
            }

            ChartAreas.AxisX.LabelStyle.Format = "E2";
            ChartAreas.AxisY.LabelStyle.Format = "E2";
            ChartTunerTesting.Invalidate();

        }

        public void UpdateMeasurementData(double MaxMeasure, double MinMeasure, double MaxSource, double MinSource, double Slope)
        {
            if (TextboxMaxMeasureValue == null || TextboxMinMeasureValue == null || TextboxMaxSourceValue == null || TextboxMinSourceValue == null || TextboxSlopeValue == null)
            {
                MessageBox.Show("Error: Measurement textboxes are not initialized.", "Error", MessageBoxButtons.OK);
                return;
            }

            TextboxMaxMeasureValue.Text = MaxMeasure.ToString("E2");
            TextboxMinMeasureValue.Text = MinMeasure.ToString("E2");
            TextboxMaxSourceValue.Text = MaxSource.ToString("E2");
            TextboxMinSourceValue.Text = MinSource.ToString("E2");
            TextboxSlopeValue.Text = Slope.ToString("E2");

            if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                LabelMaxMeasureUnit.Text = "A";
                LabelMinMeasureUnit.Text = "A";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "Ω⁻¹";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "A";
                LabelMinSourceUnit.Text = "A";
                LabelSlopeUnit.Text = "Ω";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Current")
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
