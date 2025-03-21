using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivi.Visa;
using Ivi.Visa.FormattedIO;
using Ivi.Visa.Interop;
using Keysight.Visa;

namespace Program01
{
    public partial class MeasurementSettingsForm : Form
    {
        // Fields
        public FormattedIO488 SMU;
        public Ivi.Visa.Interop.ResourceManager RsrcmngrSMU;
        public FormattedIO488 SS;
        public Ivi.Visa.Interop.ResourceManager RsrcmngrSS;

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

        public bool IsSMUConnected = false;
        public bool IsSSConnected = false;
        public bool IsModes = false;
        public bool IsFirstRun = true;

        public event Action<string, bool> ConnectionStatusChanged;
        public event EventHandler<bool> ModeChanged;
        public event EventHandler ToggleChanged;

        private Form CurrentChildForm;
        public DataChildForm DataChildForm = null;
        public ChannelSettingsChildForm ChannelSettingsChildForm = null;

        public List<double> XDataBuffer = new List<double>();
        public List<double> YDataBuffer = new List<double>();
        private List<double> X = new List<double>();
        private List<double> Y = new List<double>();

        private double LatestSourceValue;
        private double LatestMeasuredValue;

        public bool IsOn
        {
            get => IsModes;
            set
            {
                IsModes = value;
                UpdateToggleState();
            }
        }

        public MeasurementSettingsForm()
        {
            InitializeComponent();
            RsrcmngrSMU = new Ivi.Visa.Interop.ResourceManager();
            RsrcmngrSS = new Ivi.Visa.Interop.ResourceManager();
            InitializeGPIB();
        }

