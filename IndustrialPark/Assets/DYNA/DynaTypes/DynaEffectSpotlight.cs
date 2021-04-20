using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSpotlight : AssetDYNA
    {
        private const string dynaCategoryName = "effect:spotlight";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public int UnknownInt_00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Origin_Entity_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Target_Entity_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14_Rad { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14_Deg
        {
            get => MathUtil.RadiansToDegrees(UnknownFloat_14_Rad);
            set => UnknownFloat_14_Rad = MathUtil.DegreesToRadians(value);
        }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_1D { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_1E { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_1F { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_20 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_24 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_28 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_2C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_30 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_34 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_35 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_36 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte UnknownByte_37 { get; set; }

        public DynaEffectSpotlight(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.effect__spotlight, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownInt_00 = reader.ReadInt32();
            Origin_Entity_AssetID = reader.ReadUInt32();
            Target_Entity_AssetID = reader.ReadUInt32();
            UnknownInt_0C = reader.ReadInt32();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14_Rad = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
            UnknownByte_1C = reader.ReadByte();
            UnknownByte_1D = reader.ReadByte();
            UnknownByte_1E = reader.ReadByte();
            UnknownByte_1F = reader.ReadByte();
            UnknownInt_20 = reader.ReadInt32();
            UnknownInt_24 = reader.ReadInt32();
            UnknownInt_28 = reader.ReadInt32();
            UnknownFloat_2C = reader.ReadSingle();
            UnknownFloat_30 = reader.ReadSingle();
            UnknownByte_34 = reader.ReadByte();
            UnknownByte_35 = reader.ReadByte();
            UnknownByte_36 = reader.ReadByte();
            UnknownByte_37 = reader.ReadByte();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UnknownInt_00);
            writer.Write(Origin_Entity_AssetID);
            writer.Write(Target_Entity_AssetID);
            writer.Write(UnknownInt_0C);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownFloat_14_Rad);
            writer.Write(UnknownFloat_18);
            writer.Write(UnknownByte_1C);
            writer.Write(UnknownByte_1D);
            writer.Write(UnknownByte_1E);
            writer.Write(UnknownByte_1F);
            writer.Write(UnknownInt_20);
            writer.Write(UnknownInt_24);
            writer.Write(UnknownInt_28);
            writer.Write(UnknownFloat_2C);
            writer.Write(UnknownFloat_30);
            writer.Write(UnknownByte_34);
            writer.Write(UnknownByte_35);
            writer.Write(UnknownByte_36);
            writer.Write(UnknownByte_37);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            Origin_Entity_AssetID == assetID || Target_Entity_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(Origin_Entity_AssetID, ref result);
            Verify(Target_Entity_AssetID, ref result);
        }
    }
}