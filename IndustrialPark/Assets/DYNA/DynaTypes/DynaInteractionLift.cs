using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionLift : AssetDYNA
    {
        private const string dynaCategoryName = "interaction:Lift";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID ForwardScript { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackwardScript { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LiftObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StandPointer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ButtonFreq { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ProgressSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SlipSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FinalPos { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TossSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DropGravity { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask LiftFlags { get; set; } = IntFlagsDescriptor();

        public DynaInteractionLift(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.interaction__Lift, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ForwardScript = reader.ReadUInt32();
                BackwardScript = reader.ReadUInt32();
                LiftObject = reader.ReadUInt32();
                StandPointer = reader.ReadUInt32();
                ButtonFreq = reader.ReadSingle();
                ProgressSpeed = reader.ReadSingle();
                SlipSpeed = reader.ReadSingle();
                FinalPos = reader.ReadSingle();
                TossSpeed = reader.ReadSingle();
                DropGravity = reader.ReadSingle();
                LiftFlags.FlagValueInt = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(ForwardScript);
                writer.Write(BackwardScript);
                writer.Write(LiftObject);
                writer.Write(StandPointer);
                writer.Write(ButtonFreq);
                writer.Write(ProgressSpeed);
                writer.Write(SlipSpeed);
                writer.Write(FinalPos);
                writer.Write(TossSpeed);
                writer.Write(DropGravity);
                writer.Write(LiftFlags.FlagValueInt);

                return writer.ToArray();
            }
        }
    }
}