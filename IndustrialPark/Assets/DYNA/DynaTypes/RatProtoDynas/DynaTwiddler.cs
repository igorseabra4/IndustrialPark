using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaTwiddler : AssetDYNA
    {
        private const string dynaCategoryName = "Twiddler";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(TwiddleeID);
        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID TwiddleeID { get; set; }
        [Category(dynaCategoryName)]
        public uint TwiddlerType { get; set; }
        [Category(dynaCategoryName)]
        public uint Axis { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InputMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InputMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OutputMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OutputMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DefaultValue { get; set; }

        public DynaTwiddler(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Twiddler, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                TwiddleeID = reader.ReadUInt32();
                TwiddlerType = reader.ReadUInt32();
                Axis = reader.ReadUInt32();
                InputMin = reader.ReadSingle();
                InputMax = reader.ReadSingle();
                OutputMin = reader.ReadSingle();
                OutputMax = reader.ReadSingle();
                DefaultValue = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(TwiddleeID);
            writer.Write(TwiddlerType);
            writer.Write(Axis);
            writer.Write(InputMin);
            writer.Write(InputMax);
            writer.Write(OutputMin);
            writer.Write(OutputMax);
            writer.Write(DefaultValue);
        }
    }
}