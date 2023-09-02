using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class AssetWithMotion : EntityAsset
    {
        public AssetWithMotion(string assetName, AssetType assetType, BaseAssetType baseAssetType, Vector3 position) : base(assetName, assetType, baseAssetType, position) { }

        public AssetWithMotion(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness) { }

        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
                Motion.Increment();

            Matrix localW = LocalWorld();

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_model))
                ArchiveEditorFunctions.renderingDictionary[_model].Draw(renderer, localW, isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(localW, isSelected);
        }

        public override void Reset()
        {
            base.Reset();
            if (Motion != null)
                Motion.Reset();
        }

        public Matrix PlatLocalTranslation() => Motion.PlatLocalTranslation();

        public override Matrix PlatLocalRotation() => Motion.PlatLocalRotation();

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                var driver = FindDrivenByAsset(out bool useRotation);

                if (driver != null)
                {
                    return PlatLocalRotation() * PlatLocalTranslation()
                        * Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - new Vector3(driver.PositionX, driver.PositionY, driver.PositionZ))
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }

                if (Motion is Motion_ExtendRetract)
                    return Matrix.Scaling(_scale) * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * PlatLocalTranslation();

                return PlatLocalRotation() * PlatLocalTranslation()
                    * Matrix.Scaling(_scale)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(_position);
            }

            return world;
        }

        [Category("\tMotion"), TypeConverter(typeof(ExpandableObjectConverter))]
        public Motion Motion { get; set; }
    }
}