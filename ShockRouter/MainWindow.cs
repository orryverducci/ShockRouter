using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShockRouter
{
    public partial class MainWindow : Form
    {
        // Private fields
        Router router;

        public MainWindow()
        {
            // Set UI font to system font
            this.Font = SystemFonts.MessageBoxFont;
            // Initialise UI
            InitializeComponent();
            // Increase font size on source buttons
            studioButton.Font = new Font(studioButton.Font.FontFamily, 14, FontStyle.Regular);
            chartButton.Font = new Font(chartButton.Font.FontFamily, 14, FontStyle.Regular);
            obButton.Font = new Font(obButton.Font.FontFamily, 14, FontStyle.Regular);
            emergencyButton.Font = new Font(emergencyButton.Font.FontFamily, 14, FontStyle.Regular);
            // Create router
            router = new Router(this.Handle);
            // Handle router source changed event
            router.SourceChanged += SourceChanged;
            // Set to studio
            router.Source = Router.Sources.STUDIO;
            // Add audio devices to list
            studioInputComboBox.Items.Add("Default Input");
            foreach (string device in router.GetDevices())
            {
                studioInputComboBox.Items.Add(device);
            }
            // Handle audio events to update level metres
            router.SourceLevelMeterUpdate += SourceLevelMeterUpdate;
            router.OutputLevelMeterUpdate += OutputLevelMeterUpdate;
            // Add DSPs to list
            processorComboBox.Items.Add("None");
            foreach (string processor in router.GetDSPs())
            {
                processorComboBox.Items.Add(processor);
            }
            // Load user settings
            studioInputComboBox.SelectedIndex = Properties.Settings.Default.AudioInput;
            chartUrlTextBox.Text = Properties.Settings.Default.ChartShowURL;
            obUrlTextBox.Text = Properties.Settings.Default.ObURL;
            fileLabel.Text = Properties.Settings.Default.EmergencyFile;
            detectorUpDown.Value = Properties.Settings.Default.DetectorTime;
            processorComboBox.SelectedIndex = Properties.Settings.Default.Processor;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Update settings with current state
            Properties.Settings.Default.AudioInput = studioInputComboBox.SelectedIndex;
            Properties.Settings.Default.ChartShowURL = chartUrlTextBox.Text;
            Properties.Settings.Default.ObURL = obUrlTextBox.Text;
            Properties.Settings.Default.EmergencyFile = fileLabel.Text;
            Properties.Settings.Default.DetectorTime = (int)detectorUpDown.Value;
            Properties.Settings.Default.Processor = processorComboBox.SelectedIndex;
            // Save settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Updates UI to show the new source
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void SourceChanged(object sender, EventArgs e)
        {
            if (router.Source == Router.Sources.STUDIO) // If source is studio
            {
                // Make studio button bold
                studioButton.Font = new Font(studioButton.Font, FontStyle.Bold);
                chartButton.Font = new Font(chartButton.Font, FontStyle.Regular);
                obButton.Font = new Font(obButton.Font, FontStyle.Regular);
                emergencyButton.Font = new Font(emergencyButton.Font, FontStyle.Regular);
            }
            else if (router.Source == Router.Sources.SRA) // If source is chart show
            {
                // Make chart show button bold
                studioButton.Font = new Font(studioButton.Font, FontStyle.Regular);
                chartButton.Font = new Font(chartButton.Font, FontStyle.Bold);
                obButton.Font = new Font(obButton.Font, FontStyle.Regular);
                emergencyButton.Font = new Font(emergencyButton.Font, FontStyle.Regular);
            }
            else if (router.Source == Router.Sources.OB) // If source is an outside broadcast
            {
                // Make outside broadcast button bold
                studioButton.Font = new Font(studioButton.Font, FontStyle.Regular);
                chartButton.Font = new Font(chartButton.Font, FontStyle.Regular);
                obButton.Font = new Font(obButton.Font, FontStyle.Bold);
                emergencyButton.Font = new Font(emergencyButton.Font, FontStyle.Regular);
            }
            else if (router.Source == Router.Sources.EMERGENCY) // If source is the emergency output file
            {
                // Make emergency output button bold
                studioButton.Font = new Font(studioButton.Font, FontStyle.Regular);
                chartButton.Font = new Font(chartButton.Font, FontStyle.Regular);
                obButton.Font = new Font(obButton.Font, FontStyle.Regular);
                emergencyButton.Font = new Font(emergencyButton.Font, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Changes source depending on which button is clicked
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void SourceSwitch(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "studioButton")
            {
                router.Source = Router.Sources.STUDIO;
            }
            if (((Button)sender).Name == "chartButton")
            {
                router.Source = Router.Sources.SRA;
            }
            if (((Button)sender).Name == "obButton")
            {
                router.Source = Router.Sources.OB;
            }
            if (((Button)sender).Name == "emergencyButton")
            {
                router.Source = Router.Sources.EMERGENCY;
            }
        }

        /// <summary>
        /// Updates source audio meters with sent levels
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void SourceLevelMeterUpdate(object sender, Router.LevelEventArgs e)
        {
            sourceLeftMeter.Amplitude = (float)e.LeftLevel;
            sourceRightMeter.Amplitude = (float)e.RightLevel;
        }

        /// <summary>
        /// Updates output audio meters with sent levels
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void OutputLevelMeterUpdate(object sender, Router.LevelEventArgs e)
        {
            outputLeftMeter.Amplitude = (float)e.LeftLevel;
            outputRightMeter.Amplitude = (float)e.RightLevel;
        }

        /// <summary>
        /// Changes audio input to selected input
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event args</param>
        private void studioInputComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            router.InputDevice = ((ComboBox)sender).SelectedIndex - 1;
        }

        private void fileLabel_TextChanged(object sender, EventArgs e)
        {
            router.EmergencyFile = ((Label)sender).Text;
        }

        /// <summary>
        /// Sets emergency file from file selected in browse dialog
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void browseButton_Click(object sender, EventArgs e)
        {
            // Setup file dialog
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "MP3 (*.mp3)|*.mp3|WAV (*.wav)|*.wav";
            // Show dialog, change file if OK is pressed
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileLabel.Text = fileDialog.FileName;
            }
        }

        private void chartUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            router.ChartURL = ((TextBox)sender).Text;
        }

        private void obUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            router.ObURL = ((TextBox)sender).Text;
        }

        private void detectorUpDown_ValueChanged(object sender, EventArgs e)
        {
            router.SilenceDetectorTime = (int)((NumericUpDown)sender).Value;
        }

        private void processorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Disable configure button if "None" is selected
            if (((ComboBox)sender).SelectedIndex == 0)
            {
                processorConfigButton.Enabled = false;
            }
            else
            {
                processorConfigButton.Enabled = true;
            }
            // Set processor to selected option
            router.Processor = ((ComboBox)sender).SelectedIndex;
        }

        private void processorConfigButton_Click(object sender, EventArgs e)
        {
            router.ConfigureDSP(processorComboBox.SelectedItem.ToString());
        }

        private void clockIPTextBox_TextChanged(object sender, EventArgs e)
        {
            router.ClockIP = ((TextBox)sender).Text;
        }
    }
}
