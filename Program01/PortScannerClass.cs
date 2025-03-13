using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using Program01.Interfaces;

namespace Program01
{
    public class PortScannerClass
    {
        private readonly ResourceManager _RsrcMngr;
        private readonly Func<List<string>> _GPIBScanner;

        // Constructor ที่รับ Dependency Injection สำหรับ ResourceManager และ GPIBScanner (จำลองการทำงาน)
        public PortScannerClass(ResourceManager RsrcMngr = null, Func<List<string>> GPIBScanner = null)
        {
            _RsrcMngr = RsrcMngr ?? new ResourceManager(); // ใช้ ResourceManager ถ้าไม่ได้รับค่า
            _GPIBScanner = GPIBScanner ?? DefaultGPIBScanner; // ใช้ฟังก์ชันจำลองหากไม่ได้รับค่า
        }

        // ฟังก์ชันจำลองการสแกน GPIB (สามารถแทนที่ได้ด้วยการทดสอบหรือการจำลอง)
        private List<string> DefaultGPIBScanner()
        {
            // นี่คือตัวอย่างการจำลองการทำงาน
            return new List<string> { "GPIB0::5::INSTR", "GPIB0::16::INSTR" };
        }

        // ฟังก์ชันสำหรับสแกนพอร์ตทั้งหมด
        public List<string> ScanAllPorts()
        {
            List<string> AvailablePorts = new List<string>();

            try
            {
                // สแกน GPIB พอร์ต
                string[] GPIBResources = (string[])_RsrcMngr.FindRsrc("GPIB?*::?*::INSTR");
                AvailablePorts.AddRange(DefaultGPIBScanner());
            }
            catch (Exception) { }

            // สแกน Serial Ports
            AvailablePorts.AddRange(SerialPort.GetPortNames());

            try
            {
                // สแกน USB Resources
                /*string[] USBResources = (string[])_RsrcMngr.FindRsrc("USB?*::?*::INSTR");
                AvailablePorts.AddRange(USBResources);*/
            }
            catch (Exception) { }

            try
            {
                // สแกน LAN Resources
                /*string[] LANResources = (string[])_RsrcMngr.FindRsrc("TCPIP?*::?*::INSTR");
                AvailablePorts.AddRange(LANResources);*/
            }
            catch (Exception) { }

            // เรียกใช้ GPIBScanner เพื่อจำลองการตรวจจับ GPIB
            AvailablePorts.AddRange(_GPIBScanner());

            return AvailablePorts;
        }
    }
}
