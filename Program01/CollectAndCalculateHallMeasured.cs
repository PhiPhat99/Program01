using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CollectAndCalculateHallMeasured
{
    public static CollectAndCalculateHallMeasured _instance;
    private static readonly object _lock = new object();

    private Dictionary<int, List<(double Source, double Reading)>> _hallMeasurementsWithoutField = new Dictionary<int, List<(double, double)>>();
    private Dictionary<int, List<(double Source, double Reading)>> _hallMeasurementsWithNorthField = new Dictionary<int, List<(double, double)>>();
    private Dictionary<int, List<(double Source, double Reading)>> _hallMeasurementsWithSouthField = new Dictionary<int, List<(double, double)>>();
    
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
            Dictionary<int, List<(double Source, double Reading)>> targetDictionary = null;

            switch (measurementType.ToLower())
            {
                case "withoutfield":
                    targetDictionary = _hallMeasurementsWithoutField;
                    break;
                case "northfield":
                    targetDictionary = _hallMeasurementsWithNorthField;
                    break;
                case "southfield":
                    targetDictionary = _hallMeasurementsWithSouthField;
                    break;
                default:
                    Debug.WriteLine($"[WARNING] Unknown Measurement Type: {measurementType}");
                    return;
            }

            if (targetDictionary != null)
            {
                if (!targetDictionary.ContainsKey(tunerPosition))
                {
                    targetDictionary[tunerPosition] = new List<(double Source, double Reading)>();
                }
                targetDictionary[tunerPosition].AddRange(dataPairs);
                DataUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurementsWithoutField()
    {
        return _hallMeasurementsWithoutField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurementsWithNorthField()
    {
        return _hallMeasurementsWithNorthField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurementsWithSouthField()
    {
        return _hallMeasurementsWithSouthField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetAllHallMeasurementsByType(string measurementType)
    {
        switch (measurementType.ToLower())
        {
            case "withoutfield":
                return _hallMeasurementsWithoutField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            case "northfield":
                return _hallMeasurementsWithNorthField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            case "southfield":
                return _hallMeasurementsWithSouthField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            default:
                Debug.WriteLine($"[WARNING] Unknown Measurement Type: {measurementType}");
                return new Dictionary<int, List<(double Source, double Reading)>>();
        }
    }

    public void ClearAllData()
    {
        _hallMeasurementsWithoutField.Clear();
        _hallMeasurementsWithNorthField.Clear();
        _hallMeasurementsWithSouthField.Clear();
    }
}
