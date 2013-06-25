using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Un4seen.BassWasapi;

namespace RouterService
{
    class ResponseInputs : IWebResponse
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

        public ResponseInputs(AudioRouter router)
        {
            audioRouter = router;
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
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
                case "add":
                    pageContent = AddPage();
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
                responseContent += pageContent;
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
            // Setup page content
            string page = String.Empty;
            // Add page title
            page += "<div class=\"page-header\"><h1>Inputs</h1></div>";
            // Add button to add input
            page += "<div class=\"btn-group\"><a href=\"/inputs/add/\" class=\"btn\">Add Input</a></div>";
            // Open table of inputs
            page += "<table class=\"table\"><thead><tr><th>Name</th><th>Type</th><th>Options</th></tr></thead><tbody>";
            // List inputs
            foreach (IInput input in audioRouter.Inputs)
            {
                // Open row
                page += "<tr>";
                // Display name
                page += "<td>" + input.Name + "</td>";
                // Display type
                page += "<td>Not yet implemented</td>";
                // Display options
                page += "<div class=\"btn-group\"><a href=\"/inputs/edit/" + input.OutputChannel.ToString() + "/\" class=\"btn\">Change</a><a href=\"/inputs/delete/" + input.OutputChannel.ToString() + "/\" class=\"btn btn-danger\">Delete</a></div>";
                // Close row
                page += "</tr>";
            }
            // Close table of inputs
            page += "</tbody></table>";
            // Return page content
            return page;
        }

        private string AddPage()
        {
            // Get devices
            List<BASS_WASAPI_DEVICEINFO> devices = audioRouter.GetInputs();
            // Setup page content
            string page = String.Empty;
            if (devices.Count > 0) // If there is devices available
            {
                // Add page title
                page += "<div class=\"page-header\"><h1>Add Input</h1></div>";
                // Open form
                page += "<form class=\"form-horizontal\">";
                // Name item
                page +=
                    "<div class=\"control-group\"><label class=\"control-label \" for=\"inputName\">Name</label><div class=\"controls\"><input class=\"input-xxlarge\" type=\"text\" id=\"inputName\" placeholder=\"Name\"></div></div>";
                // List of devices
                page +=
                    "<div class=\"control-group\"><label class=\"control-label\" for=\"inputDevice\">Device</label><div class=\"controls\"><select class=\"input-xxlarge\">";
                foreach (BASS_WASAPI_DEVICEINFO device in audioRouter.GetInputs())
                {
                    page += "<option>" + device.name + "</option>";
                }
                page += "</select></div></div>";
                // Submit button
                page +=
                    "<div class=\"control-group\"><div class=\"controls\"><button type=\"submit\" class=\"btn\">Add</button></div></div>";
                // Close form
                page += "</form>";
            }
            else // Else if no devices are available
            {
                page += "<p>No devices are currently available</p>";
            }
            // Return page content
            return page;
        }
    }
}