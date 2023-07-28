using Newtonsoft.Json;
using System.IO;

namespace IndustrialPark
{
    public class FMOD_GcADPCMInfo : GenericAssetDataContainer
    {
        public short[] Coef { get; set; }

        public ushort Gain { get; set; }
        public ushort PredScale { get; set; }
        public ushort Yn1 { get; set; }
        public ushort Yn2 { get; set; }
        public ushort LoopPredScale { get; set; }
        public short LoopYn1 { get; set; }
        public short LoopYn2 { get; set; }

        public FMOD_GcADPCMInfo()
        {
            Coef = new short[16];
        }

        public FMOD_GcADPCMInfo(BinaryReader binaryReader)
        {
            Coef = new short[16];
            for (int i = 0; i < Coef.Length; i++)
                Coef[i] = binaryReader.ReadInt16();

            Gain = binaryReader.ReadUInt16();
            PredScale = binaryReader.ReadUInt16();
            Yn1 = binaryReader.ReadUInt16();
            Yn2 = binaryReader.ReadUInt16();
            LoopPredScale = binaryReader.ReadUInt16();
            LoopYn1 = binaryReader.ReadInt16();
            LoopYn2 = binaryReader.ReadInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            for (int i = 0; i < 16; i++)
                if (i < Coef.Length)
                    writer.Write(Coef[i]);
                else
                    writer.Write(new byte[2]);

            writer.Write(Gain);
            writer.Write(PredScale);
            writer.Write(Yn1);
            writer.Write(Yn2);
            writer.Write(LoopPredScale);
            writer.Write(LoopYn1);
            writer.Write(LoopYn2);
        }

        public FMOD_GcADPCMInfo Clone()
        {
            return new FMOD_GcADPCMInfo()
            {
                Coef = JsonConvert.DeserializeObject<short[]>(JsonConvert.SerializeObject(Coef)),
                Gain = Gain,
                PredScale = PredScale,
                Yn1 = Yn1,
                Yn2 = Yn2,
                LoopPredScale = LoopPredScale,
                LoopYn1 = LoopYn1,
                LoopYn2 = LoopYn2,
            };
        }
    }
}