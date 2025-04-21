using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Program01
{
    public class CollectAndCalculateHallMeasured
    {
        public static CollectAndCalculateHallMeasured _instance;
        private static readonly object _lock = new object();
        private Dictionary<int, List<(double Source, double Reading)>> _measurementsByPosition = new Dictionary<int, List<(double, double)>>();
        public event EventHandler DataUpdated;
        public event EventHandler CalculationCompleted;

        private CollectAndCalculateHallMeasured() { }

        public static CollectAndCalculateHallMeasured Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CollectAndCalculateHallMeasured();
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
                foreach ((double Source, double Reading) in dataPairs)
                {
                    Debug.WriteLine($"[DEBUG]    Source: {Source}, Reading: {Reading}");
                }
            }

            if (!_measurementsByPosition.ContainsKey(tunerPosition))
            {
                _measurementsByPosition[tunerPosition] = new List<(double, double)>();
            }

            _measurementsByPosition[tunerPosition].AddRange(dataPairs);
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Dictionary<int, List<(double Source, double Reading)>> GetAllMeasurementsByTuner()
        {
            return _measurementsByPosition;
        }

        public List<(double Source, double Reading)> GetAllData()
        {
            List<(double Source, double Reading)> allData = new List<(double, double)>();

            foreach (var measurements in _measurementsByPosition.Values)
            {
                allData.AddRange(measurements);
            }

            return allData;
        }

        public void ClearAllData()
        {
            _measurementsByPosition.Clear();
        }
    }
}

