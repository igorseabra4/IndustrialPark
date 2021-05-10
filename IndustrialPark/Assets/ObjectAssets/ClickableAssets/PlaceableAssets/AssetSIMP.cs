using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : EntityAsset
    {
        private const string categoryName = "Simple Object";

        [Category(categoryName)]
        public AssetSingle AnimSpeed { get; set; }
        [Category(categoryName)]
        public int InitialAnimState { get; set; }
        [Category(categoryName), Description("Only Static is functional.")]
        public FlagBitmask CollType { get; set; } = ByteFlagsDescriptor(
            "Trigger",
            "Static",
            "Dynamic",
            "NPC",
            "Player");
        [Category(categoryName), Description("Always 0.")]
        public FlagBitmask SimpFlags { get; set; } = ByteFlagsDescriptor();

        public AssetSIMP(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.SIMP, BaseAssetType.Static, position)
        {
            AnimSpeed = 1f;
            CollType.FlagValueByte = 2;

            switch (template)
            {
                case AssetTemplate.BungeeHook_SIMP:
                    Model_AssetID = "bungee_hook";
                    CollType.FlagValueByte = 0;
                    break;
                case AssetTemplate.BusStop:
                    Model_AssetID = "bus_stop";
                    ScaleX = 2f;
                    ScaleY = 2f;
                    ScaleZ = 2f;
                    break;
                case AssetTemplate.BusStop_BusSimp:
                    PositionX -= 3f;
                    VisibilityFlags.FlagValueByte = 0;
                    SolidityFlags.FlagValueByte = 0;
                    CollType.FlagValueByte = 0;
                    Model_AssetID = "bus_bind";
                    Animation_AssetID = "BUSSTOP_ANIMLIST_01";
                    break;
                case AssetTemplate.BusStop_Lights:
                    Model_AssetID = "bus_stop_lights";
                    ScaleX = 2f;
                    ScaleY = 2f;
                    ScaleZ = 2f;
                    VisibilityFlags.FlagValueByte = 0;
                    SolidityFlags.FlagValueByte = 0;
                    CollType.FlagValueByte = 0;
                    break;
                case AssetTemplate.Checkpoint_SIMP:
                    ScaleX = 0.75f;
                    ScaleY = 0.75f;
                    ScaleZ = 0.75f;
                    Model_AssetID = "checkpoint_bind";
                    Animation_AssetID = "CHECKPOINT_ANIMLIST_01";
                    break;
                case AssetTemplate.Checkpoint_SIMP_TSSM:
                    Model_AssetID = "checkpoint_bind";
                    Animation_AssetID = "CHECKPOINT_ANIM";
                    SolidityFlags.FlagValueByte = 0x22;
                    SimpFlags.FlagValueByte = 0x08;
                    break;
                case AssetTemplate.PressurePlateBase:
                    Model_AssetID = "plate_pressure_base";
                    break;
                case AssetTemplate.TaxiStand:
                    Model_AssetID = "taxi_stand";
                    break;
                case AssetTemplate.TexasHitch:
                case AssetTemplate.Swinger:
                    Model_AssetID = "trailer_hitch";
                    break;
                case AssetTemplate.ThrowFruit:
                    Model_AssetID = "fruit_throw.MINF";
                    break;
                case AssetTemplate.FreezyFruit:
                    Model_AssetID = "fruit_freezy_bind.MINF";
                    break;
                case AssetTemplate.ThrowFruitBase:
                    Model_AssetID = "fruit_throw_base";
                    CollType.FlagValueByte = 0;
                    break;
            }
        }

        public AssetSIMP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = entityHeaderEndPosition;

            AnimSpeed = reader.ReadSingle();
            InitialAnimState = reader.ReadInt32();
            CollType.FlagValueByte = reader.ReadByte();
            SimpFlags.FlagValueByte = reader.ReadByte();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeEntity(game, endianness));
            
            writer.Write(AnimSpeed);
            writer.Write(InitialAnimState);
            writer.Write(CollType.FlagValueByte);
            writer.Write(SimpFlags.FlagValueByte);
            writer.Write((short)0);

            writer.Write(SerializeLinks(endianness));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}