using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ivi.Visa.Interop;
using System.Text.RegularExpressions;
using System.Windows;

namespace Program01
{
    public class VISAPorts : IDisposable
    {
        public FormattedIO488 VISAports { get; private set; }

        public VISAPorts()
        {
            VISAports = new FormattedIO488();
        }

        public void Dispose()
        {
            if (VISAports.IO != null)
            {
                try
                {
                    VISAports.IO.Close();
                }
                catch { }
            }
        }
    }
}
