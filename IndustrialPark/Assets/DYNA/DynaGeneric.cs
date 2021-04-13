using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGeneric : AssetDYNA
    {
        private const string dynaCategoryName = "Generic Dynamic";

        [Category(dynaCategoryName)]
        public List<AssetID> UnknownData_Hex { get; set; }
        [Category(dynaCategoryName)]
        public List<float> UnknownData_Float 
        {
            get
            {
                var floats = new List<float>();
                foreach (var u in UnknownData_Hex)
                    floats.Add(BitConverter.ToSingle(BitConverter.GetBytes(u), 0));
                return floats;
            }
            set
            {
                var hex = new List<AssetID>();
                foreach (var f in value)
                    hex.Add(BitConverter.ToUInt32(BitConverter.GetBytes(f), 0));
                UnknownData_Hex = hex;
            }
        }

        public DynaGeneric(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownData_Hex = new List<AssetID>();
            var lsp = linkStartPosition(reader.BaseStream.Length, _links.Length);
            while (reader.BaseStream.Position < lsp)
                UnknownData_Hex.Add(reader.ReadUInt32());
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            foreach (var u in UnknownData_Hex)
                writer.Write(u);
            return writer.ToArray();
        }
    }
}