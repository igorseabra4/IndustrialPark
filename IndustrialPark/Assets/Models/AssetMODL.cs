using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;

        public AssetMODL(Section_AHDR AHDR) : base(AHDR) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);

            if (Functions.currentGame == Game.Incredibles)
            {
                ArchiveEditorFunctions.AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                ArchiveEditorFunctions.AddToNameDictionary(Functions.BKDRHash(newName), newName);
            }
        }

        private string newName => AHDR.ADBG.assetName.Replace(".dff", "");

        public void MovieRemoveFromDictionary()
        {
            ArchiveEditorFunctions.renderingDictionary.Remove(Functions.BKDRHash(newName));
            ArchiveEditorFunctions.nameDictionary.Remove(Functions.BKDRHash(newName));
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            if (renderBasedOnLodt)
            {
                if (AssetLODT.MaxDistances.ContainsKey(AHDR.assetID))
                {
                    if (Vector3.Distance(Program.MainForm.renderer.Camera.Position, (Vector3)world.Row4) > AssetLODT.MaxDistances[AHDR.assetID])
                        return;
                }
                else if (Vector3.Distance(Program.MainForm.renderer.Camera.Position, (Vector3)world.Row4) > 100f)
                    return;
            }

            model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color);
        }
    }
}