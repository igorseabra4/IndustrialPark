using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaVentType : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaVentType() : base() { }

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

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Constant_PARE, ref result);
            Asset.Verify(Constant_SGRP, ref result);
            Asset.Verify(Warning_PARE, ref result);
            Asset.Verify(Warning_SGRP, ref result);
            Asset.Verify(Emit_PARE, ref result);
            Asset.Verify(Emit_SGRP, ref result);
        }

        public DynaVentType(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Constant_PARE = Switch(BitConverter.ToUInt32(Data, 0x00));
            Constant_SGRP = Switch(BitConverter.ToUInt32(Data, 0x04));
            Warning_PARE = Switch(BitConverter.ToUInt32(Data, 0x08));
            Warning_SGRP = Switch(BitConverter.ToUInt32(Data, 0x0C));
            Emit_PARE = Switch(BitConverter.ToUInt32(Data, 0x10));
            Emit_SGRP = Switch(BitConverter.ToUInt32(Data, 0x14));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Constant_PARE)));
            list.AddRange(BitConverter.GetBytes(Switch(Constant_SGRP)));
            list.AddRange(BitConverter.GetBytes(Switch(Warning_PARE)));
            list.AddRange(BitConverter.GetBytes(Switch(Warning_SGRP)));
            list.AddRange(BitConverter.GetBytes(Switch(Emit_PARE)));
            list.AddRange(BitConverter.GetBytes(Switch(Emit_SGRP)));
            return list.ToArray();
        }

        [Category("Vent Type")]
        public AssetID Constant_PARE { get; set; }
        [Category("Vent Type")]
        public AssetID Constant_SGRP { get; set; }
        [Category("Vent Type")]
        public AssetID Warning_PARE { get; set; }
        [Category("Vent Type")]
        public AssetID Warning_SGRP { get; set; }
        [Category("Vent Type")]
        public AssetID Emit_PARE { get; set; }
        [Category("Vent Type")]
        public AssetID Emit_SGRP { get; set; }
    }
}