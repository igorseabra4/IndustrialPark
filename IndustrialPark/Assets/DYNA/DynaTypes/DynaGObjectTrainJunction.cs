using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTrainJunction : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:train_junction";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID InSpline_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte IsInFromForward { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Out1Spline_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Out1IsForward { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Out2Spline_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Out2IsForward { get; set; }

        public DynaGObjectTrainJunction(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__train_junction, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                InSpline_AssetID = reader.ReadUInt32();
                IsInFromForward = reader.ReadByte();
                reader.BaseStream.Position += 3;
                Out1Spline_AssetID = reader.ReadUInt32();
                Out1IsForward = reader.ReadByte();
                reader.BaseStream.Position += 3;
                Out2Spline_AssetID = reader.ReadUInt32();
                Out2IsForward = reader.ReadByte();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(InSpline_AssetID);
                writer.Write(IsInFromForward);
                writer.Write(new byte[3]);
                writer.Write(Out1Spline_AssetID);
                writer.Write(Out1IsForward);
                writer.Write(new byte[3]);
                writer.Write(Out2Spline_AssetID);
                writer.Write(Out2IsForward);
                writer.Write(new byte[3]);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (InSpline_AssetID == assetID)
                return true;
            if (Out1Spline_AssetID == assetID)
                return true;
            if (Out2Spline_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(InSpline_AssetID, ref result);
            Verify(Out1Spline_AssetID, ref result);
            Verify(Out2Spline_AssetID, ref result);
            base.Verify(ref result);
        }
    }
}