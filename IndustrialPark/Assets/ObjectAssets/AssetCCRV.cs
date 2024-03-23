using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class CameraCurveBear : GenericAssetDataContainer
    {
        public AssetSingle mCurveU { get; set; }
        public AssetSingle mCurveU2 { get; set; }
        public AssetSingle mDistanceAdjust { get; set; }
        public AssetSingle mPitchOffset { get; set; }
        public AssetSingle mTargetRadius { get; set; }
        public AssetSingle mTargetMarginAngle { get; set; }
        public AssetSingle mLeadOffset { get; set; }
        public AssetSingle mYOffset { get; set; }
        public AssetSingle mNearWallAdjust { get; set; }
        public AssetSingle mFarWallAdjust { get; set; }

        public CameraCurveBear() { }
        public CameraCurveBear(EndianBinaryReader reader)
        {
            mCurveU = reader.ReadSingle();
            mCurveU2 = reader.ReadSingle();
            mDistanceAdjust = reader.ReadSingle();
            mPitchOffset = reader.ReadSingle();
            mTargetRadius = reader.ReadSingle();
            mTargetMarginAngle = reader.ReadSingle();
            mLeadOffset = reader.ReadSingle();
            mYOffset = reader.ReadSingle();
            mNearWallAdjust = reader.ReadSingle();
            mFarWallAdjust = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(mCurveU);
            writer.Write(mCurveU2);
            writer.Write(mDistanceAdjust);
            writer.Write(mPitchOffset);
            writer.Write(mTargetRadius);
            writer.Write(mTargetMarginAngle);
            writer.Write(mLeadOffset);
            writer.Write(mYOffset);
            writer.Write(mNearWallAdjust);
            writer.Write(mFarWallAdjust);
        }
    }
    public class AssetCCRV : BaseAsset
    {
        public override string AssetInfo => $"{HexUIntTypeConverter.StringFromAssetID(mCurveID)}";

        public AssetByte mVersion { get; set; }
        public int mCameraType { get; set; }
        public FlagBitmask mFlags { get; set; } = IntFlagsDescriptor();
        public int mTransitionType { get; set; }
        public AssetSingle mTransitionTime { get; set; }
        public AssetID mCurveID { get; set; }
        public AssetID mCurveID2 { get; set; }
        public CameraCurveBear[] CurveBeads { get; set; }

        public AssetCCRV(string assetName) : base(assetName, AssetType.CameraCurve, BaseAssetType.CameraCurve)
        {
            mVersion = 4;
            CurveBeads = new CameraCurveBear[0];
        }
        public AssetCCRV(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;
                mVersion = reader.ReadByte();
                reader.BaseStream.Position += 3;
                mCameraType = reader.ReadInt32();
                mFlags.FlagValueInt = reader.ReadUInt32();
                mTransitionType = reader.ReadInt32();
                mTransitionTime = reader.ReadSingle();
                mCurveID = reader.ReadUInt32();
                mCurveID2 = reader.ReadUInt32();
                int numBeads = reader.ReadInt32();

                CurveBeads = new CameraCurveBear[numBeads];
                for (int i = 0; i < numBeads; i++)
                    CurveBeads[i] = new CameraCurveBear(reader);
            }
        }
        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(mVersion);
            writer.Write(new byte[3]);
            writer.Write(mCameraType);
            writer.Write(mFlags.FlagValueInt);
            writer.Write(mTransitionType);
            writer.Write(mTransitionTime);
            writer.Write(mCurveID);
            writer.Write(mCurveID2);
            writer.Write(CurveBeads.Length);
            foreach (var curve in CurveBeads)
                curve.Serialize(writer);
        }
    }
}
