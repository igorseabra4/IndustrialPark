using HipHopFile;
using RenderWareFile;
using SharpDX;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetJSP_INFO : Asset
    {
        private const string categoryName = "JSP Info";

        public int renderWareVersion;

        [Category(categoryName)]
        public AssetID[] JSP_AssetIDs { get; set; }

        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public HI_Tags_BEEF01 Section1 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public HI_Tags_BEEF02 Section2 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public HI_Tags_BEEF03 Section3 { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public HI_Tags_BEEF04 Section4 { get; set; }

        public AssetJSP_INFO(Section_AHDR AHDR, Game game, Endianness endianness, AssetJSP[] jspAssets) : base(AHDR, game)
        {
            JSP_AssetIDs = jspAssets.Select(j => (AssetID)j.assetID).ToArray();

            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Little))
            {
                Section1 = new HI_Tags_BEEF01(reader, game);

                renderWareVersion = Section1.RenderWareVersion;

                Section2 = new HI_Tags_BEEF02(reader, endianness);
                int done = 0;
                if (jspAssets.Sum(j => j.AtomicFlags.Length) == Section2.jspNodeList.Length)
                {
                    foreach (AssetJSP jsp in jspAssets.Reverse())
                    {
                        xJSPNodeInfo[] entries = new xJSPNodeInfo[jsp.AtomicFlags.Length];
                        for (int i = 0; i < entries.Length; i++)
                            entries[i] = Section2.jspNodeList[done + i];
                        done += entries.Length;
                        ArchiveEditorFunctions.AddToJspNodeInfo(jsp.assetID, entries.Reverse().ToArray());
                    }
                }

                if (!reader.EndOfStream && reader.PeekUInt32() == (int)RenderWareFile.Section.HI_TAGS_BEEF03)
                    Section3 = new HI_Tags_BEEF03(reader);

                if (!reader.EndOfStream && reader.PeekUInt32() == (int)RenderWareFile.Section.HI_TAGS_BEEF04)
                    Section4 = new HI_Tags_BEEF04(reader, endianness);
            }
        }

        public AssetJSP_INFO(string assetName, Game game, Platform platform) : base(assetName, AssetType.JSPInfo)
        {
            _game = game;
            Section1 = new HI_Tags_BEEF01(game);
            Section2 = new HI_Tags_BEEF02();

            if (game == Game.BFBB && platform == Platform.GameCube)
                Section3 = new HI_Tags_BEEF03();

            if (game >= Game.Incredibles)
                Section4 = new HI_Tags_BEEF04(platform);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (Section3 == null)
                dt.RemoveProperty("Section3");
            if (Section4 == null)
                dt.RemoveProperty("Section4");
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            Section1.Serialize(writer);
            Section2.Serialize(writer);
            Section3?.Serialize(writer);
            Section4?.Serialize(writer);
        }

        public void ApplyScale(Vector3 factor)
        {
            for (int i = 0; i < Section1.branchNodes.Length; i++)
            {
                switch (Section1.branchNodes[i].LeftAxis)
                {
                    case ClumpAxis.X:
                        Section1.branchNodes[i].LeftValue *= factor.X;
                        Section1.branchNodes[i].RightValue *= factor.X;
                        break;
                    case ClumpAxis.Y:
                        Section1.branchNodes[i].LeftValue *= factor.Y;
                        Section1.branchNodes[i].RightValue *= factor.Y;
                        break;
                    default:
                        Section1.branchNodes[i].LeftValue *= factor.Z;
                        Section1.branchNodes[i].RightValue *= factor.Z;
                        break;
                }
            }

            if (Section2.Version == 5)
            {
                for (int i = 0; i < Section2.branchNodes.Length; i++)
                {
                    switch (Section2.branchNodes[i].coord)
                    {
                        case 0:
                            Section2.branchNodes[i].leftValue *= factor.X;
                            Section2.branchNodes[i].rightValue *= factor.X;
                            break;
                        case 4:
                            Section2.branchNodes[i].leftValue *= factor.Y;
                            Section2.branchNodes[i].rightValue *= factor.Y;
                            break;
                        case 8:
                            Section2.branchNodes[i].leftValue *= factor.Z;
                            Section2.branchNodes[i].rightValue *= factor.Z;
                            break;
                    }
                }
            }

            for (int i = 0; i < Section3.vertexList.Length; i++)
            {
                Section3.vertexList[i].X *= factor.X;
                Section3.vertexList[i].Y *= factor.Y;
                Section3.vertexList[i].Z *= factor.Z;
            }
        }

        public override bool HasReference(uint assetID)
        {
            return JSP_AssetIDs.Contains(assetID);
        }
    }
}