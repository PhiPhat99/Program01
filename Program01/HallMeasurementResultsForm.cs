using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Program01
{
    public partial class HallMeasurementResultsForm : Form
    {
        public HallMeasurementResultsForm()
        {
            InitializeComponent();
            RichTextBoxSettings();
            LoadMeasurementResults();

            CollectAndCalculateHallMeasured.Instance.HallVoltageCalculated += OnHallVoltageCalculatedHandler;
            CollectAndCalculateHallMeasured.Instance.HallPropertiesCalculated += OnHallPropertiesCalculatedHandler;
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

            RichTextboxHallVoltage.Text = "VH :";
            RichTextboxHallCoefficient.Text = "RH :";
            RichTextboxSheetConcentration.Text = "nSheet :";
            RichTextboxBulkConcentration.Text = "nBulk :";
            RichTextboxMobility.Text = "µ :";

            RichTextboxHallCoefficientUnit.Text = "m3 / C";
            RichTextboxSheetConcentrationUnit.Text = "m-2";
            RichTextboxBulkConcentrationUnit.Text = "m-3";
            RichTextboxMobilityUnit.Text = "m2 / V⋅s";

            RichTextboxHallCoefficientUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxSheetConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxBulkConcentrationUnit.SelectionAlignment = HorizontalAlignment.Center;
            RichTextboxMobilityUnit.SelectionAlignment = HorizontalAlignment.Center;

            RichTextboxHallOutPos1.Location = new Point(120, 40);
            RichTextboxHallOutPos2.Location = new Point(120, 75);
            RichTextboxHallOutPos3.Location = new Point(120, 110);
            RichTextboxHallOutPos4.Location = new Point(120, 145);

            RichTextboxHallInSouthPos1.Location = new Point(110, 215);
            RichTextboxHallInSouthPos2.Location = new Point(110, 250);
            RichTextboxHallInSouthPos3.Location = new Point(110, 285);
            RichTextboxHallInSouthPos4.Location = new Point(110, 320);
            RichTextboxHallInNorthPos1.Location = new Point(110, 355);
            RichTextboxHallInNorthPos2.Location = new Point(110, 390);
            RichTextboxHallInNorthPos3.Location = new Point(110, 425);
            RichTextboxHallInNorthPos4.Location = new Point(110, 460);

            RichTextboxHallVoltage.Location = new Point(140, 535);
            RichTextboxHallCoefficient.Location = new Point(140, 570);
            RichTextboxSheetConcentration.Location = new Point(120, 605);
            RichTextboxBulkConcentration.Location = new Point(125, 640);
            RichTextboxMobility.Location = new Point(148, 675);


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

            RichTextboxHallVoltage.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxHallCoefficient.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxSheetConcentration.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxBulkConcentration.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            RichTextboxMobility.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);

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

            RichTextboxHallVoltage.Select(1, 1);
            RichTextboxHallCoefficient.Select(1, 1);
            RichTextboxSheetConcentration.Select(1, 5);
            RichTextboxBulkConcentration.Select(1, 4);

            RichTextboxHallCoefficientUnit.Select(1, 1);
            RichTextboxSheetConcentrationUnit.Select(1, 2);
            RichTextboxBulkConcentrationUnit.Select(1, 2);
            RichTextboxMobilityUnit.Select(1, 1);

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

            RichTextboxHallVoltage.SelectionCharOffset = -8;
            RichTextboxHallCoefficient.SelectionCharOffset = -8;
            RichTextboxSheetConcentration.SelectionCharOffset = -8;
            RichTextboxBulkConcentration.SelectionCharOffset = -8;

            RichTextboxHallCoefficientUnit.SelectionCharOffset = +8;
            RichTextboxSheetConcentrationUnit.SelectionCharOffset = +8;
            RichTextboxBulkConcentrationUnit.SelectionCharOffset = +8;
            RichTextboxMobilityUnit.SelectionCharOffset = +8;

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

            RichTextboxHallVoltage.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxHallCoefficient.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetConcentration.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxBulkConcentration.SelectionFont = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);

            RichTextboxHallCoefficientUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxSheetConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxBulkConcentrationUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            RichTextboxMobilityUnit.SelectionFont = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);

        }

        private void LoadMeasurementResults()
        {
            TextboxSourceMode.Text = GlobalSettings.Instance.SourceModeUI;
            TextboxMeasureMode.Text = GlobalSettings.Instance.MeasureModeUI;

            var voltages = GlobalSettings.Instance.HallVoltagesByPosition;

            void SetText(TextBox box, int index)
            {
                if (voltages.TryGetValue(index, out double value))
                    box.Text = value.ToString("E3");
                else
                    box.Text = "N/A";
            }

            SetText(TextboxHallOut1, 1);
            SetText(TextboxHallOut2, 2);
            SetText(TextboxHallOut3, 3);
            SetText(TextboxHallOut4, 4);

            SetText(TextboxHallInSouth1, 5);
            SetText(TextboxHallInSouth2, 6);
            SetText(TextboxHallInSouth3, 7);
            SetText(TextboxHallInSouth4, 8);

            SetText(TextboxHallInNorth1, 9);
            SetText(TextboxHallInNorth2, 10);
            SetText(TextboxHallInNorth3, 11);
            SetText(TextboxHallInNorth4, 12);

            TextboxHallVoltage.Text = GlobalSettings.Instance.TotalHallVoltage.ToString("E3");
            TextboxHallCoefficient.Text = GlobalSettings.Instance.HallCoefficient.ToString("E3");
            TextboxSheetConcentration.Text = GlobalSettings.Instance.SheetConcentration.ToString("E3");
            TextboxBulkConcentration.Text = GlobalSettings.Instance.BulkConcentration.ToString("E3");
            TextboxMobility.Text = GlobalSettings.Instance.Mobility.ToString("E3");

            SetUnit("TextboxHallOut1Unit", "V");
            SetUnit("TextboxHallOut2Unit", "V");
            SetUnit("TextboxHallOut3Unit", "V");
            SetUnit("TextboxHallOut4Unit", "V");

            SetUnit("TextboxHallInSouth1Unit", "V");
            SetUnit("TextboxHallInSouth2Unit", "V");
            SetUnit("TextboxHallInSouth3Unit", "V");
            SetUnit("TextboxHallInSouth4Unit", "V");

            SetUnit("TextboxHallInNorth1Unit", "V");
            SetUnit("TextboxHallInNorth2Unit", "V");
            SetUnit("TextboxHallInNorth3Unit", "V");
            SetUnit("TextboxHallInNorth4Unit", "V");

            SetUnit("TextboxHallVoltage1Unit", "V");
            SetUnit("TextboxHallVoltage2Unit", "V");
            SetUnit("TextboxHallVoltage3Unit", "V");
            SetUnit("TextboxHallVoltage4Unit", "V");
            SetUnit("TextboxHallVoltageUnit", "V");
        }

        private void SetUnit(string controlName, string unit)
        {
            if (Controls.Find(controlName, true).FirstOrDefault() is TextBox unitBox)
            {
                unitBox.Text = unit;
            }
        }

        private void OnHallVoltageCalculatedHandler(object sender, Dictionary<int, double> hallVoltages)
        {
            LoadMeasurementResults();
        }
        private void OnHallPropertiesCalculatedHandler(object sender, ( double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility) properties)
        {
            TextboxHallCoefficient.Text = properties.HallCoefficient.ToString("E3");
            TextboxSheetConcentration.Text = properties.SheetConcentration.ToString("E3");
            TextboxBulkConcentration.Text = properties.BulkConcentration.ToString("E3");
            TextboxMobility.Text = properties.Mobility.ToString("E3");
        }

    }
}

