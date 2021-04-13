using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class AssetWithMotion : EntityAsset
    {
        public AssetWithMotion(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
        }

        public override bool HasReference(uint assetID) => Motion.HasReference(assetID) || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Motion.Verify(ref result);
            base.Verify(ref result);
        }
        
        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
                Motion.Increment();

            Matrix localW = LocalWorld();

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, localW, isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(localW, isSelected);
        }
        
        public override void Reset()
        {
            base.Reset();
            Motion.Reset();
        }

        public Matrix PlatLocalTranslation() => Motion.PlatLocalTranslation();
        
        public override Matrix PlatLocalRotation() => Motion.PlatLocalRotation();
        
        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                EntityAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return PlatLocalRotation() * PlatLocalTranslation()
                        * Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - new Vector3(driver.PositionX, driver.PositionY, driver.PositionZ))
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

        [Category("Platform")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Motion Motion { get; set; }
    }
}