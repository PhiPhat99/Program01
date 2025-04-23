using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CollectAndCalculateHallMeasured
{
    public static CollectAndCalculateHallMeasured _instance;
    private static readonly object _lock = new object();
    private Dictionary<int, List<(double Source, double Reading, string MeasurementType)>> _hallMeasurementsByPosition = new Dictionary<int, List<(double, double, string)>>(); // ปรับปรุง Dictionary สำหรับเก็บประเภท

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

    // เมธอดสำหรับจัดเก็บข้อมูล Hall Measurement พร้อมประเภท
    public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading, string MeasurementType)> dataPairs, string measurementType)
    {
        Debug.WriteLine($"[DEBUG] StoreMeasurementData - Tuner: {tunerPosition}, Type: {measurementType}, Data Count: {dataPairs?.Count ?? 0}");

        if (dataPairs != null)
        {
            if (!_hallMeasurementsByPosition.ContainsKey(tunerPosition))
            {
                _hallMeasurementsByPosition[tunerPosition] = new List<(double Source, double Reading, string MeasurementType)>();
            }

            _hallMeasurementsByPosition[tunerPosition].AddRange(dataPairs);
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public Dictionary<int, List<(double Source, double Reading, string MeasurementType)>> GetAllHallMeasurementsByType(string measurementType)
    {
        return _hallMeasurementsByPosition
            .Where(kvp => kvp.Value.Any(m => m.MeasurementType == measurementType))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Where(m => m.MeasurementType == measurementType).ToList());
    }

    // เมธอดสำหรับดึงข้อมูล Hall Measurement ทั้งหมด
    public Dictionary<int, List<(double Source, double Reading, string MeasurementType)>> GetAllHallMeasurementsByPosition()
    {
        return _hallMeasurementsByPosition.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public void ClearAllData()
    {
        _hallMeasurementsByPosition.Clear();
    }
}
