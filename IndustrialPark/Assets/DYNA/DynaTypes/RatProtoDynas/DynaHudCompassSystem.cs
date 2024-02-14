using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaHudCompassSystem : AssetDYNA
    {
        private const string dynaCategoryName = "HUD Compass System";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(TextureID);
        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetSingle Radius { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CenterY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CenterX { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TextureID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxDistance { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MinBeadSize { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxBeadSize { get; set; }
        [Category(dynaCategoryName)]
        public uint SystemOpacity { get; set; }
        [Category(dynaCategoryName)]
        public uint BeadOpacity { get; set; }

        public DynaHudCompassSystem(string assetName) : base(assetName, DynaType.HUD_Compass_System) { }
        public DynaHudCompassSystem(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.HUD_Compass_System, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Radius = reader.ReadSingle();
                CenterY = reader.ReadSingle();
                CenterX = reader.ReadSingle();
                TextureID = reader.ReadUInt32();
                MaxDistance = reader.ReadSingle();
                MinBeadSize = reader.ReadSingle();
                MaxBeadSize = reader.ReadSingle();
                SystemOpacity = reader.ReadUInt32();
                BeadOpacity = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Radius);
            writer.Write(CenterY);
            writer.Write(CenterX);
            writer.Write(TextureID);
            writer.Write(MaxDistance);
            writer.Write(MinBeadSize);
            writer.Write(MaxBeadSize);
            writer.Write(SystemOpacity);
            writer.Write(BeadOpacity);
        }
    }
}