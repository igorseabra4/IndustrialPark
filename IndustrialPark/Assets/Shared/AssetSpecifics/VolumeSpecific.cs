using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public abstract class VolumeSpecific_Generic : GenericAssetDataContainer
    {
        protected Vector3 _position;
        public AssetSingle CenterX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                FixPosition();
            }
        }
        public AssetSingle CenterY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                FixPosition();
            }
        }
        public AssetSingle CenterZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                FixPosition();
            }
        }

        public VolumeSpecific_Generic() { }
        public VolumeSpecific_Generic(EndianBinaryReader reader)
        {
            _position.X = reader.ReadSingle();
            _position.Y = reader.ReadSingle();
            _position.Z = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(CenterX);
                writer.Write(CenterY);
                writer.Write(CenterZ);
                return writer.ToArray();
            }
        }

        public BoundingBox boundingBox;
        public BoundingSphere boundingSphere;
        protected Matrix world;
        protected Vector3[] vertices;

        public abstract void CreateTransformMatrix();
        public abstract float? GetIntersectionPosition(SharpRenderer renderer, Ray ray);
        public abstract void Draw(SharpRenderer renderer, bool isSelected);

        public virtual void FixPosition() => CreateTransformMatrix();

        public float GetDistanceFrom(Vector3 position)
        {
            float min = Vector3.Distance(position, _position);
            float d;

            foreach (var v in boundingBox.GetCorners())
                if ((d = Vector3.Distance(position, v)) < min)
                    min = d;

            return min;
        }

        public BoundingBox GetBoundingBox() => boundingBox;

        public bool ShouldDraw(SharpRenderer renderer, bool isSelected, bool dontRender, bool isInvisible)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }
    }

    public class VolumeBox : VolumeSpecific_Generic, IVolumeAsset
    {
        private Vector3 _trigPos1;

        public AssetSingle MaximumX
        {
            get => _trigPos1.X;
            set
            {
                _trigPos1.X = value;
                FixPosition();
            }
        }

        public AssetSingle MaximumY
        {
            get => _trigPos1.Y;
            set
            {
                _trigPos1.Y = value;
                FixPosition();
            }
        }
        public AssetSingle MaximumZ
        {
            get => _trigPos1.Z;
            set
            {
                _trigPos1.Z = value;
                FixPosition();
            }
        }

        private Vector3 _trigPos0;

        public AssetSingle MinimumX
        {
            get => _trigPos0.X;
            set
            {
                _trigPos0.X = value;
                FixPosition();
            }
        }
        public AssetSingle MinimumY
        {
            get => _trigPos0.Y;
            set
            {
                _trigPos0.Y = value;
                FixPosition();
            }
        }
        public AssetSingle MinimumZ
        {
            get => _trigPos0.Z;
            set
            {
                _trigPos0.Z = value;
                FixPosition();
            }
        }

        public VolumeBox() { }
        public VolumeBox(EndianBinaryReader reader) : base(reader)
        {
            _trigPos1.X = reader.ReadSingle();
            _trigPos1.Y = reader.ReadSingle();
            _trigPos1.Z = reader.ReadSingle();
            _trigPos0.X = reader.ReadSingle();
            _trigPos0.Y = reader.ReadSingle();
            _trigPos0.Z = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(base.Serialize(game, endianness));
                writer.Write(MaximumX);
                writer.Write(MaximumY);
                writer.Write(MaximumZ);
                writer.Write(MinimumX);
                writer.Write(MinimumY);
                writer.Write(MinimumZ);
                return writer.ToArray();
            }
        }

        public override void CreateTransformMatrix()
        {
            Vector3 boxSize = _trigPos1 - _trigPos0;
            Vector3 midPos = (_trigPos0 + _trigPos1) / 2f;

            world = Matrix.Scaling(boxSize) * Matrix.Translation(midPos);

            var verticesF = SharpRenderer.cubeVertices;
            vertices = new Vector3[verticesF.Count];
            for (int i = 0; i < verticesF.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(verticesF[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world);
            return null;
        }

        public override void FixPosition()
        {
            if (_trigPos0.X > _trigPos1.X)
            {
                var temp = _trigPos1.X;
                _trigPos1.X = _trigPos0.X;
                _trigPos0.X = temp;
            }
            if (_trigPos0.Y > _trigPos1.Y)
            {
                var temp = _trigPos1.Y;
                _trigPos1.Y = _trigPos0.Y;
                _trigPos0.Y = temp;
            }
            if (_trigPos0.Z > _trigPos1.Z)
            {
                var temp = _trigPos1.Z;
                _trigPos1.Z = _trigPos0.Z;
                _trigPos0.Z = temp;
            }

            base.FixPosition();
        }

        public override void Draw(SharpRenderer renderer, bool isSelected) => renderer.DrawCube(world, isSelected, 1f);
    }

    public class VolumeSphere : VolumeSpecific_Generic
    {
        private float _radius;
        public AssetSingle Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                CreateTransformMatrix();
            }
        }

        public VolumeSphere() { }
        public VolumeSphere(EndianBinaryReader reader) : base(reader)
        {
            Radius = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(base.Serialize(game, endianness));
                writer.Write(Radius);
                return writer.ToArray();
            }
        }

        public override void CreateTransformMatrix()
        {
            var center = new Vector3(CenterX, CenterY, CenterZ);
            world = Matrix.Scaling(Radius * 2f) * Matrix.Translation(center);

            boundingSphere = new BoundingSphere(center, Radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public override float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
            return null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected) => renderer.DrawSphere(world, isSelected, renderer.trigColor);
    }

    public class VolumeCylinder : VolumeSphere
    {
        private float _height;
        public AssetSingle Height
        {
            get => _height;
            set
            {
                _height = value;
                CreateTransformMatrix();
            }
        }

        public VolumeCylinder() { }
        public VolumeCylinder(EndianBinaryReader reader) : base(reader)
        {
            Height = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(base.Serialize(game, endianness));
                writer.Write(Height);
                return writer.ToArray();
            }
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(Radius * 2f, Height * 2f, Radius * 2f) * Matrix.Translation(CenterX, CenterY, CenterZ);

            List<Vector3> verticesF = SharpRenderer.cylinderVertices;
            vertices = new Vector3[verticesF.Count];
            for (int i = 0; i < verticesF.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(verticesF[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.cylinderTriangles, SharpRenderer.cylinderVertices, world);
            return null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected) => renderer.DrawCylinder(world, isSelected, renderer.trigColor);
    }
}