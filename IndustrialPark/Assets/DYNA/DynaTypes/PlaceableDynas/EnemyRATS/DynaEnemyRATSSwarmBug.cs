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
    public class DynaEnemyRATSSwarmBug : DynaEnemyRATSSwarm
    {
        public override string TypeString => "Enemy:RATS:Swarm:Bug";
        protected override short constVersion => 1;

        public DynaEnemyRATSSwarmBug(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__RATS__Swarm__Bug, position) { }
        public DynaEnemyRATSSwarmBug(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__RATS__Swarm__Bug, game, endianness) { }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}