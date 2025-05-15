using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static Program01.HallTotalMeasureValuesForm;

public enum MeasurementType
{
    NoMagneticField,
    InwardOrNorthMagneticField,
    OutwardOrSouthMagneticField
}

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    // สำหรับ Individual Chart
    public string StateKey { get; }
    public int TunerPosition { get; }
    public List<(double Source, double Reading)> IndividualData { get; }

    // สำหรับ Total Chart (รับข้อมูลทั้งหมด)
    public Dictionary<int, List<(double Source, double Reading)>> NoMagneticFieldData { get; }
    public Dictionary<int, List<(double Source, double Reading)>> OutwardOrSouthMagneticFieldData { get; }
    public Dictionary<int, List<(double Source, double Reading)>> InwardOrNorthMagneticFieldData { get; }

    // Constructor สำหรับ Individual Chart
    public HallVoltageDataUpdatedEventArgs(string stateKey, int tunerPosition, List<(double Source, double Reading)> individualData)
    {
        StateKey = stateKey;
        TunerPosition = tunerPosition;
        IndividualData = individualData;
    }

    public HallVoltageDataUpdatedEventArgs(
        Dictionary<int, List<(double Source, double Reading)>> noMagneticFieldData,
        Dictionary<int, List<(double Source, double Reading)>> outwardOrSouthMagneticFieldData,
        Dictionary<int, List<(double Source, double Reading)>> inwardOrNorthMagneticFieldData)
    {
        NoMagneticFieldData = noMagneticFieldData;
        OutwardOrSouthMagneticFieldData = outwardOrSouthMagneticFieldData;
        InwardOrNorthMagneticFieldData = inwardOrNorthMagneticFieldData;
    }
}

public class CollectAndCalculateHallMeasured
{
    private static CollectAndCalculateHallMeasured _instance;
    private static readonly object _lock = new object();

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallMeasurements =
        new Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>>()
        {
            { HallMeasurementState.NoMagneticField, new Dictionary<int, List<(double Source, double Reading)>>() },
            { HallMeasurementState.InwardOrNorthMagneticField, new Dictionary<int, List<(double Source, double Reading)>>() },
            { HallMeasurementState.OutwardOrSouthMagneticField, new Dictionary<int, List<(double Source, double Reading)>>() }
        };

    private Dictionary<int, double> _hallVoltageByPositionAndDirections = new Dictionary<int, double>();
    private double _hallVoltage;
    private double _concentration;
    private double _mobility;

    public double ElementaryCharge { get; set; } = 1.602176634e-19; // ประจุอิเล็กตรอน (C)

    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<Dictionary<int, double>> HallVoltageCalculated; // Event สำหรับส่งผล Hall Voltage
    public event EventHandler<(double Concentration, double Mobility)> HallPropertiesCalculated; // Event สำหรับส่งผล Concentration และ Mobility

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

