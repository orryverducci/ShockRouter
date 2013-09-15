using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace RouterService
{
    class StreamInput : IInput
    {
        #region Properties
        public string Name { get; set; }
        public string Type
        {
            get
            {
                return "Stream";
            }
        }
        public string Source { get; private set; }
        public int OutputChannel { get; private set; }
        public int StudioNumber { get; set; }
        #endregion

        private Timer timer = new Timer(10000);
        private SYNCPROC endProc;

        public void Start(string source)
        {
            if (source != default(string)) // If a URL has been set
            {
                endProc = new SYNCPROC(EndOfStream);
                Source = source;
                timer.Elapsed += timer_Elapsed;
                CreateStream();
            }
            else // Otherwise throw exception saying file hasn't been set
            {
                throw new ApplicationException("Unable to create input stream - no stream URL set");
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            CreateStream();
        }

        private void CreateStream()
        {
            // Create stream
                OutputChannel = Bass.BASS_StreamCreateURL(Source, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null, IntPtr.Zero);
                if (OutputChannel == default(int)) // If does not create successfully
                {
                    BASSError error = Bass.BASS_ErrorGetCode();
                    if (error != BASSError.BASS_ERROR_FILEOPEN && error != BASSError.BASS_ERROR_TIMEOUT)
                    {
                        throw new ApplicationException("Unable to create input stream - " + error.ToString());
                    }
                    timer.Start();
                }
                else
                {
                    // Start playing
                    Bass.BASS_ChannelPlay(OutputChannel, true);
                    // Set end sync
                    Bass.BASS_ChannelSetSync(OutputChannel, BASSSync.BASS_SYNC_END, 0, endProc, IntPtr.Zero);
                }
        }

        private void EndOfStream(int handle, int channel, int data, IntPtr user)
        {
            Stop();
            timer.Start();
        }

        public void Stop()
        {
            if (OutputChannel != default(int))
            {
                if (Bass.BASS_ChannelIsActive(OutputChannel) != BASSActive.BASS_ACTIVE_STOPPED)
                {
                    // Stop playing
                    Bass.BASS_ChannelStop(OutputChannel);
                }
                // Clear stream
                Bass.BASS_StreamFree(OutputChannel);
                OutputChannel = default(int);
            }
        }

        public void PutOnAir() { }
    }
}
