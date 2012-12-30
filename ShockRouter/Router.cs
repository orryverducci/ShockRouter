using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;

namespace ShockRouter
{
    class Router
    {
        #region Private Fields
        /// <summary>
        /// The currently active source
        /// </summary>
        private Sources currentSource = Sources.NONE;
        /// <summary>
        /// Handle of the recording stream
        /// </summary>
        private int recordingHandle;
        /// <summary>
        /// Handle of the chart show stream
        /// </summary>
        private int chartHandle;
        /// <summary>
        /// Handle of the emergency file stream
        /// </summary>
        private int emergencyHandle;
        /// <summary>
        /// Handle of the audio mixer
        /// </summary>
        private int mixerHandle;
        /// <summary>
        /// Peak level meter
        /// </summary>
        private DSP_PeakLevelMeter peakLevelMeter;
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

        /// <summary>
        /// Gets or set the current studio input device
        /// </summary>
        public int InputDevice
        {
            get
            {
                return Bass.BASS_RecordGetDevice();
            }
            set
            {
                InitaliseLineIn(value);
            }
        }

        /// <summary>
        /// Gets or sets the file to be played for Emergency Output
        /// </summary>
        public string EmergencyFile { get; set; }

        /// <summary>
        /// Gets or sets the URL of the chart show
        /// </summary>
        public string ChartURL { get; set; }
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
            EMERGENCY,
            /// <summary>
            /// Not initialised
            /// </summary>
            NONE
        }
        #endregion

        #region Events
        /// <summary>
        /// Signals that the source has been changed
        /// </summary>
        public event EventHandler SourceChanged;
        /// <summary>
        /// Audio level event arguments
        /// </summary>
        public class LevelEventArgs : EventArgs
        {
            /// <summary>
            /// Left audio level in db
            /// </summary>
            public double LeftLevel { get; set; }
            /// <summary>
            /// Right audio level in db
            /// </summary>
            public double RightLevel { get; set; }
        }
        /// <summary>
        /// Event handler for audio level events
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Audio level event arguments</param>
        public delegate void LevelEventHandler(object sender, LevelEventArgs e);
        /// <summary>
        /// Event to update peak level meters
        /// </summary>
        public event LevelEventHandler PeakLevelMeterUpdate;
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
            // Add Input Peak Level Meter DSP and Event Handler
            peakLevelMeter = new DSP_PeakLevelMeter(mixerHandle, 1);
            peakLevelMeter.Notification += new EventHandler(PeakLevelMeterNotification);
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

        /// <summary>
        /// Change the current source to the requested source
        /// </summary>
        /// <param name="source">Source to change to</param>
        private void ChangeSource(Sources source)
        {
            if (source != Source) // If selected source is not already chosen
            {
                // Make note of current source
                Sources previousSource = Source;
                // Start new source
                switch (source)
                {
                    case Sources.STUDIO:
                        StartStudio();
                        break;
                    case Sources.SRA:
                        StartChart();
                        break;
                    case Sources.EMERGENCY:
                        StartEmergency();
                        break;
                }
                // Stop old source
                switch (previousSource)
                {
                    case Sources.STUDIO:
                        StopStudio();
                        break;
                    case Sources.SRA:
                        StopChart();
                        break;
                    case Sources.EMERGENCY:
                        StopEmergency();
                        break;
                }
                // Send event showing source has been changed
                currentSource = source;
                if (SourceChanged != null)
                {
                    SourceChanged(this, new EventArgs());
                }
            }
        }

