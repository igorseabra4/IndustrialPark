using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public enum ParticleCommandType : int
    {
        Move = 0,
        MoveRandom = 1,
        Accelerate = 2,
        VelocityApply = 3,
        Jet = 4,
        KillSlow = 6,
        Follow = 7,
        OrbitPoint = 8,
        OrbitLine = 9,
        MoveRandomPar = 10,
        Scale3rdPolyReg = 11,
        Tex = 12,
        TexAnim = 13,
        PlayerCollision = 14,
        RandomVelocityPar = 15,
        Custom = 16,
        KillDistance = 17,
        Age = 18,
        Alpha3rdPolyReg = 19,
        ApplyWind = 20,
        RotPar = 21,
        ApplyCamMat = 22,
        RotateAround = 23,
        SmokeAlpha = 24,
        Scale = 25,
        ClipVolumes = 26,
        AnimalMagentism = 27,
        DamagePlayer = 28,
        CollideFall = 29,
        Shaper = 30,
        AlphaInOut = 31,
        SizeInOut = 32,
        DampenSpeed = 33,
        FallSticky = 34
    }

    public abstract class ParticleCommand : GenericAssetDataContainer
    {
        protected const string categoryName = "ParticleCommand";
        public bool NeedsFix = false;

        [Category(categoryName), ReadOnly(true)]
        public ParticleCommandType CommandType { get; set; }
        [Category(categoryName)]
        public bool Enabled { get; set; }
        [Category(categoryName)]
        public AssetByte Mode { get; set; }
        [Category(categoryName), Browsable(false)]
        public short Pad { get; set; }

        public ParticleCommand(ParticleCommandType commandType)
        {
            Enabled = true;
            CommandType = commandType;
        }

        public ParticleCommand(EndianBinaryReader reader, ParticleCommandType type)
        {
            byte[] test = reader.ReadBytes(4);
            if (test[2] != 0 || test[3] != 0) // Custom particle system created in IP v2024.02.10 and below
            {
                Array.Reverse(test);
                NeedsFix = true;
            }
            CommandType = type;
            Enabled = Convert.ToBoolean(test[0]);
            Mode = test[1];
            Pad = BitConverter.ToInt16(test, 2);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)CommandType);
            writer.Write(Enabled);
            writer.Write(Mode);
            writer.Write((short)0);
        }
    }

    public class ParticleCommand_Move : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Move";

        [Category(categoryNameE)]
        public AssetSingle DirectionX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DirectionY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DirectionZ { get; set; }

        public ParticleCommand_Move() : base(ParticleCommandType.Move) { }
        public ParticleCommand_Move(EndianBinaryReader reader) : base(reader, ParticleCommandType.Move)
        {
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
        }
    }

    public class ParticleCommand_MoveRandom : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": MoveRandom";

        [Category(categoryNameE)]
        public AssetSingle DimX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DimY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DimZ { get; set; }

        public ParticleCommand_MoveRandom() : base(ParticleCommandType.MoveRandom) { }

        public ParticleCommand_MoveRandom(EndianBinaryReader reader) : base(reader, ParticleCommandType.MoveRandom)
        {
            DimX = reader.ReadSingle();
            DimY = reader.ReadSingle();
            DimZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(DimX);
            writer.Write(DimY);
            writer.Write(DimZ);
        }
    }

    public class ParticleCommand_MoveRandomPar : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": MoveRandomPar";

        [Category(categoryNameE)]
        public AssetSingle DimX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DimY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DimZ { get; set; }

        public ParticleCommand_MoveRandomPar() : base(ParticleCommandType.MoveRandomPar) { }

        public ParticleCommand_MoveRandomPar(EndianBinaryReader reader) : base(reader, ParticleCommandType.MoveRandomPar)
        {
            DimX = reader.ReadSingle();
            DimY = reader.ReadSingle();
            DimZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(DimX);
            writer.Write(DimY);
            writer.Write(DimZ);
        }
    }

    public class ParticleCommand_Accelerate : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Accelerate";

        [Category(categoryNameE)]
        public AssetSingle AccelerateX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AccelerateY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AccelerateZ { get; set; }

        public ParticleCommand_Accelerate() : base(ParticleCommandType.Accelerate) { }
        public ParticleCommand_Accelerate(EndianBinaryReader reader) : base(reader, ParticleCommandType.Accelerate)
        {
            AccelerateX = reader.ReadSingle();
            AccelerateY = reader.ReadSingle();
            AccelerateZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(AccelerateX);
            writer.Write(AccelerateY);
            writer.Write(AccelerateZ);
        }
    }

    public class ParticleCommand_VelocityApply : ParticleCommand
    {
        public ParticleCommand_VelocityApply() : base(ParticleCommandType.VelocityApply) { }
        public ParticleCommand_VelocityApply(EndianBinaryReader reader) : base(reader, ParticleCommandType.VelocityApply) { }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
        }
    }

    public class ParticleCommand_Jet : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Jet";

        [Category(categoryNameE)]
        public AssetSingle CenterX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle CenterY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle CenterZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AccelerateX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AccelerateY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AccelerateZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Epsilon { get; set; }
        [Category(categoryNameE)]
        public AssetSingle RadiusSquared { get; set; }

        public ParticleCommand_Jet() : base(ParticleCommandType.Jet) { }
        public ParticleCommand_Jet(EndianBinaryReader reader) : base(reader, ParticleCommandType.Jet)
        {
            CenterX = reader.ReadSingle();
            CenterY = reader.ReadSingle();
            CenterZ = reader.ReadSingle();
            AccelerateX = reader.ReadSingle();
            AccelerateY = reader.ReadSingle();
            AccelerateZ = reader.ReadSingle();
            Gravity = reader.ReadSingle();
            Epsilon = reader.ReadSingle();
            RadiusSquared = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CenterX);
            writer.Write(CenterY);
            writer.Write(CenterZ);
            writer.Write(AccelerateX);
            writer.Write(AccelerateY);
            writer.Write(AccelerateZ);
            writer.Write(Gravity);
            writer.Write(Epsilon);
            writer.Write(RadiusSquared);
        }
    }

    public class ParticleCommand_KillSlow : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": KillSlow";

        [Category(categoryNameE)]
        public AssetSingle SpeedLimitSquared { get; set; }
        [Category(categoryNameE)]
        public uint KillLessThan { get; set; }

        public ParticleCommand_KillSlow() : base(ParticleCommandType.KillSlow) { }
        public ParticleCommand_KillSlow(EndianBinaryReader reader) : base(reader, ParticleCommandType.KillSlow)
        {
            SpeedLimitSquared = reader.ReadSingle();
            KillLessThan = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SpeedLimitSquared);
            writer.Write(KillLessThan);
        }
    }

    public class ParticleCommand_Follow : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Follow";

        [Category(categoryNameE)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Epsilon { get; set; }

        public ParticleCommand_Follow() : base(ParticleCommandType.Follow) { }
        public ParticleCommand_Follow(EndianBinaryReader reader) : base(reader, ParticleCommandType.Follow)
        {
            Gravity = reader.ReadSingle();
            Epsilon = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Gravity);
            writer.Write(Epsilon);
        }
    }

    public class ParticleCommand_OrbitPoint : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": OrbitPoint";

        [Category(categoryNameE)]
        public AssetSingle CenterX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle CenterY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle CenterZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Epsilon { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxRadiusSquared { get; set; }

        public ParticleCommand_OrbitPoint() : base(ParticleCommandType.OrbitPoint) { }
        public ParticleCommand_OrbitPoint(EndianBinaryReader reader) : base(reader, ParticleCommandType.OrbitPoint)
        {
            CenterX = reader.ReadSingle();
            CenterY = reader.ReadSingle();
            CenterZ = reader.ReadSingle();
            Gravity = reader.ReadSingle();
            Epsilon = reader.ReadSingle();
            MaxRadiusSquared = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CenterX);
            writer.Write(CenterY);
            writer.Write(CenterZ);
            writer.Write(Gravity);
            writer.Write(Epsilon);
            writer.Write(MaxRadiusSquared);
        }

    }

    public class ParticleCommand_OrbitLine : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": OrbitLine";

        [Category(categoryNameE)]
        public AssetSingle PointX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle PointY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle PointZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AxisX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AxisY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle AxisZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Epsilon { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxRadiusSquared { get; set; }

        public ParticleCommand_OrbitLine() : base(ParticleCommandType.OrbitLine) { }
        public ParticleCommand_OrbitLine(EndianBinaryReader reader) : base(reader, ParticleCommandType.OrbitLine)
        {
            PointX = reader.ReadSingle();
            PointY = reader.ReadSingle();
            PointZ = reader.ReadSingle();
            AxisX = reader.ReadSingle();
            AxisY = reader.ReadSingle();
            AxisZ = reader.ReadSingle();
            Gravity = reader.ReadSingle();
            Epsilon = reader.ReadSingle();
            MaxRadiusSquared = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(PointX);
            writer.Write(PointY);
            writer.Write(PointZ);
            writer.Write(AxisX);
            writer.Write(AxisY);
            writer.Write(AxisZ);
            writer.Write(Gravity);
            writer.Write(Epsilon);
            writer.Write(MaxRadiusSquared);
        }
    }

    public class ParticleCommand_Scale3rdPolyReg : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Scale3rdPolyReg";

        [Category(categoryNameE)]
        public AssetSingle Constant { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A2 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A3 { get; set; }

        public ParticleCommand_Scale3rdPolyReg() : base(ParticleCommandType.Scale3rdPolyReg) { }

        public ParticleCommand_Scale3rdPolyReg(EndianBinaryReader reader) : base(reader, ParticleCommandType.Scale3rdPolyReg)
        {
            Constant = reader.ReadSingle();
            A = reader.ReadSingle();
            A2 = reader.ReadSingle();
            A3 = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Constant);
            writer.Write(A);
            writer.Write(A2);
            writer.Write(A3);
        }
    }

    public class ParticleCommand_Alpha3rdPolyReg : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Alpha3rdPolyReg";

        [Category(categoryNameE)]
        public AssetSingle Constant { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A2 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle A3 { get; set; }

        public ParticleCommand_Alpha3rdPolyReg() : base(ParticleCommandType.Alpha3rdPolyReg) { }

        public ParticleCommand_Alpha3rdPolyReg(EndianBinaryReader reader) : base(reader, ParticleCommandType.Alpha3rdPolyReg)
        {
            Constant = reader.ReadSingle();
            A = reader.ReadSingle();
            A2 = reader.ReadSingle();
            A3 = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Constant);
            writer.Write(A);
            writer.Write(A2);
            writer.Write(A3);
        }
    }

    public class ParticleCommand_Tex : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Tex";

        [Category(categoryNameE)]
        public AssetSingle X1 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Y1 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle X2 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Y2 { get; set; }
        [Category(categoryNameE)]
        public AssetByte BirthMode { get; set; }
        [Category(categoryNameE)]
        public AssetByte Rows { get; set; }
        [Category(categoryNameE)]
        public AssetByte Cols { get; set; }
        [Category(categoryNameE)]
        public AssetByte UnitCount { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UnitWidth { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UnitHeight { get; set; }

        public ParticleCommand_Tex() : base(ParticleCommandType.Tex) { }
        public ParticleCommand_Tex(EndianBinaryReader reader) : base(reader, ParticleCommandType.Tex)
        {
            X1 = reader.ReadSingle();
            Y1 = reader.ReadSingle();
            X2 = reader.ReadSingle();
            Y2 = reader.ReadSingle();
            BirthMode = reader.ReadByte();
            Rows = reader.ReadByte();
            Cols = reader.ReadByte();
            UnitCount = reader.ReadByte();
            UnitWidth = reader.ReadSingle();
            UnitHeight = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(X1);
            writer.Write(Y1);
            writer.Write(X2);
            writer.Write(Y2);
            writer.Write(BirthMode);
            writer.Write(Rows);
            writer.Write(Cols);
            writer.Write(UnitCount);
            writer.Write(UnitWidth);
            writer.Write(UnitHeight);
        }

    }

    public class ParticleCommand_TexAnim : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": TexAnim";

        [Category(categoryNameE)]
        public AssetByte AnimMode { get; set; }
        [Category(categoryNameE)]
        public AssetByte AnimWrapMode { get; set; }
        [Category(categoryNameE)]
        public AssetByte PadAnim { get; set; }
        [Category(categoryNameE)]
        public AssetByte ThrottleSpeedLessThan { get; set; }
        [Category(categoryNameE)]
        public AssetSingle ThrottleSpeedSquared { get; set; }
        [Category(categoryNameE)]
        public AssetSingle ThrottleTime { get; set; }
        [Category(categoryNameE)]
        public AssetSingle ThrottleTimeElapsed { get; set; }

        public ParticleCommand_TexAnim() : base(ParticleCommandType.TexAnim) { }
        public ParticleCommand_TexAnim(EndianBinaryReader reader) : base(reader, ParticleCommandType.TexAnim)
        {
            AnimMode = reader.ReadByte();
            AnimWrapMode = reader.ReadByte();
            PadAnim = reader.ReadByte();
            ThrottleSpeedLessThan = reader.ReadByte();
            ThrottleSpeedSquared = reader.ReadSingle();
            ThrottleTime = reader.ReadSingle();
            ThrottleTimeElapsed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(AnimMode);
            writer.Write(AnimWrapMode);
            writer.Write(PadAnim);
            writer.Write(ThrottleSpeedLessThan);
            writer.Write(ThrottleSpeedSquared);
            writer.Write(ThrottleTime);
            writer.Write(ThrottleTimeElapsed);
        }
    }

    public class ParticleCommand_PlayerCollision : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": PlayerCollision";

        [Category(categoryNameE)]
        public AssetSingle MinX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MinY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MinZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxZ { get; set; }

        public ParticleCommand_PlayerCollision() : base(ParticleCommandType.PlayerCollision) { }
        public ParticleCommand_PlayerCollision(EndianBinaryReader reader) : base(reader, ParticleCommandType.PlayerCollision)
        {
            MinX = reader.ReadSingle();
            MinY = reader.ReadSingle();
            MinZ = reader.ReadSingle();
            MaxX = reader.ReadSingle();
            MaxY = reader.ReadSingle();
            MaxZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MinX);
            writer.Write(MinY);
            writer.Write(MinZ);
            writer.Write(MaxX);
            writer.Write(MaxY);
            writer.Write(MaxZ);
        }

    }

    public class ParticleCommand_RotPar : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": RotPar";

        [Category(categoryNameE)]
        public AssetSingle MinX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MinY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MinZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle MaxZ { get; set; }

        public ParticleCommand_RotPar() : base(ParticleCommandType.RotPar) { }
        public ParticleCommand_RotPar(EndianBinaryReader reader) : base(reader, ParticleCommandType.RotPar)
        {
            MinX = reader.ReadSingle();
            MinY = reader.ReadSingle();
            MinZ = reader.ReadSingle();
            MaxX = reader.ReadSingle();
            MaxY = reader.ReadSingle();
            MaxZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MinX);
            writer.Write(MinY);
            writer.Write(MinZ);
            writer.Write(MaxX);
            writer.Write(MaxY);
            writer.Write(MaxZ);
        }

    }

    public class ParticleCommand_RandomVelocityPar : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": RandomVelocityPar";

        [Category(categoryNameE)]
        public AssetSingle X { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Y { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Z { get; set; }

        public ParticleCommand_RandomVelocityPar() : base(ParticleCommandType.RandomVelocityPar) { }
        public ParticleCommand_RandomVelocityPar(EndianBinaryReader reader) : base(reader, ParticleCommandType.RandomVelocityPar)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }

    public class ParticleCommand_Custom : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Custom";

        [Category(categoryNameE)]
        public AssetID UserID { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UserValue1 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UserValue2 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UserValue3 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle UserValue4 { get; set; }

        public ParticleCommand_Custom() : base(ParticleCommandType.Custom) { }
        public ParticleCommand_Custom(EndianBinaryReader reader) : base(reader, ParticleCommandType.Custom)
        {
            UserID = reader.ReadUInt32();
            UserValue1 = reader.ReadSingle();
            UserValue2 = reader.ReadSingle();
            UserValue3 = reader.ReadSingle();
            UserValue4 = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UserID);
            writer.Write(UserValue1);
            writer.Write(UserValue2);
            writer.Write(UserValue3);
            writer.Write(UserValue4);
        }
    }

    public class ParticleCommand_KillDistance : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": KillDistance";

        [Category(categoryNameE)]
        public AssetSingle dSquared { get; set; }
        [Category(categoryNameE)]
        public uint KillGreaterThan { get; set; }

        public ParticleCommand_KillDistance() : base(ParticleCommandType.KillDistance) { }
        public ParticleCommand_KillDistance(EndianBinaryReader reader) : base(reader, ParticleCommandType.KillDistance)
        {
            dSquared = reader.ReadSingle();
            KillGreaterThan = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(dSquared);
            writer.Write(KillGreaterThan);
        }
    }

    public class ParticleCommand_Age : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Age";

        [Category(categoryNameE)]
        public AssetSingle AgeRate { get; set; }

        public ParticleCommand_Age() : base(ParticleCommandType.Age) { }
        public ParticleCommand_Age(EndianBinaryReader reader) : base(reader, ParticleCommandType.Age)
        {
            AgeRate = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(AgeRate);
        }
    }

    public class ParticleCommand_ApplyWind : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": ApplyWind";

        [Category(categoryNameE)]
        public AssetSingle Strength { get; set; }

        public ParticleCommand_ApplyWind() : base(ParticleCommandType.ApplyWind) { }
        public ParticleCommand_ApplyWind(EndianBinaryReader reader) : base(reader, ParticleCommandType.ApplyWind)
        {
            Strength = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Strength);
        }
    }

    public class ParticleCommand_ApplyCamMat : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": ApplyCamMat";

        [Category(categoryNameE)]
        public AssetSingle ApplyX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle ApplyY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle ApplyZ { get; set; }

        public ParticleCommand_ApplyCamMat() : base(ParticleCommandType.ApplyCamMat) { }
        public ParticleCommand_ApplyCamMat(EndianBinaryReader reader) : base(reader, ParticleCommandType.ApplyCamMat)
        {
            ApplyX = reader.ReadSingle();
            ApplyY = reader.ReadSingle();
            ApplyZ = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(ApplyX);
            writer.Write(ApplyY);
            writer.Write(ApplyZ);
        }
    }

    public class ParticleCommand_RotateAround : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": RotateAround";

        [Category(categoryNameE)]
        public AssetSingle PosX { get; set; }
        [Category(categoryNameE)]
        public AssetSingle PosY { get; set; }
        [Category(categoryNameE)]
        public AssetSingle PosZ { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Unused1 { get; set; }
        [Category(categoryNameE)]
        public AssetSingle RadiusGrowth { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Yaw { get; set; }

        public ParticleCommand_RotateAround() : base(ParticleCommandType.RotateAround) { }
        public ParticleCommand_RotateAround(EndianBinaryReader reader) : base(reader, ParticleCommandType.RotateAround)
        {
            PosX = reader.ReadSingle();
            PosY = reader.ReadSingle();
            PosZ = reader.ReadSingle();
            Unused1 = reader.ReadSingle();
            RadiusGrowth = reader.ReadSingle();
            Yaw = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write(PosZ);
            writer.Write(Unused1);
            writer.Write(RadiusGrowth);
            writer.Write(Yaw);
        }
    }

    public class ParticleCommand_SmokeAlpha : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": SmokeAlpha";

        [Category(categoryNameE)]
        public int Type { get; set; }

        public ParticleCommand_SmokeAlpha() : base(ParticleCommandType.SmokeAlpha) { }

        public ParticleCommand_SmokeAlpha(EndianBinaryReader reader) : base(reader, ParticleCommandType.SmokeAlpha)
        {
            Type = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Type);
        }
    }

    public class ParticleCommand_Scale : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Scale";

        [Category(categoryNameE)]
        public int Type { get; set; }

        public ParticleCommand_Scale() : base(ParticleCommandType.Scale) { }

        public ParticleCommand_Scale(EndianBinaryReader reader) : base(reader, ParticleCommandType.Scale)
        {
            Type = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Type);
        }
    }

    public class ParticleCommand_ClipVolumes : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": ClipVolumes";

        [Category(categoryNameE)]
        public int Unused { get; set; }

        public ParticleCommand_ClipVolumes() : base(ParticleCommandType.ClipVolumes) { }
        public ParticleCommand_ClipVolumes(EndianBinaryReader reader) : base(reader, ParticleCommandType.ClipVolumes)
        {
            Unused = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Unused);
        }
    }

    public class ParticleCommand_AnimalMagentism : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": AnimalMagentism";

        [Category(categoryNameE)]
        public AssetSingle Magnetism { get; set; }

        public ParticleCommand_AnimalMagentism() : base(ParticleCommandType.AnimalMagentism) { }
        public ParticleCommand_AnimalMagentism(EndianBinaryReader reader) : base(reader, ParticleCommandType.AnimalMagentism)
        {
            Magnetism = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Magnetism);
        }
    }

    public class ParticleCommand_DamagePlayer : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": DamagePlayer";

        [Category(categoryNameE)]
        public int Damage { get; set; }
        [Category(categoryNameE)]
        public int Granular { get; set; }

        public ParticleCommand_DamagePlayer() : base(ParticleCommandType.DamagePlayer) { }
        public ParticleCommand_DamagePlayer(EndianBinaryReader reader) : base(reader, ParticleCommandType.DamagePlayer)
        {
            Damage = reader.ReadInt32();
            Granular = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Damage);
            writer.Write(Granular);
        }
    }

    public class ParticleCommand_CollideFall : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": CollideFall";

        [Category(categoryNameE)]
        public AssetSingle Y { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Bounce { get; set; }

        public ParticleCommand_CollideFall() : base(ParticleCommandType.CollideFall) { }

        public ParticleCommand_CollideFall(EndianBinaryReader reader) : base(reader, ParticleCommandType.CollideFall)
        {
            Y = reader.ReadSingle();
            Bounce = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Y);
            writer.Write(Bounce);
        }
    }

    public class ParticleCommand_CollideFallSticky : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": CollideFallSticky";

        [Category(categoryNameE)]
        public AssetSingle Y { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Bounce { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Sticky { get; set; }

        public ParticleCommand_CollideFallSticky() : base(ParticleCommandType.FallSticky) { }

        public ParticleCommand_CollideFallSticky(EndianBinaryReader reader) : base(reader, ParticleCommandType.FallSticky)
        {
            Y = reader.ReadSingle();
            Bounce = reader.ReadSingle();
            Sticky = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Y);
            writer.Write(Bounce);
            writer.Write(Sticky);
        }
    }

    public class ParticleCommand_Shaper : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": Shaper";

        [Category(categoryNameE)]
        public AssetSingle[] CustAlpha { get; set; }
        [Category(categoryNameE)]
        public AssetSingle[] CustSize { get; set; }
        [Category(categoryNameE)]
        public AssetSingle DampSpeed { get; set; }
        [Category(categoryNameE)]
        public AssetSingle Gravity { get; set; }

        public ParticleCommand_Shaper() : base(ParticleCommandType.Shaper)
        {
            CustAlpha = new AssetSingle[4];
            CustSize = new AssetSingle[4];
        }

        public ParticleCommand_Shaper(EndianBinaryReader reader) : base(reader, ParticleCommandType.Shaper)
        {
            CustAlpha = new AssetSingle[4];
            for (int i = 0; i < 4; i++)
                CustAlpha[i] = reader.ReadSingle();

            CustSize = new AssetSingle[4];
            for (int i = 0; i < 4; i++)
                CustSize[i] = reader.ReadSingle();

            DampSpeed = reader.ReadSingle();
            Gravity = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            for (int i = 0; i < 4; i++)
                writer.Write(CustAlpha[i]);
            for (int i = 0; i < 4; i++)
                writer.Write(CustSize[i]);
            writer.Write(DampSpeed);
            writer.Write(Gravity);
        }
    }

    public class ParticleCommand_AlphaInOut : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": AlphaInOut";

        [Category(categoryNameE)]
        public AssetSingle[] CustAlpha { get; set; }

        public ParticleCommand_AlphaInOut() : base(ParticleCommandType.AlphaInOut)
        {
            CustAlpha = new AssetSingle[4];
        }

        public ParticleCommand_AlphaInOut(EndianBinaryReader reader) : base(reader, ParticleCommandType.AlphaInOut)
        {
            for (int i = 0; i < 4; i++)
                CustAlpha[i] = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            for (int i = 0; i < 4; i++)
                writer.Write(CustAlpha[i]);
        }
    }

    public class ParticleCommand_SizeInOut : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": SizeInOut";

        [Category(categoryNameE)]
        public AssetSingle[] CustSize { get; set; }

        public ParticleCommand_SizeInOut() : base(ParticleCommandType.SizeInOut)
        {
            CustSize = new AssetSingle[4];
        }

        public ParticleCommand_SizeInOut(EndianBinaryReader reader) : base(reader, ParticleCommandType.SizeInOut)
        {
            for (int i = 0; i < 4; i++)
                CustSize[i] = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            for (int i = 0; i < 4; i++)
                writer.Write(CustSize[i]);
        }
    }

    public class ParticleCommand_DampenData : ParticleCommand
    {
        private const string categoryNameE = categoryName + ": DampenData";

        [Category(categoryNameE)]
        public AssetSingle DampSpeed { get; set; }

        public ParticleCommand_DampenData() : base(ParticleCommandType.DampenSpeed) { }
        public ParticleCommand_DampenData(EndianBinaryReader reader) : base(reader, ParticleCommandType.DampenSpeed)
        {
            DampSpeed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(DampSpeed);
        }
    }
}
