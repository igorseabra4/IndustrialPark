using Assimp;
using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public static bool renderBasedOnLodt = false;
        public static bool renderBasedOnPipt = true;

        public AssetMODL(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness, renderer) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            AddToRenderingDictionary(assetID, this);

            if (game >= Game.Incredibles)
            {
                AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                AddToNameDictionary(Functions.BKDRHash(newName), newName);
            }
        }

        private string newName => assetName.Replace(".dff", "");

        public void RemoveFromDictionary()
        {
            RemoveFromRenderingDictionary(Functions.BKDRHash(newName));
            RemoveFromNameDictionary(Functions.BKDRHash(newName));
        }

        private Dictionary<uint, PipeInfo> pipeEntries;
        [Browsable(false)]
        public bool SpecialBlendMode { get; private set; }

        [Browsable(false)]
        public Matrix TransformMatrix => Matrix.Identity;

        public void SetPipeline(PipeInfo[] piptEntries)
        {
            pipeEntries = new Dictionary<uint, PipeInfo>();
            SpecialBlendMode = false;
            foreach (var p in piptEntries)
            {
                SpecialBlendMode |= p.SourceBlend != BlendFactorType.None || p.DestinationBlend != BlendFactorType.None;
                if (p.SubObjectBits.FlagValueInt == uint.MaxValue)
                    pipeEntries[uint.MaxValue] = p;
                else
                {
                    uint subBits = p.SubObjectBits.FlagValueInt;
                    int atomic = AtomicFlags.Length - 1;
                    for (int i = atomic; i >= 0; i--)
                    {
                        if ((subBits & 1) != 0)
                            pipeEntries[(uint)i] = p;
                        subBits >>= 1;
                    }
                }
            }
        }

        public void ResetPipeline()
        {
            SpecialBlendMode = false;
            pipeEntries = null;
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset, bool isSelected)
        {
            if (renderBasedOnPipt && pipeEntries != null)
                model.RenderPipt(renderer, world, color, uvAnimOffset, isSelected, _dontDrawMeshNumber, pipeEntries);
            else
                model.Render(renderer, world, color, uvAnimOffset, isSelected, _dontDrawMeshNumber);
        }
    }
}