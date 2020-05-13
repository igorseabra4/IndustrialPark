using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEnemyFrogFish : DynaEnemySB
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x64;

        public DynaEnemyFrogFish(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (Player_AssetID == assetID)
                return true;
            if (Unknown58 == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Asset.Verify(Player_AssetID, ref result);
            Asset.Verify(Unknown58, ref result);
            Asset.Verify(Unknown5C, ref result);
            Asset.Verify(Unknown60, ref result);
        }
        public EnemyFrogFishType Type
        {
            get => (EnemyFrogFishType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        public AssetID Player_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
        public byte UnknownByte54
        {
            get => ReadByte(0x54);
            set => Write(0x54, value);
        }
        public byte UnknownByte55
        {
            get => ReadByte(0x55);
            set => Write(0x55, value);
        }
        public byte UnknownByte56
        {
            get => ReadByte(0x56);
            set => Write(0x56, value);
        }
        public byte UnknownByte57
        {
            get => ReadByte(0x57);
            set => Write(0x57, value);
        }
        public AssetID Unknown58
        {
            get => ReadUInt(0x58);
            set => Write(0x58, value);
        }
        public AssetID Unknown5C
        {
            get => ReadUInt(0x5C);
            set => Write(0x5C, value);
        }
        public AssetID Unknown60
        {
            get => ReadUInt(0x60);
            set => Write(0x60, value);
        }
    }
}