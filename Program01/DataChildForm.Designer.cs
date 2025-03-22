namespace Program01
{
    partial class DataChildForm
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
            this.LabelMeasureValue = new System.Windows.Forms.Label();
            this.LabelSlope = new System.Windows.Forms.Label();
            this.TextboxMaxMeasureValue = new System.Windows.Forms.TextBox();
            this.ChartTunerTesting = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.LabelMaxMeasureValue = new System.Windows.Forms.Label();
            this.LabelMaxMeasureUnit = new System.Windows.Forms.Label();
            this.LabelMinMeasureUnit = new System.Windows.Forms.Label();
            this.LabelMinMeasureValue = new System.Windows.Forms.Label();
            this.TextboxMinMeasureValue = new System.Windows.Forms.TextBox();
            this.LabelMinSourceUnit = new System.Windows.Forms.Label();
            this.LabelMinSourceValue = new System.Windows.Forms.Label();
            this.TextboxMinSourceValue = new System.Windows.Forms.TextBox();
            this.LabelMaxSourceUnit = new System.Windows.Forms.Label();
            this.LabelMaxSourceValue = new System.Windows.Forms.Label();
            this.TextboxMaxSourceValue = new System.Windows.Forms.TextBox();
            this.LabelSourceValue = new System.Windows.Forms.Label();
            this.LabelSlopeUnit = new System.Windows.Forms.Label();
            this.LabelSlopeValue = new System.Windows.Forms.Label();
            this.TextboxSlopeValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChartTunerTesting)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelMeasureValue
            // 
            this.LabelMeasureValue.AutoSize = true;
            this.LabelMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMeasureValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.LabelMeasureValue.Location = new System.Drawing.Point(525, 80);
            this.LabelMeasureValue.Name = "LabelMeasureValue";
            this.LabelMeasureValue.Size = new System.Drawing.Size(172, 28);
            this.LabelMeasureValue.TabIndex = 6;
            this.LabelMeasureValue.Text = "MEASURE VALUE";
            // 
            // LabelSlope
            // 
            this.LabelSlope.AutoSize = true;
            this.LabelSlope.BackColor = System.Drawing.Color.Transparent;
            this.LabelSlope.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSlope.Location = new System.Drawing.Point(525, 406);
            this.LabelSlope.Name = "LabelSlope";
            this.LabelSlope.Size = new System.Drawing.Size(116, 28);
            this.LabelSlope.TabIndex = 8;
            this.LabelSlope.Text = "SLOPE (M) ";
            // 
            // TextboxMaxMeasureValue
            // 
            this.TextboxMaxMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMaxMeasureValue.Location = new System.Drawing.Point(635, 130);
            this.TextboxMaxMeasureValue.Multiline = true;
            this.TextboxMaxMeasureValue.Name = "TextboxMaxMeasureValue";
            this.TextboxMaxMeasureValue.ReadOnly = true;
            this.TextboxMaxMeasureValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxMaxMeasureValue.TabIndex = 10;
            // 
            // ChartTunerTesting
            // 
            this.ChartTunerTesting.BackColor = System.Drawing.Color.Transparent;
            this.ChartTunerTesting.BorderlineColor = System.Drawing.Color.Transparent;
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
            this.ChartTunerTesting.Location = new System.Drawing.Point(0, 30);
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
            this.ChartTunerTesting.Size = new System.Drawing.Size(520, 550);
            this.ChartTunerTesting.TabIndex = 11;
            this.ChartTunerTesting.Text = "Tuner Testing";
            // 
            // LabelMaxMeasureValue
            // 
            this.LabelMaxMeasureValue.AutoSize = true;
            this.LabelMaxMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxMeasureValue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxMeasureValue.Location = new System.Drawing.Point(525, 133);
            this.LabelMaxMeasureValue.Name = "LabelMaxMeasureValue";
            this.LabelMaxMeasureValue.Size = new System.Drawing.Size(97, 23);
            this.LabelMaxMeasureValue.TabIndex = 13;
            this.LabelMaxMeasureValue.Text = "MAXIMUM";
            // 
            // LabelMaxMeasureUnit
            // 
            this.LabelMaxMeasureUnit.AutoSize = true;
            this.LabelMaxMeasureUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxMeasureUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxMeasureUnit.Location = new System.Drawing.Point(730, 133);
            this.LabelMaxMeasureUnit.Name = "LabelMaxMeasureUnit";
            this.LabelMaxMeasureUnit.Size = new System.Drawing.Size(49, 23);
            this.LabelMaxMeasureUnit.TabIndex = 14;
            this.LabelMaxMeasureUnit.Text = "UNIT";
            // 
            // LabelMinMeasureUnit
            // 
            this.LabelMinMeasureUnit.AutoSize = true;
            this.LabelMinMeasureUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinMeasureUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinMeasureUnit.Location = new System.Drawing.Point(730, 189);
            this.LabelMinMeasureUnit.Name = "LabelMinMeasureUnit";
            this.LabelMinMeasureUnit.Size = new System.Drawing.Size(49, 23);
            this.LabelMinMeasureUnit.TabIndex = 17;
            this.LabelMinMeasureUnit.Text = "UNIT";
            // 
            // LabelMinMeasureValue
            // 
            this.LabelMinMeasureValue.AutoSize = true;
            this.LabelMinMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinMeasureValue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinMeasureValue.Location = new System.Drawing.Point(525, 189);
            this.LabelMinMeasureValue.Name = "LabelMinMeasureValue";
            this.LabelMinMeasureValue.Size = new System.Drawing.Size(93, 23);
            this.LabelMinMeasureValue.TabIndex = 16;
            this.LabelMinMeasureValue.Text = "MINIMUM";
            // 
            // TextboxMinMeasureValue
            // 
            this.TextboxMinMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMinMeasureValue.Location = new System.Drawing.Point(635, 186);
            this.TextboxMinMeasureValue.Multiline = true;
            this.TextboxMinMeasureValue.Name = "TextboxMinMeasureValue";
            this.TextboxMinMeasureValue.ReadOnly = true;
            this.TextboxMinMeasureValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxMinMeasureValue.TabIndex = 15;
            // 
            // LabelMinSourceUnit
            // 
            this.LabelMinSourceUnit.AutoSize = true;
            this.LabelMinSourceUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinSourceUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinSourceUnit.Location = new System.Drawing.Point(730, 356);
            this.LabelMinSourceUnit.Name = "LabelMinSourceUnit";
            this.LabelMinSourceUnit.Size = new System.Drawing.Size(49, 23);
            this.LabelMinSourceUnit.TabIndex = 24;
            this.LabelMinSourceUnit.Text = "UNIT";
            // 
            // LabelMinSourceValue
            // 
            this.LabelMinSourceValue.AutoSize = true;
            this.LabelMinSourceValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinSourceValue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinSourceValue.Location = new System.Drawing.Point(525, 356);
            this.LabelMinSourceValue.Name = "LabelMinSourceValue";
            this.LabelMinSourceValue.Size = new System.Drawing.Size(93, 23);
            this.LabelMinSourceValue.TabIndex = 23;
            this.LabelMinSourceValue.Text = "MINIMUM";
            // 
            // TextboxMinSourceValue
            // 
            this.TextboxMinSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMinSourceValue.Location = new System.Drawing.Point(635, 353);
            this.TextboxMinSourceValue.Multiline = true;
            this.TextboxMinSourceValue.Name = "TextboxMinSourceValue";
            this.TextboxMinSourceValue.ReadOnly = true;
            this.TextboxMinSourceValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxMinSourceValue.TabIndex = 22;
            // 
            // LabelMaxSourceUnit
            // 
            this.LabelMaxSourceUnit.AutoSize = true;
            this.LabelMaxSourceUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxSourceUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxSourceUnit.Location = new System.Drawing.Point(730, 300);
            this.LabelMaxSourceUnit.Name = "LabelMaxSourceUnit";
            this.LabelMaxSourceUnit.Size = new System.Drawing.Size(49, 23);
            this.LabelMaxSourceUnit.TabIndex = 21;
            this.LabelMaxSourceUnit.Text = "UNIT";
            // 
            // LabelMaxSourceValue
            // 
            this.LabelMaxSourceValue.AutoSize = true;
            this.LabelMaxSourceValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxSourceValue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxSourceValue.Location = new System.Drawing.Point(525, 300);
            this.LabelMaxSourceValue.Name = "LabelMaxSourceValue";
            this.LabelMaxSourceValue.Size = new System.Drawing.Size(97, 23);
            this.LabelMaxSourceValue.TabIndex = 20;
            this.LabelMaxSourceValue.Text = "MAXIMUM";
            // 
            // TextboxMaxSourceValue
            // 
            this.TextboxMaxSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMaxSourceValue.Location = new System.Drawing.Point(635, 297);
            this.TextboxMaxSourceValue.Multiline = true;
            this.TextboxMaxSourceValue.Name = "TextboxMaxSourceValue";
            this.TextboxMaxSourceValue.ReadOnly = true;
            this.TextboxMaxSourceValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxMaxSourceValue.TabIndex = 19;
            // 
            // LabelSourceValue
            // 
            this.LabelSourceValue.AutoSize = true;
            this.LabelSourceValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelSourceValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.LabelSourceValue.Location = new System.Drawing.Point(525, 247);
            this.LabelSourceValue.Name = "LabelSourceValue";
            this.LabelSourceValue.Size = new System.Drawing.Size(155, 28);
            this.LabelSourceValue.TabIndex = 18;
            this.LabelSourceValue.Text = "SOURCE VALUE";
            // 
            // LabelSlopeUnit
            // 
            this.LabelSlopeUnit.AutoSize = true;
            this.LabelSlopeUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelSlopeUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSlopeUnit.Location = new System.Drawing.Point(730, 450);
            this.LabelSlopeUnit.Name = "LabelSlopeUnit";
            this.LabelSlopeUnit.Size = new System.Drawing.Size(49, 23);
            this.LabelSlopeUnit.TabIndex = 27;
            this.LabelSlopeUnit.Text = "UNIT";
            // 
            // LabelSlopeValue
            // 
            this.LabelSlopeValue.AutoSize = true;
            this.LabelSlopeValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelSlopeValue.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSlopeValue.Location = new System.Drawing.Point(560, 450);
            this.LabelSlopeValue.Name = "LabelSlopeValue";
            this.LabelSlopeValue.Size = new System.Drawing.Size(58, 23);
            this.LabelSlopeValue.TabIndex = 26;
            this.LabelSlopeValue.Text = "M  =  ";
            // 
            // TextboxSlopeValue
            // 
            this.TextboxSlopeValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSlopeValue.Location = new System.Drawing.Point(635, 447);
            this.TextboxSlopeValue.Multiline = true;
            this.TextboxSlopeValue.Name = "TextboxSlopeValue";
            this.TextboxSlopeValue.ReadOnly = true;
            this.TextboxSlopeValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxSlopeValue.TabIndex = 25;
            // 
            // DataChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(248)))), ((int)(((byte)(231)))));
            this.ClientSize = new System.Drawing.Size(790, 610);
            this.Controls.Add(this.LabelSlopeUnit);
            this.Controls.Add(this.LabelSlopeValue);
            this.Controls.Add(this.TextboxSlopeValue);
            this.Controls.Add(this.LabelMinSourceUnit);
            this.Controls.Add(this.LabelMinSourceValue);
            this.Controls.Add(this.TextboxMinSourceValue);
            this.Controls.Add(this.LabelMaxSourceUnit);
            this.Controls.Add(this.LabelMaxSourceValue);
            this.Controls.Add(this.TextboxMaxSourceValue);
            this.Controls.Add(this.LabelSourceValue);
            this.Controls.Add(this.LabelMinMeasureUnit);
            this.Controls.Add(this.LabelMinMeasureValue);
            this.Controls.Add(this.TextboxMinMeasureValue);
            this.Controls.Add(this.LabelMaxMeasureUnit);
            this.Controls.Add(this.LabelMaxMeasureValue);
            this.Controls.Add(this.ChartTunerTesting);
            this.Controls.Add(this.TextboxMaxMeasureValue);
            this.Controls.Add(this.LabelSlope);
            this.Controls.Add(this.LabelMeasureValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataChildForm";
            ((System.ComponentModel.ISupportInitialize)(this.ChartTunerTesting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LabelMeasureValue;
        private System.Windows.Forms.Label LabelSlope;
        private System.Windows.Forms.TextBox TextboxMaxMeasureValue;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartTunerTesting;
        private System.Windows.Forms.Label LabelMaxMeasureValue;
        private System.Windows.Forms.Label LabelMaxMeasureUnit;
        private System.Windows.Forms.Label LabelMinMeasureUnit;
        private System.Windows.Forms.Label LabelMinMeasureValue;
        private System.Windows.Forms.TextBox TextboxMinMeasureValue;
        private System.Windows.Forms.Label LabelMinSourceUnit;
        private System.Windows.Forms.Label LabelMinSourceValue;
        private System.Windows.Forms.TextBox TextboxMinSourceValue;
        private System.Windows.Forms.Label LabelMaxSourceUnit;
        private System.Windows.Forms.Label LabelMaxSourceValue;
        private System.Windows.Forms.TextBox TextboxMaxSourceValue;
        private System.Windows.Forms.Label LabelSourceValue;
        private System.Windows.Forms.Label LabelSlopeUnit;
        private System.Windows.Forms.Label LabelSlopeValue;
        private System.Windows.Forms.TextBox TextboxSlopeValue;
    }
}