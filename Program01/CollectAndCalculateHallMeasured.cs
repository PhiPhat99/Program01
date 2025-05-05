using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CollectAndCalculateHallMeasured
{
    public static CollectAndCalculateHallMeasured _instance;
    private static readonly object _lock = new object();

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allHallMeasurements =
        new Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>>()
        {
            { HallMeasurementState.NoMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.InwardOrNorthMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.OutwardOrSouthMagneticField, new Dictionary<int, List<(double, double)>>() }
        };

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

    public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading)> dataPairs, string measurementType)
    {
        Debug.WriteLine($"[DEBUG] StoreMeasurementData - Tuner: {tunerPosition}, Type: {measurementType}, Data Count: {dataPairs?.Count ?? 0}");

        if (dataPairs != null)
        {
            HallMeasurementState state;

            switch (measurementType.ToLower())
            {
                case "nomagneticfield":
                    state = HallMeasurementState.NoMagneticField;
                    break;
                case "inwardornorthmagneticfield":
                    state = HallMeasurementState.InwardOrNorthMagneticField;
                    break;
                case "outwardorsouthmagneticfield":
                    state = HallMeasurementState.OutwardOrSouthMagneticField;
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
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurements(HallMeasurementState state)
    {
        Debug.WriteLine($"[DEBUG] GetHallMeasurements - Retrieving data for State: {state}");
        return allHallMeasurements[state].ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
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

    public void ClearAllData()
    {
        Debug.WriteLine("[DEBUG] ClearAllData - Clearing all stored Hall measurement data.");
        foreach (var state in allHallMeasurements.Keys)
        {
            allHallMeasurements[state].Clear();
        }
    }
}