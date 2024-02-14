using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyIN2Shooter : DynaEnemyIN2
    {
        private const string dynaCategoryName = "Enemy:IN2:Shooter";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public uint Unknown78 { get; set; }

        public DynaEnemyIN2Shooter(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__Shooter, position) { }
        public DynaEnemyIN2Shooter(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__IN2__Shooter, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = enemyIN2EndPosition;
                Unknown78 = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyIN2(writer);
            writer.Write(Unknown78);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
