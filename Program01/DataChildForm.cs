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
            GlobalSettingsParseValues.Instance.OnSettingsChanged += UpdateFromGlobalSettings;
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

            if (GlobalSettingsForUI.Instance.SourceModeUI == "Voltage" && GlobalSettingsForUI.Instance.MeasureModeUI == "Voltage")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettingsForUI.Instance.SourceModeUI} (Source) [{GlobalSettingsForUI.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"{GlobalSettingsForUI.Instance.MeasureModeUI} (Source) [{GlobalSettingsForUI.Instance.StepUnitUI}]";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Voltage" && GlobalSettingsForUI.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisX.Title = $"{GlobalSettingsForUI.Instance.SourceModeUI} (Source) [V]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettingsForUI.Instance.SourceLimitLevelUnitUI}]";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Current" && GlobalSettingsForUI.Instance.MeasureModeUI == "Voltage")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettingsForUI.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"{GlobalSettingsForUI.Instance.MeasureModeUI} (Measure) [V]";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Current" && GlobalSettingsForUI.Instance.MeasureModeUI == "Current")
            {
                ChartAreas.AxisX.Title = $"Current (Source) [{GlobalSettingsForUI.Instance.StepUnitUI}]";
                ChartAreas.AxisY.Title = $"Current (Measure) [{GlobalSettingsForUI.Instance.StepUnitUI}]";
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

            if (GlobalSettingsForUI.Instance.SourceModeUI == "Voltage" && GlobalSettingsForUI.Instance.MeasureModeUI == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Voltage" && GlobalSettingsForUI.Instance.MeasureModeUI == "Current")
            {
                LabelMaxMeasureUnit.Text = "A";
                LabelMinMeasureUnit.Text = "A";
                LabelMaxSourceUnit.Text = "V";
                LabelMinSourceUnit.Text = "V";
                LabelSlopeUnit.Text = "Ω⁻¹";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Current" && GlobalSettingsForUI.Instance.MeasureModeUI == "Voltage")
            {
                LabelMaxMeasureUnit.Text = "V";
                LabelMinMeasureUnit.Text = "V";
                LabelMaxSourceUnit.Text = "A";
                LabelMinSourceUnit.Text = "A";
                LabelSlopeUnit.Text = "Ω";
            }
            else if (GlobalSettingsForUI.Instance.SourceModeUI == "Current" && GlobalSettingsForUI.Instance.MeasureModeUI == "Current")
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
            if (GlobalSettingsParseValues.Instance.XDataBuffer.Count > 0 && GlobalSettingsParseValues.Instance.YDataBuffer.Count > 0)
            {
                UpdateChart(GlobalSettingsParseValues.Instance.XDataBuffer, GlobalSettingsParseValues.Instance.YDataBuffer);
                UpdateMeasurementData(GlobalSettingsParseValues.Instance.MaxMeasure, GlobalSettingsParseValues.Instance.MinMeasure, GlobalSettingsParseValues.Instance.MaxSource, GlobalSettingsParseValues.Instance.MinSource, GlobalSettingsParseValues.Instance.Slope);
            }
        }

        private void DataChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalSettingsParseValues.Instance.OnSettingsChanged -= UpdateFromGlobalSettings;
        }
    }
}
