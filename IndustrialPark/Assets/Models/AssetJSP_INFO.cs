using HipHopFile;
using RenderWareFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetJSP_INFO : Asset
    {
        private const string categoryName = "JSP Info";

        public int renderWareVersion;

        [Category(categoryName)]
        public AssetID[] JSP_AssetIDs { get; set; }

        [Category(categoryName)]
        public Platform Platform { get; set; }

        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public BFBB_CollisionData_Section1_00BEEF01 Section1 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public BFBB_CollisionData_Section2_00BEEF02 Section2 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericSection Section2_Data { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public BFBB_CollisionData_Section3_00BEEF03 Section3 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericSection Section4 { get; set; }

        public AssetJSP_INFO(Section_AHDR AHDR, Game game, Platform platform, AssetID[] jspAssetIds) : base(AHDR, game)
        {
            Platform = platform;
            JSP_AssetIDs = jspAssetIds;

            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Little))
            {
                Section1 = new BFBB_CollisionData_Section1_00BEEF01(reader, platform);

                renderWareVersion = Section1.RenderWareVersion;

                if (game == Game.BFBB)
                    Section2 = new BFBB_CollisionData_Section2_00BEEF02(reader, platform);
                else
                {
                    var currentSection = (RenderWareFile.Section)reader.ReadInt32();
                    Section2_Data = new GenericSection().Read(reader, currentSection);
                }

                if (game == Game.BFBB && Platform == Platform.GameCube)
                {
                    Section3 = new BFBB_CollisionData_Section3_00BEEF03(reader);
                }

                if (game != Game.BFBB)
                {
                    var currentSection = (RenderWareFile.Section)reader.ReadInt32();
                    Section4 = new GenericSection().Read(reader, currentSection);
                }
            }
        }

        public AssetJSP_INFO(string assetName, Game game, Platform platform) : base(assetName, AssetType.JSPInfo)
        {
            _game = game;
            Platform = platform;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            Section1.RenderWareVersion = renderWareVersion;
            Section1.Serialize(writer);

            if (game == Game.BFBB)
            {
                Section2.RenderWareVersion = renderWareVersion;
                Section2.Serialize(writer);
            }
            else
                writer.Write(Section2_Data.GetBytes(renderWareVersion));

            if (game == Game.BFBB && Platform == Platform.GameCube)
            {
                Section3.RenderWareVersion = renderWareVersion;
                Section3.Serialize(writer);
            }

            if (game != Game.BFBB)
                writer.Write(Section4.GetBytes(renderWareVersion));
        }

        public void ApplyScale(Vector3 factor)
        {
            for (int i = 0; i < Section1.branchNodes.Length; i++)
            {
                switch (Section1.branchNodes[i].LeftDirection)
                {
                    case ClumpDirection.X:
                        Section1.branchNodes[i].LeftValue *= factor.X;
                        Section1.branchNodes[i].RightValue *= factor.X;
                        break;
                    case ClumpDirection.Y:
                        Section1.branchNodes[i].LeftValue *= factor.Y;
                        Section1.branchNodes[i].RightValue *= factor.Y;
                        break;
                    default:
                        Section1.branchNodes[i].LeftValue *= factor.Z;
                        Section1.branchNodes[i].RightValue *= factor.Z;
                        break;
                }
            }

            for (int i = 0; i < Section3.vertexList.Length; i++)
            {
                Section3.vertexList[i].X *= factor.X;
                Section3.vertexList[i].Y *= factor.Y;
                Section3.vertexList[i].Z *= factor.Z;
            }
        }

        public void CreateFromJsp(AssetJSP assetJSP)
        {
            JSP_AssetIDs = new AssetID[] { assetJSP.assetID };

            var clump = assetJSP.GetClump();
            renderWareVersion = clump.renderWareVersion;

            Section1 = new BFBB_CollisionData_Section1_00BEEF01(Platform);

            Section2 = new BFBB_CollisionData_Section2_00BEEF02(Platform);

            Section2_Data = null;

            Section3 = new BFBB_CollisionData_Section3_00BEEF03(clump.geometryList.geometryList);

            Section4 = null;
        }
    }
}