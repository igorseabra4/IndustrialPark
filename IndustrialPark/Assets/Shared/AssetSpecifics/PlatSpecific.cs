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
        public AssetSingle Speed { get; set; }

        public PlatSpecific_ConveryorBelt() { }
        public PlatSpecific_ConveryorBelt(EndianBinaryReader reader)
        {
            Speed = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Speed);
                return writer.ToArray();
            }
        }
    }

    public class PlatSpecific_FallingPlatform : PlatSpecific_Generic
    {
        public AssetSingle Speed { get; set; }
        public AssetID BustModel { get; set; }

        public PlatSpecific_FallingPlatform()
        {
            BustModel = 0;
        }
        public PlatSpecific_FallingPlatform(EndianBinaryReader reader)
        {
            Speed = reader.ReadSingle();
            BustModel = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Speed);
                writer.Write(BustModel);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result) => Verify(BustModel, ref result);
    }

    public class PlatSpecific_FR : PlatSpecific_Generic
    {
        public AssetSingle fspeed { get; set; }
        public AssetSingle rspeed { get; set; }
        public AssetSingle ret_delay { get; set; }
        public AssetSingle post_ret_delay { get; set; }

        public PlatSpecific_FR() { }
        public PlatSpecific_FR(EndianBinaryReader reader)
        {
            fspeed = reader.ReadSingle();
            rspeed = reader.ReadSingle();
            ret_delay = reader.ReadSingle();
            post_ret_delay = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(fspeed);
                writer.Write(rspeed);
                writer.Write(ret_delay);
                writer.Write(post_ret_delay);
                return writer.ToArray();
            }
        }
    }

    public class PlatSpecific_BreakawayPlatform : PlatSpecific_Generic
    {
        public AssetSingle BreakawayDelay { get; set; }
        [Description("Not present in Movie")]
        public AssetID BustModel { get; set; }
        public AssetSingle ResetDelay { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor("Allow sneak");
        [Description("Incredibles only")]
        public AssetSingle UnknownFloat0C { get; set; }

        public PlatSpecific_BreakawayPlatform()
        {
            BustModel = 0;
        }

        public PlatSpecific_BreakawayPlatform(AssetTemplate template)
        {
            BreakawayDelay = 1f;
            ResetDelay = 3f;
            Settings.FlagValueInt = 1;
            UnknownFloat0C = 0.1f;

            if (template == AssetTemplate.CollapsePlatform_Spongeball)
                BreakawayDelay = 0.4f;
        }

        public PlatSpecific_BreakawayPlatform(EndianBinaryReader reader, Game game)
        {
            BreakawayDelay = reader.ReadSingle();
            if (game != Game.Incredibles)
                BustModel = reader.ReadUInt32();
            ResetDelay = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
            if (game == Game.Incredibles)
                UnknownFloat0C = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(BreakawayDelay);
                if (game != Game.Incredibles)
                    writer.Write(BustModel);
                writer.Write(ResetDelay);
                writer.Write(Settings.FlagValueInt);
                if (game == Game.Incredibles)
                    writer.Write(UnknownFloat0C);
                return writer.ToArray();
            }
        }
    }

    public class PlatSpecific_Springboard : PlatSpecific_Generic
    {
        public AssetSingle Height1 { get; set; }
        public AssetSingle Height2 { get; set; }
        public AssetSingle Height3 { get; set; }
        [Description("Not present in Scooby")]
        public AssetSingle HeightBubbleBounce { get; set; }
        public AssetID Animation1 { get; set; }
        public AssetID Animation2 { get; set; }
        public AssetID Animation3 { get; set; }
        public AssetSingle DirectionX { get; set; }
        public AssetSingle DirectionY { get; set; }
        public AssetSingle DirectionZ { get; set; }
        [Description("Not present in Scooby")]
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor(
            "Lock Camera Down",
            null,
            "Lock Player Control");

        public PlatSpecific_Springboard()
        {
            Animation1 = 0;
            Animation2 = 0;
            Animation3 = 0;
        }

        public PlatSpecific_Springboard(EndianBinaryReader reader, Game game) : this()
        {
            Height1 = reader.ReadSingle();
            Height2 = reader.ReadSingle();
            Height3 = reader.ReadSingle();
            if (game != Game.Scooby)
                HeightBubbleBounce = reader.ReadSingle();
            Animation1 = reader.ReadUInt32();
            Animation2 = reader.ReadUInt32();
            Animation3 = reader.ReadUInt32();
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            if (game != Game.Scooby)
                Settings.FlagValueInt = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Height1);
                writer.Write(Height2);
                writer.Write(Height3);
                if (game != Game.Scooby)
                    writer.Write(HeightBubbleBounce);
                writer.Write(Animation1);
                writer.Write(Animation2);
                writer.Write(Animation3);
                writer.Write(DirectionX);
                writer.Write(DirectionY);
                writer.Write(DirectionZ);
                if (game != Game.Scooby)
                    writer.Write(Settings.FlagValueInt);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Animation1, ref result);
            Verify(Animation2, ref result);
            Verify(Animation3, ref result);
        }
    }

    public class PlatSpecific_TeeterTotter : PlatSpecific_Generic
    {
        private float InitialTilt_Rad { get; set; }
        private float MaxTilt_Rad { get; set; }
        public AssetSingle InitialTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(InitialTilt_Rad);
            set => InitialTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public AssetSingle MaxTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(MaxTilt_Rad);
            set => MaxTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public AssetSingle InverseMass { get; set; }

        public PlatSpecific_TeeterTotter() { }
        public PlatSpecific_TeeterTotter(EndianBinaryReader reader)
        {
            InitialTilt_Rad = reader.ReadSingle();
            MaxTilt_Rad = reader.ReadSingle();
            InverseMass = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(InitialTilt_Rad);
                writer.Write(MaxTilt_Rad);
                writer.Write(InverseMass);

                return writer.ToArray();
            }
        }
    }

    public class PlatSpecific_Paddle : PlatSpecific_Generic
    {
        public int StartOrient { get; set; }
        public int OrientCount { get; set; }
        public AssetSingle OrientLoop { get; set; }
        public AssetSingle Orient1 { get; set; }
        public AssetSingle Orient2 { get; set; }
        public AssetSingle Orient3 { get; set; }
        public AssetSingle Orient4 { get; set; }
        public AssetSingle Orient5 { get; set; }
        public AssetSingle Orient6 { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor();
        public AssetSingle RotateSpeed { get; set; }
        public AssetSingle AccelTime { get; set; }
        public AssetSingle DecelTime { get; set; }
        public AssetSingle HubRadius { get; set; }

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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
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
}