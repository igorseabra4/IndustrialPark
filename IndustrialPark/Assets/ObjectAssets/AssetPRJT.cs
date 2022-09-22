using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPRJT : BaseAsset
    {
        private const string categoryName = "Projectile";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Model);

        [Category(categoryName)]
        public int EffectType { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public AssetID Animation { get; set; }
        [Category(categoryName)]
        public AssetID AtRestModel { get; set; }
        [Category(categoryName)]
        public AssetID AtRestAnimation { get; set; }
        [Category(categoryName)]
        public int DestructEnabled { get; set; }
        [Category(categoryName)]
        public AssetSingle DestructTime { get; set; }
        [Category(categoryName)]
        public AssetSingle DestructDist { get; set; }
        [Category(categoryName)]
        public int Oriented { get; set; }
        [Category(categoryName)]
        public int ExtraSpace1 { get; set; }
        [Category(categoryName)]
        public int ExtraSpace2 { get; set; }
        [Category(categoryName)]
        public int ExtraSpace3 { get; set; }
        [Category(categoryName)]
        public int ExtraSpace4 { get; set; }
        [Category(categoryName)]
        public int ExtraSpace5 { get; set; }
        [Category(categoryName)]
        public int ExtraSpace6 { get; set; }

        public AssetPRJT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                EffectType = reader.ReadInt32();
                Model = reader.ReadUInt32();
                Animation = reader.ReadUInt32();
                AtRestModel = reader.ReadUInt32();
                AtRestAnimation = reader.ReadUInt32();
                DestructEnabled = reader.ReadInt32();
                DestructTime = reader.ReadSingle();
                DestructDist = reader.ReadSingle();
                Oriented = reader.ReadInt32();
                ExtraSpace1 = reader.ReadInt32();
                ExtraSpace2 = reader.ReadInt32();
                ExtraSpace3 = reader.ReadInt32();
                ExtraSpace4 = reader.ReadInt32();
                ExtraSpace5 = reader.ReadInt32();
                ExtraSpace6 = reader.ReadInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

                base.Serialize(writer);
                writer.Write(EffectType);
                writer.Write(Model);
                writer.Write(Animation);
                writer.Write(AtRestModel);
                writer.Write(AtRestAnimation);
                writer.Write(DestructEnabled);
                writer.Write(DestructTime);
                writer.Write(DestructDist);
                writer.Write(Oriented);
                writer.Write(ExtraSpace1);
                writer.Write(ExtraSpace2);
                writer.Write(ExtraSpace3);
                writer.Write(ExtraSpace4);
                writer.Write(ExtraSpace5);
                writer.Write(ExtraSpace6);
                SerializeLinks(writer);
                
        }
    }
}