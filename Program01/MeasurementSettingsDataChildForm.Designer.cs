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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.LabelTuner = new System.Windows.Forms.Label();
            this.LabelMeasureValue = new System.Windows.Forms.Label();
            this.LabelSlope = new System.Windows.Forms.Label();
            this.TextboxMaxMeasureValue = new System.Windows.Forms.TextBox();
            this.ChartTunerTesting = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TextboxTuner = new System.Windows.Forms.TextBox();
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
            // LabelTuner
            // 
            this.LabelTuner.AutoSize = true;
            this.LabelTuner.BackColor = System.Drawing.Color.Transparent;
            this.LabelTuner.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTuner.Location = new System.Drawing.Point(555, 58);
            this.LabelTuner.Name = "LabelTuner";
            this.LabelTuner.Size = new System.Drawing.Size(78, 28);
            this.LabelTuner.TabIndex = 5;
            this.LabelTuner.Text = "TUNER";
            // 
            // LabelMeasureValue
            // 
            this.LabelMeasureValue.AutoSize = true;
            this.LabelMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMeasureValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.LabelMeasureValue.Location = new System.Drawing.Point(530, 110);
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
            this.LabelSlope.Location = new System.Drawing.Point(530, 436);
            this.LabelSlope.Name = "LabelSlope";
            this.LabelSlope.Size = new System.Drawing.Size(116, 28);
            this.LabelSlope.TabIndex = 8;
            this.LabelSlope.Text = "SLOPE (M) ";
            // 
            // TextboxMaxMeasureValue
            // 
            this.TextboxMaxMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMaxMeasureValue.Location = new System.Drawing.Point(635, 160);
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
            chartArea2.AxisX.MinorGrid.Enabled = true;
            chartArea2.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisX.Title = "Source Values";
            chartArea2.AxisX.TitleFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisY.MinorGrid.Enabled = true;
            chartArea2.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisY.Title = "Measure Values";
            chartArea2.AxisY.TitleFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.BackColor = System.Drawing.Color.DimGray;
            chartArea2.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea2.BackSecondaryColor = System.Drawing.Color.Silver;
            chartArea2.InnerPlotPosition.Auto = false;
            chartArea2.InnerPlotPosition.Height = 90F;
            chartArea2.InnerPlotPosition.Width = 90F;
            chartArea2.InnerPlotPosition.X = 10F;
            chartArea2.Name = "ChartAreaTunerTesting";
            chartArea2.Position.Auto = false;
            chartArea2.Position.Height = 90F;
            chartArea2.Position.Width = 90F;
            chartArea2.Position.X = 6F;
            chartArea2.Position.Y = 2F;
            this.ChartTunerTesting.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.ChartTunerTesting.Legends.Add(legend2);
            this.ChartTunerTesting.Location = new System.Drawing.Point(10, 30);
            this.ChartTunerTesting.Name = "ChartTunerTesting";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartAreaTunerTesting";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Navy;
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.LegendText = "Measurement Data";
            series2.MarkerBorderColor = System.Drawing.Color.Silver;
            series2.MarkerBorderWidth = 2;
            series2.MarkerColor = System.Drawing.Color.Silver;
            series2.MarkerSize = 6;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series2.Name = "MeasurementData";
            this.ChartTunerTesting.Series.Add(series2);
            this.ChartTunerTesting.Size = new System.Drawing.Size(520, 550);
            this.ChartTunerTesting.TabIndex = 11;
            this.ChartTunerTesting.Text = "Tuner Testing";
            // 
            // TextboxTuner
            // 
            this.TextboxTuner.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxTuner.Location = new System.Drawing.Point(640, 55);
            this.TextboxTuner.Multiline = true;
            this.TextboxTuner.Name = "TextboxTuner";
            this.TextboxTuner.ReadOnly = true;
            this.TextboxTuner.Size = new System.Drawing.Size(50, 30);
            this.TextboxTuner.TabIndex = 12;
            // 
            // LabelMaxMeasureValue
            // 
            this.LabelMaxMeasureValue.AutoSize = true;
            this.LabelMaxMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxMeasureValue.Location = new System.Drawing.Point(530, 163);
            this.LabelMaxMeasureValue.Name = "LabelMaxMeasureValue";
            this.LabelMaxMeasureValue.Size = new System.Drawing.Size(98, 23);
            this.LabelMaxMeasureValue.TabIndex = 13;
            this.LabelMaxMeasureValue.Text = "MAXIMUM";
            // 
            // LabelMaxMeasureUnit
            // 
            this.LabelMaxMeasureUnit.AutoSize = true;
            this.LabelMaxMeasureUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxMeasureUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxMeasureUnit.Location = new System.Drawing.Point(720, 163);
            this.LabelMaxMeasureUnit.Name = "LabelMaxMeasureUnit";
            this.LabelMaxMeasureUnit.Size = new System.Drawing.Size(50, 23);
            this.LabelMaxMeasureUnit.TabIndex = 14;
            this.LabelMaxMeasureUnit.Text = "UNIT";
            // 
            // LabelMinMeasureUnit
            // 
            this.LabelMinMeasureUnit.AutoSize = true;
            this.LabelMinMeasureUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinMeasureUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinMeasureUnit.Location = new System.Drawing.Point(720, 219);
            this.LabelMinMeasureUnit.Name = "LabelMinMeasureUnit";
            this.LabelMinMeasureUnit.Size = new System.Drawing.Size(50, 23);
            this.LabelMinMeasureUnit.TabIndex = 17;
            this.LabelMinMeasureUnit.Text = "UNIT";
            // 
            // LabelMinMeasureValue
            // 
            this.LabelMinMeasureValue.AutoSize = true;
            this.LabelMinMeasureValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinMeasureValue.Location = new System.Drawing.Point(530, 219);
            this.LabelMinMeasureValue.Name = "LabelMinMeasureValue";
            this.LabelMinMeasureValue.Size = new System.Drawing.Size(93, 23);
            this.LabelMinMeasureValue.TabIndex = 16;
            this.LabelMinMeasureValue.Text = "MINIMUM";
            // 
            // TextboxMinMeasureValue
            // 
            this.TextboxMinMeasureValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMinMeasureValue.Location = new System.Drawing.Point(635, 216);
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
            this.LabelMinSourceUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinSourceUnit.Location = new System.Drawing.Point(720, 386);
            this.LabelMinSourceUnit.Name = "LabelMinSourceUnit";
            this.LabelMinSourceUnit.Size = new System.Drawing.Size(50, 23);
            this.LabelMinSourceUnit.TabIndex = 24;
            this.LabelMinSourceUnit.Text = "UNIT";
            // 
            // LabelMinSourceValue
            // 
            this.LabelMinSourceValue.AutoSize = true;
            this.LabelMinSourceValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMinSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMinSourceValue.Location = new System.Drawing.Point(530, 386);
            this.LabelMinSourceValue.Name = "LabelMinSourceValue";
            this.LabelMinSourceValue.Size = new System.Drawing.Size(93, 23);
            this.LabelMinSourceValue.TabIndex = 23;
            this.LabelMinSourceValue.Text = "MINIMUM";
            // 
            // TextboxMinSourceValue
            // 
            this.TextboxMinSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMinSourceValue.Location = new System.Drawing.Point(635, 383);
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
            this.LabelMaxSourceUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxSourceUnit.Location = new System.Drawing.Point(720, 330);
            this.LabelMaxSourceUnit.Name = "LabelMaxSourceUnit";
            this.LabelMaxSourceUnit.Size = new System.Drawing.Size(50, 23);
            this.LabelMaxSourceUnit.TabIndex = 21;
            this.LabelMaxSourceUnit.Text = "UNIT";
            // 
            // LabelMaxSourceValue
            // 
            this.LabelMaxSourceValue.AutoSize = true;
            this.LabelMaxSourceValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelMaxSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMaxSourceValue.Location = new System.Drawing.Point(530, 330);
            this.LabelMaxSourceValue.Name = "LabelMaxSourceValue";
            this.LabelMaxSourceValue.Size = new System.Drawing.Size(98, 23);
            this.LabelMaxSourceValue.TabIndex = 20;
            this.LabelMaxSourceValue.Text = "MAXIMUM";
            // 
            // TextboxMaxSourceValue
            // 
            this.TextboxMaxSourceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMaxSourceValue.Location = new System.Drawing.Point(635, 327);
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
            this.LabelSourceValue.Location = new System.Drawing.Point(530, 277);
            this.LabelSourceValue.Name = "LabelSourceValue";
            this.LabelSourceValue.Size = new System.Drawing.Size(155, 28);
            this.LabelSourceValue.TabIndex = 18;
            this.LabelSourceValue.Text = "SOURCE VALUE";
            // 
            // LabelSlopeUnit
            // 
            this.LabelSlopeUnit.AutoSize = true;
            this.LabelSlopeUnit.BackColor = System.Drawing.Color.Transparent;
            this.LabelSlopeUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSlopeUnit.Location = new System.Drawing.Point(720, 480);
            this.LabelSlopeUnit.Name = "LabelSlopeUnit";
            this.LabelSlopeUnit.Size = new System.Drawing.Size(50, 23);
            this.LabelSlopeUnit.TabIndex = 27;
            this.LabelSlopeUnit.Text = "UNIT";
            // 
            // LabelSlopeValue
            // 
            this.LabelSlopeValue.AutoSize = true;
            this.LabelSlopeValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelSlopeValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSlopeValue.Location = new System.Drawing.Point(573, 480);
            this.LabelSlopeValue.Name = "LabelSlopeValue";
            this.LabelSlopeValue.Size = new System.Drawing.Size(58, 23);
            this.LabelSlopeValue.TabIndex = 26;
            this.LabelSlopeValue.Text = "M  =  ";
            // 
            // TextboxSlopeValue
            // 
            this.TextboxSlopeValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSlopeValue.Location = new System.Drawing.Point(635, 477);
            this.TextboxSlopeValue.Multiline = true;
            this.TextboxSlopeValue.Name = "TextboxSlopeValue";
            this.TextboxSlopeValue.ReadOnly = true;
            this.TextboxSlopeValue.Size = new System.Drawing.Size(80, 30);
            this.TextboxSlopeValue.TabIndex = 25;
            // 
            // MeasurementSettingsDataChildForm
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
            this.Controls.Add(this.TextboxTuner);
            this.Controls.Add(this.ChartTunerTesting);
            this.Controls.Add(this.TextboxMaxMeasureValue);
            this.Controls.Add(this.LabelSlope);
            this.Controls.Add(this.LabelMeasureValue);
            this.Controls.Add(this.LabelTuner);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MeasurementSettingsDataChildForm";
            ((System.ComponentModel.ISupportInitialize)(this.ChartTunerTesting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LabelTuner;
        private System.Windows.Forms.Label LabelMeasureValue;
        private System.Windows.Forms.Label LabelSlope;
        private System.Windows.Forms.TextBox TextboxMaxMeasureValue;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartTunerTesting;
        private System.Windows.Forms.TextBox TextboxTuner;
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