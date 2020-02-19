using System;
using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;
        public static bool renderBasedOnPipt = true;

        public AssetMODL(Section_AHDR AHDR, Game game, Platform platform, SharpRenderer renderer) : base(AHDR, game, platform, renderer) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            AddToRenderingDictionary(AHDR.assetID, this);

            if (game == Game.Incredibles)
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

            if (renderBasedOnPipt)
            {
                if (AssetPIPT.BlendModes.ContainsKey(AHDR.assetID))
                {
                    (int, BlendFactorType, BlendFactorType) sourceDest = AssetPIPT.BlendModes[AHDR.assetID];
                    renderer.device.SetBlend(BlendOperation.Add, GetSharpBlendMode(sourceDest.Item2, true), GetSharpBlendMode(sourceDest.Item3, false));
                }
                else
                    renderer.device.SetDefaultBlendState();

                renderer.device.UpdateAllStates();
            }

            model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset);
        }

        private BlendOption GetSharpBlendMode(BlendFactorType type, bool dest)
        {
            switch (type)
            {
                case BlendFactorType.Zero:
                    return BlendOption.Zero;
                case BlendFactorType.One:
                    return BlendOption.One;
                case BlendFactorType.SourceColor:
                    return BlendOption.SourceColor;
                case BlendFactorType.InverseSourceColor:
                    return BlendOption.InverseSourceColor;
                case BlendFactorType.SourceAlpha:
                    return BlendOption.SourceAlpha;
                case BlendFactorType.InverseSourceAlpha:
                    return BlendOption.InverseSourceAlpha;
                case BlendFactorType.DestinationAlpha:
                    return BlendOption.DestinationAlpha;
                case BlendFactorType.InverseDestinationAlpha:
                    return BlendOption.InverseDestinationAlpha;
                case BlendFactorType.DestinationColor:
                    return BlendOption.DestinationColor;
                case BlendFactorType.InverseDestinationColor:
                    return BlendOption.InverseDestinationColor;
                case BlendFactorType.SourceAlphaSaturated:
                    return BlendOption.SourceAlphaSaturate;
            }

            if (dest)
                return BlendOption.One;
            else
                return BlendOption.Zero;
        }
    }
}