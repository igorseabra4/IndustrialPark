using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaGObjectCameraRband : AssetDYNA
    {
        private const string dynaCategoryName = "game object:rband camera asset";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetSingle Spring_len { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Spring_const { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Damp_const { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Vertical_offset { get; set; }

        public DynaGObjectCameraRband(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__rband_camera_asset, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Spring_len = reader.ReadSingle();
                Spring_const = reader.ReadSingle();
                Damp_const = reader.ReadSingle();
                Vertical_offset = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Spring_len);
            writer.Write(Spring_const);
            writer.Write(Damp_const);
            writer.Write(Vertical_offset);
        }
    }
}