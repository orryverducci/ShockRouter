using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

namespace ShockRouter
{
    class Router
    {
        #region Private Fields
        /// <summary>
        /// The currently active source
        /// </summary>
        private Sources currentSource;
        /// <summary>
        /// Handle of the recording stream
        /// </summary>
        private int recordingHandle;
        /// <summary>
        /// Handle of the audio mixer
        /// </summary>
        private int mixerHandle;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the currently active source
        /// </summary>
        public Sources Source
        {
            get
            {
                return currentSource;
            }
            set
            {
                ChangeSource(value);
            }
        }
        #endregion

        #region Enumerations
        /// <summary>
        /// Audio sources
        /// </summary>
        public enum Sources
        {
            /// <summary>
            /// Line input from studio
            /// </summary>
            STUDIO,
            /// <summary>
            /// SRA Chart Show stream
            /// </summary>
            SRA,
            /// <summary>
            /// Outside broadcast stream
            /// </summary>
            OB,
            /// <summary>
            /// Emergency output file
            /// </summary>
            EMERGENCY
        }
        #endregion

        #region Methods
        #region Constructor and destructor
        /// <summary>
        /// Initialises the audio inputs and outputs
        /// </summary>
        public Router()
        {
            // Registration code for Bass.Net
            // Provided value should be used only for ShockRouter, not derived products
            BassNet.Registration("orry@orryverducci.co.uk", "2X24373423243720");
            // Set dll locations
            if (IntPtr.Size == 8) // If running in 64 bit
            {
                Bass.LoadMe("x64");
                BassMix.LoadMe("x64");
            }
            else // Else if running in 32 bit
            {
                Bass.LoadMe("x86");
                BassMix.LoadMe("x86");
            }
            // Initialise BASS
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero)) // If unable to initialise audio output
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
            // Create Mixer
            mixerHandle = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_NONSTOP);
            if (mixerHandle == 0) // If unable to initialise mixer
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
            Bass.BASS_ChannelPlay(mixerHandle, false);
            // Initalise Line In
            InitaliseLineIn();
        }

        /// <summary>
        /// Shuts down audio inputs and outputs
        /// </summary>
        ~Router()
        {
            BassMix.FreeMe(); // Free BASSmix
            Bass.FreeMe(); // Free BASS
        }
        #endregion

        #region Audio Sources
        /// <summary>
        /// Sets Studio Line In input to default device
        /// </summary>
        private void InitaliseLineIn()
        {
            InitaliseLineIn(-1);
        }

        /// <summary>
        /// Sets Studio Line In input to chosen device
        /// </summary>
        /// <param name="deviceID">ID of device to use</param>
        private void InitaliseLineIn(int deviceID)
        {
            if (recordingHandle != default(int)) // If a recording device is currently running
            {
                BassMix.BASS_Mixer_ChannelRemove(recordingHandle); // Remove channel from mixer
                Bass.BASS_ChannelStop(recordingHandle); // Stop recording
                recordingHandle = default(int); // Set handle to default value
            }
            if (!Bass.BASS_RecordInit(deviceID)) // If unable to initialise input device
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
            // Start recording
            recordingHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT, null, IntPtr.Zero);
            // Add to mixer
            if (!BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, recordingHandle, BASSFlag.BASS_DEFAULT)) // If unable to add to mixer
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
        }

        private void ChangeSource(Sources source)
        {
            if (source == Sources.STUDIO)
            {
                currentSource = Sources.STUDIO;
            }
        }
        #endregion
        #endregion
    }
}