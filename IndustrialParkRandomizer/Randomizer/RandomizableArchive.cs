using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using HipHopFile;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;

namespace IndustrialPark.Randomizer
{
    public class RandomizableArchive : ArchiveEditorFunctions
    {
        public bool Randomize(int seed, RandomizerFlags flags, RandomizerFlags2 flags2, RandomizerSettings settings, Random gateRandom, out bool needToAddNumbers)
        {
            if (LevelName == "hb09")
                return needToAddNumbers = false;

            bool shuffled = false;

            Random localRandom = new Random(seed);
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetTIMR timr && flags.HasFlag(RandomizerFlags.Timers))
                {
                    timr.Time *= localRandom.NextFloat(settings.timerMin, settings.timerMax);
                    shuffled = true;
                }
                else if (a is AssetDSCO dsco && flags.HasFlag(RandomizerFlags.Disco_Floors))
                {
                    byte[] bytes = dsco.PatternController;
                    localRandom.NextBytes(bytes);
                    dsco.PatternController = bytes;
                    shuffled = true;
                }
                else if (a is AssetSURF surf && flags.HasFlag(RandomizerFlags.Texture_Animations))
                {
                    if (surf.UVEffects1_TransSpeed_X == 0)
                        surf.UVEffects1_TransSpeed_X = localRandom.NextFloat(settings.surfMin, settings.surfMax);
                    else
                        surf.UVEffects1_TransSpeed_X *= localRandom.NextFloat(settings.surfMin, settings.surfMax);

                    if (surf.UVEffects1_TransSpeed_Y == 0)
                        surf.UVEffects1_TransSpeed_Y = localRandom.NextFloat(settings.surfMin, settings.surfMax);
                    else
                        surf.UVEffects1_TransSpeed_Y *= localRandom.NextFloat(settings.surfMin, settings.surfMax);

                    shuffled = true;
                }
                else if (a is AssetFLY fly && settings.disableFlythroughs && fly.Data.Length > 60)
                {
                    fly.FLY_Entries = new EntryFLY[] { fly.FLY_Entries[0] };
                    shuffled = true;
                }

            if (flags.HasFlag(RandomizerFlags.Textures))
                shuffled |= RandomizeTextures(seed, flags.HasFlag(RandomizerFlags.Textures_Special));

            if (flags.HasFlag(RandomizerFlags.Boulder_Settings))
                shuffled |= RandomizeBoulderSettings(seed, settings);

            if (flags.HasFlag(RandomizerFlags.Sounds))
                shuffled |= RandomizeSounds(seed, flags2.HasFlag(RandomizerFlags2.Mix_SND_SNDS));

            if (flags.HasFlag(RandomizerFlags.Pickup_Positions))
                shuffled |= RandomizePickupPositions(seed);

            if (flags.HasFlag(RandomizerFlags.MovePoint_Radius))
                shuffled |= RandomizeMovePointRadius(seed, settings);

            if (flags.HasFlag(RandomizerFlags.Tiki_Types) && ContainsAssetWithType(AssetType.VIL))
            {
                List<VilType> chooseFrom = new List<VilType>();
                if (settings.WoodenTiki >= 0)
                    chooseFrom.Add(VilType.tiki_wooden_bind);
                if (settings.FloatingTiki >= 0)
                    chooseFrom.Add(VilType.tiki_lovey_dovey_bind);
                if (settings.ThunderTiki >= 0)
                    chooseFrom.Add(VilType.tiki_thunder_bind);
                if (settings.ShhhTiki >= 0)
                    chooseFrom.Add(VilType.tiki_shhhh_bind);
                if (settings.StoneTiki >= 0)
                    chooseFrom.Add(VilType.tiki_stone_bind);

                List<VilType> setTo = new List<VilType>();
                for (int i = 0; i < settings.WoodenTiki; i++)
                    setTo.Add(VilType.tiki_wooden_bind);
                for (int i = 0; i < settings.FloatingTiki; i++)
                    setTo.Add(VilType.tiki_lovey_dovey_bind);
                for (int i = 0; i < settings.ThunderTiki; i++)
                    setTo.Add(VilType.tiki_thunder_bind);
                for (int i = 0; i < settings.ShhhTiki; i++)
                    setTo.Add(VilType.tiki_shhhh_bind);
                for (int i = 0; i < settings.StoneTiki; i++)
                    setTo.Add(VilType.tiki_stone_bind);

                if (LevelName == "kf04")
                    chooseFrom.Remove(VilType.tiki_stone_bind);
                
                shuffled |= ShuffleVilTypes(seed, chooseFrom, setTo,
                    flags.HasFlag(RandomizerFlags.Tiki_Models),
                    flags.HasFlag(RandomizerFlags.Tiki_Allow_Any_Type),
                    false);
            }

            if (flags.HasFlag(RandomizerFlags.Enemy_Types) && ContainsAssetWithType(AssetType.VIL))
            {
                List<VilType> chooseFrom = new List<VilType>(16);
                if (settings.Fodder >= 0)
                    chooseFrom.Add(VilType.robot_0a_fodder_bind);
                if (settings.Hammer >= 0)
                    chooseFrom.Add(VilType.ham_bind);
                if (settings.Tartar >= 0)
                    chooseFrom.Add(VilType.robot_tar_bind);
                if (settings.GLove >= 0)
                    chooseFrom.Add(VilType.g_love_bind);
                if (settings.Chuck >= 0)
                    chooseFrom.Add(VilType.robot_chuck_bind);
                if (settings.Monsoon >= 0)
                    chooseFrom.Add(VilType.robot_4a_monsoon_bind);
                if (settings.Sleepytime >= 0)
                    chooseFrom.Add(VilType.robot_sleepytime_bind);
                if (settings.Arf >= 0)
                    chooseFrom.Add(VilType.robot_arf_bind);
                if (settings.Tubelets >= 0)
                    chooseFrom.Add(VilType.tubelet_bind);
                if (settings.Slick >= 0)
                    chooseFrom.Add(VilType.robot_9a_bind);
                if (settings.BombBot >= 0)
                    chooseFrom.Add(VilType.robot_0a_bomb_bind);
                if (settings.BzztBot >= 0)
                    chooseFrom.Add(VilType.robot_0a_bzzt_bind);
                if (settings.ChompBot >= 0)
                    chooseFrom.Add(VilType.robot_0a_chomper_bind);

                List<VilType> setTo = new List<VilType>();
                for (int i = 0; i < settings.Fodder; i++)
                    setTo.Add(VilType.robot_0a_fodder_bind);
                for (int i = 0; i < settings.Hammer; i++)
                    setTo.Add(VilType.ham_bind);
                for (int i = 0; i < settings.Tartar; i++)
                    setTo.Add(VilType.robot_tar_bind);
                for (int i = 0; i < settings.GLove; i++)
                    setTo.Add(VilType.g_love_bind);
                for (int i = 0; i < settings.Chuck; i++)
                    setTo.Add(VilType.robot_chuck_bind);
                for (int i = 0; i < settings.Monsoon; i++)
                    setTo.Add(VilType.robot_4a_monsoon_bind);
                for (int i = 0; i < settings.Sleepytime; i++)
                    setTo.Add(VilType.robot_sleepytime_bind);
                for (int i = 0; i < settings.Arf; i++)
                    setTo.Add(VilType.robot_arf_bind);
                for (int i = 0; i < settings.Tubelets; i++)
                    setTo.Add(VilType.tubelet_bind);
                for (int i = 0; i < settings.Slick; i++)
                    setTo.Add(VilType.robot_9a_bind);
                for (int i = 0; i < settings.BombBot; i++)
                    setTo.Add(VilType.robot_0a_bomb_bind);
                for (int i = 0; i < settings.BzztBot; i++)
                    setTo.Add(VilType.robot_0a_bzzt_bind);
                for (int i = 0; i < settings.ChompBot; i++)
                    setTo.Add(VilType.robot_0a_chomper_bind);

                shuffled |= ShuffleVilTypes(seed, chooseFrom, setTo, false, flags.HasFlag(RandomizerFlags.Enemies_Allow_Any_Type), true);
            }
            
