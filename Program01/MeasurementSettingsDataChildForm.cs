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
using System.Windows.Markup;
using System.Xml.Linq;

namespace Program01
{
    public partial class MeasurementSettingsDataChildForm : Form
    {
        public string MeasureMode;
        public string SourceMode;

        public MeasurementSettingsDataChildForm()
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

                for (int i = 0; i < XData.Count; i++)
                {
                    ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                }
                
                if (SourceMode == "Current")
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = "Current";
                }
                else
                {
                    ChartTunerTesting.ChartAreas[0].AxisX.Title = "Voltage";
                }

                if (MeasureMode == "Current")
                {
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = "Current";
                }
                else
                {
                    ChartTunerTesting.ChartAreas[0].AxisY.Title = "Voltage";
                }

                ChartTunerTesting.Series["MeasurementData"].Points.Clear();

                for (int i = 0; i < XData.Count; i++)
                {
                    ChartTunerTesting.Series["MeasurementData"].Points.AddXY(XData[i], YData[i]);
                }
            }
        }
    }
}