        private void InitializeGPIB()
        {
            try
            {
                ComboboxVISASMUIOPort.Items.Clear();
                ComboboxVISASSIOPort.Items.Clear();

                // ค้นหาอุปกรณ์ที่เชื่อมต่อจริง พร้อมอ่าน Response
                Dictionary<string, (string Address, string Response)> DeviceResponses = FindConnectedGPIBDevicesWithResponse();

                // คำหลักสำหรับระบุอุปกรณ์
                string SMUModelKeyword = "MODEL 2450";
                string SSModelKeyword = "MODEL 7001";

                // ค้นหาที่อยู่ของอุปกรณ์ที่ตรงกับ Model ที่ต้องการ
                string[] SMUAddresses = DeviceResponses
                    .Where(d => d.Value.Response.IndexOf(SMUModelKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(d => d.Value.Address)
                    .ToArray();

                string[] SSAddresses = DeviceResponses
                    .Where(d => d.Value.Response.IndexOf(SSModelKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(d => d.Value.Address)
                    .ToArray();

                // Debug: ตรวจสอบว่าค่าถูกต้องก่อนอัปเดต ComboBox
                Debug.WriteLine($"[DEBUG] SMU Addresses: {string.Join(", ", SMUAddresses)}");
                Debug.WriteLine($"[DEBUG] SS Addresses: {string.Join(", ", SSAddresses)}");

                // อัปเดต ComboBox สำหรับ SMU และ SS
                UpdateGPIBComboBox(ComboboxVISASMUIOPort, SMUAddresses, ComboboxVISASSIOPort);
                UpdateGPIBComboBox(ComboboxVISASSIOPort, SSAddresses, ComboboxVISASMUIOPort);

                // ตรวจสอบและสร้าง SMU ถ้ายังไม่ถูกกำหนด
                if (SMU == null && SMUAddresses.Length > 0)
                {
                    SMU = new FormattedIO488(); // สร้าง SMU ใหม่
                    SMU.IO = (IMessage)RsrcmngrSMU.Open(SMUAddresses[0]); // เชื่อมต่อกับ SMU ที่พบ
                }

                if (SS == null && SSAddresses.Length > 0)
                {
                    SS = new FormattedIO488(); // สร้าง SS ใหม่
                    SS.IO = (IMessage)RsrcmngrSS.Open(SSAddresses[0]); // เชื่อมต่อกับ SS ที่พบ
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถค้นหาอุปกรณ์ GPIB: {Ex.Message}");
            }
        }

        // ค้นหาอุปกรณ์ GPIB และอ่าน Response
        private Dictionary<string, (string Address, string Response)> FindConnectedGPIBDevicesWithResponse()
        {
            Dictionary<string, (string Address, string Response)> ConnectedDevices = new Dictionary<string, (string, string)>();

            try
            {
                string[] AllDevices = RsrcmngrSMU.FindRsrc("GPIB?*::?*::INSTR");

                foreach (string Device in AllDevices)
                {
                    FormattedIO488 Sessions = new FormattedIO488();

                    try
                    {
                        Sessions.IO = (IMessage)RsrcmngrSMU.Open(Device);
                        Sessions.WriteString("*IDN?", true); // ใช้ true เพื่อให้ส่ง Line Feed (\n)

                        string Response = Sessions.ReadString().Trim();

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
                        }
                        Marshal.FinalReleaseComObject(Sessions);
                    }
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถค้นหาอุปกรณ์ GPIB: {Ex.Message}");
            }

            return ConnectedDevices;
        }

        // อัปเดต ComboBox ด้วย Address ที่ค้นพบ
        private void UpdateGPIBComboBox(ComboBox Combobox, string[] GpibAddresses, ComboBox OtherCombobox)
        {
            Combobox.Items.Clear();

            if (GpibAddresses.Length == 0)
            {
                Debug.WriteLine($"[WARNING] ไม่มีที่อยู่ GPIB สำหรับ {Combobox.Name}");
                return;
            }

            // ดึงค่าจาก ComboBox อื่นเพื่อหลีกเลี่ยง Address ซ้ำ
            HashSet<string> OtherAddresses = new HashSet<string>(
                OtherCombobox.Items.Cast<string>().DefaultIfEmpty()
            );

            foreach (string DeviceAddress in GpibAddresses)
            {
                // Debug เช็กค่าที่จะเพิ่ม
                Debug.WriteLine($"[DEBUG] กำลังเพิ่ม {DeviceAddress} ไปที่ {Combobox.Name}");

                if (!OtherAddresses.Contains(DeviceAddress) || Combobox == OtherCombobox)
                {
                    Combobox.Items.Add(DeviceAddress);
                }
            }

            // เลือกอุปกรณ์แรกหากมี
            if (Combobox.Items.Count > 0)
            {
                Combobox.SelectedIndex = 0;
            }
        }

        public static class GlobalSettings
        {
            public static event Action OnSettingsChanged;

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
            public static bool IsSMUConnected { get; private set; }
            public static bool IsSSConnected { get; private set; }
            public static bool IsModes { get; private set; }
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR");
            }
        }

        private void UpdateSMUUIAfterConnection(string Message, bool IsSMUConnected, bool IsSMU)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateSMUUIAfterConnection(Message, IsSMUConnected, IsSMU)));
                return;
            }

            if (IsSMU)
            {
                MessageBox.Show(Message, IsSMUConnected ? "INFORMATION" : "ERROR", MessageBoxButtons.OK);
                IconbuttonSMUConnection.BackColor = IsSMUConnected ? Color.Snow : Color.Snow;
                IconbuttonSMUConnection.IconColor = IsSMUConnected ? Color.GreenYellow : Color.Gainsboro;
            }
        }

