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
        public AssetDYNA(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            SetDynaSpecific(false);
        }

        public override bool HasReference(uint assetID) => _dynaSpecific.HasReference(assetID) || base.HasReference(assetID);

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

            if (currentGame == Game.BFBB)
                switch (Type_BFBB)
                {
                    case DynaType_BFBB.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen(currentPlatform) : new DynaBoulderGen(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop(currentPlatform) : new DynaBungeeDrop(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook(currentPlatform) : new DynaBungeeHook(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop(currentPlatform) : new DynaBusStop(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak(currentPlatform) : new DynaCamTweak(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough(currentPlatform) : new DynaFlythrough(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__NPCSettings:
                        _dynaSpecific = reset ? new DynaNPCSettings(currentPlatform) : new DynaNPCSettings(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox(currentPlatform) : new DynaTalkBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox(currentPlatform) : new DynaTaskBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__Taxi:
                        _dynaSpecific = reset ? new DynaTaxi(currentPlatform) : new DynaTaxi(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_BFBB(currentPlatform, Version) : new DynaTeleport_BFBB(dataMiddle, currentPlatform, Version);
                        break;
                    case DynaType_BFBB.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox(currentPlatform) : new DynaTextBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3(currentPlatform) : new DynaHudMeterFontV3(dataMiddle, currentPlatform);
                        else
                            _dynaSpecific = reset ? new DynaBase(currentPlatform) : new DynaBase(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit(currentPlatform) : new DynaHudMeterUnit(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel(currentPlatform) : new DynaHudModel(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.hud__text:
                        _dynaSpecific = reset ? new DynaHudText(currentPlatform) : new DynaHudText(dataMiddle, currentPlatform);
                        break;
                    case DynaType_BFBB.pointer:
                        _dynaSpecific = reset ? new DynaPointer(currentPlatform) : new DynaPointer(dataMiddle, currentPlatform);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase(currentPlatform) : new DynaBase(dataMiddle, currentPlatform);
                        break;
                }
            else if (currentGame == Game.Incredibles)
                switch (Type_TSSM)
                {
                    case DynaType_TSSM.pointer:
                        _dynaSpecific = reset ? new DynaPointer(currentPlatform) : new DynaPointer(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox(currentPlatform) : new DynaTalkBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox(currentPlatform) : new DynaTextBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox(currentPlatform) : new DynaTaskBox(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop(currentPlatform) : new DynaBusStop(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough(currentPlatform) : new DynaFlythrough(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen(currentPlatform) : new DynaBoulderGen(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_MovieGame(currentPlatform) : new DynaTeleport_MovieGame(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop(currentPlatform) : new DynaBungeeDrop(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook(currentPlatform) : new DynaBungeeHook(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak(currentPlatform) : new DynaCamTweak(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel(currentPlatform) : new DynaHudModel(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3(currentPlatform) : new DynaHudMeterFontV3(dataMiddle, currentPlatform);
                        else
                            _dynaSpecific = reset ? new DynaBase(currentPlatform) : new DynaBase(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit(currentPlatform) : new DynaHudMeterUnit(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.hud__text:
                        _dynaSpecific = reset ? new DynaHudText(currentPlatform) : new DynaHudText(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.effect__ScreenFade:
                        _dynaSpecific = reset ? new DynaEffectScreenFade(currentPlatform) : new DynaEffectScreenFade(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.effect__Lightning:
                        _dynaSpecific = reset ? new DynaEffectLightning(currentPlatform) : new DynaEffectLightning(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__BucketOTron:
                        _dynaSpecific = reset ? new DynaEnemyBucketOTron(currentPlatform) : new DynaEnemyBucketOTron(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__CastNCrew:
                        _dynaSpecific = reset ? new DynaEnemyCastNCrew(currentPlatform) : new DynaEnemyCastNCrew(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Critter:
                        _dynaSpecific = reset ? new DynaEnemyCritter(currentPlatform) : new DynaEnemyCritter(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Dennis:
                        _dynaSpecific = reset ? new DynaEnemyDennis(currentPlatform) : new DynaEnemyDennis(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__FrogFish:
                        _dynaSpecific = reset ? new DynaEnemyFrogFish(currentPlatform) : new DynaEnemyFrogFish(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Mindy:
                        _dynaSpecific = reset ? new DynaEnemyMindy(currentPlatform) : new DynaEnemyMindy(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Neptune:
                        _dynaSpecific = reset ? new DynaEnemyNeptune(currentPlatform) : new DynaEnemyNeptune(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Standard:
                        _dynaSpecific = reset ? new DynaEnemyStandard(currentPlatform) : new DynaEnemyStandard(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__SupplyCrate:
                        _dynaSpecific = reset ? new DynaSupplyCrate(currentPlatform) : new DynaSupplyCrate(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Turret:
                        _dynaSpecific = reset ? new DynaEnemyTurret(currentPlatform) : new DynaEnemyTurret(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__Ring:
                        _dynaSpecific = reset ? new DynaRing(currentPlatform) : new DynaRing(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__RingControl:
                        _dynaSpecific = reset ? new DynaRingControl(currentPlatform) : new DynaRingControl(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__Vent:
                        _dynaSpecific = reset ? new DynaVent(currentPlatform) : new DynaVent(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.game_object__VentType:
                        _dynaSpecific = reset ? new DynaVentType(currentPlatform) : new DynaVentType(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.SceneProperties:
                        _dynaSpecific = reset ? new DynaSceneProperties(currentPlatform) : new DynaSceneProperties(dataMiddle, currentPlatform);
                        break;
                    case DynaType_TSSM.JSPExtraData:
                        _dynaSpecific = reset ? new DynaJSPExtraData(currentPlatform) : new DynaJSPExtraData(dataMiddle, currentPlatform);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase(currentPlatform) : new DynaBase(dataMiddle, currentPlatform);
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
            if (!isSelected && (dontRender || isInvisible))
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