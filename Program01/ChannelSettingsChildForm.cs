using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Program01
{
    public partial class ChannelSettingsChildForm : Form
    {
        //Fields
        private MeasurementSettingsForm MeasurementsettingsForm;

        public ChannelSettingsChildForm()
        {
        }

        public ChannelSettingsChildForm(MeasurementSettingsForm MeasurementsettingsForm)
        {
            InitializeComponent();
            this.MeasurementsettingsForm = MeasurementsettingsForm;

            this.MeasurementsettingsForm += MeasurementSettingsForm_ModeChanged;
        }
    }

    private void MeasurementSettingsForm_ModeChanged(object sender, bool isHallMode)
    {

    }

    public void PanelToggleSwitchBase_MouseClick(object sender, MouseEventArgs e)
    {
        try
        {
            isModes = !isModes;
            UpdateToggleState();

            if (isModes == false)
            {

                PictureboxTuner1.Image = global::Program01.Properties.Resources.R_A1_VdP;
                PictureboxTuner2.Image = global::Program01.Properties.Resources.R_A2_VdP;
                PictureboxTuner3.Image = global::Program01.Properties.Resources.R_A3_VdP;
                PictureboxTuner4.Image = global::Program01.Properties.Resources.R_A4_VdP;
                PictureboxTuner5.Image = global::Program01.Properties.Resources.R_B1_VdP;
                PictureboxTuner6.Image = global::Program01.Properties.Resources.R_B2_VdP;
                PictureboxTuner7.Image = global::Program01.Properties.Resources.R_B3_VdP;
                PictureboxTuner8.Image = global::Program01.Properties.Resources.R_B4_VdP;
            }

            else if (isModes == true)
            {
                string Modes = "Hall effect";
                TextboxMagneticFields.Enabled = true;
                TextboxMagneticFields.Visible = true;
                LabelMagneticFields.Visible = true;
                LabelMagneticFieldsUnit.Visible = true;
                LabelToggleSwitchVdP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
                LabelToggleSwitchHall.ForeColor = Color.FromArgb(144, 198, 101);
                PanelToggleSwitchButton.BackColor = Color.FromArgb(95, 77, 221);
                ComboboxMagneticFieldsUnit.Visible = true;
                Debug.WriteLine($"You select: {Modes} measurement");

                PictureboxTuner1.Image = global::Program01.Properties.Resources.V_1_Hall;
                PictureboxTuner2.Image = global::Program01.Properties.Resources.V_2_Hall;
                PictureboxTuner3.Image = global::Program01.Properties.Resources.V_3_Hall;
                PictureboxTuner4.Image = global::Program01.Properties.Resources.V_4_Hall;
                PictureboxTuner5.Image = global::Program01.Properties.Resources.V_5_Hall;
                PictureboxTuner6.Image = global::Program01.Properties.Resources.V_6_Hall;
                PictureboxTuner7.Image = global::Program01.Properties.Resources.V_7_Hall;
                PictureboxTuner8.Image = global::Program01.Properties.Resources.V_8_Hall;
            }

            OnToggleChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}
