using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyRATSLeftArm : DynaEnemyRATS
    {
        public override string TypeString => "Enemy:RATS:LeftArm";
        protected override short constVersion => 1;

        public DynaEnemyRATSLeftArm(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__RATS__LeftArm, game, endianness) { }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyRATS(writer);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}