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
            this.IconbuttonApplyandEditSettings = new FontAwesome.Sharp.IconButton();
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
            this.IconbuttonSMUConnection.Location = new System.Drawing.Point(410, 20);
            this.IconbuttonSMUConnection.Name = "IconbuttonSMUConnection";
            this.IconbuttonSMUConnection.Size = new System.Drawing.Size(60, 40);
            this.IconbuttonSMUConnection.TabIndex = 0;
            this.IconbuttonSMUConnection.UseVisualStyleBackColor = false;
            this.IconbuttonSMUConnection.Click += new System.EventHandler(this.IconbuttonSMUConnection_Click);
            // 
            // LabelSMUConnection
            // 
            this.LabelSMUConnection.AutoSize = true;
            this.LabelSMUConnection.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSMUConnection.Location = new System.Drawing.Point(30, 20);
            this.LabelSMUConnection.Name = "LabelSMUConnection";
            this.LabelSMUConnection.Size = new System.Drawing.Size(373, 32);
            this.LabelSMUConnection.TabIndex = 1;
            this.LabelSMUConnection.Text = "Source Measure Unit Connection";
            // 
            // LabelSMSConnection
            // 
            this.LabelSMSConnection.AutoSize = true;
            this.LabelSMSConnection.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSMSConnection.Location = new System.Drawing.Point(520, 20);
            this.LabelSMSConnection.Name = "LabelSMSConnection";
            this.LabelSMSConnection.Size = new System.Drawing.Size(378, 32);
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
            this.IconbuttonSMSConnection.Location = new System.Drawing.Point(905, 20);
            this.IconbuttonSMSConnection.Name = "IconbuttonSMSConnection";
            this.IconbuttonSMSConnection.Size = new System.Drawing.Size(60, 40);
            this.IconbuttonSMSConnection.TabIndex = 2;
            this.IconbuttonSMSConnection.UseVisualStyleBackColor = false;
            // 
            // LabelRsense
            // 
            this.LabelRsense.AutoSize = true;
            this.LabelRsense.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRsense.Location = new System.Drawing.Point(30, 80);
            this.LabelRsense.Name = "LabelRsense";
            this.LabelRsense.Size = new System.Drawing.Size(77, 32);
            this.LabelRsense.TabIndex = 4;
            this.LabelRsense.Text = "Sense";
            // 
            // ComboboxRsense
            // 
            this.ComboboxRsense.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxRsense.FormattingEnabled = true;
            this.ComboboxRsense.Location = new System.Drawing.Point(115, 80);
            this.ComboboxRsense.Name = "ComboboxRsense";
            this.ComboboxRsense.Size = new System.Drawing.Size(134, 31);
            this.ComboboxRsense.TabIndex = 5;
            this.ComboboxRsense.SelectedIndexChanged += new System.EventHandler(this.ComboboxRsense_SelectedIndexChanged);
            // 
            // ComboboxSource
            // 
            this.ComboboxSource.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxSource.FormattingEnabled = true;
            this.ComboboxSource.Location = new System.Drawing.Point(125, 200);
            this.ComboboxSource.Name = "ComboboxSource";
            this.ComboboxSource.Size = new System.Drawing.Size(134, 31);
            this.ComboboxSource.TabIndex = 7;
            this.ComboboxSource.SelectedIndexChanged += new System.EventHandler(this.ComboboxSource_SelectedIndexChanged);
            // 
            // LabelSource
            // 
            this.LabelSource.AutoSize = true;
            this.LabelSource.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSource.Location = new System.Drawing.Point(30, 200);
            this.LabelSource.Name = "LabelSource";
            this.LabelSource.Size = new System.Drawing.Size(88, 32);
            this.LabelSource.TabIndex = 6;
            this.LabelSource.Text = "Source";
            // 
            // ComboboxMeasure
            // 
            this.ComboboxMeasure.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxMeasure.FormattingEnabled = true;
            this.ComboboxMeasure.Location = new System.Drawing.Point(145, 140);
            this.ComboboxMeasure.Name = "ComboboxMeasure";
            this.ComboboxMeasure.Size = new System.Drawing.Size(134, 31);
            this.ComboboxMeasure.TabIndex = 9;
            this.ComboboxMeasure.SelectedIndexChanged += new System.EventHandler(this.ComboboxMeasure_SelectedIndexChanged);
            // 
            // LabelMeasure
            // 
            this.LabelMeasure.AutoSize = true;
            this.LabelMeasure.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMeasure.Location = new System.Drawing.Point(30, 140);
            this.LabelMeasure.Name = "LabelMeasure";
            this.LabelMeasure.Size = new System.Drawing.Size(108, 32);
            this.LabelMeasure.TabIndex = 8;
            this.LabelMeasure.Text = "Measure";
            // 
            // LabelStartUnit
            // 
            this.LabelStartUnit.AutoSize = true;
            this.LabelStartUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStartUnit.Location = new System.Drawing.Point(210, 260);
            this.LabelStartUnit.Name = "LabelStartUnit";
            this.LabelStartUnit.Size = new System.Drawing.Size(28, 28);
            this.LabelStartUnit.TabIndex = 17;
            this.LabelStartUnit.Text = "--";
            // 
            // TextboxStart
            // 
            this.TextboxStart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStart.Location = new System.Drawing.Point(105, 260);
            this.TextboxStart.Name = "TextboxStart";
            this.TextboxStart.Size = new System.Drawing.Size(100, 30);
            this.TextboxStart.TabIndex = 12;
            // 
            // LabelStart
            // 
            this.LabelStart.AutoSize = true;
            this.LabelStart.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStart.Location = new System.Drawing.Point(30, 260);
            this.LabelStart.Name = "LabelStart";
            this.LabelStart.Size = new System.Drawing.Size(67, 32);
            this.LabelStart.TabIndex = 11;
            this.LabelStart.Text = "Start";
            // 
            // LabelStopUnit
            // 
            this.LabelStopUnit.AutoSize = true;
            this.LabelStopUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStopUnit.Location = new System.Drawing.Point(205, 320);
            this.LabelStopUnit.Name = "LabelStopUnit";
            this.LabelStopUnit.Size = new System.Drawing.Size(28, 28);
            this.LabelStopUnit.TabIndex = 16;
            this.LabelStopUnit.Text = "--";
            // 
            // LabelStepUnit
            // 
            this.LabelStepUnit.AutoSize = true;
            this.LabelStepUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStepUnit.Location = new System.Drawing.Point(205, 380);
            this.LabelStepUnit.Name = "LabelStepUnit";
            this.LabelStepUnit.Size = new System.Drawing.Size(28, 28);
            this.LabelStepUnit.TabIndex = 15;
            this.LabelStepUnit.Text = "--";
            // 
            // TextboxStop
            // 
            this.TextboxStop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStop.Location = new System.Drawing.Point(100, 320);
            this.TextboxStop.Name = "TextboxStop";
            this.TextboxStop.Size = new System.Drawing.Size(100, 30);
            this.TextboxStop.TabIndex = 14;
            // 
            // LabelStop
            // 
            this.LabelStop.AutoSize = true;
            this.LabelStop.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStop.Location = new System.Drawing.Point(30, 320);
            this.LabelStop.Name = "LabelStop";
            this.LabelStop.Size = new System.Drawing.Size(63, 32);
            this.LabelStop.TabIndex = 13;
            this.LabelStop.Text = "Stop";
            // 
            // TextboxStep
            // 
            this.TextboxStep.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxStep.Location = new System.Drawing.Point(100, 380);
            this.TextboxStep.Name = "TextboxStep";
            this.TextboxStep.Size = new System.Drawing.Size(100, 30);
            this.TextboxStep.TabIndex = 14;
            // 
            // LabelStep
            // 
            this.LabelStep.AutoSize = true;
            this.LabelStep.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStep.Location = new System.Drawing.Point(30, 380);
            this.LabelStep.Name = "LabelStep";
            this.LabelStep.Size = new System.Drawing.Size(62, 32);
            this.LabelStep.TabIndex = 13;
            this.LabelStep.Text = "Step";
            // 
            // ComboboxSourceLimitMode
            // 
            this.ComboboxSourceLimitMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxSourceLimitMode.FormattingEnabled = true;
            this.ComboboxSourceLimitMode.Location = new System.Drawing.Point(105, 440);
            this.ComboboxSourceLimitMode.Name = "ComboboxSourceLimitMode";
            this.ComboboxSourceLimitMode.Size = new System.Drawing.Size(134, 31);
            this.ComboboxSourceLimitMode.TabIndex = 12;
            this.ComboboxSourceLimitMode.SelectedIndexChanged += new System.EventHandler(this.ComboboxSourceLimitMode_SelectedIndexChanged);
            // 
            // LabelSourceLimitType
            // 
            this.LabelSourceLimitType.AutoSize = true;
            this.LabelSourceLimitType.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitType.Location = new System.Drawing.Point(30, 440);
            this.LabelSourceLimitType.Name = "LabelSourceLimitType";
            this.LabelSourceLimitType.Size = new System.Drawing.Size(68, 32);
            this.LabelSourceLimitType.TabIndex = 11;
            this.LabelSourceLimitType.Text = "Limit";
            // 
            // LabelSourceLimitLevelUnit
            // 
            this.LabelSourceLimitLevelUnit.AutoSize = true;
            this.LabelSourceLimitLevelUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitLevelUnit.Location = new System.Drawing.Point(210, 500);
            this.LabelSourceLimitLevelUnit.Name = "LabelSourceLimitLevelUnit";
            this.LabelSourceLimitLevelUnit.Size = new System.Drawing.Size(28, 28);
            this.LabelSourceLimitLevelUnit.TabIndex = 20;
            this.LabelSourceLimitLevelUnit.Text = "--";
            // 
            // TextboxSourceLimitLevel
            // 
            this.TextboxSourceLimitLevel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxSourceLimitLevel.Location = new System.Drawing.Point(105, 500);
            this.TextboxSourceLimitLevel.Name = "TextboxSourceLimitLevel";
            this.TextboxSourceLimitLevel.Size = new System.Drawing.Size(100, 30);
            this.TextboxSourceLimitLevel.TabIndex = 19;
            // 
            // LabelSourceLimitLevel
            // 
            this.LabelSourceLimitLevel.AutoSize = true;
            this.LabelSourceLimitLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSourceLimitLevel.Location = new System.Drawing.Point(30, 500);
            this.LabelSourceLimitLevel.Name = "LabelSourceLimitLevel";
            this.LabelSourceLimitLevel.Size = new System.Drawing.Size(70, 32);
            this.LabelSourceLimitLevel.TabIndex = 18;
            this.LabelSourceLimitLevel.Text = "Level";
            // 
            // LabelThickness
            // 
            this.LabelThickness.AutoSize = true;
            this.LabelThickness.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelThickness.Location = new System.Drawing.Point(30, 560);
            this.LabelThickness.Name = "LabelThickness";
            this.LabelThickness.Size = new System.Drawing.Size(118, 32);
            this.LabelThickness.TabIndex = 13;
            this.LabelThickness.Text = "Thickness";
            // 
            // LabelThicknessUnit
            // 
            this.LabelThicknessUnit.AutoSize = true;
            this.LabelThicknessUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelThicknessUnit.Location = new System.Drawing.Point(260, 560);
            this.LabelThicknessUnit.Name = "LabelThicknessUnit";
            this.LabelThicknessUnit.Size = new System.Drawing.Size(48, 28);
            this.LabelThicknessUnit.TabIndex = 22;
            this.LabelThicknessUnit.Text = "mm";
            // 
            // TextboxThickness
            // 
            this.TextboxThickness.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxThickness.Location = new System.Drawing.Point(155, 560);
            this.TextboxThickness.Name = "TextboxThickness";
            this.TextboxThickness.Size = new System.Drawing.Size(100, 30);
            this.TextboxThickness.TabIndex = 21;
            // 
            // LabelRepetition
            // 
            this.LabelRepetition.AutoSize = true;
            this.LabelRepetition.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRepetition.Location = new System.Drawing.Point(30, 620);
            this.LabelRepetition.Name = "LabelRepetition";
            this.LabelRepetition.Size = new System.Drawing.Size(126, 32);
            this.LabelRepetition.TabIndex = 23;
            this.LabelRepetition.Text = "Repetition";
            // 
            // TextboxRepetition
            // 
            this.TextboxRepetition.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxRepetition.Location = new System.Drawing.Point(165, 620);
            this.TextboxRepetition.Name = "TextboxRepetition";
            this.TextboxRepetition.Size = new System.Drawing.Size(100, 30);
            this.TextboxRepetition.TabIndex = 24;
            // 
            // LabelMagneticFields
            // 
            this.LabelMagneticFields.AutoSize = true;
            this.LabelMagneticFields.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMagneticFields.Location = new System.Drawing.Point(30, 680);
            this.LabelMagneticFields.Name = "LabelMagneticFields";
            this.LabelMagneticFields.Size = new System.Drawing.Size(218, 32);
            this.LabelMagneticFields.TabIndex = 25;
            this.LabelMagneticFields.Text = "Magnetic fields (B)";
            // 
            // TextboxMagneticFields
            // 
            this.TextboxMagneticFields.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextboxMagneticFields.Location = new System.Drawing.Point(255, 680);
            this.TextboxMagneticFields.Name = "TextboxMagneticFields";
            this.TextboxMagneticFields.Size = new System.Drawing.Size(100, 30);
            this.TextboxMagneticFields.TabIndex = 26;
            // 
            // LabelMagneticFieldsUnit
            // 
            this.LabelMagneticFieldsUnit.AutoSize = true;
            this.LabelMagneticFieldsUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMagneticFieldsUnit.Location = new System.Drawing.Point(360, 680);
            this.LabelMagneticFieldsUnit.Name = "LabelMagneticFieldsUnit";
            this.LabelMagneticFieldsUnit.Size = new System.Drawing.Size(23, 28);
            this.LabelMagneticFieldsUnit.TabIndex = 27;
            this.LabelMagneticFieldsUnit.Text = "T";
            // 
            // PanelTunerandData
            // 
            this.PanelTunerandData.BackColor = System.Drawing.Color.LightGray;
            this.PanelTunerandData.Controls.Add(this.PanelButtonTabBar);
            this.PanelTunerandData.Location = new System.Drawing.Point(410, 100);
            this.PanelTunerandData.Name = "PanelTunerandData";
            this.PanelTunerandData.Size = new System.Drawing.Size(800, 600);
            this.PanelTunerandData.TabIndex = 28;
            // 
            // PanelButtonTabBar
            // 
            this.PanelButtonTabBar.BackColor = System.Drawing.Color.Gray;
            this.PanelButtonTabBar.Controls.Add(this.ButtonData);
            this.PanelButtonTabBar.Controls.Add(this.ButtonTuner);
            this.PanelButtonTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelButtonTabBar.Location = new System.Drawing.Point(0, 0);
            this.PanelButtonTabBar.Name = "PanelButtonTabBar";
            this.PanelButtonTabBar.Size = new System.Drawing.Size(800, 40);
            this.PanelButtonTabBar.TabIndex = 0;
            // 
            // ButtonData
            // 
            this.ButtonData.BackColor = System.Drawing.Color.White;
            this.ButtonData.Dock = System.Windows.Forms.DockStyle.Left;
            this.ButtonData.FlatAppearance.BorderSize = 0;
            this.ButtonData.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.ButtonData.Location = new System.Drawing.Point(75, 0);
            this.ButtonData.Name = "ButtonData";
            this.ButtonData.Size = new System.Drawing.Size(75, 40);
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
            this.ButtonTuner.Name = "ButtonTuner";
            this.ButtonTuner.Size = new System.Drawing.Size(75, 40);
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
            this.IconbuttonClearSettings.Location = new System.Drawing.Point(50, 750);
            this.IconbuttonClearSettings.Name = "IconbuttonClearSettings";
            this.IconbuttonClearSettings.Size = new System.Drawing.Size(150, 45);
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
            this.IconbuttonRunMeasurement.Location = new System.Drawing.Point(1050, 750);
            this.IconbuttonRunMeasurement.Name = "IconbuttonRunMeasurement";
            this.IconbuttonRunMeasurement.Size = new System.Drawing.Size(120, 45);
            this.IconbuttonRunMeasurement.TabIndex = 30;
            this.IconbuttonRunMeasurement.Text = "Run";
            this.IconbuttonRunMeasurement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonRunMeasurement.UseVisualStyleBackColor = false;
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
            this.IconbuttonMeasurement.Location = new System.Drawing.Point(1000, 20);
            this.IconbuttonMeasurement.Name = "IconbuttonMeasurement";
            this.IconbuttonMeasurement.Size = new System.Drawing.Size(240, 40);
            this.IconbuttonMeasurement.TabIndex = 31;
            this.IconbuttonMeasurement.Text = "Measurement Mode";
            this.IconbuttonMeasurement.UseVisualStyleBackColor = false;
            this.IconbuttonMeasurement.Click += new System.EventHandler(this.IconbuttonMeasurement_Click);
            // 
            // IconbuttonApplyandEditSettings
            // 
            this.IconbuttonApplyandEditSettings.BackColor = System.Drawing.Color.White;
            this.IconbuttonApplyandEditSettings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IconbuttonApplyandEditSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonApplyandEditSettings.IconChar = FontAwesome.Sharp.IconChar.Circle;
            this.IconbuttonApplyandEditSettings.IconColor = System.Drawing.Color.Black;
            this.IconbuttonApplyandEditSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonApplyandEditSettings.IconSize = 36;
            this.IconbuttonApplyandEditSettings.Location = new System.Drawing.Point(910, 750);
            this.IconbuttonApplyandEditSettings.Name = "IconbuttonApplyandEditSettings";
            this.IconbuttonApplyandEditSettings.Size = new System.Drawing.Size(120, 45);
            this.IconbuttonApplyandEditSettings.TabIndex = 32;
            this.IconbuttonApplyandEditSettings.Text = "Apply";
            this.IconbuttonApplyandEditSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonApplyandEditSettings.UseVisualStyleBackColor = false;
            //this.IconbuttonApplyandEditSettings.Click += new System.EventHandler(this.IconbuttonApplyandEditSettings_Click);
            // 
            // MeasurementSettingsChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(118)))), ((int)(((byte)(176)))));
            this.ClientSize = new System.Drawing.Size(1260, 820);
            this.Controls.Add(this.IconbuttonApplyandEditSettings);
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
        private FontAwesome.Sharp.IconButton IconbuttonApplyandEditSettings;
    }
}