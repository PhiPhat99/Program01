using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                foreach ((double Source, double Reading) in dataPairs)
                {
                    Debug.WriteLine($"[DEBUG]    Source: {Source}, Reading: {Reading}");
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

        private static double F(double rs, double ra, double rb)
        {
            return Math.Exp(-Math.PI * ra / rs) + Math.Exp(-Math.PI * rb / rs) - 1.0;
        }

        private static double FPrime(double rs, double ra, double rb)
        {
            double termA = (Math.PI * ra / (rs * rs)) * Math.Exp(-Math.PI * ra / rs);
            double termB = (Math.PI * rb / (rs * rs)) * Math.Exp(-Math.PI * rb / rs);
            return termA + termB;
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

                    foreach ((double Source, double Reading) in measurementData)
                    {
                        SumSource += Source;
                        SumReading += Reading;
                    }

                    double AverageSource = SumSource / measurementData.Count;
                    double AverageReading = SumReading / measurementData.Count;
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.SourceMode}: {AverageSource} A");
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.MeasureMode}: {AverageReading} V");

                    double Resistance;

                    if (Math.Abs(AverageSource) > 1E-9)
                    {
                        Resistance = Math.Abs(AverageReading / AverageSource);
                    }
                    else
                    {
                        Resistance = 0;
                        Debug.WriteLine($"[WARNING] Position {i} - Average Source is close to zero, setting Resistance to NaN.");
                    }

                    _resistancesByPosition[i] = Resistance;
                    Debug.WriteLine($"[DEBUG]    Position {i} - Resistance: {Resistance} Ohm");
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

            double ra = GlobalSettings.Instance.ResistanceA;
            double rb = GlobalSettings.Instance.ResistanceB;
            double initialRs = (ra + rb) / 2.0;
            double tolerance = 1E-6;
            int maxIterations = 200;

            double solvedRs = SolveVanDerPauw(ra, rb, initialRs, tolerance, maxIterations);

            if (!double.IsNaN(solvedRs))
            {
                GlobalSettings.Instance.SheetResistance = solvedRs;
                Debug.WriteLine($"[DEBUG]    Sheet Resistance (Rs) calculated: {solvedRs} Ohm/square");
            }
            else
            {
                GlobalSettings.Instance.SheetResistance = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Sheet Resistance ได้");
            }

            GlobalSettings.Instance.AverageResistanceAll = (double)_resistancesByPosition.Values.Where(r => !double.IsNaN(r)).Sum() / _resistancesByPosition.Values.Count(r => !double.IsNaN(r));
            GlobalSettings.Instance.ResistancesByPosition = new Dictionary<int, double>(_resistancesByPosition);

            CalculationCompleted?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("[DEBUG] CalculateVanderPauw() completed and CalculationCompleted event invoked");
        }

        private static double SolveVanDerPauw(double ra, double rb, double initialRs, double tolerance, int maxIterations)
        {
            double rs = initialRs;

            for (int i = 0; i < maxIterations; i++)
            {
                double fValue = F(rs, ra, rb);
                double fPrimeValue = FPrime(rs, ra, rb);

                if (Math.Abs(fPrimeValue) < 1E-12)
                {
                    Debug.WriteLine("[WARNING] อนุพันธ์มีค่าใกล้เคียงศูนย์ การลู่เข้าอาจมีปัญหา");
                    return double.NaN;
                }

                double nextRs = rs - fValue / fPrimeValue;

                if (Math.Abs(nextRs - rs) < tolerance)
                {
                    return nextRs;
                }

                rs = nextRs;
            }

            Debug.WriteLine($"[WARNING] จำนวนรอบการทำซ้ำเกินค่าที่กำหนด ({maxIterations}) อาจไม่ลู่เข้าสู่คำตอบ");
            return double.NaN;
        }

        public void ClearResults()
        {
            _resistancesByPosition.Clear();
            Debug.WriteLine("[DEBUG] ClearResults() called");
        }
    }
}

