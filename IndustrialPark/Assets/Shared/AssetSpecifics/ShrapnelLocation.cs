using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum IShrapnelFragLocationType : int
    {
        Bone,
        BoneUpdated,
        BoneLocal,
        BoneLocalUpdated,
        LocTag,
        LocTagUpdated,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShrapnelLocation : GenericAssetDataContainer
    {
        private const byte padB = 0xCD;

        public IShrapnelFragLocationType FragLocationType { get; set; }
        [Description("matidx if type is Loc")]
        public int Index { get; set; }
        public AssetSingle PositionX { get; set; }
        public AssetSingle PositionY { get; set; }
        public AssetSingle PositionZ { get; set; }

        public ShrapnelLocation() { }

        public ShrapnelLocation(EndianBinaryReader reader)
        {
            FragLocationType = (IShrapnelFragLocationType)reader.ReadInt32();

            if (FragLocationType <= IShrapnelFragLocationType.BoneLocalUpdated)
                Index = reader.ReadInt32();

            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();

            if (FragLocationType >= IShrapnelFragLocationType.LocTag)
                Index = reader.ReadInt32();

            for (int i = 0; i < 0x10; i++)
                if (reader.ReadByte() != padB)
                    throw new Exception("Error reading SHRP padding: non-padding byte found at " + (reader.BaseStream.Position - 1).ToString());
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)FragLocationType);

            if (FragLocationType <= IShrapnelFragLocationType.BoneLocalUpdated)
                writer.Write(Index);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);

            if (FragLocationType >= IShrapnelFragLocationType.LocTag)
                writer.Write(Index);

            for (int i = 0; i < 0x10; i++)
                writer.Write(padB);
        }
    }

    public class ShrapnelLocation_TSSM : ShrapnelLocation
    {
        public AssetSingle RandRadius { get; set; }

        public ShrapnelLocation_TSSM() : base() { }

        public ShrapnelLocation_TSSM(EndianBinaryReader reader) : base(reader)
        {
            RandRadius = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(RandRadius);
        }
    }
}