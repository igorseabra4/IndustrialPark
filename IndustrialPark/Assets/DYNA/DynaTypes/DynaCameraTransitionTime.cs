using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaCameraTransitionTime : AssetDYNA
    {
        private const string dynaCategoryName = "Camera:transition_time";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(mDestCameraID);
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public uint mVersion { get; set; }
        [Category(dynaCategoryName)]
        public int mType { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask mFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID mDestCameraID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mAccel { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mDecel { get; set; }

        public DynaCameraTransitionTime(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.camera__transition_time, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                mVersion = reader.ReadUInt32();
                mType = reader.ReadInt32();
                mFlags.FlagValueInt = reader.ReadUInt32();
                mDestCameraID = reader.ReadUInt32();
                mTime = reader.ReadSingle();
                mAccel = reader.ReadSingle();
                mDecel = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(mVersion);
            writer.Write(mType);
            writer.Write(mFlags.FlagValueInt);
            writer.Write(mDestCameraID);
            writer.Write(mTime);
            writer.Write(mAccel);
            writer.Write(mDecel);
        }
    }
}
