using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaCameraBinary : AssetDYNA
    {
        private const string dynaCategoryName = "camera:binary_poi";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(mTargetID);
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public ushort mVersion { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask mFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID mTargetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mTargetMarginAngle { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mTargetRadiusScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mDistanceScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mPhiOffset { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mYawOffset { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mNearWallScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle mFarWallScale { get; set; }

        public DynaCameraBinary(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.camera__binary_poi, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                mVersion = reader.ReadUInt16();
                mFlags.FlagValueShort = reader.ReadUInt16();
                mTargetID = reader.ReadUInt32();
                mTargetMarginAngle = reader.ReadSingle();
                mTargetRadiusScale = reader.ReadSingle();
                mDistanceScale = reader.ReadSingle();
                mPhiOffset = reader.ReadSingle();
                mYawOffset = reader.ReadSingle();
                mNearWallScale = reader.ReadSingle();
                mFarWallScale = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(mVersion);
            writer.Write(mFlags.FlagValueShort);
            writer.Write(mTargetID);
            writer.Write(mTargetMarginAngle);
            writer.Write(mTargetRadiusScale);
            writer.Write(mDistanceScale);
            writer.Write(mPhiOffset);
            writer.Write(mYawOffset);
            writer.Write(mNearWallScale);
            writer.Write(mFarWallScale);
        }
    }
}