using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class FlagField
    {
        private uint _flags;
        public uint Flags
        {
            get => _flags;
            set
            {
                _flags = value;

                for (int i = 0; i < bitSize; i++)
                    dt.SetPropertyValue($"Flag_{i}", GetFlag(i));
            }
        }

        private readonly DynamicTypeDescriptor dt;
        private readonly int bitSize;

        public FlagField(int bitSize, string[] flagNames, DynamicTypeDescriptor dt)
        {
            this.bitSize = bitSize;
            this.dt = dt;
            _flags = 0;

            for (int i = 0; i < bitSize; i++)
                AddPropertyAt(i, flagNames);

            dt.PropertyChanged += Dt_PropertyChanged;
        }

        public override string ToString() => _flags.ToString($"X{bitSize / 4}");
        
        protected void AddPropertyAt(int i, string[] flagNames)
        {
            string flagName;
            if (i < flagNames.Length && flagNames[i] != null)
                flagName = flagNames[i];
            else
                flagName = $"UnknownFlag_{i}";

            dt.AddProperty(typeof(bool), $"Flag_{i}", GetFlag(i), flagName, "", "", true, false, false);
        }

        private void Dt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var offset = Convert.ToInt32(e.PropertyName.Split('_')[1]);
            SetFlag(offset, dt.GetPropertyValue(e.PropertyName, false));
        }

        public bool GetFlag(int offset) => (_flags & Mask(offset)) != 0;
        public void SetFlag(int offset, bool value) => _flags = value ? (_flags | Mask(offset)) : (_flags & InvMask(offset));
        
        protected static uint Mask(int bit) => (uint)(1 << bit);
        protected static uint InvMask(int bit) => uint.MaxValue ^ Mask(bit);
    }
}