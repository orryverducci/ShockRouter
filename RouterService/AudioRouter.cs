using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

namespace RouterService
{
    class AudioRouter
    {
        #region Private Fields
        /// <summary>
        /// Device for uncompressed output
        /// </summary>
        private Output output;

        /// <summary>
        /// Handle for the audio mixer
        /// </summary>
        private int mixerHandle;

        /// <summary>
        /// The router inputs
        /// </summary>
        private List<IInput> inputs = new List<IInput>();

        /// <summary>
        /// The handle of the current input
        /// </summary>
        private int currentInput;
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
            // Initialise BASS
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_VISTA_TRUEPOS, 0); // Use less precise position to reduce latency
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 5);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 50);
            // Create output
            output = new Output(1);
            // Create Mixer
            mixerHandle = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_NONSTOP); // Create mixer
            if (mixerHandle == default(int)) // If unable to initialise mixer
            {
                // Throw exception
                throw new ApplicationException("Unable to create audio mixer - " + Bass.BASS_ErrorGetCode().ToString());
            }
            // Start output
            try
            {
                output.Start(mixerHandle);
            }
            catch (ApplicationException e)
            {
                Logger.WriteLogEntry("Unable to start output: " + e.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Shuts down audio inputs, outputs and networking
        /// </summary>
        public void Shutdown()
        {
            output.Stop();
            // Free BASS
            BassMix.FreeMe();
            Bass.FreeMe();
            // Stop NetworkComms.Net
            NetworkComms.Shutdown();
        }
        #endregion

        #region Inputs
        public List<IInput> Inputs
        {
            get
            {
                return inputs;
            }
        }

        /// <summary>
        /// The ID of the current input
        /// </summary>
        public int CurrentInput
        {
            get
            {
                return currentInput;
            }
            set
            {
                if (value != default(int)) // If input is not being deleted
                {
                    // Find new input and tell it that it has been put on air
                    IInput input = inputs.Find(specifiedInput => specifiedInput.OutputChannel == value);
                    input.PutOnAir();
                    // Fade out old input
                    Bass.BASS_ChannelSlideAttribute(currentInput, BASSAttribute.BASS_ATTRIB_VOL, 0, 500);
                    // Fade in new input
                    Bass.BASS_ChannelSlideAttribute(value, BASSAttribute.BASS_ATTRIB_VOL, 1, 500);
                    
                }
                currentInput = value;
                SendChangeToClocks();
            }
        }

        /// <summary>
        /// Adds the specified input device to the available inputs
        /// </summary>
        /// <param name="name">Name of the input</param>
        /// <param name="source">The source ID or address to add</param>
        /// <param name="studio">The studio number to add</param>
        public void AddInputDevice(string name, string source, int studio)
        {
            IInput input = new DeviceInput();
            AddInput(input, name, source, studio);
        }

        /// <summary>
        /// Adds the specified input file to the available inputs
        /// </summary>
        /// <param name="name">Name of the input</param>
        /// <param name="source">The source ID to add</param>
        public void AddInputFile(string name, string source)
        {
            IInput input = new FileInput();
            AddInput(input, name, source, 0);
        }

        /// <summary>
        /// Adds the specified input stream to the available inputs
        /// </summary>
        /// <param name="name">Name of the input</param>
        /// <param name="source">The stream URL to add</param>
        public void AddInputStream(string name, string source)
        {
            IInput input = new StreamInput();
            AddInput(input, name, source, 0);
        }

        /// <summary>
        /// Adds the specified input to the available inputs
        /// </summary>
        /// <param name="input">The input device object</param>
        /// <param name="name">Name of the input</param>
        /// <param name="source">The source ID or address to add</param>
        /// <param name="studio">The studio number to add</param>
        private void AddInput(IInput input, string name, string source, int studio)
        {
            // Set source
            input.Name = name;
            input.StudioNumber = studio;
            // Start input
            try
            {
                input.Start(source);
                // Get output handle
                int outputChannel = input.OutputChannel;
                // Mute input
                Bass.BASS_ChannelSetAttribute(outputChannel, BASSAttribute.BASS_ATTRIB_VOL, 0);
                // Add input to list of inputs
                inputs.Add(input);
                // Add input to mixer
                if (!BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, outputChannel, BASSFlag.BASS_STREAM_AUTOFREE)) // If unable to add to mixer
                {
                    Logger.WriteLogEntry("Unable to add input to mixer - " + Bass.BASS_ErrorGetCode().ToString(), EventLogEntryType.Error);
                }
                // Set as current input if it is the first one
                if (CurrentInput == default(int))
                {
                    CurrentInput = input.OutputChannel;
                }
            }
            catch (Exception e)
            {
                Logger.WriteLogEntry("Unable to add input: " + e.Message, EventLogEntryType.Error);
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
                    BASS_DEVICEINFO device = Bass.BASS_RecordGetDeviceInfo(i);
                    if (device.IsEnabled)
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
        /// <param name="studio">The studio number of the input to edit</param>
        public void EditInput(string name, int id, int studio)
        {
            IInput input = inputs.Find(specifiedInput => specifiedInput.OutputChannel == id);
            if (input != null)
            {
                input.Name = name;
                input.StudioNumber = studio;
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
                if (input.OutputChannel == CurrentInput) // If the input being deleted is the current input, set current input to nothing
                {
                    CurrentInput = default(int);
                }
                BassMix.BASS_Mixer_ChannelRemove(input.OutputChannel);
                input.Stop();
                inputs.Remove(input);
            }
        }
        #endregion

        #region Outputs
        /// <summary>
        /// Get the currently available output devices
        /// </summary>
        /// <returns>A list of DeviceInfo for each available device</returns>
        public List<DeviceInfo> GetOutputs()
        {
            // Setup list
            List<DeviceInfo> outputDevices = new List<DeviceInfo>();
            // Retrieve devices
            bool continueLoop = true;
            for (int i = 0; continueLoop; i++)
            {
                try
                {
                    BASS_DEVICEINFO device = Bass.BASS_GetDeviceInfo(i);
                    if (device.IsEnabled)
                    {
                        DeviceInfo deviceInfo = new DeviceInfo();
                        deviceInfo.Name = device.name;
                        deviceInfo.ID = i;
                        outputDevices.Add(deviceInfo);
                    }
                }
                catch (Exception)
                {
                    continueLoop = false;
                }
            }
            // Output message if there is no available devices
            if (outputDevices.Count == 0)
            {
                Logger.WriteLogEntry("No output devices are currently available", EventLogEntryType.Information);
            }
            // Return devices
            return outputDevices;
        }

        /// <summary>
        /// The ID of the current output device
        /// </summary>
        public int CurrentOutput
        {
            get
            {
                return output.DeviceID;
            }
        }

        /// <summary>
        /// Change the uncompressd output device
        /// </summary>
        /// <param name="id">ID of the output device to use</param>
        public void ChangeOutput(int id)
        {
            output.ChangeOutput(id);
        }
        #endregion

        #region Networking
        public string ClockIP { get; set; }

        private void SendChangeToClocks()
        {
            // Find input
            IInput input = inputs.Find(specifiedInput => specifiedInput.OutputChannel == CurrentInput);
            // Get studio number from input
            int studioNumber;
            if (input != null) // If the input was found, retrieve input studio number
            {
                studioNumber = input.StudioNumber;
            }
            else // If input was not found (input has been deleted), return studio number as 0
            {
                studioNumber = 0;
            }
            // Send to clocks
            if (ClockIP != default(string)) // Send if an IP is set
            {
                try
                {
                    NetworkComms.SendObject("Message", ClockIP, 10000, "STUDIO - " + studioNumber.ToString());
                }
                catch { } // Do nothing on exception
            }
        }
        #endregion
    }
}