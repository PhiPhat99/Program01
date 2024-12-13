using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            public static string RsenseMode { get; set; } = "";
            public static string MeasureMode { get; set; } = "";
            public static string SourceMode { get; set; } = "";
            public static string SourceLimitType { get; set; } = "";
            public static string MagneticFieldsValue { get; set; } = string.Empty;
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
                    SMU.WriteString("SYSTem:BEEPer 888, 0.5");

                    isSMUConnected = true;
                    IconbuttonSMUConnection.BackColor = Color.AliceBlue;
                    IconbuttonSMUConnection.IconColor = Color.GreenYellow;

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
                    IconbuttonSMUConnection.BackColor = Color.Gray;
                    IconbuttonSMUConnection.IconColor = Color.Black;

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
                switch (RsenseMode)
                {
                    case "2-Wires":
                        SMU.WriteString("SENSe:RSENse OFF");
                        break;
                    case "4-Wires":
                        SMU.WriteString("SENSe:RSENse ON");
                        break;
                    case "":
                        MessageBox.Show("Please select the sense wires mode", "ERROR");
                        break;
                    default:
                        ComboboxRsense.SelectedItem = "2-Wires";
                        RsenseMode = "2-Wires";
                        SMU.WriteString("SENSe:RSENse OFF");
                        break;
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
                switch (MeasureMode)
                {
                    case "Voltage":
                        SMU.WriteString("SENSe:FUNCtion 'VOLTage'");
                        SMU.WriteString("SENSe:VOLTage:RANGe:AUTO ON");
                        break;
                    case "Current":
                        SMU.WriteString("SENSe:FUNCtion 'CURRent'");
                        SMU.WriteString("SENSe:CURRent:RANGe:AUTO ON");
                        break;
                    case "":
                        MessageBox.Show("Please select the measurement mode", "ERROR");
                        break;
                    default:
                        ComboboxMeasure.SelectedItem = "Voltage";
                        MeasureMode = "Voltage";
                        SMU.WriteString("SENSe:FUNCtion 'VOLTage'");
                        SMU.WriteString("SENSe:VOLTage:RANGe:AUTO ON");
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
                        SMU.WriteString("SOURce:VOLTage");
                        SMU.WriteString("SOURce:VOLTage:RANGe:AUTO 1");
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        SMU.WriteString("SOURce:CURRent");
                        SMU.WriteString("SOURce:CURRent:RANGe:AUTO 1");
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "":
                        MessageBox.Show("Please select the source mode", "ERROR");
                        break;
                    default:
                        ComboboxSource.SelectedItem = "Current";
                        SourceMode = "Current";
                        SMU.WriteString("SOURce:CURRent");
                        SMU.WriteString("SOURce:CURRent:RANG:AUTO ON");
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
                        //SMU.WriteString($"SOURce:CURRent:VLIMit {SourceLimitValue}");
                        LabelSourceLimitLevelUnit.Text = "V";
                        break;
                    case "Current":
                        //SMU.WriteString($"SOURce:VOLTage:ILIMit {SourceLimitValue}");
                        LabelSourceLimitLevelUnit.Text = "A";
                        break;
                    case "":
                        MessageBox.Show("Please select the source limit mode", "ERROR");
                        break;
                    default:
                        ComboboxSourceLimitMode.SelectedItem = "Voltage";
                        SourceLimit = "Voltage";
                        //SMU.WriteString($"SOURce:CURRent:VLIMit {SourceLimitValue}");
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
            IconbuttonApplyandEditSettings.IconChar = IconChar.Circle;
            IconbuttonApplyandEditSettings.BackColor = Color.White;
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

        /*private void IconbuttonApplyandEditSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isApplySettings)
                {
                    isApplySettings = true;
                    SaveSettings();
                    ApplySettingsToSMU();
                    IconbuttonApplyandEditSettings.IconChar = IconChar.CircleCheck;
                    IconbuttonApplyandEditSettings.BackColor = Color.YellowGreen;
                    MessageBox.Show("Applying measurement settings is successfully", "APPLY SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    isApplySettings = false;
                    IconbuttonApplyandEditSettings.IconChar = IconChar.Circle;
                    IconbuttonApplyandEditSettings.BackColor = Color.White;
                    MessageBox.Show("You can edit measurement settings now", "EDIT SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }*/

        private void SaveSettings()
        {
            GlobalSettings.RsenseMode = ComboboxRsense.SelectedItem?.ToString() ?? "";
            GlobalSettings.MeasureMode = ComboboxMeasure.SelectedItem?.ToString() ?? "";
            GlobalSettings.SourceMode = ComboboxSource.SelectedItem?.ToString() ?? "";
            GlobalSettings.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";
            GlobalSettings.MagneticFieldsValue = TextboxMagneticFields.Text;
        }

        private void IconbuttonSweep_Click(object sender, EventArgs e)
        {
            SMU.WriteString("*RST");
            SMU.WriteString("SOUR:FUNC CURR");
            SMU.WriteString("SOUR:CURR:RANG 1");
            SMU.WriteString("SENS:FUNC 'VOLT'");
            SMU.WriteString("SOUR:SWE:CURR:LIN:STEP 0, 10.5e-3, 5e-4, 10e-3, 3, BEST");
            SMU.WriteString("SENS:VOLT:RANG 20");
            SMU.WriteString("INIT");
            SMU.WriteString("*WAI");
            SMU.WriteString("TRAC:DATA? 1, 22, 'defbuffer1', SOUR, READ");
            SMU.WriteString("OUTP OFF");
        }

        /*private void SendCommandToSMU(string command)
        {
            try
            {
                if (SMU != null && !isSMUConnected)
                {
                    SMU.WriteString(command); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending command to SMU: {ex.Message}", "Command Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplySettingsToSMU()
        {
            try
            {
                if (savedSourceLimitMode == "Voltage" && isApplySettings == true && string.IsNullOrEmpty(TextboxSourceLimitLevel.Text))
                {
                    SendCommandToSMU($"SOURce:CURRent:VLIMit {SourceLimitValue}");
                }
                else if (savedSourceLimitMode == "Current" && isApplySettings == true && string.IsNullOrEmpty(TextboxSourceLimitLevel.Text))
                {
                    SendCommandToSMU($"SOURce:VOLTage:ILIMit {SourceLimitValue}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply settings to SMU: {ex.Message}", "Apply Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/
    }
}
