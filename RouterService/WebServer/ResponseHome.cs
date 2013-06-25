using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseHome : IWebResponse
    {
        private byte[] response;

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
                return 400;
            }
        }

        public string ContentType
        {
            get
            {
                return "text/html";
            }
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
        {
            // Setup header and footer
            string headerPath =
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\webroot\\header.html";
            string footerPath =
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\webroot\\footer.html";
            string responseContent = String.Empty;
            // Output header
            if (File.Exists(headerPath)) // If header exists
            {
                // Read file and output it as part of response
                TextReader textReader = new StreamReader(headerPath);
                responseContent += textReader.ReadToEnd();
            }
            // Output footer
            if (File.Exists(footerPath)) // If header exists
            {
                // Read file and output it as part of response
                TextReader textReader = new StreamReader(footerPath);
                responseContent += textReader.ReadToEnd();
            }
            // Output final results
            Response = Encoding.UTF8.GetBytes(responseContent);
            // Return successful result
            return true;
        }
    }
}
