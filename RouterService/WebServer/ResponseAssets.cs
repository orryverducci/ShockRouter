﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseAssets : IWebResponse
    {
        private string response;
        private int status;
        private string contentType;

        public string Response
        {
            get
            {
                return response;
            }
            private set
            {
                response = value;
            }
        }

        public int Status
        {
            get
            {
                return status;
            }
            private set
            {
                status = value;
            }
        }

        public string ContentType
        {
            get
            {
                return contentType;
            }
            private set
            {
                contentType = value;
            }
        }

        public bool GetResponse(string[] path)
        {
            // Compile path to string
            string fullPath = String.Empty;
            foreach (string segment in path)
            {
                fullPath += segment;
            }
            if (path.Length > 2) // If a file has been requested
            {
                // Set local path
                string localPath =
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                    "\\webroot" + fullPath.Replace('/', '\\').Substring(7, fullPath.Length - 7);
                // Load file
                if (File.Exists(localPath)) // If the file exists
                {
                    Status = 200;
                    // Set content type depending on extension
                    switch (Path.GetExtension(localPath))
                    {
                        case ".html":
                            ContentType = "text/html";
                            break;
                        case ".css":
                            ContentType = "text/css";
                            break;
                        case ".js":
                            ContentType = "text/javascript";
                            break;
                        case ".png":
                            ContentType = "image/png";
                            break;
                        default:
                            ContentType = "application/octet-stream";
                            break;
                    }
                    // Read file and return it as a response
                    TextReader textReader = new StreamReader(localPath);
                    Response = textReader.ReadToEnd();
                }
                else // Else return 404
                {
                    Status = 404;
                    ContentType = "text/html";
                    Response = "404 Not Found";
                }
            }
            else // Else return 403 Forbidden
            {
                Status = 403;
                ContentType = "text/html";
                Response = "403 Forbidden";
            }
            return true;
        }

    }
}