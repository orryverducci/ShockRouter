using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseStreams : IWebResponse
    {
        private AudioRouter audioRouter;
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

        public ResponseStreams(AudioRouter router)
        {
            audioRouter = router;
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
        {
            bool success;
            if (path.Length > 2) // If a subpage has been requested
            {
                success = BuildPage(path[2], queries, path);
            }
            else // Else if homepage requested
            {
                success = BuildPage("index", queries, path);
            }
            // Output final results
            Response = Encoding.UTF8.GetBytes(responseContent);
            // Return successful result
            return success;
        }

        private bool BuildPage(string page, NameValueCollection queries, string[] path)
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
                if (Status == 200) // If page was successful
                {
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
                    responseContent += pageContent;
                    // Output footer
                    if (File.Exists(footerPath)) // If header exists
                    {
                        // Read file and output it as part of response
                        TextReader textReader = new StreamReader(footerPath);
                        responseContent += textReader.ReadToEnd();
                    }
                }
                else // Else return error
                {
                    validPage = false;
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
            // Set status to successful
            Status = 200;
            // Setup page content
            string page = String.Empty;
            // Add page title
            page += "<div class=\"page-header\"><h1>Streams</h1></div>";
            // Add button to add input
            page += "<div class=\"btn-group\"><a href=\"#\" class=\"btn\">Add Stream</a></div>";
            // Open table of inputs
            page += "<table class=\"table\"><thead><tr><th>Server</th><th>Details</th><th>Options</th></tr></thead><tbody>";
            // List inputs
            foreach (IInput input in audioRouter.Inputs)
            {
                // Open row
                page += "<tr>";
                // Display name
                page += "<td>" + "Not yet implemented" + "</td>";
                // Display type
                page += "<td>" + "Not yet implemented" + "</td>";
                // Display options
                page += "<td><div class=\"btn-group\"><a href=\"/inputs/delete/" + "#" + "/\" class=\"btn btn-danger\">Delete</a></div></td>";
                // Close row
                page += "</tr>";
            }
            // Display message if there is no inputs
            if (audioRouter.Inputs.Count == 0)
            {
                page += "<td colspan=\"3\">No streams have been added</td>";
            }
            // Close table of inputs
            page += "</tbody></table>";
            // Return page content
            return page;
        }
    }
}
