namespace Program01
{
    partial class VdPTotalVoltageChildForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DataGridViewVdPResult1 = new System.Windows.Forms.DataGridView();
            this.ColumnTunerResult1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewVdPResult1)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewVdPResult1
            // 
            this.DataGridViewVdPResult1.AllowUserToResizeColumns = false;
            this.DataGridViewVdPResult1.AllowUserToResizeRows = false;
            this.DataGridViewVdPResult1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewVdPResult1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTunerResult1});
            this.DataGridViewVdPResult1.Location = new System.Drawing.Point(500, 55);
            this.DataGridViewVdPResult1.Name = "DataGridViewVdPResult1";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewVdPResult1.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridViewVdPResult1.RowHeadersVisible = false;
            this.DataGridViewVdPResult1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.DataGridViewVdPResult1.RowTemplate.Height = 24;
            this.DataGridViewVdPResult1.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.DataGridViewVdPResult1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridViewVdPResult1.Size = new System.Drawing.Size(700, 700);
            this.DataGridViewVdPResult1.TabIndex = 0;
            // 
            // ColumnTunerResult1
            // 
            this.ColumnTunerResult1.HeaderText = "Tuner 1";
            this.ColumnTunerResult1.MinimumWidth = 6;
            this.ColumnTunerResult1.Name = "ColumnTunerResult1";
            this.ColumnTunerResult1.ReadOnly = true;
            this.ColumnTunerResult1.Width = 125;
            // 
            // VdPTotalVoltageChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(138)))), ((int)(((byte)(114)))));
            this.ClientSize = new System.Drawing.Size(1260, 820);
            this.Controls.Add(this.DataGridViewVdPResult1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VdPTotalVoltageChildForm";
            this.Text = "Van der Pauw Total Voltage Form";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewVdPResult1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridViewVdPResult1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTunerResult1;
    }
}