using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShockRouter
{
    class RemoteControl
    {
        /// <summary>
        /// Listens for TCP connections from any network interface on port 7000
        /// </summary>
        TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 7000);

        /// <summary>
        /// Thread to carry out client tasks
        /// </summary>
        Thread serviceThread;

        /// <summary>
        /// Socket used for client connection
        /// </summary>
        Socket clientSocket;

        /// <summary>
        /// Stream used for network communcation
        /// </summary>
        Stream communicationStream;

        /// <summary>
        /// Stream reader for communication stream
        /// </summary>
        StreamReader streamReader;

        /// <summary>
        /// Stream write for communcation stream
        /// </summary>
        StreamWriter streamWriter;

        /// <summary>
        /// Source requested by the remote user
        /// </summary>
        public Router.Sources requestedSource;

        /// <summary>
        /// Event for when the remote changes sourche
        /// </summary>
        public event EventHandler RemoteSourceChange;

        public RemoteControl()
        {
            // Start listening for connections
            listener.Start();
            // Create and start thread to carry out client tasks
            serviceThread = new Thread(new ThreadStart(RemoteService));
            serviceThread.Start();
        }

        public void RemoteService()
        {
            while (true)
            {
                // Accept client connection
                clientSocket = listener.AcceptSocket();
                // Setup stream
                communicationStream = new NetworkStream(clientSocket);
                streamReader = new StreamReader(communicationStream);
                streamWriter = new StreamWriter(communicationStream);
                // While loop to carry out communcation
                bool active = true;
                while (active)
                {
                    // Read the command
                    string command = streamReader.ReadLine();
                    // Execute action for each command
                    switch (command)
                    {
                        case "STUDIO":
                            requestedSource = Router.Sources.STUDIO;
                            if (RemoteSourceChange != null)
                            {
                                RemoteSourceChange(this, new EventArgs());
                            }
                            break;
                        case "SRA":
                            requestedSource = Router.Sources.SRA;
                            if (RemoteSourceChange != null)
                            {
                                RemoteSourceChange(this, new EventArgs());
                            }
                            break;
                        case "OB":
                            requestedSource = Router.Sources.OB;
                            if (RemoteSourceChange != null)
                            {
                                RemoteSourceChange(this, new EventArgs());
                            }
                            break;
                        case "EMERGENCY":
                            requestedSource = Router.Sources.EMERGENCY;
                            if (RemoteSourceChange != null)
                            {
                                RemoteSourceChange(this, new EventArgs());
                            }
                            break;
                        case "EXIT":
                            // Client is exiting, close communcation
                            active = false;
                            break;
                        default:
                            // Invalid response received, ignore
                            break;
                    }
                }
                // Close socket at the end of communcation
                clientSocket.Close();
            }
        }
    }
}
