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
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [{GlobalSettings.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Source) [{GlobalSettings.Instance.StepUnitUI}]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettings.Instance.SourceModeUI} (Source) [V]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.Instance.SourceLimitLevelUnitUI}]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"{GlobalSettings.Instance.MeasureModeUI} (Measure) [V]";
            }
            else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettings.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettings.Instance.StepUnitUI}]";
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
