using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    public partial class ServiceMain : ServiceBase
    {
        #region Private fields
        private Core core;
        #endregion

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Set up logging
            Logger.InitialiseLogging();
            // Write started log
            Logger.WriteLogEntry("ShockRouter Started", EventLogEntryType.Information);
            // Setup router
            try
            {
                core = new Core();
            }
            catch (ApplicationException e) // If router fails to start
            {
                // Write log entry
                Logger.WriteLogEntry("Unable to start router. Error: " + e.Message, EventLogEntryType.Error);
                // Stop service
                Environment.Exit(1);
            }
        }

        protected override void OnStop()
        {
        }
    }
}