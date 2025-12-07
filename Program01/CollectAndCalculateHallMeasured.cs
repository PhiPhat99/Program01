using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    // ***** Properties สำหรับเก็บข้อมูลที่ส่งผ่านอีเวนต์ *****
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

// ***** DetailedHallPropertiesCalculatedEventArgs : EventArgs สำหรับส่งผ่านผลลัพธ์การคำนวณ Hall Properties ที่ทุกช่วงกระแสที่จ่าย *****
public class DetailedHallPropertiesCalculatedEventArgs : EventArgs
{
    public Dictionary<double, HallCalculationResultPerCurrent> PerCurrentPointResults { get; private set; }
    public DetailedHallPropertiesCalculatedEventArgs(Dictionary<double, HallCalculationResultPerCurrent> results)
    {
        PerCurrentPointResults = results;
    }
}

// ***** HallCalculationResultPerCurrent : คลาสสำหรับเก็บผลลัพธ์การคำนวณ Hall Properties ที่ช่วงกระแสที่จ่ายแต่ละจุด *****
public class HallCalculationResultPerCurrent
{
    public double Current { get; set; }

    // ***** HallCalculationResultPerCurrent : เมธอดสำหรับเก็บผลลัพธ์การคำนวณ Hall Voltage ที่ช่วงกระแสที่จ่ายแต่ละจุด *****
    public HallCalculationResultPerCurrent(double current)
    {
        Current = current;
        RawVhn_Minus_Vout_ByPosition = new Dictionary<int, double>();
        RawVhs_Minus_Vout_ByPosition = new Dictionary<int, double>();
    }

    // ***** Properties สำหรับเก็บผลคำนวณค่าสมบัติฮอลล์ (Hall Properties) ที่ทุกช่วงกระแสที่จ่ายในสนามแม่เหล็กทิศใต้/พุ่งออก (+B) *****
    public double Vh_SouthField { get; set; } = double.NaN;
    public double Vh_NorthField { get; set; } = double.NaN;
    public double R_Hall_SouthField { get; set; } = double.NaN; // Hall Resistance (Vh/I in Ohm)
    public double R_H_SouthField { get; set; } = double.NaN; // Hall Coefficient in cm^3/C
    public double SheetConcentration_SouthField { get; set; } = double.NaN; // Sheet Concentration in cm^-2
    public double BulkConcentration_SouthField { get; set; } = double.NaN; // Bulk Concentration in cm^-3
    public double Mobility_SouthField { get; set; } = double.NaN;   // Mobility in cm^2/(V*s)

    // ***** Properties สำหรับเก็บผลคำนวณค่าสมบัติฮอลล์ (Hall Properties) ที่ทุกช่วงกระแสที่จ่ายในสนามแม่เหล็กทิศเหนือ/พุ่งเข้า (-B) *****
    public double R_Hall_NorthField { get; set; } = double.NaN; // Hall Resistance (Vh/I in Ohm)
    public double R_H_NorthField { get; set; } = double.NaN; // Hall Coefficient in cm^3/C
    public double SheetConcentration_NorthField { get; set; } = double.NaN; // Sheet Concentration in cm^-2
    public double BulkConcentration_NorthField { get; set; } = double.NaN; // Bulk Concentration in cm^-3
    public double Mobility_NorthField { get; set; } = double.NaN; // Mobility in cm^2/(V*s)

    // ***** Properties สำหรับเก็บผลคำนวณค่าสมบัติฮอลล์จริง (Real Hall Properties) ที่ทุกช่วงกระแสที่จ่าย โดยเฉลี่ยจากทั้งสองทิศทางสนามแม่เหล็ก *****
    public double Vh_Average { get; set; } = double.NaN; // (Vh_SouthField - Vh_NorthField) / 2
    public double R_Hall_ByCurrent { get; set; } = double.NaN; // (R_Hall_SouthField - R_Hall_NorthField) / 2
    public double R_H_ByCurrent { get; set; } = double.NaN; // (R_H_SouthField - R_H_NorthField) / 2
    public double SheetConcentration_ByCurrent { get; set; } = double.NaN; // (SheetConcentration_SouthField + SheetConcentration_NorthField) / 2
    public double BulkConcentration_ByCurrent { get; set; } = double.NaN; // (BulkConcentration_SouthField + BulkConcentration_NorthField) / 2
    public double Mobility_ByCurrent { get; set; } = double.NaN; // (Mobility_SouthField + Mobility_NorthField) / 2

