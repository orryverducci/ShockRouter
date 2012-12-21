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
        Router router = new Router();

        public MainWindow()
        {
            // Set UI font to system font
            this.Font = SystemFonts.MessageBoxFont;
            // Initialise UI
            InitializeComponent();
            // Increase font size on source buttons
            studioButton.Font = new Font(studioButton.Font.FontFamily, 12, FontStyle.Regular);
            chartButton.Font = new Font(chartButton.Font.FontFamily, 12, FontStyle.Regular);
            obButton.Font = new Font(obButton.Font.FontFamily, 12, FontStyle.Regular);
            emergencyButton.Font = new Font(emergencyButton.Font.FontFamily, 12, FontStyle.Regular);
            // Handle router source changed event
            router.SourceChanged += SourceChanged;
            // Set to studio
            router.Source = Router.Sources.STUDIO;
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
    }
}
