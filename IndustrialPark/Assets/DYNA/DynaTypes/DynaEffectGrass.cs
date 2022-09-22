using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectGrass : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Grass";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetByte Orient { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask EffectGrassFlags { get; set; } = ShortFlagsDescriptor();
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID GrassMesh { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WidthScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HeightScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DensityScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WidthMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HightMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Animate { get; set; }

        public DynaEffectGrass(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__grass, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Orient = reader.ReadByte();
                reader.ReadByte();
                EffectGrassFlags.FlagValueShort = reader.ReadUInt16();
                GrassMesh = reader.ReadUInt32();
                Model = reader.ReadUInt32();
                WidthScale = reader.ReadSingle();
                HeightScale = reader.ReadSingle();
                DensityScale = reader.ReadSingle();
                WidthMin = reader.ReadSingle();
                HightMin = reader.ReadSingle();
                Animate = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Orient);
                writer.Write((byte)0);
                writer.Write(EffectGrassFlags.FlagValueShort);
                writer.Write(GrassMesh);
                writer.Write(Model);
                writer.Write(WidthScale);
                writer.Write(HeightScale);
                writer.Write(DensityScale);
                writer.Write(WidthMin);
                writer.Write(HightMin);
                writer.Write(Animate);

                
        }
    }
}