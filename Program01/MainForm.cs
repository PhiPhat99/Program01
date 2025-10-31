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
        // ***** ฟิลด์ส (Fields) ตัวแปรที่ถูกประกาศไว้ในระดับคลาส (Class) หรือโครงสร้าง (Struct) *****
        private IconButton CurrentButton;
        private Form CurrentChildForm;
        private bool IsLoggedIn = false;
        private bool IsExcelLicenseSet = false;

        // ***** คอนสทรักเตอร์ (Constructor) ส่วนที่เป็นการตั้งค่าเริ่มต้นของคลาสในฟอร์ม *****
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

        // ***** เมธอด (Method) : บล็อกหรือกลุ่มของโค้ดที่ทำงานเฉพาะอย่าง ซึ่งเป็นส่วนหนึ่งของคลาส (Class) หรือวัตถุ (Objects) *****
        // เมธอด InitializeUI() : ตั้งค่าการแสดงผลของ UI Controls ในฟอร์ม
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
            LabelProgramVersion.Text = "VERSION 0.50";
        }

        // การประกาศโครงสร้างข้อมูล (Struct) ที่ใช้ในการจัดเก็บค่าสีต่าง ๆ ที่ใช้ในฟอร์มหรือคลาส
        private readonly struct RGBColors
        {
            public static readonly Color Color1 = Color.FromArgb(172, 126, 241);
            public static readonly Color Color2 = Color.FromArgb(242, 234, 213);
            public static readonly Color Color3 = Color.FromArgb(253, 138, 114);
            public static readonly Color Color4 = Color.FromArgb(95, 77, 221);
            public static readonly Color Color5 = Color.FromArgb(249, 88, 155);
            public static readonly Color Color6 = Color.FromArgb(24, 161, 251);
        }

        // เมธอด ActivateButton() : ตอบสนองการแสดงผลการทำงานของปุ่มเมื่อทำการกดปุ่มจากผู้ใช้
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

        // เมธอด ResetButtonStyles() : คืนค่าสถานะปุ่มหากทำการกดที่ปุ่มอื่น
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

        // เมธอด OpenChildForm() : การเปิดฟอร์มต่าง ๆ เมื่อทำการกดปุ่มชื่อฟอร์มลูกที่ Sidebar Panel
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

        // เมธอด ToggleSubMenuVisibility() : แสดงผลเมนูย่อย (Sub Menu Panel) ของชื่อฟอร์มรูปแบบการวัดที่ถูกซ่อนไว้จากปุ่มหลัก
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

        // เมธอด UpdatePath() : สำหรับระบุที่อยู่ชื่อฟอร์มที่เรากำลังเปิดใช้งานอยู่
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

        // เมธอด TimerCurrentDateandRealTime_Tick() : การแสดงผลวันที่และเวลาแบบเรียล-ไทม์บน Label "LabelCurrentDateandRealTime" ของ Timer "TimerCurrentDateandRealTime"
        private void TimerCurrentDateandRealTime_Tick(object sender, EventArgs e)
        {
            LabelCurrentDateandRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
        }

        //  เมธอด PanelTabBar_MouseDown() : การลากย้ายฟอร์ม (หน้าต่างของโปรแกรม) ด้วยลากคลืกและเลื่อนไปมาที่ Panel "PanelTabBar" (แถบสีดำด้านบนของฟอร์ม)
        private void PanelTabBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // เมธอด TextboxUserName_KeyPress() : การกำหนดตัวขระที่สามารถกดพิมพ์ได้บน TextBox "TextboxUserName" ที่เป็นส่วนของชื่อผู้ใช้
        private void TextboxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        // เมธอด TextboxUserLastName_KeyPress() : การกำหนดตัวขระที่สามารถกดพิมพ์ได้บน TextBox "TextboxUserLastName" ที่เป็นส่วนของนามสกุลผู้ใช้
        private void TextboxUserLastname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '.' && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        // เมธอด IconbuttonUserLogin_Click() : ปุ่มการกดยืนการการเข้าสู่ระบบและการออกจากระบบของโปรแกรมจากผู้ใช้
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

        // เมธอด IconpictureboxExitProgram_Click() : ปุ่มการกดปิดโปรแกรม
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

        // เมธอด IconpictureboxMinimizeProgram_Click() : ปุ่มการพับหน้าจอของโปรแกรม
        private void IconpictureboxMinimizeProgram_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // เมธอด IconbuttonMeasurementSettings_Click() : ปุ่มการเปิดฟอร์มหน้าการตั้งค่าและการวัด Form "MeasurementSettingsForm"
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

        // เมธอด IconbuttonInstructions_Click() : ปุ่มการเปิดฟอร์มหน้าคำแนะนำการใช้งานโปรแกรม Form "InstructionsForm"
        private void IconbuttonInstructions_Click(object sender, EventArgs e)
        {
            try
            {
                ActivateButton(sender, RGBColors.Color6);
                UpdatePath("Help");
                OpenChildForm(new InstructionsForm());
                ToggleSubMenuVisibility(null);
            }
            catch ( Exception Ex )
            {
                MessageBox.Show($"Error: {Ex.Message}");
            }
        }

        // เมธอด IconbuttonVanderPauwMethod_Click() : ปุ่มการเลิกซ่อน/ซ่อนเมนูย่อยของการวัดแบบ Van der Pauw Method โดยภายในปุ่มหลักนี้จะมีเมนูย่อยที่ถูกซ่อนไว่อยู่ 2 ปุ่ม ได้แก่ Total Measure และ Results
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

        // เมธอด ButtonVdPTotalMeasure_Click() : ปุ่มการเปิดฟอร์มหน้าการแสดงผลข้อมูลดิบทั้งหมดจากการวัดแบบ Van der Pauw Method, Form "VdPTotalMeasureValuesForm"
        private void ButtonVdPTotalMeasure_Click(object sender, EventArgs e)
        {
            UpdatePath("Van der Pauw Method", "Total Measure");
            OpenChildForm(new VdPTotalMeasureValuesForm());
        }

        // เมธอด ButtonVdPMeasurementResults_Click() : ปุ่มการเปิดฟอร์มหน้าการแสดงผลการคำนวณค่าสมบัติทางไฟฟ้าที่ได้จากการวัดแบบ Van der Pauw Method, Form "VdPMeasurementResultsForm"
        private void ButtonVdPMeasurementResults_Click(object sender, EventArgs e)
        {
            UpdatePath("Van der Pauw Method", "Results");
            OpenChildForm(new VdPMeasurementResultsForm());
        }

        // เมธอด IconbuttonHalleffectMeasurement_Click() : ปุ่มการเลิกซ่อน/ซ่อนเมนูย่อยของการวัดแบบ Hall Effect Measurement โดยภายในปุ่มหลักนี้จะมีเมนูย่อยที่ถูกซ่อนไว่อยู่ 2 ปุ่ม ได้แก่ Total Measure และ Results
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

        // เมธอด ButtonHallTotalMeasure_Click() : ปุ่มการเปิดฟอร์มหน้าการแสดงผลข้อมูลดิบทั้งหมดจากการวัดแบบ Hall Effect Measurement, Form "HallTotalMeasureValuesForm"
        private void ButtonHallTotalMeasure_Click(object sender, EventArgs e)
        {
            UpdatePath("Hall Effect Measurement", "Total Measure");
            OpenChildForm(new HallTotalMeasureValuesForm());
        }

        // เมธอด ButtonHallMeasurementResults_Click() : ปุ่มการเปิดฟอร์มหน้าการแสดงผลการคำนวณค่าสมบัติทางไฟฟ้าที่ได้จากการวัดแบบ Hall Effect Measurement, Form "HallMeasurementResultsForm"
        private void ButtonHallMeasurementResults_Click(object sender, EventArgs e)
        {
            UpdatePath("Hall Effect Measurement", "Results");
            OpenChildForm(new HallMeasurementResultsForm());
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        // เมธอด ReleaseCapture() : ทำหน้าที่ในการปล่อยการจับเมาส์ออกจาก Panel "PanelTabBar"
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        // เมธอด SendMessage() : ทำหน้าที่ในการส่งข้อความไปยังหน้าต่างฟอร์มที่ระบุ
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        // เมธอด IconpictureboxLogo_Click() : ปุ่มการเปิดฟอร์มหน้าเริ่มต้นโปรแกรม (หน้าหลัก) และทำการซ่อนฟอร์มก่อนหน้าที่ทำการเปิดอยู่ โดยจะเปิดหน้า Form "MainForm"
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

        // เมธอด ResetDefault() : การคืนค่าเริ่มต้นของปุ่มฟอร์มที่ทำการเปิด และทำการซ่อนฟอร์มที่ถูกเปิดอยู่ก่อนหน้า หากทำการเรียกใช้เมธอดนี้ (โดยจากโปรแกรม เมธอดนี้จะถูกเรียกจากการกดปุ่มเปิดหน้าหลักเท่านั้น)
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

        // เมธอด IconbuttonBrowseFileVdPDataPathOnly_Click() : การกดเปิดเลือกโฟลเดอร์ที่ใช้ในการบันทึกเฉพาะผลการวัดแบบแวน เดอ เพาว์
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

        // เมธอด IconbuttonSaveFileVdPDataPathOnly_Click() : การกดบันทึกและสร้างไฟล์ Excel เฉพาะผลการวัดแบบแวน เดอ เพาว์
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

                if (File.Exists(newFilePath))
                {
                    try
                    {
                        File.Delete(newFilePath);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"ไม่สามารถลบไฟล์เก่าได้ กรุณาปิดไฟล์ Excel '{newFileName}' หากเปิดอยู่แล้วลองอีกครั้ง. Error: {ex.Message}", "ข้อผิดพลาดในการบันทึก", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string FirstName = TextboxUserFirstName.Text.Trim();
                string LastName = TextboxUserLastname.Text.Trim();
                string FullName = $"{FirstName}   {LastName}";

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

                string source = GlobalSettings.Instance.SourceModeUI;
                string sourceUnit = GlobalSettings.Instance.SourceModeUI == "Voltage" ? "V" : "A";
                string measure = GlobalSettings.Instance.MeasureModeUI;
                string measureUnit = GlobalSettings.Instance.MeasureModeUI == "Voltage" ? "V" : "A";

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("VdPDataSheet");
                    Action<ExcelRange> SetAlignCenterStyle = (range) =>
                    {
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    };
                    Action<ExcelRange> SetHeaderFont = (range) =>
                    {
                        range.Style.Font.Name = "Arial";
                        range.Style.Font.Size = 12;
                        range.Style.Font.Bold = true;
                        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    };
                    Action<ExcelRange, System.Drawing.Color> SetBackgroundColor = (range, color) =>
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(color);
                    };
                    Action<ExcelRange> SetAllBorders = (range) =>
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Color.SetColor(Color.Black);
                        range.Style.Border.Bottom.Color.SetColor(Color.Black);
                        range.Style.Border.Left.Color.SetColor(Color.Black);
                        range.Style.Border.Right.Color.SetColor(Color.Black);
                    };

                    int currentRow = 1;
                    int currentColumn = 1;
                    int rawDataEndColumn = (allVdPMeasurements.Count * 2);

                    worksheet.Cells[currentRow, currentColumn, currentRow, rawDataEndColumn].Merge = true;
                    worksheet.Cells[currentRow, currentColumn].Value = "Vander Pauw Raw Measurement Data";
                    SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn]);
                    SetHeaderFont(worksheet.Cells[currentRow, currentColumn]);
                    SetBackgroundColor(worksheet.Cells[currentRow, currentColumn], Color.DarkGreen);
                    worksheet.Cells[currentRow, currentColumn].Style.Font.Size = 14;
                    currentRow++;

                    foreach (var tunerData in allVdPMeasurements.OrderBy(k => k.Key))
                    {
                        var positionCell = worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2];
                        var mergedPositionRange = worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2, currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1];

                        positionCell.Value = tunerData.Key;
                        mergedPositionRange.Merge = true;
                        SetAlignCenterStyle(positionCell);
                        SetHeaderFont(positionCell);
                        SetBackgroundColor(positionCell, Color.SeaGreen);
                        SetAllBorders(mergedPositionRange);
                    }
                    currentRow++;

                    foreach (var tunerData in allVdPMeasurements.OrderBy(k => k.Key))
                    {
                        var sourceHeaderCell = worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2];
                        var readingHeaderCell = worksheet.Cells[currentRow, currentColumn + (tunerData.Key - 1) * 2 + 1];

                        sourceHeaderCell.Value = $"{source} ({sourceUnit})";
                        readingHeaderCell.Value = $"{measure} ({measureUnit})";

                        SetAlignCenterStyle(sourceHeaderCell);
                        SetHeaderFont(sourceHeaderCell);
                        SetBackgroundColor(sourceHeaderCell, Color.LightGreen);
                        SetAllBorders(sourceHeaderCell);

                        SetAlignCenterStyle(readingHeaderCell);
                        SetHeaderFont(readingHeaderCell);
                        SetBackgroundColor(readingHeaderCell, Color.LightGreen);
                        SetAllBorders(readingHeaderCell);
                    }
                    currentRow++;

                    int maxMeasurements = 0;
                    foreach (var tunerData in allVdPMeasurements.Values)
                    {
                        if (tunerData.Count > maxMeasurements)
                        {
                            maxMeasurements = tunerData.Count;
                        }
                    }

                    for (int i = 0; i < maxMeasurements; i++)
                    {
                        foreach (var tunerDataEntry in allVdPMeasurements.OrderBy(k => k.Key))
                        {
                            var sourceCell = worksheet.Cells[currentRow, currentColumn + (tunerDataEntry.Key - 1) * 2];
                            var readingCell = worksheet.Cells[currentRow, currentColumn + (tunerDataEntry.Key - 1) * 2 + 1];

                            if (tunerDataEntry.Value.Count > i)
                            {
                                sourceCell.Value = tunerDataEntry.Value[i].Source;
                                readingCell.Value = tunerDataEntry.Value[i].Reading;
                            }
                            else
                            {
                                sourceCell.Value = (object)"";
                                readingCell.Value = (object)"";
                            }

                            SetAlignCenterStyle(sourceCell);
                            SetAlignCenterStyle(readingCell);
                            SetAllBorders(sourceCell);
                            SetAllBorders(readingCell);
                            sourceCell.Style.Numberformat.Format = "0.000000000";
                            readingCell.Style.Numberformat.Format = "0.000";
                        }
                        currentRow++;
                    }

                    int calculatedDataStartColumn = rawDataEndColumn + 3;
                    int calculatedDataStartRow = 1;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Calculated Van der Pauw Properties";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1].Merge = true;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetHeaderFont(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetBackgroundColor(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn], Color.DarkBlue);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Style.Font.Size = 14;
                    calculatedDataStartRow++;

                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Parameter";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = "Value";
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetHeaderFont(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetHeaderFont(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetBackgroundColor(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn], Color.Blue);
                    SetBackgroundColor(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1], Color.Blue);
                    SetAllBorders(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAllBorders(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    calculatedDataStartRow++;

                    var rangeResA = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistance A (Ω)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistanceA;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeResA);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeResB = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistance B (Ω)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistanceB;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeResB);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeAvgRes = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Average Resistance (Ω)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = averageResistanceAll;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeAvgRes);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeSheetRes = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Sheet Resistance (Ω/Sqr)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = sheetResistance;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeSheetRes);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeThickness = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Thickness (cm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = thickness;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeThickness);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeResistivity = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Resistivity (Ω⋅cm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = resistivity;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeResistivity);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    var rangeConductivity = worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn, calculatedDataStartRow, calculatedDataStartColumn + 1];
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn].Value = "Conductivity (S/cm)";
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Value = conductivity;
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1]);
                    SetAllBorders(rangeConductivity);
                    worksheet.Cells[calculatedDataStartRow, calculatedDataStartColumn + 1].Style.Numberformat.Format = "0.000000";
                    calculatedDataStartRow++;

                    int userInfoStartColumn = calculatedDataStartColumn;
                    int userInfoStartRow = calculatedDataStartRow + 2;

                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Value = $"Username";
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1].Value = FullName;
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Style.Font.Bold = true;
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1].Style.Font.Bold = true;
                    SetAlignCenterStyle(worksheet.Cells[userInfoStartRow, userInfoStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1]);
                    SetAllBorders(worksheet.Cells[userInfoStartRow, userInfoStartColumn, userInfoStartRow, userInfoStartColumn + 1]);
                    userInfoStartRow++;

                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Value = $"Date and Time";
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cells[userInfoStartRow, userInfoStartColumn].Style.Font.Bold = true;
                    SetAlignCenterStyle(worksheet.Cells[userInfoStartRow, userInfoStartColumn]);
                    SetAlignCenterStyle(worksheet.Cells[userInfoStartRow, userInfoStartColumn + 1]);
                    SetAllBorders(worksheet.Cells[userInfoStartRow, userInfoStartColumn, userInfoStartRow, userInfoStartColumn + 1]);

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    package.SaveAs(new FileInfo(newFilePath));
                }

                MessageBox.Show($"ไฟล์ได้ถูกสร้างขึ้น และถูกจัดเก็บไว้ที่: {newFilePath}", "การบันทึกเสร็จสิ้น", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // เมธอด IconbuttonBrowseFileHallMeasurementDataPathOnly_Click() : การกดเปิดเลือกโฟลเดอร์ที่ใช้ในการบันทึกเฉพาะผลการวัดแบบปรากฏการณ์ฮอลล์
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

        // เมธอด IconbuttonSaveFileHallMeasurementDataPathOnly_Click() : การกดบันทึกและสร้างไฟล์ Excel เฉพาะผลการวัดแบบปรากฏการณ์ฮอลล์
        private void IconbuttonSaveFileHallMeasurementDataPathOnly_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsLoggedIn)
                {
                    MessageBox.Show("กรุณาเข้าสู่ระบบก่อนดำเนินการต่อ", "การบันทึกล้มเหลว", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    //ExcelPackage.License.SetNonCommercialOrganization("KMITL");
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
                    if (File.Exists(newFilePath))
                    {
                        try
                        {
                            File.Delete(newFilePath);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show($"ไม่สามารถลบไฟล์เก่าได้ กรุณาปิดไฟล์ Excel '{newFileName}' หากปิดอยู่แล้วลองอีกครั้ง. Error: {ex.Message}", "ข้อผิดพลาดในการบันทึก", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string FirstName = TextboxUserFirstName.Text.Trim();
                    string LastName = TextboxUserLastname.Text.Trim();
                    string UserFullName = $"{FirstName}   {LastName}";
                    var hallCalculator = CollectAndCalculateHallMeasured.Instance;
                    var allRawHallMeasurements = hallCalculator.AllRawMeasurements;
                    var detailedPerCurrentPointResults = hallCalculator.DetailedPerCurrentPointResults;

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ResultsDataSheet");
                        Action<ExcelRange> SetAlignCenterStyle = (range) =>
                        {
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        };
                        Action<ExcelRange> SetHeaderFont = (range) =>
                        {
                            range.Style.Font.Name = "Arial";
                            range.Style.Font.Size = 12;
                            range.Style.Font.Bold = true;
                            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        };
                        Action<ExcelRange, System.Drawing.Color> SetBackgroundColor = (range, color) =>
                        {
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(color);
                        };
                        Action<ExcelRange> SetAllBorders = (range) =>
                        {
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Color.SetColor(Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(Color.Black);
                            range.Style.Border.Left.Color.SetColor(Color.Black);
                            range.Style.Border.Right.Color.SetColor(Color.Black);
                        };

                        int currentRow = 1;
                        int currentColumn = 1;

                        worksheet.Cells[currentRow, currentColumn].Value = "Username (Firstname - Lastname)";
                        worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + 4].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn]);
                        SetHeaderFont(worksheet.Cells[currentRow, currentColumn]);
                        SetBackgroundColor(worksheet.Cells[currentRow, currentColumn], Color.DarkGray);
                        SetAllBorders(worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + 4]);

                        worksheet.Cells[currentRow, currentColumn + 5].Value = "Date and Time";
                        worksheet.Cells[currentRow, currentColumn + 5, currentRow, currentColumn + 7].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn + 5]);
                        SetHeaderFont(worksheet.Cells[currentRow, currentColumn + 5]);
                        SetBackgroundColor(worksheet.Cells[currentRow, currentColumn + 5], Color.DarkGray);
                        SetAllBorders(worksheet.Cells[currentRow, currentColumn + 5, currentRow, currentColumn + 7]);
                        currentRow++;

                        worksheet.Cells[currentRow, currentColumn].Value = UserFullName;
                        worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + 4].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn]);
                        worksheet.Cells[currentRow, currentColumn].Style.Font.Size = 14;
                        worksheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;
                        SetAllBorders(worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + 4]);


                        worksheet.Cells[currentRow, currentColumn + 5].Value = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
                        worksheet.Cells[currentRow, currentColumn + 5, currentRow, currentColumn + 7].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentColumn + 5]);
                        //worksheet.Cells[currentRow, currentColumn + 5].Style.Font.Bold = true;
                        SetAllBorders(worksheet.Cells[currentRow, currentColumn + 5, currentRow, currentColumn + 7]);
                        currentRow += 2;

                        int rawDataStartCol = 1;
                        int rawDataTitleColSpan = 1 + 2 * (allRawHallMeasurements.ContainsKey(HallMeasurementState.NoMagneticField) ? allRawHallMeasurements[HallMeasurementState.NoMagneticField].Count : 0) + 1 + 2 * (allRawHallMeasurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField) ? allRawHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField].Count : 0) + 1 + 2 * (allRawHallMeasurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField) ? allRawHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField].Count : 0) + 5;

                        worksheet.Cells[currentRow, rawDataStartCol].Value = "Hall Effect Raw Measurement Data";
                        worksheet.Cells[currentRow, rawDataStartCol, currentRow, rawDataStartCol + 14].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, rawDataStartCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, rawDataStartCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, rawDataStartCol], Color.DarkGreen);
                        worksheet.Cells[currentRow, rawDataStartCol].Style.Font.Size = 14;
                        currentRow++;

                        int currentRawDataCol = rawDataStartCol;
                        worksheet.Cells[currentRow, currentRawDataCol].Value = "No Magnetic Field (V_out)";
                        worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol], Color.Green);
                        SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4]);
                        currentRawDataCol += 5;

                        worksheet.Cells[currentRow, currentRawDataCol].Value = "South Magnetic Field (V_south)";
                        worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol], Color.Red);
                        SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4]);
                        currentRawDataCol += 5;

                        worksheet.Cells[currentRow, currentRawDataCol].Value = "North Magnetic Field (V_north)";
                        worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol], Color.Blue);
                        SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol, currentRow, currentRawDataCol + 4]);
                        currentRawDataCol += 5;
                        currentRow++;

                        currentRawDataCol = rawDataStartCol;
                        for (int i = 0; i < 3; i++)
                        {
                            worksheet.Cells[currentRow, currentRawDataCol].Value = "Current, I (A)";
                            SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol]);
                            SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol]);
                            SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol], Color.Gray);
                            SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol]);

                            worksheet.Cells[currentRow, currentRawDataCol + 1].Value = "V_1,+I (V)";
                            SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol + 1]);
                            SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol + 1]);
                            SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol + 1], Color.Gray);
                            SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol + 1]);

                            worksheet.Cells[currentRow, currentRawDataCol + 2].Value = "V_2,-I (V)";
                            SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol + 2]);
                            SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol + 2]);
                            SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol + 2], Color.Gray);
                            SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol + 2]);

                            worksheet.Cells[currentRow, currentRawDataCol + 3].Value = "V_3,+I (V)";
                            SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol + 3]);
                            SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol + 3]);
                            SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol + 3], Color.Gray);
                            SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol + 3]);

                            worksheet.Cells[currentRow, currentRawDataCol + 4].Value = "V_4,-I (V)";
                            SetAlignCenterStyle(worksheet.Cells[currentRow, currentRawDataCol + 4]);
                            SetHeaderFont(worksheet.Cells[currentRow, currentRawDataCol + 4]);
                            SetBackgroundColor(worksheet.Cells[currentRow, currentRawDataCol + 4], Color.Gray);
                            SetAllBorders(worksheet.Cells[currentRow, currentRawDataCol + 4]);

                            currentRawDataCol += 5;
                        }
                        currentRow++;

                        int maxRawDataRows = 0;
                        foreach (var stateEntry in allRawHallMeasurements)
                        {
                            foreach (var tunerEntry in stateEntry.Value)
                            {
                                if (tunerEntry.Value.Count > maxRawDataRows)
                                {
                                    maxRawDataRows = tunerEntry.Value.Count;
                                }
                            }
                        }

                        for (int i = 0; i < maxRawDataRows; i++)
                        {
                            currentRawDataCol = rawDataStartCol;
                            if (allRawHallMeasurements.ContainsKey(HallMeasurementState.NoMagneticField))
                            {
                                var noFieldData = allRawHallMeasurements[HallMeasurementState.NoMagneticField];
                                if (noFieldData.Any() && noFieldData[1].Count > i)
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = noFieldData[1][i].Item1;
                                }
                                else
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                }
                                currentRawDataCol++;

                                for (int tunerPos = 1; tunerPos <= 4; tunerPos++)
                                {
                                    if (noFieldData.ContainsKey(tunerPos) && noFieldData[tunerPos].Count > i)
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = noFieldData[tunerPos][i].Item2;
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                    }
                                    currentRawDataCol++;
                                }
                            }
                            else
                            {
                                currentRawDataCol += 5;
                            }

                            if (allRawHallMeasurements.ContainsKey(HallMeasurementState.OutwardOrSouthMagneticField))
                            {
                                var southFieldData = allRawHallMeasurements[HallMeasurementState.OutwardOrSouthMagneticField];
                                if (southFieldData.Any() && southFieldData[1].Count > i)
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = southFieldData[1][i].Item1;
                                }
                                else
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                }
                                currentRawDataCol++;

                                for (int tunerPos = 1; tunerPos <= 4; tunerPos++)
                                {
                                    if (southFieldData.ContainsKey(tunerPos) && southFieldData[tunerPos].Count > i)
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = southFieldData[tunerPos][i].Item2;
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                    }
                                    currentRawDataCol++;
                                }
                            }
                            else
                            {
                                currentRawDataCol += 5;
                            }

                            if (allRawHallMeasurements.ContainsKey(HallMeasurementState.InwardOrNorthMagneticField))
                            {
                                var northFieldData = allRawHallMeasurements[HallMeasurementState.InwardOrNorthMagneticField];
                                if (northFieldData.Any() && northFieldData[1].Count > i)
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = northFieldData[1][i].Item1;
                                }
                                else
                                {
                                    worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                }
                                currentRawDataCol++;

                                for (int tunerPos = 1; tunerPos <= 4; tunerPos++)
                                {
                                    if (northFieldData.ContainsKey(tunerPos) && northFieldData[tunerPos].Count > i)
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = northFieldData[tunerPos][i].Item2;
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, currentRawDataCol].Value = "";
                                    }
                                    currentRawDataCol++;
                                }
                            }
                            else
                            {
                                currentRawDataCol += 5;
                            }

                            for (int col = rawDataStartCol; col < currentRawDataCol; col++)
                            {
                                SetAlignCenterStyle(worksheet.Cells[currentRow, col]);
                                SetAllBorders(worksheet.Cells[currentRow, col]);
                                worksheet.Cells[currentRow, col].Style.Numberformat.Format = "0.000E+00";
                            }
                            currentRow++;
                        }
                        currentRow++;

                        int calculatedDataStartCol = 1;
                        worksheet.Cells[currentRow, calculatedDataStartCol].Value = "Calculated Hall Properties (Per Current Point)";
                        worksheet.Cells[currentRow, calculatedDataStartCol, currentRow, calculatedDataStartCol + 8].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, calculatedDataStartCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, calculatedDataStartCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, calculatedDataStartCol], Color.DarkBlue);
                        worksheet.Cells[currentRow, calculatedDataStartCol].Style.Font.Size = 14;
                        currentRow++;

                        worksheet.Cells[currentRow, calculatedDataStartCol].Value = "Current (A)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 1].Value = "V_HS (V)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 2].Value = "V_HN (V)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 3].Value = "V_H,Average (V)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 4].Value = "R_Hall (Ω)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 5].Value = "R_H (cm³/C)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 6].Value = "n_Bulk (cm⁻³)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 7].Value = "n_Sheet (cm⁻²)";
                        worksheet.Cells[currentRow, calculatedDataStartCol + 8].Value = "Mobility (cm²/V⋅s)";

                        for (int col = calculatedDataStartCol; col <= calculatedDataStartCol + 8; col++)
                        {
                            SetAlignCenterStyle(worksheet.Cells[currentRow, col]);
                            SetHeaderFont(worksheet.Cells[currentRow, col]);
                            SetBackgroundColor(worksheet.Cells[currentRow, col], Color.LightBlue);
                            SetAllBorders(worksheet.Cells[currentRow, col]);
                        }
                        currentRow++;

                        foreach (System.Collections.Generic.KeyValuePair<double, HallCalculationResultPerCurrent> currentPointEntry in detailedPerCurrentPointResults.OrderBy(keySelector => keySelector.Key))
                        {
                            var currentResult = currentPointEntry.Value;
                            worksheet.Cells[currentRow, calculatedDataStartCol].Value = currentResult.Current;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 1].Value = currentResult.Vh_SouthField;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 2].Value = currentResult.Vh_NorthField;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 3].Value = currentResult.Vh_Average;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 4].Value = currentResult.R_Hall_ByCurrent;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 5].Value = currentResult.R_H_ByCurrent;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 6].Value = currentResult.BulkConcentration_ByCurrent;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 7].Value = currentResult.SheetConcentration_ByCurrent;
                            worksheet.Cells[currentRow, calculatedDataStartCol + 8].Value = currentResult.Mobility_ByCurrent;

                            for (int col = calculatedDataStartCol; col <= calculatedDataStartCol + 8; col++)
                            {
                                SetAlignCenterStyle(worksheet.Cells[currentRow, col]);
                                SetAllBorders(worksheet.Cells[currentRow, col]);
                                worksheet.Cells[currentRow, col].Style.Numberformat.Format = "0.000E+00";
                            }
                            currentRow++;
                        }
                        currentRow++;

                        currentRow = 5;
                        int overallCalculatedDataStartCol = 19;
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Overall Hall Properties";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1].Merge = true;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol]);
                        SetHeaderFont(worksheet.Cells[currentRow, overallCalculatedDataStartCol]);
                        SetBackgroundColor(worksheet.Cells[currentRow, overallCalculatedDataStartCol], Color.DarkMagenta);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Style.Font.Size = 14;
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Parameter";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = "Value";
                        for (int col = overallCalculatedDataStartCol; col <= overallCalculatedDataStartCol + 1; col++)
                        {
                            SetAlignCenterStyle(worksheet.Cells[currentRow, col]);
                            SetHeaderFont(worksheet.Cells[currentRow, col]);
                            SetBackgroundColor(worksheet.Cells[currentRow, col], Color.LightCoral);
                            SetAllBorders(worksheet.Cells[currentRow, col]);
                        }
                        currentRow++;


                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Hall Voltage (V)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.TotalHallVoltage_Average;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000E+00";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Hall Resistance (Ω)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.HallResistance;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000E+00";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Thickness (cm)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.ThicknessValueStd;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000000";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Magnetic Fields (V⋅s/cm²)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.MagneticFieldsValueStd;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000000";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Hall Coefficient (cm³/C)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.HallCoefficient;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Bulk Concentration (cm⁻³)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.BulkConcentration;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000E+00";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Sheet Concentration (cm⁻²)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.SheetConcentration;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000E+00";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Mobility (cm²/V⋅s)";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.Mobility;
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Style.Numberformat.Format = "0.000";
                        currentRow++;

                        worksheet.Cells[currentRow, overallCalculatedDataStartCol].Value = "Semiconductor Type";
                        worksheet.Cells[currentRow, overallCalculatedDataStartCol + 1].Value = GlobalSettings.Instance.SemiconductorType.ToString();
                        SetAlignCenterStyle(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        SetAllBorders(worksheet.Cells[currentRow, overallCalculatedDataStartCol, currentRow, overallCalculatedDataStartCol + 1]);
                        currentRow++;

                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
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

        // เมธอด IconbuttonBrowseFileVdPandHallMeasurementPath_Click() : การกดเปิดเลือกโฟลเดอร์ที่ใช้ในการบันทึกผลการคำนวณค่าสมบัติทางไฟฟ้าจากการวัดแบบแวน เดอ เพาว์และแบบปรากฏการณ์ฮอลล์
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

        // เมธอด IconbuttonSaveFileVdPandHallMeasurementPath_Click() : การกดบันทึกและสร้างไฟล์ Excel สำหรับผลการคำนวณค่าสมบัติทางไฟฟ้าของการวัดแบบแวน เดอ เพาว์ และแบบปรากฏการณ์ฮอลล์
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
                    //ExcelPackage.License.SetNonCommercialOrganization("KMITL");
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
                    string UserFullName = $"{FirstName}  {LastName}";

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Measurement Results");

                        var TitleCell = worksheet.Cells[1, 1];
                        TitleCell.Value = "Van der Pauw Method & Hall Effect Measurement : Results";
                        worksheet.Cells[1, 1, 2, 10].Merge = true;
                        TitleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        TitleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        TitleCell.Style.Font.Size = 16;
                        TitleCell.Style.Font.Bold = true;
                        TitleCell.Style.Font.Color.SetColor(Color.Snow);
                        TitleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        TitleCell.Style.Fill.BackgroundColor.SetColor(Color.Navy);

                        var VanDerPauwHeader = worksheet.Cells[3, 1];
                        VanDerPauwHeader.Value = "Van der Pauw Method";
                        worksheet.Cells[3, 1, 3, 10].Merge = true;
                        VanDerPauwHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        VanDerPauwHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        VanDerPauwHeader.Style.Font.Size = 14;
                        VanDerPauwHeader.Style.Font.Bold = true;
                        VanDerPauwHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        VanDerPauwHeader.Style.Fill.BackgroundColor.SetColor(Color.Green);
                        
                        var ResA = worksheet.Cells[4, 1];
                        ResA.Value = "R_A (Ω)";
                        worksheet.Cells[4, 1, 4, 2].Merge = true;
                        ResA.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ResA.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ResA.Style.Font.Size = 12;
                        ResA.Style.Font.Bold = true;
                        ResA.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ResA.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var ResAResults = worksheet.Cells[4, 3];
                        ResAResults.Value = GlobalSettings.Instance.ResistanceA;
                        worksheet.Cells[4, 3, 4, 5].Merge = true;
                        ResAResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ResAResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ResAResults.Style.Font.Size = 12;
                        ResAResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ResAResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var ResB = worksheet.Cells[4, 6];
                        ResB.Value = "R_B (Ω)";
                        worksheet.Cells[4, 6, 4, 7].Merge = true;
                        ResB.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ResB.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ResB.Style.Font.Size = 12;
                        ResB.Style.Font.Bold = true;
                        ResB.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ResB.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var ResBResults = worksheet.Cells[4, 8];
                        ResBResults.Value = GlobalSettings.Instance.ResistanceB;
                        worksheet.Cells[4, 8, 4, 10].Merge = true;
                        ResBResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ResBResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ResBResults.Style.Font.Size = 12;
                        ResBResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ResBResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var AvgRes = worksheet.Cells[5, 1];
                        AvgRes.Value = "R_Average (Ω)";
                        worksheet.Cells[5, 1, 5, 2].Merge = true;
                        AvgRes.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        AvgRes.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        AvgRes.Style.Font.Size = 12;
                        AvgRes.Style.Font.Bold = true;
                        AvgRes.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        AvgRes.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var AvgResResults = worksheet.Cells[5, 3];
                        AvgResResults.Value = GlobalSettings.Instance.AverageResistanceAll;
                        worksheet.Cells[5, 3, 5, 5].Merge = true;
                        AvgResResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        AvgResResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        AvgResResults.Style.Font.Size = 12;
                        AvgResResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        AvgResResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var SheetRes = worksheet.Cells[5, 6];
                        SheetRes.Value = "R_Sheet (Ω/Sqr)";
                        worksheet.Cells[5, 6, 5, 7].Merge = true;
                        SheetRes.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        SheetRes.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SheetRes.Style.Font.Size = 12;
                        SheetRes.Style.Font.Bold = true;
                        SheetRes.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        SheetRes.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var SheetResResults = worksheet.Cells[5, 8];
                        SheetResResults.Value = GlobalSettings.Instance.SheetResistance;
                        worksheet.Cells[5, 8, 5, 10].Merge = true;
                        SheetResResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        SheetResResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SheetResResults.Style.Font.Size = 12;
                        SheetResResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        SheetResResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var Rho = worksheet.Cells[6, 1];
                        Rho.Value = "ρ (Ω⋅cm)";
                        worksheet.Cells[6, 1, 6, 2].Merge = true;
                        Rho.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Rho.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        Rho.Style.Font.Size = 12;
                        Rho.Style.Font.Bold = true;
                        Rho.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Rho.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var RhoResults = worksheet.Cells[6, 3];
                        RhoResults.Value = GlobalSettings.Instance.Resistivity;
                        worksheet.Cells[6, 3, 6, 5].Merge = true;
                        RhoResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        RhoResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        RhoResults.Style.Font.Size = 12;
                        RhoResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        RhoResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var Sigma = worksheet.Cells[6, 6];
                        Sigma.Value = "σ (S/cm)";
                        worksheet.Cells[6, 6, 6, 7].Merge = true;
                        Sigma.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Sigma.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        Sigma.Style.Font.Size = 12;
                        Sigma.Style.Font.Bold = true;
                        Sigma.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Sigma.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var SigmaResults = worksheet.Cells[6, 8];
                        SigmaResults.Value = GlobalSettings.Instance.Conductivity;
                        worksheet.Cells[6, 8, 6, 10].Merge = true;
                        SigmaResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        SigmaResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SigmaResults.Style.Font.Size = 12;
                        SigmaResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        SigmaResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var HallMeasurementHeader = worksheet.Cells[7, 1];
                        HallMeasurementHeader.Value = "Hall Effect Measurement";
                        worksheet.Cells[7, 1, 7, 10].Merge = true;
                        HallMeasurementHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        HallMeasurementHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        HallMeasurementHeader.Style.Font.Size = 14;
                        HallMeasurementHeader.Style.Font.Bold = true;
                        HallMeasurementHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        HallMeasurementHeader.Style.Fill.BackgroundColor.SetColor(Color.Gold);

                        var HallVoltage = worksheet.Cells[8, 1];
                        HallVoltage.Value = "V_H (V)";
                        worksheet.Cells[8, 1, 8, 2].Merge = true;
                        HallVoltage.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        HallVoltage.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        HallVoltage.Style.Font.Size = 12;
                        HallVoltage.Style.Font.Bold = true;
                        HallVoltage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        HallVoltage.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var HallVoltageResults = worksheet.Cells[8, 3];
                        worksheet.Cells[8, 3, 8, 5].Merge = true;
                        HallVoltageResults.Value = GlobalSettings.Instance.TotalHallVoltage_Average;
                        HallVoltageResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        HallVoltageResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        HallVoltageResults.Style.Font.Size = 12;
                        HallVoltageResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        HallVoltageResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var RH = worksheet.Cells[8, 6];
                        RH.Value = "R_H (cm^3/C)";
                        worksheet.Cells[8, 6, 8, 7].Merge = true;
                        RH.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        RH.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        RH.Style.Font.Size = 12;
                        RH.Style.Font.Bold = true;
                        RH.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        RH.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var RHResults = worksheet.Cells[8, 8];
                        RHResults.Value = GlobalSettings.Instance.HallCoefficient;
                        worksheet.Cells[8, 8, 8, 10].Merge = true;
                        RHResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        RHResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        RHResults.Style.Font.Size = 12;
                        RHResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        RHResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var nBulk = worksheet.Cells[9, 1];
                        nBulk.Value = "n_Bulk (cm^-3)";
                        worksheet.Cells[9, 1, 9, 2].Merge = true;
                        nBulk.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        nBulk.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        nBulk.Style.Font.Size = 12;
                        nBulk.Style.Font.Bold = true;
                        nBulk.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        nBulk.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var nBulkResults = worksheet.Cells[9, 3];
                        nBulkResults.Value = GlobalSettings.Instance.BulkConcentration;
                        worksheet.Cells[9, 3, 9, 5].Merge = true;
                        nBulkResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        nBulkResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        nBulkResults.Style.Font.Size = 12;
                        nBulkResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        nBulkResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var nSheet = worksheet.Cells[9, 6];
                        nSheet.Value = "n_Sheet (cm^-2)";
                        worksheet.Cells[9, 6, 9, 7].Merge = true;
                        nSheet.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        nSheet.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        nSheet.Style.Font.Size = 12;
                        nSheet.Style.Font.Bold = true;
                        nSheet.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        nSheet.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var nSheetResults = worksheet.Cells[9, 8];
                        nSheetResults.Value = GlobalSettings.Instance.SheetConcentration;
                        worksheet.Cells[9, 8, 9, 10].Merge = true;
                        nSheetResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        nSheetResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        nSheetResults.Style.Font.Size = 12;
                        nSheetResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        nSheetResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var Mobility = worksheet.Cells[10, 1];
                        Mobility.Value = "µ (cm^2/V⋅s)";
                        worksheet.Cells[10, 1, 10, 2].Merge = true;
                        Mobility.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Mobility.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        Mobility.Style.Font.Size = 12;
                        Mobility.Style.Font.Bold = true;
                        Mobility.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Mobility.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var MobilityResults = worksheet.Cells[10, 3];
                        MobilityResults.Value = GlobalSettings.Instance.Mobility;
                        worksheet.Cells[10, 3, 10, 5].Merge = true;
                        MobilityResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        MobilityResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        MobilityResults.Style.Font.Size = 12;
                        MobilityResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        MobilityResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var SemiconductorType = worksheet.Cells[10, 6];
                        SemiconductorType.Value = "TYPE";
                        worksheet.Cells[10, 6, 10, 7].Merge = true;
                        SemiconductorType.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        SemiconductorType.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SemiconductorType.Style.Font.Size = 12;
                        SemiconductorType.Style.Font.Bold = true;
                        SemiconductorType.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        SemiconductorType.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var SemiconductorTypeResults = worksheet.Cells[10, 8];
                        SemiconductorTypeResults.Value = GlobalSettings.Instance.SemiconductorType;
                        worksheet.Cells[10, 8, 10, 10].Merge = true;
                        SemiconductorTypeResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        SemiconductorTypeResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SemiconductorTypeResults.Style.Font.Size = 12;
                        SemiconductorTypeResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        SemiconductorTypeResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);

                        var ConstantValueHeader = worksheet.Cells[3, 12];
                        ConstantValueHeader.Value = "Constant Values";
                        worksheet.Cells[3, 12, 3, 15].Merge = true;
                        ConstantValueHeader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ConstantValueHeader.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ConstantValueHeader.Style.Font.Size = 14;
                        ConstantValueHeader.Style.Font.Bold = true;
                        ConstantValueHeader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ConstantValueHeader.Style.Fill.BackgroundColor.SetColor(Color.Violet);

                        var Thickness = worksheet.Cells[4, 12];
                        Thickness.Value = "t (cm)";
                        worksheet.Cells[4, 12, 4, 13].Merge = true;
                        Thickness.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Thickness.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        Thickness.Style.Font.Size = 12;
                        Thickness.Style.Font.Bold = true;
                        Thickness.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Thickness.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var ThicknessValue = worksheet.Cells[4, 14];
                        ThicknessValue.Value = GlobalSettings.Instance.ThicknessValueStd;
                        worksheet.Cells[4, 14, 4, 15].Merge = true;
                        ThicknessValue.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ThicknessValue.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ThicknessValue.Style.Font.Size = 12;
                        ThicknessValue.Style.Font.Bold = true;
                        ThicknessValue.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ThicknessValue.Style.Fill.BackgroundColor.SetColor(Color.Salmon);

                        var MagneticFields = worksheet.Cells[5, 12];
                        MagneticFields.Value = "B (V⋅s/cm^2)";
                        worksheet.Cells[5, 12, 5, 13].Merge = true;
                        MagneticFields.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        MagneticFields.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        MagneticFields.Style.Font.Size = 12;
                        MagneticFields.Style.Font.Bold = true;
                        MagneticFields.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        MagneticFields.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

                        var MagneticFieldsValue = worksheet.Cells[5, 14];
                        MagneticFieldsValue.Value = GlobalSettings.Instance.MagneticFieldsValueStd;
                        worksheet.Cells[5, 14, 5, 15].Merge = true;
                        MagneticFieldsValue.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        MagneticFieldsValue.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        MagneticFieldsValue.Style.Font.Size = 12;
                        MagneticFieldsValue.Style.Font.Bold = true;
                        MagneticFieldsValue.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        MagneticFieldsValue.Style.Fill.BackgroundColor.SetColor(Color.Lavender);

                        /*worksheet.Cells[7, 6].Value = "R_Hall (Ω)";
                        worksheet.Cells[7, 6, 7, 7].Merge = true;
                        worksheet.Cells[7, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[7, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        var RHallResults = worksheet.Cells[8, 8];
                        RHallResults.Value = GlobalSettings.Instance.HallResistance;
                        worksheet.Cells[8, 8, 8, 10].Merge = true;
                        RHallResults.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        RHallResults.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        RHallResults.Style.Font.Size = 12;
                        RHallResults.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        RHallResults.Style.Fill.BackgroundColor.SetColor(Color.Snow);*/

                        var Username = worksheet.Cells[14, 1];
                        Username.Value = "Username (Fistname - LastName)";
                        worksheet.Cells[14, 1, 14, 4].Merge = true;
                        Username.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Username.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        Username.Style.Font.Size = 12;
                        Username.Style.Font.Bold = true;
                        Username.Style.Font.Color.SetColor(Color.Snow);
                        Username.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        Username.Style.Fill.BackgroundColor.SetColor(Color.Black);

                        var FullName = worksheet.Cells[14, 5];
                        FullName.Value = UserFullName;
                        worksheet.Cells[14, 5, 14, 10].Merge = true;
                        FullName.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        FullName.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        FullName.Style.Font.Size = 12;
                        FullName.Style.Font.Bold = true;
                        FullName.Style.Font.Color.SetColor(Color.AntiqueWhite);
                        FullName.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        FullName.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

                        var DateNTime = worksheet.Cells[15, 1];
                        DateNTime.Value = "Date and Time";
                        worksheet.Cells[15, 1, 15, 4].Merge = true;
                        DateNTime.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        DateNTime.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        DateNTime.Style.Font.Size = 12;
                        DateNTime.Style.Font.Bold = true;
                        DateNTime.Style.Font.Color.SetColor(Color.Snow);
                        DateNTime.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        DateNTime.Style.Fill.BackgroundColor.SetColor(Color.Black);

                        var DateTimeValue = worksheet.Cells[15, 5];
                        DateTimeValue.Value = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");
                        worksheet.Cells[15, 5, 15, 10].Merge = true;
                        DateTimeValue.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        DateTimeValue.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        DateTimeValue.Style.Font.Size = 12;
                        DateTimeValue.Style.Font.Bold = false;
                        DateTimeValue.Style.Font.Color.SetColor(Color.AntiqueWhite);
                        DateTimeValue.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        DateTimeValue.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

                        package.SaveAs(new FileInfo(newFilePath));
                    }

                    MessageBox.Show($"ไฟล์ได้ถูกสร้างขึ้น และถูกจัดเก็บไว้ที่ : {newFilePath}", "การบันทึกเสร็จสิ้น", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}