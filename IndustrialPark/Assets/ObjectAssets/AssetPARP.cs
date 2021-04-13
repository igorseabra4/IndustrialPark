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

        private readonly int index;

        public StructPARP(int index)
        {
            this.index = index;
        }

        public StructPARP(EndianBinaryReader reader, int index)
        {
            Interp_0 = reader.ReadSingle();
            Interp_1 = reader.ReadSingle();
            Interp_Mode = (Interp_Mode)reader.ReadInt32();
            Frequency_RandLinStep = reader.ReadSingle();
            Frequency_SinCos = reader.ReadSingle();
            this.index = index;
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(Interp_0);
            writer.Write(Interp_1);
            writer.Write((uint)Interp_Mode);
            writer.Write(Frequency_RandLinStep);
            writer.Write(Frequency_SinCos);
            return writer.ToArray();
        }

        public string EntryFunction 
        { 
            get
            {
                switch (index)
                {
                    case 0: return "Rate (how many times per second a particle is emitted)";
                    case 1: return "Life (particle lifetime in seconds)";
                    case 2: return "Birth size in units";
                    case 3: return "Death size in units";
                    case 4: return "Start color (red component)";
                    case 5: return "Start color (green component)";
                    case 6: return "Start color (blue component)";
                    case 7: return "Start color (alpha component)";
                    case 8: return "End color (red component)";
                    case 9: return "End color (green component)";
                    case 10: return "End color (blue component)";
                    case 11: return "End color (alpha component)";
                    case 12: return "Vel_Scale (unknown/unused)";
                    case 13: return "Vel_Angle (unknown/unused)";
                    default: return "Error";
                }
            }
        }
    }

    public class AssetPARP : BaseAsset
    {
        private const string categoryName = "Particle Properties";

        [Category(categoryName)]
        public AssetID PARS_AssetID { get; set; }
        [Category(categoryName)]
        private StructPARP[] _structs { get; set; }
        [Category(categoryName), Description("Each of the 14 structs has a different function. Check wiki page for more info.")]
        public StructPARP[] Structs 
        { 
            get => _structs; 
            set
            {
                List<StructPARP> list = value.ToList();
                if (list.Count != 14)
                    MessageBox.Show("Array of PARP structs must have exactly 14 entries!");
                while (list.Count < 14)
                    list.Add(new StructPARP(list.Count));
                while (list.Count > 14)
                    list.RemoveAt(list.Count - 1);

                _structs = list.ToArray();
            } 
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float VelX { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float VelY { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float VelZ { get; set; }
        [Category(categoryName)]
        public int Emit_Limit { get; set; }
        [Category(categoryName)]
        public int Emit_limit_reset_time { get; set; }

        public AssetPARP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            PARS_AssetID = reader.ReadUInt32();

            _structs = new StructPARP[14];

            for (int i = 0; i < _structs.Length; i++)
                _structs[i] = new StructPARP(reader, i);

            VelX = reader.ReadSingle();
            VelY = reader.ReadSingle();
            VelZ = reader.ReadSingle();
            Emit_Limit = reader.ReadInt32();
            Emit_limit_reset_time = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(PARS_AssetID);

            if (_structs.Length != 14)
                throw new Exception("PARS structs must be exactly 14 entries.");
            foreach (var p in _structs)
                writer.Write(p.Serialize(platform));

            writer.Write(VelX);
            writer.Write(VelY);
            writer.Write(VelZ);
            writer.Write(Emit_Limit);
            writer.Write(Emit_limit_reset_time);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => PARS_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PARS_AssetID == 0)
                result.Add("PARP with PARS_AssetID set to 0");
            Verify(PARS_AssetID, ref result);
        }
    }
}