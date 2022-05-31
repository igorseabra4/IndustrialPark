using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ePlayerType
    {
        eALWAYS,
        eCOUNTER,
        eCHECKER,
        eTESTER,
        ePLAYER_TYPE_SIZE
    }

    public class xOneLiner : GenericAssetDataContainer
    {
        public AssetID SoundGroupNameHash { get; set; }
        public AssetSingle SoundStartDelay { get; set; }
        public AssetSingle TimeSpan { get; set; }
        public AssetSingle TimeLastPlayed { get; set; }
        public uint NumPlays { get; set; }
        public AssetSingle DelayBetweenPlays { get; set; }
        public AssetSingle Probability { get; set; }
        public AssetSingle DefaultDuration { get; set; }
        public AssetSingle LastDuration { get; set; }
        public uint MaxPlays { get; set; }
        public int SoundGroupHandle { get; set; }
        public AssetID OLManager { get; set; }
        public short EventType { get; set; }
        public short PlaysInMusicChannel { get; set; }
        public AssetID pData { get; set; }
        public ePlayerType PlayerType { get; set; }
        public int TesterDataFirstParam { get; set; }
        public AssetSingle TesterDataSecondParam { get; set; }

        public xOneLiner() { }
        public xOneLiner(EndianBinaryReader reader)
        {
            SoundGroupNameHash = reader.ReadUInt32();
            SoundStartDelay = reader.ReadSingle();
            TimeSpan = reader.ReadSingle();
            TimeLastPlayed = reader.ReadSingle();
            NumPlays = reader.ReadUInt32();
            DelayBetweenPlays = reader.ReadSingle();
            Probability = reader.ReadSingle();
            DefaultDuration = reader.ReadSingle();
            LastDuration = reader.ReadSingle();
            MaxPlays = reader.ReadUInt32();
            SoundGroupHandle = reader.ReadInt32();
            OLManager = reader.ReadUInt32();
            EventType = reader.ReadInt16();
            PlaysInMusicChannel = reader.ReadInt16();
            pData = reader.ReadUInt32();
            PlayerType = (ePlayerType)reader.ReadInt32();
            TesterDataFirstParam = reader.ReadInt32();
            TesterDataSecondParam = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SoundGroupNameHash);
                writer.Write(SoundStartDelay);
                writer.Write(TimeSpan);
                writer.Write(TimeLastPlayed);
                writer.Write(NumPlays);
                writer.Write(DelayBetweenPlays);
                writer.Write(Probability);
                writer.Write(DefaultDuration);
                writer.Write(LastDuration);
                writer.Write(MaxPlays);
                writer.Write(SoundGroupHandle);
                writer.Write(OLManager);
                writer.Write(EventType);
                writer.Write(PlaysInMusicChannel);
                writer.Write(pData);
                writer.Write((int)PlayerType);
                writer.Write(TesterDataFirstParam);
                writer.Write(TesterDataSecondParam);

                return writer.ToArray();
            }
        }
    }

    public class AssetONEL : Asset
    {
        [Category("One Liner")]
        public xOneLiner[] OneLiners { get; set; }

        private const int unkByteCount = 0x43;

        public AssetONEL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                int num = reader.ReadInt32();
                OneLiners = new xOneLiner[num];
                for (int i = 0; i < OneLiners.Length; i++)
                    OneLiners[i] = new xOneLiner(reader);
                reader.ReadBytes(unkByteCount);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(OneLiners.Length);
                foreach (var state in OneLiners)
                    writer.Write(state.Serialize(game, endianness));
                writer.Write(new byte[unkByteCount]);

                return writer.ToArray();
            }
        }
    }
}