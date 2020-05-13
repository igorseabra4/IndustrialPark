using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class Asset : EndianConvertibleWithData
    {
        [Browsable(false)]
        public int Offset => game == Game.BFBB ? 0x00 : -0x04;
        public static int DataSizeOffset(Game game) => game == Game.BFBB ? 0x00 : -0x04;

        public Section_AHDR AHDR;
        public bool isSelected;
        public bool isInvisible = false;

        public Asset(Section_AHDR AHDR, Game game, Platform platform) : base(game, platform)
        {
            this.AHDR = AHDR;
        }
        
        [Category("Asset")]
        public override byte[] Data
        {
            get => AHDR.data;
            set => AHDR.data = value; 
        }

        public override string ToString() =>  $"{AHDR.ADBG.assetName} [{AHDR.assetID.ToString("X8")}]";

        public override int GetHashCode()
        {
            return AHDR.assetID.GetHashCode();
        }

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

        public static void Verify(uint assetID, ref List<string> result)
        {
            if (assetID != 0 && !Program.MainForm.AssetExists(assetID))
                result.Add("Referenced asset 0x" + assetID.ToString("X8") + " was not found in any open archive.");
        }

        public virtual void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
        }
    }
}