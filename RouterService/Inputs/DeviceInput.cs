using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace RouterService
{
    class DeviceInput : IInput
    {
        #region Properties
        public string Name { get; set; }
        public string Source { get; set; }
        public int OutputChannel { get; protected set; }
        #endregion
        
        #region Private Fields
        private BassWasapiHandler bassWasapi;
        #endregion

        public void Start()
        {
            bassWasapi = new BassWasapiHandler(Int32.Parse(Source), true, 44100, 2, 0, 0);
            bassWasapi.Init();
            bassWasapi.Start();
            bassWasapi.SetFullDuplex(0, BASSFlag.BASS_STREAM_DECODE, false);
            OutputChannel = bassWasapi.OutputChannel;
        }
    }
}
