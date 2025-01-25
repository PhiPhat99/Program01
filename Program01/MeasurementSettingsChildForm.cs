using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
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
        private Form currentTunerandDataChildForm;
        public event EventHandler toggleChanged;
        public bool IsOn
        {
            get => isModes;
            set
            {
                isModes = value;
                updateToggleState();
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

                comboboxVISASMUIOPort.Items.Clear();
                comboboxVISASSIOPort.Items.Clear();

                foreach (string SMUaddress in SMUgpibAddress)
                {
                    comboboxVISASMUIOPort.Items.Add(SMUaddress);
                }

                if (comboboxVISASMUIOPort.Items.Count > 0)
                {
                    comboboxVISASMUIOPort.SelectedIndex = 0;
                }

                if (comboboxVISASMUIOPort.Items.Count > 0)
                {
                    comboboxVISASMUIOPort.SelectedIndex = 0;
                }

                foreach (string SSaddress in SSgpibAddress)
                {
                    comboboxVISASSIOPort.Items.Add(SSaddress);
                }

                if (comboboxVISASSIOPort.Items.Count > 0)
                {
                    comboboxVISASSIOPort.SelectedIndex = 0;
                }

                if (comboboxVISASSIOPort.Items.Count > 0)
                {
                    comboboxVISASSIOPort.SelectedIndex = 0;
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
            public static readonly Color Color2 = Color.FromArgb(242, 234, 213);
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

        /*private void saveSettings()
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
        }*/

        private void iconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboboxVISASMUIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for Source Measure Unit.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSMUAddress = comboboxVISASMUIOPort.SelectedItem.ToString();

                if (!isSMUConnected)
                {
                    SMU.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSMU.Open(selectedSMUAddress);
                    SMU.IO.Timeout = 5000;
                    SMU.WriteString("*IDN?");
                    string response = SMU.ReadString();
                    Debug.WriteLine($"{response}");

                    isSMUConnected = true;
                    playSMUConnectionMelody();

                    //System.Threading.Thread.Sleep(4000);
                    iconbuttonSMUConnection.BackColor = Color.Snow;
                    iconbuttonSMUConnection.IconColor = Color.GreenYellow;
                    MessageBox.Show("Connected to Source Measure Unit", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                        iconbuttonSMUConnection.BackColor = Color.Snow;
                        iconbuttonSMUConnection.IconColor = Color.Gainsboro;

                        MessageBox.Show("Disconnected from the Source Measure Unit.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void iconbuttonSSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboboxVISASSIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for Switch System.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSSAddress = comboboxVISASSIOPort.SelectedItem.ToString();

                if (!isSSConnected)
                {
                    SS.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSS.Open(selectedSSAddress);
                    SS.IO.Timeout = 5000;

                    SS.WriteString("*CLS");
                    SS.WriteString("*IDN?");
                    string response = SS.ReadString();
                    Debug.WriteLine($"{response}");

                    isSSConnected = true;

                    iconbuttonSSConnection.BackColor = Color.Snow;
                    iconbuttonSSConnection.IconColor = Color.GreenYellow;
                    MessageBox.Show("Connected to Switch System", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                        iconbuttonSSConnection.BackColor = Color.Snow;
                        iconbuttonSSConnection.IconColor = Color.Gainsboro;

                        MessageBox.Show("Disconnected from the Switch System.", "Disconnection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            comboboxRsense.Items.Add("2-Wires");
            comboboxRsense.Items.Add("4-Wires");

            comboboxMeasure.Items.Add("Voltage");
            comboboxMeasure.Items.Add("Current");

            comboboxSource.Items.Add("Voltage");
            comboboxSource.Items.Add("Current");

            comboboxRsense.SelectedItem = GlobalSettings.RsenseMode;
            comboboxMeasure.SelectedItem = GlobalSettings.MeasureMode;
            comboboxSource.SelectedItem = GlobalSettings.SourceMode;
            comboboxSourceLimitMode.SelectedItem = GlobalSettings.SourceLimitType;
            textboxStart.Text = GlobalSettings.StartValue;
            textboxStop.Text = GlobalSettings.StopValue;
            textboxStep.Text = GlobalSettings.StepValue;
            textboxSourceLimitLevel.Text = GlobalSettings.SourceLimitLevelValue;
            textboxThickness.Text = GlobalSettings.ThicknessValue;
            textboxRepetition.Text = GlobalSettings.RepetitionValue;
            textboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;
        }

        /*private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
        }*/

        private void comboboxRsense_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RsenseMode = comboboxRsense.SelectedItem?.ToString();
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
                            comboboxRsense.SelectedIndex = -1;
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
                            comboboxRsense.SelectedIndex = -1;
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

        private void comboboxMeasure_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MeasureMode = comboboxMeasure.SelectedItem?.ToString();
                savedMeasureMode = MeasureMode;

                switch (MeasureMode)
                {
                    case "Voltage":
                        break;
                    case "Current":
                        break;
                    default:
                        comboboxMeasure.SelectedIndex = -1;
                        MeasureMode = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void comboboxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceMode = comboboxSource.SelectedItem?.ToString();
                savedSourceMode = SourceMode;

                switch (SourceMode)
                {
                    case "Voltage":
                        updateMeasurementSettingsUnits("Voltage");
                        break;
                    case "Current":
                        updateMeasurementSettingsUnits("Current");
                        break;
                    default:
                        comboboxSource.SelectedIndex = -1;
                        SourceMode = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void comboboxSourceLimitMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceLimit = comboboxSourceLimitMode.SelectedItem?.ToString();
                savedSourceLimitMode = SourceLimit;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void updateMeasurementSettingsUnits(string SourceMode)
        {
            try
            {
                if (SourceMode == "Voltage")
                {
                    comboboxStartUnit.Items.Clear();
                    comboboxStartUnit.Items.AddRange(new string[] { "mV", "V" });
                    comboboxStartUnit.SelectedIndex = 0;

                    comboboxStopUnit.Items.Clear();
                    comboboxStopUnit.Items.AddRange(new string[] { "mV", "V" });
                    comboboxStopUnit.SelectedIndex = 0;

                    comboboxStepUnit.Items.Clear();
                    comboboxStepUnit.Items.AddRange(new string[] { "mV", "V" });
                    comboboxStepUnit.SelectedIndex = 0;

                    comboboxSourceLimitMode.Items.Clear();
                    comboboxSourceLimitMode.Items.AddRange(new string[] { "Current" });
                    comboboxSourceLimitMode.SelectedIndex = 0;

                    comboboxSourceLimitLevelUnit.Items.Clear();
                    comboboxSourceLimitLevelUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    comboboxSourceLimitLevelUnit.SelectedIndex = 0;
                }
                else if (SourceMode == "Current")
                {
                    comboboxStartUnit.Items.Clear();
                    comboboxStartUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    comboboxStartUnit.SelectedIndex = 0;

                    comboboxStopUnit.Items.Clear();
                    comboboxStopUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    comboboxStopUnit.SelectedIndex = 0;

                    comboboxStepUnit.Items.Clear();
                    comboboxStepUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    comboboxStepUnit.SelectedIndex = 0;

                    comboboxSourceLimitMode.Items.Clear();
                    comboboxSourceLimitMode.Items.AddRange(new string[] { "Voltage" });
                    comboboxSourceLimitMode.SelectedIndex = 0;

                    comboboxSourceLimitLevelUnit.Items.Clear();
                    comboboxSourceLimitLevelUnit.Items.AddRange(new string[] { "mV", "V" });
                    comboboxSourceLimitLevelUnit.SelectedIndex = 0;
                }

                comboboxThicknessUnit.Items.Clear();
                comboboxThicknessUnit.Items.AddRange(new string[] { "nm", "µm", "mm", "cm", "m" });
                comboboxThicknessUnit.SelectedIndex = 0;

                comboboxMagneticFieldsUnit.Items.Clear();
                comboboxMagneticFieldsUnit.Items.AddRange(new string[] { "T", "G" });
                comboboxMagneticFieldsUnit.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonClearSettings_Click(object sender, EventArgs e)
        {
            clearSettings();
        }

        private void clearSettings()
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                comboboxRsense.SelectedIndex = -1;
                comboboxMeasure.SelectedIndex = -1;
                comboboxSource.SelectedIndex = -1;
                comboboxSourceLimitMode.SelectedIndex = -1;
                comboboxStartUnit.SelectedIndex = -1;
                comboboxStopUnit.SelectedIndex = -1;
                comboboxStepUnit.SelectedIndex = -1;
                comboboxSourceLimitLevelUnit.SelectedIndex = -1;
                comboboxThicknessUnit.SelectedIndex = -1;
                comboboxMagneticFieldsUnit.SelectedIndex = -1;
                textboxStart.Text = "";
                textboxStep.Text = "";
                textboxStop.Text = "";
                textboxSourceLimitLevel.Text = "";
                textboxThickness.Text = "";
                textboxRepetition.Text = "";
                textboxMagneticFields.Text = "";
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

                SS.WriteString("ROUTe:OPEN ALL");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void panelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                isModes = !isModes;

                updateToggleState();

                if (isModes == false)
                {
                    string Modes = "Van der Pauw";
                    textboxMagneticFields.Enabled = false;
                    textboxMagneticFields.Visible = false;
                    labelMagneticFields.Visible = false;
                    labelMagneticFieldsUnit.Visible = false;
                    labelToggleSwitchVdP.ForeColor = Color.FromArgb(144, 198, 101);
                    labelToggleSwitchHall.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    panelToggleSwitchButton.BackColor = Color.FromArgb(253, 138, 114);
                    comboboxMagneticFieldsUnit.Visible = false;
                    Debug.WriteLine($"You select: {Modes} measurement");

                    pictureboxTuner1.Image = global::Program01.Properties.Resources.R1_VdP;
                    pictureboxTuner2.Image = global::Program01.Properties.Resources.R2_VdP;
                    pictureboxTuner3.Image = global::Program01.Properties.Resources.R3_VdP;
                    pictureboxTuner4.Image = global::Program01.Properties.Resources.R4_VdP;
                    pictureboxTuner5.Image = global::Program01.Properties.Resources.R5_VdP;
                    pictureboxTuner6.Image = global::Program01.Properties.Resources.R6_VdP;
                    pictureboxTuner7.Image = global::Program01.Properties.Resources.R7_VdP;
                    pictureboxTuner8.Image = global::Program01.Properties.Resources.R8_VdP;
                }

                else if (isModes == true)
                {
                    string Modes = "Hall effect";
                    textboxMagneticFields.Enabled = true;
                    textboxMagneticFields.Visible = true;
                    labelMagneticFields.Visible = true;
                    labelMagneticFieldsUnit.Visible = true;
                    labelToggleSwitchVdP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                    labelToggleSwitchHall.ForeColor = Color.FromArgb(144, 198, 101);
                    panelToggleSwitchButton.BackColor = Color.FromArgb(95, 77, 221);
                    comboboxMagneticFieldsUnit.Visible = true;
                    Debug.WriteLine($"You select: {Modes} measurement");

                    pictureboxTuner1.Image = global::Program01.Properties.Resources.V1_Hall;
                    pictureboxTuner2.Image = global::Program01.Properties.Resources.V2_Hall;
                    pictureboxTuner3.Image = global::Program01.Properties.Resources.V3_Hall;
                    pictureboxTuner4.Image = global::Program01.Properties.Resources.V4_Hall;
                    pictureboxTuner5.Image = global::Program01.Properties.Resources.V5_Hall;
                    pictureboxTuner6.Image = global::Program01.Properties.Resources.V6_Hall;
                    pictureboxTuner7.Image = global::Program01.Properties.Resources.V7_Hall;
                    pictureboxTuner8.Image = global::Program01.Properties.Resources.V8_Hall;
                }

                onToggleChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected virtual void onToggleChanged()
        {
            toggleChanged?.Invoke(this, EventArgs.Empty);
            panelToggleSwitchBase.BackColor = isModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void updateToggleState()
        {
            targetPosition = isModes ? panelToggleSwitchBase.Width - panelToggleSwitchButton.Width - 1 : 1;
            panelToggleSwitchButton.Location = new Point(targetPosition, panelToggleSwitchButton.Location.Y);

            if (panelToggleSwitchButton.Location.X < 0 || panelToggleSwitchButton.Location.X > panelToggleSwitchBase.Width - panelToggleSwitchButton.Width)
            {
                panelToggleSwitchButton.Location = new Point(1, panelToggleSwitchButton.Location.Y);
            }
        }

        private void playSMUConnectionMelody()
        {
            var melody = new List<(int frequency, double duration)>
            {
                /*(784, 0.150), // G5
                (699, 0.150), // F5
                (440, 0.250), // A4
                (494, 0.250), // B4
                (659, 0.150), // E5
                (587, 0.150), // D5
                (349, 0.250), // F4
                (392, 0.250), // G4
                (587, 0.150), // D5
                (523, 0.150), // C5
                (330, 0.300), // E4
                (392, 0.400), // G4*/
                (523, 0.800) // C4
            };

            foreach (var (frequency, duration) in melody)
            {
                string scpiCommand = $"SYSTem:BEEPer {frequency}, {duration}";
                SMU.WriteString(scpiCommand);
                System.Threading.Thread.Sleep((int)(duration * 1));
            }
        }

        private void pictureboxTuner1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!8)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!8)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!9)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!9)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!9)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!9)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner5_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner6_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!7)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!10)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!7)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!7)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureboxTuner8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!7)");
                }
                else if (isModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");

                    SS.WriteString("ROUTe:CLOSe (@ 1!1!8)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!10)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!9)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!7)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonTunerTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!validateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (savedSourceMode == "Voltage" && savedMeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
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
                    Debug.WriteLine($"{allValues}.");
                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INIT");
                    SMU.WriteString("*WAI");
                    SMU.WriteString("OUTPut OFF");
                    SMU.WriteString("TRACe:DATA?");
                    string measureValue = SMU.ReadString();
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
                    SMU.WriteString("TRACe:DATA?");
                    string measureValue = SMU.ReadString();
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
                    SMU.WriteString("TRACe:DATA?");
                    string measureValue = SMU.ReadString();
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
                    SMU.WriteString("TRACe:DATA?");
                    string measureValue = SMU.ReadString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /*private void iconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!validateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SMU.WriteString($"SOURce:FUNCtion VOLTage");
                SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                SMU.WriteString($"SOURce:VOLTage:ILIM {sourcelimitValue}");
                SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                SMU.WriteString($"SENSe:VOLTage:RSENse {(savedRsenseMode == "4-Wires" ? "ON" : "OFF")}");

                if (savedSourceMode == "Voltage" && savedMeasureMode == "Voltage")
                {
                    for (int tunerNumber = 1; tunerNumber <= 8; tunerNumber++)
                    {
                        SetTuner(tunerNumber);

                        for (int repetition = 0; repetition < repetitionValue; repetition++)
                        {
                            string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                            SMU.WriteString(sweepCommand);
                            SMU.WriteString("INITiate");
                            SMU.WriteString("*WAI");

                            string measurementData = SMU.ReadString();
                            Debug.WriteLine($"Measurement Data: {measurementData}");

                            int totalDelay = calculateTotalDelay(stepValue, startValue, stopValue, repetitionValue, 100);
                            System.Threading.Thread.Sleep(totalDelay);

                            SMU.WriteString("OUTPut OFF");
                        }
                    }

                    SS.WriteString("ROUTe:OPEN ALL");
                    SMU.WriteString("*CLS");
                    SS.WriteString("*CLS");
                    MessageBox.Show("Measurement completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                    for (int repetition = 1; repetition < repetitionValue; repetition++)
                    {

                        for (int tunerNumber = 1; tunerNumber <= 8; tunerNumber++)
                        {
                            SetTuner(tunerNumber);

                            string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                            SMU.WriteString(sweepCommand);
                            SMU.WriteString("INITiate");
                            SMU.WriteString("*OPC?");
                            string opcSMUResponse = SMU.ReadString();

                            if (opcSMUResponse.Trim() != "1")
                            {
                                throw new Exception("Sweep operation did not complete successfully.");
                            }

                            string measurementData = SMU.ReadString();
                            Debug.WriteLine($"Measurement Data: {measurementData}");
                            int totalDelay = calculateTotalDelay(stepValue, startValue, stopValue, repetitionValue, 100);
                            Debug.WriteLine($"Applying delay of {totalDelay} ms before switching tuner.");
                            System.Threading.Thread.Sleep(totalDelay);
                        }
                    }

                    SMU.WriteString("OUTPut OFF");
                    SS.WriteString("ROUTe:OPEN ALL");
                    SMU.WriteString("*CLS");
                    SS.WriteString("*CLS");
                    MessageBox.Show("Measurement completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                    for (int repetition = 1; repetition < repetitionValue; repetition++)
                    {

                        for (int tunerNumber = 1; tunerNumber <= 8; tunerNumber++)
                        {
                            SetTuner(tunerNumber);

                            string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                            SMU.WriteString(sweepCommand);
                            SMU.WriteString("INITiate");
                            SMU.WriteString("*OPC?");
                            string opcSMUResponse = SMU.ReadString();

                            if (opcSMUResponse.Trim() != "1")
                            {
                                throw new Exception("Sweep operation did not complete successfully.");
                            }

                            string measurementData = SMU.ReadString();
                            Debug.WriteLine($"Measurement Data: {measurementData}");
                            int totalDelay = calculateTotalDelay(stepValue, startValue, stopValue, repetitionValue, 100);
                            Debug.WriteLine($"Applying delay of {totalDelay} ms before switching tuner.");
                            System.Threading.Thread.Sleep(totalDelay);
                        }
                    }

                    SMU.WriteString("OUTPut OFF");
                    SS.WriteString("ROUTe:OPEN ALL");
                    SMU.WriteString("*CLS");
                    SS.WriteString("*CLS");
                    MessageBox.Show("Measurement completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                    for (int repetition = 1; repetition < repetitionValue; repetition++)
                    {

                        for (int tunerNumber = 1; tunerNumber <= 8; tunerNumber++)
                        {
                            SetTuner(tunerNumber);

                            string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, 100e-3, {repetitionValue}";
                            SMU.WriteString(sweepCommand);
                            SMU.WriteString("INITiate");
                            SMU.WriteString("*OPC?");
                            string opcSMUResponse = SMU.ReadString();

                            if (opcSMUResponse.Trim() != "1")
                            {
                                throw new Exception("Sweep operation did not complete successfully.");
                            }

                            string measurementData = SMU.ReadString();
                            Debug.WriteLine($"Measurement Data: {measurementData}");
                            int totalDelay = calculateTotalDelay(stepValue, startValue, stopValue, repetitionValue, 100);
                            Debug.WriteLine($"Applying delay of {totalDelay} ms before switching tuner.");
                            System.Threading.Thread.Sleep(totalDelay);
                        }
                    }
                }

                SMU.WriteString("OUTPut OFF");
                SS.WriteString("ROUTe:OPEN ALL");
                SMU.WriteString("*CLS");
                SS.WriteString("*CLS");
                MessageBox.Show("Measurement completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void setTuner(int tunerNumber)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SS.WriteString("*CLS");
                SS.WriteString("ROUTe:OPEN ALL");
                System.Threading.Thread.Sleep(500);

                var channelConfigurations = new Dictionary<int, List<string>>
                {
                    { 1, isModes == false ? new List<string> { "1!1!8", "1!2!9", "1!3!7", "1!4!10" } :
                                            new List<string> { "1!1!7", "1!2!9", "1!3!10", "1!4!8" }},
                    { 2, isModes == false ? new List<string> { "1!1!9", "1!2!8", "1!3!7", "1!4!10" } :
                                            new List<string> { "1!1!9", "1!2!7", "1!3!10", "1!4!8" }},
                    { 3, isModes == false ? new List<string> { "1!1!7", "1!2!10", "1!3!8", "1!4!9" } :
                                            new List<string> { "1!1!8", "1!2!10", "1!3!7", "1!4!9" }},
                    { 4, isModes == false ? new List<string> { "1!1!10", "1!2!7", "1!3!8", "1!4!9" } :
                                            new List<string> { "1!1!10", "1!2!8", "1!3!7", "1!4!9" }},
                    { 5, isModes == false ? new List<string> { "1!1!8", "1!2!7", "1!3!9", "1!4!10" } :
                                            new List<string> { "1!1!9", "1!2!7", "1!3!8", "1!4!10" }},
                    { 6, isModes == false ? new List<string> { "1!1!7", "1!2!8", "1!3!9", "1!4!10" } :
                                            new List<string> { "1!1!7", "1!2!9", "1!3!8", "1!4!10" }},
                    { 7, isModes == false ? new List<string> { "1!1!9", "1!2!10", "1!3!8", "1!4!7" } :
                                            new List<string> { "1!1!8", "1!2!10", "1!3!9", "1!4!7" }},
                    { 8, isModes == false ? new List<string> { "1!1!10", "1!2!9", "1!3!8", "1!4!7" } :
                                            new List<string> { "1!1!10", "1!2!8", "1!3!9", "1!4!7" }}
                };

                if (channelConfigurations.ContainsKey(tunerNumber))
                {
                    foreach (var channel in channelConfigurations[tunerNumber])
                    {
                        SS.WriteString($"ROUTe:CLOSe (@ {channel})");
                        SS.WriteString("*OPC?");
                        string response = SS.ReadString();

                        if (response.Trim() != "1")
                        {
                            throw new Exception("Channel switching failed.");
                        }

                        System.Threading.Thread.Sleep(500);
                    }
                }

                SS.WriteString("SYSTem:PRESet");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }*/

        private void iconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SMU.WriteString("SYSTem:ERRor?");
                SS.WriteString("SYSTem:ERRor?");
                string SMUrespones = SMU.ReadString();
                string SSresponse = SS.ReadString();
                Debug.WriteLine($"There is SMU error : {SMUrespones}");
                Debug.WriteLine($"There is SS error : {SSresponse}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void buttonData_Click(object sender, EventArgs e)
        {
            try
            {
                // สร้างฟอร์ม MeasurementSettingsDataChildForm และส่งข้อมูลให้ฟอร์มลูก
                var dataChildForm = new MeasurementSettingsDataChildForm();

                // อัปเดต Chart ด้วยข้อมูลเริ่มต้น
                dataChildForm.updateChartData(new[] { 5, 10, 15, 20 });

                openChildForm(dataChildForm); // เปิดฟอร์มลูกใน Panel
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void openChildForm(Form childForm)
        {
            try
            {
                // ปิดฟอร์มลูกก่อนหน้า ถ้ามี
                currentTunerandDataChildForm?.Close();
                currentTunerandDataChildForm = childForm;

                // ตั้งค่าฟอร์มลูกให้อยู่ใน Panel
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                panelTunerandData.Controls.Add(childForm);
                panelTunerandData.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void buttonTuner_Click(object sender, EventArgs e)
        {
            try
            {
                currentTunerandDataChildForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private bool validateInputs(out double start, out double stop, out double step, out int repetition, out double sourcelimit, out double thickness, out double magneticfields)
        {
            start = stop = step = sourcelimit = thickness = magneticfields = 0;
            repetition = 1;

            try
            {
                start = convertValueBasedOnUnit(comboboxStartUnit.SelectedItem.ToString(), double.Parse(textboxStart.Text));
                stop = convertValueBasedOnUnit(comboboxStopUnit.SelectedItem.ToString(), double.Parse(textboxStop.Text));
                step = convertValueBasedOnUnit(comboboxStepUnit.SelectedItem.ToString(), double.Parse(textboxStep.Text));
                sourcelimit = convertValueBasedOnUnit(comboboxSourceLimitLevelUnit.SelectedItem.ToString(), double.Parse(textboxSourceLimitLevel.Text));
                thickness = convertValueBasedOnUnit(comboboxThicknessUnit.SelectedItem.ToString(), double.Parse(textboxThickness.Text));
                magneticfields = isModes ? convertValueBasedOnUnit(comboboxMagneticFieldsUnit.SelectedItem.ToString(), double.Parse(textboxMagneticFields.Text)) : 0;
                repetition = int.Parse(textboxRepetition.Text);

                if (start >= stop || step <= 0 || repetition < 1 || repetition > 999 || thickness < 0 || sourcelimit < 0)
                {
                    return false;
                }

                if (isModes && (magneticfields < 0 || step >= stop))
                {
                    return false;
                }

                if (SourceLimit == "Current" && sourcelimit > 1.05 || sourcelimit < -1.05)
                {
                    return false;
                }

                if (SourceLimit == "Voltage" && sourcelimit > 210 || sourcelimit < -210)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private double convertValueBasedOnUnit(string unit, double value)  //  Method สำหรับการแปลงหน่วยของค่าที่ผู้ใช้ป้อนเข้ามา
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
                case "cm":
                    return value * 1E-2;  //แปลงเป็นหน่วย centiMeter
                case "m":
                    return value;  //แปลงเป็นหน่วย Meter
                case "G":
                    return value * 1E+4;  //แปลงเป็นหน่วย Gauss
                case "T":
                    return value;  //แปลงเป็นหน่วย Tesla
                default:
                    throw new Exception("Unknown unit");  //ไม่รู้จักหน่วย (Unit Error)
            }
        }

        private void iconbuttonUpdateChart_Click(object sender, EventArgs e)
        {
            try
            {
                // อัปเดตข้อมูลในฟอร์มลูก (เช่น ส่งข้อมูลใหม่ให้ Chart)
                if (currentTunerandDataChildForm is MeasurementSettingsDataChildForm dataChildForm)
                {
                    dataChildForm.updateChartData(new[] { 25, 30, 35, 40 }); // ส่งข้อมูลใหม่ไปยัง Chart
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
