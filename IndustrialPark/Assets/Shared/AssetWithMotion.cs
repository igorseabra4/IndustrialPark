using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class AssetWithMotion : EntityAsset
    {
        public AssetWithMotion(string assetName, AssetType assetType, BaseAssetType baseAssetType, Vector3 position) : base(assetName, assetType, baseAssetType, position) { }

        public AssetWithMotion(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness) { }

        public override void Reset()
        {
            base.Reset();
            Motion?.Reset();
        }

        public Matrix PlatLocalTranslation() => Motion.PlatLocalTranslation();

        public override Matrix PlatLocalRotation() => Motion.PlatLocalRotation();

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                if (driver != null)
                {
                    return Matrix.Scaling(_scale)
                        * PlatLocalRotation() * PlatLocalTranslation()
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - new Vector3(driver.PositionX, driver.PositionY, driver.PositionZ))
                        * (driverUseRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }

                if (Motion is Motion_ExtendRetract)
                    return Matrix.Scaling(_scale) * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * PlatLocalTranslation();

                return Matrix.Scaling(_scale)
                    * PlatLocalRotation() * PlatLocalTranslation()
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(_position);
            }

            return world;
        }

        [Category("\tMotion")]
        public Motion Motion { get; set; }
    }
}