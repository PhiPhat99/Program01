namespace Program01
{
    partial class HallEffectTotalVoltageChildForm
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
            this.DataGridViewResults1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewResults1)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewResults1
            // 
            this.DataGridViewResults1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewResults1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridViewResults1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewResults1.Enabled = false;
            this.DataGridViewResults1.Location = new System.Drawing.Point(500, 60);
            this.DataGridViewResults1.Name = "DataGridViewResults1";
            this.DataGridViewResults1.RowHeadersWidth = 51;
            this.DataGridViewResults1.RowTemplate.Height = 24;
            this.DataGridViewResults1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.DataGridViewResults1.Size = new System.Drawing.Size(700, 700);
            this.DataGridViewResults1.TabIndex = 0;
            // 
            // HallEffectTotalVoltageChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(77)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1260, 820);
            this.Controls.Add(this.DataGridViewResults1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HallEffectTotalVoltageChildForm";
            this.Text = "Hall effect Measurement Total Voltage Form";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewResults1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridViewResults1;
    }
}