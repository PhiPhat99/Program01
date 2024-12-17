namespace Program01
{
    partial class MeasurementSettingsChildForm
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
            this.IconbuttonSMUConnection = new FontAwesome.Sharp.IconButton();
            this.LabelSMUConnection = new System.Windows.Forms.Label();
            this.LabelSMSConnection = new System.Windows.Forms.Label();
            this.IconbuttonSMSConnection = new FontAwesome.Sharp.IconButton();
            this.LabelRsense = new System.Windows.Forms.Label();
            this.ComboboxRsense = new System.Windows.Forms.ComboBox();
            this.ComboboxSource = new System.Windows.Forms.ComboBox();
            this.LabelSource = new System.Windows.Forms.Label();
            this.ComboboxMeasure = new System.Windows.Forms.ComboBox();
            this.LabelMeasure = new System.Windows.Forms.Label();
            this.LabelStartUnit = new System.Windows.Forms.Label();
            this.TextboxStart = new System.Windows.Forms.TextBox();
            this.LabelStart = new System.Windows.Forms.Label();
            this.LabelStopUnit = new System.Windows.Forms.Label();
            this.LabelStepUnit = new System.Windows.Forms.Label();
            this.TextboxStop = new System.Windows.Forms.TextBox();
            this.LabelStop = new System.Windows.Forms.Label();
            this.TextboxStep = new System.Windows.Forms.TextBox();
            this.LabelStep = new System.Windows.Forms.Label();
            this.ComboboxSourceLimitMode = new System.Windows.Forms.ComboBox();
            this.LabelSourceLimitType = new System.Windows.Forms.Label();
            this.LabelSourceLimitLevelUnit = new System.Windows.Forms.Label();
            this.TextboxSourceLimitLevel = new System.Windows.Forms.TextBox();
            this.LabelSourceLimitLevel = new System.Windows.Forms.Label();
            this.LabelThickness = new System.Windows.Forms.Label();
            this.LabelThicknessUnit = new System.Windows.Forms.Label();
            this.TextboxThickness = new System.Windows.Forms.TextBox();
            this.LabelRepetition = new System.Windows.Forms.Label();
            this.TextboxRepetition = new System.Windows.Forms.TextBox();
            this.LabelMagneticFields = new System.Windows.Forms.Label();
            this.TextboxMagneticFields = new System.Windows.Forms.TextBox();
            this.LabelMagneticFieldsUnit = new System.Windows.Forms.Label();
            this.PanelTunerandData = new System.Windows.Forms.Panel();
            this.PanelButtonTabBar = new System.Windows.Forms.Panel();
            this.ButtonData = new System.Windows.Forms.Button();
            this.ButtonTuner = new System.Windows.Forms.Button();
            this.IconbuttonClearSettings = new FontAwesome.Sharp.IconButton();
            this.IconbuttonRunMeasurement = new FontAwesome.Sharp.IconButton();
            this.IconbuttonMeasurement = new FontAwesome.Sharp.IconButton();
            this.IconbuttonSweep = new FontAwesome.Sharp.IconButton();
            this.PanelTunerandData.SuspendLayout();
            this.PanelButtonTabBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconbuttonSMUConnection
            // 
            this.IconbuttonSMUConnection.BackColor = System.Drawing.Color.White;
            this.IconbuttonSMUConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSMUConnection.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.IconbuttonSMUConnection.IconColor = System.Drawing.Color.Black;
            this.IconbuttonSMUConnection.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonSMUConnection.IconSize = 30;
            this.IconbuttonSMUConnection.Location = new System.Drawing.Point(308, 16);
            this.IconbuttonSMUConnection.Margin = new System.Windows.Forms.Padding(2);
            this.IconbuttonSMUConnection.Name = "IconbuttonSMUConnection";
            this.IconbuttonSMUConnection.Size = new System.Drawing.Size(45, 32);
            this.IconbuttonSMUConnection.TabIndex = 0;
            this.IconbuttonSMUConnection.UseVisualStyleBackColor = false;
            this.IconbuttonSMUConnection.Click += new System.EventHandler(this.IconbuttonSMUConnection_Click);
            // 
            // LabelSMUConnection
            // 
            this.LabelSMUConnection.AutoSize = true;
            this.LabelSMUConnection.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSMUConnection.Location = new System.Drawing.Point(22, 16);
            this.LabelSMUConnection.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSMUConnection.Name = "LabelSMUConnection";
            this.LabelSMUConnection.Size = new System.Drawing.Size(293, 25);
            this.LabelSMUConnection.TabIndex = 1;
            this.LabelSMUConnection.Text = "Source Measure Unit Connection";
            // 
            // LabelSMSConnection
            // 
            this.LabelSMSConnection.AutoSize = true;
            this.LabelSMSConnection.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSMSConnection.Location = new System.Drawing.Point(390, 16);
            this.LabelSMSConnection.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSMSConnection.Name = "LabelSMSConnection";
            this.LabelSMSConnection.Size = new System.Drawing.Size(300, 25);
            this.LabelSMSConnection.TabIndex = 3;
            this.LabelSMSConnection.Text = "Switch Matrix System Connection";
            // 
            // IconbuttonSMSConnection
            // 
            this.IconbuttonSMSConnection.BackColor = System.Drawing.Color.White;
            this.IconbuttonSMSConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSMSConnection.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.IconbuttonSMSConnection.IconColor = System.Drawing.Color.Black;
            this.IconbuttonSMSConnection.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonSMSConnection.IconSize = 30;
            this.IconbuttonSMSConnection.Location = new System.Drawing.Point(679, 16);
            this.IconbuttonSMSConnection.Margin = new System.Windows.Forms.Padding(2);
            this.IconbuttonSMSConnection.Name = "IconbuttonSMSConnection";
            this.IconbuttonSMSConnection.Size = new System.Drawing.Size(45, 32);
            this.IconbuttonSMSConnection.TabIndex = 2;
            this.IconbuttonSMSConnection.UseVisualStyleBackColor = false;
            // 
            // LabelRsense
            // 
            this.LabelRsense.AutoSize = true;
            this.LabelRsense.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRsense.Location = new System.Drawing.Point(22, 65);
            this.LabelRsense.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelRsense.Name = "LabelRsense";
            this.LabelRsense.Size = new System.Drawing.Size(61, 25);
            this.LabelRsense.TabIndex = 4;
            this.LabelRsense.Text = "Sense";
            // 
            // ComboboxRsense
            // 
            this.ComboboxRsense.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxRsense.FormattingEnabled = true;
            this.ComboboxRsense.Location = new System.Drawing.Point(86, 65);
            this.ComboboxRsense.Margin = new System.Windows.Forms.Padding(2);
            this.ComboboxRsense.Name = "ComboboxRsense";
            this.ComboboxRsense.Size = new System.Drawing.Size(102, 25);
            this.ComboboxRsense.TabIndex = 5;
            this.ComboboxRsense.SelectedIndexChanged += new System.EventHandler(this.ComboboxRsense_SelectedIndexChanged);
            // 
            // ComboboxSource
            // 
            this.ComboboxSource.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxSource.FormattingEnabled = true;
            this.ComboboxSource.Location = new System.Drawing.Point(94, 162);
            this.ComboboxSource.Margin = new System.Windows.Forms.Padding(2);
            this.ComboboxSource.Name = "ComboboxSource";
            this.ComboboxSource.Size = new System.Drawing.Size(102, 25);
            this.ComboboxSource.TabIndex = 7;
            this.ComboboxSource.SelectedIndexChanged += new System.EventHandler(this.ComboboxSource_SelectedIndexChanged);
            // 
            // LabelSource
            // 
            this.LabelSource.AutoSize = true;
            this.LabelSource.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSource.Location = new System.Drawing.Point(22, 162);
            this.LabelSource.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSource.Name = "LabelSource";
            this.LabelSource.Size = new System.Drawing.Size(70, 25);
            this.LabelSource.TabIndex = 6;
            this.LabelSource.Text = "Source";
            // 
            // ComboboxMeasure
            // 
            this.ComboboxMeasure.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxMeasure.FormattingEnabled = true;
            this.ComboboxMeasure.Location = new System.Drawing.Point(109, 114);
            this.ComboboxMeasure.Margin = new System.Windows.Forms.Padding(2);
            this.ComboboxMeasure.Name = "ComboboxMeasure";
            this.ComboboxMeasure.Size = new System.Drawing.Size(102, 25);
            this.ComboboxMeasure.TabIndex = 9;
            this.ComboboxMeasure.SelectedIndexChanged += new System.EventHandler(this.ComboboxMeasure_SelectedIndexChanged);
            // 
            // LabelMeasure
            // 
            this.LabelMeasure.AutoSize = true;
            this.LabelMeasure.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMeasure.Location = new System.Drawing.Point(22, 114);
            this.LabelMeasure.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelMeasure.Name = "LabelMeasure";
            this.LabelMeasure.Size = new System.Drawing.Size(86, 25);
            this.LabelMeasure.TabIndex = 8;
            this.LabelMeasure.Text = "Measure";
            // 
            // LabelStartUnit
            // 
            this.LabelStartUnit.AutoSize = true;
            this.LabelStartUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStartUnit.Location = new System.Drawing.Point(158, 211);
            this.LabelStartUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStartUnit.Name = "LabelStartUnit";
            this.LabelStartUnit.Size = new System.Drawing.Size(22, 21);
            this.LabelStartUnit.TabIndex = 17;
            this.LabelStartUnit.Text = "--";
            // 
            // TextboxStart
            // 
            this.TextboxStart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStart.Location = new System.Drawing.Point(79, 211);
            this.TextboxStart.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxStart.Name = "TextboxStart";
            this.TextboxStart.Size = new System.Drawing.Size(76, 25);
            this.TextboxStart.TabIndex = 12;
            // 
            // LabelStart
            // 
            this.LabelStart.AutoSize = true;
            this.LabelStart.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStart.Location = new System.Drawing.Point(22, 211);
            this.LabelStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStart.Name = "LabelStart";
            this.LabelStart.Size = new System.Drawing.Size(53, 25);
            this.LabelStart.TabIndex = 11;
            this.LabelStart.Text = "Start";
            // 
            // LabelStopUnit
            // 
            this.LabelStopUnit.AutoSize = true;
            this.LabelStopUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStopUnit.Location = new System.Drawing.Point(154, 260);
            this.LabelStopUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStopUnit.Name = "LabelStopUnit";
            this.LabelStopUnit.Size = new System.Drawing.Size(22, 21);
            this.LabelStopUnit.TabIndex = 16;
            this.LabelStopUnit.Text = "--";
            // 
            // LabelStepUnit
            // 
            this.LabelStepUnit.AutoSize = true;
            this.LabelStepUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStepUnit.Location = new System.Drawing.Point(154, 309);
            this.LabelStepUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStepUnit.Name = "LabelStepUnit";
            this.LabelStepUnit.Size = new System.Drawing.Size(22, 21);
            this.LabelStepUnit.TabIndex = 15;
            this.LabelStepUnit.Text = "--";
            // 
            // TextboxStop
            // 
            this.TextboxStop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStop.Location = new System.Drawing.Point(75, 260);
            this.TextboxStop.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxStop.Name = "TextboxStop";
            this.TextboxStop.Size = new System.Drawing.Size(76, 25);
            this.TextboxStop.TabIndex = 14;
            // 
            // LabelStop
            // 
            this.LabelStop.AutoSize = true;
            this.LabelStop.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStop.Location = new System.Drawing.Point(22, 260);
            this.LabelStop.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStop.Name = "LabelStop";
            this.LabelStop.Size = new System.Drawing.Size(50, 25);
            this.LabelStop.TabIndex = 13;
            this.LabelStop.Text = "Stop";
            // 
            // TextboxStep
            // 
            this.TextboxStep.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStep.Location = new System.Drawing.Point(75, 309);
            this.TextboxStep.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxStep.Name = "TextboxStep";
            this.TextboxStep.Size = new System.Drawing.Size(76, 25);
            this.TextboxStep.TabIndex = 14;
            // 
            // LabelStep
            // 
            this.LabelStep.AutoSize = true;
            this.LabelStep.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStep.Location = new System.Drawing.Point(22, 309);
            this.LabelStep.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelStep.Name = "LabelStep";
            this.LabelStep.Size = new System.Drawing.Size(49, 25);
            this.LabelStep.TabIndex = 13;
            this.LabelStep.Text = "Step";
            // 
            // ComboboxSourceLimitMode
            // 
            this.ComboboxSourceLimitMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxSourceLimitMode.FormattingEnabled = true;
            this.ComboboxSourceLimitMode.Location = new System.Drawing.Point(79, 358);
            this.ComboboxSourceLimitMode.Margin = new System.Windows.Forms.Padding(2);
            this.ComboboxSourceLimitMode.Name = "ComboboxSourceLimitMode";
            this.ComboboxSourceLimitMode.Size = new System.Drawing.Size(102, 25);
            this.ComboboxSourceLimitMode.TabIndex = 12;
            this.ComboboxSourceLimitMode.SelectedIndexChanged += new System.EventHandler(this.ComboboxSourceLimitMode_SelectedIndexChanged);
            // 
            // LabelSourceLimitType
            // 
            this.LabelSourceLimitType.AutoSize = true;
            this.LabelSourceLimitType.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitType.Location = new System.Drawing.Point(22, 358);
            this.LabelSourceLimitType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSourceLimitType.Name = "LabelSourceLimitType";
            this.LabelSourceLimitType.Size = new System.Drawing.Size(55, 25);
            this.LabelSourceLimitType.TabIndex = 11;
            this.LabelSourceLimitType.Text = "Limit";
            // 
            // LabelSourceLimitLevelUnit
            // 
            this.LabelSourceLimitLevelUnit.AutoSize = true;
            this.LabelSourceLimitLevelUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitLevelUnit.Location = new System.Drawing.Point(158, 406);
            this.LabelSourceLimitLevelUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSourceLimitLevelUnit.Name = "LabelSourceLimitLevelUnit";
            this.LabelSourceLimitLevelUnit.Size = new System.Drawing.Size(22, 21);
            this.LabelSourceLimitLevelUnit.TabIndex = 20;
            this.LabelSourceLimitLevelUnit.Text = "--";
            // 
            // TextboxSourceLimitLevel
            // 
            this.TextboxSourceLimitLevel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSourceLimitLevel.Location = new System.Drawing.Point(79, 406);
            this.TextboxSourceLimitLevel.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxSourceLimitLevel.Name = "TextboxSourceLimitLevel";
            this.TextboxSourceLimitLevel.Size = new System.Drawing.Size(76, 25);
            this.TextboxSourceLimitLevel.TabIndex = 19;
            // 
            // LabelSourceLimitLevel
            // 
            this.LabelSourceLimitLevel.AutoSize = true;
            this.LabelSourceLimitLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitLevel.Location = new System.Drawing.Point(22, 406);
            this.LabelSourceLimitLevel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSourceLimitLevel.Name = "LabelSourceLimitLevel";
            this.LabelSourceLimitLevel.Size = new System.Drawing.Size(56, 25);
            this.LabelSourceLimitLevel.TabIndex = 18;
            this.LabelSourceLimitLevel.Text = "Level";
            // 
            // LabelThickness
            // 
            this.LabelThickness.AutoSize = true;
            this.LabelThickness.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelThickness.Location = new System.Drawing.Point(22, 455);
            this.LabelThickness.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelThickness.Name = "LabelThickness";
            this.LabelThickness.Size = new System.Drawing.Size(94, 25);
            this.LabelThickness.TabIndex = 13;
            this.LabelThickness.Text = "Thickness";
            // 
            // LabelThicknessUnit
            // 
            this.LabelThicknessUnit.AutoSize = true;
            this.LabelThicknessUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelThicknessUnit.Location = new System.Drawing.Point(195, 455);
            this.LabelThicknessUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelThicknessUnit.Name = "LabelThicknessUnit";
            this.LabelThicknessUnit.Size = new System.Drawing.Size(38, 21);
            this.LabelThicknessUnit.TabIndex = 22;
            this.LabelThicknessUnit.Text = "mm";
            // 
            // TextboxThickness
            // 
            this.TextboxThickness.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxThickness.Location = new System.Drawing.Point(116, 455);
            this.TextboxThickness.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxThickness.Name = "TextboxThickness";
            this.TextboxThickness.Size = new System.Drawing.Size(76, 25);
            this.TextboxThickness.TabIndex = 21;
            // 
            // LabelRepetition
            // 
            this.LabelRepetition.AutoSize = true;
            this.LabelRepetition.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRepetition.Location = new System.Drawing.Point(22, 504);
            this.LabelRepetition.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelRepetition.Name = "LabelRepetition";
            this.LabelRepetition.Size = new System.Drawing.Size(100, 25);
            this.LabelRepetition.TabIndex = 23;
            this.LabelRepetition.Text = "Repetition";
            // 
            // TextboxRepetition
            // 
            this.TextboxRepetition.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRepetition.Location = new System.Drawing.Point(124, 504);
            this.TextboxRepetition.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxRepetition.Name = "TextboxRepetition";
            this.TextboxRepetition.Size = new System.Drawing.Size(76, 25);
            this.TextboxRepetition.TabIndex = 24;
            // 
            // LabelMagneticFields
            // 
            this.LabelMagneticFields.AutoSize = true;
            this.LabelMagneticFields.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMagneticFields.Location = new System.Drawing.Point(22, 552);
            this.LabelMagneticFields.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelMagneticFields.Name = "LabelMagneticFields";
            this.LabelMagneticFields.Size = new System.Drawing.Size(172, 25);
            this.LabelMagneticFields.TabIndex = 25;
            this.LabelMagneticFields.Text = "Magnetic fields (B)";
            // 
            // TextboxMagneticFields
            // 
            this.TextboxMagneticFields.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMagneticFields.Location = new System.Drawing.Point(191, 552);
            this.TextboxMagneticFields.Margin = new System.Windows.Forms.Padding(2);
            this.TextboxMagneticFields.Name = "TextboxMagneticFields";
            this.TextboxMagneticFields.Size = new System.Drawing.Size(76, 25);
            this.TextboxMagneticFields.TabIndex = 26;
            // 
            // LabelMagneticFieldsUnit
            // 
            this.LabelMagneticFieldsUnit.AutoSize = true;
            this.LabelMagneticFieldsUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMagneticFieldsUnit.Location = new System.Drawing.Point(270, 552);
            this.LabelMagneticFieldsUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelMagneticFieldsUnit.Name = "LabelMagneticFieldsUnit";
            this.LabelMagneticFieldsUnit.Size = new System.Drawing.Size(19, 21);
            this.LabelMagneticFieldsUnit.TabIndex = 27;
            this.LabelMagneticFieldsUnit.Text = "T";
            // 
            // PanelTunerandData
            // 
            this.PanelTunerandData.BackColor = System.Drawing.Color.LightGray;
            this.PanelTunerandData.Controls.Add(this.PanelButtonTabBar);
            this.PanelTunerandData.Location = new System.Drawing.Point(308, 81);
            this.PanelTunerandData.Margin = new System.Windows.Forms.Padding(2);
            this.PanelTunerandData.Name = "PanelTunerandData";
            this.PanelTunerandData.Size = new System.Drawing.Size(600, 488);
            this.PanelTunerandData.TabIndex = 28;
            // 
            // PanelButtonTabBar
            // 
            this.PanelButtonTabBar.BackColor = System.Drawing.Color.Gray;
            this.PanelButtonTabBar.Controls.Add(this.ButtonData);
            this.PanelButtonTabBar.Controls.Add(this.ButtonTuner);
            this.PanelButtonTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelButtonTabBar.Location = new System.Drawing.Point(0, 0);
            this.PanelButtonTabBar.Margin = new System.Windows.Forms.Padding(2);
            this.PanelButtonTabBar.Name = "PanelButtonTabBar";
            this.PanelButtonTabBar.Size = new System.Drawing.Size(600, 32);
            this.PanelButtonTabBar.TabIndex = 0;
            // 
            // ButtonData
            // 
            this.ButtonData.BackColor = System.Drawing.Color.White;
            this.ButtonData.Dock = System.Windows.Forms.DockStyle.Left;
            this.ButtonData.FlatAppearance.BorderSize = 0;
            this.ButtonData.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.ButtonData.Location = new System.Drawing.Point(56, 0);
            this.ButtonData.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonData.Name = "ButtonData";
            this.ButtonData.Size = new System.Drawing.Size(56, 32);
            this.ButtonData.TabIndex = 1;
            this.ButtonData.Text = "Data";
            this.ButtonData.UseVisualStyleBackColor = false;
            // 
            // ButtonTuner
            // 
            this.ButtonTuner.BackColor = System.Drawing.Color.White;
            this.ButtonTuner.Dock = System.Windows.Forms.DockStyle.Left;
            this.ButtonTuner.FlatAppearance.BorderSize = 0;
            this.ButtonTuner.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonTuner.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.ButtonTuner.Location = new System.Drawing.Point(0, 0);
            this.ButtonTuner.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonTuner.Name = "ButtonTuner";
            this.ButtonTuner.Size = new System.Drawing.Size(56, 32);
            this.ButtonTuner.TabIndex = 0;
            this.ButtonTuner.Text = "Tuner";
            this.ButtonTuner.UseVisualStyleBackColor = false;
            // 
            // IconbuttonClearSettings
            // 
            this.IconbuttonClearSettings.BackColor = System.Drawing.Color.White;
            this.IconbuttonClearSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonClearSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonClearSettings.IconChar = FontAwesome.Sharp.IconChar.None;
            this.IconbuttonClearSettings.IconColor = System.Drawing.Color.Black;
            this.IconbuttonClearSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonClearSettings.Location = new System.Drawing.Point(38, 609);
            this.IconbuttonClearSettings.Margin = new System.Windows.Forms.Padding(2);
            this.IconbuttonClearSettings.Name = "IconbuttonClearSettings";
            this.IconbuttonClearSettings.Size = new System.Drawing.Size(112, 37);
            this.IconbuttonClearSettings.TabIndex = 29;
            this.IconbuttonClearSettings.Text = "Clear";
            this.IconbuttonClearSettings.UseVisualStyleBackColor = false;
            this.IconbuttonClearSettings.Click += new System.EventHandler(this.IconbuttonClearSettings_Click);
            // 
            // IconbuttonRunMeasurement
            // 
            this.IconbuttonRunMeasurement.BackColor = System.Drawing.Color.White;
            this.IconbuttonRunMeasurement.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IconbuttonRunMeasurement.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonRunMeasurement.IconChar = FontAwesome.Sharp.IconChar.Running;
            this.IconbuttonRunMeasurement.IconColor = System.Drawing.Color.Black;
            this.IconbuttonRunMeasurement.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonRunMeasurement.IconSize = 36;
            this.IconbuttonRunMeasurement.Location = new System.Drawing.Point(788, 609);
            this.IconbuttonRunMeasurement.Margin = new System.Windows.Forms.Padding(2);
            this.IconbuttonRunMeasurement.Name = "IconbuttonRunMeasurement";
            this.IconbuttonRunMeasurement.Size = new System.Drawing.Size(90, 37);
            this.IconbuttonRunMeasurement.TabIndex = 30;
            this.IconbuttonRunMeasurement.Text = "Run";
            this.IconbuttonRunMeasurement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonRunMeasurement.UseVisualStyleBackColor = false;
            this.IconbuttonRunMeasurement.Click += new System.EventHandler(this.IconbuttonRunMeasurement_Click);
            // 
            // IconbuttonMeasurement
            // 
            this.IconbuttonMeasurement.BackColor = System.Drawing.Color.White;
            this.IconbuttonMeasurement.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IconbuttonMeasurement.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.IconbuttonMeasurement.IconChar = FontAwesome.Sharp.IconChar.None;
            this.IconbuttonMeasurement.IconColor = System.Drawing.Color.Black;
            this.IconbuttonMeasurement.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonMeasurement.IconSize = 30;
            this.IconbuttonMeasurement.Location = new System.Drawing.Point(750, 16);
            this.IconbuttonMeasurement.Margin = new System.Windows.Forms.Padding(2);
            this.IconbuttonMeasurement.Name = "IconbuttonMeasurement";
            this.IconbuttonMeasurement.Size = new System.Drawing.Size(180, 32);
            this.IconbuttonMeasurement.TabIndex = 31;
            this.IconbuttonMeasurement.Text = "Measurement Mode";
            this.IconbuttonMeasurement.UseVisualStyleBackColor = false;
            this.IconbuttonMeasurement.Click += new System.EventHandler(this.IconbuttonMeasurement_Click);
            // 
            // IconbuttonSweep
            // 
            this.IconbuttonSweep.IconChar = FontAwesome.Sharp.IconChar.None;
            this.IconbuttonSweep.IconColor = System.Drawing.Color.Black;
            this.IconbuttonSweep.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonSweep.Location = new System.Drawing.Point(524, 619);
            this.IconbuttonSweep.Name = "IconbuttonSweep";
            this.IconbuttonSweep.Size = new System.Drawing.Size(75, 23);
            this.IconbuttonSweep.TabIndex = 33;
            this.IconbuttonSweep.Text = "SWEEP";
            this.IconbuttonSweep.UseVisualStyleBackColor = true;
            // 
            // MeasurementSettingsChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(118)))), ((int)(((byte)(176)))));
            this.ClientSize = new System.Drawing.Size(945, 666);
            this.Controls.Add(this.IconbuttonSweep);
            this.Controls.Add(this.IconbuttonMeasurement);
            this.Controls.Add(this.IconbuttonRunMeasurement);
            this.Controls.Add(this.IconbuttonClearSettings);
            this.Controls.Add(this.PanelTunerandData);
            this.Controls.Add(this.LabelMagneticFieldsUnit);
            this.Controls.Add(this.LabelMagneticFields);
            this.Controls.Add(this.TextboxMagneticFields);
            this.Controls.Add(this.LabelSourceLimitLevelUnit);
            this.Controls.Add(this.LabelRepetition);
            this.Controls.Add(this.LabelStartUnit);
            this.Controls.Add(this.TextboxRepetition);
            this.Controls.Add(this.TextboxStart);
            this.Controls.Add(this.TextboxSourceLimitLevel);
            this.Controls.Add(this.LabelThicknessUnit);
            this.Controls.Add(this.LabelStart);
            this.Controls.Add(this.LabelThickness);
            this.Controls.Add(this.LabelSourceLimitLevel);
            this.Controls.Add(this.TextboxThickness);
            this.Controls.Add(this.TextboxStop);
            this.Controls.Add(this.ComboboxSourceLimitMode);
            this.Controls.Add(this.LabelStop);
            this.Controls.Add(this.LabelSourceLimitType);
            this.Controls.Add(this.LabelStopUnit);
            this.Controls.Add(this.TextboxStep);
            this.Controls.Add(this.ComboboxMeasure);
            this.Controls.Add(this.LabelStep);
            this.Controls.Add(this.LabelMeasure);
            this.Controls.Add(this.LabelStepUnit);
            this.Controls.Add(this.ComboboxSource);
            this.Controls.Add(this.LabelSource);
            this.Controls.Add(this.ComboboxRsense);
            this.Controls.Add(this.LabelRsense);
            this.Controls.Add(this.LabelSMSConnection);
            this.Controls.Add(this.IconbuttonSMSConnection);
            this.Controls.Add(this.LabelSMUConnection);
            this.Controls.Add(this.IconbuttonSMUConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MeasurementSettingsChildForm";
            this.Text = "2";
            this.Load += new System.EventHandler(this.MeasurementSettingsChildForm_Load);
            this.PanelTunerandData.ResumeLayout(false);
            this.PanelButtonTabBar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FontAwesome.Sharp.IconButton IconbuttonSMUConnection;
        private System.Windows.Forms.Label LabelSMUConnection;
        private System.Windows.Forms.Label LabelSMSConnection;
        private FontAwesome.Sharp.IconButton IconbuttonSMSConnection;
        private System.Windows.Forms.Label LabelRsense;
        private System.Windows.Forms.ComboBox ComboboxRsense;
        private System.Windows.Forms.ComboBox ComboboxSource;
        private System.Windows.Forms.Label LabelSource;
        private System.Windows.Forms.ComboBox ComboboxMeasure;
        private System.Windows.Forms.Label LabelMeasure;
        private System.Windows.Forms.TextBox TextboxStop;
        private System.Windows.Forms.Label LabelStop;
        private System.Windows.Forms.TextBox TextboxStep;
        private System.Windows.Forms.Label LabelStep;
        private System.Windows.Forms.TextBox TextboxStart;
        private System.Windows.Forms.Label LabelStart;
        private System.Windows.Forms.Label LabelStepUnit;
        private System.Windows.Forms.Label LabelStartUnit;
        private System.Windows.Forms.Label LabelStopUnit;
        private System.Windows.Forms.ComboBox ComboboxSourceLimitMode;
        private System.Windows.Forms.Label LabelSourceLimitType;
        private System.Windows.Forms.Label LabelSourceLimitLevelUnit;
        private System.Windows.Forms.TextBox TextboxSourceLimitLevel;
        private System.Windows.Forms.Label LabelSourceLimitLevel;
        private System.Windows.Forms.Label LabelThickness;
        private System.Windows.Forms.Label LabelThicknessUnit;
        private System.Windows.Forms.TextBox TextboxThickness;
        private System.Windows.Forms.Label LabelRepetition;
        private System.Windows.Forms.TextBox TextboxRepetition;
        private System.Windows.Forms.Label LabelMagneticFields;
        private System.Windows.Forms.TextBox TextboxMagneticFields;
        private System.Windows.Forms.Label LabelMagneticFieldsUnit;
        private System.Windows.Forms.Panel PanelTunerandData;
        private FontAwesome.Sharp.IconButton IconbuttonClearSettings;
        private FontAwesome.Sharp.IconButton IconbuttonRunMeasurement;
        private System.Windows.Forms.Panel PanelButtonTabBar;
        private System.Windows.Forms.Button ButtonTuner;
        private System.Windows.Forms.Button ButtonData;
        private FontAwesome.Sharp.IconButton IconbuttonMeasurement;
        private FontAwesome.Sharp.IconButton IconbuttonSweep;
    }
}