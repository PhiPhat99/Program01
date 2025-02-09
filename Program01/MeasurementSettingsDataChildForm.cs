using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;
using System.Windows.Markup;
using System.Xml.Linq;
using static Program01.MeasurementSettingsChildForm;

namespace Program01
{
    public partial class MeasurementSettingsDataChildForm : Form
    {

        public MeasurementSettingsDataChildForm()
        {
            InitializeComponent();
        }

        private void MeasurementSettingsDataChildForm_Load(object sender, EventArgs e)
        {

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
                double XMax = XData.Max();
                double YMax = YData.Max();

                if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} ({GlobalSettings.StepUnit})";
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} ({GlobalSettings.StepUnit})";
                }

                else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} ({GlobalSettings.StepUnit})";
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} ({GlobalSettings.SourceLimitUnit})";
                }

                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} ({GlobalSettings.SourceLimitUnit})";
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} ({GlobalSettings.StepUnit})";
                }

                else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = $"{GlobalSettings.SourceMode} ({GlobalSettings.StepUnit})";
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = $"{GlobalSettings.MeasureMode} ({GlobalSettings.StepUnit})";
                }

                for (int i = 0; i < XData.Count; i++)
                {
                    if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        if (GlobalSettings.StepUnit == "mV")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "V")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }

                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Current")
                    {
                        if (GlobalSettings.StepUnit == "µA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E4, YData[i] * 10E4);
                        }
                        else if (GlobalSettings.StepUnit == "mA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "A")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }

                    else if (GlobalSettings.SourceMode == "Voltage" && GlobalSettings.MeasureMode == "Current")
                    {
                        if (GlobalSettings.StepUnit == "mV" && GlobalSettings.SourceLimitUnit == "µA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i] * 10E4);
                        }
                        else if (GlobalSettings.StepUnit == "mV" && GlobalSettings.SourceLimitUnit == "mA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "mV" && GlobalSettings.SourceLimitUnit == "A")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i]);
                        }
                        if (GlobalSettings.StepUnit == "V" && GlobalSettings.SourceLimitUnit == "µA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i] * 10E4);
                        }
                        else if (GlobalSettings.StepUnit == "V" && GlobalSettings.SourceLimitUnit == "mA")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "V" && GlobalSettings.SourceLimitUnit == "A")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }

                    else if (GlobalSettings.SourceMode == "Current" && GlobalSettings.MeasureMode == "Voltage")
                    {
                        if (GlobalSettings.StepUnit == "µA" && GlobalSettings.SourceLimitUnit == "mV")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E4, YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "mA" && GlobalSettings.SourceLimitUnit == "mV")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i] * 10);
                        }
                        else if (GlobalSettings.StepUnit == "A" && GlobalSettings.SourceLimitUnit == "mV")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i] * 10);
                        }
                        if (GlobalSettings.StepUnit == "µA" && GlobalSettings.SourceLimitUnit == "V")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10E4, YData[i]);
                        }
                        else if (GlobalSettings.StepUnit == "mA" && GlobalSettings.SourceLimitUnit == "V")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i] * 10, YData[i]);
                        }
                        else if (GlobalSettings.StepUnit == "A" && GlobalSettings.SourceLimitUnit == "V")
                        {
                            ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                        }
                    }
                }

                ChartTunerTesting.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";
                ChartTunerTesting.ChartAreas[0].AxisX.MajorTickMark.Size = 1.5f;
                ChartTunerTesting.ChartAreas[0].AxisY.LabelStyle.Format = "0.###";
                ChartTunerTesting.ChartAreas[0].AxisY.MajorTickMark.Size = 1.5f;
                ChartTunerTesting.Invalidate();
            }
        }

        private void MeasurementSettingsDataChildForm_FormClosing()
        {

        }
    }
}
