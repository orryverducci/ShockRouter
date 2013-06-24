using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RouterService
{
    class WebServer
    {
        /// <summary>
        /// The HttpListener for the server
        /// </summary>
        private HttpListener listener = new HttpListener();

        /// <summary>
        /// Starts the web server
        /// </summary>
        /// <param name="port">The port to listen on</param>
        public WebServer(int port)
        {
            // Setup server
            listener.Prefixes.Add("http://*:" + port.ToString() + "/");
            // Start server
            listener.Start();
            // Listen for requests on a new thread
            Thread listenerThread = new Thread(new ThreadStart(RequestListener));
            listenerThread.Start();
        }

        /// <summary>
        /// Stops the web server
        /// </summary>
        ~WebServer()
        {
            listener.Stop();
            listener.Close();
        }

        /// <summary>
        /// Listens for requests from the HTTP service
        /// </summary>
        private void RequestListener()
        {
            try
            {
                while (listener.IsListening) // As long as the server is active and listening for connections
                {
                    // Process request, when one is received, on a new thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RequestHandler), listener.GetContext());
                }
            }
            catch (Exception e) // Suppress any exceptions
            {
                Logger.WriteLogEntry("Server error: " + e.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Handles requests from clients
        /// </summary>
        /// <param name="context">The HTTPListenerContext for the request</param>
        private void RequestHandler(object context)
        {
            // Get the listener context
            HttpListenerContext listenerContext = (HttpListenerContext)context;
            try
            {
                // Parse the URL
                string[] path = listenerContext.Request.Url.Segments;
                string appRequested;
                if (path.Length > 1) // If a page has been chosen
                {
                    if (path[1].EndsWith("/"))
                    {
                        appRequested = path[1].Substring(0, path[1].Length - 1);
                    }
                    else
                    {
                        appRequested = path[1];
                    }
                }
                else
                {
                    appRequested = "home";
                }
                // Choose a response
                IWebResponse response;
                switch (appRequested)
                {
                    case "assets":
                        response = new ResponseAssets();
                        break;
                    case "home":
                        response = new ResponseHome();
                        break;
                    default:
                        response = new Response404();
                        break;
                }
                // Get response content
                if (!response.GetResponse(path)) // If response fails
                {
                    // Return error depending on code
                    switch (response.Status)
                    {
                        case 403:
                            response = new Response403();
                            break;
                        case 404:
                            response = new Response404();
                            break;
                        default:
                            response = new Response500();
                            break;
                    }
                    response.GetResponse(path);
                }
                // Send response
                listenerContext.Response.ContentType = response.ContentType;
                listenerContext.Response.StatusCode = response.Status;
                listenerContext.Response.ContentLength64 = response.Response.Length;
                listenerContext.Response.OutputStream.Write(response.Response, 0, response.Response.Length);
            }
            catch (Exception e) // Suppress any exceptions
            {
                Logger.WriteLogEntry("Server error: " + e.Message, EventLogEntryType.Error);
            } 
            finally // Close the stream after processing
            {
                listenerContext.Response.OutputStream.Close();
            }
        }
    }
}
