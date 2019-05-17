using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyCastNCrew : DynaPlaceableBase
    {
        public override string Note => "Version is always 1";

        public DynaEnemyCastNCrew() : base()
        {
        }

        public DynaEnemyCastNCrew(IEnumerable<byte> enumerable) : base (enumerable)
        {
            CreateTransformMatrix();
        }
        
        [Category("Enemy CastNCrew")]
        public EnemyCastNCrewType Type
        {
            get => (EnemyCastNCrewType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
    }
}