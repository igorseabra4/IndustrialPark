using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    /// <summary>
    /// The supported datatypes for memory modifications.
    /// </summary>
    public enum MemoryHackDataType
    {
        UINT8,
        INT32,
        FLOAT32
    }

    /// <summary>
    /// Represents a memory modification used for the UpgradePowerUp exploit.
    /// </summary>
    public struct MemoryModification
    {
        public uint MemoryAddress { get; set;}
        public byte[] OldValue { get; set;}
        public byte[] NewValue  {get; set;}
        public MemoryHackDataType DataType { get; set;}

        public MemoryModification(uint memoryAddress, 
            byte[] oldValue, byte[] newValue, MemoryHackDataType dataType)
        {
            MemoryAddress = memoryAddress;
            OldValue = oldValue;
            NewValue = newValue;
            DataType = dataType;
        }
    }
}
