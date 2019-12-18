using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetUI : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x80 + Offset;

        public AssetUI(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _textureAssetID = ReadUInt(0x5C + Offset);
        }

        public override bool HasReference(uint assetID) => TextureAssetID == assetID || AnimListAssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TextureAssetID, ref result);
            Verify(AnimListAssetID, ref result);
        }

        public override void CreateTransformMatrix()
        {
            if (_textureAssetID == 0)
            {
                world = Matrix.Scaling(_scale) * Matrix.Scaling(Width, Height, 1f)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(Position.X, -Position.Y, -Position.Z);
            }
            else
            {
                world = Matrix.Scaling(Width, Height, 1f)
                    * Matrix.Translation(Position.X, -Position.Y, -Position.Z);
            }

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            if (_textureAssetID == 0)
            {
                base.CreateBoundingBox();
            }
            else
            {
                if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) &&
                    ArchiveEditorFunctions.renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                    ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
                    CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
                else
                    CreateBoundingBox(SharpRenderer.planeVertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (!isSelected && (dontRender || isInvisible))
                return;

            if (_textureAssetID == 0)
            {
                base.Draw(renderer);
            }
            else
            {
                renderer.DrawPlane(world, isSelected, _textureAssetID, UvAnimOffset);
            }
        }

        protected override float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (_textureAssetID == 0)
            {
                return base.TriangleIntersection(r, initialDistance);
            }
            else
            {
                bool hasIntersected = false;
                float smallestDistance = 1000f;

                foreach (Triangle t in SharpRenderer.planeTriangles)
                {
                    Vector3 v1 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex1], world);
                    Vector3 v2 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex2], world);
                    Vector3 v3 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex3], world);

                    if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                    {
                        hasIntersected = true;

                        if (distance < smallestDistance)
                            smallestDistance = distance;
                    }
                }

                if (hasIntersected)
                    return smallestDistance;
                else return null;
            }
        }

        [Browsable(false)]
        public override AssetID Animation_AssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("UserInterface")]
        public AssetID Sound_AssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("UserInterface")]
        public AssetID AnimListAssetID
        {
            get => ReadUInt(0x50 + Offset);
            set => Write(0x50 + Offset, value);
        }

        [Category("UserInterface")]
        public int UIFlags
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("UserInterface")]
        public short Width
        {
            get => ReadShort(0x58 + Offset);
            set
            {
                Write(0x58 + Offset, value);
                CreateTransformMatrix();
            }
        }

        [Category("UserInterface")]
        public short Height
        {
            get => ReadShort(0x5A + Offset);
            set
            {
                Write(0x5A + Offset, value);
                CreateTransformMatrix();
            }
        }

        private uint _textureAssetID;
        [Category("UserInterface")]
        public AssetID TextureAssetID
        {
            get => _textureAssetID;
            set
            {
                _textureAssetID = value;
                Write(0x5C + Offset, _textureAssetID);
            }
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordTopLeftX
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordTopLeftY
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordTopRightX
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordTopRightY
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordBottomRightX
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordBottomRightY
        {
            get => ReadFloat(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordBottomLeftX
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("UserInterface"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextCoordBottomLeftY
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }
    }
}