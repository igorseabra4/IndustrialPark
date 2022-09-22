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
        Fire = 10
    }

    public abstract class Shrapnel : GenericAssetDataContainer
    {
        [ReadOnly(true)]
        public IShrapnelType ShrapnelType { get; set; }

        public AssetID ID { get; set; }
        public AssetID ParentID { get; set; }
        public AssetID ParentID2 { get; set; }
        public AssetSingle Lifetime { get; set; }
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

        private const byte padB = 0xCD;

        protected void ReadPad(EndianBinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
                if (reader.ReadByte() != padB)
                    throw new Exception("Error reading SHRP padding: non-padding byte found at " + (reader.BaseStream.Position - 1).ToString());
        }

        protected void WritePad(EndianBinaryWriter writer, int count)
        {
            for (int i = 0; i < count; i++)
                writer.Write(padB);
        }

        protected static ShrapnelLocation ShrapnelLocation(Game game) =>
            game == Game.Incredibles ? new ShrapnelLocation_TSSM() : new ShrapnelLocation();
        protected static ShrapnelLocation ShrapnelLocation(EndianBinaryReader reader, Game game) =>
            game == Game.Incredibles ? new ShrapnelLocation_TSSM(reader) : new ShrapnelLocation(reader);
    }

    public class ShrapnelEntry_Particle : Shrapnel
    {
        public ShrapnelLocation Source { get; set; }
        public ShrapnelLocation Velocity { get; set; }

        public byte Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public short Unknown3 { get; set; }
        public int Dummy0 { get; set; }
        [ValidReferenceRequired]
        public AssetID ParticleEmitter { get; set; }
        public int ParEmitter { get; set; }

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

            if (game == Game.Incredibles)
            {
                ReadPad(reader, 0x48);
                Dummy0 = reader.ReadInt32();
            }
            else
                ReadPad(reader, 0x30);

            ParticleEmitter = reader.ReadUInt32();
            ParEmitter = reader.ReadInt32();
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

            if (game == Game.Incredibles)
            {
                WritePad(writer, 0x48);
                writer.Write(Dummy0);
            }
            else
                WritePad(writer, 0x30);

            writer.Write(ParticleEmitter);
            writer.Write(ParEmitter);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
                dt.RemoveProperty("Dummy0");
        }
    }

    public class ShrapnelEntry_Projectile : Shrapnel
    {
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        public int ModelFile { get; set; }
        public ShrapnelLocation Launch { get; set; }
        public ShrapnelLocation Velocity { get; set; }
        public ShrapnelLocation VelocityPlusMinus { get; set; }
        public ShrapnelLocation Rotation { get; set; }
        public ShrapnelLocation RotationPlusMinus { get; set; }
        public AssetSingle Bounce { get; set; }
        public int MaxBounces { get; set; }
        public int Flags { get; set; }
        public AssetID ChildID { get; set; }
        public int Child { get; set; }
        public AssetSingle MinScale { get; set; }
        public AssetSingle MaxScale { get; set; }
        public AssetID ScaleCurveID { get; set; }
        public int ScaleCurve { get; set; }
        public AssetSingle Gravity { get; set; }

        public ShrapnelEntry_Projectile(Game game) : base(game, IShrapnelType.Projectile)
        {
            Launch = ShrapnelLocation(game);
            Velocity = ShrapnelLocation(game);
            VelocityPlusMinus = ShrapnelLocation(game);
            Rotation = ShrapnelLocation(game);
            RotationPlusMinus = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Projectile(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Projectile, reader)
        {
            Model = reader.ReadUInt32();
            ModelFile = reader.ReadInt32();
            Launch = ShrapnelLocation(reader, game);
            Velocity = ShrapnelLocation(reader, game);

            if (game == Game.Incredibles)
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

            Bounce = reader.ReadSingle();
            MaxBounces = reader.ReadInt32();
            Flags = reader.ReadInt32();
            ChildID = reader.ReadUInt32();
            Child = reader.ReadInt32();
            MinScale = reader.ReadSingle();
            MaxScale = reader.ReadSingle();
            ScaleCurveID = reader.ReadUInt32();
            ScaleCurve = reader.ReadInt32();
            Gravity = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(Model);
            writer.Write(ModelFile);
            Launch.Serialize(writer);
            Velocity.Serialize(writer);

            if (game == Game.Incredibles)
            {
                VelocityPlusMinus.Serialize(writer);
                Rotation.Serialize(writer);
                RotationPlusMinus.Serialize(writer);
            }

            writer.Write(Bounce);
            writer.Write(MaxBounces);
            writer.Write(Flags);
            writer.Write(ChildID);
            writer.Write(Child);
            writer.Write(MinScale);
            writer.Write(MaxScale);
            writer.Write(ScaleCurveID);
            writer.Write(ScaleCurve);
            writer.Write(Gravity);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("VelocityPlusMinus");
                dt.RemoveProperty("Rotation");
                dt.RemoveProperty("RotationPlusMinus");
            }
        }
    }

    public class ShrapnelEntry_Lightning : Shrapnel
    {
        public ShrapnelLocation Start { get; set; }
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
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }
        public ShrapnelLocation Source { get; set; }
        public AssetSingle Volume { get; set; }
        public AssetSingle InnerRadius { get; set; }
        public AssetSingle OuterRadius { get; set; }

        public ShrapnelEntry_Sound(Game game) : base(game, IShrapnelType.Sound)
        {
            Source = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Sound(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Sound, reader)
        {
            Sound = reader.ReadUInt32();
            Source = ShrapnelLocation(reader, game);
            if (game != Game.Incredibles)
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

            if (game != Game.Incredibles)
            {
                writer.Write(Volume);
                writer.Write(InnerRadius);
                writer.Write(OuterRadius);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Incredibles)
            {
                dt.RemoveProperty("Volume");
                dt.RemoveProperty("InnerRadius");
                dt.RemoveProperty("OuterRadius");
            }
        }
    }

    public class ShrapnelEntry_Shockwave : Shrapnel
    {
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        public AssetSingle BirthRadius { get; set; }
        public AssetSingle DeathRadius { get; set; }
        public AssetSingle BirthVelocity { get; set; }
        public AssetSingle DeathVelocity { get; set; }
        public AssetSingle BirthSpin { get; set; }
        public AssetSingle DeathSpin { get; set; }
        public AssetSingle BirthColorRed { get; set; }
        public AssetSingle BirthColorGreen { get; set; }
        public AssetSingle BirthColorBlue { get; set; }
        public AssetSingle BirthColorAlpha { get; set; }

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

        public AssetSingle DeathColorRed { get; set; }
        public AssetSingle DeathColorGreen { get; set; }
        public AssetSingle DeathColorBlue { get; set; }
        public AssetSingle DeathColorAlpha { get; set; }

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
    }

    public class ShrapnelEntry_Explosion : Shrapnel
    {
        public AssetID ExplosionType { get; set; }
        public ShrapnelLocation Location { get; set; }
        public uint Flags { get; set; }

        public ShrapnelEntry_Explosion(Game game) : base(game, IShrapnelType.Explosion)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Explosion(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Explosion, reader)
        {
            ExplosionType = reader.ReadUInt32();
            Location = ShrapnelLocation(reader, game);
            Flags = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            writer.Write(ExplosionType);
            Location.Serialize(writer);
            writer.Write(Flags);
        }
    }

    public class ShrapnelEntry_Distortion : Shrapnel
    {
        public AssetID Type { get; set; }
        public ShrapnelLocation Location { get; set; }
        public uint Flags { get; set; }

        public AssetSingle Radius { get; set; }
        public AssetSingle Duration { get; set; }
        public AssetSingle Intensity { get; set; }
        public AssetSingle Frequency { get; set; }
        public AssetSingle RepeatDelay { get; set; }

        public ShrapnelEntry_Distortion(Game game) : base(game, IShrapnelType.Distortion)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Distortion(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Distortion, reader)
        {
            Type = reader.ReadUInt32();
            Location = ShrapnelLocation(reader, game);
            Flags = reader.ReadUInt32();
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
            writer.Write(Flags);
            writer.Write(Radius);
            writer.Write(Duration);
            writer.Write(Intensity);
            writer.Write(Frequency);
            writer.Write(RepeatDelay);
        }
    }

    public class ShrapnelEntry_Fire : Shrapnel
    {
        public ShrapnelLocation Location { get; set; }
        public uint Flags { get; set; }
        public AssetSingle Radius { get; set; }
        public AssetSingle Scale { get; set; }
        public AssetSingle Fuel { get; set; }
        public AssetSingle Heat { get; set; }
        public AssetSingle Damage { get; set; }
        public AssetSingle Knockback { get; set; }

        public ShrapnelEntry_Fire(Game game) : base(game, IShrapnelType.Fire)
        {
            Location = ShrapnelLocation(game);
        }

        public ShrapnelEntry_Fire(EndianBinaryReader reader, Game game) : base(game, IShrapnelType.Fire, reader)
        {
            Location = ShrapnelLocation(reader, game);
            Flags = reader.ReadUInt32();
            Radius = reader.ReadSingle();
            Scale = reader.ReadSingle();
            Fuel = reader.ReadSingle();
            Heat = reader.ReadSingle();
            Damage = reader.ReadSingle();
            Knockback = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            SerializeEntryShrpBase(writer);

            Location.Serialize(writer);
            writer.Write(Flags);
            writer.Write(Radius);
            writer.Write(Scale);
            writer.Write(Fuel);
            writer.Write(Heat);
            writer.Write(Damage);
            writer.Write(Knockback);
        }
    }
}