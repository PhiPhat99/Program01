using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Program01
{
    public partial class HallMeasurementResultsForm : Form
    {
        public class VoltagesData
        {
            public string MeasurementType { get; set; }
            public int Position { get; set; }
            public double Voltages { get; set; }
        }

        public HallMeasurementResultsForm()
        {
            InitializeComponent();
            RichTextBoxSettings();
            LoadMeasurementResults();
            LoadTotalHallVoltagesChart();
        }

        private void RichTextBoxSettings()
        {
            RichTextboxHallOutPos1.Text = "VOut1 :";
            RichTextboxHallOutPos2.Text = "VOut2 :";
            RichTextboxHallOutPos3.Text = "VOut3 :";
            RichTextboxHallOutPos4.Text = "VOut4 :";

            RichTextboxHallInSouthPos1.Text = "VSouth1 :";
            RichTextboxHallInSouthPos2.Text = "VSouth2 :";
            RichTextboxHallInSouthPos3.Text = "VSouth3 :";
            RichTextboxHallInSouthPos4.Text = "VSouth4 :";
            RichTextboxHallInNorthPos1.Text = "VNorth1 :";
            RichTextboxHallInNorthPos2.Text = "VNorth2 :";
            RichTextboxHallInNorthPos3.Text = "VNorth3 :";
            RichTextboxHallInNorthPos4.Text = "VNorth4 :";

            RichTextboxHallVoltage1.Text = "VH1 :";
            RichTextboxHallVoltage2.Text = "VH2 :";
            RichTextboxHallVoltage3.Text = "VH3 :";
            RichTextboxHallVoltage4.Text = "VH4 :";

            RichTextboxHallVoltage.Text = "VH :";
            RichTextboxHallCoefficient.Text = "RH :";

            RichTextboxHallOutPos1.Location = new Point(120, 50);
            RichTextboxHallOutPos2.Location = new Point(120, 85);
            RichTextboxHallOutPos3.Location = new Point(120, 120);
            RichTextboxHallOutPos4.Location = new Point(120, 155);

            RichTextboxHallInSouthPos1.Location = new Point(110, 225);
            RichTextboxHallInSouthPos2.Location = new Point(110, 260);
            RichTextboxHallInSouthPos3.Location = new Point(110, 295);
            RichTextboxHallInSouthPos4.Location = new Point(110, 330);
            RichTextboxHallInNorthPos1.Location = new Point(110, 365);
            RichTextboxHallInNorthPos2.Location = new Point(110, 400);
            RichTextboxHallInNorthPos3.Location = new Point(110, 435);
            RichTextboxHallInNorthPos4.Location = new Point(110, 470);

            RichTextboxHallVoltage1.Location = new Point(140, 540);
            RichTextboxHallVoltage2.Location = new Point(140, 575);
            RichTextboxHallVoltage3.Location = new Point(140, 610);
            RichTextboxHallVoltage4.Location = new Point(140, 645);

            RichTextboxHallVoltage.Location = new Point(140, 680);
            RichTextboxHallCoefficient.Location = new Point(140, 715);

            RichTextboxHallOutPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallOutPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallInSouthPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInSouthPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallInNorthPos4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallVoltage1.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallVoltage2.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallVoltage3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallVoltage4.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallVoltage.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallCoefficient.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

            RichTextboxHallOutPos1.Select(1, 4);
            RichTextboxHallOutPos2.Select(1, 4);
            RichTextboxHallOutPos3.Select(1, 4);
            RichTextboxHallOutPos4.Select(1, 4);

            RichTextboxHallInSouthPos1.Select(1, 6);
            RichTextboxHallInSouthPos2.Select(1, 6);
            RichTextboxHallInSouthPos3.Select(1, 6);
            RichTextboxHallInSouthPos4.Select(1, 6);
            RichTextboxHallInNorthPos1.Select(1, 6);
            RichTextboxHallInNorthPos2.Select(1, 6);
            RichTextboxHallInNorthPos3.Select(1, 6);
            RichTextboxHallInNorthPos4.Select(1, 6);


            RichTextboxHallVoltage1.Select(1, 2);
            RichTextboxHallVoltage2.Select(1, 2);
            RichTextboxHallVoltage3.Select(1, 2);
            RichTextboxHallVoltage4.Select(1, 2);

            RichTextboxHallVoltage.Select(1, 1);
            RichTextboxHallCoefficient.Select(1, 1);

            RichTextboxHallOutPos1.SelectionCharOffset = -8;
            RichTextboxHallOutPos2.SelectionCharOffset = -8;
            RichTextboxHallOutPos3.SelectionCharOffset = -8;
            RichTextboxHallOutPos4.SelectionCharOffset = -8;

            RichTextboxHallInSouthPos1.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos2.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos3.SelectionCharOffset = -8;
            RichTextboxHallInSouthPos4.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos1.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos2.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos3.SelectionCharOffset = -8;
            RichTextboxHallInNorthPos4.SelectionCharOffset = -8;

            RichTextboxHallVoltage1.SelectionCharOffset = -8;
            RichTextboxHallVoltage2.SelectionCharOffset = -8;
            RichTextboxHallVoltage3.SelectionCharOffset = -8;
            RichTextboxHallVoltage4.SelectionCharOffset = -8;

            RichTextboxHallVoltage.SelectionCharOffset = -8;
            RichTextboxHallCoefficient.SelectionCharOffset = -8;

            RichTextboxHallOutPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallOutPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallInSouthPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInSouthPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallInNorthPos4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallVoltage1.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallVoltage2.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallVoltage3.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallVoltage4.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallVoltage.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallCoefficient.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void LoadMeasurementResults()
        {
            TextboxSourceMode.Text = GlobalSettings.Instance.SourceModeUI;
            TextboxMeasureMode.Text = GlobalSettings.Instance.MeasureModeUI;

            if (Controls.Find("TextboxConcentration", true).FirstOrDefault() is TextBox concentrationTextBox)
            {
                concentrationTextBox.Text = $"{GlobalSettings.Instance.Concentration}";
            }

            if (Controls.Find("TextboxMobility", true).FirstOrDefault() is TextBox mobilityTextBox)
            {
                mobilityTextBox.Text = $"{GlobalSettings.Instance.Mobility}";
            }
        }

        private void LoadTotalHallVoltagesChart()
        {

        }
    }
}
