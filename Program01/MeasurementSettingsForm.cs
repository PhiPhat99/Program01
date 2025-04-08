using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Ivi.Visa.Interop;

namespace Program01
{
    public partial class MeasurementSettingsForm : Form
    {
        // Fields
        public FormattedIO488 SMU;
        public FormattedIO488 SS;
        public ResourceManager Rsrcmngr;

        private int TargetPosition;
        private int CurrentTuner;

        public event EventHandler<bool> ModeChanged;
        public event EventHandler ToggleChanged;

        private Form CurrentChildForm;
        public DataChildForm DataChildForm = null;

        public List<double> XDataBuffer = new List<double>();
        public List<double> YDataBuffer = new List<double>();

        public bool IsOn
        {
            get => GlobalSettings.Instance.IsModes;
            set
            {
                GlobalSettings.Instance.IsModes = value;
                UpdateToggleState();
            }
        }

        public MeasurementSettingsForm()
        {
            InitializeComponent();
            Rsrcmngr = new ResourceManager();
            InitializeGPIB();
            LoadSettings();

            ComboboxRsense.Items.AddRange(new string[] { "2-Wires", "4-Wires" });
            ComboboxMeasure.Items.AddRange(new string[] { "Voltage", "Current" });
            ComboboxSource.Items.AddRange(new string[] { "Voltage", "Current" });
        }

