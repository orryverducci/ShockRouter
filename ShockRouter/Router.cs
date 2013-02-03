using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.WaDsp;
using Un4seen.Bass.Misc;

namespace ShockRouter
{
    class Router
    {
        #region Private Fields
        /// <summary>
        /// The currently active source
        /// </summary>
        private Sources currentSource = Sources.NONE; // Initialised to no source

        /// <summary>
        /// Handle of the recording stream
        /// </summary>
        private int recordingHandle;

        /// <summary>
        /// Handle of the chart show stream
        /// </summary>
        private int chartHandle;

        /// <summary>
        /// Handle of the outside broadcast stream
        /// </summary>
        private int obHandle;

        /// <summary>
        /// Handle of the emergency file stream
        /// </summary>
        private int emergencyHandle;

        /// <summary>
        /// Handle of the audio mixer
        /// </summary>
        private int mixerHandle;

        /// <summary>
        /// Array of available DSP filenames
        /// </summary>
        private string[] availableDSPs;

        /// <summary>
        /// Handle of the Winamp DSP
        /// </summary>
        private int dspHandle;

        /// <summary>
        /// Source peak level meter
        /// </summary>
        private DSP_PeakLevelMeter sourceLevelMeter;

        /// <summary>
        /// Output peak level meter
        /// </summary>
        private DSP_PeakLevelMeter outputLevelMeter;

        /// <summary>
        /// True if the stream is currently silent
        /// </summary>
        private bool currentlySilent = false;

        /// <summary>
        /// Time the source first went silent
        /// </summary>
        private DateTime silentSince;
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
        /// Gets or sets the URL of the chart show
        /// </summary>
        public string ChartURL { get; set; }

        /// <summary>
        /// Gets or sets the URL for outside broadcasts
        /// </summary>
        public string ObURL { get; set; }

        /// <summary>
        /// Gets or sets the file to be played for Emergency Output
        /// </summary>
        public string EmergencyFile { get; set; }

        /// <summary>
        /// Gets or sets the time the source can be silent for before it is switched to Emergency Output
        /// </summary>
        public int SilenceDetectorTime { get; set; }

