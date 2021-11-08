using AssetEditorColors;
using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectWaterBody : AssetDYNA
    {
        private const string dynaCategoryName = "effect:water_body";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public FlagBitmask WaterBodyFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID MotionType { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Body { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FacadeRefract { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FacadeReflect { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightDir { get; set; }

        public DynaEffectWaterBody(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__water_body, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                WaterBodyFlags.FlagValueInt = reader.ReadUInt32();
                MotionType = reader.ReadUInt32();
                Body = reader.ReadUInt32();
                FacadeRefract = reader.ReadUInt32();
                FacadeReflect = reader.ReadUInt32();
                LightDir = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(WaterBodyFlags.FlagValueInt);
                writer.Write(MotionType);
                writer.Write(Body);
                writer.Write(FacadeRefract);
                writer.Write(FacadeReflect);
                writer.Write(LightDir);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) =>
            MotionType == assetID || Body == assetID || FacadeRefract == assetID || FacadeReflect == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(MotionType, ref result);
            Verify(Body, ref result);
            Verify(FacadeRefract, ref result);
            Verify(FacadeReflect, ref result);

            base.Verify(ref result);
        }
    }
}