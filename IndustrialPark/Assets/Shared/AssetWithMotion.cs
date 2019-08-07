using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public abstract class AssetWithMotion : PlaceableAsset
    {
        public AssetWithMotion(Section_AHDR AHDR) : base(AHDR)
        {
            _motion = new Motion_Mechanism(Data.Skip(MotionStart).ToArray());
        }

        public override bool HasReference(uint assetID) => _motion.HasReference(assetID) || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            _motion.Verify(ref result);
            base.Verify(ref result);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
                _motion.Increment();

            Matrix localW = LocalWorld();

            if (!isSelected && (DontRender || isInvisible))
                return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, localW, isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(localW, isSelected);
        }
        
        public override void Reset()
        {
            base.Reset();
            _motion.Reset();
        }

        public Matrix PlatLocalTranslation()
        {
            return _motion.PlatLocalTranslation();
        }

        public override Matrix PlatLocalRotation()
        {
            return _motion.PlatLocalRotation();
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                PlaceableAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return PlatLocalRotation() * PlatLocalTranslation()
                        * Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - driver._position)
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }

                return PlatLocalRotation() * PlatLocalTranslation()
                    * Matrix.Scaling(_scale)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(_position);
            }

            return world;
        }

        protected Motion _motion;

        protected abstract int MotionStart { get; }

        [Browsable(false)]
        public Motion Motion
        {
            get => _motion;
            set
            {
                _motion = value;

                List<byte> before = Data.Take(MotionStart).ToList();
                before.AddRange(value.ToByteArray());
                before.AddRange(Data.Skip(MotionStart + Motion.Size));
                Data = before.ToArray();
            }
        }
    }
}