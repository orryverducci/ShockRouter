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
        /// The type of input
        /// </summary>
        string Type { get; }
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
        /// <param name="source">The ID or address of the source to start</param>
        void Start(string source);
        /// <summary>
        /// Stop the input
        /// </summary>
        void Stop();
        /// <summary>
        /// Tell the input it has been put on air
        /// </summary>
        void PutOnAir();
    }
}
