using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Randomizer
{
    public static class RandomizerHelper
    {
        public static void RemoveAssetsFrom(ref List<AssetPKUP> assets, params string[] names)
        {
            for (int i = 0; i < assets.Count; i++)
                foreach (string s in names)
                    if (assets[i].assetName.Equals(s))
                    {
                        assets.RemoveAt(i--);
                        break;
                    }
        }

        public static void RemoveAssetsFromContains(ref List<AssetPKUP> assets, params string[] names)
        {
            for (int i = 0; i < assets.Count; i++)
                foreach (string s in names)
                    if (assets[i].assetName.Contains(s))
                    {
                        assets.RemoveAt(i--);
                        break;
                    }
        }

        public static string[] GetLimitedSoundRandomizerNamesBfbb(string levelName) => levelName switch
        {
            "boot" => ["Box_open", "Box_shuffle_alt", "Box_shuffle_open", "Button_press", "Button_up", "springboard", "springboard_big", "teleport_snd"],
            "mnu4" => ["mnu4_bubblewand", "mnu4_exit", "mnu4_Pop_in", "mnu4_SB_jog1L", "mnu4_SB_jog1R", "sax", "shiny_object_triangle3_soft"],
            "mnu5" => ["Check1", "fanfare"],
            "sppa" => ["Pat_belly", "Pat_jump_dub", "Pat_jump_land", "Pat_jump_sngl", "Pat_lift3B", "Pat_Ouch1", "Pat_Ouch2", "Pat_Ouch3", "Pat_Ouch4", "Pat_smash_belly", "Pat_throw"],
            "spsb" => ["generic_land", "SB_bounce_hit1", "SB_bounce_hit2", "SB_bounce_start", "SB_bowl_release", "SB_Bubble_wand", "SB_bungee2", "SB_cruise_hit", "SB_cruise_start", "SB_jump_dub", "SB_jump_end", "SB_jump_sngl", "SB_ouch1", "SB_ouch2", "SB_ouch3", "SB_ouch4", "SB_wetball_end", "SB_wetball_start"],
            "spsc" => ["SC_chop", "SC_jump_dub", "SC_jump_end", "SC_jump_sngl", "SC_kick", "SC_lasso_pull_pop", "SC_lasso_stretch", "SC_lasso_throw", "SC_Ouch1", "SC_Ouch2", "SC_Ouch3", "SC_Ouch4"],
            _ => [],
        };

        public static readonly NpcType_BFBB[] importVilTypes = new NpcType_BFBB[] {
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

        public static string EditorFilesGameFolderName(Game game) => game switch
        {
            Game.BFBB => "BattleForBikiniBottom",
            Game.Incredibles => "MovieGame",
            Game.Scooby => "Scooby",
            _ => "",
        };

        public static bool IsWarpToSameLevel(string levelName, string warpName) =>
            levelName.ToLower().Equals(warpName.ToLower()) || new string(warpName.Reverse().ToArray()).ToLower().Equals(levelName.ToLower());

        public static string[] GetMarkerNamesBfbb(string levelName) => levelName.ToLower() switch
        {
            "bb01" => ["BB_SPAT_MARKER_01", "BB_SPAT_MARKER_02", "BB_SPAT_MARKER_03", "BB01MK01", "BB01MK02", "BB01MK03", "BB01MK04", "BB01MK05", "BB01MK06", "BB01MK07", "BB01MK08", "BB01MK09", "BB01MK10", "BB01MK11", "BB01MK12", "FROMBB02", "FROMBB03", "FROMBB04", "FROMHB01"],
            "bb02" => ["BB_SPAT_MARKER_04", "BB_SPAT_MARKER_05", "CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "CHECKPOINT_MK_05", "CHECKPOINT_MK_06", "CHECKPOINT_MK_07", "CHECKPOINT_MK_08", "CHECKPOINT_MK_09", "CHECKPOINT_MK_10", "CHECKPOINT_MK_11", "CHUCK_CIN_PORTAL_MK", "FROMBB01", "FROMBB03"],
            "bb04" => ["BB_SPAT_MARKER_07", "BB_SPAT_MARKER_08", "CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "FROMBB01"],
            "bc01" => ["BC_SPAT_MARKER_01", "BC01MK01", "BC01MK02", "BC01MK03", "BC01MK04", "FROMBC02", "FROMHB06"],
            "bc02" => ["BC_SPAT_MARKER_02", "BC_SPAT_MARKER_03", "BC_SPAT_MARKER_04", "BC_SPAT_MARKER_05", "BC02MK01", "BC02MK02", "BC02MK03", "BC02MK04", "BC02MK05", "FROMBC01", "FROMBC03", "FROMBC04", "FROMBC05"],
            "bc03" => ["BC_SPAT_MARKER_06", "BC03MK01", "BC03MK02", "BC03MK03", "BC03MK04", "BC03MK05", "BC03MK06", "FROMBC02", "FROMBC04"],
            "bc04" => ["BC_SPAT_MARKER_07", "BC04MK01", "BC04MK02", "FROMBC02", "FROMBC03"],
            "db01" => ["DB_SPAT_MARKER_01", "DB_SPAT_MARKER_02", "DB_SPAT_MARKER_07", "DB01MK01", "DB01MK02", "DB01MK03", "DB01MK04", "DB01MK05", "FROMDB02", "FROMDB03", "FROMDB04", "FROMDB06", "FROMHB01"],
            "db02" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "CHECKPOINT_MK_05", "CHECKPOINT_MK_06", "DB_SPAT_MARKER_03", "DB_SPAT_MARKER_04", "DB02MK01", "FROMDB01"],
            "db03" => ["DB_SPAT_MARKER_05", "DB03MK01", "DB03MK02", "DB03MK03", "DB03MK04", "DB03MK05", "DB03MK06", "FROMDB01"],
            "gl01" => ["FROMGL02", "FROMGL03", "FROMHB01", "GL_SPAT_MARKER_01", "GL_SPAT_MARKER_02", "GL_SPAT_MARKER_03", "GL_SPAT_MARKER_04", "GL01MK02", "GL01MK03", "GL01MK04", "GL01MK05", "GL01MK06", "GL01MK07", "GL01MK08", "GL01MK09", "GL01MK10", "GL01MK11", "GL01MK12", "GL01MK13", "GL01MK14", "GL01MK15"],
            "gl02" => ["FROMGL01", "FROMGL03", "GL_SPAT_MARKER_05", "GL02MK01", "GL02MK02", "GL02MK03", "GL02MK04", "GL02MK05"],
            "gl03" => ["FROMGL01", "FROMGL02", "GL_SPAT_MARKER_06", "GL_SPAT_MARKER_07", "GL_SPAT_MARKER_08", "GL03MK01", "GL03MK02", "GL03MK03", "GL03MK04", "GL03MK05", "GL03MK06", "GL03MK07", "GL03MK08"],
            "gy01" => ["FROMGY02", "FROMGY04", "FROMHB01", "GY_SPAT_MARKER_01", "GY_SPAT_MARKER_02", "GY_SPAT_MARKER_03", "GY01MK01", "GY01MK02", "GY01MK03", "TELEPRT_MARK_T"],
            "gy02" => ["FROMGY01", "FROMGY03", "GY_SPAT_MARKER_04", "GY_SPAT_MARKER_05", "GY02MK01", "GY02MK02", "GY02MK03", "GY02MK04"],
            "gy03" => ["FROMGY02", "GY_SPAT_MARKER_06", "GY_SPAT_MARKER_07", "GY03MK01", "GY03MK02", "GY03MK03"],
            "hb01" => ["FROMB101", "FROMB201", "FROMBB01", "FROMBC01", "FROMDB01", "FROMGL01", "FROMGY01", "FROMHB02", "FROMHB03", "FROMHB04", "FROMHB05", "FROMHB06", "FROMHB07", "FROMHB08", "FROMHB09", "FROMHB10", "FROMJF01", "FROMKF01", "FROMPG01", "FROMRB01", "FROMSM01", "HB_SPAT_MARKER_01", "HB_SPAT_MARKER_02", "HB_SPAT_MARKER_03", "HB_SPAT_MARKER_07", "KS_SPAT_MARKER_01", "KS_SPAT_MARKER_02", "KS_SPAT_MARKER_03"],
            "jf01" => ["CHECKP_INVIS_MK_01", "CHECKP_INVIS_MK_02", "CHECKP_INVIS_MK_03", "CHECKP_INVIS_MK_04", "CHECKP_INVIS_MK_05", "CHECKP_INVIS_MK_06", "CHECKP_INVIS_MK_07", "CHECKP_INVIS_MK_08", "FROMHB01", "FROMJF02", "FROMJF04", "JF_SPAT_MARKER_01", "JF_SPAT_MARKER_02", "JF01MK02", "JF01MK03", "JF01MK04"],
            "jf02" => ["CHECKP_INVIS_MK_01", "CHECKP_INVIS_MK_02", "CHECKP_INVIS_MK_03", "CHECKP_INVIS_MK_04", "CHECKP_INVIS_MK_05", "CHECKP_INVIS_MK_06", "CHECKP_INVIS_MK_07", "CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "FROMJF01", "FROMJF03", "JF_SPAT_MARKER_03", "JF_SPAT_MARKER_04", "SLIDEVENT_CP_MARKER_01"],
            "jf03" => ["FROMJF02", "FROMJF04", "JF_SPAT_MARKER_05", "JF_SPAT_MARKER_06", "JF03MK02", "JF03MK03", "JF03MK04", "JF03MK10", "JF03MK11", "JF03MK12", "JF03MK13", "JF03MK14", "JF03MK15", "JF03MK16", "JF03MK17"],
            "kf01" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "FROMHB01", "FROMKF02", "FROMKF05", "KF_SPAT_MARKER_01", "KF_SPAT_MARKER_02", "KF01MK10", "KF01MK11"],
            "kf02" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "FROMKF01", "FROMKF04", "KF_SPAT_MARKER_03", "KF_SPAT_MARKER_04", "KF02MK02", "KF02MK10", "KF02MK11", "KF02MK12", "KF02MK13"],
            "kf04" => ["FROMKF02", "FROMKF05", "KF_SPAT_MARKER_05", "KF_SPAT_MARKER_06"],
            "kf05" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "FROMKF01", "FROMKF04", "KF_SPAT_MARKER_07", "KF_SPAT_MARKER_08"],
            "rb01" => ["FROM_HUB", "FROMRB02", "FROMRB03", "RB_SPAT_MARKER_01", "RB_SPAT_MARKER_02", "RB_SPAT_MARKER_03", "RB_SPAT_MARKER_04", "RB01MK00", "RB01MK01", "RB01MK02", "RB01MK03", "RB01MK04", "RB01MK05", "RB01MK10", "RB01MK11", "RB01MK12", "RB01MK13"],
            "rb02" => ["FROMRB01", "FROMRB03", "RB_SPAT_MARKER_05", "RB02MK01", "RB02MK02", "RB02MK03", "RB02MK10", "RB02MK11", "RB03MK01"],
            "rb03" => ["CHECK_INVIS_MK_01", "CHECK_INVIS_MK_02", "CHECK_INVIS_MK_03", "FROM_RB02", "FROM_RB03", "FROMRB01", "RB_SPAT_MARKER_06", "RB_SPAT_MARKER_07", "RB_SPAT_MARKER_08", "RB03MK10", "RB03MK11", "RB03MK12", "RB03MK14"],
            "sm01" => ["CHECKPOINT_MK_01", "FROMHB01", "FROMSM02", "FROMSM03", "FROMSM04", "SM_SPAT_MARKER_01", "SM_SPAT_MARKER_02"],
            "sm02" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "FROMSM01", "SM_SPAT_MARKER_03", "SM_SPAT_MARKER_04", "SM02MK01"],
            "sm03" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "FROMSM01", "SM_SPAT_MARKER_05", "SM_SPAT_MARKER_06"],
            "sm04" => ["CHECKPOINT_MK_01", "CHECKPOINT_MK_02", "CHECKPOINT_MK_03", "CHECKPOINT_MK_04", "FROMSM01", "SM_SPAT_MARKER_07", "SM_SPAT_MARKER_08"],
            _ => [],
        };

        public static HashSet<string> scoobySkipPlats = ["l013", "c007", "f010"];

        public static string[] GetMarkerNamesScooby(string levelName) => levelName.ToLower() switch
        {
            "b001" => ["FROMB003", "FROMP004"],
            "e001" => ["FROME009", "FROMH001"],
            "f009" => ["FROMF003", "FROMF008"],
            "g003" => ["FROMG008", "FROMG002"],
            "g006" => ["FROMG003", "FROMG005"],
            "l015" => ["FROML014", "FROML018", "FROML019B"],
            "p001" => ["FROMP002A", "FROMP005", "FROMF003", "FROML018"],
            "r001" => ["FROMI001", "FROMI006"],
            "s002" => ["FROMS003A", "FROMS001", "FROMS001A", "FROMS003"],
            "w020" => ["FROMW026", "FROMLIGHTHOUSE", "FROMLIGHTHOUSE"],
            "w026" => ["FROMW028", "FROMW025", "FROMW020"],
            _ => [],
        };

        public static List<PickupReference> ScoobyPowerupsFrom(bool slippersLamp, bool monsterTokens, bool gumSoapUpgrades)
        {
            List<PickupReference> result = [
                PickupReference.Shovel,
                PickupReference.Springs,
                PickupReference.Helmet,
                PickupReference.Knight_Helmet,
                PickupReference.Boots,
                PickupReference.Super_Smash,
                PickupReference.Plungers,
                PickupReference.Super_Sonic_Smash,
                PickupReference.Umbrella,
                PickupReference.Gum_Machine,
                PickupReference.Soap_Bubble,
                ];
            if (slippersLamp)
            {
                result.Add(PickupReference.Slippers);
                result.Add(PickupReference.Lampshade);
            }
            if (monsterTokens)
            {
                result.Add(PickupReference.TokenGreenGhost);
                result.Add(PickupReference.TokenWolfman);
                result.Add(PickupReference.TokenWitch);
                result.Add(PickupReference.TokenTarMonster);
                result.Add(PickupReference.TokenGhostDiver);
                result.Add(PickupReference.TokenZombie);
                result.Add(PickupReference.TokenGhost);
                result.Add(PickupReference.TokenScarecrow);
                result.Add(PickupReference.TokenGargoyle);
                result.Add(PickupReference.TokenHeadlessSpectre);
                result.Add(PickupReference.TokenGeronimo);
                result.Add(PickupReference.TokenCaveman);
                result.Add(PickupReference.TokenSeaCreature);
                result.Add(PickupReference.TokenBlackKnight);
                result.Add(PickupReference.TokenCreeper);
                result.Add(PickupReference.TokenWitchDoctor);
                result.Add(PickupReference.TokenFunlandRobot);
                result.Add(PickupReference.TokenMastermind);
                result.Add(PickupReference.TokenGhostCaptainMoody);
                result.Add(PickupReference.TokenRedbeard);
                result.Add(PickupReference.TokenSpaceKook);
            }
            if (gumSoapUpgrades)
            {
                result.Add(PickupReference.Soap_Pack); // e007 - chills 1
                result.Add(PickupReference.Soap_Pack); // f001 - shock 1
                result.Add(PickupReference.Soap_Pack); // g003 - crypt 1
                result.Add(PickupReference.Soap_Pack); // l019 - lighthouse 3
                result.Add(PickupReference.Soap_Pack); // r005 - don't look down 2
                result.Add(PickupReference.Soap_Pack); // r021 - upstairs 3
                result.Add(PickupReference.Soap_Pack); // w023 - aghast 1

                result.Add(PickupReference.Gum_Pack); // c003 - cliff 3
                result.Add(PickupReference.Gum_Pack); // f003 - shock 2
                result.Add(PickupReference.Gum_Pack); // g006 - crypt 4
                result.Add(PickupReference.Gum_Pack); // l011 - coast 1
                result.Add(PickupReference.Gum_Pack); // o001 - cower 1
                result.Add(PickupReference.Gum_Pack); // r020 - upstairs 2
            }
            return result;
        }

        public static ConditionalVariableScooby GetConditionalVariableScooby(PickupReference pick) => pick switch
        {
            PickupReference.Shovel => ConditionalVariableScooby.Shovel,
            PickupReference.Springs => ConditionalVariableScooby.Spring,
            PickupReference.Slippers => ConditionalVariableScooby.Slippers,
            PickupReference.Lampshade => ConditionalVariableScooby.Lampshade,
            PickupReference.Helmet => ConditionalVariableScooby.FootballHelmet,
            PickupReference.Knight_Helmet => ConditionalVariableScooby.BlackKnight,
            PickupReference.Boots => ConditionalVariableScooby.StickyBoots,
            PickupReference.Super_Smash => ConditionalVariableScooby.LightningBolt,
            PickupReference.Plungers => ConditionalVariableScooby.Plungers,
            PickupReference.Super_Sonic_Smash => ConditionalVariableScooby.LightningBoltStun,
            PickupReference.Umbrella => ConditionalVariableScooby.Umbrella,
            PickupReference.Gum_Machine => ConditionalVariableScooby.BubbleGum,
            PickupReference.Soap_Bubble => ConditionalVariableScooby.SoapBar,
            _ => 0,
        };

        public static bool ReplacementAllowed(string levelName, PickupReference pickup, Vector3 position)
        {
            switch (levelName)
            {
                case "b001": // cellar 1
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash &&
                        pickup != PickupReference.Soap_Bubble &&
                        pickup != PickupReference.Gum_Machine;
                case "b004": // cellar 4
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Super_Smash &&
                        pickup != PickupReference.Helmet;
                case "c003": // cliff 3
                    if (Vector3.Distance(position, new Vector3(-177.5f, -11f, -34)) < 20f || (position.Z <= -67f && position.Y >= -8f)) // distant platform or upper area
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "c005": // cave in 1
                    if (position.Y > -7.9f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "c007": // cave in 3
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash;
                case "e001":
                    if (Vector3.Distance(position, new Vector3(34.0f, 1.5f, -30.33f)) < 2f)
                        return pickup != PickupReference.Helmet;
                    return true;
                case "e002": // edge 2
                    return
                        pickup != PickupReference.TokenGreenGhost &&
                        pickup != PickupReference.TokenWolfman &&
                        pickup != PickupReference.TokenWitch &&
                        pickup != PickupReference.TokenTarMonster &&
                        pickup != PickupReference.TokenGhostDiver &&
                        pickup != PickupReference.TokenZombie &&
                        pickup != PickupReference.TokenGhost &&
                        pickup != PickupReference.TokenScarecrow &&
                        pickup != PickupReference.TokenGargoyle &&
                        pickup != PickupReference.TokenHeadlessSpectre &&
                        pickup != PickupReference.TokenGeronimo &&
                        pickup != PickupReference.TokenCaveman &&
                        pickup != PickupReference.TokenSeaCreature &&
                        pickup != PickupReference.TokenBlackKnight &&
                        pickup != PickupReference.TokenCreeper &&
                        pickup != PickupReference.TokenWitchDoctor &&
                        pickup != PickupReference.TokenFunlandRobot &&
                        pickup != PickupReference.TokenMastermind &&
                        pickup != PickupReference.TokenGhostCaptainMoody &&
                        pickup != PickupReference.TokenRedbeard &&
                        pickup != PickupReference.TokenSpaceKook;
                case "e003":
                    if (position.X > 285f && position.X < 296f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Boots &&
                            pickup != PickupReference.Super_Smash;
                    return true;
                case "e007": // chills 1
                    if (position.Y >= 20f) // above trees
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "e009": // chills 3
                    if (Vector3.Distance(position, new Vector3(-28.0f, 19.5f, 2.0f)) < 4f) // above jump on bat area
                        return pickup != PickupReference.Springs;
                    return true;
                case "f001": // shock 1
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash;
                case "f003": // shock 2
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Boots &&
                        pickup != PickupReference.Gum_Machine &&
                        pickup != PickupReference.Soap_Bubble;
                case "f004": // fishy 1
                    if (position.X < -153f && position.Y > 8f) // token area
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash;
                case "f007": // fishy 4
                    if (position.Y > -3.3f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "f010": // pier 3
                    return true;
                case "g002": // grave plot 2
                    return true;
                case "g003": // crypt 1
                    if (position.Y >= 3.8f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "g005": // crypt 3
                    if (Vector3.Distance(position, new Vector3(96.39f, 3.5f, 85.22f)) < 6.77f)
                        return pickup != PickupReference.Helmet;
                    return true;
                case "g006": // crypt 4
                    return true;
                case "g008": // tomb 2
                    if (Vector3.Distance(position, new Vector3(11.0f, 11.0f, 77f)) < 2f)
                        return pickup != PickupReference.Helmet;
                    if (Vector3.Distance(position, new Vector3(13.8f, 16.3f, 39.5f)) < 5.6f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Umbrella;
                    return true;
                case "g009": // tomb 3 green ghost
                    return pickup != PickupReference.Super_Smash;
                case "h001": // hub
                    return true;
                case "h002": // playground
                    return true;
                case "i001": // clamor 1
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash &&
                        pickup != PickupReference.Soap_Bubble;
                case "i005": // manors 2
                    return true;
                case "l011": // coast 1
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Umbrella &&
                        pickup != PickupReference.Plungers;
                case "l013": // coast 2
                    if (position.Y > 0)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Soap_Bubble &&
                            pickup != PickupReference.Gum_Machine;
                    return true;
                case "l014": // coast 3
                    if (position.Z > 8.5f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Soap_Bubble &&
                            pickup != PickupReference.Gum_Machine;
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Super_Smash;
                case "l017": // lighthouse 1
                    return pickup != PickupReference.Helmet;
                case "l019": // lighthouse 3
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Umbrella &&
                        pickup != PickupReference.Soap_Bubble;
                case "o001": // cower 1
                    if (position.Y >= 39.3f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Boots;
                    return true;
                case "o002": // attic 2
                    if (position.X > -20f && position.X < 10f && position.Z > -246.35f && position.Z < -244.35f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Plungers &&
                            pickup != PickupReference.Helmet;
                    return true;
                case "o008": // black knight
                    return pickup != PickupReference.Helmet;
                case "p003": // creepy 3
                    if (position.Y > 18f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "r004": // don't look down 1
                    if (position.X < 107f && position.Y > 25f && position.Z < -160f)
                        return pickup != PickupReference.Super_Smash;
                    return true;
                case "r005": // don't look down 2
                    return
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash;
                case "r020": // upstairs 2
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Plungers &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Umbrella;
                case "r021": // upstairs 3
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet;
                case "s002": // lab 2
                    if (position.Y > 30.5f)
                        return
                            pickup != PickupReference.Shovel &&
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Knight_Helmet &&
                            pickup != PickupReference.Boots &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Plungers &&
                            pickup != PickupReference.Super_Sonic_Smash &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble &&
                            pickup != PickupReference.Slippers &&
                            pickup != PickupReference.Lampshade;
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash &&
                        pickup != PickupReference.Gum_Machine &&
                        pickup != PickupReference.Soap_Bubble;
                case "s003": // lab 3
                    if (position.Y > 26f)
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Helmet &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Gum_Machine &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "w022": // wreck 3
                    return
                        pickup != PickupReference.Springs &&
                        pickup != PickupReference.Super_Smash &&
                        pickup != PickupReference.Umbrella &&
                        pickup != PickupReference.Gum_Machine &&
                        pickup != PickupReference.Soap_Bubble;
                case "w023": // aghast 1
                    if (position.X >= 241f && position.Z <= -24.5f) // secret ship
                        return
                            pickup != PickupReference.Springs &&
                            pickup != PickupReference.Umbrella &&
                            pickup != PickupReference.Super_Smash &&
                            pickup != PickupReference.Soap_Bubble;
                    return true;
                case "w025": // aghast 2
                    if (position.Y > -6f)
                        return
                           pickup != PickupReference.Springs &&
                           pickup != PickupReference.Umbrella &&
                           pickup != PickupReference.Helmet &&
                           pickup != PickupReference.Super_Smash &&
                           pickup != PickupReference.Gum_Machine &&
                           pickup != PickupReference.Soap_Bubble;
                    return
                        pickup != PickupReference.Helmet &&
                        pickup != PickupReference.Super_Smash;
                case "w028": // shiver 2 redbeard
                    return true;
                default:
                    throw new ArgumentException($"Unsupported level for Scooby Powerups: {levelName}");
            }
        }

        public static bool PortInToSkip(Game game, string LevelName, AssetPORT port, List<string> toSkip)
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
                        if (new string[] { "TOS005", "TOE001", "T0F001" }.Contains(port.assetName))
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
    }
}
