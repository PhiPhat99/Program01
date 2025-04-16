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
        private readonly string SMUModelKeyword = "MODEL 2450";
        private readonly string SSModelKeyword = "MODEL 7001";

        private int TargetPosition;
        private int CurrentTuner;

        public event EventHandler<bool> ModeChanged;
        public event EventHandler ToggleChanged;
        private readonly string ModeName = GlobalSettings.Instance.IsHallMode ? "Hall effect" : "Van der Pauw";

        private Form CurrentChildForm;
        private DataChildForm DataChildForm = null;

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
                    SMU = new FormattedIO488
                    {
                        IO = (IMessage)Rsrcmngr.Open(SMUAddresses[0])
                    };
                }

                if (SS == null && SSAddresses.Length > 0)
                {
                    SS = new FormattedIO488
                    {
                        IO = (IMessage)Rsrcmngr.Open(SSAddresses[0])
                    };
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"ไม่ค้นพบ GPIB Address ของเครื่องมือ: {Ex.Message}", "ข้อผิดพลาด");
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

                            Debug.WriteLine($"[DEBUG] โมเดลเครื่องมือวัด: {Response}");
                            Debug.WriteLine($"[DEBUG] อุปกรณ์ที่ใช้งานอยู่: {Device}");
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine($"เครื่องมือ {Device} ไม่มีการตอบสนอง");
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
                MessageBox.Show($"ไม่พบเจอ GPIB Address ของเครื่องมือที่ทำการเชื่อมต่อ: {Ex.Message}", "ข้อผิดพลาด");
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
            Debug.WriteLine("[เริ่มการตั้งค่าโหลดข้อมูล]");

            try
            {
                Debug.WriteLine($"[LOAD] RsenseMode (From GlobalSettings): {GlobalSettings.Instance.RsenseMode}");
                SetComboBoxSelectedItem(ComboboxRsense, GlobalSettings.Instance.RsenseMode);

                Debug.WriteLine($"[LOAD] MeasureMode (From GlobalSettings): {GlobalSettings.Instance.MeasureMode}");
                SetComboBoxSelectedItem(ComboboxMeasure, GlobalSettings.Instance.MeasureMode);

                Debug.WriteLine($"[LOAD] SourceMode (From GlobalSettings): {GlobalSettings.Instance.SourceMode}");
                SetComboBoxSelectedItem(ComboboxSource, GlobalSettings.Instance.SourceMode);

                Debug.WriteLine($"[LOAD] SourceLimitType (From GlobalSettings): {GlobalSettings.Instance.SourceLimitType}");
                SetComboBoxSelectedItem(ComboboxSourceLimitMode, GlobalSettings.Instance.SourceLimitType);

                Debug.WriteLine($"[LOAD] StartUnit (From GlobalSettings): {GlobalSettings.Instance.StartUnit}");
                SetComboBoxSelectedItem(ComboboxStartUnit, GlobalSettings.Instance.StartUnit);

                Debug.WriteLine($"[LOAD] StopUnit (From GlobalSettings): {GlobalSettings.Instance.StopUnit}");
                SetComboBoxSelectedItem(ComboboxStopUnit, GlobalSettings.Instance.StopUnit);

                Debug.WriteLine($"[LOAD] StepUnit (From GlobalSettings): {GlobalSettings.Instance.StepUnit}");
                SetComboBoxSelectedItem(ComboboxStepUnit, GlobalSettings.Instance.StepUnit);

                Debug.WriteLine($"[LOAD] SourceDelayUnit (From GlobalSettings): {GlobalSettings.Instance.SourceDelayUnit}");
                SetComboBoxSelectedItem(ComboboxSourceDelayUnit, GlobalSettings.Instance.SourceDelayUnit);

                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnit}");
                SetComboBoxSelectedItem(ComboboxSourceLimitLevelUnit, GlobalSettings.Instance.SourceLimitLevelUnit);
                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (To UI): {ComboboxSourceLimitLevelUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] ThicknessUnit (From GlobalSettings): {GlobalSettings.Instance.ThicknessUnit}");
                SetComboBoxSelectedItem(ComboboxThicknessUnit, GlobalSettings.Instance.ThicknessUnit);

                Debug.WriteLine($"[LOAD] MagneticFieldsUnit (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnit}");
                SetComboBoxSelectedItem(ComboboxMagneticFieldsUnit, GlobalSettings.Instance.MagneticFieldsUnit);

                Debug.WriteLine($"[LOAD] StartValue (From GlobalSettings): {GlobalSettings.Instance.StartValue}");
                SetTextboxValue(TextboxStart, GlobalSettings.Instance.StartValue.ToString());

                Debug.WriteLine($"[LOAD] StopValue (From GlobalSettings): {GlobalSettings.Instance.StopValue}");
                SetTextboxValue(TextboxStop, GlobalSettings.Instance.StopValue.ToString());

                Debug.WriteLine($"[LOAD] StepValue (From GlobalSettings): {GlobalSettings.Instance.StepValue}");
                SetTextboxValue(TextboxStep, GlobalSettings.Instance.StepValue.ToString());

                Debug.WriteLine($"[LOAD] SourceDelayValue (From GlobalSettings): {GlobalSettings.Instance.SourceDelayValue}");
                SetTextboxValue(TextboxSourceDelay, GlobalSettings.Instance.SourceDelayValue.ToString());

                Debug.WriteLine($"[LOAD] SourceLimitLevelValue (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelValue}");
                SetTextboxValue(TextboxSourceLimitLevel, GlobalSettings.Instance.SourceLimitLevelValue.ToString());

                Debug.WriteLine($"[LOAD] ThicknessValue (From GlobalSettings): {GlobalSettings.Instance.ThicknessValue}");
                SetTextboxValue(TextboxThickness, GlobalSettings.Instance.ThicknessValue.ToString());

                Debug.WriteLine($"[LOAD] RepetitionValue (From GlobalSettings): {GlobalSettings.Instance.RepetitionValue}");
                SetTextboxValue(TextboxRepetition, GlobalSettings.Instance.RepetitionValue.ToString());

                Debug.WriteLine($"[LOAD] MagneticFieldsValue (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsValue}");
                SetTextboxValue(TextboxMagneticFields, GlobalSettings.Instance.MagneticFieldsValue.ToString());
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"การตั้งค่าโหลดข้อมูลไม่สำเร็จ: {Ex.Message}", "ข้อผิดพลาด");

                Debug.WriteLine($"[ERROR - LOAD SETTINGS]: {Ex.Message}");
                Debug.WriteLine($"[ERROR - LOAD SETTINGS - STACK TRACE]: {Ex.StackTrace}");
            }
            finally
            {
                Debug.WriteLine("[เสร็จสิ้นการตั้งค่าโหลด]");
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
            Debug.WriteLine("[เริ่มต้นการตั้งค่าบันทึกข้อมูล]");

            try
            {
                GlobalSettings.Instance.RsenseMode = ComboboxRsense.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] RsenseMode (To GlobalSettings): {GlobalSettings.Instance.RsenseMode}");

                GlobalSettings.Instance.MeasureMode = ComboboxMeasure.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MeasureMode (To GlobalSettings): {GlobalSettings.Instance.MeasureMode}");

                GlobalSettings.Instance.SourceMode = ComboboxSource.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceMode (To GlobalSettings): {GlobalSettings.Instance.SourceMode}");

                GlobalSettings.Instance.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitType (To GlobalSettings): {GlobalSettings.Instance.SourceLimitType}");

                GlobalSettings.Instance.StartUnit = ComboboxStartUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StartUnit (To GlobalSettings): {GlobalSettings.Instance.StartUnit}");

                GlobalSettings.Instance.StopUnit = ComboboxStopUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StopUnit (To GlobalSettings): {GlobalSettings.Instance.StopUnit}");

                GlobalSettings.Instance.StepUnit = ComboboxStepUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StepUnit (To GlobalSettings): {GlobalSettings.Instance.StepUnit}");

                GlobalSettings.Instance.SourceDelayUnit = ComboboxSourceDelayUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceDelayUnit (To GlobalSettings): {GlobalSettings.Instance.SourceDelayUnit}");

                GlobalSettings.Instance.SourceLimitLevelUnit = ComboboxSourceLimitLevelUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitLevelUnit (To GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnit}");

                GlobalSettings.Instance.ThicknessUnit = ComboboxThicknessUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] ThicknessUnit (To GlobalSettings): {GlobalSettings.Instance.ThicknessUnit}");

                GlobalSettings.Instance.MagneticFieldsUnit = ComboboxMagneticFieldsUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MagneticFieldsUnit (To GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnit}");

                if (double.TryParse(TextboxStart.Text, out double startValue))
                {
                    GlobalSettings.Instance.StartValue = startValue;
                    Debug.WriteLine($"[SAVE] StartValue (To GlobalSettings): {GlobalSettings.Instance.StartValue}");
                }

                if (double.TryParse(TextboxStep.Text, out double stepValue))
                {
                    GlobalSettings.Instance.StepValue = stepValue;
                    Debug.WriteLine($"[SAVE] StepValue (To GlobalSettings): {GlobalSettings.Instance.StepValue}");
                }

                if (double.TryParse(TextboxStop.Text, out double stopValue))
                {
                    GlobalSettings.Instance.StopValue = stopValue;
                    Debug.WriteLine($"[SAVE] StopValue (To GlobalSettings): {GlobalSettings.Instance.StopValue}");
                }
                
                if (double.TryParse(TextboxSourceDelay.Text, out double delayValue))
                {
                    GlobalSettings.Instance.SourceDelayValue = delayValue;
                    Debug.WriteLine($"[SAVE] SourceDelayValue (To GlobalSettings): {GlobalSettings.Instance.SourceDelayValue}");
                }

                if (double.TryParse(TextboxSourceLimitLevel.Text, out double limitlevelValue))
                {
                    GlobalSettings.Instance.SourceLimitLevelValue = limitlevelValue;
                    Debug.WriteLine($"[SAVE] SourceLimitLevelValue (To GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelValue}");
                }

                if (double.TryParse(TextboxThickness.Text, out double thicknessValue))
                {
                    GlobalSettings.Instance.ThicknessValue = thicknessValue;
                    Debug.WriteLine($"[SAVE] ThicknessValue (To GlobalSettings): {GlobalSettings.Instance.ThicknessValue}");
                }

                if (int.TryParse(TextboxRepetition.Text, out int repetitionValue))
                {
                    GlobalSettings.Instance.RepetitionValue = repetitionValue;
                    Debug.WriteLine($"[SAVE] RepetitionValue (To GlobalSettings): {GlobalSettings.Instance.RepetitionValue}");
                }

                if (double.TryParse(TextboxMagneticFields.Text, out double magneticfieldsValue))
                {
                    GlobalSettings.Instance.MagneticFieldsValue = magneticfieldsValue;
                    Debug.WriteLine($"[SAVE] MagneticFieldsValue (To GlobalSettings): {GlobalSettings.Instance.MagneticFieldsValue}");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"การตั้งค่าบันทึกข้อมูลไม่สำเร็จ: {Ex.Message}", "ข้อผิดพลาด");

                Debug.WriteLine($"[ERROR - SAVE SETTINGS]: {Ex.Message}");
                Debug.WriteLine($"[ERROR - SAVE SETTINGS - STACK TRACE]: {Ex.StackTrace}");
            }

            Debug.WriteLine("[เสร็จสิ้นการตั้งค่าบันทึกข้อมูล]");
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

                Devices = new FormattedIO488
                {
                    IO = (IMessage)Rsrcmngr.Open(Ports)
                };

                Devices.IO.Timeout = 5000;
                Devices.WriteString("*IDN?");
                string ConnectedResponse = Devices.ReadString();
                Debug.WriteLine($"Device Connected: {ConnectedResponse}");

                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"ไม่สามารถทำการเชื่อมต่อเครื่องมือได้: {Ex.Message}", "ข้อผิดพลาดในการเชื่อมต่อ", MessageBoxButtons.OK);
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
                MessageBox.Show($"ไม่สามารถตัดการเชื่อมต่อเครื่องมือได้: {Ex.Message}", "ข้อผิดพลาดในการตัดการเชื่อมต่อ", MessageBoxButtons.OK);
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
                MessageBox.Show(GlobalSettings.Instance.IsSMUConnected ? "เชื่อมต่อเครื่องมือ Source Measure Unit สำเร็จ" : "ตัดการเชื่อมต่อจากเครื่องมือ Source Measure Unit", "การแจ้งเตือน", MessageBoxButtons.OK);
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาดในการเชื่อมต่อ", MessageBoxButtons.OK);
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
                MessageBox.Show(GlobalSettings.Instance.IsSSConnected ? "เชื่อมต่อเครื่องมือ Switch System สำเร็จ" : "ตัดการเชื่อมต่อจากเครื่องมือ Switch System", "การแจ้งเตือน", MessageBoxButtons.OK);
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาดในการเชื่อมต่อ", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                    MessageBox.Show("ไม่สามารถลบล้างข้อมูลการตั้งค่าได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการลบล้างข้อมูล", MessageBoxButtons.OK);
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
                MessageBox.Show($"การลบล้างข้อมูลการตั้งค่าไม่สำเร็จ: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
            Debug.WriteLine($"You select: {ModeName} measurement");

            LabelToggleSwitchVdP.ForeColor = isHallMode ? SystemColors.ActiveCaptionText : Color.FromArgb(144, 198, 101);
            LabelToggleSwitchHall.ForeColor = isHallMode ? Color.FromArgb(144, 198, 101) : SystemColors.ActiveCaptionText;
            PanelToggleSwitchButton.BackColor = isHallMode ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
            PanelToggleSwitchBase.BackColor = isHallMode ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);

            UpdateTunerImages(isHallMode);
        }

        private void UpdateTunerImages(bool isHallMode)
        {
            if (isHallMode)
            {
                PictureboxMeasPosition1.Image = Properties.Resources.Hall_MP1negative;
                PictureboxMeasPosition2.Image = Properties.Resources.Hall_MP1positive;
                PictureboxMeasPosition3.Image = Properties.Resources.Hall_MP2negative;
                PictureboxMeasPosition4.Image = Properties.Resources.Hall_MP2positive;
                PictureboxMeasPosition5.Image = Properties.Resources.Hall_MP3negative;
                PictureboxMeasPosition6.Image = Properties.Resources.Hall_MP3positive;
                PictureboxMeasPosition7.Image = Properties.Resources.Hall_MP4negative;
                PictureboxMeasPosition8.Image = Properties.Resources.Hall_MP4positive;
            }
            else
            {
                PictureboxMeasPosition1.Image = Properties.Resources.VdP_MP1negative;
                PictureboxMeasPosition2.Image = Properties.Resources.VdP_MP1positive;
                PictureboxMeasPosition3.Image = Properties.Resources.VdP_MP2negative;
                PictureboxMeasPosition4.Image = Properties.Resources.VdP_MP2positive;
                PictureboxMeasPosition5.Image = Properties.Resources.VdP_MP3negative;
                PictureboxMeasPosition6.Image = Properties.Resources.VdP_MP3positive;
                PictureboxMeasPosition7.Image = Properties.Resources.VdP_MP4negative;
                PictureboxMeasPosition8.Image = Properties.Resources.VdP_MP4positive;
            }
        }

        protected virtual void OnToggleChanged()
        {
            ToggleChanged?.Invoke(this, EventArgs.Empty);
            ModeChanged?.Invoke(this, GlobalSettings.Instance.IsModes);

            PanelToggleSwitchBase.BackColor = GlobalSettings.Instance.IsModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void PictureboxMeasPosition1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition5_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition6_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void PictureboxMeasPosition8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการเลือกตำแหน่งการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void IconbuttonTunerTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการทดสอบการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการทดสอบการวัด", MessageBoxButtons.OK);
                    return;
                }

                SaveToGlobalSettings();

                SMU.IO.Timeout = 1000000;
                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                if (repetitionValue > 3)
                {
                    MessageBox.Show("ไม่สามารถตั้งค่าการวัดซ้ำ (Repetition) มากกว่า 3 ครั้งในการทดสอบการวัด", "การแจ้งเตือน", MessageBoxButtons.OK);
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
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

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

                    string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                    Debug.WriteLine($"{allValues}");

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
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

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

                    string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                    Debug.WriteLine($"{allValues}");
                    

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
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

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

                    string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                    Debug.WriteLine($"{allValues}");
                    

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
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

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

                    string allValues = $"Sense: {GlobalSettings.Instance.RsenseMode}, Measure: {GlobalSettings.Instance.MeasureMode}, Source: {GlobalSettings.Instance.SourceMode}, Start: {GlobalSettings.Instance.StartValue} {GlobalSettings.Instance.StartUnit}, Step: {GlobalSettings.Instance.StepValue} {GlobalSettings.Instance.StepUnit}, Source Delay: {GlobalSettings.Instance.SourceDelayValue} {GlobalSettings.Instance.SourceDelayUnit}, Stop: {GlobalSettings.Instance.StopValue} {GlobalSettings.Instance.StopUnit}, Source Limit: {GlobalSettings.Instance.SourceLimitType}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValue} {GlobalSettings.Instance.SourceLimitLevelUnit}, Repetition: {GlobalSettings.Instance.RepetitionValue}, Thickness: {GlobalSettings.Instance.ThicknessValue} {GlobalSettings.Instance.ThicknessUnit}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValue} {GlobalSettings.Instance.MagneticFieldsUnit}";
                    Debug.WriteLine($"{allValues}");

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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
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
                    MessageBox.Show("ไม่สามารถทำการดึงข้อมูลการวัดจากเครื่องมือได้ เนื่องจากไม่มีข้อมูลอยู่ในบัฟเฟอร์", "ข้อผิดพลาดในการดึงข้อมูลการวัด", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {bufferPoints}, 'defbuffer1', SOURce, READing");
                string measureRawData = SMU.ReadString()?.Trim();
                string[] dataPairs = measureRawData.Split(',');
                Debug.WriteLine($" There are {bufferCount} readings is: {measureRawData}");

                if (dataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("รูปแบบของข้อมูลในบัฟเฟอร์ไม่ถูกต้อง", "ข้อผิดพลาดในการดึงข้อมูลการวัด", MessageBoxButtons.OK);
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
                    slope = Math.Abs((maxMeasure - minMeasure) / (maxSource - minSource));
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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

        private async void IconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            DisableEditRun(true);

            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการวัด", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                SaveToGlobalSettings();
                await RunMeasurement(startValue, stopValue, stepValue, repetitionValue, sourcelevellimitValue, thicknessValue, magneticfieldsValue, delayValue, points);
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
            finally
            {
                DisableEditRun(false);
            }
        }

        private async Task RunMeasurement(double startValue, double stopValue, double stepValue, int repetitionValue, double sourcelevellimitValue, double thicknessValue, double magneticfieldsValue, double delayValue, int points)
        {
            try
            {
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
                        SMU.WriteString("OUTPut OFF");
                        SS.WriteString("*CLS");
                        MessageBox.Show($"ทำการวัด {ModeName} เสร็จสิ้นแล้ว", "การวัดเสร็จสิ้น", MessageBoxButtons.OK);

                        if (Application.OpenForms.OfType<VdPTotalMeasureValuesForm>().FirstOrDefault() is VdPTotalMeasureValuesForm VdPTotalForm)
                        {
                            VdPTotalForm.Invoke((MethodInvoker)delegate { VdPTotalForm.LoadMeasurementData(); });
                        }

                        CollectAndCalculateVdPMeasured.Instance.CalculateVanderPauw();
                        break;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
            finally
            {
                DisableEditRun(false);
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
                { 1, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!5", "1!2!4", "1!3!3", "1!4!6" } :
                                                                new List<string> { "1!1!5", "1!2!3", "1!3!6", "1!4!4" }},
                { 2, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!4", "1!2!5", "1!3!3", "1!4!6" } :
                                                                new List<string> { "1!1!3", "1!2!5", "1!3!6", "1!4!4" }},
                { 3, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!6", "1!2!3", "1!3!4", "1!4!5" } :
                                                                new List<string> { "1!1!4", "1!2!6", "1!3!3", "1!4!5" }},
                { 4, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!3", "1!2!6", "1!3!4", "1!4!5" } :
                                                                new List<string> { "1!1!6", "1!2!4", "1!3!3", "1!4!5" }},
                { 5, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!3", "1!2!4", "1!3!5", "1!4!6" } :
                                                                new List<string> { "1!1!5", "1!2!3", "1!3!4", "1!4!6" }},
                { 6, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!4", "1!2!3", "1!3!5", "1!4!6" } :
                                                                new List<string> { "1!1!3", "1!2!5", "1!3!4", "1!4!6" }},
                { 7, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!6", "1!2!5", "1!3!4", "1!4!3" } :
                                                                new List<string> { "1!1!4", "1!2!6", "1!3!5", "1!4!3" }},
                { 8, GlobalSettings.Instance.IsModes == false ? new List<string> { "1!1!5", "1!2!6", "1!3!4", "1!4!3" } :
                                                                new List<string> { "1!1!6", "1!2!4", "1!3!5", "1!4!3" }}
            };

            Debug.WriteLine($"Tuner: {Tuner}, IsModes: {GlobalSettings.Instance.IsModes}");

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
                    MessageBox.Show("ไม่สามารถทำการดึงข้อมูลการวัดจากเครื่องมือได้ เนื่องจากไม่มีข้อมูลอยู่ในบัฟเฟอร์", "ข้อผิดพลาดในการดึงข้อมูลการวัด", MessageBoxButtons.OK);
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
                    MessageBox.Show("รูปแบบของข้อมูลในบัฟเฟอร์ไม่ถูกต้อง", "ข้อผิดพลาดในการดึงข้อมูลการวัด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถตรวจสอบข้อผิดพลาดจาดเครื่องมือได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการตรวจสอบ", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("SYSTem:ERRor?");
                SS.WriteString("SYSTem:ERRor?");
                string SMUrespones = SMU.ReadString();
                string SSresponses = SS.ReadString();

                if (SMUrespones == null && SSresponses == null)
                {
                    MessageBox.Show("ไม่พบข้อผิดพลาดจากเครื่องมือ", "การตรวจสอบเสร็จสิ้น", MessageBoxButtons.OK);
                    Debug.WriteLine($"There is Source Measure Unit error : {SMUrespones}");
                    Debug.WriteLine($"There is Switch System error : {SSresponses}");
                }
                
                if (SMUrespones != null || SSresponses != null)
                {
                    MessageBox.Show($"ตรวจพบข้อผิดพลาดจากเครื่องมือ {SMUModelKeyword} {SMUrespones}: {SSModelKeyword} {SSresponses} ? ", "การตรวจสอบเสร็จสิ้น", MessageBoxButtons.OK);   // แก้ไขเงื่อนไขตรงนี้ด้วย
                    Debug.WriteLine($"There is Source Measure Unit error : {SMUrespones}");
                    Debug.WriteLine($"There is Switch System error : {SSresponses}");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
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
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetition, out double sourcelevellimit, out double thickness, out double magneticfields, out double delay, out int points)
        {
            start = stop = step = thickness = magneticfields = 0;
            repetition = points = 1;
            delay = 100;
            sourcelevellimit = 0;

            try
            {
                if (!IsValidNumber(TextboxStart.Text, ComboboxStartUnit.SelectedItem, out start) ||
                    !IsValidNumber(TextboxStop.Text, ComboboxStopUnit.SelectedItem, out stop) ||
                    !IsValidNumber(TextboxStep.Text, ComboboxStepUnit.SelectedItem, out step) ||
                    !IsValidNumber(TextboxSourceDelay.Text, ComboboxSourceDelayUnit.SelectedItem, out delay) ||
                    !IsValidNumber(TextboxSourceLimitLevel.Text, ComboboxSourceLimitLevelUnit.SelectedItem, out sourcelevellimit) ||
                    !IsValidNumber(TextboxThickness.Text, ComboboxThicknessUnit.SelectedItem, out thickness) ||
                    !IsValidNumber(TextboxMagneticFields.Text, ComboboxMagneticFieldsUnit.SelectedItem, out magneticfields))
                {
                    MessageBox.Show("กรุณาตรวจสอบและทำการตั้งค่าการวัดให้ถูกต้อง", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                points = (int)((stop - start) / step) + 1;

                if (!int.TryParse(TextboxRepetition.Text, out repetition) || repetition < 1 || repetition > 16)
                {
                    MessageBox.Show("ควรทำการตั้งค่าการวัดซ้ำมากกว่า 1 ครั้ง แต่ไม่เกิน 16 ครั้ง กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (start >= stop)
                {
                    MessageBox.Show("ควรทำการตั้งค่าเริ่มต้นการวัดน้อยกว่าค่าสิ้นสุดการวัด กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (step <= 0)
                {
                    MessageBox.Show("ควรทำการตั้งค่าระดับการวัดมากกว่า 0 กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (step >= stop)
                {
                    MessageBox.Show("ควรทำการตั้งค่าระดับการวัดน้อยกว่าค่าสิ้นสุดการวัด กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (delay < 99E-6 || delay > 10E+3)
                {
                    MessageBox.Show("ควรทำการตั้งค่าความหน่วงของการวัดอยู่ในช่วง 100 µs - 10 ks กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (thickness < 0)
                {
                    MessageBox.Show("ไม่สามารถตั้งค่าความหนาของตัวอย่างน้อยกว่า 0 ได้ กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (string.Equals(GlobalSettings.Instance.SourceLimitType, "Current", StringComparison.OrdinalIgnoreCase) && (sourcelevellimit > 1.05 || sourcelevellimit < -1.05))
                {
                    MessageBox.Show("ควรทำการตั้งค่าระดับขีดจำกัดของกระแสจากแหล่งจ่ายอยู่ในช่วง -1.05 A - 1.05 A กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (string.Equals(GlobalSettings.Instance.SourceLimitType, "Voltage", StringComparison.OrdinalIgnoreCase) && (sourcelevellimit > 21 || sourcelevellimit < -21))
                {
                    MessageBox.Show("ควรทำการตั้งค่าระดับขีดจำกัดของแรงดันจากแหล่งจ่ายอยู่ในช่วง -21 V - 21 V กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการตรวจสอบการตั้งค่าการวัด: {Ex.Message}", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
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

        private void DisableEditRun(bool shouldDisable)
        {
            ComboboxMeasure.Enabled = !shouldDisable;
            ComboboxRsense.Enabled = !shouldDisable;
            ComboboxSource.Enabled = !shouldDisable;
            ComboboxSourceLimitMode.Enabled = !shouldDisable;
            ComboboxStartUnit.Enabled = !shouldDisable;
            ComboboxStopUnit.Enabled = !shouldDisable;
            ComboboxStepUnit.Enabled = !shouldDisable;
            ComboboxSourceDelayUnit.Enabled = !shouldDisable;
            ComboboxSourceLimitLevelUnit.Enabled = !shouldDisable;
            ComboboxThicknessUnit.Enabled = !shouldDisable;
            ComboboxMagneticFieldsUnit.Enabled = !shouldDisable;

            TextboxStart.Enabled = !shouldDisable;
            TextboxStop.Enabled = !shouldDisable;
            TextboxStep.Enabled = !shouldDisable;
            TextboxSourceDelay.Enabled = !shouldDisable;
            TextboxSourceLimitLevel.Enabled = !shouldDisable;
            TextboxThickness.Enabled = !shouldDisable;
            TextboxRepetition.Enabled = !shouldDisable;
            TextboxMagneticFields.Enabled = !shouldDisable;
        }
    }
}