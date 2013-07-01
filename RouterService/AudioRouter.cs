using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.BassWasapi;

namespace RouterService
{
    class AudioRouter
    {
        #region Private Fields
        /// <summary>
        /// BASS WASAPI instance for output
        /// </summary>
        private BassWasapiHandler bassWasapi;

        /// <summary>
        /// Handle for the audio mixer
        /// </summary>
        private int mixerHandle;

        /// <summary>
        /// The router inputs
        /// </summary>
        private List<IInput> inputs = new List<IInput>();
        #endregion

        #region Properties
        public List<IInput> Inputs
        {
            get
            {
                return inputs;
            }
        }
        #endregion

        #region Constructor and Destructor
        public AudioRouter()
        {
            // Registration code for Bass.Net
            // Provided value should be used only for ShockRouter, not derived products
            BassNet.Registration("orry@orryverducci.co.uk", "2X24373423243720");
            // Load BASS libraries
            Bass.LoadMe("Bass");
            BassMix.LoadMe("Bass");
            BassWasapi.LoadMe("Bass");
            // Initialise BASS
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0); // Not playing anything via BASS, so don't need an update thread
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_VISTA_TRUEPOS, 0); // Use less precise position to reduce latency
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero); // Setup BASS with no sound output
            try
            {
                bassWasapi = new BassWasapiHandler(0, true, 44100, 2, 0, 0);
            }
            catch (ArgumentException)
            {
                throw new ApplicationException("Unable to initialise device");
            }
            bassWasapi.Init();
            // Start output
            bassWasapi.Start();
            // Create Mixer
            mixerHandle = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_STREAM_DECODE); // Create mixer
            if (mixerHandle == 0) // If unable to initialise mixer
            {
                // Throw exception
                throw new ApplicationException("Unable to create audio mixer - " + Bass.BASS_ErrorGetCode().ToString());
            }
            bassWasapi.AddOutputSource(mixerHandle, BASSFlag.BASS_DEFAULT); // Add mixer to output
        }

        /// <summary>
        /// Shuts down audio inputs and outputs
        /// </summary>
        ~AudioRouter()
        {
            Bass.FreeMe(); // Free BASS
        }
        #endregion

        #region Inputs
        /// <summary>
        /// Adds the specified input to the available inputs
        /// </summary>
        /// <param name="name">Name of the input</param>
        /// <param name="source">The source ID or address to add</param>
        public void AddInput(string name, string source)
        {
            // Create input
            IInput input = new DeviceInput();
            // Set source
            input.Name = name;
            input.Source = source;
            // Start input
            input.Start();
            if (input.OutputChannel != 0) // If input initialised successfully
            {
                // Get output handle
                int outputChannel = input.OutputChannel;
                // Add input to list of inputs
                inputs.Add(input);
                // Add input to mixer
                if (!BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, outputChannel, BASSFlag.BASS_DEFAULT)) // If unable to add to mixer
                {
                    Logger.WriteLogEntry("Unable to add input to mixer - " + Bass.BASS_ErrorGetCode().ToString(), EventLogEntryType.Error);
                }
            }
        }

        /// <summary>
        /// Get the currently available input devices
        /// </summary>
        /// <returns>A list of BASS_WASAPI_DEVICEINFO for each available device</returns>
        public List<DeviceInfo> GetInputs()
        {
            // Setup list
            List<DeviceInfo> inputDevices = new List<DeviceInfo>();
            // Retrieve devices
            bool continueLoop = true;
            for (int i = 0; continueLoop; i++)
            {
                try
                {
                    BASS_WASAPI_DEVICEINFO device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                    if (device.IsInput && !device.IsLoopback && device.IsEnabled)
                    {
                        DeviceInfo deviceInfo = new DeviceInfo();
                        deviceInfo.Name = device.name;
                        deviceInfo.ID = i;
                        inputDevices.Add(deviceInfo);
                    }
                }
                catch (Exception)
                {
                    continueLoop = false;
                }
            }
            // Output message if there is no available devices
            if (inputDevices.Count == 0)
            {
                Logger.WriteLogEntry("No input devices are currently available", EventLogEntryType.Information);
            }
            // Return devices
            return inputDevices;
        }

        /// <summary>
        /// Edit the specified input
        /// </summary>
        /// <param name="name">New name of the input</param>
        /// <param name="id">The ID of the input to edit</param>
        public void EditInput(string name, int id)
        {
            IInput input = inputs.Find(specifiedInput => specifiedInput.OutputChannel == id);
            if (input != null)
            {
                input.Name = name;
            }
        }

        /// <summary>
        /// Delete the specified input
        /// </summary>
        /// <param name="id">The ID of the input to delete</param>
        public void DeleteInput(int id)
        {
            IInput input = inputs.Find(specifiedInput => specifiedInput.OutputChannel == id);
            if (input != null)
            {
                input.Stop();
                inputs.Remove(input);
            }
        }
        #endregion
    }
}