    public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading)> dataPairs, MeasurementType measurementType)
    {
        Debug.WriteLine($"[DEBUG] StoreMeasurementData - Tuner: {tunerPosition}, Type: {measurementType}, Data Count: {dataPairs?.Count ?? 0}");

        if (dataPairs != null)
        {
            HallMeasurementState state;
            string stateKey;

            // ✅ ใช้ switch ปกติ แทน switch expression
            switch (measurementType)
            {
                case MeasurementType.NoMagneticField:
                    state = HallMeasurementState.NoMagneticField;
                    stateKey = "Out";
                    break;
                case MeasurementType.InwardOrNorthMagneticField:
                    state = HallMeasurementState.InwardOrNorthMagneticField;
                    stateKey = "North";
                    break;
                case MeasurementType.OutwardOrSouthMagneticField:
                    state = HallMeasurementState.OutwardOrSouthMagneticField;
                    stateKey = "South";
                    break;
                default:
                    Debug.WriteLine($"[WARNING] Unknown Measurement Type: {measurementType}");
                    return;
            }

            if (!allHallMeasurements[state].ContainsKey(tunerPosition))
            {
                allHallMeasurements[state][tunerPosition] = new List<(double Source, double Reading)>();
            }

            allHallMeasurements[state][tunerPosition].AddRange(dataPairs);
            Debug.WriteLine($"[DEBUG] StoreMeasurementData - Data stored for State: {state}, Tuner: {tunerPosition}, Total Data Points: {allHallMeasurements[state].Sum(kvp => kvp.Value.Count)}");

            // ✅ Event สำหรับ Individual Chart
            var individualArgs = new HallVoltageDataUpdatedEventArgs(stateKey, tunerPosition, new List<(double Source, double Reading)>(dataPairs));
            OnDataUpdated(individualArgs);
            Debug.WriteLine($"[DEBUG] StoreMeasurementData - Triggering IndividualChart Event: {stateKey}{tunerPosition}, {dataPairs.Count} points");

            // ✅ Event สำหรับ Total Chart
            var totalEventArgs = new HallVoltageDataUpdatedEventArgs(
                GetHallMeasurements(HallMeasurementState.NoMagneticField),
                GetHallMeasurements(HallMeasurementState.OutwardOrSouthMagneticField),
                GetHallMeasurements(HallMeasurementState.InwardOrNorthMagneticField)
            );
            OnDataUpdated(totalEventArgs);
            Debug.WriteLine("[DEBUG] StoreMeasurementData - Triggering DataUpdated event for TOTAL CHART");
        }
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurements(HallMeasurementState state)
    {
        Debug.WriteLine($"[DEBUG] GetHallMeasurements - Retrieving data for State: {state}");
        return allHallMeasurements[state]?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()) ?? new Dictionary<int, List<(double Source, double Reading)>>();
    }

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> GetAllHallMeasurements()
    {
        Debug.WriteLine("[DEBUG] GetAllHallMeasurements - Retrieving all Hall measurement data.");
        return allHallMeasurements.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToDictionary(
                innerKvp => innerKvp.Key,
                innerKvp => innerKvp.Value.ToList()
            )
        );
    }

    protected virtual void OnDataUpdated(HallVoltageDataUpdatedEventArgs e)
    {
        DataUpdated?.Invoke(this, e);
    }

    protected virtual void OnHallVoltageCalculated(Dictionary<int, double> hallVoltages)
    {
        HallVoltageCalculated?.Invoke(this, hallVoltages);
    }

    protected virtual void OnHallPropertiesCalculated((double Concentration, double Mobility) properties)
    {
        HallPropertiesCalculated?.Invoke(this, properties);
    }

    protected virtual void OnCalculationCompleted(EventArgs e = null)
    {
        CalculationCompleted?.Invoke(this, e);
    }

    public void ClearAllData()
    {
        Debug.WriteLine("[DEBUG] ClearAllData - Clearing all stored Hall measurement data.");
        foreach (var state in allHallMeasurements.Keys)
        {
            allHallMeasurements[state].Clear();
        }

        ClearHallResults(); // เคลียร์ผลการคำนวณด้วย
    }

    public void ClearHallResults()
    {
        _hallVoltageByPositionAndDirections.Clear();
        _hallVoltage = 0;
        _concentration = 0;
        _mobility = 0;
        Debug.WriteLine("[DEBUG] ClearHallResults() called");
    }

    public void CalculateHall()
    {
        ClearHallResults();
        Debug.WriteLine("[DEBUG] CalculateHall() called");

        // ตรวจสอบว่ามีข้อมูลเพียงพอสำหรับการคำนวณหรือไม่
        if (!allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(1) ||
            !allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(2) ||
            !allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(3) ||
            !allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(4) ||
            !allHallMeasurements[HallMeasurementState.NoMagneticField].ContainsKey(1) ||
            !allHallMeasurements[HallMeasurementState.NoMagneticField].ContainsKey(2) ||
            !allHallMeasurements[HallMeasurementState.NoMagneticField].ContainsKey(3) ||
            !allHallMeasurements[HallMeasurementState.NoMagneticField].ContainsKey(4) ||
            !allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(1) ||
            !allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(2) ||
            !allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(3) ||
            !allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(4))
        {
            Debug.WriteLine("[WARNING] CalculateHall() - Not enough data to perform calculation.");
            return;
        }

        // ดึงค่า Reading ล่าสุดของแต่ละตำแหน่ง (สมมติว่า Reading ล่าสุดคือค่าที่ต้องการ)
        double V_South1 = allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField][1].Last().Reading;
        double V_South2 = allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField][2].Last().Reading;
        double V_South3 = allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField][3].Last().Reading;
        double V_South4 = allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField][4].Last().Reading;

        double V_Out1 = allHallMeasurements[HallMeasurementState.NoMagneticField][1].Last().Reading;
        double V_Out2 = allHallMeasurements[HallMeasurementState.NoMagneticField][2].Last().Reading;
        double V_Out3 = allHallMeasurements[HallMeasurementState.NoMagneticField][3].Last().Reading;
        double V_Out4 = allHallMeasurements[HallMeasurementState.NoMagneticField][4].Last().Reading;

        double V_North1 = allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField][1].Last().Reading;
        double V_North2 = allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField][2].Last().Reading;
        double V_North3 = allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField][3].Last().Reading;
        double V_North4 = allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField][4].Last().Reading;

        // 1. คำนวณ _hallVoltageByPositionAndDirections
        _hallVoltageByPositionAndDirections[1] = V_South1 - V_Out1;
        _hallVoltageByPositionAndDirections[2] = V_South2 - V_Out2;
        _hallVoltageByPositionAndDirections[3] = V_South3 - V_Out3;
        _hallVoltageByPositionAndDirections[4] = V_South4 - V_Out4;
        _hallVoltageByPositionAndDirections[5] = V_North1 - V_Out1;
        _hallVoltageByPositionAndDirections[6] = V_North2 - V_Out2;
        _hallVoltageByPositionAndDirections[7] = V_North3 - V_Out3;
        _hallVoltageByPositionAndDirections[8] = V_North4 - V_Out4;

        // Trigger Event สำหรับ Hall Voltage by Position
        OnHallVoltageCalculated(_hallVoltageByPositionAndDirections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        Debug.WriteLine("[DEBUG] CalculateHall() - Hall Voltage by Position Calculated and Event Triggered.");

        // 2. คำนวณ _hallVoltage
        _hallVoltage = (_hallVoltageByPositionAndDirections[1] - _hallVoltageByPositionAndDirections[2] + _hallVoltageByPositionAndDirections[3] - _hallVoltageByPositionAndDirections[4] +
                      _hallVoltageByPositionAndDirections[5] - _hallVoltageByPositionAndDirections[6] + _hallVoltageByPositionAndDirections[7] - _hallVoltageByPositionAndDirections[8]) / 8;
        Debug.WriteLine($"[DEBUG] CalculateHall() - Hall Voltage (V_H): {_hallVoltage}");

        // 3. คำนวณ Concentration (n)
        if (GlobalSettings.Instance.ThicknessValueStd > 0 && ElementaryCharge > 0 && GlobalSettings.Instance.MagneticFieldsValueStd != 0)
        {
            _concentration = GlobalSettings.Instance.MagneticFieldsValueStd / (GlobalSettings.Instance.ThicknessValueStd * ElementaryCharge * _hallVoltage);
            GlobalSettings.Instance.Concentration = _concentration;
            Debug.WriteLine($"[DEBUG] CalculateHall() - Concentration (n): {GlobalSettings.Instance.Concentration}");

            // 4. คำนวณ Mobility (u)
            if (GlobalSettings.Instance.Resistivity > 0 && _concentration != 0)
            {
                _mobility = 1 / (ElementaryCharge * _concentration * GlobalSettings.Instance.Resistivity);
                GlobalSettings.Instance.Mobility = _mobility;
                Debug.WriteLine($"[DEBUG] CalculateHall() - Mobility (u): {GlobalSettings.Instance.Mobility}");

                // Trigger Event สำหรับ Concentration และ Mobility
                OnHallPropertiesCalculated((_concentration, _mobility));
                Debug.WriteLine("[DEBUG] CalculateHall() - Concentration and Mobility Calculated and Event Triggered");
            }
            else
            {
                Debug.WriteLine("[WARNING] CalculateHall() - Cannot calculate Mobility. Resistivity is zero or Concentration is zero");
            }
        }
        else
        {
            Debug.WriteLine("[WARNING] CalculateHall() - Cannot calculate Concentration. Thickness or Elementary Charge is zero, or Magnetic Field is zero");
        }

        OnCalculationCompleted();
        Debug.WriteLine("[DEBUG] CalculateHall() - Calculation Completed");
    }
}