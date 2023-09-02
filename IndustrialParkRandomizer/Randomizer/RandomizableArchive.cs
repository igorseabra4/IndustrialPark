using HipHopFile;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.Randomizer
{
    public class RandomizableArchive : ArchiveEditorFunctions
    {
        protected override Asset TryCreateAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool showMessageBox, ref string error)
        {
            try
            {
                return CreateAsset(AHDR, game, endianness);
            }
            catch
            {
                return new AssetGeneric(AHDR, game, endianness);
            }
        }

        public RandomizableArchive()
        {
            SkipTextureDisplay = true;
        }

        public static Random random;

        public bool Randomize(RandomizerSettings settings)
        {
            if (LevelName == "hb09")
                return RandomizePoliceStation(settings);

            bool shuffled = false;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetTIMR timr && settings.Timers)
                {
                    timr.Time *= random.NextFloat(settings.timerMin, settings.timerMax);
                    shuffled = true;
                }
                else if (a is AssetSURF surf && settings.Texture_Animations)
                    shuffled |= RandomizeSurf(surf, settings.surfMin, settings.surfMax);
                else if (a is AssetFLY fly && settings.disableFlythroughs && fly.Frames.Length > 1)
                {
                    fly.Frames = new FlyFrame[] { fly.Frames[0] };
                    shuffled = true;
                }

            if (settings.Disco_Floors && ContainsAssetWithType(AssetType.DiscoFloor))
                shuffled |= RandomizeDisco();

            if (settings.BoulderSettings && ContainsAssetWithType(AssetType.Boulder))
                shuffled |= RandomizeBoulderSettings(settings);

            if (settings.Pickups && ContainsAssetWithType(AssetType.Pickup))
                shuffled |= RandomizePickupPositions();

            if (settings.Shiny_Object_Gates && game == Game.Scooby && ContainsAssetWithType(AssetType.Pickup))
                shuffled |= RandomizeSnackGates(settings.shinyReqMin, settings.shinyReqMax);

            if (game == Game.BFBB)
            {
                if (settings.Tiki_Types && ContainsAssetWithType(AssetType.NPC))
                {
                    List<NpcType_BFBB> chooseFrom = new List<NpcType_BFBB>();
                    if (settings.TikiProbabilities.WoodenTiki >= 0)
                        chooseFrom.Add(NpcType_BFBB.tiki_wooden_bind);
                    if (settings.TikiProbabilities.FloatingTiki >= 0)
                        chooseFrom.Add(NpcType_BFBB.tiki_lovey_dovey_bind);
                    if (settings.TikiProbabilities.ThunderTiki >= 0)
                        chooseFrom.Add(NpcType_BFBB.tiki_thunder_bind);
                    if (settings.TikiProbabilities.ShhhTiki >= 0)
                        chooseFrom.Add(NpcType_BFBB.tiki_shhhh_bind);
                    if (settings.TikiProbabilities.StoneTiki >= 0)
                        chooseFrom.Add(NpcType_BFBB.tiki_stone_bind);

                    List<NpcType_BFBB> setTo = new List<NpcType_BFBB>();
                    for (int i = 0; i < settings.TikiProbabilities.WoodenTiki; i++)
                        setTo.Add(NpcType_BFBB.tiki_wooden_bind);
                    for (int i = 0; i < settings.TikiProbabilities.FloatingTiki; i++)
                        setTo.Add(NpcType_BFBB.tiki_lovey_dovey_bind);
                    for (int i = 0; i < settings.TikiProbabilities.ThunderTiki; i++)
                        setTo.Add(NpcType_BFBB.tiki_thunder_bind);
                    for (int i = 0; i < settings.TikiProbabilities.ShhhTiki; i++)
                        setTo.Add(NpcType_BFBB.tiki_shhhh_bind);
                    for (int i = 0; i < settings.TikiProbabilities.StoneTiki; i++)
                        setTo.Add(NpcType_BFBB.tiki_stone_bind);

                    if (LevelName == "kf04")
                        chooseFrom.Remove(NpcType_BFBB.tiki_stone_bind);

                    shuffled |= ShuffleVilTypes(chooseFrom, setTo, settings.Tiki_Models, settings.Tiki_Allow_Any_Type, false);
                }
            }
            else if (game == Game.Incredibles)
            {
                if (settings.Tiki_Types && ContainsAssetWithType(AssetType.Crate))
                {
                    List<EnemySupplyCrateType> chooseFrom = new List<EnemySupplyCrateType>();
                    if (settings.TikiProbabilities.WoodenTiki >= 0)
                        chooseFrom.Add(EnemySupplyCrateType.crate_wood_bind);
                    if (settings.TikiProbabilities.FloatingTiki >= 0)
                        chooseFrom.Add(EnemySupplyCrateType.crate_hover_bind);
                    if (settings.TikiProbabilities.ThunderTiki >= 0)
                        chooseFrom.Add(EnemySupplyCrateType.crate_explode_bind);
                    if (settings.TikiProbabilities.ShhhTiki >= 0)
                        chooseFrom.Add(EnemySupplyCrateType.crate_shrink_bind);
                    if (settings.TikiProbabilities.StoneTiki >= 0)
                        chooseFrom.Add(EnemySupplyCrateType.crate_steel_bind);

                    List<EnemySupplyCrateType> setTo = new List<EnemySupplyCrateType>();
                    for (int i = 0; i < settings.TikiProbabilities.WoodenTiki; i++)
                        setTo.Add(EnemySupplyCrateType.crate_wood_bind);
                    for (int i = 0; i < settings.TikiProbabilities.FloatingTiki; i++)
                        setTo.Add(EnemySupplyCrateType.crate_wood_bind);
                    for (int i = 0; i < settings.TikiProbabilities.ThunderTiki; i++)
                        setTo.Add(EnemySupplyCrateType.crate_explode_bind);
                    for (int i = 0; i < settings.TikiProbabilities.ShhhTiki; i++)
                        setTo.Add(EnemySupplyCrateType.crate_shrink_bind);
                    for (int i = 0; i < settings.TikiProbabilities.StoneTiki; i++)
                        setTo.Add(EnemySupplyCrateType.crate_steel_bind);

                    shuffled |= ShuffleBoxDynaTypes(chooseFrom, setTo);
                }
            }

            if (game == Game.BFBB)
            {
                if (settings.Enemy_Types && ContainsAssetWithType(AssetType.NPC))
                {
                    List<NpcType_BFBB> chooseFrom = new List<NpcType_BFBB>(16);
                    if (settings.EnemyProbabilities.Fodder >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_0a_fodder_bind);
                    if (settings.EnemyProbabilities.Hammer >= 0)
                        chooseFrom.Add(NpcType_BFBB.ham_bind);
                    if (settings.EnemyProbabilities.Tartar >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_tar_bind);
                    if (settings.EnemyProbabilities.GLove >= 0)
                        chooseFrom.Add(NpcType_BFBB.g_love_bind);
                    if (settings.EnemyProbabilities.Chuck >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_chuck_bind);
                    if (settings.EnemyProbabilities.Monsoon >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_4a_monsoon_bind);
                    if (settings.EnemyProbabilities.Sleepytime >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_sleepytime_bind);
                    if (settings.EnemyProbabilities.Arf >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_arf_bind);
                    if (settings.EnemyProbabilities.Tubelets >= 0)
                        chooseFrom.Add(NpcType_BFBB.tubelet_bind);
                    if (settings.EnemyProbabilities.Slick >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_9a_bind);
                    if (settings.EnemyProbabilities.BombBot >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_0a_bomb_bind);
                    if (settings.EnemyProbabilities.BzztBot >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_0a_bzzt_bind);
                    if (settings.EnemyProbabilities.ChompBot >= 0)
                        chooseFrom.Add(NpcType_BFBB.robot_0a_chomper_bind);

                    List<NpcType_BFBB> setTo = new List<NpcType_BFBB>();
                    for (int i = 0; i < settings.EnemyProbabilities.Fodder; i++)
                        setTo.Add(NpcType_BFBB.robot_0a_fodder_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Hammer; i++)
                        setTo.Add(NpcType_BFBB.ham_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Tartar; i++)
                        setTo.Add(NpcType_BFBB.robot_tar_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.GLove; i++)
                        setTo.Add(NpcType_BFBB.g_love_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Chuck; i++)
                        setTo.Add(NpcType_BFBB.robot_chuck_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Monsoon; i++)
                        setTo.Add(NpcType_BFBB.robot_4a_monsoon_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Sleepytime; i++)
                        setTo.Add(NpcType_BFBB.robot_sleepytime_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Arf; i++)
                        setTo.Add(NpcType_BFBB.robot_arf_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Tubelets; i++)
                        setTo.Add(NpcType_BFBB.tubelet_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.Slick; i++)
                        setTo.Add(NpcType_BFBB.robot_9a_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.BombBot; i++)
                        setTo.Add(NpcType_BFBB.robot_0a_bomb_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.BzztBot; i++)
                        setTo.Add(NpcType_BFBB.robot_0a_bzzt_bind);
                    for (int i = 0; i < settings.EnemyProbabilities.ChompBot; i++)
                        setTo.Add(NpcType_BFBB.robot_0a_chomper_bind);

                    shuffled |= ShuffleVilTypes(chooseFrom, setTo, false, settings.Enemies_Allow_Any_Type, true);
                }
            }
            else if (game == Game.Incredibles)
            {
                if (settings.Enemy_Types && ContainsAssetWithType(AssetType.Enemy))
                {
                    List<EnemyStandardType> chooseFrom = new List<EnemyStandardType>(24);

                    if (settings.EnemyProbabilitiesMovie.Flinger_Desert >= 0)
                        chooseFrom.Add(EnemyStandardType.flinger_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Flinger_Desert >= 0)
                        chooseFrom.Add(EnemyStandardType.flinger_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Flinger_Trench >= 0)
                        chooseFrom.Add(EnemyStandardType.flinger_v2_bind);
                    if (settings.EnemyProbabilitiesMovie.Flinger_Junk >= 0)
                        chooseFrom.Add(EnemyStandardType.flinger_v3_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Desert >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_de_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Goofy >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_gg_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Junk >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_jk_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Plankton >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_pt_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Trench >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_tr_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Thug >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_tt_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Arena1 >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Arena2 >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_v2_bind);
                    if (settings.EnemyProbabilitiesMovie.Fogger_Arena3 >= 0)
                        chooseFrom.Add(EnemyStandardType.fogger_v3_bind);
                    if (settings.EnemyProbabilitiesMovie.Mervyn >= 0)
                        chooseFrom.Add(EnemyStandardType.mervyn_v3_bind);
                    if (settings.EnemyProbabilitiesMovie.Minimerv >= 0)
                        chooseFrom.Add(EnemyStandardType.minimerv_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Popper_Trench >= 0)
                        chooseFrom.Add(EnemyStandardType.popper_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Popper_Plankton >= 0)
                        chooseFrom.Add(EnemyStandardType.popper_v3_bind);
                    if (settings.EnemyProbabilitiesMovie.Slammer_Goofy >= 0)
                        chooseFrom.Add(EnemyStandardType.slammer_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Slammer_Desert >= 0)
                        chooseFrom.Add(EnemyStandardType.slammer_des_bind);
                    if (settings.EnemyProbabilitiesMovie.Slammer_Thug >= 0)
                        chooseFrom.Add(EnemyStandardType.slammer_v3_bind);
                    if (settings.EnemyProbabilitiesMovie.Spinner_Thug >= 0)
                        chooseFrom.Add(EnemyStandardType.spinner_v1_bind);
                    if (settings.EnemyProbabilitiesMovie.Spinner_Junk >= 0)
                        chooseFrom.Add(EnemyStandardType.spinner_v2_bind);
                    if (settings.EnemyProbabilitiesMovie.Spinner_Plankton >= 0)
                        chooseFrom.Add(EnemyStandardType.spinner_v3_bind);

                    List<EnemyStandardType> setTo = new List<EnemyStandardType>();
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Flinger_Desert; i++)
                        setTo.Add(EnemyStandardType.flinger_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Flinger_Trench; i++)
                        setTo.Add(EnemyStandardType.flinger_v2_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Flinger_Junk; i++)
                        setTo.Add(EnemyStandardType.flinger_v3_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Desert; i++)
                        setTo.Add(EnemyStandardType.fogger_de_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Goofy; i++)
                        setTo.Add(EnemyStandardType.fogger_gg_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Junk; i++)
                        setTo.Add(EnemyStandardType.fogger_jk_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Plankton; i++)
                        setTo.Add(EnemyStandardType.fogger_pt_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Trench; i++)
                        setTo.Add(EnemyStandardType.fogger_tr_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Thug; i++)
                        setTo.Add(EnemyStandardType.fogger_tt_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Arena1; i++)
                        setTo.Add(EnemyStandardType.fogger_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Arena2; i++)
                        setTo.Add(EnemyStandardType.fogger_v2_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Fogger_Arena3; i++)
                        setTo.Add(EnemyStandardType.fogger_v3_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Mervyn; i++)
                        setTo.Add(EnemyStandardType.mervyn_v3_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Minimerv; i++)
                        setTo.Add(EnemyStandardType.minimerv_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Popper_Trench; i++)
                        setTo.Add(EnemyStandardType.popper_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Popper_Plankton; i++)
                        setTo.Add(EnemyStandardType.popper_v3_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Slammer_Goofy; i++)
                        setTo.Add(EnemyStandardType.slammer_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Slammer_Desert; i++)
                        setTo.Add(EnemyStandardType.slammer_des_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Slammer_Thug; i++)
                        setTo.Add(EnemyStandardType.slammer_v3_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Spinner_Thug; i++)
                        setTo.Add(EnemyStandardType.spinner_v1_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Spinner_Junk; i++)
                        setTo.Add(EnemyStandardType.spinner_v2_bind);
                    for (int i = 0; i < settings.EnemyProbabilitiesMovie.Spinner_Plankton; i++)
                        setTo.Add(EnemyStandardType.spinner_v3_bind);

                    shuffled |= ShuffleEnemyDynaTypes(chooseFrom, setTo, settings.Enemies_Allow_Any_Type);
                }
            }

            if (settings.MovePoint_Radius && ContainsAssetWithType(AssetType.MovePoint))
                shuffled |= RandomizeMovePointRadius(settings);

            if (settings.Markers
                && !new string[] { "hb02", "b101", "b201", "b302", "b303" }.Contains(LevelName))
                shuffled |= ShuffleMRKRPositions(
                    settings.allMenuWarpsHB01,
                    settings.Pointer_Positions,
                    settings.Player_Start,
                    settings.Bus_Stop_Trigger_Positions,
                    settings.Teleport_Box_Positions,
                    settings.Taxi_Trigger_Positions);

            if (settings.Cameras && ContainsAssetWithType(AssetType.Camera))
                shuffled |= ShuffleCameras();

            bool shinyNumbers = false;
            bool spatNumbers = false;

            if (game == Game.BFBB && settings.Shiny_Object_Gates && ContainsAssetWithType(AssetType.Conditional))
                shuffled |= ShuffleShinyGates(settings, out shinyNumbers);

            if (game == Game.BFBB)
                if ((settings.spatReqChum != 75 || settings.Spatula_Gates) && ContainsAssetWithType(AssetType.Conditional))
                    shuffled |= ShuffleSpatulaGates(settings.Spatula_Gates, settings, out spatNumbers);

            if (game == Game.Incredibles && settings.CombatArenaCounts && ContainsAssetWithType(AssetType.Counter))
                shuffled |= ShuffleCombatArenas(settings.combatMin, settings.combatMax);

            if (settings.PlatformSpeed && ContainsAssetWithType(AssetType.Platform))
                shuffled |= ShufflePlatSpeeds(settings);

            if (shinyNumbers || spatNumbers)
                ImportNumbers();

            if (settings.Scale_Of_Things)
                shuffled |= ShuffleScales(settings);

            if (settings.RingSizes && ContainsAssetWithType(AssetType.Ring))
                shuffled |= ShuffleRingScales(settings);

            if (settings.FloatingBlockChallenge && game == Game.Incredibles && ContainsAssetWithType(AssetType.Platform))
                shuffled |= ShuffleFloatingBlocks();

            if (settings.Colors)
                shuffled |=
                    ShufflePlaceableColors(settings.brightColors, settings.strongColors) |
                    ShufflePlaceableDynaColors(settings.brightColors, settings.strongColors) |
                    ShuffleLevelModelColors(settings.brightColors, settings.strongColors, settings.VertexColors);

            if (game == Game.BFBB && settings.PlayerCharacters && ContainsAssetWithType(AssetType.BusStop))
                shuffled |= ShuffleBusStops();

            if (settings.Music)
                shuffled |= RandomizePlaylistLocal();

            if (game == Game.BFBB && settings.disableCutscenes)
                shuffled |= DisableCutscenes();

            if (game == Game.Incredibles && settings.disableCutscenes)
                shuffled |= DisableCutscenesMovie();

            if (settings.openTeleportBoxes && ContainsAssetWithType(AssetType.TeleportBox))
                shuffled |= OpenTeleportBoxes();

            if (settings.invisibleLevel && ContainsAssetWithType(AssetType.JSP))
                shuffled |= MakeLevelInvisible();

            if (settings.invisibleObjects && ContainsAssetWithType(AssetType.SimpleObject))
                shuffled |= MakeObjectsInvisible();

            if (settings.Set_Scale)
            {
                shuffled |= true;
                ApplyScale(new Vector3(settings.scaleFactorX, settings.scaleFactorY, settings.scaleFactorZ));
            }

            if (settings.Textures && ContainsAssetWithType(AssetType.Texture))
                shuffled |= RandomizeTextures(settings.Textures_Special);

            if (settings.Sounds && ContainsAssetWithType(AssetType.SoundInfo))
                shuffled |= RandomizeSounds(settings.Mix_Sound_Types);

            return shuffled;
        }

        private static bool RandomizeSurf(AssetSURF surf, float surfMin, float surfMax)
        {
            if (surf.zSurfUVFX.TransSpeed_X == 0)
                surf.zSurfUVFX.TransSpeed_X = random.NextFloat(surfMin, surfMax);
            else
                surf.zSurfUVFX.TransSpeed_X *= random.NextFloat(surfMin, surfMax);

            if (surf.zSurfUVFX.TransSpeed_Y == 0)
                surf.zSurfUVFX.TransSpeed_Y = random.NextFloat(surfMin, surfMax);
            else
                surf.zSurfUVFX.TransSpeed_Y *= random.NextFloat(surfMin, surfMax);

            return true;
        }

        private bool OpenTeleportBoxes()
        {
            List<uint> dynaTeleportAssetIDs = new List<uint>();

            foreach (AssetDYNA dyna in (from asset in assetDictionary.Values
                                        where asset is AssetDYNA dyna && dyna.Type == DynaType.game_object__Teleport
                                        select asset).Cast<AssetDYNA>())
            {
                dynaTeleportAssetIDs.Add(dyna.assetID);
            }

            if (dynaTeleportAssetIDs.Count == 0)
                return false;

            AssetDPAT dispatcher = (AssetDPAT)PlaceTemplate("IP_TELEBOX", AssetTemplate.Dispatcher);

            var links = new List<Link>();
            foreach (uint u in dynaTeleportAssetIDs)
                links.Add(new Link(game)
                {
                    EventReceiveID = (ushort)(ushort)EventBFBB.ScenePrepare,
                    EventSendID = (ushort)EventBFBB.OpenTeleportBox,
                    TargetAsset = u,
                });

            dispatcher.Links = links.ToArray();

            return true;
        }

        private bool RandomizeTextures(bool hud)
        {
            var assets = (from asset in assetDictionary.Values
                          where asset.assetType == AssetType.Texture
                          && ((hud && asset.assetName.ToLower().Contains("rw3")) || (!hud))
                          select (AssetWithData)asset).ToList();

            if (assets.Count < 2)
                return false;

            var datas = (from asset in assets select asset.Data).ToList();

            foreach (var a in assets)
            {
                int value = random.Next(0, datas.Count());
                a.Data = datas[value];
                datas.RemoveAt(value);
            }

            return true;
        }

        private bool RandomizeDisco()
        {
            var assets = (from asset in assetDictionary.Values where asset.assetType == AssetType.DiscoFloor select (AssetDSCO)asset).ToList();

            foreach (var dsco in assets)
            {
                var patterns = dsco.Patterns;
                for (int i = 0; i < patterns.Length; i++)
                    patterns[i].Pattern = GenerateRandomDiscoPattern(dsco.AmountOfTiles);
            }

            return true;
        }

        private DiscoTileState[] GenerateRandomDiscoPattern(int amountOfTiles)
        {
            var result = new DiscoTileState[amountOfTiles];
            for (int i = 0; i < result.Length; i++)
                result[i] = (DiscoTileState)random.Next(0, 3);
            return result;
        }

        private bool RandomizePickupPositions()
        {
            var assets = (from asset in assetDictionary.Values where asset.assetType == AssetType.Pickup select (AssetPKUP)asset).ToList();

            if (assets.Count < 2)
                return false;

            if (game == Game.BFBB)
                VerifyPickupsBFBB(ref assets);
            else if (game == Game.Incredibles)
                VerifyPickupsMovie(ref assets);
            else if (game == Game.Scooby)
                VerifyPickupsScooby(ref assets);

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            foreach (AssetPKUP a in assets)
            {
                int value = random.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);

                if (game == Game.Incredibles)
                    RemoveFromVolume(a.assetID);
            }

            return true;
        }

        private bool RandomizeSnackGates(float snackGateMin, float snackGateMax)
        {
            var assets = (from asset in assetDictionary.Values where asset is AssetPKUP pkup && pkup.PickReferenceID.Equals(0x25E4C286) select (AssetPKUP)asset).ToList();

            if (assets.Count < 2)
                return false;

            foreach (var a in assets)
                a.PickupValue = (short)(a.PickupValue * random.NextFloat(snackGateMin, snackGateMax));

            return true;
        }

        private void VerifyPickupsBFBB(ref List<AssetPKUP> assets)
        {
            switch (LevelName)
            {
                case "hb01":
                    RemoveAssetsFrom(ref assets, "GREENSHINY_PICKUP_02", "GREENSHINY_PICKUP_03", "GREENSHINY_PICKUP_18", "GREENSHINY_PICKUP_20", "YELLOWSHINY_PICKUP_21");
                    RemoveAssetsFromContains(ref assets, "GS_MRKRABS_PICKUP", "GS_PATRICK_PICKUP");
                    break;
                case "hb02":
                    for (int i = 0; i < assets.Count; i++)
                        if (assets[i].assetName.Contains("RED"))
                        {
                            int shinyNum = Convert.ToInt32(assets[i].assetName.Split('_')[2]);
                            if (shinyNum >= 32 && shinyNum <= 47 && shinyNum != 42)
                            {
                                assets.RemoveAt(i);
                                i--;
                            }
                        }
                    break;
                case "bb01":
                    RemoveAssetsFrom(ref assets, "SHINY_RED_018", "SHINY_RED_019");
                    break;
                case "gl01":
                    RemoveAssetsFrom(ref assets, "SHINY_YELLOW_004");
                    if (ContainsAsset(new AssetID("GOLDENSPATULA_04")))
                        ((AssetPKUP)GetFromAssetID(new AssetID("GOLDENSPATULA_04"))).PositionY += 2f;
                    break;
                case "gl03":
                    if (ContainsAsset(0x0B48E8AC))
                    {
                        AssetDYNA dyna = (AssetDYNA)GetFromAssetID(0x0B48E8AC);
                        dyna.Links = new Link[0];

                        if (ContainsAsset(0xF70F6FEE))
                        {
                            ((AssetPKUP)GetFromAssetID(0xF70F6FEE)).PickupFlags = EPickupFlags.InitiallyVisible;
                            ((AssetPKUP)GetFromAssetID(0xF70F6FEE)).VisibilityFlags.FlagValueByte = 1;
                        }
                    }
                    break;
                case "sm02":
                    RemoveAssetsFrom(ref assets, "PU_SHINY_RED", "PU_SHINY_GREEN", "PU_SHINY_YELLOW", "PU_SHINY_BLUE", "PU_SHINY_PURPLE");
                    break;
                case "jf01":
                case "bc01":
                case "rb03":
                case "sm01":
                    for (int i = 0; i < assets.Count; i++)
                    {
                        foreach (Link link in assets[i].Links)
                            if (link.EventSendID == (ushort)EventBFBB.Mount)
                            {
                                assets.RemoveAt(i);
                                i--;
                                break;
                            }
                    }
                    break;
            }
        }

        private void RemoveAssetsFrom(ref List<AssetPKUP> assets, params string[] names)
        {
            for (int i = 0; i < assets.Count; i++)
                foreach (string s in names)
                    if (assets[i].assetName.Equals(s))
                    {
                        assets.RemoveAt(i--);
                        break;
                    }
        }

        private void RemoveAssetsFromContains(ref List<AssetPKUP> assets, params string[] names)
        {
            for (int i = 0; i < assets.Count; i++)
                foreach (string s in names)
                    if (assets[i].assetName.Contains(s))
                    {
                        assets.RemoveAt(i--);
                        break;
                    }
        }

        private void VerifyPickupsMovie(ref List<AssetPKUP> assets)
        {
            switch (LevelName)
            {
                case "am04":
                case "b101":
                case "b201":
                case "b301":
                case "b401":
                case "bb01":
                case "de02":
                case "fb01":
                case "fb02":
                case "fb03":
                case "gg02":
                case "pt02":
                case "pt03":
                case "sc02":
                case "tr01":
                case "tt02":
                    for (int i = 0; i < assets.Count; i++)
                        if (assets[i].PickReferenceID.Equals(0x60F808B7))
                            assets.RemoveAt(i--);
                    break;
            }
        }

        private void VerifyPickupsScooby(ref List<AssetPKUP> assets)
        {
            switch (LevelName)
            {
                case "g009":
                    RemoveAssetsFromContains(ref assets, "FLOAT");
                    break;
                case "o008":
                    RemoveAssetsFromContains(ref assets, "BOOTS");
                    break;
                case "w028":
                    RemoveAssetsFromContains(ref assets, "GUMMACHINE");
                    break;
            }
        }

        private void RemoveFromVolume(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetGRUP grup && grup.assetName.ToLower().Contains("volume"))
                {
                    var items = (from value in grup.Items select (uint)value).ToList();
                    if (items.Contains(assetID))
                    {
                        items.Remove(assetID);
                        grup.Items = (from value in items select new AssetID(value)).ToArray();
                    }
                }
        }

        private bool ShuffleCameras()
        {
            var assets = (from asset in assetDictionary.Values
                          where asset.assetType == AssetType.Camera && asset.assetName != "STARTCAM"
                          select (AssetCAM)asset).ToList();

            if (assets.Count == 0)
                return false;

            for (int i = 0; i < assets.Count; i++)
            {
                var whoTargets = FindWhoTargets(assets[i].assetID);
                if (whoTargets.Count > 0 && GetFromAssetID(whoTargets[0]) is AssetDYNA dyna
                    && (dyna.Type == DynaType.game_object__BusStop || dyna.Type == DynaType.game_object__Taxi))
                {
                    assets.RemoveAt(i);
                    i--;
                }
            }

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();
            List<Vector3[]> angles = (from asset in assets
                                      select (new Vector3[] {
                            new Vector3(asset.NormalizedForwardX, asset.NormalizedForwardY, asset.NormalizedForwardZ),
                            new Vector3(asset.NormalizedUpX, asset.NormalizedUpY, asset.NormalizedUpZ),
                            new Vector3(asset.NormalizedLeftX, asset.NormalizedLeftY, asset.NormalizedLeftZ),
                            new Vector3(asset.ViewOffsetX, asset.ViewOffsetY, asset.ViewOffsetZ)
                        })).ToList();

            foreach (AssetCAM a in assets)
            {
                int value1 = random.Next(0, positions.Count);
                int value2 = random.Next(0, angles.Count);

                a.PositionX = positions[value1].X;
                a.PositionY = positions[value1].Y;
                a.PositionZ = positions[value1].Z;

                a.NormalizedForwardX = angles[value2][0].X;
                a.NormalizedForwardY = angles[value2][0].Y;
                a.NormalizedForwardZ = angles[value2][0].Z;

                a.NormalizedUpX = angles[value2][1].X;
                a.NormalizedUpY = angles[value2][1].Y;
                a.NormalizedUpZ = angles[value2][1].Z;

                a.NormalizedLeftX = angles[value2][2].X;
                a.NormalizedLeftY = angles[value2][2].Y;
                a.NormalizedLeftZ = angles[value2][2].Z;

                a.ViewOffsetX = angles[value2][3].X;
                a.ViewOffsetY = angles[value2][3].Y;
                a.ViewOffsetZ = angles[value2][3].Z;

                positions.RemoveAt(value1);
                angles.RemoveAt(value2);
            }

            return true;
        }

        private bool ShuffleScales(RandomizerSettings settings)
        {
            var types = new AssetType[] { AssetType.Boulder, AssetType.Button, AssetType.DestructibleObject, AssetType.Platform, AssetType.SimpleObject };

            var assets = (from asset in assetDictionary.Values
                          where types.Contains(asset.assetType) && !asset.assetName.ToLower().Contains("track")
                          select (EntityAsset)asset).ToList();

            foreach (EntityAsset a in assets)
            {
                bool isSkydome = false;
                foreach (var l in a.Links)
                    if ((EventBFBB)l.EventSendID == EventBFBB.SetasSkydome && l.TargetAsset.Equals(a.assetID))
                    {
                        isSkydome = true;
                        break;
                    }
                if (isSkydome)
                    continue;

                float scale = random.NextFloat(settings.scaleMin, settings.scaleMax);

                a.ScaleX *= scale;
                a.ScaleY *= scale;
                a.ScaleZ *= scale;

                if (a is AssetPLAT plat && plat.PlatformSpecific is PlatSpecific_Springboard springboard)
                {
                    springboard.Height1 *= scale;
                    springboard.Height2 *= scale;
                    springboard.Height3 *= scale;
                    springboard.HeightBubbleBounce *= scale;
                }
            }

            return assets.Count != 0;
        }

        private bool ShuffleRingScales(RandomizerSettings settings)
        {
            var assets = (from asset in assetDictionary.Values where asset is DynaGObjectRing select (DynaGObjectRing)asset).ToList();

            bool result = assets.Count != 0;

            while (assets.Count > 0)
            {
                int index = random.Next(0, assets.Count);

                float scale = random.NextFloat(settings.ringScaleMin, settings.ringScaleMax);

                assets[index].ScaleX *= scale;
                assets[index].ScaleY *= scale;
                assets[index].ScaleZ *= scale;
                assets[index].Radius *= 2 * scale;

                assets.RemoveAt(index);
            }

            return result;
        }

        private bool ShuffleFloatingBlocks()
        {
            switch (LevelName)
            {
                case "fb01":
                case "fb02":
                case "fb03":
                    var assets = (from asset in assetDictionary.Values where asset is AssetPLAT select (AssetPLAT)asset).ToList();

                    for (int i = 0; i < assets.Count; i++)
                    {
                        if (assets[i].assetName.ToLower().Contains("dizzy") ||
                            assets[i].Model.Equals("fb_platform") ||
                            (LevelName == "fb03" && (assets[i].assetName.Equals("SPRING_SPRING_06") || assets[i].assetName.Equals("SPRING_DRIVE_MECH_06"))))
                        {
                            assets.RemoveAt(i--);
                            continue;
                        }
                        foreach (var l in assets[i].Links)
                            if (l.EventSendID == (ushort)EventTSSM.Drivenby)
                            {
                                assets.RemoveAt(i--);
                                break;
                            }
                    }

                    List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

                    foreach (var a in assets)
                    {
                        int value = random.Next(0, positions.Count);

                        Vector3 delta = positions[value] - new Vector3(a.PositionX, a.PositionY, a.PositionZ);

                        a.PositionX = positions[value].X;
                        a.PositionY = positions[value].Y;
                        a.PositionZ = positions[value].Z;

                        SetDeltaToDriven(a.assetID, delta);

                        positions.RemoveAt(value);
                    }

                    return true;
                default:
                    return false;
            }
        }

        private void SetDeltaToDriven(uint assetID, Vector3 delta)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is EntityAsset entity)
                    foreach (var l in entity.Links)
                        if (l.EventSendID == (ushort)EventTSSM.Drivenby && l.TargetAsset.Equals(assetID))
                        {
                            entity.PositionX += delta.X;
                            entity.PositionY += delta.Y;
                            entity.PositionZ += delta.Z;
                            break;
                        }
        }

        private bool ShufflePlaceableColors(bool brightColors, bool strongColors)
        {
            AssetType[] allowed = new AssetType[] {
                            AssetType.Boulder, AssetType.Button, AssetType.DestructibleObject, AssetType.Hangable, AssetType.Villain, AssetType.Pendulum,
                            AssetType.Pickup, AssetType.Platform, AssetType.Player, AssetType.SimpleObject, AssetType.NPC };

            List<EntityAsset> assets = (from asset in assetDictionary.Values
                                        where allowed.Contains(asset.assetType)
                                        select asset).Cast<EntityAsset>().ToList();

            foreach (EntityAsset a in assets)
            {
                Vector3 color = GetRandomColor(brightColors, strongColors);
                a.ColorRed = color.X;
                a.ColorGreen = color.Y;
                a.ColorBlue = color.Z;
            }

            return assets.Count != 0;
        }

        private bool ShufflePlaceableDynaColors(bool brightColors, bool strongColors)
        {
            List<DynaEnemySB> assets = (from asset in assetDictionary.Values
                                        where asset is DynaEnemySB
                                        select asset).Cast<DynaEnemySB>().ToList();

            foreach (DynaEnemySB a in assets)
            {
                Vector3 color = GetRandomColor(brightColors, strongColors);
                a.ColorRed = color.X;
                a.ColorGreen = color.Y;
                a.ColorBlue = color.Z;
            }

            return assets.Count != 0;
        }

        private bool ShuffleLevelModelColors(bool brightColors, bool strongColors, bool vertexColors)
        {
            var assets = (from asset in assetDictionary.Values
                          where asset is AssetJSP assetJsp
                          select (AssetJSP)asset).ToList();

            float max = 255f;
            bool colored = false;

            foreach (var a in assets)
            {
                try
                {
                    if (vertexColors)
                    {
                        var performOperation = new Func<Vector4, Vector4>((Vector4 oldColor) =>
                        {
                            var v = (Vector4)GetRandomColor(brightColors, strongColors);
                            v.W = oldColor.W;
                            return v;
                        });

                        a.ApplyVertexColors(performOperation);
                        colored = true;
                    }
                    else
                    {
                        var colors = GetColors(a.Data);

                        if (colors.Length == 0)
                            continue;

                        for (int i = 0; i < colors.Length; i++)
                        {
                            Vector3 color = GetRandomColor(brightColors, strongColors);

                            colors[i] = System.Drawing.Color.FromArgb(colors[i].A,
                                (byte)(color.X * max),
                                (byte)(color.Y * max),
                                (byte)(color.Z * max));
                        }

                        a.Data = SetColors(a.Data, colors);
                        colored = true;
                    }
                }
                catch { }
            }

            return colored;
        }

        private Vector3 GetRandomColor(bool brightColors, bool strongColors)
        {
            float colorMin = brightColors ? 0.5f : 0f;
            float colorMax = 1f;

            Vector3 v = new Vector3(random.NextFloat(colorMin, colorMax), random.NextFloat(colorMin, colorMax), random.NextFloat(colorMin, colorMax));

            if (strongColors)
            {
                float strongFactor = brightColors ? 0.4f : 0.7f;
                float doublestrongFactor = 2 * strongFactor;

                v -= strongFactor;

                int strongColor = random.Next(0, 6);

                switch (strongColor)
                {
                    case 0:
                        v.X += doublestrongFactor;
                        break;
                    case 1:
                        v.Y += doublestrongFactor;
                        break;
                    case 2:
                        v.Z += doublestrongFactor;
                        break;
                    case 3:
                        v.X += doublestrongFactor;
                        v.Y += doublestrongFactor;
                        break;
                    case 4:
                        v.X += doublestrongFactor;
                        v.Z += doublestrongFactor;
                        break;
                    case 5:
                        v.Y += doublestrongFactor;
                        v.Z += doublestrongFactor;
                        break;
                }
            }

            return new Vector3(ClampFloat(v.X), ClampFloat(v.Y), ClampFloat(v.Z));
        }

        private float ClampFloat(float value, float min = 0f, float max = 1f)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private System.Drawing.Color[] GetColors(byte[] data)
        {
            List<System.Drawing.Color> colors = new List<System.Drawing.Color>();

            foreach (RWSection rws in ModelAsRWSections(data, out _))
                if (rws is Clump_0010 clump)
                    foreach (Geometry_000F geo in clump.geometryList.geometryList)
                        foreach (Material_0007 mat in geo.materialList.materialList)
                        {
                            RenderWareFile.Color rwColor = mat.materialStruct.color;
                            System.Drawing.Color color = System.Drawing.Color.FromArgb(rwColor.A, rwColor.R, rwColor.G, rwColor.B);
                            colors.Add(color);
                        }

            return colors.ToArray();
        }

        private byte[] SetColors(byte[] data, System.Drawing.Color[] colors)
        {
            int i = 0;

            RWSection[] sections = ModelAsRWSections(data, out int renderWareVersion);

            foreach (RWSection rws in sections)
                if (rws is Clump_0010 clump)
                    foreach (Geometry_000F geo in clump.geometryList.geometryList)
                        foreach (Material_0007 mat in geo.materialList.materialList)
                        {
                            if (i >= colors.Length)
                                continue;

                            mat.materialStruct.color = new RenderWareFile.Color(colors[i].R, colors[i].G, colors[i].B, colors[i].A);
                            i++;
                        }

            return ModelToRWSections(sections, renderWareVersion);
        }

        private RWSection[] ModelAsRWSections(byte[] data, out int renderWareVersion, bool treatStuffAsByteArray = true)
        {
            ReadFileMethods.treatStuffAsByteArray = treatStuffAsByteArray;
            RWSection[] sections = ReadFileMethods.ReadRenderWareFile(data);
            renderWareVersion = sections[0].renderWareVersion;
            ReadFileMethods.treatStuffAsByteArray = false;
            return sections;
        }

        private byte[] ModelToRWSections(RWSection[] sections, int renderWareVersion)
        {
            ReadFileMethods.treatStuffAsByteArray = true;
            byte[] data = ReadFileMethods.ExportRenderWareFile(sections, renderWareVersion);
            ReadFileMethods.treatStuffAsByteArray = false;
            return data;
        }

        private bool ShuffleBusStops()
        {
            bool result = false;

            foreach (var dynaBusStop in (from asset in assetDictionary.Values
                                         where asset is DynaGObjectBusStop
                                         select asset).Cast<DynaGObjectBusStop>())
            {
                dynaBusStop.Player = (PlayerEnum)random.Next(0, 2);
                result = true;
            }

            if (LevelName == "hb01")
            {
                for (int i = 1; i < 4; i++)
                {
                    SetTaskDynasForVil("BUBBLEBUDDY_NPC_0" + i.ToString());
                    SetTaskDynasForVil("MRKRABS_NPC_0" + i.ToString());
                    SetTaskDynasForVil("PLANKTON_NPC_0" + i.ToString());
                }
                SetTaskDynasForVil("PATRICK_NPC_01");
            }
            else if (LevelName == "db04")
                SetTaskDynasForVil("NPC_SQUIDWARD");
            else if (LevelName == "db06")
                SetTaskDynasForVil("NPC_PATRICK");

            return result;
        }

        private void SetTaskDynasForVil(string vilAssetID)
        {
            SetTaskDynasForVil(new AssetID(vilAssetID));
        }

        private void SetTaskDynasForVil(uint vilAssetID)
        {
            if (ContainsAsset(vilAssetID))
            {
                AssetVIL vil = ((AssetVIL)GetFromAssetID(vilAssetID));
                vil.TaskBox2 = vil.TaskBox1;
            }
        }

        public bool RandomizePlayerOnSpawn()
        {
            var group = (AssetGRUP)PlaceTemplate("IP_RANDO_PLAYER_GRUP", AssetTemplate.Group);
            group.ReceiveEventDelegation = Delegation.RandomItem;
            group.Links = new Link[]
            {
                new Link(game)
                {
                    EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                    EventSendID = (ushort)EventBFBB.Run,
                    TargetAsset = group.assetID
                }
            };

            var outAssetIDs = new List<uint>();
            for (int i = 0; i < 3; i++)
            {
                var timer = (AssetTIMR)PlaceTemplate(new Vector3(), ref outAssetIDs, "IP_RANDO_PLAYER_TIMR", AssetTemplate.Timer);
                timer.Time = 0.1f;
                timer.Links = new Link[]
                {
                    new Link(game)
                    {
                        FloatParameter1 = i,
                        EventReceiveID = (ushort)EventBFBB.Expired,
                        EventSendID = (ushort)EventBFBB.SwitchPlayerCharacter,
                        TargetAsset = "SPONGEBOB"
                    },
                    new Link(game)
                    {
                        EventReceiveID = (ushort)EventBFBB.Expired,
                        EventSendID = (ushort)EventBFBB.Reset,
                        TargetAsset = timer.assetID
                    }
                };
            }

            var assetIDs = new List<AssetID>();
            foreach (uint i in outAssetIDs)
                assetIDs.Add(new AssetID(i));
            group.Items = assetIDs.ToArray();

            return true;
        }

        private bool ShuffleVilTypes(List<NpcType_BFBB> chooseFrom, List<NpcType_BFBB> setTo, bool mixModels, bool veryRandom, bool enemies)
        {
            if (veryRandom && (LevelName == "sm01" || LevelName == "gl01"))
            {
                HashSet<NpcType_BFBB> uniqueSetTo = new HashSet<NpcType_BFBB>();
                foreach (NpcType_BFBB v in setTo)
                    uniqueSetTo.Add(v);

                while (uniqueSetTo.Count > 5)
                {
                    NpcType_BFBB randomRemove = setTo[random.Next(0, setTo.Count)];
                    while (setTo.Contains(randomRemove))
                        setTo.Remove(randomRemove);
                    uniqueSetTo.Remove(randomRemove);
                }
            }

            if (setTo.Count == 0)
                return false;

            List<AssetVIL> assets = (from asset in assetDictionary.Values where asset is AssetVIL vil && chooseFrom.Contains(vil.NpcType_BFBB) select asset).Cast<AssetVIL>().ToList();
            List<NpcType_BFBB> viltypes = (from asset in assets select asset.NpcType_BFBB).ToList();
            List<AssetID> models = (from asset in assets select asset.Model).ToList();

            foreach (AssetVIL a in assets)
            {
                if (a.NpcType_BFBB == NpcType_BFBB.robot_arf_bind || a.NpcType_BFBB == NpcType_BFBB.tubelet_bind)
                    KillKids(a);

                int viltypes_value = random.Next(0, viltypes.Count);
                int model_value = mixModels ? random.Next(0, viltypes.Count) : viltypes_value;

                a.NpcType_BFBB = veryRandom ? setTo[random.Next(0, setTo.Count)] : viltypes[viltypes_value];

                if (enemies && veryRandom)
                    a.Model =
                        a.NpcType_BFBB == NpcType_BFBB.robot_sleepytime_bind ?
                        "robot_sleepy-time_bind.MINF" :
                        a.NpcType_BFBB.ToString() + ".MINF";

                else
                    a.Model = models[model_value];

                viltypes.RemoveAt(viltypes_value);
                models.RemoveAt(model_value);

                if (a.NpcType_BFBB == NpcType_BFBB.robot_arf_bind || a.NpcType_BFBB == NpcType_BFBB.tubelet_bind)
                    CreateKids(a);

                ImportNpcFile(a.NpcType_BFBB);
            }

            return assets.Count != 0;
        }

        public void ImportNpcFile(NpcType_BFBB npcType)
        {
            if (ContainsAsset(new AssetID(npcType.ToString().Replace("sleepytime", "sleepy-time") + ".MINF")))
                return;

            string hipFileName;
            switch (npcType)
            {
                case NpcType_BFBB.g_love_bind:
                    hipFileName = "g-love.HIP";
                    break;
                case NpcType_BFBB.ham_bind:
                    hipFileName = "ham-mer.HIP";
                    break;
                case NpcType_BFBB.robot_0a_bomb_bind:
                    hipFileName = "bomb-bot.HIP";
                    break;
                case NpcType_BFBB.robot_0a_bzzt_bind:
                    hipFileName = "bzzt-bot.HIP";
                    break;
                case NpcType_BFBB.robot_0a_chomper_bind:
                    hipFileName = "chomp-bot.HIP";
                    break;
                case NpcType_BFBB.robot_0a_fodder_bind:
                    hipFileName = "fodder.HIP";
                    break;
                case NpcType_BFBB.robot_4a_monsoon_bind:
                    hipFileName = "monsoon.HIP";
                    break;
                case NpcType_BFBB.robot_9a_bind:
                    hipFileName = "slick.HIP";
                    break;
                case NpcType_BFBB.robot_chuck_bind:
                    hipFileName = "chuck.HIP";
                    break;
                case NpcType_BFBB.robot_sleepytime_bind:
                    hipFileName = "sleepytime.HIP";
                    break;
                case NpcType_BFBB.robot_tar_bind:
                    hipFileName = "tar-tar.HIP";
                    break;
                case NpcType_BFBB.robot_arf_bind:
                    hipFileName = "arf_arf-dawg.HIP";
                    break;
                case NpcType_BFBB.tubelet_bind:
                    hipFileName = "tubelet.HIP";
                    break;
                case NpcType_BFBB.tiki_wooden_bind:
                    hipFileName = "tiki_wooden.HIP";
                    break;
                case NpcType_BFBB.tiki_lovey_dovey_bind:
                    hipFileName = "tiki_floating.HIP";
                    break;
                case NpcType_BFBB.tiki_thunder_bind:
                    hipFileName = "tiki_thunder.HIP";
                    break;
                case NpcType_BFBB.tiki_stone_bind:
                    hipFileName = "tiki_stone.HIP";
                    break;
                case NpcType_BFBB.tiki_shhhh_bind:
                    hipFileName = "tiki_shhh.HIP";
                    break;
                default:
                    throw new Exception("Invalid VilType");
            }

            ProgImportHip(npcType.ToString().Contains("tiki") ? "Utility" : "Enemies", hipFileName);
        }

        private void KillKids(AssetVIL vil)
        {
            var links = vil.Links.ToList();
            for (int i = 0; i < links.Count; i++)
                if (links[i].EventSendID == (ushort)EventBFBB.Connect_IOwnYou &&
                    ContainsAsset(links[i].TargetAsset) &&
                    GetFromAssetID(links[i].TargetAsset) is AssetVIL child &&
                    (child.NpcType_BFBB == NpcType_BFBB.tubelet_slave_bind || child.NpcType_BFBB == NpcType_BFBB.robot_arf_dog_bind))
                {
                    RemoveAsset(links[i].TargetAsset);
                    links.RemoveAt(i);
                    i--;
                }
            vil.Links = links.ToArray();
        }

        private void CreateKids(AssetVIL vil)
        {
            var position = new Vector3(vil.PositionX, vil.PositionY, vil.PositionZ);
            var links = new List<Link>(); // vil.Links.ToList();

            if (vil.NpcType_BFBB == NpcType_BFBB.robot_arf_bind)
            {
                var dogCount = 3;
                for (int i = 0; i < dogCount; i++)
                {
                    var dog = PlaceTemplate(position, vil.assetName + "_DOG" + i.ToString(), AssetTemplate.ArfDog);
                    links.Add(new Link(game)
                    {
                        TargetAsset = dog.assetID,
                        EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                        EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                    });
                    links.Add(new Link(game)
                    {
                        TargetAsset = dog.assetID,
                        EventReceiveID = (ushort)EventBFBB.NPCSetActiveOff,
                        EventSendID = (ushort)EventBFBB.NPCSetActiveOff
                    });
                    links.Add(new Link(game)
                    {
                        TargetAsset = dog.assetID,
                        EventReceiveID = (ushort)EventBFBB.NPCSetActiveOn,
                        EventSendID = (ushort)EventBFBB.NPCSetActiveOn
                    });
                }
            }
            else if (vil.NpcType_BFBB == NpcType_BFBB.tubelet_bind)
            {
                for (int i = 0; i < 2; i++)
                {
                    var slave = PlaceTemplate(position, vil.assetName + "_SLAVE" + i.ToString(), AssetTemplate.TubeletSlave);
                    links.Add(new Link(game)
                    {
                        TargetAsset = slave.assetID,
                        EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                        EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                    });
                    links.Add(new Link(game)
                    {
                        TargetAsset = slave.assetID,
                        EventReceiveID = (ushort)EventBFBB.NPCSetActiveOff,
                        EventSendID = (ushort)EventBFBB.NPCSetActiveOff
                    });
                    links.Add(new Link(game)
                    {
                        TargetAsset = slave.assetID,
                        EventReceiveID = (ushort)EventBFBB.NPCSetActiveOn,
                        EventSendID = (ushort)EventBFBB.NPCSetActiveOn
                    });
                }
            }

            links.AddRange(vil.Links);
            vil.Links = links.ToArray();
        }

        private bool ShuffleBoxDynaTypes(List<EnemySupplyCrateType> chooseFrom, List<EnemySupplyCrateType> setTo)
        {
            if (setTo.Count == 0)
                return false;

            var assets = (from asset in assetDictionary.Values
                          where asset is DynaEnemySupplyCrate dynaC &&
                          chooseFrom.Contains(dynaC.CrateType)
                          select (DynaEnemySupplyCrate)asset).ToList();

            foreach (DynaEnemySupplyCrate a in assets)
            {
                int index = random.Next(0, setTo.Count);
                a.CrateType = setTo[index];
                ImportCrateType(a.CrateType, false);
            }

            return assets.Count != 0;
        }

        public void ImportCrateType(EnemySupplyCrateType v, bool unimport)
        {
            if (ContainsAsset(new AssetID(v.ToString() + ".MINF")))
                return;

            string hipFileName;
            switch (v)
            {
                case EnemySupplyCrateType.crate_wood_bind:
                    hipFileName = "crate_wood.hip";
                    break;
                case EnemySupplyCrateType.crate_hover_bind:
                    hipFileName = "crate_hover.hip";
                    break;
                case EnemySupplyCrateType.crate_explode_bind:
                    hipFileName = "crate_explode.hip";
                    break;
                case EnemySupplyCrateType.crate_shrink_bind:
                    hipFileName = "crate_shrink.hip";
                    break;
                case EnemySupplyCrateType.crate_steel_bind:
                    hipFileName = "crate_steel.hip";
                    break;
                default:
                    throw new Exception("Invalid Crate Type");
            }

            if (unimport)
                ProgUnimportHip("Utility", hipFileName);
            else
                ProgImportHip("Utility", hipFileName);
        }

        private bool ShuffleEnemyDynaTypes(List<EnemyStandardType> chooseFrom, List<EnemyStandardType> setTo, bool veryRandom)
        {
            if (setTo.Count == 0)
                return false;

            var assets = (from asset in assetDictionary.Values
                          where asset is DynaEnemyStandard dynaC &&
                          chooseFrom.Contains(dynaC.EnemyType)
                          select (DynaEnemyStandard)asset).ToList();

            List<EnemyStandardType> altSetTo = (from asset in assets select asset.EnemyType).ToList();

            foreach (var a in assets)
            {
                if (veryRandom)
                {
                    int index = random.Next(0, setTo.Count);
                    a.EnemyType = setTo[index];
                    a.EnemyFlags.FlagValueInt = 12;
                    a.Unknown5C = 0;
                    a.Unknown60 = 0;
                    a.Unknown64 = 0;
                    a.Unknown68 = 0;
                }
                else
                {
                    int index = random.Next(0, altSetTo.Count);
                    a.EnemyType = altSetTo[index];
                    altSetTo.RemoveAt(index);
                }
                ImportDynaEnemyTypes(a.EnemyType, false);
            }

            return assets.Count != 0;
        }

        public void ImportDynaEnemyTypes(EnemyStandardType v, bool unimport)
        {
            if (ContainsAsset(new AssetID(v.ToString() + ".MINF")))
                return;

            string hipFileName;
            switch (v)
            {
                case EnemyStandardType.flinger_v1_bind:
                    hipFileName = "flinger_desert.hip";
                    break;
                case EnemyStandardType.flinger_v2_bind:
                    hipFileName = "flinger_trench.hip";
                    break;
                case EnemyStandardType.flinger_v3_bind:
                    hipFileName = "flinger_junkyard.hip";
                    break;
                case EnemyStandardType.fogger_de_bind:
                    hipFileName = "fogger_desert.HIP";
                    break;
                case EnemyStandardType.fogger_gg_bind:
                    hipFileName = "fogger_goofy_goober.HIP";
                    break;
                case EnemyStandardType.fogger_jk_bind:
                    hipFileName = "fogger_junkyard.hip";
                    break;
                case EnemyStandardType.fogger_pt_bind:
                    hipFileName = "fogger_planktopolis.hip";
                    break;
                case EnemyStandardType.fogger_tr_bind:
                    hipFileName = "fogger_trench.hip";
                    break;
                case EnemyStandardType.fogger_tt_bind:
                    hipFileName = "fogger_thugtug.hip";
                    break;
                case EnemyStandardType.fogger_v1_bind:
                    hipFileName = "fogger_v1.hip";
                    break;
                case EnemyStandardType.fogger_v2_bind:
                    hipFileName = "fogger_v2.hip";
                    break;
                case EnemyStandardType.fogger_v3_bind:
                    hipFileName = "fogger_v3.hip";
                    break;
                case EnemyStandardType.mervyn_v3_bind:
                    hipFileName = "mervyn.hip";
                    break;
                case EnemyStandardType.minimerv_v1_bind:
                    hipFileName = "mini_merv.hip";
                    break;
                case EnemyStandardType.popper_v1_bind:
                    hipFileName = "popper_trench.hip";
                    break;
                case EnemyStandardType.popper_v3_bind:
                    hipFileName = "popper_planktopolis.hip";
                    break;
                case EnemyStandardType.slammer_v1_bind:
                    hipFileName = "slammer_goofy_goober.hip";
                    break;
                case EnemyStandardType.slammer_des_bind:
                    hipFileName = "slammer_desert.hip";
                    break;
                case EnemyStandardType.slammer_v3_bind:
                    hipFileName = "slammer_thugtug.hip";
                    break;
                case EnemyStandardType.spinner_v1_bind:
                    hipFileName = "spinner_thugtug.hip";
                    break;
                case EnemyStandardType.spinner_v2_bind:
                    hipFileName = "spinner_junkyard.hip";
                    break;
                case EnemyStandardType.spinner_v3_bind:
                    hipFileName = "spinner_planktopolis.hip";
                    break;
                default:
                    throw new Exception("Invalid Enemy Type");
            }

            if (unimport)
                ProgUnimportHip("Enemies", hipFileName);
            else
                ProgImportHip("Enemies", hipFileName);
        }

        private bool ShuffleCombatArenas(int combatMin, int combatMax)
        {
            switch (LevelName)
            {
                case "am01":
                case "am02":
                case "am03":
                    short count1 = (short)random.Next(combatMin, combatMax + 1);
                    short count2 = (short)random.Next(combatMin, combatMax + 1);
                    short count3 = (short)random.Next(combatMin, combatMax + 1);
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_1_COUNTER_01"))).Count = count1;
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_1_DONE_COUNTER_01"))).Count = count1;
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_2_COUNTER_01"))).Count = count2;
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_2_DONE_COUNTER_01"))).Count = count2;
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_3_COUNTER_01"))).Count = count3;
                    ((AssetCNTR)GetFromAssetID(new AssetID("WAVE_3_DONE_COUNTER_01"))).Count = count3;
                    return true;
            }
            return false;
        }

        private bool DisableCutscenes()
        {
            switch (LevelName)
            {
                case "bb01":
                    uint gloveIntroDisp = new AssetID("GLOVE_INTRO_DISP");
                    uint gloveIntroTrig = new AssetID("GLOVE_INTRO_TRIG");

                    if (ContainsAsset(gloveIntroDisp))
                        ((AssetDPAT)GetFromAssetID(gloveIntroDisp)).EnabledOnStart = false;
                    if (ContainsAsset(gloveIntroTrig))
                        ((AssetTRIG)GetFromAssetID(gloveIntroTrig)).EnabledOnStart = false;
                    return true;

                case "bb02":
                    uint chuckOffDisp = new AssetID("CHUCK_CINEMATIC_OFF_DISP");

                    if (ContainsAsset(chuckOffDisp))
                        ((AssetDPAT)GetFromAssetID(chuckOffDisp)).EnabledOnStart = false;
                    return true;

                case "bc01":
                    uint arfIntroTrigDisp = new AssetID("ARF_INTRO_TRIG_DISP");

                    if (ContainsAsset(arfIntroTrigDisp))
                        ((AssetDPAT)GetFromAssetID(arfIntroTrigDisp)).EnabledOnStart = false;
                    return true;

                case "gl01":
                    uint moonsoonIntroTrig = new AssetID("MONSOON_INTRO_TRIG");

                    if (ContainsAsset(moonsoonIntroTrig))
                        ((AssetTRIG)GetFromAssetID(moonsoonIntroTrig)).EnabledOnStart = false;
                    return true;

                case "gy01":
                    uint slickIntoDisp = new AssetID("SLICK_INTRO_TRIG_DISP'");

                    if (ContainsAsset(slickIntoDisp))
                        ((AssetDPAT)GetFromAssetID(slickIntoDisp)).EnabledOnStart = false;
                    return true;

                case "jf01":
                    uint jf01fly = new AssetID("JF01_FLYTHOUGH_WIDGET");

                    if (ContainsAsset(jf01fly))
                    {
                        AssetDYNA fly = (AssetDYNA)GetFromAssetID(jf01fly);
                        List<Link> flyLinks = fly.Links.ToList();
                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventSendID == (ushort)EventBFBB.Preload)
                            {
                                flyLinks.RemoveAt(i);
                                break;
                            }
                        fly.Links = flyLinks.ToArray();
                    }

                    uint swCinemaDisp = new AssetID("SWCINEMA_DISP_01");
                    if (ContainsAsset(swCinemaDisp))
                        ((AssetDPAT)GetFromAssetID(swCinemaDisp)).EnabledOnStart = true;

                    uint hammerDisp = new AssetID("HAMMERCINEMA_DISP_01");
                    if (ContainsAsset(hammerDisp))
                        ((AssetDPAT)GetFromAssetID(hammerDisp)).EnabledOnStart = true;
                    return true;

                case "jf03":
                    uint tartarIntroDisp = new AssetID("TARTAR_CUTSCENE_OFF_DISP");

                    if (ContainsAsset(tartarIntroDisp))
                        ((AssetDPAT)GetFromAssetID(tartarIntroDisp)).EnabledOnStart = true;
                    return true;

                case "kf01":
                    Link[] csnmLinks = null;
                    uint kf01csnm = new AssetID("TUBELET_CUTSCENE_MGR");
                    if (ContainsAsset(kf01csnm))
                        csnmLinks = ((BaseAsset)GetFromAssetID(kf01csnm)).Links;

                    uint kf01fly = new AssetID("KF01_FLYTHOUGH_WIDGET");
                    if (ContainsAsset(kf01fly))
                    {
                        BaseAsset flyWidged = ((BaseAsset)GetFromAssetID(kf01fly));
                        List<Link> flyLinks = flyWidged.Links.ToList();
                        flyLinks.AddRange(csnmLinks);

                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventReceiveID == (ushort)EventBFBB.Done)
                                flyLinks[i].EventReceiveID = (ushort)EventBFBB.Stop;
                            else if (flyLinks[i].EventSendID == (ushort)EventBFBB.Preload)
                                flyLinks.RemoveAt(i--);

                        flyWidged.Links = flyLinks.ToArray();
                    }

                    return true;

                case "rb01":
                    uint rb01fly = new AssetID("RB01_FLYTHOUGH_WIDGET");

                    if (ContainsAsset(rb01fly))
                    {
                        AssetDYNA fly = (AssetDYNA)GetFromAssetID(rb01fly);
                        List<Link> flyLinks = fly.Links.ToList();
                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventSendID == (ushort)EventBFBB.Preload)
                            {
                                flyLinks.RemoveAt(i);
                                break;
                            }
                        fly.Links = flyLinks.ToArray();
                    }

                    uint sleepyDpat = new AssetID("SLEEPY_DESP_02");
                    if (ContainsAsset(sleepyDpat))
                        ((AssetDPAT)GetFromAssetID(sleepyDpat)).StateIsPersistent = false;
                    return true;
            }

            return false;
        }

        private bool DisableCutscenesMovie()
        {
            switch (LevelName)
            {
                case "b101":
                {
                    uint dpat1 = new AssetID("AUTOSAVE_DONE_DISP");
                    if (ContainsAsset(dpat1))
                    {
                        var a2 = (BaseAsset)GetFromAssetID(dpat1);
                        var a3 = a2.Links;
                        a3[0].EventSendID = (ushort)EventTSSM.Run;
                        a3[0].TargetAsset = "FRENCH_NARR2PORTAL_SCRIPT";
                        a2.Links = a3;
                    }
                    uint dpat2 = new AssetID("BOSS_DEATH_HAVE_TOKEN_SCRIPT");
                    if (ContainsAsset(dpat2))
                    {
                        var a2 = (AssetSCRP)GetFromAssetID(dpat2);
                        var a3 = a2.TimedLinks;
                        a3[0].EventSendID = (ushort)EventTSSM.Run;
                        a3[0].TargetAsset = "FRENCH_NARR2PORTAL_SCRIPT";
                        a2.TimedLinks = a3;
                    }
                    uint dpat3 = new AssetID("CIN_PLAY_DISP");
                    if (ContainsAsset(dpat3))
                    {
                        var a2 = (AssetDPAT)GetFromAssetID(dpat3);
                        var a3 = a2.Links;
                        a3[0].EventSendID = (ushort)EventTSSM.BossStageSet;
                        a3[0].TargetAsset = "FROG_FISH_NME";
                        a2.Links = a3;
                    }
                    return true;
                }
                case "b201":
                {
                    uint dpat1 = new AssetID("INTRO_CINE_DISP_01");
                    if (ContainsAsset(dpat1))
                        RemoveAsset(dpat1);

                    uint scrp1 = new AssetID("PRE_END_CUTSCENE_SCRIPT");
                    if (ContainsAsset(scrp1))
                    {
                        var a3 = (AssetSCRP)GetFromAssetID(scrp1);
                        var a4 = a3.TimedLinks;

                        a4[6].EventSendID = (ushort)EventTSSM.Done;
                        a3.TimedLinks = a4;
                    }
                    return true;
                }
            }

            return false;
        }

        private bool ShuffleMRKRPositions(bool noWarps, bool pointers, bool plyrs, bool busStops, bool teleBox, bool taxis)
        {
            var assets = new List<IClickableAsset>();

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetMRKR mrkr && VerifyMarkerStep1(mrkr, noWarps, busStops, teleBox, taxis))
                    assets.Add(mrkr);
                else if (a is AssetDYNA dyna && pointers && dyna is DynaPointer pointer && VerifyPointer(dyna))
                    assets.Add(pointer);
                else if (plyrs && a is AssetPLYR plyr)
                    assets.Add(plyr);

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            foreach (IClickableAsset a in assets)
            {
                int value = random.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);
            }

            return assets.Count != 0;
        }

        private bool VerifyPointer(AssetDYNA pointer)
        {
            if (game == Game.Incredibles)
                switch (LevelName)
                {
                    case "bb03":
                    case "jk01":
                        if (pointer.assetName.Contains("RINGRACE_POINTER"))
                            return false;
                        break;
                    case "tt01":
                        if (pointer.assetName.Contains("SONICGUITAR_POINTER"))
                            return false;
                        break;
                    case "pt03":
                        if (pointer.assetName.Contains("GGTOKEN_POINTER"))
                            return false;
                        break;
                }
            return true;
        }

        private bool VerifyMarkerStep1(AssetMRKR mrkr, bool noWarps, bool busStops, bool teleBox, bool taxis)
        {
            string assetName = mrkr.assetName;

            if (assetName.Contains("TEMP"))
                return false;
            if (noWarps && assetName.Contains("SPAT"))
                return false;

            List<uint> whoTargets = FindWhoTargets(mrkr.assetID);
            if (whoTargets.Count > 0)
            {
                foreach (uint u in whoTargets)
                    if (GetFromAssetID(u) is AssetDYNA dyna)
                    {
                        if ((busStops && dyna.Type == DynaType.game_object__BusStop) ||
                            (taxis && dyna.Type == DynaType.game_object__Taxi) ||
                            (teleBox && dyna.Type == DynaType.game_object__Teleport))
                            return true;
                    }
                    else if (GetFromAssetID(u) is AssetTRIG trig)
                        foreach (Link link in trig.Links)
                            if (link.EventSendID == (ushort)EventBFBB.SetCheckPoint)
                            {
                                if (LevelName == "sm02" && (assetName.Equals("CHECKPOINT_MK_01") || assetName.Equals("CHECKPOINT_MK_02")))
                                    mrkr.PositionY += 0.5f;

                                return VerifyMarkerStep2(assetName);
                            }
                            else if (GetFromAssetID(u) is AssetPORT)
                                return true;

                return false;
            }

            return VerifyMarkerStep2(assetName);
        }

        private bool VerifyMarkerStep2(string assetName)
        {
            try
            {
                if (game == Game.BFBB)
                    switch (LevelName)
                    {
                        case "jf01":
                            if (assetName.Contains("BOUCETREE PARTICLE MARKER"))
                                return false;
                            break;
                        case "jf04":
                            if (Convert.ToInt32(assetName.Split('_')[2]) > 3)
                                return false;
                            break;
                        case "gl01":
                            if (assetName.Contains("TOWER"))
                                return false;
                            break;
                        case "rb03":
                            if (assetName == "RB03MK12" || assetName == "RB03MK13")
                                return false;
                            break;
                        case "sm03":
                            if (Convert.ToInt32(assetName.Split('_')[2]) > 4)
                                return false;
                            break;
                        case "sm04":
                            if (Convert.ToInt32(assetName.Split('_')[2]) > 4)
                                return false;
                            break;
                        case "kf01":
                            if (Convert.ToInt32(assetName.Split('_')[2]) == 3)
                                return false;
                            break;
                        case "kf04":
                            if (Convert.ToInt32(assetName.Split('_')[2]) == 2)
                                return false;
                            break;
                        case "kf05":
                            if (Convert.ToInt32(assetName.Split('_')[2]) > 3)
                                return false;
                            break;
                        case "gy01":
                            if (assetName.Equals("TELEPRT_MARK_B"))
                                return false;
                            break;
                        case "gy03":
                            if (assetName.Contains("BALLDESTROYED"))
                                return false;
                            break;
                        case "db02":
                            if (Convert.ToInt32(assetName.Split('_')[2]) > 6)
                                return false;
                            break;
                    }
                else
                if (game == Game.Scooby)
                {
                    if (assetName.ToUpper().Contains("CAM") || assetName.ToUpper().Contains("RAIL"))
                        return false;

                    switch (LevelName)
                    {
                        case "r020":
                            if (assetName.Contains("FROMR003"))
                                return false;
                            break;
                        case "f010":
                            if (assetName.Contains("WAVE1800"))
                                return false;
                            break;
                    }
                }
            }
            catch { }

            return true;
        }

        private bool ShufflePlatSpeeds(RandomizerSettings settings)
        {
            float minMultiSpeed = settings.speedMin;
            float maxMultiSpeed = settings.speedMax;
            float minMultiTime = settings.speedMax == 0 ? 0 : 1 / settings.speedMax;
            float maxMultiTime = settings.speedMin == 0 ? 0 : 1 / settings.speedMin;

            bool result = false;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPLAT plat)
                {
                    if (plat.PlatformSpecific is PlatSpecific_ConveryorBelt p)
                    {
                        p.Speed *= random.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatformSpecific = p;
                    }
                    else if (plat.PlatformSpecific is PlatSpecific_FallingPlatform pf)
                    {
                        pf.Speed *= random.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatformSpecific = pf;
                    }
                    else if (plat.PlatformSpecific is PlatSpecific_BreakawayPlatform b)
                    {
                        b.BreakawayDelay *= random.NextFloat(minMultiTime, maxMultiTime);
                        b.ResetDelay *= random.NextFloat(minMultiTime, maxMultiTime);
                        plat.PlatformSpecific = b;
                    }
                    else if (plat.PlatformSpecific is PlatSpecific_TeeterTotter tt)
                    {
                        tt.InverseMass *= random.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatformSpecific = tt;
                    }

                    if (plat.Motion is Motion_MovePoint mp)
                    {
                        mp.Speed *= random.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.Motion = mp;
                    }
                    else if (plat.Motion is Motion_Mechanism mc)
                    {
                        mc.PostRetractDelay *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.RetractDelay *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateAccelTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateDecelTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideAccelTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideDecelTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideTime *= random.NextFloat(minMultiTime, maxMultiTime);
                        plat.Motion = mc;
                    }

                    result = true;
                }

            return result;
        }

        private bool RandomizeBoulderSettings(RandomizerSettings settings)
        {
            float min = settings.boulderMin;
            float max = settings.boulderMax;

            List<AssetBOUL> assets = (from asset in assetDictionary.Values where asset.assetType == AssetType.Boulder select asset).Cast<AssetBOUL>().ToList();

            foreach (AssetBOUL boul in assets)
            {
                boul.Gravity *= random.NextFloat(min, max);
                boul.Mass *= random.NextFloat(min, max);
                boul.BounceFactor *= random.NextFloat(min, max);
                boul.Friction *= random.NextFloat(min, max);
                boul.StartFriction *= random.NextFloat(min, max);
                boul.MaxLinearVelocity *= random.NextFloat(min, max);
                boul.MaxAngularVelocity *= random.NextFloat(min, max);
                boul.Stickiness *= random.NextFloat(min, max);
                boul.BounceDamp *= random.NextFloat(min, max);
                boul.KillTimer *= random.NextFloat(min, max);
                boul.InnerRadius *= random.NextFloat(min, max);
                boul.OuterRadius *= random.NextFloat(min, max);
            }

            return assets.Count > 0;
        }

        public bool RandomizeSounds(bool mixTypes, bool scoobyBoot = false)
        {
            bool result = false;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V1 sndi1)
                    result |= RandomizeSNDI_GCN_V1(sndi1, mixTypes, scoobyBoot);
                else if (a is AssetSNDI_GCN_V2 sndi2)
                    result |= RandomizeSNDI_GCN_V2(sndi2);
                else if (a is AssetSNDI_XBOX sndi3)
                    result |= RandomizeSNDI_XBOX(sndi3, mixTypes, scoobyBoot);
                else if (a is AssetSNDI_PS2 sndi4)
                    result |= RandomizeSNDI_PS2(sndi4, mixTypes, scoobyBoot);

            return result;
        }

        private bool RandomizeSNDI_GCN_V2(AssetSNDI_GCN_V2 sndi)
        {
            List<uint> assetIDs = new List<uint>();

            var entries = sndi.Entries;

            foreach (var v in entries)
                foreach (var u in v.SoundEntries)
                    assetIDs.Add(u.Sound);

            foreach (var v in entries)
                foreach (var u in v.SoundEntries)
                {
                    int index = random.Next(0, assetIDs.Count);
                    u.Sound = assetIDs[index];
                    assetIDs.RemoveAt(index);
                }

            sndi.Entries = entries;

            return true;
        }

        private bool RandomizeSNDI_XBOX(AssetSNDI_XBOX sndi, bool mixTypes, bool scoobyBoot)
        {
            var snd = sndi.Entries_SND.ToList();
            var snds = sndi.Entries_SNDS.ToList();

            if (scoobyBoot)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    if (!GetFromAssetID(v.Sound).assetName.Contains("thera"))
                        sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                    if (!GetFromAssetID(v.Sound).assetName.Contains("thera"))
                    {
                        int index = random.Next(0, sounds.Count);
                        v.SoundHeader = sounds[index].Item1;
                        ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                        sounds.RemoveAt(index);
                    }
            }
            else if (mixTypes)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }

                foreach (var v in snds)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }
            }
            else
            {
                List<(byte[], byte[])> soundsSND = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    soundsSND.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, soundsSND.Count);
                    v.SoundHeader = soundsSND[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = soundsSND[index].Item2;
                    soundsSND.RemoveAt(index);
                }

                List<(byte[], byte[])> soundsSNDS = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    soundsSNDS.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                {
                    int index = random.Next(0, soundsSNDS.Count);
                    v.SoundHeader = soundsSNDS[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = soundsSNDS[index].Item2;
                    soundsSNDS.RemoveAt(index);
                }
            }

            sndi.Entries_SND = snd.ToArray();
            sndi.Entries_SNDS = snds.ToArray();

            return true;
        }

        private bool RandomizeSNDI_GCN_V1(AssetSNDI_GCN_V1 sndi, bool mixTypes, bool scoobyBoot)
        {
            List<EntrySoundInfo_GCN_V1> snd = sndi.Entries_SND.ToList();
            List<EntrySoundInfo_GCN_V1> snds = sndi.Entries_SNDS.ToList();

            if (scoobyBoot)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    if (!GetFromAssetID(v.Sound).assetName.Contains("thera"))
                        sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                    if (!GetFromAssetID(v.Sound).assetName.Contains("thera"))
                    {
                        int index = random.Next(0, sounds.Count);
                        v.SoundHeader = sounds[index].Item1;
                        ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                        sounds.RemoveAt(index);
                    }
            }
            else if (mixTypes)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }

                foreach (var v in snds)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }
            }
            else
            {
                List<(byte[], byte[])> soundsSND = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    soundsSND.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, soundsSND.Count);
                    v.SoundHeader = soundsSND[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = soundsSND[index].Item2;
                    soundsSND.RemoveAt(index);
                }

                List<(byte[], byte[])> soundsSNDS = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    soundsSNDS.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.Sound)).Data));

                foreach (var v in snds)
                {
                    int index = random.Next(0, soundsSNDS.Count);
                    v.SoundHeader = soundsSNDS[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.Sound)).Data = soundsSNDS[index].Item2;
                    soundsSNDS.RemoveAt(index);
                }
            }

            sndi.Entries_SND = snd.ToArray();
            sndi.Entries_SNDS = snds.ToArray();

            return true;
        }

        private bool RandomizeSNDI_PS2(AssetSNDI_PS2 sndi, bool mixTypes, bool scoobyBoot)
        {
            var snd = sndi.Entries_SND.ToList();
            var snds = sndi.Entries_SNDS.ToList();

            if (scoobyBoot)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    if (!GetFromAssetID(v.SoundAssetID).assetName.Contains("thera"))
                        sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data));

                foreach (var v in snds)
                    if (!GetFromAssetID(v.SoundAssetID).assetName.Contains("thera"))
                    {
                        int index = random.Next(0, sounds.Count);
                        v.SoundHeader = sounds[index].Item1;
                        ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data = sounds[index].Item2;
                        sounds.RemoveAt(index);
                    }
            }
            else if (mixTypes)
            {
                List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data));

                foreach (var v in snds)
                    sounds.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }

                foreach (var v in snds)
                {
                    int index = random.Next(0, sounds.Count);
                    v.SoundHeader = sounds[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data = sounds[index].Item2;
                    sounds.RemoveAt(index);
                }
            }
            else
            {
                List<(byte[], byte[])> soundsSND = new List<(byte[], byte[])>();

                foreach (var v in snd)
                    soundsSND.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data));

                foreach (var v in snd)
                {
                    int index = random.Next(0, soundsSND.Count);
                    v.SoundHeader = soundsSND[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data = soundsSND[index].Item2;
                    soundsSND.RemoveAt(index);
                }

                List<(byte[], byte[])> soundsSNDS = new List<(byte[], byte[])>();

                foreach (var v in snds)
                    soundsSNDS.Add((v.SoundHeader, ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data));

                foreach (var v in snds)
                {
                    int index = random.Next(0, soundsSNDS.Count);
                    v.SoundHeader = soundsSNDS[index].Item1;
                    ((AssetWithData)GetFromAssetID(v.SoundAssetID)).Data = soundsSNDS[index].Item2;
                    soundsSNDS.RemoveAt(index);
                }
            }

            sndi.Entries_SND = snd.ToArray();
            sndi.Entries_SNDS = snds.ToArray();

            return true;
        }

        private bool RandomizeMovePointRadius(RandomizerSettings settings)
        {
            float min = settings.mvptMin;
            float max = settings.mvptMax;

            var assets = (from asset in assetDictionary.Values where asset.assetType == AssetType.MovePoint select asset).Cast<AssetMVPT>().ToList();

            foreach (var mvpt in assets)
            {
                if (mvpt.ArenaRadius != -1)
                    mvpt.ArenaRadius *= random.NextFloat(min, max);
                if (mvpt.game != Game.Scooby)
                {
                    if (mvpt.ZoneRadius != -1)
                        mvpt.ZoneRadius *= random.NextFloat(min, max);
                    if (mvpt.Delay != -1)
                        mvpt.Delay *= random.NextFloat(min, max);
                }
            }

            return assets.Count != 0;
        }

        private bool MakeLevelInvisible()
        {
            var assets = (from asset in assetDictionary.Values where asset.assetType == AssetType.JSP select (AssetJSP)asset).ToList();

            foreach (var a in assets)
            {
                try
                {
                    RWSection[] sections = ModelAsRWSections(a.Data, out int renderWareVersion);

                    foreach (var rws in sections)
                        if (rws is Clump_0010 clump)
                            foreach (var atomic in clump.atomicList)
                                atomic.atomicStruct.flags = AtomicFlags.None;

                    a.Data = ModelToRWSections(sections, renderWareVersion);
                }
                catch { }
            }

            return assets.Count != 0;
        }

        private bool MakeObjectsInvisible()
        {
            bool done = false;

            uint[] keepvisible = new uint[] {
                            Functions.BKDRHash("fruit_throw.MINF"),
                            Functions.BKDRHash("fruit_freezy_bind.MINF"),
                            Functions.BKDRHash("trailer_hitch")
                        };

            foreach (var asset in assetDictionary.Values)
                if (asset is AssetSIMP simp)
                {
                    if (keepvisible.Contains(simp.Model))
                        continue;

                    foreach (var link in simp.Links)
                        if (link.EventReceiveID.ToString().ToLower().Contains("hit"))
                            continue;

                    simp.ColorAlpha = 0f;
                    simp.VisibilityFlags.FlagValueByte = 0;
                    done = true;
                }

            return done;
        }

        public bool ImportCharacters()
        {
            bool patrick = false;
            bool sandy = false;

            switch (LevelName)
            {
                case "hb01":
                case "hb02":
                case "hb03":
                case "hb04":
                case "hb05":
                case "hb06":
                case "hb07":
                case "hb08":
                case "hb09":
                case "hb10":
                case "bc03":
                case "bc04":
                case "bc05":
                case "db01":
                case "db03":
                case "db04":
                case "db06":
                    patrick = sandy = true;
                    break;
                case "jf01":
                case "jf02":
                case "jf03":
                case "jf04":
                case "gl01":
                case "gl02":
                case "gl03":
                case "bc01":
                case "bc02":
                case "kf01":
                case "kf02":
                case "kf04":
                case "kf05":
                    sandy = true;
                    break;
                case "bb01":
                case "bb02":
                case "bb03":
                case "bb04":
                case "rb01":
                case "rb02":
                case "rb03":
                case "sm01":
                case "sm02":
                case "sm03":
                case "sm04":
                case "gy01":
                case "gy02":
                case "gy03":
                case "gy04":
                case "db02":
                    patrick = true;
                    break;
            }

            if (sandy)
                ProgImportHip("Utility", "sandy.hip");
            if (patrick)
                ProgImportHip("Utility", "patrick.hip");

            return patrick || sandy;
        }

        public bool UnimportCharacters()
        {
            switch (LevelName)
            {
                case "jf01":
                case "jf02":
                case "jf03":
                case "jf04":
                case "gl01":
                case "gl02":
                case "gl03":
                case "bc01":
                case "bc02":
                case "kf01":
                case "kf02":
                case "kf04":
                case "kf05":
                case "b101":
                    ProgUnimportHip("Utility", "patrick.hip");
                    break;
                case "bb01":
                case "bb02":
                case "bb03":
                case "bb04":
                case "rb01":
                case "rb02":
                case "rb03":
                case "sm01":
                case "sm02":
                case "sm03":
                case "sm04":
                case "gy01":
                case "gy02":
                case "gy03":
                case "gy04":
                case "db02":
                case "b201":
                    ProgUnimportHip("Utility", "sandy.hip");
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool FixTreedome()
        {
            if (LevelName == "hb01")
            {
                AssetPKUP spatula = (AssetPKUP)PlaceTemplate(new Vector3(8.774022f, 5.877692f, -23.492590f), "SPATULA_EXTRA_FIX", AssetTemplate.Spatula);
                spatula.Links = new Link[]
                {
                    new Link(game)
                    {
                        EventReceiveID = (ushort)EventBFBB.Pickup,
                        EventSendID = (ushort)EventBFBB.Count2,
                        TargetAsset = 0x5F45B82A,
                    }
                };

                ((AssetDYNA)GetFromAssetID(new AssetID("EXIT_TALKBOX_HB05"))).Links = new Link[0];
                ((AssetTEXT)GetFromAssetID(new AssetID("exit_hb05_text"))).Text = "You have been forbidden from entering this level :3.";

                return true;
            }
            return false;
        }

        public bool KillFinalBossCutscenes()
        {
            switch (LevelName)
            {
                case "b302":
                    uint dpat = new AssetID("BOSS3_OPEN_DISP");
                    if (ContainsAsset(dpat) && GetFromAssetID(dpat) is AssetDPAT dispatcher)
                    {
                        dispatcher.EnabledOnStart = false;
                        AssetTIMR timer = (AssetTIMR)PlaceTemplate(AssetTemplate.Timer);
                        timer.Time = 1f;
                        uint boss = new AssetID("BOSS_NPC");
                        if (ContainsAsset(boss) && GetFromAssetID(boss) is AssetVIL spongebot)
                        {
                            uint transCut = new AssetID("BOSS3_TRANSITION_CSNMANAGER");
                            Link[] links = spongebot.Links;
                            for (int i = 0; i < links.Length; i++)
                                if (links[i].EventSendID == (ushort)EventBFBB.Preload)
                                {
                                    links[i].TargetAsset = timer.assetID;
                                    links[i].EventSendID = (ushort)EventBFBB.Run;
                                }
                            spongebot.Links = links;
                            if (ContainsAsset(transCut) && GetFromAssetID(transCut) is AssetCSNM transitionCutscene)
                            {
                                Link[] cutLinks = transitionCutscene.Links;
                                for (int i = 0; i < cutLinks.Length; i++)
                                    if (cutLinks[i].EventReceiveID == (ushort)EventBFBB.Play)
                                        cutLinks[i].EventReceiveID = (ushort)EventBFBB.Run;
                                    else if (cutLinks[i].EventReceiveID == (ushort)EventBFBB.Done)
                                        cutLinks[i].EventReceiveID = (ushort)EventBFBB.Expired;
                                timer.Links = cutLinks;
                            }
                        }
                        return true;
                    }
                    break;
                case "b303":
                    uint cutTimerAssetID = new AssetID("CUTSCN_TIMER");
                    if (ContainsAsset(cutTimerAssetID) && GetFromAssetID(cutTimerAssetID) is AssetTIMR cutTimer)
                    {
                        List<Link> links = cutTimer.Links.ToList();
                        uint cut = new AssetID("WIN_GAME_CTSNMANAGER");
                        if (ContainsAsset(cut) && GetFromAssetID(cut) is AssetCSNM cutManager)
                        {
                            links.AddRange(cutManager.Links);
                            for (int i = 0; i < links.Count; i++)
                                if (links[i].EventSendID == (ushort)EventBFBB.Preload)
                                    links.RemoveAt(i--);
                                else if (links[i].EventReceiveID == (ushort)EventBFBB.Play)
                                    links[i].EventReceiveID = (ushort)EventBFBB.Run;
                                else if (links[i].EventReceiveID == (ushort)EventBFBB.Done)
                                    links[i].EventReceiveID = (ushort)EventBFBB.Expired;
                            cutTimer.Links = links.ToArray();
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void RemoveIfContains(uint assetID)
        {
            if (ContainsAsset(assetID))
                RemoveAsset(assetID);
        }

        public bool UnimportEnemies(HashSet<NpcType_BFBB> enemyVils)
        {
            foreach (NpcType_BFBB v in enemyVils)
            {
                string hipFileName;
                switch (v)
                {
                    case NpcType_BFBB.g_love_bind:
                        hipFileName = "g-love.HIP";
                        break;
                    case NpcType_BFBB.ham_bind:
                        hipFileName = "ham-mer.HIP";
                        break;
                    case NpcType_BFBB.robot_0a_bomb_bind:
                        hipFileName = "bomb-bot.HIP";
                        break;
                    case NpcType_BFBB.robot_0a_bzzt_bind:
                        hipFileName = "bzzt-bot.HIP";
                        break;
                    case NpcType_BFBB.robot_0a_chomper_bind:
                        hipFileName = "chomp-bot.HIP";
                        break;
                    case NpcType_BFBB.robot_0a_fodder_bind:
                        hipFileName = "fodder.HIP";
                        break;
                    case NpcType_BFBB.robot_4a_monsoon_bind:
                        hipFileName = "monsoon.HIP";
                        break;
                    case NpcType_BFBB.robot_9a_bind:
                        hipFileName = "slick.HIP";
                        break;
                    case NpcType_BFBB.robot_chuck_bind:
                        hipFileName = "chuck.HIP";
                        break;
                    case NpcType_BFBB.robot_sleepytime_bind:
                        hipFileName = "sleepytime.HIP";
                        break;
                    case NpcType_BFBB.robot_tar_bind:
                        hipFileName = "tar-tar.HIP";
                        break;
                    case NpcType_BFBB.robot_arf_bind:
                        hipFileName = "arf_arf-dawg.HIP";
                        break;
                    case NpcType_BFBB.tubelet_bind:
                        hipFileName = "tubelet.HIP";
                        break;
                    case NpcType_BFBB.tiki_wooden_bind:
                        hipFileName = "tiki_wooden.HIP";
                        break;
                    case NpcType_BFBB.tiki_lovey_dovey_bind:
                        hipFileName = "tiki_floating.HIP";
                        break;
                    case NpcType_BFBB.tiki_thunder_bind:
                        hipFileName = "tiki_thunder.HIP";
                        break;
                    case NpcType_BFBB.tiki_stone_bind:
                        hipFileName = "tiki_stone.HIP";
                        break;
                    case NpcType_BFBB.tiki_shhhh_bind:
                        hipFileName = "tiki_shhh.HIP";
                        break;
                    default:
                        throw new Exception("Invalid VilType");
                }

                string folderName = v.ToString().Contains("tiki") ? "Utility" : "Enemies";
                ProgUnimportHip(folderName, hipFileName);
            }

            return true;
        }

        private static readonly NpcType_BFBB[] importVilTypes = new NpcType_BFBB[] {
            NpcType_BFBB.g_love_bind,
            NpcType_BFBB.ham_bind,
            NpcType_BFBB.robot_0a_bomb_bind,
            NpcType_BFBB.robot_0a_bzzt_bind,
            NpcType_BFBB.robot_0a_chomper_bind,
            NpcType_BFBB.robot_0a_fodder_bind,
            NpcType_BFBB.robot_4a_monsoon_bind,
            NpcType_BFBB.robot_9a_bind,
            NpcType_BFBB.robot_chuck_bind,
            NpcType_BFBB.robot_sleepytime_bind,
            NpcType_BFBB.robot_tar_bind,
            NpcType_BFBB.robot_arf_bind,
            NpcType_BFBB.tubelet_bind,
            NpcType_BFBB.tiki_lovey_dovey_bind,
            NpcType_BFBB.tiki_shhhh_bind,
            NpcType_BFBB.tiki_stone_bind,
            NpcType_BFBB.tiki_thunder_bind,
            NpcType_BFBB.tiki_wooden_bind
        };

        public HashSet<NpcType_BFBB> GetEnemyTypes()
        {
            HashSet<NpcType_BFBB> outSet = new HashSet<NpcType_BFBB>();
            foreach (AssetVIL a in (from asset in assetDictionary.Values
                                    where asset is AssetVIL vil && importVilTypes.Contains(vil.NpcType_BFBB)
                                    select asset).Cast<AssetVIL>())
            {
                if (!ContainsAsset(a.Model))
                    outSet.Add(a.NpcType_BFBB);
            }
            return outSet;
        }

        public bool ImportNumbers()
        {
            ProgImportHip("Utility", "numbers.hip");
            return true;
        }

        public bool RestoreRobotLaugh()
        {
            uint assetID = new AssetID("RSB_laugh_8");
            RemoveAsset(assetID);
            ProgImportHip("Utility", "robot_laugh.HIP");
            return true;
        }

        public bool WidescreenMenu()
        {
            foreach (var u in new uint[] {
                new AssetID("AAA MNU4 CAUSTICS SURFACE UI"),
                new AssetID("BLUE ALPHA 1 BAMBOO UI"),
                new AssetID("BLUE ALPHA 2 BAMBOO UI"),
                new AssetID("BLUE ALPHA 3 BAMBOO UI"),
            })
            {
                var ui = (AssetUI)GetFromAssetID(u);
                ui.ScaleX *= 4f / 3f;
            }

            foreach (var u in new uint[] {
                new AssetID("BLUE ALPHA 1 UI"),
                new AssetID("BLUE ALPHA 2 UI"),
                new AssetID("BLUE ALPHA 3 UI"),
            })
            {
                var ui = (AssetUI)GetFromAssetID(u);
                ui.Width = (short)(ui.Width * 4f / 3f);
                ui.PositionX -= 107f;
            }

            return true;
        }

        public void ProgImportHip(string folderName, string fileName)
        {
            string gameName = game == Game.BFBB ? "BattleForBikiniBottom" : "MovieGame";
            ImportHip(Path.Combine(editorFilesFolder, gameName, platform.ToString(), folderName, fileName), true);
        }

        private void ProgUnimportHip(string folderName, string fileName)
        {
            string gameName = game == Game.BFBB ? "BattleForBikiniBottom" : "MovieGame";
            UnimportHip(HipFile.FromPath(Path.Combine(editorFilesFolder, gameName, platform.ToString(), folderName, fileName)).Item1.DICT);
        }

        public void UnimportHip(Section_DICT dict)
        {
            UnsavedChanges = true;

            foreach (Section_AHDR AHDR in dict.ATOC.AHDRList)
            {
                switch (AHDR.assetType)
                {
                    case AssetType.CollisionTable:
                    case AssetType.JawDataTable:
                    case AssetType.LevelOfDetailTable:
                    case AssetType.ShadowTable:
                    case AssetType.SoundInfo:
                        continue;
                }

                RemoveIfContains(AHDR.assetID);
            }
        }

        private string LevelName => Path.GetFileNameWithoutExtension(currentlyOpenFilePath).ToLower();

        private bool IsWarpToSameLevel(string warpName) =>
            LevelName.ToLower().Equals(warpName.ToLower()) || new string(warpName.Reverse().ToArray()).ToLower().Equals(LevelName.ToLower());

        public List<string> GetWarpNames(List<string> toSkip)
        {
            warpsRandomizer = new List<AssetPORT>();
            List<string> warpNames = new List<string>();
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPORT port && !IsWarpToSameLevel(port.DestinationLevel) && !PortInToSkip(port, toSkip))
                {
                    warpsRandomizer.Add(port);
                    warpNames.Add(port.DestinationLevel);
                }
            return warpNames;
        }

        private List<AssetPORT> warpsRandomizer;

        private bool PortInToSkip(AssetPORT port, List<string> toSkip)
        {
            string dest = port.DestinationLevel.ToLower();

            if (game == Game.BFBB)
                switch (LevelName)
                {
                    case "bb01":
                        if (port.assetName == "TOHB01")
                            return true;
                        else if (dest == "bb03")
                            return true;
                        break;
                    case "bc01":
                        if (dest == "hb01")
                            return true;
                        break;
                    case "bc02":
                        if (dest == "bc04")
                            return true;
                        break;
                    case "hb01":
                        if (dest == "tl01")
                            return true;
                        else if (port.assetName == "TOJF01")
                            return true;
                        else if (port.assetName == "TOBB01")
                            return true;
                        else if (port.assetName == "TOGL01")
                            return true;
                        else if (port.assetName == "TOBC01")
                            return true;
                        break;
                    case "jf01":
                        if (port.assetName == "TAXISTAND_PORTAL_01")
                            return true;
                        break;
                    case "jf02":
                        if (dest == "jf04")
                            return true;
                        break;
                    case "jf04":
                        if (dest == "jf02")
                            return true;
                        else if (dest == "jf80")
                            return true;
                        break;
                    case "rb01":
                        if (port.assetName == "TOHUB_PORTAL")
                            return true;
                        break;
                    case "sm01":
                        if (port.assetName == "TOHB01")
                            return true;
                        break;
                }
            else if (game == Game.Scooby)
                switch (LevelName)
                {
                    case "g001":
                        if (dest == "h001")
                            return true;
                        break;
                    case "h001":
                        if (dest == "b001" || dest == "c001" || dest == "g001" || dest == "l011" || dest == "mnu5" ||
                            dest == "o001" || dest == "p001" || dest == "r001" || dest == "s001" || dest == "w020")
                            return true;
                        break;
                    case "h002":
                        if (dest != "h001")
                            return true;
                        break;
                    case "i001":
                        if (dest != "h001" || dest != "i002" || dest != "r001")
                            return true;
                        break;
                    case "i020":
                        if (dest == "i003")
                            return true;
                        break;
                    case "i005":
                        if (dest == "r001")
                            return true;
                        break;
                    case "i006":
                        if (dest == "i004" || port.assetName == "TOR001A")
                            return true;
                        break;
                    case "o006":
                        if (dest == "h001")
                            return true;
                        break;
                    case "p001":
                        if (dest == "unti")
                            return true;
                        break;
                    case "p004":
                        if (port.assetName == "TOP005A")
                            return true;
                        break;
                    case "p005":
                        if (port.assetName == "TOP004A")
                            return true;
                        break;
                    case "r001":
                        if (port.assetName == "TOI005A")
                            return true;
                        break;
                    case "r020":
                        if (dest == "r003")
                            return true;
                        break;
                    case "r021":
                        if (dest == "r001")
                            return true;
                        break;
                    case "w027":
                    case "w028":
                        if (dest == "w024")
                            return true;
                        break;
                    case "s005":
                        return true;
                }
            else if (game == Game.Incredibles)
                switch (LevelName)
                {
                    case "b402":
                        if (port.assetName == "B402_PORTAL")
                            return true;
                        break;
                    case "jk01":
                        if (port.assetName == "FB03_PORTAL")
                            return true;
                        break;
                    case "jk02":
                        if (port.assetName == "MINDY_PORTAL_01")
                            return true;
                        if (port.assetName == "RINGRACE_PORTAL")
                            return true;
                        break;
                    case "pt01":
                        if (port.assetName == "PT02_PORTAL_01")
                            return true;
                        break;
                    case "pt02":
                        if (port.assetName == "PORTAL_TO_PT01")
                            return true;
                        break;
                    case "sc02":
                    case "tr01":
                        if (port.assetName == "TO_FINISH_PORTAL")
                            return true;
                        if (port.assetName == "TO_MINDY_PORTAL")
                            return true;
                        if (port.assetName == "TO_NEUTRAL_PORTAL")
                            return true;
                        if (port.assetName == "TO_START_PORTAL")
                            return true;
                        break;
                }

            foreach (string s in toSkip)
                if (dest.Contains(s.ToLower()))
                    return true;
                else if (new string(dest.ToArray()).ToLower().Contains(s.ToLower()))
                    return true;

            return false;
        }

        public bool SetWarpNames(ref List<string> warpNames, ref List<(string, string, string)> warpRandomizerOutput, HashSet<string> unique)
        {
            foreach (AssetPORT port in warpsRandomizer)
            {
                if (warpNames.Count == 0)
                    throw new Exception("warpNames is empty");

                int index;
                int times = 0;
                do
                {
                    index = random.Next(0, warpNames.Count);
                    times++;

                    if (times > 500)
                    {
                        warpNames.Clear();
                        warpNames.AddRange(unique);
                        times = 0;
                    }
                }
                while (IsWarpToSameLevel(warpNames[index]));

                warpRandomizerOutput.Add((LevelName.ToUpper(), port.DestinationLevel, warpNames[index]));

                port.DestinationLevel = warpNames[index];

                warpNames.RemoveAt(index);

                if (warpNames.Count == 0)
                    warpNames.AddRange(unique);
            }

            return warpsRandomizer.Count != 0;
        }

        private string MusicDispAssetName => "IP_RANDO_DISP";
        private string MusicGroupAssetName => "IP_RANDO_GROUP";

        public bool RandomizePlaylistLocal()
        {
            string musicDispAssetName = "MUSIC_DISP";

            if (ContainsAsset(new AssetID(musicDispAssetName)))
            {
                List<uint> assetIDs = FindWhoTargets(new AssetID(musicDispAssetName));
                foreach (uint assetID in assetIDs)
                {
                    BaseAsset objectAsset = (BaseAsset)GetFromAssetID(assetID);
                    Link[] links = objectAsset.Links;
                    foreach (Link link in links)
                        if (link.EventSendID == (ushort)EventBFBB.PlayMusic)
                        {
                            link.EventSendID = (ushort)EventBFBB.Run;
                            link.TargetAsset = MusicGroupAssetName + "_01";
                        }
                    objectAsset.Links = links;
                }

                return true;
            }
            return false;
        }

        public bool RandomizePlaylist()
        {
            if (ContainsAsset(new AssetID(MusicDispAssetName + "_01")) && ContainsAsset(new AssetID(MusicGroupAssetName + "_01")))
                return false;

            var dpat = PlaceTemplate(MusicDispAssetName, AssetTemplate.Dispatcher);

            var group = (AssetGRUP)PlaceTemplate(MusicGroupAssetName, AssetTemplate.Group);
            group.ReceiveEventDelegation = Delegation.RandomItem;
            group.Links = new Link[]
            {
                new Link(game)
                {
                    EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                    EventSendID = (ushort)EventBFBB.Run,
                    TargetAsset = group.assetID
                }
            };

            var outAssetIDs = new List<uint>();
            for (int i = 0; i < 17; i++)
            {
                if (i == 7 || i == 14)
                    continue;

                var timer = (AssetTIMR)PlaceTemplate(new Vector3(), ref outAssetIDs, "IP_RANDO_TIMR", template: AssetTemplate.Timer);
                timer.Time = 0.1f;
                var links = new List<Link>()
                {
                    new Link(game)
                    {
                        FloatParameter1 = i,
                        EventReceiveID = (ushort)EventBFBB.Expired,
                        EventSendID = (ushort)EventBFBB.PlayMusic,
                        TargetAsset = dpat.assetID
                    },
                    new Link(game)
                    {
                        EventReceiveID = (ushort)EventBFBB.Expired,
                        EventSendID = (ushort)EventBFBB.Reset,
                        TargetAsset = timer.assetID
                    },
                };

                timer.Links = links.ToArray();
            }

            List<AssetID> assetIDs = new List<AssetID>();
            foreach (uint i in outAssetIDs)
                assetIDs.Add(new AssetID(i));
            group.Items = assetIDs.ToArray();

            return true;
        }

        public bool MultiplyLODT(float value)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetLODT lodt && lodt.assetFileName != "lodt_doubled")
                {
                    lodt.assetFileName = "lodt_doubled";

                    EntryLODT[] lodtEntries = lodt.Entries;
                    foreach (var t in lodtEntries)
                    {
                        t.MaxDistance *= value;
                        t.LOD1_MinDistance *= value;
                        t.LOD2_MinDistance *= value;
                        t.LOD3_MinDistance *= value;
                    }

                    lodt.Entries = lodtEntries;
                }

            return true;
        }

        private bool ShuffleSpatulaGates(bool shuffleSpatulaGates, RandomizerSettings settings, out bool needToAddNumbers)
        {
            needToAddNumbers = false;

            switch (LevelName)
            {
                case "hb01":
                {
                    if (!shuffleSpatulaGates)
                        return false;
                    // Downtown
                    {
                        uint spatMechAssetID = new AssetID("SPATULA_BB_MECH_01");
                        if (ContainsAsset(spatMechAssetID))
                        {
                            ((AssetPLAT)GetFromAssetID(spatMechAssetID)).PositionX = 87.315510f;
                            ((AssetPLAT)GetFromAssetID(spatMechAssetID)).PositionZ = 10.411270f;
                        }

                        List<uint> platAssetIDs = new List<uint>();

                        uint numRightAssetID = new AssetID("NUMBER_5_BB_MECH_01");
                        if (ContainsAsset(numRightAssetID))
                        {
                            AssetPLAT plat = (AssetPLAT)GetFromAssetID(numRightAssetID);
                            plat.PositionX = 87.481760f;
                            plat.PositionZ = 9.643267f;

                            string serializedObject = JsonConvert.SerializeObject(plat.BuildAHDR(platform.Endianness()));
                            Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                            var plat2 = (AssetPLAT)AddAssetWithUniqueID(AHDR, game, platform.Endianness());

                            plat2.PositionX = 87.692600f;
                            plat2.PositionZ = 8.692189f;

                            platAssetIDs.Add(numRightAssetID);
                            platAssetIDs.Add(plat2.assetID);

                            plat.Links = new Link[] { new Link(game)
                                {
                                    EventReceiveID = (ushort)EventBFBB.Invisible,
                                    EventSendID = (ushort)EventBFBB.Invisible,
                                    TargetAsset = plat2.assetID,
                                    FloatParameter1 = 77,
                                }
                                };
                        }

                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_BB_COND_01"));
                        int i = 0;
                        foreach (uint u in platAssetIDs)
                            SetNumberPlats(value, i++, u);

                        ReplaceInText(new AssetID("Spatula_exit_bb01_text"), "5", value.ToString());
                    }

                    // GL
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_GL_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_0_GL_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_1_GL_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_gl01_text"), "10", value.ToString());

                        ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_GL_MECH_01"))).Yaw += 180f;
                    }
                    // H2
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_H2_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_5_H2_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_1_H2_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_hub2_text"), "15", value.ToString());
                    }
                    // RB
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_RB_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_5_RB_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_2_RB_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_rb01_text"), "25", value.ToString());
                    }
                    // SM
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_SM_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_0_SM_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_3_SM_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_sm01_text"), "30", value.ToString());

                        ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_SM_MECH_01"))).Yaw += 180f;
                    }
                    // H3
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_H3_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_0_H3_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_4_H3_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_hub3_text"), "40", value.ToString());

                        ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_H3_MECH_01"))).Yaw += 180f;
                    }
                    // KF
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_KF_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_0_KF_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_5_KF_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_kf01_text"), "50", value.ToString());

                        ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_KF_MECH_01"))).Yaw += 180f;
                    }
                    // GY
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);
                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_GY_COND_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_0_GY_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_6_GY_MECH_01"));
                        ReplaceInText(new AssetID("Spatula_exit_gy01_text"), "60", value.ToString());
                    }

                    needToAddNumbers = true;
                }
                break;
                case "hb08":
                {
                    if (ContainsAssetWithType(AssetType.Conditional))
                    {
                        int value = random.Next(settings.spatReqMin, settings.spatReqMax + 1);

                        if (settings.spatReqChum != -1)
                            value = settings.spatReqChum;

                        SetCondEvaluationAmount(value - 1, new AssetID("TOLL_DOOR_CONDITIONAL_01"));
                        SetNumberPlats(value, 0, new AssetID("NUMBER_5_MECH_01"));
                        SetNumberPlats(value, 1, new AssetID("NUMBER_7_MECH_01"));
                        ReplaceInText(new AssetID("exit_b301_denial_text"), "75", value.ToString());
                        ReplaceInText(new AssetID("exit_b301_description_text"), "75", value.ToString());

                        if (value != 75)
                            needToAddNumbers = true;
                    }
                }
                break;
                default:
                    return false;
            }

            return true;
        }

        private bool ShuffleShinyGates(RandomizerSettings settings, out bool needToAddNumbers)
        {
            needToAddNumbers = false;

            switch (LevelName)
            {
                case "bb01":
                case "bb02":
                {
                    int originalShinyAmount = 2100;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_COND"),
                                new AssetID("SHINYGATE_FORCE_COND_01"),
                        },
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_TALKBOX"),
                                new AssetID("SHINYGATE_1_FORCE" + (LevelName ==  "bb01" ? "D" : "") + "_TALKBOX"),
                        },
                        new List<uint>
                        {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                        },
                        new List<uint>
                        {
                                new AssetID("shinyobjectgate_1_text"),
                                new AssetID("shinyobjectgate_force_text"),
                                new AssetID(0xCD5C904B),
                                new AssetID("shinyobjectgate_notenough_text"),
                        });
                }
                break;
                case "bc02":
                {
                    int originalShinyAmount = 2300;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_2_COND"),
                                new AssetID("SHINYGATE_2_FORCE_COND"),
                        },
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_2_TALKBOX"),
                                new AssetID("SHINYGATE_2_FORCE_TALKBOX"),
                        },
                        new List<uint>
                        {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                        },
                        new List<uint>
                        {
                                new AssetID(0x03A7AC42),
                                new AssetID("shinyobjectgate_patteeter_text")
                        });
                    needToAddNumbers = true;
                }
                break;
                case "bc03":
                {
                    int originalShinyAmount = 2300;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_COND"),
                                new AssetID("SHINYGATE_1_FORCE_COND"),
                        },
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_TALKBOX"),
                                new AssetID("SHINYGATE_1_FORCE_TALKBOX"),
                        },
                        new List<uint>
                        {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                        },
                        new List<uint>
                        {
                                new AssetID("shinyobjectgate_highpath_text"),
                                new AssetID("shinyobjectgate_notenough_text"),
                        });
                    needToAddNumbers = true;
                }
                break;
                case "db01":
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        List<uint> numberPlats;

                        if (i == 1)
                            numberPlats = new List<uint> {
                                    new AssetID("CLAMGATE_SHINY_MECH_05"),
                                    new AssetID("CLAMGATE_SHINY_MECH_04"),
                                    new AssetID("CLAMGATE_SHINY_MECH_03"),
                                    new AssetID("CLAMGATE_SHINY_MECH_02")};
                        else if (i == 2)
                            numberPlats = new List<uint> {
                                    new AssetID("CLAMGATE_SHINY_MECH_09"),
                                    new AssetID("CLAMGATE_SHINY_MECH_08"),
                                    new AssetID("CLAMGATE_SHINY_MECH_07"),
                                    new AssetID("CLAMGATE_SHINY_MECH_06")};
                        else
                            numberPlats = new List<uint> {
                                    new AssetID("CLAMGATE_SHINY_MECH_14"),
                                    new AssetID("CLAMGATE_SHINY_MECH_13"),
                                    new AssetID("CLAMGATE_SHINY_MECH_12"),
                                    new AssetID("CLAMGATE_SHINY_MECH_11")};

                        int originalShinyAmount = 1000;
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                            new List<uint>
                            {
                                    new AssetID("SHINYGATE_" + i.ToString() + "_COND"),
                                    new AssetID("SHINYGATE_" + i.ToString() + "_FORCE_COND"),
                            },
                            new List<uint>
                            {
                                    new AssetID("SHINYGATE_" + i.ToString() + "_TALKBOX"),
                                    new AssetID("SHINYGATE_" + i.ToString() + "_FORCE_TALKBOX"),
                            },
                            numberPlats,
                            new List<uint>
                            {
                                    new AssetID("shinyobjectgate_path_text_" + i.ToString()),
                                    new AssetID((uint)(0x03A7AC40 + i)),
                            });
                    }
                    needToAddNumbers = true;
                }
                break;
                case "db02":
                {
                    int originalShinyAmount = 2800;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_COND"),
                                new AssetID("SHINYGATE_FORCE_COND_01"),
                        },
                        new List<uint>
                        {
                                new AssetID("SHINYGATE_1_TALKBOX"),
                        },
                        new List<uint>
                        {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                        },
                        new List<uint>
                        {
                                new AssetID("shinyobjectgate_notenough_text"),
                                new AssetID(0xCD5C904B),
                                new AssetID("shinyobjectgate_force_text"),
                                new AssetID("shinyobjectgate_1_text"),
                        });
                    needToAddNumbers = true;
                }
                break;
                case "gl01":
                {
                    int originalShinyAmount = 2200;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_CASTLE_COND"),
                                new AssetID("SHINYGATE_FORCE_COND"),
                         },
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_CASTLE_TALKBOX"),
                                new AssetID("SHINYGATE_CASTLE_FORCE_TALKBOX"),
                         },
                         new List<uint>
                         {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                         },
                         new List<uint>
                         {
                                new AssetID("shinyobjectgate_castle_text"),
                                new AssetID("shinyobjectgate_notenough_text"),
                         });
                    needToAddNumbers = true;
                }
                break;
                case "gl03":
                {
                    int originalShinyAmount = 2200;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_1_COND"),
                                new AssetID("SHINYGATE_1_FORCE_COND"),
                         },
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_1_TALKBOX"),
                                new AssetID("SHINYGATE_1_FORCE_TALKBOX"),
                         },
                         new List<uint>
                         {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                         },
                         new List<uint>
                         {
                                new AssetID("shinyobjectgate_bungee_text"),
                                new AssetID("shinyobjectgate_notenough_text"),
                         });
                    needToAddNumbers = true;
                }
                break;
                case "gy01":
                {
                    int originalShinyAmount = 2700;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                               new AssetID("SHINYGATE_CHEST_COND"),
                               new AssetID("SHINYGATE_CHEST_FORCE_COND"),
                        },
                        new List<uint>
                        {
                               new AssetID("SHINYGATE_CHEST_TALKBOX"),
                               new AssetID("SHINYGATE_CHEST_FORCE_TALKBOX"),
                        },
                        new List<uint>
                        {
                               new AssetID("CLAMGATE_SHINY_MECH_05"),
                               new AssetID("CLAMGATE_SHINY_MECH_04"),
                               new AssetID("CLAMGATE_SHINY_MECH_03"),
                               new AssetID("CLAMGATE_SHINY_MECH_02"),
                        },
                        new List<uint>
                        {
                               new AssetID("shinyobjectgate_chest_text"),
                               new AssetID("shinyobjectgate_notenough_text"),
                        });
                    needToAddNumbers = true;
                }
                break;
                case "gy02":
                {
                    int originalShinyAmount = 2700;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_COND_1"),
                                new AssetID("SHINYGATE_FORCE_COND"),
                         },
                         new List<uint>
                         {
                                new AssetID("SHINYGATE_BUNGEE_TALKBOX"),
                                new AssetID("SHINYGATE_BUNGEE_FORCE_TALKBOX"),
                         },
                         new List<uint>
                         {
                                new AssetID("CLAMGATE_SHINY_MECH_05"),
                                new AssetID("CLAMGATE_SHINY_MECH_04"),
                                new AssetID("CLAMGATE_SHINY_MECH_03"),
                                new AssetID("CLAMGATE_SHINY_MECH_02"),
                         },
                         new List<uint>
                         {
                                new AssetID(0x230ED0ED),
                                new AssetID("shinyobjectgate_bungee_text"),
                         });
                    needToAddNumbers = true;
                }
                break;
                case "hb01":
                {
                    int originalShinyAmount = 40000;
                    int shinyAmount = (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax));
                    SetGate(originalShinyAmount, shinyAmount,
                        new List<uint> { new AssetID("THEATER_CONDIT_01"), },
                        new List<uint> { new AssetID("BOGUY_TALKBOX_01"), },
                        new List<uint> {
                                new AssetID("THEATRE_MECH_06"),
                                new AssetID("THEATRE_MECH_05"),
                                new AssetID("THEATRE_MECH_04"),
                                new AssetID("THEATRE_MECH_03"),
                                new AssetID("THEATRE_MECH_02"),
                         },
                         new List<uint>());

                    string fortyThousand = "40,000";

                    ReplaceInText(0x0001A923, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0x576E065E, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0xD4F84AE7, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0x4C5ECE3F, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0xEFFC2BB5, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0xEFFC2BB6, fortyThousand, shinyAmount.ToString());
                    ReplaceInText(0x65BF2EE7, fortyThousand, shinyAmount.ToString());

                    needToAddNumbers = true;
                }
                break;
                case "hb02":
                {
                    int originalShinyAmount50 = 50;
                    SetGate(originalShinyAmount50, (int)(originalShinyAmount50 * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint> { new AssetID("SOGATE_COND_01"), },
                         new List<uint> { new AssetID("SOGATE_COND_01"), },
                         new List<uint> { new AssetID("SHINY_OBJ_MECH_04"), new AssetID("SHINY_OBJ_MECH_03"), },
                         new List<uint> { 0xB5FF7865, 0x9AA7AE41, });

                    int originalShinyAmount10 = 10;
                    SetGate(originalShinyAmount10, (int)(originalShinyAmount10 * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint> { new AssetID("SOGATE_COND_02"), },
                         new List<uint> { new AssetID("SOGATE_COND_02"), },
                         new List<uint> { new AssetID("SHINY_OBJ_MECH_07"), new AssetID("SHINY_OBJ_MECH_06"), },
                         new List<uint> { 0x21BA9BE1, 0x23CE2B75, });

                    int originalShinyAmount20 = 20;
                    SetGate(originalShinyAmount20, (int)(originalShinyAmount20 * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint> { new AssetID("SOGATE_COND_03"), },
                         new List<uint> { new AssetID("SOGATE_COND_03"), },
                         new List<uint> { new AssetID("SHINY_OBJ_MECH_10"), new AssetID("SHINY_OBJ_MECH_09"), },
                         new List<uint> { 0x21BA9BE2, 0x23CE2B76, });
                    needToAddNumbers = true;
                }
                break;
                case "jf01":
                {
                    int originalShinyAmount = 125;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint>
                         {
                                            new AssetID("SHINYGATE_1_COND"),
                                            new AssetID("SHINYGATE_FORCE_COND_01"),
                         },
                         new List<uint>
                         {
                                            new AssetID("SHINYGATE_1_TALKBOX"),
                         },
                         new List<uint>
                         {
                                            new AssetID("CLAMGATE_SHINY_MECH_04"),
                                            new AssetID("CLAMGATE_SHINY_MECH_03"),
                                            new AssetID("CLAMGATE_SHINY_MECH_02"),
                         },
                         new List<uint>
                         {
                                            new AssetID("shinyobjectgate_notenough_text"),
                                            new AssetID(0xCD5C904B),
                                            new AssetID("shinyobjectgate_1_text"),
                                            new AssetID("shinyobjectgate_force_text"),
                         });
                }
                break;
                case "jf03":
                {
                    int originalShinyAmount = 2000;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                         new List<uint>
                         {
                                            new AssetID("SHINYGATE_1_COND"),
                                            new AssetID("SHINYGATE_FORCE_COND_01"),
                         },
                         new List<uint>
                         {
                                            new AssetID("SHINYGATE_1_TALKBOX"),
                         },
                         new List<uint>
                         {
                                            new AssetID("SO_NUMBER_0_MECH_03"),
                                            new AssetID("SO_NUMBER_0_MECH_02"),
                                            new AssetID("SO_NUMBER_0_MECH_01"),
                                            new AssetID("SO_NUMBER_2_MECH"),
                         },
                         new List<uint>
                         {
                                            new AssetID("shinyobjectgate_notenough_text"),
                                            new AssetID(0xCD5C904B),
                                            new AssetID("shinyobjectgate_1_text"),
                                            new AssetID("shinyobjectgate_force_text"),
                         });
                    needToAddNumbers = true;
                }
                break;
                case "kf02":
                {
                    int originalShinyAmount = 2600;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_COND"),
                                            new AssetID("SHINYGATE_FORCE_COND_01"),
                        },
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_TALKBOX"),
                        },
                        new List<uint>
                        {
                                             new AssetID("SO_NUMBER_ 0_MECH_03"),
                                             new AssetID("SO_NUMBER_ 0_MECH_02"),
                                             new AssetID("SO_NUMBER_6_MECH_01"),
                                             new AssetID("SO_NUMBER_ 2_MECH"),
                        },
                        new List<uint>
                        {
                                            new AssetID("shinyobjectgate_notenough_text"),
                                            new AssetID(0xCD5C904B),
                                            new AssetID("shinyobjectgate_1_text"),
                                            new AssetID("shinyobjectgate_force_text"),
                        });
                    needToAddNumbers = true;
                }
                break;
                case "rb03":
                {
                    int originalShinyAmount = 2400;
                    SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_COND"),
                                            new AssetID("SHINYGATE_FORCE_COND_01"),
                        },
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_TALKBOX"),
                        },
                        new List<uint>
                        {
                                            new AssetID("SO_NUMBER_0_MECH_01"),
                                            new AssetID("SO_NUMBER_0_MECH_02"),
                                            new AssetID("SO_NUMBER_4_MECH"),
                                            new AssetID("SO_NUMBER_2_MECH_01"),
                        },
                        new List<uint>
                        {
                                            new AssetID("shinyobjectgate_notenough_text"),
                                            new AssetID(0xCD5C904B),
                                            new AssetID("shinyobjectgate_1_text"),
                                            new AssetID("shinyobjectgate_force_text"),
                        });
                    needToAddNumbers = true;
                }
                break;
                case "sm01":
                {
                    for (int i = 2; i <= 4; i++)
                    {
                        int originalShinyAmount = 1500;
                        List<uint> texts = new List<uint>
                                            {
                                                new AssetID("shinydoor_2sm0" + i.ToString() + "_force_text"),
                                                new AssetID("shinydoor_2sm0" + i.ToString() + "_notenough_text"),
                                                new AssetID("shinydoor_2sm0" + i.ToString() + "_text")
                                            };

                        if (i == 2)
                            texts.Add(0xFB25C8DC);
                        else if (i == 3)
                            texts.Add(0x586D0E0F);
                        else
                            texts.Add(0xB5B45342);

                        SetGate(originalShinyAmount, (int)(originalShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                            new List<uint>
                            {
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_COND"),
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_FORCE_COND"),
                            },
                            new List<uint>
                            {
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_TALKBOX"),
                            },
                            new List<uint>
                            {
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_MECH_05"),
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_MECH_04"),
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_MECH_03"),
                                                new AssetID("SHINYDOOR_2SM0" + i.ToString() + "_MECH_02"),
                            }, texts);
                    }

                    int bungeeShinyAmount = 2500;
                    SetGate(bungeeShinyAmount, (int)(bungeeShinyAmount * random.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_COND"),
                                            new AssetID("SHINYGATE_FORCE_COND_01"),
                        },
                        new List<uint>
                        {
                                            new AssetID("SHINYGATE_1_TALKBOX"),
                        },
                        new List<uint>
                        {
                                            new AssetID("CLAMGATE_SHINY_MECH_05"),
                                            new AssetID("CLAMGATE_SHINY_MECH_04"),
                                            new AssetID("CLAMGATE_SHINY_MECH_03"),
                                            new AssetID("CLAMGATE_SHINY_MECH_02")
                        },
                        new List<uint>
                        {
                                            new AssetID("shinyobjectgate_notenough_text"),
                                            new AssetID(0xCD5C904B),
                                            new AssetID("shinyobjectgate_1_text"),
                                            new AssetID("shinyobjectgate_force_text"),
                        });
                    needToAddNumbers = false;
                }
                break;
                default:
                    return false;
            }

            return true;
        }

        private void SetGate(int originalValue, int newValue, List<uint> condAssetIDs, List<uint> shinyGiverAssetIDs, List<uint> numberPlatAssetIDs, List<uint> textAssetIDs)
        {
            foreach (uint u in condAssetIDs)
                SetCondEvaluationAmount(newValue, u);
            foreach (uint u in shinyGiverAssetIDs)
                SetGiveShinyObjects(-newValue, u);
            int i = 0;
            foreach (uint u in numberPlatAssetIDs)
                SetNumberPlats(newValue, i++, u);
            foreach (uint u in textAssetIDs)
                ReplaceInText(u, originalValue.ToString(), newValue.ToString());
        }

        private void ReplaceInText(uint assetID, string originalText, string newText)
        {
            if (ContainsAsset(assetID))
            {
                AssetTEXT TEXT = ((AssetTEXT)GetFromAssetID(assetID));
                string text = TEXT.Text;
                if (text.Contains(originalText))
                    TEXT.Text = text.Replace(originalText, newText);
                else
                    MessageBox.Show("Text asset " + assetID.ToString("X8") + " on file " + LevelName + " doesn't contain " + originalText);
            }
            else
                MessageBox.Show("Text asset " + assetID.ToString("X8") + " not found on file " + LevelName);
        }

        private void SetCondEvaluationAmount(int shinyAmount, uint assetID)
        {
            if (ContainsAsset(assetID))
                ((AssetCOND)GetFromAssetID(assetID)).EvaluationAmount = shinyAmount;
            else
                MessageBox.Show("Cond asset " + assetID.ToString("X8") + " not found on file " + LevelName);
        }

        private void SetGiveShinyObjects(int shinyAmount, uint assetID)
        {
            if (ContainsAsset(assetID) && GetFromAssetID(assetID) is BaseAsset objectAsset)
            {
                Link[] links = objectAsset.Links;
                for (int i = 0; i < links.Length; i++)
                    if (links[i].EventSendID == (ushort)EventBFBB.GiveShinyObjects)
                        links[i].FloatParameter1 = shinyAmount;

                objectAsset.Links = links;
            }
            else
                MessageBox.Show("Asset " + assetID.ToString("X8") + " not found on file " + LevelName);
        }

        private void SetNumberPlats(int shinyAmount, int power, uint assetID)
        {
            if (ContainsAsset(assetID))
            {
                int newNumber = (shinyAmount / (int)Math.Pow(10, power)) % 10;
                SetPlaceableAssetModel(assetID, "number_" + newNumber.ToString());
            }
        }

        private void SetPlaceableAssetModel(uint assetID, string modelName)
        {
            if (ContainsAsset(assetID))
                ((EntityAsset)GetFromAssetID(assetID)).Model = modelName;
            else
                MessageBox.Show("Placeable asset " + assetID.ToString("X8") + " not found on file " + LevelName);
        }

        public bool RandomizePoliceStation(RandomizerSettings settings)
        {
            bool shuffled = false;

            if (settings.Pickups && ContainsAssetWithType(AssetType.Pickup))
                shuffled |= RandomizePickupPositions();

            if (settings.Colors)
                shuffled |=
                    ShufflePlaceableColors(settings.brightColors, settings.strongColors) |
                    ShufflePlaceableDynaColors(settings.brightColors, settings.strongColors) |
                    ShuffleLevelModelColors(settings.brightColors, settings.strongColors, settings.VertexColors);

            if (settings.invisibleLevel && ContainsAssetWithType(AssetType.JSP))
                shuffled |= MakeLevelInvisible();

            if (settings.invisibleObjects && ContainsAssetWithType(AssetType.SimpleObject))
                shuffled |= MakeObjectsInvisible();

            if (settings.Set_Scale)
            {
                shuffled |= true;
                ApplyScale(new Vector3(settings.scaleFactorX, settings.scaleFactorY, settings.scaleFactorZ));
            }

            if (settings.Textures && ContainsAssetWithType(AssetType.Texture))
                shuffled |= RandomizeTextures(settings.Textures_Special);

            if (settings.Sounds && ContainsAssetWithType(AssetType.SoundInfo))
                shuffled |= RandomizeSounds(settings.Mix_Sound_Types);

            return shuffled;
        }
    }
}