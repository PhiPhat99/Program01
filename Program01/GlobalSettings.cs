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

    private string _rsenseModeUI = "";
    public string RsenseModeUI
    {
        get => _rsenseModeUI;
        set => SetProperty(ref _rsenseModeUI, value);
    }

    private string _measureModeUI = "";
    public string MeasureModeUI
    {
        get => _measureModeUI;
        set => SetProperty(ref _measureModeUI, value);
    }

    private string _sourceModeUI = "";
    public string SourceModeUI
    {
        get => _sourceModeUI;
        set => SetProperty(ref _sourceModeUI, value);
    }

    private string _sourceLimitTypeUI = "";
    public string SourceLimitTypeUI
    {
        get => _sourceLimitTypeUI;
        set => SetProperty(ref _sourceLimitTypeUI, value);
    }

    private double _startValueUI = 0;
    public double StartValueUI
    {
        get => _startValueUI;
        set
        {
            if (SetProperty(ref _startValueUI, value))
            {
                StartValueStd = ConvertStartValueToStandardUnit();
            }
        }
    }

    private double _stopValueUI = 0;
    public double StopValueUI
    {
        get => _stopValueUI;
        set
        {
            if (SetProperty(ref _stopValueUI, value))
            {
                StopValueStd = ConvertStopValueToStandardUnit();
            }
        }
    }

    private double _stepValueUI = 0;
    public double StepValueUI
    {
        get => _stepValueUI;
        set
        {
            if (SetProperty(ref _stepValueUI, value))
            {
                StepValueStd = ConvertStepValueToStandardUnit();
            }
        }
    }

    private double _sourceDelayValueUI = 0;
    public double SourceDelayValueUI
    {
        get => _sourceDelayValueUI;
        set
        {
            if (SetProperty(ref _sourceDelayValueUI, value))
            {
                SourceDelayValueStd = ConvertSourceDelayValueToStandardUnit();
            }
        }
    }

    private double _sourceLimitLevelValueUI = 0;
    public double SourceLimitLevelValueUI
    {
        get => _sourceLimitLevelValueUI;
        set
        {
            if (SetProperty(ref _sourceLimitLevelValueUI, value))
            {
                SourceLimitLevelStd = ConvertSourceLimitLevelValueToStandardUnit();
            }
        }
    }

    private double _thicknessValueUI = 0;
    public double ThicknessValueUI
    {
        get => _thicknessValueUI;
        set
        {
            if (SetProperty(ref _thicknessValueUI, value))
            {
                ThicknessValueStd = ConvertThicknessValueToStandardUnit();
            }
        }
    }

    private int _repetitionValueUI = 1;
    public int RepetitionValueUI
    {
        get => _repetitionValueUI;
        set => SetProperty(ref _repetitionValueUI, value);
    }

    private double _magneticFieldsValueUI = 0;
    public double MagneticFieldsValueUI
    {
        get => _magneticFieldsValueUI;
        set
        {
            if (SetProperty(ref _magneticFieldsValueUI, value))
            {
                MagneticFieldsStd = ConvertMagneticFieldsValueToStandardUnit();
            }
        }
    }

    public string StartUnitUI { get; set; } = "";
    public string StopUnitUI { get; set; } = "";
    public string StepUnitUI { get; set; } = "";
    public string SourceDelayUnitUI { get; set; } = "";
    public string SourceLimitLevelUnitUI { get; set; } = "";
    public string ThicknessUnitUI { get; set; } = "";
    public string MagneticFieldsUnitUI { get; set; } = "";


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
    public double SourceDelayValueStd { get; set; }
    public double SourceLimitLevelStd { get; set; }
    public double ThicknessValueStd { get; set; }
    public double MagneticFieldsStd { get; set; }
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

    public double ConvertStartValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Start Value: {StartValueUI} {StartUnitUI}, Source Mode: {SourceModeUI}");

        if (GlobalSettings.Instance.SourceModeUI == "Current")
        {
            switch (GlobalSettings.Instance.StartUnitUI)
            {
                case "nA":
                    return _startValueUI / 1E+9;
                case "µA":
                    return _startValueUI / 1E+6;
                case "mA":
                    return _startValueUI / 1E+3;
                case "A":
                    return _startValueUI;
            }

            Debug.WriteLine($"After Convert Start Value (Standard): {StartValueStd} A");
        }
        else if (GlobalSettings.Instance.SourceModeUI == "Voltage")
        {
            switch (GlobalSettings.Instance.StartUnitUI)
            {
                case "mV":
                    return _startValueUI / 1E+3;
                case "V":
                    return _startValueUI;
            }

            Debug.WriteLine($"After Convert Start Value (Standard): {StartValueStd} V");
        }

        return _startValueUI;
    }

    public double ConvertStepValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Step Value: {StepValueUI} {StepUnitUI}, Source Mode: {SourceModeUI}");

        if (GlobalSettings.Instance.SourceModeUI == "Current")
        {
            switch (GlobalSettings.Instance.StepUnitUI)
            {
                case "nA":
                    return _stepValueUI / 1E+9;
                case "µA":
                    return _stepValueUI / 1E+6;
                case "mA":
                    return _stepValueUI / 1E+3;
                case "A":
                    return _stepValueUI;
            }

            Debug.WriteLine($"After Convert Step Value (Standard): {StepValueStd} A");
        }
        else if (GlobalSettings.Instance.SourceModeUI == "Voltage")
        {
            switch (GlobalSettings.Instance.StepUnitUI)
            {
                case "mV":
                    return _stepValueUI / 1E+3;
                case "V":
                    return _stepValueUI;
            }

            Debug.WriteLine($"After Convert Step Value (Standard): {StepValueStd} V");
        }

        return _stepValueUI;
    }

    public double ConvertStopValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Stop Value: {StopValueUI} {StopUnitUI}, Source Mode: {SourceModeUI}");
        
        if (GlobalSettings.Instance.SourceModeUI == "Current")
        {
            switch (GlobalSettings.Instance.StopUnitUI)
            {
                case "nA":
                    return _stopValueUI / 1E+9;
                case "µA":
                    return _stopValueUI / 1E+6;
                case "mA":
                    return _stopValueUI / 1E+3;
                case "A":
                    return _stopValueUI;
            }

            Debug.WriteLine($"After Convert Stop Value (Standard): {StopValueStd} A");
        }
        else if (GlobalSettings.Instance.SourceModeUI == "Voltage")
        {
            switch (GlobalSettings.Instance.StopUnitUI)
            {
                case "mV":
                    return _stopValueUI / 1E+3;
                case "V":
                    return _stopValueUI;
            }

            Debug.WriteLine($"After Convert Stop Value (Standard): {StopValueStd} V");
        }

        return _stopValueUI;
    }

    public double ConvertSourceDelayValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Source Delay Value: {SourceDelayValueUI} {SourceDelayUnitUI}");
        switch (GlobalSettings.Instance.SourceDelayUnitUI)
            {
                case "µs":
                    return _sourceDelayValueUI / 1E+6;
                case "ms":
                    return _sourceDelayValueUI / 1E+3;
                case "s":
                    return _sourceDelayValueUI;
                case "ks":
                    return _sourceDelayValueUI * 1E+3;
        }

        Debug.WriteLine($"After Convert Source Delay Value (Standard): {SourceDelayValueStd} s");
        return _sourceDelayValueUI;
    }

    public double ConvertSourceLimitLevelValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Source Limit Level Value: {SourceLimitLevelValueUI} {SourceLimitLevelUnitUI}, Source Mode: {SourceLimitTypeUI}");

        if (GlobalSettings.Instance.SourceLimitTypeUI == "Current")
        {
            switch (GlobalSettings.Instance.SourceLimitLevelUnitUI)
            {
                case "nA":
                    return _sourceLimitLevelValueUI / 1E+9;
                case "µA":
                    return _sourceLimitLevelValueUI / 1E+6;
                case "mA":
                    return _sourceLimitLevelValueUI / 1E+3;
                case "A":
                    return _sourceLimitLevelValueUI;
            }

            Debug.WriteLine($"After Convert Source Limit Level Value (Standard): {SourceLimitLevelStd} A");
        }
        else if (GlobalSettings.Instance.SourceLimitTypeUI == "Voltage")
        {
            switch (GlobalSettings.Instance.SourceLimitLevelUnitUI)
            {
                case "mV":
                    return _sourceLimitLevelValueUI / 1E+3;
                case "V":
                    return _sourceLimitLevelValueUI;
            }

            Debug.WriteLine($"After Convert Source Limit Level Value (Standard): {SourceLimitLevelStd} V");
        }

        return _sourceLimitLevelValueUI;
    }

    public double ConvertThicknessValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Thickness Value: {ThicknessValueUI} {ThicknessUnitUI}");

        switch (GlobalSettings.Instance.ThicknessUnitUI)
            {
                case "nm":
                    return _thicknessValueUI / 1E+9;
                case "µm":
                    return _thicknessValueUI / 1E+6;
                case "mm":
                    return _thicknessValueUI / 1E+3;
                case "cm":
                    return _thicknessValueUI / 1E+2;
                case "m":
                return _thicknessValueUI;
        }

        Debug.WriteLine($"After Convert Thickness Value (Standard): {ThicknessValueStd} m");
        return _thicknessValueUI;
    }

    public double ConvertMagneticFieldsValueToStandardUnit()
    {
        Debug.WriteLine($"Before Convert Magnetic Fields Value: {MagneticFieldsValueUI} {MagneticFieldsUnitUI}");

        switch (GlobalSettings.Instance.MagneticFieldsUnitUI)
        {
            case "G":
                return _magneticFieldsValueUI / 1E+4;
            case "T":
                return _magneticFieldsValueUI;
        }

        Debug.WriteLine($"After Convert Magnetic Fields Value (Standard): {MagneticFieldsStd} m");
        return _magneticFieldsValueUI;
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
