using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public abstract class DynaEnemyRATS : DynaEnemy
    {
        private const string dynaCategoryName = "Enemy:RATS";

        [Category(dynaCategoryName)]
        public AssetID LightkitID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID npcWalls { get; set; }
        [Category(dynaCategoryName)]
        public AssetID npcPerception { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UpdateDistance { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor("Enabled");

        protected int enemyRatsEndPosition => entityDynaEndPosition + 0x14;

        public DynaEnemyRATS(string assetName, DynaType type, Vector3 position) : base(assetName, type, position) { }

        public DynaEnemyRATS(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                LightkitID = reader.ReadUInt32();
                npcWalls = reader.ReadUInt32();
                npcPerception = reader.ReadUInt32();
                UpdateDistance = reader.ReadSingle();
                Flags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected void SerializeDynaEnemyRATS(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(LightkitID);
            writer.Write(npcWalls);
            writer.Write(npcPerception);
            writer.Write(UpdateDistance);
            writer.Write(Flags.FlagValueInt);
        }
    }
}