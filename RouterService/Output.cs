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
        /// ASIO channel number to use for left speaker output
        /// </summary>
        private int asioLeftChannel;

        /// <summary>
        /// ASIO channel number to use for right speaker output
        /// </summary>
        private int asioRightChannel;

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
        /// <param name="outputType">The output type to use</param>
        public Output(int mixerHandle, int deviceID, OutputType outputType)
        {
            mixer = mixerHandle;
            device = deviceID;
            type = outputType;
            asioLeftChannel = 1;
            asioRightChannel = 2;
        }

        /// <summary>
        /// Create the output device for uncompressed output
        /// </summary>
        /// <param name="mixerHandle">The handle for the mixer channel</param>
        /// <param name="deviceID">The ID of the device to use</param>
        /// <param name="outputType">The output type to use</param>
        /// <param name="leftChannel">ASIO channel to use for left speaker</param>
        /// <param name="rightChannel">ASIO channel to use for the right speaker</param>
        public Output(int mixerHandle, int deviceID, OutputType outputType, int leftChannel, int rightChannel)
        {
            mixer = mixerHandle;
            device = deviceID;
            type = outputType;
            asioLeftChannel = leftChannel;
            asioRightChannel = rightChannel;
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
                BASS_WASAPI_DEVICEINFO deviceInfo;
                deviceInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo(device);
                float bufferSize;
                if (deviceInfo.defperiod < 0.015)
                {
                    bufferSize = 0.015f;
                }
                else
                {
                    bufferSize = deviceInfo.defperiod;
                }
                if (deviceInfo == null)
                {
                    throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
                }
                wasapiCallback = new WASAPIPROC(WASAPICallback);
                if (!BassWasapi.BASS_WASAPI_Init(device, 44100, 2, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, bufferSize * 6, bufferSize, wasapiCallback, IntPtr.Zero)) // If device does not initialise successfully
                {
                    throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString()); // Throw exception with error
                }
                #if DEBUG
                Logger.WriteLogEntry("DEBUG: Output - Buffer " + (bufferSize * 6).ToString() + "s Period " + bufferSize.ToString() + "s", System.Diagnostics.EventLogEntryType.Information);
                #endif
                ClearBuffer();
                BassWasapi.BASS_WASAPI_Start();
            }
            else if (type == OutputType.ASIO)
            {
                asioCallback = new ASIOPROC(ASIOCallback);
                if (!BassAsio.BASS_ASIO_Init(device, BASSASIOInit.BASS_ASIO_THREAD))
                {
                    throw new ApplicationException(BassAsio.BASS_ASIO_ErrorGetCode().ToString()); // Throw exception with error
                }
                BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(mixer);
                BassAsio.BASS_ASIO_ChannelEnable(false, asioLeftChannel - 1, asioCallback, new IntPtr(mixer));
                BassAsio.BASS_ASIO_ChannelJoin(false, asioRightChannel - 1, asioLeftChannel - 1);
                BassAsio.BASS_ASIO_ChannelSetFormat(false, 0, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);
                BassAsio.BASS_ASIO_ChannelSetRate(false, 0, (double)info.freq);
                BassAsio.BASS_ASIO_SetRate((double)info.freq);
                ClearBuffer();
                BassAsio.BASS_ASIO_Start(441);
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

        private void ClearBuffer()
        {
            // Clear buffer
            int length = (int)Bass.BASS_ChannelSeconds2Bytes(mixer, 1);
            float[] buffer = new float[length];
            Bass.BASS_ChannelGetData(mixer, buffer, length);
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
