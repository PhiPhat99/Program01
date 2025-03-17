using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using Ivi.Visa.Interop;

namespace Program01
{
    public class PortScannerClass
    {
        private readonly ResourceManager _RsrcMngr;

        public PortScannerClass(ResourceManager RsrcMngr = null)
        {
            _RsrcMngr = RsrcMngr ?? new ResourceManager();
        }

        public List<string> ScanAllPorts()
        {
            List<string> AvailablePorts = new List<string>();

            try
            {
                Debug.WriteLine("[DEBUG] Start to finding a ports...");

                // สแกน GPIB
                string[] GPIBResources = (string[])_RsrcMngr.FindRsrc("GPIB?*::?*::INSTR");
                AvailablePorts.AddRange(GPIBResources);
            }
            catch (Exception) { }

            try
            {
                // สแกน USB
                string[] USBResources = (string[])_RsrcMngr.FindRsrc("USB?*::?*::INSTR");
                AvailablePorts.AddRange(USBResources);
            }
            catch (Exception) { }

            try
            {
                // สแกน LAN
                string[] LANResources = (string[])_RsrcMngr.FindRsrc("TCPIP?*::?*::INSTR");
                AvailablePorts.AddRange(LANResources);
            }
            catch (Exception) { }

            // สแกน Serial Ports
            AvailablePorts.AddRange(SerialPort.GetPortNames());

            return AvailablePorts;
        }
    }
}
