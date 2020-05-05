using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace IndustrialPark
{
    public enum Interp_Mode : uint
    {
        Null = 0x0,
        ConstA = 0x48E48E7A,
        ConstB = 0x48E48E7B,
        Random = 0x0FE111BF,
        Linear = 0xB7353B79,
        Sine = 0x0B326F01,
        Cosine = 0x498D7119,
        Time = 0x0B54BC19,
        Step = 0x0B354BD4,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StructPARP
    {
        public float Interp_0 { get; set; }
        public float Interp_1 { get; set; }
        public Interp_Mode Interp_Mode { get; set; }
        public float Frequency_RandLinStep { get; set; }
        public float Frequency_SinCos { get; set; }

        [ReadOnly(true)]
        public string EntryFunction { get; set; }
    }

    public class AssetPARP : BaseAsset
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
                        Interp_0 = ReadFloat(0x0C + i * 0x14 + 0x00),
                        Interp_1 = ReadFloat(0x0C + i * 0x14 + 0x04),
                        Interp_Mode = (Interp_Mode)ReadUInt(0x0C + i * 0x14 + 0x08),
                        Frequency_RandLinStep = ReadFloat(0x0C + i * 0x14 + 0x0C),
                        Frequency_SinCos = ReadFloat(0x0C + i * 0x14 + 0x10)
                    });
                }

                list[0].EntryFunction = "Rate (how many times per second a particle is emitted)";
                list[1].EntryFunction = "Life (particle lifetime in seconds)";
                list[2].EntryFunction = "Birth size in units";
                list[3].EntryFunction = "Death size in units";
                list[4].EntryFunction = "Start color (red component)";
                list[5].EntryFunction = "Start color (green component)";
                list[6].EntryFunction = "Start color (blue component)";
                list[7].EntryFunction = "Start color (alpha component)";
                list[8].EntryFunction = "End color (red component)";
                list[9].EntryFunction = "End color (green component)";
                list[10].EntryFunction = "End color (blue component)";
                list[11].EntryFunction = "End color (alpha component)";
                list[12].EntryFunction = "Vel_Scale (unknown/unused)";
                list[13].EntryFunction = "Vel_Angle (unknown/unused)";

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
                    before.AddRange(BitConverter.GetBytes(Switch(a.Interp_0)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.Interp_1)));
                    before.AddRange(BitConverter.GetBytes(Switch((uint)a.Interp_Mode)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.Frequency_RandLinStep)));
                    before.AddRange(BitConverter.GetBytes(Switch(a.Frequency_SinCos)));
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