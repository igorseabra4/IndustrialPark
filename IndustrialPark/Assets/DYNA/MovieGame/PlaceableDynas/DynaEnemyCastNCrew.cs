using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaEnemyCastNCrew : DynaPlaceableBase
    {
        public override string Note => "Version is always 1";

        public DynaEnemyCastNCrew(Platform platform) : base(platform)
        {
        }

        public DynaEnemyCastNCrew(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
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