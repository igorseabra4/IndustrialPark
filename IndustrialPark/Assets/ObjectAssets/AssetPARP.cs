using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace IndustrialPark
{
    public class StructPARP
    {
        public float UnknownFloat00 { get; set; }
        public float UnknownFloat04 { get; set; }
        public AssetID UnknownStringHash08 { get; set; }
        public float UnknownFloat0C { get; set; }
        public float UnknownFloat10 { get; set; }

        public StructPARP()
        {
            UnknownStringHash08 = 0;
        }
    }

    public class AssetPARP : ObjectAsset
    {
        public AssetPARP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x138;

        public override bool HasReference(uint assetID) => PARS_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PARS_AssetID == 0)
                result.Add("PARP with PARS_AssetID set to 0");
            Verify(PARS_AssetID, ref result);
        }

        [Category("Particle Properties")]
        public AssetID PARS_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Particle Properties")]
        public StructPARP[] Structs
        {
            get
            {
                List<StructPARP> list = new List<StructPARP>();

                for (int i = 0; i < 12; i++)
                {
                    list.Add(new StructPARP()
                    {
                        UnknownFloat00 = ReadFloat(0x0C + i * 0x14 + 0x00),
                        UnknownFloat04 = ReadFloat(0x0C + i * 0x14 + 0x04),
                        UnknownStringHash08 = ReadUInt(0x0C + i * 0x14 + 0x08),
                        UnknownFloat0C = ReadFloat(0x0C + i * 0x14 + 0x0C),
                        UnknownFloat10 = ReadFloat(0x0C + i * 0x14 + 0x10)
                    });
                }

                return list.ToArray();
            }
            set
            {
                List<StructPARP> list = value.ToList();
                if (list.Count != 12)
                    System.Windows.Forms.MessageBox.Show("Array of PARP structs must have exactly 12 entries!");
                while (list.Count < 12)
                    list.Add(new StructPARP());
                while (list.Count > 12)
                    list.RemoveAt(list.Count - 1);

                List<byte> before = Data.Take(0xC).ToList();

                foreach (StructPARP a in list)
                {
                    before.AddRange(BitConverter.GetBytes(Switch(a.UnknownFloat00)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.UnknownFloat04)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.UnknownStringHash08)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.UnknownFloat0C)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.UnknownFloat10)));
                }

                before.AddRange(Data.Skip(0xFC));
                Data = before.ToArray();
            }
        }

        [Category("Particle Properties")]
        public int UnknownIntFC
        {
            get => ReadInt(0xFC);
            set => Write(0xFC, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt100
        {
            get => ReadInt(0x100);
            set => Write(0x100, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt104
        {
            get => ReadInt(0x104);
            set => Write(0x104, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt108
        {
            get => ReadInt(0x108);
            set => Write(0x108, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt10C
        {
            get => ReadInt(0x10C);
            set => Write(0x10C, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt110
        {
            get => ReadInt(0x110);
            set => Write(0x110, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt114
        {
            get => ReadInt(0x114);
            set => Write(0x114, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt118
        {
            get => ReadInt(0x118);
            set => Write(0x118, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt11C
        {
            get => ReadInt(0x11C);
            set => Write(0x11C, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt120
        {
            get => ReadInt(0x120);
            set => Write(0x120, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt124
        {
            get => ReadInt(0x124);
            set => Write(0x124, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt128
        {
            get => ReadInt(0x128);
            set => Write(0x128, value);
        }

        [Category("Particle Properties")]
        public int UnknownInt12C
        {
            get => ReadInt(0x12C);
            set => Write(0x12C, value);
        }

        [Category("Particle Properties")]
        public int Emit_Limit
        {
            get => ReadInt(0x130);
            set => Write(0x130, value);
        }

        [Category("Particle Properties")]
        public int Emit_limit_reset_time
        {
            get => ReadInt(0x134);
            set => Write(0x134, value);
        }
    }
}