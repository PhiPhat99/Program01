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
        public event EventHandler CalculationCompleted;

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
            Debug.WriteLine("[DEBUG] CalculateVanderPauw() called");

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
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.SourceMode}: {AverageSource} {GlobalSettings.Instance.StartUnit}");
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.MeasureMode}: {AverageReading} {GlobalSettings.Instance.SourceLimitLevelUnit}");

                    double Resistance = 0;

                    if (Math.Abs(AverageSource) > 1e-9)
                    {
                        Resistance = Math.Abs(AverageReading / AverageSource);
                    }
                    else
                    {
                        Resistance = 0;
                        Debug.WriteLine($"[WARNING] Position {i} - Average Source is close to zero, setting Resistance to NaN.");
                    }

                    _resistancesByPosition[i] = Resistance;
                    Debug.WriteLine($"[DEBUG]   Position {i} - Resistance: {Resistance} Ohm");
                }
                else
                {
                    _resistancesByPosition[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] Position {i} - Average Source is close to zero, setting Resistance to NaN");
                }
            }

            double SumResistanceA = 0;
            int CountA = 0;

            for (int i = 1; i <= 4; i++)
            {
                if (_resistancesByPosition.ContainsKey(i) && !double.IsNaN(_resistancesByPosition[i]))
                {
                    SumResistanceA += _resistancesByPosition[i];
                    CountA++;
                }
            }

            GlobalSettings.Instance.ResistanceA = CountA > 0 ? SumResistanceA / CountA : double.NaN;
            Debug.WriteLine($"[DEBUG]   Resistance A: {GlobalSettings.Instance.ResistanceA} Ohm (Count: {CountA})");

            double SumResistanceB = 0;
            int CountB = 0;

            for (int i = 5; i <= 8; i++)
            {
                if (_resistancesByPosition.ContainsKey(i) && !double.IsNaN(_resistancesByPosition[i]))
                {
                    SumResistanceB += _resistancesByPosition[i];
                    CountB++;
                }
            }

            GlobalSettings.Instance.ResistanceB = CountB > 0 ? SumResistanceB / CountB : double.NaN;
            Debug.WriteLine($"[DEBUG]   Resistance B: {GlobalSettings.Instance.ResistanceB} Ohm (Count: {CountB})");

            double SumResistanceAll = 0;
            int CountAll = 0;

            foreach (var resistance in _resistancesByPosition.Values)
            {
                if (!double.IsNaN(resistance))
                {
                    SumResistanceAll += resistance;
                    CountAll++;
                }
            }

            GlobalSettings.Instance.AverageResistanceAll = CountAll > 0 ? SumResistanceAll / CountAll : double.NaN;
            GlobalSettings.Instance.ResistancesByPosition = new Dictionary<int, double>(_resistancesByPosition);
            
            CalculationCompleted?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("[DEBUG] CalculateVanderPauw() completed and CalculationCompleted event invoked");
        }

        public void ClearResults()
        {
            _resistancesByPosition.Clear();
            Debug.WriteLine("[DEBUG] ClearResults() called");
        }
    }
}

