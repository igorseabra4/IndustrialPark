using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SharpDX;

namespace IndustrialPark
{
    public class DynaBase
    {
        public virtual string Note { get => "DYNA Placement is unused"; }
        public byte[] Data { get; set; }

        public DynaBase()
        {
            Data = new byte[0];
        }

        public DynaBase(IEnumerable<byte> enumerable)
        {
            Data = enumerable.ToArray();
        }

        public virtual byte[] ToByteArray()
        {
            return Data;
        }

        public virtual bool HasReference(uint assetID)
        {
            return false;
        }

        [Browsable(false)]
        public virtual bool IsRenderableClickable { get => false; }

        [Browsable(false)]
        public virtual float PositionX { get => 0; set { } }
        [Browsable(false)]
        public virtual float PositionY { get => 0; set { } }
        [Browsable(false)]
        public virtual float PositionZ { get => 0; set { } }

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

        public virtual BoundingSphere GetGizmoCenter()
        {
            return new BoundingSphere();
        }
    }
}