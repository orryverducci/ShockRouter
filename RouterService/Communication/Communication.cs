using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;

namespace RouterService
{
    static class Communication
    {
        #region Initalisation and Shut Down
        /// <summary>
        /// Setup network communication
        /// </summary>
        static public void Initialise()
        {
            // Accept string commands
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", ProcessCommand);
        }

        /// <summary>
        /// Shut down network communication
        /// </summary>
        static public void ShutDown()
        {
            NetworkComms.Shutdown();
        }
        #endregion

        #region Packet Handling
        /// <summary>
        /// Processes the received command
        /// </summary>
        /// <param name="header">The PacketHeader associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The command received</param>
        private static void ProcessCommand(PacketHeader header, Connection connection, string command)
        {
            switch (command)
            {
                default:
                    break;
            }
        }
        #endregion
    }
}