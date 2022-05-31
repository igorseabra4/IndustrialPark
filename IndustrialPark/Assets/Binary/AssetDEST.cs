using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DestState : GenericAssetDataContainer
    {
        public int Percent { get; set; }
        public AssetID Model { get; set; }
        public AssetID Shrapnel_Destroy { get; set; }
        public AssetID Shrapnel_Hit { get; set; }
        public AssetID SoundGroup_Idle { get; set; }
        public AssetID SoundGroup_Fx { get; set; }
        public AssetID SoundGroup_Hit { get; set; }
        public AssetID SoundGroup_Fx_Switch { get; set; }
        public AssetID SoundGroup_Hit_Switch { get; set; }
        public AssetID Rumble_Hit { get; set; }
        public AssetID Rumble_Switch { get; set; }
        public int FxFlags { get; set; }
        public int nAnimations { get; set; }

        public DestState() { }
        public DestState(EndianBinaryReader reader)
        {
            Percent = reader.ReadInt32();
            Model = reader.ReadUInt32();
            Shrapnel_Destroy = reader.ReadUInt32();
            Shrapnel_Hit = reader.ReadUInt32();
            SoundGroup_Idle = reader.ReadUInt32();
            SoundGroup_Fx = reader.ReadUInt32();
            SoundGroup_Hit = reader.ReadUInt32();
            SoundGroup_Fx_Switch = reader.ReadUInt32();
            SoundGroup_Hit_Switch = reader.ReadUInt32();
            Rumble_Hit = reader.ReadUInt32();
            Rumble_Switch = reader.ReadUInt32();
            FxFlags = reader.ReadInt32();
            nAnimations = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Percent);
                writer.Write(Model);
                writer.Write(Shrapnel_Destroy);
                writer.Write(Shrapnel_Hit);
                writer.Write(SoundGroup_Idle);
                writer.Write(SoundGroup_Fx);
                writer.Write(SoundGroup_Hit);
                writer.Write(SoundGroup_Fx_Switch);
                writer.Write(SoundGroup_Hit_Switch);
                writer.Write(Rumble_Hit);
                writer.Write(Rumble_Switch);
                writer.Write(FxFlags);
                writer.Write(nAnimations);

                return writer.ToArray();
            }
        }
    }

    public class AssetDEST : Asset
    {
        private const string categoryName = "Destructible";

        [Category(categoryName)]
        public AssetID ModelInfo { get; set; }
        [Category(categoryName)]
        public int HitPoints { get; set; }
        [Category(categoryName)]
        public int HitFilter { get; set; }
        [Category(categoryName)]
        public int LaunchFlag { get; set; }
        [Category(categoryName)]
        public int Behavior { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetID SoundGroup_Idle { get; set; }
        [Category(categoryName)]
        public AssetSingle Respawn { get; set; }
        [Category(categoryName)]
        public byte TargetPriority { get; set; }
        [Category(categoryName)]
        public DestState[] States { get; set; }
        [Category(categoryName)]
        public AssetID Unknown1 { get; set; }
        [Category(categoryName)]
        public AssetID? Unknown2 { get; set; }

        public AssetDEST(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                ModelInfo = reader.ReadUInt32();
                int numStates = reader.ReadInt32();
                HitPoints = reader.ReadInt32();
                HitFilter = reader.ReadInt32();
                LaunchFlag = reader.ReadInt32();
                Behavior = reader.ReadInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                SoundGroup_Idle = reader.ReadUInt32();
                Respawn = reader.ReadSingle();
                TargetPriority = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                States = new DestState[numStates];
                for (int i = 0; i < States.Length; i++)
                    States[i] = new DestState(reader);
                Unknown1 = reader.ReadUInt32();
                if (!reader.EndOfStream)
                    Unknown2 = reader.ReadUInt32();
                else
                    Unknown2 = null;
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(ModelInfo);
                writer.Write(States.Length);
                writer.Write(HitPoints);
                writer.Write(HitFilter);
                writer.Write(LaunchFlag);
                writer.Write(Behavior);
                writer.Write(Flags.FlagValueInt);
                writer.Write(SoundGroup_Idle);
                writer.Write(Respawn);
                writer.Write(TargetPriority);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
                foreach (var state in States)
                    writer.Write(state.Serialize(game, endianness));
                writer.Write(Unknown1);
                if (Unknown2.HasValue)
                    writer.Write(Unknown2.Value);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(ModelInfo, ref result);

            if (ModelInfo == 0)
                result.Add("DEST with MINF set to 0");
        }
    }
}