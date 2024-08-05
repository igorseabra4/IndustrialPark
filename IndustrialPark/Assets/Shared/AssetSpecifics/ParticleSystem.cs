using IndustrialPark.AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public enum ParticleSystemType
    {
        Dummy,
        Waterfall,
        WaterfallMist,
        WaterfallSplash,
        Snow,
        Rain,
        EarthDust,
        Pop,
        BurrowKickup,
        Puffer
    }
    public abstract class ParticleSystem_Generic : GenericAssetDataContainer
    {
        public ParticleSystem_Generic() { }
        public ParticleSystem_Generic(EndianBinaryReader reader) { }
        public override void Serialize(EndianBinaryWriter writer) { }
    }

    public class ParticleSystem_Waterfall : ParticleSystem_Generic
    {
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        public AssetSingle WidthMin { get; set; }
        public AssetSingle WidthMax { get; set; }
        public AssetSingle VelocityMin { get; set; }
        public AssetSingle VelocityMax { get; set; }
        public AssetSingle VelocityDirection_Yaw { get; set; }
        public AssetSingle VelocityDirection_Pitch { get; set; }
        public AssetSingle VelocityDirection_Roll { get; set; }
        public AssetSingle VelocityDirectionVary { get; set; }
        public AssetSingle HeightStartMin { get; set; }
        public AssetSingle HeightStartMax { get; set; }
        public AssetSingle HeightVelocity { get; set; }
        public AssetSingle HeightAcceleration { get; set; }
        public AssetSingle HeightMax { get; set; }
        public AssetSingle IntensityMin { get; set; }
        public AssetSingle IntensityMax { get; set; }
        public AssetColor Color { get; set; }
        public AssetSingle Gravity { get; set; }
        public AssetSingle Kill_Y_Offset { get; set; }

        public ParticleSystem_Waterfall() { }
        public ParticleSystem_Waterfall(EndianBinaryReader reader)
        {
            Flags.FlagValueInt = reader.ReadUInt32();
            WidthMin = reader.ReadSingle();
            WidthMax = reader.ReadSingle();
            VelocityMin = reader.ReadSingle();
            VelocityMax = reader.ReadSingle();
            VelocityDirection_Yaw = reader.ReadSingle();
            VelocityDirection_Pitch = reader.ReadSingle();
            VelocityDirection_Roll = reader.ReadSingle();
            VelocityDirectionVary = reader.ReadSingle();
            HeightStartMin = reader.ReadSingle();
            HeightStartMax = reader.ReadSingle();
            HeightVelocity = reader.ReadSingle();
            HeightAcceleration = reader.ReadSingle();
            HeightMax = reader.ReadSingle();
            IntensityMin = reader.ReadSingle();
            IntensityMax = reader.ReadSingle();
            Color = reader.ReadColor();
            Gravity = reader.ReadSingle();
            Kill_Y_Offset = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Flags.FlagValueInt);
            writer.Write(WidthMin);
            writer.Write(WidthMax);
            writer.Write(VelocityMin);
            writer.Write(VelocityMax);
            writer.Write(VelocityDirection_Yaw);
            writer.Write(VelocityDirection_Pitch);
            writer.Write(VelocityDirection_Roll);
            writer.Write(VelocityDirectionVary);
            writer.Write(HeightStartMin);
            writer.Write(HeightStartMax);
            writer.Write(HeightVelocity);
            writer.Write(HeightAcceleration);
            writer.Write(HeightMax);
            writer.Write(IntensityMin);
            writer.Write(IntensityMax);
            writer.Write(Color);
            writer.Write(Gravity);
            writer.Write(Kill_Y_Offset);
        }
    }

    public class ParticleSystem_WaterfallMist : ParticleSystem_Generic
    {
        public AssetSingle LifeMin { get; set; }
        public AssetSingle LifeMax { get; set; }
        public AssetSingle SizeBirth { get; set; }
        public AssetSingle SizeDeath { get; set; }
        public AssetSingle SizeVary { get; set; }
        public AssetSingle VelocityMin { get; set; }
        public AssetSingle VelocityMax { get; set; }
        public AssetSingle VelocityPitchMax { get; set; }
        public AssetSingle RotationVelocityMin { get; set; }
        public AssetSingle RotationVelocityMax { get; set; }
        public AssetSingle IntensityMin { get; set; }
        public AssetSingle IntensityMax { get; set; }
        public AssetColor Color { get; set; }

        public ParticleSystem_WaterfallMist() { }
        public ParticleSystem_WaterfallMist(EndianBinaryReader reader)
        {
            LifeMin = reader.ReadSingle();
            LifeMax = reader.ReadSingle();
            SizeBirth = reader.ReadSingle();
            SizeDeath = reader.ReadSingle();
            SizeVary = reader.ReadSingle();
            VelocityMin = reader.ReadSingle();
            VelocityMax = reader.ReadSingle();
            VelocityPitchMax = reader.ReadSingle();
            RotationVelocityMin = reader.ReadSingle();
            RotationVelocityMax = reader.ReadSingle();
            IntensityMin = reader.ReadSingle();
            IntensityMax = reader.ReadSingle();
            Color = reader.ReadColor();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(LifeMin);
            writer.Write(LifeMax);
            writer.Write(SizeBirth);
            writer.Write(SizeDeath);
            writer.Write(SizeVary);
            writer.Write(VelocityMin);
            writer.Write(VelocityMax);
            writer.Write(VelocityPitchMax);
            writer.Write(RotationVelocityMin);
            writer.Write(RotationVelocityMax);
            writer.Write(IntensityMin);
            writer.Write(IntensityMax);
            writer.Write(Color);
            writer.Write(new byte[0x18]);
        }
    }

    public class ParticleSystem_WaterfallSplash : ParticleSystem_Generic
    {
        public AssetSingle LifeMin { get; set; }
        public AssetSingle LifeMax { get; set; }
        public AssetSingle SizeBirth { get; set; }
        public AssetSingle SizeDeath { get; set; }
        public AssetSingle SizeVary { get; set; }
        public AssetSingle VelocityMin { get; set; }
        public AssetSingle VelocityMax { get; set; }
        public AssetSingle VelocityPitchMin { get; set; }
        public AssetSingle VelocityPitchMax { get; set; }
        public AssetSingle IntensityMin { get; set; }
        public AssetSingle IntensityMax { get; set; }
        public AssetColor Color { get; set; }
        public AssetSingle Gravity { get; set; }
        public AssetSingle Kill_YOffset{ get; set; }

        public ParticleSystem_WaterfallSplash() { }
        public ParticleSystem_WaterfallSplash(EndianBinaryReader reader)
        {
            LifeMin = reader.ReadSingle();
            LifeMax = reader.ReadSingle();
            SizeBirth = reader.ReadSingle();
            SizeDeath = reader.ReadSingle();
            SizeVary = reader.ReadSingle();
            VelocityMin = reader.ReadSingle();
            VelocityMax = reader.ReadSingle();
            VelocityPitchMin = reader.ReadSingle();
            VelocityPitchMax = reader.ReadSingle();
            IntensityMin = reader.ReadSingle();
            IntensityMax = reader.ReadSingle();
            Color = reader.ReadColor();
            Gravity = reader.ReadSingle();
            Kill_YOffset = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(LifeMin);
            writer.Write(LifeMax);
            writer.Write(SizeBirth);
            writer.Write(SizeDeath);
            writer.Write(SizeVary);
            writer.Write(VelocityMin);
            writer.Write(VelocityMax);
            writer.Write(VelocityPitchMin);
            writer.Write(VelocityPitchMax);
            writer.Write(IntensityMin);
            writer.Write(IntensityMax);
            writer.Write(Color);
            writer.Write(Gravity);
            writer.Write(Kill_YOffset);
            writer.Write(new byte[0x14]);
        }
    }

    public class ParticleSystem_Snow : ParticleSystem_Generic
    {
        public AssetSingle Gravity { get; set; }
        public AssetSingle DriftForce { get; set; }
        public AssetSingle MaxDriftSpeed { get; set; }
        public AssetSingle MaxDriftDist { get; set; }
        public AssetSingle MinScale { get; set; }
        public AssetSingle MaxScale { get; set; }
        public AssetSingle MaxRotSpeed { get; set; }

        public ParticleSystem_Snow() { }
        public ParticleSystem_Snow(EndianBinaryReader reader) : base(reader)
        {
            Gravity = reader.ReadSingle();
            DriftForce = reader.ReadSingle();
            MaxDriftSpeed = reader.ReadSingle();
            MaxDriftDist = reader.ReadSingle();
            MinScale = reader.ReadSingle();
            MaxScale = reader.ReadSingle();
            MaxRotSpeed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Gravity);
            writer.Write(DriftForce);
            writer.Write(MaxDriftSpeed);
            writer.Write(MaxDriftDist);
            writer.Write(MinScale);
            writer.Write(MaxScale);
            writer.Write(MaxRotSpeed);
            writer.Write(new byte[0x30]);
        }
    }

    public class ParticleSystem_Rain : ParticleSystem_Generic
    {
        public AssetSingle Gravity { get; set; }
        public AssetSingle DropWidth { get; set; }
        public AssetSingle DropLength { get; set; }

        public ParticleSystem_Rain() { }
        public ParticleSystem_Rain(EndianBinaryReader reader) : base(reader)
        {
            Gravity = reader.ReadSingle();
            DropWidth = reader.ReadSingle();
            DropLength = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Gravity);
            writer.Write(DropWidth);
            writer.Write(DropLength);
            writer.Write(new byte[0x40]);
        }
    }

    public class ParticleSystem_EarthDust : ParticleSystem_Generic
    {
        public AssetSingle Gravity { get; set; }
        public AssetSingle Drift { get; set; }
        public AssetSingle Size { get; set; }
        public AssetSingle RotSpeed { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle BirthAlpha { get; set; }
        public AssetSingle DeathAlpha { get; set; }
        public AssetID ShrapID { get; set; }
        public AssetSingle ShrapEmitRate { get; set; }
        public AssetSingle InitDownVelocity { get; set; }
        public AssetSingle RandVelocity { get; set; }

        public ParticleSystem_EarthDust() { }
        public ParticleSystem_EarthDust(EndianBinaryReader reader) : base(reader)
        {
            Gravity = reader.ReadSingle();
            Drift = reader.ReadSingle();
            Size = reader.ReadSingle();
            RotSpeed = reader.ReadSingle();
            Lifetime = reader.ReadSingle();
            BirthAlpha = reader.ReadSingle();
            DeathAlpha = reader.ReadSingle();
            ShrapID = reader.ReadUInt32();
            ShrapEmitRate = reader.ReadSingle();
            InitDownVelocity = reader.ReadSingle();
            RandVelocity = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Gravity);
            writer.Write(Drift);
            writer.Write(Size);
            writer.Write(RotSpeed);
            writer.Write(Lifetime);
            writer.Write(BirthAlpha);
            writer.Write(DeathAlpha);
            writer.Write(ShrapID);
            writer.Write(ShrapEmitRate);
            writer.Write(InitDownVelocity);
            writer.Write(RandVelocity);
            writer.Write(new byte[0x20]);
        }
    }

    public class ParticleSystem_Pop : ParticleSystem_Generic
    {
        public AssetSingle Gravity { get; set; }
        public AssetSingle Drift { get; set; }
        public AssetSingle OutVelocity { get; set; }
        public AssetSingle Size { get; set; }
        public AssetSingle RotSpeed { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle MaxAlpha { get; set; }

        public ParticleSystem_Pop() { }
        public ParticleSystem_Pop(EndianBinaryReader reader) : base(reader)
        {
            Gravity = reader.ReadSingle();
            Drift = reader.ReadSingle();
            OutVelocity = reader.ReadSingle();
            Size = reader.ReadSingle();
            RotSpeed = reader.ReadSingle();
            Lifetime = reader.ReadSingle();
            MaxAlpha = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Gravity);
            writer.Write(Drift);
            writer.Write(OutVelocity);
            writer.Write(Size);
            writer.Write(RotSpeed);
            writer.Write(Lifetime);
            writer.Write(MaxAlpha);
            writer.Write(new byte[0x30]);
        }
    }

    public class ParticleSystem_BurrowKick : ParticleSystem_Generic
    {
        public AssetSingle Gravity { get; set; }
        public AssetSingle Size { get; set; }
        public AssetSingle RotSpeed { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle MaxAlpha { get; set; }
        public AssetSingle InitUpVelocity { get; set; }
        public AssetSingle RandOutVelocity { get; set; }
        public AssetID ShrapID { get; set; }
        public AssetSingle ShrapEmitRate { get; set; }
        public AssetSingle InitDownVelocity { get; set; }
        public AssetSingle RandVelocity { get; set; }

        public ParticleSystem_BurrowKick() { }
        public ParticleSystem_BurrowKick(EndianBinaryReader reader) : base(reader)
        {
            Gravity = reader.ReadSingle();
            Size = reader.ReadSingle();
            RotSpeed = reader.ReadSingle();
            Lifetime = reader.ReadSingle();
            MaxAlpha = reader.ReadSingle();
            InitUpVelocity = reader.ReadSingle();
            RandOutVelocity = reader.ReadSingle();
            ShrapID = reader.ReadUInt32();
            ShrapEmitRate = reader.ReadSingle();
            InitDownVelocity = reader.ReadSingle();
            RandVelocity = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Gravity);
            writer.Write(Size);
            writer.Write(RotSpeed);
            writer.Write(Lifetime);
            writer.Write(MaxAlpha);
            writer.Write(InitUpVelocity);
            writer.Write(RandOutVelocity);
            writer.Write(ShrapID);
            writer.Write(ShrapEmitRate);
            writer.Write(InitDownVelocity);
            writer.Write(RandVelocity);
            writer.Write(new byte[0x20]);
        }
    }

    public class ParticleSystem_Puffer : ParticleSystem_Generic
    {
        public AssetSingle MinTimeOn { get; set; }
        public AssetSingle MaxTimeOn { get; set; }
        public AssetSingle MinTimeOff { get; set; }
        public AssetSingle MaxTimeOff { get; set; }
        public AssetSingle Gravity { get; set; }
        public AssetSingle Size { get; set; }
        public AssetSingle RotSpeed { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle MaxAlpha { get; set; }
        public AssetSingle InitUpVelocity { get; set; }
        public AssetSingle RandOutVelocity { get; set; }
        public AssetID ShrapID { get; set; }
        public AssetSingle ShrapEmitRate { get; set; }
        public AssetSingle InitDownVelocity { get; set; }
        public AssetSingle RandVelocity { get; set; }

        public ParticleSystem_Puffer() { }
        public ParticleSystem_Puffer(EndianBinaryReader reader) : base(reader)
        {
            MinTimeOn = reader.ReadSingle();
            MaxTimeOn = reader.ReadSingle();
            MinTimeOff = reader.ReadSingle();
            MaxTimeOff = reader.ReadSingle();
            Gravity = reader.ReadSingle();
            Size = reader.ReadSingle();
            RotSpeed = reader.ReadSingle();
            Lifetime = reader.ReadSingle();
            MaxAlpha = reader.ReadSingle();
            InitUpVelocity = reader.ReadSingle();
            RandOutVelocity = reader.ReadSingle();
            ShrapID = reader.ReadUInt32();
            ShrapEmitRate = reader.ReadSingle();
            InitDownVelocity = reader.ReadSingle();
            RandVelocity = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(MinTimeOn);
            writer.Write(MaxTimeOn);
            writer.Write(MinTimeOff);
            writer.Write(MaxTimeOff);
            writer.Write(Gravity);
            writer.Write(Size);
            writer.Write(RotSpeed);
            writer.Write(Lifetime);
            writer.Write(MaxAlpha);
            writer.Write(InitUpVelocity);
            writer.Write(RandOutVelocity);
            writer.Write(ShrapID);
            writer.Write(ShrapEmitRate);
            writer.Write(InitDownVelocity);
            writer.Write(RandVelocity);
            writer.Write(new byte[0x10]);
        }
    }

}
