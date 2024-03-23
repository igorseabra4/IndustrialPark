using HipHopFile;
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
        public FlagBitmask FxFlags { get; set; } = IntFlagsDescriptor();
        [Category("xDestructibleAssetAttachedAnimList")]
        public AssetID[] Animations { get; set; }

        public DestState()
        {
            Animations = new AssetID[0];
        }
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
            FxFlags.FlagValueInt = reader.ReadUInt32();

            int nanimations = reader.ReadInt32();
            Animations = new AssetID[nanimations];
            for (int i = 0; i < nanimations; i++)
                Animations[i] = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
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
            writer.Write(FxFlags.FlagValueInt);

            writer.Write(Animations.Length);
            foreach (var anim in Animations)
                writer.Write(anim);
        }
    }

    public class AssetDEST : Asset
    {
        private const string categoryName = "Destructible";

        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(ModelInfo);

        [Category(categoryName), ValidReferenceRequired]
        public AssetID ModelInfo { get; set; }
        [Category(categoryName)]
        public uint HitPoints { get; set; }
        [Category(categoryName)]
        public uint HitFilter { get; set; }
        [Category(categoryName)]
        public uint HitFilter_Excluded { get; set; }
        [Category(categoryName)]
        public uint HealthPoints { get; set; }
        [Category(categoryName)]
        public uint ExpPoints { get; set; }
        [Category(categoryName)]
        public AssetSingle HealthChance { get; set; }
        [Category(categoryName)]
        public AssetSingle ExpChance { get; set; }
        [Category(categoryName)]
        public FlagBitmask LaunchFlag { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public uint Behaviour { get; set; }
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

        public AssetDEST(string assetName) : base(assetName, AssetType.Destructible)
        {
            States = new DestState[0];
        }

        public AssetDEST(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                ModelInfo = reader.ReadUInt32();
                int numStates = reader.ReadInt32();
                HitPoints = reader.ReadUInt32();
                HitFilter = reader.ReadUInt32();
                if (game >= Game.ROTU)
                {
                    HitFilter_Excluded = reader.ReadUInt32();
                    HealthPoints = reader.ReadUInt32();
                    ExpPoints = reader.ReadUInt32();
                    HealthChance = reader.ReadSingle();
                    ExpChance = reader.ReadSingle();
                }
                LaunchFlag.FlagValueInt = reader.ReadUInt32();
                Behaviour = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                SoundGroup_Idle = reader.ReadUInt32();
                Respawn = reader.ReadSingle();
                TargetPriority = reader.ReadByte();
                reader.ReadBytes(3);

                States = new DestState[numStates];
                for (int i = 0; i < States.Length; i++)
                    States[i] = new DestState(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            writer.Write(ModelInfo);
            writer.Write(States.Length);
            writer.Write(HitPoints);
            writer.Write(HitFilter);
            if (game >= Game.ROTU)
            {
                writer.Write(HitFilter_Excluded);
                writer.Write(HealthPoints);
                writer.Write(ExpPoints);
                writer.Write(HealthChance);
                writer.Write(ExpChance);
            }
            writer.Write(LaunchFlag.FlagValueInt);
            writer.Write(Behaviour);
            writer.Write(Flags.FlagValueInt);
            writer.Write(SoundGroup_Idle);
            writer.Write(Respawn);
            writer.Write(TargetPriority);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            foreach (var state in States)
                state.Serialize(writer);
            writer.Write(0xFDFDFDFD);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.ROTU)
            {
                dt.RemoveProperty("HitFilter_Excluded");
                dt.RemoveProperty("HealthPoints");
                dt.RemoveProperty("ExpPoints");
                dt.RemoveProperty("HealthChance");
                dt.RemoveProperty("ExpChance");
            }
        }
    }
}