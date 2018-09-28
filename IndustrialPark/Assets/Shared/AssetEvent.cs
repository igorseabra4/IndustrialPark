using System;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public abstract class AssetEvent
    {
        public static readonly int sizeOfStruct = 32;

        [DisplayName("Target Asset")]
        public AssetID TargetAssetID { get; set; }
        protected ushort _eventReceiveID;
        protected ushort _eventSendID;
        protected byte[] arguments;

        protected AssetEvent()
        {
            _eventReceiveID = 0;
            _eventSendID = 0;
            TargetAssetID = 0;
            arguments = new byte[24];
        }

        protected AssetEvent(byte[] data, int offset)
        {
            _eventReceiveID = Switch(BitConverter.ToUInt16(data, offset));
            _eventSendID = Switch(BitConverter.ToUInt16(data, offset + 2));
            TargetAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 4));

            arguments = new byte[24];
            for (int i = 0; i < 24; i++)
                arguments[i] = data[offset + 8 + i];
        }

        public float[] Arguments_Float
        {
            get
            {
                float[] result = new float[6];
                for (int i = 0; i < 6; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToSingle(arguments, 4 * i));

                return result;
            }
            set
            {
                float[] result = new float[6];

                if (value.Length < 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 6; i++)
                        result[i] = 0f;
                }
                else
                    for (int i = 0; i < 6; i++)
                        result[i] = value[i];

                if (value.Length > 6)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be trimmed.");

                for (int i = 0; i < 6; i++)
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
                AssetID[] result = new AssetID[6];
                for (int i = 0; i < 6; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToUInt32(arguments, 4 * i));

                return result;
            }
            set
            {
                AssetID[] result = new AssetID[6];

                if (value.Length < 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 6; i++)
                        result[i] = 0;
                }
                else
                    for (int i = 0; i < 6; i++)
                        result[i] = value[i];

                if (value.Length > 6)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be trimmed.");

                for (int i = 0; i < 6; i++)
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
            byte[] data = new byte[sizeOfStruct];

            for (int i = 0; i < arguments.Length; i++)
                data[i + 8] = arguments[i];

            if (currentPlatform == HipHopFile.Platform.GameCube)
            {
                data[0] = BitConverter.GetBytes(_eventReceiveID)[1];
                data[1] = BitConverter.GetBytes(_eventReceiveID)[0];
                data[2] = BitConverter.GetBytes(_eventSendID)[1];
                data[3] = BitConverter.GetBytes(_eventSendID)[0];
                data[4] = BitConverter.GetBytes(TargetAssetID)[3];
                data[5] = BitConverter.GetBytes(TargetAssetID)[2];
                data[6] = BitConverter.GetBytes(TargetAssetID)[1];
                data[7] = BitConverter.GetBytes(TargetAssetID)[0];
            }
            else
            {
                data[0] = BitConverter.GetBytes(_eventReceiveID)[0];
                data[1] = BitConverter.GetBytes(_eventReceiveID)[1];
                data[2] = BitConverter.GetBytes(_eventSendID)[0];
                data[3] = BitConverter.GetBytes(_eventSendID)[1];
                data[4] = BitConverter.GetBytes(TargetAssetID)[0];
                data[5] = BitConverter.GetBytes(TargetAssetID)[1];
                data[6] = BitConverter.GetBytes(TargetAssetID)[2];
                data[7] = BitConverter.GetBytes(TargetAssetID)[3];
            }

            return data;
        }
    }
}