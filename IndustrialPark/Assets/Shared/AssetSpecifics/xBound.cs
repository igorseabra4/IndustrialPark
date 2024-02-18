using Assimp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    // Todo: Merge with Volume
    public enum BoundType : byte
    {
        Sphere = 0,
        Box = 1,
        Cylinder = 2
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoundType_Generic : GenericAssetDataContainer
    {
        public BoundType_Generic() { }
        public override void Serialize(EndianBinaryWriter writer) { }

    }

    public class xSphere : BoundType_Generic
    {
        private Vector3 _center;
        public AssetSingle CenterX
        {
            get => _center.X;
            set { _center.X = value; }
        }
        public AssetSingle CenterY
        {
            get => _center.Y;
            set { _center.Y = value; }
        }
        public AssetSingle CenterZ
        {
            get => _center.Z;
            set { _center.Z = value; }
        }
        public AssetSingle Radius { get; set; }
        public xSphere() { }

        public xSphere(EndianBinaryReader reader)
        {
            _center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Radius = reader.ReadSingle();
            reader.ReadBytes(0x14);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_center.X);
            writer.Write(_center.Y);
            writer.Write(_center.Z);
            writer.Write(Radius);
            writer.Write(new byte[0x14]);
        }
    }

    public class xBBox : BoundType_Generic
    {
        private Vector3 _center;
        public AssetSingle CenterX
        {
            get => _center.X;
            set { _center.X = value; }
        }
        public AssetSingle CenterY
        {
            get => _center.Y;
            set { _center.Y = value; }
        }
        public AssetSingle CenterZ
        {
            get => _center.Z;
            set { _center.Z = value; }
        }
        protected Vector3 _upper;
        public AssetSingle UpperX
        {
            get => _upper.X;
            set { _upper.X = value; }
        }
        public AssetSingle UpperY
        {
            get => _upper.Y;
            set { _upper.Y = value; }
        }
        public AssetSingle UpperZ
        {
            get => _upper.Z;
            set { _upper.Z = value; }
        }
        protected Vector3 _lower;
        public AssetSingle LowerX
        {
            get => _lower.X;
            set { _lower.X = value; }

        }
        public AssetSingle LowerY
        {
            get => _lower.Y;
            set { _lower.Y = value; }
        }
        public AssetSingle LowerZ
        {
            get => _lower.Z;
            set { _lower.Z = value; }
        }

        public xBBox() { }

        public xBBox(EndianBinaryReader reader)
        {
            _center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _upper = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _lower = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_center.X);
            writer.Write(_center.Y);
            writer.Write(_center.Z);
            writer.Write(_upper.X);
            writer.Write(_upper.Y);
            writer.Write(_upper.Z);
            writer.Write(_lower.X);
            writer.Write(_lower.Y);
            writer.Write(_lower.Z);
        }
    }

    public class xCylinder : BoundType_Generic
    {
        private Vector3 _center;
        public AssetSingle CenterX
        {
            get => _center.X;
            set { _center.X = value; }
        }
        public AssetSingle CenterY
        {
            get => _center.Y;
            set { _center.Y = value; }
        }
        public AssetSingle CenterZ
        {
            get => _center.Z;
            set { _center.Z = value; }
        }
        public AssetSingle Radius { get; set; }
        public AssetSingle Height { get; set; }

        public xCylinder() { }

        public xCylinder(EndianBinaryReader reader)
        {
            _center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
            reader.ReadBytes(0x10);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_center.X);
            writer.Write(_center.Y);
            writer.Write(_center.Z);
            writer.Write(Radius);
            writer.Write(Height);
            writer.Write(new byte[0x10]);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class xBound
    {
        public sbyte xmin { get; set; }
        public sbyte ymin { get; set; }
        public sbyte zmin { get; set; }
        public sbyte zmin_dup { get; set; }
        public sbyte xmax { get; set; }
        public sbyte ymax { get; set; }
        public sbyte zmax { get; set; }
        public sbyte zmax_dup { get; set; }
        private Vector3 Min;
        public AssetSingle MinPositionX
        {
            get => Min.X;
            set { Min.X = value; }
        }
        public AssetSingle MinPositionY
        {
            get => Min.Y;
            set { Min.Y = value; }
        }
        public AssetSingle MinPositionZ
        {
            get => Min.Z;
            set { Min.Z = value; }
        }
        private Vector3 Max;
        public AssetSingle MaxPositionX
        {
            get => Max.X;
            set { Max.X = value; }
        }
        public AssetSingle MaxPositionY
        {
            get => Max.Y;
            set { Max.Y = value; }
        }
        public AssetSingle MaxPositionZ
        {
            get => Max.Z;
            set { Max.Z = value; }
        }
        private BoundType _boundtype;
        public BoundType Type
        {
            get => _boundtype;
            set
            {
                _boundtype = value;
                switch ((BoundType)value)
                {
                    case BoundType.Sphere:
                        Bound = new xSphere();
                        break;
                    case BoundType.Box:
                        Bound = new xBBox();
                        break;
                    case BoundType.Cylinder:
                        Bound = new xCylinder();
                        break;
                }
            }
        }
        public BoundType_Generic Bound { get; set; }

        public xBound()
        {
            Bound = new BoundType_Generic();
        }

        public xBound(EndianBinaryReader reader)
        {
            xmin = reader.ReadSByte();
            ymin = reader.ReadSByte();
            zmin = reader.ReadSByte();
            zmin_dup = reader.ReadSByte();
            xmax = reader.ReadSByte();
            ymax = reader.ReadSByte();
            zmax = reader.ReadSByte();
            zmax_dup = reader.ReadSByte();
            Min = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Max = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Type = (BoundType)reader.ReadByte();
            reader.ReadBytes(3);
            
            switch (Type)
            {
                case BoundType.Sphere:
                    Bound = new xSphere(reader);
                    break;
                case BoundType.Box:
                    Bound = new xBBox(reader);
                    break;
                case BoundType.Cylinder:
                    Bound = new xCylinder(reader);
                    break;
            }
            reader.ReadInt32();
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(xmin);
            writer.Write(ymin);
            writer.Write(zmin);
            writer.Write(zmin_dup);
            writer.Write(xmax);
            writer.Write(ymax);
            writer.Write(zmax);
            writer.Write(zmax_dup);
            writer.Write(Min.X);
            writer.Write(Min.Y);
            writer.Write(Min.Z);
            writer.Write(Max.X);
            writer.Write(Max.Y);
            writer.Write(Max.Z);
            writer.Write((byte)Type);
            writer.Write(new byte[3]);

            Bound.Serialize(writer);
            writer.Write(0);
        }

    }
}
