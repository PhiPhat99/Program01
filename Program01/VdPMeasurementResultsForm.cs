using System.Linq;
using System.Windows.Forms;

namespace Program01
{
    public partial class VdPMeasurementResultsForm : Form
    {
        public VdPMeasurementResultsForm()
        {
            InitializeComponent();
            LoadMeasurementResults();
            LoadTotalResistancesChart();
        }

        public void LoadMeasurementResults ()
        {
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
                thicknessTextBox.Text = !double.IsNaN(GlobalSettings.Instance.ThicknessValue) ? GlobalSettings.Instance.ThicknessValue.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxThicknessUnit", true).FirstOrDefault() is TextBox thicknessUnitTextBox)
            {
                thicknessUnitTextBox.Text = GlobalSettings.Instance.ThicknessUnit;
            }

            if (Controls.Find("TextboxSheetRes", true).FirstOrDefault() is TextBox sheetresistanceTextBox)
            {
                sheetresistanceTextBox.Text = !double.IsNaN(GlobalSettings.Instance.SheetResistance) ? GlobalSettings.Instance.SheetResistance.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxSheetResUnit", true).FirstOrDefault() is TextBox sheetresistanceunitTextBox)
            {
                sheetresistanceunitTextBox.Text = "Ω / Sqr";
            }
        }

        public void LoadTotalResistancesChart()
        {

        }
    }
}
