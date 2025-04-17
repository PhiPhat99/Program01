using Program01;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class GlobalSettingsParseValues
{
    private static GlobalSettingsParseValues _instance;
    private static readonly object _lock = new object();

    public CollectAndCalculateVdPMeasured CollectedMeasurements { get; private set; } = CollectAndCalculateVdPMeasured.Instance;

    public static GlobalSettingsParseValues Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalSettingsParseValues();
                    }
                }
            }

            return _instance;
        }
    }

    public event Action OnSettingsChanged;

    public readonly List<List<double[]>> _allMeasuredValues = new List<List<double[]>>();

    private readonly List<double> _xDataBuffer = new List<double>();
    public List<double> XDataBuffer => _xDataBuffer;

    private readonly List<double> _yDataBuffer = new List<double>();
    public List<double> YDataBuffer => _yDataBuffer;

    private double _maxMeasure = double.NegativeInfinity;
    public double MaxMeasure => _maxMeasure;

    private double _minMeasure = double.PositiveInfinity;
    public double MinMeasure => _minMeasure;

    private double _maxSource = double.NegativeInfinity;
    public double MaxSource => _maxSource;

    private double _minSource = double.PositiveInfinity;
    public double MinSource => _minSource;

    private double _slope = double.NaN;
    public double Slope => _slope;

    public void UpdateDataBuffer(List<double> Xdata, List<double> Ydata, double maxMeasure, double minMeasure, double maxSource, double minSource, double slope)
    {
        _xDataBuffer.Clear();
        _xDataBuffer.AddRange(Xdata);
        _yDataBuffer.Clear();
        _yDataBuffer.AddRange(Ydata);
        _maxMeasure = maxMeasure;
        _minMeasure = minMeasure;
        _maxSource = maxSource;
        _minSource = minSource;
        _slope = slope;

        OnSettingsChanged?.Invoke();
    }

    public double StartValueStd { get; set; }
    public double StopValueStd { get; set; }
    public double StepValueStd { get; set; }
    public double SourceDelayStd { get; set; }
    public double SourceLimitLevelStd { get; set; }
    public double ThicknessStd { get; set; }
    public int Repetition { get; set; }
    public double MagneticFieldsStd { get; set; }
    public Dictionary<int, double> ResistancesByPosition { get; set; }
    public double ResistanceA { get; set; }
    public double ResistanceB { get; set; }
    public double AverageResistanceAll { get; set; }
    public double SheetResistance { get; set; }
    public double Resistivity { get; set; }
    public double Conductivity { get; set; }

    private GlobalSettingsParseValues()
    {
        _allMeasuredValues = new List<List<double[]>>();
    }

    public void AddMeasuredValues(List<double[]> values, int position)
    {
        while (_allMeasuredValues.Count < position)
        {
            _allMeasuredValues.Add(null);
        }
        _allMeasuredValues[position - 1] = values;
    }

    

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string PropertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnSettingsChanged?.Invoke();
        return true;
    }
}
