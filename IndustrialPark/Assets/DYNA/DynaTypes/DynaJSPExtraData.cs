using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaJSPExtraData : AssetDYNA
    {
        private const string dynaCategoryName = "JSP Extra Data";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(JSPInfo)} {HexUIntTypeConverter.StringFromAssetID(Group)}";

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID JSPInfo { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Group { get; set; }

        public DynaJSPExtraData(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.JSPExtraData, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                JSPInfo = reader.ReadUInt32();
                Group = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(JSPInfo);
                writer.Write(Group);

                
        }
    }
}