using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Ivi.Visa.Interop;

namespace Program01
{
    public partial class MeasurementSettingsChildForm : Form
    {
        //Fields
        private Ivi.Visa.Interop.FormattedIO488 SMU;
        private Ivi.Visa.Interop.FormattedIO488 SS;
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
        private string MagneticFieldsValue;
        private int targetPosition;
        private bool isSMUConnected = false;
        private bool isSSConnected = false;
        private bool isModes = false;
        private Form CurrentTunerandDataChildForm;
        public event EventHandler ToggleChanged;
        public bool IsOn
        {
            get => isModes;
            set
            {
                isModes = value;
                UpdateToggleState();
            }
        }

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
            public static string SourceLimitLevelValue { get; set; }
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
            GlobalSettings.SourceLimitLevelValue = TextboxSourceLimitLevel.Text;
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
                    string response = SMU.ReadString();
                    Debug.WriteLine($"{response}");

                    isSMUConnected = true;
                    PlaySMUConnectionMelody();

                    System.Threading.Thread.Sleep(4000);
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
                        IconbuttonSMUConnection.IconColor = Color.Gainsboro;

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
                    string response = SS.ReadString();
                    Debug.WriteLine($"{response}");

                    isSSConnected = true;
                    //PlaySSConnectionMelody();

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
                        IconbuttonSSConnection.IconColor = Color.Gainsboro;

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

