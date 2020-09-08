using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

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

    public class Motion : AssetSpecific_Generic
    {
        public Motion(AssetWithMotion asset, MotionType type) : base(asset, asset.MotionStart)
        {
            Settings = asset.ShortFlagsDescriptor(2 + specificStart,
                 "Face movement direction", "Unknown", "Don't start moving");
            Type = type;
        }

        [Browsable(false)]
        public MotionType Type
        {
            get => (MotionType)ReadByte(0);
            set => Write(0, (byte)value);
        }

        public byte UseBanking
        {
            get => ReadByte(1);
            set => Write(1, value);
        }

        public DynamicTypeDescriptor Settings { get; set; }

        [Browsable(false)]
        public ushort Flags
        {
            get => ReadByte(2);
            set => Write(2, value);
        }

        protected int LocalFrameCounter = -1;

        public virtual void Reset()
        {
            LocalFrameCounter = -1;
        }

        public void Increment()
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
    }

    public class Motion_ExtendRetract : Motion
    {
        public Motion_ExtendRetract(AssetWithMotion asset) : base(asset, MotionType.ExtendRetract) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionX
        {
            get => ReadFloat(4);
            set => Write(4, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionY
        {
            get => ReadFloat(8);
            set => Write(8, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionZ
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionX
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionY
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionZ
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendTime
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendWaitTime
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractTime
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractWaitTime
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
    }

    public class Motion_Orbit : Motion
    {
        public Motion_Orbit(AssetWithMotion asset) : base(asset, MotionType.Orbit) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float CenterX
        {
            get => ReadFloat(0x4);
            set => Write(0x4, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CenterY
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CenterZ
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Width
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Height
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Period
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
    }

    public class Motion_Spline : Motion
    {
        public Motion_Spline(AssetWithMotion asset) : base(asset, MotionType.Spline) { }

        public int Unknown
        {
            get => ReadInt(0x4);
            set => Write(0x4, value);
        }
    }

    public class Motion_MovePoint : Motion
    {
        public Motion_MovePoint(AssetWithMotion asset, Vector3 initialPosition) : base(asset, MotionType.MovePoint)
        {
            this.initialPosition = initialPosition;

            MovePoint_Flags = asset.IntFlagsDescriptor(4 + specificStart);
        }

        public DynamicTypeDescriptor MovePoint_Flags { get; set; }

        public AssetID MVPT_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Speed
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }
        
        public override bool HasReference(uint assetID)
        {
            return MVPT_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            if (MVPT_AssetID == 0)
                result.Add("MovePoint PLAT with MVPT_AssetID set to 0");
            Asset.Verify(MVPT_AssetID, ref result);
        }

        private AssetMVPT currentMVPT;
        private Vector3 initialPosition;
        private Vector3 oldPosition;
        private Vector3 targetPosition;

        public override void Reset()
        {
            currentMVPT = FindMVPT(MVPT_AssetID);
            if (currentMVPT != null)
            {
                targetPosition = new Vector3(currentMVPT.PositionX, currentMVPT.PositionY, currentMVPT.PositionZ);
                oldPosition = targetPosition;
            }
            else
                oldPosition = initialPosition;

            base.Reset();
        }

        private AssetMVPT FindMVPT(uint assetID)
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(assetID))
                {
                    Asset asset = ae.archive.GetFromAssetID(assetID);
                    if (asset is AssetMVPT MVPT)
                        return MVPT;
                }
            return null;
        }

        public override Matrix PlatLocalTranslation()
        {
            if (currentMVPT != null)
            {
                Vector3 newPosition = Vector3.Lerp(oldPosition, targetPosition, Math.Min(1f, LocalFrameCounter / (Vector3.Distance(oldPosition, targetPosition) / (Math.Abs(Speed) / 60f))));

                if (newPosition == targetPosition)
                {
                    LocalFrameCounter = 0;
                    oldPosition = targetPosition;

                    if (currentMVPT.NextMVPTs.Length > 0)
                    {
                        currentMVPT = FindMVPT(currentMVPT.NextMVPTs[0]);
                        targetPosition = new Vector3(currentMVPT.PositionX, currentMVPT.PositionY, currentMVPT.PositionZ);
                    }
                    else
                        Reset();
                }

                return Matrix.Translation(newPosition);
            }

            return Matrix.Translation(oldPosition);
        }
    }

    public class Motion_Mechanism_TSSM : Motion_Mechanism
    {
        public Motion_Mechanism_TSSM(AssetWithMotion asset) : base(asset) { }

        public byte Unknown1
        {
            get => ReadByte(8);
            set => Write(8, value);
        }
        public byte Unknown2
        {
            get => ReadByte(9);
            set => Write(9, value);
        }
        public byte Unknown3
        {
            get => ReadByte(10);
            set => Write(10, value);
        }
        public byte Unknown4
        {
            get => ReadByte(11);
            set => Write(11, value);
        }
        public float UnknownFloat1
        {
            get => ReadFloat(0x30 + MechanismOffset);
            set => Write(0x30 + MechanismOffset, value);
        }
        public float UnknownFloat2
        {
            get => ReadFloat(0x34 + MechanismOffset);
            set => Write(0x34 + MechanismOffset, value);
        }
    }

    public class Motion_Mechanism : Motion
    {
        public Motion_Mechanism(AssetWithMotion asset) : base(asset, MotionType.Mechanism)
        {
            MovementLoopMode = asset.ByteFlagsDescriptor(5 + specificStart,
                "Return to start after moving", "Don't loop");
        }

        public enum EMovementType : byte
        {
            Slide = 0,
            Rotate = 1,
            SlideAndRotate = 2,
            SlideThenRotate = 3,
            RotateThenSlide = 4
        }

        public enum Axis : byte
        {
            X = 0,
            Y = 1,
            Z = 2
        }

        public EMovementType MovementType
        {
            get => (EMovementType)ReadByte(4);
            set => Write(4, (byte)value);
        }
        public DynamicTypeDescriptor MovementLoopMode { get; set; }
        [Browsable(false)]
        public byte MovementLoopModeByte
        {
            get => ReadByte(5);
            set => Write(5, value);
        }
        public Axis SlideAxis
        {
            get => (Axis)ReadByte(6);
            set => Write(6, (byte)value);
        }
        public Axis RotateAxis
        {
            get => (Axis)ReadByte(7);
            set => Write(7, (byte)value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SlideDistance
        {
            get => ReadFloat(0x8 + MechanismOffset);
            set => Write(0x8 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SlideTime
        {
            get => ReadFloat(0xC + MechanismOffset);
            set => Write(0xC + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SlideAccelTime
        {
            get => ReadFloat(0x10 + MechanismOffset);
            set => Write(0x10 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SlideDecelTime
        {
            get => ReadFloat(0x14 + MechanismOffset);
            set => Write(0x14 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotateDistance
        {
            get => ReadFloat(0x18 + MechanismOffset);
            set => Write(0x18 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotateTime
        {
            get => ReadFloat(0x1C + MechanismOffset);
            set => Write(0x1C + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotateAccelTime
        {
            get => ReadFloat(0x20 + MechanismOffset);
            set => Write(0x20 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotateDecelTime
        {
            get => ReadFloat(0x24 + MechanismOffset);
            set => Write(0x24 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RetractDelay
        {
            get => ReadFloat(0x28 + MechanismOffset);
            set => Write(0x28 + MechanismOffset, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PostRetractDelay
        {
            get => ReadFloat(0x2C + MechanismOffset);
            set => Write(0x2C + MechanismOffset, value);
        }

        protected int MechanismOffset => asset.game == Game.Incredibles ? 4 : 0;

        public enum CurrentMovementAction
        {
            StartWait,
            Going,
            EndWait,
            GoingBack
        }

        private CurrentMovementAction currentMovementAction;
        private int amountOfMovementsPerformed = 0;

        public override void Reset()
        {
            currentMovementAction = CurrentMovementAction.StartWait;
            amountOfMovementsPerformed = 0;

            base.Reset();
        }

        private float StartWaitRange => 60 * PostRetractDelay;
        private float GoingRange => StartWaitRange + 60 * Math.Max(MovementType != 0 ? RotateTime : 0, MovementType != EMovementType.Rotate ? SlideTime : 0);
        private float EndWaitRange => GoingRange + 60 * RetractDelay;
        private float GoingBackRange => EndWaitRange + 60 * Math.Max(MovementType != 0 ? RotateTime : 0, MovementType != EMovementType.Rotate ? SlideTime : 0);

        public override Matrix PlatLocalTranslation()
        {
            Matrix localWorld = Matrix.Identity;

            if (MovementType != EMovementType.Rotate)
            {
                float translationMultiplier = 0;

                if ((MovementLoopModeByte & 1) == 0)
                    switch (currentMovementAction)
                    {
                        case CurrentMovementAction.StartWait:
                            translationMultiplier = amountOfMovementsPerformed;
                            if (LocalFrameCounter >= StartWaitRange)
                                currentMovementAction = CurrentMovementAction.Going;
                            break;

                        case CurrentMovementAction.Going:
                            translationMultiplier = amountOfMovementsPerformed + Math.Min(1f, (LocalFrameCounter - StartWaitRange) / (60 * SlideTime));

                            if (LocalFrameCounter >= GoingRange)
                            {
                                if ((MovementLoopModeByte & 2) == 0)
                                    amountOfMovementsPerformed++;
                                else
                                    amountOfMovementsPerformed = 0;
                                currentMovementAction = CurrentMovementAction.StartWait;
                                LocalFrameCounter = 0;
                            }
                            break;
                        default:
                            Reset();
                            break;
                    }
                else
                    switch (currentMovementAction)
                    {
                        case CurrentMovementAction.StartWait:
                            if (LocalFrameCounter >= StartWaitRange)
                                currentMovementAction = CurrentMovementAction.Going;
                            break;

                        case CurrentMovementAction.Going:
                            translationMultiplier = Math.Min(1, (LocalFrameCounter - StartWaitRange) / (60 * SlideTime));
                            if (LocalFrameCounter >= GoingRange)
                                currentMovementAction = CurrentMovementAction.EndWait;
                            break;

                        case CurrentMovementAction.EndWait:
                            translationMultiplier = 1;
                            if (LocalFrameCounter >= EndWaitRange)
                                currentMovementAction = CurrentMovementAction.GoingBack;
                            break;

                        case CurrentMovementAction.GoingBack:
                            translationMultiplier = 1 - Math.Min(1, (LocalFrameCounter - EndWaitRange) / (60 * SlideTime));
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

                switch (SlideAxis)
                {
                    case Axis.X:
                        localWorld *= Matrix.Translation(translationMultiplier * SlideDistance, 0, 0);
                        break;
                    case Axis.Y:
                        localWorld *= Matrix.Translation(0, translationMultiplier * SlideDistance, 0);
                        break;
                    case Axis.Z:
                        localWorld *= Matrix.Translation(0, 0, translationMultiplier * SlideDistance);
                        break;
                }
            }

            return localWorld;
        }

        public override Matrix PlatLocalRotation()
        {
            Matrix localWorld = Matrix.Identity;

            if (MovementType > 0)
            {
                float rotationMultiplier = 0;

                if ((MovementLoopModeByte & 1) == 0)
                    switch (currentMovementAction)
                    {
                        case CurrentMovementAction.StartWait:
                            rotationMultiplier = amountOfMovementsPerformed;
                            if (LocalFrameCounter >= StartWaitRange)
                                currentMovementAction = CurrentMovementAction.Going;
                            break;

                        case CurrentMovementAction.Going:
                            rotationMultiplier = amountOfMovementsPerformed + Math.Min(1f, (LocalFrameCounter - StartWaitRange) / (60 * RotateTime));

                            if (LocalFrameCounter >= GoingRange)
                            {
                                if ((MovementLoopModeByte & 2) == 0)
                                    amountOfMovementsPerformed++;
                                else
                                    amountOfMovementsPerformed = 0;
                                currentMovementAction = CurrentMovementAction.StartWait;
                                LocalFrameCounter = 0;
                            }
                            break;
                        default:
                            Reset();
                            break;
                    }
                else
                    switch (currentMovementAction)
                    {
                        case CurrentMovementAction.StartWait:
                            if (LocalFrameCounter >= StartWaitRange)
                                currentMovementAction = CurrentMovementAction.Going;
                            break;

                        case CurrentMovementAction.Going:
                            rotationMultiplier = Math.Min(1, (LocalFrameCounter - StartWaitRange) / (60 * RotateTime));
                            if (LocalFrameCounter >= GoingRange)
                                currentMovementAction = CurrentMovementAction.EndWait;
                            break;

                        case CurrentMovementAction.EndWait:
                            rotationMultiplier = 1;
                            if (LocalFrameCounter >= EndWaitRange)
                                currentMovementAction = CurrentMovementAction.GoingBack;
                            break;

                        case CurrentMovementAction.GoingBack:
                            rotationMultiplier = 1 - Math.Min(1, (LocalFrameCounter - EndWaitRange) / (60 * RotateTime));
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

                switch (RotateAxis)
                {
                    case Axis.X:
                        localWorld = Matrix.RotationX(rotationMultiplier * MathUtil.DegreesToRadians(RotateDistance));
                        break;
                    case Axis.Y:
                        localWorld = Matrix.RotationY(rotationMultiplier * MathUtil.DegreesToRadians(RotateDistance));
                        break;
                    case Axis.Z:
                        localWorld = Matrix.RotationZ(rotationMultiplier * MathUtil.DegreesToRadians(RotateDistance));
                        break;
                }
            }

            return localWorld;
        }
    }

    public class Motion_Pendulum : Motion
    {
        public Motion_Pendulum(AssetWithMotion asset) : base(asset, MotionType.Pendulum) { }
        
        public byte PendulumFlags
        {
            get => ReadByte(4);
            set => Write(4, value);
        }
        
        public byte Plane
        {
            get => ReadByte(5);
            set => Write(5, value);
        }
        
        public byte Padding1
        {
            get => ReadByte(6);
            set => Write(6, value);
        }
        
        public byte Padding2
        {
            get => ReadByte(7);
            set => Write(7, value);
        }
        
        public float Length
        {
            get => ReadFloat(8);
            set => Write(8, value);
        }
        
        public float Range
        {
            get => ReadFloat(12);
            set => Write(12, value);
        }
        
        public float Period
        {
            get => ReadFloat(16);
            set => Write(16, value);
        }
        
        public float Phase
        {
            get => ReadFloat(20);
            set => Write(20, value);
        }
    }
}
