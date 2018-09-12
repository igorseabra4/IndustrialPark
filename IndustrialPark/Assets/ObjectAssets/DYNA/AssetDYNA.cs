using HipHopFile;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class AssetDYNA : ObjectAsset
    {
        public AssetDYNA(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset { get => Data.Length - (AmountOfEvents * AssetEvent.sizeOfStruct); }

        public void Setup()
        {
            SetDynaSpecific(false);
        }

        private void SetDynaSpecific(bool reset)
        {
            List<byte> dataBefore = Data.Take(0x10).ToList();
            List<byte> dataAfter = Data.Skip(EventStartOffset).ToList();

            switch (Type)
            {
                case DynaType.pointer:
                    _dynaSpecific = reset ? new DynaPointer() : new DynaPointer(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__talk_box:
                    _dynaSpecific = reset ? new DynaTalkBox() : new DynaTalkBox(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                default:
                    _dynaSpecific = reset ? new DynaBase() : new DynaBase(Data.Skip(0x10).Take(EventStartOffset));
                    break;
            }

            dataBefore.AddRange(_dynaSpecific.ToByteArray());
            dataBefore.AddRange(dataAfter);

            Data = dataBefore.ToArray();
        }

        public DynaType Type
        {
            get => (DynaType)ReadUInt(0x8);
            set
            {
                Write(0x8, (uint)value);
                SetDynaSpecific(true);
            }
        }

        public short Version
        {
            get => ReadShort(0xC);
            set => Write(0xC, value);
        }

        public short Unknown
        {
            get => ReadShort(0xE);
            set => Write(0xE, value);
        }

        private DynaBase _dynaSpecific;
        public DynaBase DynaBase
        {
            get => _dynaSpecific;
            set
            {
                List<byte> dataBefore = Data.Take(0x10).ToList();
                List<byte> dataAfter = Data.Skip(EventStartOffset).ToList();

                dataBefore.AddRange(value.ToByteArray());
                dataBefore.AddRange(dataAfter);

                _dynaSpecific = value;
                Data = dataBefore.ToArray();
            }
        }
    }
}