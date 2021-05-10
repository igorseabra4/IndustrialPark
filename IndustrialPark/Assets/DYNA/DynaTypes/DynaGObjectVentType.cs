using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectVentType : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:VentType";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Constant_PARE { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Constant_SGRP { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Warning_PARE { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Warning_SGRP { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Emit_PARE { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Emit_SGRP { get; set; }

        public DynaGObjectVentType(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__VentType, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            Constant_PARE = reader.ReadUInt32();
            Constant_SGRP = reader.ReadUInt32();
            Warning_PARE = reader.ReadUInt32();
            Warning_SGRP = reader.ReadUInt32();
            Emit_PARE = reader.ReadUInt32();
            Emit_SGRP = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(Constant_PARE);
            writer.Write(Constant_SGRP);
            writer.Write(Warning_PARE);
            writer.Write(Warning_SGRP);
            writer.Write(Emit_PARE);
            writer.Write(Emit_SGRP);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (Constant_PARE == assetID)
                return true;
            if (Constant_SGRP == assetID)
                return true;
            if (Warning_PARE == assetID)
                return true;
            if (Warning_SGRP == assetID)
                return true;
            if (Emit_PARE == assetID)
                return true;
            if (Emit_SGRP == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Constant_PARE, ref result);
            Verify(Constant_SGRP, ref result);
            Verify(Warning_PARE, ref result);
            Verify(Warning_SGRP, ref result);
            Verify(Emit_PARE, ref result);
            Verify(Emit_SGRP, ref result);
        }
    }
}