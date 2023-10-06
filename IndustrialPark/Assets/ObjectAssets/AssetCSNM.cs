using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCSNM : BaseAsset
    {
        private const string categoryName = "Cutscene Manager";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Cutscene { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle InterpSpeed { get; set; }

        [Category(categoryName)]
        public AssetID Subtitles { get; set; }

        [Category(categoryName)]
        public AssetSingle StartTime1 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime2 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime3 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime4 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime5 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime6 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime7 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime8 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime9 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime10 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime11 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime12 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime13 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime14 { get; set; }
        [Category(categoryName)]
        public AssetSingle StartTime15 { get; set; }

        [Category(categoryName)]
        public AssetSingle EndTime1 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime2 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime3 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime4 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime5 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime6 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime7 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime8 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime9 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime10 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime11 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime12 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime13 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime14 { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime15 { get; set; }

        [Category(categoryName)]
        public AssetID Emitter1 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter2 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter3 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter4 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter5 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter6 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter7 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter8 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter9 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter10 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter11 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter12 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter13 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter14 { get; set; }
        [Category(categoryName)]
        public AssetID Emitter15 { get; set; }

        public AssetCSNM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Cutscene = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                InterpSpeed = reader.ReadSingle();
                if (game == Game.Incredibles)
                    Subtitles = reader.ReadUInt32();

                StartTime1 = reader.ReadSingle();
                StartTime2 = reader.ReadSingle();
                StartTime3 = reader.ReadSingle();
                StartTime4 = reader.ReadSingle();
                StartTime5 = reader.ReadSingle();
                StartTime6 = reader.ReadSingle();
                StartTime7 = reader.ReadSingle();
                StartTime8 = reader.ReadSingle();
                StartTime9 = reader.ReadSingle();
                StartTime10 = reader.ReadSingle();
                StartTime11 = reader.ReadSingle();
                StartTime12 = reader.ReadSingle();
                StartTime13 = reader.ReadSingle();
                StartTime14 = reader.ReadSingle();
                StartTime15 = reader.ReadSingle();

                EndTime1 = reader.ReadSingle();
                EndTime2 = reader.ReadSingle();
                EndTime3 = reader.ReadSingle();
                EndTime4 = reader.ReadSingle();
                EndTime5 = reader.ReadSingle();
                EndTime6 = reader.ReadSingle();
                EndTime7 = reader.ReadSingle();
                EndTime8 = reader.ReadSingle();
                EndTime9 = reader.ReadSingle();
                EndTime10 = reader.ReadSingle();
                EndTime11 = reader.ReadSingle();
                EndTime12 = reader.ReadSingle();
                EndTime13 = reader.ReadSingle();
                EndTime14 = reader.ReadSingle();
                EndTime15 = reader.ReadSingle();

                Emitter1 = reader.ReadUInt32();
                Emitter2 = reader.ReadUInt32();
                Emitter3 = reader.ReadUInt32();
                Emitter4 = reader.ReadUInt32();
                Emitter5 = reader.ReadUInt32();
                Emitter6 = reader.ReadUInt32();
                Emitter7 = reader.ReadUInt32();
                Emitter8 = reader.ReadUInt32();
                Emitter9 = reader.ReadUInt32();
                Emitter10 = reader.ReadUInt32();
                Emitter11 = reader.ReadUInt32();
                Emitter12 = reader.ReadUInt32();
                Emitter13 = reader.ReadUInt32();
                Emitter14 = reader.ReadUInt32();
                Emitter15 = reader.ReadUInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Cutscene);
            writer.Write(Flags.FlagValueInt);
            writer.Write(InterpSpeed);

            if (game == Game.Incredibles)
                writer.Write(Subtitles);

            writer.Write(StartTime1);
            writer.Write(StartTime2);
            writer.Write(StartTime3);
            writer.Write(StartTime4);
            writer.Write(StartTime5);
            writer.Write(StartTime6);
            writer.Write(StartTime7);
            writer.Write(StartTime8);
            writer.Write(StartTime9);
            writer.Write(StartTime10);
            writer.Write(StartTime11);
            writer.Write(StartTime12);
            writer.Write(StartTime13);
            writer.Write(StartTime14);
            writer.Write(StartTime15);

            writer.Write(EndTime1);
            writer.Write(EndTime2);
            writer.Write(EndTime3);
            writer.Write(EndTime4);
            writer.Write(EndTime5);
            writer.Write(EndTime6);
            writer.Write(EndTime7);
            writer.Write(EndTime8);
            writer.Write(EndTime9);
            writer.Write(EndTime10);
            writer.Write(EndTime11);
            writer.Write(EndTime12);
            writer.Write(EndTime13);
            writer.Write(EndTime14);
            writer.Write(EndTime15);

            writer.Write(Emitter1);
            writer.Write(Emitter2);
            writer.Write(Emitter3);
            writer.Write(Emitter4);
            writer.Write(Emitter5);
            writer.Write(Emitter6);
            writer.Write(Emitter7);
            writer.Write(Emitter8);
            writer.Write(Emitter9);
            writer.Write(Emitter10);
            writer.Write(Emitter11);
            writer.Write(Emitter12);
            writer.Write(Emitter13);
            writer.Write(Emitter14);
            writer.Write(Emitter15);
            SerializeLinks(writer);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
                dt.RemoveProperty("Subtitles");
            base.SetDynamicProperties(dt);
        }
    }
}