using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Program01
{
    public class CollectVdPMeasuredValue
    {
        private List<(string Source, double Reading)> _measurements = new List<(string, double)>();

        // เก็บข้อมูลการวัด
        public void StoreMeasurementData(List<(string, double)> dataPairs)
        {
            _measurements.AddRange(dataPairs);
        }

        // ดึงข้อมูลทั้งหมดที่เก็บ
        public List<(string Source, double Reading)> GetData()
        {
            return _measurements;
        }

        // ดึงข้อมูลที่ใช้สำหรับการแสดงกราฟ IV Curve ตามแท็บ
        public List<(double Source, double Reading)> GetIVCurveData(int tabIndex)
        {
            // ในที่นี้ tabIndex ใช้แยกข้อมูลสำหรับการแสดงผลกราฟ
            // สำหรับตัวอย่างนี้จะใช้ทั้งหมดใน _measurements เดิม
            List<(double Source, double Reading)> ivCurveData = new List<(double, double)>();

            foreach (var measurement in _measurements)
            {
                if (double.TryParse(measurement.Source, out double source))
                {
                    ivCurveData.Add((source, measurement.Reading));
                }
            }

            return ivCurveData;
        }
    }
}

