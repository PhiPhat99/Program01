using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Program01
{
    public class InstrumentsConnectionManager
    {
        private static readonly InstrumentsConnectionManager _Instance = new InstrumentsConnectionManager();
        public static InstrumentsConnectionManager Instance => _Instance;

        private FormattedIO488 _SMU;
        private FormattedIO488 _SS;
        private Ivi.Visa.Interop.ResourceManager _RsrcMngr;

        public bool IsSMUConnected { get; private set; }
        public bool IsSSConnected { get; private set; }

        private InstrumentsConnectionManager()
        {
            _RsrcMngr = new Ivi.Visa.Interop.ResourceManager();
        }

        public bool ConnectSMU(string address)
        {
            try
            {
                DisconnectSMU(); // ปิดการเชื่อมต่อก่อนหน้า (ถ้ามี)

                _SMU = new FormattedIO488();
                _SMU.IO = (IMessage)_RsrcMngr.Open(address);

                _SMU.WriteString("*IDN?");
                _SMU.WriteString("SYSTem:BEEPer 555, 0.5");
                string response = _SMU.ReadString().Trim();
                Debug.WriteLine($"[INFO] SMU Connected: {response}");

                IsSMUConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to connect to SMU: {ex.Message}");
                return false;
            }
        }

        public void DisconnectSMU()
        {
            if (_SMU?.IO != null)
            {
                try
                {
                    _SMU.WriteString("*CLS");
                    _SMU.IO.Close();
                    Marshal.FinalReleaseComObject(_SMU.IO);
                    _SMU.IO = null;
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine($"[ERROR] Failed to disconnect SMU: {Ex.Message}");
                }
                finally
                {
                    IsSMUConnected = false;
                }
            }
        }

        public bool ConnectSS(string address)
        {
            try
            {
                DisconnectSS(); // ปิดการเชื่อมต่อก่อนหน้า (ถ้ามี)

                _SS = new FormattedIO488();
                _SS.IO = (IMessage)_RsrcMngr.Open(address);

                _SS.WriteString("*IDN?");
                string response = _SS.ReadString().Trim();
                Debug.WriteLine($"[INFO] SS Connected: {response}");

                IsSSConnected = true;
                return true;
            }
            catch (Exception Ex)
            {
                Debug.WriteLine($"[ERROR] Failed to connect to SS: {Ex.Message}");
                return false;
            }
        }

        public void DisconnectSS()
        {
            if (_SS?.IO != null)
            {
                try
                {
                    _SS.WriteString("*CLS");
                    _SS.IO.Close();
                    Marshal.FinalReleaseComObject(_SS.IO);
                    _SS.IO = null;
                }
                catch (Exception Ex)
                {
                    Debug.WriteLine($"[ERROR] Failed to disconnect SS: {Ex.Message}");
                }
                finally
                {
                    IsSSConnected = false;
                }
            }
        }

        public void ReleaseAllResources()
        {
            DisconnectSMU();
            DisconnectSS();
            Marshal.FinalReleaseComObject(_RsrcMngr);
        }
    }
}
