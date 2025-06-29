using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    public string StateKey { get; private set; }
    public int TunerPosition { get; private set; }
    public List<Tuple<double, double>> IndividualData { get; private set; }

    public HallVoltageDataUpdatedEventArgs(string stateKey, int tunerPosition, List<Tuple<double, double>> individualData)
    {
        StateKey = stateKey;
        TunerPosition = tunerPosition;
        IndividualData = individualData;
    }
}

public class DetailedHallPropertiesCalculatedEventArgs : EventArgs
{
    public Dictionary<double, HallCalculationResultPerCurrent> PerCurrentPointResults { get; private set; }
    public DetailedHallPropertiesCalculatedEventArgs(Dictionary<double, HallCalculationResultPerCurrent> results)
    {
        PerCurrentPointResults = results;
    }
}

public class HallCalculationResultPerCurrent
{
    public double Current { get; set; }
    public HallCalculationResultPerCurrent(double current)
    {
        Current = current;
        RawVhn_Minus_Vout_ByPosition = new Dictionary<int, double>(); // Initialize in constructor
        RawVhs_Minus_Vout_ByPosition = new Dictionary<int, double>(); // Initialize in constructor
    }

    public double Vh_SouthField { get; set; } = double.NaN;
    public double Vh_NorthField { get; set; } = double.NaN;
    public double R_Hall_SouthField { get; set; } = double.NaN;
    // This is Hall Resistance (Vh/I in Ohm)
    public double R_H_SouthField { get; set; } = double.NaN;
    // This is Hall Coefficient in cm^3/C
    public double SheetConcentration_SouthField { get; set; } = double.NaN;
    public double BulkConcentration_SouthField { get; set; } = double.NaN;
    public double Mobility_SouthField { get; set; } = double.NaN;
    public double R_Hall_NorthField { get; set; } = double.NaN; // This is Hall Resistance (Vh/I in Ohm)
    public double R_H_NorthField { get; set; } = double.NaN; // This is Hall Coefficient in cm^3/C
    public double SheetConcentration_NorthField { get; set; } = double.NaN;
    public double BulkConcentration_NorthField { get; set; } = double.NaN;
    public double Mobility_NorthField { get; set; } = double.NaN;

    // Properties to store the 'true' Hall values for each current point (after offset cancellation)
    public double Vh_Average { get; set; } = double.NaN; // This will store (Vh_SouthField - Vh_NorthField) / 2
    public double R_Hall_ByCurrent { get; set; } = double.NaN; // This will store (R_Hall_SouthField - R_Hall_NorthField) / 2
    public double R_H_ByCurrent { get; set; } = double.NaN; // This will store (R_H_SouthField - R_H_NorthField) / 2
    public double SheetConcentration_ByCurrent { get; set; } = double.NaN;
    public double BulkConcentration_ByCurrent { get; set; } = double.NaN;
    public double Mobility_ByCurrent { get; set; } = double.NaN;

    public Dictionary<int, double> RawVhn_Minus_Vout_ByPosition { get; set; }
    public Dictionary<int, double> RawVhs_Minus_Vout_ByPosition { get; set; }
}

public class CollectAndCalculateHallMeasured
{
    private static readonly Lazy<CollectAndCalculateHallMeasured> lazy =
        new Lazy<CollectAndCalculateHallMeasured>(() => new CollectAndCalculateHallMeasured());
    public static CollectAndCalculateHallMeasured Instance { get; } = lazy.Value;

    private CollectAndCalculateHallMeasured()
    {
        _measurements = new Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>>();

        // Initialise inner dictionaries for each HallMeasurementState
        // เพื่อให้แน่ใจว่า _measurements[state] จะไม่เป็น null เมื่อถูกเรียกใช้
        foreach (HallMeasurementState state in Enum.GetValues(typeof(HallMeasurementState)))
        {
            _measurements[state] = new Dictionary<int, List<Tuple<double, double>>>();

            // Initialise lists for each tuner position within each state
            // สมมติว่ามี 4 ตำแหน่ง Tuner ตาม NumberOfHallPositions ใน HallMeasurementResultsForm
            for (int i = 1; i <= 4; i++) // ตรงนี้อาจปรับตาม NumberOfHallPositions ถ้ามีการกำหนดค่ากลาง
            {
                _measurements[state][i] = new List<Tuple<double, double>>();
            }
        }

        AverageVhsByPosition = new Dictionary<int, double>();
        AverageVhnByPosition = new Dictionary<int, double>();
    }

    public enum SemiconductorType
    {
        Unknown,
        N,
        P
    }

    private Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> _measurements;
    public Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> Measurements
    {
        get { return _measurements; }
    }

    public IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> AllRawMeasurements => _measurements;
    private const int NumberOfHallPositions = 4;
    public Dictionary<int, double> AverageVhsByPosition { get; private set; }
    public Dictionary<int, double> AverageVhnByPosition { get; private set; }
    public Dictionary<double, HallCalculationResultPerCurrent> DetailedPerCurrentPointResults { get; private set; } = new Dictionary<double, HallCalculationResultPerCurrent>();
    // Events
    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<DetailedHallPropertiesCalculatedEventArgs> DetailedHallPropertiesUpdated;
    // Event invokers
    protected virtual void OnDataUpdated(HallVoltageDataUpdatedEventArgs e)
    {
        DataUpdated?.Invoke(this, e);
    }

