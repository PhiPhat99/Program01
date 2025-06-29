﻿using FontAwesome.Sharp;
using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public event EventHandler<MeasurementMode> ModeChanged;
        public event EventHandler ToggleChanged;

        private Form CurrentChildForm;
        private DataChildForm DataChildForm = null;

        public List<double> XDataBuffer = new List<double>();
        public List<double> YDataBuffer = new List<double>();

        private List<Dictionary<int, List<(double Source, double Reading)>>> allHallMeasurements = new List<Dictionary<int, List<(double, double)>>>();

        public bool IsVanderPauwOrHallToggle { get; set; } = false;


        public MeasurementSettingsForm()
        {
            InitializeComponent();
            Rsrcmngr = new ResourceManager();
            InitializeGPIB();
            InitializeToggleStateFromGlobalSettings();
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

        private void InitializeToggleStateFromGlobalSettings()
        {
            if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement)
            {
                IsVanderPauwOrHallToggle = true;
            }
            else
            {
                IsVanderPauwOrHallToggle = false;
            }

            Debug.WriteLine($"[DEBUG] Form Load - Initial IsVanderPauwOrHallToggle: {IsVanderPauwOrHallToggle}");
            Debug.WriteLine($"[DEBUG] Form Load - Initial CurrentMeasurementMode: {GlobalSettings.Instance.CurrentMeasurementMode}");

            UpdateToggleState();
            UpdateMeasurementMode();
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
                Debug.WriteLine($"[LOAD] RsenseMode (From GlobalSettings): {GlobalSettings.Instance.ResistanceSenseModeUI}");
                SetComboBoxSelectedItem(ComboboxRsense, GlobalSettings.Instance.ResistanceSenseModeUI);

                Debug.WriteLine($"[LOAD] MeasureMode (From GlobalSettings): {GlobalSettings.Instance.MeasureModeUI}");
                SetComboBoxSelectedItem(ComboboxMeasure, GlobalSettings.Instance.MeasureModeUI);

                Debug.WriteLine($"[LOAD] SourceMode (From GlobalSettings): {GlobalSettings.Instance.SourceModeUI}");
                SetComboBoxSelectedItem(ComboboxSource, GlobalSettings.Instance.SourceModeUI);

                Debug.WriteLine($"[LOAD] SourceLimitType (From GlobalSettings): {GlobalSettings.Instance.SourceLimitModeUI}");
                SetComboBoxSelectedItem(ComboboxSourceLimitMode, GlobalSettings.Instance.SourceLimitModeUI);

                Debug.WriteLine($"[LOAD] StartUnit (From GlobalSettings): {GlobalSettings.Instance.StartUnitUI}");
                SetComboBoxSelectedItem(ComboboxStartUnit, GlobalSettings.Instance.StartUnitUI);

                Debug.WriteLine($"[LOAD] StopUnit (From GlobalSettings): {GlobalSettings.Instance.StopUnitUI}");
                SetComboBoxSelectedItem(ComboboxStopUnit, GlobalSettings.Instance.StopUnitUI);

                Debug.WriteLine($"[LOAD] StepUnit (From GlobalSettings): {GlobalSettings.Instance.StepUnitUI}");
                SetComboBoxSelectedItem(ComboboxStepUnit, GlobalSettings.Instance.StepUnitUI);

                Debug.WriteLine($"[LOAD] SourceDelayUnit (From GlobalSettings): {GlobalSettings.Instance.SourceDelayUnitUI}");
                SetComboBoxSelectedItem(ComboboxSourceDelayUnit, GlobalSettings.Instance.SourceDelayUnitUI);

                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnitUI}");
                SetComboBoxSelectedItem(ComboboxSourceLimitLevelUnit, GlobalSettings.Instance.SourceLimitLevelUnitUI);
                Debug.WriteLine($"[LOAD] SourceLimitLevelUnit (To UI): {ComboboxSourceLimitLevelUnit.SelectedItem?.ToString()}");

                Debug.WriteLine($"[LOAD] ThicknessUnit (From GlobalSettings): {GlobalSettings.Instance.ThicknessUnitUI}");
                SetComboBoxSelectedItem(ComboboxThicknessUnit, GlobalSettings.Instance.ThicknessUnitUI);

                Debug.WriteLine($"[LOAD] MagneticFieldsUnit (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnitUI}");
                SetComboBoxSelectedItem(ComboboxMagneticFieldsUnit, GlobalSettings.Instance.MagneticFieldsUnitUI);

                Debug.WriteLine($"[LOAD] StartValue (From GlobalSettings): {GlobalSettings.Instance.StartValueUI}");
                SetTextboxValue(TextboxStart, GlobalSettings.Instance.StartValueUI.ToString());

                Debug.WriteLine($"[LOAD] StopValue (From GlobalSettings): {GlobalSettings.Instance.StopValueUI}");
                SetTextboxValue(TextboxStop, GlobalSettings.Instance.StopValueUI.ToString());

                Debug.WriteLine($"[LOAD] StepValue (From GlobalSettings): {GlobalSettings.Instance.StepValueUI}");
                SetTextboxValue(TextboxStep, GlobalSettings.Instance.StepValueUI.ToString());

                Debug.WriteLine($"[LOAD] SourceDelayValue (From GlobalSettings): {GlobalSettings.Instance.SourceDelayValueUI}");
                SetTextboxValue(TextboxSourceDelay, GlobalSettings.Instance.SourceDelayValueUI.ToString());

                Debug.WriteLine($"[LOAD] SourceLimitLevelValue (From GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelValueUI}");
                SetTextboxValue(TextboxSourceLimitLevel, GlobalSettings.Instance.SourceLimitLevelValueUI.ToString());

                Debug.WriteLine($"[LOAD] ThicknessValue (From GlobalSettings): {GlobalSettings.Instance.ThicknessValueUI}");
                SetTextboxValue(TextboxThickness, GlobalSettings.Instance.ThicknessValueUI.ToString());

                Debug.WriteLine($"[LOAD] RepetitionValue (From GlobalSettings): {GlobalSettings.Instance.RepetitionValueUI}");
                SetTextboxValue(TextboxRepetition, GlobalSettings.Instance.RepetitionValueUI.ToString());

                Debug.WriteLine($"[LOAD] MagneticFieldsValue (From GlobalSettings): {GlobalSettings.Instance.MagneticFieldsValueUI}");
                SetTextboxValue(TextboxMagneticFields, GlobalSettings.Instance.MagneticFieldsValueUI.ToString());
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

            ComboboxRsense.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxRsense.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxMeasure.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxMeasure.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSource.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxSource.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitMode.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitMode.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();

            ComboboxStartUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxStartUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxStepUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxStepUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxStopUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxStopUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();

            ComboboxSourceDelayUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxSourceDelayUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitLevelUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxSourceLimitLevelUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();

            ComboboxThicknessUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxThicknessUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();
            ComboboxMagneticFieldsUnit.SelectedIndexChanged -= (s, ea) => SaveToGlobalSettings();
            ComboboxMagneticFieldsUnit.SelectedIndexChanged += (s, ea) => SaveToGlobalSettings();

            TextboxStart.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxStart.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxStep.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxStep.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxStop.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxStop.TextChanged += (s, ea) => SaveToGlobalSettings();

            TextboxSourceDelay.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxSourceDelay.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxSourceLimitLevel.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxSourceLimitLevel.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxRepetition.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxRepetition.TextChanged += (s, ea) => SaveToGlobalSettings();

            TextboxThickness.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxThickness.TextChanged += (s, ea) => SaveToGlobalSettings();
            TextboxMagneticFields.TextChanged -= (s, ea) => SaveToGlobalSettings();
            TextboxMagneticFields.TextChanged += (s, ea) => SaveToGlobalSettings();
        }

        private void SaveToGlobalSettings()
        {
            Debug.WriteLine("[เริ่มต้นการตั้งค่าบันทึกข้อมูล]");

            try
            {
                GlobalSettings.Instance.ResistanceSenseModeUI = ComboboxRsense.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] RsenseMode (To GlobalSettings): {GlobalSettings.Instance.ResistanceSenseModeUI}");

                GlobalSettings.Instance.MeasureModeUI = ComboboxMeasure.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MeasureMode (To GlobalSettings): {GlobalSettings.Instance.MeasureModeUI}");

                GlobalSettings.Instance.SourceModeUI = ComboboxSource.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceMode (To GlobalSettings): {GlobalSettings.Instance.SourceModeUI}");

                GlobalSettings.Instance.SourceLimitModeUI = ComboboxSourceLimitMode.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitType (To GlobalSettings): {GlobalSettings.Instance.SourceLimitModeUI}");

                GlobalSettings.Instance.StartUnitUI = ComboboxStartUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StartUnit (To GlobalSettings): {GlobalSettings.Instance.StartUnitUI}");

                GlobalSettings.Instance.StopUnitUI = ComboboxStopUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StopUnit (To GlobalSettings): {GlobalSettings.Instance.StopUnitUI}");

                GlobalSettings.Instance.StepUnitUI = ComboboxStepUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] StepUnit (To GlobalSettings): {GlobalSettings.Instance.StepUnitUI}");

                GlobalSettings.Instance.SourceDelayUnitUI = ComboboxSourceDelayUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceDelayUnit (To GlobalSettings): {GlobalSettings.Instance.SourceDelayUnitUI}");

                GlobalSettings.Instance.SourceLimitLevelUnitUI = ComboboxSourceLimitLevelUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] SourceLimitLevelUnit (To GlobalSettings): {GlobalSettings.Instance.SourceLimitLevelUnitUI}");

                GlobalSettings.Instance.ThicknessUnitUI = ComboboxThicknessUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] ThicknessUnit (To GlobalSettings): {GlobalSettings.Instance.ThicknessUnitUI}");

                GlobalSettings.Instance.MagneticFieldsUnitUI = ComboboxMagneticFieldsUnit.SelectedItem?.ToString() ?? "";
                Debug.WriteLine($"[SAVE] MagneticFieldsUnit (To GlobalSettings): {GlobalSettings.Instance.MagneticFieldsUnitUI}");

                if (double.TryParse(TextboxStart.Text, out double startValue))
                {
                    string selectedStartUnit = ComboboxStartUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedStartUnit))
                    {
                        double standardStart = ConvertValueBasedOnUnit(selectedStartUnit, startValue);

                        GlobalSettings.Instance.StartValueUI = startValue;
                        GlobalSettings.Instance.StartUnitUI = selectedStartUnit;
                        GlobalSettings.Instance.StartValueStd = standardStart;
                        Debug.WriteLine($"[SAVE] Start Value UI: {GlobalSettings.Instance.StartValueUI}, Start Unit UI: {GlobalSettings.Instance.StartUnitUI}, Start Standard Value: {GlobalSettings.Instance.StartValueStd}");
                    }
                }

                if (double.TryParse(TextboxStep.Text, out double stepValue))
                {
                    string selectedStepUnit = ComboboxStepUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedStepUnit))
                    {
                        double standardStep = ConvertValueBasedOnUnit(selectedStepUnit, stepValue);

                        GlobalSettings.Instance.StepValueUI = stepValue;
                        GlobalSettings.Instance.StepUnitUI = selectedStepUnit;
                        GlobalSettings.Instance.StepValueStd = standardStep;
                        Debug.WriteLine($"[SAVE] Step Value UI: {GlobalSettings.Instance.StepValueUI}, Step Unit UI: {GlobalSettings.Instance.StepUnitUI}, Step Standard Value: {GlobalSettings.Instance.StepValueStd}");
                    }
                }

                if (double.TryParse(TextboxStop.Text, out double stopValue))
                {
                    string selectedStopUnit = ComboboxStopUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedStopUnit))
                    {
                        double standardStop = ConvertValueBasedOnUnit(selectedStopUnit, stopValue);

                        GlobalSettings.Instance.StopValueUI = stopValue;
                        GlobalSettings.Instance.StopUnitUI = selectedStopUnit;
                        GlobalSettings.Instance.StopValueStd = standardStop;
                        Debug.WriteLine($"[SAVE] Stop Value UI: {GlobalSettings.Instance.StopValueUI}, Stop Unit UI: {GlobalSettings.Instance.StopUnitUI}, Stop Standard Value: {GlobalSettings.Instance.StopValueStd}");
                    }
                }

                if (double.TryParse(TextboxSourceDelay.Text, out double delayValue))
                {
                    string selectedSourceDelayUnit = ComboboxSourceDelayUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedSourceDelayUnit))
                    {
                        double standardSourceDelay = ConvertValueBasedOnUnit(selectedSourceDelayUnit, delayValue);

                        GlobalSettings.Instance.SourceDelayValueUI = delayValue;
                        GlobalSettings.Instance.SourceDelayUnitUI = selectedSourceDelayUnit;
                        GlobalSettings.Instance.SourceDelayValueStd = standardSourceDelay;
                        Debug.WriteLine($"[SAVE] Delay Value UI: {GlobalSettings.Instance.SourceDelayValueUI}, Delay Unit UI: {GlobalSettings.Instance.SourceDelayUnitUI}, Delay Standard Value: {GlobalSettings.Instance.SourceDelayValueStd}");
                    }
                }

                if (double.TryParse(TextboxSourceLimitLevel.Text, out double limitlevelValue))
                {
                    string selectedSourceLimitLevelUnit = ComboboxSourceLimitLevelUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedSourceLimitLevelUnit))
                    {
                        double standardSourceLimitLevel = ConvertValueBasedOnUnit(selectedSourceLimitLevelUnit, limitlevelValue);

                        GlobalSettings.Instance.SourceLimitLevelValueUI = limitlevelValue;
                        GlobalSettings.Instance.SourceLimitLevelUnitUI = selectedSourceLimitLevelUnit;
                        GlobalSettings.Instance.SourceLimitLevelValueStd = standardSourceLimitLevel;
                        Debug.WriteLine($"[SAVE] Limit Level Value UI: {GlobalSettings.Instance.SourceLimitLevelValueUI}, Limit Level Unit UI: {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Limit Level Standard Value: {GlobalSettings.Instance.SourceLimitLevelValueStd}");
                    }
                }

                if (double.TryParse(TextboxThickness.Text, out double thicknessValue))
                {
                    string selectedThicknessUnit = ComboboxThicknessUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedThicknessUnit))
                    {
                        double standardThickness = ConvertValueBasedOnUnit(selectedThicknessUnit, thicknessValue);

                        GlobalSettings.Instance.ThicknessValueUI = thicknessValue;
                        GlobalSettings.Instance.ThicknessUnitUI = selectedThicknessUnit;
                        GlobalSettings.Instance.ThicknessValueStd = standardThickness;
                        Debug.WriteLine($"[SAVE] Thickness Value UI: {GlobalSettings.Instance.ThicknessValueUI}, Thickness Unit UI: {GlobalSettings.Instance.ThicknessUnitUI}, Thickness Standard Value: {GlobalSettings.Instance.ThicknessValueStd}");
                    }
                }

                if (int.TryParse(TextboxRepetition.Text, out int repetitionValue))
                {
                    GlobalSettings.Instance.RepetitionValueUI = repetitionValue;
                    Debug.WriteLine($"[SAVE] Repetition Value (To GlobalSettings): {GlobalSettings.Instance.RepetitionValueUI}");
                }

                if (double.TryParse(TextboxMagneticFields.Text, out double magneticfieldsValue))
                {
                    string selectedMagneticFieldsUnit = ComboboxMagneticFieldsUnit.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(selectedMagneticFieldsUnit))
                    {
                        double standardMagneticFields = ConvertValueBasedOnUnit(selectedMagneticFieldsUnit, magneticfieldsValue);

                        GlobalSettings.Instance.MagneticFieldsValueUI = magneticfieldsValue;
                        GlobalSettings.Instance.MagneticFieldsUnitUI = selectedMagneticFieldsUnit;
                        GlobalSettings.Instance.MagneticFieldsValueStd = standardMagneticFields;
                        Debug.WriteLine($"[SAVE] Magnetic Fields Value UI: {GlobalSettings.Instance.MagneticFieldsValueUI}, Magnetic Fields Unit UI: {GlobalSettings.Instance.MagneticFieldsUnitUI}, Magnetic Fields Standard Value: {GlobalSettings.Instance.MagneticFieldsValueStd}");
                    }
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
                GlobalSettings.Instance.ResistanceSenseModeUI = ComboboxRsense.SelectedItem?.ToString();

                if (GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    switch (GlobalSettings.Instance.ResistanceSenseModeUI)
                    {
                        case "2-Wires":
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            GlobalSettings.Instance.ResistanceSenseModeUI = "";
                            break;
                    }
                }
                else
                {
                    switch (GlobalSettings.Instance.ResistanceSenseModeUI)
                    {
                        case "2-Wires":
                            break;
                        case "4-Wires":
                            break;
                        default:
                            ComboboxRsense.SelectedIndex = -1;
                            GlobalSettings.Instance.ResistanceSenseModeUI = "";
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
                GlobalSettings.Instance.MeasureModeUI = ComboboxMeasure.SelectedItem?.ToString();

                switch (GlobalSettings.Instance.MeasureModeUI)
                {
                    case "Voltage":
                        break;
                    case "Current":
                        break;
                    default:
                        ComboboxMeasure.SelectedIndex = -1;
                        GlobalSettings.Instance.MeasureModeUI = "";
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
                GlobalSettings.Instance.SourceModeUI = ComboboxSource.SelectedItem?.ToString();

                switch (GlobalSettings.Instance.SourceModeUI)
                {
                    case "Voltage":
                        UpdateMeasurementSettingsUnits();
                        break;
                    case "Current":
                        UpdateMeasurementSettingsUnits();
                        break;
                    default:
                        ComboboxSource.SelectedIndex = -1;
                        GlobalSettings.Instance.SourceModeUI = "";
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
                GlobalSettings.Instance.SourceLimitModeUI = ComboboxSourceLimitMode.SelectedItem?.ToString();
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
                if (GlobalSettings.Instance.SourceModeUI == "Voltage")
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
                else if (GlobalSettings.Instance.SourceModeUI == "Current")
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
            ResetSettings();
        }

        private void ResetSettings()
        {
            Debug.WriteLine("[DEBUG] ResetSettings - Start");
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถล้างข้อมูลการตั้งค่าได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการลบล้างข้อมูล", MessageBoxButtons.OK);
                    Debug.WriteLine("[DEBUG] ResetSettings - เครื่องมือไม่ได้เชื่อมต่อ");
                    return;
                }

                Debug.WriteLine("[DEBUG] ResetSettings - ส่งคำสั่ง Reset ไปยัง SMU และ SS");
                SMU.WriteString("*RST");
                SS.WriteString("*RST");
                SS.WriteString("ROUTe:OPEN ALL");

                Debug.WriteLine("[DEBUG] ResetSettings - กำหนดค่า UI Controls เป็นค่าเริ่มต้นจาก GlobalSettings");

                ComboboxRsense.SelectedItem = "4-Wires";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxRsense.SelectedItem set to: {ComboboxRsense.SelectedItem}");

                ComboboxMeasure.SelectedItem = "Voltage";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxMeasure.SelectedItem set to: {ComboboxMeasure.SelectedItem}");

                ComboboxSource.SelectedItem = "Current";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxSource.SelectedItem set to: {ComboboxSource.SelectedItem}");

                ComboboxSourceLimitMode.SelectedItem = "Voltage";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxSourceLimitMode.SelectedItem set to: {ComboboxSourceLimitMode.SelectedItem}");

                ComboboxStartUnit.SelectedItem = "mA";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxStartUnit.SelectedItem set to: {ComboboxStartUnit.SelectedItem}");

                ComboboxStopUnit.SelectedItem = "mA";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxStopUnit.SelectedItem set to: {ComboboxStopUnit.SelectedItem}");

                ComboboxStepUnit.SelectedItem = "µA";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxStepUnit.SelectedItem set to: {ComboboxStepUnit.SelectedItem}");

                ComboboxSourceDelayUnit.SelectedItem = "µs";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxSourceDelayUnit.SelectedItem set to: {ComboboxSourceDelayUnit.SelectedItem}");

                ComboboxSourceLimitLevelUnit.SelectedItem = "V";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxSourceLimitLevelUnit.SelectedItem set to: {ComboboxSourceLimitLevelUnit.SelectedItem}");

                ComboboxThicknessUnit.SelectedItem = "µm";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxThicknessUnit.SelectedItem set to: {ComboboxThicknessUnit.SelectedItem}");

                ComboboxMagneticFieldsUnit.SelectedItem = "T";
                Debug.WriteLine($"[DEBUG] ResetSettings - ComboboxMagneticFieldsUnit.SelectedItem set to: {ComboboxMagneticFieldsUnit.SelectedItem}");

                TextboxStart.Text = "0";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxStart.Text set to: {TextboxStart.Text}");

                TextboxStep.Text = "0";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxStep.Text set to: {TextboxStep.Text}");

                TextboxStop.Text = "0";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxStop.Text set to: {TextboxStop.Text}");

                TextboxSourceDelay.Text = "100";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxSourceDelay.Text set to: {TextboxSourceDelay.Text}");

                TextboxSourceLimitLevel.Text = "21";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxSourceLimitLevel.Text set to: {TextboxSourceLimitLevel.Text}");

                TextboxThickness.Text = "0";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxThickness.Text set to: {TextboxThickness.Text}");

                TextboxRepetition.Text = "1";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxRepetition.Text set to: {TextboxRepetition.Text}");

                TextboxMagneticFields.Text = "0.55";
                Debug.WriteLine($"[DEBUG] ResetSettings - TextboxMagneticFields.Text set to: {TextboxMagneticFields.Text}");

                Debug.WriteLine("[DEBUG] ResetSettings - UI Controls ถูกกำหนดค่าเป็นค่าเริ่มต้นจาก GlobalSettings แล้ว");
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"การล้างข้อมูลการตั้งค่าไม่สำเร็จ: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
                Debug.WriteLine($"[ERROR] ResetSettings - เกิดข้อผิดพลาด: {Ex.Message}");
            }
            Debug.WriteLine("[DEBUG] ResetSettings - End");
        }

        public void PanelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                IsVanderPauwOrHallToggle = !IsVanderPauwOrHallToggle;
                Debug.WriteLine($"The Value of IsVanderPauwOrHallToggle boolean variable is: {IsVanderPauwOrHallToggle}");

                GlobalSettings.Instance.CurrentMeasurementMode = IsVanderPauwOrHallToggle ? MeasurementMode.HallEffectMeasurement : MeasurementMode.VanDerPauwMethod;
                Debug.WriteLine($"[DEBUG] Current Measurement Mode set to: {GlobalSettings.Instance.CurrentMeasurementMode}");

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
            TargetPosition = IsVanderPauwOrHallToggle ? PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width - 1 : 1;
            PanelToggleSwitchButton.Location = new Point(TargetPosition, PanelToggleSwitchButton.Location.Y);

            if (PanelToggleSwitchButton.Location.X < 0 || PanelToggleSwitchButton.Location.X > PanelToggleSwitchBase.Width - PanelToggleSwitchButton.Width)
            {
                PanelToggleSwitchButton.Location = new Point(1, PanelToggleSwitchButton.Location.Y);
            }
        }

        private void UpdateMeasurementMode()
        {
            bool isHallModeSelected = GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement;
            string modeName = isHallModeSelected ? "Hall Effect" : "Van der Pauw";

            Debug.WriteLine($"[DEBUG] UpdateMeasurementMode - Current Mode: {modeName}");

            if (!isHallModeSelected)
            {
                LabelToggleSwitchVdP.ForeColor = Color.FromArgb(144, 198, 101);
                LabelToggleSwitchHall.ForeColor = SystemColors.ActiveCaptionText;
                PanelToggleSwitchButton.BackColor = Color.FromArgb(253, 138, 114);
                PanelToggleSwitchBase.BackColor = Color.FromArgb(253, 138, 114);
            }
            else
            {
                LabelToggleSwitchVdP.ForeColor = SystemColors.ActiveCaptionText;
                LabelToggleSwitchHall.ForeColor = Color.FromArgb(144, 198, 101);
                PanelToggleSwitchButton.BackColor = Color.FromArgb(95, 77, 221);
                PanelToggleSwitchBase.BackColor = Color.FromArgb(95, 77, 221);
            }

            Debug.WriteLine($"[DEBUG] UpdateMeasurementMode - PanelToggleSwitchBase.BackColor: {PanelToggleSwitchBase.BackColor.ToString()}");
            UpdateTunerImages();
        }

        private void UpdateTunerImages()
        {
            if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement)
            {
                PictureboxMeasPosition1.Image = Properties.Resources.Hall_V1;
                PictureboxMeasPosition2.Image = Properties.Resources.Hall_V2;
                PictureboxMeasPosition3.Image = Properties.Resources.Hall_V3;
                PictureboxMeasPosition4.Image = Properties.Resources.Hall_V4;
                PictureboxMeasPosition5.Image = null;
                PictureboxMeasPosition6.Image = null;
                PictureboxMeasPosition7.Image = null;
                PictureboxMeasPosition8.Image = null;
            }
            else
            {
                PictureboxMeasPosition1.Image = Properties.Resources.VdP_RA1;
                PictureboxMeasPosition2.Image = Properties.Resources.VdP_RA2;
                PictureboxMeasPosition3.Image = Properties.Resources.VdP_RA3;
                PictureboxMeasPosition4.Image = Properties.Resources.VdP_RA4;
                PictureboxMeasPosition5.Image = Properties.Resources.VdP_RB1;
                PictureboxMeasPosition6.Image = Properties.Resources.VdP_RB2;
                PictureboxMeasPosition7.Image = Properties.Resources.VdP_RB3;
                PictureboxMeasPosition8.Image = Properties.Resources.VdP_RB4;
            }
        }

        protected virtual void OnToggleChanged()
        {
            ToggleChanged?.Invoke(this, EventArgs.Empty);
            ModeChanged?.Invoke(this, GlobalSettings.Instance.CurrentMeasurementMode);
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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M1");
                }
                else
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M9");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M2");

                }
                else
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M10");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");

                    SS.WriteString("ROUTe:MEMory:SAVE M3");

                }
                else
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");

                    SS.WriteString("ROUTe:MEMory:SAVE M11");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");

                    SS.WriteString("ROUTe:MEMory:SAVE M4");

                }
                else
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!5)");

                    SS.WriteString("ROUTe:MEMory:SAVE M12");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M5");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!3)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!6)");

                    SS.WriteString("ROUTe:MEMory:SAVE M6");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");

                    SS.WriteString("ROUTe:MEMory:SAVE M7");

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

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    SS.WriteString("ROUTe:OPEN ALL");
                    SS.WriteString("ROUTe:CLOSe (@ 1!1!6)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!2!5)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!3!4)");
                    SS.WriteString("ROUTe:CLOSe (@ 1!4!3)");

                    SS.WriteString("ROUTe:MEMory:SAVE M8");

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
            }
        }

        private void IconbuttonTunerTest_Click(object sender, EventArgs e)
        {
            DisableEditRun(true);

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

                if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.ResistanceSenseModeUI == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
                    Debug.WriteLine($"{allValues}");

                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceModeUI == "Voltage" && GlobalSettings.Instance.MeasureModeUI == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion VOLTage");
                    SMU.WriteString($"SOURce:VOLTage:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.ResistanceSenseModeUI == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
                    Debug.WriteLine($"{allValues}");


                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Voltage")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                    SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.ResistanceSenseModeUI == "4-Wires")
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:VOLTage:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
                    Debug.WriteLine($"{allValues}");


                    SMU.WriteString(sweepCommand);
                    SMU.WriteString("OUTPut ON");
                    SMU.WriteString("INITiate");
                    SMU.WriteString("*WAI");
                    TracingTunerData();
                    SMU.WriteString("OUTPut OFF");
                }

                else if (GlobalSettings.Instance.SourceModeUI == "Current" && GlobalSettings.Instance.MeasureModeUI == "Current")
                {
                    SMU.WriteString($"SOURce:FUNCtion CURRent");
                    SMU.WriteString($"SOURce:CURRent:RANG:AUTO ON");
                    SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                    SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                    SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
                    SMU.WriteString("TRACe:CLEar");
                    SMU.WriteString("TRACe:POINts 1000000, 'defbuffer1'");

                    if (GlobalSettings.Instance.ResistanceSenseModeUI == "4-Wires")
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse ON");
                    }
                    else
                    {
                        SMU.WriteString("SENSe:CURRent:RSENse OFF");
                    }

                    string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue},  {repetitionValue}";
                    Debug.WriteLine($"Sending command: {sweepCommand}");

                    string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
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
            finally
            {
                DisableEditRun(false);
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
                        xData.Add(measuredValue);
                        yData.Add(sourceValue);

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
                    slope = Math.Abs(1 / ((maxSource - minSource) / (maxMeasure - minMeasure)));
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
            Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - Start");
            DisableEditRun(true);

            try
            {
                Debug.WriteLine($"[DEBUG] IconbuttonRunMeasurement_Click - IsSMUConnected: {GlobalSettings.Instance.IsSMUConnected}, IsSSConnected: {GlobalSettings.Instance.IsSSConnected}");
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถทำการวัดได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการวัด", MessageBoxButtons.OK);
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เครื่องมือไม่ได้เชื่อมต่อ");
                    return;
                }

                SMU.WriteString("OUTPut OFF");
                Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - ส่งคำสั่ง 'OUTPut OFF' ไปยัง SMU");

                if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
                {
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - การ Validate Inputs ไม่สำเร็จ");
                    return;
                }

                SaveToGlobalSettings();
                Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - บันทึกค่าลง GlobalSettings แล้ว");
                Debug.WriteLine($"[DEBUG] IconbuttonRunMeasurement_Click - CurrentMeasurementMode: {GlobalSettings.Instance.CurrentMeasurementMode}");

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เริ่มการวัด Van der Pauw");
                    await RunVanDerPauwMeasurement(startValue, stopValue, stepValue, repetitionValue, sourcelevellimitValue, thicknessValue, magneticfieldsValue, delayValue, points);

                    DialogResult resultHall = MessageBox.Show($"ทำการวัด Van der Pauw เสร็จสิ้นแล้ว ต้องการทำการวัด Hall Effect Measurement ต่อหรือไม่ ?", "การวัดเสร็จสิ้น", MessageBoxButtons.YesNo);
                    Debug.WriteLine($"[DEBUG] IconbuttonRunMeasurement_Click - ผลลัพธ์ MessageBox Van der Pauw เสร็จสิ้น: {resultHall}");

                    if (resultHall == DialogResult.Yes)
                    {
                        GlobalSettings.Instance.CurrentMeasurementMode = MeasurementMode.HallEffectMeasurement;
                        Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เปลี่ยนไปโหมด Hall Effect และเริ่มการวัด");
                        await RunHallMeasurementSequence();

                        // *** ตำแหน่งที่ 1: สำหรับกรณีที่วัด Van der Pauw แล้วเลือกวัด Hall ต่อ ***
                        GlobalSettings.Instance.CollectedHallMeasurements.DebugPrintAllRawMeasurements();
                        // *******************************************************************
                    }
                }
                else if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement)
                {
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เริ่มการวัด Hall Effect โดยตรง");
                    await RunHallMeasurementSequence();

                    // *** ตำแหน่งที่ 2: สำหรับกรณีที่เลือกวัด Hall Effect โดยตรง ***
                    GlobalSettings.Instance.CollectedHallMeasurements.DebugPrintAllRawMeasurements();
                    // **********************************************************

                    // บรรทัดนี้ควรอยู่หลังการเรียก DebugPrintAllRawMeasurements()
                    GlobalSettings.Instance.CollectedHallMeasurements.CalculateAllHallProperties(GlobalSettings.Instance.ThicknessValueStd, GlobalSettings.Instance.MagneticFieldsValueStd);
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - Hall Effect Measurement completed and calculation initiated.");
                    GlobalSettings.Instance.HallMeasurementDataReady = true;
                }
                else // นี่คือส่วนที่เป็นค่า Default หรือกรณีที่ไม่ตรงกับเงื่อนไขด้านบน
                {
                    Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เริ่มการวัด Van der Pauw (default)");
                    await RunVanDerPauwMeasurement(startValue, stopValue, stepValue, repetitionValue, sourcelevellimitValue, thicknessValue, magneticfieldsValue, delayValue, points);
                    DialogResult resultHall = MessageBox.Show($"ทำการวัด Van der Pauw เสร็จสิ้นแล้ว ต้องการทำการวัด Hall Effect Measurement ต่อหรือไม่ ?", "การวัดเสร็จสิ้น", MessageBoxButtons.YesNo);
                    Debug.WriteLine($"[DEBUG] IconbuttonRunMeasurement_Click - ผลลัพธ์ MessageBox Van der Pauw เสร็จสิ้น (default): {resultHall}");

                    if (resultHall == DialogResult.Yes)
                    {
                        GlobalSettings.Instance.CurrentMeasurementMode = MeasurementMode.HallEffectMeasurement;
                        Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - เปลี่ยนไปโหมด Hall Effect และเริ่มการวัด (default)");
                        await RunHallMeasurementSequence();

                        // *** ตำแหน่งที่ 3: สำหรับกรณี Default ที่วัด Van der Pauw แล้วเลือกวัด Hall ต่อ ***
                        GlobalSettings.Instance.CollectedHallMeasurements.DebugPrintAllRawMeasurements();
                        // ****************************************************************************
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
                Debug.WriteLine($"[ERROR] IconbuttonRunMeasurement_Click - เกิดข้อผิดพลาด: {Ex.Message}");
            }
            finally
            {
                DisableEditRun(false);
                Debug.WriteLine("[DEBUG] IconbuttonRunMeasurement_Click - End");
            }
        }

        private async Task RunVanDerPauwMeasurement(double startValue, double stopValue, double stepValue, int repetitionValue, double sourcelevellimitValue, double thicknessValue, double magneticfieldsValue, double delayValue, int points)
        {
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - Start");
            GlobalSettings.Instance.CurrentMeasurementMode = MeasurementMode.VanDerPauwMethod;
            Debug.WriteLine($"[DEBUG] RunVanDerPauwMeasurement - CurrentMeasurementMode set to: {GlobalSettings.Instance.CurrentMeasurementMode}");

            UpdateToggleState();
            UpdateMeasurementMode();
            OnToggleChanged();
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - UI Updated");

            CurrentTuner = 1;
            CollectAndCalculateVdPMeasured.Instance.ClearAllData();
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - VdP Data Cleared");

            while (CurrentTuner <= 8)
            {
                Debug.WriteLine($"[DEBUG] RunVanDerPauwMeasurement - Processing Tuner: {CurrentTuner}");
                ConfigureSwitchSystem();
                await Task.Delay(600);
                UpdateMeasurementState();
                ConfigureSourceMeasureUnit();
                await ExecuteSweep();
                TracingRunMeasurement();
                await Task.Delay(1000);

                CurrentTuner++;
            }

            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - All tuners completed (Van der Pauw)");
            SMU.WriteString("OUTPut OFF");
            SS.WriteString("*CLS");
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - SMU Output Off, SS Cleared");

            if (Application.OpenForms.OfType<VdPTotalMeasureValuesForm>().FirstOrDefault() is VdPTotalMeasureValuesForm VdPTotalForm)
            {
                VdPTotalForm.Invoke((MethodInvoker)delegate { VdPTotalForm.LoadMeasurementData(); });
                Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - VdPTotalForm.LoadMeasurementData() invoked");
            }

            CollectAndCalculateVdPMeasured.Instance.CalculateVanderPauw();
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - VanderPauw Calculation Done");
            Debug.WriteLine("[DEBUG] RunVanDerPauwMeasurement - End");
        }

        private async Task RunHallMeasurementSequence()
        {
            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - Start");
            GlobalSettings.Instance.CurrentMeasurementMode = MeasurementMode.HallEffectMeasurement;
            Debug.WriteLine($"[DEBUG] RunHallMeasurementSequence - CurrentMeasurementMode set to: {GlobalSettings.Instance.CurrentMeasurementMode}");

            UpdateToggleState();
            UpdateMeasurementMode();
            OnToggleChanged();
            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - UI Updated");

            CollectAndCalculateHallMeasured.Instance.ClearAllHallData();
            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - Hall Data Cleared in CollectAndCalculateHallMeasured");

            await PerformSingleHallMeasurement(HallMeasurementState.NoMagneticField, false);
            DialogResult resultHallSouth = MessageBox.Show($"ทำการวัด Hall Effect Measurement ภายนอกสนามแม่เหล็กเสร็จสิ้นแล้ว ต้องการทำการวัด Hall Effect Measurement ภายใต้สนามแม่เหล็กทิศพุ่งออก (ทิศใต้) ต่อหรือไม่ ?", "การวัดต่อเนื่อง", MessageBoxButtons.YesNo);
            Debug.WriteLine($"[DEBUG] RunHallMeasurementSequence - ผลลัพธ์ MessageBox Hall นอกสนาม: {resultHallSouth}");

            bool allThreeMeasurementsAttempted = false; // Flag to check if we attempted all 3 measurement types

            if (resultHallSouth == DialogResult.Yes)
            {
                await PerformSingleHallMeasurement(HallMeasurementState.OutwardOrSouthMagneticField, true, "South");
                DialogResult resultHallNorth = MessageBox.Show($"ทำการวัด Hall Effect Measurement ภายใต้สนามแม่เหล็กทิศพุ่งออก (ทิศใต้) เสร็จสิ้นแล้ว โปรดทำการกลับด้านของชิ้นงานตัวอย่าง เพื่อทำการวัด Hall Effect Measurement ภายใต้สนามแม่เหล็กทิศพุ่งเข้า (ทิศเหนือ) ต่อ", "การวัดต่อเนื่อง", MessageBoxButtons.YesNo);
                Debug.WriteLine($"[DEBUG] RunHallMeasurementSequence - ผลลัพธ์ MessageBox Hall ทิศใต้: {resultHallNorth}");

                if (resultHallNorth == DialogResult.Yes)
                {
                    await PerformSingleHallMeasurement(HallMeasurementState.InwardOrNorthMagneticField, true, "North");
                    allThreeMeasurementsAttempted = true; // <--- จุดที่ 1: ตั้งค่า Flag เป็น true เมื่อมีการเรียกวัดครบ 3 สภาวะ
                }
            }

            if (allThreeMeasurementsAttempted) // เรียกคำนวณถ้าผู้ใช้ดำเนินการวัดครบ 3 ชนิด (No, South, North)
            {
                Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - All three measurement states attempted. Proceeding with calculation.");
                try
                {
                    CollectAndCalculateHallMeasured.Instance.CalculateAllHallProperties(GlobalSettings.Instance.ThicknessValueStd, GlobalSettings.Instance.MagneticFieldsValueStd);
                    Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - Hall Effect Calculation Done.");

                    if (!double.IsNaN(GlobalSettings.Instance.HallCoefficient) && !double.IsNaN(GlobalSettings.Instance.BulkConcentration) && !double.IsNaN(GlobalSettings.Instance.Mobility))
                    {
                        GlobalSettings.Instance.HallMeasurementDataReady = true;
                        MessageBox.Show("Hall Measurement sequence completed. Results are calculated and ready for viewing.", "Measurement Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Debug.WriteLine("[DEBUG] Hall Measurement results are ready."); // เพิ่ม Debug log
                    }
                    else
                    {
                        Debug.WriteLine("[WARNING] Hall Measurement Sequence incomplete. Skipping Hall properties calculation.");
                        GlobalSettings.Instance.HallMeasurementDataReady = false; // ชัดเจนว่าไม่พร้อมถ้าการวัดไม่ครบ
                        MessageBox.Show("Hall Measurement sequence was interrupted or incomplete. Hall properties will NOT be calculated.", "Measurement Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    GlobalSettings.Instance.HallMeasurementDataReady = false;
                    MessageBox.Show($"เกิดข้อผิดพลาดในการคำนวณ Hall Properties: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    Debug.WriteLine($"[ERROR] RunHallMeasurementSequence - Error during Hall calculation: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("[WARNING] Hall Measurement Sequence incomplete. Skipping Hall properties calculation.");

                GlobalSettings.Instance.HallMeasurementDataReady = false;
            }

            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - End of calculation and status check.");
            MessageBox.Show("ทำการวัด Hall Effect Measurement เสร็จสิ้นแล้ว", "การวัดเสร็จสิ้น", MessageBoxButtons.OK);
            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - Hall Effect Measurement Completed final message.");

            if (Application.OpenForms.OfType<HallTotalMeasureValuesForm>().FirstOrDefault() is HallTotalMeasureValuesForm HallTotalForm)
            {
                var allHallData = CollectAndCalculateHallMeasured.Instance.GetAllRawMeasurements();
                HallTotalForm.Invoke((Action)(() => HallTotalForm.LoadAllHallData(allHallData)));

                Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - Sent all Hall data from CollectAndCalculateHallMeasured to HallTotalForm");
            }

            Debug.WriteLine("[DEBUG] RunHallMeasurementSequence - End.");
        }

        private async Task PerformSingleHallMeasurement(HallMeasurementState state, bool hasMagneticField, string magneticFieldDirection = "")
        {
            Debug.WriteLine($"[DEBUG] PerformSingleHallMeasurement - Start (State: {state}, Magnetic Field: {hasMagneticField}, Direction: {magneticFieldDirection})");
            CurrentTuner = 1;
            GlobalSettings.Instance.CurrentHallState = state;

            while (CurrentTuner <= 4)
            {
                Debug.WriteLine($"[DEBUG] PerformSingleHallMeasurement - Processing Tuner: {CurrentTuner} (State: {state}, Magnetic Field: {hasMagneticField}, Direction: {magneticFieldDirection})");
                ConfigureSwitchSystem();
                await Task.Delay(600);
                UpdateMeasurementState();
                ConfigureSourceMeasureUnit();
                await ExecuteSweep();
                TracingRunMeasurement();
                await Task.Delay(1000);

                CurrentTuner++;
            }

            Debug.WriteLine($"[DEBUG] PerformSingleHallMeasurement - Measurement for state {state} completed.");
            SMU.WriteString("OUTPut OFF");
            SS.WriteString("*CLS");
            Debug.WriteLine("[DEBUG] PerformSingleHallMeasurement - SMU Output Off, SS Cleared");
            Debug.WriteLine("[DEBUG] PerformSingleHallMeasurement - End");
        }

        private void ConfigureSwitchSystem()
        {
            Debug.WriteLine($"[DEBUG] ConfigureSwitchSystem - Start (Current Mode: {GlobalSettings.Instance.CurrentMeasurementMode}, Tuner: {CurrentTuner})");
            SS.WriteString("ROUTe:OPEN ALL");
            MeasurementMode CurrentMode = GlobalSettings.Instance.CurrentMeasurementMode;
            var channels = GetChannelConfiguration(CurrentTuner, CurrentMode);

            Debug.WriteLine($"[DEBUG] ConfigureSwitchSystem - Channels to configure: {string.Join(", ", channels ?? new List<string>())}");

            if (channels != null && channels.Any())
            {
                foreach (var channel in channels)
                {
                    SS.WriteString($"ROUTe:CLOSe (@ {channel})");
                    Debug.WriteLine($"[DEBUG] ConfigureSwitchSystem - Closing channel: {channel}");
                }
            }
            else
            {
                Debug.WriteLine($"[DEBUG] ConfigureSwitchSystem - No channels to configure for Tuner: {CurrentTuner} in {CurrentMode} Mode");
            }
            Debug.WriteLine("[DEBUG] ConfigureSwitchSystem - End");
        }

        private Dictionary<int, List<string>> GetChannelConfigurations(MeasurementMode Mode)
        {
            Debug.WriteLine($"[DEBUG] GetChannelConfigurations - Start (Mode: {Mode})");
            Dictionary<int, List<string>> configurations = new Dictionary<int, List<string>>();

            if (Mode == MeasurementMode.VanDerPauwMethod) // Van der Pauw Mode
            {
                configurations = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "1!1!4", "1!2!5", "1!3!3", "1!4!6" }},
            { 2, new List<string> { "1!1!5", "1!2!4", "1!3!3", "1!4!6" }},
            { 3, new List<string> { "1!1!3", "1!2!6", "1!3!4", "1!4!5" }},
            { 4, new List<string> { "1!1!6", "1!2!3", "1!3!4", "1!4!5" }},
            { 5, new List<string> { "1!1!4", "1!2!3", "1!3!5", "1!4!6" }},
            { 6, new List<string> { "1!1!3", "1!2!4", "1!3!5", "1!4!6" }},
            { 7, new List<string> { "1!1!5", "1!2!6", "1!3!4", "1!4!3" }},
            { 8, new List<string> { "1!1!6", "1!2!5", "1!3!4", "1!4!3" }}
        };
                Debug.WriteLine("[DEBUG] GetChannelConfigurations - Van der Pauw Mode configurations loaded");
            }
            else if (Mode == MeasurementMode.HallEffectMeasurement) // Hall Effect Measurement Mode
            {
                configurations = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "1!1!3", "1!2!5", "1!3!4", "1!4!6" }},
            { 2, new List<string> { "1!1!5", "1!2!3", "1!3!4", "1!4!6" }},
            { 3, new List<string> { "1!1!4", "1!2!6", "1!3!3", "1!4!5" }},
            { 4, new List<string> { "1!1!6", "1!2!4", "1!3!3", "1!4!5" }}
        };
                Debug.WriteLine("[DEBUG] GetChannelConfigurations - Hall Effect Measurement Mode configurations loaded");
            }
            else
            {
                Debug.WriteLine($"[WARNING] GetChannelConfigurations - Unknown Measurement Mode: {Mode}");
            }
            Debug.WriteLine("[DEBUG] GetChannelConfigurations - End");
            return configurations;
        }

        private List<string> GetChannelConfiguration(int Position, MeasurementMode Mode)
        {
            Debug.WriteLine($"[DEBUG] GetChannelConfiguration - Start (Position: {Position}, Mode: {Mode})");
            var configurations = GetChannelConfigurations(Mode);
            Debug.WriteLine($"[DEBUG] GetChannelConfiguration - Configurations retrieved (Count: {configurations.Count})");

            if (configurations.ContainsKey(Position))
            {
                Debug.WriteLine($"[DEBUG] GetChannelConfiguration - Found configuration for Position {Position}: {string.Join(", ", configurations[Position])}");
                Debug.WriteLine("[DEBUG] GetChannelConfiguration - End (Found)");
                return configurations[Position];
            }
            else
            {
                Debug.WriteLine($"[DEBUG] GetChannelConfiguration - No configuration found for Position {Position} in {Mode} Mode.");
                Debug.WriteLine("[DEBUG] GetChannelConfiguration - End (Not Found)");
                return new List<string>();
            }
        }

        private void ConfigureSourceMeasureUnit()
        {
            Debug.WriteLine("[DEBUG] ConfigureSourceMeasureUnit - Start");
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                Debug.WriteLine("[DEBUG] ConfigureSourceMeasureUnit - ValidateInputs failed");
                return;
            }

            SMU.IO.Timeout = 1000000;
            SMU.WriteString("TRACe:CLEar");
            SMU.WriteString("TRACe:POINts 3000000, 'defbuffer1'");
            Debug.WriteLine("[DEBUG] ConfigureSourceMeasureUnit - SMU Trace Cleared and Points Set");

            if (GlobalSettings.Instance.SourceModeUI == "Current")
            {
                SMU.WriteString($"SOURce:FUNCtion CURRent");
                SMU.WriteString($"SOURce:CURRent:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:CURRent:VLIMit {sourcelevellimitValue}");
                Debug.WriteLine($"[DEBUG] ConfigureSourceMeasureUnit - SMU Source set to Current (Limit: {sourcelevellimitValue})");
            }
            else
            {
                SMU.WriteString($"SOURce:FUNCtion VOLTage");
                SMU.WriteString($"SOURce:VOLTage:RANGe:AUTO ON");
                SMU.WriteString($"SOURce:VOLTage:ILIMit {sourcelevellimitValue}");
                Debug.WriteLine($"[DEBUG] ConfigureSourceMeasureUnit - SMU Source set to Current (Limit: {sourcelevellimitValue})");
            }

            if (GlobalSettings.Instance.MeasureModeUI == "Current")
            {
                SMU.WriteString($"SENSe:FUNCtion 'CURRent'");
                SMU.WriteString($"SENSe:CURRent:RANGe:AUTO ON");
            }
            else
            {
                SMU.WriteString($"SENSe:FUNCtion 'VOLTage'");
                SMU.WriteString($"SENSe:VOLTage:RANGe:AUTO ON");
            }

            if (GlobalSettings.Instance.ResistanceSenseModeUI == "4-Wires")
            {
                SMU.WriteString($"SENSe:{GlobalSettings.Instance.MeasureModeUI}:RSENse ON");
            }
            else
            {
                SMU.WriteString($"SENSe:{GlobalSettings.Instance.MeasureModeUI}:RSENse OFF");
            }
        }

        private async Task ExecuteSweep()
        {
            Debug.WriteLine("[DEBUG] ExecuteSweep - Start");
            if (!ValidateInputs(out double startValue, out double stopValue, out double stepValue, out int repetitionValue, out double sourcelevellimitValue, out double thicknessValue, out double magneticfieldsValue, out double delayValue, out int points))
            {
                Debug.WriteLine("[DEBUG] ExecuteSweep - ValidateInputs failed");
                return;
            }

            if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement)
            {
                string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
                Debug.WriteLine($"[DEBUG] ExecuteSweep (Hall) - Parameters: {allValues}.");
            }
            else
            {
                string allValues = $"Sense: {GlobalSettings.Instance.ResistanceSenseModeUI}, Measure: {GlobalSettings.Instance.MeasureModeUI}, Source: {GlobalSettings.Instance.SourceModeUI}, Start: {GlobalSettings.Instance.StartValueUI} {GlobalSettings.Instance.StartUnitUI}, Step: {GlobalSettings.Instance.StepValueUI} {GlobalSettings.Instance.StepUnitUI}, Source Delay: {GlobalSettings.Instance.SourceDelayValueUI} {GlobalSettings.Instance.SourceDelayUnitUI}, Stop: {GlobalSettings.Instance.StopValueUI} {GlobalSettings.Instance.StopUnitUI}, Source Limit: {GlobalSettings.Instance.SourceLimitModeUI}, Limit Level: {GlobalSettings.Instance.SourceLimitLevelValueUI} {GlobalSettings.Instance.SourceLimitLevelUnitUI}, Repetition: {GlobalSettings.Instance.RepetitionValueUI}, Thickness: {GlobalSettings.Instance.ThicknessValueUI} {GlobalSettings.Instance.ThicknessUnitUI}, Magnetic Fields: {GlobalSettings.Instance.MagneticFieldsValueUI} {GlobalSettings.Instance.MagneticFieldsUnitUI}";
                Debug.WriteLine($"[DEBUG] ExecuteSweep (VdP) - Parameters: {allValues}.");
            }

            if (GlobalSettings.Instance.SourceModeUI == "Current")
            {
                string sweepCommand = $"SOURce:SWEep:CURRent:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                SMU.WriteString(sweepCommand);
                Debug.WriteLine($"[DEBUG] ExecuteSweep - Sending Current Sweep command: {sweepCommand}");
            }
            else
            {
                string sweepCommand = $"SOURce:SWEep:VOLTage:LINear:STEP {startValue}, {stopValue}, {stepValue}, {delayValue}, {repetitionValue}";
                SMU.WriteString(sweepCommand);
                Debug.WriteLine($"[DEBUG] ExecuteSweep - Sending Voltage Sweep command: {sweepCommand}");
            }

            SMU.WriteString("OUTPut ON");
            SMU.WriteString("INITiate");
            SMU.WriteString("*WAI");
            SMU.WriteString("OUTPut OFF");
            Debug.WriteLine("[DEBUG] ExecuteSweep - Output ON, Initiate, Wait, Output OFF completed");

            await Task.Delay(points * repetitionValue * (int)delayValue * 300);
            Debug.WriteLine($"[DEBUG] ExecuteSweep - Delay completed (Duration: {points * repetitionValue * (int)delayValue * 300} ms)");
            Debug.WriteLine("[DEBUG] ExecuteSweep - End");
        }

        private void UpdateMeasurementState()
        {
            Debug.WriteLine($"Measuring Tuner {CurrentTuner}");
        }

        private void TracingRunMeasurement()
        {
            Debug.WriteLine("[DEBUG] TracingRunMeasurement - Start");
            try
            {
                SMU.WriteString("TRACe:ACTual?");
                string BufferCount = SMU.ReadString().Trim();
                Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Buffer count read: {BufferCount}");

                if (!int.TryParse(BufferCount, out int BufferPoints) || BufferPoints == 0)
                {
                    MessageBox.Show("ไม่สามารถดึงข้อมูลการวัดได้: ไม่มีข้อมูลในบัฟเฟอร์", "ข้อผิดพลาด", MessageBoxButtons.OK);
                    Debug.WriteLine("[WARNING] TracingRunMeasurement - ไม่สามารถดึงข้อมูล: ไม่มีข้อมูลในบัฟเฟอร์");
                    return;
                }

                SMU.WriteString($"TRACe:DATA? 1, {BufferPoints}, 'defbuffer1', SOURce, READing");
                string RawData = SMU.ReadString().Trim();
                Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Raw Data read (Length: {RawData.Length}): {RawData}");
                Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Buffer contains: {BufferPoints} readings");

                string[] DataPairs = RawData.Split(',');
                List<Tuple<double, double>> currentMeasurements = new List<Tuple<double, double>>();
                Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Number of data pairs: {DataPairs.Length}");

                if (DataPairs.Length % 2 != 0)
                {
                    MessageBox.Show("รูปแบบข้อมูลในบัฟเฟอร์ไม่ถูกต้อง", "ข้อผิดพลาด", MessageBoxButtons.OK);
                    Debug.WriteLine("[ERROR] TracingRunMeasurement - รูปแบบข้อมูลในบัฟเฟอร์ไม่ถูกต้อง");
                    return;
                }

                for (int i = 0; i < DataPairs.Length; i += 2)
                {
                    if (double.TryParse(DataPairs[i], out double SourceValue) && double.TryParse(DataPairs[i + 1], out double MeasuredValue))
                    {
                        Debug.WriteLine($"  [DEBUG-PRE-ADD] Raw Read - Source: {SourceValue:E9} A, Measured: {MeasuredValue:E9} V (Index: {i})");

                        currentMeasurements.Add(new Tuple<double, double>(SourceValue, MeasuredValue));
                        Debug.WriteLine($"[DEBUG] TracingRunMeasurement - Data Point {i / 2 + 1}: Source={SourceValue}, Reading={MeasuredValue}");
                    }
                    else
                    {
                        Debug.WriteLine($"[WARNING] TracingRunMeasurement - ไม่สามารถ Parse ค่า Source หรือ Reading ที่ Index {i} หรือ {i + 1} ได้. Data: '{DataPairs[i]}'/'{DataPairs[i + 1]}'");
                    }
                }
                Debug.WriteLine($"[DEBUG] TracingRunMeasurement - จำนวน Data Points ที่อ่านได้: {currentMeasurements.Count}");

                if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.HallEffectMeasurement)
                {
                    HallMeasurementState States;
                    HallMeasurementState currentHallState = GlobalSettings.Instance.CurrentHallState;

                    switch (currentHallState)
                    {
                        case HallMeasurementState.NoMagneticField:
                            States = HallMeasurementState.NoMagneticField;
                            break;
                        case HallMeasurementState.InwardOrNorthMagneticField:
                            States = HallMeasurementState.InwardOrNorthMagneticField;
                            break;
                        case HallMeasurementState.OutwardOrSouthMagneticField:
                            States = HallMeasurementState.OutwardOrSouthMagneticField;
                            break;
                        default:
                            Debug.WriteLine($"[WARNING] TracingRunMeasurement (Hall Effect) - Unknown CurrentHallState: {currentHallState}, defaulting to NoMagneticField");
                            States = HallMeasurementState.NoMagneticField;
                            break;
                    }

                    Debug.WriteLine($"[DEBUG] TracingRunMeasurement (Hall Effect) - Tuner: {CurrentTuner}, Type: {States}, Data Points Read: {currentMeasurements.Count}");
                    Debug.WriteLine($"Current Hall State: {currentHallState}, Measurement Type: {States}");
                    Debug.WriteLine($"  [DEBUG-TO-STORE] Data for State: {States}, Tuner: {CurrentTuner}:");

                    foreach (var dataPoint in currentMeasurements)
                    {
                        Debug.WriteLine($"    - Current: {dataPoint.Item1:E9} A, Voltage: {dataPoint.Item2:E9} V");
                    }

                    GlobalSettings.Instance.CollectedHallMeasurements.StoreMeasurementData(States, CurrentTuner, currentMeasurements);
                    Debug.WriteLine("[DEBUG] TracingRunMeasurement (Hall Effect) - Data stored in CollectedHallMeasurements"); 
                }
                else if (GlobalSettings.Instance.CurrentMeasurementMode == MeasurementMode.VanDerPauwMethod)
                {
                    var convertedMeasurementsForVdP = currentMeasurements.Select(t => (Source: t.Item1, Reading: t.Item2)).ToList();
                    Debug.WriteLine($"[DEBUG] TracingRunMeasurement (Van der Pauw) - Tuner: {CurrentTuner}, Data Points Read: {currentMeasurements.Count}");
                    CollectAndCalculateVdPMeasured.Instance.StoreMeasurementData(CurrentTuner, convertedMeasurementsForVdP);
                    Debug.WriteLine("[DEBUG] TracingRunMeasurement (Van der Pauw) - Data stored in CollectedVdPMeasurements");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {Ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK);
                Debug.WriteLine($"[ERROR] TracingRunMeasurement - เกิดข้อผิดพลาด: {Ex.Message}");
            }

            Debug.WriteLine("[DEBUG] TracingRunMeasurement - End");
        }

        private void IconbuttonErrorCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalSettings.Instance.IsSMUConnected || !GlobalSettings.Instance.IsSSConnected)
                {
                    MessageBox.Show("ไม่สามารถตรวจสอบข้อผิดพลาดจากเครื่องมือได้ เนื่องจากไม่ได้ทำการเชื่อมต่อเครื่องมือ", "ข้อผิดพลาดในการตรวจสอบ", MessageBoxButtons.OK);
                    return;
                }

                string smuError = null;
                string ssError = null;

                SMU.WriteString("SYSTem:ERRor?");
                smuError = SMU.ReadString().Trim();
                Debug.WriteLine($"Source Measure Unit error response: {smuError}");

                SS.WriteString("SYSTem:ERRor?");
                ssError = SS.ReadString().Trim();
                Debug.WriteLine($"Switch System error response: {ssError}");

                if (string.IsNullOrEmpty(smuError) && string.IsNullOrEmpty(ssError))
                {
                    MessageBox.Show("ไม่พบข้อผิดพลาดจากเครื่องมือ", "การตรวจสอบเสร็จสิ้น", MessageBoxButtons.OK);
                }
                else
                {
                    string errorMessage = "ตรวจพบข้อผิดพลาดจากเครื่องมือ:\n";
                    if (!string.IsNullOrEmpty(smuError) && !smuError.ToLower().Contains("no error")) // ตรวจสอบว่ามี Error จริง และไม่ใช่ข้อความ "No error"
                    {
                        errorMessage += $"Source Measure Unit {SMUModelKeyword}: {smuError}\n";
                    }
                    if (!string.IsNullOrEmpty(ssError) && !ssError.ToLower().Contains("no error")) // ตรวจสอบว่ามี Error จริง และไม่ใช่ข้อความ "No error"
                    {
                        errorMessage += $"Switch System {SSModelKeyword}: {ssError}\n";
                    }

                    if (errorMessage.EndsWith(":\n"))
                    {
                        MessageBox.Show("ไม่พบข้อผิดพลาดจากเครื่องมือใด ๆ", "การตรวจสอบเสร็จสิ้น", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "พบข้อผิดพลาด", MessageBoxButtons.OK);
                    }
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

                if (string.Equals(GlobalSettings.Instance.SourceLimitModeUI, "Current", StringComparison.OrdinalIgnoreCase) && (sourcelevellimit > 1.05 || sourcelevellimit < -1.05))
                {
                    MessageBox.Show("ควรทำการตั้งค่าระดับขีดจำกัดของกระแสจากแหล่งจ่ายอยู่ในช่วง -1.05 A - 1.05 A กรุณาทำการตั้งค่าการวัดใหม่", "ข้อผิดพลาดในการตั้งค่าการวัด", MessageBoxButtons.OK);
                    return false;
                }

                if (string.Equals(GlobalSettings.Instance.SourceLimitModeUI, "Voltage", StringComparison.OrdinalIgnoreCase) && (sourcelevellimit > 21 || sourcelevellimit < -21))
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
            ComboboxVISASMUIOPort.Enabled = !shouldDisable;
            ComboboxVISASSIOPort.Enabled = !shouldDisable;
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

            IconbuttonClearSettings.Enabled = !shouldDisable;
            IconbuttonErrorCheck.Enabled = !shouldDisable;
            IconbuttonRunMeasurement.Enabled = !shouldDisable;
            IconbuttonTunerTest.Enabled = !shouldDisable;
        }
    }
}