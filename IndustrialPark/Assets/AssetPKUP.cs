using HipHopFile;
using SharpDX;
using System;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetPKUP : RenderableAsset
    {
        public int pickEntryID;

        public AssetPKUP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = false)
        {
            pickEntryID = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x54));

            base.Setup(defaultMode);
        }

        public override void Draw()
        {
            if (AssetPICK.pick != null)
            {
                if (AssetPICK.pick.pickEntries.ContainsKey(pickEntryID))
                {
                    if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pick.pickEntries[pickEntryID].unknown4))
                    {
                        ArchiveEditorFunctions.renderingDictionary[AssetPICK.pick.pickEntries[pickEntryID].unknown4].Draw(world);
                    }
                    else
                    {
                        SharpRenderer.DrawCube(world);
                    }
                }
                else
                {
                    SharpRenderer.DrawCube(world);
                }
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }
}