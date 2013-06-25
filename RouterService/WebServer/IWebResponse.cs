using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    interface IWebResponse
    {
        /// <summary>
        /// The reponse
        /// </summary>
        byte[] Response { get; }
        /// <summary>
        /// The reponse HTTP status code
        /// </summary>
        int Status { get; }
        /// <summary>
        /// The content type of the response
        /// </summary>
        string ContentType { get; }
        /// <summary>
        /// Get the response. Returns true if successful
        /// </summary>
        bool GetResponse(string[] path, NameValueCollection queries);
    }
}
