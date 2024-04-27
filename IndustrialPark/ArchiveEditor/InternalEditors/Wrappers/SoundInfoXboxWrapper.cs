namespace IndustrialPark
{
    public class SoundInfoXboxWrapper
    {
        public EntrySoundInfo_XBOX Entry;

        public SoundInfoXboxWrapper(EntrySoundInfo_XBOX entry)
        {
            Entry = entry;
        }

        public XboxFormat wFormatTag
        {
            get => Entry.wFormatTag;
            set => Entry.wFormatTag = value;
        }
        public short nChannels
        {
            get => Entry.nChannels;
            set => Entry.nChannels = value;
        }
        public int nSamplesPerSec
        {
            get => Entry.nSamplesPerSec;
            set => Entry.nSamplesPerSec = value;
        }
        public int nAvgBytesPerSec
        {
            get => Entry.nAvgBytesPerSec;
            set => Entry.nAvgBytesPerSec = value;
        }
        public short nBlockAlign
        {
            get => Entry.nBlockAlign;
            set => Entry.nBlockAlign = value;
        }
        public short wBitsPerSample
        {
            get => Entry.wBitsPerSample;
            set => Entry.wBitsPerSample = value;
        }
        public short cbSize
        {
            get => Entry.cbSize;
            set => Entry.cbSize = value;
        }
        public short NibblesPerBlock
        {
            get => Entry.NibblesPerBlock;
            set => Entry.NibblesPerBlock = value;
        }
        public int dataSize
        {
            get => Entry.dataSize;
            set => Entry.dataSize = value;
        }
        public FlagBitmask Flags
        {
            get => Entry.Flags;
            set => Entry.Flags = value;
        }

        public int UnknownIncrediblesPC
        {
            get => Entry.UnknownIncrediblesPC;
            set => Entry.UnknownIncrediblesPC = value;
        }
    }
}