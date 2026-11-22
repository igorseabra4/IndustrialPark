using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaLogicFunctionGenerator : AssetDYNA
    {
        private const string dynaCategoryName = "logic:Function Generator";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetSingle StartCycleWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MiddleCycleWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EndCycleWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle StartPulseWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MiddlePulseWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EndPulseWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MiddleTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EndTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte MiddleEnabled { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte EndEnabled { get; set; }

        public DynaLogicFunctionGenerator(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.logic__FunctionGenerator, reader)
        {
            StartCycleWidth = reader.ReadSingle();
            MiddleCycleWidth = reader.ReadSingle();
            EndCycleWidth = reader.ReadSingle();
            StartPulseWidth = reader.ReadSingle();
            MiddlePulseWidth = reader.ReadSingle();
            EndPulseWidth = reader.ReadSingle();
            MiddleTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            MiddleEnabled = reader.ReadByte();
            EndEnabled = reader.ReadByte();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(StartCycleWidth);
            writer.Write(MiddleCycleWidth);
            writer.Write(EndCycleWidth);
            writer.Write(StartPulseWidth);
            writer.Write(MiddlePulseWidth);
            writer.Write(EndPulseWidth);
            writer.Write(MiddleTime);
            writer.Write(EndTime);
            writer.Write(MiddleEnabled);
            writer.Write(EndEnabled);
            writer.Write((short)0);


        }
    }
}