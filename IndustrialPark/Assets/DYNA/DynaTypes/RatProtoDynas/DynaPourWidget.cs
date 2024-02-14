using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaPourWidget : AssetDYNA
    {
        private const string dynaCategoryName = "Pour Widget";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetSingle Volume { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FillInputID { get; set; }
        [Category(dynaCategoryName)]
        public uint FillInputType { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FillInputMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FillInputMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FillFlowRate { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TiltInputID { get; set; }
        [Category(dynaCategoryName)]
        public uint TiltInputType { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TiltInputMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TiltInputMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FillWeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpillWeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FoamWeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetID ScoreCounterID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FlowOutputID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FillOutputID { get; set; }

        public DynaPourWidget(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Pour_Widget, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Volume = reader.ReadSingle();
                FillInputID = reader.ReadUInt32();
                FillInputType = reader.ReadUInt32();
                FillInputMin = reader.ReadSingle();
                FillInputMax = reader.ReadSingle();
                FillFlowRate = reader.ReadSingle();
                TiltInputID = reader.ReadUInt32();
                TiltInputType = reader.ReadUInt32();
                TiltInputMin = reader.ReadSingle();
                TiltInputMax = reader.ReadSingle();
                FillWeight = reader.ReadSingle();
                SpillWeight = reader.ReadSingle();
                FoamWeight = reader.ReadSingle();
                ScoreCounterID = reader.ReadUInt32();
                FlowOutputID = reader.ReadUInt32();
                FillOutputID = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Volume);
            writer.Write(FillInputID);
            writer.Write(FillInputType);
            writer.Write(FillInputMin);
            writer.Write(FillInputMax);
            writer.Write(FillFlowRate);
            writer.Write(TiltInputID);
            writer.Write(TiltInputType);
            writer.Write(TiltInputMin);
            writer.Write(TiltInputMax);
            writer.Write(FillWeight);
            writer.Write(SpillWeight);
            writer.Write(FoamWeight);
            writer.Write(ScoreCounterID);
            writer.Write(FlowOutputID);
            writer.Write(FillOutputID);
        }
    }
}