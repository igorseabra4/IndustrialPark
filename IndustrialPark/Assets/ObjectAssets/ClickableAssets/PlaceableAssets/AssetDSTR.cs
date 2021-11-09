using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : EntityAsset
    {
        protected const string categoryName = "Destructable";

        [Category(categoryName)]
        public int AnimationSpeed { get; set; }
        [Category(categoryName)]
        public int InitialAnimationState { get; set; }
        [Category(categoryName)]
        public int Health { get; set; }
        [Category(categoryName)]
        public AssetID SpawnItem_AssetID { get; set; }
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
        public AssetID DestroyShrapnel_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID HitShrapnel_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID DestroySFX_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID HitSFX_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID HitModel_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID DestroyModel_AssetID { get; set; }

        public AssetDSTR(string assetName, Vector3 position) : base(assetName, AssetType.DSTR, BaseAssetType.DestructObj, position)
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
                SpawnItem_AssetID = reader.ReadUInt32();
                HitMask.FlagValueInt = reader.ReadUInt32();
                CollType = reader.ReadByte();
                FxType = reader.ReadByte();
                reader.ReadInt16();
                BlastRadius = reader.ReadSingle();
                BlastStrength = reader.ReadSingle();
                if (game != Game.Scooby)
                {
                    DestroyShrapnel_AssetID = reader.ReadUInt32();
                    HitShrapnel_AssetID = reader.ReadUInt32();
                    DestroySFX_AssetID = reader.ReadUInt32();
                    HitSFX_AssetID = reader.ReadUInt32();
                    HitModel_AssetID = reader.ReadUInt32();
                    DestroyModel_AssetID = reader.ReadUInt32();
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
                writer.Write(SpawnItem_AssetID);
                writer.Write(HitMask.FlagValueInt);
                writer.Write(CollType);
                writer.Write(FxType);
                writer.Write((short)0);
                writer.Write(BlastRadius);
                writer.Write(BlastStrength);

                if (game != Game.Scooby)
                {
                    writer.Write(DestroyShrapnel_AssetID);
                    writer.Write(HitShrapnel_AssetID);
                    writer.Write(DestroySFX_AssetID);
                    writer.Write(HitSFX_AssetID);
                    writer.Write(HitModel_AssetID);
                    writer.Write(DestroyModel_AssetID);
                }
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => SpawnItem_AssetID == assetID || DestroyShrapnel_AssetID == assetID ||
            HitShrapnel_AssetID == assetID || DestroySFX_AssetID == assetID || HitSFX_AssetID == assetID ||
            HitModel_AssetID == assetID || DestroyModel_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            Verify(SpawnItem_AssetID, ref result);

            if (game != Game.Scooby)
            {
                Verify(DestroyShrapnel_AssetID, ref result);
                Verify(HitShrapnel_AssetID, ref result);
                Verify(DestroySFX_AssetID, ref result);
                Verify(HitSFX_AssetID, ref result);
                Verify(HitModel_AssetID, ref result);
                Verify(DestroyModel_AssetID, ref result);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("DestroyShrapnel_AssetID");
                dt.RemoveProperty("HitShrapnel_AssetID");
                dt.RemoveProperty("DestroySFX_AssetID");
                dt.RemoveProperty("HitSFX_AssetID");
                dt.RemoveProperty("HitModel_AssetID");
                dt.RemoveProperty("DestroyModel_AssetID");
            }

            base.SetDynamicProperties(dt);
        }
    }
}