using System;
using System.ComponentModel;
using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public abstract class RenderableAssetWithPosition : RenderableAsset
    {
        public RenderableAssetWithPosition(Section_AHDR AHDR) : base(AHDR) { }

        public abstract float PositionX { get; set; }
        public abstract float PositionY { get; set; }
        public abstract float PositionZ { get; set; }
        [Browsable(false)]
        public Vector3 Position { get { return new Vector3(PositionX, PositionY, PositionZ); } set { } }
        public abstract float ScaleX { get; set; }
        public abstract float ScaleY { get; set; }
        public abstract float ScaleZ { get; set; }
        [Browsable(false)]
        public Vector3 Scale { get { return new Vector3(ScaleX, ScaleY, ScaleZ); } set { } }
    }
}