            if (flags.HasFlag(RandomizerFlags.Marker_Positions) && ContainsAssetWithType(AssetType.MRKR)
                && !new string[] { "hb02", "b101", "b201", "b302", "b303" }.Contains(LevelName))
                shuffled |= ShuffleMRKRPositions(seed,
                    settings.allMenuWarpsHB01,
                    flags2.HasFlag(RandomizerFlags2.Pointer_Positions),
                    flags.HasFlag(RandomizerFlags.Player_Start),
                    flags2.HasFlag(RandomizerFlags2.Bus_Stop_Positions),
                    flags2.HasFlag(RandomizerFlags2.Teleport_Box_Positions),
                    flags2.HasFlag(RandomizerFlags2.Taxi_Positions));

            if (flags.HasFlag(RandomizerFlags.Platform_Speeds))
                shuffled |= ShufflePlatSpeeds(seed, settings);
            
            if (flags.HasFlag(RandomizerFlags.Cameras))
                shuffled |= ShuffleCameras(seed);

            bool shinyNumbers = false;
            bool spatNumbers = false;

            if (flags.HasFlag(RandomizerFlags.Shiny_Object_Gates) && ContainsAssetWithType(AssetType.COND))
                shuffled |= ShuffleShinyGates(gateRandom, settings, out shinyNumbers);

            if ((settings.spatReqChum != 75 || flags.HasFlag(RandomizerFlags.Spatula_Gates)) && ContainsAssetWithType(AssetType.COND))
                shuffled |= ShuffleSpatulaGates(gateRandom, flags.HasFlag(RandomizerFlags.Spatula_Gates), settings, out spatNumbers);

            needToAddNumbers = shinyNumbers | spatNumbers;

            if (flags2.HasFlag(RandomizerFlags2.Scale_Of_Things))
               ShuffleScales(seed, settings);
            
            if (flags2.HasFlag(RandomizerFlags2.Buttons))
                shuffled |= ShuffleButtons(seed);

            if (flags.HasFlag(RandomizerFlags.Colors))
            {
                Random r = new Random(seed);
                shuffled |= ShufflePlaceableColors(r, settings.brightColors, settings.strongColors) | ShuffleModelColors(r, settings.brightColors, settings.strongColors);
            }

            if (flags.HasFlag(RandomizerFlags.Player_Characters))
                shuffled |= ShuffleBusStops(gateRandom);
            
            if (flags.HasFlag(RandomizerFlags.Music))
                shuffled |= RandomizePlaylistLocal();

            if (settings.disableCutscenes)
                shuffled |= DisableCutscenes();

            if (settings.openTeleportBoxes)
                shuffled |= OpenTeleportBoxes();
            
            return shuffled;
        }

        private bool OpenTeleportBoxes()
        {
            List<uint> dynaTeleportAssetIDs = new List<uint>();
            int layerIndex = -1;

            foreach (AssetDYNA dyna in (from asset in assetDictionary.Values
                                        where asset is AssetDYNA dyna && dyna.Type_BFBB == DynaType_BFBB.game_object__Teleport
                                        select asset).Cast<AssetDYNA>())
            {
                dynaTeleportAssetIDs.Add(dyna.AssetID);
                layerIndex = GetLayerFromAssetID(dyna.AssetID);
            }

            if (dynaTeleportAssetIDs.Count == 0)
                return false;

            var links = new List<LinkBFBB>();
            foreach (uint u in dynaTeleportAssetIDs)
                links.Add(new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                {
                    EventReceiveID = EventBFBB.ScenePrepare,
                    EventSendID = EventBFBB.OpenTeleportBox,
                    TargetAssetID = u,
                    Arguments_Float = new float[4]
                });

            AssetDPAT dispatcher = (AssetDPAT)GetFromAssetID(PlaceTemplate(new Vector3(), layerIndex, out _, ref dynaTeleportAssetIDs, "IP_TELEBOX", AssetTemplate.Dispatcher));
            dispatcher.LinksBFBB = links.ToArray();

            return true;
        }

        private bool RandomizeTextures(int seed, bool hud)
        {
            Random r = new Random(seed);

            List<Asset> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.RWTX
                                  && ((hud && asset.AHDR.ADBG.assetName.ToLower().Contains("rw3")) || (!hud))
                                  select asset).ToList();

            if (assets.Count < 2)
                return false;

            List<byte[]> datas = (from asset in assets select asset.Data).ToList();
            
            foreach (Asset a in assets)
            {
                int value = r.Next(0, datas.Count);
                a.Data = datas[value];
                datas.RemoveAt(value);
            }

