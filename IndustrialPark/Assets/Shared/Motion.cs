using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

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

    public class Motion
    {
        public static int Size => HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles ? 0x3C : 0x30;

        [Category("Motion"), ReadOnly(true)]
        public MotionType Type { get; set; }
        [Category("Motion")]
        public byte UseBanking { get; set; }
        [Category("Motion")]
        [Description("0 = None\n1 = Unknown\n2 = Unknown\n4=Don't start moving\nAdd numbers to enable multiple")]
        public short Flags { get; set; }

        public Motion() { }

        public Motion(byte[] data)
        {
            Type = (MotionType)(data[0]);
            UseBanking = data[1];
            Flags = Switch(BitConverter.ToInt16(data, 2));
        }
        
        public virtual byte[] ToByteArray()
        {
            List<byte> data = new List<byte>
            {
                (byte)Type,
                UseBanking
            };
            data.AddRange(BitConverter.GetBytes(Switch(Flags)));
            
            return data.ToArray();
        }

        public virtual bool HasReference(uint assetID)
        {
            return false;
        }

        public virtual void Verify(ref List<string> result)
        {

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
        public Motion_ExtendRetract()
        {
            Type = MotionType.ExtendRetract;
        }

        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionX { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionY { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractPositionZ { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionX { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionY { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendDeltaPositionZ { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendTime { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float ExtendWaitTime { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractTime { get; set; }
        [Category("Motion: ExtendRetract"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractWaitTime { get; set; }

        public Motion_ExtendRetract(byte[] data) : base(data)
        {
            RetractPositionX = Switch(BitConverter.ToSingle(data, 4));
            RetractPositionY = Switch(BitConverter.ToSingle(data, 8));
            RetractPositionZ = Switch(BitConverter.ToSingle(data, 12));
            ExtendDeltaPositionX = Switch(BitConverter.ToSingle(data, 16));
            ExtendDeltaPositionY = Switch(BitConverter.ToSingle(data, 20));
            ExtendDeltaPositionZ = Switch(BitConverter.ToSingle(data, 24));
            ExtendTime = Switch(BitConverter.ToSingle(data, 28));
            ExtendWaitTime = Switch(BitConverter.ToSingle(data, 32));
            RetractTime = Switch(BitConverter.ToSingle(data, 36));
            RetractWaitTime = Switch(BitConverter.ToSingle(data, 40));
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.AddRange(BitConverter.GetBytes(Switch(RetractPositionX)));
            data.AddRange(BitConverter.GetBytes(Switch(RetractPositionY)));
            data.AddRange(BitConverter.GetBytes(Switch(RetractPositionZ)));
            data.AddRange(BitConverter.GetBytes(Switch(ExtendDeltaPositionX)));
            data.AddRange(BitConverter.GetBytes(Switch(ExtendDeltaPositionY)));
            data.AddRange(BitConverter.GetBytes(Switch(ExtendDeltaPositionZ)));
            data.AddRange(BitConverter.GetBytes(Switch(ExtendTime)));
            data.AddRange(BitConverter.GetBytes(Switch(ExtendWaitTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RetractTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RetractWaitTime)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class Motion_Orbit : Motion
    {
        public Motion_Orbit()
        {
            Type = MotionType.Orbit;
        }

        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float CenterX { get; set; }
        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float CenterY { get; set; }
        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float CenterZ { get; set; }
        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float Width { get; set; }
        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float Height { get; set; }
        [Category("Motion: Orbit"), TypeConverter(typeof(FloatTypeConverter))]
        public float Period { get; set; }

        public Motion_Orbit(byte[] data) : base(data)
        {
            CenterX = Switch(BitConverter.ToSingle(data, 4));
            CenterY = Switch(BitConverter.ToSingle(data, 8));
            CenterZ = Switch(BitConverter.ToSingle(data, 12));
            Width = Switch(BitConverter.ToSingle(data, 16));
            Height = Switch(BitConverter.ToSingle(data, 20));
            Period = Switch(BitConverter.ToSingle(data, 24));
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.AddRange(BitConverter.GetBytes(Switch(CenterX)));
            data.AddRange(BitConverter.GetBytes(Switch(CenterY)));
            data.AddRange(BitConverter.GetBytes(Switch(CenterZ)));
            data.AddRange(BitConverter.GetBytes(Switch(Width)));
            data.AddRange(BitConverter.GetBytes(Switch(Height)));
            data.AddRange(BitConverter.GetBytes(Switch(Period)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class Motion_Spline : Motion
    {
        public Motion_Spline()
        {
            Type = MotionType.Spline;
        }

        [Category("Motion: Spline")]
        public int Unknown { get; set; }

        public Motion_Spline(byte[] data) : base(data)
        {
            Unknown = Switch(BitConverter.ToInt32(data, 4));
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.AddRange(BitConverter.GetBytes(Switch(Unknown)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }

    public class Motion_MovePoint : Motion
    {
        public Motion_MovePoint()
        {
            Type = MotionType.MovePoint;
        }

        [Category("Motion: MovePoint")]
        public uint MovePoint_Flags { get; set; }
        [Category("Motion: MovePoint")]
        public AssetID MVPT_AssetID { get; set; }
        [Category("Motion: MovePoint"), TypeConverter(typeof(FloatTypeConverter))]
        public float Speed { get; set; }

        public Motion_MovePoint(byte[] data, Vector3 initialPosition) : base(data)
        {
            MovePoint_Flags = Switch(BitConverter.ToUInt32(data, 4));
            MVPT_AssetID = Switch(BitConverter.ToUInt32(data, 8));
            Speed = Switch(BitConverter.ToSingle(data, 12));

            this.initialPosition = initialPosition;
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.AddRange(BitConverter.GetBytes(Switch(MovePoint_Flags)));
            data.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));
            data.AddRange(BitConverter.GetBytes(Switch(Speed)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
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

    public class Motion_Mechanism : Motion
    {
        public Motion_Mechanism()
        {
            Type = MotionType.Mechanism;
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

        [Category("Motion: Mechanism")]
        public EMovementType MovementType { get; set; }
        [Category("Motion: Mechanism")]
        [Description("0 = None\n1 = Return to start after moving\n2 = Don't loop\nAdd to enable multiple")]
        public byte MovementLoopMode { get; set; }
        [Category("Motion: Mechanism")]
        public Axis SlideAxis { get; set; }
        [Category("Motion: Mechanism")]
        public Axis RotateAxis { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float SlideDistance { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float SlideTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float SlideAccelTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float SlideDecelTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float RotateDistance { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float RotateTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float RotateAccelTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float RotateDecelTime { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float RetractDelay { get; set; }
        [Category("Motion: Mechanism"), TypeConverter(typeof(FloatTypeConverter))]
        public float PostRetractDelay { get; set; }

        [Category("TSSM Only")]
        public byte Unknown1 { get; set; }
        [Category("TSSM Only")]
        public byte Unknown2 { get; set; }
        [Category("TSSM Only")]
        public byte Unknown3 { get; set; }
        [Category("TSSM Only")]
        public byte Unknown4 { get; set; }
        [Category("TSSM Only")]
        public float UnknownFloat1 { get; set; }
        [Category("TSSM Only")]
        public float UnknownFloat2 { get; set; }

        public Motion_Mechanism(byte[] data) : base(data)
        {
            MovementType = (EMovementType)data[4];
            MovementLoopMode = data[5];
            SlideAxis = (Axis)data[6];
            RotateAxis = (Axis)data[7];

            int offset = 0;
            if (HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles)
            {
                Unknown1 = data[8];
                Unknown2 = data[9];
                Unknown3 = data[10];
                Unknown4 = data[11];

                offset = 4;
            }

            SlideDistance = Switch(BitConverter.ToSingle(data, 8 + offset));
            SlideTime = Switch(BitConverter.ToSingle(data, 12 + offset));
            SlideAccelTime = Switch(BitConverter.ToSingle(data, 16 + offset));
            SlideDecelTime = Switch(BitConverter.ToSingle(data, 20 + offset));
            RotateDistance = Switch(BitConverter.ToSingle(data, 24 + offset));
            RotateTime = Switch(BitConverter.ToSingle(data, 28 + offset));
            RotateAccelTime = Switch(BitConverter.ToSingle(data, 32 + offset));
            RotateDecelTime = Switch(BitConverter.ToSingle(data, 36 + offset));
            RetractDelay = Switch(BitConverter.ToSingle(data, 40 + offset));
            PostRetractDelay = Switch(BitConverter.ToSingle(data, 44 + offset));

            if (HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles)
            {
                UnknownFloat1 = Switch(BitConverter.ToSingle(data, 52));
                UnknownFloat2 = Switch(BitConverter.ToSingle(data, 56));
            }
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.Add((byte)MovementType);
            data.Add(MovementLoopMode);
            data.Add((byte)SlideAxis);
            data.Add((byte)RotateAxis);

            if (HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles)
            {
                data.Add(Unknown1);
                data.Add(Unknown2);
                data.Add(Unknown3);
                data.Add(Unknown4);
            }

            data.AddRange(BitConverter.GetBytes(Switch(SlideDistance)));
            data.AddRange(BitConverter.GetBytes(Switch(SlideTime)));
            data.AddRange(BitConverter.GetBytes(Switch(SlideAccelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(SlideDecelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RotateDistance)));
            data.AddRange(BitConverter.GetBytes(Switch(RotateTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RotateAccelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RotateDecelTime)));
            data.AddRange(BitConverter.GetBytes(Switch(RetractDelay)));
            data.AddRange(BitConverter.GetBytes(Switch(PostRetractDelay)));

            if (HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles)
            {
                data.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
                data.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            }

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }

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
        private float GoingRange => StartWaitRange + 60 * Math.Max(MovementType != 0 ? RotateTime :0, MovementType != EMovementType.Rotate ? SlideTime : 0);
        private float EndWaitRange => GoingRange + 60 * RetractDelay;
        private float GoingBackRange => EndWaitRange + 60 * Math.Max(MovementType != 0 ? RotateTime : 0, MovementType != EMovementType.Rotate ? SlideTime : 0);

        public override Matrix PlatLocalTranslation()
        {
            Matrix localWorld = Matrix.Identity;

            if (MovementType != EMovementType.Rotate)
            {
                float translationMultiplier = 0;

                if ((MovementLoopMode & 1) == 0)
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
                                if ((MovementLoopMode & 2) == 0)
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

                if ((MovementLoopMode & 1) == 0)
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
                                if ((MovementLoopMode & 2) == 0)
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
        public Motion_Pendulum()
        {
            Type = MotionType.Pendulum;
        }

        [Category("Motion: Pendulum")]
        public byte PendulumFlags { get; set; }
        [Category("Motion: Pendulum")]
        public byte Plane { get; set; }
        [Category("Motion: Pendulum")]
        public float Length { get; set; }
        [Category("Motion: Pendulum")]
        public float Range { get; set; }
        [Category("Motion: Pendulum")]
        public float Period { get; set; }
        [Category("Motion: Pendulum")]
        public float Phase { get; set; }

        public Motion_Pendulum(byte[] data) : base(data)
        {
            PendulumFlags = data[4];
            Plane = data[5];
            Length = Switch(BitConverter.ToSingle(data, 8));
            Range = Switch(BitConverter.ToSingle(data, 12));
            Period = Switch(BitConverter.ToSingle(data, 16));
            Phase = Switch(BitConverter.ToSingle(data, 20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = base.ToByteArray().ToList();

            data.Add(PendulumFlags);
            data.Add(Plane);
            data.Add(0);
            data.Add(0);
            
            data.AddRange(BitConverter.GetBytes(Switch(Length)));
            data.AddRange(BitConverter.GetBytes(Switch(Range)));
            data.AddRange(BitConverter.GetBytes(Switch(Period)));
            data.AddRange(BitConverter.GetBytes(Switch(Phase)));

            while (data.Count < Size)
                data.Add(0);

            return data.ToArray();
        }
    }
}
