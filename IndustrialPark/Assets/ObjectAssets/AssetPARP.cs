using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace IndustrialPark
{
    public class StructPARP
    {
        public float InterpStart { get; set; }
        public float InterpEnd { get; set; }
        public AssetID InterpMode { get; set; }
        public float Frequency { get; set; }
        public float Frequency2 { get; set; }

        public StructPARP()
        {
            InterpMode = 0;
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

        [Category("Particle Properties"), Description("Each of the 14 structs has a different function. Check wiki page for more info.")]
        public StructPARP[] Structs
        {
            get
            {
                List<StructPARP> list = new List<StructPARP>();

                for (int i = 0; i < 14; i++)
                {
                    list.Add(new StructPARP()
                    {
                        InterpStart = ReadFloat(0x0C + i * 0x14 + 0x00),
                        InterpEnd = ReadFloat(0x0C + i * 0x14 + 0x04),
                        InterpMode = ReadUInt(0x0C + i * 0x14 + 0x08),
                        Frequency = ReadFloat(0x0C + i * 0x14 + 0x0C),
                        Frequency2 = ReadFloat(0x0C + i * 0x14 + 0x10)
                    });
                }

                return list.ToArray();
            }
            set
            {
                List<StructPARP> list = value.ToList();
                if (list.Count != 14)
                    System.Windows.Forms.MessageBox.Show("Array of PARP structs must have exactly 14 entries!");
                while (list.Count < 14)
                    list.Add(new StructPARP());
                while (list.Count > 14)
                    list.RemoveAt(list.Count - 1);

                List<byte> before = Data.Take(0xC).ToList();

                foreach (StructPARP a in list)
                {
                    before.AddRange(BitConverter.GetBytes(Switch(a.InterpStart)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.InterpEnd)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.InterpMode)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.Frequency)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.Frequency2)));
                }

                before.AddRange(Data.Skip(0x124));
                Data = before.ToArray();
            }
        }

        [Category("Particle Properties"), TypeConverter(typeof(FloatTypeConverter))]
        public float VelX
        {
            get => ReadFloat(0x124);
            set => Write(0x124, value);
        }

        [Category("Particle Properties"), TypeConverter(typeof(FloatTypeConverter))]
        public float VelY
        {
            get => ReadFloat(0x128);
            set => Write(0x128, value);
        }

        [Category("Particle Properties"), TypeConverter(typeof(FloatTypeConverter))]
        public float VelZ
        {
            get => ReadFloat(0x12C);
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