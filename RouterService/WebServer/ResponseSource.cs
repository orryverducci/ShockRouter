using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    class ResponseSource : IWebResponse
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

        public ResponseSource(AudioRouter router)
        {
            audioRouter = router;
        }

        public bool GetResponse(string[] path, NameValueCollection queries)
        {
            bool success;
            if (path.Length == 3) // If a source has been requested
            {
                int source;
                if (path[2].EndsWith("/"))
                {
                    source = Int32.Parse(path[2].Substring(0, path[2].Length - 1));
                }
                else
                {
                    source = Int32.Parse(path[2]);
                }
                audioRouter.ResetSilenceDetector();
                audioRouter.CurrentInput = source;
                Status = 303;
                success = true;
            }
            else
            {
                Status = 404;
                success = false;
            }
            Response = Encoding.UTF8.GetBytes(String.Empty);
            return success;
        }

    }
}
