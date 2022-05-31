using HipHopFile;
using Newtonsoft.Json;
using System;

namespace IndustrialPark
{
    public enum LinkType
    {
        Normal,
        Timed,
        Progress
    }

    public class Link : GenericAssetDataContainer
    {
        public static int sizeOfStruct => 32;

        private Game game;

        public AssetID TargetAsset { get; set; }
        public ushort EventReceiveID;
        public ushort EventSendID;
        public AssetID Parameter1 { get; set; }
        public AssetID Parameter2 { get; set; }
        public AssetID Parameter3 { get; set; }
        public AssetID Parameter4 { get; set; }
        [JsonIgnore]
        public float FloatParameter1
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter1), 0);
            set => Parameter1 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        [JsonIgnore]
        public float FloatParameter2
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter2), 0);
            set => Parameter2 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        [JsonIgnore]
        public float FloatParameter3
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter3), 0);
            set => Parameter3 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        [JsonIgnore]
        public float FloatParameter4
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter4), 0);
            set => Parameter4 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        public AssetID ArgumentAsset { get; set; }
        public AssetID SourceCheckAsset { get; set; }

        public float Time { get; set; } // only for timed
        public int Flags { get; set; } // only for progress

        private Link()
        {
        }

        public Link(Game game)
        {
            this.game = game;
        }

        public Link(EndianBinaryReader reader, LinkType type, Game game)
        {
            this.game = game;

            if (type == LinkType.Normal)
            {
                EventReceiveID = reader.ReadUInt16();
                EventSendID = reader.ReadUInt16();
                TargetAsset = reader.ReadUInt32();
                Parameter1 = reader.ReadUInt32();
                Parameter2 = reader.ReadUInt32();
                Parameter3 = reader.ReadUInt32();
                Parameter4 = reader.ReadUInt32();
                ArgumentAsset = reader.ReadUInt32();
                SourceCheckAsset = reader.ReadUInt32();
            }
            else
            {
                Time = reader.ReadSingle();
                if (type == LinkType.Progress)
                    Flags = reader.ReadInt32();
                TargetAsset = reader.ReadUInt32();
                EventSendID = (ushort)reader.ReadUInt32();
                Parameter1 = reader.ReadUInt32();
                Parameter2 = reader.ReadUInt32();
                Parameter3 = reader.ReadUInt32();
                Parameter4 = reader.ReadUInt32();
                ArgumentAsset = reader.ReadUInt32();
            }
        }

        public byte[] Serialize(LinkType type, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                if (type == LinkType.Normal)
                {
                    writer.Write(EventReceiveID);
                    writer.Write(EventSendID);
                    writer.Write(TargetAsset);
                    writer.Write(Parameter1);
                    writer.Write(Parameter2);
                    writer.Write(Parameter3);
                    writer.Write(Parameter4);
                    writer.Write(ArgumentAsset);
                    writer.Write(SourceCheckAsset);
                }
                else
                {
                    writer.Write(Time);
                    if (type == LinkType.Progress)
                        writer.Write(Flags);
                    writer.Write(TargetAsset);
                    writer.Write((int)EventSendID);
                    writer.Write(Parameter1);
                    writer.Write(Parameter2);
                    writer.Write(Parameter3);
                    writer.Write(Parameter4);
                    writer.Write(ArgumentAsset);
                }

                return writer.ToArray();
            }
        }

        public override string ToString()
        {
            var recEvent = Enum.GetName(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB), EventReceiveID);
            var sndEvent = Enum.GetName(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB), EventSendID);

            string result = "";

            result += EventReceiveID != 0 ? recEvent.ToString() : Time.ToString();
            result += $" => {sndEvent} => ";
            result += HexUIntTypeConverter.Legacy ? TargetAsset.ToString("X8") : Program.MainForm.GetAssetNameFromID(TargetAsset);

            return result;
        }
    }
}