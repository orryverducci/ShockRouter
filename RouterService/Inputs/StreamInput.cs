using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

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

        private int streamChannel;
        private Timer timer = new Timer(10000);
        private SYNCPROC endProc;

        public void Start(string source)
        {
            if (source != default(string)) // If a URL has been set
            {
                endProc = new SYNCPROC(EndOfStream);
                Source = source;
                timer.Elapsed += timer_Elapsed;
                OutputChannel = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_MIXER_NONSTOP | BASSFlag.BASS_STREAM_DECODE);
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                    {
                        CreateStream();
                    });
                backgroundWorker.RunWorkerAsync();
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
            streamChannel = Bass.BASS_StreamCreateURL(Source, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null, IntPtr.Zero);
            if (streamChannel == default(int)) // If does not create successfully
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                if (error != BASSError.BASS_ERROR_FILEOPEN && error != BASSError.BASS_ERROR_TIMEOUT)
                {
                   timer.Start();
                }
                else
                {
                   Logger.WriteLogEntry("Unable to create input stream - " + error.ToString(), EventLogEntryType.Error);
                }
            }
            else
            {
                // Set end sync
                Bass.BASS_ChannelSetSync(streamChannel, BASSSync.BASS_SYNC_END, 0, endProc, IntPtr.Zero);
                // Add to mixer
                BassMix.BASS_Mixer_StreamAddChannel(OutputChannel, streamChannel, BASSFlag.BASS_STREAM_AUTOFREE);
                // Start playing
                //Bass.BASS_ChannelPlay(streamChannel, true);
            }
        }

        private void EndOfStream(int handle, int channel, int data, IntPtr user)
        {
            Bass.BASS_StreamFree(streamChannel);
            streamChannel = default(int);
            timer.Start();
        }

        public void Stop()
        {
            if (OutputChannel != default(int))
            {
                if (streamChannel != default(int))
                {
                    if (Bass.BASS_ChannelIsActive(streamChannel) != BASSActive.BASS_ACTIVE_STOPPED)
                    {
                        // Stop playing
                        Bass.BASS_ChannelStop(streamChannel);
                    }
                }
                // Clear stream
                Bass.BASS_StreamFree(streamChannel);
                Bass.BASS_StreamFree(OutputChannel);
                streamChannel = default(int);
                OutputChannel = default(int);
            }
        }

        public void PutOnAir() { }
    }
}
