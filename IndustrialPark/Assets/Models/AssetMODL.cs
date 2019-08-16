using HipHopFile;
using SharpDX;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;

        public AssetMODL(Section_AHDR AHDR, Game game, Platform platform, SharpRenderer renderer) : base(AHDR, game, platform, renderer) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            AddToRenderingDictionary(AHDR.assetID, this);

            if (currentGame == Game.Incredibles)
            {
                AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                AddToNameDictionary(Functions.BKDRHash(newName), newName);
            }
        }

        private string newName => AHDR.ADBG.assetName.Replace(".dff", "");

        public void MovieRemoveFromDictionary()
        {
            renderingDictionary.Remove(Functions.BKDRHash(newName));
            nameDictionary.Remove(Functions.BKDRHash(newName));
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset)
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

            model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset);
        }
    }
}