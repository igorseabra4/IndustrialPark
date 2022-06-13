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

        public AssetSIMP(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.SimpleObject, BaseAssetType.Static, position)
        {
            AnimSpeed = 1f;
            CollType.FlagValueByte = 2;

            switch (template)
            {
                case AssetTemplate.Bungee_Hook_SIMP:
                    Model = "bungee_hook";
                    CollType.FlagValueByte = 0;
                    break;
                case AssetTemplate.Bus_Stop:
                    Model = "bus_stop";
                    ScaleX = 2f;
                    ScaleY = 2f;
                    ScaleZ = 2f;
                    break;
                case AssetTemplate.Bus_Stop_BusSimp:
                    PositionX -= 3f;
                    VisibilityFlags.FlagValueByte = 0;
                    SolidityFlags.FlagValueByte = 0;
                    CollType.FlagValueByte = 0;
                    Model = "bus_bind";
                    Animation = "BUSSTOP_ANIMLIST_01";
                    break;
                case AssetTemplate.Bus_Stop_Lights:
                    Model = "bus_stop_lights";
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
                    Model = "checkpoint_bind";
                    Animation = "CHECKPOINT_ANIMLIST_01";
                    break;
                case AssetTemplate.Checkpoint_SIMP_TSSM:
                    Model = "checkpoint_bind";
                    Animation = "CHECKPOINT_ANIM";
                    SolidityFlags.FlagValueByte = 0x22;
                    SimpFlags.FlagValueByte = 0x08;
                    break;
                case AssetTemplate.Pressure_Plate_Base:
                    Model = "plate_pressure_base";
                    break;
                case AssetTemplate.Taxi_Stand:
                    Model = "taxi_stand";
                    break;
                case AssetTemplate.Texas_Hitch:
                case AssetTemplate.Swinger:
                    Model = "trailer_hitch";
                    break;
                case AssetTemplate.Throw_Fruit:
                    Model = "fruit_throw.MINF";
                    break;
                case AssetTemplate.Freezy_Fruit:
                    Model = "fruit_freezy_bind.MINF";
                    break;
                case AssetTemplate.Throw_Fruit_Base:
                    Model = "fruit_throw_base";
                    CollType.FlagValueByte = 0;
                    break;
                case AssetTemplate.Red_Button_Base:
                    Model = "rbsi0002";
                    Pitch -= 90f;
                    break;
                case AssetTemplate.Floor_Button_Base:
                    Model = "rbus0002";
                    break;
                case AssetTemplate.Red_Button_Smash_Base:
                    Model = "rbsl0002";
                    break;
                case AssetTemplate.Floor_Button_Smash_Base:
                    Model = "rbue0002";
                    break;
                case AssetTemplate.Cauldron:
                    Model = "rcue0001";
                    Surface = "SCARE";
                    break;
                case AssetTemplate.Flower:
                    Model = "dig_flower_bind";
                    Animation = "DIG_ANIM_TABLE";
                    SolidityFlags.FlagValueByte = 0x03;
                    CollType.FlagValueByte = 0;
                    break;
            }
        }

        public AssetSIMP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                AnimSpeed = reader.ReadSingle();
                InitialAnimState = reader.ReadInt32();
                CollType.FlagValueByte = reader.ReadByte();
                SimpFlags.FlagValueByte = reader.ReadByte();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));

                writer.Write(AnimSpeed);
                writer.Write(InitialAnimState);
                writer.Write(CollType.FlagValueByte);
                writer.Write(SimpFlags.FlagValueByte);
                writer.Write((short)0);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}