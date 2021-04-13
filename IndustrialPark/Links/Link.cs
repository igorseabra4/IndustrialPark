using System;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class Link
    {
        public static readonly int sizeOfStruct = 32;

        private Game game;

        public AssetID TargetAssetID { get; set; }
        public ushort EventReceiveID;
        public ushort EventSendID;
        public AssetID Parameter1 { get; set; }
        public AssetID Parameter2 { get; set; }
        public AssetID Parameter3 { get; set; }
        public AssetID Parameter4 { get; set; }
        public float FloatParameter1 
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter1), 0);
            set => Parameter1 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        public float FloatParameter2
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter2), 0);
            set => Parameter2 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        public float FloatParameter3
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter3), 0);
            set => Parameter3 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        public float FloatParameter4
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(Parameter4), 0);
            set => Parameter4 = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        }
        public AssetID ArgumentAssetID { get; set; }
        public AssetID SourceCheckAssetID { get; set; }

        protected bool IsTimed = false;
        public float Time { get; set; }

        public Link(bool isTimed, Game game)
        {
            this.game = game;

            EventReceiveID = 0;
            EventSendID = 0;
            TargetAssetID = 0;
            Parameter1 = 0;
            Parameter2 = 0;
            Parameter3 = 0;
            Parameter4 = 0;
            ArgumentAssetID = 0;
            SourceCheckAssetID = 0;
            IsTimed = isTimed;
        }

        public Link(EndianBinaryReader reader, bool isTimed, Game game)
        {
            this.game = game;

            if (IsTimed = isTimed)
            {
                Time = reader.ReadSingle();
                TargetAssetID = reader.ReadUInt32();
                EventSendID = (ushort)reader.ReadUInt32();
                Parameter1 = reader.ReadUInt32();
                Parameter2 = reader.ReadUInt32();
                Parameter3 = reader.ReadUInt32();
                Parameter4 = reader.ReadUInt32();
                ArgumentAssetID = reader.ReadUInt32();
                SourceCheckAssetID = 0;
            }
            else
            {
                EventReceiveID = reader.ReadUInt16();
                EventSendID = reader.ReadUInt16();
                TargetAssetID = reader.ReadUInt32();
                Parameter1 = reader.ReadUInt32();
                Parameter2 = reader.ReadUInt32();
                Parameter3 = reader.ReadUInt32();
                Parameter4 = reader.ReadUInt32();
                ArgumentAssetID = reader.ReadUInt32();
                SourceCheckAssetID = reader.ReadUInt32();
            }
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            if (IsTimed)
            {
                writer.Write(Time);
                writer.Write(TargetAssetID);
                writer.Write((int)EventSendID);
                writer.Write(Parameter1);
                writer.Write(Parameter2);
                writer.Write(Parameter3);
                writer.Write(Parameter4);
                writer.Write(ArgumentAssetID);
            }
            else
            {
                writer.Write(EventReceiveID);
                writer.Write(EventSendID);
                writer.Write(TargetAssetID);
                writer.Write(Parameter1);
                writer.Write(Parameter2);
                writer.Write(Parameter3);
                writer.Write(Parameter4);
                writer.Write(ArgumentAssetID);
                writer.Write(SourceCheckAssetID);
            }

            return writer.ToArray();
        }

        public bool HasReference(uint assetID) => TargetAssetID == assetID || ArgumentAssetID == assetID || SourceCheckAssetID == assetID
            || Parameter1 == assetID || Parameter2 == assetID || Parameter3 == assetID || Parameter4 == assetID;

        public override string ToString()
        {
            var recEvent = Enum.GetName(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB), EventReceiveID);
            var sndEvent = Enum.GetName(game == Game.Incredibles ? typeof(EventTSSM) : typeof(EventBFBB), EventSendID);

            return (IsTimed ? Time.ToString() : recEvent) + $" => {sndEvent} => " + (AssetIDTypeConverter.Legacy ? TargetAssetID.ToString("X8") : Program.MainForm.GetAssetNameFromID(TargetAssetID));
        }
    }
}