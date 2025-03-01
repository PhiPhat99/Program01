namespace Program01
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.PanelTabBar = new System.Windows.Forms.Panel();
            this.IconpictureboxMinimizeProgram = new FontAwesome.Sharp.IconPictureBox();
            this.IconpictureboxExitProgram = new FontAwesome.Sharp.IconPictureBox();
            this.PanelTitleTabBar = new System.Windows.Forms.Panel();
            this.LabelUserLogin = new System.Windows.Forms.Label();
            this.LabelCurrentDateandRealTime = new System.Windows.Forms.Label();
            this.LabelTitleCurrentForm = new System.Windows.Forms.Label();
            this.IconpictureboxCurrentForm = new FontAwesome.Sharp.IconPictureBox();
            this.PanelDesktop = new System.Windows.Forms.Panel();
            this.IconbuttonUserLogin = new FontAwesome.Sharp.IconButton();
            this.IconbuttonBrowseFileVdPandHallMeasurementPath = new FontAwesome.Sharp.IconButton();
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.IconbuttonBrowseFileVdPDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.IconbuttonSaveFileVdPandHallMeasurementPath = new FontAwesome.Sharp.IconButton();
            this.IconbuttonSaveFileHallMeasurementDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.IconbuttonSaveFileVdPDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.TextboxFileVdPandHallMeasurementDataPath = new System.Windows.Forms.TextBox();
            this.TextboxFileHallMeasurementDataPath = new System.Windows.Forms.TextBox();
            this.TextboxFileVdPDataPath = new System.Windows.Forms.TextBox();
            this.LabelUserLastname = new System.Windows.Forms.Label();
            this.TextboxUserLastname = new System.Windows.Forms.TextBox();
            this.LabelUserFirstName = new System.Windows.Forms.Label();
            this.TextboxUserFirstName = new System.Windows.Forms.TextBox();
            this.LabelUserFirstNameLastname = new System.Windows.Forms.Label();
            this.LabelSaveVdPandHallMeasurementData = new System.Windows.Forms.Label();
            this.LabelSaveHallMeasurementDataOnly = new System.Windows.Forms.Label();
            this.LabelSaveVdPDataOnly = new System.Windows.Forms.Label();
            this.LabelProgramTitle = new System.Windows.Forms.Label();
            this.PanelHome = new System.Windows.Forms.Panel();
            this.IconpictureboxLogo = new System.Windows.Forms.PictureBox();
            this.IconbuttonHelp = new FontAwesome.Sharp.IconButton();
            this.PanelSideMenu = new System.Windows.Forms.Panel();
            this.PanelHallMeasurementSubMenu = new System.Windows.Forms.Panel();
            this.ButtonHallMeasurementResults = new System.Windows.Forms.Button();
            this.ButtonHallTotalMeasure = new System.Windows.Forms.Button();
            this.IconbuttonHalleffectMeasurement = new FontAwesome.Sharp.IconButton();
            this.PanelVanderPauwSubMenu = new System.Windows.Forms.Panel();
            this.ButtonVdPMeasurementResults = new System.Windows.Forms.Button();
            this.ButtonVdPTotalMeasure = new System.Windows.Forms.Button();
            this.IconbuttonVanderPauwMethod = new FontAwesome.Sharp.IconButton();
            this.IconbuttonMeasurementSettings = new FontAwesome.Sharp.IconButton();
            this.TimerCurrentDateandRealTime = new System.Windows.Forms.Timer(this.components);
            this.FolderBrowserDialogVdPFile = new System.Windows.Forms.FolderBrowserDialog();
            this.FolderBrowserDialogHallMeasurementFile = new System.Windows.Forms.FolderBrowserDialog();
            this.FolderBrowserDialogVdPandHallMeasurementFile = new System.Windows.Forms.FolderBrowserDialog();
            this.PanelTabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxMinimizeProgram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxExitProgram)).BeginInit();
            this.PanelTitleTabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxCurrentForm)).BeginInit();
            this.PanelDesktop.SuspendLayout();
            this.PanelHome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxLogo)).BeginInit();
            this.PanelSideMenu.SuspendLayout();
            this.PanelHallMeasurementSubMenu.SuspendLayout();
            this.PanelVanderPauwSubMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelTabBar
            // 
            this.PanelTabBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.PanelTabBar.Controls.Add(this.IconpictureboxMinimizeProgram);
            this.PanelTabBar.Controls.Add(this.IconpictureboxExitProgram);
            this.PanelTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTabBar.Location = new System.Drawing.Point(240, 0);
            this.PanelTabBar.Name = "PanelTabBar";
            this.PanelTabBar.Size = new System.Drawing.Size(1260, 30);
            this.PanelTabBar.TabIndex = 1;
            this.PanelTabBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelTabBar_MouseDown);
            // 
            // IconpictureboxMinimizeProgram
            // 
            this.IconpictureboxMinimizeProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IconpictureboxMinimizeProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.IconpictureboxMinimizeProgram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IconpictureboxMinimizeProgram.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.IconpictureboxMinimizeProgram.IconChar = FontAwesome.Sharp.IconChar.Minus;
            this.IconpictureboxMinimizeProgram.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.IconpictureboxMinimizeProgram.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconpictureboxMinimizeProgram.IconSize = 24;
            this.IconpictureboxMinimizeProgram.Location = new System.Drawing.Point(1198, 3);
            this.IconpictureboxMinimizeProgram.Name = "IconpictureboxMinimizeProgram";
            this.IconpictureboxMinimizeProgram.Size = new System.Drawing.Size(24, 24);
            this.IconpictureboxMinimizeProgram.TabIndex = 2;
            this.IconpictureboxMinimizeProgram.TabStop = false;
            this.IconpictureboxMinimizeProgram.Click += new System.EventHandler(this.IconpictureboxMinimizeProgram_Click);
            // 
            // IconpictureboxExitProgram
            // 
            this.IconpictureboxExitProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IconpictureboxExitProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.IconpictureboxExitProgram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IconpictureboxExitProgram.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.IconpictureboxExitProgram.IconChar = FontAwesome.Sharp.IconChar.X;
            this.IconpictureboxExitProgram.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.IconpictureboxExitProgram.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconpictureboxExitProgram.IconSize = 24;
            this.IconpictureboxExitProgram.Location = new System.Drawing.Point(1228, 3);
            this.IconpictureboxExitProgram.Name = "IconpictureboxExitProgram";
            this.IconpictureboxExitProgram.Size = new System.Drawing.Size(24, 24);
            this.IconpictureboxExitProgram.TabIndex = 0;
            this.IconpictureboxExitProgram.TabStop = false;
            this.IconpictureboxExitProgram.Click += new System.EventHandler(this.IconpictureboxExitProgram_Click);
            // 
            // PanelTitleTabBar
            // 
            this.PanelTitleTabBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.PanelTitleTabBar.Controls.Add(this.LabelUserLogin);
            this.PanelTitleTabBar.Controls.Add(this.LabelCurrentDateandRealTime);
            this.PanelTitleTabBar.Controls.Add(this.LabelTitleCurrentForm);
            this.PanelTitleTabBar.Controls.Add(this.IconpictureboxCurrentForm);
            this.PanelTitleTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTitleTabBar.Location = new System.Drawing.Point(240, 30);
            this.PanelTitleTabBar.Name = "PanelTitleTabBar";
            this.PanelTitleTabBar.Size = new System.Drawing.Size(1260, 150);
            this.PanelTitleTabBar.TabIndex = 2;
            // 
            // LabelUserLogin
            // 
            this.LabelUserLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelUserLogin.AutoSize = true;
            this.LabelUserLogin.BackColor = System.Drawing.Color.Transparent;
            this.LabelUserLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelUserLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelUserLogin.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LabelUserLogin.Location = new System.Drawing.Point(950, 90);
            this.LabelUserLogin.Name = "LabelUserLogin";
            this.LabelUserLogin.Size = new System.Drawing.Size(65, 28);
            this.LabelUserLogin.TabIndex = 3;
            this.LabelUserLogin.Text = "Guest";
            // 
            // LabelCurrentDateandRealTime
            // 
            this.LabelCurrentDateandRealTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelCurrentDateandRealTime.AutoSize = true;
            this.LabelCurrentDateandRealTime.BackColor = System.Drawing.Color.Transparent;
            this.LabelCurrentDateandRealTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelCurrentDateandRealTime.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCurrentDateandRealTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LabelCurrentDateandRealTime.Location = new System.Drawing.Point(950, 50);
            this.LabelCurrentDateandRealTime.Name = "LabelCurrentDateandRealTime";
            this.LabelCurrentDateandRealTime.Size = new System.Drawing.Size(168, 28);
            this.LabelCurrentDateandRealTime.TabIndex = 2;
            this.LabelCurrentDateandRealTime.Text = "--/--/----   --:--:--";
            // 
            // LabelTitleCurrentForm
            // 
            this.LabelTitleCurrentForm.AutoSize = true;
            this.LabelTitleCurrentForm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitleCurrentForm.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LabelTitleCurrentForm.Location = new System.Drawing.Point(90, 50);
            this.LabelTitleCurrentForm.Name = "LabelTitleCurrentForm";
            this.LabelTitleCurrentForm.Size = new System.Drawing.Size(68, 28);
            this.LabelTitleCurrentForm.TabIndex = 1;
            this.LabelTitleCurrentForm.Text = "Home";
            // 
            // IconpictureboxCurrentForm
            // 
            this.IconpictureboxCurrentForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.IconpictureboxCurrentForm.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.IconpictureboxCurrentForm.IconChar = FontAwesome.Sharp.IconChar.HomeUser;
            this.IconpictureboxCurrentForm.IconColor = System.Drawing.Color.WhiteSmoke;
            this.IconpictureboxCurrentForm.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconpictureboxCurrentForm.IconSize = 48;
            this.IconpictureboxCurrentForm.Location = new System.Drawing.Point(36, 36);
            this.IconpictureboxCurrentForm.Name = "IconpictureboxCurrentForm";
            this.IconpictureboxCurrentForm.Size = new System.Drawing.Size(48, 48);
            this.IconpictureboxCurrentForm.TabIndex = 0;
            this.IconpictureboxCurrentForm.TabStop = false;
            // 
            // PanelDesktop
            // 
            this.PanelDesktop.BackColor = System.Drawing.Color.Snow;
            this.PanelDesktop.Controls.Add(this.IconbuttonUserLogin);
            this.PanelDesktop.Controls.Add(this.IconbuttonBrowseFileVdPandHallMeasurementPath);
            this.PanelDesktop.Controls.Add(this.IconbuttonBrowseFileHallMeasurementDataPathOnly);
            this.PanelDesktop.Controls.Add(this.IconbuttonBrowseFileVdPDataPathOnly);
            this.PanelDesktop.Controls.Add(this.IconbuttonSaveFileVdPandHallMeasurementPath);
            this.PanelDesktop.Controls.Add(this.IconbuttonSaveFileHallMeasurementDataPathOnly);
            this.PanelDesktop.Controls.Add(this.IconbuttonSaveFileVdPDataPathOnly);
            this.PanelDesktop.Controls.Add(this.TextboxFileVdPandHallMeasurementDataPath);
            this.PanelDesktop.Controls.Add(this.TextboxFileHallMeasurementDataPath);
            this.PanelDesktop.Controls.Add(this.TextboxFileVdPDataPath);
            this.PanelDesktop.Controls.Add(this.LabelUserLastname);
            this.PanelDesktop.Controls.Add(this.TextboxUserLastname);
            this.PanelDesktop.Controls.Add(this.LabelUserFirstName);
            this.PanelDesktop.Controls.Add(this.TextboxUserFirstName);
            this.PanelDesktop.Controls.Add(this.LabelUserFirstNameLastname);
            this.PanelDesktop.Controls.Add(this.LabelSaveVdPandHallMeasurementData);
            this.PanelDesktop.Controls.Add(this.LabelSaveHallMeasurementDataOnly);
            this.PanelDesktop.Controls.Add(this.LabelSaveVdPDataOnly);
            this.PanelDesktop.Controls.Add(this.LabelProgramTitle);
            this.PanelDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelDesktop.Location = new System.Drawing.Point(240, 180);
            this.PanelDesktop.Name = "PanelDesktop";
            this.PanelDesktop.Size = new System.Drawing.Size(1260, 820);
            this.PanelDesktop.TabIndex = 3;
            // 
            // IconbuttonUserLogin
            // 
            this.IconbuttonUserLogin.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonUserLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonUserLogin.IconChar = FontAwesome.Sharp.IconChar.SignIn;
            this.IconbuttonUserLogin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(161)))), ((int)(((byte)(251)))));
            this.IconbuttonUserLogin.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonUserLogin.IconSize = 24;
            this.IconbuttonUserLogin.Location = new System.Drawing.Point(900, 240);
            this.IconbuttonUserLogin.Name = "IconbuttonUserLogin";
            this.IconbuttonUserLogin.Size = new System.Drawing.Size(100, 30);
            this.IconbuttonUserLogin.TabIndex = 19;
            this.IconbuttonUserLogin.Text = "Login";
            this.IconbuttonUserLogin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonUserLogin.UseVisualStyleBackColor = true;
            this.IconbuttonUserLogin.Click += new System.EventHandler(this.IconbuttonUserLogin_Click);
            // 
            // IconbuttonBrowseFileVdPandHallMeasurementPath
            // 
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.IconColor = System.Drawing.Color.Gold;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.IconSize = 24;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.Location = new System.Drawing.Point(770, 558);
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.Name = "IconbuttonBrowseFileVdPandHallMeasurementPath";
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.Size = new System.Drawing.Size(110, 30);
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.TabIndex = 18;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.Text = "Browse";
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.UseVisualStyleBackColor = true;
            this.IconbuttonBrowseFileVdPandHallMeasurementPath.Click += new System.EventHandler(this.IconbuttonBrowseFileVdPandHallMeasurementPath_Click);
            // 
            // IconbuttonBrowseFileHallMeasurementDataPathOnly
            // 
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.IconColor = System.Drawing.Color.Gold;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.IconSize = 24;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.Location = new System.Drawing.Point(770, 458);
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.Name = "IconbuttonBrowseFileHallMeasurementDataPathOnly";
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.Size = new System.Drawing.Size(110, 30);
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.TabIndex = 17;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.Text = "Browse";
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonBrowseFileHallMeasurementDataPathOnly.Click += new System.EventHandler(this.IconbuttonBrowseFileHallMeasurementDataPathOnly_Click);
            // 
            // IconbuttonBrowseFileVdPDataPathOnly
            // 
            this.IconbuttonBrowseFileVdPDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonBrowseFileVdPDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconColor = System.Drawing.Color.Gold;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconSize = 24;
            this.IconbuttonBrowseFileVdPDataPathOnly.Location = new System.Drawing.Point(770, 360);
            this.IconbuttonBrowseFileVdPDataPathOnly.Name = "IconbuttonBrowseFileVdPDataPathOnly";
            this.IconbuttonBrowseFileVdPDataPathOnly.Size = new System.Drawing.Size(110, 30);
            this.IconbuttonBrowseFileVdPDataPathOnly.TabIndex = 16;
            this.IconbuttonBrowseFileVdPDataPathOnly.Text = "Browse";
            this.IconbuttonBrowseFileVdPDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonBrowseFileVdPDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonBrowseFileVdPDataPathOnly.Click += new System.EventHandler(this.IconbuttonBrowseFileVdPDataPathOnly_Click);
            // 
            // IconbuttonSaveFileVdPandHallMeasurementPath
            // 
            this.IconbuttonSaveFileVdPandHallMeasurementPath.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.IconColor = System.Drawing.Color.Green;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.IconSize = 24;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.Location = new System.Drawing.Point(900, 558);
            this.IconbuttonSaveFileVdPandHallMeasurementPath.Name = "IconbuttonSaveFileVdPandHallMeasurementPath";
            this.IconbuttonSaveFileVdPandHallMeasurementPath.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonSaveFileVdPandHallMeasurementPath.TabIndex = 14;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.Text = "Save";
            this.IconbuttonSaveFileVdPandHallMeasurementPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.UseVisualStyleBackColor = true;
            this.IconbuttonSaveFileVdPandHallMeasurementPath.Click += new System.EventHandler(this.IconbuttonSaveFileVdPandHallMeasurementPath_Click);
            // 
            // IconbuttonSaveFileHallMeasurementDataPathOnly
            // 
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.IconColor = System.Drawing.Color.Green;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.IconSize = 24;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.Location = new System.Drawing.Point(900, 458);
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.Name = "IconbuttonSaveFileHallMeasurementDataPathOnly";
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.TabIndex = 13;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.Text = "Save";
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonSaveFileHallMeasurementDataPathOnly.Click += new System.EventHandler(this.IconbuttonSaveFileHallMeasurementDataPathOnly_Click);
            // 
            // IconbuttonSaveFileVdPDataPathOnly
            // 
            this.IconbuttonSaveFileVdPDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonSaveFileVdPDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSaveFileVdPDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.IconbuttonSaveFileVdPDataPathOnly.IconColor = System.Drawing.Color.Green;
            this.IconbuttonSaveFileVdPDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonSaveFileVdPDataPathOnly.IconSize = 24;
            this.IconbuttonSaveFileVdPDataPathOnly.Location = new System.Drawing.Point(900, 360);
            this.IconbuttonSaveFileVdPDataPathOnly.Name = "IconbuttonSaveFileVdPDataPathOnly";
            this.IconbuttonSaveFileVdPDataPathOnly.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonSaveFileVdPDataPathOnly.TabIndex = 12;
            this.IconbuttonSaveFileVdPDataPathOnly.Text = "Save";
            this.IconbuttonSaveFileVdPDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonSaveFileVdPDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonSaveFileVdPDataPathOnly.Click += new System.EventHandler(this.IconbuttonSaveFileVdPDataPathOnly_Click);
            // 
            // TextboxFileVdPandHallMeasurementDataPath
            // 
            this.TextboxFileVdPandHallMeasurementDataPath.Location = new System.Drawing.Point(85, 558);
            this.TextboxFileVdPandHallMeasurementDataPath.Multiline = true;
            this.TextboxFileVdPandHallMeasurementDataPath.Name = "TextboxFileVdPandHallMeasurementDataPath";
            this.TextboxFileVdPandHallMeasurementDataPath.ReadOnly = true;
            this.TextboxFileVdPandHallMeasurementDataPath.Size = new System.Drawing.Size(640, 30);
            this.TextboxFileVdPandHallMeasurementDataPath.TabIndex = 11;
            // 
            // TextboxFileHallMeasurementDataPath
            // 
            this.TextboxFileHallMeasurementDataPath.Location = new System.Drawing.Point(85, 458);
            this.TextboxFileHallMeasurementDataPath.Multiline = true;
            this.TextboxFileHallMeasurementDataPath.Name = "TextboxFileHallMeasurementDataPath";
            this.TextboxFileHallMeasurementDataPath.ReadOnly = true;
            this.TextboxFileHallMeasurementDataPath.Size = new System.Drawing.Size(640, 30);
            this.TextboxFileHallMeasurementDataPath.TabIndex = 10;
            // 
            // TextboxFileVdPDataPath
            // 
            this.TextboxFileVdPDataPath.Location = new System.Drawing.Point(85, 358);
            this.TextboxFileVdPDataPath.Multiline = true;
            this.TextboxFileVdPDataPath.Name = "TextboxFileVdPDataPath";
            this.TextboxFileVdPDataPath.ReadOnly = true;
            this.TextboxFileVdPDataPath.Size = new System.Drawing.Size(640, 30);
            this.TextboxFileVdPDataPath.TabIndex = 9;
            // 
            // LabelUserLastname
            // 
            this.LabelUserLastname.AutoSize = true;
            this.LabelUserLastname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelUserLastname.Location = new System.Drawing.Point(480, 240);
            this.LabelUserLastname.Name = "LabelUserLastname";
            this.LabelUserLastname.Size = new System.Drawing.Size(104, 28);
            this.LabelUserLastname.TabIndex = 8;
            this.LabelUserLastname.Text = "Lastname:";
            // 
            // TextboxUserLastname
            // 
            this.TextboxUserLastname.Location = new System.Drawing.Point(600, 240);
            this.TextboxUserLastname.Multiline = true;
            this.TextboxUserLastname.Name = "TextboxUserLastname";
            this.TextboxUserLastname.Size = new System.Drawing.Size(240, 30);
            this.TextboxUserLastname.TabIndex = 7;
            this.TextboxUserLastname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextboxUserLastname_KeyPress);
            // 
            // LabelUserFirstName
            // 
            this.LabelUserFirstName.AutoSize = true;
            this.LabelUserFirstName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelUserFirstName.Location = new System.Drawing.Point(80, 240);
            this.LabelUserFirstName.Name = "LabelUserFirstName";
            this.LabelUserFirstName.Size = new System.Drawing.Size(106, 28);
            this.LabelUserFirstName.TabIndex = 6;
            this.LabelUserFirstName.Text = "Firstname:";
            // 
            // TextboxUserFirstName
            // 
            this.TextboxUserFirstName.Location = new System.Drawing.Point(200, 240);
            this.TextboxUserFirstName.Multiline = true;
            this.TextboxUserFirstName.Name = "TextboxUserFirstName";
            this.TextboxUserFirstName.Size = new System.Drawing.Size(240, 30);
            this.TextboxUserFirstName.TabIndex = 5;
            this.TextboxUserFirstName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextboxUserName_KeyPress);
            // 
            // LabelUserFirstNameLastname
            // 
            this.LabelUserFirstNameLastname.AutoSize = true;
            this.LabelUserFirstNameLastname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelUserFirstNameLastname.Location = new System.Drawing.Point(80, 190);
            this.LabelUserFirstNameLastname.Name = "LabelUserFirstNameLastname";
            this.LabelUserFirstNameLastname.Size = new System.Drawing.Size(211, 28);
            this.LabelUserFirstNameLastname.TabIndex = 4;
            this.LabelUserFirstNameLastname.Text = "FirstName - Lastname";
            // 
            // LabelSaveVdPandHallMeasurementData
            // 
            this.LabelSaveVdPandHallMeasurementData.AutoSize = true;
            this.LabelSaveVdPandHallMeasurementData.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSaveVdPandHallMeasurementData.Location = new System.Drawing.Point(80, 510);
            this.LabelSaveVdPandHallMeasurementData.Name = "LabelSaveVdPandHallMeasurementData";
            this.LabelSaveVdPandHallMeasurementData.Size = new System.Drawing.Size(637, 28);
            this.LabelSaveVdPandHallMeasurementData.TabIndex = 3;
            this.LabelSaveVdPandHallMeasurementData.Text = "Save file data of Van der Pauw method and Hall effect measurement";
            // 
            // LabelSaveHallMeasurementDataOnly
            // 
            this.LabelSaveHallMeasurementDataOnly.AutoSize = true;
            this.LabelSaveHallMeasurementDataOnly.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSaveHallMeasurementDataOnly.Location = new System.Drawing.Point(80, 410);
            this.LabelSaveHallMeasurementDataOnly.Name = "LabelSaveHallMeasurementDataOnly";
            this.LabelSaveHallMeasurementDataOnly.Size = new System.Drawing.Size(440, 28);
            this.LabelSaveHallMeasurementDataOnly.TabIndex = 2;
            this.LabelSaveHallMeasurementDataOnly.Text = "Save file data of Hall effect measurement only ";
            // 
            // LabelSaveVdPDataOnly
            // 
            this.LabelSaveVdPDataOnly.AutoSize = true;
            this.LabelSaveVdPDataOnly.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSaveVdPDataOnly.Location = new System.Drawing.Point(80, 310);
            this.LabelSaveVdPDataOnly.Name = "LabelSaveVdPDataOnly";
            this.LabelSaveVdPDataOnly.Size = new System.Drawing.Size(417, 28);
            this.LabelSaveVdPDataOnly.TabIndex = 1;
            this.LabelSaveVdPDataOnly.Text = "Save file data of Van der Pauw method only ";
            // 
            // LabelProgramTitle
            // 
            this.LabelProgramTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelProgramTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelProgramTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LabelProgramTitle.Location = new System.Drawing.Point(30, 30);
            this.LabelProgramTitle.Name = "LabelProgramTitle";
            this.LabelProgramTitle.Size = new System.Drawing.Size(1200, 120);
            this.LabelProgramTitle.TabIndex = 0;
            this.LabelProgramTitle.Text = "SOFTWARE PROGRAM FOR CONTROLLING INSTRUMENTS OF HALL EFFECT MEASUREMENT";
            // 
            // PanelHome
            // 
            this.PanelHome.BackColor = System.Drawing.Color.White;
            this.PanelHome.Controls.Add(this.IconpictureboxLogo);
            this.PanelHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHome.Location = new System.Drawing.Point(0, 0);
            this.PanelHome.Margin = new System.Windows.Forms.Padding(0);
            this.PanelHome.Name = "PanelHome";
            this.PanelHome.Size = new System.Drawing.Size(240, 180);
            this.PanelHome.TabIndex = 0;
            // 
            // IconpictureboxLogo
            // 
            this.IconpictureboxLogo.Image = global::Program01.Properties.Resources.Applied_Physics_KMITL_Logo;
            this.IconpictureboxLogo.Location = new System.Drawing.Point(30, 0);
            this.IconpictureboxLogo.Name = "IconpictureboxLogo";
            this.IconpictureboxLogo.Size = new System.Drawing.Size(180, 180);
            this.IconpictureboxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.IconpictureboxLogo.TabIndex = 0;
            this.IconpictureboxLogo.TabStop = false;
            this.IconpictureboxLogo.Click += new System.EventHandler(this.IconpictureboxLogo_Click);
            // 
            // IconbuttonHelp
            // 
            this.IconbuttonHelp.Dock = System.Windows.Forms.DockStyle.Top;
            this.IconbuttonHelp.FlatAppearance.BorderSize = 0;
            this.IconbuttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonHelp.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonHelp.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonHelp.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            this.IconbuttonHelp.IconColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonHelp.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonHelp.IconSize = 30;
            this.IconbuttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonHelp.Location = new System.Drawing.Point(0, 180);
            this.IconbuttonHelp.Margin = new System.Windows.Forms.Padding(0);
            this.IconbuttonHelp.Name = "IconbuttonHelp";
            this.IconbuttonHelp.Size = new System.Drawing.Size(240, 60);
            this.IconbuttonHelp.TabIndex = 1;
            this.IconbuttonHelp.Text = "Help";
            this.IconbuttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonHelp.UseVisualStyleBackColor = true;
            this.IconbuttonHelp.Click += new System.EventHandler(this.IconbuttonHelp_Click);
            // 
            // PanelSideMenu
            // 
            this.PanelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.PanelSideMenu.Controls.Add(this.PanelHallMeasurementSubMenu);
            this.PanelSideMenu.Controls.Add(this.IconbuttonHalleffectMeasurement);
            this.PanelSideMenu.Controls.Add(this.PanelVanderPauwSubMenu);
            this.PanelSideMenu.Controls.Add(this.IconbuttonVanderPauwMethod);
            this.PanelSideMenu.Controls.Add(this.IconbuttonMeasurementSettings);
            this.PanelSideMenu.Controls.Add(this.IconbuttonHelp);
            this.PanelSideMenu.Controls.Add(this.PanelHome);
            this.PanelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.PanelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.PanelSideMenu.Margin = new System.Windows.Forms.Padding(0);
            this.PanelSideMenu.Name = "PanelSideMenu";
            this.PanelSideMenu.Size = new System.Drawing.Size(240, 1000);
            this.PanelSideMenu.TabIndex = 0;
            // 
            // PanelHallMeasurementSubMenu
            // 
            this.PanelHallMeasurementSubMenu.Controls.Add(this.ButtonHallMeasurementResults);
            this.PanelHallMeasurementSubMenu.Controls.Add(this.ButtonHallTotalMeasure);
            this.PanelHallMeasurementSubMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHallMeasurementSubMenu.ForeColor = System.Drawing.Color.Snow;
            this.PanelHallMeasurementSubMenu.Location = new System.Drawing.Point(0, 510);
            this.PanelHallMeasurementSubMenu.Name = "PanelHallMeasurementSubMenu";
            this.PanelHallMeasurementSubMenu.Size = new System.Drawing.Size(240, 90);
            this.PanelHallMeasurementSubMenu.TabIndex = 17;
            this.PanelHallMeasurementSubMenu.Visible = false;
            // 
            // ButtonHallMeasurementResults
            // 
            this.ButtonHallMeasurementResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonHallMeasurementResults.FlatAppearance.BorderSize = 0;
            this.ButtonHallMeasurementResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonHallMeasurementResults.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonHallMeasurementResults.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonHallMeasurementResults.Location = new System.Drawing.Point(0, 45);
            this.ButtonHallMeasurementResults.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonHallMeasurementResults.Name = "ButtonHallMeasurementResults";
            this.ButtonHallMeasurementResults.Size = new System.Drawing.Size(240, 45);
            this.ButtonHallMeasurementResults.TabIndex = 4;
            this.ButtonHallMeasurementResults.Text = "Measurement Results";
            this.ButtonHallMeasurementResults.UseVisualStyleBackColor = true;
            this.ButtonHallMeasurementResults.Click += new System.EventHandler(this.ButtonHallMeasurementResults_Click);
            // 
            // ButtonHallTotalMeasure
            // 
            this.ButtonHallTotalMeasure.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonHallTotalMeasure.FlatAppearance.BorderSize = 0;
            this.ButtonHallTotalMeasure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonHallTotalMeasure.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonHallTotalMeasure.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonHallTotalMeasure.Location = new System.Drawing.Point(0, 0);
            this.ButtonHallTotalMeasure.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonHallTotalMeasure.Name = "ButtonHallTotalMeasure";
            this.ButtonHallTotalMeasure.Size = new System.Drawing.Size(240, 45);
            this.ButtonHallTotalMeasure.TabIndex = 3;
            this.ButtonHallTotalMeasure.Text = "Total Measure";
            this.ButtonHallTotalMeasure.UseVisualStyleBackColor = true;
            this.ButtonHallTotalMeasure.Click += new System.EventHandler(this.ButtonHallTotalMeasure_Click);
            // 
            // IconbuttonHalleffectMeasurement
            // 
            this.IconbuttonHalleffectMeasurement.Dock = System.Windows.Forms.DockStyle.Top;
            this.IconbuttonHalleffectMeasurement.FlatAppearance.BorderSize = 0;
            this.IconbuttonHalleffectMeasurement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonHalleffectMeasurement.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonHalleffectMeasurement.ForeColor = System.Drawing.Color.Snow;
            this.IconbuttonHalleffectMeasurement.IconChar = FontAwesome.Sharp.IconChar.Magnet;
            this.IconbuttonHalleffectMeasurement.IconColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonHalleffectMeasurement.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonHalleffectMeasurement.IconSize = 30;
            this.IconbuttonHalleffectMeasurement.Location = new System.Drawing.Point(0, 450);
            this.IconbuttonHalleffectMeasurement.Margin = new System.Windows.Forms.Padding(0);
            this.IconbuttonHalleffectMeasurement.Name = "IconbuttonHalleffectMeasurement";
            this.IconbuttonHalleffectMeasurement.Size = new System.Drawing.Size(240, 60);
            this.IconbuttonHalleffectMeasurement.TabIndex = 16;
            this.IconbuttonHalleffectMeasurement.Text = "Hall effect Measurement";
            this.IconbuttonHalleffectMeasurement.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonHalleffectMeasurement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonHalleffectMeasurement.UseVisualStyleBackColor = true;
            this.IconbuttonHalleffectMeasurement.Click += new System.EventHandler(this.IconbuttonHalleffectMeasurement_Click);
            // 
            // PanelVanderPauwSubMenu
            // 
            this.PanelVanderPauwSubMenu.Controls.Add(this.ButtonVdPMeasurementResults);
            this.PanelVanderPauwSubMenu.Controls.Add(this.ButtonVdPTotalMeasure);
            this.PanelVanderPauwSubMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelVanderPauwSubMenu.Location = new System.Drawing.Point(0, 360);
            this.PanelVanderPauwSubMenu.Name = "PanelVanderPauwSubMenu";
            this.PanelVanderPauwSubMenu.Size = new System.Drawing.Size(240, 90);
            this.PanelVanderPauwSubMenu.TabIndex = 15;
            this.PanelVanderPauwSubMenu.Visible = false;
            // 
            // ButtonVdPMeasurementResults
            // 
            this.ButtonVdPMeasurementResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonVdPMeasurementResults.FlatAppearance.BorderSize = 0;
            this.ButtonVdPMeasurementResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonVdPMeasurementResults.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonVdPMeasurementResults.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonVdPMeasurementResults.Location = new System.Drawing.Point(0, 45);
            this.ButtonVdPMeasurementResults.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonVdPMeasurementResults.Name = "ButtonVdPMeasurementResults";
            this.ButtonVdPMeasurementResults.Size = new System.Drawing.Size(240, 45);
            this.ButtonVdPMeasurementResults.TabIndex = 4;
            this.ButtonVdPMeasurementResults.Text = "Measurement Results";
            this.ButtonVdPMeasurementResults.UseVisualStyleBackColor = true;
            this.ButtonVdPMeasurementResults.Click += new System.EventHandler(this.ButtonVdPMeasurementResults_Click);
            // 
            // ButtonVdPTotalMeasure
            // 
            this.ButtonVdPTotalMeasure.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonVdPTotalMeasure.FlatAppearance.BorderSize = 0;
            this.ButtonVdPTotalMeasure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonVdPTotalMeasure.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonVdPTotalMeasure.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonVdPTotalMeasure.Location = new System.Drawing.Point(0, 0);
            this.ButtonVdPTotalMeasure.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonVdPTotalMeasure.Name = "ButtonVdPTotalMeasure";
            this.ButtonVdPTotalMeasure.Size = new System.Drawing.Size(240, 45);
            this.ButtonVdPTotalMeasure.TabIndex = 3;
            this.ButtonVdPTotalMeasure.Text = "Total Measure";
            this.ButtonVdPTotalMeasure.UseVisualStyleBackColor = true;
            this.ButtonVdPTotalMeasure.Click += new System.EventHandler(this.ButtonVdPTotalMeasure_Click);
            // 
            // IconbuttonVanderPauwMethod
            // 
            this.IconbuttonVanderPauwMethod.Dock = System.Windows.Forms.DockStyle.Top;
            this.IconbuttonVanderPauwMethod.FlatAppearance.BorderSize = 0;
            this.IconbuttonVanderPauwMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonVanderPauwMethod.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonVanderPauwMethod.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonVanderPauwMethod.IconChar = FontAwesome.Sharp.IconChar.Diamond;
            this.IconbuttonVanderPauwMethod.IconColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonVanderPauwMethod.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonVanderPauwMethod.IconSize = 30;
            this.IconbuttonVanderPauwMethod.Location = new System.Drawing.Point(0, 300);
            this.IconbuttonVanderPauwMethod.Margin = new System.Windows.Forms.Padding(0);
            this.IconbuttonVanderPauwMethod.Name = "IconbuttonVanderPauwMethod";
            this.IconbuttonVanderPauwMethod.Size = new System.Drawing.Size(240, 60);
            this.IconbuttonVanderPauwMethod.TabIndex = 14;
            this.IconbuttonVanderPauwMethod.Text = "Van der Pauw Method";
            this.IconbuttonVanderPauwMethod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonVanderPauwMethod.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonVanderPauwMethod.UseVisualStyleBackColor = true;
            this.IconbuttonVanderPauwMethod.Click += new System.EventHandler(this.IconbuttonVanderPauwMethod_Click);
            // 
            // IconbuttonMeasurementSettings
            // 
            this.IconbuttonMeasurementSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.IconbuttonMeasurementSettings.FlatAppearance.BorderSize = 0;
            this.IconbuttonMeasurementSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonMeasurementSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconbuttonMeasurementSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonMeasurementSettings.IconChar = FontAwesome.Sharp.IconChar.Gears;
            this.IconbuttonMeasurementSettings.IconColor = System.Drawing.Color.WhiteSmoke;
            this.IconbuttonMeasurementSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.IconbuttonMeasurementSettings.IconSize = 30;
            this.IconbuttonMeasurementSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonMeasurementSettings.Location = new System.Drawing.Point(0, 240);
            this.IconbuttonMeasurementSettings.Margin = new System.Windows.Forms.Padding(0);
            this.IconbuttonMeasurementSettings.Name = "IconbuttonMeasurementSettings";
            this.IconbuttonMeasurementSettings.Size = new System.Drawing.Size(240, 60);
            this.IconbuttonMeasurementSettings.TabIndex = 2;
            this.IconbuttonMeasurementSettings.Text = "Measurement Settings";
            this.IconbuttonMeasurementSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IconbuttonMeasurementSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonMeasurementSettings.UseVisualStyleBackColor = true;
            this.IconbuttonMeasurementSettings.Click += new System.EventHandler(this.IconbuttonMeasurementSettings_Click);
            // 
            // TimerCurrentDateandRealTime
            // 
            this.TimerCurrentDateandRealTime.Interval = 1000;
            this.TimerCurrentDateandRealTime.Tick += new System.EventHandler(this.TimerCurrentDateandRealTime_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1500, 1000);
            this.Controls.Add(this.PanelDesktop);
            this.Controls.Add(this.PanelTitleTabBar);
            this.Controls.Add(this.PanelTabBar);
            this.Controls.Add(this.PanelSideMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.PanelTabBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxMinimizeProgram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxExitProgram)).EndInit();
            this.PanelTitleTabBar.ResumeLayout(false);
            this.PanelTitleTabBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxCurrentForm)).EndInit();
            this.PanelDesktop.ResumeLayout(false);
            this.PanelDesktop.PerformLayout();
            this.PanelHome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.IconpictureboxLogo)).EndInit();
            this.PanelSideMenu.ResumeLayout(false);
            this.PanelHallMeasurementSubMenu.ResumeLayout(false);
            this.PanelVanderPauwSubMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelTabBar;
        private FontAwesome.Sharp.IconPictureBox IconpictureboxExitProgram;
        private FontAwesome.Sharp.IconPictureBox IconpictureboxMinimizeProgram;
        private System.Windows.Forms.Panel PanelTitleTabBar;
        private FontAwesome.Sharp.IconPictureBox IconpictureboxCurrentForm;
        private System.Windows.Forms.Label LabelTitleCurrentForm;
        private System.Windows.Forms.Panel PanelDesktop;
        private System.Windows.Forms.Panel PanelHome;
        private System.Windows.Forms.PictureBox IconpictureboxLogo;
        private FontAwesome.Sharp.IconButton IconbuttonHelp;
        private System.Windows.Forms.Panel PanelSideMenu;
        private FontAwesome.Sharp.IconButton IconbuttonMeasurementSettings;
        private System.Windows.Forms.Panel PanelHallMeasurementSubMenu;
        private System.Windows.Forms.Button ButtonHallMeasurementResults;
        private System.Windows.Forms.Button ButtonHallTotalMeasure;
        private FontAwesome.Sharp.IconButton IconbuttonHalleffectMeasurement;
        private System.Windows.Forms.Panel PanelVanderPauwSubMenu;
        private System.Windows.Forms.Button ButtonVdPMeasurementResults;
        private System.Windows.Forms.Button ButtonVdPTotalMeasure;
        private FontAwesome.Sharp.IconButton IconbuttonVanderPauwMethod;
        private System.Windows.Forms.Label LabelProgramTitle;
        private System.Windows.Forms.Label LabelSaveVdPDataOnly;
        private System.Windows.Forms.Label LabelUserFirstNameLastname;
        private System.Windows.Forms.Label LabelSaveVdPandHallMeasurementData;
        private System.Windows.Forms.Label LabelSaveHallMeasurementDataOnly;
        private System.Windows.Forms.Label LabelUserLastname;
        private System.Windows.Forms.TextBox TextboxUserLastname;
        private System.Windows.Forms.Label LabelUserFirstName;
        private System.Windows.Forms.TextBox TextboxUserFirstName;
        private System.Windows.Forms.TextBox TextboxFileVdPandHallMeasurementDataPath;
        private System.Windows.Forms.TextBox TextboxFileHallMeasurementDataPath;
        private System.Windows.Forms.TextBox TextboxFileVdPDataPath;
        private FontAwesome.Sharp.IconButton IconbuttonSaveFileVdPandHallMeasurementPath;
        private FontAwesome.Sharp.IconButton IconbuttonSaveFileHallMeasurementDataPathOnly;
        private FontAwesome.Sharp.IconButton IconbuttonSaveFileVdPDataPathOnly;
        private FontAwesome.Sharp.IconButton IconbuttonBrowseFileVdPDataPathOnly;
        private FontAwesome.Sharp.IconButton IconbuttonBrowseFileVdPandHallMeasurementPath;
        private FontAwesome.Sharp.IconButton IconbuttonBrowseFileHallMeasurementDataPathOnly;
        private System.Windows.Forms.Label LabelCurrentDateandRealTime;
        private System.Windows.Forms.Timer TimerCurrentDateandRealTime;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogVdPFile;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogHallMeasurementFile;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogVdPandHallMeasurementFile;
        private System.Windows.Forms.Label LabelUserLogin;
        private FontAwesome.Sharp.IconButton IconbuttonUserLogin;
    }
}

