﻿using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaNPCGroup : AssetDYNA
    {
        private const string dynaCategoryName = "npc:group";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public int MaxAttackers { get; set; }

        public DynaNPCGroup(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.npc__group, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;
                MaxAttackers = reader.ReadInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(MaxAttackers);

        }
    }
}