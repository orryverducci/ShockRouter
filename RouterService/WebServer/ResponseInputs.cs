using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseInputs : IWebResponse
    {
        private byte[] response;
        private string responseContent = String.Empty;
        private int status;
        private string pageContent = String.Empty;

        public byte[] Response
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
                return "text/html";
            }
        }

        public bool GetResponse(string[] path)
        {
            bool success;
            if (path.Length > 2) // If a subpage has been requested
            {
                success = BuildPage(path[2]);
            }
            else // Else if homepage requested
            {
                success = BuildPage("index");
            }
            // Output final results
            Response = Encoding.UTF8.GetBytes(responseContent);
            // Return successful result
            return success;
        }

        private bool BuildPage(string page)
        {
            bool validPage = true;
            if (page.EndsWith("/"))
            {
                page = page.Substring(0, page.Length - 1);
            }
            // Get page content
            switch (page)
            {
                case "index":
                    pageContent = IndexPage();
                    break;
                default:
                    validPage = false;
                    break;
            }
            if (validPage) // If a valid page, return it
            {
                // Set status to successful
                Status = 200;
                // Setup header and footer
                string headerPath =
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                    "\\webroot\\header.html";
                string footerPath =
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                    "\\webroot\\footer.html";
                // Output header
                if (File.Exists(headerPath)) // If header exists
                {
                    // Read file and output it as part of response
                    TextReader textReader = new StreamReader(headerPath);
                    responseContent += textReader.ReadToEnd();
                }
                // Output page content
                responseContent += "<div class=\"page-header\"><h1>Inputs</h1></div>";
                // Output footer
                if (File.Exists(footerPath)) // If header exists
                {
                    // Read file and output it as part of response
                    TextReader textReader = new StreamReader(footerPath);
                    responseContent += textReader.ReadToEnd();
                }
            }
            else // Else if not a valid page, return an error
            {
                Status = 404;
            }
            return validPage;
        }

        private string IndexPage()
        {
            return "";
        }
    }
}
