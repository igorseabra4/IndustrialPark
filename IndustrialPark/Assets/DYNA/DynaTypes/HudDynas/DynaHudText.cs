﻿using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudText : DynaHud
    {
        protected override short constVersion => 1;

        private const string dynaCategoryName = "hud:text";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(TextBox)} {HexUIntTypeConverter.StringFromAssetID(Text)}";

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID TextBox { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Text { get; set; }

        public DynaHudText(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__text, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudEnd;

                TextBox = reader.ReadUInt32();
                Text = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaHud(writer);
            writer.Write(TextBox);
            writer.Write(Text);
        }
    }
}