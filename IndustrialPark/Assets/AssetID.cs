using System;
using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(AssetIDTypeConverter))]
    public class AssetID
    {
        public AssetID()
        {
            value = 0;
        }

        public AssetID(uint value)
        {
            this.value = value;
        }

        private uint value;
        public string Value {
            get
            {
                try
                {
                    return value.ToString("X8");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Unable to display asset ID");
                    return "";
                }
            }
            set
            {
                try
                {
                    this.value = Convert.ToUInt32(value, 16);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Incorrect format");
                }
            }
        }

        public override string ToString()
        {
            return value.ToString("X8");
        }

        public string ToString(string v)
        {
            return value.ToString(v);
        }

        public static implicit operator uint(AssetID value)
        {
            return value.value;
        }

        public static implicit operator AssetID(uint value)
        {
            return new AssetID(value);
        }
    }
}