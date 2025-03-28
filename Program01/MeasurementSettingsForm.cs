using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Markup;
using FontAwesome.Sharp;
using Ivi.Visa.Interop;

namespace Program01
{
    public partial class MeasurementSettingsForm : Form
    {
        // Fields
        public FormattedIO488 SMU;
        public FormattedIO488 SS;
        public Ivi.Visa.Interop.ResourceManager Rsrcmngr;

        public string RsenseMode;
        public string MeasureMode;
        public string SourceMode;
        public string SourceLimitMode;
        public string StartValue;
        public string StepValue;
        public string StopValue;
        public string SourceDelayValue;
        public string SourceLimitLevelValue;
        public string RepetitionValue;
        public string ThicknessValue;
        private string MagneticFieldsValue;

        public double MaxSource = double.MinValue;
        public double MinSource = double.MaxValue;
        public double MaxMeasure = double.MinValue;
        public double MinMeasure = double.MaxValue;
        public double Slope;

        private int TargetPosition;
        private int CurrentTuner;

        public event EventHandler<bool> ModeChanged;
        public event EventHandler ToggleChanged;
        public event Action<List<double>, List<double>> MeasurementCompleted;

        private Form CurrentChildForm;
        public DataChildForm DataChildForm = null;
        public ChannelSettingsChildForm ChannelSettingsChildForm = null;

        public List<double> XDataBuffer = new List<double>();
        public List<double> YDataBuffer = new List<double>();
        private List<double> X = new List<double>();
        private List<double> Y = new List<double>();

        public VdPTotalMeasureValueForm VdPTotalMeasureForm;
        public event Action<List<double>, List<double>, double, double, double, double, double> DataUpdated;

        public bool IsOn
        {
            get => GlobalSettings.IsModes;
            set
            {
                GlobalSettings.IsModes = value;
                UpdateToggleState();
            }
        }

