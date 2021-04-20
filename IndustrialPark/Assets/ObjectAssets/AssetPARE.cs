using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EmitterType : byte
    {
        Point = 0,
        CircleEdge = 1,
        Circle = 2,
        RectEdge = 3,
        Rect = 4,
        Line = 5,
        Volume = 6,
        SphereEdge = 7,
        Sphere = 8,
        OffsetPoint = 9,
        SphereEdge10 = 10,
        SphereEdge11 = 11,
        VCylEdge = 12,
        OCircleEdge = 13,
        OCircle = 14,
        EntityBone = 15,
        EntityBound = 16,
    }

    public class AssetPARE : BaseAsset
    {
        private const string categoryName = "Particle Emitter";

        [Category(categoryName)]
        public FlagBitmask EmitterFlags { get; set; } = ByteFlagsDescriptor();
        private EmitterType _emitterType;
        [Category(categoryName)]
        public EmitterType EmitterType
        {
            get => _emitterType;
            set
            {
                _emitterType = value;
                switch (_emitterType)
                {
                    case EmitterType.CircleEdge:
                    case EmitterType.Circle:
                    case EmitterType.OCircleEdge:
                    case EmitterType.OCircle:
                        PareSpecific = new PareSpecific_xPECircle();
                        break;
                    case EmitterType.RectEdge:
                    case EmitterType.Rect:
                        PareSpecific = new PareSpecific_tagEmitRect();
                        break;
                    case EmitterType.Line:
                        PareSpecific = new PareSpecific_tagEmitLine();
                        break;
                    case EmitterType.Volume:
                        PareSpecific = new PareSpecific_tagEmitVolume();
                        break;
                    case EmitterType.SphereEdge:
                    case EmitterType.Sphere:
                    case EmitterType.SphereEdge10:
                    case EmitterType.SphereEdge11:
                        PareSpecific = new PareSpecific_tagEmitSphere();
                        break;
                    case EmitterType.OffsetPoint:
                        PareSpecific = new PareSpecific_tagEmitOffsetPoint();
                        break;
                    case EmitterType.VCylEdge:
                        PareSpecific = new PareSpecific_xPEVCyl();
                        break;
                    case EmitterType.EntityBone:
                        PareSpecific = new PareSpecific_xPEEntBone();
                        break;
                    case EmitterType.EntityBound:
                        PareSpecific = new PareSpecific_xPEEntBound();
                        break;
                    default:
                        PareSpecific = new PareSpecific_Generic();
                        break;
                }
            }
        }
        [Category(categoryName)]
        public AssetID PARP_AssetID { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public PareSpecific_Generic PareSpecific { get; set; }
        [Category(categoryName)]
        public AssetID Emitter_AssetID { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosX { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosY { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosZ { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_X { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_Y { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_Z { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_AngleVariation { get; set; }
        [Category(categoryName)]
        public int CullMode { get; set; }
        [Category(categoryName)]
        public AssetSingle CullDistanceSqr { get; set; }

        public AssetPARE(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseHeaderEndPosition;

            EmitterFlags.FlagValueByte = reader.ReadByte();
            _emitterType = (EmitterType)reader.ReadByte();
            reader.BaseStream.Position += 2;
            PARP_AssetID = reader.ReadUInt32();

            // should be at 0x10 now
            switch (_emitterType)
            {
                case EmitterType.CircleEdge:
                case EmitterType.Circle:
                case EmitterType.OCircleEdge:
                case EmitterType.OCircle:
                    PareSpecific = new PareSpecific_xPECircle(reader);
                    break;
                case EmitterType.RectEdge:
                case EmitterType.Rect:
                    PareSpecific = new PareSpecific_tagEmitRect(reader);
                    break;
                case EmitterType.Line:
                    PareSpecific = new PareSpecific_tagEmitLine(reader);
                    break;
                case EmitterType.Volume:
                    PareSpecific = new PareSpecific_tagEmitVolume(reader);
                    break;
                case EmitterType.SphereEdge:
                case EmitterType.Sphere:
                case EmitterType.SphereEdge10:
                case EmitterType.SphereEdge11:
                    PareSpecific = new PareSpecific_tagEmitSphere(reader);
                    break;
                case EmitterType.OffsetPoint:
                    PareSpecific = new PareSpecific_tagEmitOffsetPoint(reader);
                    break;
                case EmitterType.VCylEdge:
                    PareSpecific = new PareSpecific_xPEVCyl(reader);
                    break;
                case EmitterType.EntityBone:
                    PareSpecific = new PareSpecific_xPEEntBone(reader);
                    break;
                case EmitterType.EntityBound:
                    PareSpecific = new PareSpecific_xPEEntBound(reader);
                    break;
                default:
                    PareSpecific = new PareSpecific_Generic();
                    break;
            } // 0x1C in size

            reader.BaseStream.Position = 0x2C;
            Emitter_AssetID = reader.ReadUInt32();
            Emitter_PosX = reader.ReadSingle();
            Emitter_PosY = reader.ReadSingle();
            Emitter_PosZ = reader.ReadSingle();
            Velocity_X = reader.ReadSingle();
            Velocity_Y = reader.ReadSingle();
            Velocity_Z = reader.ReadSingle();
            Velocity_AngleVariation = reader.ReadSingle();
            CullMode = reader.ReadInt32();
            CullDistanceSqr = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(EmitterFlags.FlagValueByte);
            writer.Write((byte)_emitterType);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(PARP_AssetID);
            writer.Write(PareSpecific.Serialize(game, platform));
            while (writer.BaseStream.Length < 0x2C)
                writer.Write((byte)0);
            writer.Write(Emitter_AssetID);
            writer.Write(Emitter_PosX);
            writer.Write(Emitter_PosY);
            writer.Write(Emitter_PosZ);
            writer.Write(Velocity_X);
            writer.Write(Velocity_Y);
            writer.Write(Velocity_Z);
            writer.Write(Velocity_AngleVariation);
            writer.Write(CullMode);
            writer.Write(CullDistanceSqr);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            PareSpecific.HasReference(assetID) || PARP_AssetID == assetID || Emitter_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            PareSpecific.Verify(ref result);

            if (PARP_AssetID == 0)
                result.Add("PARE with PARP_AssetID set to 0");
            Verify(PARP_AssetID, ref result);
            Verify(Emitter_AssetID, ref result);
        }
    }
}