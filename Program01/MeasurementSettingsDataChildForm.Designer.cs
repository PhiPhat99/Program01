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
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chartTunerTesting = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textboxCurrentSourceRange = new System.Windows.Forms.TextBox();
            this.labelCurrentTunerTesting = new System.Windows.Forms.Label();
            this.labelCurrentSourceRangeUnit = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.labelTunerTesting = new System.Windows.Forms.Label();
            this.labelTunerEquations = new System.Windows.Forms.Label();
            this.labelSlopeTunerTesting = new System.Windows.Forms.Label();
            this.textboxSlopeTunerTesting = new System.Windows.Forms.TextBox();
            this.labelSlopeTunerTestingUnit = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartTunerTesting)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTunerTesting
            // 
            this.chartTunerTesting.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisX.Title = "CURRENT (Ampere)";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea1.AxisY.MinorGrid.Enabled = true;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.Title = "VOLTAGE (Volt)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.BackSecondaryColor = System.Drawing.Color.Silver;
            chartArea1.BorderColor = System.Drawing.Color.Transparent;
            chartArea1.BorderWidth = 0;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 90F;
            chartArea1.InnerPlotPosition.Width = 85F;
            chartArea1.InnerPlotPosition.X = 15F;
            chartArea1.InnerPlotPosition.Y = 3F;
            chartArea1.Name = "ChartareaTunerTesting";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 95F;
            chartArea1.Position.Width = 95F;
            this.chartTunerTesting.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartTunerTesting.Legends.Add(legend1);
            this.chartTunerTesting.Location = new System.Drawing.Point(30, 40);
            this.chartTunerTesting.Name = "chartTunerTesting";
            series1.BorderColor = System.Drawing.Color.Transparent;
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartareaTunerTesting";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.MidnightBlue;
            series1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(221)))), ((int)(((byte)(255)))));
            series1.MarkerBorderWidth = 2;
            series1.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(221)))), ((int)(((byte)(255)))));
            series1.MarkerSize = 4;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "I-V Curve";
            series1.ShadowColor = System.Drawing.Color.Transparent;
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValuesPerPoint = 4;
            this.chartTunerTesting.Series.Add(series1);
            this.chartTunerTesting.Size = new System.Drawing.Size(500, 500);
            this.chartTunerTesting.TabIndex = 0;
            title1.Name = "TitleTunerTesting";
            title1.Text = "TUNER TESTING";
            title1.Visible = false;
            this.chartTunerTesting.Titles.Add(title1);
            // 
            // textboxCurrentSourceRange
            // 
            this.textboxCurrentSourceRange.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxCurrentSourceRange.Location = new System.Drawing.Point(560, 220);
            this.textboxCurrentSourceRange.Multiline = true;
            this.textboxCurrentSourceRange.Name = "textboxCurrentSourceRange";
            this.textboxCurrentSourceRange.Size = new System.Drawing.Size(120, 30);
            this.textboxCurrentSourceRange.TabIndex = 1;
            // 
            // labelCurrentTunerTesting
            // 
            this.labelCurrentTunerTesting.AutoSize = true;
            this.labelCurrentTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentTunerTesting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentTunerTesting.Location = new System.Drawing.Point(550, 180);
            this.labelCurrentTunerTesting.Name = "labelCurrentTunerTesting";
            this.labelCurrentTunerTesting.Size = new System.Drawing.Size(220, 23);
            this.labelCurrentTunerTesting.TabIndex = 2;
            this.labelCurrentTunerTesting.Text = "CURRENT SOURCE RANGE";
            // 
            // labelCurrentSourceRangeUnit
            // 
            this.labelCurrentSourceRangeUnit.AutoSize = true;
            this.labelCurrentSourceRangeUnit.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentSourceRangeUnit.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentSourceRangeUnit.Location = new System.Drawing.Point(700, 227);
            this.labelCurrentSourceRangeUnit.Name = "labelCurrentSourceRangeUnit";
            this.labelCurrentSourceRangeUnit.Size = new System.Drawing.Size(63, 19);
            this.labelCurrentSourceRangeUnit.TabIndex = 3;
            this.labelCurrentSourceRangeUnit.Text = "Ampere";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(640, 110);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(120, 31);
            this.comboBox1.TabIndex = 4;
            // 
            // labelTunerTesting
            // 
            this.labelTunerTesting.AutoSize = true;
            this.labelTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.labelTunerTesting.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTunerTesting.Location = new System.Drawing.Point(540, 110);
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
            this.labelTunerEquations.Location = new System.Drawing.Point(540, 280);
            this.labelTunerEquations.Name = "labelTunerEquations";
            this.labelTunerEquations.Size = new System.Drawing.Size(125, 28);
            this.labelTunerEquations.TabIndex = 6;
            this.labelTunerEquations.Text = "EQUATIONS";
            // 
            // labelSlopeTunerTesting
            // 
            this.labelSlopeTunerTesting.AutoSize = true;
            this.labelSlopeTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.labelSlopeTunerTesting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlopeTunerTesting.Location = new System.Drawing.Point(550, 390);
            this.labelSlopeTunerTesting.Name = "labelSlopeTunerTesting";
            this.labelSlopeTunerTesting.Size = new System.Drawing.Size(66, 23);
            this.labelSlopeTunerTesting.TabIndex = 8;
            this.labelSlopeTunerTesting.Text = "SLOPE ";
            // 
            // textboxSlopeTunerTesting
            // 
            this.textboxSlopeTunerTesting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxSlopeTunerTesting.Location = new System.Drawing.Point(560, 430);
            this.textboxSlopeTunerTesting.Multiline = true;
            this.textboxSlopeTunerTesting.Name = "textboxSlopeTunerTesting";
            this.textboxSlopeTunerTesting.Size = new System.Drawing.Size(120, 30);
            this.textboxSlopeTunerTesting.TabIndex = 7;
            // 
            // labelSlopeTunerTestingUnit
            // 
            this.labelSlopeTunerTestingUnit.AutoSize = true;
            this.labelSlopeTunerTestingUnit.BackColor = System.Drawing.Color.Transparent;
            this.labelSlopeTunerTestingUnit.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlopeTunerTestingUnit.Location = new System.Drawing.Point(700, 437);
            this.labelSlopeTunerTestingUnit.Name = "labelSlopeTunerTestingUnit";
            this.labelSlopeTunerTestingUnit.Size = new System.Drawing.Size(41, 19);
            this.labelSlopeTunerTestingUnit.TabIndex = 9;
            this.labelSlopeTunerTestingUnit.Text = "Ohm";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(560, 330);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 30);
            this.textBox1.TabIndex = 10;
            // 
            // MeasurementSettingsDataChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(248)))), ((int)(((byte)(231)))));
            this.ClientSize = new System.Drawing.Size(790, 610);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelSlopeTunerTestingUnit);
            this.Controls.Add(this.labelSlopeTunerTesting);
            this.Controls.Add(this.textboxSlopeTunerTesting);
            this.Controls.Add(this.labelTunerEquations);
            this.Controls.Add(this.labelTunerTesting);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.labelCurrentSourceRangeUnit);
            this.Controls.Add(this.labelCurrentTunerTesting);
            this.Controls.Add(this.textboxCurrentSourceRange);
            this.Controls.Add(this.chartTunerTesting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MeasurementSettingsDataChildForm";
            ((System.ComponentModel.ISupportInitialize)(this.chartTunerTesting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartTunerTesting;
        private System.Windows.Forms.TextBox textboxCurrentSourceRange;
        private System.Windows.Forms.Label labelCurrentTunerTesting;
        private System.Windows.Forms.Label labelCurrentSourceRangeUnit;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelTunerTesting;
        private System.Windows.Forms.Label labelTunerEquations;
        private System.Windows.Forms.Label labelSlopeTunerTesting;
        private System.Windows.Forms.TextBox textboxSlopeTunerTesting;
        private System.Windows.Forms.Label labelSlopeTunerTestingUnit;
        private System.Windows.Forms.TextBox textBox1;
    }
}