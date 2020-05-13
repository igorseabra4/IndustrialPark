using System.ComponentModel;
using SharpDX;

namespace IndustrialPark
{
    public abstract class DynaBase : AssetSpecific_Generic
    {
        [Browsable(false)]
        public abstract int StructSize { get; }
        
        public DynaBase(AssetDYNA asset) : base(asset, 0x10) { }
        
        [Browsable(false)]
        public virtual bool IsRenderableClickable => false;

        [Browsable(false)]
        public virtual float PositionX { get => 0; set { } }
        [Browsable(false)]
        public virtual float PositionY { get => 0; set { } }
        [Browsable(false)]
        public virtual float PositionZ { get => 0; set { } }
        [Browsable(false)]
        public virtual float Yaw { get => 0; set { } }
        [Browsable(false)]
        public virtual float Pitch { get => 0; set { } }
        [Browsable(false)]
        public virtual float Roll { get => 0; set { } }
        [Browsable(false)]
        public virtual float ScaleX { get => 0; set { } }
        [Browsable(false)]
        public virtual float ScaleY { get => 0; set { } }
        [Browsable(false)]
        public virtual float ScaleZ { get => 0; set { } }

        public virtual void CreateTransformMatrix()
        {
        }

        public virtual void Draw(SharpRenderer renderer, bool isSelected)
        {
        }

        public virtual BoundingBox GetBoundingBox()
        {
            return new BoundingBox();
        }

        public virtual float GetDistance(Vector3 cameraPosition)
        {
            return 0;
        }

        public virtual float? IntersectsWith(Ray ray)
        {
            return null;
        }
        
        public virtual BoundingSphere GetObjectCenter()
        {
            return new BoundingSphere();
        }
    }
}