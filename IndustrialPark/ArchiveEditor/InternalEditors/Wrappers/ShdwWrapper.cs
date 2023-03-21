namespace IndustrialPark
{
    public class ShdwWrapper
    {
        public EntrySHDW Entry;

        public ShdwWrapper(EntrySHDW entry)
        {
            Entry = entry;
        }

        public AssetID ShadowModel
        {
            get => Entry.ShadowModel;
            set => Entry.ShadowModel = value;
        }

        public int Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }
}
