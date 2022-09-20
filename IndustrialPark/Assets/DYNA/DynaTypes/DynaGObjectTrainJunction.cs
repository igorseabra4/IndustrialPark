using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTrainJunction : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:train_junction";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID InSpline { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte IsInFromForward { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OutSpline1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Out1IsForward { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OutSpline2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Out2IsForward { get; set; }

        public DynaGObjectTrainJunction(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__train_junction, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                InSpline = reader.ReadUInt32();
                IsInFromForward = reader.ReadByte();
                reader.BaseStream.Position += 3;
                OutSpline1 = reader.ReadUInt32();
                Out1IsForward = reader.ReadByte();
                reader.BaseStream.Position += 3;
                OutSpline2 = reader.ReadUInt32();
                Out2IsForward = reader.ReadByte();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(InSpline);
                writer.Write(IsInFromForward);
                writer.Write(new byte[3]);
                writer.Write(OutSpline1);
                writer.Write(Out1IsForward);
                writer.Write(new byte[3]);
                writer.Write(OutSpline2);
                writer.Write(Out2IsForward);
                writer.Write(new byte[3]);

                return writer.ToArray();
            }
        }
    }
}