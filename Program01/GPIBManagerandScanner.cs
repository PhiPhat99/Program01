using System;
using System.Collections.Generic;
using Ivi.Visa.Interop;
using Ivi.Visa;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using Keysight.Visa;
using System.Runtime.InteropServices;

namespace Program01
{
    public class GPIBManagerandScanner
    {
        private Ivi.Visa.Interop.ResourceManager Resourcemanager;
        private bool IsUsingKeysightVISA = false;
        private Dictionary<string, IMessageBasedSession> InstrumentSessions;
        private Dictionary<string, Ivi.Visa.Interop.FormattedIO488> ConnectedInstruments;

        public GPIBManagerandScanner()
        {
            // กำหนดค่าเริ่มต้นให้ Dictionary และ ResourceManager
            InstrumentSessions = new Dictionary<string, IMessageBasedSession>();
            ConnectedInstruments = new Dictionary<string, FormattedIO488>();
            //Resourcemanager = new ResourceManager();
            try
            {
                using (var session = GlobalResourceManager.Open("GPIB0::5::INSTR") as IMessageBasedSession)
                {
                    if (session != null)
                    {
                        IsUsingKeysightVISA = false;  // กำลังใช้ NI-VISA
                        Console.WriteLine("✅ กำลังใช้ NI-VISA หรือ IVI-VISA");
                    }
                }
            }
            catch (Exception)
            {
                IsUsingKeysightVISA = true; // กำลังใช้ Keysight VISA
                Console.WriteLine("⚠️ ใช้ Keysight VISA API");
            }
        }

        // ฟังก์ชันในการสแกนพอร์ต GPIB ที่เชื่อมต่ออยู่
        public List<string> ScanGPIBPorts()
        {
            try
            {
                return Resourcemanager.FindRsrc("GPIB?*::?*::INSTR")
                    .Where(Device => Device == "GPIB0::5::INSTR" || Device == "GPIB1::5::INSTR" ||
                                  Device == "GPIB2::5::INSTR" || Device == "GPIB0::16::INSTR" ||
                                  Device == "GPIB1::16::INSTR" || Device == "GPIB2::16::INSTR")
                    .ToList();
            }
            catch (VisaException Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถสแกนพอร์ต GPIB ได้: {Ex.Message}");
                return new List<string>();
            }
        }

        public bool ConnectInstrument(string GPIBAddress)
        {
            try
            {
                if (IsUsingKeysightVISA)
                {
                    // ใช้ Keysight VISA
                    var session = (GpibSession)Factory.Open(GPIBAddress); // เชื่อมต่อผ่าน Keysight VISA
                    Console.WriteLine($"[INFO] เชื่อมต่ออุปกรณ์ {GPIBAddress} ผ่าน Keysight VISA");

                    // เพิ่ม session ลงใน InstrumentSessions
                    InstrumentSessions[GPIBAddress] = session;
                }
                else
                {
                    // ใช้ NI-VISA
                    var session = (IMessageBasedSession)GlobalResourceManager.Open(GPIBAddress); // เชื่อมต่อผ่าน NI-VISA
                    Console.WriteLine($"[INFO] เชื่อมต่ออุปกรณ์ {GPIBAddress} ผ่าน NI-VISA");

                    // เพิ่ม session ลงใน InstrumentSessions
                    InstrumentSessions[GPIBAddress] = session;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] การเชื่อมต่อล้มเหลว: {ex.Message}");
                return false;
            }
        }

        // ฟังก์ชันในการยกเลิกการเชื่อมต่อเครื่องมือ
        public bool DisconnectInstrument(string GPIBAddress)
        {
            try
            {
                if (ConnectedInstruments.ContainsKey(GPIBAddress))
                {
                    var session = ConnectedInstruments[GPIBAddress];

                    // รีเซ็ตก่อนปิดการเชื่อมต่อ
                    session.WriteString("*CLS");
                    session.WriteString("*RST");

                    // ปิดการเชื่อมต่อและล้างค่า COM object
                    session.IO.Close();
                    Marshal.FinalReleaseComObject(session.IO);

                    ConnectedInstruments.Remove(GPIBAddress);

                    Debug.WriteLine($"[INFO] ตัดการเชื่อมต่อ {GPIBAddress} สำเร็จ");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"[WARNING] ไม่พบอุปกรณ์ {GPIBAddress} ที่ต้องการตัดการเชื่อมต่อ!");
                    return false;
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] ไม่สามารถยกเลิกการเชื่อมต่อ {GPIBAddress}: {Ex.Message}");
                return false;
            }
        }

        // ฟังก์ชันตรวจสอบว่าเครื่องมือเชื่อมต่ออยู่หรือไม่
        public bool IsConnected(string GPIBAddress)
        {
            return ConnectedInstruments.ContainsKey(GPIBAddress);
        }

        public string SendCommand(string GPIBAddress, string command)
        {
            try
            {
                if (!ConnectedInstruments.ContainsKey(GPIBAddress))
                {
                    Debug.WriteLine($"[ERROR] อุปกรณ์ {GPIBAddress} ยังไม่ได้เชื่อมต่อ!");
                    return null;
                }

                var session = ConnectedInstruments[GPIBAddress];

                session.WriteString(command);
                string response = session.ReadString();
                Debug.WriteLine($"[COMMAND] {GPIBAddress} >> {command}");
                Debug.WriteLine($"[RESPONSE] {response}");

                return response;
            }
            catch (VisaException VisaEx)
            {
                Debug.WriteLine($"[ERROR] การส่งคำสั่งไปยัง {GPIBAddress} ล้มเหลว: {VisaEx.Message}");
                return null;
            }
        }

        public string QueryInstrument(string GPIBAddress, string command)
        {
            try
            {
                if (!InstrumentSessions.ContainsKey(GPIBAddress))
                {
                    MessageBox.Show("อุปกรณ์ยังไม่ได้เชื่อมต่อ!");
                    return null;
                }

                var session = InstrumentSessions[GPIBAddress];

                // ใช้ FormattedIO488 เพื่อส่งคำสั่ง
                FormattedIO488 io = new FormattedIO488();
                io.IO = (IMessage)session;  // เชื่อมโยง Session กับ IO

                io.WriteString(command);
                string response = io.ReadString();
                return response;
            }
            catch (VisaException VisaEx)
            {
                MessageBox.Show("อ่านค่าล้มเหลว: " + VisaEx.Message);
                return null;
            }
        }
    }
}
