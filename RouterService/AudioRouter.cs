using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace RouterService
{
    class AudioRouter
    {
        #region Private Fields
        /// <summary>
        /// BASS WASAPI instance for output
        /// </summary>
        BassWasapiHandler bassWasapi;
        #endregion

        #region Constructor and Destructor
        public AudioRouter()
        {
            // Registration code for Bass.Net
            // Provided value should be used only for ShockRouter, not derived products
            BassNet.Registration("orry@orryverducci.co.uk", "2X24373423243720");
            // Load BASS libraries
            Bass.LoadMe("Bass");
            BassWasapi.LoadMe("Bass");
            // Initialise BASS
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0); // Not playing anything via BASS, so don't need an update thread
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero); // Setup BASS with no sound output
            bassWasapi = new BassWasapiHandler(-1, true, 44100, 2, 0, 0);
            bassWasapi.Init();
        }

        /// <summary>
        /// Shuts down audio inputs and outputs
        /// </summary>
        ~AudioRouter()
        {
            Bass.FreeMe(); // Free BASS
        }
        #endregion
    }
}
