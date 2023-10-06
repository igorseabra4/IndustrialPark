using HipHopFile;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class AssetCSN : Asset
    {
        public AssetCSN(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
                uint numData = reader.ReadUInt32();
                uint numTime = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                uint headerSize = reader.ReadUInt32();
                uint visCount = reader.ReadUInt32();
                uint visSize = reader.ReadUInt32();
                uint breakCount = reader.ReadUInt32();
                reader.ReadUInt32();

                headerSize -= numData * 16 + numTime * 4 + visSize * 4 + breakCount * 8 + 4;

                if (headerSize == 0x50)
                {
                    _audioTrackCount = 1;
                    _audioTracks = new CutsceneAudioTrackV1[] { new CutsceneAudioTrackV1(reader) };
                }
                else
                {
                    _audioTrackCount = 32;
                    _audioTracks = new CutsceneAudioTrackV1[_audioTrackCount];

                    int soundStrLen;
                    if (headerSize == 0x830)
                        soundStrLen = 28;
                    else if (headerSize == 0x1030)
                        soundStrLen = 60;
                    else
                        throw new System.Exception($"Unknown asset format: header size {headerSize}");

                    for (int i = 0; i < _audioTracks.Length; i++)
                        _audioTracks[i] = new CutsceneAudioTrackV2(reader, soundStrLen);
                }

                CutsceneData = new CutsceneData[numData];
                for (int i = 0; i < CutsceneData.Length; i++)
                    CutsceneData[i] = new CutsceneData(reader, game);

                uint[] CutsceneTime = new uint[numTime];
                for (int i = 0; i < CutsceneTime.Length; i++)
                    CutsceneTime[i] = reader.ReadUInt32();

                reader.ReadUInt32();

                int visBytesCount = visCount != 0 ? (int)(visSize * 4 / visCount) : 0;
                Visibility = new byte[visCount][];
                for (int i = 0; i < Visibility.Length; i++)
                    Visibility[i] = reader.ReadBytes(visBytesCount);

                CutsceneBreaks = new CutsceneBreak[breakCount];
                for (int i = 0; i < CutsceneBreaks.Length; i++)
                    CutsceneBreaks[i] = new CutsceneBreak(reader);

                TimeChunks = new TimeChunk[numTime];
                for (int i = 0; i < TimeChunks.Length; i++)
                {
                    reader.BaseStream.Position = CutsceneTime[i];
                    TimeChunks[i] = new TimeChunk(reader);
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(new byte[12 * 4]);

            for (int i = 0; i < _audioTracks.Length; i++)
                _audioTracks[i].Serialize(writer);

            for (int i = 0; i < CutsceneData.Length; i++)
                CutsceneData[i].Serialize(writer);

            for (int i = 0; i < TimeChunks.Length; i++)
                writer.Write(0);

            writer.Write(0);

            for (int i = 0; i < Visibility.Length; i++)
                for (int j = 0; j < Visibility[i].Length; j++)
                    writer.Write(Visibility[i][j]);

            for (int i = 0; i < CutsceneBreaks.Length; i++)
                CutsceneBreaks[i].Serialize(writer);

            uint headerSize = (uint)writer.BaseStream.Position;

            PadTo0800(writer);

            uint maxModel = 0;

            for (int i = 0; i < CutsceneData.Length; i++)
            {
                CutsceneData[i].fileOffset = (uint)writer.BaseStream.Position;
                CutsceneData[i].Data.Serialize(writer);
                CutsceneData[i].chunkSize = (uint)(writer.BaseStream.Position - CutsceneData[i].fileOffset);
                PadTo0800(writer);
                uint size = (uint)(writer.BaseStream.Position - CutsceneData[i].fileOffset);
                if (CutsceneData[i].chunkSize == 0)
                    CutsceneData[i].fileOffset = 0;
                if (CutsceneData[i].DataType == CutsceneDataType.Model && size > maxModel)
                    maxModel = size;
            }

            uint maxBufEven = 0;
            uint maxBufOdd = 0;

            uint[] timeChunkOffsets = new uint[TimeChunks.Length];

            for (int i = 0; i < TimeChunks.Length; i++)
            {
                timeChunkOffsets[i] = (uint)writer.BaseStream.Position;
                TimeChunks[i].chunkIndex = i;
                TimeChunks[i].Serialize(writer);
                PadTo0800(writer);
                uint size = (uint)(writer.BaseStream.Position - timeChunkOffsets[i]);
                if (i % 2 == 0)
                {
                    if (size > maxBufEven)
                        maxBufEven = size;
                }
                else
                {
                    if (size > maxBufOdd)
                        maxBufOdd = size;
                }
            }

            writer.BaseStream.Position = 0;

            writer.WriteMagic("CTSN");
            writer.Write(assetID);
            writer.Write(CutsceneData.Length);
            writer.Write(TimeChunks.Length);
            writer.Write(maxModel);
            writer.Write(maxBufEven);
            writer.Write(maxBufOdd);
            writer.Write(headerSize);
            writer.Write(Visibility.Length);
            writer.Write(Visibility.Sum(v => v.Length) / 4);
            writer.Write(CutsceneBreaks.Length);
            writer.Write(0);

            for (int i = 0; i < _audioTracks.Length; i++)
                _audioTracks[i].Serialize(writer);

            for (int i = 0; i < CutsceneData.Length; i++)
                CutsceneData[i].Serialize(writer);

            for (int i = 0; i < timeChunkOffsets.Length; i++)
                writer.Write(timeChunkOffsets[i]);

            writer.Write((uint)writer.BaseStream.Length);
        }

        protected void PadTo0800(EndianBinaryWriter writer)
        {
            while (writer.BaseStream.Position % 0x800 != 0)
                writer.Write((byte)0);
        }

        private const string categoryName = "Cutscene";

        private int _audioTrackCount;
        private CutsceneAudioTrackV1[] _audioTracks;
        public CutsceneAudioTrackV1[] AudioTracks
        {
            get => _audioTracks;
            set
            {
                if (value.Length == _audioTrackCount)
                    _audioTracks = value;
                else
                    throw new System.Exception($"Must have {(_audioTrackCount == 1 ? "1 entry" : $"{_audioTrackCount} entries")}");
            }
        }

        public CutsceneData[] CutsceneData { get; set; }
        public TimeChunk[] TimeChunks { get; set; }
        public byte[][] Visibility { get; set; }
        public CutsceneBreak[] CutsceneBreaks { get; set; }

        public void ExtractToFolder(string folderName, Endianness endianness)
        {
            var list = new List<CutsceneData>();
            list.AddRange(CutsceneData);
            foreach (var tc in TimeChunks)
                list.AddRange(tc.CutsceneData);

            foreach (var cd in list)
            {
                byte[] bytes;
                using (var writer = new EndianBinaryWriter(endianness))
                {
                    if (cd.Data is CutsceneDataAnimation anim)
                        anim.Animation.Serialize(writer);
                    else if (cd.Data is CutsceneDataCameraScooby cameraScooby)
                        cameraScooby.Animation.Serialize(writer);
                    else
                        cd.Data.Serialize(writer);
                    bytes = writer.ToArray();
                }

                if (bytes.Length == 0)
                    continue;

                var path = Path.Combine(folderName, $"[{cd.DataType.ToString()}] {Program.MainForm.GetAssetNameFromID(cd.AssetID)}");
                string newpath;
                int k = 0;
                do
                {
                    newpath = path + "_" + k.ToString();
                    k++;
                }
                while (File.Exists(newpath));

                File.WriteAllBytes(newpath, bytes);
            }
        }
    }
}