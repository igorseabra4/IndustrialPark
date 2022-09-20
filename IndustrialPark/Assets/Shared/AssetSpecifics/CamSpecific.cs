using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class CamSpecific_Generic : GenericAssetDataContainer
    {
        public CamSpecific_Generic() { }
    }

    public class CamSpecific_Follow : CamSpecific_Generic
    {
        public AssetSingle Rotation { get; set; }
        public AssetSingle Distance { get; set; }
        public AssetSingle Height { get; set; }
        public AssetSingle RubberBand { get; set; }
        public AssetSingle StartSpeed { get; set; }
        public AssetSingle EndSpeed { get; set; }

        public CamSpecific_Follow() { }
        public CamSpecific_Follow(EndianBinaryReader reader)
        {
            Rotation = reader.ReadSingle();
            Distance = reader.ReadSingle();
            Height = reader.ReadSingle();
            RubberBand = reader.ReadSingle();
            StartSpeed = reader.ReadSingle();
            EndSpeed = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Rotation);
                writer.Write(Distance);
                writer.Write(Height);
                writer.Write(RubberBand);
                writer.Write(StartSpeed);
                writer.Write(EndSpeed);
                return writer.ToArray();
            }
        }
    }

    public class CamSpecific_Shoulder : CamSpecific_Generic
    {
        public AssetSingle Distance { get; set; }
        public AssetSingle Height { get; set; }
        public AssetSingle RealignSpeed { get; set; }
        public AssetSingle RealignDelay { get; set; }
        public AssetSingle Unknown1 { get; set; }
        public AssetSingle Unknown2 { get; set; }

        public CamSpecific_Shoulder() { }
        public CamSpecific_Shoulder(EndianBinaryReader reader)
        {
            Distance = reader.ReadSingle();
            Height = reader.ReadSingle();
            RealignSpeed = reader.ReadSingle();
            RealignDelay = reader.ReadSingle();
            Unknown1 = reader.ReadSingle();
            Unknown2 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Distance);
                writer.Write(Height);
                writer.Write(RealignSpeed);
                writer.Write(RealignDelay);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                return writer.ToArray();
            }
        }
    }

    public class CamSpecific_Static : CamSpecific_Generic
    {
        public int Unused { get; set; }

        public CamSpecific_Static() { }
        public CamSpecific_Static(EndianBinaryReader reader)
        {
            Unused = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unused);
                return writer.ToArray();
            }
        }
    }

    public class CamSpecific_Path : CamSpecific_Generic
    {
        public AssetID Unknown { get; set; }
        public AssetSingle TimeEnd { get; set; }
        public AssetSingle TimeDelay { get; set; }

        public CamSpecific_Path() { }
        public CamSpecific_Path(EndianBinaryReader reader)
        {
            Unknown = reader.ReadUInt32();
            TimeEnd = reader.ReadSingle();
            TimeDelay = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unknown);
                writer.Write(TimeEnd);
                writer.Write(TimeDelay);
                return writer.ToArray();
            }
        }
    }

    public class CamSpecific_StaticFollow : CamSpecific_Generic
    {
        public AssetSingle RubberBand { get; set; }

        public CamSpecific_StaticFollow() { }
        public CamSpecific_StaticFollow(EndianBinaryReader reader)
        {
            RubberBand = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(RubberBand);
                return writer.ToArray();
            }
        }
    }

    public class CamSpecific_Other : CamSpecific_Generic
    {
        public AssetSingle Unknown1 { get; set; }
        public AssetSingle Unknown2 { get; set; }
        public AssetSingle Unknown3 { get; set; }
        public AssetSingle Unknown4 { get; set; }
        public AssetSingle Unknown5 { get; set; }
        public AssetSingle Unknown6 { get; set; }

        public CamSpecific_Other() { }
        public CamSpecific_Other(EndianBinaryReader reader)
        {
            Unknown1 = reader.ReadSingle();
            Unknown2 = reader.ReadSingle();
            Unknown3 = reader.ReadSingle();
            Unknown4 = reader.ReadSingle();
            Unknown5 = reader.ReadSingle();
            Unknown6 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(Unknown6);
                return writer.ToArray();
            }
        }
    }
}
