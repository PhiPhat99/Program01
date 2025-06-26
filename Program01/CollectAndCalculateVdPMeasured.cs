using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CollectAndCalculateVdPMeasured
{
    private static CollectAndCalculateVdPMeasured _instance;
    private static readonly object _lock = new object();
    private readonly Dictionary<int, List<(double Source, double Reading)>> _measurements = new Dictionary<int, List<(double, double)>>();
    private readonly Dictionary<int, double> _resistances = new Dictionary<int, double>();

    public event EventHandler DataUpdated;
    public event EventHandler CalculationCompleted;

    private CollectAndCalculateVdPMeasured() { }

    public static CollectAndCalculateVdPMeasured Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ?? (_instance = new CollectAndCalculateVdPMeasured());
            }
        }
    }

    public void StoreMeasurementData(int tunerPosition, List<(double Source, double Reading)> data)
    {
        if (data == null || data.Count == 0)
        {
            return;
        }

        if (!_measurements.ContainsKey(tunerPosition))
        {
            _measurements[tunerPosition] = new List<(double, double)>();
        }

        _measurements[tunerPosition].AddRange(data);
        DataUpdated?.Invoke(this, EventArgs.Empty);
    }

    public Dictionary<int, List<(double Source, double Reading)>> GetAllMeasurementsByTuner()
    {
        return _measurements.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
    }

    public void ClearAllData()
    {
        _measurements.Clear();
        _resistances.Clear();
    }

    public void CalculateVanderPauw()
    {
        _resistances.Clear();
        Debug.WriteLine("[DEBUG] CalculateVanderPauw() called");

        for (int i = 1; i <= 8; i++)
        {
            if (!_measurements.ContainsKey(i))
            {
                _resistances[i] = double.NaN;
                continue;
            }

            var data = _measurements[i];
            double avgI = data.Average(d => d.Source);
            double avgV = data.Average(d => d.Reading);

            _resistances[i] = Math.Abs(avgI) > 1e-9 ? Math.Abs(avgV / avgI) : 0;
        }

        GlobalSettings.Instance.ResistancesByPosition = new Dictionary<int, double>(_resistances);

        GlobalSettings.Instance.ResistanceA = AveragePairs(new[] { 1, 2 }, new[] { 3, 4 });
        GlobalSettings.Instance.ResistanceB = AveragePairs(new[] { 5, 6 }, new[] { 7, 8 });
        GlobalSettings.Instance.AverageResistanceAll = _resistances.Values.Where(r => !double.IsNaN(r)).DefaultIfEmpty().Average();

        double Rs = SolveVanDerPauw(
            GlobalSettings.Instance.ResistanceA,
            GlobalSettings.Instance.ResistanceB,
            (GlobalSettings.Instance.ResistanceA + GlobalSettings.Instance.ResistanceB) / 2, 1E-6, 300
        );

        GlobalSettings.Instance.SheetResistance = double.IsNaN(Rs) ? double.NaN : Rs;
        GlobalSettings.Instance.Resistivity = double.IsNaN(Rs) ? double.NaN : Rs * GlobalSettings.Instance.ThicknessValueStd;
        GlobalSettings.Instance.Conductivity = !double.IsNaN(GlobalSettings.Instance.Resistivity) && GlobalSettings.Instance.Resistivity != 0
            ? 1 / GlobalSettings.Instance.Resistivity
            : double.NaN;

        CalculationCompleted?.Invoke(this, EventArgs.Empty);
    }

    private double AveragePairs(int[] groupA, int[] groupB)
    {
        var list = new List<double>();
        list.AddRange(groupA.Select(i => _resistances.ContainsKey(i) ? _resistances[i] : double.NaN).Where(r => !double.IsNaN(r)));
        list.AddRange(groupB.Select(i => _resistances.ContainsKey(i) ? _resistances[i] : double.NaN).Where(r => !double.IsNaN(r)));
        return list.Count > 0 ? list.Average() : double.NaN;
    }

    private static double F(double Rs, double Ra, double Rb)
    {
        return Math.Exp(-Math.PI * Ra / Rs) + Math.Exp(-Math.PI * Rb / Rs) - 1.0;
    }

    private static double FPrime(double Rs, double Ra, double Rb)
    {
        double a = Math.PI * Ra / (Rs * Rs);
        double b = Math.PI * Rb / (Rs * Rs);
        return a * Math.Exp(-Math.PI * Ra / Rs) + b * Math.Exp(-Math.PI * Rb / Rs);
    }

    private static double SolveVanDerPauw(double Ra, double Rb, double RsInitial, double tolerance, int maxIter)
    {
        double Rs = RsInitial;
        for (int i = 0; i < maxIter; i++)
        {
            double f = F(Rs, Ra, Rb);
            double fPrime = FPrime(Rs, Ra, Rb);

            if (Math.Abs(fPrime) < 1E-12) return double.NaN;

            double nextRs = Rs - f / fPrime;
            if (Math.Abs(nextRs - Rs) < tolerance)
                return nextRs;

            Rs = nextRs;
        }
        return double.NaN;
    }
}