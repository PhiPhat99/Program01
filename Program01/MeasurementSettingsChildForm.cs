using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Converters;
using FontAwesome.Sharp;
using Ivi.Visa.Interop;

namespace Program01
{
    public partial class MeasurementSettingsChildForm : Form
    {
        //Fields
        private Ivi.Visa.Interop.FormattedIO488 SMU;
        private string SMU2450Address = $"GPIB3::18::INSTR";
        private ResourceManager resourcemanager;
        private string SourceLimitValue;
        private string StartValue;
        private string StepValue;
        private string StopValue;
        private string ThicknessValue;
        private string RepetitionValue;
        private string MagneticFieldsValue;
        private string RsenseMode;
        private string MeasureMode;
        private string SourceMode;
        private string SourceLimit;
        private string savedRsenseMode;
        private string savedMeasureMode;
        private string savedSourceMode;
        private string savedSourceLimitMode;
        private string savedMagneticFieldsValue;
        private bool isSMUConnected = false;
        private bool isMeasured = false;
        private bool isApplySettings = false;
        private string RsenseModeSCPICommand = "";
        private string MeasureModeSCPICommand = "";
        private string MeasureRangeSCPICommand = "";

        private BindingSource bindingSource;

        public MeasurementSettingsChildForm()
        {
            InitializeComponent();
            InitializeGPIB(); 
        }

        private void InitializeGPIB()
        {
            resourcemanager = new Ivi.Visa.Interop.ResourceManager();
            SMU = new Ivi.Visa.Interop.FormattedIO488();
        }

        private readonly struct RGBColors
        {
            public static readonly Color Color1 = Color.FromArgb(172, 126, 241);
            public static readonly Color Color2 = Color.FromArgb(219, 118, 176);
            public static readonly Color Color3 = Color.FromArgb(253, 138, 114);
            public static readonly Color Color4 = Color.FromArgb(95, 77, 221);
            public static readonly Color Color5 = Color.FromArgb(249, 88, 155);
            public static readonly Color Color6 = Color.FromArgb(24, 161, 251);
        }

        public static class GlobalSettings
        {
            public static string RsenseMode { get; set; }
            public static string MeasureMode { get; set; }
            public static string SourceMode { get; set; }
            public static string SourceLimitType { get; set; }
            public static string StartValue { get; set; }
            public static string StopValue { get; set; }
            public static string StepValue { get; set; }
            public static string ThicknessValue { get; set; }
            public static string RepetitionValue { get; set; }
            public static string SourceLimitValue { get; set; }
            public static string MagneticFieldsValue { get; set; }
        }

