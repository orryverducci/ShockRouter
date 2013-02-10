using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RouterClient
{
    public partial class MainWindow : Form
    {
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
            // Load user settings
            ipTextBox.Text = Properties.Settings.Default.ServerIP;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save user settings
            Properties.Settings.Default.ServerIP = ipTextBox.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Sends chosen source to server
        /// </summary>
        /// <param name="source">Source to change to</param>
        private void SendCommand(string source)
        {
            // Send source information to clock
            if (ipTextBox.Text != "") // If an IP has been set
            {
                try
                {
                    TcpClient client = new TcpClient(ipTextBox.Text, 7000);
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    writer.AutoFlush = true;
                    writer.WriteLine(source);
                    writer.WriteLine("EXIT");
                    client.GetStream().Close();
                    client.Close();
                }
                catch
                {
                    // Do nothing if an exception occurs
                }
            }
        }

        private void studioButton_Click(object sender, EventArgs e)
        {
            SendCommand("STUDIO");
        }

        private void chartButton_Click(object sender, EventArgs e)
        {
            SendCommand("SRA");
        }

        private void obButton_Click(object sender, EventArgs e)
        {
            SendCommand("OB");
        }

        private void emergencyButton_Click(object sender, EventArgs e)
        {
            SendCommand("EMERGENCY");
        }
    }
}
