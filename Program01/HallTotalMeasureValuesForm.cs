﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Program01
{
    public partial class HallTotalMeasureValuesForm : Form
    {
        public HallTotalMeasureValuesForm()
        {
            InitializeComponent();
        }

        /*private void LoadHallTotalMeasurementData()
        {
            var HallData = CollectHallMeasuredValue.Instance;
            DatagridviewHallTotalMesure.Rows.Clear();

            if (DatagridviewVdPTotalMesure.Columns.Count == 0)
            {
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure1", "TunerMeasure1");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure2", "TunerMeasure2");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure3", "TunerMeasure3");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure4", "TunerMeasure4");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure5", "TunerMeasure5");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure6", "TunerMeasure6");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure7", "TunerMeasure7");
                DatagridviewVdPTotalMesure.Columns.Add("TunerMeasure8", "TunerMeasure8");
            }

            for (int i = 0; i < HallData.Measured.Count; i++)
            {
                int RowsIndex = DatagridviewHallTotalMesure.Rows.Add();

                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure1"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure2"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure3"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure4"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure5"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure6"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure7"].Value = VdPData.Voltages[i];
                DatagridviewVdPTotalMesure.Rows[RowsIndex].Cells["TunerMeasure8"].Value = VdPData.Voltages[i];
            }
        }

        private void HallTotalMeasureValuesForm_Load(object sender, EventArgs e)
        {
            LoadHallTotalMeasurementData();
        }

        private void DatagridviewHallTotalMesure_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (DatagridviewhallTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure1" || DatagridviewHallTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure2" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure3" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure4" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure5" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure6" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure7" || DatagridviewVdPTotalMesure.Columns[e.ColumnIndex].Name == "TunerMeasure8")
            {
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double result))
                {
                    e.Value = result.ToString("F5");
                }
            }
        }*/
    }
}
