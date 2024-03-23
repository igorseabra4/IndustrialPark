using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectCamTweak : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Camera_Tweak";
        public override string TypeString => dynaCategoryName;
        public override string Note => "Version is always 1 for BFBB-Incredibles and 2 in RatProto";

        [Category(dynaCategoryName)]
        public int Priority { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Time { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PitchAdjust { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DistAdjust { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PivotHeightAdjust { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FovAdjust { get; set; }

        public DynaGObjectCamTweak(string assetName, int version) : base(assetName, DynaType.game_object__Camera_Tweak, (short)version)
        {
        }
        public DynaGObjectCamTweak(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Camera_Tweak, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Priority = reader.ReadInt32();
                Time = reader.ReadSingle();
                PitchAdjust = reader.ReadSingle();
                DistAdjust = reader.ReadSingle();
                if (Version == 2)
                {
                    PivotHeightAdjust = reader.ReadSingle();
                    FovAdjust = reader.ReadSingle();
                }
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(Priority);
            writer.Write(Time);
            writer.Write(PitchAdjust);
            writer.Write(DistAdjust);
            if (Version == 2)
            {
                writer.Write(PivotHeightAdjust);
                writer.Write(FovAdjust);
            }

        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (Version == 1)
            {
                dt.RemoveProperty("PivotHeightAdjust");
                dt.RemoveProperty("FovAdjust");
            }
        }
    }
}