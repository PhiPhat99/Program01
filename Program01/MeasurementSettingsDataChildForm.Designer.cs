namespace Program01
{
    partial class MeasurementSettingsDataChildForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.labelTunerTesting = new System.Windows.Forms.Label();
            this.labelTunerEquations = new System.Windows.Forms.Label();
            this.labelSlopeTunerTesting = new System.Windows.Forms.Label();
            this.textboxSlopeTunerTesting = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ChartTunerTesting = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.ChartTunerTesting)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(620, 60);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(80, 31);
            this.comboBox1.TabIndex = 4;
            // 
            // labelTunerTesting
            // 
            this.labelTunerTesting.AutoSize = true;
            this.labelTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.labelTunerTesting.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTunerTesting.Location = new System.Drawing.Point(520, 60);
            this.labelTunerTesting.Name = "labelTunerTesting";
            this.labelTunerTesting.Size = new System.Drawing.Size(78, 28);
            this.labelTunerTesting.TabIndex = 5;
            this.labelTunerTesting.Text = "TUNER";
            // 
            // labelTunerEquations
            // 
            this.labelTunerEquations.AutoSize = true;
            this.labelTunerEquations.BackColor = System.Drawing.Color.Transparent;
            this.labelTunerEquations.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelTunerEquations.Location = new System.Drawing.Point(520, 110);
            this.labelTunerEquations.Name = "labelTunerEquations";
            this.labelTunerEquations.Size = new System.Drawing.Size(125, 28);
            this.labelTunerEquations.TabIndex = 6;
            this.labelTunerEquations.Text = "EQUATIONS";
            // 
            // labelSlopeTunerTesting
            // 
            this.labelSlopeTunerTesting.AutoSize = true;
            this.labelSlopeTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.labelSlopeTunerTesting.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlopeTunerTesting.Location = new System.Drawing.Point(520, 205);
            this.labelSlopeTunerTesting.Name = "labelSlopeTunerTesting";
            this.labelSlopeTunerTesting.Size = new System.Drawing.Size(77, 28);
            this.labelSlopeTunerTesting.TabIndex = 8;
            this.labelSlopeTunerTesting.Text = "SLOPE ";
            // 
            // textboxSlopeTunerTesting
            // 
            this.textboxSlopeTunerTesting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxSlopeTunerTesting.Location = new System.Drawing.Point(530, 250);
            this.textboxSlopeTunerTesting.Multiline = true;
            this.textboxSlopeTunerTesting.Name = "textboxSlopeTunerTesting";
            this.textboxSlopeTunerTesting.Size = new System.Drawing.Size(100, 30);
            this.textboxSlopeTunerTesting.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(530, 155);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(220, 30);
            this.textBox1.TabIndex = 10;
            // 
            // ChartTunerTesting
            // 
            this.ChartTunerTesting.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.Title = "Source Values";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MinorGrid.Enabled = true;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.Title = "Measure Values";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.BackColor = System.Drawing.Color.DimGray;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.BackSecondaryColor = System.Drawing.Color.Silver;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 90F;
            chartArea1.InnerPlotPosition.Width = 90F;
            chartArea1.InnerPlotPosition.X = 10F;
            chartArea1.Name = "ChartAreaTunerTesting";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 90F;
            chartArea1.Position.Width = 90F;
            chartArea1.Position.X = 6F;
            chartArea1.Position.Y = 2F;
            this.ChartTunerTesting.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.ChartTunerTesting.Legends.Add(legend1);
            this.ChartTunerTesting.Location = new System.Drawing.Point(15, 45);
            this.ChartTunerTesting.Name = "ChartTunerTesting";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartAreaTunerTesting";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.Navy;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.LegendText = "Measurement Data";
            series1.MarkerBorderColor = System.Drawing.Color.Silver;
            series1.MarkerBorderWidth = 2;
            series1.MarkerColor = System.Drawing.Color.Silver;
            series1.MarkerSize = 6;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series1.Name = "MeasurementData";
            this.ChartTunerTesting.Series.Add(series1);
            this.ChartTunerTesting.Size = new System.Drawing.Size(510, 510);
            this.ChartTunerTesting.TabIndex = 11;
            this.ChartTunerTesting.Text = "Tuner Testing";
            // 
            // MeasurementSettingsDataChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(248)))), ((int)(((byte)(231)))));
            this.ClientSize = new System.Drawing.Size(790, 610);
            this.Controls.Add(this.ChartTunerTesting);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelSlopeTunerTesting);
            this.Controls.Add(this.textboxSlopeTunerTesting);
            this.Controls.Add(this.labelTunerEquations);
            this.Controls.Add(this.labelTunerTesting);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MeasurementSettingsDataChildForm";
            ((System.ComponentModel.ISupportInitialize)(this.ChartTunerTesting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelTunerTesting;
        private System.Windows.Forms.Label labelTunerEquations;
        private System.Windows.Forms.Label labelSlopeTunerTesting;
        private System.Windows.Forms.TextBox textboxSlopeTunerTesting;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartTunerTesting;
    }
}