using System.ComponentModel;

namespace IndustrialPark
{
    public class LodtWrapper
    {
        public EntryLODT Entry;

        public LodtWrapper(EntryLODT entry)
        {
            Entry = entry;
        }

        [Category("All Levels")]
        public AssetSingle MaxDistance
        {
            get => Entry.MaxDistance;
            set => Entry.MaxDistance = value;
        }
        [Category("Level 1"), DisplayName("Model")]
        public AssetID LOD1_Model
        {
            get => Entry.LOD1_Model;
            set => Entry.LOD1_Model = value;
        }
        [Category("Level 1"), DisplayName("Distance")]
        public AssetSingle LOD1_MinDistance
        {
            get => Entry.LOD1_MinDistance;
            set => Entry.LOD1_MinDistance = value;
        }
        [Category("Level 2"), DisplayName("Model")]
        public AssetID LOD2_Model
        {
            get => Entry.LOD2_Model;
            set => Entry.LOD2_Model = value;
        }
        [Category("Level 2"), DisplayName("Distance")]
        public AssetSingle LOD2_MinDistance
        {
            get => Entry.LOD2_MinDistance;
            set => Entry.LOD2_MinDistance = value;
        }
        [Category("Level 3"), DisplayName("Model")]
        public AssetID LOD3_Model
        {
            get => Entry.LOD3_Model;
            set => Entry.LOD3_Model = value;
        }
        [Category("Level 3"), DisplayName("Distance")]
        public AssetSingle LOD3_MinDistance
        {
            get => Entry.LOD3_MinDistance;
            set => Entry.LOD3_MinDistance = value;
        }
        [Category("Unknown - Movie/Incredibles only")]
        public AssetSingle Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }
}
