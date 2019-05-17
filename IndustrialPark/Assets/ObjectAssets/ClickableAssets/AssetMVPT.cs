using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMVPT : AssetMVPT_Scooby
    {
        protected override int EventStartOffset => 0x28 + 4 * SiblingAmount;

        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _zoneRadius = ReadFloat(0x20);
            _arenaRadius = ReadFloat(0x24);

            CreateTransformMatrix();
        }
        
        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Movement Angle - Enemy will rotate around the point this amount, -1 means disabled")]
        public float Delay
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        private float _zoneRadius;
        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Enemy will circle around the point in this distance, -1 means disabled")]
        public float ZoneRadius
        {
            get => _zoneRadius;
            set
            {
                _zoneRadius = value;
                Write(0x20, _zoneRadius);
                CreateTransformMatrix();
            }
        }

        [Category("Move Point"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Enemy will be able to see you from this radius (as in a sphere trigger), -1 means disabled")]
        public override float ArenaRadius
        {
            get => _arenaRadius;
            set
            {
                _arenaRadius = value;
                Write(0x24, _arenaRadius);
                CreateTransformMatrix();
            }
        }

        protected override int NextStartOffset => 0x28;
    }
}