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
        foreach (HallMeasurementState state in Enum.GetValues(typeof(HallMeasurementState)))
        {
            _measurements[state] = new Dictionary<int, List<Tuple<double, double>>>();
            for (int i = 1; i <= NumberOfHallPositions; i++)
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
        Debug.WriteLine("[DEBUG] CalculateHallVoltages - Start: Calculating true Hall Voltages (V_HS, V_HN) for each current.");
        var allCurrents = _measurements.Values
                                    .SelectMany(tunerData => tunerData.Values)
                                    .SelectMany(list => list)
                                    .Select(tuple => tuple.Item1)
                                    .Distinct()
                                    .OrderBy(c => c)
                                    .ToList();
        if (!allCurrents.Any())
        {
            Debug.WriteLine("[WARNING] CalculateHallVoltages - No current values found in any measurement data. Cannot calculate Hall Voltages.");
            DetailedPerCurrentPointResults.Clear();
            return;
        }

        Debug.WriteLine("[DEBUG] allCurrents identified:");
        foreach (var c in allCurrents)
        {
            Debug.WriteLine($"  - Current (from allCurrents): {c:E9} A");
        }

        Debug.WriteLine($"[DEBUG] GlobalSettings.Instance.CurrentTolerance: {GlobalSettings.Instance.CurrentTolerance:E9}");
        DetailedPerCurrentPointResults.Clear();
        foreach (double current_A in allCurrents)
        {
            Debug.WriteLine($"\n[DEBUG] Calculating True Hall Voltages for Target Current: {current_A:E9} A");
            HallCalculationResultPerCurrent currentResult = new HallCalculationResultPerCurrent(current_A);
            double[] vOuts = new double[4];
            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                int tunerPos = i + 1;
                bool stateExists = _measurements.ContainsKey(HallMeasurementState.NoMagneticField);
                bool tunerExists = stateExists && _measurements[HallMeasurementState.NoMagneticField].ContainsKey(tunerPos);
                int dataCount = tunerExists ? _measurements[HallMeasurementState.NoMagneticField][tunerPos].Count : 0;
                Debug.WriteLine($"  [DEBUG] Checking V_Out for Tuner {tunerPos}. StateExists: {stateExists}, TunerExists: {tunerExists}, DataCount: {dataCount}");
                if (!tunerExists || dataCount == 0)
                {
                    vOuts[i] = double.NaN;
                    Debug.WriteLine($"  [WARNING] V_Out[{tunerPos}] for {current_A:E9} A: NaN (State, Tuner key missing, or no data for NoMagneticField)");
                }
                else
                {
                    var dataList = _measurements[HallMeasurementState.NoMagneticField][tunerPos];
                    Debug.WriteLine($"    [DEBUG-V_OUT_DATALIST] Data points for NoMagneticField, Tuner {tunerPos} (Count: {dataList.Count}):");
                    foreach (var dp in dataList)
                    {
                        double diff = Math.Abs(dp.Item1 - current_A);
                        Debug.WriteLine($"      - Stored Current: {dp.Item1:E9} A, Voltage: {dp.Item2:E9} V, Diff to Target {current_A:E9} A: {diff:E9} (Tolerance: {GlobalSettings.Instance.CurrentTolerance:E9})");
                    }

                    var data = dataList
                        .Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance)
                        .OrderBy(d => Math.Abs(d.Item1 - current_A))
                        .FirstOrDefault();
                    vOuts[i] = data != null ? data.Item2 : double.NaN;
                    string foundCurrentStr = data != null ? $"{data.Item1:E9} A" : "N/A";
                    string diffStr = data != null ? $"{Math.Abs(data.Item1 - current_A):E9}" : "N/A";
                    Debug.WriteLine($"  [DEBUG] V_Out[{tunerPos}] for Target Current {current_A:E9} A: {vOuts[i]:E9} V " +
                                        (data == null ? $"(Missing data point. Found current: {foundCurrentStr}, Difference: {diffStr})" : $"(Found Current: {foundCurrentStr}, Diff: {diffStr})"));
                }
            }

            if (vOuts.Any(double.IsNaN))
            {
                Debug.WriteLine($"[WARNING] V_Out data is incomplete for current {current_A:E9}. Hall Voltages for this current will be NaN. Skipping to next current.");
                DetailedPerCurrentPointResults[current_A] = currentResult;
                continue;
            }

            // Calculation for V_out_avg (Voltage offset due to misalignment or thermal effects)
            // Equation: V_out_avg = (V_out_1 - V_out_2 + V_out_3 - V_out_4) / 4.0
            double v_out_avg = (vOuts[0] - vOuts[1] + vOuts[2] - vOuts[3]) / 4.0;
            Debug.WriteLine($"[DEBUG] V_OUT_AVG for {current_A:E9} A: {v_out_avg:E9}");
            double[] vhsRaw = new double[4];
            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                int tunerPos = i + 1;
                bool stateExists = _measurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField);
                bool tunerExists = stateExists && _measurements[HallMeasurementState.OutwardOrSouthMagneticField].ContainsKey(tunerPos);
                int dataCount = tunerExists ? _measurements[HallMeasurementState.OutwardOrSouthMagneticField][tunerPos].Count : 0;
                Debug.WriteLine($"  [DEBUG] Checking V_HS for Tuner {tunerPos}. StateExists: {stateExists}, TunerExists: {tunerExists}, DataCount: {dataCount}");
                if (!tunerExists || dataCount == 0)
                {
                    vhsRaw[i] = double.NaN;
                    Debug.WriteLine($"  [WARNING] V_HS[{tunerPos}] for {current_A:E9} A: NaN (State, Tuner key missing, or no data for South Magnetic Field)");
                }
                else
                {
                    var dataList = _measurements[HallMeasurementState.OutwardOrSouthMagneticField][tunerPos];
                    Debug.WriteLine($"    [DEBUG-V_HS_DATALIST] Data points for OutwardOrSouthMagneticField, Tuner {tunerPos} (Count: {dataList.Count}):");
                    foreach (var dp in dataList)
                    {
                        double diff = Math.Abs(dp.Item1 - current_A);
                        Debug.WriteLine($"      - Stored Current: {dp.Item1:E9} A, Voltage: {dp.Item2:E9} V, Diff to Target {current_A:E9} A: {diff:E9} (Tolerance: {GlobalSettings.Instance.CurrentTolerance:E9})");
                    }

                    var data = dataList.FirstOrDefault(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance);
                    vhsRaw[i] = data != null ? data.Item2 : double.NaN;
                    string foundCurrentStr = data != null ? $"{data.Item1:E9} A" : "N/A";
                    string diffStr = data != null ? $"{Math.Abs(data.Item1 - current_A):E9}" : "N/A";
                    Debug.WriteLine($"  [DEBUG] V_HS[{tunerPos}] for Target Current {current_A:E9} A: {vhsRaw[i]:E9} V " +
                                        (data == null ? $"(Missing data point. Found current: {foundCurrentStr}, Difference: {diffStr})" : $"(Found Current: {foundCurrentStr}, Diff: {diffStr})"));
                }
            }

            if (vhsRaw.Any(double.IsNaN))
            {
                Debug.WriteLine($"[WARNING] V_HS data is incomplete for current {current_A:E9}. Hall Voltages for this current will be NaN.");
            }

            // Store raw Vhs-Vout for each position for this current
            // Equation: RawVhs_Minus_Vout_ByPosition[i+1] = V_HS_i - V_out_i
            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                if (!double.IsNaN(vhsRaw[i]) && !double.IsNaN(vOuts[i]))
                {
                    currentResult.RawVhs_Minus_Vout_ByPosition[i + 1] = vhsRaw[i] - vOuts[i];
                    Debug.WriteLine($"  [DEBUG] RawVhs_Minus_Vout for Tuner {i + 1}: {currentResult.RawVhs_Minus_Vout_ByPosition[i + 1]:E9}");
                }
            }

            // Calculate Vh_SouthField using the Vhs-Vout adjusted values
            // Equation: Vh_SouthField = ((V_HS_1 - V_Out_1) - (V_HS_2 - V_Out_2) + (V_HS_3 - V_Out_3) - (V_HS_4 - V_Out_4)) / 4.0
            currentResult.Vh_SouthField = (currentResult.RawVhs_Minus_Vout_ByPosition[1] -
                                           currentResult.RawVhs_Minus_Vout_ByPosition[2] +
                                           currentResult.RawVhs_Minus_Vout_ByPosition[3] -
                                           currentResult.RawVhs_Minus_Vout_ByPosition[4]) / 4.0;
            Debug.WriteLine($"[DEBUG] Vh_SouthField (calculated from Vhs-Vout adjusted values) for {current_A:E9} A: {currentResult.Vh_SouthField:E9}");


            // Step 4: Collect V_HN (North Field) for all 4 tuners for this specific current
            double[] vhnRaw = new double[4];
            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                int tunerPos = i + 1;
                bool stateExists = _measurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField);
                bool tunerExists = stateExists && _measurements[HallMeasurementState.InwardOrNorthMagneticField].ContainsKey(tunerPos);
                int dataCount = tunerExists ? _measurements[HallMeasurementState.InwardOrNorthMagneticField][tunerPos].Count : 0;
                Debug.WriteLine($"  [DEBUG] Checking V_HN for Tuner {tunerPos}. StateExists: {stateExists}, TunerExists: {tunerExists}, DataCount: {dataCount}");
                if (!tunerExists || dataCount == 0)
                {
                    vhnRaw[i] = double.NaN;
                    Debug.WriteLine($"  [WARNING] V_HN[{tunerPos}] for {current_A:E9} A: NaN (State, Tuner key missing, or no data for North Magnetic Field)");
                }
                else
                {
                    var dataList = _measurements[HallMeasurementState.InwardOrNorthMagneticField][tunerPos];
                    // *** เพิ่ม Debug: แสดงข้อมูลทุกจุดใน dataList และ Diff ของแต่ละจุดเทียบกับ targetCurrent ***
                    Debug.WriteLine($"    [DEBUG-V_HN_DATALIST] Data points for InwardOrNorthMagneticField, Tuner {tunerPos} (Count: {dataList.Count}):");
                    foreach (var dp in dataList)
                    {
                        double diff = Math.Abs(dp.Item1 - current_A);
                        Debug.WriteLine($"      - Stored Current: {dp.Item1:E9} A, Voltage: {dp.Item2:E9} V, Diff to Target {current_A:E9} A: {diff:E9} (Tolerance: {GlobalSettings.Instance.CurrentTolerance:E9})");
                    }
                    // **********************************************************************************************

                    var data = dataList.FirstOrDefault(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance);
                    vhnRaw[i] = data != null ? data.Item2 : double.NaN;
                    string foundCurrentStr = data != null ? $"{data.Item1:E9} A" : "N/A";
                    string diffStr = data != null ? $"{Math.Abs(data.Item1 - current_A):E9}" : "N/A";
                    Debug.WriteLine($"  [DEBUG] V_HN[{tunerPos}] for Target Current {current_A:E9} A: {vhnRaw[i]:E9} V " +
                                        (data == null ? $"(Missing data point. Found current: {foundCurrentStr}, Difference: {diffStr})" : $"(Found Current: {foundCurrentStr}, Diff: {diffStr})"));
                }
            }

            if (vhnRaw.Any(double.IsNaN))
            {
                Debug.WriteLine($"[WARNING] V_HN data is incomplete for current {current_A:E9}. Hall Voltages for this current will be NaN.");
            }

            // Store raw Vhn-Vout for each position for this current
            // Equation: RawVhn_Minus_Vout_ByPosition[i+1] = V_HN_i - V_out_i
            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                if (!double.IsNaN(vhnRaw[i]) && !double.IsNaN(vOuts[i]))
                {
                    currentResult.RawVhn_Minus_Vout_ByPosition[i + 1] = vhnRaw[i] - vOuts[i];
                    Debug.WriteLine($"  [DEBUG] RawVhn_Minus_Vout for Tuner {i + 1}: {currentResult.RawVhn_Minus_Vout_ByPosition[i + 1]:E9}");
                }
            }

            // Calculate Vh_NorthField using the Vhn-Vout adjusted values
            // Equation: Vh_NorthField = ((V_HN_1 - V_Out_1) - (V_HN_2 - V_Out_2) + (V_HN_3 - V_Out_3) - (V_HN_4 - V_Out_4)) / 4.0
            currentResult.Vh_NorthField = (currentResult.RawVhn_Minus_Vout_ByPosition[1] -
                                           currentResult.RawVhn_Minus_Vout_ByPosition[2] +
                                           currentResult.RawVhn_Minus_Vout_ByPosition[3] -
                                           currentResult.RawVhn_Minus_Vout_ByPosition[4]) / 4.0;
            Debug.WriteLine($"[DEBUG] Vh_NorthField (calculated from Vhn-Vout adjusted values) for {current_A:E9} A: {currentResult.Vh_NorthField:E9}");

            DetailedPerCurrentPointResults[current_A] = currentResult;
        }

        Debug.WriteLine("[DEBUG] CalculateHallVoltages - End.");
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

        } // End foreach (var entry in DetailedPerCurrentPointResults)
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