﻿using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBungeeDrop : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:bungee_drop";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Marker);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Marker { get; set; }
        [Category(dynaCategoryName)]
        public int SetViewAngle { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ViewAngle { get; set; }

        public DynaGObjectBungeeDrop(string assetName, uint mrkrAssetId) : base(assetName, DynaType.game_object__bungee_drop)
        {
            Marker = mrkrAssetId;
            SetViewAngle = 1;
        }

        public DynaGObjectBungeeDrop(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__bungee_drop, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Marker = reader.ReadUInt32();
                SetViewAngle = reader.ReadInt32();
                ViewAngle = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(Marker);
            writer.Write(SetViewAngle);
            writer.Write(ViewAngle);


        }
    }
}