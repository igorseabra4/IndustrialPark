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
                case DynaType.game_object__text_box:
                    _dynaSpecific = reset ? new DynaTextBox() : new DynaTextBox(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__task_box:
                    _dynaSpecific = reset ? new DynaBase() : new DynaBase(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__NPCSettings:
                    _dynaSpecific = reset ? new DynaNPCSettings() : new DynaNPCSettings(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__BusStop:
                    _dynaSpecific = reset ? new DynaBusStop() : new DynaBusStop(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__Flythrough:
                    _dynaSpecific = reset ? new DynaFlythrough() : new DynaFlythrough(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__BoulderGenerator:
                    _dynaSpecific = reset ? new DynaBoulderGen() : new DynaBoulderGen(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__Teleport:
                    _dynaSpecific = reset ? new DynaTeleport() : new DynaTeleport(Data.Skip(0x10).Take(EventStartOffset), Version);
                    break;
                case DynaType.game_object__Taxi:
                    _dynaSpecific = reset ? new DynaTaxi() : new DynaTaxi(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__bungee_drop:
                    _dynaSpecific = reset ? new DynaBungeeDrop() : new DynaBungeeDrop(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__bungee_hook:
                    _dynaSpecific = reset ? new DynaBungeeHook() : new DynaBungeeHook(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.game_object__Camera_Tweak:
                    _dynaSpecific = reset ? new DynaCamTweak() : new DynaCamTweak(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.hud__model:
                    _dynaSpecific = reset ? new DynaHudModel() : new DynaHudModel(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.hud__meter__font:
                    if (Version == 3)
                        _dynaSpecific = reset ? new DynaHudMeterFontV3() : new DynaHudMeterFontV3(Data.Skip(0x10).Take(EventStartOffset));
                    else
                        _dynaSpecific = reset ? new DynaBase() : new DynaBase(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.hud__meter__unit:
                    _dynaSpecific = reset ? new DynaHudMeterUnit() : new DynaHudMeterUnit(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                case DynaType.hud__text:
                    _dynaSpecific = reset ? new DynaHudText() : new DynaHudText(Data.Skip(0x10).Take(EventStartOffset));
                    break;
                default:
                    //System.Windows.Forms.MessageBox.Show("Unknown DYNA type found: [" + ((int)Type).ToString("X8") + "] on asset " + ToString() + ".");
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