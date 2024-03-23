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
    public class DynaEnemyRATSSwarmOwl : DynaEnemyRATSSwarm
    {
        public override string TypeString => "Enemy:RATS:Swarm:Owl";
        protected override short constVersion => 1;

        public DynaEnemyRATSSwarmOwl(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__RATS__Swarm__Owl, position) { }
        public DynaEnemyRATSSwarmOwl(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__RATS__Swarm__Owl, game, endianness) { }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}