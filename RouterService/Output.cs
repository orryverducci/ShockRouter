﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

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
        #endregion

        /// <summary>
        /// Create the output device for uncompressed output
        /// </summary>
        /// <param name="deviceID">The ID of the device to use</param>
        public Output(int deviceID)
        {
            device = deviceID;
            // Init all outputs
            bool continueLoop = true;
            for (int i = 0; continueLoop; i++)
            {
                try
                {
                    BASS_DEVICEINFO currentDevice = Bass.BASS_GetDeviceInfo(i);
                    if (currentDevice.IsEnabled)
                    {
                        if (Bass.BASS_Init(i, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero) == false)
                        {
                            throw new ArgumentException("Unable to create output: Device " + device.ToString() + " - " + Bass.BASS_ErrorGetCode().ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    continueLoop = false;
                }
            }
            Bass.BASS_SetDevice(device);
        }

        public int DeviceID
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// Start output
        /// </summary>
        /// <param name="mixerHandle">The handle of the mixer stream</param>
        public void Start(int mixerHandle)
        {
            mixer = mixerHandle;
            ClearBuffer();
            Bass.BASS_ChannelPlay(mixer, false);
        }

        private void ClearBuffer()
        {
            // Clear buffer
            int length = (int)Bass.BASS_ChannelSeconds2Bytes(mixer, 1);
            float[] buffer = new float[length];
            Bass.BASS_ChannelGetData(mixer, buffer, length);
        }

        public void ChangeOutput(int deviceID)
        {
            Bass.BASS_ChannelSetDevice(mixer, deviceID);
            device = deviceID;
        }

        /// <summary>
        /// Stop output
        /// </summary>
        public void Stop()
        {
            Bass.BASS_ChannelStop(mixer);
        }
    }
}
