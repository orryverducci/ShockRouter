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
            if (!BassWasapi.BASS_WASAPI_Init(-1, 44100, 2, BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 0, 0, null, IntPtr.Zero)) // If unable to initialise audio output
            {
                // Throw exception
                throw new ApplicationException(Bass.BASS_ErrorGetCode().ToString());
            }
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
