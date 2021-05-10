using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class PareSpecific_Generic : GenericAssetDataContainer
    {
        public PareSpecific_Generic() { }
    }

    public class PareSpecific_xPECircle : PareSpecific_Generic
    {
        public AssetSingle Radius { get; set; }
        public AssetSingle Deflection { get; set; }
        public AssetSingle DirX { get; set; }
        public AssetSingle DirY { get; set; }
        public AssetSingle DirZ { get; set; }

        public PareSpecific_xPECircle() { }
        public PareSpecific_xPECircle(EndianBinaryReader reader)
        {
            Radius = reader.ReadSingle();
            Deflection = reader.ReadSingle();
            DirX = reader.ReadSingle();
            DirY = reader.ReadSingle();
            DirZ = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Radius);
            writer.Write(Deflection);
            writer.Write(DirX);
            writer.Write(DirY);
            writer.Write(DirZ);
            return writer.ToArray();
        }
    }

    public class PareSpecific_tagEmitSphere : PareSpecific_Generic
    {
        public AssetSingle Radius { get; set; }

        public PareSpecific_tagEmitSphere() { }
        public PareSpecific_tagEmitSphere(EndianBinaryReader reader)
        {
            Radius = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Radius);
            return writer.ToArray();
        }
    }

    public class PareSpecific_tagEmitRect : PareSpecific_Generic
    {
        public AssetSingle X_Len { get; set; }
        public AssetSingle Z_Len { get; set; }

        public PareSpecific_tagEmitRect() { }
        public PareSpecific_tagEmitRect(EndianBinaryReader reader)
        {
            X_Len = reader.ReadSingle();
            Z_Len = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(X_Len);
            writer.Write(Z_Len);
            return writer.ToArray();
        }
    }

    public class PareSpecific_tagEmitLine : PareSpecific_Generic
    {
        public AssetSingle Position_0_X { get; set; }
        public AssetSingle Position_0_Y { get; set; }
        public AssetSingle Position_0_Z { get; set; }
        public AssetSingle Position_1_X { get; set; }
        public AssetSingle Position_1_Y { get; set; }
        public AssetSingle Position_1_Z { get; set; }
        public AssetSingle Radius { get; set; }

        public PareSpecific_tagEmitLine() { }
        public PareSpecific_tagEmitLine(EndianBinaryReader reader)
        {
            Position_0_X = reader.ReadSingle();
            Position_0_Y = reader.ReadSingle();
            Position_0_Z = reader.ReadSingle();
            Position_1_X = reader.ReadSingle();
            Position_1_Y = reader.ReadSingle();
            Position_1_Z = reader.ReadSingle();
            Radius = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Position_0_X);
            writer.Write(Position_0_Y);
            writer.Write(Position_0_Z);
            writer.Write(Position_1_X);
            writer.Write(Position_1_Y);
            writer.Write(Position_1_Z);
            writer.Write(Radius);
            return writer.ToArray();
        }
    }

    public class PareSpecific_tagEmitVolume : PareSpecific_Generic
    {
        public AssetID VolumeAssetID { get; set; }

        public PareSpecific_tagEmitVolume()
        {
            VolumeAssetID = 0;
        }

        public PareSpecific_tagEmitVolume(EndianBinaryReader reader)
        {
            VolumeAssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(VolumeAssetID);
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => VolumeAssetID == assetID;
        public override void Verify(ref List<string> result) => Asset.Verify(VolumeAssetID, ref result);
    }

    public class PareSpecific_tagEmitOffsetPoint : PareSpecific_Generic
    {
        public AssetSingle Position_X { get; set; }
        public AssetSingle Position_Y { get; set; }
        public AssetSingle Position_Z { get; set; }

        public PareSpecific_tagEmitOffsetPoint() { }
        public PareSpecific_tagEmitOffsetPoint(EndianBinaryReader reader)
        {
            Position_X = reader.ReadSingle();
            Position_Y = reader.ReadSingle();
            Position_Z = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Position_X);
            writer.Write(Position_Y);
            writer.Write(Position_Z);
            return writer.ToArray();
        }
    }

    public class PareSpecific_xPEVCyl : PareSpecific_Generic
    {
        public AssetSingle Height { get; set; }
        public AssetSingle Radius { get; set; }
        public AssetSingle Deflection { get; set; }

        public PareSpecific_xPEVCyl() { }
        public PareSpecific_xPEVCyl(EndianBinaryReader reader)
        {
            Height = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Deflection = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Height);
            writer.Write(Radius);
            writer.Write(Deflection);
            return writer.ToArray();
        }
    }

    public class PareSpecific_xPEEntBone : PareSpecific_Generic
    {
        public byte flags { get; set; }
        public byte type { get; set; }
        public byte bone { get; set; }
        public AssetSingle OffsetX { get; set; }
        public AssetSingle OffsetY { get; set; }
        public AssetSingle OffsetZ { get; set; }
        public AssetSingle Radius { get; set; }
        public AssetSingle Deflection { get; set; }

        public PareSpecific_xPEEntBone() { }
        public PareSpecific_xPEEntBone(EndianBinaryReader reader)
        {
            flags = reader.ReadByte();
            type = reader.ReadByte();
            bone = reader.ReadByte();
            reader.ReadByte();
            OffsetX = reader.ReadSingle();
            OffsetY = reader.ReadSingle();
            OffsetZ = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Deflection = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(flags);
            writer.Write(type);
            writer.Write(bone);
            writer.Write((byte)0);
            writer.Write(OffsetX);
            writer.Write(OffsetY);
            writer.Write(OffsetZ);
            writer.Write(Radius);
            writer.Write(Deflection);
            return writer.ToArray();
        }
    }

    public class PareSpecific_xPEEntBound : PareSpecific_Generic
    {
        public byte flags { get; set; }
        public byte type { get; set; }
        public byte pad1 { get; set; }
        public byte pad2 { get; set; }
        public AssetSingle Expand { get; set; }
        public AssetSingle Deflection { get; set; }

        public PareSpecific_xPEEntBound() { }
        public PareSpecific_xPEEntBound(EndianBinaryReader reader)
        {
            flags = reader.ReadByte();
            type = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            Expand = reader.ReadSingle();
            Deflection = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(flags);
            writer.Write(type);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(Expand);
            writer.Write(Deflection);
            return writer.ToArray();
        }
    }
}
