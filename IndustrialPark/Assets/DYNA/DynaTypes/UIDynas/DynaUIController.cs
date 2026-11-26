using HipHopFile;

namespace IndustrialPark.Assets
{
    public class DynaUIController : DynaUI
    {
        private const string dynaCategoryName = "ui:controller";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 0;

        public DynaUIController(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.ui__controller, reader)
        {
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaUI(writer);
        }
    }
}