using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class DynaBase : EndianConvertibleWithData
    {
        public virtual string Note { get => "DYNA Placement is unused"; }
        public override byte[] Data { get; set; }

        public DynaBase(Platform platform) : base(EndianConverter.PlatformEndianness(platform))
        {
            Data = new byte[0];
        }

        public DynaBase(IEnumerable<byte> enumerable, Platform platform) : this (platform)
        {
            Data = enumerable.ToArray();
        }

        public virtual byte[] ToByteArray()
        {
            return Data;
        }

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

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

        public delegate void DynaBasePropertyChanged(DynaBase value);
        public DynaBasePropertyChanged dynaSpecificPropertyChanged;
    }
}