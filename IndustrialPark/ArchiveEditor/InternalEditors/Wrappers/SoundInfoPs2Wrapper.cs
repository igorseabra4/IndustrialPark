using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class SoundInfoPs2Wrapper
    {
        public EntrySoundInfo_PS2 Entry;

        public SoundInfoPs2Wrapper(EntrySoundInfo_PS2 entry)
        {
            Entry = entry;
        }

        public uint Version
        {
            get => Entry.Version;
            set => Entry.Version = value;
        }

        public uint DataSize
        {
            get => Entry.DataSize;
            set => Entry.DataSize = value;
        }

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
    }
}