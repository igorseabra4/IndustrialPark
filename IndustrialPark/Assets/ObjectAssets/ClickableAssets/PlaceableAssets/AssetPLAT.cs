using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLAT : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => Functions.currentGame == Game.Incredibles ? 0xC8 : 0xC0;

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (ANIM_AssetID_1 == assetID)
                return true;
            if (ANIM_AssetID_2 == assetID)
                return true;
            if (MVPT_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(ANIM_AssetID_1, ref result);
            Verify(ANIM_AssetID_2, ref result);
            if (PlatformType == PlatType.MovePoint)
                Verify(MVPT_AssetID, ref result);
        }

        private enum CurrentMovementAction
        {
            StartWait,
            Going,
            EndWait,
            GoingBack
        }

        private float localFrameCounter = -1;
        private int amountOfMovementsPerformed = 0;

        private CurrentMovementAction currentMovementAction = CurrentMovementAction.StartWait;

        private float StartWaitRange => 60 * (Movement_StartPointWait);
        private float GoingRange => StartWaitRange + 60 * Math.Max(MovementMode == 0 ? 0 : MovementRotation_Time, MovementMode != 1 ? MovementTranslation_Time : 0);
        private float EndWaitRange => GoingRange + 60 * Movement_EndPointWait;
        private float GoingBackRange => EndWaitRange + 60 * Math.Max(MovementMode == 0 ? 0 : MovementRotation_Time, MovementMode != 1 ? MovementTranslation_Time : 0);

        public void Reset()
        {
            currentMovementAction = CurrentMovementAction.StartWait;
            localFrameCounter = -1;
            amountOfMovementsPerformed = 0;
        }

        public Matrix PlatLocalTranslation()
        {
            Matrix localWorld = Matrix.Identity;

            if (MovementMode != 1)
            {
                float translationMultiplier = 0;

                switch (MovementLoopType)
                {
                    case 0:
                        switch (currentMovementAction)
                        {
                            case CurrentMovementAction.StartWait:
                                translationMultiplier = amountOfMovementsPerformed;
                                if (localFrameCounter >= StartWaitRange)
                                    currentMovementAction = CurrentMovementAction.Going;
                                break;

                            case CurrentMovementAction.Going:
                                translationMultiplier = amountOfMovementsPerformed + Math.Min(1f, (localFrameCounter - StartWaitRange) / (60 * MovementTranslation_Time));

                                if (localFrameCounter >= GoingRange)
                                {
                                    amountOfMovementsPerformed++;
                                    currentMovementAction = CurrentMovementAction.StartWait;
                                    localFrameCounter = 0;
                                }
                                break;
                        }
                        break;
                    case 1:
                    case 2:
                        switch (currentMovementAction)
                        {
                            case CurrentMovementAction.StartWait:
                                if (localFrameCounter >= StartWaitRange)
                                    currentMovementAction = CurrentMovementAction.Going;
                                break;

                            case CurrentMovementAction.Going:
                                translationMultiplier = Math.Min(1, (localFrameCounter - StartWaitRange) / (60 * MovementTranslation_Time));
                                if (localFrameCounter >= GoingRange)
                                    currentMovementAction = CurrentMovementAction.EndWait;
                                break;

                            case CurrentMovementAction.EndWait:
                                translationMultiplier = 1;
                                if (localFrameCounter >= EndWaitRange)
                                    currentMovementAction = CurrentMovementAction.GoingBack;
                                break;

                            case CurrentMovementAction.GoingBack:
                                translationMultiplier = 1 - Math.Min(1, (localFrameCounter - EndWaitRange) / (60 * MovementTranslation_Time));
                                if (localFrameCounter >= GoingBackRange)
                                {
                                    currentMovementAction = CurrentMovementAction.StartWait;
                                    localFrameCounter = 0;
                                }
                                break;
                        }
                        break;
                }

                switch (MovementTranslation_Direction)
                {
                    case 0:
                        localWorld *= Matrix.Translation(translationMultiplier * MovementTranslation_Distance, 0, 0);
                        break;
                    case 1:
                        localWorld *= Matrix.Translation(0, translationMultiplier * MovementTranslation_Distance, 0);
                        break;
                    case 2:
                        localWorld *= Matrix.Translation(0, 0, translationMultiplier * MovementTranslation_Distance);
                        break;
                }
            }

            return localWorld;
        }

        public override Matrix PlatLocalRotation()
        {
            Matrix localWorld = Matrix.Identity;

            if (MovementMode >= 1)
            {
                float rotationMultiplier = 0;

                switch (MovementLoopType)
                {
                    case 0:
                        switch (currentMovementAction)
                        {
                            case CurrentMovementAction.StartWait:
                                rotationMultiplier = amountOfMovementsPerformed;
                                if (localFrameCounter >= StartWaitRange)
                                    currentMovementAction = CurrentMovementAction.Going;
                                break;

                            case CurrentMovementAction.Going:
                                rotationMultiplier = amountOfMovementsPerformed + Math.Min(1f, (localFrameCounter - StartWaitRange) / (60 * MovementRotation_Time));

                                if (localFrameCounter >= GoingRange)
                                {
                                    amountOfMovementsPerformed++;
                                    currentMovementAction = CurrentMovementAction.StartWait;
                                    localFrameCounter = 0;
                                }
                                break;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                        switch (currentMovementAction)
                        {
                            case CurrentMovementAction.StartWait:
                                if (localFrameCounter >= StartWaitRange)
                                    currentMovementAction = CurrentMovementAction.Going;
                                break;

                            case CurrentMovementAction.Going:
                                rotationMultiplier = Math.Min(1, (localFrameCounter - StartWaitRange) / (60 * MovementRotation_Time));
                                if (localFrameCounter >= GoingRange)
                                    currentMovementAction = CurrentMovementAction.EndWait;
                                break;

                            case CurrentMovementAction.EndWait:
                                rotationMultiplier = 1;
                                if (localFrameCounter >= EndWaitRange)
                                    currentMovementAction = CurrentMovementAction.GoingBack;
                                break;

                            case CurrentMovementAction.GoingBack:
                                rotationMultiplier = 1 - Math.Min(1, (localFrameCounter - EndWaitRange) / (60 * MovementRotation_Time));
                                if (localFrameCounter >= GoingBackRange)
                                {
                                    currentMovementAction = CurrentMovementAction.StartWait;
                                    localFrameCounter = 0;
                                }
                                break;
                        }
                        break;
                }

                switch (MovementRotation_Axis)
                {
                    case 0:
                        localWorld = Matrix.RotationX(rotationMultiplier * MathUtil.DegreesToRadians(MovementRotation_Degrees));
                        break;
                    case 1:
                        localWorld = Matrix.RotationY(rotationMultiplier * MathUtil.DegreesToRadians(MovementRotation_Degrees));
                        break;
                    case 2:
                        localWorld = Matrix.RotationZ(rotationMultiplier * MathUtil.DegreesToRadians(MovementRotation_Degrees));
                        break;
                }
            }

            return localWorld;
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                PlaceableAsset driver = FindDrivenByAsset(out bool found, out bool useRotation);

                if (found)
                {
                    return PlatLocalRotation() * PlatLocalTranslation()
                        * Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - driver._position)
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }

                return PlatLocalRotation() * PlatLocalTranslation()
                    * Matrix.Scaling(_scale)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(_position);
            }

            return world;
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
                localFrameCounter++;

            if (DontRender || isInvisible)
                return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Category("Platform")]
        public PlatType PlatformType
        {
            get => (PlatType)ReadByte(0x09 + Offset);
            set => Write(0x09 + Offset, (byte)value);
        }

        [Category("Platform")]
        public PlatTypeSpecific PlatformSubtype
        {
            get => (PlatTypeSpecific)ReadByte(0x54 + Offset);
            set => Write(0x54 + Offset, (byte)value);
        }

        [Category("Platform")]
        public byte UnknownByte_55
        {
            get => ReadByte(0x55 + Offset);
            set => Write(0x55 + Offset, value);
        }

        [Category("Platform")]
        public short CollisionType
        {
            get => ReadShort(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float Float58
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Browsable(false)]
        public int Int58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Browsable(false)]
        public float Float5C
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Browsable(false)]
        public int Int5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Browsable(false)]
        public float Float60
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Browsable(false)]
        public int Int60
        {
            get => ReadInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Browsable(false)]
        public float Float64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Browsable(false)]
        public int Int64
        {
            get => ReadInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Platform")]
        public AssetID ANIM_AssetID_1
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Platform")]
        public AssetID ANIM_AssetID_2
        {
            get => ReadUInt(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float LaunchDirectionX
        {
            get => ReadFloat(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float LaunchDirectionY
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float LaunchDirectionZ
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Platform")]
        public int MovementLockMode
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_84
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_88
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_8C
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_90
        {
            get => ReadByte(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_91
        {
            get => ReadByte(0x91 + Offset);
            set => Write(0x91 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_92
        {
            get => ReadByte(0x92 + Offset);
            set => Write(0x92 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_93
        {
            get => ReadByte(0x93 + Offset);
            set => Write(0x93 + Offset, value);
        }

        [Category("Platform")]
        public byte MovementMode
        {
            get => ReadByte(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("Platform")]
        public byte MovementLoopType
        {
            get => ReadByte(0x95 + Offset);
            set { Write(0x95 + Offset, value); Reset(); }
        }

        [Category("Platform")]
        public byte MovementTranslation_Direction
        {
            get => ReadByte(0x96 + Offset);
            set { Write(0x96 + Offset, value); Reset(); }
        }

        [Category("Platform")]
        public byte MovementRotation_Axis
        {
            get => ReadByte(0x97 + Offset);
            set { Write(0x97 + Offset, value); Reset(); }
        }

        private int OffsetForTSSM => Functions.currentGame == Game.Incredibles ? 4 : 0;

        [Category("Platform")]
        public AssetID MVPT_AssetID
        {
            get { return ReadUInt(0x98 + Offset); }
            set { Write(0x98 + Offset, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementTranslation_Distance
        {
            get => ReadFloat(0x98 + Offset + OffsetForTSSM);
            set { Write(0x98 + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementTranslation_Time
        {
            get => ReadFloat(0x9C + Offset + OffsetForTSSM);
            set { Write(0x9C + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementTranslation_EaseStart
        {
            get => ReadFloat(0xA0 + Offset + OffsetForTSSM);
            set => Write(0xA0 + Offset + OffsetForTSSM, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementTranslation_EaseEnd
        {
            get => ReadFloat(0xA4 + Offset + OffsetForTSSM);
            set => Write(0xA4 + Offset + OffsetForTSSM, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementRotation_Degrees
        {
            get => ReadFloat(0xA8 + Offset + OffsetForTSSM);
            set { Write(0xA8 + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementRotation_Time
        {
            get => ReadFloat(0xAC + Offset + OffsetForTSSM);
            set { Write(0xAC + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementRotation_EaseStart
        {
            get => ReadFloat(0xB0 + Offset + OffsetForTSSM);
            set => Write(0xB0 + Offset + OffsetForTSSM, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementRotation_EaseEnd
        {
            get => ReadFloat(0xB4 + Offset + OffsetForTSSM);
            set => Write(0xB4 + Offset + OffsetForTSSM, value);
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float Movement_StartPointWait
        {
            get => ReadFloat(0xB8 + Offset + OffsetForTSSM);
            set { Write(0xB8 + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float Movement_EndPointWait
        {
            get => ReadFloat(0xBC + Offset + OffsetForTSSM);
            set { Write(0xBC + Offset + OffsetForTSSM, value); Reset(); }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float TSSM_Unknown_C0
        {
            get
            {
                if (Functions.currentGame == Game.Incredibles)
                    return ReadFloat(0xC0 + Offset + OffsetForTSSM);
                return 0;
            }
            set
            {
                if (Functions.currentGame == Game.Incredibles)
                {
                    Write(0xC0 + Offset + OffsetForTSSM, value);
                }
            }
        }

        [Category("Platform"), TypeConverter(typeof(FloatTypeConverter))]
        public float TSSM_Unknown_C4
        {
            get
            {
                if (Functions.currentGame == Game.Incredibles)
                    return ReadFloat(0xC4 + Offset + OffsetForTSSM);
                return 0;
            }
            set
            {
                if (Functions.currentGame == Game.Incredibles)
                {
                    Write(0xC4 + Offset + OffsetForTSSM, value);
                }
            }
        }
    }
}