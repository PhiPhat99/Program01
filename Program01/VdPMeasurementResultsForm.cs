using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace Program01
{
    public partial class VdPMeasurementResultsForm : Form
    {
        public class ResistanceData
        {
            public int Position { get; set; }
            public double Resistance { get; set; }
        }

        public VdPMeasurementResultsForm()
        {
            InitializeComponent();
            LoadMeasurementResults();
            LoadTotalResistancesChart();
        }
        public void LoadMeasurementResults ()
        {
            TextboxSourceMode.Text = GlobalSettings.Instance.SourceModeUI;
            TextboxMeasureMode.Text = GlobalSettings.Instance.MeasureModeUI;

            for (int i = 1; i <= 8; i++)
            {
                if (Controls.Find($"TextboxRes{i}", true).FirstOrDefault() is TextBox resistanceTextBox)
                {
                    if (GlobalSettings.Instance.ResistancesByPosition.ContainsKey(i) && !double.IsNaN(GlobalSettings.Instance.ResistancesByPosition[i]))
                    {
                        resistanceTextBox.Text = GlobalSettings.Instance.ResistancesByPosition[i].ToString("F5");
                    }
                    else
                    {
                        resistanceTextBox.Text = "N/A";
                    }
                }
                if (Controls.Find($"TextboxRes{i}Unit", true).FirstOrDefault() is TextBox unitTextBox)
                {
                    unitTextBox.Text = "Ω";
                }
            }

            if (Controls.Find("TextboxResA", true).FirstOrDefault() is TextBox resistanceATextBox)
            {
                resistanceATextBox.Text = !double.IsNaN(GlobalSettings.Instance.ResistanceA) ? GlobalSettings.Instance.ResistanceA.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxResAUnit", true).FirstOrDefault() is TextBox unitATextBox)
            {
                unitATextBox.Text = "Ω";
            }

            if (Controls.Find("TextboxResB", true).FirstOrDefault() is TextBox resistanceBTextBox)
            {
                resistanceBTextBox.Text = !double.IsNaN(GlobalSettings.Instance.ResistanceB) ? GlobalSettings.Instance.ResistanceB.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxResBUnit", true).FirstOrDefault() is TextBox unitBTextBox)
            {
                unitBTextBox.Text = "Ω";
            }


            if (Controls.Find("TextboxAvgRes", true).FirstOrDefault() is TextBox averageResistanceTextBox)
            {
                averageResistanceTextBox.Text = !double.IsNaN(GlobalSettings.Instance.AverageResistanceAll) ? GlobalSettings.Instance.AverageResistanceAll.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxAvgResUnit", true).FirstOrDefault() is TextBox averageUnitTextBox)
            {
                averageUnitTextBox.Text = "Ω";
            }

            if (Controls.Find("TextboxThickness", true).FirstOrDefault() is TextBox thicknessTextBox)
            {
                thicknessTextBox.Text = $"{GlobalSettings.Instance.ThicknessValueUI}";
            }
            if (Controls.Find("TextboxThicknessUnit", true).FirstOrDefault() is TextBox thicknessUnitTextBox)
            {
                thicknessUnitTextBox.Text = $"{GlobalSettings.Instance.ThicknessUnitUI}";
            }

            if (Controls.Find("TextboxSheetRes", true).FirstOrDefault() is TextBox sheetresistanceTextBox)
            {
                sheetresistanceTextBox.Text = !double.IsNaN(GlobalSettings.Instance.SheetResistance) ? GlobalSettings.Instance.SheetResistance.ToString("F5") : "N/A";
                Debug.WriteLine($"Sheet Resistance: {GlobalSettings.Instance.SheetResistance}");
            }
            if (Controls.Find("TextboxSheetResUnit", true).FirstOrDefault() is TextBox sheetresistanceunitTextBox)
            {
                sheetresistanceunitTextBox.Text = "Ω / Sqr";
            }

            if (Controls.Find("TextboxResistivity", true).FirstOrDefault() is TextBox resistivityTextBox)
            {
                resistivityTextBox.Text = !double.IsNaN(GlobalSettings.Instance.Resistivity) ? GlobalSettings.Instance.Resistivity.ToString("F5") : "N/A";
                Debug.WriteLine($"Resistivity: {GlobalSettings.Instance.SheetResistance} x {GlobalSettings.Instance.ThicknessValueStd}");
            }
            if (Controls.Find("TextboxResistivityUnit", true).FirstOrDefault() is TextBox resistivityunitTextBox)
            {
                resistivityunitTextBox.Text = "Ω ⋅ m";
            }

            if (Controls.Find("TextboxConductivity", true).FirstOrDefault() is TextBox conductivityTextBox)
            { 
                conductivityTextBox.Text = !double.IsNaN(GlobalSettings.Instance.Conductivity) ? GlobalSettings.Instance.Conductivity.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxConductivityUnit", true).FirstOrDefault() is TextBox conductivityunitTextBox)
            {
                conductivityunitTextBox.Text = "S / m";
            }
        }

        public void LoadTotalResistancesChart()
        {
            Debug.WriteLine("[DEBUG] LoadTotalResistancesChart() called");

            if (ChartTotalResistances == null)
            {
                Debug.WriteLine("[ERROR] Chart Control 'ChartTotalResistances' ไม่พบใน Form");
                return;
            }

            List<ResistanceData> resistanceDataSource = new List<ResistanceData>();

            for (int i = 1; i <= 8; i++)
            {
                if (GlobalSettings.Instance.ResistancesByPosition.ContainsKey(i))
                {
                    resistanceDataSource.Add(new ResistanceData { Position = i, Resistance = GlobalSettings.Instance.ResistancesByPosition[i] });
                }
                else
                {
                    resistanceDataSource.Add(new ResistanceData { Position = i, Resistance = double.NaN });
                }
            }

            ChartTotalResistances.DataSource = resistanceDataSource;

            if (ChartTotalResistances.Series.Count > 0)
            {
                Series resistanceSeries = ChartTotalResistances.Series[0];
            }

            if (ChartTotalResistances.ChartAreas.Count > 0)
            {
                ChartTotalResistances.ChartAreas[0].RecalculateAxesScale();
            }
        }
    }
}

