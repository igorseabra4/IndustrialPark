using HipHopFile;
using IndustrialPark.ArchiveEditors;
using IndustrialPark.Assets;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IndustrialPark.Archives;

public static class AssetService
{
    public static Asset ConvertHeaderToAsset(Section_AHDR AHDR, bool testBuild, out AssetError error)
    {
        error = new AssetError() { reason = ErrorReason.NoError, assetType = AHDR.assetType, assetID = AHDR.assetID, assetName = AHDR.ADBG.assetName };

        Asset newAsset;
        try
        {
            newAsset = ConvertHeaderToAsset(AHDR, ref error);
        }
        catch (Exception ex)
        {
            error.reason = ErrorReason.ParseException;
            error.exception = ex.Message;
            newAsset = new AssetGeneric(AHDR);
        }

        // test if IP can build the same asset it read
        if (testBuild && !newAsset.SkipBuildTesting && error.reason == ErrorReason.NoError)
        {
            var endianness = AHDR.Platform.Endianness();
            var built = newAsset.Serialize(endianness);
            var isSndiV2 = newAsset is AssetSNDI_GCN_V2;
            if (!BuiltDataMatches(AHDR.data, built, isSndiV2))
            {
                error.reason = ErrorReason.CannotBuild;
                newAsset = new AssetGeneric(AHDR);
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

        if ((newAsset is AssetGeneric or AssetGenericBase or DynaGeneric) && error.reason == ErrorReason.NoError)
            error.reason = ErrorReason.NotImplemented;

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

    private static Asset ConvertHeaderToAsset(Section_AHDR AHDR, ref AssetError error)
    {
        if (AHDR.data.Length == 0)
        {
            if (AHDR.Platform == Platform.GameCube && AHDR.Game >= Game.Incredibles && (AHDR.assetType is AssetType.Sound or AssetType.SoundStream))
                return new AssetSound(AHDR);

            error.reason = ErrorReason.NoData;
            return new AssetGeneric(AHDR);
        }

        // intention is to, eventually, be able to read asset data directly from the hip stream, avoiding copy of the data into a byte array
        // will require inspecting asset readers and serializers to make uses of BaseStream.Position and BaseStream.Length agnostic to this
        using var reader = new EndianBinaryReader(AHDR.data, AHDR.Platform);

        if (AHDR.IsDyna)
            return ConvertHeaderToDYNA(AHDR, reader);

        return AHDR.assetType switch
        {
            AssetType.Animation when AHDR.Game == Game.Scooby && AHDR.ADBG.assetName.EndsWith("mp") => new AssetGeneric(AHDR),
            AssetType.Animation when AHDR.Game >= Game.ROTU => new AssetANIM_V2(AHDR, reader),
            AssetType.Animation => new AssetANIM_V1(AHDR, reader),
            AssetType.BSP or AssetType.JSP => new AssetJSP(AHDR),
            AssetType.JSPInfo => new AssetJSP_INFO(AHDR, reader),
            AssetType.Model => new AssetMODL(AHDR),
            AssetType.Texture or AssetType.TextureStream => new AssetRWTX(AHDR, reader),
            AssetType.SoundInfo when AHDR.Platform == Platform.GameCube && AHDR.Game < Game.Incredibles => new AssetSNDI_GCN_V1(AHDR, reader),
            AssetType.SoundInfo when AHDR.Platform == Platform.GameCube => new AssetSNDI_GCN_V2(AHDR, reader),
            AssetType.SoundInfo when AHDR.Platform == Platform.Xbox => new AssetSNDI_XBOX(AHDR, reader),
            AssetType.SoundInfo when AHDR.Platform == Platform.PS2 => new AssetSNDI_PS2(AHDR, reader),
            AssetType.Spline => new AssetSPLN(AHDR, reader),
            AssetType.SplinePath => new AssetSPLP(AHDR, reader),
            AssetType.WireframeModel => new AssetWIRE(AHDR, reader),
            AssetType.AnimationList => new AssetALST(AHDR, reader),
            AssetType.AnimationTable => new AssetATBL(AHDR, reader),
            AssetType.AttackTable => new AssetATKT(AHDR, reader),
            AssetType.Boulder => new AssetBOUL(AHDR, reader),
            AssetType.Button => new AssetBUTN(AHDR, reader),
            AssetType.Camera => new AssetCAM(AHDR, reader),
            AssetType.CameraCurve => new AssetCCRV(AHDR, reader),
            AssetType.Counter => new AssetCNTR(AHDR, reader),
            AssetType.CollisionTable => new AssetCOLL(AHDR, reader),
            AssetType.Conditional => new AssetCOND(AHDR, reader),
            AssetType.Credits => new AssetCRDT(AHDR, reader),
            AssetType.Cutscene => new AssetCSN(AHDR, reader),
            AssetType.CutsceneManager => new AssetCSNM(AHDR, reader),
            AssetType.Destructible => new AssetDEST(AHDR, reader),
            AssetType.Dispatcher => new AssetDPAT(AHDR, reader),
            AssetType.DiscoFloor => new AssetDSCO(AHDR, reader),
            AssetType.DestructibleObject => new AssetDSTR(AHDR, reader),
            AssetType.DashTrack => new AssetDTRK(AHDR, reader),
            AssetType.Duplicator => new AssetDUPC(AHDR, reader),
            AssetType.ElectricArcGenerator => new AssetEGEN(AHDR, reader),
            AssetType.Environment => new AssetENV(AHDR, reader),
            AssetType.Flythrough => new AssetFLY(AHDR, reader),
            AssetType.Fog => new AssetFOG(AHDR, reader),
            AssetType.Group => new AssetGRUP(AHDR, reader),
            AssetType.GrassMesh => new AssetGRSM(AHDR, reader),
            AssetType.Gust => new AssetGUST(AHDR, reader),
            AssetType.Hangable => new AssetHANG(AHDR, reader),
            AssetType.JawDataTable => new AssetJAW(AHDR, reader),
            AssetType.Light => new AssetLITE(AHDR, reader),
            AssetType.LightKit => new AssetLKIT(AHDR, reader),
            AssetType.LobMaster => new AssetLOBM(AHDR, reader),
            AssetType.LevelOfDetailTable => new AssetLODT(AHDR, reader),
            AssetType.SurfaceMapper => new AssetMAPR(AHDR, reader),
            AssetType.ModelInfo => new AssetMINF(AHDR, reader),
            AssetType.Marker => new AssetMRKR(AHDR, reader),
            AssetType.MovePoint => new AssetMVPT(AHDR, reader),
            AssetType.Villain => new AssetNPC(AHDR, reader),
            AssetType.OneLiner => new AssetONEL(AHDR, reader),
            AssetType.ParticleEmitter => new AssetPARE(AHDR, reader),
            AssetType.ParticleProperties => new AssetPARP(AHDR, reader),
            AssetType.ParticleSystem => new AssetPARS(AHDR, reader),
            AssetType.Pendulum => new AssetPEND(AHDR, reader),
            AssetType.ProgressScript => new AssetPGRS(AHDR, reader),
            AssetType.PickupTable => new AssetPICK(AHDR, reader),
            AssetType.PipeInfoTable => new AssetPIPT(AHDR, reader),
            AssetType.Pickup => new AssetPKUP(AHDR, reader),
            AssetType.Platform => new AssetPLAT(AHDR, reader),
            AssetType.Player => new AssetPLYR(AHDR, reader),
            AssetType.Portal => new AssetPORT(AHDR, reader),
            AssetType.Projectile => new AssetPRJT(AHDR, reader),
            AssetType.ReactiveAnimation => new AssetRANM(AHDR, reader),
            AssetType.Script => new AssetSCRP(AHDR, reader),
            AssetType.SDFX => new AssetSDFX(AHDR, reader),
            AssetType.SFX => new AssetSFX(AHDR, reader),
            AssetType.SoundGroup => new AssetSGRP(AHDR, reader),
            AssetType.Track or AssetType.SimpleObject => new AssetSIMP(AHDR, reader),
            AssetType.ShadowTable => new AssetSHDW(AHDR, reader),
            AssetType.Shrapnel => new AssetSHRP(AHDR, reader),
            AssetType.Surface => new AssetSURF(AHDR, reader),
            AssetType.Text => new AssetTEXT(AHDR, reader),
            AssetType.Trigger => new AssetTRIG(AHDR, reader),
            AssetType.Timer => new AssetTIMR(AHDR, reader),
            AssetType.ThrowableTable => new AssetTRWT(AHDR, reader),
            AssetType.PickupTypes => new AssetTPIK(AHDR, reader),
            AssetType.UserInterface => new AssetUI(AHDR, reader),
            AssetType.UserInterfaceFont => new AssetUIFT(AHDR, reader),
            AssetType.UserInterfaceMotion => new AssetUIM(AHDR, reader),
            AssetType.NPC => new AssetVIL(AHDR, reader),
            AssetType.NPCProperties => new AssetVILP(AHDR, reader),
            AssetType.Volume => new AssetVOLU(AHDR, reader),
            AssetType.ZipLine => new AssetZLIN(AHDR, reader),
            AssetType.Sound or AssetType.SoundStream => new AssetSound(AHDR),
            AssetType.NavigationMesh or AssetType.SlideProperty or AssetType.SceneSettings => new AssetGenericBase(AHDR, reader),
            AssetType.BinkVideo or AssetType.CutsceneStreamingSound or AssetType.MorphTarget or AssetType.NPCSettings or AssetType.RawImage or AssetType.Subtitles or AssetType.UIFN or AssetType.Null or AssetType.CutsceneTableOfContents or AssetType.SoundInfo => new AssetGeneric(AHDR),
            _ => null,
        };
        throw new Exception($"Unknown asset type ({AHDR.assetType})");
    }

    private static AssetDYNA ConvertHeaderToDYNA(Section_AHDR AHDR, EndianBinaryReader reader)
    {
        reader.BaseStream.Position += 8;
        DynaType type = (DynaType)reader.ReadUInt32();
        reader.BaseStream.Position -= 12;

        return type switch
        {
            DynaType.audio__conversation => new DynaAudioConversation(AHDR, reader),
            DynaType.Checkpoint => new DynaCheckpoint(AHDR, reader),
            DynaType.camera__preset => new DynaCameraPreset(AHDR, reader),
            DynaType.Enemy__SB__BucketOTron => new DynaEnemyBucketOTron(AHDR, reader),
            DynaType.Enemy__SB__CastNCrew => new DynaEnemyCastNCrew(AHDR, reader),
            DynaType.Enemy__SB__Critter => new DynaEnemyCritter(AHDR, reader),
            DynaType.Enemy__SB__Dennis => new DynaEnemyDennis(AHDR, reader),
            DynaType.Enemy__SB__FrogFish => new DynaEnemyFrogFish(AHDR, reader),
            DynaType.Enemy__SB__Mindy => new DynaEnemyMindy(AHDR, reader),
            DynaType.Enemy__SB__Neptune => new DynaEnemyNeptune(AHDR, reader),
            DynaType.Enemy__SB__Standard => new DynaEnemyStandard(AHDR, reader),
            DynaType.Enemy__SB__SupplyCrate => new DynaEnemySupplyCrate(AHDR, reader),
            DynaType.Enemy__SB__Turret => new DynaEnemyTurret(AHDR, reader),
            DynaType.Incredibles__Icon => new DynaIncrediblesIcon(AHDR, reader),
            DynaType.JSPExtraData => new DynaJSPExtraData(AHDR, reader),
            DynaType.SceneProperties => new DynaSceneProperties(AHDR, reader),
            DynaType.effect__Flamethrower => new DynaEffectFlamethrower(AHDR, reader),
            DynaType.effect__LensFlareElement => new DynaEffectLensFlare(AHDR, reader),
            DynaType.effect__LensFlareSource => new DynaEffectLensFlareSource(AHDR, reader),
            DynaType.effect__Lightning => new DynaEffectLightning(AHDR, reader),
            DynaType.effect__Rumble => new DynaEffectRumble(AHDR, reader),
            DynaType.effect__RumbleSphericalEmitter => new DynaEffectRumbleSphere(AHDR, reader),
            DynaType.effect__ScreenFade => new DynaEffectScreenFade(AHDR, reader),
            DynaType.effect__Splash => new DynaEffectSplash(AHDR, reader),
            DynaType.effect__grass => new DynaEffectGrass(AHDR, reader),
            DynaType.effect__smoke_emitter => new DynaEffectSmokeEmitter(AHDR, reader),
            DynaType.effect__spark_emitter => new DynaEffectSparkEmitter(AHDR, reader),
            DynaType.effect__spotlight => new DynaEffectSpotlight(AHDR, reader),
            DynaType.effect__uber_laser => new DynaEffectUberLaser(AHDR, reader),
            DynaType.effect__water_body => new DynaEffectWaterBody(AHDR, reader),
            DynaType.Effect__particle_generator => new DynaEffectParticleGenerator(AHDR, reader),
            DynaType.game_object__BoulderGenerator => new DynaGObjectBoulderGen(AHDR, reader),
            DynaType.game_object__BusStop => new DynaGObjectBusStop(AHDR, reader),
            DynaType.game_object__Camera_Tweak => new DynaGObjectCamTweak(AHDR, reader),
            DynaType.game_object__Flythrough => new DynaGObjectFlythrough(AHDR, reader),
            DynaType.game_object__Grapple => new DynaGObjectGrapple(AHDR, reader),
            DynaType.game_object__Hangable => new DynaGObjectHangable(AHDR, reader),
            DynaType.game_object__IN_Pickup => new DynaGObjectInPickup(AHDR, reader),
            DynaType.game_object__NPCSettings => new DynaGObjectNPCSettings(AHDR, reader),
            DynaType.game_object__RaceTimer => new DynaGObjectRaceTimer(AHDR, reader),
            DynaType.game_object__Ring => new DynaGObjectRing(AHDR, reader),
            DynaType.game_object__RingControl => new DynaGObjectRingControl(AHDR, reader),
            DynaType.game_object__RubbleGenerator => new DynaGObjectRubbleGenerator(AHDR, reader),
            DynaType.game_object__Taxi => new DynaGObjectTaxi(AHDR, reader),
            DynaType.game_object__Teleport => new DynaGObjectTeleport(AHDR, reader),
            DynaType.game_object__Turret => new DynaGObjectTurret(AHDR, reader),
            DynaType.game_object__Vent => new DynaGObjectVent(AHDR, reader),
            DynaType.game_object__VentType => new DynaGObjectVentType(AHDR, reader),
            DynaType.game_object__bungee_drop => new DynaGObjectBungeeDrop(AHDR, reader),
            DynaType.game_object__bungee_hook => new DynaGObjectBungeeHook(AHDR, reader),
            DynaType.game_object__camera_param_asset => new DynaGObjectCameraParamAsset(AHDR, reader),
            DynaType.game_object__dash_camera_spline => new DynaGObjectDashCameraSpline(AHDR, reader),
            DynaType.game_object__flame_emitter => new DynaGObjectFlameEmitter(AHDR, reader),
            DynaType.game_object__laser_beam => new DynaGObjectLaserBeam(AHDR, reader),
            DynaType.game_object__talk_box => new DynaGObjectTalkBox(AHDR, reader),
            DynaType.game_object__task_box => new DynaGObjectTaskBox(AHDR, reader),
            DynaType.game_object__text_box => new DynaGObjectTextBox(AHDR, reader),
            DynaType.game_object__train_car => new DynaGObjectTrainCar(AHDR, reader),
            DynaType.game_object__train_junction => new DynaGObjectTrainJunction(AHDR, reader),
            DynaType.hud__image => new DynaHudImage(AHDR, reader),
            DynaType.hud__meter__font => new DynaHudMeterFont(AHDR, reader),
            DynaType.hud__meter__unit => new DynaHudMeterUnit(AHDR, reader),
            DynaType.hud__model => new DynaHudModel(AHDR, reader),
            DynaType.hud__text => new DynaHudText(AHDR, reader),
            DynaType.interaction__Launch => new DynaInteractionLaunch(AHDR, reader),
            DynaType.interaction__Lift => new DynaInteractionLift(AHDR, reader),
            DynaType.interaction__Turn => new DynaInteractionTurn(AHDR, reader),
            DynaType.logic__reference => new DynaLogicReference(AHDR, reader),
            DynaType.logic__FunctionGenerator => new DynaLogicFunctionGenerator(AHDR, reader),
            DynaType.npc__group => new DynaNPCGroup(AHDR, reader),
            DynaType.pointer => new DynaPointer(AHDR, reader),
            DynaType.ui__box => new DynaUIBox(AHDR, reader),
            DynaType.ui__controller => new DynaUIController(AHDR, reader),
            DynaType.ui__image => new DynaUIImage(AHDR, reader),
            DynaType.ui__model => new DynaUIModel(AHDR, reader),
            DynaType.ui__text => new DynaUIText(AHDR, reader),
            DynaType.ui__text__userstring => new DynaUITextUserString(AHDR, reader),
            DynaType.Interest_Pointer => new DynaInterestPointer(AHDR, reader),
            DynaType.camera__binary_poi => new DynaCameraBinary(AHDR, reader),
            DynaType.effect__LightEffectFlicker => new DynaEffectLightFlicker(AHDR, reader),
            DynaType.effect__LightEffectStrobe => new DynaEffectLightStrobe(AHDR, reader),
            DynaType.effect__ScreenWarp => new DynaEffectScreenWarp(AHDR, reader),
            DynaType.effect__light => new DynaEffectLight(AHDR, reader),
            DynaType.game_object__bullet_mark => new DynaGObjectBulletMark(AHDR, reader),
            DynaType.game_object__bullet_time => new DynaGObjectBulletTime(AHDR, reader),
            DynaType.game_object__rband_camera_asset => new DynaGObjectCameraRband(AHDR, reader),
            DynaType.npc__CoverPoint => new DynaNPCCoverpoint(AHDR, reader),
            DynaType.npc__NPC_Custom_AV => new DynaNPCCustomAV(AHDR, reader),
            DynaType.AnalogDeflection => new DynaAnalogDeflection(AHDR, reader),
            DynaType.AnalogDirection => new DynaAnalogDirection(AHDR, reader),
            DynaType.camera__transition_time => new DynaCameraTransitionTime(AHDR, reader),
            DynaType.Carrying_CarryableProperty_GenericUseProperty => new DynaCarryablePropertyGeneric(AHDR, reader),
            DynaType.Carrying_CarryableProperty_UsePropertyAttract => new DynaCarryablePropertyAttract(AHDR, reader),
            DynaType.Carrying_CarryableProperty_UsePropertyRepel => new DynaCarryablePropertyRepel(AHDR, reader),
            DynaType.Carrying_CarryableProperty_UsePropertySwipe => new DynaCarryablePropertySwipe(AHDR, reader),
            DynaType.ContextObject_PoleSwing => new DynaCObjectPoleSwing(AHDR, reader),
            DynaType.ContextObject_Springboard => new DynaCObjectSpringBoard(AHDR, reader),
            DynaType.ContextObject_Tightrope => new DynaCObjectTightrope(AHDR, reader),
            DynaType.Enemy__NPC_Gate => new DynaNpcGate(AHDR, reader),
            DynaType.Enemy__NPC_Walls => new DynaNpcWalls(AHDR, reader),
            DynaType.Enemy__RATS__LeftArm => new DynaEnemyRATSLeftArm(AHDR, reader),
            DynaType.Enemy__RATS__RightArm => new DynaEnemyRATSRightArm(AHDR, reader),
            DynaType.Enemy__RATS__Swarm__Bug => new DynaEnemyRATSSwarmBug(AHDR, reader),
            DynaType.Enemy__RATS__Swarm__Owl => new DynaEnemyRATSSwarmOwl(AHDR, reader),
            DynaType.Enemy__RATS__Thief => new DynaEnemyRATSThief(AHDR, reader),
            DynaType.Enemy__RATS__Waiter => new DynaEnemyRATSWaiter(AHDR, reader),
            DynaType.HUD_Compass_Object => new DynaHudCompassObject(AHDR, reader),
            DynaType.HUD_Compass_System => new DynaHudCompassSystem(AHDR, reader),
            DynaType.logic__Mission => new DynaLogicMission(AHDR, reader),
            DynaType.logic__Task => new DynaLogicTask(AHDR, reader),
            DynaType.Pour_Widget => new DynaPourWidget(AHDR, reader),
            DynaType.Twiddler => new DynaTwiddler(AHDR, reader),
            DynaType.Enemy__IN2__Bomber => new DynaEnemyIN2Bomber(AHDR, reader),
            DynaType.Enemy__IN2__BossUnderminerDrill => new DynaEnemyIN2BossUnderminerDrill(AHDR, reader),
            DynaType.Enemy__IN2__BossUnderminerUM => new DynaEnemyIN2BossUnderminerUM(AHDR, reader),
            DynaType.Enemy__IN2__Chicken => new DynaEnemyIN2Chicken(AHDR, reader),
            DynaType.Enemy__IN2__Driller => new DynaEnemyIN2Driller(AHDR, reader),
            DynaType.Enemy__IN2__Enforcer => new DynaEnemyIN2Enforcer(AHDR, reader),
            DynaType.Enemy__IN2__Humanoid => new DynaEnemyIN2Humanoid(AHDR, reader),
            DynaType.Enemy__IN2__Rat => new DynaEnemyIN2Rat(AHDR, reader),
            DynaType.Enemy__IN2__RobotTank => new DynaEnemyIN2RobotTank(AHDR, reader),
            DynaType.Enemy__IN2__Scientist => new DynaEnemyIN2Scientist(AHDR, reader),
            DynaType.Enemy__IN2__Shooter => new DynaEnemyIN2Shooter(AHDR, reader),
            DynaType.effect__RumbleBoxEmitter or DynaType.effect__Waterhose or DynaType.Enemy__SB or DynaType.camera__transition_path or DynaType.effect__BossBrain or DynaType.game_object__FreezableObject or DynaType.Carrying_CarryableObject or DynaType.interaction__IceBridge or DynaType.interaction__SwitchLever or DynaType.Unknown_EBC04E7B => new DynaGeneric(AHDR, type, reader),
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
            Section_AHDR AHDR = asset.BuildAHDR(platform);

            var previousAssetID = AHDR.assetID;

            var assetName = GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, false, false, exists);
            if (assetName != AHDR.ADBG.assetName || AHDR.assetID == 0 || string.IsNullOrEmpty(AHDR.ADBG.assetName))
            {
                AHDR.assetID = Functions.BKDRHash(assetName);
                AHDR.ADBG.assetName = assetName;
            }

            if (previousAssetID != AHDR.assetID)
                references.Add(previousAssetID, AHDR.assetID);

            result.Add((ConvertHeaderToAsset(AHDR, false, out _), getLayer(previousAssetID)));
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
            var AHDR = asset.BuildAHDR(sessionPlatform);

            if (asset is AssetSound)
                try
                {
                    AHDR.data = getSoundData(AHDR.assetID, AHDR.data);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + " The asset will be copied as it is.");
                }

            result.Add(new AssetClipboard(AHDR, asset is AssetJSP_INFO jspInfo ? jspInfo.JSP_AssetIDs : null, GetRWVersion(AHDR)));
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

            if (IsRenderWareStream(AHDR.assetType) && ChangeRWVersion.IsVersionMismatch(sessionPlatform, sessionGame, AHDR.assetType, clipboard[i].rwVersion))
            {
                AHDR.data = ChangeRWVersion.ChangeVersion(AHDR, ref targetVersion, ref rememberForAll);
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

            var asset = ConvertHeaderToAsset(AHDR, false, out _);

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

    public static List<Asset> FindWhoTargets(uint assetID) =>
        [.. Program.ArchiveEditors.SelectMany(ae => ae.archive?.FindWhoTargets(assetID) ?? [])];

    public static bool HasWhoTargets(uint assetID) =>
        Program.ArchiveEditors.Any(ae => ae.archive != null && ae.archive.HasWhoTargets(assetID));

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
