using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static HipHopFile.Functions;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class TimedEvent
    {
        public static readonly int sizeOfStruct = 32;

        public float Time { get; set; }
        [DisplayName("Target Asset")]
        public AssetID TargetAssetID { get; set; }
        private int _eventSendID;
        [DisplayName("Send Event")]
        public EventTypeTSSM EventID_TSSM { get => (EventTypeTSSM)_eventSendID; set => _eventSendID = (int)value; }
        [DisplayName("Send Event")]
        public EventTypeIncredibles EventID_Incredibles { get => (EventTypeIncredibles)_eventSendID; set => _eventSendID = (int)value; }

        public override string ToString()
        {
            return $"{Time.ToString()} => {EventID_TSSM.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }

        protected byte[] arguments;

        public TimedEvent()
        {
            _eventSendID = 0;
            TargetAssetID = 0;
            arguments = new byte[20];
        }

        public TimedEvent(byte[] data, int offset)
        {
            Time = ConverterFunctions.Switch(BitConverter.ToSingle(data, offset));
            TargetAssetID = ConverterFunctions.Switch(BitConverter.ToUInt32(data, offset + 4));
            _eventSendID = ConverterFunctions.Switch(BitConverter.ToInt32(data, offset + 8));

            arguments = new byte[20];
            for (int i = 0; i < 20; i++)
                arguments[i] = data[offset + 12 + i];
        }

        public float[] Arguments_Float
        {
            get
            {
                float[] result = new float[5];
                for (int i = 0; i < 5; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToSingle(arguments, 4 * i));

                return result;
            }
            set
            {
                float[] result = new float[5];

                if (value.Length < 5)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 5. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 5; i++)
                        result[i] = 0f;
                }
                else
                    for (int i = 0; i < 5; i++)
                        result[i] = value[i];

                if (value.Length > 6)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 5. Your values will be trimmed.");

                for (int i = 0; i < 5; i++)
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
                AssetID[] result = new AssetID[5];
                for (int i = 0; i < 5; i++)
                    result[i] = ConverterFunctions.Switch(BitConverter.ToUInt32(arguments, 4 * i));

                return result;
            }
            set
            {
                AssetID[] result = new AssetID[5];

                if (value.Length < 5)
                {
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 5. Your values will be padded.");
                    for (int i = 0; i < value.Length; i++)
                        result[i] = value[i];
                    for (int i = value.Length; i < 5; i++)
                        result[i] = 0;
                }
                else
                    for (int i = 0; i < 5; i++)
                        result[i] = value[i];

                if (value.Length > 5)
                    System.Windows.Forms.MessageBox.Show("Arguments must have an exact length of 5. Your values will be trimmed.");

                for (int i = 0; i < 5; i++)
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
                data[i + 12] = arguments[i];

            if (currentPlatform == HipHopFile.Platform.GameCube)
            {
                data[0] = BitConverter.GetBytes(Time)[3];
                data[1] = BitConverter.GetBytes(Time)[2];
                data[2] = BitConverter.GetBytes(Time)[1];
                data[3] = BitConverter.GetBytes(Time)[0];
                data[4] = BitConverter.GetBytes(TargetAssetID)[3];
                data[5] = BitConverter.GetBytes(TargetAssetID)[2];
                data[6] = BitConverter.GetBytes(TargetAssetID)[1];
                data[7] = BitConverter.GetBytes(TargetAssetID)[0];
                data[8] = BitConverter.GetBytes(_eventSendID)[3];
                data[9] = BitConverter.GetBytes(_eventSendID)[2];
                data[10] = BitConverter.GetBytes(_eventSendID)[1];
                data[11] = BitConverter.GetBytes(_eventSendID)[0];
            }
            else
            {
                data[0] = BitConverter.GetBytes(Time)[0];
                data[1] = BitConverter.GetBytes(Time)[1];
                data[2] = BitConverter.GetBytes(Time)[2];
                data[3] = BitConverter.GetBytes(Time)[3];
                data[4] = BitConverter.GetBytes(TargetAssetID)[0];
                data[5] = BitConverter.GetBytes(TargetAssetID)[1];
                data[6] = BitConverter.GetBytes(TargetAssetID)[2];
                data[7] = BitConverter.GetBytes(TargetAssetID)[3];
                data[8] = BitConverter.GetBytes(_eventSendID)[0];
                data[9] = BitConverter.GetBytes(_eventSendID)[1];
                data[10] = BitConverter.GetBytes(_eventSendID)[2];
                data[11] = BitConverter.GetBytes(_eventSendID)[3];
            }

            return data;
        }
    }

    public class AssetSRCP : ObjectAsset
    {
        public AssetSRCP(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (TimedEvent a in TimedEvents)
                if (a.TargetAssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        [Category("Scripted Event")]
        public float UnknownFloat08
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }

        [Category("Scripted Event"), ReadOnly(true)]
        public int TimedEventCount
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Scripted Event"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag1
        {
            get => ReadByte(0x10);
            set => Write(0x10, value);
        }

        [Category("Scripted Event"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2
        {
            get => ReadByte(0x11);
            set => Write(0x11, value);
        }

        [Category("Scripted Event"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3
        {
            get => ReadByte(0x12);
            set => Write(0x12, value);
        }

        [Category("Scripted Event"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4
        {
            get => ReadByte(0x13);
            set => Write(0x13, value);
        }

        [Category("Scripted Event")]
        public TimedEvent[] TimedEvents
        {
            get
            {
                TimedEvent[] events = new TimedEvent[TimedEventCount];

                for (int i = 0; i < TimedEventCount; i++)
                    events[i] = new TimedEvent(Data, 0x14 + i * TimedEvent.sizeOfStruct);
                
                return events.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(0x14).ToList();
                List<byte> restOfOldData = Data.Skip(0x14 + TimedEvent.sizeOfStruct * TimedEventCount).ToList();

                foreach (TimedEvent i in value)
                    newData.AddRange(i.ToByteArray());

                newData.AddRange(restOfOldData);
                Data = newData.ToArray();

                TimedEventCount = value.Length;
            }
        }
    }
}