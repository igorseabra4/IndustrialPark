using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zSurfMatFX : GenericAssetDataContainer
    {
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        public AssetID BumpMapTexture { get; set; }
        public AssetID EnvMapTexture { get; set; }
        public AssetSingle Shininess { get; set; }
        public AssetSingle Bumpiness { get; set; }
        public AssetID DualMapTexture { get; set; }

        public zSurfMatFX() { }
        public zSurfMatFX(EndianBinaryReader reader)
        {
            Flags.FlagValueInt = reader.ReadUInt32();
            BumpMapTexture = reader.ReadUInt32();
            EnvMapTexture = reader.ReadUInt32();
            Shininess = reader.ReadSingle();
            Bumpiness = reader.ReadSingle();
            DualMapTexture = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            writer.Write(Flags.FlagValueInt);
            writer.Write(BumpMapTexture);
            writer.Write(EnvMapTexture);
            writer.Write(Shininess);
            writer.Write(Bumpiness);
            writer.Write(DualMapTexture);

        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zSurfColorFX : GenericAssetDataContainer
    {
        public FlagBitmask Flags { get; set; } = ShortFlagsDescriptor();
        public short Mode { get; set; }
        public AssetSingle Speed { get; set; }

        public zSurfColorFX() { }
        public zSurfColorFX(EndianBinaryReader reader)
        {
            Flags.FlagValueShort = reader.ReadUInt16();
            Mode = reader.ReadInt16();
            Speed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Flags.FlagValueShort);
            writer.Write(Mode);
            writer.Write(Speed);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zSurfTextureAnim : GenericAssetDataContainer
    {
        public short Padding { get; set; }
        public short Mode { get; set; }
        public AssetID Group { get; set; }
        public AssetSingle Speed { get; set; }

        public zSurfTextureAnim() { }
        public zSurfTextureAnim(EndianBinaryReader reader)
        {
            Padding = reader.ReadInt16();
            Mode = reader.ReadInt16();
            Group = reader.ReadUInt32();
            Speed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            writer.Write(Padding);
            writer.Write(Mode);
            writer.Write(Group);
            writer.Write(Speed);

        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zSurfUVFX : GenericAssetDataContainer
    {
        public int Mode { get; set; }
        public AssetSingle Rot { get; set; }
        public AssetSingle RotSpd { get; set; }
        public AssetSingle Trans_X { get; set; }
        public AssetSingle Trans_Y { get; set; }
        public AssetSingle Trans_Z { get; set; }
        public AssetSingle TransSpeed_X { get; set; }
        public AssetSingle TransSpeed_Y { get; set; }
        public AssetSingle TransSpeed_Z { get; set; }
        public AssetSingle Scale_X { get; set; }
        public AssetSingle Scale_Y { get; set; }
        public AssetSingle Scale_Z { get; set; }
        public AssetSingle ScaleSpeed_X { get; set; }
        public AssetSingle ScaleSpeed_Y { get; set; }
        public AssetSingle ScaleSpeed_Z { get; set; }
        public AssetSingle Min_X { get; set; }
        public AssetSingle Min_Y { get; set; }
        public AssetSingle Min_Z { get; set; }
        public AssetSingle Max_X { get; set; }
        public AssetSingle Max_Y { get; set; }
        public AssetSingle Max_Z { get; set; }
        public AssetSingle MinMaxSpeed_X { get; set; }
        public AssetSingle MinMaxSpeed_Y { get; set; }
        public AssetSingle MinMaxSpeed_Z { get; set; }

        public zSurfUVFX() { }
        public zSurfUVFX(EndianBinaryReader reader)
        {
            Mode = reader.ReadInt32();
            Rot = reader.ReadSingle();
            RotSpd = reader.ReadSingle();
            Trans_X = reader.ReadSingle();
            Trans_Y = reader.ReadSingle();
            Trans_Z = reader.ReadSingle();
            TransSpeed_X = reader.ReadSingle();
            TransSpeed_Y = reader.ReadSingle();
            TransSpeed_Z = reader.ReadSingle();
            Scale_X = reader.ReadSingle();
            Scale_Y = reader.ReadSingle();
            Scale_Z = reader.ReadSingle();
            ScaleSpeed_X = reader.ReadSingle();
            ScaleSpeed_Y = reader.ReadSingle();
            ScaleSpeed_Z = reader.ReadSingle();
            Min_X = reader.ReadSingle();
            Min_Y = reader.ReadSingle();
            Min_Z = reader.ReadSingle();
            Max_X = reader.ReadSingle();
            Max_Y = reader.ReadSingle();
            Max_Z = reader.ReadSingle();
            MinMaxSpeed_X = reader.ReadSingle();
            MinMaxSpeed_Y = reader.ReadSingle();
            MinMaxSpeed_Z = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Mode);
            writer.Write(Rot);
            writer.Write(RotSpd);
            writer.Write(Trans_X);
            writer.Write(Trans_Y);
            writer.Write(Trans_Z);
            writer.Write(TransSpeed_X);
            writer.Write(TransSpeed_Y);
            writer.Write(TransSpeed_Z);
            writer.Write(Scale_X);
            writer.Write(Scale_Y);
            writer.Write(Scale_Z);
            writer.Write(ScaleSpeed_X);
            writer.Write(ScaleSpeed_Y);
            writer.Write(ScaleSpeed_Z);
            writer.Write(Min_X);
            writer.Write(Min_Y);
            writer.Write(Min_Z);
            writer.Write(Max_X);
            writer.Write(Max_Y);
            writer.Write(Max_Z);
            writer.Write(MinMaxSpeed_X);
            writer.Write(MinMaxSpeed_Y);
            writer.Write(MinMaxSpeed_Z);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zFootstepsData : GenericAssetDataContainer
    {
        public AssetID ParticleEmitter;
        public AssetID Sound;
        public AssetID Texture;
        public AssetSingle Duration;

        public zFootstepsData() { }
        public zFootstepsData(EndianBinaryReader reader)
        {
            ParticleEmitter = reader.ReadUInt32();
            Sound = reader.ReadUInt32();
            Texture = reader.ReadUInt32();
            Duration = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(ParticleEmitter);
            writer.Write(Sound);
            writer.Write(Texture);
            writer.Write(Duration);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class zHitDecalData : GenericAssetDataContainer
    {
        public AssetID Texture;
        public AssetSingle SizeX;
        public AssetSingle SizeY;

        public zHitDecalData() { }
        public zHitDecalData(EndianBinaryReader reader)
        {
            Texture = reader.ReadUInt32();
            SizeX = reader.ReadSingle();
            SizeY = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Texture);
            writer.Write(SizeX);
            writer.Write(SizeY);
        }
    }

    public enum zHitSource : uint
    {
        zHS_EVENT,
        zHS_GENERAL,
        zHS_PROJECTILE,
        zHS_EXPLOSION,
        zHS_LASER,
        zHS_ENERGY,
        zHS_FIRE,
        zHS_SURFACE,
        zHS_MELEE_HIGH,
        zHS_MELEE_MID,
        zHS_MELEE_LOW,
        zHS_MELEE_UP,
        zHS_MELEE_BACK,
        zHS_MELEE_DIZZY,
        zHS_THROW,
        zHS_WATER,
        zHS_DEATHPLANE,
        zHS_INCREDI,
        zHS_KNOCKBACK,
        zHS_LASERBEAM,
        zHS_INFINITE_FALL,
        zHS_COUNT,
        zHS_FORCE_INT = 0xffffffff
    }

    public class AssetSURF : BaseAsset
    {
        private const string categoryName = "Surface";

        [Category(categoryName)]
        public byte DamageType { get; set; }
        [Category(categoryName)]
        public byte Sticky { get; set; }
        [Category(categoryName)]
        public byte DamageFlags { get; set; }
        [Category(categoryName)]
        public byte SurfaceType { get; set; }
        [Category(categoryName)]
        public byte Phys_Pad { get; set; }
        [Category(categoryName)]
        public byte SlideStart { get; set; }
        [Category(categoryName)]
        public byte SlideStop { get; set; }
        [Category(categoryName)]
        public FlagBitmask PhysicsFlags { get; set; } = ByteFlagsDescriptor(
            "Slide Off Player",
            "Angle Player",
            null,
            "No Stand",
            "Out Of Bounds",
            "No Stand 2",
            "Ledge Grab");
        [Category(categoryName)]
        public AssetSingle Friction { get; set; }
        [Category(categoryName)]
        public zSurfMatFX zSurfMatFX { get; set; }
        [Category(categoryName)]
        public zSurfColorFX zSurfColorFX { get; set; }
        [Category(categoryName)]
        public FlagBitmask TextureAnimFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public zSurfTextureAnim zSurfTextureAnim1 { get; set; }
        [Category(categoryName)]
        public zSurfTextureAnim zSurfTextureAnim2 { get; set; }
        [Category(categoryName)]
        public FlagBitmask UVEffectsFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public zSurfUVFX zSurfUVFX { get; set; }
        [Category(categoryName)]
        public zSurfUVFX zSurfUVFX2 { get; set; }
        [Category(categoryName)]
        public byte On { get; set; }
        [Category(categoryName)]
        public AssetSingle OutOfBoundsDelay { get; set; }
        [Category(categoryName)]
        public AssetSingle WalljumpScaleXZ { get; set; }
        [Category(categoryName)]
        public AssetSingle WalljumpScaleY { get; set; }
        [Category(categoryName)]
        public AssetSingle DamageTimer { get; set; }
        [Category(categoryName)]
        public AssetSingle DamageBounce { get; set; }
        [Category(categoryName)]
        public int UnknownInt { get; set; }

        // TSSM/Incredibles Only
        [Category(categoryName)]
        public AssetID ImpactSound { get; set; }
        [Category(categoryName)]
        public byte DashImpactType { get; set; }
        [Category(categoryName)]
        public AssetSingle DashImpactThrowBack { get; set; }
        [Category(categoryName)]
        public AssetSingle DashSprayMagnitude { get; set; }
        [Category(categoryName)]
        public AssetSingle DashCoolRate { get; set; }
        [Category(categoryName)]
        public AssetSingle DashCoolAmount { get; set; }
        [Category(categoryName)]
        public AssetSingle DashPass { get; set; }
        [Category(categoryName)]
        public AssetSingle DashRampMaxDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle DashRampMinDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle DashRampKeySpeed { get; set; }
        [Category(categoryName)]
        public AssetSingle DashRampMaxHeight { get; set; }
        [Category(categoryName)]
        public AssetID DashRampTarget_MovePoint { get; set; }
        [Category(categoryName)]
        public int DamageAmount { get; set; }
        [Category(categoryName)]
        public zHitSource HitSourceDamageType { get; set; }
        [Category(categoryName)]
        public zFootstepsData OffSurface { get; set; }
        [Category(categoryName)]
        public zFootstepsData OnSurface { get; set; }
        [Category(categoryName)]
        public zHitDecalData HitDecalData0 { get; set; }
        [Category(categoryName)]
        public zHitDecalData HitDecalData1 { get; set; }
        [Category(categoryName)]
        public zHitDecalData HitDecalData2 { get; set; }
        [Category(categoryName)]
        public AssetSingle OffSurfaceTime { get; set; }
        [Category(categoryName)]
        public byte SwimmableSurface { get; set; }
        [Category(categoryName)]
        public byte DashFall { get; set; }
        [Category(categoryName)]
        public byte NeedButtonPress { get; set; }
        [Category(categoryName)]
        public byte DashAttack { get; set; }
        [Category(categoryName)]
        public byte FootstepDecals { get; set; }
        [Category(categoryName)]
        public byte DrivingSurfaceType { get; set; }

        public AssetSURF(string assetName) : base(assetName, AssetType.Surface, BaseAssetType.Surface)
        {
            zSurfMatFX = new zSurfMatFX();
            zSurfColorFX = new zSurfColorFX();
            zSurfTextureAnim1 = new zSurfTextureAnim();
            zSurfTextureAnim2 = new zSurfTextureAnim();
            zSurfUVFX = new zSurfUVFX();
            zSurfUVFX2 = new zSurfUVFX();
            OffSurface = new zFootstepsData();
            OnSurface = new zFootstepsData();
            HitDecalData0 = new zHitDecalData();
            HitDecalData1 = new zHitDecalData();
            HitDecalData2 = new zHitDecalData();
        }

        public AssetSURF(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                DamageType = reader.ReadByte();
                Sticky = reader.ReadByte();
                DamageFlags = reader.ReadByte();
                SurfaceType = reader.ReadByte();
                Phys_Pad = reader.ReadByte();
                SlideStart = reader.ReadByte();
                SlideStop = reader.ReadByte();
                PhysicsFlags.FlagValueByte = reader.ReadByte();
                Friction = reader.ReadSingle();
                zSurfMatFX = new zSurfMatFX(reader);
                zSurfColorFX = new zSurfColorFX(reader);
                TextureAnimFlags.FlagValueInt = reader.ReadUInt32();
                zSurfTextureAnim1 = new zSurfTextureAnim(reader);
                zSurfTextureAnim2 = new zSurfTextureAnim(reader);
                UVEffectsFlags.FlagValueInt = reader.ReadUInt32();
                zSurfUVFX = new zSurfUVFX(reader);

                if (game != Game.Scooby)
                {
                    zSurfUVFX2 = new zSurfUVFX(reader);
                    On = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }
                else
                    zSurfUVFX2 = new zSurfUVFX();

                OutOfBoundsDelay = reader.ReadSingle();
                WalljumpScaleXZ = reader.ReadSingle();
                WalljumpScaleY = reader.ReadSingle();
                DamageTimer = reader.ReadSingle();
                DamageBounce = reader.ReadSingle();

                if (game == Game.Scooby)
                {
                    UnknownInt = reader.ReadInt32();
                    On = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }

                if (game >= Game.Incredibles)
                {
                    ImpactSound = reader.ReadUInt32();
                    DashImpactType = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    DashImpactThrowBack = reader.ReadSingle();
                    DashSprayMagnitude = reader.ReadSingle();
                    DashCoolRate = reader.ReadSingle();
                    DashCoolAmount = reader.ReadSingle();
                    DashPass = reader.ReadSingle();
                    DashRampMaxDistance = reader.ReadSingle();
                    DashRampMinDistance = reader.ReadSingle();
                    DashRampKeySpeed = reader.ReadSingle();
                    DashRampMaxHeight = reader.ReadSingle();
                    DashRampTarget_MovePoint = reader.ReadUInt32();
                    DamageAmount = reader.ReadInt32();
                    HitSourceDamageType = (zHitSource)reader.ReadInt32();
                    OffSurface = new zFootstepsData(reader);
                    OnSurface = new zFootstepsData(reader);
                    HitDecalData0 = new zHitDecalData(reader);
                    HitDecalData1 = new zHitDecalData(reader);
                    HitDecalData2 = new zHitDecalData(reader);
                    OffSurfaceTime = reader.ReadSingle();
                    SwimmableSurface = reader.ReadByte();
                    DashFall = reader.ReadByte();
                    NeedButtonPress = reader.ReadByte();
                    DashAttack = reader.ReadByte();
                    FootstepDecals = reader.ReadByte();
                    reader.ReadInt32();
                    DrivingSurfaceType = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }
                else
                {
                    OffSurface = new zFootstepsData();
                    OnSurface = new zFootstepsData();
                    HitDecalData0 = new zHitDecalData();
                    HitDecalData1 = new zHitDecalData();
                    HitDecalData2 = new zHitDecalData();
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(DamageType);
            writer.Write(Sticky);
            writer.Write(DamageFlags);
            writer.Write(SurfaceType);
            writer.Write(Phys_Pad);
            writer.Write(SlideStart);
            writer.Write(SlideStop);
            writer.Write(PhysicsFlags.FlagValueByte);
            writer.Write(Friction);
            zSurfMatFX.Serialize(writer);
            zSurfColorFX.Serialize(writer);
            writer.Write(TextureAnimFlags.FlagValueInt);
            zSurfTextureAnim1.Serialize(writer);
            zSurfTextureAnim2.Serialize(writer);
            writer.Write(UVEffectsFlags.FlagValueInt);
            zSurfUVFX.Serialize(writer);
            if (game != Game.Scooby)
            {
                zSurfUVFX2.Serialize(writer);
                writer.Write(On);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            writer.Write(OutOfBoundsDelay);
            writer.Write(WalljumpScaleXZ);
            writer.Write(WalljumpScaleY);
            writer.Write(DamageTimer);
            writer.Write(DamageBounce);
            if (game == Game.Scooby)
            {
                writer.Write(UnknownInt);
                writer.Write(On);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            if (game >= Game.Incredibles)
            {
                writer.Write(ImpactSound);
                writer.Write(DashImpactType);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write(DashImpactThrowBack);
                writer.Write(DashSprayMagnitude);
                writer.Write(DashCoolRate);
                writer.Write(DashCoolAmount);
                writer.Write(DashPass);
                writer.Write(DashRampMaxDistance);
                writer.Write(DashRampMinDistance);
                writer.Write(DashRampKeySpeed);
                writer.Write(DashRampMaxHeight);
                writer.Write(DashRampTarget_MovePoint);
                writer.Write(DamageAmount);
                writer.Write((int)HitSourceDamageType);
                OffSurface.Serialize(writer);
                OnSurface.Serialize(writer);
                HitDecalData0.Serialize(writer);
                HitDecalData1.Serialize(writer);
                HitDecalData2.Serialize(writer);
                writer.Write(OffSurfaceTime);
                writer.Write(SwimmableSurface);
                writer.Write(DashFall);
                writer.Write(NeedButtonPress);
                writer.Write(DashAttack);
                writer.Write(FootstepDecals);
                writer.Write(0);
                writer.Write(DrivingSurfaceType);
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            SerializeLinks(writer);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("zSurfUVFX2");
            else
                dt.RemoveProperty("UnknownInt");

            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("ImpactSound");
                dt.RemoveProperty("DashImpactType");
                dt.RemoveProperty("DashImpactThrowBack");
                dt.RemoveProperty("DashSprayMagnitude");
                dt.RemoveProperty("DashCoolRate");
                dt.RemoveProperty("DashCoolAmount");
                dt.RemoveProperty("DashPass");
                dt.RemoveProperty("DashRampMaxDistance");
                dt.RemoveProperty("DashRampMinDistance");
                dt.RemoveProperty("DashRampKeySpeed");
                dt.RemoveProperty("DashRampMaxHeight");
                dt.RemoveProperty("DashRampTarget_MovePoint");
                dt.RemoveProperty("DamageAmount");
                dt.RemoveProperty("HitSourceDamageType");
                dt.RemoveProperty("OffSurface");
                dt.RemoveProperty("OnSurface");
                dt.RemoveProperty("HitDecalData0");
                dt.RemoveProperty("HitDecalData1");
                dt.RemoveProperty("HitDecalData2");
                dt.RemoveProperty("OffSurfaceTime");
                dt.RemoveProperty("SwimmableSurface");
                dt.RemoveProperty("DashFall");
                dt.RemoveProperty("NeedButtonPress");
                dt.RemoveProperty("DashAttack");
                dt.RemoveProperty("FootstepDecals");
                dt.RemoveProperty("DrivingSurfaceType");
            }

            base.SetDynamicProperties(dt);
        }
    }
}