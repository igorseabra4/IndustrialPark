using System.ComponentModel;

namespace IndustrialPark
{
    public class LinkTSSM : Link
    {
        [DisplayName("Receive Event")]
        public EventTSSM EventReceiveID { get => (EventTSSM)_eventReceiveID; set => _eventReceiveID = (ushort)value; }
        [DisplayName("Send Event")]
        public EventTSSM EventSendID { get => (EventTSSM)_eventSendID; set => _eventSendID = (ushort)value; }

        public LinkTSSM(bool isTimed = false) : base(isTimed) { }

        public LinkTSSM(byte[] data, int offset, bool isTimed) : base(data, offset, isTimed) { }

        public override string ToString()
        {
            return (IsTimed ? Time.ToString() : EventReceiveID.ToString()) + $" => {EventSendID.ToString()} => {Program.MainForm.GetAssetNameFromID(TargetAssetID)}";
        }
    }
}