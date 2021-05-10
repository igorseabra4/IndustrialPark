using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ConditionalOperation
    {
        EQUAL_TO = 0,
        GREATER_THAN = 1,
        LESS_THAN = 2,
        GREATER_THAN_OR_EQUAL_TO = 3,
        LESS_THAN_OR_EQUAL_TO = 4,
        NOT_EQUAL_TO = 5
    }

    public enum ConditionalVariableBFBB : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        IsVibrationOn = 0x3B93C93F,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentLevelCollectable = 0x9653DA31,
        PatSocks = 0x18249056,
        TotalPatsSocks = 0xD1FEEEE2,
        ShinyObjects = 0xD6FCCFE7,
        GoldenSpatulas = 0xC7E0F71C,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C
    }

    public enum ConditionalVariableTSSM : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        IsVibrationOn = 0x3B93C93F,
        AreSubtitlesEnabled = 0xD1A7DE2C,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C,
        TimerSecondsLeft = 0x6897B48B,
        TimerMillisecondsLeft = 0xF4FE2282,
        IsMNUS = 0x649FA12A,
        DemoType = 0x0B9F22CF,
        GoofyGooberTokens = 0x43DD1E00,
        ManlinessPoints = 0xD8A29291,
        LevelTreasureChests = 0xFE31C583,
        PlayerCurrentHealth = 0x25CD9F4A,
        IsReferenceNULL = 0x1F5BAA4D,
        AlwaysPortal = 0x5B85F809
    }

    public class AssetCOND : BaseAsset
    {
        private const string catName = "Conditional";

        [Category(catName), DisplayName(catName)]
        public AssetID Conditional_Scooby { get; set; }
        [Category(catName), DisplayName(catName)]
        public ConditionalVariableBFBB Conditional_BFBB
        {
            get => (ConditionalVariableBFBB)(uint)Conditional_Scooby;
            set => Conditional_Scooby = (uint)value;
        }
        [Category(catName), DisplayName(catName)]
        public ConditionalVariableTSSM Conditional_TSSM
        {
            get => (ConditionalVariableTSSM)(uint)Conditional_Scooby;
            set => Conditional_Scooby = (uint)value;
        }
        [Category(catName)]
        public ConditionalOperation Operation { get; set; }
        [Category(catName)]
        public int EvaluationAmount { get; set; }
        [Category(catName)]
        public AssetID AssetUnderEvaluation { get; set; }

        public AssetCOND(string assetName) : base(assetName, AssetType.COND, BaseAssetType.Cond)
        {
        }

        public AssetCOND(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            EvaluationAmount = reader.ReadInt32();
            Conditional_Scooby = reader.ReadUInt32();
            Operation = (ConditionalOperation)reader.ReadUInt32();
            if (game != Game.Scooby)
                AssetUnderEvaluation = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(SerializeBase(endianness));

            writer.Write(EvaluationAmount);
            writer.Write(Conditional_Scooby);
            writer.Write((uint)Operation);
            if (game != Game.Scooby)
                writer.Write(AssetUnderEvaluation);

            writer.Write(SerializeLinks(endianness));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => AssetUnderEvaluation == assetID || Conditional_Scooby == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(AssetUnderEvaluation, ref result);

            if ((int)Operation > 6)
                result.Add("COND with unknown operation type: " + Operation.ToString());
            if (Conditional_Scooby == 0)
                result.Add("COND with Conditional set to 0");
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("Conditional_BFBB");
                dt.RemoveProperty("AssetUnderEvaluation");
            }
            else
                dt.RemoveProperty("ScoobyConditional");

            if (game == Game.Incredibles)
                dt.RemoveProperty("Conditional_BFBB");
            else
                dt.RemoveProperty("Conditional_TSSM");

            base.SetDynamicProperties(dt);
        }
    }
}