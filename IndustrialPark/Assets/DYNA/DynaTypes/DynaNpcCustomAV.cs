using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaNPCCustomAV : AssetDYNA
    {
        private const string dynaCategoryName = "npc:NPC Custom AV";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public AssetID AnimID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AnimDelay { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundGroupID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SoundDelay { get; set; }
        [Category(dynaCategoryName)]
        public bool WaitOnSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LoopTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FaceTargetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AnimBlendTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpeedMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpeedMax { get; set; }
        [Category(dynaCategoryName)]
        public bool RandomStartTime { get; set; }

        public DynaNPCCustomAV(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.npc__NPC_Custom_AV, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                AnimID = reader.ReadUInt32();
                AnimDelay = reader.ReadSingle();
                SoundGroupID = reader.ReadUInt32();
                SoundDelay = reader.ReadSingle();
                WaitOnSound = reader.ReadByteBool();
                reader.BaseStream.Position += 3;
                LoopTime = reader.ReadSingle();
                FaceTargetID = reader.ReadUInt32();
                AnimBlendTime = reader.ReadSingle();
                SpeedMin = reader.ReadSingle();
                SpeedMax = reader.ReadSingle();
                RandomStartTime = reader.ReadByteBool();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(AnimID);
            writer.Write(AnimDelay);
            writer.Write(SoundGroupID);
            writer.Write(SoundDelay);
            writer.Write(WaitOnSound);
            writer.Write(new byte[3]);
            writer.Write(LoopTime);
            writer.Write(FaceTargetID);
            writer.Write(AnimBlendTime);
            writer.Write(SpeedMin);
            writer.Write(SpeedMax);
            writer.Write(RandomStartTime);
            writer.Write(new byte[3]);
        }
    }
}