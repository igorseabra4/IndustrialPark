using System;
using System.Linq;

namespace IndustrialPark
{
    public class SoundInfoGcn1Wrapper
    {
        public EntrySoundInfo_GCN_V1 Entry;

        public SoundInfoGcn1Wrapper(EntrySoundInfo_GCN_V1 entry)
        {
            Entry = entry;
        }

        public uint num_samples
        {
            get => Entry.num_samples;
            set => Entry.num_samples = value;
        }
        public uint num_adpcm_nibble
        {
            get => Entry.num_adpcm_nibbles;
            set => Entry.num_adpcm_nibbles = value;
        }
        public uint sample_rate
        {
            get => Entry.sample_rate; 
            set => Entry.sample_rate = value;
        }
        public bool Loop
        {
            get => Entry.Loop;
            set => Entry.Loop = value;
        }
        public ushort format
        {
            get => Entry.format; 
            set => Entry.format = value;
        }
        public uint loop_start_offset
        {
            get => Entry.loop_start_offset; 
            set => Entry.loop_start_offset = value;
        }
        public uint loop_end_offset
        {
            get => Entry.loop_end_offset; 
            set => Entry.loop_end_offset = value;
        }
        public uint initial_offset_value
        {
            get => Entry.initial_offset_value;
            set => Entry.initial_offset_value = value;
        }
        public short[] coefs
        {
            get => Entry.coefs;
            set => Entry.coefs = value;
        }
        public ushort gain_factor
        {
            get => Entry.gain_factor;
            set => Entry.gain_factor = value;
        }
        public ushort pred_scale
        {
            get => Entry.pred_scale;
            set => Entry.pred_scale = value;
        }
        public ushort yn1
        {
            get => Entry.yn1;
            set => Entry.yn1 = value;
        }
        public ushort yn2
        {
            get => Entry.yn2;
            set => Entry.yn2 = value;
        }
        public ushort loop_pred_scale
        {
            get => Entry.loop_pred_scale;
            set => Entry.loop_pred_scale = value;
        }
        public ushort loop_yn1
        {
            get => Entry.loop_yn1;
            set => Entry.loop_yn2 = value;
        }
        public ushort loop_yn2
        {
            get => Entry.loop_yn2;
            set => Entry.loop_yn2 = value;
        }
        public byte[] pad
        {
            get => Entry.pad;
            set => Entry.pad = value;
        }
    }
}