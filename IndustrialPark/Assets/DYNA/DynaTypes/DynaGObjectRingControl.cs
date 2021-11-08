using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum DynaRingControlPlayerType
    {
        Drive = 0,
        SpongebobPatrick = 1,
        Spongeball = 2,
        Unknown3 = 3,
        Slide = 4,
        SonicWaveGuitar = 5
    }

    public class DynaGObjectRingControl : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:RingControl";

        protected override short constVersion => 3;
        
        public static uint RingModelAssetID = 0;

        [Category(dynaCategoryName)]
        public DynaRingControlPlayerType PlayerType { get; set; }
        private uint _ringModelAssetID;
        [Category(dynaCategoryName)]
        public AssetID RingModel_AssetID
        {
            get => _ringModelAssetID;
            set
            {
                _ringModelAssetID = value;
                RingModelAssetID = value;
            }
        }
        [Category(dynaCategoryName)]
        public AssetSingle DefaultWarningTime { get; set; }
        [Category(dynaCategoryName)]
        public int UnusedOffset { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RingSoundGroup_AssetID1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RingSoundGroup_AssetID2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RingSoundGroup_AssetID3 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RingSoundGroup_AssetID4 { get; set; }
        [Category(dynaCategoryName)]
        public int NumNextRingsToShow { get; set; }
        [Category(dynaCategoryName)]
        public AssetID[] Ring_AssetIDs { get; set; }

        public DynaGObjectRingControl(string assetName) : base(assetName, DynaType.game_object__RingControl, 3)
        {
            RingModel_AssetID = "test_ring";
            UnusedOffset = 40;
            RingSoundGroup_AssetID1 = "RING_SGRP";
            RingSoundGroup_AssetID2 = 0;
            RingSoundGroup_AssetID3 = 0;
            RingSoundGroup_AssetID4 = 0;
            NumNextRingsToShow = 1;
            Ring_AssetIDs = new AssetID[0];
        }

        public DynaGObjectRingControl(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__RingControl, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                PlayerType = (DynaRingControlPlayerType)reader.ReadInt32();
                RingModel_AssetID = reader.ReadUInt32();
                DefaultWarningTime = reader.ReadSingle();
                int ringCount = reader.ReadInt32();
                UnusedOffset = reader.ReadInt32();
                RingSoundGroup_AssetID1 = reader.ReadUInt32();
                RingSoundGroup_AssetID2 = reader.ReadUInt32();
                RingSoundGroup_AssetID3 = reader.ReadUInt32();
                RingSoundGroup_AssetID4 = reader.ReadUInt32();
                NumNextRingsToShow = reader.ReadInt32();
                Ring_AssetIDs = new AssetID[ringCount];
                for (int i = 0; i < Ring_AssetIDs.Length; i++)
                    Ring_AssetIDs[i] = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write((int)PlayerType);
                writer.Write(RingModel_AssetID);
                writer.Write(DefaultWarningTime);
                writer.Write(Ring_AssetIDs.Length);
                writer.Write(UnusedOffset);
                writer.Write(RingSoundGroup_AssetID1);
                writer.Write(RingSoundGroup_AssetID2);
                writer.Write(RingSoundGroup_AssetID3);
                writer.Write(RingSoundGroup_AssetID4);
                writer.Write(NumNextRingsToShow);
                foreach (var i in Ring_AssetIDs)
                    writer.Write(i);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (RingModel_AssetID == assetID)
                return true;
            if (RingSoundGroup_AssetID1 == assetID)
                return true;
            if (RingSoundGroup_AssetID2 == assetID)
                return true;
            if (RingSoundGroup_AssetID3 == assetID)
                return true;
            if (RingSoundGroup_AssetID4 == assetID)
                return true;
            foreach (var ring in Ring_AssetIDs)
                if (ring == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (RingModel_AssetID == 0)
                result.Add("Ring Control with no Ring Model reference");
            Verify(RingModel_AssetID, ref result);

            if (RingSoundGroup_AssetID1 == 0)
                result.Add("Ring Control with no SGRP reference");
            Verify(RingSoundGroup_AssetID1, ref result);

            foreach (var ring in Ring_AssetIDs)
                Verify(ring, ref result);

            base.Verify(ref result);
        }
    }
}