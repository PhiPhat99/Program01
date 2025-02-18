using System;
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
    public partial class VdPTotalVoltageChildForm : Form
    {
        //Fields
        private DataTable RowsDataTable1;

        public VdPTotalVoltageChildForm()
        {
            InitializeComponent();
            InitializeDataGridViewAndRowsTable();
            UpdatingResults();
        }

        private void InitializeDataGridViewAndRowsTable()
        {

            DataGridViewVdPResult1.DataSource = RowsDataTable1;

        }

        private async void UpdatingResults()
        {
            Random random = new Random();
            int Running = 0;
            int Repeat = 5;

            if (RowsDataTable1 != null && RowsDataTable1.Rows.Count > 0)
            {
                while (Running <= Repeat)
                {
                    for (int i = 0; i < RowsDataTable1.Rows.Count; i++)
                    {
                        double R1 = random.NextDouble() * 10;

                        if (R1 > 8)
                        {
                            R1 = 0;
                        }

                        RowsDataTable1.Rows[i]["R1"] = R1;
                    }

                    await Task.Delay(1000);
                }
            }
            else
            {
                MessageBox.Show("Error: RowsDataTable is null or has no rows!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
