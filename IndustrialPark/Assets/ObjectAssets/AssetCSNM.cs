using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCSNM : BaseAsset
    {
        private const string categoryName = "Cutscene Manager";

        [Category(categoryName)]
        public AssetID CSN_AssetID { get; set; }
        [Category(categoryName)]
        public FlagBitmask CsnmFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public int InterpSpeed { get; set; }
        [Category(categoryName)]
        public int UnknownInt14 { get; set; }
        [Category(categoryName)]
        public int UnknownInt18 { get; set; }
        [Category(categoryName)]
        public int UnknownInt1C { get; set; }
        [Category(categoryName)]
        public int UnknownInt20 { get; set; }
        [Category(categoryName)]
        public int UnknownInt24 { get; set; }
        [Category(categoryName)]
        public int UnknownInt28 { get; set; }
        [Category(categoryName)]
        public int UnknownInt2C { get; set; }
        [Category(categoryName)]
        public int UnknownInt30 { get; set; }
        [Category(categoryName)]
        public int UnknownInt34 { get; set; }
        [Category(categoryName)]
        public int UnknownInt38 { get; set; }
        [Category(categoryName)]
        public int UnknownInt3C { get; set; }
        [Category(categoryName)]
        public int UnknownInt40 { get; set; }
        [Category(categoryName)]
        public int UnknownInt44 { get; set; }
        [Category(categoryName)]
        public int UnknownInt48 { get; set; }
        [Category(categoryName)]
        public int UnknownInt4C { get; set; }
        [Category(categoryName)]
        public int UnknownInt50 { get; set; }
        [Category(categoryName)]
        public int UnknownInt54 { get; set; }
        [Category(categoryName)]
        public int UnknownInt58 { get; set; }
        [Category(categoryName)]
        public int UnknownInt5C { get; set; }
        [Category(categoryName)]
        public int UnknownInt60 { get; set; }
        [Category(categoryName)]
        public int UnknownInt64 { get; set; }
        [Category(categoryName)]
        public int UnknownInt68 { get; set; }
        [Category(categoryName)]
        public int UnknownInt6C { get; set; }
        [Category(categoryName)]
        public int UnknownInt70 { get; set; }
        [Category(categoryName)]
        public int UnknownInt74 { get; set; }
        [Category(categoryName)]
        public int UnknownInt78 { get; set; }
        [Category(categoryName)]
        public int UnknownInt7C { get; set; }
        [Category(categoryName)]
        public int UnknownInt80 { get; set; }
        [Category(categoryName)]
        public int UnknownInt84 { get; set; }
        [Category(categoryName)]
        public int UnknownInt88 { get; set; }
        [Category(categoryName)]
        public int UnknownInt8C { get; set; }
        [Category(categoryName)]
        public int UnknownInt90 { get; set; }
        [Category(categoryName)]
        public int UnknownInt94 { get; set; }
        [Category(categoryName)]
        public int UnknownInt98 { get; set; }
        [Category(categoryName)]
        public int UnknownInt9C { get; set; }
        [Category(categoryName)]
        public int UnknownIntA0 { get; set; }
        [Category(categoryName)]
        public int UnknownIntA4 { get; set; }
        [Category(categoryName)]
        public int UnknownIntA8 { get; set; }
        [Category(categoryName)]
        public int UnknownIntAC { get; set; }
        [Category(categoryName)]
        public int UnknownIntB0 { get; set; }
        [Category(categoryName)]
        public int UnknownIntB4 { get; set; }
        [Category(categoryName)]
        public int UnknownIntB8 { get; set; }
        [Category(categoryName)]
        public int UnknownIntBC { get; set; }
        [Category(categoryName)]
        public int UnknownIntC0 { get; set; }
        [Category(categoryName)]
        public int UnknownIntC4 { get; set; }
        [Category(categoryName)] // incredibles only
        public int UnknownIntC8 { get; set; }

        public AssetCSNM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                CSN_AssetID = reader.ReadUInt32();
                CsnmFlags.FlagValueInt = reader.ReadUInt32();
                InterpSpeed = reader.ReadInt32();
                UnknownInt14 = reader.ReadInt32();
                UnknownInt18 = reader.ReadInt32();
                UnknownInt1C = reader.ReadInt32();
                UnknownInt20 = reader.ReadInt32();
                UnknownInt24 = reader.ReadInt32();
                UnknownInt28 = reader.ReadInt32();
                UnknownInt2C = reader.ReadInt32();
                UnknownInt30 = reader.ReadInt32();
                UnknownInt34 = reader.ReadInt32();
                UnknownInt38 = reader.ReadInt32();
                UnknownInt3C = reader.ReadInt32();
                UnknownInt40 = reader.ReadInt32();
                UnknownInt44 = reader.ReadInt32();
                UnknownInt48 = reader.ReadInt32();
                UnknownInt4C = reader.ReadInt32();
                UnknownInt50 = reader.ReadInt32();
                UnknownInt54 = reader.ReadInt32();
                UnknownInt58 = reader.ReadInt32();
                UnknownInt5C = reader.ReadInt32();
                UnknownInt60 = reader.ReadInt32();
                UnknownInt64 = reader.ReadInt32();
                UnknownInt68 = reader.ReadInt32();
                UnknownInt6C = reader.ReadInt32();
                UnknownInt70 = reader.ReadInt32();
                UnknownInt74 = reader.ReadInt32();
                UnknownInt78 = reader.ReadInt32();
                UnknownInt7C = reader.ReadInt32();
                UnknownInt80 = reader.ReadInt32();
                UnknownInt84 = reader.ReadInt32();
                UnknownInt88 = reader.ReadInt32();
                UnknownInt8C = reader.ReadInt32();
                UnknownInt90 = reader.ReadInt32();
                UnknownInt94 = reader.ReadInt32();
                UnknownInt98 = reader.ReadInt32();
                UnknownInt9C = reader.ReadInt32();
                UnknownIntA0 = reader.ReadInt32();
                UnknownIntA4 = reader.ReadInt32();
                UnknownIntA8 = reader.ReadInt32();
                UnknownIntAC = reader.ReadInt32();
                UnknownIntB0 = reader.ReadInt32();
                UnknownIntB4 = reader.ReadInt32();
                UnknownIntB8 = reader.ReadInt32();
                UnknownIntBC = reader.ReadInt32();
                UnknownIntC0 = reader.ReadInt32();
                UnknownIntC4 = reader.ReadInt32();
                if (game == Game.Incredibles)
                    UnknownIntC8 = reader.ReadInt32();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(CSN_AssetID);
                writer.Write(CsnmFlags.FlagValueInt);
                writer.Write(InterpSpeed);
                writer.Write(UnknownInt14);
                writer.Write(UnknownInt18);
                writer.Write(UnknownInt1C);
                writer.Write(UnknownInt20);
                writer.Write(UnknownInt24);
                writer.Write(UnknownInt28);
                writer.Write(UnknownInt2C);
                writer.Write(UnknownInt30);
                writer.Write(UnknownInt34);
                writer.Write(UnknownInt38);
                writer.Write(UnknownInt3C);
                writer.Write(UnknownInt40);
                writer.Write(UnknownInt44);
                writer.Write(UnknownInt48);
                writer.Write(UnknownInt4C);
                writer.Write(UnknownInt50);
                writer.Write(UnknownInt54);
                writer.Write(UnknownInt58);
                writer.Write(UnknownInt5C);
                writer.Write(UnknownInt60);
                writer.Write(UnknownInt64);
                writer.Write(UnknownInt68);
                writer.Write(UnknownInt6C);
                writer.Write(UnknownInt70);
                writer.Write(UnknownInt74);
                writer.Write(UnknownInt78);
                writer.Write(UnknownInt7C);
                writer.Write(UnknownInt80);
                writer.Write(UnknownInt84);
                writer.Write(UnknownInt88);
                writer.Write(UnknownInt8C);
                writer.Write(UnknownInt90);
                writer.Write(UnknownInt94);
                writer.Write(UnknownInt98);
                writer.Write(UnknownInt9C);
                writer.Write(UnknownIntA0);
                writer.Write(UnknownIntA4);
                writer.Write(UnknownIntA8);
                writer.Write(UnknownIntAC);
                writer.Write(UnknownIntB0);
                writer.Write(UnknownIntB4);
                writer.Write(UnknownIntB8);
                writer.Write(UnknownIntBC);
                writer.Write(UnknownIntC0);
                writer.Write(UnknownIntC4);
                if (game == Game.Incredibles)
                    writer.Write(UnknownIntC8);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => CSN_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (CSN_AssetID == 0)
                result.Add("CNSM with CSN_AssetID set to 0");
            Verify(CSN_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
                dt.RemoveProperty("UnknownIntC8");
            base.SetDynamicProperties(dt);
        }
    }
}