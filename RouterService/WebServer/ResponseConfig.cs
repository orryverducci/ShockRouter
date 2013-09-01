using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseConfig : IWebResponse
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

        public ResponseConfig(AudioRouter router)
        {
            audioRouter = router;
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
        {
            string responseContent = String.Empty;
            if (path.Length > 2) // If a subpage has been requested, return not found error
            {
                Status = 404;
            }
            else // Else return output page
            {
                bool addQueriesSet = false;
                string changeID = null;
                string changeIP = null;
                // Check for queries changing a device ID
                for (int i = 0; i < queries.Count; i++)
                {
                    if (queries.GetKey(i) == "id")
                    {
                        addQueriesSet = true;
                        changeID = queries.Get(i);
                    }
                    if (queries.GetKey(i) == "clockip")
                    {
                        addQueriesSet = true;
                        changeIP = queries.Get(i);
                    }
                }
                if (addQueriesSet) // If a query adding a device has been sent
                {
                    if (changeID != null && changeIP != null) // If all queries are set
                    {
                        audioRouter.ChangeOutput(changeID);
                        audioRouter.ClockIP = changeIP;
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
                    // Setup header and footer
                    string headerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\webroot\\header.html";
                    string footerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\webroot\\footer.html";
                    // Output header
                    if (File.Exists(headerPath)) // If header exists
                    {
                        // Read file and output it as part of response
                        TextReader textReader = new StreamReader(headerPath);
                        responseContent += textReader.ReadToEnd();
                    }
                    // Add page title
                    responseContent += "<div class=\"page-header\"><h1>Configuration</h1></div>";
                    // Open form
                    responseContent += "<form class=\"form-horizontal\" role=\"form\" action=\"/config/\" method=\"get\">";
                    // List of devices
                    responseContent += "<div class=\"form-group\"><label class=\"col-lg-2 control-label\" for=\"output\">Output Device</label><div class=\"col-lg-10\"><select id=\"output\" name=\"id\" class=\"form-control\">";
                    foreach (DeviceInfo device in audioRouter.GetWASAPIOutputs())
                    {
                        if (("1-" + device.ID) == audioRouter.CurrentOutput) // If current device, select it on page load
                        {
                            responseContent += "<option value=\"1-" + device.ID + "\" selected>WASAPI: " + device.Name + "</option>";
                        }
                        else
                        {
                            responseContent += "<option value=\"1-" + device.ID + "\">WASAPI: " + device.Name + "</option>";
                        }
                    }
                    foreach (DeviceInfo device in audioRouter.GetASIOOutputs())
                    {
                        if (("2-" + device.ID) == audioRouter.CurrentOutput) // If current device, select it on page load
                        {
                            responseContent += "<option value=\"2-" + device.ID + "\" selected>ASIO: " + device.Name + "</option>";
                        }
                        else
                        {
                            responseContent += "<option value=\"2-" + device.ID + "\">ASIO: " + device.Name + "</option>";
                        }
                    }
                    responseContent += "</select></div></div>";
                    // Clock IP item
                    responseContent += "<div class=\"form-group\"><label class=\"col-lg-2 control-label\" for=\"clockIP\">Clock IP</label><div class=\"col-lg-10\"><input class=\"form-control\" type=\"text\" id=\"clockIP\" name=\"clockip\" placeholder=\"0.0.0.0\" value=\"" + audioRouter.ClockIP + "\"></div></div>";
                    // Submit button
                    responseContent += "<div class=\"form-group\"><div class=\"col-lg-10 col-lg-offset-2\"><button type=\"submit\" class=\"btn\">Change</button></div></div>";
                    // Close uncompressed output form form
                    responseContent += "</form>";
                    // Output footer
                    if (File.Exists(footerPath)) // If header exists
                    {
                        // Read file and output it as part of response
                        TextReader textReader = new StreamReader(footerPath);
                        responseContent += textReader.ReadToEnd();
                    }
                }
            }
            // Output final results
            Response = Encoding.UTF8.GetBytes(responseContent);
            // Return successful result
            return true;
        }
    }
}
