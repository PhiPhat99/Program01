using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Program01.Interfaces;

namespace Program01
{
    public class InstrumentsGPIBPortDetector : IGPIBDetectorInterfaces, IDisposable
    {
        private ResourceManager RsrcMngr;

        public InstrumentsGPIBPortDetector()
        {
            RsrcMngr = new ResourceManager();
        }

        public List<string> GetValidInstruments(string[] DetectedAddresses)
        {
            List<string> ValidAddresses = new List<string>();
            var KeithleyRegEx = new Regex(@"KEITHLEY\s+Model\s+\d+", RegexOptions.IgnoreCase);

            foreach (string Address in DetectedAddresses)
            {
                if (!Address.StartsWith("GPIB"))
                {
                    continue;
                }

                string Response = QueryDevice(Address);

                if (!string.IsNullOrEmpty(Response) && KeithleyRegEx.IsMatch(Response))
                {
                    ValidAddresses.Add(Address);
                    Debug.WriteLine($"[INFO] พบอุปกรณ์ Keithley ที่ {Address}: {Response}");
                }
            }

            return ValidAddresses;
        }

        private string QueryDevice(string Address)
        {
            /*using (GPIBDevice DeviceWrapper = new GPIBDevice())
            {
                var Device = DeviceWrapper.Device;
                try
                {
                    //Device.IO = (IMessage)RsrcMngr.Open(Address);
                    //Device.WriteString("*IDN?");

                    return Device.ReadString();
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine($"[ERROR] ไม่สามารถส่งคำสั่งไปยัง {Address}: {Ex.Message}");

                    return string.Empty;
                }
            }*/

            if (Address == "GPIB0::5::INSTR" || Address == "GPIB1::5::INSTR" || Address == "GPIB2::5::INSTR" || Address == "GPIB3::5::INSTR")
            {
                return "KEITHLEY Model 2450 SourceMeter";
            }
            else if (Address == "GPIB0::16::INSTR" || Address == "GPIB1::16::INSTR" || Address == "GPIB2::16::INSTR" || Address == "GPIB3::16::INSTR")
            {
                return "KEITHLEY Model 7001 Switch System";
            }

            return string.Empty;
        }

        public void Dispose()
        {
            if (RsrcMngr != null)
            {
                RsrcMngr = null;
            }
        }
    }
}

