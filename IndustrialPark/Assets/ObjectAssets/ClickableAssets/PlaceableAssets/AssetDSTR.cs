using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : EntityAsset
    {
        protected const string categoryName = "Destructible Object";

        [Category(categoryName)]
        public int AnimationSpeed { get; set; }
        [Category(categoryName)]
        public int InitialAnimationState { get; set; }
        [Category(categoryName)]
        public int Health { get; set; }
        [Category(categoryName)]
        public AssetID SpawnItem { get; set; }
        [Category(categoryName)]
        public FlagBitmask HitMask { get; set; } = IntFlagsDescriptor(
            null, // 0
            null, // 1
            null, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            "Bubble Spin", // 12
            "Bubble Bounce", // 13
            "Bubble Bash", // 14
            "Bubble Bowl", // 15
            "Cruise Bubble", // 16
            null, // 17 bungee
            null, // 18 Thrown Enemy/Tiki
            null, // 19 Throw Fruit
            null, // 20 Patrick Slam
            null, // 21 null
            null, // 22 (Pressure Plate) Player Stand
            null, // 23 (Pressure Plate) Enemy Stand
            null, // 24 (Pressure Plate) Boulder/Bubble Bowl
            null, // 25 (Pressure Plate) Stone Tiki
            null, // 26 Sandy Melee/Sliding
            null, // 27 Patrick Melee/Sliding
            null, // 28 (Pressure Plate) Throw Fruit
            null // 29  Patrick Cartwheel
            );

        [Category(categoryName)]
        public byte CollType { get; set; }
        [Category(categoryName)]
        public byte FxType { get; set; }
        [Category(categoryName)]
        public AssetSingle BlastRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle BlastStrength { get; set; }
        [Category(categoryName)]
        public AssetID DestroyShrapnel { get; set; }
        [Category(categoryName)]
        public AssetID HitShrapnel { get; set; }
        [Category(categoryName)]
        public AssetID DestroySFX { get; set; }
        [Category(categoryName)]
        public AssetID HitSFX { get; set; }
        [Category(categoryName)]
        public AssetID HitModel { get; set; }
        [Category(categoryName)]
        public AssetID DestroyModel { get; set; }

        public AssetDSTR(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.DestructibleObject, BaseAssetType.DestructObj, position)
        {
            CollType = 2;
            BlastRadius = 4f;
            BlastStrength = 1f;

            if (template == AssetTemplate.Crate)
            {
                Health = 1;
                HitMask.FlagValueInt = 459;
                Model = "destruct_crate_bind";
                Animation = "CRATE WOBBLE";
                _links = new Link[]
                {
                    new Link(Game.Scooby)
                    {
                        EventReceiveID = (ushort)EventScooby.Destroy,
                        EventSendID = (ushort)EventScooby.LobMasterShootFromWidget,
                        TargetAsset = "CRATESHARDLOB"
                    },
                    new Link(Game.Scooby)
                    {
                        EventReceiveID = (ushort)EventScooby.Destroy,
                        EventSendID = (ushort)EventScooby.Play,
                        TargetAsset = "SMASH"
                    }
                };
            }
        }

        public AssetDSTR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                AnimationSpeed = reader.ReadInt32();
                InitialAnimationState = reader.ReadInt32();
                Health = reader.ReadInt32();
                SpawnItem = reader.ReadUInt32();
                HitMask.FlagValueInt = reader.ReadUInt32();
                CollType = reader.ReadByte();
                FxType = reader.ReadByte();
                reader.ReadInt16();
                BlastRadius = reader.ReadSingle();
                BlastStrength = reader.ReadSingle();
                if (game != Game.Scooby)
                {
                    DestroyShrapnel = reader.ReadUInt32();
                    HitShrapnel = reader.ReadUInt32();
                    DestroySFX = reader.ReadUInt32();
                    HitSFX = reader.ReadUInt32();
                    HitModel = reader.ReadUInt32();
                    DestroyModel = reader.ReadUInt32();
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(AnimationSpeed);
            writer.Write(InitialAnimationState);
            writer.Write(Health);
            writer.Write(SpawnItem);
            writer.Write(HitMask.FlagValueInt);
            writer.Write(CollType);
            writer.Write(FxType);
            writer.Write((short)0);
            writer.Write(BlastRadius);
            writer.Write(BlastStrength);

            if (game != Game.Scooby)
            {
                writer.Write(DestroyShrapnel);
                writer.Write(HitShrapnel);
                writer.Write(DestroySFX);
                writer.Write(HitSFX);
                writer.Write(HitModel);
                writer.Write(DestroyModel);
            }
            SerializeLinks(writer);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("DestroyShrapnel");
                dt.RemoveProperty("HitShrapnel");
                dt.RemoveProperty("DestroySFX");
                dt.RemoveProperty("HitSFX");
                dt.RemoveProperty("HitModel");
                dt.RemoveProperty("DestroyModel");
            }

            base.SetDynamicProperties(dt);
        }
    }
}