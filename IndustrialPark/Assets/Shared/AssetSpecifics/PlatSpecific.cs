using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class PlatSpecific_Generic : GenericAssetDataContainer
    {
        public PlatSpecific_Generic() { }
    }

    public class PlatSpecific_ConveryorBelt : PlatSpecific_Generic
    {
        public float Speed { get; set; }

        public PlatSpecific_ConveryorBelt() { }
        public PlatSpecific_ConveryorBelt(EndianBinaryReader reader)
        {
            Speed = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(Speed);
            return writer.ToArray();
        }
    }

    public class PlatSpecific_FallingPlatform : PlatSpecific_Generic
    {
        public float Speed { get; set; }
        public AssetID BustModel_AssetID { get; set; }

        public PlatSpecific_FallingPlatform()
        {
            BustModel_AssetID = 0;
        }
        public PlatSpecific_FallingPlatform(EndianBinaryReader reader)
        {
            Speed = reader.ReadSingle();
            BustModel_AssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(Speed);
            writer.Write(BustModel_AssetID);
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => BustModel_AssetID == assetID;
        public override void Verify(ref List<string> result) => Verify(BustModel_AssetID, ref result);
    }

    public class PlatSpecific_FR : PlatSpecific_Generic
    {
        public float fspeed { get; set; }
        public float rspeed { get; set; }
        public float ret_delay { get; set; }
        public float post_ret_delay { get; set; }

        public PlatSpecific_FR() { }
        public PlatSpecific_FR(EndianBinaryReader reader)
        {
            fspeed = reader.ReadSingle();
            rspeed = reader.ReadSingle();
            ret_delay = reader.ReadSingle();
            post_ret_delay = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(fspeed);
            writer.Write(rspeed);
            writer.Write(ret_delay);
            writer.Write(post_ret_delay);
            return writer.ToArray();
        }
    }

    public class PlatSpecific_BreakawayPlatform : PlatSpecific_Generic
    {
        public float BreakawayDelay { get; set; }
        [Description("Not present in Movie")]
        public AssetID BustModel_AssetID { get; set; }
        public float ResetDelay { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor("Allow sneak");
        [Description("Movie only")]
        public float UnknownFloat0C { get; set; }

        public PlatSpecific_BreakawayPlatform()
        {
            BustModel_AssetID = 0;
        }
        public PlatSpecific_BreakawayPlatform(EndianBinaryReader reader, Game game)
        {
            BreakawayDelay = reader.ReadSingle();
            if (game != Game.Incredibles)
                BustModel_AssetID = reader.ReadUInt32();
            ResetDelay = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
            if (game == Game.Incredibles)
                UnknownFloat0C = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(BreakawayDelay);
            if (game != Game.Incredibles)
                writer.Write(BustModel_AssetID);
            writer.Write(ResetDelay);
            writer.Write(Settings.FlagValueInt);
            if (game == Game.Incredibles)
                writer.Write(UnknownFloat0C);
            return writer.ToArray();
        }
    }

    public class PlatSpecific_Springboard : PlatSpecific_Generic
    {
        public float Height1 { get; set; }
        public float Height2 { get; set; }
        public float Height3 { get; set; }
        public float HeightBubbleBounce { get; set; }
        public AssetID Anim1_AssetID { get; set; }
        public AssetID Anim2_AssetID { get; set; }
        public AssetID Anim3_AssetID { get; set; }
        public float DirectionX { get; set; }
        public float DirectionY { get; set; }
        public float DirectionZ { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor(
            "Lock Camera Down",
            null,
            "Lock Player Control");

        public PlatSpecific_Springboard()
        {
            Anim1_AssetID = 0;
            Anim2_AssetID = 0;
            Anim3_AssetID = 0;
        }
        public PlatSpecific_Springboard(EndianBinaryReader reader) : this()
        {
            Height1 = reader.ReadSingle();
            Height2 = reader.ReadSingle();
            Height3 = reader.ReadSingle();
            HeightBubbleBounce = reader.ReadSingle();
            Anim1_AssetID = reader.ReadUInt32();
            Anim2_AssetID = reader.ReadUInt32();
            Anim3_AssetID = reader.ReadUInt32();
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Height1);
            writer.Write(Height2);
            writer.Write(Height3);
            writer.Write(HeightBubbleBounce);
            writer.Write(Anim1_AssetID);
            writer.Write(Anim2_AssetID);
            writer.Write(Anim3_AssetID);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
            writer.Write(Settings.FlagValueInt);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Anim1_AssetID == assetID || Anim2_AssetID == assetID || Anim3_AssetID == assetID;

        public override void Verify(ref List<string> result)
        {
            Verify(Anim1_AssetID, ref result);
            Verify(Anim2_AssetID, ref result);
            Verify(Anim3_AssetID, ref result);
        }
    }

    public class PlatSpecific_TeeterTotter : PlatSpecific_Generic
    {
        private float InitialTilt_Rad { get; set; }
        private float MaxTilt_Rad { get; set; }
        public float InitialTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(InitialTilt_Rad);
            set => InitialTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public float MaxTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(MaxTilt_Rad);
            set => MaxTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public float InverseMass { get; set; }

        public PlatSpecific_TeeterTotter() { }
        public PlatSpecific_TeeterTotter(EndianBinaryReader reader)
        {
            InitialTilt_Rad = reader.ReadSingle();
            MaxTilt_Rad = reader.ReadSingle();
            InverseMass = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(InitialTilt_Rad);
            writer.Write(MaxTilt_Rad);
            writer.Write(InverseMass);

            return writer.ToArray();
        }
    }

    public class PlatSpecific_Paddle : PlatSpecific_Generic
    {
        public int StartOrient { get; set; }
        public int OrientCount { get; set; }
        public float OrientLoop { get; set; }
        public float Orient1 { get; set; }
        public float Orient2 { get; set; }
        public float Orient3 { get; set; }
        public float Orient4 { get; set; }
        public float Orient5 { get; set; }
        public float Orient6 { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor();
        public float RotateSpeed { get; set; }
        public float AccelTime { get; set; }
        public float DecelTime { get; set; }
        public float HubRadius { get; set; }

        public PlatSpecific_Paddle() { }
        public PlatSpecific_Paddle(EndianBinaryReader reader)
        {
            StartOrient = reader.ReadInt32();
            OrientCount = reader.ReadInt32();
            OrientLoop = reader.ReadSingle();
            Orient1 = reader.ReadSingle();
            Orient2 = reader.ReadSingle();
            Orient3 = reader.ReadSingle();
            Orient4 = reader.ReadSingle();
            Orient5 = reader.ReadSingle();
            Orient6 = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
            RotateSpeed = reader.ReadSingle();
            AccelTime = reader.ReadSingle();
            DecelTime = reader.ReadSingle();
            HubRadius = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(StartOrient);
            writer.Write(OrientCount);
            writer.Write(OrientLoop);
            writer.Write(Orient1);
            writer.Write(Orient2);
            writer.Write(Orient3);
            writer.Write(Orient4);
            writer.Write(Orient5);
            writer.Write(Orient6);
            writer.Write(Settings.FlagValueInt);
            writer.Write(RotateSpeed);
            writer.Write(AccelTime);
            writer.Write(DecelTime);
            writer.Write(HubRadius);

            return writer.ToArray();
        }
    }
}