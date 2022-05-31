using AssetEditorColors;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTurret : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Turret";

        protected override short constVersion => 7;

        [Category(dynaCategoryName)]
        public AssetID BaseObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetID GunObject { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle YawRange { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle YawSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PitchRange { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PitchSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RecoveryTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OverheatFraction { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CoolingSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OverheatTime { get; set; }
        [Category(dynaCategoryName)]
        public short HitPoints { get; set; }
        [Category(dynaCategoryName)]
        public short Damage { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraOffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraOffsetY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraOffsetZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FinalPointer { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor LaserColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset1X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset1Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset1Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset2X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset2Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Offset2Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LaserLength { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LaserThickness { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LaserSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LaserSoundGroup { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TargetTexture { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TargetTextureSizeX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TargetTextureSizeY { get; set; }

        public DynaGObjectTurret(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Turret, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                BaseObject = reader.ReadUInt32();
                GunObject = reader.ReadUInt32();
                YawRange = reader.ReadSingle();
                YawSpeed = reader.ReadSingle();
                PitchRange = reader.ReadSingle();
                PitchSpeed = reader.ReadSingle();
                RecoveryTime = reader.ReadSingle();
                OverheatFraction = reader.ReadSingle();
                CoolingSpeed = reader.ReadSingle();
                OverheatTime = reader.ReadSingle();
                HitPoints = reader.ReadInt16();
                Damage = reader.ReadInt16();
                CameraOffsetX = reader.ReadSingle();
                CameraOffsetY = reader.ReadSingle();
                CameraOffsetZ = reader.ReadSingle();
                FinalPointer = reader.ReadUInt32();
                LaserColor = reader.ReadColor();
                Offset1X = reader.ReadSingle();
                Offset1Y = reader.ReadSingle();
                Offset1Z = reader.ReadSingle();
                Offset2X = reader.ReadSingle();
                Offset2Y = reader.ReadSingle();
                Offset2Z = reader.ReadSingle();
                LaserLength = reader.ReadSingle();
                LaserThickness = reader.ReadSingle();
                LaserSpeed = reader.ReadSingle();
                LaserSoundGroup = reader.ReadUInt32();
                TargetTexture = reader.ReadUInt32();
                TargetTextureSizeX = reader.ReadSingle();
                TargetTextureSizeY = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(BaseObject);
                writer.Write(GunObject);
                writer.Write(YawRange);
                writer.Write(YawSpeed);
                writer.Write(PitchRange);
                writer.Write(PitchSpeed);
                writer.Write(RecoveryTime);
                writer.Write(OverheatFraction);
                writer.Write(CoolingSpeed);
                writer.Write(OverheatTime);
                writer.Write(HitPoints);
                writer.Write(Damage);
                writer.Write(CameraOffsetX);
                writer.Write(CameraOffsetY);
                writer.Write(CameraOffsetZ);
                writer.Write(FinalPointer);
                writer.Write(LaserColor);
                writer.Write(Offset1X);
                writer.Write(Offset1Y);
                writer.Write(Offset1Z);
                writer.Write(Offset2X);
                writer.Write(Offset2Y);
                writer.Write(Offset2Z);
                writer.Write(LaserLength);
                writer.Write(LaserThickness);
                writer.Write(LaserSpeed);
                writer.Write(LaserSoundGroup);
                writer.Write(TargetTexture);
                writer.Write(TargetTextureSizeX);
                writer.Write(TargetTextureSizeY);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(BaseObject, ref result);
            Verify(GunObject, ref result);
            Verify(FinalPointer, ref result);
            Verify(LaserSoundGroup, ref result);
            Verify(TargetTexture, ref result);
            base.Verify(ref result);
        }
    }
}