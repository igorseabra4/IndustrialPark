using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPARS : BaseAsset
    {
        private const string categoryName = "Particle System";

        [Category(categoryName)]
        public int PARS_Type { get; set; }
        [Category(categoryName)]
        public AssetID PARS_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID TextureAssetID { get; set; }
        [Category(categoryName)]
        public FlagBitmask ParsFlags { get; set; } = ByteFlagsDescriptor();
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

                PARS_Type = reader.ReadInt32();
                PARS_AssetID = reader.ReadUInt32();
                TextureAssetID = reader.ReadUInt32();
                ParsFlags.FlagValueByte = reader.ReadByte();
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(PARS_Type);
                writer.Write(PARS_AssetID);
                writer.Write(TextureAssetID);
                writer.Write(ParsFlags.FlagValueByte);
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
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => TextureAssetID == assetID || PARS_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TextureAssetID, ref result);
            Verify(PARS_AssetID, ref result);
        }
    }
}