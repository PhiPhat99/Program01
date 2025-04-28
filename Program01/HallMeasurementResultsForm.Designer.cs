namespace Program01
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.PanelResultsChart = new System.Windows.Forms.Panel();
            this.ChartHallTotalVoltages = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.PanelMeasureAndSource = new System.Windows.Forms.Panel();
            this.TextboxSourceMode = new System.Windows.Forms.TextBox();
            this.TextboxMeasureMode = new System.Windows.Forms.TextBox();
            this.LabelMeasureMode = new System.Windows.Forms.Label();
            this.LabelSourceMode = new System.Windows.Forms.Label();
            this.PanelResults = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox27 = new System.Windows.Forms.TextBox();
            this.textBox28 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox23 = new System.Windows.Forms.TextBox();
            this.textBox24 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox25 = new System.Windows.Forms.TextBox();
            this.textBox26 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.textBox22 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.textBox20 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.LabelHallOutVoltages = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.TextboxRes1 = new System.Windows.Forms.TextBox();
            this.LabelHallOutVoltage2 = new System.Windows.Forms.Label();
            this.TextboxRes2 = new System.Windows.Forms.TextBox();
            this.LabelHallOutVoltage1 = new System.Windows.Forms.Label();
            this.LabelHallOutVoltage4 = new System.Windows.Forms.Label();
            this.TextboxRes4 = new System.Windows.Forms.TextBox();
            this.LabelHallOutVoltage3 = new System.Windows.Forms.Label();
            this.TextboxRes3 = new System.Windows.Forms.TextBox();
            this.LabelResistance5 = new System.Windows.Forms.Label();
            this.TextboxRes5 = new System.Windows.Forms.TextBox();
            this.TextboxRes5Unit = new System.Windows.Forms.TextBox();
            this.TextboxRes4Unit = new System.Windows.Forms.TextBox();
            this.TextboxRes3Unit = new System.Windows.Forms.TextBox();
            this.TextboxRes2Unit = new System.Windows.Forms.TextBox();
            this.TextboxRes1Unit = new System.Windows.Forms.TextBox();
            this.PanelResultsChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartHallTotalVoltages)).BeginInit();
            this.PanelMeasureAndSource.SuspendLayout();
            this.PanelResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelResultsChart
            // 
            this.PanelResultsChart.BackColor = System.Drawing.Color.Transparent;
            this.PanelResultsChart.Controls.Add(this.ChartHallTotalVoltages);
            this.PanelResultsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelResultsChart.Location = new System.Drawing.Point(0, 80);
            this.PanelResultsChart.Name = "PanelResultsChart";
            this.PanelResultsChart.Size = new System.Drawing.Size(840, 740);
            this.PanelResultsChart.TabIndex = 129;
            // 
            // ChartHallTotalVoltages
            // 
            chartArea4.Name = "ChartArea1";
            this.ChartHallTotalVoltages.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.ChartHallTotalVoltages.Legends.Add(legend4);
            this.ChartHallTotalVoltages.Location = new System.Drawing.Point(60, 50);
            this.ChartHallTotalVoltages.Name = "ChartHallTotalVoltages";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.ChartHallTotalVoltages.Series.Add(series4);
            this.ChartHallTotalVoltages.Size = new System.Drawing.Size(720, 600);
            this.ChartHallTotalVoltages.TabIndex = 124;
            this.ChartHallTotalVoltages.Text = "chart1";
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
            this.TextboxSourceMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSourceMode.Location = new System.Drawing.Point(476, 26);
            this.TextboxSourceMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxSourceMode.Name = "TextboxSourceMode";
            this.TextboxSourceMode.ReadOnly = true;
            this.TextboxSourceMode.Size = new System.Drawing.Size(150, 30);
            this.TextboxSourceMode.TabIndex = 61;
            // 
            // TextboxMeasureMode
            // 
            this.TextboxMeasureMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMeasureMode.Location = new System.Drawing.Point(170, 26);
            this.TextboxMeasureMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxMeasureMode.Name = "TextboxMeasureMode";
            this.TextboxMeasureMode.ReadOnly = true;
            this.TextboxMeasureMode.Size = new System.Drawing.Size(150, 30);
            this.TextboxMeasureMode.TabIndex = 60;
            // 
            // LabelMeasureMode
            // 
            this.LabelMeasureMode.AutoSize = true;
            this.LabelMeasureMode.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMeasureMode.Location = new System.Drawing.Point(40, 25);
            this.LabelMeasureMode.Name = "LabelMeasureMode";
            this.LabelMeasureMode.Size = new System.Drawing.Size(111, 28);
            this.LabelMeasureMode.TabIndex = 58;
            this.LabelMeasureMode.Text = "MEASURE :";
            // 
            // LabelSourceMode
            // 
            this.LabelSourceMode.AutoSize = true;
            this.LabelSourceMode.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceMode.Location = new System.Drawing.Point(360, 25);
            this.LabelSourceMode.Name = "LabelSourceMode";
            this.LabelSourceMode.Size = new System.Drawing.Size(97, 28);
            this.LabelSourceMode.TabIndex = 56;
            this.LabelSourceMode.Text = "SOURCE :";
            // 
            // PanelResults
            // 
            this.PanelResults.BackColor = System.Drawing.Color.DarkGray;
            this.PanelResults.Controls.Add(this.label15);
            this.PanelResults.Controls.Add(this.textBox27);
            this.PanelResults.Controls.Add(this.textBox28);
            this.PanelResults.Controls.Add(this.label13);
            this.PanelResults.Controls.Add(this.textBox23);
            this.PanelResults.Controls.Add(this.textBox24);
            this.PanelResults.Controls.Add(this.label14);
            this.PanelResults.Controls.Add(this.textBox25);
            this.PanelResults.Controls.Add(this.textBox26);
            this.PanelResults.Controls.Add(this.label12);
            this.PanelResults.Controls.Add(this.textBox21);
            this.PanelResults.Controls.Add(this.textBox22);
            this.PanelResults.Controls.Add(this.label11);
            this.PanelResults.Controls.Add(this.textBox19);
            this.PanelResults.Controls.Add(this.textBox20);
            this.PanelResults.Controls.Add(this.label1);
            this.PanelResults.Controls.Add(this.textBox17);
            this.PanelResults.Controls.Add(this.textBox18);
            this.PanelResults.Controls.Add(this.textBox5);
            this.PanelResults.Controls.Add(this.label6);
            this.PanelResults.Controls.Add(this.textBox6);
            this.PanelResults.Controls.Add(this.label8);
            this.PanelResults.Controls.Add(this.label9);
            this.PanelResults.Controls.Add(this.textBox11);
            this.PanelResults.Controls.Add(this.label10);
            this.PanelResults.Controls.Add(this.textBox12);
            this.PanelResults.Controls.Add(this.textBox13);
            this.PanelResults.Controls.Add(this.textBox14);
            this.PanelResults.Controls.Add(this.textBox15);
            this.PanelResults.Controls.Add(this.textBox16);
            this.PanelResults.Controls.Add(this.LabelHallOutVoltages);
            this.PanelResults.Controls.Add(this.label7);
            this.PanelResults.Controls.Add(this.textBox1);
            this.PanelResults.Controls.Add(this.label2);
            this.PanelResults.Controls.Add(this.textBox2);
            this.PanelResults.Controls.Add(this.label3);
            this.PanelResults.Controls.Add(this.label4);
            this.PanelResults.Controls.Add(this.textBox3);
            this.PanelResults.Controls.Add(this.label5);
            this.PanelResults.Controls.Add(this.textBox4);
            this.PanelResults.Controls.Add(this.textBox7);
            this.PanelResults.Controls.Add(this.textBox8);
            this.PanelResults.Controls.Add(this.textBox9);
            this.PanelResults.Controls.Add(this.textBox10);
            this.PanelResults.Controls.Add(this.TextboxRes1);
            this.PanelResults.Controls.Add(this.LabelHallOutVoltage2);
            this.PanelResults.Controls.Add(this.TextboxRes2);
            this.PanelResults.Controls.Add(this.LabelHallOutVoltage1);
            this.PanelResults.Controls.Add(this.LabelHallOutVoltage4);
            this.PanelResults.Controls.Add(this.TextboxRes4);
            this.PanelResults.Controls.Add(this.LabelHallOutVoltage3);
            this.PanelResults.Controls.Add(this.TextboxRes3);
            this.PanelResults.Controls.Add(this.LabelResistance5);
            this.PanelResults.Controls.Add(this.TextboxRes5);
            this.PanelResults.Controls.Add(this.TextboxRes5Unit);
            this.PanelResults.Controls.Add(this.TextboxRes4Unit);
            this.PanelResults.Controls.Add(this.TextboxRes3Unit);
            this.PanelResults.Controls.Add(this.TextboxRes2Unit);
            this.PanelResults.Controls.Add(this.TextboxRes1Unit);
            this.PanelResults.Dock = System.Windows.Forms.DockStyle.Right;
            this.PanelResults.Location = new System.Drawing.Point(840, 0);
            this.PanelResults.Name = "PanelResults";
            this.PanelResults.Size = new System.Drawing.Size(420, 820);
            this.PanelResults.TabIndex = 127;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(114, 750);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(82, 23);
            this.label15.TabIndex = 167;
            this.label15.Text = "Mobility :";
            // 
            // textBox27
            // 
            this.textBox27.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox27.Location = new System.Drawing.Point(200, 749);
            this.textBox27.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox27.Name = "textBox27";
            this.textBox27.ReadOnly = true;
            this.textBox27.Size = new System.Drawing.Size(100, 27);
            this.textBox27.TabIndex = 168;
            // 
            // textBox28
            // 
            this.textBox28.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox28.Location = new System.Drawing.Point(325, 749);
            this.textBox28.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox28.Name = "textBox28";
            this.textBox28.ReadOnly = true;
            this.textBox28.Size = new System.Drawing.Size(50, 27);
            this.textBox28.TabIndex = 169;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(20, 715);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(176, 23);
            this.label13.TabIndex = 164;
            this.label13.Text = "Sheet Concentration :";
            // 
            // textBox23
            // 
            this.textBox23.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox23.Location = new System.Drawing.Point(200, 714);
            this.textBox23.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox23.Name = "textBox23";
            this.textBox23.ReadOnly = true;
            this.textBox23.Size = new System.Drawing.Size(100, 27);
            this.textBox23.TabIndex = 165;
            // 
            // textBox24
            // 
            this.textBox24.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox24.Location = new System.Drawing.Point(325, 714);
            this.textBox24.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox24.Name = "textBox24";
            this.textBox24.ReadOnly = true;
            this.textBox24.Size = new System.Drawing.Size(50, 27);
            this.textBox24.TabIndex = 166;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(31, 680);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(166, 23);
            this.label14.TabIndex = 161;
            this.label14.Text = "Bulk Concentration :";
            // 
            // textBox25
            // 
            this.textBox25.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox25.Location = new System.Drawing.Point(200, 679);
            this.textBox25.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox25.Name = "textBox25";
            this.textBox25.ReadOnly = true;
            this.textBox25.Size = new System.Drawing.Size(100, 27);
            this.textBox25.TabIndex = 162;
            // 
            // textBox26
            // 
            this.textBox26.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox26.Location = new System.Drawing.Point(325, 679);
            this.textBox26.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox26.Name = "textBox26";
            this.textBox26.ReadOnly = true;
            this.textBox26.Size = new System.Drawing.Size(50, 27);
            this.textBox26.TabIndex = 163;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(39, 645);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(158, 23);
            this.label12.TabIndex = 158;
            this.label12.Text = "1 / Hall Resistance :";
            // 
            // textBox21
            // 
            this.textBox21.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox21.Location = new System.Drawing.Point(200, 644);
            this.textBox21.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox21.Name = "textBox21";
            this.textBox21.ReadOnly = true;
            this.textBox21.Size = new System.Drawing.Size(100, 27);
            this.textBox21.TabIndex = 159;
            // 
            // textBox22
            // 
            this.textBox22.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox22.Location = new System.Drawing.Point(325, 644);
            this.textBox22.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox22.Name = "textBox22";
            this.textBox22.ReadOnly = true;
            this.textBox22.Size = new System.Drawing.Size(50, 27);
            this.textBox22.TabIndex = 160;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(62, 610);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(134, 23);
            this.label11.TabIndex = 155;
            this.label11.Text = "Hall Resistance :";
            // 
            // textBox19
            // 
            this.textBox19.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox19.Location = new System.Drawing.Point(200, 609);
            this.textBox19.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox19.Name = "textBox19";
            this.textBox19.ReadOnly = true;
            this.textBox19.Size = new System.Drawing.Size(100, 27);
            this.textBox19.TabIndex = 156;
            // 
            // textBox20
            // 
            this.textBox20.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox20.Location = new System.Drawing.Point(325, 609);
            this.textBox20.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox20.Name = "textBox20";
            this.textBox20.ReadOnly = true;
            this.textBox20.Size = new System.Drawing.Size(50, 27);
            this.textBox20.TabIndex = 157;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(42, 555);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 23);
            this.label1.TabIndex = 152;
            this.label1.Text = "Hall In Resistance :";
            // 
            // textBox17
            // 
            this.textBox17.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox17.Location = new System.Drawing.Point(201, 554);
            this.textBox17.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox17.Name = "textBox17";
            this.textBox17.ReadOnly = true;
            this.textBox17.Size = new System.Drawing.Size(100, 27);
            this.textBox17.TabIndex = 153;
            // 
            // textBox18
            // 
            this.textBox18.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox18.Location = new System.Drawing.Point(325, 554);
            this.textBox18.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox18.Name = "textBox18";
            this.textBox18.ReadOnly = true;
            this.textBox18.Size = new System.Drawing.Size(50, 27);
            this.textBox18.TabIndex = 154;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(200, 484);
            this.textBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(100, 27);
            this.textBox5.TabIndex = 141;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(54, 520);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 23);
            this.label6.TabIndex = 142;
            this.label6.Text = "North Voltage 4 :";
            // 
            // textBox6
            // 
            this.textBox6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(200, 519);
            this.textBox6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(100, 27);
            this.textBox6.TabIndex = 143;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(56, 485);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(141, 23);
            this.label8.TabIndex = 140;
            this.label8.Text = "North Voltage 3 :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(56, 450);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(141, 23);
            this.label9.TabIndex = 146;
            this.label9.Text = "North Voltage 2 :";
            // 
            // textBox11
            // 
            this.textBox11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(200, 449);
            this.textBox11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(100, 27);
            this.textBox11.TabIndex = 147;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(58, 415);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(139, 23);
            this.label10.TabIndex = 144;
            this.label10.Text = "North Voltage 1 :";
            // 
            // textBox12
            // 
            this.textBox12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(200, 414);
            this.textBox12.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.Size = new System.Drawing.Size(100, 27);
            this.textBox12.TabIndex = 145;
            // 
            // textBox13
            // 
            this.textBox13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(325, 449);
            this.textBox13.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(50, 27);
            this.textBox13.TabIndex = 151;
            // 
            // textBox14
            // 
            this.textBox14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(325, 414);
            this.textBox14.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox14.Name = "textBox14";
            this.textBox14.ReadOnly = true;
            this.textBox14.Size = new System.Drawing.Size(50, 27);
            this.textBox14.TabIndex = 150;
            // 
            // textBox15
            // 
            this.textBox15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(325, 484);
            this.textBox15.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox15.Name = "textBox15";
            this.textBox15.ReadOnly = true;
            this.textBox15.Size = new System.Drawing.Size(50, 27);
            this.textBox15.TabIndex = 149;
            // 
            // textBox16
            // 
            this.textBox16.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox16.Location = new System.Drawing.Point(325, 519);
            this.textBox16.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox16.Name = "textBox16";
            this.textBox16.ReadOnly = true;
            this.textBox16.Size = new System.Drawing.Size(50, 27);
            this.textBox16.TabIndex = 148;
            // 
            // LabelHallOutVoltages
            // 
            this.LabelHallOutVoltages.AutoSize = true;
            this.LabelHallOutVoltages.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallOutVoltages.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallOutVoltages.Location = new System.Drawing.Point(30, 10);
            this.LabelHallOutVoltages.Name = "LabelHallOutVoltages";
            this.LabelHallOutVoltages.Size = new System.Drawing.Size(144, 23);
            this.LabelHallOutVoltages.TabIndex = 139;
            this.LabelHallOutVoltages.Text = "Hall Out Voltages";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(30, 240);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 23);
            this.label7.TabIndex = 138;
            this.label7.Text = "Hall In Voltages";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(200, 274);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(100, 27);
            this.textBox1.TabIndex = 124;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(56, 310);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 23);
            this.label2.TabIndex = 125;
            this.label2.Text = "South Voltage 2 :";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(200, 309);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(100, 27);
            this.textBox2.TabIndex = 126;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(58, 275);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 23);
            this.label3.TabIndex = 123;
            this.label3.Text = "South Voltage 1 :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(55, 380);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 23);
            this.label4.TabIndex = 129;
            this.label4.Text = "South Voltage 4 :";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(200, 379);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(100, 27);
            this.textBox3.TabIndex = 130;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(56, 345);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 23);
            this.label5.TabIndex = 127;
            this.label5.Text = "South Voltage 3 :";
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(200, 344);
            this.textBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(100, 27);
            this.textBox4.TabIndex = 128;
            // 
            // textBox7
            // 
            this.textBox7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox7.Location = new System.Drawing.Point(325, 379);
            this.textBox7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(50, 27);
            this.textBox7.TabIndex = 136;
            // 
            // textBox8
            // 
            this.textBox8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(325, 344);
            this.textBox8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(50, 27);
            this.textBox8.TabIndex = 135;
            // 
            // textBox9
            // 
            this.textBox9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(325, 309);
            this.textBox9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(50, 27);
            this.textBox9.TabIndex = 134;
            // 
            // textBox10
            // 
            this.textBox10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(325, 274);
            this.textBox10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(50, 27);
            this.textBox10.TabIndex = 133;
            // 
            // TextboxRes1
            // 
            this.TextboxRes1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes1.Location = new System.Drawing.Point(200, 44);
            this.TextboxRes1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes1.Name = "TextboxRes1";
            this.TextboxRes1.ReadOnly = true;
            this.TextboxRes1.Size = new System.Drawing.Size(100, 27);
            this.TextboxRes1.TabIndex = 57;
            // 
            // LabelHallOutVoltage2
            // 
            this.LabelHallOutVoltage2.AutoSize = true;
            this.LabelHallOutVoltage2.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallOutVoltage2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallOutVoltage2.Location = new System.Drawing.Point(38, 80);
            this.LabelHallOutVoltage2.Name = "LabelHallOutVoltage2";
            this.LabelHallOutVoltage2.Size = new System.Drawing.Size(160, 23);
            this.LabelHallOutVoltage2.TabIndex = 58;
            this.LabelHallOutVoltage2.Text = "Hall Out Voltage 2 :";
            // 
            // TextboxRes2
            // 
            this.TextboxRes2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes2.Location = new System.Drawing.Point(200, 79);
            this.TextboxRes2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes2.Name = "TextboxRes2";
            this.TextboxRes2.ReadOnly = true;
            this.TextboxRes2.Size = new System.Drawing.Size(100, 27);
            this.TextboxRes2.TabIndex = 60;
            // 
            // LabelHallOutVoltage1
            // 
            this.LabelHallOutVoltage1.AutoSize = true;
            this.LabelHallOutVoltage1.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallOutVoltage1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallOutVoltage1.Location = new System.Drawing.Point(38, 45);
            this.LabelHallOutVoltage1.Name = "LabelHallOutVoltage1";
            this.LabelHallOutVoltage1.Size = new System.Drawing.Size(158, 23);
            this.LabelHallOutVoltage1.TabIndex = 56;
            this.LabelHallOutVoltage1.Text = "Hall Out Voltage 1 :";
            // 
            // LabelHallOutVoltage4
            // 
            this.LabelHallOutVoltage4.AutoSize = true;
            this.LabelHallOutVoltage4.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallOutVoltage4.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallOutVoltage4.Location = new System.Drawing.Point(38, 150);
            this.LabelHallOutVoltage4.Name = "LabelHallOutVoltage4";
            this.LabelHallOutVoltage4.Size = new System.Drawing.Size(161, 23);
            this.LabelHallOutVoltage4.TabIndex = 69;
            this.LabelHallOutVoltage4.Text = "Hall Out Voltage 4 :";
            // 
            // TextboxRes4
            // 
            this.TextboxRes4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes4.Location = new System.Drawing.Point(200, 149);
            this.TextboxRes4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes4.Name = "TextboxRes4";
            this.TextboxRes4.ReadOnly = true;
            this.TextboxRes4.Size = new System.Drawing.Size(100, 27);
            this.TextboxRes4.TabIndex = 70;
            // 
            // LabelHallOutVoltage3
            // 
            this.LabelHallOutVoltage3.AutoSize = true;
            this.LabelHallOutVoltage3.BackColor = System.Drawing.Color.Transparent;
            this.LabelHallOutVoltage3.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelHallOutVoltage3.Location = new System.Drawing.Point(38, 115);
            this.LabelHallOutVoltage3.Name = "LabelHallOutVoltage3";
            this.LabelHallOutVoltage3.Size = new System.Drawing.Size(160, 23);
            this.LabelHallOutVoltage3.TabIndex = 67;
            this.LabelHallOutVoltage3.Text = "Hall Out Voltage 3 :";
            // 
            // TextboxRes3
            // 
            this.TextboxRes3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes3.Location = new System.Drawing.Point(200, 114);
            this.TextboxRes3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes3.Name = "TextboxRes3";
            this.TextboxRes3.ReadOnly = true;
            this.TextboxRes3.Size = new System.Drawing.Size(100, 27);
            this.TextboxRes3.TabIndex = 68;
            // 
            // LabelResistance5
            // 
            this.LabelResistance5.AutoSize = true;
            this.LabelResistance5.BackColor = System.Drawing.Color.Transparent;
            this.LabelResistance5.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelResistance5.Location = new System.Drawing.Point(30, 185);
            this.LabelResistance5.Name = "LabelResistance5";
            this.LabelResistance5.Size = new System.Drawing.Size(168, 23);
            this.LabelResistance5.TabIndex = 73;
            this.LabelResistance5.Text = "Hall Out Resistance :";
            // 
            // TextboxRes5
            // 
            this.TextboxRes5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes5.Location = new System.Drawing.Point(200, 184);
            this.TextboxRes5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes5.Name = "TextboxRes5";
            this.TextboxRes5.ReadOnly = true;
            this.TextboxRes5.Size = new System.Drawing.Size(100, 27);
            this.TextboxRes5.TabIndex = 74;
            // 
            // TextboxRes5Unit
            // 
            this.TextboxRes5Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes5Unit.Location = new System.Drawing.Point(325, 184);
            this.TextboxRes5Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes5Unit.Name = "TextboxRes5Unit";
            this.TextboxRes5Unit.ReadOnly = true;
            this.TextboxRes5Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxRes5Unit.TabIndex = 112;
            // 
            // TextboxRes4Unit
            // 
            this.TextboxRes4Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes4Unit.Location = new System.Drawing.Point(325, 149);
            this.TextboxRes4Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes4Unit.Name = "TextboxRes4Unit";
            this.TextboxRes4Unit.ReadOnly = true;
            this.TextboxRes4Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxRes4Unit.TabIndex = 111;
            // 
            // TextboxRes3Unit
            // 
            this.TextboxRes3Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes3Unit.Location = new System.Drawing.Point(325, 114);
            this.TextboxRes3Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes3Unit.Name = "TextboxRes3Unit";
            this.TextboxRes3Unit.ReadOnly = true;
            this.TextboxRes3Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxRes3Unit.TabIndex = 110;
            // 
            // TextboxRes2Unit
            // 
            this.TextboxRes2Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes2Unit.Location = new System.Drawing.Point(325, 79);
            this.TextboxRes2Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes2Unit.Name = "TextboxRes2Unit";
            this.TextboxRes2Unit.ReadOnly = true;
            this.TextboxRes2Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxRes2Unit.TabIndex = 109;
            // 
            // TextboxRes1Unit
            // 
            this.TextboxRes1Unit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRes1Unit.Location = new System.Drawing.Point(325, 44);
            this.TextboxRes1Unit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TextboxRes1Unit.Name = "TextboxRes1Unit";
            this.TextboxRes1Unit.ReadOnly = true;
            this.TextboxRes1Unit.Size = new System.Drawing.Size(50, 27);
            this.TextboxRes1Unit.TabIndex = 108;
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
            ((System.ComponentModel.ISupportInitialize)(this.ChartHallTotalVoltages)).EndInit();
            this.PanelMeasureAndSource.ResumeLayout(false);
            this.PanelMeasureAndSource.PerformLayout();
            this.PanelResults.ResumeLayout(false);
            this.PanelResults.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelResultsChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartHallTotalVoltages;
        private System.Windows.Forms.Panel PanelMeasureAndSource;
        private System.Windows.Forms.TextBox TextboxSourceMode;
        private System.Windows.Forms.TextBox TextboxMeasureMode;
        private System.Windows.Forms.Label LabelMeasureMode;
        private System.Windows.Forms.Label LabelSourceMode;
        private System.Windows.Forms.Panel PanelResults;
        private System.Windows.Forms.TextBox TextboxRes1;
        private System.Windows.Forms.Label LabelHallOutVoltage2;
        private System.Windows.Forms.TextBox TextboxRes2;
        private System.Windows.Forms.Label LabelHallOutVoltage1;
        private System.Windows.Forms.Label LabelHallOutVoltage4;
        private System.Windows.Forms.TextBox TextboxRes4;
        private System.Windows.Forms.Label LabelHallOutVoltage3;
        private System.Windows.Forms.TextBox TextboxRes3;
        private System.Windows.Forms.Label LabelResistance5;
        private System.Windows.Forms.TextBox TextboxRes5;
        private System.Windows.Forms.TextBox TextboxRes5Unit;
        private System.Windows.Forms.TextBox TextboxRes4Unit;
        private System.Windows.Forms.TextBox TextboxRes3Unit;
        private System.Windows.Forms.TextBox TextboxRes2Unit;
        private System.Windows.Forms.TextBox TextboxRes1Unit;
        private System.Windows.Forms.Label LabelHallOutVoltages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox15;
        private System.Windows.Forms.TextBox textBox16;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox17;
        private System.Windows.Forms.TextBox textBox18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox23;
        private System.Windows.Forms.TextBox textBox24;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox25;
        private System.Windows.Forms.TextBox textBox26;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox21;
        private System.Windows.Forms.TextBox textBox22;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox19;
        private System.Windows.Forms.TextBox textBox20;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox27;
        private System.Windows.Forms.TextBox textBox28;
    }
}