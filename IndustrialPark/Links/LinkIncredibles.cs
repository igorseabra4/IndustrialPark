using System.ComponentModel;

namespace IndustrialPark
{
    public class LinkIncredibles : Link
    {
        [DisplayName("Receive Event")]
        public EventIncredibles EventReceiveID { get => (EventIncredibles)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventIncredibles EventSendID { get => (EventIncredibles)_eventSendID; set => _eventSendID = (ushort)value; }

        public LinkIncredibles(bool isTimed = false) : base(isTimed) { }

        public LinkIncredibles(byte[] data, int offset, bool isTimed) : base(data, offset, isTimed) { }

        public override string ToString()
        {
            return (IsTimed ? Time.ToString() : EventReceiveID.ToString()) + $" => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}