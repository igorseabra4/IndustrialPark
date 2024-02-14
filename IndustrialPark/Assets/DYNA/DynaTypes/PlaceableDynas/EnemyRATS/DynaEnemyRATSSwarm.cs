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
    public abstract class DynaEnemyRATSSwarm : DynaEnemyRATS
    {
        private const string dynaCategoryName = "Enemy:RATS:Swarm";

        [Category(dynaCategoryName)]
        public int MemberNumber { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpawnRadius { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Attractor { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Repeller { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags_Swarm { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID LoopSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OneShotSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OneShotSoundMinTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OneShotSoundMaxTime { get; set; }

        public DynaEnemyRATSSwarm(string assetName, DynaType type, Vector3 position) : base(assetName, type, position) { }

        public DynaEnemyRATSSwarm(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = enemyRatsEndPosition;

                MemberNumber = reader.ReadInt32();
                SpawnRadius = reader.ReadSingle();
                Attractor = reader.ReadUInt32();
                Repeller = reader.ReadUInt32();
                Flags_Swarm.FlagValueInt = reader.ReadUInt32();
                LoopSound = reader.ReadUInt32();
                OneShotSound = reader.ReadUInt32();
                OneShotSoundMinTime = reader.ReadSingle();
                OneShotSoundMaxTime = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyRATS(writer);
            writer.Write(MemberNumber);
            writer.Write(SpawnRadius);
            writer.Write(Attractor);
            writer.Write(Repeller);
            writer.Write(Flags.FlagValueInt);
            writer.Write(LoopSound);
            writer.Write(OneShotSound);
            writer.Write(OneShotSoundMinTime);
            writer.Write(OneShotSoundMaxTime);
        }
    }
}