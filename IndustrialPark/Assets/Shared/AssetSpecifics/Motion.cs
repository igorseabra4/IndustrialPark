using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum MotionType : byte
    {
        ExtendRetract = 0,
        Orbit = 1,
        Spline = 2,
        MovePoint = 3,
        Mechanism = 4,
        Pendulum = 5,
        Other = 6,
    }

    public enum CurrentMovementAction
    {
        StartWait,
        Going,
        EndWait,
        GoingBack
    }

    public class Motion : GenericAssetDataContainer
    {
        [Browsable(false)]
        public MotionType Type { get; set; }
        public byte UseBanking { get; set; }
        public FlagBitmask MotionFlags { get; set; } = ShortFlagsDescriptor(
            "Face movement direction",
            null,
            "Don't start moving");

        public Motion(Game game, MotionType type)
        {
            _game = game;
            Type = type;
        }

        public Motion(EndianBinaryReader reader, Game game) : this(game, (MotionType)reader.ReadByte())
        {
            UseBanking = reader.ReadByte();
            MotionFlags.FlagValueShort = reader.ReadUInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(UseBanking);
            writer.Write(MotionFlags.FlagValueShort);
        }

        protected int LocalFrameCounter = -1;

        public virtual void Reset()
        {
            LocalFrameCounter = -1;
        }

        public virtual void Update()
        {
            LocalFrameCounter++;
        }

        public virtual Matrix PlatLocalTranslation()
        {
            return Matrix.Identity;
        }

        public virtual Matrix PlatLocalRotation()
        {
            return Matrix.Identity;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public class Motion_ExtendRetract : Motion
    {
        private const string Note = "Note: for Movement Preview to work correctly, RetractPosition must be the same as Position (X, Y, Z)";

        [Description(Note)]
        public AssetSingle RetractPositionX { get; set; }
        [Description(Note)]
        public AssetSingle RetractPositionY { get; set; }
        [Description(Note)]
        public AssetSingle RetractPositionZ { get; set; }
        public AssetSingle ExtendDeltaPositionX { get; set; }
        public AssetSingle ExtendDeltaPositionY { get; set; }
        public AssetSingle ExtendDeltaPositionZ { get; set; }
        public AssetSingle ExtendTime { get; set; }
        public AssetSingle ExtendWaitTime { get; set; }
        public AssetSingle RetractTime { get; set; }
        public AssetSingle RetractWaitTime { get; set; }

        public Motion_ExtendRetract(Game game) : base(game, MotionType.ExtendRetract) { }

        public Motion_ExtendRetract(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            RetractPositionX = reader.ReadSingle();
            RetractPositionY = reader.ReadSingle();
            RetractPositionZ = reader.ReadSingle();
            ExtendDeltaPositionX = reader.ReadSingle();
            ExtendDeltaPositionY = reader.ReadSingle();
            ExtendDeltaPositionZ = reader.ReadSingle();
            ExtendTime = reader.ReadSingle();
            ExtendWaitTime = reader.ReadSingle();
            RetractTime = reader.ReadSingle();
            RetractWaitTime = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(RetractPositionX);
            writer.Write(RetractPositionY);
            writer.Write(RetractPositionZ);
            writer.Write(ExtendDeltaPositionX);
            writer.Write(ExtendDeltaPositionY);
            writer.Write(ExtendDeltaPositionZ);
            writer.Write(ExtendTime);
            writer.Write(ExtendWaitTime);
            writer.Write(RetractTime);
            writer.Write(RetractWaitTime);
        }

        private CurrentMovementAction currentMovementAction;

        public override void Reset()
        {
            currentMovementAction = CurrentMovementAction.StartWait;

            StartWaitRange = 60 * RetractWaitTime;
            GoingRange = StartWaitRange + 60 * ExtendTime;
            EndWaitRange = GoingRange + 60 * ExtendWaitTime;
            GoingBackRange = EndWaitRange + 60 * RetractTime;

            base.Reset();
        }

        private float StartWaitRange;
        private float GoingRange;
        private float EndWaitRange;
        private float GoingBackRange;

        public override Matrix PlatLocalTranslation()
        {
            float translationMultiplier = 0;

            switch (currentMovementAction)
            {
                case CurrentMovementAction.StartWait:
                    if (LocalFrameCounter >= StartWaitRange)
                        currentMovementAction = CurrentMovementAction.Going;
                    break;

                case CurrentMovementAction.Going:
                    if (ExtendTime == 0)
                    {
                        currentMovementAction = CurrentMovementAction.StartWait;
                        break;
                    }
                    translationMultiplier = (LocalFrameCounter - StartWaitRange) / (60 * ExtendTime);
                    if (LocalFrameCounter >= GoingRange)
                        currentMovementAction = CurrentMovementAction.EndWait;
                    break;

                case CurrentMovementAction.EndWait:
                    translationMultiplier = 1;
                    if (LocalFrameCounter >= EndWaitRange)
                        currentMovementAction = CurrentMovementAction.GoingBack;
                    break;

                case CurrentMovementAction.GoingBack:
                    if (RetractTime == 0)
                    {
                        currentMovementAction = CurrentMovementAction.StartWait;
                        break;
                    }
                    translationMultiplier = 1 - ((LocalFrameCounter - EndWaitRange) / (60 * RetractTime));
                    if (LocalFrameCounter >= GoingBackRange)
                    {
                        currentMovementAction = CurrentMovementAction.StartWait;
                        LocalFrameCounter = 0;
                    }
                    break;

                default:
                    Reset();
                    break;
            }

            return Matrix.Translation(
                RetractPositionX + ExtendDeltaPositionX * translationMultiplier,
                RetractPositionY + ExtendDeltaPositionY * translationMultiplier,
                RetractPositionZ + ExtendDeltaPositionZ * translationMultiplier);
        }
    }

    public class Motion_Orbit : Motion
    {
        public AssetSingle CenterX { get; set; }
        public AssetSingle CenterY { get; set; }
        public AssetSingle CenterZ { get; set; }
        public AssetSingle Width { get; set; }
        public AssetSingle Height { get; set; }
        public AssetSingle Period { get; set; }

        public Motion_Orbit(Game game) : base(game, MotionType.Orbit) { }
        public Motion_Orbit(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            CenterX = reader.ReadSingle();
            CenterY = reader.ReadSingle();
            CenterZ = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Period = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CenterX);
            writer.Write(CenterY);
            writer.Write(CenterZ);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Period);
        }

        public override Matrix PlatLocalTranslation()
        {
            var param = MathUtil.Pi + LocalFrameCounter * MathUtil.TwoPi / (Period * 60);

            return Matrix.Translation(
                Width + Width * (float)Math.Cos(param),
                0,
                Height * (float)Math.Sin(param));
        }
    }

    public class Motion_Spline : Motion
    {
        public string Note => "Speed and LeanModifier are only present in Movie/Incredibles.";

        public int SplineID { get; set; }
        public AssetSingle Speed { get; set; }
        public AssetSingle LeanModifier { get; set; }

        public Motion_Spline(Game game) : base(game, MotionType.Spline)
        {
            _game = game;
        }

        public Motion_Spline(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            _game = game;

            SplineID = reader.ReadInt32();
            Speed = reader.ReadSingle();
            LeanModifier = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SplineID);
            if (game >= Game.Incredibles)
            {
                writer.Write(Speed);
                writer.Write(LeanModifier);
            }
        }
    }

    public class Motion_MovePoint : Motion
    {
        public FlagBitmask MovePointFlags { get; set; } = IntFlagsDescriptor();
        [ValidReferenceRequired]
        public AssetID MovePoint { get; set; }
        public AssetSingle Speed { get; set; }

        public Motion_MovePoint(Game game, Vector3 initialPosition) : base(game, MotionType.MovePoint)
        {
            this.initialPosition = initialPosition;

            MovePoint = 0;
        }

        public Motion_MovePoint(EndianBinaryReader reader, Game game, Vector3 initialPosition) : base(reader, game)
        {
            this.initialPosition = initialPosition;

            MovePointFlags.FlagValueInt = reader.ReadUInt32();
            MovePoint = reader.ReadUInt32();
            Speed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MovePointFlags.FlagValueInt);
            writer.Write(MovePoint);
            writer.Write(Speed);
        }

        private AssetMVPT currentMVPT;
        private Vector3 initialPosition;
        private Vector3 oldPosition;
        private Vector3 targetPosition;
        private int delay;

        public override void Reset()
        {
            currentMVPT = FindMVPT(MovePoint);
            if (currentMVPT != null)
            {
                delay = (int)(currentMVPT.Delay * 60);
                targetPosition = new Vector3(currentMVPT.PositionX, currentMVPT.PositionY, currentMVPT.PositionZ);
                oldPosition = targetPosition;
            }
            else
                oldPosition = initialPosition;

            base.Reset();
        }

        private AssetMVPT FindMVPT(uint assetID)
        {
            if (Program.MainForm != null && assetID != 0)
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    if (ae.archive.TryGetAsset(assetID, out Asset asset))
                    {
                        if (asset is AssetMVPT MVPT)
                            return MVPT;
                    }
            return null;
        }

        public override Matrix PlatLocalTranslation()
        {
            if (delay > 0)
            {
                delay--;
                LocalFrameCounter--;
            }
            else if (currentMVPT != null)
            {
                Vector3 newPosition = Vector3.Lerp(oldPosition, targetPosition, Math.Min(1f, LocalFrameCounter / (Vector3.Distance(oldPosition, targetPosition) / (Math.Abs(Speed) / 60f))));

                if (Vector3.Distance(newPosition, targetPosition) < 0.05f)
                {
                    LocalFrameCounter = 0;
                    oldPosition = targetPosition;

                    if (currentMVPT.NextMovePoints.Length > 0)
                    {
                        currentMVPT = FindMVPT(currentMVPT.NextMovePoints[0]);
                        targetPosition = new Vector3(currentMVPT.PositionX, currentMVPT.PositionY, currentMVPT.PositionZ);
                        delay = (int)(currentMVPT.Delay * 60);
                    }
                    else
                        Reset();
                }

                return Matrix.Translation(newPosition);
            }

            return Matrix.Translation(oldPosition);
        }

        public void SetInitialPosition(Vector3 position)
        {
            initialPosition = position;
        }
    }

    public enum EMovementType : byte
    {
        Slide = 0,
        Rotate = 1,
        SlideAndRotate = 2,
        SlideThenRotate = 3,
        RotateThenSlide = 4,
        Scale = 5
    }

    public enum EMechanismFlags : byte
    {
        None = 0,
        ReturnToStart = 1,
        DontLoop = 2,
        ReturnToStartAndDontLoop = 3
    }

    public enum Axis : byte
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    public class Motion_Mechanism : Motion
    {
        public EMovementType MovementType { get; set; }
        public EMechanismFlags MovementLoopMode { get; set; }
        public Axis SlideAxis { get; set; }
        public Axis RotateAxis { get; set; }
        public AssetSingle SlideDistance { get; set; }
        public AssetSingle SlideTime { get; set; }
        public AssetSingle SlideAccelTime { get; set; }
        public AssetSingle SlideDecelTime { get; set; }
        public AssetSingle RotateDistance { get; set; }
        public AssetSingle RotateTime { get; set; }
        public AssetSingle RotateAccelTime { get; set; }
        public AssetSingle RotateDecelTime { get; set; }
        public AssetSingle RetractDelay { get; set; }
        public AssetSingle PostRetractDelay { get; set; }
        [Description("Movie/Incredibles only")]
        public byte ScaleAxis { get; set; }
        [Description("Movie/Incredibles only")]
        public AssetSingle ScaleAmount { get; set; }
        [Description("Movie/Incredibles only")]
        public AssetSingle ScaleDuration { get; set; }

        public Motion_Mechanism(Game game) : this(game, MotionType.Mechanism) { }
        public Motion_Mechanism(Game game, MotionType motionType) : base(game, motionType) { }
        public Motion_Mechanism(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            MovementType = (EMovementType)reader.ReadByte();
            MovementLoopMode = (EMechanismFlags)reader.ReadByte();
            SlideAxis = (Axis)reader.ReadByte();
            RotateAxis = (Axis)reader.ReadByte();
            if (game >= Game.Incredibles)
            {
                ScaleAxis = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }
            SlideDistance = reader.ReadSingle();
            SlideTime = reader.ReadSingle();
            SlideAccelTime = reader.ReadSingle();
            SlideDecelTime = reader.ReadSingle();
            RotateDistance = reader.ReadSingle();
            RotateTime = reader.ReadSingle();
            RotateAccelTime = reader.ReadSingle();
            RotateDecelTime = reader.ReadSingle();
            RetractDelay = reader.ReadSingle();
            PostRetractDelay = reader.ReadSingle();
            if (game >= Game.Incredibles)
            {
                ScaleAmount = reader.ReadSingle();
                ScaleDuration = reader.ReadSingle();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((byte)MovementType);
            writer.Write((byte)MovementLoopMode);
            writer.Write((byte)SlideAxis);
            writer.Write((byte)RotateAxis);
            if (game >= Game.Incredibles)
            {
                writer.Write(ScaleAxis);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            writer.Write(SlideDistance);
            writer.Write(SlideTime);
            writer.Write(SlideAccelTime);
            writer.Write(SlideDecelTime);
            writer.Write(RotateDistance);
            writer.Write(RotateTime);
            writer.Write(RotateAccelTime);
            writer.Write(RotateDecelTime);
            writer.Write(RetractDelay);
            writer.Write(PostRetractDelay);
            if (game >= Game.Incredibles)
            {
                writer.Write(ScaleAmount);
                writer.Write(ScaleDuration);
            }
        }

        private CurrentMovementAction currentMovementAction;
        private int amountOfMovementsPerformed;
        private float slideMovementMultiplier;
        private float rotationMovementMultiplier;

        public override void Reset()
        {
            currentMovementAction = CurrentMovementAction.StartWait;
            amountOfMovementsPerformed = 0;
            slideMovementMultiplier = 0;
            rotationMovementMultiplier = 0;
            base.Reset();
        }

        private bool HasSlide =>
            MovementType == EMovementType.Slide ||
            MovementType == EMovementType.SlideAndRotate ||
            MovementType == EMovementType.SlideThenRotate ||
            MovementType == EMovementType.RotateThenSlide;
        private bool HasRotation =>
            MovementType == EMovementType.Rotate ||
            MovementType == EMovementType.SlideAndRotate ||
            MovementType == EMovementType.SlideThenRotate ||
            MovementType == EMovementType.RotateThenSlide;
        private bool ReturnsToStart => ((int)MovementLoopMode & (int)EMechanismFlags.ReturnToStart) != 0;
        private bool DontLoop => ((int)MovementLoopMode & (int)EMechanismFlags.DontLoop) != 0;

        private float MovementDuration => MovementType switch
        {
            EMovementType.Slide => SlideTime,
            EMovementType.Rotate => RotateTime,
            EMovementType.SlideAndRotate => Math.Max(SlideTime, RotateTime),
            EMovementType.SlideThenRotate => SlideTime + RotateTime,
            EMovementType.RotateThenSlide => RotateTime + SlideTime,
            _ => 0f,
        };

        private float StartWaitRange => 60f * PostRetractDelay;
        private float GoingRange => StartWaitRange + 60f * MovementDuration;
        private float EndWaitRange => GoingRange + 60f * RetractDelay;
        private float GoingBackRange => EndWaitRange + 60f * MovementDuration;

        private static float CalculateMovementProgress(float time, float duration, float accelTime, float decelTime)
        {
            if (duration <= 0f)
                return 1f;

            time = Math.Max(0f, Math.Min(time, duration));

            accelTime = Math.Max(0f, accelTime);
            decelTime = Math.Max(0f, decelTime);

            if (accelTime + decelTime > duration)
            {
                float total = accelTime + decelTime;
                if (total > 0f)
                {
                    float scale = duration / total;
                    accelTime *= scale;
                    decelTime *= scale;
                }
            }

            float cruiseTime = duration - accelTime - decelTime;

            if (accelTime <= 0f && decelTime <= 0f)
                return time / duration;

            float denominator = cruiseTime + (accelTime + decelTime) * 0.5f;
            if (denominator <= 0f)
                return time / duration;

            float peakVelocity = 1f / denominator;

            if (accelTime > 0f && time < accelTime)
            {
                float acceleration = peakVelocity / accelTime;
                return 0.5f * acceleration * time * time;
            }

            float accelerationDistance = 0.5f * peakVelocity * accelTime;

            if (time < accelTime + cruiseTime)
                return accelerationDistance + peakVelocity * (time - accelTime);

            if (decelTime > 0f)
            {
                float decelerationTime = time - accelTime - cruiseTime;
                float cruiseDistance = peakVelocity * cruiseTime;
                float decelerationStartDistance = accelerationDistance + cruiseDistance;
                float deceleration = peakVelocity / decelTime;
                return Math.Min(1f, decelerationStartDistance + peakVelocity * decelerationTime - 0.5f * deceleration * decelerationTime * decelerationTime);
            }
            return Math.Min(1f, accelerationDistance + peakVelocity * cruiseTime);
        }

        private float GetSlideProgress(float time)
        {
            if (!HasSlide)
                return 0f;
            return MovementType switch
            {
                EMovementType.Slide or EMovementType.SlideAndRotate => CalculateMovementProgress(time, SlideTime, SlideAccelTime, SlideDecelTime),
                EMovementType.SlideThenRotate => CalculateMovementProgress(time, SlideTime, SlideAccelTime, SlideDecelTime),
                EMovementType.RotateThenSlide => CalculateMovementProgress(time - RotateTime, SlideTime, SlideAccelTime, SlideDecelTime),
                _ => 0f,
            };
        }

        private float GetRotationProgress(float time)
        {
            if (!HasRotation)
                return 0f;
            return MovementType switch
            {
                EMovementType.Rotate or EMovementType.SlideAndRotate => CalculateMovementProgress(time, RotateTime, RotateAccelTime, RotateDecelTime),
                EMovementType.SlideThenRotate => CalculateMovementProgress(time - SlideTime, RotateTime, RotateAccelTime, RotateDecelTime),
                EMovementType.RotateThenSlide => CalculateMovementProgress(time, RotateTime, RotateAccelTime, RotateDecelTime),
                _ => 0f,
            };
        }

        public override void Update()
        {
            base.Update();

            switch (currentMovementAction)
            {
                case CurrentMovementAction.StartWait:
                    slideMovementMultiplier = HasSlide ? amountOfMovementsPerformed : 0f;
                    rotationMovementMultiplier = HasRotation ? amountOfMovementsPerformed : 0f;
                    if (LocalFrameCounter >= StartWaitRange)
                        currentMovementAction = CurrentMovementAction.Going;
                    break;
                case CurrentMovementAction.Going:
                {
                    float movementTime = Math.Max(0f, (LocalFrameCounter - StartWaitRange) / 60f);
                    float slideProgress = GetSlideProgress(movementTime);
                    float rotationProgress = GetRotationProgress(movementTime);
                    float movementOffset = amountOfMovementsPerformed;
                    if (ReturnsToStart)
                    {
                        slideMovementMultiplier = HasSlide ? slideProgress : 0f;
                        rotationMovementMultiplier = HasRotation ? rotationProgress : 0f;
                    }
                    else
                    {
                        slideMovementMultiplier = HasSlide ? movementOffset + slideProgress : 0f;
                        rotationMovementMultiplier = HasRotation ? movementOffset + rotationProgress : 0f;
                    }
                    if (LocalFrameCounter >= GoingRange)
                    {
                        if (ReturnsToStart)
                        {
                            slideMovementMultiplier = HasSlide ? 1f : 0f;
                            rotationMovementMultiplier = HasRotation ? 1f : 0f;
                            currentMovementAction = CurrentMovementAction.EndWait;
                        }
                        else
                        {
                            if (!DontLoop)
                                amountOfMovementsPerformed++;
                            slideMovementMultiplier = HasSlide ? amountOfMovementsPerformed : 0f;
                            rotationMovementMultiplier = HasRotation ? amountOfMovementsPerformed : 0f;
                            LocalFrameCounter = 0;
                            currentMovementAction = CurrentMovementAction.StartWait;
                        }
                    }
                    break;
                }
                case CurrentMovementAction.EndWait:
                    slideMovementMultiplier = HasSlide ? 1f : 0f;
                    rotationMovementMultiplier = HasRotation ? 1f : 0f;
                    if (LocalFrameCounter >= EndWaitRange)
                        currentMovementAction = CurrentMovementAction.GoingBack;
                    break;
                case CurrentMovementAction.GoingBack:
                {
                    float movementTime = Math.Max(0f, (LocalFrameCounter - EndWaitRange) / 60f);
                    float slideProgress = GetSlideProgress(movementTime);
                    float rotationProgress = GetRotationProgress(movementTime);
                    slideMovementMultiplier = HasSlide ? 1f - slideProgress : 0f;
                    rotationMovementMultiplier = HasRotation ? 1f - rotationProgress : 0f;
                    if (LocalFrameCounter >= GoingBackRange)
                    {
                        slideMovementMultiplier = 0f;
                        rotationMovementMultiplier = 0f;
                        amountOfMovementsPerformed = 0;
                        LocalFrameCounter = 0;
                        currentMovementAction = CurrentMovementAction.StartWait;
                    }
                    break;
                }
            }
        }

        public override Matrix PlatLocalTranslation()
        {
            if (!HasSlide)
                return Matrix.Identity;
            float distance = slideMovementMultiplier * (float)SlideDistance;
            return SlideAxis switch
            {
                Axis.X => Matrix.Translation(distance, 0f, 0f),
                Axis.Y => Matrix.Translation(0f, distance, 0f),
                Axis.Z => Matrix.Translation(0f, 0f, distance),
                _ => Matrix.Identity,
            };
        }

        public override Matrix PlatLocalRotation()
        {
            if (!HasRotation)
                return Matrix.Identity;
            float angle = rotationMovementMultiplier * MathUtil.DegreesToRadians((float)RotateDistance);
            return RotateAxis switch
            {
                Axis.X => Matrix.RotationX(angle),
                Axis.Y => Matrix.RotationY(angle),
                Axis.Z => Matrix.RotationZ(angle),
                _ => Matrix.Identity,
            };
        }
    }

    public class Motion_Pendulum : Motion
    {
        public AssetByte PendulumFlags { get; set; }
        public AssetByte Plane { get; set; }
        public AssetSingle Length { get; set; }
        public AssetSingle Range
        {
            get => MathUtil.RadiansToDegrees(Range_Rad);
            set => Range_Rad = MathUtil.DegreesToRadians(value);
        }
        public AssetSingle Period { get; set; }
        public AssetSingle Phase
        {
            get => MathUtil.RadiansToDegrees(Phase_Rad);
            set => Phase_Rad = MathUtil.DegreesToRadians(value);
        }
        private AssetSingle Range_Rad;
        private AssetSingle Phase_Rad;

        public Motion_Pendulum(Game game) : base(game, MotionType.Pendulum) { }
        public Motion_Pendulum(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            PendulumFlags = reader.ReadByte();
            Plane = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            Length = reader.ReadSingle();
            Range_Rad = reader.ReadSingle();
            Period = reader.ReadSingle();
            Phase_Rad = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(PendulumFlags);
            writer.Write(Plane);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(Length);
            writer.Write(Range_Rad);
            writer.Write(Period);
            writer.Write(Phase_Rad);
        }

        float speed = 0;
        float angle;
        bool comingBack = false;

        public override void Reset()
        {
            speed = 2 * Range_Rad / (Period * 30);
            angle = -Range_Rad;
            comingBack = false;
            base.Reset();
        }

        public override Matrix PlatLocalRotation()
        {
            if (LocalFrameCounter >= (Period * 30))
            {
                LocalFrameCounter = 0;
                comingBack = !comingBack;
            }

            //if (comingBack)
            //    angle = Range_Rad - LocalFrameCounter * speed;
            //else
            //    angle = -Range_Rad + LocalFrameCounter * speed;

            angle = (comingBack ? 1 : -1) * (Range_Rad - LocalFrameCounter * speed);

            return Matrix.Translation(0, -Length, 0) *
                Matrix.RotationZ(angle) *
                Matrix.Translation(0, Length, 0);
        }
    }
}
