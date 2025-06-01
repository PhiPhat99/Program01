using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    public string StateKey { get; }
    public int TunerPosition { get; }
    public List<(double Source, double Reading)> IndividualData { get; }

    public Dictionary<int, List<(double Source, double Reading)>> NoMagneticFieldData { get; private set; } = new Dictionary<int, List<(double Source, double Reading)>>();
    public Dictionary<int, List<(double Source, double Reading)>> SouthFieldData { get; private set; } = new Dictionary<int, List<(double Source, double Reading)>>();
    public Dictionary<int, List<(double Source, double Reading)>> NorthFieldData { get; private set; } = new Dictionary<int, List<(double Source, double Reading)>>();

    public HallVoltageDataUpdatedEventArgs(string stateKey, int tunerPosition, List<(double Source, double Reading)> individualData)
    {
        StateKey = stateKey;
        TunerPosition = tunerPosition;
        IndividualData = individualData;
    }

    public HallVoltageDataUpdatedEventArgs(
        Dictionary<int, List<(double Source, double Reading)>> noMagneticFieldData,
        Dictionary<int, List<(double Source, double Reading)>> southData,
        Dictionary<int, List<(double Source, double Reading)>> northData)
    {
        NoMagneticFieldData = noMagneticFieldData;
        SouthFieldData = southData;
        NorthFieldData = northData;
    }
}

public class CollectAndCalculateHallMeasured
{
    private static CollectAndCalculateHallMeasured _instance;
    private static readonly object _lock = new object();

    private readonly Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>> _measurements =
        new Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>>()
        {
            { HallMeasurementState.NoMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.InwardOrNorthMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.OutwardOrSouthMagneticField, new Dictionary<int, List<(double, double)>>() }
        };

    private Dictionary<int, List<(double Current, double HallVoltage)>> _hallIVHSouthData;
    public Dictionary<int, List<(double Current, double HallVoltage)>> HallIVHSouthData
    {
        get { return _hallIVHSouthData; }
        private set { _hallIVHSouthData = value; }
    }

    private Dictionary<int, List<(double Current, double HallVoltage)>> _hallIVHNorthData;
    public Dictionary<int, List<(double Current, double HallVoltage)>> HallIVHNorthData
    {
        get { return _hallIVHNorthData; }
        private set { _hallIVHNorthData = value; }
    }

    private readonly Dictionary<int, double> _averageCurrentByPosition = new Dictionary<int, double>(); // เก็บค่ากระแสเฉลี่ยสำหรับแต่ละตำแหน่ง

    public enum SemiconductorType
    {
        Unknown,
        N,
        P
    }

    public event EventHandler<Dictionary<int, List<(double Current, double HallVoltage)>>> HallIVHSouthDataCalculated;
    public event EventHandler<Dictionary<int, List<(double Current, double HallVoltage)>>> HallIVHNorthDataCalculated;
    private Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> lastMeasurements;
    private readonly Dictionary<int, double> _hallVoltageByPosition = new Dictionary<int, double>();
    public double ElementaryCharge { get; set; } = 1.602176634E-19; // ประจุอิเล็กตรอน (C)

    // Event
    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<Dictionary<int, double>> HallVoltageCalculated;
    public event EventHandler<Dictionary<int, List<(double Current, double HallVoltage)>>> HallIVHDataCalculated;
    public event EventHandler<(double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility)> HallPropertiesCalculated;
    public event EventHandler<SemiconductorType> SemiconductorTypeCalculated;
    public event EventHandler<Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>>> AllHallDataUpdated;

    private CollectAndCalculateHallMeasured() { }

