using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaLogicReference : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x4;

        public DynaLogicReference(AssetDYNA asset) : base(asset) { }
        
        public AssetID Unknown
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
    }
}