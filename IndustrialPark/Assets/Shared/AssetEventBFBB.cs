using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEventBFBB : AssetEvent
    {
        [DisplayName("Receive Event")]
        public EventTypeBFBB EventReceiveID { get => (EventTypeBFBB)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventTypeBFBB EventSendID { get => (EventTypeBFBB)_eventSendID; set => _eventSendID = (ushort)value; }

        public AssetEventBFBB() : base() { }

        public AssetEventBFBB(byte[] data, int offset) : base(data, offset) { }

        public override string ToString()
        {
            return $"{EventReceiveID.ToString()} => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}