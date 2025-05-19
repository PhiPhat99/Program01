using System;
using System.Collections.Generic;
using System.Linq;

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    // สำหรับ Chart แต่ละแบบ
    public string StateKey { get; }
    public int TunerPosition { get; }
    public List<(double Source, double Reading)> IndividualData { get; }

    public Dictionary<int, List<(double Source, double Reading)>> NoMagneticFieldData { get; }
    public Dictionary<int, List<(double Source, double Reading)>> SouthFieldData { get; }
    public Dictionary<int, List<(double Source, double Reading)>> NorthFieldData { get; }

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

    // เก็บข้อมูลการวัดสำหรับสถานะแม่เหล็กแต่ละแบบ
    private readonly Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> _measurements =
        new Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>>()
        {
            { HallMeasurementState.NoMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.InwardOrNorthMagneticField, new Dictionary<int, List<(double, double)>>() },
            { HallMeasurementState.OutwardOrSouthMagneticField, new Dictionary<int, List<(double, double)>>() }
        };

    public event EventHandler<SemiconductorType> SemiconductorTypeCalculated;

    public enum SemiconductorType
    {
        Unknown,
        NType,
        PType
    }

    private readonly Dictionary<int, double> _hallVoltageByPosition = new Dictionary<int, double>();
    public double ElementaryCharge { get; set; } = 1.602176634E-19;

    // Event
    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<Dictionary<int, double>> HallVoltageCalculated;
    public event EventHandler<Dictionary<int, List<(double Current, double HallVoltage)>>> HallIVHDataCalculated;
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

        // แจ้ง Event สำหรับกราฟเดี่ยว
        OnDataUpdated(new HallVoltageDataUpdatedEventArgs(state.ToString(), tuner, new List<(double, double)>(data)));

        // แจ้ง Event สำหรับกราฟรวม
        OnDataUpdated(new HallVoltageDataUpdatedEventArgs(
            GetHallMeasurements(HallMeasurementState.NoMagneticField),
            GetHallMeasurements(HallMeasurementState.OutwardOrSouthMagneticField),
            GetHallMeasurements(HallMeasurementState.InwardOrNorthMagneticField)));
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetHallMeasurements(HallMeasurementState state)
    {
        return _measurements[state].ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public Dictionary<HallMeasurementState, Dictionary<int, List<(double, double)>>> GetAllHallMeasurements()
    {
        return _measurements.ToDictionary(
            s => s.Key,
            s => s.Value.ToDictionary(p => p.Key, p => p.Value.ToList()));
    }

    public void ClearAllData()
    {
        foreach (var state in _measurements.Keys)
            _measurements[state].Clear();

        ClearHallResults();
    }

    public void ClearHallResults()
    {
        _hallVoltageByPosition.Clear();
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

        return denominator == 0 ? double.NaN : numerator / denominator;
    }

    public void CalculateHall()
    {
        ClearHallResults();
        var slopes = new Dictionary<int, double>();
        var hallVoltages = new Dictionary<int, double>();
        var ivhDataByPosition = new Dictionary<int, List<(double Current, double HallVoltage)>>();

        for (int pos = 1; pos <= 4; pos++)
        {
            if (!_measurements.All(m => m.Value.ContainsKey(pos))) continue;

            var outData = _measurements[HallMeasurementState.NoMagneticField][pos];
            var southData = _measurements[HallMeasurementState.OutwardOrSouthMagneticField][pos];
            var northData = _measurements[HallMeasurementState.InwardOrNorthMagneticField][pos];

            var southHall = new List<(double, double)>();
            var northHall = new List<(double, double)>();
            var ivhData = new List<(double Current, double HallVoltage)>();

            for (int i = 0; i < outData.Count; i++)
            {
                double current = outData[i].Source;

                if (i < southData.Count)
                    southHall.Add((southData[i].Source, southData[i].Reading - outData[i].Reading));

                if (i < northData.Count)
                    northHall.Add((northData[i].Source, northData[i].Reading - outData[i].Reading));

                // คำนวณ Hall Voltage โดยเฉลี่ยจาก South และ North (อาจต้องปรับตามหลักการวัด)
                double avgHallVoltage = double.NaN;
                if (i < southData.Count && i < northData.Count)
                {
                    avgHallVoltage = (southData[i].Reading - outData[i].Reading + (northData[i].Reading - outData[i].Reading)) / 2;
                    ivhData.Add((current, avgHallVoltage));
                }
                else if (i < southData.Count)
                {
                    avgHallVoltage = southData[i].Reading - outData[i].Reading;
                    ivhData.Add((current, avgHallVoltage));
                }
                else if (i < northData.Count)
                {
                    avgHallVoltage = northData[i].Reading - outData[i].Reading;
                    ivhData.Add((current, avgHallVoltage));
                }

                if (outData.Count > 0 && !double.IsNaN(avgHallVoltage))
                {
                    if (!hallVoltages.ContainsKey(pos))
                    {
                        hallVoltages[pos] = avgHallVoltage; // อาจต้องพิจารณาว่าจะใช้ค่าใดเป็น VH หลัก
                    }
                }
            }

            double slopeS = CalculateSlope(southHall);
            double slopeN = CalculateSlope(northHall);
            slopes[pos] = (!double.IsNaN(slopeS) && !double.IsNaN(slopeN)) ? (slopeS + slopeN) / 2 : double.NaN;

            if (outData.Count > 0) hallVoltages[pos] = outData[0].Reading;

            if (ivhData.Count > 0)
            {
                ivhDataByPosition[pos] = ivhData;
            }
        }

        GlobalSettings.Instance.HallVoltagesByPosition = hallVoltages;
        OnHallVoltageCalculated(hallVoltages);
        OnHallIVHDataCalculated(ivhDataByPosition);


        var validSlopes = slopes.Values.Where(s => !double.IsNaN(s)).ToList();
        
        if (validSlopes.Count == 0 || GlobalSettings.Instance.MagneticFieldsValueStd == 0 || ElementaryCharge == 0)
        {
            OnCalculationCompleted();
            return;
        }

        double avgSlope = validSlopes.Average();
        double R_H, n, mu;

        if (GlobalSettings.Instance.ThicknessValueStd > 0)
        {
            R_H = avgSlope * GlobalSettings.Instance.ThicknessValueStd / GlobalSettings.Instance.MagneticFieldsValueStd;
            n = 1 / (R_H * ElementaryCharge);
            GlobalSettings.Instance.BulkConcentration = n;
        }
        else
        {
            R_H = avgSlope / GlobalSettings.Instance.MagneticFieldsValueStd;
            n = 1 / (R_H * ElementaryCharge);
            GlobalSettings.Instance.SheetConcentration = n;
        }

        SemiconductorType type = SemiconductorType.Unknown;
        
        if (!double.IsNaN(GlobalSettings.Instance.HallCoefficient))
        {
            if (GlobalSettings.Instance.HallCoefficient > 0)
            {
                type = SemiconductorType.PType; // หาก RH > 0 หมายถึง P-type
            }
            else if (GlobalSettings.Instance.HallCoefficient < 0)
            {
                type = SemiconductorType.NType; // หาก RH < 0 หมายถึง N-type
            }
        }

        if (GlobalSettings.Instance.SheetResistance > 0 && !double.IsNaN(GlobalSettings.Instance.SheetConcentration))
            mu = 1 / (GlobalSettings.Instance.SheetResistance * GlobalSettings.Instance.SheetConcentration * ElementaryCharge);
        else if (GlobalSettings.Instance.Resistivity > 0 && !double.IsNaN(GlobalSettings.Instance.BulkConcentration))
            mu = 1 / (GlobalSettings.Instance.Resistivity * GlobalSettings.Instance.BulkConcentration * ElementaryCharge);
        else
            mu = double.NaN;

        GlobalSettings.Instance.HallCoefficient = R_H;
        GlobalSettings.Instance.Mobility = mu;
        GlobalSettings.Instance.SemiconductorType = type;

        OnHallPropertiesCalculated((R_H, GlobalSettings.Instance.SheetConcentration, GlobalSettings.Instance.BulkConcentration, mu));
        OnCalculationCompleted();
    }

    // Trigger Events
    private void OnDataUpdated(HallVoltageDataUpdatedEventArgs e) => DataUpdated?.Invoke(this, e);
    private void OnHallVoltageCalculated(Dictionary<int, double> voltages) => HallVoltageCalculated?.Invoke(this, voltages);
    private void OnHallIVHDataCalculated(Dictionary<int, List<(double Current, double HallVoltage)>> data) => HallIVHDataCalculated?.Invoke(this, data);
    private void OnHallPropertiesCalculated((double, double, double, double) props) => HallPropertiesCalculated?.Invoke(this, props);
    private void OnCalculationCompleted() => CalculationCompleted?.Invoke(this, EventArgs.Empty);
    protected virtual void OnSemiconductorTypeCalculated(SemiconductorType type)
    {
        SemiconductorTypeCalculated?.Invoke(this, type);
    }
}
