using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterService
{
    interface IInput
    {
        /// <summary>
        /// The name of the input
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The input source. For a device, this is the device ID. For a stream or file, this is the URL or path.
        /// </summary>
        string Source { get; }
        /// <summary>
        /// The output channel handle
        /// </summary>
        int OutputChannel { get; }
        /// <summary>
        /// The number of the studio (only relevent for device inputs)
        /// </summary>
        int StudioNumber { get; set; }
        /// <summary>
        /// Start the input
        /// </summary>
        void Start(string source);
        /// <summary>
        /// Stop the input
        /// </summary>
        void Stop();
    }
}
