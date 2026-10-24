using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetUIM : BaseAsset
    {
        private const string categoryName = "User Interface Motion";
        public override string AssetInfo => $"{Commands.Length} commands";

        [Category(categoryName)]
        public AssetByte In { get; set; }
        [Category(categoryName)]
        public AssetSingle TotalTime { get; set; }
        [Category(categoryName)]
        public AssetSingle LoopTime { get; set; }
        [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor)), AssetPropertyCollectionOptions(allowCopy: false)]
        public UIMCommand[] Commands { get; set; }

        public AssetUIM(string assetName) : base(assetName, AssetType.UserInterfaceMotion, BaseAssetType.UIM)
        {
            Commands = [];
        }

        public AssetUIM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                byte cmdCount = reader.ReadByte();
                In = reader.ReadByte();
                reader.ReadInt16();
                var cmdSize = reader.ReadInt32();
                TotalTime = reader.ReadSingle();
                LoopTime = reader.ReadSingle();
                var cmdStart = reader.BaseStream.Position;

                Commands = new UIMCommand[cmdCount];
                for (int i = 0; i < cmdCount; i++)
                {
                    var cmdType = (UIMCommandType)reader.ReadInt32();
                    Commands[i] = cmdType switch
                    {
                        UIMCommandType.Move => new UIMCommand_Move(reader),
                        UIMCommandType.Scale => new UIMCommand_Scale(reader),
                        UIMCommandType.Rotate => new UIMCommand_Rotate(reader),
                        UIMCommandType.Opacity => new UIMCommand_Opacity(reader),
                        UIMCommandType.AbsoluteScale => new UIMCommand_AbsoluteScale(reader),
                        UIMCommandType.Brightness => new UIMCommand_Brightness(reader),
                        UIMCommandType.Color => new UIMCommand_Color(reader),
                        UIMCommandType.UVScroll => new UIMCommand_UVScroll(reader),
                        _ => throw new Exception($"Unsupported UIM command: {cmdType}"),
                    };
                }

                if (reader.BaseStream.Position - cmdStart != cmdSize)
                    throw new Exception("UIM loading was invalid");
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((byte)Commands.Length);
            writer.Write(In);
            writer.Write((short)0);
            var cmdSizePos = writer.BaseStream.Position;
            writer.Write(0);
            writer.Write(TotalTime);
            writer.Write(LoopTime);

            var cmdStart = writer.BaseStream.Position;
            foreach (var c in Commands)
                c.Serialize(writer);
            var cmdEnd = writer.BaseStream.Position;
            writer.BaseStream.Position = cmdSizePos;
            writer.Write((int)(cmdEnd - cmdStart));
            writer.BaseStream.Position = cmdEnd;
            SerializeLinks(writer);
        }

        public static UIMCommand CreateEntry(UIMCommandType cmdType)
        {
            return cmdType switch
            {
                UIMCommandType.Move => new UIMCommand_Move(),
                UIMCommandType.Scale => new UIMCommand_Scale(),
                UIMCommandType.Rotate => new UIMCommand_Rotate(),
                UIMCommandType.Opacity => new UIMCommand_Opacity(),
                UIMCommandType.AbsoluteScale => new UIMCommand_AbsoluteScale(),
                UIMCommandType.Brightness => new UIMCommand_Brightness(),
                UIMCommandType.Color => new UIMCommand_Color(),
                UIMCommandType.UVScroll => new UIMCommand_UVScroll(),
                _ => throw new Exception($"Unsupported UIM command: {cmdType}"),
            };
        }
    }
}