using HipHopFile;
using SharpDX;
using System.Collections.Generic;
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
        public FlagBitmask HitMask { get; set; } = IntFlagsDescriptor();
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

        public AssetDSTR(string assetName, Vector3 position) : base(assetName, AssetType.DestructibleObject, BaseAssetType.DestructObj, position)
        {
            CollType = 2;
            BlastRadius = 4f;
            BlastStrength = 1f;
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
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
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            Verify(SpawnItem, ref result);

            if (game != Game.Scooby)
            {
                Verify(DestroyShrapnel, ref result);
                Verify(HitShrapnel, ref result);
                Verify(DestroySFX, ref result);
                Verify(HitSFX, ref result);
                Verify(HitModel, ref result);
                Verify(DestroyModel, ref result);
            }
        }

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