using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectTrainCar : DynaEnemy
    {
        private const string dynaCategoryName = "game_object:train_car";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Model);
        protected override short constVersion => 7;

        [Category(dynaCategoryName)]
        public AssetID ParentCar { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartSpline { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitialU { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FrontAxleDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RearAxleDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FrontHitchDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RearHitchDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AxleWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte HaveSparks { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte IsDestructible { get; set; }
        [Category(dynaCategoryName)]
        public AssetID NavMesh { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte NavMeshGroupIndex { get; set; }

        public DynaGObjectTrainCar(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__train_car, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                ParentCar = reader.ReadUInt32();
                StartSpline = reader.ReadUInt32();
                InitialU = reader.ReadSingle();
                FrontAxleDist = reader.ReadSingle();
                RearAxleDist = reader.ReadSingle();
                FrontHitchDist = reader.ReadSingle();
                RearHitchDist = reader.ReadSingle();
                AxleWidth = reader.ReadSingle();
                HaveSparks = reader.ReadByte();
                IsDestructible = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                NavMesh = reader.ReadUInt32();
                NavMeshGroupIndex = reader.ReadByte();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(ParentCar);
            writer.Write(StartSpline);
            writer.Write(InitialU);
            writer.Write(FrontAxleDist);
            writer.Write(RearAxleDist);
            writer.Write(FrontHitchDist);
            writer.Write(RearHitchDist);
            writer.Write(AxleWidth);
            writer.Write(HaveSparks);
            writer.Write(IsDestructible);
            writer.Write((short)0);
            writer.Write(NavMesh);
            writer.Write(NavMeshGroupIndex);
            writer.Write((byte)0);
            writer.Write((short)0);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}