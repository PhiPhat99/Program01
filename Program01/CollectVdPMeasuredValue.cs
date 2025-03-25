using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Program01
{
    public class VdPMeasurementData
    {
        private static VdPMeasurementData instance;
        public List<double> VdPMeasured { get; set; } = new List<double>();

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

        public void AddMeasurement(List<double> measurements)
        {
            VdPMeasured.AddRange(measurements);
        }

        public void ClearMeasurements()
        {
            VdPMeasured.Clear();
        }
    }

    public class CollectVdPMeasuredValue
    {
        public static VdPMeasurementData Instance => VdPMeasurementData.Instance;
    }
}

