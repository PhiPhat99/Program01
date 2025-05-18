using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    private double _bulkConcentration;
    private double _sheetConcentration;
    private double _mobility;

    public double ElementaryCharge { get; set; } = 1.602176634e-19; // ประจุอิเล็กตรอน (C)

    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<Dictionary<int, double>> HallVoltageCalculated; // Event สำหรับส่งผล Hall Voltage
    public event EventHandler<(double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility)> HallPropertiesCalculated;
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

    public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading)> dataPairs, HallMeasurementState State)
    {
        Debug.WriteLine($"[DEBUG] StoreMeasurementData - Tuner: {tunerPosition}, Type: {State}, Data Count: {dataPairs?.Count ?? 0}");

        if (dataPairs != null)
        {
            HallMeasurementState state;
            string stateKey;

            switch (State)
            {
                case HallMeasurementState.NoMagneticField:
                    state = HallMeasurementState.NoMagneticField;
                    stateKey = "Out";
                    break;
                case HallMeasurementState.InwardOrNorthMagneticField:
                    state = HallMeasurementState.InwardOrNorthMagneticField;
                    stateKey = "North";
                    break;
                case HallMeasurementState.OutwardOrSouthMagneticField:
                    state = HallMeasurementState.OutwardOrSouthMagneticField;
                    stateKey = "South";
                    break;
                default:
                    Debug.WriteLine($"[WARNING] Unknown Measurement Type: {State}");
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

    protected virtual void OnHallPropertiesCalculated((double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility) properties)
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
        _bulkConcentration = 0;
        _sheetConcentration = 0;
        _mobility = 0;
        Debug.WriteLine("[DEBUG] ClearHallResults() called");
    }

    private double CalculateIVHallSlope(List<(double Current, double Voltage)> dataPoints)
    {
        if (dataPoints == null || dataPoints.Count < 2)
        {
            Debug.WriteLine("[WARNING] CalculateIVHallSlope - Not enough data points to calculate slope.");
            return 0; // หรือค่าที่เหมาะสมกับการจัดการ Error ของคุณ
        }

        double sumX = 0;
        double sumY = 0;
        double sumXY = 0;
        double sumX2 = 0;
        int n = dataPoints.Count;

        foreach ((double current, double voltage) in dataPoints)
        {
            sumX += current;
            sumY += voltage;
            sumXY += current * voltage;
            sumX2 += current * current;
        }

        double meanX = sumX / n;
        double meanY = sumY / n;

        double numerator = sumXY - n * meanX * meanY;
        double denominator = sumX2 - n * meanX * meanX;

        if (denominator == 0)
        {
            Debug.WriteLine("[WARNING] CalculateIVHallSlope - Denominator is zero, cannot calculate slope.");
            return double.NaN; // หรือค่าที่เหมาะสมกับการจัดการ Error ของคุณ
        }

        double slope = numerator / denominator;
        Debug.WriteLine($"[DEBUG] CalculateIVHallSlope - Slope calculated using simple linear regression: {slope}");
        return slope;
    }

    public void CalculateHall()
    {
        ClearHallResults();
        Debug.WriteLine("[DEBUG] CalculateHall() called");

        Dictionary<int, double> averageSlopes = new Dictionary<int, double>();
        Dictionary<int, double> hallVoltagesByPosition = new Dictionary<int, double>(); // Dictionary สำหรับเก็บ Voltages

        for (int position = 1; position <= 4; position++)
        {
            if (allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(position) &&
                allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(position) &&
                allHallMeasurements[HallMeasurementState.NoMagneticField].ContainsKey(position))
            {
                List<(double Source, double Reading)> southData = allHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField][position].OrderBy(d => d.Source).ToList(); // Order by Source เพื่อให้จับคู่ได้ง่าย
                List<(double Source, double Reading)> northData = allHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField][position].OrderBy(d => d.Source).ToList();
                List<(double Source, double Reading)> outData = allHallMeasurements[HallMeasurementState.NoMagneticField][position].OrderBy(d => d.Source).ToList();

                // ดึงค่า Voltage สำหรับแต่ละตำแหน่งและสภาวะ (ใช้ค่าแรกที่พบ)
                if (outData.Count > 0) hallVoltagesByPosition[position] = outData[0].Reading;
                if (southData.Count > 0) hallVoltagesByPosition[position + 4] = southData[0].Reading; // Index 5-8 สำหรับ South
                if (northData.Count > 0) hallVoltagesByPosition[position + 8] = northData[0].Reading; // Index 9-12 สำหรับ North

                if (southData.Count >= 2 && northData.Count >= 2 && outData.Count >= 2)
                {
                    List<(double Current, double HallVoltage)> southHallVoltageData = new List<(double Current, double HallVoltage)>();
                    List<(double Current, double HallVoltage)> northHallVoltageData = new List<(double Current, double HallVoltage)>();

                    for (int i = 0; i < southData.Count; i++)
                    {
                        if (i < outData.Count && southData[i].Source == outData[i].Source)
                        {
                            southHallVoltageData.Add((southData[i].Source, southData[i].Reading - outData[i].Reading));
                        }
                        if (i < northData.Count && northData[i].Source == outData[i].Source)
                        {
                            northHallVoltageData.Add((northData[i].Source, northData[i].Reading - outData[i].Reading));
                        }
                    }

                    double slopeSouth = CalculateIVHallSlope(southHallVoltageData);
                    double slopeNorth = CalculateIVHallSlope(northHallVoltageData);

                    if (!double.IsNaN(slopeSouth) && !double.IsNaN(slopeNorth))
                    {
                        averageSlopes[position] = (slopeSouth + slopeNorth) / 2;
                        Debug.WriteLine($"[DEBUG] CalculateHall - Average Slope for Position {position}: {averageSlopes[position]}");
                    }
                    else
                    {
                        Debug.WriteLine($"[WARNING] CalculateHall - Could not calculate slope for South or North at Position {position}.");
                        averageSlopes[position] = double.NaN;
                    }
                }
                else
                {
                    Debug.WriteLine($"[WARNING] CalculateHall - Not enough data points for at least one state at Position {position}.");
                    averageSlopes[position] = double.NaN;
                }
            }
            else
            {
                Debug.WriteLine($"[WARNING] CalculateHall - Missing data for at least one state at Position {position}.");
                // อาจจะตั้งค่า N/A หรือ 0 ใน hallVoltagesByPosition ด้วย
            }
        }

        GlobalSettings.Instance.HallVoltagesByPosition = hallVoltagesByPosition; // เก็บ Voltages ใน GlobalSettings

        // Trigger Event สำหรับ Hall Voltages
        OnHallVoltageCalculated(hallVoltagesByPosition);

        double totalAverageSlope = averageSlopes.Values.Where(s => !double.IsNaN(s)).Average();
        double hallCoefficient = double.NaN;
        double sheetConcentration = double.NaN;
        double bulkConcentration = double.NaN;
        double mobility = double.NaN;

        if (!double.IsNaN(totalAverageSlope) && ElementaryCharge > 0 && GlobalSettings.Instance.MagneticFieldsValueStd != 0)
        {
            // คำนวณ Hall Coefficient
            if (GlobalSettings.Instance.ThicknessValueStd > 0) // Bulk sample
            {
                hallCoefficient = (totalAverageSlope * GlobalSettings.Instance.ThicknessValueStd) / GlobalSettings.Instance.MagneticFieldsValueStd;
                bulkConcentration = 1 / (hallCoefficient * ElementaryCharge);
                GlobalSettings.Instance.BulkConcentration = bulkConcentration;
                Debug.WriteLine($"[DEBUG] CalculateHall() - Hall Coefficient (Bulk): {hallCoefficient}");
                Debug.WriteLine($"[DEBUG] CalculateHall() - Bulk Concentration (n_b): {GlobalSettings.Instance.BulkConcentration}");
            }
            else // Sheet sample (หรือความหนาไม่ทราบ)
            {
                hallCoefficient = totalAverageSlope / GlobalSettings.Instance.MagneticFieldsValueStd;
                sheetConcentration = 1 / (hallCoefficient * ElementaryCharge);
                GlobalSettings.Instance.SheetConcentration = sheetConcentration;
                Debug.WriteLine($"[DEBUG] CalculateHall() - Hall Coefficient (Sheet): {hallCoefficient}");
                Debug.WriteLine($"[DEBUG] CalculateHall() - Sheet Concentration (n_s): {GlobalSettings.Instance.SheetConcentration}");
            }

            GlobalSettings.Instance.HallCoefficient = hallCoefficient;

            // คำนวณ Mobility (สมมติว่าคุณมีค่า Resistivity/Sheet Resistance อยู่ใน GlobalSettings)
            if (GlobalSettings.Instance.SheetResistance > 0 && !double.IsNaN(sheetConcentration) && ElementaryCharge != 0)
            {
                mobility = 1 / (GlobalSettings.Instance.SheetResistance * sheetConcentration * ElementaryCharge);
                GlobalSettings.Instance.Mobility = mobility;
                Debug.WriteLine($"[DEBUG] CalculateHall() - Mobility (Sheet): {GlobalSettings.Instance.Mobility}");
            }
            else if (GlobalSettings.Instance.Resistivity > 0 && GlobalSettings.Instance.ThicknessValueStd > 0 && !double.IsNaN(bulkConcentration) && ElementaryCharge != 0)
            {
                mobility = 1 / (GlobalSettings.Instance.Resistivity * bulkConcentration * ElementaryCharge);
                GlobalSettings.Instance.Mobility = mobility;
                Debug.WriteLine($"[DEBUG] CalculateHall() - Mobility (Bulk): {GlobalSettings.Instance.Mobility}");
            }
            else
            {
                GlobalSettings.Instance.Mobility = double.NaN;
                Debug.WriteLine("[WARNING] CalculateHall() - Cannot calculate Mobility. Check Resistance/Resistivity, Concentration, or Thickness.");
            }

            // Trigger Event สำหรับ Hall Properties
            OnHallPropertiesCalculated((GlobalSettings.Instance.HallCoefficient, GlobalSettings.Instance.SheetConcentration, GlobalSettings.Instance.BulkConcentration, GlobalSettings.Instance.Mobility));
            Debug.WriteLine("[DEBUG] CalculateHall() - Hall Coefficient, Concentrations, and Mobility Calculated and Event Triggered");
        }
        else
        {
            Debug.WriteLine("[WARNING] CalculateHall() - Cannot calculate Hall Coefficient or Concentrations. Check slopes, charge, or magnetic field.");
            GlobalSettings.Instance.HallCoefficient = double.NaN;
            GlobalSettings.Instance.SheetConcentration = double.NaN;
            GlobalSettings.Instance.BulkConcentration = double.NaN;
            GlobalSettings.Instance.Mobility = double.NaN;
        }

        OnCalculationCompleted();
        Debug.WriteLine("[DEBUG] CalculateHall() - Calculation Completed");
    }
}