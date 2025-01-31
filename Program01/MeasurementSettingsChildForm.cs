using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private int currentTuner;
        private int currentRepeat;
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

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboboxVISASMUIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for Source Measure Unit.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    IconbuttonSMUConnection.BackColor = Color.Snow;
                    IconbuttonSMUConnection.IconColor = Color.GreenYellow;
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

                        IconbuttonSMUConnection.BackColor = Color.Snow;
                        IconbuttonSMUConnection.IconColor = Color.Gainsboro;

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

        private void IconbuttonSSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboboxVISASSIOPort.SelectedItem == null)
                {
                    MessageBox.Show("Please select a GPIB Address for Switch System.", "Address Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSSAddress = ComboboxVISASSIOPort.SelectedItem.ToString();

                if (!isSSConnected)
                {
                    SS.IO = (Ivi.Visa.Interop.IMessage)resourcemanagerSS.Open(selectedSSAddress);
                    SS.IO.Timeout = 5000;

                    SS.WriteString("*CLS");
                    SS.IO.Timeout = 5000;
                    SS.WriteString("*IDN?");
                    string response = SS.ReadString();
                    Debug.WriteLine($"{response}");

                    isSSConnected = true;

                    IconbuttonSSConnection.BackColor = Color.Snow;
                    IconbuttonSSConnection.IconColor = Color.GreenYellow;
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

                        IconbuttonSSConnection.BackColor = Color.Snow;
                        IconbuttonSSConnection.IconColor = Color.Gainsboro;

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
            TextboxSourceLimit.Text = GlobalSettings.SourceLimitLevelValue;
            TextboxThickness.Text = GlobalSettings.ThicknessValue;
            TextboxRepetition.Text = GlobalSettings.RepetitionValue;
            TextboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;
        }

        /*private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
        }*/

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
                    ComboboxStartUnit.Items.AddRange(new string[] { "mV", "V" });
                    ComboboxStartUnit.SelectedIndex = 0;

                    ComboboxStopUnit.Items.Clear();
                    ComboboxStopUnit.Items.AddRange(new string[] { "mV", "V" });
                    ComboboxStopUnit.SelectedIndex = 0;

                    ComboboxStepUnit.Items.Clear();
                    ComboboxStepUnit.Items.AddRange(new string[] { "mV", "V" });
                    ComboboxStepUnit.SelectedIndex = 0;

                    ComboboxSourceLimitMode.Items.Clear();
                    ComboboxSourceLimitMode.Items.AddRange(new string[] { "Current" });
                    ComboboxSourceLimitMode.SelectedIndex = 0;

                    ComboboxSourceLimitUnit.Items.Clear();
                    ComboboxSourceLimitUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxSourceLimitUnit.SelectedIndex = 0;
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

                    ComboboxSourceLimitUnit.Items.Clear();
                    ComboboxSourceLimitUnit.Items.AddRange(new string[] { "mV", "V" });
                    ComboboxSourceLimitUnit.SelectedIndex = 0;
                }

                ComboboxSourceDelayUnit.Items.Clear();
                ComboboxSourceDelayUnit.Items.AddRange(new string[] { "µs", "ms", "s", "ks" });
                ComboboxSourceDelayUnit.SelectedIndex = 0;

                ComboboxThicknessUnit.Items.Clear();
                ComboboxThicknessUnit.Items.AddRange(new string[] { "nm", "µm", "mm", "cm", "m" });
                ComboboxThicknessUnit.SelectedIndex = 0;

                ComboboxMagneticFieldsUnit.Items.Clear();
                ComboboxMagneticFieldsUnit.Items.AddRange(new string[] { "T", "G" });
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
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ComboboxRsense.SelectedIndex = -1;
                ComboboxMeasure.SelectedIndex = -1;
                ComboboxSource.SelectedIndex = -1;
                ComboboxSourceLimitMode.SelectedIndex = -1;
                ComboboxStartUnit.SelectedIndex = -1;
                ComboboxStopUnit.SelectedIndex = -1;
                ComboboxStepUnit.SelectedIndex = -1;
                ComboboxSourceDelayUnit.SelectedIndex = -1;
                ComboboxSourceLimitUnit.SelectedIndex = -1;
                ComboboxThicknessUnit.SelectedIndex = -1;
                ComboboxMagneticFieldsUnit.SelectedIndex = -1;
                TextboxStart.Text = "";
                TextboxStep.Text = "";
                TextboxStop.Text = "";
                TextboxSourceDelay.Text = "";
                TextboxSourceLimit.Text = "";
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

                SS.WriteString("ROUTe:OPEN ALL");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
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

                    PictureboxTuner1.Image = global::Program01.Properties.Resources.R1_VdP;
                    PictureboxTuner2.Image = global::Program01.Properties.Resources.R2_VdP;
                    PictureboxTuner3.Image = global::Program01.Properties.Resources.R3_VdP;
                    PictureboxTuner4.Image = global::Program01.Properties.Resources.R4_VdP;
                    PictureboxTuner5.Image = global::Program01.Properties.Resources.R5_VdP;
                    PictureboxTuner6.Image = global::Program01.Properties.Resources.R6_VdP;
                    PictureboxTuner7.Image = global::Program01.Properties.Resources.R7_VdP;
                    PictureboxTuner8.Image = global::Program01.Properties.Resources.R8_VdP;
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

                    PictureboxTuner1.Image = global::Program01.Properties.Resources.V1_Hall;
                    PictureboxTuner2.Image = global::Program01.Properties.Resources.V2_Hall;
                    PictureboxTuner3.Image = global::Program01.Properties.Resources.V3_Hall;
                    PictureboxTuner4.Image = global::Program01.Properties.Resources.V4_Hall;
                    PictureboxTuner5.Image = global::Program01.Properties.Resources.V5_Hall;
                    PictureboxTuner6.Image = global::Program01.Properties.Resources.V6_Hall;
                    PictureboxTuner7.Image = global::Program01.Properties.Resources.V7_Hall;
                    PictureboxTuner8.Image = global::Program01.Properties.Resources.V8_Hall;
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

        private void PictureboxTuner1_Click(object sender, EventArgs e)
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

        private void PictureboxTuner2_Click(object sender, EventArgs e)
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

        private void PictureboxTuner3_Click(object sender, EventArgs e)
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

        private void PictureboxTuner4_Click(object sender, EventArgs e)
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

        private void PictureboxTuner5_Click(object sender, EventArgs e)
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

        private void PictureboxTuner6_Click(object sender, EventArgs e)
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

        private void PictureboxTuner7_Click(object sender, EventArgs e)
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

        private void PictureboxTuner8_Click(object sender, EventArgs e)
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

        private void IconbuttonTunerTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
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

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Source Delay: {delayValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");
                    Debug.WriteLine($"{allValues}.");
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

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Source Delay: {delayValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
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

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Source Delay: {delayValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
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

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    string allValues = $"Sense: {savedRsenseMode}, Measure: {savedMeasureMode}, Source: {savedSourceMode}, Start: {startValue}, Step: {stepValue}, Source Delay: {delayValue}, Stop: {stopValue}, Source Limit: {savedSourceLimitMode}, Limit Level: {sourcelimitValue}, Repetition: {repetitionValue}, Thickness: {thicknessValue}, Magnetic Fields: {magneticfieldsValue}";
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

        private void IconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isSMUConnected && !isSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                currentTuner = 1;
                currentRepeat = 0;
                RunMeasurement();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void RunMeasurement()
        {
            try
            {
                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                while (currentTuner <= 8)
                {
                    ConfigureSwitchSystem();
                    ConfigureSourceMeasureUnit();
                    UpdateMeasurementState();

                    await ExecuteSweep();
                    await Task.Delay(points * repetitionValue * 400);

                    if (currentTuner > 8)
                    {
                        Debug.WriteLine("All tuners completed.");
                        MessageBox.Show("Measurement completed", "Measurement Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SMU.WriteString("OUTPut OFF");
                        SS.WriteString("*CLS");
                        //break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ConfigureSwitchSystem()
        {
            SS.WriteString("ROUTe:OPEN ALL");

            var channels = GetChannelConfiguration(currentTuner, isModes);
            foreach (var channel in channels)
            {
                SS.WriteString($"ROUTe:CLOSe (@ {channel})");
                Debug.WriteLine($"ROUTe:CLOSe (@ {channel})");
            }
        }

        private List<string> GetChannelConfiguration(int tuner, bool isModes)
        {
            var configurations = new Dictionary<int, List<string>>
            {
                { 1, isModes == false ? new List<string> { "1!1!8", "1!2!9", "1!3!7", "1!4!10" } :
                                        new List<string> { "1!1!7", "1!2!9", "1!3!10", "1!4!8" }},
                { 2, isModes == false ? new List<string> { "1!1!9", "1!2!8", "1!3!7", "1!4!10" } :
                                        new List<string> { "1!1!9", "1!2!7", "1!3!10", "1!4!8" }},
                { 3, isModes == false ? new List<string> { "1!1!7", "1!2!10", "1!3!8", "1!4!9" } :
                                        new List<string> { "1!1!10", "1!2!8", "1!3!7", "1!4!9" }},
                { 4, isModes == false ? new List<string> { "1!1!10", "1!2!7", "1!3!8", "1!4!9" } :
                                        new List<string> { "1!1!8", "1!2!10", "1!3!7", "1!4!9" }},
                { 5, isModes == false ? new List<string> { "1!1!8", "1!2!7", "1!3!9", "1!4!10" } :
                                        new List<string> { "1!1!7", "1!2!9", "1!3!8", "1!4!10" }},
                { 6, isModes == false ? new List<string> { "1!1!7", "1!2!8", "1!3!9", "1!4!10" } :
                                        new List<string> { "1!1!9", "1!2!7", "1!3!8", "1!4!10" }},
                { 7, isModes == false ? new List<string> { "1!1!9", "1!2!10", "1!3!8", "1!4!7" } :
                                        new List<string> { "1!1!10", "1!2!8", "1!3!9", "1!4!7" }},
                { 8, isModes == false ? new List<string> { "1!1!10", "1!2!9", "1!3!8", "1!4!7" } :
                                        new List<string> { "1!1!8", "1!2!10", "1!3!9", "1!4!7" }}
            };

            return configurations.ContainsKey(tuner) ? configurations[tuner] : new List<string>();
        }

        private void ConfigureSourceMeasureUnit()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (savedSourceMode == "Current")
            {
                SMU.WriteString($"SOURce:FUNCtion CURRent");
                SMU.WriteString($"SOURce:CURRent:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelimitValue}");

            }
            else
            {
                SMU.WriteString($"SOURce:FUNCtion VOLTage");
                SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelimitValue}");
            }

            if (savedMeasureMode == "Current")
            {
                SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
            }
            else
            {
                SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
            }

            if (savedRsenseMode == "4-Wires")
            {
                SMU.WriteString($"SENSe:{savedMeasureMode}:RSENse ON");
            }
            else
            {
                SMU.WriteString($"SENSe:{savedMeasureMode}:RSENse OFF");
            }
        }

        private async Task ExecuteSweep()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sweepCommand = $"SOURce:SWEep:{savedSourceMode}:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
            Debug.WriteLine($"Sending command: {sweepCommand}");

            SMU.WriteString(sweepCommand);
            SMU.WriteString("OUTPut ON");
            SMU.WriteString("INITiate");
            SMU.WriteString("*WAI");
            SMU.WriteString("OUTPut OFF");
            await Task.Delay(points * repetitionValue * 100);
        }

        private void UpdateMeasurementState()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentRepeat++;
            Debug.WriteLine($"Measuring Tuner {currentTuner}, Repeat Count: {currentRepeat} / {repetitionValue}");


            if (currentRepeat == repetitionValue)
            {
                currentRepeat = 0;
                currentTuner++;
            }
        }

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
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

        private void ButtonData_Click(object sender, EventArgs e)
        {
            try
            {
                var dataChildForm = new MeasurementSettingsDataChildForm();

                dataChildForm.UpdateChartData(new[] { 5, 10, 15, 20 });

                OpenChildForm(dataChildForm);
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

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetition, out double sourcelimit, out double thickness, out double magneticfields, out double delay, out int points)
        {
            start = stop = step = sourcelimit = thickness = magneticfields = delay = 0;
            repetition = points = 1;

            try
            {
                start = ConvertValueBasedOnUnit(ComboboxStartUnit.SelectedItem.ToString(), double.Parse(TextboxStart.Text));
                stop = ConvertValueBasedOnUnit(ComboboxStopUnit.SelectedItem.ToString(), double.Parse(TextboxStop.Text));
                step = ConvertValueBasedOnUnit(ComboboxStepUnit.SelectedItem.ToString(), double.Parse(TextboxStep.Text));
                delay = ConvertValueBasedOnUnit(ComboboxSourceDelayUnit.SelectedItem.ToString(), double.Parse(TextboxSourceDelay.Text));
                sourcelimit = ConvertValueBasedOnUnit(ComboboxSourceLimitUnit.SelectedItem.ToString(), double.Parse(TextboxSourceLimit.Text));
                thickness = ConvertValueBasedOnUnit(ComboboxThicknessUnit.SelectedItem.ToString(), double.Parse(TextboxThickness.Text));
                magneticfields = isModes ? ConvertValueBasedOnUnit(ComboboxMagneticFieldsUnit.SelectedItem.ToString(), double.Parse(TextboxMagneticFields.Text)) : 0;
                repetition = int.Parse(TextboxRepetition.Text);
                points = (int)((stop - start) / step) + 1;


                if (start >= stop || step <= 0 || repetition < 1 || repetition > 999 || thickness < 0 || sourcelimit < 0 || delay < 49E-6 || delay > 10E+3 || points < 1 )
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

        private double ConvertValueBasedOnUnit(string unit, double value)  //  Method สำหรับการแปลงหน่วยของค่าที่ผู้ใช้ป้อนเข้ามา
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
                case "µs":
                    return value * 1E-6;  //แปลงเป็นหน่วย microSecond
                case "ms":
                    return value * 1E-3;  //แปลงเป็นหน่วย milliSecond
                case "s":
                    return value;    //แปลงเป็นหน่วย Second
                case "ks":
                    return value * 1E+3;  //แปลงเป็นหน่วย kiloSecond
                case "G":
                    return value * 1E+4;  //แปลงเป็นหน่วย Gauss
                case "T":
                    return value;  //แปลงเป็นหน่วย Tesla
                default:
                    throw new Exception("Unknown unit");  //ไม่รู้จักหน่วย (Unit Error)
            }
        }

        private void IconbuttonUpdateChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentTunerandDataChildForm is MeasurementSettingsDataChildForm dataChildForm)
                {
                    dataChildForm.UpdateChartData(new[]  { 0 });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
