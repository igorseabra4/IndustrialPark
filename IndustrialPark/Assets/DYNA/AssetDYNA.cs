using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetDYNA : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        public AssetDYNA(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            SetDynaSpecific(false);
        }

        public override bool HasReference(uint assetID) => DynaBase.HasReference(assetID) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            DynaBase.Verify(ref result);
            base.Verify(ref result);
        }

        public void SetDynaSpecific(bool reset)
        {
            switch (Type)
            {
                case DynaType.game_object__BoulderGenerator:
                    DynaBase = new DynaBoulderGen(this);
                    break;
                case DynaType.game_object__bungee_drop:
                    DynaBase = new DynaBungeeDrop(this);
                    break;
                case DynaType.game_object__bungee_hook:
                    DynaBase = new DynaBungeeHook(this);
                    break;
                case DynaType.game_object__BusStop:
                    DynaBase = new DynaBusStop(this);
                    break;
                case DynaType.game_object__Camera_Tweak:
                    DynaBase = new DynaCamTweak(this);
                    break;
                case DynaType.game_object__Flythrough:
                    DynaBase = new DynaFlythrough(this);
                    break;
                case DynaType.game_object__NPCSettings:
                    DynaBase = new DynaNPCSettings(this);
                    break;
                case DynaType.game_object__talk_box:
                    DynaBase = new DynaTalkBox(this);
                    break;
                case DynaType.game_object__task_box:
                    DynaBase = new DynaTaskBox(this);
                    break;
                case DynaType.game_object__Taxi:
                    DynaBase = new DynaTaxi(this);
                    break;
                case DynaType.game_object__Teleport:
                    //if (game == Game.BFBB)
                        DynaBase = new DynaTeleport_BFBB(this, Version);
                    //else
                    //    _dynaSpecific = new DynaTeleport(this);
                    break;
                case DynaType.game_object__text_box:
                    DynaBase = new DynaTextBox(this);
                    break;
                case DynaType.hud__meter__font:
                    if (Version == 3)
                        DynaBase = new DynaHudMeterFontV3(this);
                    else
                        DynaBase = new DynaBase(this);
                    break;
                case DynaType.hud__meter__unit:
                    DynaBase = new DynaHudMeterUnit(this);
                    break;
                case DynaType.hud__model:
                    DynaBase = new DynaHudModel(this);
                    break;
                case DynaType.hud__text:
                    DynaBase = new DynaHudText(this);
                    break;
                case DynaType.pointer:
                    DynaBase = new DynaPointer(this);
                    break;
                case DynaType.effect__ScreenFade:
                    DynaBase = new DynaEffectScreenFade(this);
                    break;
                case DynaType.effect__Lightning:
                    DynaBase = new DynaEffectLightning(this);
                    break;
                case DynaType.Enemy__SB__BucketOTron:
                    DynaBase = new DynaEnemyBucketOTron(this);
                    break;
                case DynaType.Enemy__SB__CastNCrew:
                    DynaBase = new DynaEnemyCastNCrew(this);
                    break;
                case DynaType.Enemy__SB__Critter:
                    DynaBase = new DynaEnemyCritter(this);
                    break;
                case DynaType.Enemy__SB__Dennis:
                    DynaBase = new DynaEnemyDennis(this);
                    break;
                case DynaType.Enemy__SB__FrogFish:
                    DynaBase = new DynaEnemyFrogFish(this);
                    break;
                case DynaType.Enemy__SB__Mindy:
                    DynaBase = new DynaEnemyMindy(this);
                    break;
                case DynaType.Enemy__SB__Neptune:
                    DynaBase = new DynaEnemyNeptune(this);
                    break;
                case DynaType.Enemy__SB__Standard:
                    DynaBase = new DynaEnemyStandard(this);
                    break;
                case DynaType.Enemy__SB__SupplyCrate:
                    DynaBase = new DynaSupplyCrate(this);
                    break;
                case DynaType.Enemy__SB__Turret:
                    DynaBase = new DynaEnemyTurret(this);
                    break;
                case DynaType.game_object__Ring:
                    DynaBase = new DynaRing(this);
                    break;
                case DynaType.game_object__RingControl:
                    DynaBase = new DynaRingControl(this);
                    break;
                case DynaType.game_object__Vent:
                    DynaBase = new DynaVent(this);
                    break;
                case DynaType.game_object__VentType:
                    DynaBase = new DynaVentType(this);
                    break;
                case DynaType.SceneProperties:
                    DynaBase = new DynaSceneProperties(this);
                    break;
                case DynaType.JSPExtraData:
                    DynaBase = new DynaJSPExtraData(this);
                    break;
                case DynaType.game_object__IN_Pickup:
                    DynaBase = new DynaInPickup(this);
                    break;
                default:
                    DynaBase = new DynaBase(this);
                    break;
            }

            if (reset)
            {
                List<byte> dataBefore = Data.Take(0x10).ToList();
                for (int i = 0; i < DynaBase.StructSize; i++)
                    dataBefore.Add(0);
                dataBefore.AddRange(Data.Skip(EventStartOffset));
                Data = dataBefore.ToArray();
            }

            if (IsRenderableClickable)
            {
                CreateTransformMatrix();
                if (!renderableAssetSetCommon.Contains(this))
                    renderableAssetSetCommon.Add(this);
            }
            else if (renderableAssetSetCommon.Contains(this))
                renderableAssetSetCommon.Remove(this);
        }
        
        [Category("Dynamic")]
        public DynaType Type
        {
            get => (DynaType)ReadUInt(0x8);
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

        [Category("Dynamic")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DynaBase DynaBase { get; set; }

        public static bool dontRender = false;

        [Browsable(false)]
        public bool IsRenderableClickable { get => DynaBase.IsRenderableClickable; }

        [Browsable(false)]
        public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);
        [Browsable(false)]
        public float PositionX { get => DynaBase.PositionX; set => DynaBase.PositionX = value; }
        [Browsable(false)]
        public float PositionY { get => DynaBase.PositionY; set => DynaBase.PositionY = value; }
        [Browsable(false)]
        public float PositionZ { get => DynaBase.PositionZ; set => DynaBase.PositionZ = value; }
        [Browsable(false)]
        public float Yaw { get => DynaBase.Yaw; set => DynaBase.Yaw = value; }
        [Browsable(false)]
        public float Pitch { get => DynaBase.Pitch; set => DynaBase.Pitch = value; }
        [Browsable(false)]
        public float Roll { get => DynaBase.Roll; set => DynaBase.Roll = value; }
        [Browsable(false)]
        public float ScaleX { get => DynaBase.ScaleX; set => DynaBase.ScaleX = value; }
        [Browsable(false)]
        public float ScaleY { get => DynaBase.ScaleY; set => DynaBase.ScaleY = value; }
        [Browsable(false)]
        public float ScaleZ { get => DynaBase.ScaleZ; set => DynaBase.ScaleZ = value; }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            return DynaBase.IntersectsWith(ray);
        }
        
        public void CreateTransformMatrix()
        {
            DynaBase.CreateTransformMatrix();
        }

        public void Draw(SharpRenderer renderer)
        {
            if (!isSelected && (dontRender || isInvisible))
                return;

            DynaBase.Draw(renderer, isSelected);
        }

        public BoundingBox GetBoundingBox()
        {
            return DynaBase.GetBoundingBox();
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return DynaBase.GetDistance(cameraPosition);
        }

        public BoundingSphere GetObjectCenter()
        {
            return DynaBase.GetObjectCenter();
        }
    }
}