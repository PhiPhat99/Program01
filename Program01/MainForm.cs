using FontAwesome.Sharp;
using OfficeOpenXml;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Program01
{
    public partial class MainForm : Form
    {
        private IconButton CurrentButton;
        private Form CurrentChildForm;
        private bool IsLoggedIn = false;
        private bool IsExcelLicenseSet = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();

            if (!IsExcelLicenseSet)
            {
                ExcelPackage.License.SetNonCommercialOrganization("KMITL");
                IsExcelLicenseSet = true;
            }
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
                        MessageBox.Show("คุณได้เข้าสู่ระบบเรียบร้อยแล้ว", "เข้าสู่ระบบสำเร็จ", MessageBoxButtons.OK);

                    }
                    else
                    {
                        MessageBox.Show("กรุณากรอกชื่อ - นามสกุลก่อนดำเนินการใดๆ ต่อ", "ชื่อผู้ใช้งานไม่ถูกต้อง", MessageBoxButtons.OK);
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
                    MessageBox.Show("คุณได้ออกจากระบบเรียบร้อยแล้ว", "ออกจากระบบสำเร็จ", MessageBoxButtons.OK);

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
                DialogResult result = MessageBox.Show("คุณต้องการออกจากโปรแกรมหรือไม่?", "ยืนยันการออกจากโปรแกรม", MessageBoxButtons.YesNo);

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

        private void IconbuttonMeasurementSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การเข้าถึงล้มเหลว", MessageBoxButtons.OK);
                }
                else
                {
                    ActivateButton(sender, RGBColors.Color2);
                    UpdatePath("Measurement And Settings");
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การเข้าถึงล้มเหลว", MessageBoxButtons.OK);
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
            UpdatePath("Van der Pauw Method", "Results");
            OpenChildForm(new VdPMeasurementResultsForm());
        }

        private void IconbuttonHalleffectMeasurement_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การเข้าถึงล้มเหลว", MessageBoxButtons.OK);
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
            UpdatePath("Hall Effect Measurement", "Results");
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การสืบค้นล้มเหลว", MessageBoxButtons.OK);
                }
                else
                {
                    FolderBrowserDialogVdPFile.Description = "กรุณาเลือกโฟลเดอร์สำหรับการบันทึกไฟล์";
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                    return;
                }

                string vdpFilePath = TextboxFileVdPDataPath.Text;

                if (string.IsNullOrWhiteSpace(vdpFilePath))
                {
                    MessageBox.Show("กรุณาเลือกที่อยู่ในการบันทึกไฟล์ก่อน!", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                    return;
                }

                if (!Directory.Exists(vdpFilePath))
                {
                    Directory.CreateDirectory(vdpFilePath);
                    MessageBox.Show($"ไดเรกทอรีได้ถูกสร้างไว้แล้ว: {vdpFilePath}", "การแจ้งเตือนข้อมูล");
                }

                string newFileName = "VanderPauwResultsData.xlsx";
                string newFilePath = Path.Combine(vdpFilePath, newFileName);
                string firstName = TextboxUserFirstName.Text.Trim();
                string lastName = TextboxUserLastname.Text.Trim();
                string userFullName = $"{firstName} {lastName}";

                var vdpCalculator = CollectAndCalculateVdPMeasured.Instance;
                var allVdPMeasurements = vdpCalculator.GetAllMeasurementsByTuner();
                var resistancesByPosition = GlobalSettings.Instance.ResistancesByPosition;
                double resistanceA = GlobalSettings.Instance.ResistanceA;
                double resistanceB = GlobalSettings.Instance.ResistanceB;
                double averageResistanceAll = GlobalSettings.Instance.AverageResistanceAll;
                double sheetResistance = GlobalSettings.Instance.SheetResistance;
                double resistivity = GlobalSettings.Instance.Resistivity;
                double conductivity = GlobalSettings.Instance.Conductivity;
                double thickness = GlobalSettings.Instance.ThicknessValueStd;

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("VdPDataSheet");

                    // Helper method สำหรับการกำหนด Style การจัดแนวกึ่งกลาง
                    Action<ExcelRange> SetAlignCenterStyle = (range) =>
                    {
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    };

                    int currentRow = 1;
                    int currentColumn = 1;

                    // **Start: Writing Raw VdP Measurement Data (ค่า Source/Reading ในแถวเดียว)**
                    worksheet.Cells[currentRow, currentColumn].Value = "Vander Pauw Raw Measurement Data";
                    worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + (allVdPMeasurements.Count * 2) - 1].Merge = true;
                    SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn]);
                    currentRow++;

                    worksheet.Cells[currentRow, currentColumn].Value = "ตำแหน่งที่ทำการวัด";
                    foreach (var tunerData in allVdPMeasurements.OrderBy(k => k.Key))
                    {
                        worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1].Value = tunerData.Key;
                        worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1, currentRow, currentColumn + (tunerData.Key - 1) * 2 + 2].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1]);
                    }
                    currentRow++;

                    // เขียน Header "Source" และ "Reading" ในแถวเดียวกัน
                    foreach (var tunerData in allVdPMeasurements.OrderBy(k => k.Key))
                    {
                        worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1].Value = "Source";
                        worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 2].Value = "Reading";
                    }
                    currentRow++;

                    // เขียนค่า Source และ Reading ในแถวเดียวกัน
                    foreach (var tunerData in allVdPMeasurements.OrderBy(k => k.Key))
                    {
                        // สมมติว่ามีข้อมูล Source และ Reading เพียงชุดเดียวต่อตำแหน่งเพื่อให้แสดงในแถวเดียว
                        if (tunerData.Value.Count > 0)
                        {
                            worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1].Value = tunerData.Value.First().Source;
                            worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 2].Value = tunerData.Value.First().Reading;
                        }
                    }
                    currentRow++;
                    // **End: Writing Raw VdP Measurement Data**

                    // **Start: Writing Calculated Van der Pauw Properties (ด้านขวา)**
                    int calculatedDataStartColumn = currentColumn + (allVdPMeasurements.Count * 2) + 2;
                    int calculatedDataStartRow = 1;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Calculated Van der Pauw Properties";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1].Merge = true;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    calculatedDataStartRow++;
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Parameter";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = "Value";
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistance A (Ohm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistanceA;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistance B (Ohm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistanceB;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Average Resistance (All) (Ohm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = averageResistanceAll;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Sheet Resistance (Ohm/sq)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = sheetResistance;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistivity (Ohm-m)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistivity;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Conductivity (S/m)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = conductivity;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Thickness (m)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = thickness;
                    calculatedDataStartRow++;
                    // **End: Writing Calculated Van der Pauw Properties**

                    // **Start: Writing User Information and Time (ด้านล่างขวา)**
                    int userInfoStartColumn = calculatedDataStartColumn;
                    int userInfoStartRow = calculatedDataStartRow + 2;

                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Value = $"User:";
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1].Value = userFullName;
                    userInfoStartRow++;
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Value = $"Date and Time:";
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    // **End: Writing User Information and Time**

                    package.SaveAs(new FileInfo(newFilePath));
                }

                MessageBox.Show($"ไฟล์ได้ถูกสร้างขึ้น และถูกจัดเก็บไว้ที่: {newFilePath}", "การบันทึกเสร็จสิ้น", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void IconbuttonBrowseFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การสืบค้นล้มเหลว", MessageBoxButtons.OK);
                }
                else 
                {
                    FolderBrowserDialogHallMeasurementFile.Description = "กรุณาเลือกโฟลเดอร์สำหรับการบันทึกไฟล์";
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                }
                else
                {
                    ExcelPackage.License.SetNonCommercialOrganization("KMITL");
                    string HallFilePath = TextboxFileHallMeasurementDataPath.Text;

                    if (string.IsNullOrWhiteSpace(HallFilePath))
                    {
                        MessageBox.Show("กรุณาเลือกที่อยู่ในการบันทึกไฟล์ก่อน!", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                        return;
                    }

                    string directory = HallFilePath;

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        MessageBox.Show($"ไดเรกทอรีได้ถูกสร้างไว้แล้ว: {directory}", "การแจ้งเตือนข้อมูล");
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

                        worksheet.Cells[1, 11].Value = "Total Current Source & Hall Out Magnetic Field Voltage Measured";
                        worksheet.Cells[1, 11, 2, 20].Merge = true;
                        worksheet.Cells[1, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 11].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 11].Value = "I_s (A)";
                        worksheet.Cells[3, 11, 3, 12].Merge = true;
                        worksheet.Cells[3, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 11].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 13].Value = "V_ho1 (V)";
                        worksheet.Cells[3, 13, 3, 14].Merge = true;
                        worksheet.Cells[3, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 13].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 15].Value = "V_ho2 (V)";
                        worksheet.Cells[3, 15, 3, 16].Merge = true;
                        worksheet.Cells[3, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 15].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 17].Value = "V_ho3 (V)";
                        worksheet.Cells[3, 17, 3, 18].Merge = true;
                        worksheet.Cells[3, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 17].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 19].Value = "V_ho4 (V)";
                        worksheet.Cells[3, 19, 3, 20].Merge = true;
                        worksheet.Cells[3, 19].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 19].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[1, 21].Value = "Total Current Source & Hall In Magnetic Field (South) Voltage Measured";
                        worksheet.Cells[1, 21, 2, 30].Merge = true;
                        worksheet.Cells[1, 21].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 21].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 21].Value = "I_s (A)";
                        worksheet.Cells[3, 21, 3, 22].Merge = true;
                        worksheet.Cells[3, 21].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 21].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 23].Value = "V_ho(S)1 (V)";
                        worksheet.Cells[3, 23, 3, 24].Merge = true;
                        worksheet.Cells[3, 23].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 23].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 25].Value = "V_ho(S)2 (V)";
                        worksheet.Cells[3, 25, 3, 26].Merge = true;
                        worksheet.Cells[3, 25].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 25].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 27].Value = "V_ho(S)3 (V)";
                        worksheet.Cells[3, 27, 3, 28].Merge = true;
                        worksheet.Cells[3, 27].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 27].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 29].Value = "V_ho(S)4 (V)";
                        worksheet.Cells[3, 29, 3, 30].Merge = true;
                        worksheet.Cells[3, 29].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 29].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[1, 31].Value = "Total Current Source & Hall In Magnetic Field (North) Voltage Measured";
                        worksheet.Cells[1, 31, 2, 40].Merge = true;
                        worksheet.Cells[1, 31].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 31].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 31].Value = "I_s (A)";
                        worksheet.Cells[3, 31, 3, 32].Merge = true;
                        worksheet.Cells[3, 31].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 31].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 33].Value = "V_ho(N)1 (V)";
                        worksheet.Cells[3, 33, 3, 34].Merge = true;
                        worksheet.Cells[3, 33].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 33].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 35].Value = "V_ho(N)2 (V)";
                        worksheet.Cells[3, 35, 3, 36].Merge = true;
                        worksheet.Cells[3, 35].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 35].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 37].Value = "V_ho(N)3 (V)";
                        worksheet.Cells[3, 37, 3, 38].Merge = true;
                        worksheet.Cells[3, 37].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 37].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[3, 39].Value = "V_ho(N)4 (V)";
                        worksheet.Cells[3, 39, 3, 40].Merge = true;
                        worksheet.Cells[3, 39].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, 39].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        package.SaveAs(new FileInfo(newFilePath));
                    }
                    MessageBox.Show($"ไฟล์ได้ถูกสร้างขึ้น และถูกจัดเก็บไว้ที่: {newFilePath}", "การบันทึกเสร็จสิ้น", MessageBoxButtons.OK);
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การสืบค้นล้มเหลว", MessageBoxButtons.OK);
                }
                else
                {
                    FolderBrowserDialogVdPandHallMeasurementFile.Description = "กรุณาเลือกโฟลเดอร์สำหรับการบันทึกไฟล์";
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
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                }
                else
                {
                    ExcelPackage.License.SetNonCommercialOrganization("KMITL");
                    string VdPandHallFilePath = TextboxFileVdPandHallMeasurementDataPath.Text;

                    if (string.IsNullOrWhiteSpace(VdPandHallFilePath))
                    {
                        MessageBox.Show("กรุณาเลือกที่อยู่ในการบันทึกไฟล์ก่อน!", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                        return;
                    }

                    string directory = VdPandHallFilePath;

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        MessageBox.Show($"ไดเรกทอรีได้ถูกสร้างไว้แล้ว: {directory}", "การแจ้งเตือนข้อมูล");
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

                    MessageBox.Show($"ไฟล์ได้ถูกสร้างขึ้น และถูกจัดเก็บไว้ที่: {newFilePath}", "การบันทึกเสร็จสิ้น", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}