    public static CollectAndCalculateHallMeasured Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = _instance ?? new CollectAndCalculateHallMeasured();
                }
            }
            return _instance;
        }
    }

    public void StoreMeasurementData(int tuner, List<(double Source, double Reading)> data, HallMeasurementState state)
    {
        if (data == null || data.Count == 0) return;

        if (!_measurements[state].ContainsKey(tuner))
            _measurements[state][tuner] = new List<(double, double)>();

        _measurements[state][tuner].AddRange(data);

        Debug.WriteLine($"[DEBUG - StoreMeasurementData] State: {state}, Tuner: {tuner}, Data Count: {data.Count}");
        foreach (var item in data)
        {
            Debug.WriteLine($"[DEBUG - StoreMeasurementData]    Source: {item.Source}, Reading: {item.Reading}");
        }

        OnDataUpdated(new HallVoltageDataUpdatedEventArgs(state.ToString(), tuner, new List<(double, double)>(data)));
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurements(HallMeasurementState state)
    {
        return _measurements[state].ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>> GetAllHallMeasurements()
    {
        return _measurements.ToDictionary(s => s.Key, s => s.Value.ToDictionary(p => p.Key, p => p.Value.ToList()));
    }

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> GetLastMeasurements()
    {
        return lastMeasurements?.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToDictionary(
                innerKvp => innerKvp.Key,
                innerKvp => innerKvp.Value.ToList()
            )
        );
    }

    public void ClearAllData()
    {
        foreach (var state in _measurements.Keys) _measurements[state].Clear();
        ClearHallResults();
    }

    public void ClearHallResults()
    {
        _hallVoltageByPosition.Clear();
        _averageCurrentByPosition.Clear(); // ล้างค่ากระแสเฉลี่ย
        GlobalSettings.Instance.TotalHallVoltage = double.NaN;
        GlobalSettings.Instance.HallCoefficient = double.NaN;
        GlobalSettings.Instance.SheetConcentration = double.NaN;
        GlobalSettings.Instance.BulkConcentration = double.NaN;
        GlobalSettings.Instance.Mobility = double.NaN;
    }

    private double CalculateSlope(List<(double X, double Y)> data)
    {
        int n = data.Count;
        if (n < 2) return double.NaN;

        double sumX = data.Sum(d => d.X);
        double sumY = data.Sum(d => d.Y);
        double sumXY = data.Sum(d => d.X * d.Y);
        double sumX2 = data.Sum(d => d.X * d.X);

        double numerator = sumXY - (sumX * sumY / n);
        double denominator = sumX2 - (sumX * sumX / n);
        return denominator == 0 ? double.NaN : numerator / denominator; // คำนวณความชันจากข้อมูล (X, Y) โดยใช้ Linear Regression
    }

    public void CalculateHall()
    {
        ClearHallResults();
        var slopes = new Dictionary<int, double>();
        var hallVoltagesByPosition = new Dictionary<int, double>();
        var averageCurrentsByPosition = new Dictionary<int, double>(); // เก็บค่ากระแสเฉลี่ยต่อตำแหน่ง
        var ivhSouthDataByPosition = new Dictionary<int, List<(double Current, double HallVoltage)>>();
        var ivhNorthDataByPosition = new Dictionary<int, List<(double Current, double HallVoltage)>>();

        double sumHallVoltage = 0;
        int validHallVoltageCount = 0;

        // เก็บค่า VHS และ VHN สำหรับแต่ละตำแหน่ง
        var vhsValues = new Dictionary<int, List<double>>();
        var vhnValues = new Dictionary<int, List<double>>();

        for (int pos = 1; pos <= 4; pos++)
        {
            if (!_measurements.All(m => m.Value.ContainsKey(pos))) continue;

            var outData = _measurements[HallMeasurementState.NoMagneticField][pos];
            var southData = _measurements[HallMeasurementState.OutwardOrSouthMagneticField][pos];
            var northData = _measurements[HallMeasurementState.InwardOrNorthMagneticField][pos];

            var southHall = new List<(double Source, double DeltaVoltage)>();
            var northHall = new List<(double Source, double DeltaVoltage)>();
            var ivhSouthList = new List<(double Current, double HallVoltage)>();
            var ivhNorthList = new List<(double Current, double HallVoltage)>();

            double currentHallVoltageSum = 0;
            double currentSum = 0; // ผลรวมของค่ากระแสเพื่อหาค่าเฉลี่ย
            int validHallVoltageForPositionCount = 0;
            int currentCount = 0; // จำนวนจุดข้อมูลกระแส

            vhsValues[pos] = new List<double>();
            vhnValues[pos] = new List<double>();

            for (int i = 0; i < outData.Count; i++)
            {
                double current = outData[i].Item1;
                double vhs = i < southData.Count ? southData[i].Item2 - outData[i].Item2 : double.NaN; // VHS
                double vhn = i < northData.Count ? northData[i].Item2 - outData[i].Item2 : double.NaN; // VHN

                Debug.WriteLine($"[DEBUG - CalculateHall - Pos {pos}, Index {i}] Current: {current}, VHS: {vhs}, VHN: {vhn}");

                if (!double.IsNaN(vhs))
                {
                    southHall.Add((current, vhs));
                    ivhSouthList.Add((current, vhs));
                    vhsValues[pos].Add(vhs);
                }
                if (!double.IsNaN(vhn))
                {
                    northHall.Add((current, vhn));
                    ivhNorthList.Add((current, vhn));
                    vhnValues[pos].Add(vhn);
                }

                // คำนวณ Hall Voltage (VH) แบบใหม่ - ยังไม่เฉลี่ยรวมทุกตำแหน่ง
                double hallVoltage = (!double.IsNaN(vhs) && !double.IsNaN(vhn)) ? (vhs + vhn) / 2 : !double.IsNaN(vhs) ? vhs : vhn;
                Debug.WriteLine($"[DEBUG - CalculateHall - Pos {pos}, Index {i}] Hall Voltage (VH individual): {hallVoltage}");

                if (!double.IsNaN(hallVoltage))
                {
                    currentHallVoltageSum += hallVoltage;
                    currentSum += current; // สะสมค่ากระแส
                    validHallVoltageForPositionCount++;
                    currentCount++; // นับจำนวนจุดข้อมูลกระแส
                }
            }

            double averageHallVoltageForPosition = validHallVoltageForPositionCount > 0 ? currentHallVoltageSum / validHallVoltageForPositionCount : double.NaN;
            hallVoltagesByPosition[pos] = averageHallVoltageForPosition;
            double averageCurrentForPosition = currentCount > 0 ? currentSum / currentCount : double.NaN; // คำนวณกระแสเฉลี่ยต่อตำแหน่ง
            averageCurrentsByPosition[pos] = averageCurrentForPosition;
            Debug.WriteLine($"[DEBUG - CalculateHall - Pos {pos}] Average Hall Voltage (VH per pos): {averageHallVoltageForPosition}, Average Current (I avg per pos): {averageCurrentForPosition}");

            if (!double.IsNaN(averageHallVoltageForPosition))
            {
                sumHallVoltage += averageHallVoltageForPosition;
                validHallVoltageCount++;
            }

            // คำนวณ Slope (R_Hall เบื้องต้น) สำหรับแต่ละทิศทางและตำแหน่ง
            double slopeS = CalculateSlope(southHall);
            double slopeN = CalculateSlope(northHall);
            slopes[pos] = (!double.IsNaN(slopeS) && !double.IsNaN(slopeN)) ? (slopeS + slopeN) / 2 : double.NaN; // R_Hall เบื้องต้นต่อตำแหน่ง (เฉลี่ย South/North)
            Debug.WriteLine($"[DEBUG - CalculateHall - Pos {pos}] Slope South (R_Hall_S): {slopeS}, Slope North (R_Hall_N): {slopeN}, Average Slope (R_Hall_avg_SN_per_pos): {slopes[pos]}");

            if (ivhSouthList.Count > 0)
            {
                ivhSouthDataByPosition[pos] = ivhSouthList;
            }
            if (ivhNorthList.Count > 0)
            {
                ivhNorthDataByPosition[pos] = ivhNorthList;
            }
        }

        // คำนวณ VH ตามสมการที่ 3
        double totalVh = 0;
        totalVh += vhsValues.TryGetValue(1, out var listVHS1) && listVHS1.Any() ? listVHS1.Average() : 0;
        totalVh -= vhsValues.TryGetValue(2, out var listVHS2) && listVHS2.Any() ? listVHS2.Average() : 0;
        totalVh += vhsValues.TryGetValue(3, out var listVHS3) && listVHS3.Any() ? listVHS3.Average() : 0;
        totalVh -= vhsValues.TryGetValue(4, out var listVHS4) && listVHS4.Any() ? listVHS4.Average() : 0;
        totalVh += vhnValues.TryGetValue(1, out var listVHN1) && listVHN1.Any() ? listVHN1.Average() : 0;
        totalVh -= vhnValues.TryGetValue(2, out var listVHN2) && listVHN2.Any() ? listVHN2.Average() : 0;
        totalVh += vhnValues.TryGetValue(3, out var listVHN3) && listVHN3.Any() ? listVHN3.Average() : 0;
        totalVh -= vhnValues.TryGetValue(4, out var listVHN4) && listVHN4.Any() ? listVHN4.Average() : 0;
        double averageVh = totalVh / 8.0;
        GlobalSettings.Instance.TotalHallVoltage = averageVh;
        Debug.WriteLine($"[DEBUG - CalculateHall] Total Average VH (Equation 3): {averageVh}");

        // คำนวณ RHall ตามสมการที่ 5 (VH / I(avg)) โดยใช้ค่าเฉลี่ยกระแสจากทุกตำแหน่ง
        double totalAverageCurrent = averageCurrentsByPosition.Values.Sum() / averageCurrentsByPosition.Count;
        double rHall = double.IsNaN(averageVh) || totalAverageCurrent == 0 ? double.NaN : averageVh / totalAverageCurrent;
        GlobalSettings.Instance.HallResistance = rHall;
        Debug.WriteLine($"[DEBUG - CalculateHall] Hall Resistance (R_Hall - Equation 5): {rHall}");

        lastMeasurements = _measurements.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToDictionary(
                innerKvp => innerKvp.Key,
                innerKvp => innerKvp.Value.ToList()
            )
        );
        Debug.WriteLine("[DEBUG - CalculateHall] Hall calculation done, data stored.");

        HallIVHSouthData = ivhSouthDataByPosition;
        HallIVHNorthData = ivhNorthDataByPosition;
        GlobalSettings.Instance.HallVoltagesByPosition = hallVoltagesByPosition;
        GlobalSettings.Instance.AverageCurrentsByPosition = averageCurrentsByPosition; // เก็บค่ากระแสเฉลี่ย
        OnHallVoltageCalculated(hallVoltagesByPosition);
        Debug.WriteLine($"[DEBUG - CalculateHall] Hall Voltages by Position: {string.Join(", ", hallVoltagesByPosition)}");
        Debug.WriteLine($"[DEBUG - CalculateHall] Average Currents by Position: {string.Join(", ", averageCurrentsByPosition)}");
        OnHallIVHSouthDataCalculated(ivhSouthDataByPosition);
        OnHallIVHNorthDataCalculated(ivhNorthDataByPosition);

        Debug.WriteLine($"[DEBUG - CalculateHall] Total Average Hall Voltage (เดิม): {GlobalSettings.Instance.TotalHallVoltage}"); // แสดงค่าเดิมด้วย
        CalculateHallProperties(slopes); // ส่งค่า slopes ไปคำนวณ RH และอื่นๆ
        OnCalculationCompleted();
        Debug.WriteLine("[DEBUG - CalculateHall] Triggering AllHallDataUpdated event");
    }

    private void CalculateHallProperties(Dictionary<int, double> slopes)
    {
        // คำนวณ R_H ตามสมการที่ 6: R_H = R_Hall / B
        double rHallCoefficient = double.IsNaN(GlobalSettings.Instance.HallResistance) || GlobalSettings.Instance.MagneticFieldsValueStd == 0
            ? double.NaN
            : GlobalSettings.Instance.HallResistance / GlobalSettings.Instance.MagneticFieldsValueStd;
        GlobalSettings.Instance.HallCoefficient = rHallCoefficient;
        Debug.WriteLine($"[DEBUG - CalculateHallProperties] Hall Coefficient (R_H - Equation 6): {rHallCoefficient}");

        // คำนวณ nb ตามสมการที่ 8: nb = B / (q * t * RHall)
        double nb = (GlobalSettings.Instance.MagneticFieldsValueStd != 0 && ElementaryCharge != 0 && GlobalSettings.Instance.ThicknessValueStd != 0 && !double.IsNaN(GlobalSettings.Instance.HallResistance))
            ? GlobalSettings.Instance.MagneticFieldsValueStd / (ElementaryCharge * GlobalSettings.Instance.ThicknessValueStd * GlobalSettings.Instance.HallResistance)
            : double.NaN;
        GlobalSettings.Instance.BulkConcentration = nb;
        Debug.WriteLine($"[DEBUG - CalculateHallProperties] Bulk Concentration (nb - Equation 8): {nb}");

        // คำนวณ ns ตามสมการที่ 7: ns = nb * t
        double ns = !double.IsNaN(nb) ? nb * GlobalSettings.Instance.ThicknessValueStd : double.NaN;
        GlobalSettings.Instance.SheetConcentration = ns;
        Debug.WriteLine($"[DEBUG - CalculateHallProperties] Sheet Concentration (ns - Equation 7): {ns}");

        // คำนวณ mu ตามสมการที่ 9: µ = 1 / (n * q * ρ) โดยใช้ nb เป็น n
        double mu = (nb != 0 && ElementaryCharge != 0 && GlobalSettings.Instance.Resistivity != 0)
            ? 1 / (nb * ElementaryCharge * GlobalSettings.Instance.Resistivity)
            : double.NaN;
        GlobalSettings.Instance.Mobility = mu;
        Debug.WriteLine($"[DEBUG - CalculateHallProperties] Mobility (µ - Equation 9 using nb): {mu}");

        var type = DetermineSemiconductorType(rHallCoefficient);
        OnHallPropertiesCalculated((rHallCoefficient, ns, nb, mu));
        OnSemiconductorTypeCalculated(type);
    }

    /*private double CalculateHallCoefficient(double avgSlope)
    {
        // Hall Coefficient (R_H) = V_H * t / (I * B)
        // โดยที่ avgSlope คือ Hall Resistance (R_Hall = V_H / I)
        // ดังนั้น R_H = R_Hall * t / B
        if (GlobalSettings.Instance.ThicknessValueStd == 0 || GlobalSettings.Instance.MagneticFieldsValueStd == 0 || GlobalSettings.Instance.AverageCurrentsByPosition == null || GlobalSettings.Instance.AverageCurrentsByPosition.Values.Any(v => v == 0))
            return double.NaN;

        // ใช้ค่ากระแสเฉลี่ยจากทุกตำแหน่ง
        double averageCurrent = GlobalSettings.Instance.AverageCurrentsByPosition.Values.Average();
        double rHallCoefficient = (averageCurrent != 0) ? (avgSlope * GlobalSettings.Instance.ThicknessValueStd) / GlobalSettings.Instance.MagneticFieldsValueStd : double.NaN; // R_H = R_Hall * t / B
        GlobalSettings.Instance.HallCoefficient = rHallCoefficient;
        Debug.WriteLine($"[DEBUG] Hall Coefficient (R_H): {rHallCoefficient} m^3/C");
        return rHallCoefficient;
    }

    private double CalculateSheetConcentration(double rHallCoefficient)
    {
        // Sheet Concentration (n_s) = 1 / (|R_H| * |e|)
        // โดยที่ R_H คือ Hall Coefficient และ e คือประจุอิเล็กตรอน
        if (rHallCoefficient == 0 || ElementaryCharge == 0) return double.NaN;
        double ns = 1 / (Math.Abs(rHallCoefficient) * Math.Abs(ElementaryCharge));
        GlobalSettings.Instance.SheetConcentration = ns;
        Debug.WriteLine($"[DEBUG] Sheet Concentration (n_s): {ns} m^-2");
        return ns;
    }

    private double CalculateBulkConcentration(double rHallCoefficient)
    {
        // Bulk Concentration (n_b) = 1 / (|R_H| * |e|)
        // โดยที่ R_H คือ Hall Coefficient และ e คือประจุอิเล็กตรอน
        if (rHallCoefficient == 0 || ElementaryCharge == 0 || GlobalSettings.Instance.ThicknessValueStd == 0) return double.NaN;
        double nb = 1 / (Math.Abs(rHallCoefficient) * Math.Abs(ElementaryCharge));
        GlobalSettings.Instance.BulkConcentration = nb;
        Debug.WriteLine($"[DEBUG] Bulk Concentration (n_b): {nb} m^-3");
        return nb;
    }

    private double CalculateMobility(double rHallCoefficient)
    {
        // Mobility (µ) = |R_H| / Resistivity
        // โดยที่ R_H คือ Hall Coefficient และ Resistivity คือความต้านทานจำเพาะ
        if (Math.Abs(GlobalSettings.Instance.Resistivity) < double.Epsilon || double.IsNaN(rHallCoefficient)) return double.NaN;
        double mu = Math.Abs(rHallCoefficient) / GlobalSettings.Instance.Resistivity;
        GlobalSettings.Instance.Mobility = mu;
        Debug.WriteLine($"[DEBUG] Mobility (µ): {mu} m^2/V⋅s");
        return mu;
    }*/

    private SemiconductorType DetermineSemiconductorType(double rHallCoefficient)
    {
        if (rHallCoefficient < 0)
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: N-type (Hall Coefficient < 0)");
            return SemiconductorType.N;
        }
        else if (rHallCoefficient > 0)
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: P-type (Hall Coefficient > 0)");
            return SemiconductorType.P;
        }
        else
        {
            Debug.WriteLine("[DEBUG] Semiconductor Type: Unknown (Hall Coefficient is 0)");
            return SemiconductorType.Unknown;
        }
    }

    public Dictionary<int, double> GetAverageNoFieldVoltagesByPosition()
    {
        var noFieldMeasurements = _measurements[HallMeasurementState.NoMagneticField];
        var avgVoltages = new Dictionary<int, double>();
        foreach (var kvp in noFieldMeasurements)
        {
            if (kvp.Value != null && kvp.Value.Any())
            {
                avgVoltages[kvp.Key] = kvp.Value.Average(item => item.Item2); // เข้าถึง Element ที่สองของ Tuple (Reading)
            }
        }
        Debug.WriteLine($"[DEBUG - GetAverageNoFieldVoltagesByPosition] Average No Field Voltages: {string.Join(", ", avgVoltages.Select(kv => $"Pos {kv.Key}: {kv.Value}"))}");
        return avgVoltages;
    }

    public Dictionary<int, double> GetAverageSouthFieldVoltagesByPosition()
    {
        var southMeasurements = _measurements[HallMeasurementState.OutwardOrSouthMagneticField];
        var avgVoltages = new Dictionary<int, double>();
        foreach (var kvp in southMeasurements)
        {
            if (kvp.Value != null && kvp.Value.Any())
            {
                avgVoltages[kvp.Key] = kvp.Value.Average(item => item.Item2); // เข้าถึง Element ที่สองของ Tuple (Reading)
            }
        }
        Debug.WriteLine($"[DEBUG - GetAverageSouthFieldVoltagesByPosition] Average South Field Voltages: {string.Join(", ", avgVoltages.Select(kv => $"Pos {kv.Key}: {kv.Value}"))}");
        return avgVoltages;
    }

    public Dictionary<int, double> GetAverageNorthFieldVoltagesByPosition()
    {
        var northMeasurements = _measurements[HallMeasurementState.InwardOrNorthMagneticField];
        var avgVoltages = new Dictionary<int, double>();
        foreach (var kvp in northMeasurements)
        {
            if (kvp.Value != null && kvp.Value.Any())
            {
                avgVoltages[kvp.Key] = kvp.Value.Average(item => item.Item2); // เข้าถึง Element ที่สองของ Tuple (Reading)
            }
        }
        Debug.WriteLine($"[DEBUG - GetAverageNorthFieldVoltagesByPosition] Average North Field Voltages: {string.Join(", ", avgVoltages.Select(kv => $"Pos {kv.Key}: {kv.Value}"))}");
        return avgVoltages;
    }

    // Trigger Events
    protected virtual void OnDataUpdated(HallVoltageDataUpdatedEventArgs e)
    {
        DataUpdated?.Invoke(this, e);
    }

    protected virtual void OnCalculationCompleted()
    {
        CalculationCompleted?.Invoke(this, EventArgs.Empty);
        AllHallDataUpdated?.Invoke(this, GetAllHallMeasurements());
    }

    protected virtual void OnHallVoltageCalculated(Dictionary<int, double> hallVoltages)
    {
        HallVoltageCalculated?.Invoke(this, hallVoltages);
    }

    protected virtual void OnHallIVHSouthDataCalculated(Dictionary<int, List<(double Current, double HallVoltage)>> ivhData)
    {
        HallIVHSouthDataCalculated?.Invoke(this, ivhData);
    }

    protected virtual void OnHallIVHNorthDataCalculated(Dictionary<int, List<(double Current, double HallVoltage)>> ivhData)
    {
        HallIVHNorthDataCalculated?.Invoke(this, ivhData);
    }

    protected virtual void OnHallPropertiesCalculated((double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility) properties)
    {
        HallPropertiesCalculated?.Invoke(this, properties);
    }

    protected virtual void OnSemiconductorTypeCalculated(SemiconductorType type)
    {
        SemiconductorTypeCalculated?.Invoke(this, type);
    }
}