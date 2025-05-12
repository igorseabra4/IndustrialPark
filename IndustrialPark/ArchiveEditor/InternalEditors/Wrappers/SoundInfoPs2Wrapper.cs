using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class SoundInfoPs2Wrapper
    {
        public EntrySoundInfo_PS2 Entry;
        public byte[] SoundData;
        private bool isStream;

        public SoundInfoPs2Wrapper(EntrySoundInfo_PS2 entry, byte[] soundData, bool isStream)
        {
            Entry = entry;
            SoundData = soundData;
            this.isStream = isStream;

            EntrySoundInfo_PS2.GetLoopingRange(soundData, out _startLoopBlock, out _endLoopBlock);
        }

        public uint Version => Entry.Version;

        public uint DataSize => Entry.DataSize;

        public uint SampleRate
        {
            get => Entry.SampleRate;
            set => Entry.SampleRate = value;
        }

        public uint StreamInterleaveCount
        {
            get => Entry.StreamInterleaveCount;
            set => Entry.StreamInterleaveCount = value;
        }

        public uint StreamInterleaveSize
        {
            get => Entry.StreamInterleaveSize;
            set => Entry.StreamInterleaveSize = value;    
        }

        public uint Reserved2
        {
            get => Entry.reserved2;
            set => Entry.reserved2 = value;
        }
        public string Trackname
        {
            get => Entry.TrackName;
            set => Entry.TrackName = value;
        }

        private int _startLoopBlock;
        public int StartLoopBlock
        {
            get => _startLoopBlock;
            set
            {
                if (isStream)
                    MessageBox.Show("Looping sound streams may not function perfectly due to the underlying streaming mechanism. Use with caution.",
                        "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);

                if (value < 0 || value >= NumBlocks)
                    throw new ArgumentOutOfRangeException("Start loop block needs to be greater than or equal 0 and less than \"DataSize\" / 16");
                _startLoopBlock = value;
                EntrySoundInfo_PS2.SetLoopingRange(ref SoundData, value, EndLoopBlock);
            }
        }

        private int _endLoopBlock;
        public int EndLoopBlock
        {
            get => _endLoopBlock;
            set
            {
                if (isStream)
                    MessageBox.Show("Looping sound streams may not function perfectly due to the underlying streaming mechanism. Use with caution.",
                        "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);

                if (value <= 1 || value >= NumBlocks)
                    throw new ArgumentOutOfRangeException("End loop block needs to be greater than or equal 1 and less than \"DataSize\" / 16");
                _endLoopBlock = value;
                EntrySoundInfo_PS2.SetLoopingRange(ref SoundData, StartLoopBlock, value);
            }
        }

        public int NumBlocks => (SoundData.Length / 16) - 1;

        public bool IsLooped
        {
            get => EntrySoundInfo_PS2.GetLoopingRange(SoundData, out int _, out int _);
            set
            {
                if (isStream)
                    MessageBox.Show("Looping sound streams may not function perfectly due to the underlying streaming mechanism. Use with caution.",
                        "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);

                if (value == true)
                    EntrySoundInfo_PS2.SetLoopingRange(ref SoundData);
                else
                    EntrySoundInfo_PS2.ClearLoopingFlags(ref SoundData, isStream);
            }
        }
    }
}