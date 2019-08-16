using System.ComponentModel;

namespace IndustrialPark
{
    public class LinkBFBB : Link
    {
        [DisplayName("Receive Event")]
        public EventBFBB EventReceiveID { get => (EventBFBB)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventBFBB EventSendID { get => (EventBFBB)_eventSendID; set => _eventSendID = (ushort)value; }

        public LinkBFBB(Endianness endianness, bool isTimed) : base(endianness, isTimed) { }

        public LinkBFBB(byte[] data, int offset, bool isTimed, Endianness endianness) : base(data, offset, isTimed, endianness) { }

        public override string ToString()
        {
            return (IsTimed ? Time.ToString() : EventReceiveID.ToString()) + $" => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}