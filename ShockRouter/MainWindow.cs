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
