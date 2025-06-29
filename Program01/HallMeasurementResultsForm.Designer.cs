﻿namespace Program01
{
    partial class HallMeasurementResultsForm
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
            this.PanelResultsChart = new System.Windows.Forms.Panel();
            this.PanelMeasureAndSource = new System.Windows.Forms.Panel();
            this.TextboxSourceMode = new System.Windows.Forms.TextBox();
            this.TextboxMeasureMode = new System.Windows.Forms.TextBox();
            this.LabelMeasureMode = new System.Windows.Forms.Label();
            this.LabelSourceMode = new System.Windows.Forms.Label();
            this.TextboxHallInSouth1Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth2Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth3Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth4Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth3 = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth4 = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth2 = new System.Windows.Forms.TextBox();
            this.TextboxHallInSouth1 = new System.Windows.Forms.TextBox();
            this.LabelHallInVoltageHeader = new System.Windows.Forms.Label();
            this.RichTextboxHallInSouthPos1 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInSouthPos2 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInSouthPos3 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInSouthPos4 = new System.Windows.Forms.RichTextBox();
            this.PanelResults = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.IconbuttonNType = new FontAwesome.Sharp.IconButton();
            this.IconbuttonPType = new FontAwesome.Sharp.IconButton();
            this.TextboxHallResUnit = new System.Windows.Forms.TextBox();
            this.LabelTypes = new System.Windows.Forms.Label();
            this.RichTextboxHallCoefficientUnit = new System.Windows.Forms.RichTextBox();
            this.RichTextboxMobilityUnit = new System.Windows.Forms.RichTextBox();
            this.RichTextboxSheetConcentrationUnit = new System.Windows.Forms.RichTextBox();
            this.RichTextboxMobility = new System.Windows.Forms.RichTextBox();
            this.RichTextboxSheetConcentration = new System.Windows.Forms.RichTextBox();
            this.TextboxMobility = new System.Windows.Forms.TextBox();
            this.TextboxSheetConcentration = new System.Windows.Forms.TextBox();
            this.RichTextboxBulkConcentrationUnit = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallRes = new System.Windows.Forms.RichTextBox();
            this.RichTextboxBulkConcentration = new System.Windows.Forms.RichTextBox();
            this.TextboxHallRes = new System.Windows.Forms.TextBox();
            this.TextboxBulkConcentration = new System.Windows.Forms.TextBox();
            this.RichTextboxHallVoltage = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallCoefficient = new System.Windows.Forms.RichTextBox();
            this.TextboxHallVoltage = new System.Windows.Forms.TextBox();
            this.TextboxHallCoefficient = new System.Windows.Forms.TextBox();
            this.TextboxHallVoltageUnit = new System.Windows.Forms.TextBox();
            this.LabelHallPropertiesHeader = new System.Windows.Forms.Label();
            this.RichTextboxHallInNorthPos4 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInNorthPos3 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInNorthPos2 = new System.Windows.Forms.RichTextBox();
            this.RichTextboxHallInNorthPos1 = new System.Windows.Forms.RichTextBox();
            this.TextboxHallInNorth1 = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth2 = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth4 = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth3 = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth4Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth3Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth2Unit = new System.Windows.Forms.TextBox();
            this.TextboxHallInNorth1Unit = new System.Windows.Forms.TextBox();
            this.ChartHallVoltageResults = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.PanelResultsChart.SuspendLayout();
            this.PanelMeasureAndSource.SuspendLayout();
            this.PanelResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartHallVoltageResults)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelResultsChart
            // 
            this.PanelResultsChart.BackColor = System.Drawing.Color.Transparent;
            this.PanelResultsChart.Controls.Add(this.ChartHallVoltageResults);
            this.PanelResultsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelResultsChart.Location = new System.Drawing.Point(0, 80);
            this.PanelResultsChart.Name = "PanelResultsChart";
            this.PanelResultsChart.Size = new System.Drawing.Size(840, 740);
            this.PanelResultsChart.TabIndex = 129;
            // 
            // PanelMeasureAndSource
            // 
            this.PanelMeasureAndSource.BackColor = System.Drawing.Color.DimGray;
            this.PanelMeasureAndSource.Controls.Add(this.TextboxSourceMode);
            this.PanelMeasureAndSource.Controls.Add(this.TextboxMeasureMode);
            this.PanelMeasureAndSource.Controls.Add(this.LabelMeasureMode);
            this.PanelMeasureAndSource.Controls.Add(this.LabelSourceMode);
            this.PanelMeasureAndSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelMeasureAndSource.Location = new System.Drawing.Point(0, 0);
            this.PanelMeasureAndSource.Name = "PanelMeasureAndSource";
            this.PanelMeasureAndSource.Size = new System.Drawing.Size(840, 80);
            this.PanelMeasureAndSource.TabIndex = 128;
            // 
            // TextboxSourceMode
            // 
            this.TextboxSourceMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSourceMode.Location = new System.Drawing.Point(530, 24);
            this.TextboxSourceMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxSourceMode.Name = "TextboxSourceMode";
            this.TextboxSourceMode.ReadOnly = true;
            this.TextboxSourceMode.Size = new System.Drawing.Size(150, 30);
            this.TextboxSourceMode.TabIndex = 61;
            this.TextboxSourceMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxMeasureMode
            // 
            this.TextboxMeasureMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMeasureMode.Location = new System.Drawing.Point(210, 24);
            this.TextboxMeasureMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxMeasureMode.Name = "TextboxMeasureMode";
            this.TextboxMeasureMode.ReadOnly = true;
            this.TextboxMeasureMode.Size = new System.Drawing.Size(150, 30);
            this.TextboxMeasureMode.TabIndex = 60;
            this.TextboxMeasureMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabelMeasureMode
            // 
            this.LabelMeasureMode.AutoSize = true;
            this.LabelMeasureMode.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMeasureMode.Location = new System.Drawing.Point(90, 25);
            this.LabelMeasureMode.Name = "LabelMeasureMode";
            this.LabelMeasureMode.Size = new System.Drawing.Size(111, 28);
            this.LabelMeasureMode.TabIndex = 58;
            this.LabelMeasureMode.Text = "MEASURE :";
            // 
            // LabelSourceMode
            // 
            this.LabelSourceMode.AutoSize = true;
            this.LabelSourceMode.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceMode.Location = new System.Drawing.Point(420, 25);
            this.LabelSourceMode.Name = "LabelSourceMode";
            this.LabelSourceMode.Size = new System.Drawing.Size(97, 28);
            this.LabelSourceMode.TabIndex = 56;
            this.LabelSourceMode.Text = "SOURCE :";
            // 
            // TextboxHallInSouth1Unit
            // 
            this.TextboxHallInSouth1Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth1Unit.Location = new System.Drawing.Point(325, 104);
            this.TextboxHallInSouth1Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth1Unit.Name = "TextboxHallInSouth1Unit";
            this.TextboxHallInSouth1Unit.ReadOnly = true;
            this.TextboxHallInSouth1Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInSouth1Unit.TabIndex = 133;
            this.TextboxHallInSouth1Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth2Unit
            // 
            this.TextboxHallInSouth2Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth2Unit.Location = new System.Drawing.Point(325, 149);
            this.TextboxHallInSouth2Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth2Unit.Name = "TextboxHallInSouth2Unit";
            this.TextboxHallInSouth2Unit.ReadOnly = true;
            this.TextboxHallInSouth2Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInSouth2Unit.TabIndex = 134;
            this.TextboxHallInSouth2Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth3Unit
            // 
            this.TextboxHallInSouth3Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth3Unit.Location = new System.Drawing.Point(325, 194);
            this.TextboxHallInSouth3Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth3Unit.Name = "TextboxHallInSouth3Unit";
            this.TextboxHallInSouth3Unit.ReadOnly = true;
            this.TextboxHallInSouth3Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInSouth3Unit.TabIndex = 135;
            this.TextboxHallInSouth3Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth4Unit
            // 
            this.TextboxHallInSouth4Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth4Unit.Location = new System.Drawing.Point(325, 239);
            this.TextboxHallInSouth4Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth4Unit.Name = "TextboxHallInSouth4Unit";
            this.TextboxHallInSouth4Unit.ReadOnly = true;
            this.TextboxHallInSouth4Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInSouth4Unit.TabIndex = 136;
            this.TextboxHallInSouth4Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth3
            // 
            this.TextboxHallInSouth3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth3.Location = new System.Drawing.Point(200, 194);
            this.TextboxHallInSouth3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth3.Name = "TextboxHallInSouth3";
            this.TextboxHallInSouth3.ReadOnly = true;
            this.TextboxHallInSouth3.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInSouth3.TabIndex = 128;
            this.TextboxHallInSouth3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth4
            // 
            this.TextboxHallInSouth4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth4.Location = new System.Drawing.Point(200, 239);
            this.TextboxHallInSouth4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth4.Name = "TextboxHallInSouth4";
            this.TextboxHallInSouth4.ReadOnly = true;
            this.TextboxHallInSouth4.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInSouth4.TabIndex = 130;
            this.TextboxHallInSouth4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth2
            // 
            this.TextboxHallInSouth2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth2.Location = new System.Drawing.Point(200, 149);
            this.TextboxHallInSouth2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth2.Name = "TextboxHallInSouth2";
            this.TextboxHallInSouth2.ReadOnly = true;
            this.TextboxHallInSouth2.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInSouth2.TabIndex = 126;
            this.TextboxHallInSouth2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInSouth1
            // 
            this.TextboxHallInSouth1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInSouth1.Location = new System.Drawing.Point(200, 104);
            this.TextboxHallInSouth1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInSouth1.Name = "TextboxHallInSouth1";
            this.TextboxHallInSouth1.ReadOnly = true;
            this.TextboxHallInSouth1.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInSouth1.TabIndex = 124;
            this.TextboxHallInSouth1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabelHallInVoltageHeader
            // 
            this.LabelHallInVoltageHeader.AutoSize = true;
            this.LabelHallInVoltageHeader.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallInVoltageHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallInVoltageHeader.Location = new System.Drawing.Point(30, 65);
            this.LabelHallInVoltageHeader.Name = "LabelHallInVoltageHeader";
            this.LabelHallInVoltageHeader.Size = new System.Drawing.Size(165, 28);
            this.LabelHallInVoltageHeader.TabIndex = 138;
            this.LabelHallInVoltageHeader.Text = "HALL VOLTAGES";
            // 
            // RichTextboxHallInSouthPos1
            // 
            this.RichTextboxHallInSouthPos1.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInSouthPos1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInSouthPos1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInSouthPos1.Location = new System.Drawing.Point(100, 105);
            this.RichTextboxHallInSouthPos1.Name = "RichTextboxHallInSouthPos1";
            this.RichTextboxHallInSouthPos1.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInSouthPos1.TabIndex = 174;
            this.RichTextboxHallInSouthPos1.Text = "VHS1:";
            // 
            // RichTextboxHallInSouthPos2
            // 
            this.RichTextboxHallInSouthPos2.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInSouthPos2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInSouthPos2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInSouthPos2.Location = new System.Drawing.Point(100, 150);
            this.RichTextboxHallInSouthPos2.Name = "RichTextboxHallInSouthPos2";
            this.RichTextboxHallInSouthPos2.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInSouthPos2.TabIndex = 175;
            this.RichTextboxHallInSouthPos2.Text = "VHS2:";
            // 
            // RichTextboxHallInSouthPos3
            // 
            this.RichTextboxHallInSouthPos3.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInSouthPos3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInSouthPos3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInSouthPos3.Location = new System.Drawing.Point(100, 195);
            this.RichTextboxHallInSouthPos3.Name = "RichTextboxHallInSouthPos3";
            this.RichTextboxHallInSouthPos3.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInSouthPos3.TabIndex = 176;
            this.RichTextboxHallInSouthPos3.Text = "VHS3:";
            // 
            // RichTextboxHallInSouthPos4
            // 
            this.RichTextboxHallInSouthPos4.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInSouthPos4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInSouthPos4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInSouthPos4.Location = new System.Drawing.Point(100, 240);
            this.RichTextboxHallInSouthPos4.Name = "RichTextboxHallInSouthPos4";
            this.RichTextboxHallInSouthPos4.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInSouthPos4.TabIndex = 177;
            this.RichTextboxHallInSouthPos4.Text = "VHS4:";
            // 
            // PanelResults
            // 
            this.PanelResults.AutoScroll = true;
            this.PanelResults.BackColor = System.Drawing.Color.DarkGray;
            this.PanelResults.Controls.Add(this.label1);
            this.PanelResults.Controls.Add(this.IconbuttonNType);
            this.PanelResults.Controls.Add(this.IconbuttonPType);
            this.PanelResults.Controls.Add(this.TextboxHallResUnit);
            this.PanelResults.Controls.Add(this.LabelTypes);
            this.PanelResults.Controls.Add(this.RichTextboxHallCoefficientUnit);
            this.PanelResults.Controls.Add(this.RichTextboxMobilityUnit);
            this.PanelResults.Controls.Add(this.RichTextboxSheetConcentrationUnit);
            this.PanelResults.Controls.Add(this.RichTextboxMobility);
            this.PanelResults.Controls.Add(this.RichTextboxSheetConcentration);
            this.PanelResults.Controls.Add(this.TextboxMobility);
            this.PanelResults.Controls.Add(this.TextboxSheetConcentration);
            this.PanelResults.Controls.Add(this.RichTextboxBulkConcentrationUnit);
            this.PanelResults.Controls.Add(this.RichTextboxHallRes);
            this.PanelResults.Controls.Add(this.RichTextboxBulkConcentration);
            this.PanelResults.Controls.Add(this.TextboxHallRes);
            this.PanelResults.Controls.Add(this.TextboxBulkConcentration);
            this.PanelResults.Controls.Add(this.RichTextboxHallVoltage);
            this.PanelResults.Controls.Add(this.RichTextboxHallCoefficient);
            this.PanelResults.Controls.Add(this.TextboxHallVoltage);
            this.PanelResults.Controls.Add(this.TextboxHallCoefficient);
            this.PanelResults.Controls.Add(this.TextboxHallVoltageUnit);
            this.PanelResults.Controls.Add(this.LabelHallPropertiesHeader);
            this.PanelResults.Controls.Add(this.RichTextboxHallInNorthPos4);
            this.PanelResults.Controls.Add(this.RichTextboxHallInNorthPos3);
            this.PanelResults.Controls.Add(this.RichTextboxHallInNorthPos2);
            this.PanelResults.Controls.Add(this.RichTextboxHallInNorthPos1);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth1);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth2);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth4);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth3);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth4Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth3Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth2Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInNorth1Unit);
            this.PanelResults.Controls.Add(this.RichTextboxHallInSouthPos4);
            this.PanelResults.Controls.Add(this.RichTextboxHallInSouthPos3);
            this.PanelResults.Controls.Add(this.RichTextboxHallInSouthPos2);
            this.PanelResults.Controls.Add(this.RichTextboxHallInSouthPos1);
            this.PanelResults.Controls.Add(this.LabelHallInVoltageHeader);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth1);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth2);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth4);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth3);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth4Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth3Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth2Unit);
            this.PanelResults.Controls.Add(this.TextboxHallInSouth1Unit);
            this.PanelResults.Dock = System.Windows.Forms.DockStyle.Right;
            this.PanelResults.Location = new System.Drawing.Point(840, 0);
            this.PanelResults.Name = "PanelResults";
            this.PanelResults.Size = new System.Drawing.Size(420, 820);
            this.PanelResults.TabIndex = 127;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(305, 28);
            this.label1.TabIndex = 238;
            this.label1.Text = "HALL MEASUREMENT RESULTS";
            // 
            // IconbuttonNType
            // 
            this.IconbuttonNType.BackColor = System.Drawing.Color.Snow;
            this.IconbuttonNType.Enabled = false;
            this.IconbuttonNType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IconbuttonNType.IconChar = FontAwesome.Sharp.IconChar.N;
            this.IconbuttonNType.IconColor = System.Drawing.Color.Black;
            this.IconbuttonNType.IconFont = FontAwesome.Sharp.IconFont.Brands;
            this.IconbuttonNType.IconSize = 24;
            this.IconbuttonNType.Location = new System.Drawing.Point(196, 779);
            this.IconbuttonNType.Name = "IconbuttonNType";
            this.IconbuttonNType.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonNType.TabIndex = 235;
            this.IconbuttonNType.UseVisualStyleBackColor = false;
            // 
            // IconbuttonPType
            // 
            this.IconbuttonPType.BackColor = System.Drawing.Color.Snow;
            this.IconbuttonPType.Enabled = false;
            this.IconbuttonPType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IconbuttonPType.IconChar = FontAwesome.Sharp.IconChar.P;
            this.IconbuttonPType.IconColor = System.Drawing.Color.Black;
            this.IconbuttonPType.IconFont = FontAwesome.Sharp.IconFont.Brands;
            this.IconbuttonPType.IconSize = 24;
            this.IconbuttonPType.Location = new System.Drawing.Point(287, 779);
            this.IconbuttonPType.Name = "IconbuttonPType";
            this.IconbuttonPType.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonPType.TabIndex = 236;
            this.IconbuttonPType.UseVisualStyleBackColor = false;
            // 
            // TextboxHallResUnit
            // 
            this.TextboxHallResUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallResUnit.Location = new System.Drawing.Point(325, 554);
            this.TextboxHallResUnit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallResUnit.Name = "TextboxHallResUnit";
            this.TextboxHallResUnit.ReadOnly = true;
            this.TextboxHallResUnit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallResUnit.TabIndex = 237;
            this.TextboxHallResUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabelTypes
            // 
            this.LabelTypes.AutoSize = true;
            this.LabelTypes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTypes.Location = new System.Drawing.Point(105, 780);
            this.LabelTypes.Name = "LabelTypes";
            this.LabelTypes.Size = new System.Drawing.Size(59, 28);
            this.LabelTypes.TabIndex = 234;
            this.LabelTypes.Text = "TYPE";
            // 
            // RichTextboxHallCoefficientUnit
            // 
            this.RichTextboxHallCoefficientUnit.BackColor = System.Drawing.SystemColors.Control;
            this.RichTextboxHallCoefficientUnit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallCoefficientUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallCoefficientUnit.Location = new System.Drawing.Point(325, 599);
            this.RichTextboxHallCoefficientUnit.Name = "RichTextboxHallCoefficientUnit";
            this.RichTextboxHallCoefficientUnit.ReadOnly = true;
            this.RichTextboxHallCoefficientUnit.Size = new System.Drawing.Size(50, 27);
            this.RichTextboxHallCoefficientUnit.TabIndex = 233;
            this.RichTextboxHallCoefficientUnit.Text = "";
            // 
            // RichTextboxMobilityUnit
            // 
            this.RichTextboxMobilityUnit.BackColor = System.Drawing.SystemColors.Control;
            this.RichTextboxMobilityUnit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxMobilityUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxMobilityUnit.ForeColor = System.Drawing.Color.Black;
            this.RichTextboxMobilityUnit.Location = new System.Drawing.Point(325, 734);
            this.RichTextboxMobilityUnit.Name = "RichTextboxMobilityUnit";
            this.RichTextboxMobilityUnit.ReadOnly = true;
            this.RichTextboxMobilityUnit.Size = new System.Drawing.Size(50, 27);
            this.RichTextboxMobilityUnit.TabIndex = 232;
            this.RichTextboxMobilityUnit.Text = "";
            // 
            // RichTextboxSheetConcentrationUnit
            // 
            this.RichTextboxSheetConcentrationUnit.BackColor = System.Drawing.SystemColors.Control;
            this.RichTextboxSheetConcentrationUnit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxSheetConcentrationUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxSheetConcentrationUnit.ForeColor = System.Drawing.Color.Black;
            this.RichTextboxSheetConcentrationUnit.Location = new System.Drawing.Point(325, 689);
            this.RichTextboxSheetConcentrationUnit.Name = "RichTextboxSheetConcentrationUnit";
            this.RichTextboxSheetConcentrationUnit.ReadOnly = true;
            this.RichTextboxSheetConcentrationUnit.Size = new System.Drawing.Size(50, 27);
            this.RichTextboxSheetConcentrationUnit.TabIndex = 229;
            this.RichTextboxSheetConcentrationUnit.Text = "";
            // 
            // RichTextboxMobility
            // 
            this.RichTextboxMobility.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxMobility.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxMobility.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxMobility.Location = new System.Drawing.Point(160, 735);
            this.RichTextboxMobility.Name = "RichTextboxMobility";
            this.RichTextboxMobility.Size = new System.Drawing.Size(30, 35);
            this.RichTextboxMobility.TabIndex = 231;
            this.RichTextboxMobility.Text = "µ:";
            // 
            // RichTextboxSheetConcentration
            // 
            this.RichTextboxSheetConcentration.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxSheetConcentration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxSheetConcentration.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxSheetConcentration.Location = new System.Drawing.Point(107, 690);
            this.RichTextboxSheetConcentration.Name = "RichTextboxSheetConcentration";
            this.RichTextboxSheetConcentration.Size = new System.Drawing.Size(75, 35);
            this.RichTextboxSheetConcentration.TabIndex = 228;
            this.RichTextboxSheetConcentration.Text = "nSheet:";
            // 
            // TextboxMobility
            // 
            this.TextboxMobility.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMobility.Location = new System.Drawing.Point(200, 734);
            this.TextboxMobility.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxMobility.Name = "TextboxMobility";
            this.TextboxMobility.ReadOnly = true;
            this.TextboxMobility.Size = new System.Drawing.Size(100, 27);
            this.TextboxMobility.TabIndex = 230;
            this.TextboxMobility.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxSheetConcentration
            // 
            this.TextboxSheetConcentration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSheetConcentration.Location = new System.Drawing.Point(200, 689);
            this.TextboxSheetConcentration.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxSheetConcentration.Name = "TextboxSheetConcentration";
            this.TextboxSheetConcentration.ReadOnly = true;
            this.TextboxSheetConcentration.Size = new System.Drawing.Size(100, 27);
            this.TextboxSheetConcentration.TabIndex = 227;
            this.TextboxSheetConcentration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RichTextboxBulkConcentrationUnit
            // 
            this.RichTextboxBulkConcentrationUnit.BackColor = System.Drawing.SystemColors.Control;
            this.RichTextboxBulkConcentrationUnit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxBulkConcentrationUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxBulkConcentrationUnit.ForeColor = System.Drawing.Color.Black;
            this.RichTextboxBulkConcentrationUnit.Location = new System.Drawing.Point(325, 644);
            this.RichTextboxBulkConcentrationUnit.Name = "RichTextboxBulkConcentrationUnit";
            this.RichTextboxBulkConcentrationUnit.ReadOnly = true;
            this.RichTextboxBulkConcentrationUnit.Size = new System.Drawing.Size(50, 27);
            this.RichTextboxBulkConcentrationUnit.TabIndex = 229;
            this.RichTextboxBulkConcentrationUnit.Text = "";
            // 
            // RichTextboxHallRes
            // 
            this.RichTextboxHallRes.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallRes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallRes.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallRes.Location = new System.Drawing.Point(130, 555);
            this.RichTextboxHallRes.Name = "RichTextboxHallRes";
            this.RichTextboxHallRes.Size = new System.Drawing.Size(60, 35);
            this.RichTextboxHallRes.TabIndex = 226;
            this.RichTextboxHallRes.Text = "RHall:";
            // 
            // RichTextboxBulkConcentration
            // 
            this.RichTextboxBulkConcentration.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxBulkConcentration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxBulkConcentration.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxBulkConcentration.Location = new System.Drawing.Point(118, 645);
            this.RichTextboxBulkConcentration.Name = "RichTextboxBulkConcentration";
            this.RichTextboxBulkConcentration.Size = new System.Drawing.Size(60, 35);
            this.RichTextboxBulkConcentration.TabIndex = 228;
            this.RichTextboxBulkConcentration.Text = "nBulk:";
            // 
            // TextboxHallRes
            // 
            this.TextboxHallRes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallRes.Location = new System.Drawing.Point(200, 554);
            this.TextboxHallRes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallRes.Name = "TextboxHallRes";
            this.TextboxHallRes.ReadOnly = true;
            this.TextboxHallRes.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallRes.TabIndex = 225;
            this.TextboxHallRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxBulkConcentration
            // 
            this.TextboxBulkConcentration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxBulkConcentration.Location = new System.Drawing.Point(200, 644);
            this.TextboxBulkConcentration.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxBulkConcentration.Name = "TextboxBulkConcentration";
            this.TextboxBulkConcentration.ReadOnly = true;
            this.TextboxBulkConcentration.Size = new System.Drawing.Size(100, 27);
            this.TextboxBulkConcentration.TabIndex = 227;
            this.TextboxBulkConcentration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RichTextboxHallVoltage
            // 
            this.RichTextboxHallVoltage.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallVoltage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallVoltage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallVoltage.Location = new System.Drawing.Point(130, 510);
            this.RichTextboxHallVoltage.Name = "RichTextboxHallVoltage";
            this.RichTextboxHallVoltage.Size = new System.Drawing.Size(50, 35);
            this.RichTextboxHallVoltage.TabIndex = 206;
            this.RichTextboxHallVoltage.Text = "VH:";
            // 
            // RichTextboxHallCoefficient
            // 
            this.RichTextboxHallCoefficient.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallCoefficient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallCoefficient.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallCoefficient.Location = new System.Drawing.Point(130, 600);
            this.RichTextboxHallCoefficient.Name = "RichTextboxHallCoefficient";
            this.RichTextboxHallCoefficient.Size = new System.Drawing.Size(50, 35);
            this.RichTextboxHallCoefficient.TabIndex = 226;
            this.RichTextboxHallCoefficient.Text = "RH:";
            // 
            // TextboxHallVoltage
            // 
            this.TextboxHallVoltage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallVoltage.Location = new System.Drawing.Point(200, 509);
            this.TextboxHallVoltage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallVoltage.Name = "TextboxHallVoltage";
            this.TextboxHallVoltage.ReadOnly = true;
            this.TextboxHallVoltage.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallVoltage.TabIndex = 204;
            this.TextboxHallVoltage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallCoefficient
            // 
            this.TextboxHallCoefficient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallCoefficient.Location = new System.Drawing.Point(200, 599);
            this.TextboxHallCoefficient.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallCoefficient.Name = "TextboxHallCoefficient";
            this.TextboxHallCoefficient.ReadOnly = true;
            this.TextboxHallCoefficient.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallCoefficient.TabIndex = 225;
            this.TextboxHallCoefficient.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallVoltageUnit
            // 
            this.TextboxHallVoltageUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallVoltageUnit.Location = new System.Drawing.Point(325, 509);
            this.TextboxHallVoltageUnit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallVoltageUnit.Name = "TextboxHallVoltageUnit";
            this.TextboxHallVoltageUnit.ReadOnly = true;
            this.TextboxHallVoltageUnit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallVoltageUnit.TabIndex = 205;
            this.TextboxHallVoltageUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabelHallPropertiesHeader
            // 
            this.LabelHallPropertiesHeader.AutoSize = true;
            this.LabelHallPropertiesHeader.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallPropertiesHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallPropertiesHeader.Location = new System.Drawing.Point(30, 465);
            this.LabelHallPropertiesHeader.Name = "LabelHallPropertiesHeader";
            this.LabelHallPropertiesHeader.Size = new System.Drawing.Size(183, 28);
            this.LabelHallPropertiesHeader.TabIndex = 194;
            this.LabelHallPropertiesHeader.Text = "HALL PROPERTIES";
            // 
            // RichTextboxHallInNorthPos4
            // 
            this.RichTextboxHallInNorthPos4.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInNorthPos4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInNorthPos4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInNorthPos4.Location = new System.Drawing.Point(100, 420);
            this.RichTextboxHallInNorthPos4.Name = "RichTextboxHallInNorthPos4";
            this.RichTextboxHallInNorthPos4.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInNorthPos4.TabIndex = 189;
            this.RichTextboxHallInNorthPos4.Text = "VHN4:";
            // 
            // RichTextboxHallInNorthPos3
            // 
            this.RichTextboxHallInNorthPos3.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInNorthPos3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInNorthPos3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInNorthPos3.Location = new System.Drawing.Point(100, 375);
            this.RichTextboxHallInNorthPos3.Name = "RichTextboxHallInNorthPos3";
            this.RichTextboxHallInNorthPos3.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInNorthPos3.TabIndex = 188;
            this.RichTextboxHallInNorthPos3.Text = "VHN3:";
            // 
            // RichTextboxHallInNorthPos2
            // 
            this.RichTextboxHallInNorthPos2.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInNorthPos2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInNorthPos2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInNorthPos2.Location = new System.Drawing.Point(100, 330);
            this.RichTextboxHallInNorthPos2.Name = "RichTextboxHallInNorthPos2";
            this.RichTextboxHallInNorthPos2.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInNorthPos2.TabIndex = 187;
            this.RichTextboxHallInNorthPos2.Text = "VHN2:";
            // 
            // RichTextboxHallInNorthPos1
            // 
            this.RichTextboxHallInNorthPos1.BackColor = System.Drawing.Color.DarkGray;
            this.RichTextboxHallInNorthPos1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextboxHallInNorthPos1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextboxHallInNorthPos1.Location = new System.Drawing.Point(100, 285);
            this.RichTextboxHallInNorthPos1.Name = "RichTextboxHallInNorthPos1";
            this.RichTextboxHallInNorthPos1.Size = new System.Drawing.Size(70, 35);
            this.RichTextboxHallInNorthPos1.TabIndex = 186;
            this.RichTextboxHallInNorthPos1.Text = "VHN1:";
            // 
            // TextboxHallInNorth1
            // 
            this.TextboxHallInNorth1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth1.Location = new System.Drawing.Point(200, 284);
            this.TextboxHallInNorth1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth1.Name = "TextboxHallInNorth1";
            this.TextboxHallInNorth1.ReadOnly = true;
            this.TextboxHallInNorth1.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInNorth1.TabIndex = 178;
            this.TextboxHallInNorth1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth2
            // 
            this.TextboxHallInNorth2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth2.Location = new System.Drawing.Point(200, 329);
            this.TextboxHallInNorth2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth2.Name = "TextboxHallInNorth2";
            this.TextboxHallInNorth2.ReadOnly = true;
            this.TextboxHallInNorth2.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInNorth2.TabIndex = 179;
            this.TextboxHallInNorth2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth4
            // 
            this.TextboxHallInNorth4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth4.Location = new System.Drawing.Point(200, 419);
            this.TextboxHallInNorth4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth4.Name = "TextboxHallInNorth4";
            this.TextboxHallInNorth4.ReadOnly = true;
            this.TextboxHallInNorth4.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInNorth4.TabIndex = 181;
            this.TextboxHallInNorth4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth3
            // 
            this.TextboxHallInNorth3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth3.Location = new System.Drawing.Point(200, 374);
            this.TextboxHallInNorth3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth3.Name = "TextboxHallInNorth3";
            this.TextboxHallInNorth3.ReadOnly = true;
            this.TextboxHallInNorth3.Size = new System.Drawing.Size(100, 27);
            this.TextboxHallInNorth3.TabIndex = 180;
            this.TextboxHallInNorth3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth4Unit
            // 
            this.TextboxHallInNorth4Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth4Unit.Location = new System.Drawing.Point(325, 419);
            this.TextboxHallInNorth4Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth4Unit.Name = "TextboxHallInNorth4Unit";
            this.TextboxHallInNorth4Unit.ReadOnly = true;
            this.TextboxHallInNorth4Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInNorth4Unit.TabIndex = 185;
            this.TextboxHallInNorth4Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth3Unit
            // 
            this.TextboxHallInNorth3Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth3Unit.Location = new System.Drawing.Point(325, 374);
            this.TextboxHallInNorth3Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth3Unit.Name = "TextboxHallInNorth3Unit";
            this.TextboxHallInNorth3Unit.ReadOnly = true;
            this.TextboxHallInNorth3Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInNorth3Unit.TabIndex = 184;
            this.TextboxHallInNorth3Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth2Unit
            // 
            this.TextboxHallInNorth2Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth2Unit.Location = new System.Drawing.Point(325, 329);
            this.TextboxHallInNorth2Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth2Unit.Name = "TextboxHallInNorth2Unit";
            this.TextboxHallInNorth2Unit.ReadOnly = true;
            this.TextboxHallInNorth2Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInNorth2Unit.TabIndex = 183;
            this.TextboxHallInNorth2Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TextboxHallInNorth1Unit
            // 
            this.TextboxHallInNorth1Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxHallInNorth1Unit.Location = new System.Drawing.Point(325, 284);
            this.TextboxHallInNorth1Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxHallInNorth1Unit.Name = "TextboxHallInNorth1Unit";
            this.TextboxHallInNorth1Unit.ReadOnly = true;
            this.TextboxHallInNorth1Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxHallInNorth1Unit.TabIndex = 182;
            this.TextboxHallInNorth1Unit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ChartHallVoltageResults
            // 
            this.ChartHallVoltageResults.BackColor = System.Drawing.Color.Transparent;
            this.ChartHallVoltageResults.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea1.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.Title = "Current (A)";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MinorGrid.Enabled = true;
            chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.DimGray;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.Title = "Voltage (V)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.BackSecondaryColor = System.Drawing.Color.Silver;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 90F;
            chartArea1.Position.Width = 90F;
            chartArea1.Position.X = 3F;
            chartArea1.Position.Y = 3F;
            this.ChartHallVoltageResults.ChartAreas.Add(chartArea1);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.AutoFitMinFontSize = 8;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Font = new System.Drawing.Font("Segoe UI Semibold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            legend1.TitleBackColor = System.Drawing.Color.Transparent;
            this.ChartHallVoltageResults.Legends.Add(legend1);
            this.ChartHallVoltageResults.Location = new System.Drawing.Point(20, 55);
            this.ChartHallVoltageResults.Name = "ChartHallVoltageResults";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Transparent;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.ChartHallVoltageResults.Series.Add(series1);
            this.ChartHallVoltageResults.Size = new System.Drawing.Size(800, 640);
            this.ChartHallVoltageResults.TabIndex = 0;
            // 
            // HallMeasurementResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1260, 820);
            this.Controls.Add(this.PanelResultsChart);
            this.Controls.Add(this.PanelMeasureAndSource);
            this.Controls.Add(this.PanelResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HallMeasurementResultsForm";
            this.Text = "Hall effect Measurement Measurement Results Form";
            this.PanelResultsChart.ResumeLayout(false);
            this.PanelMeasureAndSource.ResumeLayout(false);
            this.PanelMeasureAndSource.PerformLayout();
            this.PanelResults.ResumeLayout(false);
            this.PanelResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartHallVoltageResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelResultsChart;
        private System.Windows.Forms.Panel PanelMeasureAndSource;
        private System.Windows.Forms.TextBox TextboxSourceMode;
        private System.Windows.Forms.TextBox TextboxMeasureMode;
        private System.Windows.Forms.Label LabelMeasureMode;
        private System.Windows.Forms.Label LabelSourceMode;
        private System.Windows.Forms.TextBox TextboxHallInSouth1Unit;
        private System.Windows.Forms.TextBox TextboxHallInSouth2Unit;
        private System.Windows.Forms.TextBox TextboxHallInSouth3Unit;
        private System.Windows.Forms.TextBox TextboxHallInSouth4Unit;
        private System.Windows.Forms.TextBox TextboxHallInSouth3;
        private System.Windows.Forms.TextBox TextboxHallInSouth4;
        private System.Windows.Forms.TextBox TextboxHallInSouth2;
        private System.Windows.Forms.TextBox TextboxHallInSouth1;
        private System.Windows.Forms.Label LabelHallInVoltageHeader;
        private System.Windows.Forms.RichTextBox RichTextboxHallInSouthPos1;
        private System.Windows.Forms.RichTextBox RichTextboxHallInSouthPos2;
        private System.Windows.Forms.RichTextBox RichTextboxHallInSouthPos3;
        private System.Windows.Forms.RichTextBox RichTextboxHallInSouthPos4;
        private System.Windows.Forms.Panel PanelResults;
        private System.Windows.Forms.RichTextBox RichTextboxHallInNorthPos4;
        private System.Windows.Forms.RichTextBox RichTextboxHallInNorthPos3;
        private System.Windows.Forms.RichTextBox RichTextboxHallInNorthPos2;
        private System.Windows.Forms.RichTextBox RichTextboxHallInNorthPos1;
        private System.Windows.Forms.TextBox TextboxHallInNorth1;
        private System.Windows.Forms.TextBox TextboxHallInNorth2;
        private System.Windows.Forms.TextBox TextboxHallInNorth4;
        private System.Windows.Forms.TextBox TextboxHallInNorth3;
        private System.Windows.Forms.TextBox TextboxHallInNorth4Unit;
        private System.Windows.Forms.TextBox TextboxHallInNorth2Unit;
        private System.Windows.Forms.TextBox TextboxHallInNorth1Unit;
        private System.Windows.Forms.Label LabelHallPropertiesHeader;
        private System.Windows.Forms.RichTextBox RichTextboxHallVoltage;
        private System.Windows.Forms.TextBox TextboxHallVoltage;
        private System.Windows.Forms.TextBox TextboxHallVoltageUnit;
        private System.Windows.Forms.RichTextBox RichTextboxSheetConcentrationUnit;
        private System.Windows.Forms.RichTextBox RichTextboxSheetConcentration;
        private System.Windows.Forms.TextBox TextboxSheetConcentration;
        private System.Windows.Forms.RichTextBox RichTextboxHallRes;
        private System.Windows.Forms.TextBox TextboxHallRes;
        private System.Windows.Forms.RichTextBox RichTextboxHallCoefficientUnit;
        private System.Windows.Forms.RichTextBox RichTextboxMobilityUnit;
        private System.Windows.Forms.RichTextBox RichTextboxMobility;
        private System.Windows.Forms.TextBox TextboxMobility;
        private System.Windows.Forms.RichTextBox RichTextboxBulkConcentrationUnit;
        private System.Windows.Forms.RichTextBox RichTextboxBulkConcentration;
        private System.Windows.Forms.TextBox TextboxBulkConcentration;
        private System.Windows.Forms.RichTextBox RichTextboxHallCoefficient;
        private System.Windows.Forms.TextBox TextboxHallCoefficient;
        private FontAwesome.Sharp.IconButton IconbuttonPType;
        private FontAwesome.Sharp.IconButton IconbuttonNType;
        private System.Windows.Forms.Label LabelTypes;
        private System.Windows.Forms.TextBox TextboxHallResUnit;
        private System.Windows.Forms.TextBox TextboxHallInNorth3Unit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartHallVoltageResults;
    }
}