using System;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;
using static HipHopFile.Functions;
using System.Collections.Generic;

namespace IndustrialPark
{
    public abstract class Link
    {
        public static readonly int sizeOfStruct = 32;

        [DisplayName("Target Asset")]
        public AssetID TargetAssetID { get; set; }
        protected ushort _eventReceiveID;
        protected ushort _eventSendID;
        protected byte[] arguments;
        public AssetID ArgumentAssetID { get; set; }
        public AssetID SourceCheckAssetID { get; set; }

        protected bool IsTimed = false;
        public float Time { get; set; }

        protected Link(bool isTimed)
        {
            _eventReceiveID = 0;
            _eventSendID = 0;
            TargetAssetID = 0;
            arguments = new byte[16];
            ArgumentAssetID = 0;
            SourceCheckAssetID = 0;
            IsTimed = isTimed;
        }

        protected Link(byte[] data, int offset, bool isTimed)
        {
            IsTimed = isTimed;

            TargetAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 4));

            if (isTimed)
            {
                Time = ConverterFunctions.Switch(BitConverter.ToSingle(data, offset));
                _eventSendID = (ushort)ConverterFunctions.Switch(BitConverter.ToInt32(data, offset + 8));

                arguments = new byte[16];
                for (int i = 0; i < 16; i++)
                    arguments[i] = data[offset + 12 + i];

                ArgumentAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 0x1C));
                SourceCheckAssetID = 0;
            }
            else
            {
                _eventReceiveID = Switch(BitConverter.ToUInt16(data, offset));
                _eventSendID = Switch(BitConverter.ToUInt16(data, offset + 2));

                arguments = new byte[16];
                for (int i = 0; i < 16; i++)
                    arguments[i] = data[offset + 8 + i];

                ArgumentAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 0x18));
                SourceCheckAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 0x1C));
            }
        }

        public float[] Arguments_Float
        {
            get
            {
                float[] result = new float[4];
                for (int i = 0; i < 4; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToSingle(arguments, 4 * i));

                return result;
            }
            set
            {
                float[] result = new float[4];

                if (value.Length < 4)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 4. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 4; i++)
                        result[i] = 0f;
                }
                else
                    for (int i = 0; i < 4; i++)
                        result[i] = value[i];

                if (value.Length > 4)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 4. Your values will be trimmed.");

                for (int i = 0; i < 4; i++)
                {
                    byte[] r = BitConverter.GetBytes(ConverterFunctions.Switch(result[i]));

                    arguments[i * 4 + 0] = r[0];
                    arguments[i * 4 + 1] = r[1];
                    arguments[i * 4 + 2] = r[2];
                    arguments[i * 4 + 3] = r[3];
                }
            }
        }

        public AssetID[] Arguments_Hex
        {
            get
            {
                AssetID[] result = new AssetID[4];
                for (int i = 0; i < 4; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToUInt32(arguments, 4 * i));

                return result;
            }
            set
            {
                AssetID[] result = new AssetID[4];

                if (value.Length < 4)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 4. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 4; i++)
                        result[i] = 0;
                }
                else
                    for (int i = 0; i < 4; i++)
                        result[i] = value[i];

                if (value.Length > 4)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 4. Your values will be trimmed.");

                for (int i = 0; i < 4; i++)
                {
                    byte[] r = BitConverter.GetBytes(ConverterFunctions.Switch(result[i]));

                    arguments[i * 4 + 0] = r[0];
                    arguments[i * 4 + 1] = r[1];
                    arguments[i * 4 + 2] = r[2];
                    arguments[i * 4 + 3] = r[3];
                }
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> data = new List<byte>();

            if (IsTimed)
            {
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(Time)));
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(TargetAssetID)));
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch((int)_eventSendID)));
                data.AddRange(arguments);
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(ArgumentAssetID)));
            }
            else
            {
                data.AddRange(BitConverter.GetBytes(Switch(_eventReceiveID)));
                data.AddRange(BitConverter.GetBytes(Switch(_eventSendID)));
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(TargetAssetID)));
                data.AddRange(arguments);
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(ArgumentAssetID)));
                data.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(SourceCheckAssetID)));
            }

            return data.ToArray();
        }
    }
}