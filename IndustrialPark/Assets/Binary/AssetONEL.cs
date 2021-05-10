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
        public AssetID m_soundGroupNameHash { get; set; }
        public AssetSingle m_fSoundStartDelay { get; set; }
        public AssetSingle m_fTimeSpan { get; set; }
        public AssetSingle m_fTimeLastPlayed { get; set; }
        public uint m_uNumPlays { get; set; }
        public AssetSingle m_fDelayBetweenPlays { get; set; }
        public AssetSingle m_fProbability { get; set; }
        public AssetSingle m_fDefaultDuration { get; set; }
        public AssetSingle m_fLastDuration { get; set; }
        public uint m_uMaxPlays { get; set; }
        public int m_soundGroupHandle { get; set; }
        public int m_pOLManager { get; set; }
        public short m_eventType { get; set; }
        public short m_bPlaysInMusicChannel { get; set; }
        public AssetID m_pData { get; set; }
        public ePlayerType m_playerType { get; set; }
        public int m_testerDataFirstParam { get; set; }
        public AssetSingle m_testerDataSecondParam { get; set; }

        public xOneLiner() { }
        public xOneLiner(EndianBinaryReader reader)
        {
            m_soundGroupNameHash = reader.ReadUInt32();
            m_fSoundStartDelay = reader.ReadSingle();
            m_fTimeSpan = reader.ReadSingle();
            m_fTimeLastPlayed = reader.ReadSingle();
            m_uNumPlays = reader.ReadUInt32();
            m_fDelayBetweenPlays = reader.ReadSingle();
            m_fProbability = reader.ReadSingle();
            m_fDefaultDuration = reader.ReadSingle();
            m_fLastDuration = reader.ReadSingle();
            m_uMaxPlays = reader.ReadUInt32();
            m_soundGroupHandle = reader.ReadInt32();
            m_pOLManager = reader.ReadInt32();
            m_eventType = reader.ReadInt16();
            m_bPlaysInMusicChannel = reader.ReadInt16();
            m_pData = reader.ReadUInt32();
            m_playerType = (ePlayerType)reader.ReadInt32();
            m_testerDataFirstParam = reader.ReadInt32();
            m_testerDataSecondParam = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(m_soundGroupNameHash);
            writer.Write(m_fSoundStartDelay);
            writer.Write(m_fTimeSpan);
            writer.Write(m_fTimeLastPlayed);
            writer.Write(m_uNumPlays);
            writer.Write(m_fDelayBetweenPlays);
            writer.Write(m_fProbability);
            writer.Write(m_fDefaultDuration);
            writer.Write(m_fLastDuration);
            writer.Write(m_uMaxPlays);
            writer.Write(m_soundGroupHandle);
            writer.Write(m_pOLManager);
            writer.Write(m_eventType);
            writer.Write(m_bPlaysInMusicChannel);
            writer.Write(m_pData);
            writer.Write((int)m_playerType);
            writer.Write(m_testerDataFirstParam);
            writer.Write(m_testerDataSecondParam);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            m_soundGroupNameHash == assetID ||
            m_pData == assetID;
    }

    public class AssetONEL : Asset
    {
        [Category("One Liner")]
        public xOneLiner[] OneLiners { get; set; }

        private const int unkByteCount = 0x43;

        public AssetONEL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);

            int numStates = reader.ReadInt32();
            OneLiners = new xOneLiner[numStates];
            for (int i = 0; i < OneLiners.Length; i++)
                OneLiners[i] = new xOneLiner(reader);
            reader.ReadBytes(unkByteCount);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(OneLiners.Length);
            foreach (var state in OneLiners)
                writer.Write(state.Serialize(game, endianness));
            writer.Write(new byte[unkByteCount]);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var s in OneLiners)
                if (s.HasReference(assetID))
                    return true;

            return false;
        }
    }
}