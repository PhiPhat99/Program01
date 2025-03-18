using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using Ivi.Visa.Interop;

namespace Program01
{
    public class PortScanner
    {
        private readonly ResourceManager ResourceManagers;

        public PortScanner(ResourceManager RsrcMngrs)
        {
            ResourceManagers = RsrcMngrs ?? throw new ArgumentNullException(nameof(RsrcMngrs));
        }

        private List<string> AvailablePorts;
        private static bool IsFirstRun = true;

        public void ClearAvailablePorts()
        {
            AvailablePorts.Clear();
            Debug.WriteLine("[DEBUG] Cleared AvailablePorts before scanning.");
        }

        public List<string> ScanAllPorts()
        {
            ClearAvailablePorts();
            List<string> AvailablePorts = new List<string>();
            Debug.WriteLine("[DEBUG] Start to finding a ports...");

            if (IsFirstRun)
            {
                ClearPorts();
                IsFirstRun = false;
            }

            try
            {
                string[] GPIBResources = (string[])ResourceManagers.FindRsrc("GPIB?*::?*::INSTR");
                AvailablePorts.AddRange(GPIBResources);
                Debug.WriteLine($"[DEBUG] Found GPIB ports: {string.Join(", ", GPIBResources)}");
            }
            catch (System.Runtime.InteropServices.COMException Ex)
            {
                int HResult = System.Runtime.InteropServices.Marshal.GetHRForException(Ex);

                if (HResult == unchecked((int)0x80040011))
                {
                    Debug.WriteLine("[WARN] No GPIB devices found or VISA GPIB driver issue");
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Failed to scan GPIB: {Ex.Message} (HRESULT: {HResult})");
                }
            }

            try
            {
                string[] USBResources = (string[])ResourceManagers.FindRsrc("USB?*::?*::INSTR");
                AvailablePorts.AddRange(USBResources);
                Debug.WriteLine($"[DEBUG] Found USB ports: {string.Join(", ", USBResources)}");
            }
            catch (System.Runtime.InteropServices.COMException Ex)
            {
                int HResult = System.Runtime.InteropServices.Marshal.GetHRForException(Ex);

                if (HResult == unchecked((int)0x80040011))
                {
                    Debug.WriteLine("[WARN] No USB devices found or VISA USB driver issue");
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Failed to scan USB: {Ex.Message} (HRESULT: {HResult})");
                }
            }

            try
            {
                string[] LANResources = (string[])ResourceManagers.FindRsrc("TCPIP?*::?*::INSTR");
                AvailablePorts.AddRange(LANResources);
                Debug.WriteLine($"[DEBUG] Found LAN ports: {string.Join(", ", LANResources)}");
            }
            catch (System.Runtime.InteropServices.COMException Ex)
            {
                int HResult = System.Runtime.InteropServices.Marshal.GetHRForException(Ex);

                if (HResult == unchecked((int)0x80040011))
                {
                    Debug.WriteLine("[WARN] No LAN devices found or VISA LAN driver issue");
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Failed to scan LAN: {Ex.Message} (HRESULT: {HResult})");
                }
            }

            string[] SerialPorts = SerialPort.GetPortNames();
            AvailablePorts.AddRange(SerialPort.GetPortNames());
            Debug.WriteLine($"[DEBUG] Found Serial ports: {string.Join(", ", SerialPorts)}");

            if (AvailablePorts.Count > 0)
            {
                Debug.WriteLine($"[INFO] Total connected ports: {AvailablePorts.Count}");
                Debug.WriteLine($"[INFO] Connected Ports List: {string.Join(", ", AvailablePorts)}");
            }
            else
            {
                Debug.WriteLine("[INFO] No ports detected");
            }

            return AvailablePorts;
        }

        public void ClearPorts()
        {
            try
            {
                Debug.WriteLine("[DEBUG] Clearing all ports before scanning...");

                foreach (string PortName in SerialPort.GetPortNames())
                {
                    try
                    {
                        using (SerialPort SP = new SerialPort(PortName))
                        {
                            if (SP.IsOpen)
                            {
                                SP.Close();
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        Debug.WriteLine($"[ERROR] Failed to close SerialPort {PortName}: {Ex.Message}");
                    }
                }

                Debug.WriteLine("[DEBUG] Serial ports cleared successfully.");

                try
                {
                    if (ResourceManagers != null)
                    {
                        string[] GPIBResources = (string[])ResourceManagers.FindRsrc("GPIB?*::?*::INSTR");
                        string[] USBResources = (string[])ResourceManagers.FindRsrc("USB?*::?*::INSTR");
                        string[] LANResources = (string[])ResourceManagers.FindRsrc("TCPIP?*::?*::INSTR");
                        var AllResources = AvailablePorts.ToArray();

                        foreach (var Resources in AllResources)
                        {
                            try
                            {
                                using (VISAPorts VISADevice = new VISAPorts())
                                {
                                    VISADevice.VISAports.IO = (IMessage)ResourceManagers.Open(Resources);
                                    Debug.WriteLine($"[DEBUG] Closing connection to {Resources}...");
                                    VISADevice.VISAports.IO.Close();
                                }
                            }
                            catch (Exception Ex)
                            {
                                Debug.WriteLine($"[ERROR] Failed to close device {Resources}: {Ex.Message}");
                            }
                        }

                        Debug.WriteLine("[DEBUG] VISA-connected devices (GPIB, USB, LAN) cleared successfully.");
                    }
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine($"[ERROR] Failed to list and close VISA resources: {Ex.Message}");
                }

                Debug.WriteLine("[DEBUG] All ports cleared successfully.");
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] Error while clearing ports: {Ex.Message}");
            }
        }
    }
}
