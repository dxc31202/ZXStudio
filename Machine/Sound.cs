using System;
using System.IO;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace ZXStudio
{
    public class Sound : IDisposable
    {
        private int timeToOutSound;
        private float averagedSound;
        private int soundCounter;
        protected int Cycles;
        public float soundOut;
        private byte[][] bData;
        private AudioBuffer buffer;
        private const int BUFFER_COUNT = 5;
        private int bytesPerSample;
        private int currentBuffer;
        private XAudio2 device;
        private bool isMute;
        private MasteringVoice masteringVoice;
        private int playBuffer;
        public bool readyToPlay;
        private int SAMPLE_SIZE;
        private float[][] sampleData;
        private int samplePos;
        private SourceVoice sourceVoice;
        private int submittedBuffers;
        private WaveFormat waveFormat;

        public int TimeToOutSound
        {
            get { return timeToOutSound; }
            set
            {
                timeToOutSound = value;
                if (timeToOutSound >= 0x4f)
                {
                    OutSound();
                    timeToOutSound -= 0x4f;
                }

            }
        }
        bool stereoSound = true;
        public void OutSound()
        {
            averagedSound /= (float)soundCounter;
            EndSample(averagedSound);
            stereoSound = true;
            AddSample(averagedChannelSamples);
            averagedChannelSamples[0] = 0f;
            averagedChannelSamples[1] = 0f;
            averagedChannelSamples[2] = 0f;
            averagedSound = 0f;
            soundCounter = 0;
        }

        public void EndSample(float beeperSound)
        {
            if (stereoSound)
            {
                averagedChannelSamples[0] = (averagedChannelSamples[ChannelLeft] + averagedChannelSamples[ChannelCenter]) / ((float)soundSampleCounter);
                averagedChannelSamples[1] = (averagedChannelSamples[ChannelRight] + averagedChannelSamples[ChannelCenter]) / ((float)soundSampleCounter);
                averagedChannelSamples[0] = beeperSound;
            }
            else
            {
                averagedChannelSamples[0] = ((averagedChannelSamples[ChannelLeft] + averagedChannelSamples[ChannelCenter]) + averagedChannelSamples[ChannelRight]) / ((float)soundSampleCounter);
                averagedChannelSamples[1] = ((averagedChannelSamples[ChannelLeft] + averagedChannelSamples[ChannelCenter]) + averagedChannelSamples[ChannelRight]) / ((float)soundSampleCounter);
                averagedChannelSamples[0] = (((averagedChannelSamples[ChannelLeft] + averagedChannelSamples[ChannelCenter]) + averagedChannelSamples[ChannelRight]) / ((float)soundSampleCounter)) + beeperSound;
            }
            soundSampleCounter = 0;
        }
        private float[] channel_mix = new float[3];
        int soundSampleCounter;
        public float[] averagedChannelSamples = new float[3];
        private int ChannelCenter = 2;
        private int ChannelLeft = 1;
        private int ChannelRight = 1;

        public void UpdateAudio(int dt)
        {
            TimeToOutSound += dt;
            Cycles += dt;
            while (Cycles > 0x10)
            {
                Cycles -= 0x10;
                averagedChannelSamples[0] += channel_mix[0];
                averagedChannelSamples[1] += channel_mix[1];
                averagedChannelSamples[2] += channel_mix[2];
                soundSampleCounter++;
            }
            averagedSound += soundOut;
            soundCounter++;
        }

        public Sound(IntPtr handle, short BitsPerSample, short Channels, int SamplesPerSecond)
        {
            WaveFormat format;
            SAMPLE_SIZE = 0x372;
            sampleData = new float[5][];
            bData = new byte[5][];

            format = new WaveFormat();
            format.BitsPerSample = BitsPerSample;
            format.Channels = Channels;
            format.SamplesPerSecond = SamplesPerSecond;
            format.BlockAlignment = (short)((format.Channels * format.BitsPerSample) / 8);
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlignment;
            format.FormatTag = WaveFormatTag.IeeeFloat;

            device = new SlimDX.XAudio2.XAudio2(XAudio2Flags.None, ProcessorSpecifier.DefaultProcessor);
            device.StartEngine();
            masteringVoice = new MasteringVoice(device, Channels, SamplesPerSecond);
            sourceVoice = new SourceVoice(device, format, VoiceFlags.None);
            sourceVoice.Volume = 0.5f;
            buffer = new AudioBuffer();
            buffer.AudioData = new MemoryStream();
            waveFormat = format;
            bytesPerSample = (waveFormat.BitsPerSample / 8) * Channels;
            for (int i = 0; i < 5; i++)
            {
                sampleData[i] = new float[SAMPLE_SIZE * Channels];
                bData[i] = new byte[SAMPLE_SIZE * bytesPerSample];
            }
            sourceVoice.SubmitSourceBuffer(buffer);


        }


        public void AddSample(float[] soundOut)
        {
            if (samplePos >= (SAMPLE_SIZE * waveFormat.Channels))
            {
                playBuffer = currentBuffer;
                currentBuffer = ++currentBuffer % 5;
                samplePos = 0;
                if (!isMute)
                {
                    PlayBuffer();
                }
            }
            else
            {
                SubmitBlankBuffer();
            }
            for (int i = 0; i < waveFormat.Channels; i++)
            {
                sampleData[currentBuffer][samplePos] = soundOut[i];
                samplePos++;
            }
        }

        public bool FinishedPlaying()
        {
            return (sourceVoice.State.BuffersQueued < 2);
        }

        public void Play()
        {
            isMute = false;
            sourceVoice.Start();
        }

        public void PlayBuffer()
        {
            if (sourceVoice.State.BuffersQueued < 4)
            {
                int index = 0;
                for (int i = 0; i < (SAMPLE_SIZE * waveFormat.Channels); i++)
                {
                    float num3 = sampleData[playBuffer][i];
                    byte[] bytes = BitConverter.GetBytes(num3);
                    bData[playBuffer][index] = bytes[0];
                    bData[playBuffer][index + 1] = bytes[1];
                    bData[playBuffer][index + 2] = bytes[2];
                    bData[playBuffer][index + 3] = bytes[3];
                    index += bytesPerSample / waveFormat.Channels;
                }
                buffer.AudioData.SetLength(0L);
                buffer.AudioData = new MemoryStream(bData[playBuffer], 0, bData[playBuffer].Length);
                buffer.AudioData.Position = 0L;
                buffer.AudioBytes = bData[playBuffer].Length;
                buffer.Flags = BufferFlags.None;
                buffer.Context = (IntPtr)playBuffer;
                sourceVoice.SubmitSourceBuffer(buffer);
                submittedBuffers++;
                if (submittedBuffers > 1)
                {
                    if (!readyToPlay)
                    {
                        readyToPlay = true;
                    }
                    submittedBuffers = 2;
                }
            }
        }

        public void Reset()
        {
            Cycles = 0;
        }

        public void SetVolume(float vol)
        {
            masteringVoice.Volume = vol;
            sourceVoice.Volume = vol;
        }

        public void Shutdown()
        {
            buffer.Dispose();
            sourceVoice.FlushSourceBuffers();
            buffer = null;
            sourceVoice.Dispose();
            sourceVoice = null;
            masteringVoice.Dispose();
            masteringVoice = null;
            device.StopEngine();
            device.Dispose();
        }



        public void Stop()
        {
            sourceVoice.FlushSourceBuffers();
            sourceVoice.Stop();
            isMute = true;
        }

        public void SubmitBlankBuffer()
        {
            if (sourceVoice.State.BuffersQueued > 0)
            {
                sourceVoice.Discontinuity();
            }
        }

        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (buffer != null)
                    buffer.Dispose();
                if (device != null)
                    device.Dispose();


                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }




    }
}