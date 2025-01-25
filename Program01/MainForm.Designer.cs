namespace Program01
{
    partial class Program01Form
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
            this.panelTabBar = new System.Windows.Forms.Panel();
            this.iconpictureboxMinimizeProgram = new FontAwesome.Sharp.IconPictureBox();
            this.iconpictureboxExitProgram = new FontAwesome.Sharp.IconPictureBox();
            this.panelTitleTabBar = new System.Windows.Forms.Panel();
            this.labelUserLogin = new System.Windows.Forms.Label();
            this.labelCurrentDateandRealTime = new System.Windows.Forms.Label();
            this.labelTitleCurrentForm = new System.Windows.Forms.Label();
            this.iconpictureboxCurrentForm = new FontAwesome.Sharp.IconPictureBox();
            this.panelDesktop = new System.Windows.Forms.Panel();
            this.iconbuttonUserLogin = new FontAwesome.Sharp.IconButton();
            this.iconbuttonBrowseFileVdPandHallMeasurementPath = new FontAwesome.Sharp.IconButton();
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.IconbuttonBrowseFileVdPDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.iconbuttonSaveFileVdPandHallMeasurementPath = new FontAwesome.Sharp.IconButton();
            this.iconbuttonSaveFileHallMeasurementDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.IconbuttonSaveFileVdPDataPathOnly = new FontAwesome.Sharp.IconButton();
            this.textboxFileVdPandHallMeasurementDataPath = new System.Windows.Forms.TextBox();
            this.textboxFileHallMeasurementDataPath = new System.Windows.Forms.TextBox();
            this.textboxFileVdPDataPath = new System.Windows.Forms.TextBox();
            this.labelUserLastname = new System.Windows.Forms.Label();
            this.textboxUserLastname = new System.Windows.Forms.TextBox();
            this.labelUserFirstName = new System.Windows.Forms.Label();
            this.textboxUserFirstName = new System.Windows.Forms.TextBox();
            this.labelUserFirstNameLastname = new System.Windows.Forms.Label();
            this.labelSaveVdPandHallMeasurementData = new System.Windows.Forms.Label();
            this.labelSaveHallMeasurementDataOnly = new System.Windows.Forms.Label();
            this.labelSaveVdPDataOnly = new System.Windows.Forms.Label();
            this.labelProgramTitle = new System.Windows.Forms.Label();
            this.panelHome = new System.Windows.Forms.Panel();
            this.iconpictureboxLogo = new System.Windows.Forms.PictureBox();
            this.iconbuttonHelp = new FontAwesome.Sharp.IconButton();
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.PanelHallMeasurementSubMenu = new System.Windows.Forms.Panel();
            this.buttonHallMeasurementResults = new System.Windows.Forms.Button();
            this.buttonHallTotalVoltage = new System.Windows.Forms.Button();
            this.iconbuttonHalleffectMeasurement = new FontAwesome.Sharp.IconButton();
            this.PanelVanderPauwSubMenu = new System.Windows.Forms.Panel();
            this.buttonVdPMeasurementResults = new System.Windows.Forms.Button();
            this.buttonVdPTotalVoltage = new System.Windows.Forms.Button();
            this.iconbuttonVanderPauwMethod = new FontAwesome.Sharp.IconButton();
            this.iconbuttonMeasurementSettings = new FontAwesome.Sharp.IconButton();
            this.TimerCurrentDateandRealTime = new System.Windows.Forms.Timer(this.components);
            this.FolderBrowserDialogVdPFile = new System.Windows.Forms.FolderBrowserDialog();
            this.FolderBrowserDialogHallMeasurementFile = new System.Windows.Forms.FolderBrowserDialog();
            this.FolderBrowserDialogVdPandHallMeasurementFile = new System.Windows.Forms.FolderBrowserDialog();
            this.panelTabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxMinimizeProgram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxExitProgram)).BeginInit();
            this.panelTitleTabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxCurrentForm)).BeginInit();
            this.panelDesktop.SuspendLayout();
            this.panelHome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxLogo)).BeginInit();
            this.panelSideMenu.SuspendLayout();
            this.PanelHallMeasurementSubMenu.SuspendLayout();
            this.PanelVanderPauwSubMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTabBar
            // 
            this.panelTabBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.panelTabBar.Controls.Add(this.iconpictureboxMinimizeProgram);
            this.panelTabBar.Controls.Add(this.iconpictureboxExitProgram);
            this.panelTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTabBar.Location = new System.Drawing.Point(240, 0);
            this.panelTabBar.Name = "panelTabBar";
            this.panelTabBar.Size = new System.Drawing.Size(1260, 30);
            this.panelTabBar.TabIndex = 1;
            this.panelTabBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTabBar_MouseDown);
            // 
            // iconpictureboxMinimizeProgram
            // 
            this.iconpictureboxMinimizeProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconpictureboxMinimizeProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.iconpictureboxMinimizeProgram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.iconpictureboxMinimizeProgram.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.iconpictureboxMinimizeProgram.IconChar = FontAwesome.Sharp.IconChar.Minus;
            this.iconpictureboxMinimizeProgram.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.iconpictureboxMinimizeProgram.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconpictureboxMinimizeProgram.IconSize = 24;
            this.iconpictureboxMinimizeProgram.Location = new System.Drawing.Point(1198, 3);
            this.iconpictureboxMinimizeProgram.Name = "iconpictureboxMinimizeProgram";
            this.iconpictureboxMinimizeProgram.Size = new System.Drawing.Size(24, 24);
            this.iconpictureboxMinimizeProgram.TabIndex = 2;
            this.iconpictureboxMinimizeProgram.TabStop = false;
            this.iconpictureboxMinimizeProgram.Click += new System.EventHandler(this.iconpictureboxMinimizeProgram_Click);
            // 
            // iconpictureboxExitProgram
            // 
            this.iconpictureboxExitProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconpictureboxExitProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.iconpictureboxExitProgram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.iconpictureboxExitProgram.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.iconpictureboxExitProgram.IconChar = FontAwesome.Sharp.IconChar.X;
            this.iconpictureboxExitProgram.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.iconpictureboxExitProgram.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconpictureboxExitProgram.IconSize = 24;
            this.iconpictureboxExitProgram.Location = new System.Drawing.Point(1228, 3);
            this.iconpictureboxExitProgram.Name = "iconpictureboxExitProgram";
            this.iconpictureboxExitProgram.Size = new System.Drawing.Size(24, 24);
            this.iconpictureboxExitProgram.TabIndex = 0;
            this.iconpictureboxExitProgram.TabStop = false;
            this.iconpictureboxExitProgram.Click += new System.EventHandler(this.iconpictureboxExitProgram_Click);
            // 
            // panelTitleTabBar
            // 
            this.panelTitleTabBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.panelTitleTabBar.Controls.Add(this.labelUserLogin);
            this.panelTitleTabBar.Controls.Add(this.labelCurrentDateandRealTime);
            this.panelTitleTabBar.Controls.Add(this.labelTitleCurrentForm);
            this.panelTitleTabBar.Controls.Add(this.iconpictureboxCurrentForm);
            this.panelTitleTabBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitleTabBar.Location = new System.Drawing.Point(240, 30);
            this.panelTitleTabBar.Name = "panelTitleTabBar";
            this.panelTitleTabBar.Size = new System.Drawing.Size(1260, 150);
            this.panelTitleTabBar.TabIndex = 2;
            // 
            // labelUserLogin
            // 
            this.labelUserLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserLogin.AutoSize = true;
            this.labelUserLogin.BackColor = System.Drawing.Color.Transparent;
            this.labelUserLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelUserLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserLogin.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelUserLogin.Location = new System.Drawing.Point(950, 90);
            this.labelUserLogin.Name = "labelUserLogin";
            this.labelUserLogin.Size = new System.Drawing.Size(65, 28);
            this.labelUserLogin.TabIndex = 3;
            this.labelUserLogin.Text = "Guest";
            // 
            // labelCurrentDateandRealTime
            // 
            this.labelCurrentDateandRealTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentDateandRealTime.AutoSize = true;
            this.labelCurrentDateandRealTime.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentDateandRealTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelCurrentDateandRealTime.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentDateandRealTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelCurrentDateandRealTime.Location = new System.Drawing.Point(950, 50);
            this.labelCurrentDateandRealTime.Name = "labelCurrentDateandRealTime";
            this.labelCurrentDateandRealTime.Size = new System.Drawing.Size(168, 28);
            this.labelCurrentDateandRealTime.TabIndex = 2;
            this.labelCurrentDateandRealTime.Text = "--/--/----   --:--:--";
            // 
            // labelTitleCurrentForm
            // 
            this.labelTitleCurrentForm.AutoSize = true;
            this.labelTitleCurrentForm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitleCurrentForm.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTitleCurrentForm.Location = new System.Drawing.Point(90, 50);
            this.labelTitleCurrentForm.Name = "labelTitleCurrentForm";
            this.labelTitleCurrentForm.Size = new System.Drawing.Size(68, 28);
            this.labelTitleCurrentForm.TabIndex = 1;
            this.labelTitleCurrentForm.Text = "Home";
            // 
            // iconpictureboxCurrentForm
            // 
            this.iconpictureboxCurrentForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.iconpictureboxCurrentForm.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.iconpictureboxCurrentForm.IconChar = FontAwesome.Sharp.IconChar.HomeUser;
            this.iconpictureboxCurrentForm.IconColor = System.Drawing.Color.WhiteSmoke;
            this.iconpictureboxCurrentForm.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconpictureboxCurrentForm.IconSize = 48;
            this.iconpictureboxCurrentForm.Location = new System.Drawing.Point(36, 36);
            this.iconpictureboxCurrentForm.Name = "iconpictureboxCurrentForm";
            this.iconpictureboxCurrentForm.Size = new System.Drawing.Size(48, 48);
            this.iconpictureboxCurrentForm.TabIndex = 0;
            this.iconpictureboxCurrentForm.TabStop = false;
            // 
            // panelDesktop
            // 
            this.panelDesktop.BackColor = System.Drawing.Color.Snow;
            this.panelDesktop.Controls.Add(this.iconbuttonUserLogin);
            this.panelDesktop.Controls.Add(this.iconbuttonBrowseFileVdPandHallMeasurementPath);
            this.panelDesktop.Controls.Add(this.iconbuttonBrowseFileHallMeasurementDataPathOnly);
            this.panelDesktop.Controls.Add(this.IconbuttonBrowseFileVdPDataPathOnly);
            this.panelDesktop.Controls.Add(this.iconbuttonSaveFileVdPandHallMeasurementPath);
            this.panelDesktop.Controls.Add(this.iconbuttonSaveFileHallMeasurementDataPathOnly);
            this.panelDesktop.Controls.Add(this.IconbuttonSaveFileVdPDataPathOnly);
            this.panelDesktop.Controls.Add(this.textboxFileVdPandHallMeasurementDataPath);
            this.panelDesktop.Controls.Add(this.textboxFileHallMeasurementDataPath);
            this.panelDesktop.Controls.Add(this.textboxFileVdPDataPath);
            this.panelDesktop.Controls.Add(this.labelUserLastname);
            this.panelDesktop.Controls.Add(this.textboxUserLastname);
            this.panelDesktop.Controls.Add(this.labelUserFirstName);
            this.panelDesktop.Controls.Add(this.textboxUserFirstName);
            this.panelDesktop.Controls.Add(this.labelUserFirstNameLastname);
            this.panelDesktop.Controls.Add(this.labelSaveVdPandHallMeasurementData);
            this.panelDesktop.Controls.Add(this.labelSaveHallMeasurementDataOnly);
            this.panelDesktop.Controls.Add(this.labelSaveVdPDataOnly);
            this.panelDesktop.Controls.Add(this.labelProgramTitle);
            this.panelDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDesktop.Location = new System.Drawing.Point(240, 180);
            this.panelDesktop.Name = "panelDesktop";
            this.panelDesktop.Size = new System.Drawing.Size(1260, 820);
            this.panelDesktop.TabIndex = 3;
            // 
            // iconbuttonUserLogin
            // 
            this.iconbuttonUserLogin.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.iconbuttonUserLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonUserLogin.IconChar = FontAwesome.Sharp.IconChar.User;
            this.iconbuttonUserLogin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(161)))), ((int)(((byte)(251)))));
            this.iconbuttonUserLogin.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconbuttonUserLogin.IconSize = 24;
            this.iconbuttonUserLogin.Location = new System.Drawing.Point(720, 190);
            this.iconbuttonUserLogin.Name = "iconbuttonUserLogin";
            this.iconbuttonUserLogin.Size = new System.Drawing.Size(100, 30);
            this.iconbuttonUserLogin.TabIndex = 19;
            this.iconbuttonUserLogin.Text = "Login";
            this.iconbuttonUserLogin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonUserLogin.UseVisualStyleBackColor = true;
            this.iconbuttonUserLogin.Click += new System.EventHandler(this.iconbuttonUserLogin_Click);
            // 
            // iconbuttonBrowseFileVdPandHallMeasurementPath
            // 
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.IconColor = System.Drawing.Color.Gold;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.IconSize = 24;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.Location = new System.Drawing.Point(460, 498);
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.Name = "iconbuttonBrowseFileVdPandHallMeasurementPath";
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.Size = new System.Drawing.Size(110, 30);
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.TabIndex = 18;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.Text = "Browse";
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.UseVisualStyleBackColor = true;
            this.iconbuttonBrowseFileVdPandHallMeasurementPath.Click += new System.EventHandler(this.iconbuttonBrowseFileVdPandHallMeasurementPath_Click);
            // 
            // iconbuttonBrowseFileHallMeasurementDataPathOnly
            // 
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.IconColor = System.Drawing.Color.Gold;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.IconSize = 24;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.Location = new System.Drawing.Point(460, 398);
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.Name = "iconbuttonBrowseFileHallMeasurementDataPathOnly";
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.Size = new System.Drawing.Size(110, 30);
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.TabIndex = 17;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.Text = "Browse";
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.UseVisualStyleBackColor = true;
            this.iconbuttonBrowseFileHallMeasurementDataPathOnly.Click += new System.EventHandler(this.iconbuttonBrowseFileHallMeasurementDataPathOnly_Click);
            // 
            // IconbuttonBrowseFileVdPDataPathOnly
            // 
            this.IconbuttonBrowseFileVdPDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonBrowseFileVdPDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconColor = System.Drawing.Color.Gold;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonBrowseFileVdPDataPathOnly.IconSize = 24;
            this.IconbuttonBrowseFileVdPDataPathOnly.Location = new System.Drawing.Point(460, 300);
            this.IconbuttonBrowseFileVdPDataPathOnly.Name = "IconbuttonBrowseFileVdPDataPathOnly";
            this.IconbuttonBrowseFileVdPDataPathOnly.Size = new System.Drawing.Size(110, 30);
            this.IconbuttonBrowseFileVdPDataPathOnly.TabIndex = 16;
            this.IconbuttonBrowseFileVdPDataPathOnly.Text = "Browse";
            this.IconbuttonBrowseFileVdPDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonBrowseFileVdPDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonBrowseFileVdPDataPathOnly.Click += new System.EventHandler(this.iconbuttonBrowseFileVdPDataPathOnly_Click);
            // 
            // iconbuttonSaveFileVdPandHallMeasurementPath
            // 
            this.iconbuttonSaveFileVdPandHallMeasurementPath.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.IconColor = System.Drawing.Color.Green;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.IconSize = 24;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.Location = new System.Drawing.Point(590, 498);
            this.iconbuttonSaveFileVdPandHallMeasurementPath.Name = "iconbuttonSaveFileVdPandHallMeasurementPath";
            this.iconbuttonSaveFileVdPandHallMeasurementPath.Size = new System.Drawing.Size(90, 30);
            this.iconbuttonSaveFileVdPandHallMeasurementPath.TabIndex = 14;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.Text = "Save";
            this.iconbuttonSaveFileVdPandHallMeasurementPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.UseVisualStyleBackColor = true;
            this.iconbuttonSaveFileVdPandHallMeasurementPath.Click += new System.EventHandler(this.iconbuttonSaveFileVdPandHallMeasurementPath_Click);
            // 
            // iconbuttonSaveFileHallMeasurementDataPathOnly
            // 
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.IconColor = System.Drawing.Color.Green;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.IconSize = 24;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.Location = new System.Drawing.Point(590, 398);
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.Name = "iconbuttonSaveFileHallMeasurementDataPathOnly";
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.Size = new System.Drawing.Size(90, 30);
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.TabIndex = 13;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.Text = "Save";
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.UseVisualStyleBackColor = true;
            this.iconbuttonSaveFileHallMeasurementDataPathOnly.Click += new System.EventHandler(this.iconbuttonSaveFileHallMeasurementDataPathOnly_Click);
            // 
            // IconbuttonSaveFileVdPDataPathOnly
            // 
            this.IconbuttonSaveFileVdPDataPathOnly.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.IconbuttonSaveFileVdPDataPathOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IconbuttonSaveFileVdPDataPathOnly.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.IconbuttonSaveFileVdPDataPathOnly.IconColor = System.Drawing.Color.Green;
            this.IconbuttonSaveFileVdPDataPathOnly.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.IconbuttonSaveFileVdPDataPathOnly.IconSize = 24;
            this.IconbuttonSaveFileVdPDataPathOnly.Location = new System.Drawing.Point(590, 300);
            this.IconbuttonSaveFileVdPDataPathOnly.Name = "IconbuttonSaveFileVdPDataPathOnly";
            this.IconbuttonSaveFileVdPDataPathOnly.Size = new System.Drawing.Size(90, 30);
            this.IconbuttonSaveFileVdPDataPathOnly.TabIndex = 12;
            this.IconbuttonSaveFileVdPDataPathOnly.Text = "Save";
            this.IconbuttonSaveFileVdPDataPathOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IconbuttonSaveFileVdPDataPathOnly.UseVisualStyleBackColor = true;
            this.IconbuttonSaveFileVdPDataPathOnly.Click += new System.EventHandler(this.iconbuttonSaveFileVdPDataPathOnly_Click);
            // 
            // textboxFileVdPandHallMeasurementDataPath
            // 
            this.textboxFileVdPandHallMeasurementDataPath.Location = new System.Drawing.Point(85, 498);
            this.textboxFileVdPandHallMeasurementDataPath.Multiline = true;
            this.textboxFileVdPandHallMeasurementDataPath.Name = "textboxFileVdPandHallMeasurementDataPath";
            this.textboxFileVdPandHallMeasurementDataPath.ReadOnly = true;
            this.textboxFileVdPandHallMeasurementDataPath.Size = new System.Drawing.Size(360, 30);
            this.textboxFileVdPandHallMeasurementDataPath.TabIndex = 11;
            // 
            // textboxFileHallMeasurementDataPath
            // 
            this.textboxFileHallMeasurementDataPath.Location = new System.Drawing.Point(85, 398);
            this.textboxFileHallMeasurementDataPath.Multiline = true;
            this.textboxFileHallMeasurementDataPath.Name = "textboxFileHallMeasurementDataPath";
            this.textboxFileHallMeasurementDataPath.ReadOnly = true;
            this.textboxFileHallMeasurementDataPath.Size = new System.Drawing.Size(360, 30);
            this.textboxFileHallMeasurementDataPath.TabIndex = 10;
            // 
            // textboxFileVdPDataPath
            // 
            this.textboxFileVdPDataPath.Location = new System.Drawing.Point(85, 298);
            this.textboxFileVdPDataPath.Multiline = true;
            this.textboxFileVdPDataPath.Name = "textboxFileVdPDataPath";
            this.textboxFileVdPDataPath.ReadOnly = true;
            this.textboxFileVdPDataPath.Size = new System.Drawing.Size(360, 30);
            this.textboxFileVdPDataPath.TabIndex = 9;
            // 
            // labelUserLastname
            // 
            this.labelUserLastname.AutoSize = true;
            this.labelUserLastname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserLastname.Location = new System.Drawing.Point(400, 190);
            this.labelUserLastname.Name = "labelUserLastname";
            this.labelUserLastname.Size = new System.Drawing.Size(104, 28);
            this.labelUserLastname.TabIndex = 8;
            this.labelUserLastname.Text = "Lastname:";
            // 
            // textboxUserLastname
            // 
            this.textboxUserLastname.Location = new System.Drawing.Point(510, 190);
            this.textboxUserLastname.Multiline = true;
            this.textboxUserLastname.Name = "textboxUserLastname";
            this.textboxUserLastname.Size = new System.Drawing.Size(180, 30);
            this.textboxUserLastname.TabIndex = 7;
            this.textboxUserLastname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxUserLastname_KeyPress);
            // 
            // labelUserFirstName
            // 
            this.labelUserFirstName.AutoSize = true;
            this.labelUserFirstName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserFirstName.Location = new System.Drawing.Point(80, 190);
            this.labelUserFirstName.Name = "labelUserFirstName";
            this.labelUserFirstName.Size = new System.Drawing.Size(106, 28);
            this.labelUserFirstName.TabIndex = 6;
            this.labelUserFirstName.Text = "Firstname:";
            // 
            // textboxUserFirstName
            // 
            this.textboxUserFirstName.Location = new System.Drawing.Point(190, 190);
            this.textboxUserFirstName.Multiline = true;
            this.textboxUserFirstName.Name = "textboxUserFirstName";
            this.textboxUserFirstName.Size = new System.Drawing.Size(180, 30);
            this.textboxUserFirstName.TabIndex = 5;
            this.textboxUserFirstName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxUserName_KeyPress);
            // 
            // labelUserFirstNameLastname
            // 
            this.labelUserFirstNameLastname.AutoSize = true;
            this.labelUserFirstNameLastname.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserFirstNameLastname.Location = new System.Drawing.Point(80, 150);
            this.labelUserFirstNameLastname.Name = "labelUserFirstNameLastname";
            this.labelUserFirstNameLastname.Size = new System.Drawing.Size(211, 28);
            this.labelUserFirstNameLastname.TabIndex = 4;
            this.labelUserFirstNameLastname.Text = "FirstName - Lastname";
            // 
            // labelSaveVdPandHallMeasurementData
            // 
            this.labelSaveVdPandHallMeasurementData.AutoSize = true;
            this.labelSaveVdPandHallMeasurementData.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSaveVdPandHallMeasurementData.Location = new System.Drawing.Point(80, 450);
            this.labelSaveVdPandHallMeasurementData.Name = "labelSaveVdPandHallMeasurementData";
            this.labelSaveVdPandHallMeasurementData.Size = new System.Drawing.Size(637, 28);
            this.labelSaveVdPandHallMeasurementData.TabIndex = 3;
            this.labelSaveVdPandHallMeasurementData.Text = "Save file data of Van der Pauw method and Hall effect measurement";
            // 
            // labelSaveHallMeasurementDataOnly
            // 
            this.labelSaveHallMeasurementDataOnly.AutoSize = true;
            this.labelSaveHallMeasurementDataOnly.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSaveHallMeasurementDataOnly.Location = new System.Drawing.Point(80, 350);
            this.labelSaveHallMeasurementDataOnly.Name = "labelSaveHallMeasurementDataOnly";
            this.labelSaveHallMeasurementDataOnly.Size = new System.Drawing.Size(440, 28);
            this.labelSaveHallMeasurementDataOnly.TabIndex = 2;
            this.labelSaveHallMeasurementDataOnly.Text = "Save file data of Hall effect measurement only ";
            // 
            // labelSaveVdPDataOnly
            // 
            this.labelSaveVdPDataOnly.AutoSize = true;
            this.labelSaveVdPDataOnly.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSaveVdPDataOnly.Location = new System.Drawing.Point(80, 250);
            this.labelSaveVdPDataOnly.Name = "labelSaveVdPDataOnly";
            this.labelSaveVdPDataOnly.Size = new System.Drawing.Size(417, 28);
            this.labelSaveVdPDataOnly.TabIndex = 1;
            this.labelSaveVdPDataOnly.Text = "Save file data of Van der Pauw method only ";
            // 
            // labelProgramTitle
            // 
            this.labelProgramTitle.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgramTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelProgramTitle.Location = new System.Drawing.Point(30, 30);
            this.labelProgramTitle.Name = "labelProgramTitle";
            this.labelProgramTitle.Size = new System.Drawing.Size(1200, 100);
            this.labelProgramTitle.TabIndex = 0;
            this.labelProgramTitle.Text = "SOFTWARE PROGRAM FOR CONTROLLING INSTRUMENTS OF HALL EFFECT MEASUREMENT";
            // 
            // panelHome
            // 
            this.panelHome.BackColor = System.Drawing.Color.White;
            this.panelHome.Controls.Add(this.iconpictureboxLogo);
            this.panelHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHome.Location = new System.Drawing.Point(0, 0);
            this.panelHome.Margin = new System.Windows.Forms.Padding(0);
            this.panelHome.Name = "panelHome";
            this.panelHome.Size = new System.Drawing.Size(240, 180);
            this.panelHome.TabIndex = 0;
            // 
            // iconpictureboxLogo
            // 
            this.iconpictureboxLogo.Image = global::Program01.Properties.Resources.Applied_Physics_KMITL_Logo;
            this.iconpictureboxLogo.Location = new System.Drawing.Point(30, 0);
            this.iconpictureboxLogo.Name = "iconpictureboxLogo";
            this.iconpictureboxLogo.Size = new System.Drawing.Size(180, 180);
            this.iconpictureboxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconpictureboxLogo.TabIndex = 0;
            this.iconpictureboxLogo.TabStop = false;
            this.iconpictureboxLogo.Click += new System.EventHandler(this.iconpictureboxLogo_Click);
            // 
            // iconbuttonHelp
            // 
            this.iconbuttonHelp.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconbuttonHelp.FlatAppearance.BorderSize = 0;
            this.iconbuttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonHelp.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconbuttonHelp.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonHelp.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            this.iconbuttonHelp.IconColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonHelp.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconbuttonHelp.IconSize = 30;
            this.iconbuttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonHelp.Location = new System.Drawing.Point(0, 180);
            this.iconbuttonHelp.Margin = new System.Windows.Forms.Padding(0);
            this.iconbuttonHelp.Name = "iconbuttonHelp";
            this.iconbuttonHelp.Size = new System.Drawing.Size(240, 60);
            this.iconbuttonHelp.TabIndex = 1;
            this.iconbuttonHelp.Text = "Help";
            this.iconbuttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonHelp.UseVisualStyleBackColor = true;
            this.iconbuttonHelp.Click += new System.EventHandler(this.iconbuttonHelp_Click);
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(33)))), ((int)(((byte)(80)))));
            this.panelSideMenu.Controls.Add(this.PanelHallMeasurementSubMenu);
            this.panelSideMenu.Controls.Add(this.iconbuttonHalleffectMeasurement);
            this.panelSideMenu.Controls.Add(this.PanelVanderPauwSubMenu);
            this.panelSideMenu.Controls.Add(this.iconbuttonVanderPauwMethod);
            this.panelSideMenu.Controls.Add(this.iconbuttonMeasurementSettings);
            this.panelSideMenu.Controls.Add(this.iconbuttonHelp);
            this.panelSideMenu.Controls.Add(this.panelHome);
            this.panelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(240, 1000);
            this.panelSideMenu.TabIndex = 0;
            // 
            // PanelHallMeasurementSubMenu
            // 
            this.PanelHallMeasurementSubMenu.Controls.Add(this.buttonHallMeasurementResults);
            this.PanelHallMeasurementSubMenu.Controls.Add(this.buttonHallTotalVoltage);
            this.PanelHallMeasurementSubMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHallMeasurementSubMenu.ForeColor = System.Drawing.Color.Snow;
            this.PanelHallMeasurementSubMenu.Location = new System.Drawing.Point(0, 510);
            this.PanelHallMeasurementSubMenu.Name = "PanelHallMeasurementSubMenu";
            this.PanelHallMeasurementSubMenu.Size = new System.Drawing.Size(240, 90);
            this.PanelHallMeasurementSubMenu.TabIndex = 17;
            this.PanelHallMeasurementSubMenu.Visible = false;
            // 
            // buttonHallMeasurementResults
            // 
            this.buttonHallMeasurementResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonHallMeasurementResults.FlatAppearance.BorderSize = 0;
            this.buttonHallMeasurementResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHallMeasurementResults.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHallMeasurementResults.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.buttonHallMeasurementResults.Location = new System.Drawing.Point(0, 45);
            this.buttonHallMeasurementResults.Margin = new System.Windows.Forms.Padding(0);
            this.buttonHallMeasurementResults.Name = "buttonHallMeasurementResults";
            this.buttonHallMeasurementResults.Size = new System.Drawing.Size(240, 45);
            this.buttonHallMeasurementResults.TabIndex = 4;
            this.buttonHallMeasurementResults.Text = "Measurement Results";
            this.buttonHallMeasurementResults.UseVisualStyleBackColor = true;
            this.buttonHallMeasurementResults.Click += new System.EventHandler(this.buttonHallMeasurementResults_Click);
            // 
            // buttonHallTotalVoltage
            // 
            this.buttonHallTotalVoltage.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonHallTotalVoltage.FlatAppearance.BorderSize = 0;
            this.buttonHallTotalVoltage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHallTotalVoltage.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHallTotalVoltage.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.buttonHallTotalVoltage.Location = new System.Drawing.Point(0, 0);
            this.buttonHallTotalVoltage.Margin = new System.Windows.Forms.Padding(0);
            this.buttonHallTotalVoltage.Name = "buttonHallTotalVoltage";
            this.buttonHallTotalVoltage.Size = new System.Drawing.Size(240, 45);
            this.buttonHallTotalVoltage.TabIndex = 3;
            this.buttonHallTotalVoltage.Text = "Total Voltage";
            this.buttonHallTotalVoltage.UseVisualStyleBackColor = true;
            this.buttonHallTotalVoltage.Click += new System.EventHandler(this.buttonHallTotalVoltage_Click);
            // 
            // iconbuttonHalleffectMeasurement
            // 
            this.iconbuttonHalleffectMeasurement.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconbuttonHalleffectMeasurement.FlatAppearance.BorderSize = 0;
            this.iconbuttonHalleffectMeasurement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonHalleffectMeasurement.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconbuttonHalleffectMeasurement.ForeColor = System.Drawing.Color.Snow;
            this.iconbuttonHalleffectMeasurement.IconChar = FontAwesome.Sharp.IconChar.Magnet;
            this.iconbuttonHalleffectMeasurement.IconColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonHalleffectMeasurement.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconbuttonHalleffectMeasurement.IconSize = 30;
            this.iconbuttonHalleffectMeasurement.Location = new System.Drawing.Point(0, 450);
            this.iconbuttonHalleffectMeasurement.Margin = new System.Windows.Forms.Padding(0);
            this.iconbuttonHalleffectMeasurement.Name = "iconbuttonHalleffectMeasurement";
            this.iconbuttonHalleffectMeasurement.Size = new System.Drawing.Size(240, 60);
            this.iconbuttonHalleffectMeasurement.TabIndex = 16;
            this.iconbuttonHalleffectMeasurement.Text = "Hall effect Measurement";
            this.iconbuttonHalleffectMeasurement.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonHalleffectMeasurement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonHalleffectMeasurement.UseVisualStyleBackColor = true;
            this.iconbuttonHalleffectMeasurement.Click += new System.EventHandler(this.iconbuttonHalleffectMeasurement_Click);
            // 
            // PanelVanderPauwSubMenu
            // 
            this.PanelVanderPauwSubMenu.Controls.Add(this.buttonVdPMeasurementResults);
            this.PanelVanderPauwSubMenu.Controls.Add(this.buttonVdPTotalVoltage);
            this.PanelVanderPauwSubMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelVanderPauwSubMenu.Location = new System.Drawing.Point(0, 360);
            this.PanelVanderPauwSubMenu.Name = "PanelVanderPauwSubMenu";
            this.PanelVanderPauwSubMenu.Size = new System.Drawing.Size(240, 90);
            this.PanelVanderPauwSubMenu.TabIndex = 15;
            this.PanelVanderPauwSubMenu.Visible = false;
            // 
            // buttonVdPMeasurementResults
            // 
            this.buttonVdPMeasurementResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonVdPMeasurementResults.FlatAppearance.BorderSize = 0;
            this.buttonVdPMeasurementResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonVdPMeasurementResults.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonVdPMeasurementResults.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.buttonVdPMeasurementResults.Location = new System.Drawing.Point(0, 45);
            this.buttonVdPMeasurementResults.Margin = new System.Windows.Forms.Padding(0);
            this.buttonVdPMeasurementResults.Name = "buttonVdPMeasurementResults";
            this.buttonVdPMeasurementResults.Size = new System.Drawing.Size(240, 45);
            this.buttonVdPMeasurementResults.TabIndex = 4;
            this.buttonVdPMeasurementResults.Text = "Measurement Results";
            this.buttonVdPMeasurementResults.UseVisualStyleBackColor = true;
            this.buttonVdPMeasurementResults.Click += new System.EventHandler(this.buttonVdPMeasurementResults_Click);
            // 
            // buttonVdPTotalVoltage
            // 
            this.buttonVdPTotalVoltage.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonVdPTotalVoltage.FlatAppearance.BorderSize = 0;
            this.buttonVdPTotalVoltage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonVdPTotalVoltage.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonVdPTotalVoltage.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.buttonVdPTotalVoltage.Location = new System.Drawing.Point(0, 0);
            this.buttonVdPTotalVoltage.Margin = new System.Windows.Forms.Padding(0);
            this.buttonVdPTotalVoltage.Name = "buttonVdPTotalVoltage";
            this.buttonVdPTotalVoltage.Size = new System.Drawing.Size(240, 45);
            this.buttonVdPTotalVoltage.TabIndex = 3;
            this.buttonVdPTotalVoltage.Text = "Total Voltage";
            this.buttonVdPTotalVoltage.UseVisualStyleBackColor = true;
            this.buttonVdPTotalVoltage.Click += new System.EventHandler(this.buttonVdPTotalVoltage_Click);
            // 
            // iconbuttonVanderPauwMethod
            // 
            this.iconbuttonVanderPauwMethod.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconbuttonVanderPauwMethod.FlatAppearance.BorderSize = 0;
            this.iconbuttonVanderPauwMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonVanderPauwMethod.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconbuttonVanderPauwMethod.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonVanderPauwMethod.IconChar = FontAwesome.Sharp.IconChar.Diamond;
            this.iconbuttonVanderPauwMethod.IconColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonVanderPauwMethod.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconbuttonVanderPauwMethod.IconSize = 30;
            this.iconbuttonVanderPauwMethod.Location = new System.Drawing.Point(0, 300);
            this.iconbuttonVanderPauwMethod.Margin = new System.Windows.Forms.Padding(0);
            this.iconbuttonVanderPauwMethod.Name = "iconbuttonVanderPauwMethod";
            this.iconbuttonVanderPauwMethod.Size = new System.Drawing.Size(240, 60);
            this.iconbuttonVanderPauwMethod.TabIndex = 14;
            this.iconbuttonVanderPauwMethod.Text = "Van der Pauw Method";
            this.iconbuttonVanderPauwMethod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonVanderPauwMethod.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonVanderPauwMethod.UseVisualStyleBackColor = true;
            this.iconbuttonVanderPauwMethod.Click += new System.EventHandler(this.iconbuttonVanderPauwMethod_Click);
            // 
            // iconbuttonMeasurementSettings
            // 
            this.iconbuttonMeasurementSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.iconbuttonMeasurementSettings.FlatAppearance.BorderSize = 0;
            this.iconbuttonMeasurementSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconbuttonMeasurementSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconbuttonMeasurementSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonMeasurementSettings.IconChar = FontAwesome.Sharp.IconChar.Gears;
            this.iconbuttonMeasurementSettings.IconColor = System.Drawing.Color.WhiteSmoke;
            this.iconbuttonMeasurementSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconbuttonMeasurementSettings.IconSize = 30;
            this.iconbuttonMeasurementSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonMeasurementSettings.Location = new System.Drawing.Point(0, 240);
            this.iconbuttonMeasurementSettings.Margin = new System.Windows.Forms.Padding(0);
            this.iconbuttonMeasurementSettings.Name = "iconbuttonMeasurementSettings";
            this.iconbuttonMeasurementSettings.Size = new System.Drawing.Size(240, 60);
            this.iconbuttonMeasurementSettings.TabIndex = 2;
            this.iconbuttonMeasurementSettings.Text = "Measurement Settings";
            this.iconbuttonMeasurementSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconbuttonMeasurementSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconbuttonMeasurementSettings.UseVisualStyleBackColor = true;
            this.iconbuttonMeasurementSettings.Click += new System.EventHandler(this.iconbuttonMeasurementSettings_Click);
            // 
            // TimerCurrentDateandRealTime
            // 
            this.TimerCurrentDateandRealTime.Interval = 1000;
            this.TimerCurrentDateandRealTime.Tick += new System.EventHandler(this.timerCurrentDateandRealTime_Tick);
            // 
            // Program01Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1500, 1000);
            this.Controls.Add(this.panelDesktop);
            this.Controls.Add(this.panelTitleTabBar);
            this.Controls.Add(this.panelTabBar);
            this.Controls.Add(this.panelSideMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Program01Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program01";
            this.panelTabBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxMinimizeProgram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxExitProgram)).EndInit();
            this.panelTitleTabBar.ResumeLayout(false);
            this.panelTitleTabBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxCurrentForm)).EndInit();
            this.panelDesktop.ResumeLayout(false);
            this.panelDesktop.PerformLayout();
            this.panelHome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconpictureboxLogo)).EndInit();
            this.panelSideMenu.ResumeLayout(false);
            this.PanelHallMeasurementSubMenu.ResumeLayout(false);
            this.PanelVanderPauwSubMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelTabBar;
        private FontAwesome.Sharp.IconPictureBox iconpictureboxExitProgram;
        private FontAwesome.Sharp.IconPictureBox iconpictureboxMinimizeProgram;
        private System.Windows.Forms.Panel panelTitleTabBar;
        private FontAwesome.Sharp.IconPictureBox iconpictureboxCurrentForm;
        private System.Windows.Forms.Label labelTitleCurrentForm;
        private System.Windows.Forms.Panel panelDesktop;
        private System.Windows.Forms.Panel panelHome;
        private System.Windows.Forms.PictureBox iconpictureboxLogo;
        private FontAwesome.Sharp.IconButton iconbuttonHelp;
        private System.Windows.Forms.Panel panelSideMenu;
        private FontAwesome.Sharp.IconButton iconbuttonMeasurementSettings;
        private System.Windows.Forms.Panel PanelHallMeasurementSubMenu;
        private System.Windows.Forms.Button buttonHallMeasurementResults;
        private System.Windows.Forms.Button buttonHallTotalVoltage;
        private FontAwesome.Sharp.IconButton iconbuttonHalleffectMeasurement;
        private System.Windows.Forms.Panel PanelVanderPauwSubMenu;
        private System.Windows.Forms.Button buttonVdPMeasurementResults;
        private System.Windows.Forms.Button buttonVdPTotalVoltage;
        private FontAwesome.Sharp.IconButton iconbuttonVanderPauwMethod;
        private System.Windows.Forms.Label labelProgramTitle;
        private System.Windows.Forms.Label labelSaveVdPDataOnly;
        private System.Windows.Forms.Label labelUserFirstNameLastname;
        private System.Windows.Forms.Label labelSaveVdPandHallMeasurementData;
        private System.Windows.Forms.Label labelSaveHallMeasurementDataOnly;
        private System.Windows.Forms.Label labelUserLastname;
        private System.Windows.Forms.TextBox textboxUserLastname;
        private System.Windows.Forms.Label labelUserFirstName;
        private System.Windows.Forms.TextBox textboxUserFirstName;
        private System.Windows.Forms.TextBox textboxFileVdPandHallMeasurementDataPath;
        private System.Windows.Forms.TextBox textboxFileHallMeasurementDataPath;
        private System.Windows.Forms.TextBox textboxFileVdPDataPath;
        private FontAwesome.Sharp.IconButton iconbuttonSaveFileVdPandHallMeasurementPath;
        private FontAwesome.Sharp.IconButton iconbuttonSaveFileHallMeasurementDataPathOnly;
        private FontAwesome.Sharp.IconButton IconbuttonSaveFileVdPDataPathOnly;
        private FontAwesome.Sharp.IconButton IconbuttonBrowseFileVdPDataPathOnly;
        private FontAwesome.Sharp.IconButton iconbuttonBrowseFileVdPandHallMeasurementPath;
        private FontAwesome.Sharp.IconButton iconbuttonBrowseFileHallMeasurementDataPathOnly;
        private System.Windows.Forms.Label labelCurrentDateandRealTime;
        private System.Windows.Forms.Timer TimerCurrentDateandRealTime;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogVdPFile;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogHallMeasurementFile;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialogVdPandHallMeasurementFile;
        private System.Windows.Forms.Label labelUserLogin;
        private FontAwesome.Sharp.IconButton iconbuttonUserLogin;
    }
}

