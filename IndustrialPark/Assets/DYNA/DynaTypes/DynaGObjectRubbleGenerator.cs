using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class DynaGObjectRubbleGenerator : AssetDYNA, IAssetAddSelected
    {
        public enum ILandEffectType
        {
            None = 0,
            Dust = 1
        }

        private const string dynaCategoryName = "game_object:RubbleGenerator";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{Models.Length} models";

        protected override short constVersion => 5;

        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID SpawnFromMarker { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SpawnToMarker { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpawnRate { get; set; }
        [Category(dynaCategoryName)]
        public int SpawnLimit { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MinVelocity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxVelocity { get; set; }
        [Category(dynaCategoryName)]
        public uint MinRotations { get; set; }
        [Category(dynaCategoryName)]
        public uint MaxRotations { get; set; }
        [Category(dynaCategoryName)]
        public ILandEffectType LandEffectType { get; set; }
        [Category(dynaCategoryName)]
        public uint RepeatType { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LaunchSoundGroup { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LandSoundGroup { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID[] Models { get; set; }

        public DynaGObjectRubbleGenerator(string assetName) : base(assetName, DynaType.game_object__RubbleGenerator)
        {
            SpawnRate = 2f;
            MinVelocity = 10f;
            MaxVelocity = 15f;
            Models = new AssetID[0];
        }

        public DynaGObjectRubbleGenerator(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__RubbleGenerator, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Flags.FlagValueInt = reader.ReadUInt32();
                SpawnFromMarker = reader.ReadUInt32();
                SpawnToMarker = reader.ReadUInt32();
                var modelCount = reader.ReadUInt32();
                var modelOffset = reader.ReadUInt32();
                SpawnRate = reader.ReadSingle();
                SpawnLimit = reader.ReadInt32();
                MinVelocity = reader.ReadSingle();
                MaxVelocity = reader.ReadSingle();
                MinRotations = reader.ReadUInt32();
                MaxRotations = reader.ReadUInt32();
                LandEffectType = (ILandEffectType)reader.ReadInt32();
                RepeatType = reader.ReadUInt32();
                LaunchSoundGroup = reader.ReadUInt32();
                LandSoundGroup = reader.ReadUInt32();
                Models = new AssetID[modelCount];
                for (int i = 0; i < modelCount; i++)
                    Models[i] = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(Flags.FlagValueInt);
            writer.Write(SpawnFromMarker);
            writer.Write(SpawnToMarker);
            writer.Write(Models.Length);
            writer.Write(0x3C);
            writer.Write(SpawnRate);
            writer.Write(SpawnLimit);
            writer.Write(MinVelocity);
            writer.Write(MaxVelocity);
            writer.Write(MinRotations);
            writer.Write(MaxRotations);
            writer.Write((int)LandEffectType);
            writer.Write(RepeatType);
            writer.Write(LaunchSoundGroup);
            writer.Write(LandSoundGroup);
            foreach (var i in Models)
                writer.Write(i);


        }

        [Browsable(false)]
        public string GetItemsText => "models";

        public void AddItems(List<uint> items)
        {
            var models = Models.ToList();
            foreach (var u in items)
                if (!models.Contains(u))
                    models.Add(u);
            Models = models.ToArray();
        }
    }
}