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
        public AssetSingle UnknownFloat1 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RingSoundGroup_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt2 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt3 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt4 { get; set; }
        [Category(dynaCategoryName)]
        public bool RingsAreVisible { get; set; }
        [Category(dynaCategoryName)]
        public AssetID[] Ring_AssetIDs { get; set; }

        public DynaGObjectRingControl(string assetName) : base(assetName, DynaType.game_object__RingControl, 3)
        {
            RingModel_AssetID = "test_ring";
            UnknownInt1 = 40;
            RingSoundGroup_AssetID = "RING_SGRP";
            RingsAreVisible = true;
            Ring_AssetIDs = new AssetID[0];
        }

        public DynaGObjectRingControl(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__RingControl, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            PlayerType = (DynaRingControlPlayerType)reader.ReadInt32();
            RingModel_AssetID = reader.ReadUInt32();
            UnknownFloat1 = reader.ReadSingle();
            int ringCount = reader.ReadInt32();
            UnknownInt1 = reader.ReadInt32();
            RingSoundGroup_AssetID = reader.ReadUInt32();
            UnknownInt2 = reader.ReadInt32();
            UnknownInt3 = reader.ReadInt32();
            UnknownInt4 = reader.ReadInt32();
            RingsAreVisible = reader.ReadInt32Bool();
            Ring_AssetIDs = new AssetID[ringCount];
            for (int i = 0; i < Ring_AssetIDs.Length; i++)
                Ring_AssetIDs[i] = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write((int)PlayerType);
            writer.Write(RingModel_AssetID);
            writer.Write(UnknownFloat1);
            writer.Write(Ring_AssetIDs.Length);
            writer.Write(UnknownInt1);
            writer.Write(RingSoundGroup_AssetID);
            writer.Write(UnknownInt2);
            writer.Write(UnknownInt3);
            writer.Write(UnknownInt4);
            writer.Write(RingsAreVisible ? 1 : 0);
            foreach (var i in Ring_AssetIDs)
                writer.Write(i);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (RingModel_AssetID == assetID)
                return true;
            if (RingSoundGroup_AssetID == assetID)
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

            if (RingSoundGroup_AssetID == 0)
                result.Add("Ring Control with no SGRP reference");
            Verify(RingSoundGroup_AssetID, ref result);

            foreach (var ring in Ring_AssetIDs)
                Verify(ring, ref result);
        }
    }
}