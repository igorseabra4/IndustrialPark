using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetFOG : BaseAsset
    {
        private const string categoryName = "Fog";

        [Category(categoryName)]
        public AssetColor FogColor { get; set; }
        private AssetColor _backgroundColor;
        [Category(categoryName)]
        public AssetColor BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                if (SharpRenderer.Fog == this)
                    SharpRenderer.backgroundColor = new SharpDX.Color4(value.ToVector4());
            }
        }
        [Category(categoryName)]
        public AssetSingle FogDensity { get; set; }
        [Category(categoryName)]
        public AssetSingle StartDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle EndDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle TransitionTime { get; set; }
        [Category(categoryName)]
        public byte FogType { get; set; }

        public AssetFOG(string assetName) : base(assetName, AssetType.Fog, BaseAssetType.Fog)
        {
            BackgroundColor = new AssetColor();
            FogColor = new AssetColor();
            FogDensity = 1;
            StartDistance = 100;
            EndDistance = 400;
        }

        public AssetFOG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                BackgroundColor = reader.ReadColor();
                FogColor = reader.ReadColor();
                FogDensity = reader.ReadSingle();
                StartDistance = reader.ReadSingle();
                EndDistance = reader.ReadSingle();
                TransitionTime = reader.ReadSingle();
                FogType = reader.ReadByte();
            }
            if (Links.Any(l => (EventBFBB)l.EventSendID == EventBFBB.On))
                ArchiveEditorFunctions.AddToRenderableFOGs(this);
        }

        [Browsable(false)]
        public static bool DontRender = false;

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(BackgroundColor);
            writer.Write(FogColor);
            writer.Write(FogDensity);
            writer.Write(StartDistance);
            writer.Write(EndDistance);
            writer.Write(TransitionTime);
            writer.Write(FogType);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);

            SerializeLinks(writer);
        }
    }
}