using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMVPT : RenderableAssetWithPosition
    {
        public static bool dontRender = false;

        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _scale = new Vector3(ReadFloat(0x1C), ReadFloat(0x20), ReadFloat(0x24));
                        
            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected override float? TriangleIntersection(Ray r, float distance)
        {
            return TriangleIntersection(r, distance, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;
            base.Draw(renderer);
        }

        public AssetID AssetID
        {
            get { return ReadUInt(0); }
            set { Write(0, value); }
        }

        private Vector3 _position;
        public override float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x8, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0xC, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x10, _position.Z);
                CreateTransformMatrix();
            }
        }

        private Vector3 _scale;
        public override float ScaleX
        {
            get { return _scale.X; }
            set
            {
                _scale.X = value;
                Write(0x1C, _scale.X);
                CreateTransformMatrix();
            }
        }

        public override float ScaleY
        {
            get { return _position.Y; }
            set
            {
                _scale.Y = value;
                Write(0x20, _scale.Y);
                CreateTransformMatrix();
            }
        }

        public override float ScaleZ
        {
            get { return _position.Z; }
            set
            {
                _scale.Z = value;
                Write(0x24, _scale.Z);
                CreateTransformMatrix();
            }
        }

        public AssetID[] OtherMVPTAssetIDs
        {
            get
            {
                List<AssetID> _otherMVPTs = new List<AssetID>();
                short amount = ReadShort(0x1A);
                for (int i = 0; i < amount; i++)
                    _otherMVPTs.Add(ReadUInt(0x28 + 4 * i));

                return _otherMVPTs.ToArray();
            }
            set
            {
                List<AssetID> newValues = value.ToList();
                short oldAmountOfPairs = ReadShort(0x1A);

                List<byte> newData = Data.Take(0x28).ToList();
                List<byte> restOfOldData = Data.Skip(0x28 + 4 * oldAmountOfPairs).ToList();

                foreach (AssetID i in newValues)
                    newData.AddRange(BitConverter.GetBytes(i).Reverse());

                newData.AddRange(restOfOldData);
                newData[0x1A] = BitConverter.GetBytes((short)newValues.Count)[1];
                newData[0x1B] = BitConverter.GetBytes((short)newValues.Count)[0];

                Data = newData.ToArray();
            }
        }
    }
}