        private void SaveSettings()
        {
            GlobalSettings.RsenseMode = ComboboxRsense.SelectedItem?.ToString() ?? "";
            GlobalSettings.MeasureMode = ComboboxMeasure.SelectedItem?.ToString() ?? "";
            GlobalSettings.SourceMode = ComboboxSource.SelectedItem?.ToString() ?? "";
            GlobalSettings.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";
            GlobalSettings.MagneticFieldsValue = TextboxMagneticFields.Text;
        }

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected)
                {
                    SMU.IO = (Ivi.Visa.Interop.IMessage)resourcemanager.Open(SMU2450Address);
                    SMU.IO.Timeout = 10000;
                    SMU.WriteString("*IDN?");
                    SMU.WriteString("SYSTem:BEEPer 888, 1");
                    string response = SMU.ReadString();

                    isSMUConnected = true;
                    IconbuttonSMUConnection.BackColor = Color.Snow;
                    IconbuttonSMUConnection.IconColor = Color.GreenYellow;

                    MessageBox.Show($"Instrument Response: {response}");
                    MessageBox.Show("Connected to SMU", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (SMU.IO != null)
                    {
                        SMU.WriteString("*RST"); 
                        SMU.IO.Close();          
                        SMU.IO = null; 
                    }

                    isSMUConnected = false;
                    IconbuttonSMUConnection.BackColor = Color.Snow;
                    IconbuttonSMUConnection.IconColor = Color.Gray;

                    MessageBox.Show("Disconnected from the SMU.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void MeasurementSettingsChildForm_Load(object sender, EventArgs e)
        {
            ComboboxRsense.Items.Add("2-Wires");
            ComboboxRsense.Items.Add("4-Wires");

            ComboboxMeasure.Items.Add("Voltage");
            ComboboxMeasure.Items.Add("Current");

            ComboboxSource.Items.Add("Voltage");
            ComboboxSource.Items.Add("Current");

            ComboboxSourceLimitMode.Items.Add("Voltage");
            ComboboxSourceLimitMode.Items.Add("Current");

            ComboboxRsense.SelectedItem = GlobalSettings.RsenseMode;
            ComboboxMeasure.SelectedItem = GlobalSettings.MeasureMode;
            ComboboxSource.SelectedItem = GlobalSettings.SourceMode;
            ComboboxSourceLimitMode.SelectedItem = GlobalSettings.SourceLimitType;
            TextboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;
        }

        private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void ComboboxRsense_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RsenseMode = ComboboxRsense.SelectedItem?.ToString();
                savedRsenseMode = RsenseMode;
                if (MeasureMode == "Voltage")
                {
                    switch (RsenseMode)
                    {
                        case "2-Wires":
                            //SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                            break;
                        case "4-Wires":
                            //SMU.WriteString("SENSe:VOLTage:RSENse ON");
                            break;
                        default:
                            ComboboxRsense.SelectedItem = "2-Wires";
                            RsenseMode = "2-Wires";
                            //SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                            break;
                    }
                }
               else if (MeasureMode == "Current")
                {
                    switch (RsenseMode)
                    {
                        case "2-Wires":
                            //SMU.WriteString("SENSe:CURRent:RSENse OFF");
                            break;
                        case "4-Wires":
                            //SMU.WriteString("SENSe:CURRent:RSENse ON");
                            break;
                        default:
                            ComboboxRsense.SelectedItem = "2-Wires";
                            RsenseMode = "2-Wires";
                            //SMU.WriteString("SENSe:CURRent:RSENse OFF");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void ComboboxMeasure_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MeasureMode = ComboboxMeasure.SelectedItem?.ToString();
                savedMeasureMode = MeasureMode;
                if (ComboboxMeasure.SelectedItem == null)
                {
                    MessageBox.Show("Please select a valid measure mode.");
                    return;
                }
                switch (MeasureMode)
                {
                    case "Voltage":
                        //SMU.WriteString("SENSe:FUNCtion 'VOLTage'");
                        //SMU.WriteString("SENSe:VOLTage:RANGe:AUTO ON");
                        break;
                    case "Current":
                        //SMU.WriteString("SENSe:FUNCtion 'CURRent'");
                        //SMU.WriteString("SENSe:CURRent:RANGe:AUTO ON");
                        break;
                    default:
                        ComboboxMeasure.SelectedItem = "Voltage";
                        MeasureMode = "Voltage";
                        //SMU.WriteString("SENSe:FUNCtion 'VOLTage'");
                        //SMU.WriteString("SENSe:VOLTage:RANGe:AUTO ON");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void ComboboxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceMode = ComboboxSource.SelectedItem?.ToString();
                savedSourceMode = SourceMode;
                switch (SourceMode)
                {
                    case "Voltage":
                        //SMU.WriteString("SOURce:FUNCtion VOLTage");
                        //SMU.WriteString("SOURce:VOLTage:RANGe:AUTO ON");
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        //SMU.WriteString("SOURce:FUNCtion CURRent");
                        //SMU.WriteString("SOURce:CURRent:RANGe:AUTO ON");
                        UpdateMeasurementSettingsUnits();
                        break;
                    default:
                        ComboboxSource.SelectedItem = "Current";
                        SourceMode = "Current";
                        //SMU.WriteString("SOURce:FUNCtion CURRent");
                        //SMU.WriteString("SOURce:CURRent:RANGe:AUTO ON");
                        UpdateMeasurementSettingsUnits();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void ComboboxSourceLimitMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceLimit = ComboboxSourceLimitMode.SelectedItem?.ToString();
                savedSourceLimitMode = SourceLimit;
                switch (SourceLimit)
                {
                    case "Voltage":
                        LabelSourceLimitLevelUnit.Text = "V";
                        break;
                    case "Current":
                        LabelSourceLimitLevelUnit.Text = "A";
                        break;
                    case "":
                        MessageBox.Show("Please select the source limit mode", "ERROR");
                        break;
                    default:
                        ComboboxSourceLimitMode.SelectedItem = "Voltage";
                        SourceLimit = "Voltage";
                        LabelSourceLimitLevelUnit.Text = "V";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void UpdateMeasurementSettingsUnits()
        {
            try
            {
                if (SourceMode == "Voltage")
                {
                    LabelStartUnit.Text = "V";
                    LabelStepUnit.Text = "V";
                    LabelStopUnit.Text = "V";
                }
                else if (SourceMode == "Current")
                {
                    LabelStartUnit.Text = "mA";
                    LabelStepUnit.Text = "mA";
                    LabelStopUnit.Text = "mA";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void IconbuttonClearSettings_Click(object sender, EventArgs e)
        {
            SMU.WriteString("*RST");
            ClearSettings();
        }

        private void ClearSettings()
        {
            ComboboxRsense.SelectedIndex = 0;
            ComboboxMeasure.SelectedIndex = 1;
            ComboboxSource.SelectedIndex = 0;
            ComboboxSourceLimitMode.SelectedIndex = 1;
            TextboxMagneticFields.Text = "";
            savedRsenseMode = "";
            savedMeasureMode = "";
            savedSourceMode = "";
            savedSourceLimitMode = "";
            savedMagneticFieldsValue = "";
            isApplySettings = false;
        }

        private void IconbuttonMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isMeasured)
                {
                    isMeasured = true;
                    TextboxMagneticFields.Enabled = false;
                    IconbuttonMeasurement.Text = "Van der Pauw";
                    IconbuttonMeasurement.IconChar = IconChar.Diamond;
                    IconbuttonMeasurement.TextImageRelation = TextImageRelation.ImageBeforeText;
                    IconbuttonMeasurement.TextAlign = ContentAlignment.MiddleCenter;
                    IconbuttonMeasurement.ImageAlign = ContentAlignment.MiddleCenter;
                    IconbuttonMeasurement.IconColor = RGBColors.Color3;
                    IconbuttonMeasurement.BackColor = Color.LightGreen;
                }
                else
                {
                    isMeasured = false;
                    TextboxMagneticFields.Enabled = true;
                    IconbuttonMeasurement.Text = "Hall Measurement";
                    IconbuttonMeasurement.IconChar = IconChar.Magnet;
                    IconbuttonMeasurement.TextImageRelation = TextImageRelation.ImageBeforeText;
                    IconbuttonMeasurement.TextAlign = ContentAlignment.MiddleCenter;
                    IconbuttonMeasurement.ImageAlign = ContentAlignment.MiddleCenter;
                    IconbuttonMeasurement.IconColor = RGBColors.Color4;
                    IconbuttonMeasurement.BackColor = Color.LightGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void IconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();
                if (!isSMUConnected) return;

                if (!ValidateInputs(out double start, out double stop, out double step, out double sourceLimit, out int repetition, out double thickness)) return;

                if (start > stop)
                {
                    MessageBox.Show("Start Value must be less than Stop Value.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateSourceLimit(sourceLimit)) return;

                ConfigureSMU(savedSourceMode, savedMeasureMode, savedRsenseMode, savedSourceLimitMode, start, stop, step, sourceLimit, repetition);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs(out double start, out double stop, out double step, out double sourceLimit, out int repetition, out double thickness)
        {
            start = stop = step = sourceLimit = thickness = 0;
            repetition = 0;

            if (!string.IsNullOrEmpty(StartValue) || !string.IsNullOrEmpty(StopValue) ||
                !string.IsNullOrEmpty(StepValue) || !string.IsNullOrEmpty(SourceLimitValue) ||
                !string.IsNullOrEmpty(RepetitionValue) || !string.IsNullOrEmpty(ThicknessValue))
            {
                MessageBox.Show("Please fill in all values completely", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (double.TryParse(StartValue, out start) || double.TryParse(StopValue, out stop) ||
                double.TryParse(StepValue, out step) || double.TryParse(SourceLimitValue, out sourceLimit) ||
                int.TryParse(RepetitionValue, out repetition) || double.TryParse(ThicknessValue, out thickness))
            {
                MessageBox.Show("Invalid input values. Check formats.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ValidateSourceLimit(double sourceLimit)
        {
            if ((savedSourceMode == "Voltage" && savedSourceLimitMode == "Current" && sourceLimit > 1.05) ||
                (savedSourceMode == "Current" && savedSourceLimitMode == "Voltage" && sourceLimit > 21))
            {
                MessageBox.Show($"Source Limit exceeds allowable limits.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ConfigureSMU(string sourceMode, string measureMode, string rsenseMode, string sourceLimitMode,
                                  double start, double stop, double step, double sourceLimit, int repetition)
        {
            SMU.WriteString("*RST");
            SMU.WriteString($"SOURce:FUNCtion {sourceMode}");
            SMU.WriteString($"SOURce:{sourceMode}:RANG:AUTO ON");
            SMU.WriteString($"SENSe:FUNCtion '{measureMode}'");
            SMU.WriteString($"SENSe:{measureMode}:RANGe:AUTO ON");

            if (rsenseMode == "4-Wires")
                SMU.WriteString("SENSe:CURRent:RSENse ON");
            else
                SMU.WriteString("SENSe:CURRent:RSENse OFF");

            SMU.WriteString($"SOURce:SWEep:VOLTage:LINear:STEP {start}, {stop}, {step}, {sourceLimit}, {repetition}, BEST");
            SMU.WriteString("OUTPut ON");
            SMU.WriteString("INIT");
            SMU.WriteString("*WAI");
            SMU.WriteString("TRACe:DATA? 1, 11, 'defbuffer1', SOURce, READ");
            SMU.WriteString("OUTPut OFF");
        }

        /*private void IconbuttonSweep_Click(object sender, EventArgs e)
        {
            SMU.WriteString("*RST");
            SMU.WriteString("SOUR:FUNC CURR");
            SMU.WriteString("SOUR:CURR:RANG:AUTO ON");
            SMU.WriteString("SENS:FUNC 'VOLT'");
            SMU.WriteString("SENS:VOLT:RSEN ON");
            SMU.WriteString("SOUR:SWE:CURR:LIN:STEP 0, 10.5e-3, 5e-4, 10e-3, 3, BEST");
            SMU.WriteString("SENS:VOLT:RANG:AUTO ON");
            SMU.WriteString("INIT");
            SMU.WriteString("*WAI");
            SMU.WriteString("TRAC:DATA? 1, 22, 'defbuffer1', SOUR, READ");
            SMU.WriteString("OUTP OFF");
        }*/
    }
}
