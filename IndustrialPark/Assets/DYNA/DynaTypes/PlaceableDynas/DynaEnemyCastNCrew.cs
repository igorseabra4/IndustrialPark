using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaEnemyCastNCrew : DynaEnemySB
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x50;

        public DynaEnemyCastNCrew(AssetDYNA asset) : base(asset) { }
        
        public EnemyCastNCrewType Type
        {
            get => (EnemyCastNCrewType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
    }
}