    protected virtual void OnCalculationCompleted()
    {
        CalculationCompleted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnDetailedHallPropertiesCalculated(DetailedHallPropertiesCalculatedEventArgs e)
    {
        DetailedHallPropertiesUpdated?.Invoke(this, e);
    }

    public void StoreMeasurementData(HallMeasurementState state, int tunerPosition, List<Tuple<double, double>> measurements)
    {
        if (!_measurements.ContainsKey(state))
        {
            _measurements[state] = new Dictionary<int, List<Tuple<double, double>>>();
        }
        if (!_measurements[state].ContainsKey(tunerPosition))
        {
            _measurements[state][tunerPosition] = new List<Tuple<double, double>>();
        }

        Debug.WriteLine($"\n[DEBUG-RAW-STORE] Storing data for State: {state}, Tuner: {tunerPosition}");
        if (measurements != null && measurements.Any())
        {
            foreach (var m in measurements)
            {
                Debug.WriteLine($"  [DEBUG-RAW-STORE]   Current: {m.Item1:E5} A, Voltage: {m.Item2:E5} V");
            }
        }
        else
        {
            Debug.WriteLine("  [DEBUG-RAW-STORE]   No data points provided for storage.");
        }

        _measurements[state][tunerPosition].AddRange(measurements);
        Debug.WriteLine($"[DEBUG-RAW-STORE] StoreMeasurementData - State: {state}, Position: {tunerPosition}, Added {measurements.Count} points. Total points now: {_measurements[state][tunerPosition].Count}");
        OnDataUpdated(new HallVoltageDataUpdatedEventArgs(state.ToString(), tunerPosition, _measurements[state][tunerPosition]));
    }

    public Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> GetAllRawMeasurements()
    {
        return _measurements;
    }

    public void DebugPrintAllRawMeasurements()
    {
        Debug.WriteLine("\n=== DEBUGGING ALL RAW HALL MEASUREMENTS (AFTER ALL STORAGE) ===");
        if (!_measurements.Any())
        {
            Debug.WriteLine("No raw measurement data has been stored yet.");
            Debug.WriteLine("===========================================\n");
            return;
        }

        foreach (var stateEntry in _measurements)
        {
            HallMeasurementState state = stateEntry.Key;
            Dictionary<int, List<Tuple<double, double>>> tunerData = stateEntry.Value;
            Debug.WriteLine($"--- State: {state} ---");
            for (int tunerPos = 1; tunerPos <= NumberOfHallPositions; tunerPos++)
            {
                Debug.WriteLine($"  Tuner Position: {tunerPos}");
                if (tunerData.ContainsKey(tunerPos))
                {
                    List<Tuple<double, double>> dataPoints = tunerData[tunerPos];
                    Debug.WriteLine($"    Total points for Tuner {tunerPos}: {dataPoints.Count}");
                    if (!dataPoints.Any())
                    {
                        Debug.WriteLine("      No data points for this Tuner position in this state.");
                    }
                    else
                    {
                        foreach (var point in dataPoints)
                        {
                            Debug.WriteLine($"      Current: {point.Item1:E5} A, Voltage: {point.Item2:E5} V");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"    No data collected for Tuner {tunerPos} in this State (Key not found).");
                }
            }
        }
        Debug.WriteLine("=== END OF RAW HALL MEASUREMENTS DEBUGGING ===\n");
    }

    public void CalculateAllHallProperties(double thickness_cm_unused, double magneticField_Vs_per_cm2_unused) // Renamed parameters as they are now taken from GlobalSettings
    {
        Debug.WriteLine("[DEBUG] CalculateAllHallProperties - Starting overall calculation sequence.");
        // Use values directly from GlobalSettings as they are pre-converted
        double magneticField_Vs_per_cm2 = GlobalSettings.Instance.MagneticFieldsValueStd;
        // In V s/cm^2
        double thickness_cm = GlobalSettings.Instance.ThicknessValueStd;
        // In cm
        double resistivity_ohm_cm = GlobalSettings.Instance.Resistivity;
        // In Ohm.cm
        double elementaryCharge_C = GlobalSettings.Instance.ElementaryCharge;
        // In Coulomb

        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Thickness (t): {thickness_cm:E9} cm");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Magnetic Field (B): {magneticField_Vs_per_cm2:E9} V s/cm^2");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Resistivity (rho): {resistivity_ohm_cm:E9} Ohm.cm");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Elementary Charge (e): {elementaryCharge_C:E9} C");


        CalculateHallVoltages();
        // The error you saw here 'r.Vh_SouthField' was due to an old HallCalculationResultPerCurrent definition.
        // It should be fixed if you replace the entire file.
        if (DetailedPerCurrentPointResults.Any(r => !double.IsNaN(r.Value.Vh_SouthField) || !double.IsNaN(r.Value.Vh_NorthField)))
        {
            // Pass the parameters directly to avoid re-fetching in each method if they are consistent
            CalculateAverageHallVoltagesByPosition();
            CalculateHallPropertiesByFieldDirection();
            CalculateOverallHallProperties();
            OnCalculationCompleted();
            OnDetailedHallPropertiesCalculated(new DetailedHallPropertiesCalculatedEventArgs(DetailedPerCurrentPointResults));
        }
        else
        {
            Debug.WriteLine("[WARNING] CalculateAllHallProperties - No valid Hall Voltages calculated. Skipping further property calculations.");
            GlobalSettings.Instance.TotalHallVoltage_Average = double.NaN;
            GlobalSettings.Instance.HallResistance = double.NaN;
            GlobalSettings.Instance.HallCoefficient = double.NaN;
            GlobalSettings.Instance.SheetConcentration = double.NaN;
            GlobalSettings.Instance.BulkConcentration = double.NaN;
            GlobalSettings.Instance.Mobility = double.NaN;
            GlobalSettings.Instance.SemiconductorType = SemiconductorType.Unknown;
            OnCalculationCompleted();
            OnDetailedHallPropertiesCalculated(new DetailedHallPropertiesCalculatedEventArgs(DetailedPerCurrentPointResults));
        }
    }

