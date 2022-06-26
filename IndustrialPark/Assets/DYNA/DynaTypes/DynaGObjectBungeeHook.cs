using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBungeeHook : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:bungee_hook";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Entity);

        protected override short constVersion => 13;

        [Category(dynaCategoryName)]
        public AssetID Entity { get; set; }
        [Category(dynaCategoryName)]
        public int EnterX { get; set; }
        [Category(dynaCategoryName)]
        public int EnterY { get; set; }
        [Category(dynaCategoryName)]
        public int EnterZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AttachDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AttachTravelTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DetachDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DetachFreeFallTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DetachAccel { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TurnUnused1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TurnUnused2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalFrequency { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalGravity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalDive { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalMinDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalMaxDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VerticalDamp { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HorizontalMaxDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraRestDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Cameraview_angle { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraOffset { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraOffsetDir { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraTurnSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraVelScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraRollSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraUnused1_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraUnused1_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CameraUnused1_Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CollisionHitLoss { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CollisionDamageVelocity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CollisionHitVelocity { get; set; }

        public DynaGObjectBungeeHook(string assetName, uint entityAssetID) : base(assetName, DynaType.game_object__bungee_hook, 13)
        {
            Entity = entityAssetID;

            AttachDist = 3;
            AttachTravelTime = 0.5f;
            DetachDist = 10;
            DetachFreeFallTime = 1;
            DetachAccel = 2;
            TurnUnused1 = 25;
            TurnUnused2 = 0.95f;
            VerticalFrequency = 2;
            VerticalGravity = 9.8f;
            VerticalDive = 2;
            VerticalMinDist = 2;
            VerticalMaxDist = 40;
            VerticalDamp = 0.05f;
            HorizontalMaxDist = 2;
            CameraRestDist = 5;
            Cameraview_angle = 220;
            CameraOffset = 0.5f;
            CameraOffsetDir = 180;
            CameraTurnSpeed = 0.05f;
            CameraVelScale = 0;
            CameraRollSpeed = 0.05f;
            CameraUnused1_X = 0.2f;
            CameraUnused1_Y = 0.25f;
            CameraUnused1_Z = 0.2f;
            CollisionHitLoss = 0.1f;
            CollisionDamageVelocity = 0.6f;
            CollisionHitVelocity = 0.2f;
        }

        public DynaGObjectBungeeHook(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__bungee_hook, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Entity = reader.ReadUInt32();
                EnterX = reader.ReadInt32();
                EnterY = reader.ReadInt32();
                EnterZ = reader.ReadInt32();
                AttachDist = reader.ReadSingle();
                AttachTravelTime = reader.ReadSingle();
                DetachDist = reader.ReadSingle();
                DetachFreeFallTime = reader.ReadSingle();
                DetachAccel = reader.ReadSingle();
                TurnUnused1 = reader.ReadSingle();
                TurnUnused2 = reader.ReadSingle();
                VerticalFrequency = reader.ReadSingle();
                VerticalGravity = reader.ReadSingle();
                VerticalDive = reader.ReadSingle();
                VerticalMinDist = reader.ReadSingle();
                VerticalMaxDist = reader.ReadSingle();
                VerticalDamp = reader.ReadSingle();
                HorizontalMaxDist = reader.ReadSingle();
                CameraRestDist = reader.ReadSingle();
                Cameraview_angle = reader.ReadSingle();
                CameraOffset = reader.ReadSingle();
                CameraOffsetDir = reader.ReadSingle();
                CameraTurnSpeed = reader.ReadSingle();
                CameraVelScale = reader.ReadSingle();
                CameraRollSpeed = reader.ReadSingle();
                CameraUnused1_X = reader.ReadSingle();
                CameraUnused1_Y = reader.ReadSingle();
                CameraUnused1_Z = reader.ReadSingle();
                CollisionHitLoss = reader.ReadSingle();
                CollisionDamageVelocity = reader.ReadSingle();
                CollisionHitVelocity = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Entity);
                writer.Write(EnterX);
                writer.Write(EnterY);
                writer.Write(EnterZ);
                writer.Write(AttachDist);
                writer.Write(AttachTravelTime);
                writer.Write(DetachDist);
                writer.Write(DetachFreeFallTime);
                writer.Write(DetachAccel);
                writer.Write(TurnUnused1);
                writer.Write(TurnUnused2);
                writer.Write(VerticalFrequency);
                writer.Write(VerticalGravity);
                writer.Write(VerticalDive);
                writer.Write(VerticalMinDist);
                writer.Write(VerticalMaxDist);
                writer.Write(VerticalDamp);
                writer.Write(HorizontalMaxDist);
                writer.Write(CameraRestDist);
                writer.Write(Cameraview_angle);
                writer.Write(CameraOffset);
                writer.Write(CameraOffsetDir);
                writer.Write(CameraTurnSpeed);
                writer.Write(CameraVelScale);
                writer.Write(CameraRollSpeed);
                writer.Write(CameraUnused1_X);
                writer.Write(CameraUnused1_Y);
                writer.Write(CameraUnused1_Z);
                writer.Write(CollisionHitLoss);
                writer.Write(CollisionDamageVelocity);
                writer.Write(CollisionHitVelocity);

                return writer.ToArray();
            }
        }
        
        public override void Verify(ref List<string> result)
        {
            Verify(Entity, ref result);

            base.Verify(ref result);
        }
    }
}