        private void InitializeGPIB()
        {
            try
            {
                ComboboxVISASMUIOPort.Items.Clear();
                ComboboxVISASSIOPort.Items.Clear();

                Dictionary<string, (string Address, string Response)> DeviceResponses = FindConnectedGPIBDevicesWithResponse();

                string SMUModelKeyword = "MODEL 2450";
                string SSModelKeyword = "MODEL 7001";
                string[] SMUAddresses = DeviceResponses
                    .Where(d => d.Value.Response.IndexOf(SMUModelKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(d => d.Value.Address)
                    .ToArray();
                string[] SSAddresses = DeviceResponses
                    .Where(d => d.Value.Response.IndexOf(SSModelKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(d => d.Value.Address)
                    .ToArray();

                Debug.WriteLine($"[DEBUG] Source Measure Unit Addresses: {string.Join(", ", SMUAddresses)}");
                Debug.WriteLine($"[DEBUG] Switch System Addresses: {string.Join(", ", SSAddresses)}");

                UpdateGPIBComboBox(ComboboxVISASMUIOPort, SMUAddresses, ComboboxVISASSIOPort);
                UpdateGPIBComboBox(ComboboxVISASSIOPort, SSAddresses, ComboboxVISASMUIOPort);

                if (SMU == null && SMUAddresses.Length > 0)
                {
                    SMU = new FormattedIO488();
                    SMU.IO = (IMessage)Rsrcmngr.Open(SMUAddresses[0]);
                }

                if (SS == null && SSAddresses.Length > 0)
                {
                    SS = new FormattedIO488();
                    SS.IO = (IMessage)Rsrcmngr.Open(SSAddresses[0]);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถค้นหาอุปกรณ์ GPIB: {Ex.Message}");
            }
        }

        private Dictionary<string, (string Address, string Response)> FindConnectedGPIBDevicesWithResponse()
        {
            Dictionary<string, (string Address, string Response)> ConnectedDevices = new Dictionary<string, (string, string)>();

            try
            {
                string[] AllDevices = Rsrcmngr.FindRsrc("GPIB?*::?*::INSTR");

                foreach (string Device in AllDevices)
                {
                    FormattedIO488 Sessions = new FormattedIO488();

                    try
                    {
                        Sessions.IO = (IMessage)Rsrcmngr.Open(Device);
                        Sessions.WriteString("*IDN?", true);
                        Sessions.IO.Timeout = 3000;
                        string Response = Sessions.ReadString();
                        Response = Response?.Trim() ?? "Unknown Device";

                        if (!string.IsNullOrEmpty(Response))
                        {
                            ConnectedDevices[Device] = (Device, Response);
                            Debug.WriteLine($"[DEBUG] Instruments Model: {Response}");
                            Debug.WriteLine($"[DEBUG] อุปกรณ์ที่ใช้งานอยู่: {Device}");
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine($"[ERROR] อุปกรณ์ {Device} ไม่ตอบสนอง");
                    }
                    finally
                    {
                        if (Sessions.IO != null)
                        {
                            Sessions.IO.Close();
                            Marshal.FinalReleaseComObject(Sessions.IO);
                            Sessions.IO = null;
                        }

                        Marshal.FinalReleaseComObject(Sessions);
                        Sessions = null;
                    }
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถค้นหาอุปกรณ์ GPIB: {Ex.Message}");
            }

            return ConnectedDevices;
        }

        private void UpdateGPIBComboBox(ComboBox Combobox, string[] GpibAddresses, ComboBox OtherCombobox)
        {
            Combobox.Items.Clear();

            if (GpibAddresses.Length == 0)
            {
                Debug.WriteLine($"[WARNING] ไม่มีที่อยู่ GPIB สำหรับ {Combobox.Name}");
                return;
            }

            HashSet<string> OtherAddresses = new HashSet<string>(
                OtherCombobox.Items.Cast<string>().DefaultIfEmpty()
            );

            foreach (string DeviceAddress in GpibAddresses)
            {
                Debug.WriteLine($"[DEBUG] กำลังเพิ่ม {DeviceAddress} ไปที่ {Combobox.Name}");

                if (!OtherAddresses.Contains(DeviceAddress) || Combobox == OtherCombobox)
                {
                    Combobox.Items.Add(DeviceAddress);
                }
            }

            if (Combobox.Items.Count > 0)
            {
                Combobox.SelectedIndex = 0;
            }
        }

        public class MeasurementEventArgs : EventArgs
        {
            public List<double[]> MeasuredValues { get; }

            public MeasurementEventArgs(List<double[]> measuredValues)
            {
                MeasuredValues = measuredValues;
            }
        }

        private void LoadSettings()
        {
            Debug.WriteLine("[LOAD SETTINGS STARTED]");

            try
            {
                Debug.WriteLine($"[LOAD] RsenseMode (From GlobalSettings): {GlobalSettings.Instance.RsenseMode}");
                SetComboBoxSelectedItem(ComboboxRsense, GlobalSettings.Instance.RsenseMode);
                Debug.WriteLine($"[LOAD] RsenseMode (To UI): {ComboboxRsense.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] MeasureMode (From GlobalSettings): {GlobalSettings.Instance.MeasureMode}");
                SetComboBoxSelectedItem(ComboboxMeasure, GlobalSettings.Instance.MeasureMode);
                Debug.WriteLine($"[LOAD] MeasureMode (To UI): {ComboboxMeasure.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] SourceMode (From GlobalSettings): {GlobalSettings.Instance.SourceMode}");
                SetComboBoxSelectedItem(ComboboxSource, GlobalSettings.Instance.SourceMode);
                Debug.WriteLine($"[LOAD] SourceMode (To UI): {ComboboxSource.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] SourceLimitType (From GlobalSettings): {GlobalSettings.Instance.SourceLimitType}");
                SetComboBoxSelectedItem(ComboboxSourceLimitMode, GlobalSettings.Instance.SourceLimitType);
                Debug.WriteLine($"[LOAD] SourceLimitType (To UI): {ComboboxSourceLimitMode.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] StartUnit (From GlobalSettings): {GlobalSettings.Instance.StartUnit}");
                SetComboBoxSelectedItem(ComboboxStartUnit, GlobalSettings.Instance.StartUnit);
                Debug.WriteLine($"[LOAD] StartUnit (To UI): {ComboboxStartUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] StopUnit (From GlobalSettings): {GlobalSettings.Instance.StopUnit}");
                SetComboBoxSelectedItem(ComboboxStopUnit, GlobalSettings.Instance.StopUnit);
                Debug.WriteLine($"[LOAD] StopUnit (To UI): {ComboboxStopUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] StepUnit (From GlobalSettings): {GlobalSettings.Instance.StepUnit}");
                SetComboBoxSelectedItem(ComboboxStepUnit, GlobalSettings.Instance.StepUnit);
                Debug.WriteLine($"[LOAD] StepUnit (To UI): {ComboboxStepUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] SourceDelayUnit (From GlobalSettings): {GlobalSettings.Instance.SourceDelayUnit}");
                SetComboBoxSelectedItem(ComboboxSourceDelayUnit, GlobalSettings.Instance.SourceDelayUnit);
                Debug.WriteLine($"[LOAD] SourceDelayUnit (To UI): {ComboboxSourceDelayUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnit}");
                SetComboBoxSelectedItem(ComboboxSourceLimitLevelUnit, GlobalSettings.Instance.SourceLimitLevelUnit);
                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (To UI): {ComboboxSourceLimitLevelUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] ThicknessUnit (From GlobalSettings): {GlobalSettings.Instance.ThicknessUnit}");
                SetComboBoxSelectedItem(ComboboxThicknessUnit, GlobalSettings.Instance.ThicknessUnit);
                Debug.WriteLine($"[LOAD] ThicknessUnit (To UI): {ComboboxThicknessUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] MagneticFieldsUnit (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnit}");
                SetComboBoxSelectedItem(ComboboxMagneticFieldsUnit, GlobalSettings.Instance.MagneticFieldsUnit);
                Debug.WriteLine($"[LOAD] MagneticFieldsUnit (To UI): {ComboboxMagneticFieldsUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] StartValue (From GlobalSettings): {GlobalSettings.Instance.StartValue}");
                SetTextboxValue(TextboxStart, GlobalSettings.Instance.StartValue);
                Debug.WriteLine($"[LOAD] StartValue (To UI): {TextboxStart.Text}");

                Debug.WriteLine($"[LOAD] StopValue (From GlobalSettings): {GlobalSettings.Instance.StopValue}");
                SetTextboxValue(TextboxStop, GlobalSettings.Instance.StopValue);
                Debug.WriteLine($"[LOAD] StopValue (To UI): {TextboxStop.Text}");

                Debug.WriteLine($"[LOAD] StepValue (From GlobalSettings): {GlobalSettings.Instance.StepValue}");
                SetTextboxValue(TextboxStep, GlobalSettings.Instance.StepValue);
                Debug.WriteLine($"[LOAD] StepValue (To UI): {TextboxStep.Text}");

                Debug.WriteLine($"[LOAD] SourceDelayValue (From GlobalSettings): {GlobalSettings.Instance.SourceDelayValue}");
                SetTextboxValue(TextboxSourceDelay, GlobalSettings.Instance.SourceDelayValue);
                Debug.WriteLine($"[LOAD] SourceDelayValue (To UI): {TextboxSourceDelay.Text}");

                Debug.WriteLine($"[LOAD] SourceLimitLevelValue (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelValue}");
                SetTextboxValue(TextboxSourceLimitLevel, GlobalSettings.Instance.SourceLimitLevelValue);
                Debug.WriteLine($"[LOAD] SourceLimitLevelValue (To UI): {TextboxSourceLimitLevel.Text}");

                Debug.WriteLine($"[LOAD] ThicknessValue (From GlobalSettings): {GlobalSettings.Instance.ThicknessValue}");
                SetTextboxValue(TextboxThickness, GlobalSettings.Instance.ThicknessValue);
                Debug.WriteLine($"[LOAD] ThicknessValue (To UI): {TextboxThickness.Text}");

                Debug.WriteLine($"[LOAD] RepetitionValue (From GlobalSettings): {GlobalSettings.Instance.RepetitionValue}");
                SetTextboxValue(TextboxRepetition, GlobalSettings.Instance.RepetitionValue);
                Debug.WriteLine($"[LOAD] RepetitionValue (To UI): {TextboxRepetition.Text}");
                
                if (GlobalSettings.Instance.IsModes)
                {
                    Debug.WriteLine($"[LOAD] MagneticFieldsValue (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsValue}");
                    SetTextboxValue(TextboxMagneticFields, GlobalSettings.Instance.MagneticFieldsValue);
                    Debug.WriteLine($"[LOAD] MagneticFieldsValue (To UI): {TextboxMagneticFields.Text}");

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error loading settings: {Ex.Message}", "ERROR");
                Debug.WriteLine($"[ERROR - LOAD SETTINGS]: {Ex.Message}");
                Debug.WriteLine($"[ERROR - LOAD SETTINGS - STACK TRACE]: {Ex.StackTrace}");
            }
            finally
            {
                Debug.WriteLine("[LOAD SETTINGS FINISHED]");
            }
        }

        private void SetComboBoxSelectedItem(ComboBox combobox, string value)
        {
            if (combobox.Items.Contains(value))
            {
                combobox.SelectedItem = value;
            }
            else
            {
                combobox.SelectedIndex = -1;
            }
        }

        private void SetTextboxValue(TextBox textbox, string value)
        {
            textbox.Text = value ?? "";
        }

        private void MeasurementSettingsChildForm_Load(object sender, EventArgs e)
        {
            UpdateUIAfterConnection("Restoring SMU Connection Status", GlobalSettings.Instance.IsSMUConnected, IconbuttonSMUConnection);
            UpdateUIAfterConnection("Restoring SS Connection Status", GlobalSettings.Instance.IsSSConnected, IconbuttonSSConnection);
            UpdateMeasurementMode();
            UpdateToggleState();
            LoadSettings();

            ComboboxRsense.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxMeasure.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSource.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitMode.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            TextboxStart.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxStop.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxStep.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxSourceDelay.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxSourceLimitLevel.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxRepetition.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxThickness.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxMagneticFields.TextChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxStartUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxStopUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxStepUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSourceDelayUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitLevelUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxThicknessUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxMagneticFieldsUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
        }

        private void SaveToGlobalSettings()
        {
            Debug.WriteLine("[SAVE SETTINGS STARTED]");

            try
            {
                Debug.WriteLine($"[SAVE] RsenseMode (From UI): {ComboboxRsense.SelectedItem?.ToString()}");
                GlobalSettings.Instance.RsenseMode = ComboboxRsense.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] RsenseMode (To GlobalSettings): {GlobalSettings.Instance.RsenseMode}");

                Debug.WriteLine($"[SAVE] MeasureMode (From UI): {ComboboxMeasure.SelectedItem?.ToString()}");
                GlobalSettings.Instance.MeasureMode = ComboboxMeasure.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MeasureMode (To GlobalSettings): {GlobalSettings.Instance.MeasureMode}");

                Debug.WriteLine($"[SAVE] SourceMode (From UI): {ComboboxSource.SelectedItem?.ToString()}");
                GlobalSettings.Instance.SourceMode = ComboboxSource.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceMode (To GlobalSettings): {GlobalSettings.Instance.SourceMode}");

                Debug.WriteLine($"[SAVE] SourceLimitType (From UI): {ComboboxSourceLimitMode.SelectedItem?.ToString()}");
                GlobalSettings.Instance.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitType (To GlobalSettings): {GlobalSettings.Instance.SourceLimitType}");

                Debug.WriteLine($"[SAVE] StartUnit (From UI): {ComboboxStartUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.StartUnit = ComboboxStartUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StartUnit (To GlobalSettings): {GlobalSettings.Instance.StartUnit}");

                Debug.WriteLine($"[SAVE] StopUnit (From UI): {ComboboxStopUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.StopUnit = ComboboxStopUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StopUnit (To GlobalSettings): {GlobalSettings.Instance.StopUnit}");

                Debug.WriteLine($"[SAVE] StepUnit (From UI): {ComboboxStepUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.StepUnit = ComboboxStepUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StepUnit (To GlobalSettings): {GlobalSettings.Instance.StepUnit}");

                Debug.WriteLine($"[SAVE] SourceDelayUnit (From UI): {ComboboxSourceDelayUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.SourceDelayUnit = ComboboxSourceDelayUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceDelayUnit (To GlobalSettings): {GlobalSettings.Instance.SourceDelayUnit}");

                Debug.WriteLine($"[SAVE] SourceLimitLevelUnit (From UI): {ComboboxSourceLimitLevelUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.SourceLimitLevelUnit = ComboboxSourceLimitLevelUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitLevelUnit (To GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnit}");

                Debug.WriteLine($"[SAVE] ThicknessUnit (From UI): {ComboboxThicknessUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.ThicknessUnit = ComboboxThicknessUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] ThicknessUnit (To GlobalSettings): {GlobalSettings.Instance.ThicknessUnit}");

                Debug.WriteLine($"[SAVE] MagneticFieldsUnit (From UI): {ComboboxMagneticFieldsUnit.SelectedItem?.ToString()}");
                GlobalSettings.Instance.MagneticFieldsUnit = ComboboxMagneticFieldsUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MagneticFieldsUnit (To GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnit}");

                Debug.WriteLine($"[SAVE] StartValue (From UI): {TextboxStart.Text}");
                GlobalSettings.Instance.StartValue = TextboxStart.Text;
                Debug.WriteLine($"[SAVE] StartValue (To GlobalSettings): {GlobalSettings.Instance.StartValue}");

                Debug.WriteLine($"[SAVE] StopValue (From UI): {TextboxStop.Text}");
                GlobalSettings.Instance.StopValue = TextboxStop.Text;
                Debug.WriteLine($"[SAVE] StopValue (To GlobalSettings): {GlobalSettings.Instance.StopValue}");

                Debug.WriteLine($"[SAVE] StepValue (From UI): {TextboxStep.Text}");
                GlobalSettings.Instance.StepValue = TextboxStep.Text;
                Debug.WriteLine($"[SAVE] StepValue (To GlobalSettings): {GlobalSettings.Instance.StepValue}");

                Debug.WriteLine($"[SAVE] SourceDelayValue (From UI): {TextboxSourceDelay.Text}");
                GlobalSettings.Instance.SourceDelayValue = TextboxSourceDelay.Text;
                Debug.WriteLine($"[SAVE] SourceDelayValue (To GlobalSettings): {GlobalSettings.Instance.SourceDelayValue}");

                Debug.WriteLine($"[SAVE] SourceLimitLevelValue (From UI): {TextboxSourceLimitLevel.Text}");
                GlobalSettings.Instance.SourceLimitLevelValue = TextboxSourceLimitLevel.Text;
                Debug.WriteLine($"[SAVE] SourceLimitLevelValue (To GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelValue}");

                Debug.WriteLine($"[SAVE] ThicknessValue (From UI): {TextboxThickness.Text}");
                GlobalSettings.Instance.ThicknessValue = TextboxThickness.Text;
                Debug.WriteLine($"[SAVE] ThicknessValue (To GlobalSettings): {GlobalSettings.Instance.ThicknessValue}");

                Debug.WriteLine($"[SAVE] RepetitionValue (From UI): {TextboxRepetition.Text}");
                GlobalSettings.Instance.RepetitionValue = TextboxRepetition.Text;
                Debug.WriteLine($"[SAVE] RepetitionValue (To GlobalSettings): {GlobalSettings.Instance.RepetitionValue}");

                if (GlobalSettings.Instance.IsModes)
                {
                    Debug.WriteLine($"[SAVE] MagneticFieldsValue (From UI): {TextboxMagneticFields.Text}");
                    GlobalSettings.Instance.MagneticFieldsValue = TextboxMagneticFields.Text;
                    Debug.WriteLine($"[SAVE] MagneticFieldsValue (To GlobalSettings): {GlobalSettings.Instance.MagneticFieldsValue}");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error saving settings: {Ex.Message}", "ERROR");
                Debug.WriteLine($"[ERROR - SAVE SETTINGS]: {Ex.Message}");
                Debug.WriteLine($"[ERROR - SAVE SETTINGS - STACK TRACE]: {Ex.StackTrace}");
            }
            Debug.WriteLine("[SAVE SETTINGS FINISHED]");
        }

        private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveToGlobalSettings();
            GlobalSettings.Instance.OnSettingsChanged -= LoadSettings;
        }

        private bool ConnectDevice(ref FormattedIO488 Devices, string Ports)
        {
            try
            {
                if (Devices != null && Devices.IO != null)
                {
                    Devices.IO.Close();
                    Marshal.FinalReleaseComObject(Devices.IO);
                }

                Devices = new FormattedIO488();
                Devices.IO = (IMessage)Rsrcmngr.Open(Ports);
                Devices.IO.Timeout = 5000;

                Devices.WriteString("*IDN?");
                string ConnectedResponse = Devices.ReadString();
                Debug.WriteLine($"Device Connected: {ConnectedResponse}");

                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error connecting to device: {Ex.Message}", "Connection Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private bool DisconnectDevice(ref FormattedIO488 Device)
        {
            try
            {
                if (Device != null)
                {
                    if (Device.IO != null)
                    {
                        Device.WriteString("*CLS");
                        Device.IO.Close();
                        Marshal.FinalReleaseComObject(Device.IO);
                        Device.IO = null;
                    }
                    Marshal.FinalReleaseComObject(Device);
                    Device = null;
                }
                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error disconnecting device: {Ex.Message}", "Disconnection Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private void UpdateUIAfterConnection(string Message, bool IsConnected, IconButton Buttons)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateUIAfterConnection(Message, IsConnected, Buttons)));
                return;
            }

            Debug.WriteLine(IsConnected ? "[INFORMATION]" : "[ERROR]");
            Debug.WriteLine(Message);

            Buttons.BackColor = IsConnected ? Color.Gray : Color.Snow;
            Buttons.IconColor = IsConnected ? Color.GreenYellow : Color.Gainsboro;

            Debug.WriteLine(Buttons == IconbuttonSMUConnection ? $"SMU Connected: {GlobalSettings.Instance.IsSMUConnected}" : $"SS Connected: {GlobalSettings.Instance.IsSSConnected}");
        }

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool Success = false;

                if (!GlobalSettings.Instance.IsSMUConnected)
                {
                    if (SMU == null)
                    {
                        SMU = new FormattedIO488();
                    }

                    Success = ConnectDevice(ref SMU, ComboboxVISASMUIOPort.SelectedItem?.ToString());
                    
                    if (Success)
                    {
                        GlobalSettings.Instance.IsSMUConnected = true;
                    }
                }
                else
                {
                    Success = DisconnectDevice(ref SMU);

                    if (Success)
                    {
                        GlobalSettings.Instance.IsSMUConnected = false;
                        SMU = null;
                    }
                }

                UpdateUIAfterConnection(GlobalSettings.Instance.IsSMUConnected ? "Connected to Source Measure Unit" : "Disconnected from Source Measure Unit", GlobalSettings.Instance.IsSMUConnected, IconbuttonSMUConnection);
                MessageBox.Show(GlobalSettings.Instance.IsSMUConnected ? "Connected to Source Measure Unit" : "Disconnected from Source Measure Unit", "Informationc");
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}", "Connection Error", MessageBoxButtons.OK);
            }
        }

        private void IconbuttonSSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool Success = false;

                if (!GlobalSettings.Instance.IsSSConnected)
                {
                    if (SS == null)
                    {
                        SS = new FormattedIO488();
                    }

                    Success = ConnectDevice(ref SS, ComboboxVISASSIOPort.SelectedItem?.ToString());

                    if (Success)
                    {
                        GlobalSettings.Instance.IsSSConnected = true;
                    }
                }
                else
                {
                    Success = DisconnectDevice(ref SS);
                    if (Success)
                    {
                        GlobalSettings.Instance.IsSSConnected = false;
                        SS = null;
                    }
                }

                UpdateUIAfterConnection(GlobalSettings.Instance.IsSSConnected ? "Connected to Switch System" : "Disconnected from Switch System", GlobalSettings.Instance.IsSSConnected, IconbuttonSSConnection);
                MessageBox.Show(GlobalSettings.Instance.IsSSConnected ? "Connected to Switch System" : "Disconnected from Switch System", "Information");
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}", "Connection Error", MessageBoxButtons.OK);
            }
        }

        private void ComboboxRsense_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.Instance.RsenseMode = ComboboxRsense.SelectedItem?.ToString();

                if (GlobalSettings.Instance.MeasureMode == "Voltage")
                {
                    switch (GlobalSettings.Instance.RsenseMode)
                    {
                        case "2-Wires":
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            GlobalSettings.Instance.RsenseMode = "";
                            break;
                    }
                }

                else
                {
                    switch (GlobalSettings.Instance.RsenseMode)
                    {
                        case "2-Wires":
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            GlobalSettings.Instance.RsenseMode = "";
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxMeasure_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.Instance.MeasureMode = ComboboxMeasure.SelectedItem?.ToString();

                switch (GlobalSettings.Instance.MeasureMode)
                {
                    case "Voltage":
                        break;
                    case "Current":
                        break;
                    default:
                        ComboboxMeasure.SelectedIndex = -1;
                        GlobalSettings.Instance.MeasureMode = "";
                        break;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.Instance.SourceMode = ComboboxSource.SelectedItem?.ToString();

                switch (GlobalSettings.Instance.SourceMode)
                {
                    case "Voltage":
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        UpdateMeasurementSettingsUnits();
                        break;
                    default:
                        ComboboxSource.SelectedIndex = -1;
                        GlobalSettings.Instance.SourceMode = "";
                        break;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxSourceLimitMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.Instance.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void UpdateMeasurementSettingsUnits()
        {
            try
            {
                if (GlobalSettings.Instance.SourceMode == "Voltage")
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

                    ComboboxSourceLimitLevelUnit.Items.Clear();
                    ComboboxSourceLimitLevelUnit.Items.AddRange(new string[] { "nA", "µA", "mA", "A" });
                    ComboboxSourceLimitLevelUnit.SelectedIndex = 0;
                }
                else if (GlobalSettings.Instance.SourceMode == "Current")
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

                UpdateOtherUnits();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void UpdateOtherUnits()
        {
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

        private void IconbuttonClearSettings_Click(object sender, EventArgs e)
        {
            ClearSettings();
        }

        private void ClearSettings()
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("*CLS");
                SMU.WriteString("*RST");
                SS.WriteString("*CLS");
                SS.WriteString("ROUTe:OPEN ALL");

                ComboboxRsense.SelectedIndex = -1;
                ComboboxMeasure.SelectedIndex = -1;
                ComboboxSource.SelectedIndex = -1;
                ComboboxSourceLimitMode.SelectedIndex = -1;
                ComboboxStartUnit.SelectedIndex = -1;
                ComboboxStopUnit.SelectedIndex = -1;
                ComboboxStepUnit.SelectedIndex = -1;
                ComboboxSourceDelayUnit.SelectedIndex = -1;
                ComboboxSourceLimitLevelUnit.SelectedIndex = -1;
                ComboboxThicknessUnit.SelectedIndex = -1;
                ComboboxMagneticFieldsUnit.SelectedIndex = -1;

                TextboxStart.Clear();
                TextboxStep.Clear();
                TextboxStop.Clear();
                TextboxSourceDelay.Clear();
                TextboxSourceLimitLevel.Clear();
                TextboxThickness.Clear();
                TextboxRepetition.Clear();
                TextboxMagneticFields.Clear();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        public void PanelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                GlobalSettings.Instance.IsModes = !GlobalSettings.Instance.IsModes;
                GlobalSettings.Instance.IsHallMode = GlobalSettings.Instance.IsModes;
                GlobalSettings.Instance.IsVanDerPauwMode = !GlobalSettings.Instance.IsModes;

                UpdateToggleState();
                UpdateMeasurementMode();
                OnToggleChanged();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        public void UpdateToggleState()
        {
            TargetPosition = GlobalSettings.Instance.IsModes ? PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width - 1 : 1;
            PanelToggleSwitchButton.Location = new Point(TargetPosition, PanelToggleSwitchButton.Location.Y);

            if (PanelToggleSwitchButton.Location.X < 0 || PanelToggleSwitchButton.Location.X > PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width)
            {
                PanelToggleSwitchButton.Location = new Point(1, PanelToggleSwitchButton.Location.Y);
            }
        }

        private void UpdateMeasurementMode()
        {
            bool isHallMode = GlobalSettings.Instance.IsHallMode;
            string ModeName = isHallMode ? "Hall effect" : "Van der Pauw";
            Debug.WriteLine($"You select: {ModeName} measurement");

            TextboxMagneticFields.Enabled = isHallMode;
            TextboxMagneticFields.Visible = isHallMode;
            ComboboxMagneticFieldsUnit.Visible = isHallMode;
            LabelMagneticFields.Visible = isHallMode;
            LabelMagneticFieldsUnit.Visible = isHallMode;

            LabelToggleSwitchVdP.ForeColor = isHallMode ? System.Drawing.SystemColors.ActiveCaptionText : Color.FromArgb(144, 198, 101);
            LabelToggleSwitchHall.ForeColor = isHallMode ? Color.FromArgb(144, 198, 101) : System.Drawing.SystemColors.ActiveCaptionText;
            PanelToggleSwitchButton.BackColor = isHallMode ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
            PanelToggleSwitchBase.BackColor = isHallMode ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);

            UpdateTunerImages(isHallMode);
        }

        private void UpdateTunerImages(bool isHallMode)
        {
            if (isHallMode)
            {
                PictureboxTuner1.Image = Properties.Resources.V_1_Hall;
                PictureboxTuner2.Image = Properties.Resources.V_2_Hall;
                PictureboxTuner3.Image = Properties.Resources.V_3_Hall;
                PictureboxTuner4.Image = Properties.Resources.V_4_Hall;
                PictureboxTuner5.Image = Properties.Resources.V_5_Hall;
                PictureboxTuner6.Image = Properties.Resources.V_6_Hall;
                PictureboxTuner7.Image = Properties.Resources.V_7_Hall;
                PictureboxTuner8.Image = Properties.Resources.V_8_Hall;
            }
            else
            {
                PictureboxTuner1.Image = Properties.Resources.R_A1_VdP;
                PictureboxTuner2.Image = Properties.Resources.R_A2_VdP;
                PictureboxTuner3.Image = Properties.Resources.R_A3_VdP;
                PictureboxTuner4.Image = Properties.Resources.R_A4_VdP;
                PictureboxTuner5.Image = Properties.Resources.R_B1_VdP;
                PictureboxTuner6.Image = Properties.Resources.R_B2_VdP;
                PictureboxTuner7.Image = Properties.Resources.R_B3_VdP;
                PictureboxTuner8.Image = Properties.Resources.R_B4_VdP;
            }
        }

        protected virtual void OnToggleChanged()
        {
            ToggleChanged?.Invoke(this, EventArgs.Empty);
            ModeChanged?.Invoke(this, GlobalSettings.Instance.IsModes);

            PanelToggleSwitchBase.BackColor = GlobalSettings.Instance.IsModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void PictureboxTuner1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!4)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!4)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner5_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner6_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void PictureboxTuner8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (GlobalSettings.Instance.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonTunerTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SaveToGlobalSettings();

                SMU.IO.Timeout = 1000000;
                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "WARNING", MessageBoxButtons.OK);
                    return;
                }

                if (repetitionValue > 3)
                {
                    MessageBox.Show("Cannot set the repetition value greater than 3 in Tuner test", "WARNING", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.Instance.IsModes == true)
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceMode == "Voltage" && GlobalSettings.Instance.MeasureMode == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.Instance.IsModes == true)
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.Instance.IsModes == true)
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceMode == "Current" && GlobalSettings.Instance.MeasureMode == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue},  {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.Instance.IsModes == true)
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                SaveToGlobalSettings();
                RunMeasurement();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private async void RunMeasurement()
        {
            try
            {
                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                CurrentTuner = 1;
                CollectAndCalculateVdPMeasured.Instance.ClearAllData();

                while (CurrentTuner <= 8)
                {
                    ConfigureSwitchSystem();
                    await Task.Delay(600);
                    UpdateMeasurementState();
                    ConfigureSourceMeasureUnit();
                    await ExecuteSweep();
                    TracingRunMeasurement();
                    await Task.Delay(1000);

                    CurrentTuner++;

                    if (CurrentTuner > 8)
                    {
                        Debug.WriteLine("[DEBUG] All tuners completed");
                        MessageBox.Show("Measurement completed", "Measurement Successfully", MessageBoxButtons.OK);
                        SMU.WriteString("OUTPut OFF");
                        SS.WriteString("*CLS");

                        if (Application.OpenForms.OfType<VdPTotalMeasureValuesForm>().FirstOrDefault() is VdPTotalMeasureValuesForm VdPTotalForm)
                        {
                            VdPTotalForm.Invoke((MethodInvoker)delegate { VdPTotalForm.LoadMeasurementData(); });
                        }

                        break;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ConfigureSwitchSystem()
        {
            SS.WriteString("ROUTe:OPEN ALL");
            var channels = GetChannelConfiguration(CurrentTuner, GlobalSettings.Instance.IsModes);

            foreach (var channel in channels)
            {
                SS.WriteString($"ROUTe:CLOSe (@ {channel})");
            }
        }

        private List<string> GetChannelConfiguration(int Tuner, bool IsModes)
        {
            var configurations = new Dictionary<int, List<string>>
    {
        { 1, IsModes == false ? new List<string> { "1!1!4", "1!2!5", "1!3!3", "1!4!6" } :
                        new List<string> { "1!1!3", "1!2!5", "1!3!6", "1!4!4" }},
        { 2, IsModes == false ? new List<string> { "1!1!5", "1!2!4", "1!3!3", "1!4!6" } :
                        new List<string> { "1!1!5", "1!2!3", "1!3!6", "1!4!4" }},
        { 3, IsModes == false ? new List<string> { "1!1!3", "1!2!6", "1!3!4", "1!4!5" } :
                        new List<string> { "1!1!6", "1!2!4", "1!3!3", "1!4!5" }},
        { 4, IsModes == false ? new List<string> { "1!1!6", "1!2!3", "1!3!4", "1!4!5" } :
                        new List<string> { "1!1!4", "1!2!6", "1!3!3", "1!4!5" }},
        { 5, IsModes == false ? new List<string> { "1!1!4", "1!2!3", "1!3!5", "1!4!6" } :
                        new List<string> { "1!1!3", "1!2!5", "1!3!4", "1!4!6" }},
        { 6, IsModes == false ? new List<string> { "1!1!3", "1!2!4", "1!3!5", "1!4!6" } :
                        new List<string> { "1!1!5", "1!2!3", "1!3!4", "1!4!6" }},
        { 7, IsModes == false ? new List<string> { "1!1!5", "1!2!6", "1!3!4", "1!4!3" } :
                        new List<string> { "1!1!6", "1!2!4", "1!3!5", "1!4!3" }},
        { 8, IsModes == false ? new List<string> { "1!1!6", "1!2!5", "1!3!4", "1!4!3" } :
                        new List<string> { "1!1!4", "1!2!6", "1!3!5", "1!4!3" }}
    };

            Debug.WriteLine($"Tuner: {Tuner}, IsModes: {IsModes}");

            foreach (var channel in configurations[Tuner])
            {
                Debug.WriteLine($"Channel: {channel}");
            }

            return configurations.ContainsKey(Tuner) ? configurations[Tuner] : new List<string>();
        }

        private void ConfigureSourceMeasureUnit()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                return;
            }

            SMU.IO.Timeout = 1000000;
            SMU.WriteString("TRACe:CLEar");
            SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

            if (GlobalSettings.Instance.SourceMode == "Current")
            {
                SMU.WriteString($"SOURce:FUNCtion CURRent");
                SMU.WriteString($"SOURce:CURRent:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
            }
            else
            {
                SMU.WriteString($"SOURce:FUNCtion VOLTage");
                SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
            }

            if (GlobalSettings.Instance.MeasureMode == "Current")
            {
                SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
            }
            else
            {
                SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
            }

            if (GlobalSettings.Instance.RsenseMode == "4-Wires")
            {
                SMU.WriteString($"SENSe:{GlobalSettings.Instance.MeasureMode}:RSENse ON");
            }
            else
            {
                SMU.WriteString($"SENSe:{GlobalSettings.Instance.MeasureMode}:RSENse OFF");
            }
        }

        private async Task ExecuteSweep()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                return;
            }

            if (GlobalSettings.Instance.IsModes == true)
            {
                string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                Debug.WriteLine($"{allValues}.");
            }
            else
            {
                string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                Debug.WriteLine($"{allValues}.");
            }

            if (GlobalSettings.Instance.SourceMode == "Current")
            {
                string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                SMU.WriteString(sweepCommand);
                Debug.WriteLine($"Sending command: {sweepCommand}");
            }
            else
            {
                string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                SMU.WriteString(sweepCommand);
                Debug.WriteLine($"Sending command: {sweepCommand}");
            }

            SMU.WriteString("OUTPut ON");
            SMU.WriteString("INITiate");
            SMU.WriteString("*WAI");
            SMU.WriteString("OUTPut OFF");
            await Task.Delay(points * repetitionValue * (int)delayValue * 300);
        }

        private void UpdateMeasurementState()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK);
                return;
            }

            Debug.WriteLine($"Measuring Tuner {CurrentTuner}");
        }

