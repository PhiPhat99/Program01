using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            RichTextBoxSettings();
            LoadMeasurementResults();
            LoadTotalResistancesChart();
        }

        private void RichTextBoxSettings()
        {
            RichTextboxResMP1N.Text = "RA1 :";
            RichTextboxResMP1P.Text = "RA2 :";
            RichTextboxResMP2N.Text = "RA3 :";
            RichTextboxResMP2P.Text = "RA4 :";
            RichTextboxResMP3N.Text = "RB1 :";
            RichTextboxResMP3P.Text = "RB2 :";
            RichTextboxResMP4N.Text = "RB3 :";
            RichTextboxResMP4P.Text = "RB4 :";
            RichTextboxResA.Text = "RA :";
            RichTextboxResB.Text = "RB :";
            RichTextboxAvgRes.Text = "RAverage :";
            RichTextboxSheetRes.Text = "RSheet :";

            RichTextboxResMP1N.Location = new Point(135, 20);
            RichTextboxResMP1P.Location = new Point(135, 70);
            RichTextboxResMP2N.Location = new Point(135, 120);
            RichTextboxResMP2P.Location = new Point(135, 170);
            RichTextboxResMP3N.Location = new Point(135, 220);
            RichTextboxResMP3P.Location = new Point(135, 270);
            RichTextboxResMP4N.Location = new Point(135, 320);
            RichTextboxResMP4P.Location = new Point(135, 370);

            RichTextboxResA.Location = new Point(138, 420);
            RichTextboxResB.Location = new Point(138, 470); ;
            RichTextboxAvgRes.Location = new Point(100, 520);
            RichTextboxSheetRes.Location = new Point(110, 620);

            RichTextboxResMP1N.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP1P.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP2N.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP2P.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP3N.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP3P.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP4N.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResMP4P.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxResA.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxResB.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point); ;
            RichTextboxAvgRes.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxSheetRes.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxResMP1N.Select(1, 2);
            RichTextboxResMP1P.Select(1, 2);
            RichTextboxResMP2N.Select(1, 2);
            RichTextboxResMP2P.Select(1, 2);
            RichTextboxResMP3N.Select(1, 2);
            RichTextboxResMP3P.Select(1, 2);
            RichTextboxResMP4N.Select(1, 2);
            RichTextboxResMP4P.Select(1, 2);
            RichTextboxResA.Select(1, 1);
            RichTextboxResB.Select(1, 1);
            RichTextboxAvgRes.Select(1, 7);
            RichTextboxSheetRes.Select(1, 5);

            RichTextboxResMP1N.SelectionCharOffset = -8;
            RichTextboxResMP1P.SelectionCharOffset = -8;
            RichTextboxResMP2N.SelectionCharOffset = -8;
            RichTextboxResMP2P.SelectionCharOffset = -8;
            RichTextboxResMP3N.SelectionCharOffset = -8;
            RichTextboxResMP3P.SelectionCharOffset = -8;
            RichTextboxResMP4N.SelectionCharOffset = -8;
            RichTextboxResMP4P.SelectionCharOffset = -8;
            RichTextboxResA.SelectionCharOffset = -8;
            RichTextboxResB.SelectionCharOffset = -8;
            RichTextboxAvgRes.SelectionCharOffset = -8;
            RichTextboxSheetRes.SelectionCharOffset = -8;

            RichTextboxResMP1N.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP1P.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP2N.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP2P.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP3N.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP3P.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP4N.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResMP4P.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResA.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxResB.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxAvgRes.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetRes.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
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
                        resistanceTextBox.Text = GlobalSettings.Instance.ResistancesByPosition[i].ToString("F6");
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
                ChartTotalResistances.ChartAreas[0].AxisY.LabelStyle.Format = "E3";
                ChartTotalResistances.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
                ChartTotalResistances.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
                
                ChartTotalResistances.ChartAreas[0].RecalculateAxesScale();
                ChartTotalResistances.Invalidate();
            }
        }
    }
}

