using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaNpcGate : AssetDYNA
    {
        private const string dynaCategoryName = "Enemy:NPC Gate";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(NpcWallsGroup);
        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetID NpcWallsGroup { get; set; }
        [Category(dynaCategoryName)]
        public uint GateFacing { get; set; }
        [Category(dynaCategoryName)]
        public AssetID NpcsGroup { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

        public DynaNpcGate(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__NPC_Gate, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                NpcWallsGroup = reader.ReadUInt32();
                GateFacing = reader.ReadUInt32();
                NpcsGroup = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(NpcWallsGroup);
            writer.Write(GateFacing);
            writer.Write(NpcsGroup);
            writer.Write(Flags.FlagValueInt);
        }
    }
}