using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionTurn : AssetDYNA
    {
        private const string dynaCategoryName = "interaction:Turn";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID ForwardScript { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackwardScript { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TurnObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Increment { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TurnRate { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RestoreRate { get; set; }
        [Category(dynaCategoryName)]
        public int GoalIncrement { get; set; }
        [Category(dynaCategoryName)]
        public int MinIncrement { get; set; }
        [Category(dynaCategoryName)]
        public int MaxIncrement { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask TurnFlags { get; set; } = IntFlagsDescriptor();

        public DynaInteractionTurn(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.interaction__Turn, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ForwardScript = reader.ReadUInt32();
                BackwardScript = reader.ReadUInt32();
                TurnObject = reader.ReadUInt32();
                Increment = reader.ReadSingle();
                TurnRate = reader.ReadSingle();
                RestoreRate = reader.ReadSingle();
                GoalIncrement = reader.ReadInt32();
                MinIncrement = reader.ReadInt32();
                MaxIncrement = reader.ReadInt32();
                TurnFlags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(ForwardScript);
            writer.Write(BackwardScript);
            writer.Write(TurnObject);
            writer.Write(Increment);
            writer.Write(TurnRate);
            writer.Write(RestoreRate);
            writer.Write(GoalIncrement);
            writer.Write(MinIncrement);
            writer.Write(MaxIncrement);
            writer.Write(TurnFlags.FlagValueInt);


        }
    }
}