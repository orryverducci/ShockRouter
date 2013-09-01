using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;

namespace RouterService
{
    class Output
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
        /// Output type to use
        /// </summary>
        private OutputType type;

        /// <summary>
        /// Callback to send output to device
        /// </summary>
        private ASIOPROC asioCallback;

        /// <summary>
        /// Callback to send output to device
        /// </summary>
        private WASAPIPROC wasapiCallback;
        #endregion

        public enum OutputType
        {
            WASAPI,
            ASIO
        }

        /// <summary>
        /// Create the output device for uncompressed output
        /// </summary>
        /// <param name="mixerHandle">The handle for the mixer channel</param>
        /// <param name="deviceID">The ID of the device to use</param>
        /// <param name="type">The output type to use</param>
        public Output(int mixerHandle, int deviceID, OutputType outputType)
        {
            mixer = mixerHandle;
            device = deviceID;
            type = outputType;
        }

        public int DeviceID
        {
            get
            {
                return device;
            }
        }

        public OutputType Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Start output
        /// </summary>
        public void Start()
        {
            if (type == OutputType.WASAPI)
            {
                wasapiCallback = new WASAPIPROC(WASAPICallback);
                if (!BassWasapi.BASS_WASAPI_Init(device, 44100, 2, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 0.009f, 0.003f, wasapiCallback, IntPtr.Zero)) // If device does not initialise successfully
                {
                    throw new ArgumentException(Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
                }
                BassWasapi.BASS_WASAPI_Start();
            }
            else if (type == OutputType.ASIO)
            {
                asioCallback = new ASIOPROC(ASIOCallback);
                if (!BassAsio.BASS_ASIO_Init(device, BASSASIOInit.BASS_ASIO_THREAD))
                {
                    throw new ArgumentException(BassAsio.BASS_ASIO_ErrorGetCode().ToString()); // Throw exception with error
                }
                BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(mixer);
                BassAsio.BASS_ASIO_ChannelEnable(false, 0, asioCallback, new IntPtr(mixer));
                for (int i = 1; i < info.chans; i++)
                {
                    BassAsio.BASS_ASIO_ChannelJoin(false, i, 0);
                }
                BassAsio.BASS_ASIO_ChannelSetFormat(false, 0, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);
                BassAsio.BASS_ASIO_ChannelSetRate(false, 0, (double)info.freq);
                BassAsio.BASS_ASIO_SetRate((double)info.freq);
                BassAsio.BASS_ASIO_Start(0);
            }
        }

        /// <summary>
        /// The callback to output to the device
        /// </summary>
        /// <param name="input">Is the device an input device</param>
        /// <param name="channel">The channel number</param>
        /// <param name="buffer">Audio buffer to be written to</param>
        /// <param name="length">Length of the audio buffer</param>
        /// <param name="user">User instance</param>
        /// <returns></returns>
        private int ASIOCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
        {
            return Bass.BASS_ChannelGetData(user.ToInt32(), buffer, length);
        }

        /// <summary>
        /// The callback to output to the device
        /// </summary>
        /// <param name="buffer">Audio buffer to be written to</param>
        /// <param name="length">Length of the audio buffer</param>
        /// <param name="user">User instance</param>
        /// <returns></returns>
        private int WASAPICallback(IntPtr buffer, int length, IntPtr user)
        {
            return Bass.BASS_ChannelGetData(mixer, buffer, length);
        }

        /// <summary>
        /// Stop output
        /// </summary>
        public void Stop()
        {
            // Free device
            if (type == OutputType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_SetDevice(device);
                BassWasapi.BASS_WASAPI_Free();
            }
            else if (type == OutputType.ASIO)
            {
                BassAsio.BASS_ASIO_SetDevice(device);
                BassAsio.BASS_ASIO_Free();
            }
            // Clear callback
            wasapiCallback = null;
        }
    }
}
