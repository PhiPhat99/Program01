using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Program01
{
    public class CollectAndCalculateVdPMeasured
    {
        public static CollectAndCalculateVdPMeasured _instance;
        private static readonly object _lock = new object();
        private Dictionary<int, List<(double Source, double Reading)>> _measurementsByTuner = new Dictionary<int, List<(double, double)>>();
        private Dictionary<int, double> _resistancesByPosition = new Dictionary<int, double>();
        public event EventHandler DataUpdated;

        private CollectAndCalculateVdPMeasured() { }

        public static CollectAndCalculateVdPMeasured Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CollectAndCalculateVdPMeasured();
                        }
                    }
                }
                return _instance;
            }
        }

        public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading)> dataPairs)
        {
            Debug.WriteLine($"[DEBUG] StoreMeasurementData - Tuner: {tunerPosition}, Data Count: {dataPairs?.Count ?? 0}");

            if (dataPairs != null)
            {
                foreach (var data in dataPairs)
                {
                    Debug.WriteLine($"[DEBUG]    Source: {data.Source}, Reading: {data.Reading}");
                }
            }

            if (!_measurementsByTuner.ContainsKey(tunerPosition))
            {
                _measurementsByTuner[tunerPosition] = new List<(double, double)>();
            }

            _measurementsByTuner[tunerPosition].AddRange(dataPairs);
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Dictionary<int, List<(double Source, double Reading)>> GetAllMeasurementsByTuner()
        {
            return _measurementsByTuner;
        }

        public List<(double Source, double Reading)> GetAllData()
        {
            List<(double Source, double Reading)> allData = new List<(double, double)>();

            foreach (var measurements in _measurementsByTuner.Values)
            {
                allData.AddRange(measurements);
            }

            return allData;
        }

        public void ClearAllData()
        {
            _measurementsByTuner.Clear();
        }

        public void CalculateVanderPauw()
        {
            ClearResults();

            for (int i = 1; i <= 8; i++)
            {
                if (_measurementsByTuner.ContainsKey(i) && _measurementsByTuner[i].Count > 0)
                {
                    List<(double Source, double Reading)> measurementData = _measurementsByTuner[i];

                    double SumSource = 0;
                    double SumReading = 0;

                    foreach (var dataPoint in measurementData)
                    {
                        SumSource += dataPoint.Source;
                        SumReading += dataPoint.Reading;
                    }

                    double AverageSource = SumSource / measurementData.Count;
                    double AverageReading = SumReading / measurementData.Count;

                    double Resistance = 0;

                    if (Math.Abs(AverageSource) > 1e-9)
                    {
                        Resistance = AverageReading / AverageSource;
                    }
                    else
                    {
                        Resistance = 0;
                    }

                    _resistancesByPosition[i] = Resistance;

                }
                else
                {
                    _resistancesByPosition[i] = double.NaN;
                }

                
            }
        }

        public void ClearResults()
        {
            _resistancesByPosition.Clear();
        }
    }
}

