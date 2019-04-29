using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV_TSSM : AssetENV
    {
        public AssetENV_TSSM(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x5C;

        [Category("Environment (TSSM)")]
        public int UnknownInt44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category("Environment (TSSM)")]
        public int UnknownInt48
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        [Category("Environment (TSSM)")]
        public int UnknownInt4c
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Environment (TSSM)")]
        public int UnknownInt50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Environment (TSSM)")]
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category("Environment (TSSM)")]
        public int UnknownInt58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }
    }
}