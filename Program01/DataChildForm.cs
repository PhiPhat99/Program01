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
        }   

        public void UpdateChart(List<double> XData, List<double> YData)
        {
            if (ChartTunerTesting.InvokeRequired)
            {
                ChartTunerTesting.Invoke(new Action(() => UpdateChart(XData, YData)));
            }
            else
            {
                ChartTunerTesting.Series["MeasurementData"].Points.Clear();

                if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) V";
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) V";
                }

                else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) V";

                    if (GlobalSettings.SourceLimitLevelUnit == "nA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) µA";
                    }
                    else if (GlobalSettings.SourceLimitLevelUnit == "µA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) mA";
                    }
                    else if (GlobalSettings.SourceLimitLevelUnit == "mA" || GlobalSettings.SourceLimitLevelUnit == "A")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) A";
                    }
                }

                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                {
                    if (GlobalSettings.StepUnit == "nA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) µA";
                    }
                    else if (GlobalSettings.StepUnit == "µA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) mA";
                    }
                    else if (GlobalSettings.StepUnit == "mA" || GlobalSettings.StepUnit == "A")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) A";
                    }

                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) V";
                }

                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                {
                    if (GlobalSettings.StepUnit == "nA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) µA";
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) µA";
                    }
                    else if (GlobalSettings.StepUnit == "µA")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) mA";
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) mA";
                    }
                    else if (GlobalSettings.StepUnit == "mA" || GlobalSettings.StepUnit == "A")
                    {
                        ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} (Source) A";
                        ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} (Measure) A";
                    }
                }

                for (int i = 0; i < XData.Count; i++)
                {
                    if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                    }

                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                    {
                        if (GlobalSettings.StepUnit == "nA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E5, YData[i] * 10E5);
                        }
                        else if (GlobalSettings.StepUnit == "µA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E2, YData[i] * 10E2);
                        }
                        else if (GlobalSettings.StepUnit == "mA" || GlobalSettings.StepUnit == "A")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }

                    else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                    {
                        if ((GlobalSettings.StepUnit == "mV" || GlobalSettings.StepUnit == "V") && GlobalSettings.SourceLimitLevelUnit == "nA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i] * 10E5);
                        }
                        else if ((GlobalSettings.StepUnit == "mV" || GlobalSettings.StepUnit == "V") && GlobalSettings.SourceLimitLevelUnit == "µA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i] * 10E2);
                        }
                        else if ((GlobalSettings.StepUnit == "mV" || GlobalSettings.StepUnit == "V") && (GlobalSettings.SourceLimitLevelUnit == "mA" || GlobalSettings.SourceLimitLevelUnit == "A"))
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }

                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        if (GlobalSettings.StepUnit == "nA" && (GlobalSettings.SourceLimitLevelUnit == "mV" || GlobalSettings.SourceLimitLevelUnit == "V"))
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E5, YData[i]);
                        }
                        else if (GlobalSettings.StepUnit == "µA" && (GlobalSettings.SourceLimitLevelUnit == "mV" || GlobalSettings.SourceLimitLevelUnit == "V"))
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E2, YData[i]);
                        }
                        else if ((GlobalSettings.StepUnit == "mA" || GlobalSettings.StepUnit == "A") && (GlobalSettings.SourceLimitLevelUnit == "mV" || GlobalSettings.SourceLimitLevelUnit == "V"))
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }
                }

                ChartTunerTesting.ChartAreas[0].AxisX.LabelStyle.Format = "0.####";
                ChartTunerTesting.ChartAreas[0].AxisX.MajorTickMark.Size = 1.5f;
                ChartTunerTesting.ChartAreas[0].AxisY.LabelStyle.Format = "0.####";
                ChartTunerTesting.ChartAreas[0].AxisY.MajorTickMark.Size = 1.5f;
                ChartTunerTesting.Invalidate();
            }
        }

        public void UpdateMeasurementData(double MaxMeasure, double MinMeasure, double MaxSource, double MinSource, double Slope)
        {
            try
            {
                TextboxMaxMeasureValue.Text = MaxMeasure.ToString("F5");
                TextboxMinMeasureValue.Text = MinMeasure.ToString("F5");
                TextboxMaxSourceValue.Text = MaxSource.ToString("F5");
                TextboxMinSourceValue.Text = MinSource.ToString("F5");
                TextboxSlopeValue.Text = Slope.ToString("F5");
                
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