        /// <summary>
        /// Sets the processor from one of the available ones, with index starting at 1. An index of 0 sets no processor
        /// </summary>
        public int Processor
        {
            set
            {
                InitialiseDSP(value);
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
        /// Event to update source level meters
        /// </summary>
        public event LevelEventHandler SourceLevelMeterUpdate;

        /// <summary>
        /// Event to update output level meters
        /// </summary>
        public event LevelEventHandler OutputLevelMeterUpdate;
        #endregion

        #region Methods
        #region Constructor and destructor
        /// <summary>
        /// Initialises the audio inputs and outputs
        /// </summary>
        /// <param name="mainWindowHandle">Handle of the main user interface window</param>
        public Router(IntPtr mainWindowHandle)
        {
            // Registration code for Bass.Net
            // Provided value should be used only for ShockRouter, not derived products
            BassNet.Registration("orry@orryverducci.co.uk", "2X24373423243720");
            // Load BASS libraries
            Bass.LoadMe();
            BassMix.LoadMe();
            BassWaDsp.LoadMe();
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
            // Initalise Winamp DSP support
            BassWaDsp.BASS_WADSP_Init(mainWindowHandle);
            // Find Winamp DSPs
            if (Directory.Exists(Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files (x86)")) // If on 64 bit Windows
            {
                if (Directory.Exists(Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files (x86)\\Winamp\\Plugins"))
                {
                    WINAMP_DSP.FindPlugins(Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files (x86)\\Winamp\\Plugins");
                }
            }
            else // If on 32 bit Windows
            {
                if (Directory.Exists(Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files\\Winamp\\Plugins"))
                {
                    WINAMP_DSP.FindPlugins(Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files\\Winamp\\Plugins");
                }
            }
            // Create list of Winamp DSPs
            if (WINAMP_DSP.PlugIns.Count > 0) // If there is plugins available
            {
                availableDSPs = new string[WINAMP_DSP.PlugIns.Count]; // Set size of array
                // Get information for each DSP and add to list
                for (int i = 0; i < WINAMP_DSP.PlugIns.Count; i++)
                {
                    availableDSPs[i] = WINAMP_DSP.PlugIns[i].File;
                }
            }
            // Initalise Line In
            InitaliseLineIn();
            // Add source peak level meter DSP and event handler
            sourceLevelMeter = new DSP_PeakLevelMeter(mixerHandle, 3);
            sourceLevelMeter.Notification += new EventHandler(SourceLevelMeterNotification);
            // Add output peak level meter DSP and event handler
            outputLevelMeter = new DSP_PeakLevelMeter(mixerHandle, 1);
            outputLevelMeter.Notification += new EventHandler(OutputLevelMeterNotification);
        }

        /// <summary>
        /// Shuts down audio inputs and outputs
        /// </summary>
        ~Router()
        {
            FreeDSP(); // Stops running DSP
            BassWaDsp.BASS_WADSP_Free(); // Free DSP resources
            BassWaDsp.FreeMe(); // Free BASS Winamp DSP
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
                    case Sources.OB:
                        StartOB();
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
                    case Sources.OB:
                        StopOB();
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
                if (!Bass.BASS_RecordInit(-1)) // Try default device
                {
                    // If that fails throw exception
                    throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
                }
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
        /// Starts playback of the chart show stream
        /// </summary>
        private void StartChart()
        {
            if (ChartURL != default(string)) // If a URL has been set
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
        /// Stops playback of the chart show stream
        /// </summary>
        private void StopChart()
        {
            // Set sync function for fade down
            SYNCPROC streamEndSync = new SYNCPROC(RemoveChartStream);
            Bass.BASS_ChannelSetSync(chartHandle, BASSSync.BASS_SYNC_SLIDE, 0, streamEndSync, IntPtr.Zero);
            // Fade down
            Bass.BASS_ChannelSlideAttribute(chartHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }

        /// <summary>
        /// Removes the chart show stream from the mixer
        /// </summary>
        /// <param name="handle">Handle for the sync</param>
        /// <param name="channel">Handle for the channel</param>
        /// <param name="data">Data associated with the sync</param>
        /// <param name="user">User instance handle</param>
        private void RemoveChartStream(int handle, int channel, int data, IntPtr user)
        {
            // Remove from mixer
            BassMix.BASS_Mixer_ChannelRemove(chartHandle);
            // Stop playing
            Bass.BASS_ChannelStop(chartHandle);
            // Free stream
            Bass.BASS_StreamFree(chartHandle);
        }
        #endregion

        #region Outside Broadcast Source
        /// <summary>
        /// Starts playback of the outside broadcast stream
        /// </summary>
        private void StartOB()
        {
            if (ObURL != default(string)) // If a URL has been set
            {
                // Create stream
                obHandle = Bass.BASS_StreamCreateURL(ObURL, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null, IntPtr.Zero);
                // Set to mute
                Bass.BASS_ChannelSetAttribute(obHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);
                // Start playing from start
                Bass.BASS_ChannelPlay(obHandle, true);
                // Add to mixer
                BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, obHandle, BASSFlag.BASS_DEFAULT);
                // Fade up
                Bass.BASS_ChannelSlideAttribute(obHandle, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
            }
            else // Otherwise throw exception saying file hasn't been set
            {
                throw new ApplicationException("No outside broadcast URL is set");
            }
        }

        /// <summary>
        /// Stops playback of the outside broadcast stream
        /// </summary>
        private void StopOB()
        {
            // Set sync function for fade down
            SYNCPROC streamEndSync = new SYNCPROC(RemoveOBStream);
            Bass.BASS_ChannelSetSync(obHandle, BASSSync.BASS_SYNC_SLIDE, 0, streamEndSync, IntPtr.Zero);
            // Fade down
            Bass.BASS_ChannelSlideAttribute(obHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }

        /// <summary>
        /// Removes the outside broadcast stream from the mixer
        /// </summary>
        /// <param name="handle">Handle for the sync</param>
        /// <param name="channel">Handle for the channel</param>
        /// <param name="data">Data associated with the sync</param>
        /// <param name="user">User instance handle</param>
        private void RemoveOBStream(int handle, int channel, int data, IntPtr user)
        {
            // Remove from mixer
            BassMix.BASS_Mixer_ChannelRemove(obHandle);
            // Stop playing
            Bass.BASS_ChannelStop(obHandle);
            // Free stream
            Bass.BASS_StreamFree(obHandle);
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
            SYNCPROC streamEndSync = new SYNCPROC(RemoveEmergencyStream);
            Bass.BASS_ChannelSetSync(emergencyHandle, BASSSync.BASS_SYNC_SLIDE, 0, streamEndSync, IntPtr.Zero);
            // Fade down
            Bass.BASS_ChannelSlideAttribute(emergencyHandle, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
        }

        /// <summary>
        /// Removes the emergency stream from the mixer
        /// </summary>
        /// <param name="handle">Handle for the sync</param>
        /// <param name="channel">Handle for the channel</param>
        /// <param name="data">Data associated with the sync</param>
        /// <param name="user">User instance handle</param>
        private void RemoveEmergencyStream(int handle, int channel, int data, IntPtr user)
        {
            // Remove from mixer
            BassMix.BASS_Mixer_ChannelRemove(emergencyHandle);
            // Stop playing
            Bass.BASS_ChannelStop(emergencyHandle);
            // Free stream
            Bass.BASS_StreamFree(emergencyHandle);
        }
        #endregion

        #region Digital Signal Processing
        /// <summary>
        /// Retrieves a list of the available DSPs
        /// </summary>
        /// <returns>List of DSPs, in order of their ID, starting at 1</returns>
        public List<string> GetDSPs()
        {
            // Create list
            List<string> dsps = new List<string>();
            // Return plugins
            if (availableDSPs != null)
            {
                for (int i = 0; i < availableDSPs.Count(); i++)
                {
                    dsps.Add(WINAMP_DSP.PlugIns[i].Description);
                }
            }
            // Return list of devices
            return dsps;
        }

        /// <summary>
        /// Sets the processing DSP to the specified index
        /// </summary>
        /// <param name="dspIndex">Index of the DSP to be loaded</param>
        private void InitialiseDSP(int dspIndex)
        {
            // Free DSP running, if any
            FreeDSP();
            // Load DSP
            if (dspIndex > 0) // If a DSP is chosen
            {
                dspHandle = BassWaDsp.BASS_WADSP_Load(availableDSPs[dspIndex - 1], 5, 5, 100, 100, null);
                BassWaDsp.BASS_WADSP_Start(dspHandle, 0, mixerHandle);
                BassWaDsp.BASS_WADSP_ChannelSetDSP(dspHandle, mixerHandle, 2);
            }
        }

        /// <summary>
        /// Opens the configuration window for the DSP
        /// </summary>
        /// <param name="dspName"></param>
        public void ConfigureDSP(string dspName)
        {
            BassWaDsp.BASS_WADSP_Config(dspHandle);
        }

        /// <summary>
        /// Stops the DSP
        /// </summary>
        private void FreeDSP()
        {
            if (dspHandle != default(int)) // If a DSP is running
            {
                BassWaDsp.BASS_WADSP_Stop(dspHandle);
                BassWaDsp.BASS_WADSP_FreeDSP(dspHandle);
                dspHandle = default(int);
            }
        }
        #endregion

        #region Levels Meters Update and Silence Detection
        /// <summary>
        /// Triggers event sending source level meter values, and monitors for periods of silence
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event argument</param>
        private void SourceLevelMeterNotification(object sender, EventArgs e)
        {
            // Update level meter
            if (SourceLevelMeterUpdate != null)
            {
                // Create event args containing levels
                LevelEventArgs levelEvent = new LevelEventArgs();
                levelEvent.LeftLevel = sourceLevelMeter.LevelL_dBV;
                levelEvent.RightLevel = sourceLevelMeter.LevelR_dBV;
                // Trigger event
                SourceLevelMeterUpdate(null, levelEvent);
            }
            // If silent and not already on emergency output, carry out silence detection tasks
            if (Source != Sources.EMERGENCY && sourceLevelMeter.LevelL_dBV <= -40 && sourceLevelMeter.LevelL_dBV <= -40)
            {
                if (currentlySilent) // If already noted as being silent
                {
                    // If silent longer than set silence detection period, switch to emergency output
                    if (silentSince.AddSeconds(SilenceDetectorTime) <= DateTime.Now)
                    {
                        ChangeSource(Sources.EMERGENCY);
                        currentlySilent = false;
                    }
                }
                else // Else make note that the stream is silent
                {
                    currentlySilent = true;
                    silentSince = DateTime.Now;
                }
            }
            else // Else stream is not silent
            {
                currentlySilent = false;
            }
        }

        /// <summary>
        /// Triggers event sending output level meter values
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event argument</param>
        private void OutputLevelMeterNotification(object sender, EventArgs e)
        {
            if (OutputLevelMeterUpdate != null)
            {
                // Create event args containing levels
                LevelEventArgs levelEvent = new LevelEventArgs();
                levelEvent.LeftLevel = outputLevelMeter.LevelL_dBV;
                levelEvent.RightLevel = outputLevelMeter.LevelR_dBV;
                // Trigger event
                OutputLevelMeterUpdate(null, levelEvent);
            }
        }
        #endregion
        #endregion
    }
}