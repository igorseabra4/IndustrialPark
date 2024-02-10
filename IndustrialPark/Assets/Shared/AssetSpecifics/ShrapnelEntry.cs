using DiscordRPC;
using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum IShrapnelType : int
    {
        Inactive = 0,
        Group = 1,
        Shrapnel = 2,
        Particle = 3,
        Projectile = 4,
        Lightning = 5,
        Sound = 6,
        Shockwave = 7,
        Explosion = 8,
        Distortion = 9,
        Fire = 10,
        Light = 11,
        Smoke = 12,
        Goo = 13
    }

    public abstract class Shrapnel : GenericAssetDataContainer
    {
        private const string categoryName = "\tFragAsset";

        [Category(categoryName), ReadOnly(true)]
        public IShrapnelType ShrapnelType { get; set; }
        [Category(categoryName)]
        public AssetID ID { get; set; }
        [Category(categoryName)]
        public AssetID ParentID { get; set; }
        [Category(categoryName)]
        public AssetID ParentID2 { get; set; }
        [Category(categoryName)]
        public AssetSingle Lifetime { get; set; }
        [Category(categoryName)]
        public AssetSingle Delay { get; set; }

        public Shrapnel(Game game, IShrapnelType type)
        {
            _game = game;
            ShrapnelType = type;
        }

        public Shrapnel(Game game, IShrapnelType type, EndianBinaryReader reader) : this(game, type)
        {
            ID = reader.ReadUInt32();
            ParentID = reader.ReadUInt32();
            ParentID2 = reader.ReadUInt32();
            Lifetime = reader.ReadSingle();
            Delay = reader.ReadSingle();
        }

        public void SerializeEntryShrpBase(EndianBinaryWriter writer)
        {
            writer.Write((int)ShrapnelType);
            writer.Write(ID);
            writer.Write(ParentID);
            writer.Write(ParentID2);
            writer.Write(Lifetime);
            writer.Write(Delay);
        }

        private byte padB => (byte)((game >= Game.ROTU) ? 0x00 : 0xCD);

        protected void ReadPad(EndianBinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
                if (reader.ReadByte() != padB)
                    throw new Exception("Error reading SHRP padding: non-padding byte found at " + (reader.BaseStream.Position - 1).ToString("x"));
        }

        protected void WritePad(EndianBinaryWriter writer, int count)
        {
            for (int i = 0; i < count; i++)
                writer.Write(padB);
        }

        protected static ShrapnelLocation ShrapnelLocation(Game game) =>
            game >= Game.Incredibles ? new ShrapnelLocation_TSSM(game) : new ShrapnelLocation(game);
        protected static ShrapnelLocation ShrapnelLocation(EndianBinaryReader reader, Game game) =>
            game >= Game.Incredibles ? new ShrapnelLocation_TSSM(reader, game) : new ShrapnelLocation(reader, game);

        public override string ToString()
        {
            return $"{ShrapnelType}";
        }
    }

    public class ShrapnelEntry_Particle : Shrapnel
    {
        private const string categoryName = "Particle";

        [Category(categoryName)]
        public ShrapnelLocation Source { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Velocity { get; set; }
        [Category("xParEmitterCustomSettings")]
        public byte Unknown1 { get; set; }
        [Category("xParEmitterCustomSettings")]
        public short Unknown2 { get; set; }
        [Category("xParEmitterCustomSettings")]
        public short Unknown3 { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetID ParticleEmitter { get; set; }
        public ShrapnelEntry_Particle(Game game) : base(game, IShrapnelType.Particle)
        {
            Source = ShrapnelLocation(game);
            Velocity = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Particle(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Particle, reader)
        {
            Source = ShrapnelLocation(reader, game);
            Velocity = ShrapnelLocation(reader, game);
            ReadPad(reader, 0x4);
            Unknown1 = reader.ReadByte();
            ReadPad(reader, 0x133);
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadInt16();

            if (game >= Game.Incredibles)
                ReadPad(reader, 0x48);

            if (game == Game.Incredibles)
                Flags.FlagValueInt = reader.ReadUInt32();

            if (game < Game.Incredibles)
                ReadPad(reader, 0x30);

            ParticleEmitter = reader.ReadUInt32();
            reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            Source.Serialize(writer);
            Velocity.Serialize(writer);
            WritePad(writer, 0x4);
            writer.Write(Unknown1);
            WritePad(writer, 0x133);
            writer.Write(Unknown2);
            writer.Write(Unknown3);

            if (game >= Game.Incredibles)
                WritePad(writer, 0x48);

            if (game == Game.Incredibles)
                writer.Write(Flags.FlagValueInt);

            if (game < Game.Incredibles)
                WritePad(writer, 0x30);

            writer.Write(ParticleEmitter);
            writer.Write(0);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
                dt.RemoveProperty("Flags");
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(ParticleEmitter)}]";
        }
    }

    public class ShrapnelEntry_Projectile : Shrapnel
    {
        private const string categoryName = "Projectile";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Launch { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Velocity { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation VelocityPlusMinus { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Rotation { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation RotationPlusMinus { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation InitRot { get; set; }
        [Category(categoryName)]
        public AssetSingle Bounce { get; set; }
        [Category(categoryName)]
        public int MaxBounces { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetID ChildID { get; set; }
        [Category(categoryName)]
        public AssetSingle MinScale { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxScale { get; set; }
        [Category(categoryName)]
        public AssetID ScaleCurveID { get; set; }
        [Category(categoryName)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryName)]
        public ShrapnelLightInfo LightInfo { get; set; }

        public ShrapnelEntry_Projectile(Game game) : base(game, IShrapnelType.Projectile)
        {
            Launch = ShrapnelLocation(game);
            Velocity = ShrapnelLocation(game);
            VelocityPlusMinus = ShrapnelLocation(game);
            Rotation = ShrapnelLocation(game);
            RotationPlusMinus = ShrapnelLocation(game);
            InitRot = ShrapnelLocation(game);
            LightInfo = new ShrapnelLightInfo();
        }

        public ShrapnelEntry_Projectile(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Projectile, reader)
        {
            Model = reader.ReadUInt32();
            reader.ReadInt32();
            Launch = ShrapnelLocation(reader, game);
            Velocity = ShrapnelLocation(reader, game);

            if (game >= Game.Incredibles)
            {
                VelocityPlusMinus = ShrapnelLocation(reader, game);
                Rotation = ShrapnelLocation(reader, game);
                RotationPlusMinus = ShrapnelLocation(reader, game);
            }
            else
            {
                VelocityPlusMinus = ShrapnelLocation(game);
                Rotation = ShrapnelLocation(game);
                RotationPlusMinus = ShrapnelLocation(game);
            }
            if (game >= Game.ROTU)
                InitRot = ShrapnelLocation(reader, game);
            else
                InitRot = ShrapnelLocation(game);

            Bounce = reader.ReadSingle();
            MaxBounces = reader.ReadInt32();
            Flags.FlagValueInt = reader.ReadUInt32();
            ChildID = reader.ReadUInt32();
            reader.ReadInt32();
            MinScale = reader.ReadSingle();
            MaxScale = reader.ReadSingle();
            ScaleCurveID = reader.ReadUInt32();
            reader.ReadInt32();
            Gravity = reader.ReadSingle();

            if (game >= Game.ROTU)
                LightInfo = new ShrapnelLightInfo(reader);
            else
                LightInfo = new ShrapnelLightInfo();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(Model);
            writer.Write(0);
            Launch.Serialize(writer);
            Velocity.Serialize(writer);

            if (game >= Game.Incredibles)
            {
                VelocityPlusMinus.Serialize(writer);
                Rotation.Serialize(writer);
                RotationPlusMinus.Serialize(writer);
            }
            if (game >= Game.ROTU)
                InitRot.Serialize(writer);

            writer.Write(Bounce);
            writer.Write(MaxBounces);
            writer.Write(Flags.FlagValueInt);
            writer.Write(ChildID);
            writer.Write(0);
            writer.Write(MinScale);
            writer.Write(MaxScale);
            writer.Write(ScaleCurveID);
            writer.Write(0);
            writer.Write(Gravity);

            if (game >= Game.ROTU)
                LightInfo.Serialize(writer);

        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("VelocityPlusMinus");
                dt.RemoveProperty("Rotation");
                dt.RemoveProperty("RotationPlusMinus");
            }
            if (game < Game.ROTU)
            {
                dt.RemoveProperty("InitRot");
                dt.RemoveProperty("LightInfo");
            }
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(Model)}]";
        }
    }

    public class ShrapnelEntry_Lightning : Shrapnel
    {
        private const string categoryName = "Lightning";

        [Category(categoryName)]
        public ShrapnelLocation Start { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation End { get; set; }

        public ShrapnelEntry_Lightning(Game game) : base(game, IShrapnelType.Lightning)
        {
            Start = ShrapnelLocation(game);
            End = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Lightning(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Lightning, reader)
        {
            Start = ShrapnelLocation(reader, game);
            End = ShrapnelLocation(reader, game);
            ReadPad(reader, 0x8);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);
            Start.Serialize(writer);
            End.Serialize(writer);
            WritePad(writer, 0x8);
        }
    }

    public class ShrapnelEntry_Sound : Shrapnel
    {
        private const string categoryName = "Sound";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Sound { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Source { get; set; }
        [Category(categoryName)]
        public AssetSingle Volume { get; set; }
        [Category(categoryName)]
        public AssetSingle InnerRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle OuterRadius { get; set; }

        public ShrapnelEntry_Sound(Game game) : base(game, IShrapnelType.Sound)
        {
            Source = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Sound(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Sound, reader)
        {
            Sound = reader.ReadUInt32();
            Source = ShrapnelLocation(reader, game);
            if (game < Game.Incredibles)
            {
                Volume = reader.ReadSingle();
                InnerRadius = reader.ReadSingle();
                OuterRadius = reader.ReadSingle();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(Sound);
            Source.Serialize(writer);

            if (game < Game.Incredibles)
            {
                writer.Write(Volume);
                writer.Write(InnerRadius);
                writer.Write(OuterRadius);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game >= Game.Incredibles)
            {
                dt.RemoveProperty("Volume");
                dt.RemoveProperty("InnerRadius");
                dt.RemoveProperty("OuterRadius");
            }
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(Sound)}]";
        }
    }

    public class ShrapnelEntry_Shockwave : Shrapnel
    {
        private const string categoryName = "Shockwave";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthVelocity { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathVelocity { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthSpin { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathSpin { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthColorRed { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthColorGreen { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthColorBlue { get; set; }
        [Category(categoryName)]
        public AssetSingle BirthColorAlpha { get; set; }

        [Category(categoryName)]
        public AssetColor BirthColorRGBA
        {
            get => AssetColor.FromVector4(BirthColorRed, BirthColorGreen, BirthColorBlue, BirthColorAlpha);
            set
            {
                var val = value.ToVector4();
                BirthColorRed = val.X;
                BirthColorGreen = val.Y;
                BirthColorBlue = val.Z;
                BirthColorAlpha = val.W;
            }
        }

        [Category(categoryName)]
        public AssetSingle DeathColorRed { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathColorGreen { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathColorBlue { get; set; }
        [Category(categoryName)]
        public AssetSingle DeathColorAlpha { get; set; }

        [Category(categoryName)]
        public AssetColor DeathColorRGBA
        {
            get => AssetColor.FromVector4(DeathColorRed, DeathColorGreen, DeathColorBlue, DeathColorAlpha);
            set
            {
                var val = value.ToVector4();
                DeathColorRed = val.X;
                DeathColorGreen = val.Y;
                DeathColorBlue = val.Z;
                DeathColorAlpha = val.W;
            }
        }

        public ShrapnelEntry_Shockwave(Game game) : base(game, IShrapnelType.Shockwave) { }

        public ShrapnelEntry_Shockwave(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Shockwave, reader)
        {
            Model = reader.ReadUInt32();
            BirthRadius = reader.ReadSingle();
            DeathRadius = reader.ReadSingle();
            BirthVelocity = reader.ReadSingle();
            DeathVelocity = reader.ReadSingle();
            BirthSpin = reader.ReadSingle();
            DeathSpin = reader.ReadSingle();
            BirthColorRed = reader.ReadSingle();
            BirthColorGreen = reader.ReadSingle();
            BirthColorBlue = reader.ReadSingle();
            BirthColorAlpha = reader.ReadSingle();
            DeathColorRed = reader.ReadSingle();
            DeathColorGreen = reader.ReadSingle();
            DeathColorBlue = reader.ReadSingle();
            DeathColorAlpha = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(Model);
            writer.Write(BirthRadius);
            writer.Write(DeathRadius);
            writer.Write(BirthVelocity);
            writer.Write(DeathVelocity);
            writer.Write(BirthSpin);
            writer.Write(DeathSpin);
            writer.Write(BirthColorRed);
            writer.Write(BirthColorGreen);
            writer.Write(BirthColorBlue);
            writer.Write(BirthColorAlpha);
            writer.Write(DeathColorRed);
            writer.Write(DeathColorGreen);
            writer.Write(DeathColorBlue);
            writer.Write(DeathColorAlpha);
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(Model)}]";
        }
    }

    public class ShrapnelEntry_Explosion : Shrapnel
    {
        private const string categoryName = "Explosion";

        [Category(categoryName)]
        public AssetID ExplosionType { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

        public ShrapnelEntry_Explosion(Game game) : base(game, IShrapnelType.Explosion)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Explosion(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Explosion, reader)
        {
            ExplosionType = reader.ReadUInt32();
            Location = ShrapnelLocation(reader, game);
            Flags.FlagValueInt = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(ExplosionType);
            Location.Serialize(writer);
            writer.Write(Flags.FlagValueInt);
        }
    }

    public class ShrapnelEntry_Distortion : Shrapnel
    {
        private const string categoryName = "Distortion";

        [Category(categoryName)]
        public AssetID Type { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle Radius { get; set; }
        [Category(categoryName)]
        public AssetSingle Duration { get; set; }
        [Category(categoryName)]
        public AssetSingle Intensity { get; set; }
        [Category(categoryName)]
        public AssetSingle Frequency { get; set; }
        [Category(categoryName)]
        public AssetSingle RepeatDelay { get; set; }

        public ShrapnelEntry_Distortion(Game game) : base(game, IShrapnelType.Distortion)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Distortion(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Distortion, reader)
        {
            Type = reader.ReadUInt32();
            Location = ShrapnelLocation(reader, game);
            Flags.FlagValueInt = reader.ReadUInt32();
            Radius = reader.ReadSingle();
            Duration = reader.ReadSingle();
            Intensity = reader.ReadSingle();
            Frequency = reader.ReadSingle();
            RepeatDelay = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(Type);
            Location.Serialize(writer);
            writer.Write(Flags.FlagValueInt);
            writer.Write(Radius);
            writer.Write(Duration);
            writer.Write(Intensity);
            writer.Write(Frequency);
            writer.Write(RepeatDelay);
        }
    }

    public class ShrapnelEntry_Fire : Shrapnel
    {
        private const string categoryName = "Fire";

        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle Radius { get; set; }
        [Category(categoryName)]
        public AssetSingle Scale { get; set; }
        [Category(categoryName)]
        public AssetSingle Fuel { get; set; }
        [Category(categoryName)]
        public AssetSingle Heat { get; set; }
        [Category(categoryName)]
        public AssetSingle Damage { get; set; }
        [Category(categoryName)]
        public AssetSingle Knockback { get; set; }
        [Category(categoryName)]
        public AssetSingle HeatMagnify { get; set; }
        [Category(categoryName)]
        public AssetSingle Height { get; set; }
        [Category(categoryName)]
        public xBound Bound { get; set; }

        public ShrapnelEntry_Fire(Game game) : base(game, IShrapnelType.Fire)
        {
            Location = ShrapnelLocation(game);
            Bound = new xBound();
        }

        public ShrapnelEntry_Fire(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Fire, reader)
        {
            Location = ShrapnelLocation(reader, game);
            Flags.FlagValueInt = reader.ReadUInt32();
            Radius = reader.ReadSingle();
            Scale = reader.ReadSingle();
            Fuel = reader.ReadSingle();
            Heat = reader.ReadSingle();
            Damage = reader.ReadSingle();
            Knockback = reader.ReadSingle();

            if (game >= Game.ROTU)
            {
                HeatMagnify = reader.ReadSingle();
                Height = reader.ReadSingle();
                reader.ReadInt32();
                Bound = new xBound(reader);
            }
            else
                Bound = new xBound();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            Location.Serialize(writer);
            writer.Write(Flags.FlagValueInt);
            writer.Write(Radius);
            writer.Write(Scale);
            writer.Write(Fuel);
            writer.Write(Heat);
            writer.Write(Damage);
            writer.Write(Knockback);

            if (game >= Game.ROTU)
            {
                writer.Write(HeatMagnify);
                writer.Write(Height);
                writer.Write(0);
                Bound.Serialize(writer);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.ROTU)
            {
                dt.RemoveProperty("HeatMagnify");
                dt.RemoveProperty("Height");
                dt.RemoveProperty("Bound");
            }
        }
    }

    public class ShrapnelEntry_Light : Shrapnel
    {
        private const string categoryName = "Light";

        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public ShrapnelLightInfo LightInfo { get; set; }

        public ShrapnelEntry_Light(Game game) : base(game, IShrapnelType.Light)
        {
            Location = ShrapnelLocation(game);
            LightInfo = new ShrapnelLightInfo();
        }

        public ShrapnelEntry_Light(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Light, reader)
        {
            Location = ShrapnelLocation(reader, game);
            LightInfo = new ShrapnelLightInfo(reader);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            Location.Serialize(writer);
            LightInfo.Serialize(writer);
        }
    }

    public class ShrapnelEntry_Smoke : Shrapnel
    {
        private const string categoryName = "Smoke";

        [Category(categoryName)]
        public AssetID ParticleEmitterID { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public AssetSingle Rate { get; set; }
        [Category(categoryName)]
        public AssetSingle Radius { get; set; }
        
        public ShrapnelEntry_Smoke(Game game) : base(game, IShrapnelType.Smoke)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Smoke(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Smoke, reader)
        {
            ParticleEmitterID = reader.ReadUInt32();
            Location = ShrapnelLocation(reader, game);
            Rate = reader.ReadSingle();
            Radius = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);
            writer.Write(ParticleEmitterID);
            Location.Serialize(writer);
            writer.Write(Rate);
            writer.Write(Radius);
            writer.Write(0);
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(ParticleEmitterID)}]";
        }
    }

    public class ShrapnelEntry_Goo : Shrapnel
    {
        private const string categoryName = "Goo";

        [Category(categoryName)]
        public AssetID DropModelInfoID { get; set; }
        [Category(categoryName)]
        public AssetID SplatModelInfoID { get; set; }
        [Category(categoryName)]
        public AssetID SplatSndID { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Location { get; set; }
        [Category(categoryName)]
        public ShrapnelLocation Velocity { get; set; }
        [Category(categoryName)]
        public AssetSingle MinScale { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxScale { get; set;}

        public ShrapnelEntry_Goo(Game game) : base(game, IShrapnelType.Goo)
        {
            Location = ShrapnelLocation(game);
            Velocity = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Goo(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Goo, reader)
        {
            DropModelInfoID = reader.ReadUInt32();
            reader.ReadInt32();
            SplatModelInfoID = reader.ReadUInt32();
            reader.ReadInt32();
            SplatSndID = reader.ReadUInt32();
            reader.ReadInt32();
            Location = ShrapnelLocation(reader, game);
            Velocity = ShrapnelLocation(reader, game);
            MinScale = reader.ReadSingle();
            MaxScale = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);
            writer.Write(DropModelInfoID);
            writer.Write(0);
            writer.Write(SplatModelInfoID);
            writer.Write(0);
            writer.Write(SplatSndID);
            writer.Write(0);
            Location.Serialize(writer);
            Velocity.Serialize(writer);
            writer.Write(MinScale);
            writer.Write(MaxScale);
        }

        public override string ToString()
        {
            return $"[{categoryName} - {HexUIntTypeConverter.StringFromAssetID(DropModelInfoID)}]";
        }
    }
}