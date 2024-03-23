using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaSceneProperties : AssetDYNA
    {
        private const string dynaCategoryName = "Scene Properties";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID[] idle03Extras { get; set; }
        [Category(dynaCategoryName)]
        public AssetID[] idle04Extras { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte bombCount { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte extraIdleDelay { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte hdrGlow { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte hdrDarken { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackgroundMusic { get; set; }
        [Category(dynaCategoryName)]
        public int scenePropertiesFlags { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle waterTileWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle lodFadeDistance { get; set; }

        private const string incrediblesCategory = dynaCategoryName + " (Incredibles version only)";

        [Category(incrediblesCategory)]
        public AssetSingle WaterTileOffsetX { get; set; }
        [Category(incrediblesCategory)]
        public AssetSingle WaterTileOffsetY { get; set; }
        [Category(incrediblesCategory)]
        public AssetByte NumCheckpoints { get; set; }
        [Category(incrediblesCategory)]
        public AssetSingle GrassDistFade { get; set; }
        [Category(incrediblesCategory)]
        public AssetSingle GrassDistCull { get; set; }
        [Category(incrediblesCategory)]
        public uint PiggyBank { get; set; }
        [Category(incrediblesCategory)]
        public uint MaxAnimationMem { get; set; }
        [Category(incrediblesCategory)]
        public uint MaxArtMem { get; set; }
        [Category(incrediblesCategory)]
        public uint MaxDesignMem { get; set; }
        [Category(incrediblesCategory)]
        public uint MaxProgrammingMem { get; set; }


        [Category(dynaCategoryName)]
        public EVersionIncrediblesOthers AssetVersion { get; set; } = EVersionIncrediblesOthers.Others;

        public DynaSceneProperties(string assetName) : base(assetName, DynaType.SceneProperties)
        {
            idle03Extras = new AssetID[0];
            idle04Extras = new AssetID[0];
            bombCount = 0x14;
            extraIdleDelay = 0x05;
            hdrGlow = 0x03;
            hdrDarken = 0x1E;
            BackgroundMusic = 0;
            scenePropertiesFlags = 1;
            waterTileWidth = 0;
            lodFadeDistance = 4;
        }

        public DynaSceneProperties(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.SceneProperties, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                idle03Extras = new AssetID[reader.ReadInt32()];
                var idle03ExtraOffset = reader.ReadInt32() + 0x10;
                idle04Extras = new AssetID[reader.ReadInt32()];
                var idle04ExtraOffset = reader.ReadInt32() + 0x10;

                bombCount = reader.ReadByte();
                extraIdleDelay = reader.ReadByte();
                hdrGlow = reader.ReadByte();
                hdrDarken = reader.ReadByte();
                BackgroundMusic = reader.ReadUInt32();
                scenePropertiesFlags = reader.ReadInt32();
                waterTileWidth = reader.ReadSingle();
                lodFadeDistance = reader.ReadSingle();

                if (reader.BaseStream.Length > 0x44)
                {
                    AssetVersion = EVersionIncrediblesOthers.Incredibles;
                    WaterTileOffsetX = reader.ReadSingle();
                    WaterTileOffsetY = reader.ReadSingle();
                    NumCheckpoints = reader.ReadByte();
                    reader.ReadBytes(3);
                    GrassDistFade = reader.ReadSingle();
                    GrassDistCull = reader.ReadSingle();
                    PiggyBank = reader.ReadUInt32();
                    MaxAnimationMem = reader.ReadUInt32();
                    MaxArtMem = reader.ReadUInt32();
                    MaxDesignMem = reader.ReadUInt32();
                    MaxProgrammingMem = reader.ReadUInt32();
                }
                else
                {
                    AssetVersion = EVersionIncrediblesOthers.Others;
                }

                reader.BaseStream.Position = idle03ExtraOffset;
                for (int i = 0; i < idle03Extras.Length; i++)
                    idle03Extras[i] = reader.ReadUInt32();
                reader.BaseStream.Position = idle04ExtraOffset;
                for (int i = 0; i < idle04Extras.Length; i++)
                    idle04Extras[i] = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            var idlesStart = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(bombCount);
            writer.Write(extraIdleDelay);
            writer.Write(hdrGlow);
            writer.Write(hdrDarken);
            writer.Write(BackgroundMusic);
            writer.Write(scenePropertiesFlags);
            writer.Write(waterTileWidth);
            writer.Write(lodFadeDistance);
            if (AssetVersion == EVersionIncrediblesOthers.Incredibles)
            {
                writer.Write(WaterTileOffsetX);
                writer.Write(WaterTileOffsetY);
                writer.Write(NumCheckpoints);
                writer.Write(new byte[3]);
                writer.Write(GrassDistFade);
                writer.Write(GrassDistCull);
                writer.Write(PiggyBank);
                writer.Write(MaxAnimationMem);
                writer.Write(MaxArtMem);
                writer.Write(MaxDesignMem);
                writer.Write(MaxProgrammingMem);
                writer.Write(new byte[12]);
            }
            else
            {
                writer.Write(new byte[16]);
            }

            var idle03Pos = (int)(writer.BaseStream.Position - idlesStart);
            foreach (var u in idle03Extras)
                writer.Write(u);

            var idle04Pos = (int)(writer.BaseStream.Position - idlesStart);
            foreach (var u in idle04Extras)
                writer.Write(u);

            var dynaDataEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = idlesStart;

            writer.Write(idle03Extras.Length);
            writer.Write(idle03Pos);
            writer.Write(idle04Extras.Length);
            writer.Write(idle04Pos);

            writer.BaseStream.Position = dynaDataEnd;
        }
    }
}