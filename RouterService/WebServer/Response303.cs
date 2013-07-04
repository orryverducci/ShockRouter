using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class Response303 : IWebResponse
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
                return 303;
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
            string responseContent = String.Empty;
            Response = Encoding.UTF8.GetBytes(responseContent);
            return true;
        }
    }
}
