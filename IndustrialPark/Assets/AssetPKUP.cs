using HipHopFile;

namespace IndustrialPark
{
    public class AssetPKUP : PlaceableAsset
    {
        public static new bool dontRender = false;

        private uint _pickEntryID;
        public uint PickEntryID
        {
            get { return _pickEntryID; }
            set
            {
                _pickEntryID = value;
                Write(0x54, value);
            }
        }

        public AssetPKUP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = false)
        {
            _pickEntryID = ReadUInt(0x54);

            base.Setup(renderer, defaultMode);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            if (AssetPICK.pick != null)
            {
                if (AssetPICK.pick.pickEntries.ContainsKey(_pickEntryID))
                {
                    if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pick.pickEntries[_pickEntryID].unknown4))
                    {
                        ArchiveEditorFunctions.renderingDictionary[AssetPICK.pick.pickEntries[_pickEntryID].unknown4].Draw(renderer, world, isSelected);
                    }
                    else
                    {
                        renderer.DrawCube(world, isSelected);
                    }
                }
                else
                {
                    renderer.DrawCube(world, isSelected);
                }
            }
            else
            {
                renderer.DrawCube(world, isSelected);
            }
        }
    }
}