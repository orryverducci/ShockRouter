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
        
        #region Private Fields
        private WASAPIPROC inputCallback;
        #endregion

        public void Start(string source)
        {
            Source = source;
            inputCallback = new WASAPIPROC(InputCallback);
            OutputChannel = Bass.BASS_StreamCreatePush(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, IntPtr.Zero);
            if (!BassWasapi.BASS_WASAPI_Init(Int32.Parse(Source), 44100, 2, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 0.009f, 0.003f, inputCallback, IntPtr.Zero)) // If device does not initialise successfully
            {
                throw new ArgumentException(Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
            }
            if (OutputChannel == default(int)) // If does not start recording successfully
            {
                throw new ArgumentException("Unable to create input stream"); // Throw exception with error
            }
            BassWasapi.BASS_WASAPI_Start();
        }

        private int InputCallback(IntPtr buffer, int length, IntPtr user)
        {
            Bass.BASS_StreamPutData(OutputChannel, buffer, length);
            return 1;
        }

        public void Stop()
        {
            // Free device
            BassWasapi.BASS_WASAPI_SetDevice(Int32.Parse(Source));
            BassWasapi.BASS_WASAPI_Free();
            // Clear callback
            inputCallback = null;
            // Clear stream
            Bass.BASS_StreamFree(OutputChannel);
            OutputChannel = default(int);
        }

        public void PutOnAir() { }
    }
}
