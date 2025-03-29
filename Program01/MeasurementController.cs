using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program01
{
    public class MeasurementController
    {
        private CollectVdPMeasuredValue _dataCollector;

        public MeasurementController(CollectVdPMeasuredValue dataCollector)
        {
            _dataCollector = dataCollector;
        }

        public void TracingRunMeasurement(List<double> XData, List<double> YData)
        {
            // แปลงข้อมูลการวัดเป็นรูปแบบที่ CollectVdPMeasuredValue สามารถจัดการได้
            List<(string Source, double Reading)> dataPairs = new List<(string, double)>();

            for (int i = 0; i < XData.Count; i++)
            {
                dataPairs.Add((XData[i].ToString(), YData[i])); // ส่งข้อมูลในรูปแบบที่ CollectVdPMeasuredValue ใช้
            }

            // ส่งข้อมูลไปยัง CollectVdPMeasuredValue เพื่อเก็บข้อมูล
            _dataCollector.StoreMeasurementData(dataPairs);

            // แจ้งให้ฟอร์ม VdPTotalMeasureValuesForm อัปเดตข้อมูล
            // คุณสามารถเพิ่มการเรียก UpdateData() ของฟอร์ม VdPTotalMeasureValuesForm ที่นี่ได้
        }
    }
}
