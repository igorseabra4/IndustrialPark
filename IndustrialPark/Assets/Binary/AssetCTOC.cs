//using HipHopFile;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;

//namespace IndustrialPark
//{
//    public class xCutsceneBreak : GenericAssetDataContainer
//    {
//        public AssetSingle Time { get; set; }
//        public int Index { get; set; }

//        public xCutsceneBreak()
//        {
//        }

//        public xCutsceneBreak(EndianBinaryReader reader)
//        {
//            Time = reader.ReadSingle();
//            Index = reader.ReadInt32();
//        }

//        public override void Serialize(EndianBinaryWriter writer)
//        {
//            using (var writer = new EndianBinaryWriter(endianness))
//            {
//                writer.Write(Time);
//                writer.Write(Index);
//                return writer.ToArray();
//            }
//        }
//    }

//    public class xCutsceneData : GenericAssetDataContainer
//    {
//        public int DataType { get; set; }
//        public AssetID AssetID { get; set; }
//        public bool Internal { get; set; }
//        public byte[] Data;

//        public xCutsceneData()
//        {
//            Data = new byte[0];
//        }

//        public xCutsceneData(EndianBinaryReader reader)
//        {
//            DataType = reader.ReadInt32();
//            AssetID = reader.ReadUInt32();
//            var chunkSize = reader.ReadInt32();
//            var fileOffset = reader.ReadInt32();
//            if (chunkSize == 0 && fileOffset == 0)
//            {
//                Internal = true;
//                Data = new byte[0];
//            }
//            else
//            {
//                Internal = false;
//                var curr = reader.BaseStream.Position;
//                reader.BaseStream.Position = fileOffset;
//                Data = reader.ReadBytes(chunkSize);
//                reader.BaseStream.Position = curr;
//            }
//        }

//        public override void Serialize(EndianBinaryWriter writer)
//        {
//            using (var writer = new EndianBinaryWriter(endianness))
//            {
//                writer.Write(DataType);
//                writer.Write(AssetID);
//                writer.Write(0);
//                writer.Write(0);

//                return writer.ToArray();
//            }
//        }
//    }

//    public class AssetCSN : Asset
//    {
//        private const string categoryName = "Cutscene Table of Contents";

//        [Category(categoryName)]
//        public uint MaxModel { get; set; }
//        [Category(categoryName)]
//        public uint MaxBufEven { get; set; }
//        [Category(categoryName)]
//        public uint MaxBufOdd { get; set; }
//        [Category(categoryName)]
//        public string SoundLeft { get; set; }
//        [Category(categoryName)]
//        public string SoundRight { get; set; }

//        [Category(categoryName)]
//        public xCutsceneData[] Data { get; set; }
//        [Category(categoryName)]
//        public int[] Visibility { get; set; }
//        [Category(categoryName)]
//        public xCutsceneBreak[] BreakList { get; set; }

//        public AssetCSN(string assetName) : base(assetName, AssetType.Cutscene)
//        {
//            Data = new xCutsceneData[0];
//            Visibility = new int[0];
//            BreakList = new xCutsceneBreak[0];
//        }

//        public AssetCSN(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
//        {
//            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
//            {
//                reader.ReadInt32();
//                reader.ReadInt32();
//                var numData = reader.ReadInt32();
//                var numTime = reader.ReadInt32();
//                MaxModel = reader.ReadUInt32();
//                MaxBufEven = reader.ReadUInt32();
//                MaxBufOdd = reader.ReadUInt32();
//                var headerSize = reader.ReadUInt32();
//                var visCount = reader.ReadUInt32();
//                var visSize = reader.ReadUInt32();
//                var breakCount = reader.ReadUInt32();
//                reader.ReadUInt32();
//                SoundLeft = reader.ReadString(16).Trim('\0');
//                SoundRight = reader.ReadString(16).Trim('\0');

//                Data = new xCutsceneData[numData];
//                for (int i = 0; i < Data.Length; i++)
//                    Data[i] = new xCutsceneData(reader);

//                var timeChunkOffs = new int[numTime];
//                for (int i = 0; i < timeChunkOffs.Length; i++)
//                    timeChunkOffs[i] = reader.ReadInt32();

//                Visibility = new int[visCount];
//                for (int i = 0; i < Visibility.Length; i++)
//                    Visibility[i] = reader.ReadInt32();

//                BreakList = new xCutsceneBreak[numData];
//                for (int i = 0; i < BreakList.Length; i++)
//                    BreakList[i] = new xCutsceneBreak(reader);
//            }
//        }

//        public override void Serialize(EndianBinaryWriter writer)
//        {
//            using (var writer = new EndianBinaryWriter(endianness))
//            {
//                writer.WriteMagic("CTSN");
//                writer.Write(assetID);
//                writer.Write(Data.Length);

//                writer.Write(TimeChunkOffs);
//                writer.Write(Visibility);
//                writer.Write(BreakList.Serialize(game, endianness));

//                return writer.ToArray();
//            }
//        }
//    }
//}