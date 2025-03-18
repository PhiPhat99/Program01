using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Program01
{
    public class VdPMeasurementData
    {
        private static VdPMeasurementData instance;

        public List<double> Voltages { get; set; } = new List<double>();

        private VdPMeasurementData() { }

        public static VdPMeasurementData Instance
        {
            get
            {
                if (instance == null)
                    instance = new VdPMeasurementData();
                return instance;
            }
        }


        public void AddMeasurement(double voltage1, double voltage2, double voltage3, double voltage4, double voltage5, double voltage6, double voltage7, double voltage8)
        {
            Voltages.Add(voltage1);
            Voltages.Add(voltage2);
            Voltages.Add(voltage3);
            Voltages.Add(voltage4);
            Voltages.Add(voltage5);
            Voltages.Add(voltage6);
            Voltages.Add(voltage7);
            Voltages.Add(voltage8);
        }

        public void ClearMeasurements()
        {
            Voltages.Clear();
        }
    }

    public class CollectVdPVoltage
    {
        public static VdPMeasurementData Instance => VdPMeasurementData.Instance;
    }
}

