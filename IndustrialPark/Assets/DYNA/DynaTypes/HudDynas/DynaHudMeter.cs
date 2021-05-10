using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class DynaHudMeter : DynaHud
    {
        private const string dynaCategoryName = "hud:meter";

        [Category(dynaCategoryName)]
        public AssetSingle StartValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MinValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle IncrementTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DecrementTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartIncrement_SoundAssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Increment_SoundAssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartDecrement_SoundAssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Decrement_SoundAssetID { get; set; }

        protected int dynaHudMeterEnd => dynaHudEnd + 36;

        public DynaHudMeter(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaHudEnd;

            StartValue = reader.ReadSingle();
            MinValue = reader.ReadSingle();
            MaxValue = reader.ReadSingle();
            IncrementTime = reader.ReadSingle();
            DecrementTime = reader.ReadSingle();
            StartIncrement_SoundAssetID = reader.ReadUInt32();
            Increment_SoundAssetID = reader.ReadUInt32();
            StartDecrement_SoundAssetID = reader.ReadUInt32();
            Decrement_SoundAssetID = reader.ReadUInt32();
        }

        protected byte[] SerializeDynaHudMeter(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(SerializeDynaHud(endianness));
            writer.Write(StartValue);
            writer.Write(MinValue);
            writer.Write(MaxValue);
            writer.Write(IncrementTime);
            writer.Write(DecrementTime);
            writer.Write(StartIncrement_SoundAssetID);
            writer.Write(Increment_SoundAssetID);
            writer.Write(StartDecrement_SoundAssetID);
            writer.Write(Decrement_SoundAssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (StartIncrement_SoundAssetID == assetID)
                return true;
            if (Increment_SoundAssetID == assetID)
                return true;
            if (StartDecrement_SoundAssetID == assetID)
                return true;
            if (Decrement_SoundAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(StartIncrement_SoundAssetID, ref result);
            Verify(Increment_SoundAssetID, ref result);
            Verify(StartDecrement_SoundAssetID, ref result);
            Verify(Decrement_SoundAssetID, ref result);
        }
    }
}