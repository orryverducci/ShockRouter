using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace RouterService
{
    class DeviceInput : IInput
    {
        #region Properties
        public string Name { get; set; }
        public string Type
        {
            get
            {
                return "Device";
            }
        }
        public string Source { get; private set; }
        public int OutputChannel { get; private set; }
        public int StudioNumber { get; set; }
        #endregion

        public void Start(string source)
        {
            Source = source;
            Bass.BASS_RecordInit(Int32.Parse(Source));
            OutputChannel = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT, null, IntPtr.Zero);
            if (OutputChannel == default(int)) // If does not start recording successfully
            {
                throw new ArgumentException("Unable to create input stream: " + Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
            }
        }

        public void Stop()
        {
            // Free device
            Bass.BASS_RecordSetDevice(Int32.Parse(Source));
            Bass.BASS_RecordFree();
            // Clear stream
            Bass.BASS_StreamFree(OutputChannel);
            OutputChannel = default(int);
        }

        public void PutOnAir() { }
    }
}
