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
    class FileInput : IInput
    {
        #region Properties
        public string Name { get; set; }
        public string Type
        {
            get
            {
                return "File";
            }
        }
        public string Source { get; private set; }
        public int OutputChannel { get; private set; }
        public int StudioNumber { get; set; }
        #endregion

        public void Start(string source)
        {
            Source = source;
            OutputChannel = Bass.BASS_StreamCreateFile(source, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_LOOP);
            if (OutputChannel == default(int)) // If does not start recording successfully
            {
                throw new ArgumentException("Unable to create input stream"); // Throw exception with error
            }
            // Start playing
            Bass.BASS_ChannelPlay(OutputChannel, true);
        }

        public void Stop()
        {
            // Stop playback
            // Stop playing
            Bass.BASS_ChannelStop(OutputChannel);
            // Clear stream
            Bass.BASS_StreamFree(OutputChannel);
            OutputChannel = default(int);
        }
    }
}
