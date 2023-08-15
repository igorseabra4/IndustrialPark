﻿using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyNeptuneType : uint
    {
        neptune_bind = 0x8F85AF53
    }

    public class DynaEnemyNeptune : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Neptune";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => NeptuneType.ToString();

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyNeptuneType NeptuneType
        {
            get => (EnemyNeptuneType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Unknown50 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }

        public DynaEnemyNeptune(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Neptune, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Unknown50 = reader.ReadUInt32();
                Unknown54 = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(Unknown50);
            writer.Write(Unknown54);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}