using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyIN2Rat : DynaEnemyIN2
    {
        public override string TypeString => "Enemy:IN2:Rat";
        protected override short constVersion => 1;

        public DynaEnemyIN2Rat(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__Rat, position) { }
        public DynaEnemyIN2Rat(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__IN2__Rat, game, endianness) { }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyIN2(writer);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