        #region Studio Source
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
                Bass.BASS_RecordFree(); // Free recording resources
                recordingHandle = default(int); // Set handle to default value
            }
            if (!Bass.BASS_RecordInit(deviceID)) // If unable to initialise input device
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
            // Start recording
            recordingHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT, null, IntPtr.Zero);
            // Set volume to 0 if not current source
            if (Source != Sources.STUDIO)
            {
                Bass.BASS_ChannelSetAttribute(recordingHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);
            }
            // Add to mixer
            if (!BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, recordingHandle, BASSFlag.BASS_DEFAULT)) // If unable to add to mixer
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
        }

        /// <summary>
        /// Retrieves a list of the available audio devices
        /// </summary>
        /// <returns>List of device names, in order of their ID, starting at 1</returns>
        public List<string> GetDevices()
        {
            // Create list
            List<string> devices = new List<string>();
            // Get information for each device and add to list
            foreach(BASS_DEVICEINFO device in Bass.BASS_RecordGetDeviceInfos())
            {
                devices.Add(device.name);
            }
            // Return list of devices
            return devices;
        }

        /// <summary>
        /// Starts playback of the studio input
        /// </summary>
        private void StartStudio()
        {
            // Fade up studio input
            Bass.BASS_ChannelSlideAttribute(recordingHandle, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
        }

        /// <summary>
        /// Stops playback of the studio input
        /// </summary>
        private void StopStudio()
        {
            // Fade down studio input
            Bass.BASS_ChannelSlideAttribute(recordingHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }
        #endregion

        #region Chart Source
        /// <summary>
        /// Starts playback of the emergency file
        /// </summary>
        private void StartChart()
        {
            if (ChartURL != default(string)) // If a file has been set
            {
                // Create stream
                chartHandle = Bass.BASS_StreamCreateURL(ChartURL, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null, IntPtr.Zero);
                // Set to mute
                Bass.BASS_ChannelSetAttribute(chartHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);
                // Start playing from start
                Bass.BASS_ChannelPlay(chartHandle, true);
                // Add to mixer
                BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, chartHandle, BASSFlag.BASS_DEFAULT);
                // Fade up
                Bass.BASS_ChannelSlideAttribute(chartHandle, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
            }
            else // Otherwise throw exception saying file hasn't been set
            {
                throw new ApplicationException("No chart URL is set");
            }
        }

        /// <summary>
        /// Stops playback of the emergency file
        /// </summary>
        private void StopChart()
        {
            // Set sync function for fade down
            SYNCPROC _mySync = new SYNCPROC(delegate(int handle, int channel, int data, IntPtr user)
            {
                // Remove from mixer
                BassMix.BASS_Mixer_ChannelRemove(chartHandle);
                // Stop playing
                Bass.BASS_ChannelStop(chartHandle);
                // Free stream
                Bass.BASS_StreamFree(chartHandle);
            });
            Bass.BASS_ChannelSetSync(chartHandle, BASSSync.BASS_SYNC_SLIDE, 0, _mySync, IntPtr.Zero);
            // Fade down
            Bass.BASS_ChannelSlideAttribute(chartHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }
        #endregion

        #region Emergency Source
        /// <summary>
        /// Starts playback of the emergency file
        /// </summary>
        private void StartEmergency()
        {
            if (EmergencyFile != default(string)) // If a file has been set
            {
                // Create file stream
                emergencyHandle = Bass.BASS_StreamCreateFile(EmergencyFile, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_LOOP);
                // Set to mute
                Bass.BASS_ChannelSetAttribute(emergencyHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);
                // Start playing from start
                Bass.BASS_ChannelPlay(emergencyHandle, true);
                // Add to mixer
                BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, emergencyHandle, BASSFlag.BASS_DEFAULT);
                // Fade up
                Bass.BASS_ChannelSlideAttribute(emergencyHandle, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
            }
            else // Otherwise throw exception saying file hasn't been set
            {
                throw new ApplicationException("No emergency file is set");
            }
        }

        /// <summary>
        /// Stops playback of the emergency file
        /// </summary>
        private void StopEmergency()
        {
            // Set sync function for fade down
            SYNCPROC _mySync = new SYNCPROC(delegate(int handle, int channel, int data, IntPtr user) {
                // Remove from mixer
                BassMix.BASS_Mixer_ChannelRemove(emergencyHandle);
                // Stop playing
                Bass.BASS_ChannelStop(emergencyHandle);
                // Free stream
                Bass.BASS_StreamFree(emergencyHandle);
            });
            Bass.BASS_ChannelSetSync(emergencyHandle, BASSSync.BASS_SYNC_SLIDE, 0, _mySync, IntPtr.Zero);
            // Fade down
            Bass.BASS_ChannelSlideAttribute(emergencyHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }
        #endregion

        /// <summary>
        /// Triggers event sending peak level meter values
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event argument</param>
        private void PeakLevelMeterNotification(object sender, EventArgs e)
        {
            if (PeakLevelMeterUpdate != null)
            {
                // Create event args containing levels
                LevelEventArgs levelEvent = new LevelEventArgs();
                levelEvent.LeftLevel = peakLevelMeter.LevelL_dBV;
                levelEvent.RightLevel = peakLevelMeter.LevelR_dBV;
                // Trigger event
                PeakLevelMeterUpdate(null, levelEvent);
            }
        }
        #endregion
    }
}