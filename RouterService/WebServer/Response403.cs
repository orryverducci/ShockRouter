using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class Response403 : IWebResponse
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
                return 403;
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
            string responseContent = "403 Forbidden";
            Response = Encoding.UTF8.GetBytes(responseContent);
            return true;
        }
    }
}
