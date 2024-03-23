using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyIN2Driller : DynaEnemyIN2
    {
        public override string TypeString => "Enemy:IN2:Driller";
        protected override short constVersion => 1;

        public DynaEnemyIN2Driller(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__Driller, position) { }
        public DynaEnemyIN2Driller(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__IN2__Driller, game, endianness) { }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyIN2(writer);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
