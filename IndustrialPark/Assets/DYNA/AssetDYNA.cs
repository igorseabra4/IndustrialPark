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

            if (game == Game.BFBB)
                switch (Type_BFBB)
                {
                    case DynaType_BFBB.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen(platform) : new DynaBoulderGen(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop(platform) : new DynaBungeeDrop(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook(platform) : new DynaBungeeHook(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop(platform) : new DynaBusStop(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak(platform) : new DynaCamTweak(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough(platform) : new DynaFlythrough(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__NPCSettings:
                        _dynaSpecific = reset ? new DynaNPCSettings(platform) : new DynaNPCSettings(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox(platform) : new DynaTalkBox(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox(platform) : new DynaTaskBox(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__Taxi:
                        _dynaSpecific = reset ? new DynaTaxi(platform) : new DynaTaxi(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_BFBB(platform, Version) : new DynaTeleport_BFBB(dataMiddle, platform, Version);
                        break;
                    case DynaType_BFBB.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox(platform) : new DynaTextBox(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3(platform) : new DynaHudMeterFontV3(dataMiddle, platform);
                        else
                            _dynaSpecific = reset ? new DynaBase(platform) : new DynaBase(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit(platform) : new DynaHudMeterUnit(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel(platform) : new DynaHudModel(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.hud__text:
                        _dynaSpecific = reset ? new DynaHudText(platform) : new DynaHudText(dataMiddle, platform);
                        break;
                    case DynaType_BFBB.pointer:
                        _dynaSpecific = reset ? new DynaPointer(platform) : new DynaPointer(dataMiddle, platform);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase(platform) : new DynaBase(dataMiddle, platform);
                        break;
                }
            else if (game == Game.Incredibles)
                switch (Type_TSSM)
                {
                    case DynaType_TSSM.pointer:
                        _dynaSpecific = reset ? new DynaPointer(platform) : new DynaPointer(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__talk_box:
                        _dynaSpecific = reset ? new DynaTalkBox(platform) : new DynaTalkBox(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__text_box:
                        _dynaSpecific = reset ? new DynaTextBox(platform) : new DynaTextBox(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__task_box:
                        _dynaSpecific = reset ? new DynaTaskBox(platform) : new DynaTaskBox(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__BusStop:
                        _dynaSpecific = reset ? new DynaBusStop(platform) : new DynaBusStop(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__Flythrough:
                        _dynaSpecific = reset ? new DynaFlythrough(platform) : new DynaFlythrough(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__BoulderGenerator:
                        _dynaSpecific = reset ? new DynaBoulderGen(platform) : new DynaBoulderGen(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__Teleport:
                        _dynaSpecific = reset ? new DynaTeleport_MovieGame(platform) : new DynaTeleport_MovieGame(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__bungee_drop:
                        _dynaSpecific = reset ? new DynaBungeeDrop(platform) : new DynaBungeeDrop(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__bungee_hook:
                        _dynaSpecific = reset ? new DynaBungeeHook(platform) : new DynaBungeeHook(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__Camera_Tweak:
                        _dynaSpecific = reset ? new DynaCamTweak(platform) : new DynaCamTweak(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.hud__model:
                        _dynaSpecific = reset ? new DynaHudModel(platform) : new DynaHudModel(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.hud__meter__font:
                        if (Version == 3)
                            _dynaSpecific = reset ? new DynaHudMeterFontV3(platform) : new DynaHudMeterFontV3(dataMiddle, platform);
                        else
                            _dynaSpecific = reset ? new DynaBase(platform) : new DynaBase(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.hud__meter__unit:
                        _dynaSpecific = reset ? new DynaHudMeterUnit(platform) : new DynaHudMeterUnit(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.hud__text:
                        _dynaSpecific = reset ? new DynaHudText(platform) : new DynaHudText(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.effect__ScreenFade:
                        _dynaSpecific = reset ? new DynaEffectScreenFade(platform) : new DynaEffectScreenFade(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.effect__Lightning:
                        _dynaSpecific = reset ? new DynaEffectLightning(platform) : new DynaEffectLightning(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__BucketOTron:
                        _dynaSpecific = reset ? new DynaEnemyBucketOTron(platform) : new DynaEnemyBucketOTron(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__CastNCrew:
                        _dynaSpecific = reset ? new DynaEnemyCastNCrew(platform) : new DynaEnemyCastNCrew(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Critter:
                        _dynaSpecific = reset ? new DynaEnemyCritter(platform) : new DynaEnemyCritter(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Dennis:
                        _dynaSpecific = reset ? new DynaEnemyDennis(platform) : new DynaEnemyDennis(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__FrogFish:
                        _dynaSpecific = reset ? new DynaEnemyFrogFish(platform) : new DynaEnemyFrogFish(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Mindy:
                        _dynaSpecific = reset ? new DynaEnemyMindy(platform) : new DynaEnemyMindy(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Neptune:
                        _dynaSpecific = reset ? new DynaEnemyNeptune(platform) : new DynaEnemyNeptune(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Standard:
                        _dynaSpecific = reset ? new DynaEnemyStandard(platform) : new DynaEnemyStandard(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__SupplyCrate:
                        _dynaSpecific = reset ? new DynaSupplyCrate(platform) : new DynaSupplyCrate(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.Enemy__SB__Turret:
                        _dynaSpecific = reset ? new DynaEnemyTurret(platform) : new DynaEnemyTurret(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__Ring:
                        _dynaSpecific = reset ? new DynaRing(platform) : new DynaRing(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__RingControl:
                        _dynaSpecific = reset ? new DynaRingControl(platform) : new DynaRingControl(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__Vent:
                        _dynaSpecific = reset ? new DynaVent(platform) : new DynaVent(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.game_object__VentType:
                        _dynaSpecific = reset ? new DynaVentType(platform) : new DynaVentType(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.SceneProperties:
                        _dynaSpecific = reset ? new DynaSceneProperties(platform) : new DynaSceneProperties(dataMiddle, platform);
                        break;
                    case DynaType_TSSM.JSPExtraData:
                        _dynaSpecific = reset ? new DynaJSPExtraData(platform) : new DynaJSPExtraData(dataMiddle, platform);
                        break;
                    default:
                        _dynaSpecific = reset ? new DynaBase(platform) : new DynaBase(dataMiddle, platform);
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