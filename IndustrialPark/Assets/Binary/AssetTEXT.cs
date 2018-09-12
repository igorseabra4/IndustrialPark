using System;
using System.Collections.Generic;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetTEXT : Asset
    {
        public AssetTEXT(Section_AHDR AHDR) : base(AHDR)
        {
        }
        
        public string Text
        {
            get => System.Text.Encoding.ASCII.GetString(Data, 4, ReadInt(0));

            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));
                foreach (char c in value)
                    newData.Add((byte)c);
                newData.Add(0);

                while (newData.Count % 4 != 0)
                    newData.Add(0);

                Data = newData.ToArray();
            }
        }
    }
}