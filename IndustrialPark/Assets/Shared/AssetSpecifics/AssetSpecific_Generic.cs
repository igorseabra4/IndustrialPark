using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSpecific_Generic : EndianConvertible
    {
        protected Asset asset;
        public AssetSpecific_Generic(Asset asset, int specificStart) : base(EndianConverter.PlatformEndianness(asset.platform))
        {
            this.asset = asset;
            this.specificStart = specificStart;
        }
        
        protected readonly int specificStart;

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

        protected byte ReadByte(int j) => asset.Data[j + specificStart];

        protected float ReadFloat(int j)
        {
            j += specificStart;
            if (asset.platform == Platform.GameCube)
                return BitConverter.ToSingle(new byte[] {
                    asset.Data[j + 3],
                    asset.Data[j + 2],
                    asset.Data[j + 1],
                    asset.Data[j] }, 0);

            return BitConverter.ToSingle(asset.Data, j);
        }

        protected short ReadShort(int j)
        {
            j += specificStart;
            if (asset.platform == Platform.GameCube)
                return BitConverter.ToInt16(new byte[] {
                    asset.Data[j + 1],
                    asset.Data[j] }, 0);

            return BitConverter.ToInt16(asset.Data, j);
        }

        protected int ReadInt(int j)
        {
            j += specificStart;
            if (asset.platform == Platform.GameCube)
                return BitConverter.ToInt32(new byte[] {
                    asset.Data[j + 3],
                    asset.Data[j + 2],
                    asset.Data[j + 1],
                    asset.Data[j] }, 0);

            return BitConverter.ToInt32(asset.Data, j);
        }

        protected uint ReadUInt(int j)
        {
            j += specificStart;
            if (asset.platform == Platform.GameCube)
                return BitConverter.ToUInt32(new byte[] {
                    asset.Data[j + 3],
                    asset.Data[j + 2],
                    asset.Data[j + 1],
                    asset.Data[j] }, 0);

            return BitConverter.ToUInt32(asset.Data, j);
        }

        protected void Write(int j, byte value) => asset.Data[j + specificStart] = value;

        protected void Write(int j, float value)
        {
            j += specificStart;
            var split = BitConverter.GetBytes(value);

            if (asset.platform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + i] = split[i];
        }

        protected void Write(int j, short value)
        {
            j += specificStart;
            byte[] split = BitConverter.GetBytes(value);

            if (asset.platform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                asset.Data[j + i] = split[i];
        }

        protected void Write(int j, int value)
        {
            j += specificStart;
            byte[] split = BitConverter.GetBytes(value);

            if (asset.platform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + i] = split[i];
        }

        protected void Write(int j, uint value)
        {
            j += specificStart;
            byte[] split = BitConverter.GetBytes(value);

            if (asset.platform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                asset.Data[j + i] = split[i];
        }
    }
}
