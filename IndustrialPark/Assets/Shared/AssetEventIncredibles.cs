using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEventIncredibles : AssetEvent
    {
        [DisplayName("Receive Event")]
        public EventTypeIncredibles EventReceiveID { get => (EventTypeIncredibles)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventTypeIncredibles EventSendID { get => (EventTypeIncredibles)_eventSendID; set => _eventSendID = (ushort)value; }

        public AssetEventIncredibles() : base() { }

        public AssetEventIncredibles(byte[] data, int offset) : base(data, offset) { }

        public override string ToString()
        {
            return $"{EventReceiveID.ToString()} => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}