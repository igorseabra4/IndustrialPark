using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTaskBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:task_box";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetByte Persistent { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Loop { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Enable { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Retry { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TalkBox_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID NextTaskBox_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Begin_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Description_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Reminder_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Success_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Failure_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID End_TextID { get; set; }

        public DynaGObjectTaskBox(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__task_box, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            Persistent = reader.ReadByte();
            Loop = reader.ReadByte();
            Enable = reader.ReadByte();
            Retry = reader.ReadByte();
            TalkBox_AssetID = reader.ReadUInt32();
            NextTaskBox_AssetID = reader.ReadUInt32();
            Begin_TextID = reader.ReadUInt32();
            Description_TextID = reader.ReadUInt32();
            Reminder_TextID = reader.ReadUInt32();
            Success_TextID = reader.ReadUInt32();
            Failure_TextID = reader.ReadUInt32();
            End_TextID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Persistent);
            writer.Write(Loop);
            writer.Write(Enable);
            writer.Write(Retry);
            writer.Write(TalkBox_AssetID);
            writer.Write(NextTaskBox_AssetID);
            writer.Write(Begin_TextID);
            writer.Write(Description_TextID);
            writer.Write(Reminder_TextID);
            writer.Write(Success_TextID);
            writer.Write(Failure_TextID);
            writer.Write(End_TextID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (TalkBox_AssetID == assetID)
                return true;
            if (NextTaskBox_AssetID == assetID)
                return true;
            if (Begin_TextID == assetID)
                return true;
            if (Description_TextID == assetID)
                return true;
            if (Reminder_TextID == assetID)
                return true;
            if (Success_TextID == assetID)
                return true;
            if (Failure_TextID == assetID)
                return true;
            if (End_TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(TalkBox_AssetID, ref result);
            Verify(NextTaskBox_AssetID, ref result);
            Verify(Begin_TextID, ref result);
            Verify(Description_TextID, ref result);
            Verify(Reminder_TextID, ref result);
            Verify(Success_TextID, ref result);
            Verify(Failure_TextID, ref result);
            Verify(End_TextID, ref result);
        }
    }
}