using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetDYNA : ObjectAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        public AssetDYNA(Section_AHDR AHDR) : base(AHDR)
        {
            SetDynaSpecific(false);
        }

        public override bool HasReference(uint assetID)
        {
            if (_dynaSpecific.HasReference(assetID))
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            _dynaSpecific.Verify(ref result);

            base.Verify(ref result);
        }

        public void SetDynaSpecific(bool reset)
        {
            List<byte> dataBefore = Data.Take(0x10).ToList();
            List<byte> dataMiddle = Data.Skip(0x10).Take(EventStartOffset).ToList();
            List<byte> dataAfter = Data.Skip(EventStartOffset).ToList();

            if (Functions.currentGame == Game.BFBB)
                switch (Type_BFBB)
                {
                    case DynaType_BFBB.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen() : new DynaBoulderGen(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop() : new DynaBungeeDrop(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook() : new DynaBungeeHook(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop() : new DynaBusStop(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak() : new DynaCamTweak(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough() : new DynaFlythrough(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__NPCSettings:
                        _dynaSpecific = reset ? new DynaNPCSettings() : new DynaNPCSettings(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox() : new DynaTalkBox(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox() : new DynaTaskBox(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__Taxi:
                        _dynaSpecific = reset ? new DynaTaxi() : new DynaTaxi(dataMiddle);
                        break;
                    case DynaType_BFBB.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_BFBB(Version) : new DynaTeleport_BFBB(dataMiddle, Version);
                        break;
                    case DynaType_BFBB.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox() : new DynaTextBox(dataMiddle);
                        break;
                    case DynaType_BFBB.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3() : new DynaHudMeterFontV3(dataMiddle);
                        else
                            _dynaSpecific = reset ? new DynaBase() : new DynaBase(dataMiddle);
                        break;
                    case DynaType_BFBB.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit() : new DynaHudMeterUnit(dataMiddle);
                        break;
                    case DynaType_BFBB.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel() : new DynaHudModel(dataMiddle);
                        break;
                    case DynaType_BFBB.hud__text:
                        _dynaSpecific = reset ? new DynaHudText() : new DynaHudText(dataMiddle);
                        break;
                    case DynaType_BFBB.pointer:
                        _dynaSpecific = reset ? new DynaPointer() : new DynaPointer(dataMiddle);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase() : new DynaBase(dataMiddle);
                        break;
                }
            else if (Functions.currentGame == Game.Incredibles)
                switch (Type_TSSM)
                {
                    case DynaType_TSSM.pointer:
                        _dynaSpecific = reset ? new DynaPointer() : new DynaPointer(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox() : new DynaTalkBox(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox() : new DynaTextBox(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox() : new DynaTaskBox(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop() : new DynaBusStop(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough() : new DynaFlythrough(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen() : new DynaBoulderGen(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_MovieGame() : new DynaTeleport_MovieGame(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop() : new DynaBungeeDrop(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook() : new DynaBungeeHook(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak() : new DynaCamTweak(dataMiddle);
                        break;
                    case DynaType_TSSM.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel() : new DynaHudModel(dataMiddle);
                        break;
                    case DynaType_TSSM.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3() : new DynaHudMeterFontV3(dataMiddle);
                        else
                            _dynaSpecific = reset ? new DynaBase() : new DynaBase(dataMiddle);
                        break;
                    case DynaType_TSSM.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit() : new DynaHudMeterUnit(dataMiddle);
                        break;
                    case DynaType_TSSM.hud__text:
                        _dynaSpecific = reset ? new DynaHudText() : new DynaHudText(dataMiddle);
                        break;
                    case DynaType_TSSM.effect__ScreenFade:
                        _dynaSpecific = reset ? new DynaEffectScreenFade() : new DynaEffectScreenFade(dataMiddle);
                        break;
                    case DynaType_TSSM.effect__Lightning:
                        _dynaSpecific = reset ? new DynaEffectLightning() : new DynaEffectLightning(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__BucketOTron:
                        _dynaSpecific = reset ? new DynaEnemyBucketOTron() : new DynaEnemyBucketOTron(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__CastNCrew:
                        _dynaSpecific = reset ? new DynaEnemyCastNCrew() : new DynaEnemyCastNCrew(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Critter:
                        _dynaSpecific = reset ? new DynaEnemyCritter() : new DynaEnemyCritter(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Dennis:
                        _dynaSpecific = reset ? new DynaEnemyDennis() : new DynaEnemyDennis(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__FrogFish:
                        _dynaSpecific = reset ? new DynaEnemyFrogFish() : new DynaEnemyFrogFish(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Mindy:
                        _dynaSpecific = reset ? new DynaEnemyMindy() : new DynaEnemyMindy(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Neptune:
                        _dynaSpecific = reset ? new DynaEnemyNeptune() : new DynaEnemyNeptune(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Standard:
                        _dynaSpecific = reset ? new DynaEnemyStandard() : new DynaEnemyStandard(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__SupplyCrate:
                        _dynaSpecific = reset ? new DynaSupplyCrate() : new DynaSupplyCrate(dataMiddle);
                        break;
                    case DynaType_TSSM.Enemy__SB__Turret:
                        _dynaSpecific = reset ? new DynaEnemyTurret() : new DynaEnemyTurret(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__Ring:
                        _dynaSpecific = reset ? new DynaRing() : new DynaRing(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__RingControl:
                        _dynaSpecific = reset ? new DynaRingControl() : new DynaRingControl(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__Vent:
                        _dynaSpecific = reset ? new DynaVent() : new DynaVent(dataMiddle);
                        break;
                    case DynaType_TSSM.game_object__VentType:
                        _dynaSpecific = reset ? new DynaVentType() : new DynaVentType(dataMiddle);
                        break;
                    case DynaType_TSSM.SceneProperties:
                        _dynaSpecific = reset ? new DynaSceneProperties() : new DynaSceneProperties(dataMiddle);
                        break;
                    case DynaType_TSSM.JSPExtraData:
                        _dynaSpecific = reset ? new DynaJSPExtraData() : new DynaJSPExtraData(dataMiddle);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase() : new DynaBase(dataMiddle);
                        break;
                }

            dataBefore.AddRange(_dynaSpecific.ToByteArray());
            dataBefore.AddRange(dataAfter);

            Data = dataBefore.ToArray();
            
            _dynaSpecific.dynaSpecificPropertyChanged += OnDynaSpecificPropertyChange;

            if (IsRenderableClickable)
            {
                CreateTransformMatrix();
                if (!renderableAssetSetCommon.Contains(this))
                    renderableAssetSetCommon.Add(this);
            }
            else if (renderableAssetSetCommon.Contains(this))
                renderableAssetSetCommon.Remove(this);
        }

        public void OnDynaSpecificPropertyChange(DynaBase value)
        {
            List<byte> dataBefore = Data.Take(0x10).ToList();
            List<byte> dataAfter = Data.Skip(EventStartOffset).ToList();

            dataBefore.AddRange(value.ToByteArray());
            dataBefore.AddRange(dataAfter);

            _dynaSpecific = value;
            Data = dataBefore.ToArray();
        }

        [Category("Dynamic")]
        public DynaType_BFBB Type_BFBB
        {
            get => (DynaType_BFBB)ReadUInt(0x8);
            set
            {
                Write(0x8, (uint)value);
                SetDynaSpecific(true);
            }
        }

        [Category("Dynamic")]
        public DynaType_TSSM Type_TSSM
        {
            get => (DynaType_TSSM)ReadUInt(0x8);
            set
            {
                Write(0x8, (uint)value);
                SetDynaSpecific(true);
            }
        }

        [Category("Dynamic")]
        public short Version
        {
            get => ReadShort(0xC);
            set => Write(0xC, value);
        }

        [Category("Dynamic")]
        public short Handle
        {
            get => ReadShort(0xE);
            set => Write(0xE, value);
        }

        private DynaBase _dynaSpecific;
        [Browsable(false)]
        public DynaBase DynaBase
        {
            get => _dynaSpecific;
            set
            {
                List<byte> dataBefore = Data.Take(0x10).ToList();
                List<byte> dataAfter = Data.Skip(EventStartOffset).ToList();

                dataBefore.AddRange(value.ToByteArray());
                dataBefore.AddRange(dataAfter);

                _dynaSpecific = value;

                Data = dataBefore.ToArray();
            }
        }

        public static bool dontRender = false;

        [Browsable(false)]
        public bool IsRenderableClickable { get => _dynaSpecific.IsRenderableClickable; }

        [Browsable(false)]
        public float PositionX { get => _dynaSpecific.PositionX; set => _dynaSpecific.PositionX = value; }
        [Browsable(false)]
        public float PositionY { get => _dynaSpecific.PositionY; set => _dynaSpecific.PositionY = value; }
        [Browsable(false)]
        public float PositionZ { get => _dynaSpecific.PositionZ; set => _dynaSpecific.PositionZ = value; }
        [Browsable(false)]
        public float Yaw { get => _dynaSpecific.Yaw; set => _dynaSpecific.Yaw = value; }
        [Browsable(false)]
        public float Pitch { get => _dynaSpecific.Pitch; set => _dynaSpecific.Pitch = value; }
        [Browsable(false)]
        public float Roll { get => _dynaSpecific.Roll; set => _dynaSpecific.Roll = value; }
        [Browsable(false)]
        public float ScaleX { get => _dynaSpecific.ScaleX; set => _dynaSpecific.ScaleX = value; }
        [Browsable(false)]
        public float ScaleY { get => _dynaSpecific.ScaleY; set => _dynaSpecific.ScaleY = value; }
        [Browsable(false)]
        public float ScaleZ { get => _dynaSpecific.ScaleZ; set => _dynaSpecific.ScaleZ = value; }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            return _dynaSpecific.IntersectsWith(ray);
        }

        public BoundingSphere GetGizmoCenter()
        {
            return _dynaSpecific.GetGizmoCenter();
        }

        public void CreateTransformMatrix()
        {
            _dynaSpecific.CreateTransformMatrix();
        }

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible)
                return;

            _dynaSpecific.Draw(renderer, isSelected);
        }

        public BoundingBox GetBoundingBox()
        {
            return _dynaSpecific.GetBoundingBox();
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return _dynaSpecific.GetDistance(cameraPosition);
        }

        public BoundingSphere GetObjectCenter()
        {
            return _dynaSpecific.GetObjectCenter();
        }
    }
}