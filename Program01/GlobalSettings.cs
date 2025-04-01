using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Program01
{
    public class GlobalSettings
    {
        private static GlobalSettings _instance;
        private static readonly object _lock = new object();

        private GlobalSettings() { }

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

        private string _rsenseMode;
        public string RsenseMode
        {
            get => _rsenseMode;
            set
            {
                if (_rsenseMode != value)
                {
                    _rsenseMode = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Sense-Wires Mode updated to: {_rsenseMode}");

                }
            }
        }

        private string _measureMode;
        public string MeasureMode
        {
            get => _measureMode;
            set
            {
                if (_measureMode != value)
                {
                    _measureMode = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Measure Mode updated to: {_measureMode}");
                }
            }
        }

        private string _sourceMode;
        public string SourceMode
        {
            get => _sourceMode;
            set
            {
                if (_sourceMode != value)
                {
                    _sourceMode = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Source Mode updated to: {_sourceMode}");
                }
            }
        }

        private string _sourceLimitType;
        public string SourceLimitType
        {
            get => _sourceLimitType;
            set
            {
                if (_sourceLimitType != value)
                {
                    _sourceLimitType = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Source Limit Mode updated to: {_sourceLimitType}");
                }
            }
        }

        private string _startValue;
        public string StartValue
        {
            get => _startValue;
            set
            {
                if (_startValue != value)
                {
                    _startValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Start Value updated to: {_startValue}");
                }
            }
        }

        private string _stopValue;
        public string StopValue
        {
            get => _stopValue;
            set
            {
                if (_stopValue != value)
                {
                    _stopValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Stop Value updated to: {_stopValue}");

                }
            }
        }

        private string _stepValue;
        public string StepValue
        {
            get => _stepValue;
            set
            {
                if (_stepValue != value)
                {
                    _stepValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Step Value updated to: {_stepValue}");
                }
            }
        }

        private string _sourceDelayValue;
        public string SourceDelayValue
        {
            get => _sourceDelayValue;
            set
            {
                if (_sourceDelayValue != value)
                {
                    _sourceDelayValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Source Delay Value updated to: {_sourceDelayValue}");

                }
            }
        }

        private string _sourceLimitLevelValue;
        public string SourceLimitLevelValue
        {
            get => _sourceLimitLevelValue;
            set
            {
                if (_sourceLimitLevelValue != value)
                {
                    _sourceLimitLevelValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Source Limit Level Value updated to: {_sourceLimitLevelValue}");

                }
            }
        }

        private string _thicknessValue;
        public string ThicknessValue
        {
            get => _thicknessValue;
            set
            {
                if (_thicknessValue != value)
                {
                    _thicknessValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Thickness Value updated to: {_thicknessValue}");

                }
            }
        }

        private string _repetitionValue;
        public string RepetitionValue
        {
            get => _repetitionValue;
            set
            {
                if (_repetitionValue != value)
                {
                    _repetitionValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Repetition Value updated to: {_repetitionValue}");
                }
            }
        }

        private string _magneticFieldsValue;
        public string MagneticFieldsValue
        {
            get => _magneticFieldsValue;
            set
            {
                if (_magneticFieldsValue != value)
                {
                    _magneticFieldsValue = value;
                    OnSettingsChanged?.Invoke();
                    Debug.WriteLine($"[DEBUG] Magnetic Fields Value updated to: {_magneticFieldsValue}");

                }
            }
        }

        public string StartUnit { get; set; }
        public string StopUnit { get; set; }
        public string StepUnit { get; set; }
        public string SourceDelayUnit { get; set; }
        public string SourceLimitLevelUnit { get; set; }
        public string ThicknessUnit { get; set; }
        public string MagneticFieldsUnit { get; set; }

        public List<double> XDataBuffer { get; private set; } = new List<double>();
        public List<double> YDataBuffer { get; private set; } = new List<double>();

        public double MaxMeasure { get; private set; } = double.NegativeInfinity;
        public double MinMeasure { get; private set; } = double.PositiveInfinity;
        public double MaxSource { get; private set; } = double.NegativeInfinity;
        public double MinSource { get; private set; } = double.PositiveInfinity;
        public double Slope { get; private set; } = double.NaN;

        public void UpdateDataBuffer(List<double> Xdata, List<double> Ydata, double maxMeasure, double minMeasure, double maxSource, double minSource, double slope)
        {
            XDataBuffer = new List<double>(Xdata);
            YDataBuffer = new List<double>(Ydata);
            MaxMeasure = maxMeasure;
            MinMeasure = minMeasure;
            MaxSource = maxSource;
            MinSource = minSource;
            Slope = slope;

            OnSettingsChanged?.Invoke();
        }
    }
}