        private void TracingRunMeasurement()
        {
            try
            {
                SMU.WriteString("TRACe:ACTual?");
                string BufferCount = SMU.ReadString().Trim();
                Debug.WriteLine($"Buffer count: {BufferCount}");

                if (!int.TryParse(BufferCount, out int BufferPoints) || BufferPoints == 0)
                {
                    MessageBox.Show("No data in buffer!", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {BufferPoints}, 'defbuffer1', SOURce, READing");
                string RawData = SMU.ReadString().Trim();
                Debug.WriteLine($"Buffer contains: {BufferPoints} readings");
                Debug.WriteLine($"Raw Data: {RawData}");

                string[] DataPairs = RawData.Split(',');
                List<(double Source, double Reading)> currentMeasurements = new List<(double, double)>();
                List<double> XData = new List<double>();
                List<double> YData = new List<double>();
                Debug.WriteLine($"Number of data pairs: {DataPairs.Length}");

                if (DataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("Invalid buffer data format!", "Error", MessageBoxButtons.OK);
                    return;
                }

                for (int i = 0; i < DataPairs.Length; i += 2)
                {
                    if (double.TryParse(DataPairs[i], out double SourceValue) && double.TryParse(DataPairs[i + 1], out double MeasuredValue))
                    {
                        XData.Add(SourceValue);
                        YData.Add(MeasuredValue);
                        currentMeasurements.Add((SourceValue, MeasuredValue));
                    }
                }

                if (GlobalSettings.Instance.IsModes)
                {
                    
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Tuner: {CurrentTuner}, Data Points Read: {currentMeasurements.Count}");
                    GlobalSettings.Instance.CollectedMeasurements.StoreMeasurementData(CurrentTuner, currentMeasurements);

                    List<double[]> measuredValues = YData.Zip(XData, (y, x) => new double[] { y, x }).ToList();
                    GlobalSettings.Instance.AddMeasuredValues(measuredValues, CurrentTuner);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("SYSTem:ERRor?");
                SS.WriteString("SYSTem:ERRor?");
                string SMUrespones = SMU.ReadString();
                string SSresponses = SS.ReadString();
                Debug.WriteLine($"There is Source Measure Unit error : {SMUrespones}");
                Debug.WriteLine($"There is Switch System error : {SSresponses}");
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void OpenChildForm(Form ChildForm)
        {
            try
            {
                if (CurrentChildForm != null && CurrentChildForm != ChildForm)
                {
                    CurrentChildForm.Hide();
                }

                CurrentChildForm = ChildForm;
                ChildForm.TopLevel = false;
                ChildForm.FormBorderStyle = FormBorderStyle.None;
                ChildForm.Dock = DockStyle.Fill;
                PanelTunerandData.Controls.Add(ChildForm);
                PanelTunerandData.Tag = ChildForm;
                ChildForm.BringToFront();
                ChildForm.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ButtonData_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataChildForm == null || DataChildForm.IsDisposed)
                {
                    DataChildForm = new DataChildForm();
                    OpenChildForm(DataChildForm);
                }
                else
                {
                    if (!DataChildForm.Visible)
                    {
                        DataChildForm.Show();
                    }
                }

                if (GlobalSettings.Instance.XDataBuffer.Count > 0 && GlobalSettings.Instance.YDataBuffer.Count > 0)
                {
                    DataChildForm.UpdateChart(GlobalSettings.Instance.XDataBuffer, GlobalSettings.Instance.YDataBuffer);
                    DataChildForm.UpdateMeasurementData(GlobalSettings.Instance.MaxMeasure, GlobalSettings.Instance.MinMeasure, GlobalSettings.Instance.MaxSource, GlobalSettings.Instance.MinSource, GlobalSettings.Instance.Slope);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ButtonTuner_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentChildForm?.Hide();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetition, out double sourcelevellimit, out double thickness, out double magneticfields, out double delay, out int points)
        {
            start = stop = step = sourcelevellimit = thickness = magneticfields = delay = 0;
            repetition = points = 1;

            try
            {
                if (!IsValidNumber(TextboxStart.Text, ComboboxStartUnit.SelectedItem, out start) ||
                    !IsValidNumber(TextboxStop.Text, ComboboxStopUnit.SelectedItem, out stop) ||
                    !IsValidNumber(TextboxStep.Text, ComboboxStepUnit.SelectedItem, out step) ||
                    !IsValidNumber(TextboxSourceDelay.Text, ComboboxSourceDelayUnit.SelectedItem, out delay) ||
                    !IsValidNumber(TextboxSourceLimitLevel.Text, ComboboxSourceLimitLevelUnit.SelectedItem, out sourcelevellimit) ||
                    !IsValidNumber(TextboxThickness.Text, ComboboxThicknessUnit.SelectedItem, out thickness))
                {
                    return false;
                }

                if (GlobalSettings.Instance.IsModes)
                {
                    if (!IsValidNumber(TextboxMagneticFields.Text, ComboboxMagneticFieldsUnit.SelectedItem, out magneticfields))
                    {
                        return false;
                    }

                    if (magneticfields <= 0)
                    {
                        return false;
                    }
                }

                if (!int.TryParse(TextboxRepetition.Text, out repetition) || repetition < 1)
                {
                    return false;
                }

                points = (int)((stop - start) / step) + 1;

                if (start >= stop || step <= 0 || repetition < 1 || repetition > 999 || thickness < 0 || sourcelevellimit < 0 ||
                    delay < 49E-6 || delay > 10E+3 || points < 1 || step >= stop)
                {
                    return false;
                }

                if (string.Equals(GlobalSettings.Instance.SourceLimitType, "Current", StringComparison.OrdinalIgnoreCase) &&
                    (sourcelevellimit > 1.05 || sourcelevellimit < -1.05))
                {
                    return false;
                }

                if (string.Equals(GlobalSettings.Instance.SourceLimitType, "Voltage", StringComparison.OrdinalIgnoreCase) &&
                    (sourcelevellimit > 210 || sourcelevellimit < -210))
                {
                    return false;
                }

                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Validation Error: {Ex.Message}", "Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private bool IsValidNumber(string textValue, object unit, out double result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(textValue) || unit == null)
            {
                return false;
            }

            // ตรวจสอบให้แน่ใจว่าค่าที่ป้อนเป็นตัวเลข
            if (double.TryParse(textValue, out double value))
            {
                result = ConvertValueBasedOnUnit(unit.ToString(), value);
                return true;
            }

            return false;
        }

        private double ConvertValueBasedOnUnit(string unit, double value)  //  Method สำหรับการแปลงหน่วยของค่าที่ป้อนมาใน Textbox ผ่านการเลือก Combobox
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

        private bool ValidateInputConvert(string textValue, object unit, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(textValue) || unit == null)
            {
                return false;
            }

            if (double.TryParse(textValue, out double value))
            {
                result = ConvertValueBasedOnUnit(unit.ToString(), value);
                return true;
            }

            return false;
        }

        private void TracingTunerData()
        {
            try
            {
                List<double> xData = new List<double>();
                List<double> yData = new List<double>();
                double maxMeasure = double.NegativeInfinity;
                double minMeasure = double.PositiveInfinity;
                double maxSource = double.NegativeInfinity;
                double minSource = double.PositiveInfinity;
                double slope = double.NaN;

                SMU.WriteString("TRACe:ACTual?");
                string bufferCount = SMU.ReadString()?.Trim();

                if (!int.TryParse(bufferCount, out int bufferPoints) || bufferPoints == 0)
                {
                    MessageBox.Show("No data in buffer!", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {bufferPoints}, 'defbuffer1', SOURce, READing");
                string measureRawData = SMU.ReadString()?.Trim();
                string[] dataPairs = measureRawData.Split(',');
                Debug.WriteLine($" There are {bufferCount} readings is: {measureRawData}");

                if (dataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("Invalid buffer data format!", "Error", MessageBoxButtons.OK);
                    return;
                }

                for (int i = 0; i < dataPairs.Length; i += 2)
                {
                    if (double.TryParse(dataPairs[i], out double sourceValue) &&
                        double.TryParse(dataPairs[i + 1], out double measuredValue))
                    {
                        xData.Add(sourceValue);
                        yData.Add(measuredValue);

                        Debug.Write($" Source Values: {sourceValue}" + Environment.NewLine);
                        Debug.Write($"Measured Values: {measuredValue}" + Environment.NewLine);

                        maxSource = Math.Max(maxSource, sourceValue);
                        minSource = Math.Min(minSource, sourceValue);
                        maxMeasure = Math.Max(maxMeasure, measuredValue);
                        minMeasure = Math.Min(minMeasure, measuredValue);
                    }
                }

                if (maxSource != minSource)
                {
                    slope = (maxMeasure - minMeasure) / (maxSource - minSource);
                }

                Debug.Write($"Minimum Source Values: {minSource}" + Environment.NewLine);
                Debug.Write($"Maximum Source Values: {maxSource}" + Environment.NewLine);

                Debug.Write($"Minimum Measured Values: {minMeasure}" + Environment.NewLine);
                Debug.Write($"Maximum Measured Values: {maxMeasure}" + Environment.NewLine);

                Debug.Write($"Slope Values: {slope}" + Environment.NewLine);

                GlobalSettings.Instance.UpdateDataBuffer(xData, yData, maxMeasure, minMeasure, maxSource, minSource, slope);
                UpdateDataChildForm();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void UpdateDataChildForm()
        {
            var dataChildForm = Application.OpenForms.OfType<DataChildForm>().FirstOrDefault();

            if (dataChildForm != null && !dataChildForm.IsDisposed)
            {
                dataChildForm.UpdateChart(XDataBuffer, YDataBuffer);
                dataChildForm.UpdateMeasurementData(GlobalSettings.Instance.MaxMeasure, GlobalSettings.Instance.MinMeasure, GlobalSettings.Instance.MaxSource, GlobalSettings.Instance.MinSource, GlobalSettings.Instance.Slope);
            }
        }

        private void TextboxStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxStop_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxStep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxSourceDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxSourceLimitLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxThickness_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void TextboxRepetition_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void TextboxMagneticFields_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void DisableEditRun(bool IsRun)
        {
            if (!IsRun)
            {
                IsRun = true;
                //GlobalSettings.Instance.IsRun = IsRun;

                ComboboxMeasure.Enabled = false;
                ComboboxRsense.Enabled = false;
                ComboboxSource.Enabled = false;
                ComboboxSourceLimitMode.Enabled = false;
                ComboboxStartUnit.Enabled = false;
                ComboboxStopUnit.Enabled = false;
                ComboboxStepUnit.Enabled = false;
                ComboboxSourceDelayUnit.Enabled = false;
                ComboboxSourceLimitLevelUnit.Enabled = false;
                ComboboxThicknessUnit.Enabled = false;
                ComboboxMagneticFieldsUnit.Enabled = false;

                TextboxStart.Enabled = false;
                TextboxStop.Enabled = false;
                TextboxStep.Enabled = false;
                TextboxSourceDelay.Enabled = false;
                TextboxSourceDelay.Enabled = false;
                TextboxSourceLimitLevel.Enabled = false;
                TextboxThickness.Enabled = false;
                TextboxRepetition.Enabled = false;
                TextboxMagneticFields.Enabled = false;
            }
            else
            {
                IsRun = false;
                //GlobalSettings.Instance.IsRun = IsRun;

                ComboboxMeasure.Enabled = true;
                ComboboxRsense.Enabled = true;
                ComboboxSource.Enabled = true;
                ComboboxSourceLimitMode.Enabled = true;
                ComboboxStartUnit.Enabled = true;
                ComboboxStopUnit.Enabled = true;
                ComboboxStepUnit.Enabled = true;
                ComboboxSourceDelayUnit.Enabled = true;
                ComboboxSourceLimitLevelUnit.Enabled = true;
                ComboboxThicknessUnit.Enabled = true;
                ComboboxMagneticFieldsUnit.Enabled = true;

                TextboxStart.Enabled = true;
                TextboxStop.Enabled = true;
                TextboxStep.Enabled = true;
                TextboxSourceDelay.Enabled = true;
                TextboxSourceDelay.Enabled = true;
                TextboxSourceLimitLevel.Enabled = true;
                TextboxThickness.Enabled = true;
                TextboxRepetition.Enabled = true;
                TextboxMagneticFields.Enabled = true;
            }
        }
    }
}