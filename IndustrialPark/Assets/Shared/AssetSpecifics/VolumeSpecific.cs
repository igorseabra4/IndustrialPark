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

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(CenterX);
            writer.Write(CenterY);
            writer.Write(CenterZ);
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

        public abstract void ApplyScale(Vector3 factor, float singleFactor);
    }

    public class VolumeBox : VolumeSpecific_Generic, IVolumeAsset
    {
        private Vector3 _maximum;

        public AssetSingle MaximumX
        {
            get => _maximum.X;
            set
            {
                _maximum.X = value;
                FixPosition();
            }
        }

        public AssetSingle MaximumY
        {
            get => _maximum.Y;
            set
            {
                _maximum.Y = value;
                FixPosition();
            }
        }
        public AssetSingle MaximumZ
        {
            get => _maximum.Z;
            set
            {
                _maximum.Z = value;
                FixPosition();
            }
        }

        private Vector3 _minimum;

        public AssetSingle MinimumX
        {
            get => _minimum.X;
            set
            {
                _minimum.X = value;
                FixPosition();
            }
        }
        public AssetSingle MinimumY
        {
            get => _minimum.Y;
            set
            {
                _minimum.Y = value;
                FixPosition();
            }
        }
        public AssetSingle MinimumZ
        {
            get => _minimum.Z;
            set
            {
                _minimum.Z = value;
                FixPosition();
            }
        }

        public VolumeBox() { }
        public VolumeBox(EndianBinaryReader reader) : base(reader)
        {
            _maximum.X = reader.ReadSingle();
            _maximum.Y = reader.ReadSingle();
            _maximum.Z = reader.ReadSingle();
            _minimum.X = reader.ReadSingle();
            _minimum.Y = reader.ReadSingle();
            _minimum.Z = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MaximumX);
            writer.Write(MaximumY);
            writer.Write(MaximumZ);
            writer.Write(MinimumX);
            writer.Write(MinimumY);
            writer.Write(MinimumZ);
        }

        public override void CreateTransformMatrix()
        {
            Vector3 boxSize = _maximum - _minimum;
            Vector3 midPos = (_minimum + _maximum) / 2f;

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

        public void SetPositions(float x0, float y0, float z0, float x1, float y1, float z1)
        {
            _minimum.X = x0;
            _minimum.Y = y0;
            _minimum.Z = z0;
            _maximum.X = x1;
            _maximum.Y = y1;
            _maximum.Z = z1;
            FixPosition();
        }

        public override void FixPosition()
        {
            if (_minimum.X > _maximum.X)
            {
                var temp = _maximum.X;
                _maximum.X = _minimum.X;
                _minimum.X = temp;
            }
            if (_minimum.Y > _maximum.Y)
            {
                var temp = _maximum.Y;
                _maximum.Y = _minimum.Y;
                _minimum.Y = temp;
            }
            if (_minimum.Z > _maximum.Z)
            {
                var temp = _maximum.Z;
                _maximum.Z = _minimum.Z;
                _minimum.Z = temp;
            }

            base.FixPosition();
        }

        public override void Draw(SharpRenderer renderer, bool isSelected) => renderer.DrawCube(world, isSelected, 1f);

        public override void ApplyScale(Vector3 factor, float singleFactor)
        {
            _minimum.X *= factor.X;
            _minimum.Y *= factor.Y;
            _minimum.Z *= factor.Z;

            _maximum.X *= factor.X;
            _maximum.Y *= factor.Y;
            _maximum.Z *= factor.Z;

            _position.X *= factor.X;
            _position.Y *= factor.Y;
            _position.Z *= factor.Z;

            FixPosition();
        }
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

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Radius);
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

        public override void ApplyScale(Vector3 factor, float singleFactor)
        {
            _position.X *= factor.X;
            _position.Y *= factor.Y;
            _position.Z *= factor.Z;
            _radius *= singleFactor;
            FixPosition();
        }
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

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Height);
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

        public override void ApplyScale(Vector3 factor, float singleFactor)
        {
            _height *= factor.Y;
            base.ApplyScale(factor, singleFactor);
        }
    }
}