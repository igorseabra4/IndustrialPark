using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaNpcWalls : AssetDYNA
    {
        private const string dynaCategoryName = "Enemy:NPC Walls";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(npcWallsGroup);
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID npcWallsGroup { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

        public DynaNpcWalls(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__NPC_Walls, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                npcWallsGroup = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(npcWallsGroup);
            writer.Write(Flags.FlagValueInt);
        }
    }
}