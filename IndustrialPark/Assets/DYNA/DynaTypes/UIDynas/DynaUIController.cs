﻿using HipHopFile;

namespace IndustrialPark
{
    public class DynaUIController : DynaUI
    {
        private const string dynaCategoryName = "ui:controller";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 0;

        public DynaUIController(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__controller, game, endianness)
        {
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaUI(endianness));
                return writer.ToArray();
            }
        }
    }
}