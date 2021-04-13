using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class FlagsField
    {
        public uint Flags { get; set; }
        private readonly DynamicTypeDescriptor dt;

        public FlagsField(int bitSize, string[] flagNames, DynamicTypeDescriptor dt)
        {
            Flags = 0;
            this.dt = dt;

            for (uint i = 0; i < bitSize; i++)
                AddPropertyAt(i, flagNames);

            dt.PropertyChanged += Dt_PropertyChanged;
        }

        public override string ToString() => Flags.ToString($"X8");
        
        protected void AddPropertyAt(uint i, string[] flagNames)
        {
            string flagName;
            if (i < flagNames.Length && flagNames[i] != null)
                flagName = flagNames[i];
            else
                flagName = $"UnknownFlag_{i}";

            dt.AddProperty(typeof(bool), $"Flag_{i}", GetFlag(i), flagName, "", "", false, false, false);
        }

        private void Dt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            uint offset = Convert.ToUInt32(e.PropertyName.Split('_')[1]);
            SetFlag(offset, dt.GetPropertyValue(e.PropertyName, false));
        }

        public bool GetFlag(uint offset) => (Flags & Mask(offset)) != 0;
        public void SetFlag(uint offset, bool value) => Flags = value ? (Flags | Mask(offset)) : (Flags & InvMask(offset));
        
        protected static uint Mask(uint bit) => (uint)Math.Pow(2, bit);
        protected static uint InvMask(uint bit) => uint.MaxValue - Mask(bit);
    }
}