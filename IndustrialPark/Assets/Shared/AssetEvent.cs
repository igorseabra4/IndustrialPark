using System;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class AssetEvent
    {
        public static readonly int sizeOfStruct = 32;

        [DisplayName("Receive Event")]
        public EventType EventReceiveID { get; set; }
        [DisplayName("Send Event")]
        public EventType EventSendID { get; set; }
        [DisplayName("Target Asset")]
        public AssetID TargetAssetID { get; set; }
        private byte[] arguments;

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

                for (int i = 0; i < 6; i++)
                    result[i] = value[i];

                if (value.Length > 6)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be trimmed.");
                else if (value.Length < 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 6; i++)
                        result[i] = 0f;
                }

                for (int i = 0; i < 6; i++)
                {
                    byte[] r = BitConverter.GetBytes(ConverterFunctions.Switch(value[i]));

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

                for (int i = 0; i < 6; i++)
                    result[i] = value[i];

                if (value.Length > 6)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be trimmed.");
                else if (value.Length < 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 6; i++)
                        result[i] = 0;
                }

                for (int i = 0; i < 6; i++)
                {
                    byte[] r = BitConverter.GetBytes(ConverterFunctions.Switch(value[i]));

                    arguments[i * 4 + 0] = r[0];
                    arguments[i * 4 + 1] = r[1];
                    arguments[i * 4 + 2] = r[2];
                    arguments[i * 4 + 3] = r[3];
                }
            }
        }

        public AssetEvent()
        {
            EventReceiveID = 0;
            EventSendID = 0;
            TargetAssetID = 0;
            arguments = new byte[24];
        }

        public static AssetEvent FromByteArray(byte[] data, int offset)
        {
            AssetEvent newEvent = new AssetEvent()
            {
                EventReceiveID = (EventType)Switch(BitConverter.ToUInt16(data, offset)),
                EventSendID = (EventType)Switch(BitConverter.ToUInt16(data, offset + 2)),
                TargetAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 4))
            };
            for (int i = 0; i < 24; i++)
                newEvent.arguments[i] = data[offset + 8 + i];

            return newEvent;
        }

        public byte[] ToByteArray()
        {
            byte[] data = new byte[sizeOfStruct];

            for (int i = 0; i < arguments.Length; i++)
                data[i + 8] = arguments[i];

            if (currentPlatform != HipHopFile.Platform.GameCube)
            {
                data[0] = BitConverter.GetBytes((ushort)EventReceiveID)[0];
                data[1] = BitConverter.GetBytes((ushort)EventReceiveID)[1];
                data[2] = BitConverter.GetBytes((ushort)EventSendID)[0];
                data[3] = BitConverter.GetBytes((ushort)EventSendID)[1];
                data[4] = BitConverter.GetBytes(TargetAssetID)[0];
                data[5] = BitConverter.GetBytes(TargetAssetID)[1];
                data[6] = BitConverter.GetBytes(TargetAssetID)[2];
                data[7] = BitConverter.GetBytes(TargetAssetID)[3];
            }
            else
            {
                data[0] = BitConverter.GetBytes((ushort)EventReceiveID)[1];
                data[1] = BitConverter.GetBytes((ushort)EventReceiveID)[0];
                data[2] = BitConverter.GetBytes((ushort)EventSendID)[1];
                data[3] = BitConverter.GetBytes((ushort)EventSendID)[0];
                data[4] = BitConverter.GetBytes(TargetAssetID)[3];
                data[5] = BitConverter.GetBytes(TargetAssetID)[2];
                data[6] = BitConverter.GetBytes(TargetAssetID)[1];
                data[7] = BitConverter.GetBytes(TargetAssetID)[0];
            }

            return data;
        }

        public override string ToString()
        {
            return $"{EventReceiveID.ToString()} => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}