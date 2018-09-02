using System;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

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

        private float[] arguments;
        public float[] Arguments
        {
            get => arguments;
            set
            {
                if (value.Length > 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be trimmed.");
                    for (int i = 0; i < 6; i++)
                        arguments[i] = value[i];
                }
                else if (value.Length < 6)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 6. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        arguments[i] = value[i];
                    for (int i = value.Length; i < 6; i++)
                        arguments[i] = 0f;
                }
                else
                    arguments = value;
            }
        }

        public AssetEvent()
        {
            EventReceiveID = 0;
            EventSendID = 0;
            TargetAssetID = 0;
            arguments = new float[6];
        }

        public static AssetEvent FromByteArray(byte[] data, int offset)
        {
            AssetEvent newEvent = new AssetEvent()
            {
                EventReceiveID = (EventType)Switch(BitConverter.ToUInt16(data, offset)),
                EventSendID = (EventType)Switch(BitConverter.ToUInt16(data, offset + 2)),
                TargetAssetID = Switch(BitConverter.ToUInt32(data, offset + 4))
            };
            for (int i = 0; i < newEvent.arguments.Length; i++)
                newEvent.arguments[i] = Switch(BitConverter.ToSingle(data, offset + 8 + i * 4));

            return newEvent;
        }

        public byte[] ToByteArray()
        {
            byte[] data = new byte[sizeOfStruct];

            data[0] = BitConverter.GetBytes((ushort)EventReceiveID)[1];
            data[1] = BitConverter.GetBytes((ushort)EventReceiveID)[0];
            data[2] = BitConverter.GetBytes((ushort)EventSendID)[1];
            data[3] = BitConverter.GetBytes((ushort)EventSendID)[0];
            data[4] = BitConverter.GetBytes(TargetAssetID)[3];
            data[5] = BitConverter.GetBytes(TargetAssetID)[2];
            data[6] = BitConverter.GetBytes(TargetAssetID)[1];
            data[7] = BitConverter.GetBytes(TargetAssetID)[0];

            for (int i = 0; i < arguments.Length; i++)
            {
                byte[] getBytes = BitConverter.GetBytes(arguments[i]);
                data[i * 4 + 8] = getBytes[3];
                data[i * 4 + 9] = getBytes[2];
                data[i * 4 + 10] = getBytes[1];
                data[i * 4 + 11] = getBytes[0];
            }

            return data;
        }

        public override string ToString()
        {
            return $"{EventReceiveID.ToString()} => {EventSendID.ToString()} => [{TargetAssetID.ToString()}]";
        }
    }
}