    public Dictionary<int, double> RawVhn_Minus_Vout_ByPosition { get; set; } // Vhn - Vout per Tuner Position
    public Dictionary<int, double> RawVhs_Minus_Vout_ByPosition { get; set; } // Vhs - Vout per Tuner Position
}

// **** CollectAndCalculateHallMeasured : คลาสหลักสำหรับเก็บข้อมูลการวัดฮอลล์และคำนวณสมบัติฮอลล์ *****
public class CollectAndCalculateHallMeasured
{
    private const int NumberOfHallPositions = 4;
    private static readonly Lazy<CollectAndCalculateHallMeasured> lazy = new Lazy<CollectAndCalculateHallMeasured>(() => new CollectAndCalculateHallMeasured());
    public static CollectAndCalculateHallMeasured Instance { get; } = lazy.Value;

    // ***** Constructor ส่วนตัวเพื่อป้องกันการสร้างอินสแตนซ์จากภายนอก *****
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

    // ***** Enum สำหรับชนิดสารของ Sample *****
    public enum SemiconductorType
    {
        Unknown,
        N,
        P
    }

    // ***** _measurements : โครงสร้างข้อมูลสำหรับเก็บข้อมูลการวัดฮอลล์ *****
    private Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> _measurements;
    public Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> Measurements
    {
        get { return _measurements; }
    }

    // ***** AllRawMeasurements, AverageVhsByPosition, AverageVhnByPosition, DetailedPerCurrentPointResults : Properties สำหรับเข้าถึงข้อมูลการวัดและผลลัพธ์การคำนวณ *****
    public IReadOnlyDictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> AllRawMeasurements => _measurements;
    public Dictionary<int, double> AverageVhsByPosition { get; private set; }
    public Dictionary<int, double> AverageVhnByPosition { get; private set; }
    public Dictionary<double, HallCalculationResultPerCurrent> DetailedPerCurrentPointResults { get; private set; } = new Dictionary<double, HallCalculationResultPerCurrent>();

    // ***** DataUpdated, CalculationCompleted, DetailedHallPropertiesUpdated : อีเวนต์สำหรับแจ้งเตือนการอัปเดตข้อมูลและผลลัพธ์การคำนวณ *****
    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<DetailedHallPropertiesCalculatedEventArgs> DetailedHallPropertiesUpdated;

    // ***** OnDataUpdated, OnCalculationCompleted, OnDetailedHallPropertiesCalculated : เมธอดสำหรับเรียกใช้อีเวนต์ *****
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

    // **** StoreMeasurementData : เมธอดสำหรับเก็บข้อมูลการวัดฮอลล์ *****
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

    // ***** GetAllRawMeasurements : เมธอดสำหรับดึงข้อมูลดิบของการวัดฮอลล์ทั้งหมด *****
    public Dictionary<HallMeasurementState, Dictionary<int, List<Tuple<double, double>>>> GetAllRawMeasurements()
    {
        return _measurements;
    }

    // ***** DebugPrintAllRawMeasurements : เมธอดสำหรับแสดงผลข้อมูลดิบของการวัดฮอลล์ทั้งหมดเพื่อการดีบัก *****
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

    // ***** CalculateAllHallProperties : เมธอดหลักสำหรับคำนวณสมบัติฮอลล์ทั้งหมด *****
    public void CalculateAllHallProperties(double thickness_std, double magneticField_Vs_per_cm2_std)
    {
        Debug.WriteLine("[DEBUG] CalculateAllHallProperties - Starting overall calculation sequence.");
        double magneticField_Vs_per_cm2 = GlobalSettings.Instance.MagneticFieldsValueStd;
        double thickness_cm = GlobalSettings.Instance.ThicknessValueStd;
        double resistivity_ohm_cm = GlobalSettings.Instance.Resistivity;
        double elementaryCharge_C = GlobalSettings.Instance.ElementaryCharge;

        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Thickness (t): {thickness_cm:E9} cm");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Magnetic Field (B): {magneticField_Vs_per_cm2:E9} V s/cm^2");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Resistivity (rho): {resistivity_ohm_cm:E9} Ohm.cm");
        Debug.WriteLine($"[DEBUG-GLOBAL-SETTINGS-INPUT] Elementary Charge (e): {elementaryCharge_C:E9} C");

        CalculateHallVoltages();

