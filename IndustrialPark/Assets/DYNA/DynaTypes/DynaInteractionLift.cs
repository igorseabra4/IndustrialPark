using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionLift : AssetDYNA
    {
        private const string dynaCategoryName = "interaction:Lift";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID ForwardScript_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackwardScript_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LiftObject_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StandPointer_AssetID { get; set; }
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

                ForwardScript_AssetID = reader.ReadUInt32();
                BackwardScript_AssetID = reader.ReadUInt32();
                LiftObject_AssetID = reader.ReadUInt32();
                StandPointer_AssetID = reader.ReadUInt32();
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
                writer.Write(ForwardScript_AssetID);
                writer.Write(BackwardScript_AssetID);
                writer.Write(LiftObject_AssetID);
                writer.Write(StandPointer_AssetID);
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

        public override bool HasReference(uint assetID) => ForwardScript_AssetID == assetID || BackwardScript_AssetID == assetID ||
            LiftObject_AssetID == assetID || StandPointer_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(ForwardScript_AssetID, ref result);
            Verify(BackwardScript_AssetID, ref result);
            Verify(LiftObject_AssetID, ref result);
            Verify(StandPointer_AssetID, ref result);
            base.Verify(ref result);
        }
    }
}