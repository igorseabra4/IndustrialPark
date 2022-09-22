using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        [Category(categoryName)]
        public UIMCommand[] Commands { get; set; }

        public AssetUIM(string assetName) : base(assetName, AssetType.UserInterfaceMotion, BaseAssetType.UIM)
        {
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
                    UIMCommand newCmd;
                    switch (cmdType)
                    {
                        case UIMCommandType.Move:
                            newCmd = new UIMCommand_Move(reader);
                            break;
                        case UIMCommandType.Scale:
                            newCmd = new UIMCommand_Scale(reader);
                            break;
                        case UIMCommandType.Rotate:
                            newCmd = new UIMCommand_Rotate(reader);
                            break;
                        case UIMCommandType.Opacity:
                            newCmd = new UIMCommand_Opacity(reader);
                            break;
                        case UIMCommandType.AbsoluteScale:
                            newCmd = new UIMCommand_AbsoluteScale(reader);
                            break;
                        case UIMCommandType.Brightness:
                            newCmd = new UIMCommand_Brightness(reader);
                            break;
                        case UIMCommandType.Color:
                            newCmd = new UIMCommand_Color(reader);
                            break;
                        case UIMCommandType.UVScroll:
                            newCmd = new UIMCommand_UVScroll(reader);
                            break;
                        default:
                            throw new Exception($"Unsupported UIM command: {cmdType}");
                    }
                    Commands[i] = newCmd;
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

        public void AddEntry(UIMCommandType cmdType)
        {
            UIMCommand newCmd;
            switch (cmdType)
            {
                case UIMCommandType.Move:
                    newCmd = new UIMCommand_Move();
                    break;
                case UIMCommandType.Scale:
                    newCmd = new UIMCommand_Scale();
                    break;
                case UIMCommandType.Rotate:
                    newCmd = new UIMCommand_Rotate();
                    break;
                case UIMCommandType.Opacity:
                    newCmd = new UIMCommand_Opacity();
                    break;
                case UIMCommandType.AbsoluteScale:
                    newCmd = new UIMCommand_AbsoluteScale();
                    break;
                case UIMCommandType.Brightness:
                    newCmd = new UIMCommand_Brightness();
                    break;
                case UIMCommandType.Color:
                    newCmd = new UIMCommand_Color();
                    break;
                case UIMCommandType.UVScroll:
                    newCmd = new UIMCommand_UVScroll();
                    break;
                default:
                    throw new Exception($"Unsupported UIM command: {cmdType}");
            }
            var commands = Commands.ToList();
            commands.Add(newCmd);
            Commands = commands.ToArray();
        }
    }
}