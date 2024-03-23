using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaGObjectBulletMark : AssetDYNA
    {
        private const string dynaCategoryName = "game object:bullet mark";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(TextureID);
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID TextureID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Size { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Lifetime { get; set; }

        public DynaGObjectBulletMark(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__bullet_mark, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                TextureID = reader.ReadUInt32();
                Size = reader.ReadSingle();
                Lifetime = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(TextureID);
            writer.Write(Size);
            writer.Write(Lifetime);
        }
    }
}