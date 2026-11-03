using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IndustrialPark.Archive;

public static class AssetService
{
    public static Asset ConvertHeaderToAsset(Section_AHDR AHDR, Game game, Platform platform, bool testBuild, out AssetError error)
    {
        error = new AssetError() { reason = ErrorReason.NoError, assetID = AHDR.assetID, assetName = AHDR.ADBG.assetName };

        Asset newAsset;
        try
        {
            newAsset = ConvertHeaderToAsset(AHDR, game, platform);
        }
        catch (Exception ex)
        {
            if (AHDR.data.Length == 0)
            {
                error.reason = ErrorReason.NoData;
                error.errorMessage = "Asset is empty and does not contain any data";
            }
            else
            {
                error.reason = ErrorReason.Exception;
                error.errorMessage = $"Asset format currently not supported by Industrial Park: {ex.Message}";
            }

            newAsset = new AssetGeneric(AHDR, game, platform.Endianness());
        }

        // test if IP can build the same asset it read
        if (testBuild && !newAsset.SkipBuildTesting && error.reason == ErrorReason.NoError)
        {
            var endianness = platform.Endianness();
            var built = newAsset.Serialize(endianness);
            var isSndiV2 = newAsset is AssetSNDI_GCN_V2;
            if (!BuiltDataMatches(AHDR.data, built, isSndiV2))
            {
                error.reason = ErrorReason.SequenceNotEqual;
                error.errorMessage = "Asset format not supported by Industrial Park";
                newAsset = new AssetGeneric(AHDR, game, endianness);
#if DEBUG
                string folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "build_test_out");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string assetName = $"[{AHDR.assetType}] {AHDR.ADBG.assetName}";
                File.WriteAllBytes(Path.Combine(folder, assetName + " correct"), AHDR.data);
                File.WriteAllBytes(Path.Combine(folder, assetName + " incorct"), built);
#endif
            }
        }

