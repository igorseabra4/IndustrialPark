using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;

        public AssetMODL(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
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