    public void CalculateHallVoltages()
    {
        Debug.WriteLine("[DEBUG] CollectAndCalculateHallMeasured: CalculateHallVoltages - Start");

        if (DetailedPerCurrentPointResults == null)
        {
            DetailedPerCurrentPointResults = new Dictionary<double, HallCalculationResultPerCurrent>();
        }
        else
        {
            DetailedPerCurrentPointResults.Clear();
        }

        if (!_measurements.ContainsKey(HallMeasurementState.NoMagneticField) ||
            !_measurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField) ||
            !_measurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
        {
            Debug.WriteLine("[ERROR] CollectAndCalculateHallMeasured: Missing Hall measurement states. Cannot calculate Hall voltages.");
            return;
        }

        // Assuming all states have the same current points
        List<double> currentPoints = _measurements[HallMeasurementState.NoMagneticField]
            .SelectMany(kv => kv.Value.Select(t => t.Item1))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        foreach (double current_A in currentPoints)
        {
            Debug.WriteLine($"[DEBUG] Processing Current: {current_A:E9} A");
            HallCalculationResultPerCurrent currentResult = new HallCalculationResultPerCurrent(current_A);

            double[] vOuts = new double[NumberOfHallPositions];
            double[] vhsRaw = new double[NumberOfHallPositions];
            double[] vhnRaw = new double[NumberOfHallPositions];

            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                // Retrieve V_Out for current_A at position i+1
                var dataList_Out = _measurements[HallMeasurementState.NoMagneticField][i + 1];
                var matchingVoltages_Out = dataList_Out
                    .Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance)
                    .Select(d => d.Item2)
                    .ToList();

                if (matchingVoltages_Out.Any())
                {
                    vOuts[i] = matchingVoltages_Out.Average(); // ใช้ค่าเฉลี่ยหากมีหลายจุดสำหรับกระแสเดียวกัน
                }
                else
                {
                    vOuts[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No V_Out data found for current {current_A:E9} A at Tuner {i + 1}");
                }

                // Retrieve Vhs (South Field) for current_A at position i+1
                var dataList_South = _measurements[HallMeasurementState.OutwardOrSouthMagneticField][i + 1];
                var matchingVoltages_South = dataList_South
                    .Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance)
                    .Select(d => d.Item2)
                    .ToList();

                if (matchingVoltages_South.Any())
                {
                    vhsRaw[i] = matchingVoltages_South.Average(); // ใช้ค่าเฉลี่ยหากมีหลายจุดสำหรับกระแสเดียวกัน
                }
                else
                {
                    vhsRaw[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No Vhs data found for current {current_A:E9} A at Tuner {i + 1}");
                }

                // Retrieve Vhn (North Field) for current_A at position i+1
                var dataList_North = _measurements[HallMeasurementState.InwardOrNorthMagneticField][i + 1];
                var matchingVoltages_North = dataList_North
                    .Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance)
                    .Select(d => d.Item2)
                    .ToList();

                if (matchingVoltages_North.Any())
                {
                    vhnRaw[i] = matchingVoltages_North.Average(); // ใช้ค่าเฉลี่ยหากมีหลายจุดสำหรับกระแสเดียวกัน
                }
                else
                {
                    vhnRaw[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No Vhn data found for current {current_A:E9} A at Tuner {i + 1}");
                }

                // Populate the RawVhn_Minus_Vout_ByPosition and RawVhs_Minus_Vout_ByPosition dictionaries
                // ตรวจสอบให้แน่ใจว่าค่าไม่เป็น NaN ก่อนทำการลบ เพื่อป้องกัน NaN propagation
                if (!double.IsNaN(vhsRaw[i]) && !double.IsNaN(vOuts[i]))
                {
                    currentResult.RawVhs_Minus_Vout_ByPosition[i + 1] = vhsRaw[i] - vOuts[i];
                    Debug.WriteLine($"[DEBUG] Tuner {i + 1}: RawVhs_Minus_Vout = {currentResult.RawVhs_Minus_Vout_ByPosition[i + 1]:E9} V");
                }
                else
                {
                    currentResult.RawVhs_Minus_Vout_ByPosition[i + 1] = double.NaN;
                    Debug.WriteLine($"[WARNING] Tuner {i + 1}: RawVhs_Minus_Vout set to NaN due to missing raw data.");
                }

                if (!double.IsNaN(vhnRaw[i]) && !double.IsNaN(vOuts[i]))
                {
                    currentResult.RawVhn_Minus_Vout_ByPosition[i + 1] = vhnRaw[i] - vOuts[i];
                    Debug.WriteLine($"[DEBUG] Tuner {i + 1}: RawVhn_Minus_Vout = {currentResult.RawVhn_Minus_Vout_ByPosition[i + 1]:E9} V");
                }
                else
                {
                    currentResult.RawVhn_Minus_Vout_ByPosition[i + 1] = double.NaN;
                    Debug.WriteLine($"[WARNING] Tuner {i + 1}: RawVhn_Minus_Vout set to NaN due to missing raw data.");
                }
            }

            // Calculate Vh_SouthField and Vh_NorthField using the offset-cancelled values
            // ตรวจสอบให้แน่ใจว่าทุกตำแหน่งมีข้อมูลที่ถูกต้อง (ไม่ใช่ NaN) ก่อนคำนวณ
            if (!double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[1]) &&
                !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[2]) &&
                !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[3]) &&
                !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[4]))
            {
                currentResult.Vh_SouthField = (currentResult.RawVhs_Minus_Vout_ByPosition[1] - currentResult.RawVhs_Minus_Vout_ByPosition[2] +
                                               currentResult.RawVhs_Minus_Vout_ByPosition[3] - currentResult.RawVhs_Minus_Vout_ByPosition[4]) / 4.0;
            }
            else
            {
                currentResult.Vh_SouthField = double.NaN;
                Debug.WriteLine($"[WARNING] Vh_SouthField for current {current_A:E9} A set to NaN due to incomplete offset-cancelled Vhs data.");
            }

            if (!double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[1]) &&
                !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[2]) &&
                !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[3]) &&
                !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[4]))
            {
                currentResult.Vh_NorthField = (currentResult.RawVhn_Minus_Vout_ByPosition[1] - currentResult.RawVhn_Minus_Vout_ByPosition[2] +
                                               currentResult.RawVhn_Minus_Vout_ByPosition[3] - currentResult.RawVhn_Minus_Vout_ByPosition[4]) / 4.0;
            }
            else
            {
                currentResult.Vh_NorthField = double.NaN;
                Debug.WriteLine($"[WARNING] Vh_NorthField for current {current_A:E9} A set to NaN due to incomplete offset-cancelled Vhn data.");
            }


            // Calculate Vh_Average (Total Hall Voltage)
            if (!double.IsNaN(currentResult.Vh_SouthField) && !double.IsNaN(currentResult.Vh_NorthField))
            {
                currentResult.Vh_Average = (currentResult.Vh_SouthField - currentResult.Vh_NorthField) / 2.0;
            }
            else
            {
                currentResult.Vh_Average = double.NaN;
                Debug.WriteLine($"[WARNING] Vh_Average for current {current_A:E9} A set to NaN due to incomplete Vh_SouthField or Vh_NorthField.");
            }


            DetailedPerCurrentPointResults[current_A] = currentResult;
        }

        Debug.WriteLine("[DEBUG] CollectAndCalculateHallMeasured: CalculateHallVoltages - End");
    }

    private void CalculateAverageHallVoltagesByPosition()
    {
        Debug.WriteLine("[DEBUG] CalculateAverageHallVoltagesByPosition - Start: Calculating average Vhs and Vhn by each position.");

        // Clear previous averages to ensure fresh calculation
        AverageVhsByPosition.Clear();
        AverageVhnByPosition.Clear();

        // Initialize dictionaries to sum values and count points for each position
        Dictionary<int, double> sumVhsByPosition = new Dictionary<int, double>();
        Dictionary<int, int> countVhsByPosition = new Dictionary<int, int>();
        Dictionary<int, double> sumVhnByPosition = new Dictionary<int, double>();
        Dictionary<int, int> countVhnByPosition = new Dictionary<int, int>();

        // Initialize sums and counts for all 4 positions
        for (int i = 1; i <= NumberOfHallPositions; i++)
        {
            sumVhsByPosition[i] = 0.0;
            countVhsByPosition[i] = 0;
            sumVhnByPosition[i] = 0.0;
            countVhnByPosition[i] = 0;
        }

        // Iterate through all detailed results (HallCalculationResultPerCurrent)
        // Each entry represents calculation results for a specific current point.
        foreach (var currentResultEntry in DetailedPerCurrentPointResults)
        {
            var currentResult = currentResultEntry.Value;

            // Process RawVhs_Minus_Vout_ByPosition for current HallMeasurementState.OutwardOrSouthMagneticField
            foreach (var positionEntry in currentResult.RawVhs_Minus_Vout_ByPosition)
            {
                int position = positionEntry.Key;
                double value = positionEntry.Value;

                // Only include valid numbers in the average
                if (!double.IsNaN(value) && !double.IsInfinity(value))
                {
                    sumVhsByPosition[position] += value;
                    countVhsByPosition[position]++;
                }
                else
                {
                    Debug.WriteLine($"[WARNING] Skipping NaN/Infinity RawVhs_Minus_Vout for current {currentResult.Current:E9} A, position {position}.");
                }
            }

            // Process RawVhn_Minus_Vout_ByPosition for current HallMeasurementState.InwardOrNorthMagneticField
            foreach (var positionEntry in currentResult.RawVhn_Minus_Vout_ByPosition)
            {
                int position = positionEntry.Key;
                double value = positionEntry.Value;

                // Only include valid numbers in the average
                if (!double.IsNaN(value) && !double.IsInfinity(value))
                {
                    sumVhnByPosition[position] += value;
                    countVhnByPosition[position]++;
                }
                else
                {
                    Debug.WriteLine($"[WARNING] Skipping NaN/Infinity RawVhn_Minus_Vout for current {currentResult.Current:E9} A, position {position}.");
                }
            }
        }

        // Calculate averages and populate AverageVhsByPosition and AverageVhnByPosition
        for (int i = 1; i <= NumberOfHallPositions; i++)
        {
            if (countVhsByPosition[i] > 0)
            {
                AverageVhsByPosition[i] = sumVhsByPosition[i] / countVhsByPosition[i];
                Debug.WriteLine($"[DEBUG] AverageVhsByPosition[{i}]: {AverageVhsByPosition[i]:E9} V (from {countVhsByPosition[i]} points)");
            }
            else
            {
                AverageVhsByPosition[i] = double.NaN;
                Debug.WriteLine($"[WARNING] No valid data points to calculate AverageVhsByPosition[{i}]. Set to NaN.");
            }

            if (countVhnByPosition[i] > 0)
            {
                AverageVhnByPosition[i] = sumVhnByPosition[i] / countVhnByPosition[i];
                Debug.WriteLine($"[DEBUG] AverageVhnByPosition[{i}]: {AverageVhnByPosition[i]:E9} V (from {countVhnByPosition[i]} points)");
            }
            else
            {
                AverageVhnByPosition[i] = double.NaN;
                Debug.WriteLine($"[WARNING] No valid data points to calculate AverageVhnByPosition[{i}]. Set to NaN.");
            }
        }

        Debug.WriteLine("[DEBUG] CalculateAverageHallVoltagesByPosition - End.");
    }

    public void CalculateHallPropertiesByFieldDirection()
    {
        Debug.WriteLine("[DEBUG] CalculateHallPropertiesByFieldDirection - Start.");
        // Changed variable names to reflect actual units as per user's clarification
        double magneticField_Vs_per_cm2 = GlobalSettings.Instance.MagneticFieldsValueStd;
        // Magnetic Field in Vs/cm^2 (equivalent to Tesla)
        double thickness_cm = GlobalSettings.Instance.ThicknessValueStd;
        // Thickness in centimeters
        double elementaryCharge = GlobalSettings.Instance.ElementaryCharge;
        // Elementary Charge in Coulombs
        // Assumption: Resistivity is in Ohm.cm for consistent unit calculation for Mobility
        double resistivity_ohm_cm = GlobalSettings.Instance.Resistivity;
        // Resistivity in Ohm.cm

        Debug.WriteLine($"[DEBUG-CALC-PROPS] Parameters: thickness_cm: {thickness_cm:E9} cm, magneticField_Vs_per_cm2: {magneticField_Vs_per_cm2:E9} Vs/cm^2, ElementaryCharge: {elementaryCharge:E9} C, Resistivity: {resistivity_ohm_cm:E9} Ohm.cm (Assumed)");
        if (double.IsNaN(magneticField_Vs_per_cm2) || magneticField_Vs_per_cm2 == 0 || double.IsNaN(thickness_cm) || thickness_cm == 0 || double.IsNaN(resistivity_ohm_cm) || resistivity_ohm_cm == 0)
        {
            Debug.WriteLine("[WARNING] CalculateHallPropertiesByFieldDirection - Magnetic Field, Thickness, or Resistivity is not set or zero. Cannot calculate detailed Hall properties.");
            return;
        }

        foreach (var entry in DetailedPerCurrentPointResults)
        {
            double current = entry.Key;
            HallCalculationResultPerCurrent result = entry.Value;

            Debug.WriteLine($"\n[DEBUG-CALC-PROPS] Processing Current: {current:E9} A");

            // Calculations for South Field (Magnetic Field Outward/Positive)
            if (!double.IsNaN(result.Vh_SouthField) && current != 0)
            {
                // 1. Hall Resistance (R_Hall) - Vh / I (Unit: Ohm)
                // Equation: R_Hall_SouthField = Vh_SouthField / Current
                result.R_Hall_SouthField = result.Vh_SouthField / current;
                // 2. Hall Coefficient (R_H) - Vh * t / (I * B) (Unit: cm^3/C)
                //    Using thickness_cm and magneticField_Vs_per_cm2 directly yields cm^3/C
                // Equation: R_H_SouthField = (Vh_SouthField * thickness_cm) / (current * magneticField_Vs_per_cm2)
                result.R_H_SouthField = (result.Vh_SouthField * thickness_cm) / (current * magneticField_Vs_per_cm2);
                // 3. Bulk Concentration (n_Bulk) - 1 / (q * |R_H|) (Unit: cm^-3)
                // Equation: n_Bulk_SouthField = 1 / (elementaryCharge * |R_H_SouthField|)
                result.BulkConcentration_SouthField = 1.0 / (elementaryCharge * Math.Abs(result.R_H_SouthField));
                // 4. Sheet Concentration (n_Sheet) - n_Bulk * t (Unit: cm^-2)
                // Equation: n_Sheet_SouthField = BulkConcentration_SouthField * thickness_cm
                result.SheetConcentration_SouthField = result.BulkConcentration_SouthField * thickness_cm;
                // 5. Mobility (Mu) - |R_H| / Resistivity (Unit: cm^2/(V*s))
                //    Requires R_H in cm^3/C and Resistivity in Ohm.cm
                // Equation: Mobility_SouthField = |R_H_SouthField| / resistivity_ohm_cm
                result.Mobility_SouthField = Math.Abs(result.R_H_SouthField) / resistivity_ohm_cm;
            }
            else
            {
                // If Vh_SouthField is NaN or current is 0, set all SouthField properties to NaN
                result.R_Hall_SouthField = double.NaN;
                result.R_H_SouthField = double.NaN;
                result.SheetConcentration_SouthField = double.NaN;
                result.BulkConcentration_SouthField = double.NaN;
                result.Mobility_SouthField = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] Vh_SouthField is NaN or Current is 0 for South Field. Properties set to NaN.");
            }

            // Calculations for North Field (Magnetic Field Inward/Negative)
            if (!double.IsNaN(result.Vh_NorthField) && current != 0)
            {
                // 1. Hall Resistance (R_Hall) - Vh / I (Unit: Ohm)
                // Equation: R_Hall_NorthField = Vh_NorthField / Current
                // Note: The magnetic field for North is in the opposite direction.
                // If B_North = -B_South, then R_H_North = (Vh_NorthField * t) / (I * (-B_South)) = -R_H_South (if Vh also flips sign)
                // However, the formula calculates based on the absolute magnetic field magnitude,
                // and the Vh_NorthField itself is expected to have the opposite sign of Vh_SouthField.
                // So, R_Hall_NorthField here will have the sign of Vh_NorthField.
                result.R_Hall_NorthField = result.Vh_NorthField / current;
                // 2. Hall Coefficient (R_H) - Vh * t / (I * B) (Unit: cm^3/C)
                // Equation: R_H_NorthField = (Vh_NorthField * thickness_cm) / (current * magneticField_Vs_per_cm2)
                // This 'magneticField_Vs_cm2' is the magnitude.
                // The expected R_H for North field should be the negative of the R_H for South field if Vh changes sign perfectly.
                // For a consistent R_H (true), we might need to negate R_H_NorthField later in the averaging step.
                result.R_H_NorthField = (result.Vh_NorthField * thickness_cm) / (current * magneticField_Vs_per_cm2);
                // 3. Bulk Concentration (n_Bulk) - 1 / (q * |R_H|) (Unit: cm^-3)
                // Equation: n_Bulk_NorthField = 1 / (elementaryCharge * |R_H_NorthField|)
                result.BulkConcentration_NorthField = 1.0 / (elementaryCharge * Math.Abs(result.R_H_NorthField));
                // 4. Sheet Concentration (n_Sheet) - n_Bulk * t (Unit: cm^-2)
                // Equation: n_Sheet_NorthField = BulkConcentration_NorthField * thickness_cm
                result.SheetConcentration_NorthField = result.BulkConcentration_NorthField * thickness_cm;
                // 5. Mobility (Mu) - |R_H| / Resistivity (Unit: cm^2/(V*s))
                // Equation: Mobility_NorthField = |R_H_NorthField| / resistivity_ohm_cm
                result.Mobility_NorthField = Math.Abs(result.R_H_NorthField) / resistivity_ohm_cm;
            }
            else
            {
                // If Vh_NorthField is NaN or current is 0, set all NorthField properties to NaN
                result.R_Hall_NorthField = double.NaN;
                result.R_H_NorthField = double.NaN;
                result.SheetConcentration_NorthField = double.NaN;
                result.BulkConcentration_NorthField = double.NaN;
                result.Mobility_NorthField = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] Vh_NorthField is NaN or Current is 0 for North Field. Properties set to NaN.");
            }

            // Debugging output for each current point
            Debug.WriteLine($"  [DEBUG-CALC-PROPS] Current {current:E9} A Results:");
            Debug.WriteLine($"    - Vh_SouthField: {result.Vh_SouthField:E9} V, Vh_NorthField: {result.Vh_NorthField:E9} V");
            Debug.WriteLine($"    - R_Hall_SouthField (Hall Res.): {result.R_Hall_SouthField:E9} Ohm, R_Hall_NorthField (Hall Res.): {result.R_Hall_NorthField:E9} Ohm");
            Debug.WriteLine($"    - R_H_SouthField (Hall Coeff.): {result.R_H_SouthField:E9} cm^3/C, R_H_NorthField (Hall Coeff.): {result.R_H_NorthField:E9} cm^3/C");
            Debug.WriteLine($"    - BulkConcentration_SouthField: {result.BulkConcentration_SouthField:E9} cm^-3, BulkConcentration_NorthField: {result.BulkConcentration_NorthField:E9} cm^-3");
            Debug.WriteLine($"    - SheetConcentration_SouthField: {result.SheetConcentration_SouthField:E9} cm^-2, SheetConcentration_NorthField: {result.SheetConcentration_NorthField:E9} cm^-2");
            Debug.WriteLine($"    - Mobility_SouthField: {result.Mobility_SouthField:E9} cm^2/(V*s), Mobility_NorthField: {result.Mobility_NorthField:E9} cm^2/(V*s)");

            // --- CRITICAL FIX: Calculate the 'true' Hall properties for THIS current point using (South - North) / 2 ---
            // This cancels offset errors and ensures correct sign before averaging across all currents.
            if (!double.IsNaN(result.Vh_SouthField) && !double.IsNaN(result.Vh_NorthField))
            {
                result.Vh_Average = (result.Vh_SouthField - result.Vh_NorthField) / 2.0;
                Debug.WriteLine($"  [DEBUG-CALC-PROPS] Vh_Average (True Vh for current {current:E9}): {result.Vh_Average:E9} V");
            }
            else
            {
                result.Vh_Average = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] Vh_Average is NaN for current {current:E9} due to missing Vh_SouthField or Vh_NorthField.");
            }

            if (!double.IsNaN(result.R_Hall_SouthField) && !double.IsNaN(result.R_Hall_NorthField))
            {
                result.R_Hall_ByCurrent = (result.R_Hall_SouthField - result.R_Hall_NorthField) / 2.0;
                Debug.WriteLine($"  [DEBUG-CALC-PROPS] R_Hall_ByCurrent (True R_Hall for current {current:E9}): {result.R_Hall_ByCurrent:E9} Ohm");
            }
            else
            {
                result.R_Hall_ByCurrent = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] R_Hall_ByCurrent is NaN for current {current:E9} due to missing R_Hall_SouthField or R_Hall_NorthField.");
            }

            if (!double.IsNaN(result.R_H_SouthField) && !double.IsNaN(result.R_H_NorthField))
            {
                result.R_H_ByCurrent = (result.R_H_SouthField - result.R_H_NorthField) / 2.0;
                Debug.WriteLine($"  [DEBUG-CALC-PROPS] R_H_ByCurrent (True R_H for current {current:E9}): {result.R_H_ByCurrent:E9} cm^3/C");
            }
            else
            {
                result.R_H_ByCurrent = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] R_H_ByCurrent is NaN for current {current:E9} due to missing R_H_SouthField or R_H_NorthField.");
            }

            // Now calculate Bulk, Sheet Concentration, and Mobility using the correctly signed R_H_ByCurrent
            if (!double.IsNaN(result.R_H_ByCurrent) && elementaryCharge != 0)
            {
                result.BulkConcentration_ByCurrent = 1.0 / (elementaryCharge * Math.Abs(result.R_H_ByCurrent));
                result.SheetConcentration_ByCurrent = result.BulkConcentration_ByCurrent * thickness_cm;
            }
            else
            {
                result.BulkConcentration_ByCurrent = double.NaN;
                result.SheetConcentration_ByCurrent = double.NaN;
            }

            if (!double.IsNaN(result.R_H_ByCurrent) && resistivity_ohm_cm != 0)
            {
                result.Mobility_ByCurrent = Math.Abs(result.R_H_ByCurrent) / resistivity_ohm_cm;
            }
            else
            {
                result.Mobility_ByCurrent = double.NaN;
            }

            // Add debug for ByCurrent values
            Debug.WriteLine($"  [DEBUG-CALC-PROPS] Current {current:E9} Final ByCurrent Results:");
            Debug.WriteLine($"    - Vh_Average (ByCurrent, True Vh): {result.Vh_Average:E9} V");
            Debug.WriteLine($"    - R_Hall_ByCurrent (Hall Res., True): {result.R_Hall_ByCurrent:E9} Ohm");
            Debug.WriteLine($"    - R_H_ByCurrent (Hall Coeff., True): {result.R_H_ByCurrent:E9} cm^3/C");
            Debug.WriteLine($"    - BulkConcentration_ByCurrent: {result.BulkConcentration_ByCurrent:E9} cm^-3");
            Debug.WriteLine($"    - SheetConcentration_ByCurrent: {result.SheetConcentration_ByCurrent:E9} cm^-2");
            Debug.WriteLine($"    - Mobility_ByCurrent: {result.Mobility_ByCurrent:E9} cm^2/(V*s)");

        }

        Debug.WriteLine("[DEBUG] CalculateHallPropertiesByFieldDirection - End.");
    }

    public void CalculateOverallHallProperties()
    {
        Debug.WriteLine("[DEBUG] CalculateOverallHallProperties - Start.");

        // --- Step 1: Directly average the 'true' ByCurrent properties across all currents ---
        // These properties (Vh_Average, R_Hall_ByCurrent, R_H_ByCurrent etc.) now correctly incorporate the (South - North) / 2 offset cancellation.

        GlobalSettings.Instance.TotalHallVoltage_Average = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.Vh_Average))
            .Select(r => r.Vh_Average)
            .DefaultIfEmpty(double.NaN)
            .Average();

        GlobalSettings.Instance.HallResistance = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.R_Hall_ByCurrent))
            .Select(r => r.R_Hall_ByCurrent)
            .DefaultIfEmpty(double.NaN)
            .Average();

        GlobalSettings.Instance.HallCoefficient = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.R_H_ByCurrent))
            .Select(r => r.R_H_ByCurrent)
            .DefaultIfEmpty(double.NaN)
            .Average();

        GlobalSettings.Instance.BulkConcentration = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.BulkConcentration_ByCurrent))
            .Select(r => r.BulkConcentration_ByCurrent)
            .DefaultIfEmpty(double.NaN)
            .Average();

        GlobalSettings.Instance.SheetConcentration = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.SheetConcentration_ByCurrent))
            .Select(r => r.SheetConcentration_ByCurrent)
            .DefaultIfEmpty(double.NaN)
            .Average();

        GlobalSettings.Instance.Mobility = DetailedPerCurrentPointResults.Values
            .Where(r => !double.IsNaN(r.Mobility_ByCurrent))
            .Select(r => r.Mobility_ByCurrent)
            .DefaultIfEmpty(double.NaN)
            .Average();


        // Determine semiconductor type based on the final overall Hall Coefficient
        GlobalSettings.Instance.SemiconductorType = DetermineSemiconductorType(GlobalSettings.Instance.HallCoefficient);

        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Hall Voltage: {GlobalSettings.Instance.TotalHallVoltage_Average:E9} V");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Hall Resistance: {GlobalSettings.Instance.HallResistance:E9} Ohm");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Hall Coefficient: {GlobalSettings.Instance.HallCoefficient:E9} cm^3/C");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Bulk Concentration: {GlobalSettings.Instance.BulkConcentration:E9} cm^-3");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Sheet Concentration: {GlobalSettings.Instance.SheetConcentration:E9} cm^-2");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Mobility: {GlobalSettings.Instance.Mobility:E9} cm^2/(V*s)");
        Debug.WriteLine($"[DEBUG-OVERALL-CALC] Final Overall Semiconductor Type: {GlobalSettings.Instance.SemiconductorType}");

        Debug.WriteLine("[DEBUG] CalculateOverallHallProperties - End.");
    }

    private SemiconductorType DetermineSemiconductorType(double rHallCoefficient)
    {
        // บรรทัดนี้จะแสดงค่า Hall Coefficient จริงที่ถูกส่งเข้ามา
        Debug.WriteLine($"[DEBUG] DetermineSemiconductorType: rHallCoefficient received: {rHallCoefficient:E20}");
        // บรรทัดนี้จะแสดงค่า Threshold ปัจจุบัน
        Debug.WriteLine($"[DEBUG] DetermineSemiconductorType: Current MinHallCoefficientThresholdForTypeDetermination: {GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination:E20}");

        if (double.IsNaN(rHallCoefficient) || rHallCoefficient == 0 || Math.Abs(rHallCoefficient) < GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination)
        {
            Debug.WriteLine($"[DEBUG] Semiconductor Type: Unknown (Condition met: IsNaN={double.IsNaN(rHallCoefficient)}, IsZero={rHallCoefficient == 0}, BelowThreshold={Math.Abs(rHallCoefficient) < GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination})");
            return SemiconductorType.Unknown;
        }
        else if (rHallCoefficient < 0)
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: N-type (Hall Coefficient < 0)");
            return SemiconductorType.N;
        }
        else // rHallCoefficient > 0
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: P-type (Hall Coefficient > 0)");
            return SemiconductorType.P;
        }
    }

    public void ClearAllHallData()
    {
        foreach (HallMeasurementState state in Enum.GetValues(typeof(HallMeasurementState)))
        {
            foreach (var tunerData in _measurements[state].Values)
            {
                tunerData.Clear();
            }
        }
        DetailedPerCurrentPointResults.Clear();

        GlobalSettings.Instance.TotalHallVoltage_Average = double.NaN;
        GlobalSettings.Instance.HallResistance = double.NaN;
        GlobalSettings.Instance.HallCoefficient = double.NaN;
        GlobalSettings.Instance.SheetConcentration = double.NaN;
        GlobalSettings.Instance.BulkConcentration = double.NaN;
        GlobalSettings.Instance.Mobility = double.NaN;
        GlobalSettings.Instance.SemiconductorType = SemiconductorType.Unknown;

        AverageVhsByPosition.Clear();
        AverageVhnByPosition.Clear();
        GlobalSettings.Instance.HallMeasurementDataReady = false;
        Debug.WriteLine("[DEBUG] CollectAndCalculateHallMeasured: All data cleared and HallMeasurementDataReady reset to false.");
    }
}