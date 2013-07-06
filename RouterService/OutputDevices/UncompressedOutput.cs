using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace RouterService
{
    class UncompressedOutput
    {
        #region Private fields
        /// <summary>
        /// Handle for the mixer channel
        /// </summary>
        private int mixer;

        /// <summary>
        /// ID for the current device
        /// </summary>
        private int device;

        /// <summary>
        /// Callback to send output to device
        /// </summary>
        private WASAPIPROC outputCallback;
        #endregion

        /// <summary>
        /// Create the output device for uncompressed output
        /// </summary>
        /// <param name="mixerHandle">The handle for the mixer channel</param>
        public UncompressedOutput(int mixerHandle, int deviceID)
        {
            mixer = mixerHandle;
            device = deviceID;
        }

        /// <summary>
        /// Start output
        /// </summary>
        public void Start()
        {
            outputCallback = new WASAPIPROC(OutputCallback);
            if (!BassWasapi.BASS_WASAPI_Init(device, 44100, 2, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 0.009f, 0.003f, outputCallback, IntPtr.Zero)) // If device does not initialise successfully
            {
                throw new ArgumentException(Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
            }
            BassWasapi.BASS_WASAPI_Start();
        }

        /// <summary>
        /// The callback to output to the device
        /// </summary>
        /// <param name="buffer">Audio buffer to be written to</param>
        /// <param name="length">Length of the audio buffer</param>
        /// <param name="user">User instance</param>
        /// <returns></returns>
        private int OutputCallback(IntPtr buffer, int length, IntPtr user)
        {
            return Bass.BASS_ChannelGetData(mixer, buffer, length);
        }

        /// <summary>
        /// Stop output
        /// </summary>
        public void Stop()
        {
            // Free device
            BassWasapi.BASS_WASAPI_SetDevice(device);
            BassWasapi.BASS_WASAPI_Free();
            // Clear callback
            outputCallback = null;
        }
    }
}