        if (DetailedPerCurrentPointResults.Any(r => !double.IsNaN(r.Value.Vh_SouthField) || !double.IsNaN(r.Value.Vh_NorthField)))
        {
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

    // ***** CalculateHallVoltages : เมธอดสำหรับคำนวณแรงดันฮอลล์ (Hall Voltages) ที่ทุกช่วงกระแสที่จ่าย *****
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

        if (!_measurements.ContainsKey(HallMeasurementState.NoMagneticField) || !_measurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField) || !_measurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
        {
            Debug.WriteLine("[ERROR] CollectAndCalculateHallMeasured: Missing Hall measurement states. Cannot calculate Hall voltages.");
            return;
        }

        List<double> currentPoints = _measurements[HallMeasurementState.NoMagneticField].SelectMany(kv => kv.Value.Select(t => t.Item1)).Distinct().OrderBy(c => c).ToList();

        foreach (double current_A in currentPoints)
        {
            Debug.WriteLine($"[DEBUG] Processing Current: {current_A:E9} A");
            HallCalculationResultPerCurrent currentResult = new HallCalculationResultPerCurrent(current_A);

            double[] vOuts = new double[NumberOfHallPositions];
            double[] vhsRaw = new double[NumberOfHallPositions];
            double[] vhnRaw = new double[NumberOfHallPositions];

            for (int i = 0; i < NumberOfHallPositions; i++)
            {
                var dataList_Out = _measurements[HallMeasurementState.NoMagneticField][i + 1];
                var matchingVoltages_Out = dataList_Out
                    .Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance)
                    .Select(d => d.Item2)
                    .ToList();

                if (matchingVoltages_Out.Any())
                {
                    vOuts[i] = matchingVoltages_Out.Average();
                }
                else
                {
                    vOuts[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No V_Out data found for current {current_A:E9} A at Tuner {i + 1}");
                }

                var dataList_South = _measurements[HallMeasurementState.OutwardOrSouthMagneticField][i + 1];
                var matchingVoltages_South = dataList_South.Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance).Select(d => d.Item2).ToList();

                if (matchingVoltages_South.Any())
                {
                    vhsRaw[i] = matchingVoltages_South.Average();
                }
                else
                {
                    vhsRaw[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No Vhs data found for current {current_A:E9} A at Tuner {i + 1}");
                }

                var dataList_North = _measurements[HallMeasurementState.InwardOrNorthMagneticField][i + 1];
                var matchingVoltages_North = dataList_North.Where(d => Math.Abs(d.Item1 - current_A) < GlobalSettings.Instance.CurrentTolerance).Select(d => d.Item2).ToList();

                if (matchingVoltages_North.Any())
                {
                    vhnRaw[i] = matchingVoltages_North.Average();
                }
                else
                {
                    vhnRaw[i] = double.NaN;
                    Debug.WriteLine($"[WARNING] No Vhn data found for current {current_A:E9} A at Tuner {i + 1}");
                }

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

            if (!double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[1]) && !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[2]) &&
                !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[3]) && !double.IsNaN(currentResult.RawVhs_Minus_Vout_ByPosition[4]))
            {
                currentResult.Vh_SouthField = (currentResult.RawVhs_Minus_Vout_ByPosition[1] - currentResult.RawVhs_Minus_Vout_ByPosition[2] +
                                               currentResult.RawVhs_Minus_Vout_ByPosition[3] - currentResult.RawVhs_Minus_Vout_ByPosition[4]) / 4.0;
            }
            else
            {
                currentResult.Vh_SouthField = double.NaN;
                Debug.WriteLine($"[WARNING] Vh_SouthField for current {current_A:E9} A set to NaN due to incomplete offset-cancelled Vhs data.");
            }

