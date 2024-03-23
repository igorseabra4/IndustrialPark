using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyIN2BossUnderminerDrill : DynaEnemyIN2
    {
        public override string TypeString => "Enemy:IN2:BossUnderminerDrill";
        protected override short constVersion => 1;

        public DynaEnemyIN2BossUnderminerDrill(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__BossUnderminerDrill, position) { }
        public DynaEnemyIN2BossUnderminerDrill(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__IN2__BossUnderminerDrill, game, endianness) { }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyIN2(writer);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
