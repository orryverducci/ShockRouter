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
                case "add":
                    pageContent = AddPage(queries, path);
                    break;
                case "edit":
                    pageContent = EditPage(queries, path);
                    break;
                case "delete":
                    pageContent = DeletePage(path);
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
            page += "<div class=\"page-header\"><h1>Inputs</h1></div>";
            // Add button to add input
            page += "<div class=\"btn-group\"><a href=\"#\" class=\"btn dropdown-toggle\" data-toggle=\"dropdown\">Add Input <span class=\"caret\"></span></a><ul class=\"dropdown-menu\"><li><a href=\"/inputs/add/device/\">Device</a></li><li><a href=\"/inputs/add/file/\">File</a></li><li><a href=\"/inputs/add/stream/\">Online Stream</a></li></ul></div>";
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
                page += "<td>" + input.Type + "</td>";
                // Display options
                page += "<td><div class=\"btn-group\"><a href=\"/inputs/edit/" + input.OutputChannel.ToString() + "/\" class=\"btn\">Change</a><a href=\"/inputs/delete/" + input.OutputChannel.ToString() + "/\" class=\"btn btn-danger\">Delete</a></div></td>";
                // Close row
                page += "</tr>";
            }
            // Display message if there is no inputs
            if (audioRouter.Inputs.Count == 0)
            {
                page += "<td colspan=\"3\">No inputs have been added</td>";
            }
            // Close table of inputs
            page += "</tbody></table>";
            // Return page content
            return page;
        }

        private string AddPage(NameValueCollection queries, string[] path)
        {
            string page = String.Empty;
            if (path.Length < 4) // If add index page requested
            {
                bool addQueriesSet = false;
                string addType = null;
                string addID = null;
                string addName = null;
                string addStudio = null;
                // Check for queries adding a device ID
                for (int i = 0; i < queries.Count; i++)
                {
                    if (queries.GetKey(i) == "id")
                    {
                        addQueriesSet = true;
                        addID = queries.Get(i);
                    }
                    else if (queries.GetKey(i) == "name")
                    {
                        addQueriesSet = true;
                        addName = queries.Get(i);
                    }
                    else if (queries.GetKey(i) == "studio")
                    {
                        addQueriesSet = true;
                        addStudio = queries.Get(i);
                    }
                    else if (queries.GetKey(i) == "type")
                    {
                        addQueriesSet = true;
                        addType = queries.Get(i);
                    }
                }
                if (addQueriesSet) // If a query adding a device has been sent
                {
                    if (addID != null && addName != null && ((addType == "device" && addStudio != null) || addType == "file" || addType=="stream")) // If all the requried queries are set
                    {
                        if (addType == "device")
                        {
                            audioRouter.AddInputDevice(addName, addID, Int32.Parse(addStudio));
                        }
                        else if (addType == "file")
                        {
                            audioRouter.AddInputFile(addName, addID);
                        }
                        else if (addType == "stream")
                        {
                            audioRouter.AddInputStream(addName, addID);
                        }
                        Status = 303; // Return redirect code
                    }
                    else // Else return server error
                    {
                        Status = 500;
                    }
                }
                else // Else return server error
                {
                    Status = 500;
                }
            }
            else if (path.Length < 5) // Else if input types have been requested
            {
                // Get input type to add
                string inputType = path[3];
                if (inputType.EndsWith("/"))
                {
                    inputType = inputType.Substring(0, inputType.Length - 1);
                }
                if (inputType == "device" || inputType == "file" || inputType == "stream") // If a valid input type
                {
                    // Set status to successful
                    Status = 200;
                    // Add page title
                    page += "<div class=\"page-header\"><h1>Add Input</h1></div>";
                    // Open form
                    page += "<form class=\"form-horizontal\" action=\"/inputs/add/\" method=\"get\">";
                    // Name item
                    page += "<div class=\"control-group\"><label class=\"control-label \" for=\"inputName\">Name</label><div class=\"controls\"><input class=\"input-xxlarge\" type=\"text\" id=\"inputName\" name=\"name\" placeholder=\"Name\"></div></div>";
                    // Device specific options
                    if (inputType == "device")
                    {
                        List<DeviceInfo> devices = audioRouter.GetInputs(false);
                        if (devices.Count > 0) // If there is devices available
                        {
                            // List of devices
                            page +=
                                "<div class=\"control-group\"><label class=\"control-label\" for=\"inputDevice\">Device</label><div class=\"controls\"><select id=\"inputDevice\" name=\"id\" class=\"input-xxlarge\">";
                            foreach (DeviceInfo device in devices)
                            {
                                page += "<option value=\"" + device.ID + "\">" + device.Name + "</option>";
                            }
                            page += "</select></div></div>";
                        }
                        else // Else if no devices are available
                        {
                            page += "<p>No devices are currently available</p>";
                        }
                        // Studio number
                        page += "<div class=\"control-group\"><label class=\"control-label\" for=\"inputStudio\">Studio Number</label><div class=\"controls\"><select id=\"inputStudio\" name=\"studio\" class=\"input-xxlarge\">";
                        page += "<option value=\"0\" selected>None</option>";
                        for (int i = 1; i < 11; i++)
                        {
                            page += "<option value=\"" + i.ToString() + "\">" + i.ToString() + "</option>";
                        }
                        page += "</select></div></div>";
                        // Type Field
                        page += "<input type=\"hidden\" name=\"type\" value=\"device\">";
                    }
                    // File specific options
                    else if (inputType == "file")
                    {
                        // Filename item
                        page += "<div class=\"control-group\"><label class=\"control-label \" for=\"inputFilename\">Filename</label><div class=\"controls\"><input class=\"input-xxlarge\" type=\"text\" id=\"inputFilename\" name=\"id\" placeholder=\"Filename\"></div></div>";
                        // Type Field
                        page += "<input type=\"hidden\" name=\"type\" value=\"file\">";
                    }
                    // Stream specific options
                    else if (inputType == "stream")
                    {
                        // Filename item
                        page += "<div class=\"control-group\"><label class=\"control-label \" for=\"inputURL\">Stream URL</label><div class=\"controls\"><input class=\"input-xxlarge\" type=\"text\" id=\"inputURL\" name=\"id\" placeholder=\"URL\"></div></div>";
                        // Type Field
                        page += "<input type=\"hidden\" name=\"type\" value=\"stream\">";
                    }
                    // Submit button
                    page += "<div class=\"control-group\"><div class=\"controls\"><button type=\"submit\" class=\"btn\">Add</button></div></div>";
                    // Close form
                    page += "</form>";
                }
                else // Else return not found error
                {
                    Status = 404;
                }
            }
            else // Else if further subpages have been requested, return not found error
            {
                Status = 404;
            }
            // Return page content
            return page;
        }

        private string EditPage(NameValueCollection queries, string[] path)
        {
            string page = String.Empty;
            if (path.Length > 4) // If a subpage has been requested, return not found
            {
                Status = 404;
            }
            else if (path.Length < 4) // If no device has been parsed, process queries or return
            {
                bool editQueriesSet = false;
                string editID = null;
                string editName = null;
                string editStudio = null;
                // Check for queries editing a device
                for (int i = 0; i < queries.Count; i++)
                {
                    if (queries.GetKey(i) == "id")
                    {
                        editQueriesSet = true;
                        editID = queries.Get(i);
                    }
                    else if (queries.GetKey(i) == "name")
                    {
                        editQueriesSet = true;
                        editName = queries.Get(i);
                    }
                    else if (queries.GetKey(i) == "studio")
                    {
                        editQueriesSet = true;
                        editStudio = queries.Get(i);
                    }
                }
                if (editStudio == null)
                {
                    editStudio = "0";
                }
                if (editQueriesSet) // If queries received, edit device
                {
                    if (editID != null && editName != null) // If all queries are set
                    {
                        audioRouter.EditInput(editName, Int32.Parse(editID), Int32.Parse(editStudio));
                        Status = 303;
                    }
                    else // Else return error
                    {
                        Status = 500;
                    }
                }
                else // If not, return error
                {
                    Status = 500;
                }
            }
            else // Else display edit page
            {
                // Get input
                int inputChannelHandle;
                if (path[3].EndsWith("/"))
                {
                    inputChannelHandle = Int32.Parse(path[3].Substring(0, path[3].Length - 1));
                }
                else
                {
                    inputChannelHandle = Int32.Parse(path[3]);
                }
                IInput input = audioRouter.Inputs.Find(specifiedInput => specifiedInput.OutputChannel == inputChannelHandle);
                if (input != null) // If input is found, return edit page
                {
                    // Return success
                    Status = 200;
                    // Add page title
                    page += "<div class=\"page-header\"><h1>Change Input</h1></div>";
                    // Open form
                    page += "<form class=\"form-horizontal\" action=\"/inputs/edit/\" method=\"get\">";
                    // Name item
                    page += "<div class=\"control-group\"><label class=\"control-label \" for=\"inputName\">Name</label><div class=\"controls\"><input class=\"input-xxlarge\" type=\"text\" id=\"inputName\" name=\"name\" placeholder=\"Name\" value=\"" + input.Name + "\"></input></div></div>";
                    // Studio number
                    if (input.Type == "Device")
                    {
                        page += "<div class=\"control-group\"><label class=\"control-label\" for=\"inputStudio\">Studio Number</label><div class=\"controls\"><select id=\"inputStudio\" name=\"studio\" class=\"input-xxlarge\">";
                        if (input.StudioNumber == 0)
                        {
                            page += "<option value=\"0\" selected>None</option>";
                        }
                        else
                        {
                            page += "<option value=\"0\">None</option>";
                        }
                        for (int i = 1; i < 11; i++)
                        {
                            if (input.StudioNumber == i)
                            {
                                page += "<option value=\"" + i.ToString() + "\" selected>" + i.ToString() + "</option>";
                            }
                            else
                            {
                                page += "<option value=\"" + i.ToString() + "\">" + i.ToString() + "</option>";
                            }
                        }
                        page += "</select></div></div>";
                    }
                    // Device notice
                    page += "<div class=\"control-group\"><div class=\"controls\"><i>To change the input, this input has to be deleted and readded.</i></div></div>";
                    // ID Field
                    page += "<input type=\"hidden\" name=\"id\" value=\"" + inputChannelHandle.ToString() + "\">";
                    // Submit button
                    page += "<div class=\"control-group\"><div class=\"controls\"><button type=\"submit\" class=\"btn\">Change</button></div></div>";
                    // Close form
                    page += "</form>";
                }
                else // Else if input is not found, return an error
                {
                    Status = 500;
                }
            }
            // Return page content
            return page;
        }

        private string DeletePage(string[] path)
        {
            string page = String.Empty;
            if (path.Length > 4) // If a subpage has been requested, return not found
            {
                Status = 404;
            }
            else if (path.Length < 4) // If no device has been parsed return error
            {
                Status = 500;
            }
            else // Else process deletion
            {
                int inputChannelHandle;
                if (path[3].EndsWith("/"))
                {
                    inputChannelHandle = Int32.Parse(path[3].Substring(0, path[3].Length - 1));
                }
                else
                {
                    inputChannelHandle = Int32.Parse(path[3]);
                }
                audioRouter.DeleteInput(inputChannelHandle);
                Status = 303;
            }
            return page;
        }
    }
}