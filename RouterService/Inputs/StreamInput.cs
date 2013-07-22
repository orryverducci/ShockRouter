using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace RouterService
{
    class StreamInput : IInput
    {
        #region Properties
        public string Name { get; set; }
        public string Type
        {
            get
            {
                return "Stream";
            }
        }
        public string Source { get; private set; }
        public int OutputChannel { get; private set; }
        public int StudioNumber { get; set; }
        #endregion

        public void Start(string source)
        {
            if (source != default(string)) // If a URL has been set
            {
                Source = source;
                // Create stream
                OutputChannel = Bass.BASS_StreamCreateURL(source, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null, IntPtr.Zero);
                if (OutputChannel == default(int)) // If does not create successfully
                {
                    throw new ArgumentException("Unable to create input stream"); // Throw exception with error
                }
                // Start playing
                Bass.BASS_ChannelPlay(OutputChannel, true);
            }
            else // Otherwise throw exception saying file hasn't been set
            {
                throw new ApplicationException("Unable to create input stream - no stream URL set");
            }
        }

        public void Stop()
        {
            // Stop playing
            Bass.BASS_ChannelStop(OutputChannel);
            // Clear stream
            Bass.BASS_StreamFree(OutputChannel);
            OutputChannel = default(int);
        }

        public void PutOnAir() { }
    }
}