        public MeasurementSettingsForm()
        {
            InitializeComponent();
            Rsrcmngr = new Ivi.Visa.Interop.ResourceManager();
            InitializeGPIB();
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

        public static class GlobalSettings
        {
            public static event Action OnSettingsChanged;

            private static bool CIsFirstRunProgram;
            public static bool IsFirstRunProgram
            {
                get => CIsFirstRunProgram;
                set
                {
                    if (CIsFirstRunProgram != value)
                    {
                        CIsFirstRunProgram = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            private static bool CIsSMUConnected;
            public static bool IsSMUConnected
            {
                get => CIsSMUConnected;
                set
                {
                    if (CIsSMUConnected != value)
                    {
                        CIsSMUConnected = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            public static bool CIsSSConnected;
            public static bool IsSSConnected
            {
                get => CIsSSConnected;
                set
                {
                    if (CIsSSConnected != value)
                    {
                        CIsSSConnected = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            public static bool CIsModes;
            public static bool IsModes
            {
                get => CIsModes;
                set
                {
                    if (CIsModes != value)
                    {
                        CIsModes = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            private static bool CIsHallMode;
            public static bool IsHallMode
            {
                get => CIsHallMode;
                set
                {
                    if (CIsHallMode != value)
                    {
                        CIsHallMode = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            private static bool CIsVanDerPauwMode;
            public static bool IsVanDerPauwMode
            {
                get => CIsVanDerPauwMode;
                set
                {
                    if (CIsVanDerPauwMode != value)
                    {
                        CIsVanDerPauwMode = value;
                        OnSettingsChanged?.Invoke();
                    }
                }
            }

            private static string CRsenseMode;
            public static string RsenseMode
            {
                get => CRsenseMode;
                set { CRsenseMode = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CMeasureMode;
            public static string MeasureMode
            {
                get => CMeasureMode;
                set { CMeasureMode = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CSourceMode;
            public static string SourceMode
            {
                get => CSourceMode;
                set { CSourceMode = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CSourceLimitType;
            public static string SourceLimitType
            {
                get => CSourceLimitType;
                set { CSourceLimitType = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CStartValue;
            public static string StartValue
            {
                get => CStartValue;
                set { CStartValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CStopValue;
            public static string StopValue
            {
                get => CStopValue;
                set { CStopValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CStepValue;
            public static string StepValue
            {
                get => CStepValue;
                set { CStepValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CSourceDelayValue;
            public static string SourceDelayValue
            {
                get => CSourceDelayValue;
                set { CSourceDelayValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CSourceLimitLevelValue;
            public static string SourceLimitLevelValue
            {
                get => CSourceLimitLevelValue;
                set { CSourceLimitLevelValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CThicknessValue;
            public static string ThicknessValue
            {
                get => CThicknessValue;
                set { CThicknessValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CRepetitionValue;
            public static string RepetitionValue
            {
                get => CRepetitionValue;
                set { CRepetitionValue = value; OnSettingsChanged?.Invoke(); }
            }

            private static string CMagneticFieldsValue;
            public static string MagneticFieldsValue
            {
                get => CMagneticFieldsValue;
                set { CMagneticFieldsValue = value; OnSettingsChanged?.Invoke(); }
            }

            public static string StartUnit { get; set; }
            public static string StopUnit { get; set; }
            public static string StepUnit { get; set; }
            public static string SourceDelayUnit { get; set; }
            public static string SourceLimitLevelUnit { get; set; }
            public static string ThicknessUnit { get; set; }
            public static string MagneticFieldsUnit { get; set; }

            // Buffer Data สำหรับเก็บค่าที่ใช้ใน DataChildForm
            public static List<double> XDataBuffer { get; private set; } = new List<double>();
            public static List<double> YDataBuffer { get; private set; } = new List<double>();

            public static double MaxMeasure { get; private set; } = double.NegativeInfinity;
            public static double MinMeasure { get; private set; } = double.PositiveInfinity;
            public static double MaxSource { get; private set; } = double.NegativeInfinity;
            public static double MinSource { get; private set; } = double.PositiveInfinity;
            public static double Slope { get; private set; } = double.NaN;

            public static void UpdateDataBuffer(List<double> Xdata, List<double> Ydata, double maxMeasure, double minMeasure, double maxSource, double minSource, double slope)
            {
                XDataBuffer = new List<double>(Xdata);
                YDataBuffer = new List<double>(Ydata);
                MaxMeasure = maxMeasure;
                MinMeasure = minMeasure;
                MaxSource = maxSource;
                MinSource = minSource;
                Slope = slope;

                OnSettingsChanged?.Invoke();
            }
        }

        private void SaveToGlobal()
        {
            try
            {
                GlobalSettings.RsenseMode = ComboboxRsense.SelectedItem?.ToString() ?? "";
                GlobalSettings.MeasureMode = ComboboxMeasure.SelectedItem?.ToString() ?? "";
                GlobalSettings.SourceMode = ComboboxSource.SelectedItem?.ToString() ?? "";
                GlobalSettings.SourceLimitType = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";

                GlobalSettings.StartUnit = ComboboxStartUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.StepUnit = ComboboxStepUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.StopUnit = ComboboxStopUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.SourceDelayUnit = ComboboxSourceDelayUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.SourceLimitLevelUnit = ComboboxSourceLimitLevelUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.ThicknessUnit = ComboboxThicknessUnit.SelectedItem?.ToString() ?? "";
                GlobalSettings.MagneticFieldsUnit = ComboboxMagneticFieldsUnit.SelectedItem?.ToString() ?? "";

                GlobalSettings.StartValue = TextboxStart.Text;
                GlobalSettings.StopValue = TextboxStop.Text;
                GlobalSettings.StepValue = TextboxStep.Text;
                GlobalSettings.SourceDelayValue = TextboxSourceDelay.Text;
                GlobalSettings.SourceLimitLevelValue = TextboxSourceLimitLevel.Text;
                GlobalSettings.ThicknessValue = TextboxThickness.Text;
                GlobalSettings.RepetitionValue = TextboxRepetition.Text;
                GlobalSettings.MagneticFieldsValue = TextboxMagneticFields.Text;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}", "ERROR");
            }
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

            Debug.WriteLine(Buttons == IconbuttonSMUConnection ? $"SMU Connected: {GlobalSettings.IsSMUConnected}" : $"SS Connected: {GlobalSettings.IsSSConnected}");
        }

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool Success = false;

                if (!GlobalSettings.IsSMUConnected)
                {
                    // ตรวจสอบว่ามีอ็อบเจ็กต์ SMU หรือยัง
                    if (SMU == null)
                    {
                        SMU = new FormattedIO488(); // สร้างอ็อบเจ็กต์ใหม่
                    }

                    // ดำเนินการเชื่อมต่อ
                    Success = ConnectDevice(ref SMU, ComboboxVISASMUIOPort.SelectedItem?.ToString());
                    
                    if (Success)
                    {
                        GlobalSettings.IsSMUConnected = true;
                    }
                }
                else
                {
                    // ตัดการเชื่อมต่อ
                    Success = DisconnectDevice(ref SMU);

                    if (Success)
                    {
                        GlobalSettings.IsSMUConnected = false;
                        SMU = null;
                    }
                }

                // อัปเดต UI ตามสถานะการเชื่อมต่อ
                UpdateUIAfterConnection(GlobalSettings.IsSMUConnected ? "Connected to Source Measure Unit" : "Disconnected from Source Measure Unit", GlobalSettings.IsSMUConnected, IconbuttonSMUConnection);
                MessageBox.Show(GlobalSettings.IsSMUConnected ? "Connected to Source Measure Unit" : "Disconnected from Source Measure Unit", "Informationc");
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

                if (!GlobalSettings.IsSSConnected)
                {
                    if (SS == null)
                    {
                        SS = new FormattedIO488();
                    }

                    Success = ConnectDevice(ref SS, ComboboxVISASSIOPort.SelectedItem?.ToString());
                    if (Success)
                    {
                        GlobalSettings.IsSSConnected = true;
                    }
                }
                else
                {
                    Success = DisconnectDevice(ref SS);
                    if (Success)
                    {
                        GlobalSettings.IsSSConnected = false;
                        SS = null;
                    }
                }

                UpdateUIAfterConnection(GlobalSettings.IsSSConnected ? "Connected to Switch System" : "Disconnected from Switch System", GlobalSettings.IsSSConnected, IconbuttonSSConnection);
                MessageBox.Show(GlobalSettings.IsSSConnected ? "Connected to Switch System" : "Disconnected from Switch System", "Information");
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}", "Connection Error", MessageBoxButtons.OK);
            }
        }

        private void MeasurementSettingsChildForm_Load(object sender, EventArgs e)
        {
            UpdateUIAfterConnection("Restoring SMU Connection Status", GlobalSettings.IsSMUConnected, IconbuttonSMUConnection);
            UpdateUIAfterConnection("Restoring SS Connection Status", GlobalSettings.IsSSConnected, IconbuttonSSConnection);
            UpdateMeasurementMode();
            UpdateToggleState();

            ComboboxRsense.Items.Clear();
            ComboboxRsense.Items.AddRange(new string[] { "2-Wires", "4-Wires" });

            ComboboxMeasure.Items.Clear();
            ComboboxMeasure.Items.AddRange(new string[] { "Voltage", "Current" });

            ComboboxSource.Items.Clear();
            ComboboxSource.Items.AddRange(new string[] { "Voltage", "Current" });

            LoadFromGlobal();

            SetComboBoxSelectedItem(ComboboxRsense, GlobalSettings.RsenseMode);
            SetComboBoxSelectedItem(ComboboxMeasure, GlobalSettings.MeasureMode);
            SetComboBoxSelectedItem(ComboboxSource, GlobalSettings.SourceMode);
            SetComboBoxSelectedItem(ComboboxSourceLimitMode, GlobalSettings.SourceLimitType);
            SetComboBoxSelectedItem(ComboboxStartUnit, GlobalSettings.StartUnit);
            SetComboBoxSelectedItem(ComboboxStepUnit, GlobalSettings.StepUnit);
            SetComboBoxSelectedItem(ComboboxStopUnit, GlobalSettings.StopUnit);
            SetComboBoxSelectedItem(ComboboxSourceDelayUnit, GlobalSettings.SourceDelayUnit);
            SetComboBoxSelectedItem(ComboboxSourceLimitLevelUnit, GlobalSettings.SourceLimitLevelUnit);
            SetComboBoxSelectedItem(ComboboxThicknessUnit, GlobalSettings.ThicknessUnit);
            SetComboBoxSelectedItem(ComboboxMagneticFieldsUnit, GlobalSettings.MagneticFieldsUnit);

            TextboxStart.Text = GlobalSettings.StartValue;
            TextboxStop.Text = GlobalSettings.StopValue;
            TextboxStep.Text = GlobalSettings.StepValue;
            TextboxSourceDelay.Text = GlobalSettings.SourceDelayValue;
            TextboxSourceLimitLevel.Text = GlobalSettings.SourceLimitLevelValue;
            TextboxThickness.Text = GlobalSettings.ThicknessValue;
            TextboxRepetition.Text = GlobalSettings.RepetitionValue;
            TextboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;
        }

        private void LoadFromGlobal()
        {
            try
            {
                TextboxStart.Text = GlobalSettings.StartValue;
                TextboxStop.Text = GlobalSettings.StopValue;
                TextboxStep.Text = GlobalSettings.StepValue;
                TextboxSourceDelay.Text = GlobalSettings.SourceDelayValue;
                TextboxSourceLimitLevel.Text = GlobalSettings.SourceLimitLevelValue;
                TextboxThickness.Text = GlobalSettings.ThicknessValue;
                TextboxRepetition.Text = GlobalSettings.RepetitionValue;
                TextboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;

                SetComboBoxSelectedItem(ComboboxRsense, GlobalSettings.RsenseMode);
                SetComboBoxSelectedItem(ComboboxMeasure, GlobalSettings.MeasureMode);
                SetComboBoxSelectedItem(ComboboxSource, GlobalSettings.SourceMode);
                SetComboBoxSelectedItem(ComboboxSourceLimitMode, GlobalSettings.SourceLimitType);

                SetComboBoxSelectedItem(ComboboxStartUnit, GlobalSettings.StartUnit);
                SetComboBoxSelectedItem(ComboboxStepUnit, GlobalSettings.StepUnit);
                SetComboBoxSelectedItem(ComboboxStopUnit, GlobalSettings.StopUnit);
                SetComboBoxSelectedItem(ComboboxSourceDelayUnit, GlobalSettings.SourceDelayUnit);
                SetComboBoxSelectedItem(ComboboxSourceLimitLevelUnit, GlobalSettings.SourceLimitLevelUnit);
                SetComboBoxSelectedItem(ComboboxThicknessUnit, GlobalSettings.ThicknessUnit);
                SetComboBoxSelectedItem(ComboboxMagneticFieldsUnit, GlobalSettings.MagneticFieldsUnit);
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}", "ERROR");
            }
        }

        private void SetComboBoxSelectedItem(ComboBox Comboboxs, string Value)
        {
            if (!string.IsNullOrEmpty(Value) && Comboboxs.Items.Contains(Value))
            {
                Comboboxs.SelectedItem = Value;
            }
            else
            {
                Comboboxs.SelectedIndex = -1;
            }
        }

        private void ComboboxRsense_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RsenseMode = ComboboxRsense.SelectedItem?.ToString();

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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxMeasure_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MeasureMode = ComboboxMeasure.SelectedItem?.ToString();

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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceMode = ComboboxSource.SelectedItem?.ToString();

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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ComboboxSourceLimitMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SourceLimitMode = ComboboxSourceLimitMode.SelectedItem?.ToString();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
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
                TextboxStart.Text = "";
                TextboxStep.Text = "";
                TextboxStop.Text = "";
                TextboxSourceDelay.Text = "";
                TextboxSourceLimitLevel.Text = "";
                TextboxThickness.Text = "";
                TextboxRepetition.Text = "";
                TextboxMagneticFields.Text = "";
                RsenseMode = "";
                MeasureMode = "";
                SourceMode = "";
                SourceLimitMode = "";
                StartValue = "";
                StopValue = "";
                StepValue = "";
                SourceLimitLevelValue = "";
                ThicknessValue = "";
                RepetitionValue = "";
                MagneticFieldsValue = "";
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
                GlobalSettings.IsModes = !GlobalSettings.IsModes;
                GlobalSettings.IsHallMode = GlobalSettings.IsModes;
                GlobalSettings.IsVanDerPauwMode = !GlobalSettings.IsModes;

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
            TargetPosition = GlobalSettings.IsModes ? PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width - 1 : 1;
            PanelToggleSwitchButton.Location = new Point(TargetPosition, PanelToggleSwitchButton.Location.Y);

            if (PanelToggleSwitchButton.Location.X < 0 || PanelToggleSwitchButton.Location.X > PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width)
            {
                PanelToggleSwitchButton.Location = new Point(1, PanelToggleSwitchButton.Location.Y);
            }
        }

        private void UpdateMeasurementMode()
        {
            bool isHallMode = GlobalSettings.IsHallMode;
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
                PictureboxTuner1.Image = global::Program01.Properties.Resources.V_1_Hall;
                PictureboxTuner2.Image = global::Program01.Properties.Resources.V_2_Hall;
                PictureboxTuner3.Image = global::Program01.Properties.Resources.V_3_Hall;
                PictureboxTuner4.Image = global::Program01.Properties.Resources.V_4_Hall;
                PictureboxTuner5.Image = global::Program01.Properties.Resources.V_5_Hall;
                PictureboxTuner6.Image = global::Program01.Properties.Resources.V_6_Hall;
                PictureboxTuner7.Image = global::Program01.Properties.Resources.V_7_Hall;
                PictureboxTuner8.Image = global::Program01.Properties.Resources.V_8_Hall;
            }
            else
            {
                PictureboxTuner1.Image = global::Program01.Properties.Resources.R_A1_VdP;
                PictureboxTuner2.Image = global::Program01.Properties.Resources.R_A2_VdP;
                PictureboxTuner3.Image = global::Program01.Properties.Resources.R_A3_VdP;
                PictureboxTuner4.Image = global::Program01.Properties.Resources.R_A4_VdP;
                PictureboxTuner5.Image = global::Program01.Properties.Resources.R_B1_VdP;
                PictureboxTuner6.Image = global::Program01.Properties.Resources.R_B2_VdP;
                PictureboxTuner7.Image = global::Program01.Properties.Resources.R_B3_VdP;
                PictureboxTuner8.Image = global::Program01.Properties.Resources.R_B4_VdP;
            }
        }

        protected virtual void OnToggleChanged()
        {
            ToggleChanged?.Invoke(this, EventArgs.Empty);
            ModeChanged?.Invoke(this, GlobalSettings.IsModes);

            PanelToggleSwitchBase.BackColor = GlobalSettings.IsModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void PictureboxTuner1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.IsModes == true)
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!4)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (GlobalSettings.IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (GlobalSettings.IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
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
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SaveToGlobal();
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

                if (SourceMode == "Voltage" && MeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.IsModes == true)
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}, Magnetic Fields: {TextboxMagneticFields.Text} {ComboboxMagneticFieldsUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (SourceMode == "Voltage" && MeasureMode == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.IsModes == true)
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}, Magnetic Fields: {TextboxMagneticFields.Text} {ComboboxMagneticFieldsUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (SourceMode == "Current" && MeasureMode == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.IsModes == true)
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}, Magnetic Fields: {TextboxMagneticFields.Text} {ComboboxMagneticFieldsUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (SourceMode == "Current" && MeasureMode == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");

                    if (RsenseMode == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue},  {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    if (GlobalSettings.IsModes == true)
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}, Magnetic Fields: {TextboxMagneticFields.Text} {ComboboxMagneticFieldsUnit.SelectedItem}";
                        Debug.WriteLine($"{allValues}.");
                    }
                    else
                    {
                        string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}";
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonRunMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                CollectVdPMeasuredValue.Instance.ClearMeasurements();
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

                while (CurrentTuner <= 8)
                {
                    ConfigureSwitchSystem();
                    await Task.Delay(200);
                    UpdateMeasurementState();
                    ConfigureSourceMeasureUnit();
                    await ExecuteSweep();
                    await Task.Delay(points * repetitionValue * 300);
                    TracingRunMeasurement();
                    CurrentTuner++;

                    if (CurrentTuner > 8)
                    {
                        Debug.WriteLine("All tuners completed.");
                        MessageBox.Show("Measurement completed", "Measurement Successfully", MessageBoxButtons.OK);
                        SMU.WriteString("OUTPut OFF");
                        SS.WriteString("*CLS");
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
            var channels = GetChannelConfiguration(CurrentTuner, GlobalSettings.IsModes);
            /*List<string> Combined4Channels = new List<string>();
            int memoryNumber = 1;  // เพิ่มการนับหน่วยความจำ*/

            foreach (var channel in channels)
            {
                SS.WriteString($"ROUTe:CLOSe (@ {channel})");
                /*Combined4Channels.Add(channel);  // เพิ่มช่องสัญญาณเข้าใน List

                // เมื่อครบ 4 ช่อง ให้บันทึกลงในหน่วยความจำ
                if (Combined4Channels.Count == 4)
                {
                    // สร้างคำสั่ง ROUTe:MEMory:SAVE M<number> และบันทึกค่า
                    string combinedChannelsStr = string.Join(",", Combined4Channels);
                    SS.WriteString($"ROUTe:MEMory:SAVE M{memoryNumber}");

                    // ตรวจสอบผ่าน Debug
                    Debug.WriteLine($"Saving to M{memoryNumber}: (@{combinedChannelsStr})");

                    // รีเซ็ต List สำหรับช่องสัญญาณ
                    Combined4Channels.Clear();
                    memoryNumber++;  // เพิ่มหมายเลขหน่วยความจำ
                }*/
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

            if (SourceMode == "Current")
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

            if (MeasureMode == "Current")
            {
                SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
            }
            else
            {
                SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
            }

            if (RsenseMode == "4-Wires")
            {
                SMU.WriteString($"SENSe:{MeasureMode}:RSENse ON");
            }
            else
            {
                SMU.WriteString($"SENSe:{MeasureMode}:RSENse OFF");
            }
        }

        private async Task ExecuteSweep()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                return;
            }

            if (GlobalSettings.IsModes == true)
            {
                string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}, Magnetic Fields: {TextboxMagneticFields.Text} {ComboboxMagneticFieldsUnit.SelectedItem}";
                Debug.WriteLine($"{allValues}.");
            }
            else
            {
                string allValues = $"Sense: {RsenseMode}, Measure: {MeasureMode}, Source: {SourceMode}, Start: {TextboxStart.Text} {ComboboxStartUnit.SelectedItem}, Step: {TextboxStep.Text} {ComboboxStepUnit.SelectedItem}, Source Delay: {TextboxSourceDelay.Text} {ComboboxSourceDelayUnit.SelectedItem}, Stop: {TextboxStop.Text} {ComboboxStopUnit.SelectedItem}, Source Limit: {SourceLimitMode}, Limit Level: {TextboxSourceLimitLevel.Text} {ComboboxSourceLimitLevelUnit.SelectedItem}, Repetition: {TextboxRepetition.Text}, Thickness: {TextboxThickness.Text} {ComboboxThicknessUnit.SelectedItem}";
                Debug.WriteLine($"{allValues}.");
            }

            if (SourceMode == "Current")
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
            await Task.Delay(points * repetitionValue * (int)delayValue * 250);
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

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.IsSMUConnected || !GlobalSettings.IsSSConnected)
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

                if (GlobalSettings.XDataBuffer.Count > 0 && GlobalSettings.YDataBuffer.Count > 0)
                {
                    DataChildForm.UpdateChart(GlobalSettings.XDataBuffer, GlobalSettings.YDataBuffer);
                    DataChildForm.UpdateMeasurementData(GlobalSettings.MaxMeasure, GlobalSettings.MinMeasure, GlobalSettings.MaxSource, GlobalSettings.MinSource, GlobalSettings.Slope);
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

                if (GlobalSettings.IsModes)
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

                if (string.Equals(SourceLimitMode, "Current", StringComparison.OrdinalIgnoreCase) &&
                    (sourcelevellimit > 1.05 || sourcelevellimit < -1.05))
                {
                    return false;
                }

                if (string.Equals(SourceLimitMode, "Voltage", StringComparison.OrdinalIgnoreCase) &&
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

                GlobalSettings.UpdateDataBuffer(xData, yData, maxMeasure, minMeasure, maxSource, minSource, slope);
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
                dataChildForm.UpdateMeasurementData(GlobalSettings.MaxMeasure, GlobalSettings.MinMeasure, GlobalSettings.MaxSource, GlobalSettings.MinSource, GlobalSettings.Slope);
            }
        }

        private void TracingRunMeasurement()
        {
            try
            {
                // ส่งคำสั่งไปยังเครื่องมือเพื่อขอข้อมูล
                SMU.WriteString("TRACe:ACTual?");
                string BufferCount = SMU.ReadString().Trim();
                Debug.WriteLine($"Buffer count: {BufferCount}");

                // ตรวจสอบว่า buffer มีข้อมูลหรือไม่
                if (!int.TryParse(BufferCount, out int BufferPoints) || BufferPoints == 0)
                {
                    MessageBox.Show("No data in buffer!", "Error", MessageBoxButtons.OK);
                    return;
                }

                // ขอข้อมูลการวัดจากเครื่องมือ
                SMU.WriteString($"TRACe:DATA? 1, {BufferPoints}, 'defbuffer1', SOURce, READing");
                string MeasureRawData = SMU.ReadString().Trim();
                Debug.WriteLine($"Buffer contains: {BufferPoints} readings");
                Debug.WriteLine($"Measured Raw Data: {MeasureRawData}");

                string[] DataPairs = MeasureRawData.Split(',');
                List<double> XData = new List<double>();
                List<double> YData = new List<double>();

                // ตรวจสอบว่า ข้อมูลใน buffer มีรูปแบบที่ถูกต้องหรือไม่
                if (DataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("Invalid buffer data format!", "Error", MessageBoxButtons.OK);
                    return;
                }

                // แปลงข้อมูลและเก็บลงในรายการ XData และ YData
                for (int i = 0; i < DataPairs.Length; i += 2)
                {
                    if (double.TryParse(DataPairs[i], out double SourceValue) && double.TryParse(DataPairs[i + 1], out double MeasuredValue))
                    {
                        XData.Add(SourceValue);
                        YData.Add(MeasuredValue);
                    }
                }

                // แจ้งให้ฟอร์มอื่นทราบเมื่อการวัดเสร็จสิ้น
                MeasurementCompleted?.Invoke(XData, YData);

                // ถ้าคุณต้องการเรียกเมธอดของฟอร์มที่แสดงผลโดยตรง:
                //VdPTotalMeasureValuesForm.UpdateMeasurementData(XData, YData);
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveToGlobal();
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
    }
}