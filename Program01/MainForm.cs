using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using OfficeOpenXml;

namespace Program01
{
    public partial class Program01Form : Form
    {
        private IconButton CurrentButton;
        private Form CurrentChildForm;
        private bool isLoggedIn = false;

        public Program01Form()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

            toggleSubMenuVisibility(null);

            TimerCurrentDateandRealTime = new Timer();
            TimerCurrentDateandRealTime.Interval = 1000;
            TimerCurrentDateandRealTime.Tick += timerCurrentDateandRealTime_Tick;
            TimerCurrentDateandRealTime.Start();
            labelCurrentDateandRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private readonly struct RGBColors
        {
            public static readonly Color Color1 = Color.FromArgb(172, 126, 241);
            public static readonly Color Color2 = Color.FromArgb(242, 234, 213);
            public static readonly Color Color3 = Color.FromArgb(253, 138, 114);
            public static readonly Color Color4 = Color.FromArgb(95, 77, 221);
            public static readonly Color Color5 = Color.FromArgb(249, 88, 155);
            public static readonly Color Color6 = Color.FromArgb(24, 161, 251);
        }

        private void activateButton(object sender, Color color)
        {
            try
            {
                if (sender is IconButton button)
                {
                    resetButtonStyles();

                    CurrentButton = button;
                    CurrentButton.BackColor = Color.FromArgb(31, 31, 80);
                    CurrentButton.ForeColor = color;
                    CurrentButton.IconColor = color;
                    CurrentButton.TextAlign = ContentAlignment.MiddleCenter;
                    CurrentButton.TextImageRelation = TextImageRelation.TextBeforeImage;
                    CurrentButton.ImageAlign = ContentAlignment.MiddleRight;

                    iconpictureboxCurrentForm.IconChar = button.IconChar;
                    iconpictureboxCurrentForm.IconColor = color;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void resetButtonStyles()
        {
            try
            {
                if (CurrentButton != null)
                {
                    CurrentButton.BackColor = Color.FromArgb(31, 33, 80);
                    CurrentButton.ForeColor = Color.Snow;
                    CurrentButton.IconColor = Color.Snow;
                    CurrentButton.TextAlign = ContentAlignment.MiddleLeft;
                    CurrentButton.TextImageRelation = TextImageRelation.ImageBeforeText;
                    CurrentButton.ImageAlign = ContentAlignment.MiddleLeft;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void openChildForm(Form childForm)
        {
            try
            {
                CurrentChildForm?.Close();
                CurrentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                panelDesktop.Controls.Add(childForm);
                panelDesktop.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void toggleSubMenuVisibility(Panel activeSubMenu)
        {
            try
            {
                PanelVanderPauwSubMenu.Visible = false;
                PanelHallMeasurementSubMenu.Visible = false;
                if (activeSubMenu != null)
                {
                    activeSubMenu.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void updatePath(string primary, string sub = "")
        {
            try
            {
                string PrimaryPath = primary;
                string SubPath = sub;

                labelTitleCurrentForm.Text = string.IsNullOrEmpty(sub)
                    ? primary
                    : $"{primary} > {sub}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void timerCurrentDateandRealTime_Tick(object sender, EventArgs e)
        {
            labelCurrentDateandRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
        }

        private void panelTabBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void textboxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textboxUserLastname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void iconbuttonUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    string FirstName = textboxUserFirstName.Text.Trim();
                    string LastName = textboxUserLastname.Text.Trim();
                    if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    {
                        isLoggedIn = true;
                        labelUserLogin.Text = $"{FirstName} {LastName}";
                        textboxUserFirstName.Enabled = false;
                        textboxUserLastname.Enabled = false;
                        iconbuttonUserLogin.IconChar = IconChar.User;
                        iconbuttonUserLogin.IconColor = Color.Black;
                        iconbuttonUserLogin.TextImageRelation = TextImageRelation.TextBeforeImage;
                        iconbuttonUserLogin.Text = "Logout";
                        MessageBox.Show("You have been logged in successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("Please fill in both the Firstname - Lastname before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    isLoggedIn = false;
                    labelUserLogin.Text = "Guest";
                    textboxUserFirstName.Enabled = true;
                    textboxUserLastname.Enabled = true;
                    iconbuttonUserLogin.IconColor = RGBColors.Color6;
                    iconbuttonUserLogin.TextImageRelation = TextImageRelation.ImageBeforeText;
                    iconbuttonUserLogin.Text = "Login";
                    MessageBox.Show("You have been logged out successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconpictureboxExitProgram_Click(object sender, EventArgs e) => Application.Exit();

        private void iconpictureboxMinimizeProgram_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void iconbuttonHelp_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    activateButton(sender, RGBColors.Color1);
                    updatePath("Help");
                    openChildForm(new HelpChildForm());
                    toggleSubMenuVisibility(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonMeasurementSettings_Click(object sender, EventArgs e)
        {
            try
            {
                /*if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else*/
                //{
                    activateButton(sender, RGBColors.Color2);
                    updatePath("Measurement Settings");
                    openChildForm(new MeasurementSettingsChildForm());
                    toggleSubMenuVisibility(null);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonVanderPauwMethod_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (PanelVanderPauwSubMenu.Visible)
                    {
                        toggleSubMenuVisibility(null);
                        resetButtonStyles();
                    }
                    else
                    {
                        activateButton(sender, RGBColors.Color3);
                        updatePath("Van der Pauw Method");
                        toggleSubMenuVisibility(PanelVanderPauwSubMenu);
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void buttonVdPTotalVoltage_Click(object sender, EventArgs e)
        {
            updatePath("Van der Pauw Method", "Total Voltage");
            openChildForm(new VdPTotalVoltageChildForm());
        }

        private void buttonVdPMeasurementResults_Click(object sender, EventArgs e)
        {
            updatePath("Van der Pauw Method", "Measurement Results");
            openChildForm(new VdPMeasurementResultsChildForm());
        }

        private void iconbuttonHalleffectMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (PanelHallMeasurementSubMenu.Visible)
                    {
                        toggleSubMenuVisibility(null);
                        resetButtonStyles();
                    }
                    else
                    {
                        activateButton(sender, RGBColors.Color4);
                        updatePath("Hall Effect Measurement");
                        toggleSubMenuVisibility(PanelHallMeasurementSubMenu);
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void buttonHallTotalVoltage_Click(object sender, EventArgs e)
        {
            updatePath("Hall Effect Measurement", "Total Voltage");
            openChildForm(new HallEffectMeasurementResultsChildForm());
        }

        private void buttonHallMeasurementResults_Click(object sender, EventArgs e)
        {
            updatePath("Hall Effect Measurement", "Measurement Results");
            openChildForm(new HallEffectMeasurementResultsChildForm());
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void iconpictureboxLogo_Click(object sender, EventArgs e)
        {
            try
            {
                updatePath("Home");
                resetDefault();
                resetButtonStyles();
                toggleSubMenuVisibility(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void resetDefault()
        {
            try
            {
                CurrentChildForm.Close();

                CurrentButton.BackColor = Color.FromArgb(31, 33, 80);
                CurrentButton.ForeColor = Color.Snow;
                CurrentButton.IconColor = Color.Snow;

                iconpictureboxCurrentForm.IconChar = IconChar.HomeUser;
                iconpictureboxCurrentForm.IconColor = Color.Snow;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonBrowseFileVdPDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    FolderBrowserDialogVdPFile.Description = "Please select the folder for saving the file";
                    FolderBrowserDialogVdPFile.ShowNewFolderButton = true;

                    if (FolderBrowserDialogVdPFile.ShowDialog() == DialogResult.OK)
                    {
                        string selectedVdPFilePath = FolderBrowserDialogVdPFile.SelectedPath;
                        if (selectedVdPFilePath.Length > 60)
                        {
                            textboxFileVdPDataPath.Text = selectedVdPFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            textboxFileVdPDataPath.Text = selectedVdPFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonSaveFileVdPDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string VdPFilePath = textboxFileVdPDataPath.Text;
                    if (string.IsNullOrWhiteSpace(VdPFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string directory = VdPFilePath;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        MessageBox.Show($"Directory created: {directory}", "Info");
                    }
                    string newFileName = "VanderPauwResultsData.xlsx";
                    string newFilePath = Path.Combine(directory, newFileName);
                    string FirstName = textboxUserFirstName.Text.Trim();
                    string LastName = textboxUserLastname.Text.Trim();
                    string UserFullName = $"{FirstName} {LastName}";
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ResultsDataSheet");
                        worksheet.Cells[1, 1].Value = "Username (Firstname - Lastname)";
                        worksheet.Cells[1, 1, 1, 5].Merge = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, 6].Value = "Date and Time";
                        worksheet.Cells[1, 6, 1, 8].Merge = true;
                        worksheet.Cells[1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 1].Value = UserFullName;
                        worksheet.Cells[2, 1, 2, 5].Merge = true;
                        worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 6].Value = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
                        worksheet.Cells[2, 6, 2, 8].Merge = true;
                        worksheet.Cells[2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        package.SaveAs(new FileInfo(newFilePath));
                    }
                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonBrowseFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else 
                {
                    FolderBrowserDialogHallMeasurementFile.Description = "Please select the folder for saving the file";
                    FolderBrowserDialogHallMeasurementFile.ShowNewFolderButton = true;

                    if (FolderBrowserDialogHallMeasurementFile.ShowDialog() == DialogResult.OK)
                    {
                        string selectedHallFilePath = FolderBrowserDialogHallMeasurementFile.SelectedPath;
                        if (selectedHallFilePath.Length > 60)
                        {
                            textboxFileHallMeasurementDataPath.Text = selectedHallFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            textboxFileHallMeasurementDataPath.Text = selectedHallFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonSaveFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string HallFilePath = textboxFileHallMeasurementDataPath.Text;
                    if (string.IsNullOrWhiteSpace(HallFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string directory = HallFilePath;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        MessageBox.Show($"Directory created: {directory}", "Info");
                    }
                    string newFileName = "HallMeasurementResultsData.xlsx";
                    string newFilePath = Path.Combine(directory, newFileName);
                    string FirstName = textboxUserFirstName.Text.Trim();
                    string LastName = textboxUserLastname.Text.Trim();
                    string UserFullName = $"{FirstName} {LastName}";
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ResultsDataSheet");
                        worksheet.Cells[1, 1].Value = "Username (Firstname - Lastname)";
                        worksheet.Cells[1, 1, 1, 5].Merge = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, 6].Value = "Date and Time";
                        worksheet.Cells[1, 6, 1, 8].Merge = true;
                        worksheet.Cells[1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 1].Value = UserFullName;
                        worksheet.Cells[2, 1, 2, 5].Merge = true;
                        worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 6].Value = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
                        worksheet.Cells[2, 6, 2, 8].Merge = true;
                        worksheet.Cells[2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        package.SaveAs(new FileInfo(newFilePath));
                    }
                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonBrowseFileVdPandHallMeasurementPath_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    FolderBrowserDialogVdPandHallMeasurementFile.Description = "Please select the folder for saving the file";
                    FolderBrowserDialogVdPandHallMeasurementFile.ShowNewFolderButton = true;

                    if (FolderBrowserDialogVdPandHallMeasurementFile.ShowDialog() == DialogResult.OK)
                    {
                        string selectedVdPandHallFilePath = FolderBrowserDialogVdPandHallMeasurementFile.SelectedPath;
                        if (selectedVdPandHallFilePath.Length > 60)
                        {
                            textboxFileVdPandHallMeasurementDataPath.Text = selectedVdPandHallFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            textboxFileVdPandHallMeasurementDataPath.Text = selectedVdPandHallFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void iconbuttonSaveFileVdPandHallMeasurementPath_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string VdPandHallFilePath = textboxFileVdPandHallMeasurementDataPath.Text;
                    if (string.IsNullOrWhiteSpace(VdPandHallFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string directory = VdPandHallFilePath;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        MessageBox.Show($"Directory created: {directory}", "Info");
                    }
                    string newFileName = "VanderPauwandHallMeasurementResultsData.xlsx";
                    string newFilePath = Path.Combine(directory, newFileName);
                    string FirstName = textboxUserFirstName.Text.Trim();
                    string LastName = textboxUserLastname.Text.Trim();
                    string UserFullName = $"{FirstName} {LastName}";
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ResultsDataSheet");
                        worksheet.Cells[1, 1].Value = "Username (Firstname - Lastname)";
                        worksheet.Cells[1, 1, 1, 5].Merge = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, 6].Value = "Date and Time";
                        worksheet.Cells[1, 6, 1, 8].Merge = true;
                        worksheet.Cells[1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 1].Value = UserFullName;
                        worksheet.Cells[2, 1, 2, 5].Merge = true;
                        worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[2, 6].Value = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
                        worksheet.Cells[2, 6, 2, 8].Merge = true;
                        worksheet.Cells[2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        package.SaveAs(new FileInfo(newFilePath));
                    }
                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}