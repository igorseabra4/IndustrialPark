using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaNPCGroup : AssetDYNA
    {
        private const string dynaCategoryName = "npc:group";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public int MaxAttackers { get; set; }

        public DynaNPCGroup(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.npc__group, reader)
        {
            MaxAttackers = reader.ReadInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(MaxAttackers);
        }
    }
}