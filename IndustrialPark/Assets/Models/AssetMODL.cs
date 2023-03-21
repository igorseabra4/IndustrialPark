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

            if (game == Game.Incredibles)
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

        private Dictionary<uint, (BlendOption, BlendOption)> blendModes;
        [Browsable(false)]
        public bool SpecialBlendMode { get; private set; }

        [Browsable(false)]
        public Matrix TransformMatrix => Matrix.Identity;

        public void SetBlendModes((uint, BlendFactorType, BlendFactorType)[] sourceDest)
        {
            blendModes = new Dictionary<uint, (BlendOption, BlendOption)>();
            SpecialBlendMode = false;
            foreach (var f in sourceDest)
            {
                blendModes[f.Item1 == 0xFFFFFFFF ? 0xFFFFFFFF : (uint)(Math.Log(f.Item1, 2) - 1)] = (GetSharpBlendMode(f.Item2, true), GetSharpBlendMode(f.Item3, false));
                SpecialBlendMode |= f.Item2 != BlendFactorType.None || f.Item3 != BlendFactorType.None;
            }
        }

        public void ResetBlendModes()
        {
            SpecialBlendMode = false;
            blendModes = null;
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset)
        {
            if (renderBasedOnPipt && blendModes != null)
                model.RenderPipt(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset, _dontDrawMeshNumber, blendModes);
            else
                model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset, _dontDrawMeshNumber);
        }

        private static BlendOption GetSharpBlendMode(BlendFactorType type, bool dest)
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

            return dest ? BlendOption.One : BlendOption.Zero;
        }
    }
}