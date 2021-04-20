using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class RenderableRotatableDynaBase : RenderableDynaBase, IRotatableAsset
    {
        private const string dynaCategoryName = "DYNA Placement";
        
        protected float _yaw;
        [Category(dynaCategoryName)]
        public AssetSingle Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set { _yaw = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _pitch;
        [Category(dynaCategoryName)]
        public AssetSingle Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set { _pitch = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        protected float _roll;
        [Category(dynaCategoryName)]
        public AssetSingle Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set { _roll = MathUtil.DegreesToRadians(value); CreateTransformMatrix(); }
        }

        public RenderableRotatableDynaBase(Section_AHDR AHDR, DynaType type, Game game, Platform platform) : base(AHDR, type, game, platform) { }

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * Matrix.Translation(_position);

            CreateBoundingBox();
        }
    }
}