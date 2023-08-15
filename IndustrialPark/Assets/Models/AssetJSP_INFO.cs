using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;

namespace IndustrialPark
{
    public class AssetJSP_INFO : AssetWithData
    {
        public AssetJSP_INFO(Section_AHDR AHDR, Game game, AssetID[] jspAssetIds) : base(AHDR, game)
        {
            JSP_AssetIDs = jspAssetIds;
        }

        public int renderWareVersion;

        public RWSection[] File
        {
            get
            {
                RWSection[] data = ReadFileMethods.ReadRenderWareFile(Data);
                renderWareVersion = data[0].renderWareVersion;
                return data;
            }
            set => Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
        }

        public AssetID[] JSP_AssetIDs { get; set; }

        public void ApplyScale(Vector3 factor)
        {
            var sections = File;
            foreach (var s in sections)
            {
                if (s is BFBB_CollisionData_Section1_00BEEF01 s1)
                {
                    for (int i = 0; i < s1.branchNodes.Length; i++)
                    {
                        switch (s1.branchNodes[i].LeftDirection)
                        {
                            case ClumpDirection.X:
                                s1.branchNodes[i].LeftValue *= factor.X;
                                s1.branchNodes[i].RightValue *= factor.X;
                                break;
                            case ClumpDirection.Y:
                                s1.branchNodes[i].LeftValue *= factor.Y;
                                s1.branchNodes[i].RightValue *= factor.Y;
                                break;
                            default:
                                s1.branchNodes[i].LeftValue *= factor.Z;
                                s1.branchNodes[i].RightValue *= factor.Z;
                                break;
                        }
                    }
                }
                else if (s is BFBB_CollisionData_Section3_00BEEF03 s3)
                {
                    for (int i = 0; i < s3.vertexList.Length; i++)
                    {
                        s3.vertexList[i].X *= factor.X;
                        s3.vertexList[i].Y *= factor.Y;
                        s3.vertexList[i].Z *= factor.Z;
                    }
                }
            }
            File = sections;
        }
    }
}