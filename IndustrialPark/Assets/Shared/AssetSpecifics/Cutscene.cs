using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class CutsceneAudioTrackV1 : GenericAssetDataContainer
    {
        public string SoundLeft { get; set; }
        public string SoundRight { get; set; }

        public CutsceneAudioTrackV1()
        {
            SoundLeft = "";
            SoundRight = "";
        }

        public CutsceneAudioTrackV1(EndianBinaryReader reader)
        {
            SoundLeft = new string(reader.ReadChars(16));
            SoundRight = new string(reader.ReadChars(16));
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.WritePaddedString(SoundLeft, 16);
            writer.WritePaddedString(SoundRight, 16);
        }
    }

    public class CutsceneAudioTrackV2 : CutsceneAudioTrackV1
    {
        private readonly int soundStrLen;

        public AssetID SoundLeftID { get; set; }
        public AssetID SoundRightID { get; set; }

        public CutsceneAudioTrackV2(EndianBinaryReader reader, int soundStrLen)
        {
            this.soundStrLen = soundStrLen;

            SoundLeftID = reader.ReadUInt32();
            SoundRightID = reader.ReadUInt32();
            SoundLeft = new string(reader.ReadChars(soundStrLen));
            SoundRight = new string(reader.ReadChars(soundStrLen));
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(SoundLeftID);
            writer.Write(SoundRightID);
            writer.WritePaddedString(SoundLeft, soundStrLen);
            writer.WritePaddedString(SoundRight, soundStrLen);
        }
    }

    public enum CutsceneDataType : uint
    {
        Null = 0,
        Model = 1,
        Animation = 2,
        Camera = 3,
        MorphTarget = 4,
        Sound = 5,
        JDeltaModel = 6,
        JDeltaAnim = 7,
    }

    public class CutsceneData : GenericAssetDataContainer
    {
        public bool inTimeChunk = false;

        private CutsceneDataType _dataType;

        public CutsceneDataType DataType
        {
            get => _dataType;
            //set
            //{
            //    _dataType = value;
            //}
        }

        public AssetID AssetID { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CutsceneDataData Data { get; set; }

        public CutsceneData(EndianBinaryReader reader, Game game, bool inTimeChunk = false)
        {
            _game = game;
            this.inTimeChunk = inTimeChunk;

            _dataType = (CutsceneDataType)reader.ReadUInt32();
            AssetID = reader.ReadUInt32();
            int chunkSize = reader.ReadInt32();
            uint fileOffset = reader.ReadUInt32();

            if (this.inTimeChunk)
            {
                reader.BaseStream.Position += fileOffset;
                Data = ReadCutsceneData(reader, chunkSize);
                return;
            }
            else if (chunkSize == 0 && fileOffset == 0)
            {
                Data = new CutsceneDataGeneric(reader, 0);
                return;
            }

            var pos = reader.BaseStream.Position;
            reader.BaseStream.Position = fileOffset;
            Data = ReadCutsceneData(reader, chunkSize);
            reader.BaseStream.Position = pos;
        }

        private CutsceneDataData ReadCutsceneData(EndianBinaryReader reader, int chunkSize)
        {
            switch (_dataType)
            {
                case CutsceneDataType.Animation:
                    return new CutsceneDataAnimation(reader, chunkSize);
                case CutsceneDataType.Camera:
                    if (game == Game.Scooby)
                        return new CutsceneDataCameraScooby(reader, chunkSize);
                    return new CutsceneDataCamera(reader);
            }
            return new CutsceneDataGeneric(reader, chunkSize);
        }

        public uint chunkSize;
        public uint fileOffset;

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((uint)DataType);
            writer.Write(AssetID);
            var pos = writer.BaseStream.Position;

            if (inTimeChunk)
            {
                writer.Write(0);
                writer.Write(0);
                var prev = writer.BaseStream.Position;
                Data.Serialize(writer);
                var pos2 = writer.BaseStream.Position;
                uint len = (uint)(pos2 - prev);
                writer.BaseStream.Position = pos;
                writer.Write(len);
                writer.BaseStream.Position = pos2;
            }
            else
            {
                writer.Write(chunkSize);
                writer.Write(fileOffset);
            }
        }
    }

    public abstract class CutsceneDataData : GenericAssetDataContainer
    {
        protected abstract byte[] GetData();
        protected abstract void SetData(byte[] data);

        [Editor(typeof(ByteArrayEditor), typeof(UITypeEditor))]
        public string ExportData
        {
            get
            {
                ByteArrayEditor.IsImport = false;
                ByteArrayEditor.Data = GetData();
                return "Click here ->";
            }
            set { }
        }

        [Editor(typeof(ByteArrayEditor), typeof(UITypeEditor))]
        public string ImportData
        {
            get
            {
                ByteArrayEditor.IsImport = true;
                return "Click here ->";
            }
            set
            {
                if (value == "imported_replace")
                    SetData(ByteArrayEditor.Data);
            }
        }
    }

    public class CutsceneDataGeneric : CutsceneDataData
    {
        public byte[] Data { get; set; }

        public CutsceneDataGeneric(EndianBinaryReader reader, int length)
        {
            Data = reader.ReadBytes(length);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Data);
        }

        protected override byte[] GetData() => Data;
        protected override void SetData(byte[] data) => Data = data;
    }

    public class CutsceneDataAnimation : CutsceneDataData
    {
        private Endianness endianness;

        public uint RootIndex { get; set; }
        public AssetSingle TranslateX { get; set; }
        public AssetSingle TranslateY { get; set; }
        public AssetSingle TranslateZ { get; set; }
        public AssetANIM Animation { get; set; }
        public byte[] UnknownBytes { get; set; }

        public CutsceneDataAnimation(EndianBinaryReader reader, int chunkSize)
        {
            endianness = reader.endianness;

            var start = reader.BaseStream.Position;
            RootIndex = reader.ReadUInt32();
            TranslateX = reader.ReadSingle();
            TranslateY = reader.ReadSingle();
            TranslateZ = reader.ReadSingle();
            Animation = new AssetANIM(reader);
            var unkBytes = new List<byte>();
            while (reader.BaseStream.Position < start + chunkSize)
                unkBytes.Add(reader.ReadByte());
            UnknownBytes = unkBytes.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(RootIndex);
            writer.Write(TranslateX);
            writer.Write(TranslateY);
            writer.Write(TranslateZ);
            Animation.Pad = false;
            Animation.Serialize(writer);
            foreach (byte b in UnknownBytes)
                writer.Write(b);
        }

        protected override byte[] GetData()
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                Animation.Serialize(writer);
                return writer.ToArray();
            }
        }

        protected override void SetData(byte[] data)
        {
            using (var reader = new EndianBinaryReader(data, endianness))
                Animation = new AssetANIM(reader);
        }
    }

    public class CutsceneDataCameraScooby : CutsceneDataData
    {
        private Endianness endianness;

        public uint Unknown { get; set; }
        public AssetSingle PositionX { get; set; }
        public AssetSingle PositionY { get; set; }
        public AssetSingle PositionZ { get; set; }
        public (float, float)[] Frames { get; set; }
        public AssetANIM Animation { get; set; }
        public byte[] UnknownBytes { get; set; }

        public CutsceneDataCameraScooby(EndianBinaryReader reader, int chunkSize)
        {
            endianness = reader.endianness;

            var start = reader.BaseStream.Position;
            Unknown = reader.ReadUInt32();
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
            uint numFrames = reader.ReadUInt32();
            Frames = new (float, float)[numFrames];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = (reader.ReadSingle(), reader.ReadSingle());
            Animation = new AssetANIM(reader);
            var unkBytes = new List<byte>();
            while (reader.BaseStream.Position < start + chunkSize)
                unkBytes.Add(reader.ReadByte());
            UnknownBytes = unkBytes.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(Frames.Length);
            for (int i = 0; i < Frames.Length; i++)
            {
                writer.Write(Frames[i].Item1);
                writer.Write(Frames[i].Item2);
            }
            Animation.Serialize(writer);
            foreach (byte b in UnknownBytes)
                writer.Write(b);
        }

        protected override byte[] GetData()
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                Animation.Serialize(writer);
                return writer.ToArray();
            }
        }

        protected override void SetData(byte[] data)
        {
            using (var reader = new EndianBinaryReader(data, endianness))
                Animation = new AssetANIM(reader);
        }
    }

    public class CutsceneDataCamera : CutsceneDataData
    {
        private Endianness endianness;

        public FlyFrame[] Frames { get; set; }

        public CutsceneDataCamera(EndianBinaryReader reader)
        {
            endianness = reader.endianness;

            uint numFrames = reader.ReadUInt32();
            Frames = new FlyFrame[numFrames];
            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = new FlyFrame(reader);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Frames.Length);
            foreach (var f in Frames)
                f.Serialize(writer);
        }

        protected override byte[] GetData()
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                foreach (var f in Frames)
                    f.Serialize(writer);
                return writer.ToArray();
            }
        }

        protected override void SetData(byte[] data)
        {
            using (var reader = new EndianBinaryReader(data, endianness))
            {
                var frames = new List<FlyFrame>();
                while (!reader.EndOfStream)
                    frames.Add(new FlyFrame(reader));
                Frames = frames.ToArray();
            }
        }
    }

    public class CutsceneBreak : GenericAssetDataContainer
    {
        public AssetSingle Time { get; set; }
        public int Index { get; set; }

        public CutsceneBreak(EndianBinaryReader reader)
        {
            Time = reader.ReadSingle();
            Index = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(Index);
        }
    }

    public class TimeChunk : GenericAssetDataContainer
    {
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public int chunkIndex;

        public CutsceneData[] CutsceneData { get; set; }

        public TimeChunk(EndianBinaryReader reader)
        {
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            uint numData = reader.ReadUInt32();
            reader.ReadUInt32();

            CutsceneData = new CutsceneData[numData];
            for (int i = 0; i < CutsceneData.Length; i++)
            {
                CutsceneData[i] = new CutsceneData(reader, game, true);
                while (reader.BaseStream.Position % 16 != 0)
                    reader.BaseStream.Position++;
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write(CutsceneData.Length);
            writer.Write(chunkIndex);

            for (int i = 0; i < CutsceneData.Length; i++)
            {
                CutsceneData[i].inTimeChunk = true;
                CutsceneData[i].Serialize(writer);
                while (writer.BaseStream.Position % 16 != 0)
                    writer.BaseStream.Position++;
            }
        }
    }
}
