using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public abstract class DynaCarryableProperty : AssetDYNA
    {
        private const string dynaCategoryName = "Carrying:Carryable Property";

        [Category(dynaCategoryName)]
        public AssetID ShrapnelID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID AnimCode { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

        protected int carryablePropertyEndPosition => dynaDataStartPosition + 12;

        public DynaCarryableProperty(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ShrapnelID = reader.ReadUInt32();
                AnimCode = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected void SerializeCarryableDyna(EndianBinaryWriter writer)
        {
            writer.Write(ShrapnelID);
            writer.Write(AnimCode);
            writer.Write(Flags.FlagValueInt);
        }
    }

    public class DynaCarryablePropertyAttract : DynaCarryableProperty
    {
        private const string dynaCategoryName = "Carrying:Carryable Property:Use Property Attract";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;


        public DynaCarryablePropertyAttract(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Carrying_CarryableProperty_UsePropertyAttract, game, endianness)
        {
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeCarryableDyna(writer);
        }
    }

    public class DynaCarryablePropertyGeneric : DynaCarryableProperty
    {
        private const string dynaCategoryName = "Carrying:Carryable Property:Generic Use Property";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID useAnimCode { get; set; }

        public DynaCarryablePropertyGeneric(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Carrying_CarryableProperty_GenericUseProperty, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = carryablePropertyEndPosition;

                useAnimCode = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeCarryableDyna(writer);
            writer.Write(useAnimCode);
        }
    }

    public class DynaCarryablePropertyRepel : DynaCarryableProperty
    {
        private const string dynaCategoryName = "Carrying:Carryable Property:Use Property Repel";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;


        public DynaCarryablePropertyRepel(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Carrying_CarryableProperty_UsePropertyRepel, game, endianness)
        {
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeCarryableDyna(writer);
        }
    }

    public class DynaCarryablePropertySwipe : DynaCarryableProperty
    {
        private const string dynaCategoryName = "Carrying:Carryable Property:Generic Use Property";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID useAnimCode { get; set; }

        public DynaCarryablePropertySwipe(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Carrying_CarryableProperty_UsePropertySwipe, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = carryablePropertyEndPosition;

                useAnimCode = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeCarryableDyna(writer);
            writer.Write(useAnimCode);
        }
    }
}