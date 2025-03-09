using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Program01
{
    public class InstrumentsGPIBPortDetector
    {
        private ResourceManager ResourceManager;
        private Dictionary<string, string> InstrumentModels;

        public InstrumentsGPIBPortDetector()
        {
            ResourceManager = new ResourceManager();
        }

        public List<string> GetValidInstruments(string[] DetectedAddresses)
        {
            List<string> ValidAddresses = new List<string>();
            var keithleyRegex = new Regex(@"KEITHLEY\s+Model\s+\d+", RegexOptions.IgnoreCase);

            foreach (string Address in DetectedAddresses)
            {
                string response = QueryDevice(Address);  // ✅ เรียกใช้ QueryDevice

                if (!string.IsNullOrEmpty(response) && keithleyRegex.IsMatch(response))
                {
                    ValidAddresses.Add(Address);
                    Debug.WriteLine($"[INFO] พบอุปกรณ์ Keithley ที่ {Address}: {response}");
                }
            }

            return ValidAddresses;
        }

        private string QueryDevice(string address)  // ✅ ใส่ไว้ที่นี่
        {
            FormattedIO488 device = new FormattedIO488();
            try
            {
                device.IO = (IMessage)ResourceManager.Open(address);
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
                if (device.IO != null)
                {
                    try { device.IO.Close(); } catch { }
                }
            }
        }
    }

}
