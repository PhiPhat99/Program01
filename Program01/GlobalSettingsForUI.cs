using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class GlobalSettingsForUI
{
    private static GlobalSettingsForUI _instance;
    private static readonly object _lock = new object();

    public static GlobalSettingsForUI Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalSettingsForUI();
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
        set => SetProperty(ref _startValueUI, value);

    }

    private double _stopValueUI = 0;
    public double StopValueUI
    {
        get => _stopValueUI;
        set => SetProperty(ref _stopValueUI, value);

    }

    private double _stepValueUI = 0;
    public double StepValueUI
    {
        get => _stepValueUI;
        set => SetProperty(ref _stepValueUI, value);

    }

    private double _sourceDelayValueUI = 0;
    public double SourceDelayValueUI
    {
        get => _sourceDelayValueUI;
        set => SetProperty(ref _sourceDelayValueUI, value);

    }

    private double _sourceLimitLevelValueUI = 0;
    public double SourceLimitLevelValueUI
    {
        get => _sourceLimitLevelValueUI;
        set => SetProperty(ref _sourceLimitLevelValueUI, value);

    }

    private double _thicknessValueUI = 0;
    public double ThicknessValueUI
    {
        get => _thicknessValueUI;
        set => SetProperty(ref _thicknessValueUI, value);

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
        set => SetProperty(ref _magneticFieldsValueUI, value);

    }

    public string StartUnitUI { get; set; } = "";
    public string StopUnitUI { get; set; } = "";
    public string StepUnitUI { get; set; } = "";
    public string SourceDelayUnitUI { get; set; } = "";
    public string SourceLimitLevelUnitUI { get; set; } = "";
    public string ThicknessUnitUI { get; set; } = "";
    public string MagneticFieldsUnitUI { get; set; } = "";

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