        private void UpdateSSUIAfterConnection(string Message, bool IsSSConnected, bool IsSS)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateSSUIAfterConnection(Message, IsSSConnected, IsSS)));
                return;
            }
            else
            {
                if (IsSS)
                {
                    MessageBox.Show(Message, IsSSConnected ? "INFORMATION" : "ERROR", MessageBoxButtons.OK);
                    IconbuttonSSConnection.BackColor = IsSSConnected ? Color.Snow : Color.Snow;
                    IconbuttonSSConnection.IconColor = IsSSConnected ? Color.GreenYellow : Color.Gainsboro;
                }
            }
        }

        private void IconbuttonSMUConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (SMU == null)
                {
                    MessageBox.Show("Error: SMU object is null.", "Connection Error", MessageBoxButtons.OK);
                    return;
                }

                if (!IsSMUConnected)
                {
                    // ตรวจสอบว่า SMU.IO ปลอดภัยจากการค้างก่อน
                    if (SMU.IO != null)
                    {
                        SMU.IO.Close();
                        Marshal.FinalReleaseComObject(SMU.IO); // ปล่อยทรัพยากรที่เกี่ยวข้อง
                        SMU.IO = null;
                    }

                    // สร้างออบเจ็กต์ SMU ใหม่ทุกครั้ง
                    SMU = new FormattedIO488();
                    SMU.IO = (IMessage)RsrcmngrSMU.Open(ComboboxVISASMUIOPort.SelectedItem.ToString()); // เชื่อมต่อใหม่

                    // เชื่อมต่อ SMU ใหม่
                    SMU.WriteString("*IDN?");
                    SMU.IO.Timeout = 5000;
                    string ConnectionResponse = SMU.ReadString();
                    SMU.WriteString("SYSTem:BEEPer 555, 0.3");

                    Debug.WriteLine($"{ConnectionResponse}");

                    IsSMUConnected = true;
                    UpdateSMUUIAfterConnection("Connected to SMU", true, true);
                }
                else
                {
                    // ตัดการเชื่อมต่อ SMU
                    try
                    {
                        // ปิดการเชื่อมต่อ SMU และปล่อยทรัพยากร
                        if (SMU.IO != null)
                        {
                            SMU.WriteString("*CLS");
                            SMU.WriteString("*RST");

                            SMU.IO.Close();
                            Marshal.FinalReleaseComObject(SMU.IO);  // ปล่อยทรัพยากร
                            SMU.IO = null;
                        }

                        // รีเซ็ตตัวแปรสถานะการเชื่อมต่อ
                        IsSMUConnected = false;

                        UpdateSMUUIAfterConnection("Disconnected from SMU", false, true);

                        // ไม่ตั้งค่า SMU เป็น null ที่นี่
                        // SMU = null; // ไม่ต้องทำการตั้งค่า SMU เป็น null
                    }
                    catch (Exception DisconnectEx)
                    {
                        MessageBox.Show($"Error during disconnection: {DisconnectEx.Message}", "Disconnection Error", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK);
            }
        }

        private void IconbuttonSSConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (SS == null)
                {
                    MessageBox.Show("Error: SS object is null.", "Connection Error", MessageBoxButtons.OK);
                    return;
                }

                if (!IsSSConnected)
                {
                    // ตรวจสอบว่า SMU.IO ปลอดภัยจากการค้างก่อน
                    if (SS.IO != null)
                    {
                        SS.IO.Close();
                        Marshal.FinalReleaseComObject(SS.IO); // ปล่อยทรัพยากรที่เกี่ยวข้อง
                        SS.IO = null;
                    }

                    // สร้างออบเจ็กต์ SS ใหม่ทุกครั้ง
                    SS = new FormattedIO488();
                    SS.IO = (IMessage)RsrcmngrSS.Open(ComboboxVISASSIOPort.SelectedItem.ToString()); // เชื่อมต่อใหม่

                    // เชื่อมต่อ SS ใหม่
                    SS.WriteString("*IDN?");
                    SS.IO.Timeout = 5000;
                    string ConnectionResponse = SS.ReadString();

                    Debug.WriteLine($"{ConnectionResponse}");

                    IsSSConnected = true;
                    UpdateSSUIAfterConnection("Connected to SS", true, true);
                }
                else
                {
                    // ตัดการเชื่อมต่อ SS
                    try
                    {
                        // ปิดการเชื่อมต่อ SS และปล่อยทรัพยากร
                        if (SS.IO != null)
                        {
                            SS.WriteString("*CLS");
                            SS.WriteString("*RST");

                            SS.IO.Close();
                            Marshal.FinalReleaseComObject(SS.IO);  // ปล่อยทรัพยากร
                            SS.IO = null;
                        }

                        // รีเซ็ตตัวแปรสถานะการเชื่อมต่อ
                        IsSSConnected = false;

                        UpdateSSUIAfterConnection("Disconnected from SS", false, true);

                        // ไม่ตั้งค่า SMU เป็น null ที่นี่
                        // SMU = null; // ไม่ต้องทำการตั้งค่า SMU เป็น null
                    }
                    catch (Exception DisconnectEx)
                    {
                        MessageBox.Show($"Error during disconnection: {DisconnectEx.Message}", "Disconnection Error", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK);
            }
        }

        private void MeasurementSettingsChildForm_Load(object sender, EventArgs e)
        {
            /*IconbuttonSMUConnection.Enabled = !IsSMUConnected;
            IconbuttonSSConnection.Enabled = !IsSSConnected;
            PanelToggleSwitchButton.Enabled = !IsModes;*/

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
            ComboboxStartUnit.SelectedItem = GlobalSettings.StartUnit;
            ComboboxStepUnit.SelectedItem = GlobalSettings.StepUnit;
            ComboboxStopUnit.SelectedItem = GlobalSettings.StopUnit;
            ComboboxSourceDelayUnit.SelectedItem = GlobalSettings.SourceDelayUnit;
            ComboboxSourceLimitLevelUnit.SelectedItem = GlobalSettings.SourceLimitLevelUnit;
            ComboboxThicknessUnit.SelectedItem = GlobalSettings.ThicknessUnit;
            ComboboxMagneticFieldsUnit.SelectedItem = GlobalSettings.MagneticFieldsUnit;

            TextboxStart.Text = GlobalSettings.StartValue;
            TextboxStop.Text = GlobalSettings.StopValue;
            TextboxStep.Text = GlobalSettings.StepValue;
            TextboxSourceDelay.Text = GlobalSettings.SourceDelayValue;
            TextboxSourceLimitLevel.Text = GlobalSettings.SourceLimitLevelValue;
            TextboxThickness.Text = GlobalSettings.ThicknessValue;
            TextboxRepetition.Text = GlobalSettings.RepetitionValue;
            TextboxMagneticFields.Text = GlobalSettings.MagneticFieldsValue;
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
                SourceLimitMode = ComboboxSourceLimitMode.SelectedItem?.ToString();
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
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

                SS.WriteString("ROUTe:OPEN ALL");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void PanelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                IsModes = !IsModes;
                UpdateToggleState();
                UpdateMeasurementMode(IsModes);
                OnToggleChanged();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        public void UpdateToggleState()
        {
            TargetPosition = IsModes ? PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width - 1 : 1;
            PanelToggleSwitchButton.Location = new Point(TargetPosition, PanelToggleSwitchButton.Location.Y);

            if (PanelToggleSwitchButton.Location.X < 0 || PanelToggleSwitchButton.Location.X > PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width)
            {
                PanelToggleSwitchButton.Location = new Point(1, PanelToggleSwitchButton.Location.Y);
            }
        }

        private void UpdateMeasurementMode(bool isHallMode)
        {
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
            ModeChanged?.Invoke(this, IsModes);

            PanelToggleSwitchBase.BackColor = IsModes ? Color.FromArgb(95, 77, 221) : Color.FromArgb(253, 138, 114);
        }

        private void PictureboxTuner1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (IsModes == true)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!4)");
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (IsModes == false)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");
                }
                else if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
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

                    if (IsModes == true)
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

                    if (IsModes == true)
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

                    if (IsModes == true)
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

                    if (IsModes == true)
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
                if (!IsSMUConnected && !IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
                    return;
                }

                SMU.WriteString("OUTPut OFF");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    return;
                }

                //CollectVdPTotalMeasurementClass.Instance.ClearMeasurements();
                RunMeasurement();

                if (IsModes)
                {
                    //CollectVdPTotalMeasurementClass.Instance.AddMeasurement();
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
                    UpdateMeasurementState();
                    ConfigureSourceMeasureUnit();
                    await ExecuteSweep();
                    TracingRunMeasurement();
                    FetchingData();
                    await Task.Delay(points * repetitionValue * 300);
                    CurrentTuner++;

                    if (CurrentTuner > 8)
                    {
                        Debug.WriteLine("All tuners completed.");
                        MessageBox.Show("Measurement completed", "Measurement Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SMU.WriteString("OUTPut OFF");
                        SS.WriteString("*CLS");
                        break;
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
            var channels = GetChannelConfiguration(CurrentTuner, IsModes);

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

            return configurations.ContainsKey(Tuner) ? configurations[Tuner] : new List<string>();
        }

        private void ConfigureSourceMeasureUnit()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                return;
            }

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

            SMU.WriteString("TRACe:CLEar");
            SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");
        }

        private async Task ExecuteSweep()
        {
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                return;
            }

            if (IsModes == true)
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
                MessageBox.Show("Invalid input values. Please ensure all fields are correctly filled.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Debug.WriteLine($"Measuring Tuner {CurrentTuner}");
        }

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsSMUConnected && IsSSConnected)
                {
                    MessageBox.Show("The instrument(s) is not connected", "Error", MessageBoxButtons.OK);
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
                    OpenChildForm(DataChildForm);
                }

                if (XDataBuffer.Count > 0 && YDataBuffer.Count > 0)
                {
                    DataChildForm.UpdateChart(XDataBuffer, YDataBuffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ButtonTunerSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChannelSettingsChildForm == null || ChannelSettingsChildForm.IsDisposed)
                {
                    ChannelSettingsChildForm = new ChannelSettingsChildForm(this);
                    OpenChildForm(ChannelSettingsChildForm);
                }
                else
                {
                    if (!ChannelSettingsChildForm.Visible)
                    {
                        ChannelSettingsChildForm.Show();
                    }
                    OpenChildForm(ChannelSettingsChildForm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ButtonTuner_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentChildForm?.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private bool ValidateInputs(out double start, out double stop, out double step, out int repetition, out double sourcelevellimit, out double thickness, out double magneticfields, out double delay, out int points)
        {
            start = stop = step = sourcelevellimit = thickness = magneticfields = delay = 0;
            repetition = points = 1;

            try
            {
                if (!ValidateInputConvert(TextboxStart.Text, ComboboxStartUnit.SelectedItem, out start) ||
                    !ValidateInputConvert(TextboxStop.Text, ComboboxStopUnit.SelectedItem, out stop) ||
                    !ValidateInputConvert(TextboxStep.Text, ComboboxStepUnit.SelectedItem, out step) ||
                    !ValidateInputConvert(TextboxSourceDelay.Text, ComboboxSourceDelayUnit.SelectedItem, out delay) ||
                    !ValidateInputConvert(TextboxSourceLimitLevel.Text, ComboboxSourceLimitLevelUnit.SelectedItem, out sourcelevellimit) ||
                    !ValidateInputConvert(TextboxThickness.Text, ComboboxThicknessUnit.SelectedItem, out thickness))
                {
                    return false;
                }

                if (IsModes)
                {
                    if (!ValidateInputConvert(TextboxMagneticFields.Text, ComboboxMagneticFieldsUnit.SelectedItem, out magneticfields))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Validation Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
                XDataBuffer?.Clear();
                YDataBuffer?.Clear();
                SMU.WriteString("TRACe:ACTual?");
                string BufferCount = SMU.ReadString().Trim();

                if (!int.TryParse(BufferCount, out int BufferPoints) || BufferPoints == 0)
                {
                    MessageBox.Show("No data in buffer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {BufferPoints}, 'defbuffer1', SOURce, READing");
                string MeasureRawData = SMU.ReadString().Trim();
                Debug.WriteLine($"Buffer contains: {BufferPoints} readings.");
                Debug.WriteLine($"Measured Raw Data: {MeasureRawData}");

                string[] DataPairs = MeasureRawData.Split(',');
                List<double> XData = new List<double>();
                List<double> YData = new List<double>();

                if (DataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("Invalid buffer data format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                for (int i = 0; i < DataPairs.Length; i += 2)
                {
                    if (double.TryParse(DataPairs[i], out double SourceValue) && double.TryParse(DataPairs[i + 1], out double MeasuredValue))
                    {
                        XData.Add(SourceValue);
                        YData.Add(MeasuredValue);

                        MaxSource = Math.Max(MaxSource, SourceValue);
                        MinSource = Math.Min(MinSource, SourceValue);
                        MaxMeasure = Math.Max(MaxMeasure, MeasuredValue);
                        MinMeasure = Math.Min(MinMeasure, MeasuredValue);

                        if (MaxSource != MinSource)
                        {
                            Slope = (MaxMeasure - MinMeasure) / (MaxSource - MinSource);
                        }
                    }
                }

                XDataBuffer = new List<double>(XData);
                YDataBuffer = new List<double>(YData);

                if (DataChildForm != null && !DataChildForm.IsDisposed)
                {
                    DataChildForm.UpdateChart(XDataBuffer, YDataBuffer);
                    DataChildForm.UpdateMeasurementData(MaxMeasure, MinMeasure, MaxSource, MinSource, Slope);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void TracingRunMeasurement()
        {
            try
            {
                SMU.WriteString("TRACe:ACTual?");
                string BufferCount = SMU.ReadString().Trim();

                if (!int.TryParse(BufferCount, out int BufferPoints) || BufferPoints == 0)
                {
                    MessageBox.Show("No data in buffer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {BufferPoints}, 'defbuffer1', SOURce, READing");
                string MeasureRawData = SMU.ReadString();
                Debug.WriteLine($"Buffer contains: {BufferPoints} readings");
                Debug.WriteLine($"Measured Raw Data: {MeasureRawData}");

                string[] DataPairs = MeasureRawData.Split(',');
                List<double> XData = new List<double>();
                List<double> YData = new List<double>();

                if (DataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("Invalid buffer data format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                for (int i = 0; i < DataPairs.Length; i += 2)
                {
                    if (double.TryParse(DataPairs[i], out double SourceValue) && double.TryParse(DataPairs[i + 1], out double MeasuredValue))
                    {
                        XData.Add(SourceValue);
                        YData.Add(MeasuredValue);
                    }
                }

                XDataBuffer = new List<double>(XData);
                YDataBuffer = new List<double>(YData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void FetchingData()
        {
            try
            {
                X.Clear();
                Y.Clear();
                SMU.WriteString("FETCh? 'defbuffer1', SOURce, READing");
                string LatestRawData = SMU.ReadString().Trim();
                string[] DataPairs = LatestRawData.Split(',');

                if (DataPairs.Length >= 2)
                {
                    if (double.TryParse(DataPairs[0], out double SourceValue) && double.TryParse(DataPairs[1], out double MeasuredValue))
                    {
                        LatestSourceValue = SourceValue;
                        LatestMeasuredValue = MeasuredValue;
                        X.Add(SourceValue);
                        Y.Add(MeasuredValue);
                    }

                    else
                    {
                        Debug.WriteLine("Error: Unable to parse values.");
                    }
                }
                else
                {
                    Debug.WriteLine("Error: Insufficient data.");
                }

                Debug.WriteLine($"Source Value at Tuner {CurrentTuner}: {LatestSourceValue}, Measure Value at Tuner {CurrentTuner}: {LatestMeasuredValue}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void MeasurementSettingsChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveToGlobal();
        }

        private void IconbuttonMeasure_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            CollectVdPVoltage.Instance.ClearMeasurements();

            // กำหนดจำนวนแถวที่ต้องการ
            int NumberOfRows = rand.Next(5, 10);

            // สร้างค่าการวัดสุ่ม
            for (int i = 0; i < NumberOfRows; i++)
            {
                double RandomVoltage1 = rand.NextDouble() * 21;
                double RandomVoltage2 = rand.NextDouble() * 21;
                double RandomVoltage3 = rand.NextDouble() * 21;
                double RandomVoltage4 = rand.NextDouble() * 21;
                double RandomVoltage5 = rand.NextDouble() * 21;
                double RandomVoltage6 = rand.NextDouble() * 21;
                double RandomVoltage7 = rand.NextDouble() * 21;
                double RandomVoltage8 = rand.NextDouble() * 21;

                CollectVdPVoltage.Instance.AddMeasurement(RandomVoltage1, RandomVoltage2, RandomVoltage3, RandomVoltage4, RandomVoltage5, RandomVoltage6, RandomVoltage7, RandomVoltage8);
            }

            MessageBox.Show("บันทึกค่าการวัดเรียบร้อยแล้ว!");
        }
    }
}