        private void MeasurementSettingsChildForm_Load(object sender, EventArgs e)
        {
            ComboboxRsense.Items.Add("2-Wires");
            ComboboxRsense.Items.Add("4-Wires");

            ComboboxMeasure.Items.Add("Voltage");
            ComboboxMeasure.Items.Add("Current");

            ComboboxSource.Items.Add("Voltage");
            ComboboxSource.Items.Add("Current");

            ComboboxRsense.SelectedItem = GlobalSettings.RsenseMode;
            ComboboxMeasure.SelectedItem = GlobalSettings.MeasureMode;
            ComboboxSource.SelectedItem = GlobalSettings.SourceMode;
            ComboboxSourceLimitMode.SelectedItem = GlobalSettings.SourceLimitType;
            TextboxStart.Text = GlobalSettings.StartValue;
            TextboxStop.Text = GlobalSettings.StopValue;
            TextboxStep.Text = GlobalSettings.StepValue;
            TextboxSourceLimitLevel.Text = GlobalSettings.SourceLimitLevelValue;
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
                        UpdateMeasurementSettingsUnits("Voltage");
                        break;
                    case "Current":
                        UpdateMeasurementSettingsUnits("Current");
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UpdateMeasurementSettingsUnits(string SourceMode)
        {
            try
            {
                if (SourceMode == "Voltage")
                {
                    ComboboxStartUnit.Items.Clear();
                    ComboboxStartUnit.Items.AddRange(new string[] { "mV", "V"});
                    ComboboxStartUnit.SelectedIndex = 0;

                    ComboboxStopUnit.Items.Clear();
                    ComboboxStopUnit.Items.AddRange(new string[] { "mV", "V"});
                    ComboboxStopUnit.SelectedIndex = 0;

                    ComboboxStepUnit.Items.Clear();
                    ComboboxStepUnit.Items.AddRange(new string[] { "mV", "V"});
                    ComboboxStepUnit.SelectedIndex = 0;

                    ComboboxSourceLimitMode.Items.Clear();
                    ComboboxSourceLimitMode.Items.AddRange(new string[] { "Current" });
                    ComboboxSourceLimitMode.SelectedIndex = 0;

                    ComboboxSourceLimitLevelUnit.Items.Clear();
                    ComboboxSourceLimitLevelUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxSourceLimitLevelUnit.SelectedIndex = 0;
                }
                else if (SourceMode == "Current")
                {
                    ComboboxStartUnit.Items.Clear();
                    ComboboxStartUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxStartUnit.SelectedIndex = 0;

                    ComboboxStopUnit.Items.Clear();
                    ComboboxStopUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxStopUnit.SelectedIndex = 0;

                    ComboboxStepUnit.Items.Clear();
                    ComboboxStepUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxStepUnit.SelectedIndex = 0;

                    ComboboxSourceLimitMode.Items.Clear();
                    ComboboxSourceLimitMode.Items.AddRange(new string[] { "Voltage" });
                    ComboboxSourceLimitMode.SelectedIndex = 0;

                    ComboboxSourceLimitLevelUnit.Items.Clear();
                    ComboboxSourceLimitLevelUnit.Items.AddRange(new string[] { "mV", "V" });
                    ComboboxSourceLimitLevelUnit.SelectedIndex = 0;
                }

                ComboboxThicknessUnit.Items.Clear();
                ComboboxThicknessUnit.Items.AddRange(new string[] { "nm", "µm", "mm", "m" });
                ComboboxThicknessUnit.SelectedIndex = 0;

                ComboboxMagneticFieldsUnit.Items.Clear();
                ComboboxMagneticFieldsUnit.Items.AddRange(new string[] { "G", "T"});
                ComboboxMagneticFieldsUnit.SelectedIndex = 0;
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
            ComboboxStartUnit.SelectedIndex = -1;
            ComboboxStopUnit.SelectedIndex = -1;
            ComboboxStepUnit.SelectedIndex = -1;
            ComboboxSourceLimitLevelUnit.SelectedIndex = -1;
            ComboboxThicknessUnit.SelectedIndex = -1;
            ComboboxMagneticFieldsUnit.SelectedIndex = -1;
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
            MagneticFieldsValue = "";
        }

        private void PanelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                isModes = !isModes;

                UpdateToggleState();

                if (isModes == false)
                {
                    string Modes = "Van der Pauw";
                    TextboxMagneticFields.Enabled = false;
                    TextboxMagneticFields.Visible = false;
                    LabelMagneticFields.Visible = false;
                    LabelMagneticFieldsUnit.Visible = false;
                    LabelToggleSwitchVdP.ForeColor = Color.FromArgb(144, 198, 101);
                    LabelToggleSwitchHall.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    PanelToggleSwitchButton.BackColor = Color.FromArgb(253, 138, 114);
                    ComboboxMagneticFieldsUnit.Visible = false;
                    Debug.WriteLine($"You select: {Modes} measurement");

                    PictureboxTuner1.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R1_VdP.png");
                    PictureboxTuner2.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R2_VdP.png");
                    PictureboxTuner3.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R3_VdP.png");
                    PictureboxTuner4.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R4_VdP.png");
                    PictureboxTuner5.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R5_VdP.png");
                    PictureboxTuner6.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R6_VdP.png");
                    PictureboxTuner7.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R7_VdP.png");
                    PictureboxTuner8.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\R8_VdP.png");
                }

                else if (isModes == true)
                {
                    string Modes = "Hall effect";
                    TextboxMagneticFields.Enabled = true;
                    TextboxMagneticFields.Visible = true;
                    LabelMagneticFields.Visible = true;
                    LabelMagneticFieldsUnit.Visible = true;
                    LabelToggleSwitchVdP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    LabelToggleSwitchHall.ForeColor = Color.FromArgb(144, 198, 101);
                    PanelToggleSwitchButton.BackColor = Color.FromArgb(95, 77, 221);
                    ComboboxMagneticFieldsUnit.Visible = true;
                    Debug.WriteLine($"You select: {Modes} measurement");

                    PictureboxTuner1.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V1_Hall.png");
                    PictureboxTuner2.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V2_Hall.png");
                    PictureboxTuner3.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V3_Hall.png");
                    PictureboxTuner4.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V4_Hall.png");
                    PictureboxTuner5.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V5_Hall.png");
                    PictureboxTuner6.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V6_Hall.png");
                    PictureboxTuner7.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V7_Hall.png");
                    PictureboxTuner8.Image = Image.FromFile("C:\\Users\\HP\\OneDrive\\เดสก์ท็อป\\Results\\V8_Hall.png");
                }

                OnToggleChanged();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected virtual void OnToggleChanged()
        {
            ToggleChanged?.Invoke(this, EventArgs.Empty);
            PanelToggleSwitchBase.BackColor = isModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void UpdateToggleState()
        {
            targetPosition = isModes ? PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width - 1 : 1;
            PanelToggleSwitchButton.Location = new Point(targetPosition, PanelToggleSwitchButton.Location.Y);

            if (PanelToggleSwitchButton.Location.X < 0 || PanelToggleSwitchButton.Location.X > PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width)
            {
                PanelToggleSwitchButton.Location = new Point(1, PanelToggleSwitchButton.Location.Y);
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

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string startUnit = ComboboxStartUnit.SelectedItem.ToString();
                string stopUnit = ComboboxStopUnit.SelectedItem.ToString();
                string stepUnit = ComboboxStepUnit.SelectedItem.ToString();
                startValue = ConvertValueBasedOnUnit(startUnit, startValue);
                stopValue = ConvertValueBasedOnUnit(stopUnit, stopValue);
                stepValue = ConvertValueBasedOnUnit(stepUnit, stepValue);

                string sourcelimitUnit = ComboboxSourceLimitLevelUnit.SelectedItem.ToString();
                sourcelimitValue = ConvertValueBasedOnUnit(sourcelimitUnit, sourcelimitValue);

                string thicknessUnit = ComboboxThicknessUnit.SelectedItem.ToString();
                thicknessValue = ConvertValueBasedOnUnit(thicknessUnit, thicknessValue);

                string magneticfieldsUnit = ComboboxMagneticFieldsUnit.SelectedItem.ToString();
                magneticfieldsValue = ConvertValueBasedOnUnit(magneticfieldsUnit, magneticfieldsValue);

                if (savedSourceMode == "Voltage" && savedMeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIM {sourcelimitValue}");
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
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    Debug.WriteLine($"{allValues}");
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
                    SMU.WriteString($"SOURce:VOLTage:ILIM {sourcelimitValue}");
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
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    Debug.WriteLine($"{allValues}");
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
                    SMU.WriteString($"SOURce:CURRent:VLIM {sourcelimitValue}");
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
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    Debug.WriteLine($"{allValues}");
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
                    SMU.WriteString($"SOURce:CURRent:VLIM {sourcelimitValue}");
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
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    Debug.WriteLine($"{allValues}.");
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

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetitions, out double sourcelimit, out double thickness, out double magneticfields)
        {
            start = stop = step = sourcelimit = 0;
            repetitions = 1;
            thickness = 0;
            magneticfields = 0;
            
            if (isModes == false)
            {
                if (!double.TryParse(TextboxStart.Text, out start) || !double.TryParse(TextboxStop.Text, out stop) || !double.TryParse(TextboxStep.Text, out step) || !int.TryParse(TextboxRepetition.Text, out repetitions) || !double.TryParse(TextboxSourceLimitLevel.Text, out sourcelimit) || !double.TryParse(TextboxThickness.Text, out thickness) || start >= stop || step <= 0 || repetitions < 1)
                {
                    return false;
                }
            }
            else
            {
                if (!double.TryParse(TextboxStart.Text, out start) || !double.TryParse(TextboxStop.Text, out stop) || !double.TryParse(TextboxStep.Text, out step) || !int.TryParse(TextboxRepetition.Text, out repetitions) || !double.TryParse(TextboxSourceLimitLevel.Text, out sourcelimit) || !double.TryParse(TextboxThickness.Text, out thickness) || !double.TryParse(TextboxMagneticFields.Text, out magneticfields) || start >= stop || step <= 0 || repetitions < 1 || magneticfields < 0)
                {
                    return false;
                }
            }

            return true;
        }

        private double ConvertValueBasedOnUnit(string unit, double value)  //Method สำหรับการแปลงค่า
        {
            switch (unit)
            {
                case "mV":
                    return value * 1E-3;  // แปลงเป็นหน่วย milliVolt
                case "V":
                    return value;  // แปลงเป็นหน่วย Volt
                case "nA":
                    return value * 1E-9;  // แปลงเป็นหน่วย nanoAmpere
                case "µA":
                    return value * 1E-6;  // แปลงเป็นหน่วย microAmpere
                case "mA":
                    return value * 1E-3;  // แปลงเป็นหน่วย milliAmpere
                case "A":
                    return value;  // แปลงเป็นหน่วย Ampere
                case "nm":
                    return value * 1E-9; //แปลงเป็นหน่วย nanoMeter
                case "µm":
                    return value * 1E-6;  //แปลงเป็นหน่วย microMeter
                case "mm":
                    return value * 1E-3;  //แปลงเป็นหน่วย milliMeter
                case "m":
                    return value;  //แปลงเป็นหน่วย Meter
                case "G":
                    return value * 1E+4;  //แปลงเป็นหน่วย Gauss
                case "T":
                    return value;  //แปลงเป็นหน่วย Tesla
                default:
                    throw new Exception("Unknown unit");  //ไม่รู้จักหน่วย (Error)
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

        private void PlaySMUConnectionMelody()
        {
            var melody = new List<(int frequency, double duration)>
            {
                (784, 0.150), // G5
                //(20, 0.100),
                (699, 0.150), // F5
                //(20, 0.100),
                (440, 0.250), // A4
                //(20, 0.100),
                (494, 0.250), // B4
                //(20, 0.100),
                (659, 0.150), // E5
                //(20, 0.100),
                (587, 0.150), // D5
                //(20, 0.100),
                (349, 0.250), // F4
                //(20, 0.100),
                (392, 0.250), // G4
                //(20, 0.100),
                (587, 0.150), // D5
                //(20, 0.400),
                (523, 0.150), // C5
                //(20, 0.400),
                (330, 0.300), // E4
                //(20, 0.800),
                (392, 0.400), // G4
                //(20, 1.000),
                (523, 1.000) // C4
            };

            foreach (var (frequency, duration) in melody)
            {
                string scpiCommand = $"SYSTem:BEEPer {frequency}, {duration}";
                SMU.WriteString(scpiCommand);
                //Console.Beep(frequency, (int)duration);
                System.Threading.Thread.Sleep((int)(duration * 1));
            }
        }

        /*private void PlaySSConnectionMelody()
        {
            var melody = new List<(int frequency, double duration)>
            {
                (372, 125), // F#4+3 Hz
                (372, 125), // F#4+3 Hz
                (372, 125), // F#4+3 Hz
                (372, 125), // F#4+3 Hz
                (314, 150), // D#4+3 Hz
                (332, 150), // E4+3 Hz
                (372, 1000), // F#4+3 Hz
            };

            foreach (var (frequency, duration) in melody)
            {
                //string scpiCommand = $"SYSTem:BEEPer {frequency}, {duration}";
                //SMU.WriteString(scpiCommand);
                Console.Beep(frequency, (int)duration);
                System.Threading.Thread.Sleep((int)(duration * 0.2));
            }
        }*/

        private void PictureboxTuner1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    Debug.WriteLine("SMU or SS is not connected. Exiting function.");
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("");
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
