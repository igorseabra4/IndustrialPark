using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaAnalogDirection : AssetDYNA
    {
        private const string dynaCategoryName = "Analog Direction";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Stick { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OutputID1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OutputID2 { get; set; }

        public DynaAnalogDirection(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.AnalogDirection, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Stick = reader.ReadUInt32();
                OutputID1 = reader.ReadUInt32();
                OutputID2 = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Stick);
            writer.Write(OutputID1);
            writer.Write(OutputID2);
        }
    }
}