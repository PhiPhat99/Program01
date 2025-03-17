using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ivi.Visa.Interop;
using System.Text.RegularExpressions;
using System.Windows;

namespace Program01
{
    public class GPIBDevice : IDisposable
    {
        public FormattedIO488 Device { get; private set; }

        public GPIBDevice()
        {
            Device = new FormattedIO488();
        }

        public void Dispose()
        {
            if (Device.IO != null)
            {
                try
                {
                    Device.IO.Close();
                }
                catch { }
            }
        }
    }
}
