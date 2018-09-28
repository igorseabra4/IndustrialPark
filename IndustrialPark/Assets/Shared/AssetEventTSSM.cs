using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEventTSSM : AssetEvent
    {
        [DisplayName("Receive Event")]
        public EventTypeTSSM EventReceiveID { get => (EventTypeTSSM)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventTypeTSSM EventSendID { get => (EventTypeTSSM)_eventSendID; set => _eventSendID = (ushort)value; }

        public AssetEventTSSM() : base() { }

        public AssetEventTSSM(byte[] data, int offset) : base(data, offset) { }

        public override string ToString()
        {
            return $"{EventReceiveID.ToString()} => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}