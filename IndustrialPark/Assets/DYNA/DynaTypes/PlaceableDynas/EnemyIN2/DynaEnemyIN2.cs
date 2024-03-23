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
    public abstract class DynaEnemyIN2 : DynaEnemy
    {
        private const string dynaCategoryName = "Enemy:IN2";

        [Category(dynaCategoryName)]
        public AssetID NavigationMeshID { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask NPCFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID RespawnCounterID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SettingsHashID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID InterestPointerID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightKitID { get; set; }

        protected int enemyIN2EndPosition => entityDynaEndPosition + 0x18;

        public DynaEnemyIN2(string assetName, DynaType type, Vector3 position) : base(assetName, type, position) { }
        public DynaEnemyIN2(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                NavigationMeshID = reader.ReadUInt32();
                NPCFlags.FlagValueInt = reader.ReadUInt32();
                RespawnCounterID = reader.ReadUInt32();
                SettingsHashID = reader.ReadUInt32();
                InterestPointerID = reader.ReadUInt32();
                LightKitID = reader.ReadUInt32();
            }
        }

        protected void SerializeDynaEnemyIN2(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(NavigationMeshID);
            writer.Write(NPCFlags.FlagValueInt);
            writer.Write(RespawnCounterID);
            writer.Write(SettingsHashID);
            writer.Write(InterestPointerID);
            writer.Write(LightKitID);
        }
    }
}
