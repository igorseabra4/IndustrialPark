﻿using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum CONDOperation
    {
        EQUAL_TO = 0,
        GREATER_THAN = 1,
        LESS_THAN = 2,
        GREATER_THAN_OR_EQUAL_TO = 3,
        LESS_THAN_OR_EQUAL_TO = 4,
        NOT_EQUAL_TO = 5
    }

    public enum CONDVariable_BFBB : uint
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

    public enum CONDVariable_TSSM : uint
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
        public AssetCOND(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => AssetUnderEvaluation == assetID || ScoobyConditional == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(AssetUnderEvaluation, ref result);

            if ((int)Operation > 6)
                result.Add("COND with unknown operation type: " + Operation.ToString());
            if (ScoobyConditional == 0)
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

        protected override int EventStartOffset => game == Game.Scooby ? 0x14 : 0x18;

        [Category("Conditional"), DisplayName("Conditional")]
        public CONDVariable_BFBB Conditional_BFBB
        {
            get => (CONDVariable_BFBB)ReadUInt(0xC);
            set => Write(0xC, (uint)value);
        }

        [Category("Conditional"), DisplayName("Conditional")]
        public CONDVariable_TSSM Conditional_TSSM
        {
            get => (CONDVariable_TSSM)ReadUInt(0xC);
            set => Write(0xC, (uint)value);
        }

        [Category("Conditional"), DisplayName("Conditional")]
        public AssetID ScoobyConditional
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Conditional")]
        public CONDOperation Operation
        {
            get => (CONDOperation)ReadInt(0x10);
            set => Write(0x10, (int)value);
        }

        [Category("Conditional")]
        public int EvaluationAmount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Conditional")]
        public AssetID AssetUnderEvaluation
        {
            get => game == Game.Scooby ? 0 : ReadUInt(0x14);
            set
            {
                if (game != Game.Scooby)
                    Write(0x14, value);
            }
        }
    }
}