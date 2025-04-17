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
                    if (GlobalSettingsParseValues.Instance.ResistancesByPosition.ContainsKey(i) && !double.IsNaN(GlobalSettingsParseValues.Instance.ResistancesByPosition[i]))
                    {
                        resistanceTextBox.Text = GlobalSettingsParseValues.Instance.ResistancesByPosition[i].ToString("F5");
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
                resistanceATextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.ResistanceA) ? GlobalSettingsParseValues.Instance.ResistanceA.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxResAUnit", true).FirstOrDefault() is TextBox unitATextBox)
            {
                unitATextBox.Text = "Ω";
            }

            if (Controls.Find("TextboxResB", true).FirstOrDefault() is TextBox resistanceBTextBox)
            {
                resistanceBTextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.ResistanceB) ? GlobalSettingsParseValues.Instance.ResistanceB.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxResBUnit", true).FirstOrDefault() is TextBox unitBTextBox)
            {
                unitBTextBox.Text = "Ω";
            }


            if (Controls.Find("TextboxAvgRes", true).FirstOrDefault() is TextBox averageResistanceTextBox)
            {
                averageResistanceTextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.AverageResistanceAll) ? GlobalSettingsParseValues.Instance.AverageResistanceAll.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxAvgResUnit", true).FirstOrDefault() is TextBox averageUnitTextBox)
            {
                averageUnitTextBox.Text = "Ω";
            }

            if (Controls.Find("TextboxThickness", true).FirstOrDefault() is TextBox thicknessTextBox)
            {
                thicknessTextBox.Text = !double.IsNaN(GlobalSettingsForUI.Instance.ThicknessValueUI) ? GlobalSettingsForUI.Instance.ThicknessValueUI.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxThicknessUnit", true).FirstOrDefault() is TextBox thicknessUnitTextBox)
            {
                thicknessUnitTextBox.Text = GlobalSettingsForUI.Instance.ThicknessUnitUI;
            }

            if (Controls.Find("TextboxSheetRes", true).FirstOrDefault() is TextBox sheetresistanceTextBox)
            {
                sheetresistanceTextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.SheetResistance) ? GlobalSettingsParseValues.Instance.SheetResistance.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxSheetResUnit", true).FirstOrDefault() is TextBox sheetresistanceunitTextBox)
            {
                sheetresistanceunitTextBox.Text = "Ω / Sqr";
            }

            if (Controls.Find("TextboxResistivity", true).FirstOrDefault() is TextBox resistivityTextBox)
            {
                resistivityTextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.Resistivity) ? GlobalSettingsParseValues.Instance.Resistivity.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxResistivityUnit", true).FirstOrDefault() is TextBox resistivityunitTextBox)
            {
                resistivityunitTextBox.Text = "Ω ⋅ m";
            }

            if (Controls.Find("TextboxConductivity", true).FirstOrDefault() is TextBox conductivityTextBox)
            { 
                conductivityTextBox.Text = !double.IsNaN(GlobalSettingsParseValues.Instance.Conductivity) ? GlobalSettingsParseValues.Instance.Conductivity.ToString("F5") : "N/A";
            }
            if (Controls.Find("TextboxConductivityUnit", true).FirstOrDefault() is TextBox conductivityunitTextBox)
            {
                conductivityunitTextBox.Text = "Ω / m";
            }
        }

        public void LoadTotalResistancesChart()
        {

        }
    }
}
