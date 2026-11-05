using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;
        public static bool renderBasedOnPipt = true;

        public AssetMODL(Section_AHDR AHDR, Game game) : base(AHDR, game) { }

        public string fileNameWithoutExtension => assetName.Replace(".dff", "");

        private Dictionary<uint, PipeInfo> pipeEntries;
        private bool _specialBlendMode;
        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => _specialBlendMode;

        [Browsable(false)]
        public Matrix TransformMatrix => Matrix.Identity;

        public void SetPipeline(IEnumerable<PipeInfo> piptEntries)
        {
            pipeEntries = new Dictionary<uint, PipeInfo>();
            _specialBlendMode = false;
            foreach (var p in piptEntries)
            {
                _specialBlendMode |= p.SourceBlend != BlendFactorType.None || p.DestinationBlend != BlendFactorType.None;
                if (p.SubObjectBits.FlagValueInt == uint.MaxValue)
                    pipeEntries[uint.MaxValue] = p;
                else
                {
                    uint subBits = p.SubObjectBits.FlagValueInt;
                    int atomic = AtomicFlags.Length - 1;
                    for (int i = atomic; i >= 0; i--)
                    {
                        if ((subBits & 1) != 0)
                        {
                            if (assetName == "test_ring")
                            {
                                var p2 = p.Clone();
                                p2.LightingMode = LightingMode.Prelight;
                                pipeEntries[(uint)i] = p2;
                            }
                            else
                            {
                                pipeEntries[(uint)i] = p;
                            }
                        }
                        subBits >>= 1;
                    }
                }
            }
        }

        public void ResetPipeline()
        {
            _specialBlendMode = false;
            pipeEntries = null;
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset, bool isSelected)
        {
            if (renderBasedOnPipt && pipeEntries != null)
                model.RenderPipt(renderer, world, color, uvAnimOffset, isSelected || this.isSelected, _dontDrawMeshNumber, pipeEntries);
            else
                model.Render(renderer, world, color, uvAnimOffset, isSelected || this.isSelected, _dontDrawMeshNumber);
        }
    }
}