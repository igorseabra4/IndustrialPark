using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaGObjectBulletTime : AssetDYNA
    {
        private const string dynaCategoryName = "game object:bullet time";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetSingle Frequency { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Fadeout { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OriginalTimer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OriginalScale { get; set; }
        [Category(dynaCategoryName)]
        public bool Global { get; set; }

        public DynaGObjectBulletTime(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__bullet_time, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Frequency = reader.ReadSingle();
                Fadeout = reader.ReadSingle();
                OriginalTimer = reader.ReadSingle();
                OriginalScale = reader.ReadSingle();
                Global = reader.ReadByteBool();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Frequency);
            writer.Write(Fadeout);
            writer.Write(OriginalTimer);
            writer.Write(OriginalScale);
            writer.Write(Global);
            writer.Write(new byte[3]);
        }
    }
}