using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program01
{
    public class MeasurementController
    {
        private CollectVdPMeasuredValue _dataCollector;

        public MeasurementController(CollectVdPMeasuredValue dataCollector)
        {
            _dataCollector = dataCollector ?? throw new ArgumentNullException(nameof(dataCollector), "DataCollector cannot be null.");
        }

        public void SetDataCollector(CollectVdPMeasuredValue dataCollector)
        {
            _dataCollector = dataCollector ?? throw new ArgumentNullException(nameof(dataCollector), "DataCollector cannot be null.");
        }

        public void TracingRunMeasurement(List<double> XData, List<double> YData)
        {
            if (_dataCollector == null)
            {
                throw new InvalidOperationException("DataCollector is not set. Please initialize it before calling TracingRunMeasurement.");
            }

            List<(string Source, double Reading)> dataPairs = new List<(string, double)>();

            for (int i = 0; i < XData.Count; i++)
            {
                dataPairs.Add((XData[i].ToString(), YData[i]));
            }

            _dataCollector.StoreMeasurementData(dataPairs);
        }
    }
}
