using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class FlagsField : AssetSpecific_Generic
    {
        private readonly DynamicTypeDescriptor dt;
        public abstract uint Flags { get; set; }

        public FlagsField(Asset asset, int flagLoc, DynamicTypeDescriptor dt)
            : base(asset, flagLoc)
        {
            this.dt = dt;
            dt.PropertyChanged += Dt_PropertyChanged;
        }

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

        public void SetFlag(uint offset, bool value)
        {
            Flags = value ? (Flags | Mask(offset)) : (Flags & InvMask(offset));
        }

        public bool GetFlag(uint offset)
        {
            return (Flags & Mask(offset)) != 0;
        }
    }
}