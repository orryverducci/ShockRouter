using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseOutputs : IWebResponse
    {
        private AudioRouter audioRouter;
        private byte[] response;
        private int status;

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

        public ResponseOutputs(AudioRouter router)
        {
            audioRouter = router;
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
        {
            if (path.Length > 2) // If a subpage has been requested, return not found error
            {
                Status = 404;
            }
            else // Else return output page
            {
                bool addQueriesSet = false;
                string changeID = null;
                // Check for queries chanding a device ID
                for (int i = 0; i < queries.Count; i++)
                {
                    if (queries.GetKey(i) == "id")
                    {
                        addQueriesSet = true;
                        changeID = queries.Get(i);
                    }
                }
                if (addQueriesSet) // If a query adding a device has been sent
                {
                    if (changeID != null) // If both queries are set
                    {
                        audioRouter.ChangeOutput(Int32.Parse(changeID));
                        Status = 303;
                    }
                    else // Else return server error
                    {
                        Status = 500;
                    }
                }
                else
                {
                    Status = 200;
                    string responseContent = String.Empty;
                    List<DeviceInfo> devices = audioRouter.GetOutputs();
                    int currentDevice = audioRouter.CurrentUncompressedOutput;
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
                    // Add page title
                    responseContent += "<div class=\"page-header\"><h1>Outputs</h1></div>";
                    // Open uncompressed output form
                    responseContent += "<form class=\"form-horizontal\" action=\"/outputs/\" method=\"get\">";
                    // List of devices
                    responseContent +=
                        "<div class=\"control-group\"><label class=\"control-label\" for=\"output\">Output Device</label><div class=\"controls\"><select id=\"output\" name=\"id\" class=\"input-xxlarge\">";
                    foreach (DeviceInfo device in devices)
                    {
                        if (device.ID == currentDevice) // If current device, select it on page load
                        {
                            responseContent += "<option value=\"" + device.ID + "\" selected>" + device.Name + "</option>";
                        }
                        else
                        {
                            responseContent += "<option value=\"" + device.ID + "\">" + device.Name + "</option>";
                        }
                    }
                    responseContent += "</select></div></div>";
                    // Submit button
                    responseContent +=
                        "<div class=\"control-group\"><div class=\"controls\"><button type=\"submit\" class=\"btn\">Change</button></div></div>";
                    // Close uncompressed output form form
                    responseContent += "</form>";
                    // Output footer
                    if (File.Exists(footerPath)) // If header exists
                    {
                        // Read file and output it as part of response
                        TextReader textReader = new StreamReader(footerPath);
                        responseContent += textReader.ReadToEnd();
                    }
                    // Output final results
                    Response = Encoding.UTF8.GetBytes(responseContent);
                }
            }
            // Return successful result
            return true;
        }
    }
}
