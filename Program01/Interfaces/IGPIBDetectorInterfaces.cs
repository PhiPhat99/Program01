using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program01.Interfaces
{
    interface IGPIBDetectorInterfaces
    {
        List<string> GetValidInstruments(string[] DetectedAddresses);
    }
}
