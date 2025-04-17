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
                    Debug.WriteLine($"The Average {GlobalSettingsForUI.Instance.SourceModeUI}: {AverageSource} A");
                    Debug.WriteLine($"The Average {GlobalSettingsForUI.Instance.MeasureModeUI}: {AverageReading} V");

                    double Resistance;

                    if (Math.Abs(AverageSource) > 1E-9)
                    {
                        Resistance = Math.Abs((double)AverageReading / AverageSource);
                    }
                    else
                    {
                        Resistance = 0;
                        Debug.WriteLine($"[WARNING] Position {i} - Average Source is close to zero, setting Resistance to 0.");
                    }

                    _resistancesByPosition[i] = Resistance;
                    Debug.WriteLine($"[DEBUG]    Position {i} - Resistance: {Resistance} Ohm");
                }
                else
                {
                    _resistancesByPosition[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] Position {i} - No measurement data.");
                }
            }

            double SumResistanceA = 0;
            int CountA = 0;

            // รวมค่าความต้านทานจาก Position 1 และ 2
            double avgRes1_2 = 0;
            int count1_2 = 0;
            if (_resistancesByPosition.ContainsKey(1) && !double.IsNaN(_resistancesByPosition[1])) { avgRes1_2 += _resistancesByPosition[1]; count1_2++; }
            if (_resistancesByPosition.ContainsKey(2) && !double.IsNaN(_resistancesByPosition[2])) { avgRes1_2 += _resistancesByPosition[2]; count1_2++; }
            if (count1_2 > 0) { SumResistanceA += avgRes1_2 / count1_2; CountA++; }

            // รวมค่าความต้านทานจาก Position 5 และ 6
            double avgRes5_6 = 0;
            int count5_6 = 0;
            if (_resistancesByPosition.ContainsKey(5) && !double.IsNaN(_resistancesByPosition[5])) { avgRes5_6 += _resistancesByPosition[5]; count5_6++; }
            if (_resistancesByPosition.ContainsKey(6) && !double.IsNaN(_resistancesByPosition[6])) { avgRes5_6 += _resistancesByPosition[6]; count5_6++; }
            if (count5_6 > 0) { SumResistanceA += avgRes5_6 / count5_6; CountA++; }

            GlobalSettingsParseValues.Instance.ResistanceA = CountA > 0 ? SumResistanceA / CountA : double.NaN;
            Debug.WriteLine($"[DEBUG]    Resistance A: {GlobalSettingsParseValues.Instance.ResistanceA} Ohm (Count: {CountA} pairs)");

            double SumResistanceB = 0;
            int CountB = 0;

            // รวมค่าความต้านทานจาก Position 3 และ 4
            double avgRes3_4 = 0;
            int count3_4 = 0;
            if (_resistancesByPosition.ContainsKey(3) && !double.IsNaN(_resistancesByPosition[3])) { avgRes3_4 += _resistancesByPosition[3]; count3_4++; }
            if (_resistancesByPosition.ContainsKey(4) && !double.IsNaN(_resistancesByPosition[4])) { avgRes3_4 += _resistancesByPosition[4]; count3_4++; }
            if (count3_4 > 0) { SumResistanceB += avgRes3_4 / count3_4; CountB++; }

            // รวมค่าความต้านทานจาก Position 7 และ 8
            double avgRes7_8 = 0;
            int count7_8 = 0;
            if (_resistancesByPosition.ContainsKey(7) && !double.IsNaN(_resistancesByPosition[7])) { avgRes7_8 += _resistancesByPosition[7]; count7_8++; }
            if (_resistancesByPosition.ContainsKey(8) && !double.IsNaN(_resistancesByPosition[8])) { avgRes7_8 += _resistancesByPosition[8]; count7_8++; }
            if (count7_8 > 0) { SumResistanceB += avgRes7_8 / count7_8; CountB++; }

            GlobalSettingsParseValues.Instance.ResistanceB = CountB > 0 ? SumResistanceB / CountB : double.NaN;
            Debug.WriteLine($"[DEBUG]    Resistance B: {GlobalSettingsParseValues.Instance.ResistanceB} Ohm (Count: {CountB} pairs)");

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

            double ra = GlobalSettingsParseValues.Instance.ResistanceA;
            double rb = GlobalSettingsParseValues.Instance.ResistanceB;
            double initialRs = (ra + rb) / 2.0;
            double tolerance = 1E-6;
            int maxIterations = 200;
            double solvedRs = SolveVanDerPauw(ra, rb, initialRs, tolerance, maxIterations);

            if (!double.IsNaN(solvedRs))
            {
                GlobalSettingsParseValues.Instance.SheetResistance = solvedRs;
                Debug.WriteLine($"[DEBUG]    Sheet Resistance (Rs) calculated: {solvedRs} Ohm / square");
            }
            else
            {
                GlobalSettingsParseValues.Instance.SheetResistance = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Sheet Resistance ได้");
            }

            GlobalSettingsParseValues.Instance.AverageResistanceAll = CountAll > 0 ? SumResistanceAll / CountAll : double.NaN;
            GlobalSettingsParseValues.Instance.ResistancesByPosition = new Dictionary<int, double>(_resistancesByPosition);

            double Resistivity = GlobalSettingsParseValues.Instance.SheetResistance;  //  แก้ไขวิธีการคำนวณใหม่ เพราะคำนวณไม่ถูกต้อง
            
            if (!double.IsNaN (Resistivity))
            {
                GlobalSettingsParseValues.Instance.Resistivity = Resistivity;
                Debug.WriteLine($"[DEBUG]   Resistivity (ρ) calculated: {GlobalSettingsParseValues.Instance.Resistivity} Ohm ⋅ meter");
            }
            else
            {
                GlobalSettingsParseValues.Instance.Resistivity = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Resistivity ได้");
            }

            double Conductivity = 1 / GlobalSettingsParseValues.Instance.Resistivity;  //  แก้ไขวิธีการคำนวณใหม่ เพราะคำนวณไม่ถูกต้อง

            if (!double.IsNaN (Conductivity))
            {
                GlobalSettingsParseValues.Instance.Conductivity = Conductivity;
                Debug.WriteLine($"[DEBUG]   Conductivity (σ) calculated: {GlobalSettingsParseValues.Instance.Conductivity} Ohm / meter");
            }
            else
            {
                GlobalSettingsParseValues.Instance.Conductivity = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Conductivity ได้");
            }

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
