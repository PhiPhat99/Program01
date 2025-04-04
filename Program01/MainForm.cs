using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FontAwesome.Sharp;
using OfficeOpenXml;

namespace Program01
{
    public partial class MainForm : Form
    {
        private IconButton CurrentButton;
        private Form CurrentChildForm;
        private bool IsLoggedIn = false;

        public MainForm()
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

            ToggleSubMenuVisibility(null);

            TimerCurrentDateandRealTime = new Timer
            {
                Interval = 1000
            };

            TimerCurrentDateandRealTime.Tick += TimerCurrentDateandRealTime_Tick;
            TimerCurrentDateandRealTime.Start();
            LabelCurrentDateandRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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

        private void ActivateButton(object sender, Color color)
        {
            try
            {
                if (sender is IconButton button)
                {
                    ResetButtonStyles();

                    CurrentButton = button;
                    CurrentButton.BackColor = Color.FromArgb(31, 31, 80);
                    CurrentButton.ForeColor = color;
                    CurrentButton.IconColor = color;
                    CurrentButton.TextAlign = ContentAlignment.MiddleCenter;
                    CurrentButton.TextImageRelation = TextImageRelation.TextBeforeImage;
                    CurrentButton.ImageAlign = ContentAlignment.MiddleRight;
                    IconpictureboxCurrentForm.IconChar = button.IconChar;
                    IconpictureboxCurrentForm.IconColor = color;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ResetButtonStyles()
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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                CurrentChildForm?.Hide();
                CurrentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                PanelDesktop.Controls.Add(childForm);
                PanelDesktop.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void ToggleSubMenuVisibility(Panel activeSubMenu)
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
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void UpdatePath(string primary, string sub = "")
        {
            try
            {
                string PrimaryPath = primary;
                string SubPath = sub;

                LabelTitleCurrentForm.Text = string.IsNullOrEmpty(sub)
                    ? primary
                    : $"{primary} > {sub}";
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void TimerCurrentDateandRealTime_Tick(object sender, EventArgs e)
        {
            LabelCurrentDateandRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
        }

        private void PanelTabBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void TextboxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void TextboxUserLastname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void IconbuttonUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    string FirstName = TextboxUserFirstName.Text.Trim();
                    string LastName = TextboxUserLastname.Text.Trim();

                    if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    {
                        IsLoggedIn = true;
                        LabelUserLogin.Text = $"{FirstName} {LastName}";
                        TextboxUserFirstName.Enabled = false;
                        TextboxUserLastname.Enabled = false;
                        IconbuttonUserLogin.IconChar = IconChar.SignOut;
                        IconbuttonUserLogin.IconColor = Color.Black;
                        IconbuttonUserLogin.TextImageRelation = TextImageRelation.TextBeforeImage;
                        IconbuttonUserLogin.Text = "Logout";
                        MessageBox.Show("You have been logged in successfully", "Login Successful", MessageBoxButtons.OK);

                    }
                    else
                    {
                        MessageBox.Show("Please fill in both the Firstname - Lastname before proceeding", "Invalid Username", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    IsLoggedIn = false;
                    LabelUserLogin.Text = "Guest";
                    TextboxUserFirstName.Enabled = true;
                    TextboxUserLastname.Enabled = true;
                    IconbuttonUserLogin.IconColor = RGBColors.Color6;
                    IconbuttonUserLogin.TextImageRelation = TextImageRelation.ImageBeforeText;
                    IconbuttonUserLogin.Text = "Login";
                    MessageBox.Show("You have been logged out successfully", "Logout Successful", MessageBoxButtons.OK);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconpictureboxExitProgram_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Do you want to exit the program?", "Confirm Exit", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconpictureboxMinimizeProgram_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void IconbuttonHelp_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Accessibility Failed", MessageBoxButtons.OK);
                }
                else
                {
                    ActivateButton(sender, RGBColors.Color1);
                    UpdatePath("Help");
                    OpenChildForm(new HelpForm());
                    ToggleSubMenuVisibility(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonMeasurementSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Accessibility Failed", MessageBoxButtons.OK);
                }
                else
                {
                    ActivateButton(sender, RGBColors.Color2);
                    UpdatePath("Measurement Settings");
                    OpenChildForm(new MeasurementSettingsForm());
                    ToggleSubMenuVisibility(null);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonVanderPauwMethod_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Accessibility Failed", MessageBoxButtons.OK);
                }
                else
                {
                    if (PanelVanderPauwSubMenu.Visible)
                    {
                        ToggleSubMenuVisibility(null);
                        ResetButtonStyles();
                    }
                    else
                    {
                        ActivateButton(sender, RGBColors.Color3);
                        UpdatePath("Van der Pauw Method");
                        ToggleSubMenuVisibility(PanelVanderPauwSubMenu);
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ButtonVdPTotalMeasure_Click(object sender, EventArgs e)
        {
            UpdatePath("Van der Pauw Method", "Total Measure");
            OpenChildForm(new VdPTotalMeasureValuesForm());

        }

        private void ButtonVdPMeasurementResults_Click(object sender, EventArgs e)
        {
            UpdatePath("Van der Pauw Method", "Measurement Results");
            OpenChildForm(new VdPMeasurementResultsForm());
        }

        private void IconbuttonHalleffectMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Accessibility Failed", MessageBoxButtons.OK);
                }
                else
                {
                    if (PanelHallMeasurementSubMenu.Visible)
                    {
                        ToggleSubMenuVisibility(null);
                        ResetButtonStyles();
                    }
                    else
                    {
                        ActivateButton(sender, RGBColors.Color4);
                        UpdatePath("Hall Effect Measurement");
                        ToggleSubMenuVisibility(PanelHallMeasurementSubMenu);
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ButtonHallTotalMeasure_Click(object sender, EventArgs e)
        {
            UpdatePath("Hall Effect Measurement", "Total Measure");
            OpenChildForm(new HallTotalMeasureValuesForm());
        }

        private void ButtonHallMeasurementResults_Click(object sender, EventArgs e)
        {
            UpdatePath("Hall Effect Measurement", "Measurement Results");
            OpenChildForm(new HallMeasurementResultsForm());
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void IconpictureboxLogo_Click(object sender, EventArgs e)
        {
            try
            {
                UpdatePath("Home");
                ResetDefault();
                ResetButtonStyles();
                ToggleSubMenuVisibility(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ResetDefault()
        {
            try
            {
                CurrentChildForm.Hide();
                CurrentButton.BackColor = Color.FromArgb(31, 33, 80);
                CurrentButton.ForeColor = Color.Snow;
                CurrentButton.IconColor = Color.Snow;
                IconpictureboxCurrentForm.IconChar = IconChar.HomeUser;
                IconpictureboxCurrentForm.IconColor = Color.Snow;
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonBrowseFileVdPDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Browsing Failed", MessageBoxButtons.OK);
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
                            TextboxFileVdPDataPath.Text = selectedVdPFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            TextboxFileVdPDataPath.Text = selectedVdPFilePath;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonSaveFileVdPDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Saving Failed", MessageBoxButtons.OK);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string VdPFilePath = TextboxFileVdPDataPath.Text;

                    if (string.IsNullOrWhiteSpace(VdPFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Saving Failed", MessageBoxButtons.OK);
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
                    string FirstName = TextboxUserFirstName.Text.Trim();
                    string LastName = TextboxUserLastname.Text.Trim();
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

                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Saving Successful", MessageBoxButtons.OK);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        private void IconbuttonBrowseFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Browsing Failed", MessageBoxButtons.OK);
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
                            TextboxFileHallMeasurementDataPath.Text = selectedHallFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            TextboxFileHallMeasurementDataPath.Text = selectedHallFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonSaveFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Saving Failed", MessageBoxButtons.OK);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string HallFilePath = TextboxFileHallMeasurementDataPath.Text;

                    if (string.IsNullOrWhiteSpace(HallFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Saving Failed", MessageBoxButtons.OK);
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
                    string FirstName = TextboxUserFirstName.Text.Trim();
                    string LastName = TextboxUserLastname.Text.Trim();
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
                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Saving Successful", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonBrowseFileVdPandHallMeasurementPath_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Browsing Failed", MessageBoxButtons.OK);
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
                            TextboxFileVdPandHallMeasurementDataPath.Text = selectedVdPandHallFilePath.Substring(0, 55) + "...";
                        }
                        else
                        {
                            TextboxFileVdPandHallMeasurementDataPath.Text = selectedVdPandHallFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonSaveFileVdPandHallMeasurementPath_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("Please log in before proceeding", "Saving Failed", MessageBoxButtons.OK);
                }
                else
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    string VdPandHallFilePath = TextboxFileVdPandHallMeasurementDataPath.Text;

                    if (string.IsNullOrWhiteSpace(VdPandHallFilePath))
                    {
                        MessageBox.Show("Please enter the file path first!", "Saving Failed", MessageBoxButtons.OK);
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
                    string FirstName = TextboxUserFirstName.Text.Trim();
                    string LastName = TextboxUserLastname.Text.Trim();
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

                    MessageBox.Show($"File has been created successfully at: {newFilePath}", "Saving Successful", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}