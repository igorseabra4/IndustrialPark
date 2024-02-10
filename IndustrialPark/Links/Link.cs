using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public enum LinkType
    {
        Normal,
        Timed,
        TimedRotu,
        Progress
    }

    public class Link : GenericAssetDataContainer
    {
        public static int sizeOfStruct => 32;

        [ValidReferenceRequired]
        public AssetID TargetAsset { get; set; }
        public ushort EventReceiveID;
        public ushort EventSendID;
        [IgnoreVerification]
        public AssetID Parameter1 { get; set; }
        [IgnoreVerification]
        public AssetID Parameter2 { get; set; }
        [IgnoreVerification]
        public AssetID Parameter3 { get; set; }
        [IgnoreVerification]
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

        public float Time { get; set; } // only for progress, timed and timed rotu
        public int Flags { get; set; } // only for progress
        public bool Enabled { get; set; } // only for timed rotu

        [JsonConstructor]
        private Link()
        {
        }

        public Link(Game game)
        {
            _game = game;
        }

        public Link(EndianBinaryReader reader, LinkType type, Game game)
        {
            _game = game;

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
            if (type == LinkType.TimedRotu)
            {
                Enabled = reader.ReadBoolean();
                reader.ReadBytes(3);
            }
        }

        public override void Serialize(EndianBinaryWriter writer) { }

        public void Serialize(LinkType type, EndianBinaryWriter writer)
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
            if (type == LinkType.TimedRotu)
            {
                writer.Write(Enabled);
                writer.Write(new byte[3]);
            }
            
        }

        public override string ToString()
        {
            string recEvent;
            string sndEvent;
            switch (game)
            {
                case Game.Scooby:
                    recEvent = ((EventScooby)EventReceiveID).ToString();
                    sndEvent = ((EventScooby)EventSendID).ToString();
                    break;
                case Game.BFBB:
                    recEvent = ((EventBFBB)EventReceiveID).ToString();
                    sndEvent = ((EventBFBB)EventSendID).ToString();
                    break;
                case Game.Incredibles:
                    recEvent = ((EventTSSM)EventReceiveID).ToString();
                    sndEvent = ((EventTSSM)EventSendID).ToString();
                    break;
                case Game.ROTU:
                    recEvent = ((EventROTU)EventReceiveID).ToString();
                    sndEvent = ((EventROTU)EventSendID).ToString();
                    break;
                case Game.RatProto:
                    recEvent = ((EventRatProto)EventReceiveID).ToString();
                    sndEvent = ((EventRatProto)EventSendID).ToString();
                    break;
                default:
                    recEvent = "?";
                    sndEvent = "?";
                    break;
            }

            string assetName = HexUIntTypeConverter.StringFromAssetID(TargetAsset);
            if (LinkListEditor.LinkType == LinkType.Normal)
                return $"{recEvent} => {sndEvent} => {assetName}";
            else
                return $"{(LinkListEditor.LinkType == LinkType.TimedRotu && Enabled == false ? "[DISABLED] " : "")}{Time.ToString()} => {sndEvent} => {assetName}";
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            var eventCount = Enum.GetValues(game == Game.Scooby ? typeof(EventScooby) : game == Game.BFBB ? typeof(EventBFBB) : game == Game.Incredibles ? typeof(EventTSSM) : game == Game.ROTU ? typeof(EventROTU) : typeof(EventRatProto)).Length;

            if (EventReceiveID == 0 || EventReceiveID > eventCount)
                result.Add("Link receives event of unknown type: " + EventReceiveID.ToString());
            if (EventSendID == 0 || EventSendID > eventCount)
                result.Add("Link sends event of unknown type: " + EventSendID.ToString());
        }
    }
}