            if (!double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[1]) && !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[2]) &&
                !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[3]) && !double.IsNaN(currentResult.RawVhn_Minus_Vout_ByPosition[4]))
            {
                currentResult.Vh_NorthField = (currentResult.RawVhn_Minus_Vout_ByPosition[1] - currentResult.RawVhn_Minus_Vout_ByPosition[2] +
                                               currentResult.RawVhn_Minus_Vout_ByPosition[3] - currentResult.RawVhn_Minus_Vout_ByPosition[4]) / 4.0;
            }
            else
            {
                currentResult.Vh_NorthField = double.NaN;
                Debug.WriteLine($"[WARNING] Vh_NorthField for current {current_A:E9} A set to NaN due to incomplete offset-cancelled Vhn data.");
            }

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

    // ***** CalculateAverageHallVoltagesByPosition : เมธอดสำหรับคำนวณค่าเฉลี่ยแรงดันฮอลล์ (Hall Voltages) โดยแยกตามตำแหน่ง *****
    private void CalculateAverageHallVoltagesByPosition()
    {
        Debug.WriteLine("[DEBUG] CalculateAverageHallVoltagesByPosition - Start: Calculating average Vhs and Vhn by each position.");

        AverageVhsByPosition.Clear();
        AverageVhnByPosition.Clear();

        Dictionary<int, double> sumVhsByPosition = new Dictionary<int, double>();
        Dictionary<int, int> countVhsByPosition = new Dictionary<int, int>();
        Dictionary<int, double> sumVhnByPosition = new Dictionary<int, double>();
        Dictionary<int, int> countVhnByPosition = new Dictionary<int, int>();

        for (int i = 1; i <= NumberOfHallPositions; i++)
        {
            sumVhsByPosition[i] = 0.0;
            countVhsByPosition[i] = 0;
            sumVhnByPosition[i] = 0.0;
            countVhnByPosition[i] = 0;
        }

        foreach (var currentResultEntry in DetailedPerCurrentPointResults)
        {
            var currentResult = currentResultEntry.Value;
            foreach (var positionEntry in currentResult.RawVhs_Minus_Vout_ByPosition)
            {
                int position = positionEntry.Key;
                double value = positionEntry.Value;
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

            foreach (var positionEntry in currentResult.RawVhn_Minus_Vout_ByPosition)
            {
                int position = positionEntry.Key;
                double value = positionEntry.Value;
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

    // ***** CalculateHallPropertiesByFieldDirection : เมธอดสำหรับคำนวณสมบัติฮอลล์ (Hall Properties) แยกตามทิศทางสนามแม่เหล็ก *****
    public void CalculateHallPropertiesByFieldDirection()
    {
        Debug.WriteLine("[DEBUG] CalculateHallPropertiesByFieldDirection - Start.");

        double magneticField_Vs_per_cm2 = GlobalSettings.Instance.MagneticFieldsValueStd;
        double thickness_cm = GlobalSettings.Instance.ThicknessValueStd;
        double elementaryCharge = GlobalSettings.Instance.ElementaryCharge;
        double resistivity_ohm_cm = GlobalSettings.Instance.Resistivity;

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
            
            // ***** หาก Vh_SouthField ไม่เป็น NaN และ Current ไม่เป็น 0 ให้คำนวณสมบัติฮอลล์ของสนามแม่เหล็กทิศใต้ *****
            if (!double.IsNaN(result.Vh_SouthField) && current != 0)
            {
                result.R_Hall_SouthField = result.Vh_SouthField / current; // 1. ความต้านทานฮอลล์ของสนามแม่เหล็กทิศใต้ : R_Hall(+B) = Vhs / I (หน่วย: Ohm)
                result.R_H_SouthField = (result.Vh_SouthField * thickness_cm) / (current * magneticField_Vs_per_cm2); // 2. ค่าสัมประสิทธิ์ของฮอลล์ของสนามแม่เหล็กทิศใต้ : R_H(+B) = Vhs * t / (I * B) (หน่วย: cm^3/C)
                result.BulkConcentration_SouthField = 1.0 / (elementaryCharge * Math.Abs(result.R_H_SouthField)); // 3. ความเข้มข้นของพาหะในเนื้อสารของสนามแม่เหล็กทิศใต้ : n_Bulk(+B) = 1 / (q * |R_H(+B)|) (หน่วย: cm^-3)
                result.SheetConcentration_SouthField = result.BulkConcentration_SouthField * thickness_cm; // 4. ความเข้มข้นของพาหะในพื้นผิวของสนามแม่เหล็กทิศใต้ : n_Sheet(+B) = n_Bulk(+B) * t (หน่วย: cm^-2)
                result.Mobility_SouthField = Math.Abs(result.R_H_SouthField) / resistivity_ohm_cm; // 5. สภาพคล่องพาหะของสนามแม่เหล็กทิศใต้ : μ(+B) = |R_H(+B)| / ρ (หน่วย: cm^2/(V*s))
            }
            // ***** หาก Vh_SouthField เป็น NaN หรือ Current เป็น 0 ให้ตั้งค่าทุกสมบัติของสนามแม่เหล็กทิศใต้เป็น NaN *****
            else
            {
                result.R_Hall_SouthField = double.NaN;
                result.R_H_SouthField = double.NaN;
                result.SheetConcentration_SouthField = double.NaN;
                result.BulkConcentration_SouthField = double.NaN;
                result.Mobility_SouthField = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] Vh_SouthField is NaN or Current is 0 for South Field. Properties set to NaN.");
            }

            // ***** หาก Vh_NorthField ไม่เป็น NaN และ Current ไม่เป็น 0 ให้คำนวณสมบัติฮอลล์ของสนามแม่เหล็กทิศเหนือ *****
            if (!double.IsNaN(result.Vh_NorthField) && current != 0)
            {
                result.R_Hall_NorthField = result.Vh_NorthField / current; // 1. ความต้านทานฮอลล์ของสนามแม่เหล็กทิศเหนือ : R_Hall(-B) = Vhn / I (หน่วย: Ohm)
                result.R_H_NorthField = (result.Vh_NorthField * thickness_cm) / (current * magneticField_Vs_per_cm2); // 2. ค่าสัมประสิทธิ์ของฮอลล์ของสนามแม่เหล็กทิศเหนือ : R_H(-B) = Vhn * t / (I * B) (หน่วย: cm^3/C)
                result.BulkConcentration_NorthField = 1.0 / (elementaryCharge * Math.Abs(result.R_H_NorthField)); // 3. ความเข้มข้นของพาหะในเนื้อสารของสนามแม่เหล็กทิศเหนือ : n_Bulk(-B) = 1 / (q * |R_H(-B)|) (หน่วย: cm^-3)
                result.SheetConcentration_NorthField = result.BulkConcentration_NorthField * thickness_cm; // 4. ความเข้มข้นของพาหะในพื้นผิวของสนามแม่เหล็กทิศเหนือ : n_Sheet(-B) = n_Bulk(-B) * t (หน่วย: cm^-2)
                result.Mobility_NorthField = Math.Abs(result.R_H_NorthField) / resistivity_ohm_cm; // 5. สภาพคล่องพาหะของสนามแม่เหล็กทิศเหนือ : μ(-B) = |R_H(-B)| / ρ (หน่วย: cm^2/(V*s))
            }
            // ***** หาก Vh_NorthField เป็น NaN หรือ Current เป็น 0 ให้ตั้งค่าทุกสมบัติของสนามแม่เหล็กทิศเหนือเป็น NaN *****
            else
            {
                result.R_Hall_NorthField = double.NaN;
                result.R_H_NorthField = double.NaN;
                result.SheetConcentration_NorthField = double.NaN;
                result.BulkConcentration_NorthField = double.NaN;
                result.Mobility_NorthField = double.NaN;
                Debug.WriteLine($"  [WARNING-CALC-PROPS] Vh_NorthField is NaN or Current is 0 for North Field. Properties set to NaN.");
            }

            // ***** Debug : แสดงผลลัพธ์สมบัติฮอลล์ที่คำนวณได้สำหรับแต่ละทิศทางสนามแม่เหล็ก *****
            Debug.WriteLine($"  [DEBUG-CALC-PROPS] Current {current:E9} A Results:");
            Debug.WriteLine($"    - Vh_SouthField: {result.Vh_SouthField:E9} V, Vh_NorthField: {result.Vh_NorthField:E9} V");
            Debug.WriteLine($"    - R_Hall_SouthField (Hall Res.): {result.R_Hall_SouthField:E9} Ohm, R_Hall_NorthField (Hall Res.): {result.R_Hall_NorthField:E9} Ohm");
            Debug.WriteLine($"    - R_H_SouthField (Hall Coeff.): {result.R_H_SouthField:E9} cm^3/C, R_H_NorthField (Hall Coeff.): {result.R_H_NorthField:E9} cm^3/C");
            Debug.WriteLine($"    - BulkConcentration_SouthField: {result.BulkConcentration_SouthField:E9} cm^-3, BulkConcentration_NorthField: {result.BulkConcentration_NorthField:E9} cm^-3");
            Debug.WriteLine($"    - SheetConcentration_SouthField: {result.SheetConcentration_SouthField:E9} cm^-2, SheetConcentration_NorthField: {result.SheetConcentration_NorthField:E9} cm^-2");
            Debug.WriteLine($"    - Mobility_SouthField: {result.Mobility_SouthField:E9} cm^2/(V*s), Mobility_NorthField: {result.Mobility_NorthField:E9} cm^2/(V*s)");

            // ***** คำนวณแรงดันฮอลล์โดยเฉลี่ยในแต่ละช่วงกระแสโดยใช้ความแตกต่างระหว่างทิศทางสนามแม่เหล็ก (South - North) / 2 *****
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

            // ***** คำนวณความต้านทานฮอลล์ของแต่ละช่วงกระแสโดยใช้ความแตกต่างระหว่างทิศทางสนามแม่เหล็ก (South - North) / 2 *****
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

            // ***** คำนวณค่าสัมประสิทธิ์ฮอลล์ของแต่ละช่วงกระแสโดยใช้ความแตกต่างระหว่างทิศทางสนามแม่เหล็ก (South - North) / 2 *****
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

            // ***** คำนวณความเข้มข้นของพาหะทั้งในเนื้อสารและพื้นผิว โดยใช้ค่าสัมประสิทธิ์ฮอลล์ที่คำนวณได้จากแต่ละช่วงกระแส *****
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

            // ***** คำนวณสภาพคล่องพาหะ โดยใช้ค่าสัมประสิทธิ์ฮอลล์ที่คำนวณได้จากแต่ละช่วงกระแส *****
            if (!double.IsNaN(result.R_H_ByCurrent) && resistivity_ohm_cm != 0)
            {
                result.Mobility_ByCurrent = Math.Abs(result.R_H_ByCurrent) / resistivity_ohm_cm;
            }
            else
            {
                result.Mobility_ByCurrent = double.NaN;
            }

            // ***** Debug : แสดงผลลัพธ์สมบัติฮอลล์ที่คำนวณได้ในแต่ละช่วงกระแส *****
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

    // ***** CalculateOverallHallProperties : เมธอดสำหรับคำนวณสมบัติฮอลล์ที่แท้จริง (True Hall Properties) โดยรวมทั้งหมด *****
    public void CalculateOverallHallProperties()
    {
        Debug.WriteLine("[DEBUG] CalculateOverallHallProperties - Start.");

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

    // ***** DetermineSemiconductorType : เมธอดสำหรับกำหนดชนิดของสารกึ่งตัวนำจากค่าสัมประสิทธิ์ฮอลล์ *****
    private SemiconductorType DetermineSemiconductorType(double rHallCoefficient)
    {
        // ***** Debug : แสดงค่า Hall Coefficient จริงที่ถูกส่งเข้ามา *****
        Debug.WriteLine($"[DEBUG] DetermineSemiconductorType: rHallCoefficient received: {rHallCoefficient:E20}");
        
        // ***** Debug : แสดงค่า Threshold ปัจจุบัน *****
        Debug.WriteLine($"[DEBUG] DetermineSemiconductorType: Current MinHallCoefficientThresholdForTypeDetermination: {GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination:E20}");

        // ***** ตรวจสอบเงื่อนไขเพื่อกำหนดชนิดของสารกึ่งตัวนำ *****
        // ***** หากค่า Hall Coefficient เป็น NaN, เป็น 0, หรือต่ำกว่าเกณฑ์ที่กำหนด ให้คืนค่า Unknown (ไม่ทราบชนิดของสาร) *****
        if (double.IsNaN(rHallCoefficient) || rHallCoefficient == 0 || Math.Abs(rHallCoefficient) < GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination)
        {
            Debug.WriteLine($"[DEBUG] Semiconductor Type: Unknown (Condition met: IsNaN={double.IsNaN(rHallCoefficient)}, IsZero={rHallCoefficient == 0}, BelowThreshold={Math.Abs(rHallCoefficient) < GlobalSettings.Instance.MinHallCoefficientThresholdForTypeDetermination})");
            return SemiconductorType.Unknown;
        }
        // ***** หากค่า Hall Coefficient น้อยกว่า 0 ให้คืนค่า N-type (สารกึ่งตัวนำชนิด N) *****
        else if (rHallCoefficient < 0)
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: N-type (Hall Coefficient < 0)");
            return SemiconductorType.N;
        }
        // ***** หากค่า Hall Coefficient มากกว่า 0 ให้คืนค่า P-type (สารกึ่งตัวนำชนิด P) *****
        else
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: P-type (Hall Coefficient > 0)");
            return SemiconductorType.P;
        }
    }

    // ***** ClearAllHallData : เมธอดสำหรับล้างข้อมูลการวัดฮอลล์ทั้งหมด *****
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