            return true;
        }

        private bool RandomizePickupPositions(int seed)
        {
            Random r = new Random(seed);

            List<AssetPKUP> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.PKUP select asset).Cast<AssetPKUP>().ToList();

            if (assets.Count < 2)
                return false;

            switch (LevelName)
            {
                case "hb01":
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("GREENSHINY_PICKUP_02")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("GREENSHINY_PICKUP_03")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("GREENSHINY_PICKUP_18")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("GREENSHINY_PICKUP_20")));
                    for (int i = 0; i < assets.Count; i++)
                        if (assets[i].AHDR.ADBG.assetName.Contains("GS_MRKRABS_PICKUP") || assets[i].AHDR.ADBG.assetName.Contains("GS_PATRICK_PICKUP"))
                        {
                            assets.RemoveAt(i);
                            i--;
                        }
                    break;
                case "hb02":
                    for (int i = 0; i < assets.Count; i++)
                        if (assets[i].AHDR.ADBG.assetName.Contains("RED"))
                        {
                            int shinyNum = Convert.ToInt32(assets[i].AHDR.ADBG.assetName.Split('_')[2]);
                            if (shinyNum >= 32 && shinyNum <= 47 && shinyNum != 42)
                            {
                                assets.RemoveAt(i);
                                i--;
                            }
                        }
                    break;
                case "bb01":
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("SHINY_RED_019")));
                    break;
                case "gl01":
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("SHINY_YELLOW_004")));
                    if (ContainsAsset(new AssetID("GOLDENSPATULA_04")))
                        ((AssetPKUP)GetFromAssetID(new AssetID("GOLDENSPATULA_04"))).PositionY += 2f;
                    break;
                case "gl03":
                    if (ContainsAsset(0x0B48E8AC))
                    {
                        AssetDYNA dyna = (AssetDYNA)GetFromAssetID(0x0B48E8AC);
                        dyna.LinksBFBB = new LinkBFBB[0];

                        if (ContainsAsset(0xF70F6FEE))
                        {
                            ((AssetPKUP)GetFromAssetID(0xF70F6FEE)).PickupFlags = 2;
                            ((AssetPKUP)GetFromAssetID(0xF70F6FEE)).Visible = true;
                        }
                    }
                    break;
                case "sm02":
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("PU_SHINY_RED")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("PU_SHINY_GREEN")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("PU_SHINY_YELLOW")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("PU_SHINY_BLUE")));
                    assets.Remove((AssetPKUP)GetFromAssetID(new AssetID("PU_SHINY_PURPLE")));
                    break;
                case "jf01":
                case "bc01":
                case "rb03":
                case "sm01":
                    for (int i = 0; i < assets.Count; i++)
                    {
                        foreach (LinkBFBB link in assets[i].LinksBFBB)
                            if (link.EventSendID == EventBFBB.Mount)
                            {
                                assets.RemoveAt(i);
                                i--;
                                break;
                            }
                    }
                    break;
            }

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            foreach (AssetPKUP a in assets)
            {
                int value = r.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);
            }

            return true;
        }

        private bool ShuffleCameras(int seed)
        {
            Random r = new Random(seed);

            List<AssetCAM> assets = (from asset in assetDictionary.Values
                                     where asset.AHDR.assetType == AssetType.CAM && asset.AHDR.ADBG.assetName != "STARTCAM"
                                     select asset).Cast<AssetCAM>().ToList();

            if (assets.Count == 0)
                return false;

            for (int i = 0; i < assets.Count; i++)
            {
                var whoTargets = FindWhoTargets(assets[i].AssetID);
                if (whoTargets.Count > 0 && GetFromAssetID(whoTargets[0]) is AssetDYNA dyna
                    && (dyna.Type_BFBB == DynaType_BFBB.game_object__BusStop || dyna.Type_BFBB == DynaType_BFBB.game_object__Taxi))
                {
                    assets.RemoveAt(i);
                    i--;
                }
            }

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();
            List<Vector3[]> angles = (from asset in assets select (new Vector3[] {
                new Vector3(asset.NormalizedForwardX, asset.NormalizedForwardY, asset.NormalizedForwardZ),
                new Vector3(asset.NormalizedUpX, asset.NormalizedUpY, asset.NormalizedUpZ),
                new Vector3(asset.NormalizedLeftX, asset.NormalizedLeftY, asset.NormalizedLeftZ),
                new Vector3(asset.ViewOffsetX, asset.ViewOffsetY, asset.ViewOffsetZ)
            })).ToList();

            foreach (AssetCAM a in assets)
            {
                int value1 = r.Next(0, positions.Count);
                int value2 = r.Next(0, angles.Count);

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
        
        private bool ShuffleScales(int seed, RandomizerSettings settings)
        {
            Random r = new Random(seed);

            List<PlaceableAsset> assets = (from asset in assetDictionary.Values
                                           where new AssetType[] {
                                               AssetType.BOUL, AssetType.BUTN, AssetType.DSTR, AssetType.PLAT, AssetType.SIMP
                                           }.Contains(asset.AHDR.assetType) && !asset.AHDR.ADBG.assetName.ToLower().Contains("track")
                                           select asset).Cast<PlaceableAsset>().ToList();
            
            foreach (PlaceableAsset a in assets)
            {
                float scale = r.NextFloat(settings.scaleMin, settings.scaleMax);

                a.ScaleX *= scale;
                a.ScaleY *= scale;
                a.ScaleZ *= scale;
            }

            return assets.Count != 0;
        }

        private bool ShufflePlaceableColors(Random r, bool brightColors, bool strongColors)
        {
            AssetType[] allowed = new AssetType[] {
                AssetType.BOUL, AssetType.BUTN, AssetType.DSTR, AssetType.HANG, AssetType.NPC, AssetType.PEND,
                AssetType.PKUP, AssetType.PLAT, AssetType.PLYR, AssetType.SIMP, AssetType.VIL };

            List<PlaceableAsset> assets = (from asset in assetDictionary.Values
                                           where allowed.Contains(asset.AHDR.assetType)
                                           select asset).Cast<PlaceableAsset>().ToList();

            foreach (PlaceableAsset a in assets)
            {
                Vector3 color = GetRandomColor(r, brightColors, strongColors);
                a.ColorRed = color.X;
                a.ColorGreen = color.Y;
                a.ColorBlue = color.Z;
            }

            return assets.Count != 0;
        }

        private bool ShuffleModelColors(Random r, bool brightColors, bool strongColors)
        {
            List<Asset> assets = (from asset in assetDictionary.Values
                                   where new AssetType[] { AssetType.JSP, AssetType.BSP }.Contains(asset.AHDR.assetType) select asset).ToList();

            float max = 255f;
            bool colored = false;

            foreach (Asset a in assets)
            {
                var colors = GetColors(a.Data);

                if (colors.Length == 0)
                    continue;

                for (int i = 0; i < colors.Length; i++)
                {
                    Vector3 color = GetRandomColor(r, brightColors, strongColors);

                    colors[i] = System.Drawing.Color.FromArgb(colors[i].A,
                        (byte)(color.X * max),
                        (byte)(color.Y * max),
                        (byte)(color.Z * max));
                }

                try
                {
                    a.Data = SetColors(a.Data, colors);
                    colored = true;
                }
                catch { }
            }

            return colored;
        }

        private Vector3 GetRandomColor(Random r, bool brightColors, bool strongColors)
        {
            float colorMin = brightColors ? 0.5f : 0f;
            float colorMax = 1f;

            Vector3 v = new Vector3(r.NextFloat(colorMin, colorMax), r.NextFloat(colorMin, colorMax), r.NextFloat(colorMin, colorMax));

            if (strongColors)
            {
                float strongFactor = brightColors ? 0.4f : 0.7f;
                float doublestrongFactor = 2 * strongFactor;

                v -= strongFactor;

                int strongColor = r.Next(0, 6);

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

        private bool ShuffleButtons(int seed)
        {
            Random r = new Random(seed);

            bool result = false;
            for (int i = 0; i < 2; i++)
            {
                List<AssetBUTN> assets = (from asset in assetDictionary.Values where asset is AssetBUTN butn && butn.ActMethod == (AssetBUTN.ButnActMethod)i select asset).Cast<AssetBUTN>().ToList();

                List<byte[]> datas = (from asset in assets select asset.Data.Skip(8).Take(asset.Data.Length - Link.sizeOfStruct * asset.AmountOfEvents - 8).ToArray()).ToList();

                foreach (AssetBUTN a in assets)
                {
                    int InitialButtonState = a.InitialButtonState;
                    bool ResetAfterDelay = a.ResetAfterDelay;
                    float ResetDelay = a.ResetDelay;

                    int value = r.Next(0, datas.Count);
                    var newData = a.Data.Take(8).ToList();
                    newData.AddRange(datas[value]);
                    newData.AddRange(a.Data.Skip(a.Data.Length - Link.sizeOfStruct * a.AmountOfEvents));
                    a.Data = newData.ToArray();
                    datas.RemoveAt(value);

                    a.InitialButtonState = InitialButtonState;
                    a.ResetAfterDelay = ResetAfterDelay;
                    a.ResetDelay = ResetDelay;

                    result = true;
                }
            }
            return result;
        }

        private bool ShuffleBusStops(Random r)
        {
            bool result = false;

            foreach (AssetDYNA dyna in (from asset in assetDictionary.Values
                                        where asset is AssetDYNA dyna && dyna.Type_BFBB == DynaType_BFBB.game_object__BusStop
                                        select asset).Cast<AssetDYNA>())
            {
                DynaBusStop dynaBusStop = (DynaBusStop)dyna.DynaBase;
                dynaBusStop.Player = (DynaBusStop.PlayerEnum)r.Next(0, 2);
                dyna.DynaBase = dynaBusStop;
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
                vil.TaskDYNA2_AssetID = vil.TaskDYNA1_AssetID;
            }
        }

        public bool RandomizePlayerOnSpawn()
        {
            int defaultLayerIndex = -1;
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].layerType == (int)LayerType_BFBB.DEFAULT)
                {
                    defaultLayerIndex = i;
                    break;
                }

            List<uint> outAssetIDs = new List<uint>();

            AssetGRUP group = (AssetGRUP)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayerIndex, out _, ref outAssetIDs, "IP_RANDO_PLAYER_GRUP", template: AssetTemplate.Group));
            group.ReceiveEventDelegation = AssetGRUP.Delegation.RandomItem;
            group.LinksBFBB = new LinkBFBB[]
            {
                    new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                    {
                        Arguments_Float = new float[] {0, 0, 0, 0},
                        EventReceiveID = EventBFBB.ScenePrepare,
                        EventSendID = EventBFBB.Run,
                        TargetAssetID = group.AssetID
                    }
            };

            outAssetIDs = new List<uint>();
            for (int i = 0; i < 3; i++)
            {
                AssetTIMR timer = (AssetTIMR)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayerIndex, out _, ref outAssetIDs, "IP_RANDO_PLAYER_TIMR", template: AssetTemplate.Timer));
                timer.Time = 0.1f;
                timer.LinksBFBB = new LinkBFBB[]
                {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[] { i, 0, 0, 0 },
                            EventReceiveID = EventBFBB.Expired,
                            EventSendID = EventBFBB.SwitchPlayerCharacter,
                            TargetAssetID = "SPONGEBOB"
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[] { 0, 0, 0, 0 },
                            EventReceiveID = EventBFBB.Expired,
                            EventSendID = EventBFBB.Reset,
                            TargetAssetID = timer.AssetID
                        }
                };
            }

            List<AssetID> assetIDs = new List<AssetID>();
            foreach (uint i in outAssetIDs)
                assetIDs.Add(new AssetID(i));
            group.GroupItems = assetIDs.ToArray();

            return true;
        }

        private bool ShuffleVilTypes(int seed, List<VilType> chooseFrom, List<VilType> setTo, bool mixModels, bool veryRandom, bool enemies)
        {
            Random r = new Random(seed);

            if (veryRandom && (LevelName == "sm01" || LevelName == "gl01"))
            {
                HashSet<VilType> uniqueSetTo = new HashSet<VilType>();
                foreach (VilType v in setTo)
                    uniqueSetTo.Add(v);

                while (uniqueSetTo.Count > 5)
                {
                    VilType randomRemove = setTo[r.Next(0, setTo.Count)];
                    while (setTo.Contains(randomRemove))
                        setTo.Remove(randomRemove);
                    uniqueSetTo.Remove(randomRemove);
                }
            }

            if (setTo.Count == 0)
                return false;

            List<AssetVIL> assets = (from asset in assetDictionary.Values where asset is AssetVIL vil && chooseFrom.Contains(vil.VilType) select asset).Cast<AssetVIL>().ToList();
            List<VilType> viltypes = (from asset in assets select asset.VilType).ToList();
            List<AssetID> models = (from asset in assets select asset.Model_AssetID).ToList();

            foreach (AssetVIL a in assets)
            {
                VilType prevVilType = a.VilType;

                int viltypes_value = r.Next(0, viltypes.Count);
                int model_value = mixModels ? r.Next(0, viltypes.Count) : viltypes_value;

                a.VilType = veryRandom ? setTo[r.Next(0, setTo.Count)] : viltypes[viltypes_value];

                if (enemies && veryRandom)
                    a.Model_AssetID =
                        a.VilType == VilType.robot_sleepytime_bind ?
                        "robot_sleepy-time_bind.MINF" :
                        a.VilType.ToString() + ".MINF";

                else a.Model_AssetID = models[model_value];

                viltypes.RemoveAt(viltypes_value);
                models.RemoveAt(model_value);

                if (prevVilType == VilType.robot_arf_bind || prevVilType == VilType.tubelet_bind)
                {
                    List<LinkBFBB> links = a.LinksBFBB.ToList();
                    for (int i = 0; i < links.Count; i++)
                        if (links[i].EventSendID == EventBFBB.Connect_IOwnYou && ContainsAsset(links[i].TargetAssetID) &&
                            GetFromAssetID(links[i].TargetAssetID) is AssetVIL vil &&
                            (vil.VilType == VilType.tubelet_slave_bind || vil.VilType == VilType.robot_arf_dog_bind))
                        {
                            RemoveAsset(links[i].TargetAssetID);
                            links.RemoveAt(i);
                            i--;
                        }
                    a.LinksBFBB = links.ToArray();
                }

                if (a.VilType == VilType.robot_arf_bind || a.VilType == VilType.tubelet_bind)
                {
                    List<uint> assetIDs = new List<uint>();
                    int layerIndex = GetLayerFromAssetID(a.AHDR.assetID);
                    Vector3 position = a.Position;
                    List<LinkBFBB> links = a.LinksBFBB.ToList();
                    AssetVIL vil = (AssetVIL)GetFromAssetID(PlaceTemplate(position, layerIndex, out _, ref assetIDs,
                        "RANDO_" + (a.VilType == VilType.tubelet_bind ? "TUBELET" : "ARF"),
                       (a.VilType == VilType.tubelet_bind ? AssetTemplate.Tubelet : AssetTemplate.Arf)));
                    links.AddRange(vil.LinksBFBB);
                    a.LinksBFBB = links.ToArray();
                    RemoveAsset(vil.AHDR.assetID);
                    foreach (uint u in assetIDs)
                        if (ContainsAsset(u) && GetFromAssetID(u) is AssetMVPT)
                            RemoveAsset(u);
                }
            }

            return assets.Count != 0;
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
                        ((AssetDPAT)GetFromAssetID(chuckOffDisp)).EnabledOnStart = true;
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
                        List<LinkBFBB> flyLinks = fly.LinksBFBB.ToList();
                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventSendID == EventBFBB.Preload)
                            {
                                flyLinks.RemoveAt(i);
                                break;
                            }
                        fly.LinksBFBB = flyLinks.ToArray();
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
                    LinkBFBB[] csnmLinks = null;
                    uint kf01csnm = new AssetID("TUBELET_CUTSCENE_MGR");
                    if (ContainsAsset(kf01csnm))
                        csnmLinks = ((ObjectAsset)GetFromAssetID(kf01csnm)).LinksBFBB;

                    uint kf01fly = new AssetID("KF01_FLYTHOUGH_WIDGET");
                    if (ContainsAsset(kf01fly))
                    {
                        ObjectAsset flyWidged = ((ObjectAsset)GetFromAssetID(kf01fly));
                        List<LinkBFBB> flyLinks = flyWidged.LinksBFBB.ToList();
                        flyLinks.AddRange(csnmLinks);

                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventReceiveID == EventBFBB.Done)
                                flyLinks[i].EventReceiveID = EventBFBB.Stop;
                            else if (flyLinks[i].EventSendID == EventBFBB.Preload)
                                flyLinks.RemoveAt(i--);

                        flyWidged.LinksBFBB = flyLinks.ToArray();
                    }

                    return true;

                case "rb01":
                    uint rb01fly = new AssetID("RB01_FLYTHOUGH_WIDGET");

                    if (ContainsAsset(rb01fly))
                    {
                        AssetDYNA fly = (AssetDYNA)GetFromAssetID(rb01fly);
                        List<LinkBFBB> flyLinks = fly.LinksBFBB.ToList();
                        for (int i = 0; i < flyLinks.Count; i++)
                            if (flyLinks[i].EventSendID == EventBFBB.Preload)
                            {
                                flyLinks.RemoveAt(i);
                                break;
                            }
                        fly.LinksBFBB = flyLinks.ToArray();
                    }

                    uint sleepyDpat = new AssetID("SLEEPY_DESP_02");
                    if (ContainsAsset(sleepyDpat))
                        ((AssetDPAT)GetFromAssetID(sleepyDpat)).StateIsPersistent = false;
                    return true;
            }

            return false;
        }

        private bool ShuffleMRKRPositions(int seed, bool noWarps, bool pointers, bool plyrs, bool busStops, bool teleBox, bool taxis)
        {
            Random r = new Random(seed);

            List<IClickableAsset> assets = new List<IClickableAsset>();

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetMRKR mrkr && VerifyMarkerStep1(mrkr, noWarps, busStops, teleBox, taxis))
                    assets.Add(mrkr);
                else if (a is AssetDYNA dyna && pointers && dyna.Type_BFBB == DynaType_BFBB.pointer)
                    assets.Add(dyna);
                else if (plyrs && a is AssetPLYR plyr)
                    assets.Add(plyr);

            List<Vector3> positions = (from asset in assets select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();
            
            foreach (IClickableAsset a in assets)
            {
                int value = r.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);

                if (a is AssetDYNA dyna)
                    dyna.OnDynaSpecificPropertyChange(dyna.DynaBase);
            }

            return assets.Count != 0;
        }

        private bool VerifyMarkerStep1(AssetMRKR mrkr, bool noWarps, bool busStops, bool teleBox, bool taxis)
        {
            string assetName = mrkr.AHDR.ADBG.assetName;

            if (assetName.Contains("TEMP"))
                return false;
            if (noWarps && assetName.Contains("SPAT"))
                return false;

            List<uint> whoTargets = FindWhoTargets(mrkr.AHDR.assetID);
            if (whoTargets.Count > 0)
            {
                foreach (uint u in whoTargets)
                    if (GetFromAssetID(u) is AssetDYNA dyna)
                    {
                        if ((busStops && dyna.Type_BFBB == DynaType_BFBB.game_object__BusStop) ||
                            (taxis && dyna.Type_BFBB == DynaType_BFBB.game_object__Taxi) ||
                            (teleBox && dyna.Type_BFBB == DynaType_BFBB.game_object__Teleport))
                            return true;
                    }
                    else if (GetFromAssetID(u) is AssetTRIG trig)
                        foreach (LinkBFBB link in trig.LinksBFBB)
                            if (link.EventSendID == EventBFBB.SetCheckPoint)
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
                {
                    if (LevelName == "r020")
                    {
                        if (assetName.Contains("FROMR003"))
                            return false;
                    }
                }
            }
            catch { }

            return true;
        }

        private bool ShufflePlatSpeeds(int seed, RandomizerSettings settings)
        {
            Random r = new Random(seed);

            float minMultiSpeed = settings.speedMin;
            float maxMultiSpeed = settings.speedMax;
            float minMultiTime = settings.speedMax == 0 ? 0 : 1 / settings.speedMax;
            float maxMultiTime = settings.speedMin == 0 ? 0 : 1 / settings.speedMin;

            bool result = false;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPLAT plat)
                {
                    if (plat.PlatSpecific is PlatSpecific_ConveryorBelt p)
                    {
                        p.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = p;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_FallingPlatform pf)
                    {
                        pf.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = pf;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_BreakawayPlatform b)
                    {
                        b.BreakawayDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        b.ResetDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        plat.PlatSpecific = b;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_TeeterTotter tt)
                    {
                        tt.InverseMass *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = tt;
                    }

                    if (plat.Motion is Motion_MovePoint mp)
                    {
                        mp.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.Motion = mp;
                    }
                    else if (plat.Motion is Motion_Mechanism mc)
                    {
                        mc.PostRetractDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RetractDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateAccelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateDecelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideAccelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideDecelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        plat.Motion = mc;
                    }

                    result = true;
                }

            return result;
        }
        
        private bool RandomizeBoulderSettings(int seed, RandomizerSettings settings)
        {
            Random r = new Random(seed);

            float min = settings.boulderMin;
            float max = settings.boulderMax;

            List<AssetBOUL> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.BOUL select asset).Cast<AssetBOUL>().ToList();

            foreach (AssetBOUL boul in assets)
            {
                boul.Gravity *= r.NextFloat(min, max);
                boul.Mass *= r.NextFloat(min, max);
                boul.BounceFactor *= r.NextFloat(min, max);
                boul.Friction *= r.NextFloat(min, max);
                boul.StartFriction *= r.NextFloat(min, max);
                boul.MaxLinearVelocity *= r.NextFloat(min, max);
                boul.MaxAngularVelocity *= r.NextFloat(min, max);
                boul.Stickiness *= r.NextFloat(min, max);
                boul.BounceDamp *= r.NextFloat(min, max);
                boul.KillTimer *= r.NextFloat(min, max);
                boul.InnerRadius *= r.NextFloat(min, max);
                boul.OuterRadius *= r.NextFloat(min, max);
            }

            return assets.Count > 0;
        }

        public bool RandomizeSounds(int seed, bool mixTypes, bool scoobyBoot = false)
        {
            Random r = new Random(seed);

            bool result = false;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V1 sndi)
                {
                    List<EntrySoundInfo_GCN_V1> snd = sndi.Entries_SND.ToList();
                    List<EntrySoundInfo_GCN_V1> snds = sndi.Entries_SNDS.ToList();
                    
                    if (scoobyBoot)
                    {
                        List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                        foreach (var v in snds)
                            if (!GetFromAssetID(v.SoundAssetID).AHDR.ADBG.assetName.Contains("thera"))
                                sounds.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snds)
                            if (!GetFromAssetID(v.SoundAssetID).AHDR.ADBG.assetName.Contains("thera"))
                            {
                                int index = r.Next(0, sounds.Count);
                                v.SoundHeader = sounds[index].Item1;
                                GetFromAssetID(v.SoundAssetID).Data = sounds[index].Item2;
                                sounds.RemoveAt(index);
                            }
                    }
                    else if (mixTypes)
                    {
                        List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                        foreach (var v in snd)
                            sounds.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snds)
                            sounds.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snd)
                        {
                            int index = r.Next(0, sounds.Count);
                            v.SoundHeader = sounds[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = sounds[index].Item2;
                            sounds.RemoveAt(index);
                        }

                        foreach (var v in snds)
                        {
                            int index = r.Next(0, sounds.Count);
                            v.SoundHeader = sounds[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = sounds[index].Item2;
                            sounds.RemoveAt(index);
                        }
                    }
                    else
                    {
                        List<(byte[], byte[])> soundsSND = new List<(byte[], byte[])>();

                        foreach (var v in snd)
                            soundsSND.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snd)
                        {
                            int index = r.Next(0, soundsSND.Count);
                            v.SoundHeader = soundsSND[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = soundsSND[index].Item2;
                            soundsSND.RemoveAt(index);
                        }

                        List<(byte[], byte[])> soundsSNDS = new List<(byte[], byte[])>();

                        foreach (var v in snds)
                            soundsSNDS.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snds)
                        {
                            int index = r.Next(0, soundsSNDS.Count);
                            v.SoundHeader = soundsSNDS[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = soundsSNDS[index].Item2;
                            soundsSNDS.RemoveAt(index);
                        }
                    }

                    sndi.Entries_SND = snd.ToArray();
                    sndi.Entries_SNDS = snds.ToArray();

                    result = true;
                }

            return result;
        }
        
        private bool RandomizeMovePointRadius(int seed, RandomizerSettings settings)
        {
            Random r = new Random(seed);

            float min = settings.mvptMin;
            float max = settings.mvptMax;

            List<AssetMVPT_Scooby> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.MVPT select asset).Cast<AssetMVPT_Scooby>().ToList();

            foreach (AssetMVPT_Scooby mvpt in assets)
            {
                if (mvpt.ArenaRadius != -1)
                    mvpt.ArenaRadius *= r.NextFloat(min, max);
                if (mvpt is AssetMVPT mvpts)
                {
                    if (mvpts.ZoneRadius != -1)
                        mvpts.ZoneRadius *= r.NextFloat(min, max);
                    if (mvpts.Delay != -1)
                        mvpts.Delay *= r.NextFloat(min, max);
                }
            }

            return assets.Count != 0;
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
                    UnimportHip("Utility", "patrick.hip");
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
                    UnimportHip("Utility", "sandy.hip");
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
                int defaultLayer = GetLayerFromAssetID(new AssetID("EXIT_TO_HB05"));
                List<uint> nothing = new List<uint>();
                AssetPKUP spatula = (AssetPKUP)(GetFromAssetID(PlaceTemplate(new Vector3(8.774022f, 5.877692f, -23.492590f), defaultLayer, out _, ref nothing, template: AssetTemplate.Spatula)));
                spatula.LinksBFBB = new LinkBFBB[]
                {
                new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                {
                    EventReceiveID = EventBFBB.Pickup,
                    EventSendID = EventBFBB.Count2,
                    TargetAssetID = 0x5F45B82A,
                    Arguments_Float = new float[4]
                }
                };

                ((AssetDYNA)GetFromAssetID(new AssetID("EXIT_TALKBOX_HB05"))).LinksBFBB = new LinkBFBB[0];
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
                        int defaultLayer = GetLayerFromAssetID(dpat);
                        List<uint> vs = new List<uint>();
                        AssetTIMR timer = (AssetTIMR)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayer, out _, ref vs, template: AssetTemplate.Timer));
                        timer.Time = 1f;
                        uint boss = new AssetID("BOSS_NPC");
                        if (ContainsAsset(boss) && GetFromAssetID(boss) is AssetVIL spongebot)
                        {
                            uint transCut = new AssetID("BOSS3_TRANSITION_CSNMANAGER");
                            LinkBFBB[] links = spongebot.LinksBFBB;
                            for (int i = 0; i < links.Length; i++)
                                if (links[i].EventSendID == EventBFBB.Preload)
                                {
                                    links[i].TargetAssetID = timer.AssetID;
                                    links[i].EventSendID = EventBFBB.Run;
                                }
                            spongebot.LinksBFBB = links;
                            if (ContainsAsset(transCut) && GetFromAssetID(transCut) is AssetCSNM transitionCutscene)
                            {
                                LinkBFBB[] cutLinks = transitionCutscene.LinksBFBB;
                                for (int i = 0; i < cutLinks.Length; i++)
                                    if (cutLinks[i].EventReceiveID == EventBFBB.Play)
                                        cutLinks[i].EventReceiveID = EventBFBB.Run;
                                    else if (cutLinks[i].EventReceiveID == EventBFBB.Done)
                                        cutLinks[i].EventReceiveID = EventBFBB.Expired;
                                timer.LinksBFBB = cutLinks;
                            }
                        }
                        return true;
                    }
                    break;
                case "b303":
                    uint cutTimerAssetID = new AssetID("CUTSCN_TIMER");
                    if (ContainsAsset(cutTimerAssetID) && GetFromAssetID(cutTimerAssetID) is AssetTIMR cutTimer)
                    {
                        List<LinkBFBB> links = cutTimer.LinksBFBB.ToList();
                        uint cut = new AssetID("WIN_GAME_CTSNMANAGER");
                        if (ContainsAsset(cut) && GetFromAssetID(cut) is AssetCSNM cutManager)
                        {
                            links.AddRange(cutManager.LinksBFBB);
                            for (int i = 0; i < links.Count; i++)
                                if (links[i].EventSendID == EventBFBB.Preload)
                                    links.RemoveAt(i--);
                                else if (links[i].EventReceiveID == EventBFBB.Play)
                                    links[i].EventReceiveID = EventBFBB.Run;
                                else if (links[i].EventReceiveID == EventBFBB.Done)
                                    links[i].EventReceiveID = EventBFBB.Expired;
                            cutTimer.LinksBFBB = links.ToArray();
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

        public bool UnimportEnemies(HashSet<VilType> enemyVils)
        {
            foreach (VilType v in enemyVils)
            {
                string hipFileName;
                switch (v)
                {
                    case VilType.g_love_bind:
                        hipFileName = "g-love.HIP"; break;
                    case VilType.ham_bind:
                        hipFileName = "ham-mer.HIP"; break;
                    case VilType.robot_0a_bomb_bind:
                        hipFileName = "bomb-bot.HIP"; break;
                    case VilType.robot_0a_bzzt_bind:
                        hipFileName = "bzzt-bot.HIP"; break;
                    case VilType.robot_0a_chomper_bind:
                        hipFileName = "chomp-bot.HIP"; break;
                    case VilType.robot_0a_fodder_bind:
                        hipFileName = "fodder.HIP"; break;
                    case VilType.robot_4a_monsoon_bind:
                        hipFileName = "monsoon.HIP"; break;
                    case VilType.robot_9a_bind:
                        hipFileName = "slick.HIP"; break;
                    case VilType.robot_chuck_bind:
                        hipFileName = "chuck.HIP"; break;
                    case VilType.robot_sleepytime_bind:
                        hipFileName = "sleepytime.HIP"; break;
                    case VilType.robot_tar_bind:
                        hipFileName = "tar-tar.HIP"; break;
                    case VilType.robot_arf_bind:
                        hipFileName = "arf_arf-dawg.HIP"; break;
                    case VilType.tubelet_bind:
                        hipFileName = "tubelet.HIP"; break;
                    case VilType.tiki_wooden_bind:
                        hipFileName = "tiki_wooden.HIP"; break;
                    case VilType.tiki_lovey_dovey_bind:
                        hipFileName = "tiki_floating.HIP"; break;
                    case VilType.tiki_thunder_bind:
                        hipFileName = "tiki_thunder.HIP"; break;
                    case VilType.tiki_stone_bind:
                        hipFileName = "tiki_stone.HIP"; break;
                    case VilType.tiki_shhhh_bind:
                        hipFileName = "tiki_shhh.HIP"; break;
                    default:
                        throw new Exception("Invalid VilType");
                }

                string folderName = v.ToString().Contains("tiki") ? "Utility" : "Enemies";
                UnimportHip(folderName, hipFileName);
            }

            return true;
        }

        private void ReplaceReferences(uint oldAssetID, uint newAssetID)
        {
            foreach (uint a in FindWhoTargets(oldAssetID))
            {
                if (GetFromAssetID(a) is ObjectAsset asset)
                {
                    LinkBFBB[] links = asset.LinksBFBB;
                    for (int i = 0; i < links.Length; i++)
                        if (links[i].TargetAssetID == oldAssetID)
                            links[i].TargetAssetID = newAssetID;
                    asset.LinksBFBB = links.ToArray();
                }
                else if (GetFromAssetID(a) is AssetGRUP grup)
                {
                    List<AssetID> assetIDs = grup.GroupItems.ToList();
                    for (int i = 0; i < assetIDs.Count; i++)
                        if (assetIDs[i] == oldAssetID)
                            assetIDs[i] = newAssetID;
                    grup.GroupItems = assetIDs.ToArray();
                }
            }
        }

        private HashSet<VilType> GetVilTypesInLevel(List<VilType> chooseFrom)
        {
            HashSet<VilType> viltypes = new HashSet<VilType>();

            foreach (AssetVIL a in (from asset in assetDictionary.Values
                                    where asset is AssetVIL vil && chooseFrom.Contains(vil.VilType)
                                    select asset).Cast<AssetVIL>())
                viltypes.Add(a.VilType);

            return viltypes;
        }

        private static VilType[] importVilTypes = new VilType[] {
            VilType.g_love_bind,
            VilType.ham_bind,
            VilType.robot_0a_bomb_bind,
            VilType.robot_0a_bzzt_bind,
            VilType.robot_0a_chomper_bind,
            VilType.robot_0a_fodder_bind,
            VilType.robot_4a_monsoon_bind,
            VilType.robot_9a_bind,
            VilType.robot_chuck_bind,
            VilType.robot_sleepytime_bind,
            VilType.robot_tar_bind,
            VilType.robot_arf_bind,
            VilType.tubelet_bind,
            VilType.tiki_lovey_dovey_bind,
            VilType.tiki_shhhh_bind,
            VilType.tiki_stone_bind,
            VilType.tiki_thunder_bind,
            VilType.tiki_wooden_bind
        };

        public HashSet<VilType> GetEnemyTypes()
        {
            HashSet<VilType> outSet = new HashSet<VilType>();
            foreach (AssetVIL a in (from asset in assetDictionary.Values
                                    where asset is AssetVIL vil && importVilTypes.Contains(vil.VilType)
                                    select asset).Cast<AssetVIL>())
            {
                if (!ContainsAsset(a.Model_AssetID))
                    outSet.Add(a.VilType);
            }
            return outSet;
        }

        public bool ImportEnemyTypes(HashSet<VilType> inSet)
        {
            bool imported = false;

            foreach (VilType v in inSet)
            {
                if (ContainsAsset(new AssetID(v.ToString().Replace("sleepytime", "sleepy-time") + ".MINF")))
                    continue;

                string hipFileName = null;
                switch (v)
                {
                    case VilType.g_love_bind:
                        hipFileName = "g-love.HIP"; break;
                    case VilType.ham_bind:
                        hipFileName = "ham-mer.HIP"; break;
                    case VilType.robot_0a_bomb_bind:
                        hipFileName = "bomb-bot.HIP"; break;
                    case VilType.robot_0a_bzzt_bind:
                        hipFileName = "bzzt-bot.HIP"; break;
                    case VilType.robot_0a_chomper_bind:
                        hipFileName = "chomp-bot.HIP"; break;
                    case VilType.robot_0a_fodder_bind:
                        hipFileName = "fodder.HIP"; break;
                    case VilType.robot_4a_monsoon_bind:
                        hipFileName = "monsoon.HIP"; break;
                    case VilType.robot_9a_bind:
                        hipFileName = "slick.HIP"; break;
                    case VilType.robot_chuck_bind:
                        hipFileName = "chuck.HIP"; break;
                    case VilType.robot_sleepytime_bind:
                        hipFileName = "sleepytime.HIP"; break;
                    case VilType.robot_tar_bind:
                        hipFileName = "tar-tar.HIP"; break;
                    case VilType.robot_arf_bind:
                        hipFileName = "arf_arf-dawg.HIP"; break;
                    case VilType.tubelet_bind:
                        hipFileName = "tubelet.HIP"; break;
                    case VilType.tiki_wooden_bind:
                        hipFileName = "tiki_wooden.HIP"; break;
                    case VilType.tiki_lovey_dovey_bind:
                        hipFileName = "tiki_floating.HIP"; break;
                    case VilType.tiki_thunder_bind:
                        hipFileName = "tiki_thunder.HIP"; break;
                    case VilType.tiki_stone_bind:
                        hipFileName = "tiki_stone.HIP"; break;
                    case VilType.tiki_shhhh_bind:
                        hipFileName = "tiki_shhh.HIP"; break;
                    default:
                        throw new Exception("Invalid VilType");
                }

                ProgImportHip(v.ToString().Contains("tiki") ? "Utility" : "Enemies", hipFileName);
                imported = true;
            }

            return imported;
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
            DICT.LTOC.LHDRList.RemoveAt(14);
            DICT.LTOC.LHDRList[13].assetIDlist.Add(assetID);
            return true;
        }

        public void ProgImportHip(string folderName, string fileName)
        {
            ImportHip(Path.Combine(editorFilesFolder, "BattleForBikiniBottom", platform.ToString(), folderName, fileName), true);
        }

        private void UnimportHip(string folderName, string fileName)
        {
            UnimportHip(new HipFile(Path.Combine(editorFilesFolder, "BattleForBikiniBottom", platform.ToString(), folderName, fileName)).DICT);
        }
        
        public void UnimportHip(Section_DICT dict)
        {
            UnsavedChanges = true;

            foreach (Section_AHDR AHDR in dict.ATOC.AHDRList)
            {
                switch (AHDR.assetType)
                {
                    case AssetType.COLL:
                    case AssetType.JAW:
                    case AssetType.LODT:
                    case AssetType.SHDW:
                    case AssetType.SNDI:
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
                        if (port.AHDR.ADBG.assetName == "TOHB01")
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
                        else if (port.AHDR.ADBG.assetName == "TOJF01")
                            return true;
                        else if (port.AHDR.ADBG.assetName == "TOBB01")
                            return true;
                        else if (port.AHDR.ADBG.assetName == "TOGL01")
                            return true;
                        else if (port.AHDR.ADBG.assetName == "TOBC01")
                            return true;
                        break;
                    case "jf01":
                        if (port.AHDR.ADBG.assetName == "TAXISTAND_PORTAL_01")
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
                        if (port.AHDR.ADBG.assetName == "TOHUB_PORTAL")
                            return true;
                        break;
                    case "sm01":
                        if (port.AHDR.ADBG.assetName == "TOHB01")
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
                        if (dest == "i004" || port.AHDR.ADBG.assetName == "TOR001A")
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
                        if (port.AHDR.ADBG.assetName == "TOP005A")
                            return true;
                        break;
                    case "p005":
                        if (port.AHDR.ADBG.assetName == "TOP004A")
                            return true;
                        break;
                    case "r001":
                        if (port.AHDR.ADBG.assetName == "TOI005A")
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
            

            foreach (string s in toSkip)
                if (dest.Contains(s.ToLower()))
                    return true;
                else if (new string(dest.ToArray()).ToLower().Contains(s.ToLower()))
                    return true;

            return false;
        }

        public bool SetWarpNames(Random r, ref List<string> warpNames, ref List<(string, string, string)> warpRandomizerOutput, HashSet<string> unique)
        {
            foreach (AssetPORT port in warpsRandomizer)
            {
                if (warpNames.Count == 0)
                    throw new Exception("warpNames is empty");

                int index;
                int times = 0;
                do
                {
                    index = r.Next(0, warpNames.Count);
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

        private string musicDispAssetName => "IP_RANDO_DISP";
        private string musicGroupAssetName => "IP_RANDO_GROUP";

        public bool RandomizePlaylistLocal()
        {
            string musicDispAssetName = "MUSIC_DISP";

            if (ContainsAsset(new AssetID(musicDispAssetName)))
            {
                List<uint> assetIDs = FindWhoTargets(new AssetID(musicDispAssetName));
                foreach (uint assetID in assetIDs)
                {
                    ObjectAsset objectAsset = (ObjectAsset)GetFromAssetID(assetID);
                    LinkBFBB[] links = objectAsset.LinksBFBB;
                    foreach (LinkBFBB link in links)
                        if (link.EventSendID == EventBFBB.PlayMusic)
                        {
                            link.EventSendID = EventBFBB.Run;
                            link.Arguments_Float = new float[] { 0, 0, 0, 0 };
                            link.TargetAssetID = musicGroupAssetName + "_01";
                        }
                    objectAsset.LinksBFBB = links;
                }

                return true;
            }
            return false;
        }

        public bool RandomizePlaylist()
        {
            if (ContainsAsset(new AssetID(musicDispAssetName + "_01")) && ContainsAsset(new AssetID(musicGroupAssetName + "_01")))
                return false;

            int defaultLayerIndex = -1;
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].layerType == (int)LayerType_BFBB.DEFAULT)
                {
                    defaultLayerIndex = i;
                    break;
                }

            List<uint> outAssetIDs = new List<uint>();
            uint dpat = PlaceTemplate(new Vector3(), defaultLayerIndex, out _, ref outAssetIDs, musicDispAssetName, template: AssetTemplate.Dispatcher);

            AssetGRUP group = (AssetGRUP)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayerIndex, out _, ref outAssetIDs, musicGroupAssetName, template: AssetTemplate.Group));
            group.ReceiveEventDelegation = AssetGRUP.Delegation.RandomItem;
            group.LinksBFBB = new LinkBFBB[]
            {
                new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                {
                    Arguments_Float = new float[] {0, 0, 0, 0},
                    EventReceiveID = EventBFBB.ScenePrepare,
                    EventSendID = EventBFBB.Run,
                    TargetAssetID = group.AssetID
                }
            };

            outAssetIDs = new List<uint>();
            for (int i = 0; i < 17; i++)
            {
                if (i == 7 || i == 14)
                    continue;

                AssetTIMR timer = (AssetTIMR)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayerIndex, out _, ref outAssetIDs, "IP_RANDO_TIMR", template: AssetTemplate.Timer));
                timer.Time = 0.1f;
                var links = new List<LinkBFBB>()
                {
                    new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                    {
                        Arguments_Float = new float[] {i, 0, 0, 0},
                        EventReceiveID = EventBFBB.Expired,
                        EventSendID = EventBFBB.PlayMusic,
                        TargetAssetID = dpat
                    },
                    new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                    {
                        Arguments_Float = new float[] {0, 0, 0, 0},
                        EventReceiveID = EventBFBB.Expired,
                        EventSendID = EventBFBB.Reset,
                        TargetAssetID = timer.AssetID
                    },
                };
                
                timer.LinksBFBB = links.ToArray();
            }

            List<AssetID> assetIDs = new List<AssetID>();
            foreach (uint i in outAssetIDs)
                assetIDs.Add(new AssetID(i));
            group.GroupItems = assetIDs.ToArray();

            return true;
        }

        public bool MultiplyLODT(float value)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetLODT lodt && lodt.AHDR.ADBG.assetFileName != "lodt_doubled")
                {
                    lodt.AHDR.ADBG.assetFileName = "lodt_doubled";

                    EntryLODT[] lodtEntries = lodt.LODT_Entries;
                    foreach (var t in lodtEntries)
                    {
                        t.MaxDistance *= value;
                        t.LOD1_Distance *= value;
                        t.LOD2_Distance *= value;
                        t.LOD3_Distance *= value;
                    }

                    lodt.LODT_Entries = lodtEntries;
                }

            return true;
        }

        private bool ShuffleSpatulaGates(Random r, bool shuffleSpatulaGates, RandomizerSettings settings, out bool needToAddNumbers)
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

                                string serializedObject = JsonConvert.SerializeObject(plat.AHDR);
                                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                                uint newAssetID = AddAssetWithUniqueID(GetLayerFromAssetID(numRightAssetID), AHDR);

                                AssetPLAT plat2 = (AssetPLAT)GetFromAssetID(newAssetID);

                                plat2.PositionX = 87.692600f;
                                plat2.PositionZ = 8.692189f;

                                platAssetIDs.Add(numRightAssetID);
                                platAssetIDs.Add(newAssetID);

                                plat.LinksBFBB = new LinkBFBB[] { new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                                {
                                    EventReceiveID = EventBFBB.Invisible,
                                    EventSendID = EventBFBB.Invisible,
                                    TargetAssetID = plat2.AssetID,
                                    Arguments_Float = new float[]{ 77, 0, 0, 0 },
                                }
                                };
                            }

                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_BB_COND_01"));
                            int i = 0;
                            foreach (uint u in platAssetIDs)
                                SetNumberPlats(value, i++, u);

                            ReplaceInText(new AssetID("Spatula_exit_bb01_text"), "5", value.ToString());
                        }

                        // GL
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_GL_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_0_GL_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_1_GL_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_gl01_text"), "10", value.ToString());

                            ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_GL_MECH_01"))).Yaw += 180f;
                        }
                        // H2
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_H2_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_5_H2_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_1_H2_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_hub2_text"), "15", value.ToString());
                        }
                        // RB
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_RB_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_5_RB_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_2_RB_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_rb01_text"), "25", value.ToString());
                        }
                        // SM
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_SM_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_0_SM_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_3_SM_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_sm01_text"), "30", value.ToString());

                            ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_SM_MECH_01"))).Yaw += 180f;
                        }
                        // H3
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_H3_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_0_H3_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_4_H3_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_hub3_text"), "40", value.ToString());

                            ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_H3_MECH_01"))).Yaw += 180f;
                        }
                        // KF
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
                            SetCondEvaluationAmount(value - 1, new AssetID("TOLL_BOOTH_KF_COND_01"));
                            SetNumberPlats(value, 0, new AssetID("NUMBER_0_KF_MECH_01"));
                            SetNumberPlats(value, 1, new AssetID("NUMBER_5_KF_MECH_01"));
                            ReplaceInText(new AssetID("Spatula_exit_kf01_text"), "50", value.ToString());

                            ((AssetPLAT)GetFromAssetID(new AssetID("NUMBER_0_KF_MECH_01"))).Yaw += 180f;
                        }
                        // GY
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);
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
                        if (ContainsAssetWithType(AssetType.COND))
                        {
                            int value = r.Next(settings.spatReqMin, settings.spatReqMax + 1);

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

        private bool ShuffleShinyGates(Random r, RandomizerSettings settings, out bool needToAddNumbers)
        {
            needToAddNumbers = false;

            switch (LevelName)
            {
                case "bb01":
                case "bb02":
                    {
                        int originalShinyAmount = 2100;
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                            SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        int shinyAmount = (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax));
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
                        SetGate(originalShinyAmount50, (int)(originalShinyAmount50 * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                             new List<uint> { new AssetID("SOGATE_COND_01"), },
                             new List<uint> { new AssetID("SOGATE_COND_01"), },
                             new List<uint> { new AssetID("SHINY_OBJ_MECH_04"), new AssetID("SHINY_OBJ_MECH_03"), },
                             new List<uint> { 0xB5FF7865, 0x9AA7AE41, });

                        int originalShinyAmount10 = 10;
                        SetGate(originalShinyAmount10, (int)(originalShinyAmount10 * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
                             new List<uint> { new AssetID("SOGATE_COND_02"), },
                             new List<uint> { new AssetID("SOGATE_COND_02"), },
                             new List<uint> { new AssetID("SHINY_OBJ_MECH_07"), new AssetID("SHINY_OBJ_MECH_06"), },
                             new List<uint> { 0x21BA9BE1, 0x23CE2B75, });

                        int originalShinyAmount20 = 20;
                        SetGate(originalShinyAmount20, (int)(originalShinyAmount20 * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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

                            SetGate(originalShinyAmount, (int)(originalShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
                        SetGate(bungeeShinyAmount, (int)(bungeeShinyAmount * r.NextFloat(settings.shinyReqMin, settings.shinyReqMax)),
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
            if (ContainsAsset(assetID) && GetFromAssetID(assetID) is ObjectAsset objectAsset)
            {
                LinkBFBB[] links = objectAsset.LinksBFBB;
                for (int i = 0; i < links.Length; i++)
                    if (links[i].EventSendID == EventBFBB.GiveShinyObjects)
                    {
                        float[] arguments = links[i].Arguments_Float;
                        arguments[0] = shinyAmount;
                        links[i].Arguments_Float = arguments;
                    }
                objectAsset.LinksBFBB = links;
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
                ((PlaceableAsset)GetFromAssetID(assetID)).Model_AssetID = modelName;
            else
                MessageBox.Show("Placeable asset " + assetID.ToString("X8") + " not found on file " + LevelName);
        }
    }
}