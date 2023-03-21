using System.Collections.Generic;
using System;
using System.IO;
using DiscordRPC;
using RenderWareFile.Sections;
using SharpDX;
using static System.Windows.Forms.AxHost;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel;
using SharpDX.Direct3D11;
using System.Linq;

namespace IndustrialPark
{
    public class SoundInfoGcn1Wrapper
    {
        public uint num_samples { get; set; }
        public uint num_adpcm_nibbles { get; set; }
        public uint sample_rate { get; set; }
        public ushort loop_flag { get; set; }
        public ushort format { get; set; }
        public uint loop_start_offset { get; set; }
        public uint loop_end_offset { get; set; }
        public uint initial_offset_value { get; set; }
        public short[] coefs { get; set; }
        public ushort gain_factor { get; set; }
        public ushort pred_scale { get; set; }
        public ushort yn1 { get; set; }
        public ushort yn2 { get; set; }
        public ushort loop_pred_scale { get; set; }
        public ushort loop_yn1 { get; set; }
        public ushort loop_yn2 { get; set; }
        private byte[] pad;

        public SoundInfoGcn1Wrapper(byte[] header)
        {
            using (var reader = new EndianBinaryReader(header, Endianness.Big))
            {
                num_samples = reader.ReadUInt32();
                num_adpcm_nibbles = reader.ReadUInt32();
                sample_rate = reader.ReadUInt32();
                loop_flag = reader.ReadUInt16();
                format = reader.ReadUInt16();
                loop_start_offset = reader.ReadUInt32();
                loop_end_offset = reader.ReadUInt32();
                initial_offset_value = reader.ReadUInt32();
                coefs = new short[16];
                for (int i = 0; i < coefs.Length; i++)
                    coefs[i] = reader.ReadInt16();
                gain_factor = reader.ReadUInt16();
                pred_scale = reader.ReadUInt16();
                yn1 = reader.ReadUInt16();
                yn2 = reader.ReadUInt16();
                loop_pred_scale = reader.ReadUInt16();
                loop_yn1 = reader.ReadUInt16();
                loop_yn2 = reader.ReadUInt16();
                pad = new byte[22];
                for (int i = 0; i < pad.Length; i++)
                    pad[i] = reader.ReadByte();
            }
            if (!Enumerable.SequenceEqual(header, ToByteArray()))
                throw new Exception("Unable to open sound editor");
        }

        public byte[] ToByteArray()
        {
            using (var writer = new EndianBinaryWriter(Endianness.Big))
            {
                writer.Write(num_samples);
                writer.Write(num_adpcm_nibbles);
                writer.Write(sample_rate);
                writer.Write(loop_flag);
                writer.Write(format);
                writer.Write(loop_start_offset);
                writer.Write(loop_end_offset);
                writer.Write(initial_offset_value);
                for (int i = 0; i < 16; i++)
                    if (i < coefs.Length)
                        writer.Write(coefs[i]);
                    else
                        writer.Write((short)0);
                writer.Write(gain_factor);
                writer.Write(pred_scale);
                writer.Write(yn1);
                writer.Write(yn2);
                writer.Write(loop_pred_scale);
                writer.Write(loop_yn1);
                writer.Write(loop_yn2);
                for (int i = 0; i < 22; i++)
                    if (i < pad.Length)
                        writer.Write(pad[i]);
                    else
                        writer.Write((byte)0);
                return writer.ToArray();
            }
        }
    }
}