        return newAsset;
    }

    private static bool BuiltDataMatches(byte[] existingData, byte[] builtData, bool isSndiV2)
    {
        // A previous error in SNDI building causes correct SNDI's to show up as an error.
        // This is caused by a wrong AssetID set in the first 4 bytes.
        // When serializing, the correct id will be written but testing fails with the existing data.
        // Happens with levels created before the fix commit (823f78662b1c4660c39d6dc6e8b8d71d9db8b033)
        // Skip asset id and test remaining data.
        if (isSndiV2)
            return Enumerable.SequenceEqual(existingData.Skip(4), builtData.Skip(4));
        return Enumerable.SequenceEqual(existingData, builtData);
    }

    private static Asset ConvertHeaderToAsset(Section_AHDR AHDR, Game game, Platform platform)
    {
        var endianness = platform.Endianness();

        if (AHDR.IsDyna)
            return ConvertHeaderToDYNA(AHDR, game, endianness);

        switch (AHDR.assetType)
        {
            case AssetType.Animation when AHDR.data.Length == 0 || (game == Game.Scooby && AHDR.ADBG.assetName.EndsWith("mp")):
                return new AssetGeneric(AHDR, game, endianness);
            case AssetType.Animation when game >= Game.ROTU:
                return new AssetANIM_V2(AHDR, game, endianness);
            case AssetType.Animation:
                return new AssetANIM_V1(AHDR, game, endianness);
            case AssetType.BSP:
            case AssetType.JSP:
                return new AssetJSP(AHDR, game);
            case AssetType.JSPInfo:
                return new AssetJSP_INFO(AHDR, game, endianness);
            case AssetType.Model:
                return new AssetMODL(AHDR, game);
            case AssetType.Texture when AHDR.data.Length != 0:
            case AssetType.TextureStream when AHDR.data.Length != 0:
                return new AssetRWTX(AHDR, game, endianness);
            case AssetType.SoundInfo when platform == Platform.GameCube && game < Game.Incredibles:
                return new AssetSNDI_GCN_V1(AHDR, game);
            case AssetType.SoundInfo when platform == Platform.GameCube:
                return new AssetSNDI_GCN_V2(AHDR, game);
            case AssetType.SoundInfo when platform == Platform.Xbox:
                return new AssetSNDI_XBOX(AHDR, game);
            case AssetType.SoundInfo when platform == Platform.PS2:
                return new AssetSNDI_PS2(AHDR, game);
            case AssetType.Spline:
                return new AssetSPLN(AHDR, game, endianness, Program.Renderer);
            case AssetType.SplinePath:
                return new AssetSPLP(AHDR, game, endianness);
            case AssetType.WireframeModel:
                return new AssetWIRE(AHDR, game, endianness, Program.Renderer);
            case AssetType.AnimationList:
                return new AssetALST(AHDR, game, endianness);
            case AssetType.AnimationTable:
                return new AssetATBL(AHDR, game, endianness);
            case AssetType.AttackTable:
                return new AssetATKT(AHDR, game, endianness);
            case AssetType.Boulder:
                return new AssetBOUL(AHDR, game, endianness);
            case AssetType.Button:
                return new AssetBUTN(AHDR, game, endianness);
            case AssetType.Camera:
                return new AssetCAM(AHDR, game, endianness);
            case AssetType.CameraCurve:
                return new AssetCCRV(AHDR, game, endianness);
            case AssetType.Counter:
                return new AssetCNTR(AHDR, game, endianness);
            case AssetType.CollisionTable:
                return new AssetCOLL(AHDR, game, endianness);
            case AssetType.Conditional:
                return new AssetCOND(AHDR, game, endianness);
            case AssetType.Credits:
                return new AssetCRDT(AHDR, game, endianness);
            case AssetType.Cutscene:
                return new AssetCSN(AHDR, game, endianness);
            case AssetType.CutsceneManager:
                return new AssetCSNM(AHDR, game, endianness);
            case AssetType.Destructible:
                return new AssetDEST(AHDR, game, endianness);
            case AssetType.Dispatcher:
                return new AssetDPAT(AHDR, game, endianness);
            case AssetType.DiscoFloor:
                return new AssetDSCO(AHDR, game, endianness);
            case AssetType.DestructibleObject:
                return new AssetDSTR(AHDR, game, endianness);
            case AssetType.DashTrack:
                return new AssetDTRK(AHDR, game, endianness);
            case AssetType.Duplicator:
                return new AssetDUPC(AHDR, game, endianness);
            case AssetType.ElectricArcGenerator:
                return new AssetEGEN(AHDR, game, endianness);
            case AssetType.Environment:
                return new AssetENV(AHDR, game, endianness);
            case AssetType.Flythrough:
                return new AssetFLY(AHDR, game);
            case AssetType.Fog:
                return new AssetFOG(AHDR, game, endianness);
            case AssetType.Group:
                return new AssetGRUP(AHDR, game, endianness);
            case AssetType.GrassMesh:
                return new AssetGRSM(AHDR, game, endianness);
            case AssetType.Gust:
                return new AssetGUST(AHDR, game, endianness);
            case AssetType.Hangable:
                return new AssetHANG(AHDR, game, endianness);
            case AssetType.JawDataTable:
                return new AssetJAW(AHDR, game, endianness);
            case AssetType.Light:
                return new AssetLITE(AHDR, game, endianness);
            case AssetType.LightKit when AHDR.data.Length != 0:
                return new AssetLKIT(AHDR, game, endianness);
            case AssetType.LobMaster:
                return new AssetLOBM(AHDR, game, endianness);
            case AssetType.LevelOfDetailTable:
                return new AssetLODT(AHDR, game, endianness);
            case AssetType.SurfaceMapper:
                return new AssetMAPR(AHDR, game, endianness);
            case AssetType.ModelInfo:
                return new AssetMINF(AHDR, game, endianness);
            case AssetType.Marker:
                return new AssetMRKR(AHDR, game, endianness);
            case AssetType.MovePoint:
                return new AssetMVPT(AHDR, game, endianness);
            case AssetType.Villain:
                return new AssetNPC(AHDR, game, endianness);
            case AssetType.OneLiner:
                return new AssetONEL(AHDR, game, endianness);
            case AssetType.ParticleEmitter:
                return new AssetPARE(AHDR, game, endianness);
            case AssetType.ParticleProperties:
                return new AssetPARP(AHDR, game, endianness);
            case AssetType.ParticleSystem:
                return new AssetPARS(AHDR, game, endianness);
            case AssetType.Pendulum:
                return new AssetPEND(AHDR, game, endianness);
            case AssetType.ProgressScript:
                return new AssetPGRS(AHDR, game, endianness);
            case AssetType.PickupTable:
                return new AssetPICK(AHDR, game, endianness);
            case AssetType.PipeInfoTable:
                return new AssetPIPT(AHDR, game, endianness);
            case AssetType.Pickup:
                return new AssetPKUP(AHDR, game, endianness);
            case AssetType.Platform:
                return new AssetPLAT(AHDR, game, endianness);
            case AssetType.Player:
                return new AssetPLYR(AHDR, game, endianness);
            case AssetType.Portal:
                return new AssetPORT(AHDR, game, endianness);
            case AssetType.Projectile:
                return new AssetPRJT(AHDR, game, endianness);
            case AssetType.ReactiveAnimation:
                return new AssetRANM(AHDR, game, endianness);
            case AssetType.Script:
                return new AssetSCRP(AHDR, game, endianness);
            case AssetType.SDFX:
                return new AssetSDFX(AHDR, game, endianness);
            case AssetType.SFX:
                return new AssetSFX(AHDR, game, endianness);
            case AssetType.SoundGroup:
                return new AssetSGRP(AHDR, game, endianness);
            case AssetType.Track:
            case AssetType.SimpleObject:
                return new AssetSIMP(AHDR, game, endianness);
            case AssetType.ShadowTable:
                return new AssetSHDW(AHDR, game, endianness);
            case AssetType.Shrapnel:
                return new AssetSHRP(AHDR, game, endianness);
            case AssetType.Surface:
                return new AssetSURF(AHDR, game, endianness);
            case AssetType.Text:
                return new AssetTEXT(AHDR, game, endianness);
            case AssetType.Trigger:
                return new AssetTRIG(AHDR, game, endianness);
            case AssetType.Timer:
                return new AssetTIMR(AHDR, game, endianness);
            case AssetType.ThrowableTable:
                return new AssetTRWT(AHDR, game, endianness);
            case AssetType.PickupTypes:
                return new AssetTPIK(AHDR, game, endianness);
            case AssetType.UserInterface:
                return new AssetUI(AHDR, game, endianness);
            case AssetType.UserInterfaceFont:
                return new AssetUIFT(AHDR, game, endianness);
            case AssetType.UserInterfaceMotion:
                return new AssetUIM(AHDR, game, endianness);
            case AssetType.NPC:
                return new AssetVIL(AHDR, game, endianness);
            case AssetType.NPCProperties:
                return new AssetVILP(AHDR, game, endianness);
            case AssetType.Volume:
                return new AssetVOLU(AHDR, game, endianness);
            case AssetType.ZipLine:
                return new AssetZLIN(AHDR, game, endianness);

            case AssetType.Sound:
            case AssetType.SoundStream:
                return new AssetSound(AHDR, game, platform);

            case AssetType.NavigationMesh:
            case AssetType.SlideProperty:
            case AssetType.SceneSettings:
                return new AssetGenericBase(AHDR, game, endianness);

            case AssetType.BinkVideo:
            case AssetType.CutsceneStreamingSound:
            case AssetType.MorphTarget:
            case AssetType.NPCSettings:
            case AssetType.RawImage:
            case AssetType.Subtitles:
            case AssetType.UIFN:

            case AssetType.Null:

            case AssetType.CutsceneTableOfContents:

            case AssetType.SoundInfo:
            case AssetType.Texture:
            case AssetType.TextureStream:
            case AssetType.LightKit:

                return new AssetGeneric(AHDR, game, endianness);
            default:
                return null;
        }
        throw new Exception($"Unknown asset type ({AHDR.assetType})");
    }

    private static AssetDYNA ConvertHeaderToDYNA(Section_AHDR AHDR, Game game, Endianness endianness)
    {
        DynaType type;
        using (var reader = new EndianBinaryReader(AHDR.data, endianness))
        {
            reader.BaseStream.Position = 8;
            type = (DynaType)reader.ReadUInt32();
        }

        return type switch
        {
            DynaType.audio__conversation => new DynaAudioConversation(AHDR, game, endianness),
            DynaType.Checkpoint => new DynaCheckpoint(AHDR, game, endianness),
            DynaType.camera__preset => new DynaCameraPreset(AHDR, game, endianness),
            DynaType.Enemy__SB__BucketOTron => new DynaEnemyBucketOTron(AHDR, game, endianness),
            DynaType.Enemy__SB__CastNCrew => new DynaEnemyCastNCrew(AHDR, game, endianness),
            DynaType.Enemy__SB__Critter => new DynaEnemyCritter(AHDR, game, endianness),
            DynaType.Enemy__SB__Dennis => new DynaEnemyDennis(AHDR, game, endianness),
            DynaType.Enemy__SB__FrogFish => new DynaEnemyFrogFish(AHDR, game, endianness),
            DynaType.Enemy__SB__Mindy => new DynaEnemyMindy(AHDR, game, endianness),
            DynaType.Enemy__SB__Neptune => new DynaEnemyNeptune(AHDR, game, endianness),
            DynaType.Enemy__SB__Standard => new DynaEnemyStandard(AHDR, game, endianness),
            DynaType.Enemy__SB__SupplyCrate => new DynaEnemySupplyCrate(AHDR, game, endianness),
            DynaType.Enemy__SB__Turret => new DynaEnemyTurret(AHDR, game, endianness),
            DynaType.Incredibles__Icon => new DynaIncrediblesIcon(AHDR, game, endianness),
            DynaType.JSPExtraData => new DynaJSPExtraData(AHDR, game, endianness),
            DynaType.SceneProperties => new DynaSceneProperties(AHDR, game, endianness),
            DynaType.effect__Flamethrower => new DynaEffectFlamethrower(AHDR, game, endianness),
            DynaType.effect__LensFlareElement => new DynaEffectLensFlare(AHDR, game, endianness),
            DynaType.effect__LensFlareSource => new DynaEffectLensFlareSource(AHDR, game, endianness),
            DynaType.effect__Lightning => new DynaEffectLightning(AHDR, game, endianness),
            DynaType.effect__Rumble => new DynaEffectRumble(AHDR, game, endianness),
            DynaType.effect__RumbleSphericalEmitter => new DynaEffectRumbleSphere(AHDR, game, endianness),
            DynaType.effect__ScreenFade => new DynaEffectScreenFade(AHDR, game, endianness),
            DynaType.effect__Splash => new DynaEffectSplash(AHDR, game, endianness),
            DynaType.effect__grass => new DynaEffectGrass(AHDR, game, endianness),
            DynaType.effect__smoke_emitter => new DynaEffectSmokeEmitter(AHDR, game, endianness),
            DynaType.effect__spark_emitter => new DynaEffectSparkEmitter(AHDR, game, endianness),
            DynaType.effect__spotlight => new DynaEffectSpotlight(AHDR, game, endianness),
            DynaType.effect__uber_laser => new DynaEffectUberLaser(AHDR, game, endianness),
            DynaType.effect__water_body => new DynaEffectWaterBody(AHDR, game, endianness),
            DynaType.Effect__particle_generator => new DynaEffectParticleGenerator(AHDR, game, endianness),
            DynaType.game_object__BoulderGenerator => new DynaGObjectBoulderGen(AHDR, game, endianness),
            DynaType.game_object__BusStop => new DynaGObjectBusStop(AHDR, game, endianness),
            DynaType.game_object__Camera_Tweak => new DynaGObjectCamTweak(AHDR, game, endianness),
            DynaType.game_object__Flythrough => new DynaGObjectFlythrough(AHDR, game, endianness),
            DynaType.game_object__Grapple => new DynaGObjectGrapple(AHDR, game, endianness),
            DynaType.game_object__Hangable => new DynaGObjectHangable(AHDR, game, endianness),
            DynaType.game_object__IN_Pickup => new DynaGObjectInPickup(AHDR, game, endianness),
            DynaType.game_object__NPCSettings => new DynaGObjectNPCSettings(AHDR, game, endianness),
            DynaType.game_object__RaceTimer => new DynaGObjectRaceTimer(AHDR, game, endianness),
            DynaType.game_object__Ring => new DynaGObjectRing(AHDR, game, endianness),
            DynaType.game_object__RingControl => new DynaGObjectRingControl(AHDR, game, endianness),
            DynaType.game_object__RubbleGenerator => new DynaGObjectRubbleGenerator(AHDR, game, endianness),
            DynaType.game_object__Taxi => new DynaGObjectTaxi(AHDR, game, endianness),
            DynaType.game_object__Teleport => new DynaGObjectTeleport(AHDR, game, endianness),
            DynaType.game_object__Turret => new DynaGObjectTurret(AHDR, game, endianness),
            DynaType.game_object__Vent => new DynaGObjectVent(AHDR, game, endianness),
            DynaType.game_object__VentType => new DynaGObjectVentType(AHDR, game, endianness),
            DynaType.game_object__bungee_drop => new DynaGObjectBungeeDrop(AHDR, game, endianness),
            DynaType.game_object__bungee_hook => new DynaGObjectBungeeHook(AHDR, game, endianness),
            DynaType.game_object__camera_param_asset => new DynaGObjectCameraParamAsset(AHDR, game, endianness),
            DynaType.game_object__dash_camera_spline => new DynaGObjectDashCameraSpline(AHDR, game, endianness),
            DynaType.game_object__flame_emitter => new DynaGObjectFlameEmitter(AHDR, game, endianness),
            DynaType.game_object__laser_beam => new DynaGObjectLaserBeam(AHDR, game, endianness),
            DynaType.game_object__talk_box => new DynaGObjectTalkBox(AHDR, game, endianness),
            DynaType.game_object__task_box => new DynaGObjectTaskBox(AHDR, game, endianness),
            DynaType.game_object__text_box => new DynaGObjectTextBox(AHDR, game, endianness),
            DynaType.game_object__train_car => new DynaGObjectTrainCar(AHDR, game, endianness),
            DynaType.game_object__train_junction => new DynaGObjectTrainJunction(AHDR, game, endianness),
            DynaType.hud__image => new DynaHudImage(AHDR, game, endianness),
            DynaType.hud__meter__font => new DynaHudMeterFont(AHDR, game, endianness),
            DynaType.hud__meter__unit => new DynaHudMeterUnit(AHDR, game, endianness),
            DynaType.hud__model => new DynaHudModel(AHDR, game, endianness),
            DynaType.hud__text => new DynaHudText(AHDR, game, endianness),
            DynaType.interaction__Launch => new DynaInteractionLaunch(AHDR, game, endianness),
            DynaType.interaction__Lift => new DynaInteractionLift(AHDR, game, endianness),
            DynaType.interaction__Turn => new DynaInteractionTurn(AHDR, game, endianness),
            DynaType.logic__reference => new DynaLogicReference(AHDR, game, endianness),
            DynaType.logic__FunctionGenerator => new DynaLogicFunctionGenerator(AHDR, game, endianness),
            DynaType.npc__group => new DynaNPCGroup(AHDR, game, endianness),
            DynaType.pointer => new DynaPointer(AHDR, game, endianness),
            DynaType.ui__box => new DynaUIBox(AHDR, game, endianness),
            DynaType.ui__controller => new DynaUIController(AHDR, game, endianness),
            DynaType.ui__image => new DynaUIImage(AHDR, game, endianness),
            DynaType.ui__model => new DynaUIModel(AHDR, game, endianness),
            DynaType.ui__text => new DynaUIText(AHDR, game, endianness),
            DynaType.ui__text__userstring => new DynaUITextUserString(AHDR, game, endianness),
            DynaType.Interest_Pointer => new DynaInterestPointer(AHDR, game, endianness),
            DynaType.camera__binary_poi => new DynaCameraBinary(AHDR, game, endianness),
            DynaType.effect__LightEffectFlicker => new DynaEffectLightFlicker(AHDR, game, endianness),
            DynaType.effect__LightEffectStrobe => new DynaEffectLightStrobe(AHDR, game, endianness),
            DynaType.effect__ScreenWarp => new DynaEffectScreenWarp(AHDR, game, endianness),
            DynaType.effect__light => new DynaEffectLight(AHDR, game, endianness),
            DynaType.game_object__bullet_mark => new DynaGObjectBulletMark(AHDR, game, endianness),
            DynaType.game_object__bullet_time => new DynaGObjectBulletTime(AHDR, game, endianness),
            DynaType.game_object__rband_camera_asset => new DynaGObjectCameraRband(AHDR, game, endianness),
            DynaType.npc__CoverPoint => new DynaNPCCoverpoint(AHDR, game, endianness),
            DynaType.npc__NPC_Custom_AV => new DynaNPCCustomAV(AHDR, game, endianness),
            DynaType.AnalogDeflection => new DynaAnalogDeflection(AHDR, game, endianness),
            DynaType.AnalogDirection => new DynaAnalogDirection(AHDR, game, endianness),
            DynaType.camera__transition_time => new DynaCameraTransitionTime(AHDR, game, endianness),
            DynaType.Carrying_CarryableProperty_GenericUseProperty => new DynaCarryablePropertyGeneric(AHDR, game, endianness),
            DynaType.Carrying_CarryableProperty_UsePropertyAttract => new DynaCarryablePropertyAttract(AHDR, game, endianness),
            DynaType.Carrying_CarryableProperty_UsePropertyRepel => new DynaCarryablePropertyRepel(AHDR, game, endianness),
            DynaType.Carrying_CarryableProperty_UsePropertySwipe => new DynaCarryablePropertySwipe(AHDR, game, endianness),
            DynaType.ContextObject_PoleSwing => new DynaCObjectPoleSwing(AHDR, game, endianness),
            DynaType.ContextObject_Springboard => new DynaCObjectSpringBoard(AHDR, game, endianness),
            DynaType.ContextObject_Tightrope => new DynaCObjectTightrope(AHDR, game, endianness),
            DynaType.Enemy__NPC_Gate => new DynaNpcGate(AHDR, game, endianness),
            DynaType.Enemy__NPC_Walls => new DynaNpcWalls(AHDR, game, endianness),
            DynaType.Enemy__RATS__LeftArm => new DynaEnemyRATSLeftArm(AHDR, game, endianness),
            DynaType.Enemy__RATS__RightArm => new DynaEnemyRATSRightArm(AHDR, game, endianness),
            DynaType.Enemy__RATS__Swarm__Bug => new DynaEnemyRATSSwarmBug(AHDR, game, endianness),
            DynaType.Enemy__RATS__Swarm__Owl => new DynaEnemyRATSSwarmOwl(AHDR, game, endianness),
            DynaType.Enemy__RATS__Thief => new DynaEnemyRATSThief(AHDR, game, endianness),
            DynaType.Enemy__RATS__Waiter => new DynaEnemyRATSWaiter(AHDR, game, endianness),
            DynaType.HUD_Compass_Object => new DynaHudCompassObject(AHDR, game, endianness),
            DynaType.HUD_Compass_System => new DynaHudCompassSystem(AHDR, game, endianness),
            DynaType.logic__Mission => new DynaLogicMission(AHDR, game, endianness),
            DynaType.logic__Task => new DynaLogicTask(AHDR, game, endianness),
            DynaType.Pour_Widget => new DynaPourWidget(AHDR, game, endianness),
            DynaType.Twiddler => new DynaTwiddler(AHDR, game, endianness),
            DynaType.Enemy__IN2__Bomber => new DynaEnemyIN2Bomber(AHDR, game, endianness),
            DynaType.Enemy__IN2__BossUnderminerDrill => new DynaEnemyIN2BossUnderminerDrill(AHDR, game, endianness),
            DynaType.Enemy__IN2__BossUnderminerUM => new DynaEnemyIN2BossUnderminerUM(AHDR, game, endianness),
            DynaType.Enemy__IN2__Chicken => new DynaEnemyIN2Chicken(AHDR, game, endianness),
            DynaType.Enemy__IN2__Driller => new DynaEnemyIN2Driller(AHDR, game, endianness),
            DynaType.Enemy__IN2__Enforcer => new DynaEnemyIN2Enforcer(AHDR, game, endianness),
            DynaType.Enemy__IN2__Humanoid => new DynaEnemyIN2Humanoid(AHDR, game, endianness),
            DynaType.Enemy__IN2__Rat => new DynaEnemyIN2Rat(AHDR, game, endianness),
            DynaType.Enemy__IN2__RobotTank => new DynaEnemyIN2RobotTank(AHDR, game, endianness),
            DynaType.Enemy__IN2__Scientist => new DynaEnemyIN2Scientist(AHDR, game, endianness),
            DynaType.Enemy__IN2__Shooter => new DynaEnemyIN2Shooter(AHDR, game, endianness),
            DynaType.effect__RumbleBoxEmitter or DynaType.effect__Waterhose or DynaType.Enemy__SB or DynaType.camera__transition_path or DynaType.effect__BossBrain or DynaType.game_object__FreezableObject or DynaType.Carrying_CarryableObject or DynaType.interaction__IceBridge or DynaType.interaction__SwitchLever or DynaType.Unknown_EBC04E7B or DynaType.Null => new DynaGeneric(AHDR, type, game, endianness),
            _ => throw new Exception("Unknown DYNA type: " + type.ToString("X8")),
        };
    }

    public static void ReplaceReferencesOn(Asset asset, Dictionary<uint, uint> references)
    {
        foreach ((uint oldAssetId, uint newAssetId) in references)
            asset.ReplaceReferences(oldAssetId, newAssetId);
    }

    public static string GetUniqueAssetName(string assetName, uint assetID, bool forceNumber, bool ignoreNumber, Func<uint, bool> exists)
    {
        int candidateNumber = 0;
        char separator = '_';

        string suffix = "";
        if (assetName.EndsWith(".rw3", StringComparison.InvariantCultureIgnoreCase))
        {
            suffix = ".rw3";
            assetName = assetName.Replace(".rw3", "", StringComparison.InvariantCultureIgnoreCase);
        }

        while (exists(assetID) || forceNumber)
        {
            if (candidateNumber > 10000)
            {
                candidateNumber = 0;
                ignoreNumber = false;
                assetName = "ASSET_01";
            }

            forceNumber = false;
            candidateNumber++;

            if (!ignoreNumber)
            {
                var match = System.Text.RegularExpressions.Regex.Match(assetName, @"(\D?)(\d+)$");
                int len = 2;

                if (match.Success)
                {
                    separator = match.Groups[1].Value.FirstOrDefault('_');
                    assetName = assetName[..match.Index];
                    len = match.Length - 1;
                }

                assetName += $"{separator}{candidateNumber.ToString($"D{len}")}";
            }

            assetID = Functions.BKDRHash(assetName + suffix);
        }

        return assetName + suffix;
    }

    public static IEnumerable<(Asset asset, int layerIndex)> DuplicateAssets(
        IEnumerable<Asset> assets,
        Platform platform,
        Func<uint, int> getLayer,
        Func<uint, bool> exists,
        bool updateReferences)
    {
        var result = new List<(Asset asset, int layerIndex)>();
        Dictionary<uint, uint> references = new Dictionary<uint, uint>();

        foreach (var asset in assets)
        {
            Section_AHDR AHDR = asset.BuildAHDR(platform.Endianness());

            var previousAssetID = AHDR.assetID;

            var assetName = GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, false, false, exists);
            if (assetName != AHDR.ADBG.assetName || AHDR.assetID == 0 || string.IsNullOrEmpty(AHDR.ADBG.assetName))
            {
                AHDR.assetID = Functions.BKDRHash(assetName);
                AHDR.ADBG.assetName = assetName;
            }

            if (previousAssetID != AHDR.assetID)
                references.Add(previousAssetID, AHDR.assetID);

            result.Add((ConvertHeaderToAsset(AHDR, asset.game, platform, false, out _), getLayer(previousAssetID)));
        }

        if (updateReferences)
            foreach (var (asset, _) in result)
                ReplaceReferencesOn(asset, references);

        return result;
    }

    public delegate bool TryGetAsset(uint u, out Asset asset);

    public static IEnumerable<Asset> GetAssetsAndHierarchy(params IEnumerable<Asset> assets)
    {
        List<Asset> result = [.. assets];

        for (int i = 0; i < result.Count; i++)
        {
            var hierarchy = result[i].GetHierarchy();
            foreach (var u in hierarchy)
                if (u != 0 && !result.Any(a => a.assetID == u) && Program.ArchiveManager.ArchiveEditorContainingAsset(u) is ArchiveEditor ae)
                    result.Add(ae.archive.GetAsset(u));
        }

        return result;
    }

    public static List<AssetClipboard> AssetsToClipboard(IEnumerable<Asset> assets, Platform sessionPlatform, Func<uint, byte[], byte[]> getSoundData)
    {
        var result = new List<AssetClipboard>();
        foreach (Asset asset in assets)
        {
            var AHDR = asset.BuildAHDR(sessionPlatform.Endianness());

            if (asset is AssetSound)
                try
                {
                    AHDR.data = getSoundData(AHDR.assetID, AHDR.data);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + " The asset will be copied as it is.");
                }

            result.Add(new AssetClipboard(asset.game, sessionPlatform, AHDR, asset is AssetJSP_INFO jspInfo ? jspInfo.JSP_AssetIDs : null, GetRWVersion(AHDR)));
        }
        return result;
    }

    public static List<Asset> ClipboardToAssets(
        Platform sessionPlatform,
        Game sessionGame,
        bool updateRefs,
        bool replaceExisting,
        Func<uint, bool> exists,
        bool isTemplate,
        List<AssetClipboard> clipboard)
    {
        var result = new List<Asset>();

        Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();
        RWVersion? targetVersion = null;
        bool rememberForAll = false;

        for (int i = 0; i < clipboard.Count; i++)
        {
            Section_AHDR AHDR = clipboard[i].AHDR.ToAHDR();

            if (IsRenderWareStream(AHDR.assetType) && Dialogs.ChangeRWVersion.IsVersionMismatch(sessionPlatform, sessionGame, AHDR.assetType, clipboard[i].rwVersion))
            {
                AHDR.data = Dialogs.ChangeRWVersion.ChangeVersion(AHDR, ref targetVersion, ref rememberForAll);
            }

            uint previousAssetID = AHDR.assetID;

            if (exists(AHDR.assetID))
            {
                if (isTemplate || !replaceExisting)
                {
                    AHDR.ADBG.assetName = GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, false, false, exists);
                    AHDR.assetID = new AssetID(AHDR.ADBG.assetName);

                    referenceUpdate.Add(previousAssetID, AHDR.assetID);
                }
            }

            var asset = ConvertHeaderToAsset(AHDR, clipboard[i].game, clipboard[i].platform, false, out _);

            if (asset is AssetJSP_INFO jspInfo)
                jspInfo.JSP_AssetIDs = clipboard[i].jspExtraInfo;

            result.Add(asset);
        }

        foreach (var asset in result)
        {
            if (updateRefs || isTemplate)
                ReplaceReferencesOn(asset, referenceUpdate);
        }

        return result;
    }

    public static IEnumerable<string> GetUnconvertableAssets(IEnumerable<Asset> assets)
    {
        var result = new List<string>();
        foreach (var asset in assets)
        {
            if (asset.assetType == AssetType.SoundInfo ||
                asset is AssetSound ||
                asset is AssetGeneric ||
                asset is AssetGenericBase ||
                asset is DynaGeneric ||
                asset is AssetJSP ||
                asset is AssetJSP_INFO ||
                (asset is AssetMODL modl && modl.IsNativeData) ||
                asset is AssetRWTX)
                result.Add($"[{AssetTypeContainer.AssetTypeToString(asset.assetType)}] {asset.assetName}");
        }
        return result;
    }

    public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
    {
        if (assetType.IsDyna())
            return AHDRFlags.SOURCE_VIRTUAL;
        return assetType switch
        {
            AssetType.AnimationList or AssetType.Boulder or AssetType.Button or AssetType.Camera or AssetType.Counter or AssetType.CollisionTable or
            AssetType.Conditional or AssetType.CutsceneManager or AssetType.CutsceneTableOfContents or AssetType.Dispatcher or AssetType.DiscoFloor or
            AssetType.DestructibleObject or AssetType.ElectricArcGenerator or AssetType.Environment or AssetType.Fog or AssetType.Hangable or
            AssetType.Group or AssetType.JawDataTable or AssetType.LevelOfDetailTable or AssetType.SurfaceMapper or AssetType.ModelInfo or
            AssetType.Marker or AssetType.MovePoint or AssetType.ParticleEmitter or AssetType.ParticleProperties or AssetType.ParticleSystem or
            AssetType.Pendulum or AssetType.PickupTable or AssetType.PipeInfoTable or AssetType.Pickup or AssetType.Platform or AssetType.Player or
            AssetType.Portal or AssetType.SFX or AssetType.ShadowTable or AssetType.Shrapnel or AssetType.SimpleObject or AssetType.SoundInfo or
            AssetType.Surface or AssetType.Text or AssetType.Timer or AssetType.Trigger or AssetType.UserInterface or AssetType.UserInterfaceFont or
            AssetType.NPC or AssetType.NPCProperties => AHDRFlags.SOURCE_VIRTUAL,

            AssetType.Cutscene or AssetType.Flythrough or AssetType.RawImage => AHDRFlags.SOURCE_FILE,

            AssetType.Animation or AssetType.Credits or AssetType.Sound or AssetType.SoundStream => AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM,

            AssetType.BSP or AssetType.Model => AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM,

            AssetType.AnimationTable or AssetType.JSP or AssetType.JSPInfo or AssetType.Texture or AssetType.TextureStream => AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM,

            AssetType.LightKit => AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM,
            _ => 0,
        };
    }

    public static IEnumerable<Asset> WhoTargets(uint assetID)
    {
        foreach (ArchiveEditor archiveEditor in Program.ArchiveEditors)
            foreach (var item in archiveEditor.archive?.FindWhoTargets(assetID) ?? [])
                yield return item;
    }

    /// <summary>
    /// Returns boolean if <see cref="AssetType"/> is a RenderWare binary
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsRenderWareStream(AssetType type)
    {
        if (type == AssetType.Model || type == AssetType.Texture || type == AssetType.TextureStream ||
            type == AssetType.BSP || type == AssetType.JSP || type == AssetType.JSPInfo)
            return true;
        return false;
    }

    /// <summary>
    /// Returns the version from a RenderWare binary.
    /// </summary>
    /// <param name="ahdr"></param>
    /// <returns>RenderWare version if successful. Undefined if no data or invalid version. null if not a renderware asset</returns>
    private static RWVersion? GetRWVersion(Section_AHDR ahdr)
    {
        if (IsRenderWareStream(ahdr.assetType))
        {
            if (ahdr.data.Length > 12)
            {
                RWVersion ver = new RWVersion(BitConverter.ToInt32(ahdr.data.Skip(8).Take(4).ToArray()));
                return RWVersion.IsValidRange(ver) ? ver : RWVersion.Undefined;
            }
            return RWVersion.Undefined;
        }
        return null;
    }

}
