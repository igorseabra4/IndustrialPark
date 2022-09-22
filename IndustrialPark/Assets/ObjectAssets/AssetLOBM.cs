using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetLOBM : BaseAsset
    {
        private const string categoryName = "LobMaster";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Projectile);

        [Category(categoryName)]
        public AssetID LobMasterType { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID Projectile { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionX { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionY { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionZ { get; set; }
        [Category(categoryName)]
        public AssetSingle RotationX { get; set; }
        [Category(categoryName)]
        public AssetSingle RotationY { get; set; }
        [Category(categoryName)]
        public AssetSingle RotationZ { get; set; }
        [Category(categoryName)]
        public AssetSingle LaunchSpeed { get; set; }
        [Category(categoryName)]
        public AssetSingle SpeedRandomPct { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleX { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleY { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleZ { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle MaxLifetime { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxDistance { get; set; }
        [Category(categoryName)]
        public AssetID Aid_MovePoint { get; set; }
        [Category(categoryName)]
        public int SalvoCount { get; set; }
        [Category(categoryName)]
        public int AmmoCount { get; set; }
        [Category(categoryName)]
        public AssetSingle ArcCoeffFactor { get; set; }
        [Category(categoryName)]
        public int DebrisConeAngle { get; set; }
        [Category(categoryName)]
        public int NumBounce { get; set; }
        [Category(categoryName)]
        public int PowerupType { get; set; }
        [Category(categoryName)]
        public AssetSingle HeavyFactor { get; set; }
        [Category(categoryName)]
        public AssetSingle TumbleRotationX { get; set; }
        [Category(categoryName)]
        public AssetSingle TumbleRotationY { get; set; }
        [Category(categoryName)]
        public AssetSingle TumbleRotationZ { get; set; }
        [Category(categoryName)]
        public AssetSingle CollideDelay { get; set; }
        [Category(categoryName)]
        public AssetSingle AtRestPeriod { get; set; }
        [Category(categoryName)]
        public int Mode { get; set; }

        public AssetLOBM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                LobMasterType = reader.ReadUInt32();
                Projectile = reader.ReadUInt32();
                PositionX = reader.ReadSingle();
                PositionY = reader.ReadSingle();
                PositionZ = reader.ReadSingle();
                RotationX = reader.ReadSingle();
                RotationY = reader.ReadSingle();
                RotationZ = reader.ReadSingle();
                LaunchSpeed = reader.ReadSingle();
                SpeedRandomPct = reader.ReadSingle();
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                ScaleZ = reader.ReadSingle();
                Flags.FlagValueInt = reader.ReadUInt32();
                MaxLifetime = reader.ReadSingle();
                MaxDistance = reader.ReadSingle();
                Aid_MovePoint = reader.ReadUInt32();
                SalvoCount = reader.ReadInt32();
                AmmoCount = reader.ReadInt32();
                ArcCoeffFactor = reader.ReadSingle();
                DebrisConeAngle = reader.ReadInt32();
                NumBounce = reader.ReadInt32();
                PowerupType = reader.ReadInt32();
                HeavyFactor = reader.ReadSingle();
                TumbleRotationX = reader.ReadSingle();
                TumbleRotationY = reader.ReadSingle();
                TumbleRotationZ = reader.ReadSingle();
                CollideDelay = reader.ReadSingle();
                AtRestPeriod = reader.ReadSingle();
                Mode = reader.ReadInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

                base.Serialize(writer);
                writer.Write(LobMasterType);
                writer.Write(Projectile);
                writer.Write(PositionX);
                writer.Write(PositionY);
                writer.Write(PositionZ);
                writer.Write(RotationX);
                writer.Write(RotationY);
                writer.Write(RotationZ);
                writer.Write(LaunchSpeed);
                writer.Write(SpeedRandomPct);
                writer.Write(ScaleX);
                writer.Write(ScaleY);
                writer.Write(ScaleZ);
                writer.Write(Flags.FlagValueInt);
                writer.Write(MaxLifetime);
                writer.Write(MaxDistance);
                writer.Write(Aid_MovePoint);
                writer.Write(SalvoCount);
                writer.Write(AmmoCount);
                writer.Write(ArcCoeffFactor);
                writer.Write(DebrisConeAngle);
                writer.Write(NumBounce);
                writer.Write(PowerupType);
                writer.Write(HeavyFactor);
                writer.Write(TumbleRotationX);
                writer.Write(TumbleRotationY);
                writer.Write(TumbleRotationZ);
                writer.Write(CollideDelay);
                writer.Write(AtRestPeriod);
                writer.Write(Mode);

                SerializeLinks(writer);

                
        }
    }
}