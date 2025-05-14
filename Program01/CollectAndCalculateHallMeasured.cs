using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Program01.HallTotalMeasureValuesForm;

public enum MeasurementType
{
    NoMagneticField,
    InwardOrNorthMagneticField,
    OutwardOrSouthMagneticField
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

    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
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

    public void ClearAllData()
    {
        Debug.WriteLine("[DEBUG] ClearAllData - Clearing all stored Hall measurement data.");
        foreach (var state in allHallMeasurements.Keys)
        {
            allHallMeasurements[state].Clear();
        }
    }
}