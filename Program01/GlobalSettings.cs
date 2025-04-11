using Program01;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class GlobalSettings
{
    private static GlobalSettings _instance;
    private static readonly object _lock = new object();

    public CollectAndCalculateVdPMeasured CollectedMeasurements { get; private set; } = CollectAndCalculateVdPMeasured.Instance;

    public static GlobalSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalSettings();
                    }
                }
            }
            return _instance;
        }
    }

    public event Action OnSettingsChanged;

    private bool _isSMUConnected;
    public bool IsSMUConnected
    {
        get => _isSMUConnected;
        set
        {
            if (_isSMUConnected != value)
            {
                _isSMUConnected = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private bool _isSSConnected;
    public bool IsSSConnected
    {
        get => _isSSConnected;
        set
        {
            if (_isSSConnected != value)
            {
                _isSSConnected = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private bool _isModes;
    public bool IsModes
    {
        get => _isModes;
        set
        {
            if (_isModes != value)
            {
                _isModes = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private bool _isHallMode;
    public bool IsHallMode
    {
        get => _isHallMode;
        set
        {
            if (_isHallMode != value)
            {
                _isHallMode = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private bool _isVanDerPauwMode;
    public bool IsVanDerPauwMode
    {
        get => _isVanDerPauwMode;
        set
        {
            if (_isVanDerPauwMode != value)
            {
                _isVanDerPauwMode = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private bool _isRun;
    public bool IsRun
    {
        get => _isRun;
        set
        {
            if (_isRun != value)
            {
                _isRun = value;
                OnSettingsChanged?.Invoke();
            }
        }
    }

    private string _rsenseMode = "";
    public string RsenseMode
    {
        get => _rsenseMode;
        set => SetProperty(ref _rsenseMode, value);
    }

    private string _measureMode = "";
    public string MeasureMode
    {
        get => _measureMode;
        set => SetProperty(ref _measureMode, value);
    }

    private string _sourceMode = "";
    public string SourceMode
    {
        get => _sourceMode;
        set => SetProperty(ref _sourceMode, value);

    }

    private string _sourceLimitType = "";
    public string SourceLimitType
    {
        get => _sourceLimitType;
        set => SetProperty(ref _sourceLimitType, value);

    }

    private double _startValue = 0;
    public double StartValue
    {
        get => _startValue;
        set => SetProperty(ref _startValue, value);

    }

    private double _stopValue = 0;
    public double StopValue
    {
        get => _stopValue;
        set => SetProperty(ref _stopValue, value);

    }

    private double _stepValue = 0;
    public double StepValue
    {
        get => _stepValue;
        set => SetProperty(ref _stepValue, value);

    }

    private double _sourceDelayValue = 0;
    public double SourceDelayValue
    {
        get => _sourceDelayValue;
        set => SetProperty(ref _sourceDelayValue, value);

    }

    private double _sourceLimitLevelValue = 0;
    public double SourceLimitLevelValue
    {
        get => _sourceLimitLevelValue;
        set => SetProperty(ref _sourceLimitLevelValue, value);

    }

    private double _thicknessValue = 0;
    public double ThicknessValue
    {
        get => _thicknessValue;
        set => SetProperty(ref _thicknessValue, value);

    }

    private int _repetitionValue = 1;
    public int RepetitionValue
    {
        get => _repetitionValue;
        set => SetProperty(ref _repetitionValue, value);

    }

    private double _magneticFieldsValue = 0;
    public double MagneticFieldsValue
    {
        get => _magneticFieldsValue;
        set => SetProperty(ref _magneticFieldsValue, value);

    }

    public string StartUnit { get; set; } = "";
    public string StopUnit { get; set; } = "";
    public string StepUnit { get; set; } = "";
    public string SourceDelayUnit { get; set; } = "";
    public string SourceLimitLevelUnit { get; set; } = "";
    public string ThicknessUnit { get; set; } = "";
    public string MagneticFieldsUnit { get; set; } = "";

    private readonly List<List<double[]>> _allMeasuredValues = new List<List<double[]>>();

    public List<List<double[]>> AllMeasuredValues => _allMeasuredValues;

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

    public Dictionary<int, double> ResistancesByPosition { get; set; }
    public double ResistanceA { get; set; }
    public double ResistanceB { get; set; }
    public double AverageResistanceAll { get; set; }
    public double SheetResistance {  get; set; }

    private GlobalSettings()
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