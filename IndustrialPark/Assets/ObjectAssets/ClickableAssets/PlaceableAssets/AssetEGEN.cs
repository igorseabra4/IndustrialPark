using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEGEN : EntityAsset
    {
        protected const string categoryName = "Electric Arc";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_X { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_Y { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Src_dpos_Z { get; set; }
        [Category(categoryName)]
        public byte DamageType { get; set; }
        [Category(categoryName)]
        public FlagBitmask EgenFlags { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ActiveTimeSeconds { get; set; }
        [Category(categoryName)]
        public AssetID OnAnim_AssetID { get; set; }

        public AssetEGEN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

            Src_dpos_X = reader.ReadSingle();
            Src_dpos_Y = reader.ReadSingle();
            Src_dpos_Z = reader.ReadSingle();
            DamageType = reader.ReadByte();
            EgenFlags.FlagValueByte = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            ActiveTimeSeconds = reader.ReadSingle();
            OnAnim_AssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));

            writer.Write(Src_dpos_X);
            writer.Write(Src_dpos_Y);
            writer.Write(Src_dpos_Z);
            writer.Write(DamageType);
            writer.Write(EgenFlags.FlagValueByte);
            writer.Write((short)0);
            writer.Write(ActiveTimeSeconds);
            writer.Write(OnAnim_AssetID);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
        
        public override bool HasReference(uint assetID) => OnAnim_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(OnAnim_AssetID, ref result);
        }
    }
}