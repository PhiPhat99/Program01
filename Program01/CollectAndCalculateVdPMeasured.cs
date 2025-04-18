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

        private static double F(double Rs, double Ra, double Rb)
        {
            return Math.Exp(-Math.PI * Ra / Rs) + Math.Exp(-Math.PI * Rb / Rs) - 1.0;
        }

        private static double FPrime(double Rs, double Ra, double Rb)
        {
            double TermA = (Math.PI * Ra / (Rs * Rs)) * Math.Exp(-Math.PI * Ra / Rs);
            double TermB = (Math.PI * Rb / (Rs * Rs)) * Math.Exp(-Math.PI * Rb / Rs);
            return TermA + TermB;
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
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.SourceModeUI}: {AverageSource} A");
                    Debug.WriteLine($"The Average {GlobalSettings.Instance.MeasureModeUI}: {AverageReading} V");

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
            double AvgRes1_2 = 0;
            int Count1_2 = 0;
            if (_resistancesByPosition.ContainsKey(1) && !double.IsNaN(_resistancesByPosition[1])) { AvgRes1_2 += _resistancesByPosition[1]; Count1_2++; }
            if (_resistancesByPosition.ContainsKey(2) && !double.IsNaN(_resistancesByPosition[2])) { AvgRes1_2 += _resistancesByPosition[2]; Count1_2++; }
            if (Count1_2 > 0) { SumResistanceA += AvgRes1_2 / Count1_2; CountA++; }

            // รวมค่าความต้านทานจาก Position 3 และ 4
            double AvgRes3_4 = 0;
            int Count3_4 = 0;
            if (_resistancesByPosition.ContainsKey(3) && !double.IsNaN(_resistancesByPosition[3])) { AvgRes3_4 += _resistancesByPosition[3]; Count3_4++; }
            if (_resistancesByPosition.ContainsKey(4) && !double.IsNaN(_resistancesByPosition[4])) { AvgRes3_4 += _resistancesByPosition[4]; Count3_4++; }
            if (Count3_4 > 0) { SumResistanceA += AvgRes3_4 / Count3_4; CountA++; }

            GlobalSettings.Instance.ResistanceA = CountA > 0 ? SumResistanceA / CountA : double.NaN;
            Debug.WriteLine($"[DEBUG]    Resistance A: {GlobalSettings.Instance.ResistanceA} Ohm (Count: {CountA} pairs)");

            double SumResistanceB = 0;
            int CountB = 0;

            // รวมค่าความต้านทานจาก Position 5 และ 6
            double AvgRes5_6 = 0;
            int Count5_6 = 0;
            if (_resistancesByPosition.ContainsKey(5) && !double.IsNaN(_resistancesByPosition[5])) { AvgRes5_6 += _resistancesByPosition[5]; Count5_6++; }
            if (_resistancesByPosition.ContainsKey(6) && !double.IsNaN(_resistancesByPosition[6])) { AvgRes5_6 += _resistancesByPosition[6]; Count5_6++; }
            if (Count5_6 > 0) { SumResistanceB += AvgRes5_6 / Count5_6; CountB++; }

            // รวมค่าความต้านทานจาก Position 7 และ 8
            double AvgRes7_8 = 0;
            int Count7_8 = 0;
            if (_resistancesByPosition.ContainsKey(7) && !double.IsNaN(_resistancesByPosition[7])) { AvgRes7_8 += _resistancesByPosition[7]; Count7_8++; }
            if (_resistancesByPosition.ContainsKey(8) && !double.IsNaN(_resistancesByPosition[8])) { AvgRes7_8 += _resistancesByPosition[8]; Count7_8++; }
            if (Count7_8 > 0) { SumResistanceB += AvgRes7_8 / Count7_8; CountB++; }

            GlobalSettings.Instance.ResistanceB = CountB > 0 ? SumResistanceB / CountB : double.NaN;
            Debug.WriteLine($"[DEBUG]    Resistance B: {GlobalSettings.Instance.ResistanceB} Ohm (Count: {CountB} pairs)");

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

            double Res_A = GlobalSettings.Instance.ResistanceA;
            double Res_B = GlobalSettings.Instance.ResistanceB;
            double InitialRs = (Res_A + Res_B) / 2.0;
            double Tolerance = 1E-6;
            int MaxIterations = 200;
            double SolvedRs = SolveVanDerPauw(Res_A, Res_B, InitialRs, Tolerance, MaxIterations);

            if (!double.IsNaN(SolvedRs))
            {
                GlobalSettings.Instance.SheetResistance = SolvedRs;
                Debug.WriteLine($"[DEBUG]    Sheet Resistance (Rs) calculated: {SolvedRs} Ohm / square");
            }
            else
            {
                GlobalSettings.Instance.SheetResistance = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Sheet Resistance ได้");
            }

            GlobalSettings.Instance.AverageResistanceAll = CountAll > 0 ? SumResistanceAll / CountAll : double.NaN;
            GlobalSettings.Instance.ResistancesByPosition = new Dictionary<int, double>(_resistancesByPosition);

            double Resistivity = GlobalSettings.Instance.SheetResistance * GlobalSettings.Instance.ThicknessValueStd;  //  แก้ไขวิธีการคำนวณใหม่ เพราะคำนวณไม่ถูกต้อง
            
            if (!double.IsNaN (Resistivity))
            {
                GlobalSettings.Instance.Resistivity = Resistivity;
                Debug.WriteLine($"[DEBUG]   Resistivity (ρ) calculated: {GlobalSettings.Instance.Resistivity} Ohm ⋅ meter");
            }
            else
            {
                GlobalSettings.Instance.Resistivity = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Resistivity ได้");
            }

            double Conductivity = 1 / GlobalSettings.Instance.Resistivity;  //  แก้ไขวิธีการคำนวณใหม่ เพราะคำนวณไม่ถูกต้อง

            if (!double.IsNaN (Conductivity))
            {
                GlobalSettings.Instance.Conductivity = Conductivity;
                Debug.WriteLine($"[DEBUG]   Conductivity (σ) calculated: {GlobalSettings.Instance.Conductivity} Ohm / meter");
            }
            else
            {
                GlobalSettings.Instance.Conductivity = double.NaN;
                Debug.WriteLine("[DEBUG]    ไม่สามารถหาค่า Conductivity ได้");
            }

            CalculationCompleted?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("[DEBUG] CalculateVanderPauw() completed and CalculationCompleted event invoked");
        }

        private static double SolveVanDerPauw(double Ra, double Rb, double InitialRs, double Tolerance, int MaxIterations)
        {
            double Rs = InitialRs;

            for (int i = 0; i < MaxIterations; i++)
            {
                double fValue = F(Rs, Ra, Rb);
                double fPrimeValue = FPrime(Rs, Ra, Rb);

                if (Math.Abs(fPrimeValue) < 1E-12)
                {
                    Debug.WriteLine("[WARNING] อนุพันธ์มีค่าใกล้เคียงศูนย์ การลู่เข้าอาจมีปัญหา");
                    return double.NaN;
                }

                double NextRs = Rs - fValue / fPrimeValue;

                if (Math.Abs(NextRs - Rs) < Tolerance)
                {
                    return NextRs;
                }

                Rs = NextRs;
            }

            Debug.WriteLine($"[WARNING] จำนวนรอบการทำซ้ำเกินค่าที่กำหนด ({MaxIterations}) อาจไม่ลู่เข้าสู่คำตอบ");
            return double.NaN;
        }

        public void ClearResults()
        {
            _resistancesByPosition.Clear();
            Debug.WriteLine("[DEBUG] ClearResults() called");
        }
    }
}
