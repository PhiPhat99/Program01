using Program01;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private string _rsenseModeUI = "4-Wires";
    public string RsenseModeUI
    {
        get => _rsenseModeUI;
        set => SetProperty(ref _rsenseModeUI, value);
    }

    private string _measureModeUI = "Voltage";
    public string MeasureModeUI
    {
        get => _measureModeUI;
        set => SetProperty(ref _measureModeUI, value);
    }

    private string _sourceModeUI = "Current";
    public string SourceModeUI
    {
        get => _sourceModeUI;
        set => SetProperty(ref _sourceModeUI, value);
    }

    private string _sourceLimitTypeUI = "Voltage";
    public string SourceLimitTypeUI
    {
        get => _sourceLimitTypeUI;
        set => SetProperty(ref _sourceLimitTypeUI, value);
    }

    private double _startValueUI = 0;
    public double StartValueUI
    {
        get => _startValueUI;
        set => SetProperty(ref _startValueUI, value);      
    }

    private string _startUnitUI = "mA";
    public string StartUnitUI
    {
        get => _startUnitUI;
        set => SetProperty(ref _startUnitUI, value);
    }

    private double _stopValueUI = 0;
    public double StopValueUI
    {
        get => _stopValueUI;
        set => SetProperty(ref _startValueUI, value);
    }

    private string _stopUnitUI = "mA";
    public string StopUnitUI
    {
        get => _stopUnitUI;
        set => SetProperty(ref _stopUnitUI, value);
    }

    private double _stepValueUI = 0;
    public double StepValueUI
    {
        get => _stepValueUI;
        set => SetProperty(ref _stepValueUI, value);
    }

    private string _stepUnitUI = "µA";
    public string StepUnitUI
    {
        get => _stepUnitUI;
        set => SetProperty(ref _stepUnitUI, value);
    }

    private double _sourceDelayValueUI = 100;
    public double SourceDelayValueUI
    {
        get => _sourceDelayValueUI;
        set => SetProperty(ref _sourceDelayValueUI, value);
    }

    private string _sourcedelayUnitUI = "µs";
    public string SourceDelayUnitUI
    {
        get => _sourcedelayUnitUI;
        set => SetProperty(ref _sourcedelayUnitUI, value);
    }

    private double _sourceLimitLevelValueUI = 21;
    public double SourceLimitLevelValueUI
    {
        get => _sourceLimitLevelValueUI;
        set => SetProperty(ref _sourceLimitLevelValueUI, value);
    }

    private string _sourcelimitlevelUnitUI = "V";
    public string SourceLimitLevelUnitUI
    {
        get => _sourcelimitlevelUnitUI;
        set => SetProperty(ref _sourcelimitlevelUnitUI, value);
    }

    private double _thicknessValueUI = 0;
    public double ThicknessValueUI
    {
        get => _thicknessValueUI;
        set => SetProperty(ref _thicknessValueUI, value);
    }

    private string _thicknessUnitUI = "m";
    public string ThicknessUnitUI
    {
        get => _thicknessUnitUI;
        set => SetProperty(ref _thicknessUnitUI, value);
    }

    private int _repetitionValueUI = 1;
    public int RepetitionValueUI
    {
        get => _repetitionValueUI;
        set => SetProperty(ref _repetitionValueUI, value);
    }

    private double _magneticFieldsValueUI = 0.55;
    public double MagneticFieldsValueUI
    {
        get => _magneticFieldsValueUI;
        set => SetProperty(ref _magneticFieldsValueUI, value);
    }

    private string _magneticFieldsUnitUI = "T";
    public string MagneticFieldsUnitUI
    {
        get => _magneticFieldsUnitUI;
        set => SetProperty(ref _magneticFieldsUnitUI, value);
    }

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

    private double _startStd = 0;
    public double StartValueStd
    {
        get => _startStd;
        set => _startStd = value;
    }

    private double _stopStd = 0;
    public double StopValueStd
    {
        get => _stopStd;
        set => _stopStd = value;
    }

    private double _stepStd = 0;
    public double StepValueStd
    {
        get => _stepStd;
        set => _stepStd = value;
    }

    private double _sourcedelayStd = 0;
    public double SourceDelayValueStd
    {
        get => _sourcedelayStd;
        set => _sourcedelayStd = value;
    }

    private double _sourcelimitlevelStd = 0;
    public double SourceLimitLevelValueStd
    {
        get => _sourcelimitlevelStd;
        set => _sourcelimitlevelStd = value;
    }

    private double _thicknessStd = 0;
    public double ThicknessValueStd
    {
        get => _thicknessStd;
        set => _thicknessStd = value;
    }

    private double _magneticfieldsStd = 0;
    public double MagneticFieldsValueStd
    {
        get => _magneticfieldsStd;
        set => _magneticfieldsStd = value;
    }

    public int Repetition { get; set; }
    public Dictionary<int, double> ResistancesByPosition { get; set; }
    public double ResistanceA { get; set; }
    public double ResistanceB { get; set; }
    public double AverageResistanceAll { get; set; }
    public double SheetResistance { get; set; }
    public double Resistivity { get; set; }
    public double Conductivity { get; set; }

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
