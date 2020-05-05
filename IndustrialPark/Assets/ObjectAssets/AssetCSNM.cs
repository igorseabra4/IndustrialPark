using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCSNM : BaseAsset
    {
        public AssetCSNM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0xC8 + (game == Game.Incredibles ? 4 : 0);

        public override bool HasReference(uint assetID) => CSN_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (CSN_AssetID == 0)
                result.Add("CNSM with CSN_AssetID set to 0");
            Verify(CSN_AssetID, ref result);
        }

        [Category("Cutscene Manager")]
        public AssetID CSN_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Cutscene Manager")]
        public DynamicTypeDescriptor CsnmFlags => IntFlagsDescriptor(0xC);

        [Category("Cutscene Manager")]
        public int InterpSpeed
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt40
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt48
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C);
            set => Write(0x5C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt60
        {
            get => ReadInt(0x60);
            set => Write(0x60, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt64
        {
            get => ReadInt(0x64);
            set => Write(0x64, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt68
        {
            get => ReadInt(0x68);
            set => Write(0x68, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt6C
        {
            get => ReadInt(0x6C);
            set => Write(0x6C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt70
        {
            get => ReadInt(0x70);
            set => Write(0x70, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt74
        {
            get => ReadInt(0x74);
            set => Write(0x74, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt78
        {
            get => ReadInt(0x78);
            set => Write(0x78, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt7C
        {
            get => ReadInt(0x7C);
            set => Write(0x7C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt80
        {
            get => ReadInt(0x80);
            set => Write(0x80, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt84
        {
            get => ReadInt(0x84);
            set => Write(0x84, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt88
        {
            get => ReadInt(0x88);
            set => Write(0x88, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt8C
        {
            get => ReadInt(0x8C);
            set => Write(0x8C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt90
        {
            get => ReadInt(0x90);
            set => Write(0x90, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt94
        {
            get => ReadInt(0x94);
            set => Write(0x94, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt98
        {
            get => ReadInt(0x98);
            set => Write(0x98, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownInt9C
        {
            get => ReadInt(0x9C);
            set => Write(0x9C, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntA0
        {
            get => ReadInt(0xA0);
            set => Write(0xA0, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntA4
        {
            get => ReadInt(0xA4);
            set => Write(0xA4, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntA8
        {
            get => ReadInt(0xA8);
            set => Write(0xA8, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntAC
        {
            get => ReadInt(0xAC);
            set => Write(0xAC, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntB0
        {
            get => ReadInt(0xB0);
            set => Write(0xB0, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntB4
        {
            get => ReadInt(0xB4);
            set => Write(0xB4, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntB8
        {
            get => ReadInt(0xB8);
            set => Write(0xB8, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntBC
        {
            get => ReadInt(0xBC);
            set => Write(0xBC, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntC0
        {
            get => ReadInt(0xC0);
            set => Write(0xC0, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntC4
        {
            get => ReadInt(0xC4);
            set => Write(0xC4, value);
        }

        [Category("Cutscene Manager")]
        public int UnknownIntC8
        {
            get
            {
                if (game == Game.Incredibles)
                    return ReadInt(0xC8);
                return 0;
            }
            set
            {
                if (game == Game.Incredibles)
                    Write(0xC8, value);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.Incredibles)
                dt.RemoveProperty("UnknownIntC8");
            base.SetDynamicProperties(dt);
        }
    }
}