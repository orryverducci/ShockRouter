using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class Core
    {
        #region Private fields
        private AudioRouter router;
        private Datacaster datacaster;
        private WebServer webServer;
        #endregion

        public Core()
        {
            // Setup audio router
            try
            {
                router = new AudioRouter();
            }
            catch (ApplicationException e) // If audio router fails to start
            {
                throw new ApplicationException("Unable to start audio - " + e.Message);
            }
            // Start the web server on port 7000
            webServer = new WebServer(7000, router);
        }
    }
}
