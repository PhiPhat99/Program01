using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ivi.Visa.Interop;
using System.Text.RegularExpressions;

namespace Program01
{
    public class GPIBScanner : IDisposable
    {
        private ResourceManager resourcemanager;
        private List<string> ValidAddresses = new List<string>();

        public GPIBScanner()
        {
            resourcemanager = new ResourceManager();
        }

        public string[] ScanAllGPIBDevices()
        {
            ValidAddresses.Clear();

            try
            {
                string[] deviceList = resourcemanager.FindRsrc("GPIB?*::INSTR");

                foreach (string address in deviceList)
                {
                    try
                    {
                        string queryResponse = QueryDevice(address);

                        if (!string.IsNullOrEmpty(queryResponse) && IsKeithleyDevice(queryResponse))
                        {
                            ValidAddresses.Add(address);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ERROR] ไม่สามารถสแกนอุปกรณ์ที่ {address}: {ex.Message}");
                    }
                }

                return ValidAddresses.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถสแกนอุปกรณ์ GPIB ได้: {ex.Message}");
                return new string[0];
            }
        }

        private bool IsKeithleyDevice(string deviceResponse)
        {
            var regex = new Regex(@"KEITHLEY\s+Model\s+\d+", RegexOptions.IgnoreCase);
            return regex.IsMatch(deviceResponse);
        }

        private string QueryDevice(string address)
        {
            using (FormattedIO488 device = new FormattedIO488())
            {
                try
                {
                    device.IO = (IMessage)resourcemanager.Open(address);
                    device.WriteString("*IDN?");
                    return device.ReadString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] ไม่สามารถส่งคำสั่งไปยัง {address}: {ex.Message}");
                    return string.Empty;
                }
                finally
                {
                    try { device.IO.Close(); } catch { }
                }
            }
        }

        public void Dispose()
        {
            if (resourcemanager != null)
            {
                try { resourcemanager.Close(); } catch { }
                resourcemanager = null;
            }
        }
    }
}
