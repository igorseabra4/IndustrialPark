using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static void PopulateTemplateMenusAt(ToolStripMenuItem menu, EventHandler eventHandler)
        {
            ToolStripMenuItem controllers = new ToolStripMenuItem("Stage Controllers");
            controllers.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Counter.ToString()),
                new ToolStripMenuItem(AssetTemplate.Conditional.ToString()),
                new ToolStripMenuItem(AssetTemplate.Dispatcher.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fog.ToString()),
                new ToolStripMenuItem(AssetTemplate.Group.ToString()),
                new ToolStripMenuItem(AssetTemplate.Portal.ToString()),
                new ToolStripMenuItem(AssetTemplate.Script.ToString()),
                new ToolStripMenuItem(AssetTemplate.SoundGroup.ToString()),
                new ToolStripMenuItem(AssetTemplate.Text.ToString()),
                new ToolStripMenuItem(AssetTemplate.Timer.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.CamTweak.ToString())
            });
            foreach (ToolStripItem i in controllers.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            // BFBB
            ToolStripMenuItem pickupsBFBB = new ToolStripMenuItem("Pickups and Tikis");
            pickupsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Shiny_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Yellow.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Green.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Purple.ToString()),
                new ToolStripMenuItem(AssetTemplate.Underwear.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spongeball.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.WoodenTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.FloatingTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThunderTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.ShhhTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.StoneTiki.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Spatula.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sock.ToString()),
                new ToolStripMenuItem(AssetTemplate.Golden_Underwear.ToString()),
                new ToolStripMenuItem(AssetTemplate.Smelly_Sundae.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.SteeringWheel.ToString()),
                new ToolStripMenuItem(AssetTemplate.Artwork.ToString()),
                new ToolStripMenuItem(AssetTemplate.PowerCrystal.ToString()),
            });
            foreach (ToolStripItem i in pickupsBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem enemiesBFBB = new ToolStripMenuItem("Enemies");
            enemiesBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Fodder.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hammer.ToString()),
                new ToolStripMenuItem(AssetTemplate.TarTar.ToString()),
                new ToolStripMenuItem(AssetTemplate.ChompBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.GLove.ToString()),
                new ToolStripMenuItem(AssetTemplate.Chuck.ToString()),
                new ToolStripMenuItem(AssetTemplate.Chuck_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Monsoon.ToString()),
                new ToolStripMenuItem(AssetTemplate.Monsoon_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sleepytime.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sleepytime_Moving.ToString()),
                new ToolStripMenuItem(AssetTemplate.Arf.ToString()),
                new ToolStripMenuItem(AssetTemplate.BombBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.Tubelet.ToString()),
                new ToolStripMenuItem(AssetTemplate.BzztBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slick.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slick_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jellyfish_Pink.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jellyfish_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Duplicatotron.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.DuplicatotronSettings.ToString())
            });
            foreach (ToolStripItem i in enemiesBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem stageitemsBFBB = new ToolStripMenuItem("Stage Items");
            stageitemsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Button_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.PressurePlate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint_Invisible.ToString()),
                new ToolStripMenuItem(AssetTemplate.BusStop.ToString()),
                new ToolStripMenuItem(AssetTemplate.TeleportBox.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThrowFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.FreezyFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.TexasHitch.ToString()),
                new ToolStripMenuItem(AssetTemplate.TexasHitch_PLAT.ToString()),
                new ToolStripMenuItem(AssetTemplate.BungeeHook.ToString()),
                new ToolStripMenuItem(AssetTemplate.BungeeDrop.ToString()),
                new ToolStripMenuItem(AssetTemplate.HoveringPlatform.ToString()),
                new ToolStripMenuItem(AssetTemplate.Springboard.ToString()),
                new ToolStripMenuItem(AssetTemplate.TaxiStand.ToString()),
            });
            foreach (ToolStripItem i in stageitemsBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem bfbb = new ToolStripMenuItem("Battle For Bikini Bottom");
            bfbb.DropDownItems.AddRange(new ToolStripItem[]
            {
                pickupsBFBB,
                enemiesBFBB,
                stageitemsBFBB
            });

            // Movie
            ToolStripMenuItem pickupsTSSM = new ToolStripMenuItem("Pickups and Crates");
            pickupsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Manliness_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Yellow.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Green.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Purple.ToString()),
                new ToolStripMenuItem(AssetTemplate.KrabbyPatty.ToString()),
                new ToolStripMenuItem(AssetTemplate.GoofyGooberToken.ToString()),
                new ToolStripMenuItem(AssetTemplate.TreasureChest.ToString()),
                new ToolStripMenuItem(AssetTemplate.Nitro.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Wood_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hover_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Explode_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shrink_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Steel_Crate.ToString()),
            });
            foreach (ToolStripItem i in pickupsTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem enemiesTSSM = new ToolStripMenuItem("Enemies");
            enemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Fogger_GoofyGoober.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Junkyard.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Planktopolis.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v1.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v2.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v3.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Slammer_GoofyGoober.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slammer_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slammer_ThugTug.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Flinger_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Flinger_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Flinger_Junkyard.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Spinner_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spinner_Junkyard.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spinner_Planktopolis.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Popper_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Popper_Planktopolis.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Minimerv.ToString()),
                new ToolStripMenuItem(AssetTemplate.Mervyn.ToString()),
            });
            foreach (ToolStripItem i in enemiesTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem moreEnemiesTSSM = new ToolStripMenuItem("More Enemies");
            moreEnemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Jelly_Critter.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jelly_Bucket.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Turret_v1.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v2.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v3.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_BB.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_DE.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_GG.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_TR.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_JK.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_PT.ToString())
            });
            foreach (ToolStripItem i in moreEnemiesTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem stageitemsTSSM = new ToolStripMenuItem("Stage Items");
            stageitemsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Button_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint_Invisible.ToString()),
                new ToolStripMenuItem(AssetTemplate.TeleportBox.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThrowFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.FreezyFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger_PLAT.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_Planktopolis.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_Spongeball.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Ring.ToString()),
                new ToolStripMenuItem(AssetTemplate.RingControl.ToString())
            });
            foreach (ToolStripItem i in stageitemsTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem tssm = new ToolStripMenuItem("Movie Game");
            tssm.DropDownItems.AddRange(new ToolStripItem[]
            {
                pickupsTSSM,
                enemiesTSSM,
                moreEnemiesTSSM,
                stageitemsTSSM
            });

            ToolStripMenuItem placeable = new ToolStripMenuItem("Placeable");
            placeable.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Camera.ToString()),
                new ToolStripMenuItem(AssetTemplate.Marker.ToString()),
                new ToolStripMenuItem(AssetTemplate.Point_MVPT.ToString()),
                new ToolStripMenuItem(AssetTemplate.Area_MVPT.ToString()),
                new ToolStripMenuItem(AssetTemplate.Box_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sphere_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Cylinder_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.SFX_OnEvent.ToString()),
                new ToolStripMenuItem(AssetTemplate.SFX_OnRadius.ToString()),
                new ToolStripMenuItem(AssetTemplate.Dyna_Pointer.ToString()),
                new ToolStripMenuItem(AssetTemplate.Player.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Boulder_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Button_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Destructible_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.ElectricArc_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hangable_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.NPC_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Pendulum_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Platform_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.SIMP_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.VIL_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.UI_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.UIFT_Generic.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.SDFX.ToString()),
                new ToolStripMenuItem(AssetTemplate.LightEmitter_Generic.ToString()),
            });
            foreach (ToolStripItem i in placeable.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem others = new ToolStripMenuItem("Other");
            others.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.AnimationList.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollisionTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.Default_Glow_Scene_Prop.ToString()),
                new ToolStripMenuItem(AssetTemplate.Environment.ToString()),
                new ToolStripMenuItem(AssetTemplate.JawData.ToString()),
                new ToolStripMenuItem(AssetTemplate.LevelOfDetailTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.MaterialMap.ToString()),
                new ToolStripMenuItem(AssetTemplate.MINF_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.PipeInfoTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.ShadowTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.SoundInfo.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.EmptyBSP.ToString()),
                new ToolStripMenuItem(AssetTemplate.EmptySND.ToString()),
                new ToolStripMenuItem(AssetTemplate.EmptySNDS.ToString()),
            });
            foreach (ToolStripItem i in others.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem paste = new ToolStripMenuItem(AssetTemplate.PasteClipboard.ToString());
            paste.Click += eventHandler;

            menu.DropDownItems.AddRange(new ToolStripItem[] { controllers, placeable, bfbb, tssm, others, paste });
        }

        public void SetAssetPositionToView(uint assetID)
        {
            Vector3 Position = Program.MainForm.renderer.Camera.Position + 3 * Program.MainForm.renderer.Camera.Forward;

            if (GetFromAssetID(assetID) is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.Forward);
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.Up);
                cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.Right);
            }
            else if (GetFromAssetID(assetID) is IClickableAsset ir)
            {
                ir.PositionX = Position.X;
                ir.PositionY = Position.Y;
                ir.PositionZ = Position.Z;
            }
        }
        
        public static AssetTemplate CurrentAssetTemplate { get; set; } = AssetTemplate.Null;
        public static string CurrentUserTemplate { get; set; } = "";

        public static bool persistentShinies = true;
        public static bool chainPointMVPTs = false;
        public static uint chainPointMVPTlast = 0;

        private uint PlaceUserTemplate(Vector3 position, int layerIndex, ref List<uint> assetIDs, AssetTemplate template)
        {
            if (template == AssetTemplate.PasteClipboard)
                PasteAssetsFromClipboard(layerIndex, out assetIDs, dontReplace: true);
            else
            {
                try
                {
                    var clipboard = JsonConvert.DeserializeObject<AssetClipboard>(File.ReadAllText(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate)));
                    PasteAssetsFromClipboard(layerIndex, out assetIDs, clipboard, forceRefUpdate: true, dontReplace: true);
                }
                catch
                {
                    return 0;
                }
            }

            if (assetIDs.Count > 0)
                foreach (uint assetID in assetIDs)
                    if (GetFromAssetID(assetID) is AssetCAM cam)
                    {
                        cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                        cam.SetNormalizedForward(Program.MainForm.renderer.Camera.Forward);
                        cam.SetNormalizedUp(Program.MainForm.renderer.Camera.Up);
                        cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.Right);
                    }
                    else if (GetFromAssetID(assetID) is IClickableAsset ica)
                    {
                        Vector3 delta = position - new Vector3(ica.PositionX, ica.PositionY, ica.PositionZ);

                        ica.PositionX = position.X;
                        ica.PositionY = position.Y;
                        ica.PositionZ = position.Z;

                        if (ica is AssetTRIG trig)
                        {
                            if (trig.Shape == TriggerShape.Box)
                            {
                                trig.SetPositions(
                                    trig.Position0X + delta.X,
                                    trig.Position0Y + delta.Y,
                                    trig.Position0Z + delta.Z,
                                    trig.Position1X + delta.X,
                                    trig.Position1Y + delta.Y,
                                    trig.Position1Z + delta.Z);
                            }
                            else
                            {
                                trig.Position0X = position.X;
                                trig.Position0Y = position.Y;
                                trig.Position0Z = position.Z;
                            }
                        }
                    }

            return 0;
        }

        public uint PlaceTemplate(Vector3 position, int layerIndex, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            var assetIDs = new List<uint>();
            return PlaceTemplate(position, layerIndex, ref assetIDs, customName, template);
        }

        public uint PlaceTemplate(Vector3 position, int layerIndex, ref List<uint> assetIDs, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            throw new NotImplementedException();
        }

        public static int MaximumBoundary => 1000;

        private byte[] GenerateBlankBSP()
        {
            Vertex3 Max = new Vertex3(MaximumBoundary, MaximumBoundary, MaximumBoundary);
            Vertex3 Min = new Vertex3(-MaximumBoundary, -MaximumBoundary, -MaximumBoundary);

            return ReadFileMethods.ExportRenderWareFile(new World_000B()
            {
                worldStruct = new WorldStruct_0001()
                {
                    rootIsWorldSector = 1,
                    inverseOrigin = new Vertex3(-0f, -0f, -0f),
                    numTriangles = 0,
                    numVertices = 0,
                    numPlaneSectors = 0,
                    numAtomicSectors = 1,
                    colSectorSize = 0,
                    worldFlags = WorldFlags.HasOneSetOfTextCoords | WorldFlags.HasVertexColors | WorldFlags.WorldSectorsOverlap | (WorldFlags)0x00010000,
                    boxMaximum = new Vertex3(),
                    boxMinimum = new Vertex3(),
                },

                materialList = new MaterialList_0008()
                {
                    materialListStruct = new MaterialListStruct_0001()
                    {
                        materialCount = 0
                    },
                    materialList = new Material_0007[0]
                },

                firstWorldChunk = new AtomicSector_0009()
                {
                    atomicSectorStruct = new AtomicSectorStruct_0001()
                    {
                        matListWindowBase = 0,
                        numTriangles = 0,
                        numVertices = 0,
                        boxMaximum = Max,
                        boxMinimum = Min,
                        collSectorPresent = 0,
                        unused = 0,
                        vertexArray = new Vertex3[0],
                        colorArray = new RenderWareFile.Color[0],
                        uvArray = new Vertex2[0],
                        triangleArray = new Triangle[0]
                    },
                    atomicSectorExtension = new Extension_0003()
                    {
                        extensionSectionList = new List<RWSection>()
                        { 
                            new BinMeshPLG_050E()
                            {
                                binMeshHeaderFlags = BinMeshHeaderFlags.TriangleList,
                                numMeshes = 0,
                                totalIndexCount = 0,
                                binMeshList = new BinMesh[0]
                            },
                            new CollisionPLG_011D_Scooby()
                            {
                                splits = new Split_Scooby[0],
                                startIndex_amountOfTriangles = new short[][] { new short[] { 0, 0 } },
                                triangles = new int[0]
                            }
                        }
                    }
                },

                worldExtension = new Extension_0003()
            }, 0x0310);
        }

        public static string pkupsMinfName => "pickups.MINF";
        public string playerName => game == Game.Scooby ? "SCOOBY" : game == Game.BFBB ? "SPONGEBOB" : "HERO";
        public string environmentName => game == Game.Scooby ? "ENV" : game == Game.BFBB ? "ENVIRONMENT" : "ENV_THEWORLD";
        public string startCamName => game == Game.Scooby ? "START" : game == Game.BFBB ? "STARTCAM" : "CAM_START";

        private void PlaceDefaultAssets()
        {
            if (game != Game.Incredibles)
                AddLayer((int)LayerType_BFBB.BSP);
            AddLayer();

            int defaultLayer = game == Game.Incredibles ? 0 : 1;

            AssetPLYR player = (AssetPLYR)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayer, template: AssetTemplate.Player));

            AssetENV env = (AssetENV)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayer, customName: environmentName, template: AssetTemplate.Environment));

            env.StartCameraAssetID = PlaceTemplate(new Vector3(0, 100, 100), defaultLayer, customName: startCamName, template: AssetTemplate.StartCamera);

            if (game != Game.Incredibles)
            {
                env.BSP_AssetID = PlaceTemplate(new Vector3(), 0, customName: "empty_bsp", template: AssetTemplate.EmptyBSP);
                PlaceTemplate(new Vector3(), defaultLayer, customName: pkupsMinfName, template: AssetTemplate.MINF_Generic);
            }

            if (game == Game.BFBB)
            {
                env.Object_LKIT_AssetID = PlaceTemplate(new Vector3(), defaultLayer, customName: "lights", template: AssetTemplate.LKIT_lights);
                player.LightKit_AssetID = PlaceTemplate(new Vector3(), defaultLayer, customName: "JF_SB_lights", template: AssetTemplate.LKIT_JF_SB_lights);
            }
            else if (game == Game.Incredibles)
            {
                AssetLKIT light_kit = (AssetLKIT)GetFromAssetID(PlaceTemplate(new Vector3(), defaultLayer, customName: "jf01_light_kit", template: AssetTemplate.LKIT_lights));
                player.LightKit_AssetID = light_kit.assetID;
                env.Object_LKIT_AssetID = light_kit.assetID;

                PlaceTemplate(new Vector3(), defaultLayer, customName: "DEFAULT_GLOW_SCENE_PROP", template: AssetTemplate.Default_Glow_Scene_Prop);
            }
        }
    }
}