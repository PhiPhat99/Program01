using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Ivi.Visa.Interop;
using OfficeOpenXml.Drawing.Slicer.Style;

namespace Program01
{
    public partial class MeasurementSettingsChildForm : Form
    {
        //Fields
        private Ivi.Visa.Interop.FormattedIO488 SMU;
        //private string SMU2450Address = $"GPIB3::18::INSTR";
        private Ivi.Visa.Interop.FormattedIO488 SS;
        //private string SS7001Address = $"GPIB3::7::INSTR";
        private ResourceManager resourcemanagerSMU;
        private ResourceManager resourcemanagerSS;
        private string RsenseMode;
        private string MeasureMode;
        private string SourceMode;
        private string SourceLimit;
        private string savedRsenseMode;
        private string savedMeasureMode;
        private string savedSourceMode;
        private string savedSourceLimitMode;
        private string savedStartValue;
        private string savedStopValue;
        private string savedStepValue;
        private string savedSourceLimitValue;
        private string savedThicknessValue;
        private string savedRepetitionValue;
        private string savedMagneticFieldsValue;
        private bool isSMUConnected = false;
        private bool isMeasured = false;
        private bool isSSConnected = false;
        private Form CurrentTunerandDataChildForm;

        public MeasurementSettingsChildForm()
        {
            InitializeComponent();
            InitializeGPIB();
        }

        private void InitializeGPIB()
        {
            try
            {
                resourcemanagerSMU = new Ivi.Visa.Interop.ResourceManager();
                resourcemanagerSS = new Ivi.Visa.Interop.ResourceManager();

                SMU = new Ivi.Visa.Interop.FormattedIO488();
                SS = new Ivi.Visa.Interop.FormattedIO488();

                string[] SMUgpibAddress = resourcemanagerSMU.FindRsrc("GPIB?*::?*::INSTR");
                string[] SSgpibAddress = resourcemanagerSS.FindRsrc("GPIB?*::?*::INSTR");

                ComboboxVISASMUIOPort.Items.Clear();
                ComboboxVISASSIOPort.Items.Clear();

                foreach (string SMUaddress in SMUgpibAddress)
                {
                    ComboboxVISASMUIOPort.Items.Add(SMUaddress);
                }

                if (ComboboxVISASMUIOPort.Items.Count > 0)
                {
                    ComboboxVISASMUIOPort.SelectedIndex = 0;
                }

                if (ComboboxVISASMUIOPort.Items.Count > 0)
                {
                    ComboboxVISASMUIOPort.SelectedIndex = 0;
                }

                foreach (string SSaddress in SSgpibAddress)
                {
                    ComboboxVISASSIOPort.Items.Add(SSaddress);
                }

                if (ComboboxVISASSIOPort.Items.Count > 0)
                {
                    ComboboxVISASSIOPort.SelectedIndex = 0;
                }

                if (ComboboxVISASSIOPort.Items.Count > 0)
                {
                    ComboboxVISASSIOPort.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing GPIB: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                if (ComboboxVISASMUIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for SMU.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSMUAddress = ComboboxVISASMUIOPort.SelectedItem.ToString();

                if (!isSMUConnected)
                {
                    SMU.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSMU.Open(selectedSMUAddress);
                    SMU.IO.Timeout = 5000;
                    SMU.WriteString("*IDN?");
                    SMU.WriteString("SYSTem:BEEPer 888, 1");
                    
                    isSMUConnected = true;

                    IconbuttonSMUConnection.BackColor = Color.Snow;
                    IconbuttonSMUConnection.IconColor = Color.GreenYellow;
                    MessageBox.Show("Connected to SMU", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    try
                    {
                        if (SMU.IO != null)
                        {
                            SMU.WriteString("*CLS");
                            SMU.WriteString("*RST");

                            SMU.IO.Close();
                            SMU.IO = null;
                        }

                        isSMUConnected = false;

                        IconbuttonSMUConnection.BackColor = Color.Snow;
                        IconbuttonSMUConnection.IconColor = Color.Gray;

                        MessageBox.Show("Disconnected from the SMU.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void IconbuttonSSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboboxVISASSIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for SS.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSSAddress = ComboboxVISASSIOPort.SelectedItem.ToString();

                if (!isSSConnected)
                {
                    SS.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSS.Open(selectedSSAddress);
                    SS.IO.Timeout = 5000;

                    SS.WriteString("*CLS");  
                    SS.WriteString("*IDN?");
                    
                    isSSConnected = true;

                    IconbuttonSSConnection.BackColor = Color.Snow;
                    IconbuttonSSConnection.IconColor = Color.GreenYellow;
                    MessageBox.Show("Connected to SS", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    try
                    {
                        if (SS.IO != null)
                        {
                            SS.WriteString("*CLS");
                            SS.WriteString("*RST");

                            SS.IO.Close();
                            SS.IO = null;
                        }

                        isSSConnected = false;

                        IconbuttonSSConnection.BackColor = Color.Snow;
                        IconbuttonSSConnection.IconColor = Color.Gray;

                        MessageBox.Show("Disconnected from the SS.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void IconbuttonRefreshGPIBAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (SMU.IO == null && SS.IO == null)
                {
                    InitializeGPIB();
                    MessageBox.Show("GPIB addresses refreshed.", "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Can not be refresh the GPIB addressed. Please power off the instruments before refresh", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonClearSettings_Click(object sender, EventArgs e)
        {
            ClearSettings();
        }

        private void ClearSettings()
        {
            ComboboxRsense.SelectedIndex = -1;
            ComboboxMeasure.SelectedIndex = -1;
            ComboboxSource.SelectedIndex = -1;
            ComboboxSourceLimitMode.SelectedIndex = -1;
            TextboxStart.Text = "";
            TextboxStep.Text = "";
            TextboxStop.Text = "";
            TextboxSourceLimitLevel.Text = "";
            TextboxThickness.Text = "";
            TextboxRepetition.Text = "";
            TextboxMagneticFields.Text = "";
            savedRsenseMode = "";
            savedMeasureMode = "";
            savedSourceMode = "";
            savedSourceLimitMode = "";
            savedStartValue = "";
            savedStopValue = "";
            savedStepValue = "";
            savedSourceLimitValue = "";
            savedThicknessValue = "";
            savedRepetitionValue = "";
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
                MessageBox.Show($"Error: {ex.Message}");
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

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourceLimitValue))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (savedSourceMode == "Voltage" && savedMeasureMode == "Voltage")
                {
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

                else if (savedSourceMode == "Voltage" && savedMeasureMode == "Current")
                {
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

                else if (savedSourceMode == "Current" && savedMeasureMode == "Voltage")
                {
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

                else if (savedSourceMode == "Current" && savedMeasureMode == "Current")
                {
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
                MessageBox.Show($"Error: {ex.Message}");
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
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetitions, out double sourceLimit)
        {
            start = stop = step = sourceLimit = 0;
            repetitions = 0;

            if (!double.TryParse(TextboxStart.Text, out start) ||
                !double.TryParse(TextboxStop.Text, out stop) ||
                !double.TryParse(TextboxStep.Text, out step) ||
                !int.TryParse(TextboxRepetition.Text, out repetitions) ||
                !double.TryParse(TextboxSourceLimitLevel.Text, out sourceLimit) ||
                start >= stop || step <= 0 || repetitions < 1)
            {
                return false;
            }

            return true;
        }
    }
}
