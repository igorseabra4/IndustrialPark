﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSpecific_Generic : EndianConvertibleWithData
    {
        public EndianConvertibleWithData asset;
        public AssetSpecific_Generic(EndianConvertibleWithData asset, int specificStart) : base(asset.game, asset.platform)
        {
            this.asset = asset;
            this.specificStart = specificStart;
        }
        
        protected readonly int specificStart;

        [Browsable(false)]
        public override byte[] Data
        { 
            get => asset.Data.Skip(specificStart).ToArray();
            set { }
        }

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

        protected override void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + specificStart + i] = split[i];
        }

        protected override void Write(int j, byte value)
        {
            asset.Data[j + specificStart] = value;
        }

        protected override void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                asset.Data[j + specificStart + i] = split[i];
        }

        protected override void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                asset.Data[j + specificStart + i] = split[i];
        }

        protected override void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + specificStart + i] = split[i];
        }

        protected override void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + specificStart + i] = split[i];
        }
    }
}
