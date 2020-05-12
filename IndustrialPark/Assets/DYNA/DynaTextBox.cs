namespace IndustrialPark
{
    public class DynaDefault : DynaBase
    {
        private int structSize;
        public override int StructSize => structSize;

        public DynaDefault(AssetDYNA asset, int structSize) : base(asset)
        {
            this.structSize = structSize;
        }

        public AssetID[] UnknownData
        {
            get
            {
                AssetID[] data = new AssetID[StructSize / 4];
                for (int i = 0; i < StructSize / 4; i++)
                    data[i] = ReadUInt(i * 4);
                return data;
            }
            set
            {
                for (int i = 0; i < StructSize / 4; i++)
                    Write(i * 4, value[i]);
            }
        }

        public float[] UnknownData_Float
        {
            get
            {
                float[] data = new float[StructSize / 4];
                for (int i = 0; i < StructSize / 4; i++)
                    data[i] = ReadFloat(i * 4);
                return data;
            }
            set
            {
                for (int i = 0; i < StructSize / 4; i++)
                    Write(i * 4, value[i]);
            }
        }
    }
}