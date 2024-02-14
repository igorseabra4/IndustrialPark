using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class DynaEnemyRATSThief : DynaEnemyRATS
    {
        private const string dynaCategoryName = "Enemy:RATS:Thief";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public AssetID FirstMovePoint { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Attractor { get; set; }
        [Category(dynaCategoryName)]
        public AssetID HideCirclesGroup { get; set; }

        public DynaEnemyRATSThief(string assetName, Vector3 position, uint mvptAssetID) : base(assetName, DynaType.Enemy__RATS__Thief, position)
        {
            FirstMovePoint = mvptAssetID;
        }

        public DynaEnemyRATSThief(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__RATS__Thief, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = enemyRatsEndPosition;

                FirstMovePoint = reader.ReadUInt32();
                Attractor = reader.ReadUInt32();
                HideCirclesGroup = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaEnemyRATS(writer);
            writer.Write(FirstMovePoint);
            writer.Write(Attractor);
            writer.Write(HideCirclesGroup);
        }


        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}