using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV_TSSM : AssetENV
    {
        public AssetENV_TSSM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x5C;

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt48
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category("Environment"), Description("Movie only.")]
        public int UnknownInt58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }
    }
}