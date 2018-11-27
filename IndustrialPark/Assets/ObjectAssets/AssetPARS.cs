using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPARS : ObjectAsset
    {
        public AssetPARS(Section_AHDR AHDR) : base(AHDR) { }
        
        public override bool HasReference(uint assetID)
        {
            if (TextureAssetID == assetID)
                return true;
            
            return base.HasReference(assetID);
        }

        [Category("Particle System")]
        public int Unknown08
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Particle System")]
        public int Unknown0C
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Particle System")]
        public AssetID TextureAssetID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Particle System")]
        public byte UnknownByte14
        {
            get => ReadByte(0x14);
            set => Write(0x14, value);
        }

        [Category("Particle System")]
        public byte UnknownByte15
        {
            get => ReadByte(0x15);
            set => Write(0x15, value);
        }

        [Category("Particle System")]
        public byte UnknownByte16
        {
            get => ReadByte(0x16);
            set => Write(0x16, value);
        }

        [Category("Particle System")]
        public byte UnknownByte17
        {
            get => ReadByte(0x17);
            set => Write(0x17, value);
        }

        [Category("Particle System")]
        public byte UnknownByte18
        {
            get => ReadByte(0x18);
            set => Write(0x18, value);
        }

        [Category("Particle System")]
        public byte UnknownByte19
        {
            get => ReadByte(0x19);
            set => Write(0x19, value);
        }

        [Category("Particle System")]
        public byte UnknownByte1A
        {
            get => ReadByte(0x1A);
            set => Write(0x1A, value);
        }

        [Category("Particle System")]
        public byte UnknownByte1B
        {
            get => ReadByte(0x1B);
            set => Write(0x1B, value);
        }

        [Category("Particle System"), ReadOnly(true)]
        public int SizeOfStruct
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Particle System")]
        public AssetID[] Struct
        {
            get
            {
                List<AssetID> list = new List<AssetID>();

                for (int i = 0x20; i < 0x20 + SizeOfStruct; i += 4)
                    list.Add(ReadUInt(i));

                return list.ToArray();
            }
            set
            {
                List<byte> before = Data.Take(0x20).ToList();

                foreach (AssetID a in value)
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(a)));

                before.AddRange(Data.Skip(0x20 + SizeOfStruct));
                Data = before.ToArray();

                SizeOfStruct = value.Length * 4;
            }
        }
    }
}