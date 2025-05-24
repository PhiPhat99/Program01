using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class HallVoltageDataUpdatedEventArgs : EventArgs
{
    // สำหรับ Chart แต่ละแบบ
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

    // เก็บข้อมูลการวัดสำหรับสถานะแม่เหล็กแต่ละแบบ
    private readonly Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> _measurements =
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
    public double ElementaryCharge { get; set; } = 1.602176634E-19;

    // Event
    public event EventHandler<HallVoltageDataUpdatedEventArgs> DataUpdated;
    public event EventHandler CalculationCompleted;
    public event EventHandler<Dictionary<int, double>> HallVoltageCalculated;
    public event EventHandler<Dictionary<int, List<(double Current, double HallVoltage)>>> HallIVHDataCalculated;
    public event EventHandler<(double HallCoefficient, double SheetConcentration, double BulkConcentration, double Mobility)> HallPropertiesCalculated;
    public event EventHandler<SemiconductorType> SemiconductorTypeCalculated;

    // *** นี่คือ Event ที่เพิ่มเข้ามา ***
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

        // Raise Event สำหรับกราฟแต่ละ Tab
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
        return denominator == 0 ? double.NaN : numerator / denominator;
    }

    public void CalculateHall()
    {
        ClearHallResults();
        var slopes = new Dictionary<int, double>();
        var hallVoltagesByPosition = new Dictionary<int, double>();
        var ivhSouthDataByPosition = new Dictionary<int, List<(double Current, double HallVoltage)>>();
        var ivhNorthDataByPosition = new Dictionary<int, List<(double Current, double HallVoltage)>>();

        double sumHallVoltage = 0;
        int validHallVoltageCount = 0;

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
            int validHallVoltageForPositionCount = 0;

            for (int i = 0; i < outData.Count; i++)
            {
                double current = outData[i].Source;
                double deltaVhSouth = i < southData.Count ? southData[i].Reading - outData[i].Reading : double.NaN;
                double deltaVhNorth = i < northData.Count ? northData[i].Reading - outData[i].Reading : double.NaN;

                if (!double.IsNaN(deltaVhSouth))
                {
                    southHall.Add((current, deltaVhSouth));
                    ivhSouthList.Add((current, deltaVhSouth));
                }
                if (!double.IsNaN(deltaVhNorth))
                {
                    northHall.Add((current, deltaVhNorth));
                    ivhNorthList.Add((current, deltaVhNorth));
                }

                double hallVoltage = !double.IsNaN(deltaVhSouth) && !double.IsNaN(deltaVhNorth)
                    ? (deltaVhSouth + deltaVhNorth) / 2
                    : !double.IsNaN(deltaVhSouth) ? deltaVhSouth : deltaVhNorth;

                if (!double.IsNaN(hallVoltage))
                {
                    currentHallVoltageSum += hallVoltage;
                    validHallVoltageForPositionCount++;
                }
            }

            double averageHallVoltage = validHallVoltageForPositionCount > 0 ? currentHallVoltageSum / validHallVoltageForPositionCount : double.NaN;
            hallVoltagesByPosition[pos] = averageHallVoltage;
            if (!double.IsNaN(averageHallVoltage))
            {
                sumHallVoltage += averageHallVoltage;
                validHallVoltageCount++;
            }

            double slopeS = CalculateSlope(southHall);
            double slopeN = CalculateSlope(northHall);
            slopes[pos] = (!double.IsNaN(slopeS) && !double.IsNaN(slopeN)) ? (slopeS + slopeN) / 2 : double.NaN;

            if (ivhSouthList.Count > 0) ivhSouthDataByPosition[pos] = ivhSouthList;
            if (ivhNorthList.Count > 0) ivhNorthDataByPosition[pos] = ivhNorthList;
        }

        lastMeasurements = _measurements.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToDictionary(
                innerKvp => innerKvp.Key,
                innerKvp => innerKvp.Value.ToList()
            )
        );
        Debug.WriteLine("[DEBUG - CollectAndCalculate] Hall calculation done, data stored.");

        HallIVHSouthData = ivhSouthDataByPosition;
        HallIVHNorthData = ivhNorthDataByPosition;
        GlobalSettings.Instance.HallVoltagesByPosition = hallVoltagesByPosition;
        OnHallVoltageCalculated(hallVoltagesByPosition);
        OnHallIVHSouthDataCalculated(ivhSouthDataByPosition);
        OnHallIVHNorthDataCalculated(ivhNorthDataByPosition);

        GlobalSettings.Instance.TotalHallVoltage = validHallVoltageCount > 0 ? sumHallVoltage / validHallVoltageCount : double.NaN;

        CalculateHallProperties(slopes);
        OnCalculationCompleted();
        Debug.WriteLine("[DEBUG - CollectAndCalculate] Triggering AllHallDataUpdated event.");
    }

    private void CalculateHallProperties(Dictionary<int, double> slopes)
    {
        var validSlopes = slopes.Values.Where(s => !double.IsNaN(s)).ToList();
        double avgSlope = validSlopes.Count > 0 ? validSlopes.Average() : double.NaN;

        double hallResistance = avgSlope;
        GlobalSettings.Instance.HallResistance = hallResistance;
        Debug.WriteLine($"[DEBUG] Hall Resistance (RH): {hallResistance} Ohm");

        double rHallCoefficient = CalculateHallCoefficient(avgSlope);
        double ns = CalculateSheetConcentration(avgSlope);
        double nb = CalculateBulkConcentration(rHallCoefficient);
        double mu = CalculateMobility(nb);
        var type = DetermineSemiconductorType(rHallCoefficient);

        OnHallPropertiesCalculated((rHallCoefficient, ns, nb, mu));
        OnSemiconductorTypeCalculated(type);
    }

    private double CalculateHallCoefficient(double avgSlope)
    {
        if (GlobalSettings.Instance.ThicknessValueStd == 0 || GlobalSettings.Instance.MagneticFieldsValueStd == 0)
            return double.NaN;

        double rHallCoefficient = GlobalSettings.Instance.TotalHallVoltage * GlobalSettings.Instance.ThicknessValueStd / GlobalSettings.Instance.StopValueStd * GlobalSettings.Instance.MagneticFieldsValueStd;
        GlobalSettings.Instance.HallCoefficient = rHallCoefficient;
        Debug.WriteLine($"[DEBUG] Hall Coefficient (RH): {rHallCoefficient} m^3/C");
        return rHallCoefficient;
    }

    private double CalculateSheetConcentration(double avgSlope)
    {
        if (avgSlope == 0 || ElementaryCharge == 0) return double.NaN;
        double ns = 1 / Math.Abs(avgSlope * ElementaryCharge);
        GlobalSettings.Instance.SheetConcentration = ns;
        Debug.WriteLine($"[DEBUG] Sheet Concentration (ns): {ns} 1/m^2");
        return ns;
    }

    private double CalculateBulkConcentration(double rHallCoefficient)
    {
        if (rHallCoefficient == 0 || ElementaryCharge == 0) return double.NaN;
        double nb = 1 / Math.Abs(rHallCoefficient * ElementaryCharge);
        GlobalSettings.Instance.BulkConcentration = nb;
        Debug.WriteLine($"[DEBUG] Bulk Concentration (nb): {nb} 1/m^3");
        return nb;
    }

    private double CalculateMobility(double nb)
    {
        if (GlobalSettings.Instance.Resistivity <= 0 || nb == 0 || ElementaryCharge == 0)
            return double.NaN;

        double mu = 1 / (GlobalSettings.Instance.Resistivity * nb * ElementaryCharge);
        GlobalSettings.Instance.Mobility = mu;
        Debug.WriteLine($"[DEBUG] Mobility (mu): {mu} m^2/Vs");
        return mu;
    }

    private SemiconductorType DetermineSemiconductorType(double rHallCoefficient)
    {
        if (double.IsNaN(rHallCoefficient)) return SemiconductorType.Unknown;
        var type = rHallCoefficient > 0 ? SemiconductorType.P : rHallCoefficient < 0 ? SemiconductorType.N : SemiconductorType.Unknown;
        GlobalSettings.Instance.SemiconductorType = type;
        Debug.WriteLine($"[DEBUG] Semiconductor Type: {type}");
        return type;
    }


    // Properties สำหรับค่าเฉลี่ย Voltage ในแต่ละสถานะและตำแหน่ง
    public Dictionary<int, double> GetAverageNoFieldVoltagesByPosition()
    {
        return _measurements[HallMeasurementState.NoMagneticField]
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average(item => item.Reading));
    }

    public Dictionary<int, double> GetAverageSouthFieldVoltagesByPosition()
    {
        return _measurements[HallMeasurementState.OutwardOrSouthMagneticField]
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average(item => item.Reading));
    }

    public Dictionary<int, double> GetAverageNorthFieldVoltagesByPosition()
    {
        return _measurements[HallMeasurementState.InwardOrNorthMagneticField]
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average(item => item.Reading));
    }

    // Trigger Events
    private void OnDataUpdated(HallVoltageDataUpdatedEventArgs e) => DataUpdated?.Invoke(this, e);
    private void OnHallVoltageCalculated(Dictionary<int, double> voltages) => HallVoltageCalculated?.Invoke(this, voltages);
    private void OnHallIVHSouthDataCalculated(Dictionary<int, List<(double Current, double HallVoltage)>> data) => HallIVHSouthDataCalculated?.Invoke(this, data);
    private void OnHallIVHNorthDataCalculated(Dictionary<int, List<(double Current, double HallVoltage)>> data) => HallIVHNorthDataCalculated?.Invoke(this, data); private void OnHallPropertiesCalculated((double, double, double, double) props) => HallPropertiesCalculated?.Invoke(this, props);
    private void OnCalculationCompleted() => CalculationCompleted?.Invoke(this, EventArgs.Empty);
    protected virtual void OnSemiconductorTypeCalculated(SemiconductorType type)
    {
        SemiconductorTypeCalculated?.Invoke(this, type);
    }
    protected virtual void OnAllHallDataUpdated(Dictionary<HallMeasurementState, Dictionary<int, List<(double Source, double Reading)>>> allData)
    {
        AllHallDataUpdated?.Invoke(this, allData);
        Debug.WriteLine("[DEBUG - CollectAndCalculate] AllHallDataUpdated event invoked.");
    }
}