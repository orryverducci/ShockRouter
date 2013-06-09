using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    /// <summary>
    /// Provides event logging functionality
    /// </summary>
    static class Logger
    {
        /// <summary>
        /// Sets up event logging
        /// </summary>
        static public void InitialiseLogging()
        {
            // Create Event Log Source if it doesn't exist
            if (!EventLog.SourceExists("ShockRouter"))
            {
                EventLog.CreateEventSource("ShockRouter", "Application");
            }
        }

        /// <summary>
        /// Write an entry to the event log
        /// </summary>
        /// <param name="message">The message to be written to the log</param>
        /// <param name="type">The log entry type</param>
        static public void WriteLogEntry(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry("ShockRouter", message, type);
        }
    }
}