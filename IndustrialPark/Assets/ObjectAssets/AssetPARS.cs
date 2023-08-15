using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPARS : BaseAsset
    {
        private const string categoryName = "Particle System";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        [Category(categoryName)]
        public int Type { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID ParticleSystem { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID Texture { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = ByteFlagsDescriptor();
        [Category(categoryName)]
        public byte Priority { get; set; }
        [Category(categoryName)]
        public short MaxParticles { get; set; }
        [Category(categoryName)]
        public byte RenderFunction { get; set; }
        [Category(categoryName)]
        public byte RenderSourceBlendMode { get; set; }
        [Category(categoryName)]
        public byte RenderDestBlendMode { get; set; }
        [Category(categoryName)]
        public byte CmdCount { get; set; }
        [Category(categoryName)]
        public AssetID[] Cmd { get; set; }
        [Category(categoryName)]
        public byte Unknown01 { get; set; }
        [Category(categoryName)]
        public byte Unknown02 { get; set; }
        [Category(categoryName)]
        public byte Unknown03 { get; set; }
        [Category(categoryName)]
        public byte Unknown04 { get; set; }

        public AssetPARS(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Type = reader.ReadInt32();
                ParticleSystem = reader.ReadUInt32();
                Texture = reader.ReadUInt32();
                Flags.FlagValueByte = reader.ReadByte();
                Priority = reader.ReadByte();
                MaxParticles = reader.ReadInt16();
                RenderFunction = reader.ReadByte();
                RenderSourceBlendMode = reader.ReadByte();
                RenderDestBlendMode = reader.ReadByte();
                CmdCount = reader.ReadByte();
                int cmdSize = reader.ReadInt32() / 4;
                Cmd = new AssetID[cmdSize];
                for (int i = 0; i < cmdSize; i++)
                    Cmd[i] = reader.ReadUInt32();
                if (game == Game.Incredibles)
                {
                    Unknown01 = reader.ReadByte();
                    Unknown02 = reader.ReadByte();
                    Unknown03 = reader.ReadByte();
                    Unknown04 = reader.ReadByte();
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            base.Serialize(writer);
            writer.Write(Type);
            writer.Write(ParticleSystem);
            writer.Write(Texture);
            writer.Write(Flags.FlagValueByte);
            writer.Write(Priority);
            writer.Write(MaxParticles);
            writer.Write(RenderFunction);
            writer.Write(RenderSourceBlendMode);
            writer.Write(RenderDestBlendMode);
            writer.Write(CmdCount);
            writer.Write(Cmd.Length * 4);
            foreach (var c in Cmd)
                writer.Write(c);

            if (game == Game.Incredibles)
            {
                writer.Write(Unknown01);
                writer.Write(Unknown02);
                writer.Write(Unknown03);
                writer.Write(Unknown04);
            }
            SerializeLinks(writer);

        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("Unknown01");
                dt.RemoveProperty("Unknown02");
                dt.RemoveProperty("Unknown03");
                dt.RemoveProperty("Unknown04");
            }
        }
    }
}