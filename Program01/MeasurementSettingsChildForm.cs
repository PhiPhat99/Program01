using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
using OfficeOpenXml.Drawing.Slicer.Style;

namespace Program01
{
    public partial class MeasurementSettingsChildForm : Form
    {
        //Fields
        private Ivi.Visa.Interop.FormattedIO488 SMU;
        private string SMU2450Address = $"GPIB3::18::INSTR";
        private Ivi.Visa.Interop.FormattedIO488 SMS;
        private string SMS7001Address = $"GPIB3::7::INSTR";
        private ResourceManager resourcemanagerSMU;
        private ResourceManager resourcemanagerSMS;
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
        private bool isSMSConnected = false;
        private bool isFinished;
        private Form CurrentTunerandDataChildForm;

        public MeasurementSettingsChildForm()
        {
            InitializeComponent();
            InitializeGPIB(); 
        }

        private void InitializeGPIB()
        {
            resourcemanagerSMU = new Ivi.Visa.Interop.ResourceManager();
            resourcemanagerSMS = new Ivi.Visa.Interop.ResourceManager();
            SMU = new Ivi.Visa.Interop.FormattedIO488();
            SMS = new Ivi.Visa.Interop.FormattedIO488();
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
            GlobalSettings.StartValue = TextboxStart.Text;
            GlobalSettings.StopValue = TextboxStop.Text;
            GlobalSettings.StepValue = TextboxStep.Text;
            GlobalSettings.SourceLimitValue = TextboxSourceLimitLevel.Text;
            GlobalSettings.ThicknessValue = TextboxThickness.Text;
            GlobalSettings.RepetitionValue = TextboxRepetition.Text;
            GlobalSettings.MagneticFieldsValue = TextboxMagneticFields.Text;
        }

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected)
                {
                    SMU.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSMU.Open(SMU2450Address);
                    SMU.IO.Timeout = 10000;
                    SMU.WriteString("*IDN?");
                    SMU.WriteString("SYSTem:BEEPer 888, 1");

                    isSMUConnected = true;
                    IconbuttonSMUConnection.BackColor = Color.Snow;
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

        private void IconbuttonSMSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMSConnected)
                {
                    SMS.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSMS.Open(SMS7001Address);
                    SMS.IO.Timeout = 5000;
                    SMS.WriteString("*CLS");
                    SMS.WriteString("*IDN?");

                    isSMSConnected = true;
                    IconbuttonSMSConnection.BackColor = Color.Snow;
                    IconbuttonSMSConnection.IconColor = Color.GreenYellow;

                    MessageBox.Show("Connected to SMS", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    try
                    {
                        if (SMS.IO != null)
                        {
                            SMS.WriteString("*CLS");
                            SMS.WriteString("*RST");
                            SMS.WriteString("*GTL");
                            SMS.IO.Close();
                            SMS.IO = null;
                        }

                        isSMSConnected = false;
                        IconbuttonSMSConnection.BackColor = Color.Snow;
                        IconbuttonSMSConnection.IconColor = Color.Gray;

                        MessageBox.Show("Disconnected from the SMS.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception disconnectEx)
                    {
                        MessageBox.Show($"Error during disconnection: {disconnectEx.Message}", "Disconnection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            TextboxStart.Text = GlobalSettings.StartValue;
            TextboxStop.Text = GlobalSettings.StopValue;
            TextboxStep.Text = GlobalSettings.StepValue;
            TextboxSourceLimitLevel.Text = GlobalSettings.SourceLimitValue;
            TextboxThickness.Text = GlobalSettings.ThicknessValue;
            TextboxRepetition.Text = GlobalSettings.RepetitionValue;
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
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            RsenseMode = "";
                            break;
                    }
                }

                else if (MeasureMode == "Current")
                {
                    switch (RsenseMode)
                    {
                        case "2-Wires":
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            RsenseMode = "";
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

                switch (MeasureMode)
                {
                    case "Voltage":
                        break;
                    case "Current":
                        break;
                    default:
                        ComboboxMeasure.SelectedIndex = -1;
                        MeasureMode = "";
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
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        UpdateMeasurementSettingsUnits();
                        break;
                    default:
                        ComboboxSource.SelectedIndex = -1;
                        SourceMode = "";
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
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        UpdateMeasurementSettingsUnits();
                        break;
                    default:
                        ComboboxSourceLimitMode.SelectedIndex = -1;
                        SourceLimit = "";
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

                    ComboboxSourceLimitMode.SelectedItem = "Current";
                    SourceLimit = "Current";
                    LabelSourceLimitLevelUnit.Text = "A";
                }

                else if (SourceMode == "Current")
                {
                    LabelStartUnit.Text = "A";
                    LabelStepUnit.Text = "A";
                    LabelStopUnit.Text = "A";

                    ComboboxSourceLimitMode.SelectedItem = "Voltage";
                    SourceLimit = "Voltage";
                    LabelSourceLimitLevelUnit.Text = "V";
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
            ComboboxRsense.SelectedIndex = -1;
            ComboboxMeasure.SelectedIndex = -1;
            ComboboxSource.SelectedIndex = -1;
            ComboboxSourceLimitMode.SelectedIndex = -1;
            TextboxMagneticFields.Text = "";
            savedRsenseMode = "";
            savedMeasureMode = "";
            savedSourceMode = "";
            savedSourceLimitMode = "";
            savedMagneticFieldsValue = "";
            LabelStartUnit.Text = "";
            LabelStepUnit.Text = "";
            LabelStopUnit.Text = "";
            LabelSourceLimitLevelUnit.Text = "";
        }

        private void IconbuttonMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isMeasured)
                {
                    isMeasured = true;
                    TextboxMagneticFields.Enabled = false;
                    TextboxMagneticFields.Visible = false;
                    LabelMagneticFields.Visible = false;
                    LabelMagneticFieldsUnit.Visible = false;
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
                    TextboxMagneticFields.Visible = true;
                    LabelMagneticFields.Visible = true;
                    LabelMagneticFieldsUnit.Visible = true;
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
                if (!isSMUConnected)
                {
                    Debug.WriteLine("SMU is not connected. Exiting function.");
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!double.TryParse(TextboxStart.Text, out double startValue) || !double.TryParse(TextboxStop.Text, out double stopValue) || !double.TryParse(TextboxStep.Text, out double stepValue) || !int.TryParse(TextboxRepetition.Text, out int repetitionValue) || !double.TryParse(TextboxSourceLimitLevel.Text, out double sourceLimitValue))
                {
                    Debug.WriteLine($"Invalid inputs: StartValue = {TextboxStart.Text}, StopValue = {TextboxStop.Text}, StepValue = {TextboxStep.Text}, Repetition = {TextboxRepetition.Text}, SourceLimit = {TextboxSourceLimitLevel.Text}");
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (savedSourceMode == "Voltage" && savedMeasureMode == "Voltage" && isFinished)
                {
                    isFinished = false;
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIM {sourceLimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");

                    if (savedRsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INIT");
                    SMU.WriteString("*WAI");
                    SMU.WriteString("OUTPut OFF");                    
                }

                else if (savedSourceMode == "Voltage" && savedMeasureMode == "Current" && isFinished)
                {
                    isFinished = false;
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIM {sourceLimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");

                    if (savedRsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INIT");
                    SMU.WriteString("*WAI");
                    SMU.WriteString("OUTPut OFF");
                }

                else if (savedSourceMode == "Current" && savedMeasureMode == "Voltage" && !isFinished)
                {
                    isFinished = false;
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIM {sourceLimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");

                    if (savedRsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INIT");
                    SMU.WriteString("*WAI");
                    SMU.WriteString("OUTPut OFF");
                }

                else if (savedSourceMode == "Current" && savedMeasureMode == "Current" && isFinished)
                {
                    isFinished = false;
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIM {sourceLimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");

                    if (savedRsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INIT");
                    SMU.WriteString("*WAI");
                    SMU.WriteString("OUTPut OFF");
                }

                //isFinished = true;
                /*if (isFinished == false)
                {
                    MessageBox.Show("The measurement is started, do not disturbed", "MEASURE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The measurement is finished", "FINISH", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ButtonData_Click(object sender, EventArgs e)
        {
            try
            {
                OpenChildForm(new MeasurementSettingsDataChildForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                CurrentTunerandDataChildForm?.Close();
                CurrentTunerandDataChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                PanelTunerandData.Controls.Add(childForm);
                PanelTunerandData.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void ButtonTuner_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentTunerandDataChildForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }
    }
}
