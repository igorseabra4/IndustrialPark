using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark.RenderWare
{
    internal class RwUtility
    {
        private static readonly string RwPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RenderWare");

        private static readonly string AssimpPath = Path.Combine(RwPath, "assimp-vc143-mtd.dll");

        private static readonly string Rw31ExePath = Path.Combine(RwPath, "RenderWare31.exe");
        private static readonly string Rw31DllPath = Path.Combine(RwPath, "RW31.dll");
        private static readonly string Rw35ExePath = Path.Combine(RwPath, "RenderWare35.exe");

        private static readonly string IniPath = Path.Combine(RwPath, "config.ini");
        private static readonly string BSPPath = Path.Combine(RwPath, "world.bsp");
        private static readonly string LogOutputPath = Path.Combine(RwPath, "log.txt");

        internal static bool IsRw31Installed => File.Exists(Rw31ExePath) && File.Exists(Rw31DllPath) && File.Exists(AssimpPath);
        internal static bool IsRw35Installed => File.Exists(Rw35ExePath) && File.Exists(AssimpPath);

        private static Process RwProcess;

        private static int WorldFlagsFromConfiguration(bool normals, bool vertexColor, bool uvCoords, bool triStrip, bool ignoreMatColor)
        {
            return (int)(WorldFlags.HasVertexPositions | WorldFlags.WorldSectorsOverlap |
                (normals ? WorldFlags.HasNormals | WorldFlags.UseLighting : 0) |
                (vertexColor ? WorldFlags.HasVertexColors : 0) |
                (uvCoords ? WorldFlags.HasOneSetOfTextCoords : 0) |
                (triStrip ? WorldFlags.UseTriangleStrips : 0) |
                (ignoreMatColor ? 0 : WorldFlags.ModulateMaterialColors));
        }

        /// <summary>
        /// Write 3.1 config ini file. Empty fields will use default renderware values
        /// </summary>
        /// <returns></returns>
        private static void Write31ConfigIni(bool normals, bool vertexColor, bool uvCoords, bool triStrip, bool ignoreMatColor, bool noMaterials, bool flipUV, bool collTree)
        {
            string rtImport =
                "[RtWorldImportParameters]\r\n" +
                "worldSectorMaxSize=\r\n" +
                "maxWorldSectorPolygons=\r\n" +
                "maxClosestCheck=\r\n" +
                "weldThreshold=\r\n" +
                "angularThreshold=\r\n" +
                "spaceFillingSectors=\r\n" +
                "calcNormals=\r\n" +
                "maxOverlapPercent=\r\n" +
                "noAlphaInOverlap=\r\n" +
                "conditionGeometry=\r\n" +
                "userSpecifiedBBox=\r\n" +
                "userBBox.inf.x=\r\n" +
                "userBBox.inf.y=\r\n" +
                "userBBox.inf.z=\r\n" +
                "userBBox.sup.x=\r\n" +
                "userBBox.sup.y=\r\n" +
                "userBBox.sup.z=\r\n" +
                "uvLimit=\r\n" +
                "retainCreases=\r\n" +
                "fixTJunctions=\r\n" +
                "generateCollTrees=\r\n" +
                "weldPolygons=\r\n" +
                $"flags={WorldFlagsFromConfiguration(normals, vertexColor, uvCoords, triStrip, ignoreMatColor)}\r\n" +
                "mode=\r\n" +
                "sortPolygons=\r\n" +
                "cullZeroAreaPolygons=\r\n";

            string assimp =
                "[Assimp]\r\n" +
                $"removeNormals={(normals ? "false" : "true")}\r\n" +
                $"removeVertexColors={(vertexColor ? "false" : "true")}\r\n" +
                $"removeUVCoords={(uvCoords ? "false" : "true")}\r\n" +
                $"removeMaterials={(noMaterials ? "true" : "false")}\r\n" +
                $"flipUV={(flipUV ? "true" : "false")}\r\n" +
                $"collTree={(collTree ? "true" : "false")}\r\n";

            File.WriteAllText(IniPath, rtImport + assimp);
        }

        /// <summary>
        /// Write 3.5 config ini file. Empty fields will use default renderware values
        /// </summary>
        /// <returns></returns>
        private static void Write35ConfigIni(bool normals, bool vertexColor, bool uvCoords, bool triStrip, bool ignoreMatColor, bool noMaterials, bool flipUV, bool collTree)
        {
            string rtImport =
                "[RtWorldParameters]\r\n" +
                "worldSectorMaxSize=\r\n" +
                "maxWorldSectorPolygons=\r\n" +
                "maxOverlapPercent=\r\n" +
                "userSpecifiedBBox=\r\n" +
                "userBBox.inf.x=\r\n" +
                "userBBox.inf.y=\r\n" +
                "userBBox.inf.z=\r\n" +
                "userBBox.sup.x=\r\n" +
                "userBBox.sup.y=\r\n" +
                "userBBox.sup.z=\r\n" +
                "fixTJunctions=\r\n" +
                $"flags={WorldFlagsFromConfiguration(normals, vertexColor, uvCoords, triStrip, ignoreMatColor)}\r\n" +
                "numTexCoordSets=\r\n" +
                "terminatorCheck=\r\n" +
                "calcNormals=\r\n" +
                "conditionGeometry=\r\n";

            string assimp =
                "[Assimp]\r\n" +
                $"removeNormals={(normals ? "false" : "true")}\r\n" +
                $"removeVertexColors={(vertexColor ? "false" : "true")}\r\n" +
                $"removeUVCoords={(uvCoords ? "false" : "true")}\r\n" +
                $"removeMaterials={(noMaterials ? "true" : "false")}\r\n" +
                $"flipUV={(flipUV ? "true" : "false")}\r\n" +
                $"collTree={(collTree ? "true" : "false")}\r\n";

            string rtGCond =
                "[RtGCondParameters]\r\n" +
                "flags=\r\n" +
                "weldThreshold=\r\n" +
                "angularThreshold=\r\n" +
                "uvThreshold=\r\n" +
                "preLitThreshold=\r\n" +
                "areaThreshold=\r\n" +
                "uvLimitMin=\r\n" +
                "uvLimitMax=\r\n" +
                "textureMode=\r\n" +
                "polyNormalsThreshold=\r\n" +
                "polyUVsThreshold=\r\n" +
                "polyPreLitsThreshold=\r\n" +
                "decimationMode=\r\n" +
                "decimationPasses=\r\n" +
                "convexPartitioningMode=\r\n";

            File.WriteAllText(IniPath, rtImport + assimp + rtGCond);
        }

        /// <summary>
        /// Builds a BSP through renderware directly. If successful, will create a "world.bsp" file in the same directory it was called from.
        /// </summary>
        /// <param name="modelPath">Path to an assimp supported model file</param>
        /// <param name="game">Scooby: 3.1 BSP<para/>BFBB: 3.5 BSP</param>
        /// <param name="normals">True if normals should be imported (and automatically generated by assimp when no normals are present)</param>
        /// <param name="vertexColor">True if vertex color should be imported (and set to 0xFFFFFFFF by default when no color is present)</param>
        /// <param name="uvCoords">True if uv coords are present</param>
        /// <param name="triStrip">Triangle strips if true, triangle list otherwise</param>
        /// <param name="ignoreMatColor">True if material color should not be taken into account</param>
        /// <param name="noMaterials">True if all materials should be removed by assimp</param>
        /// <param name="flipUV">Flip uv coords</param>
        /// <param name="collTree">True if collision tree should be generated</param>
        /// <returns>BSP as byte array</returns>
        internal static byte[] PerformBSPConversion(string modelPath, HipHopFile.Game game, 
            bool normals, bool vertexColor, bool uvCoords, bool triStrip, bool ignoreMatColor, bool noMaterials, bool flipUV, bool collTree)
        {
            byte[] bsp;
            if (game == HipHopFile.Game.Scooby)
            {
                if (!IsRw31Installed)
                    throw new Exception($"Make sure \"RenderWare31.exe\", \"RW31.dll\" and \"assimp-vc143-mtd.dll\" exist in {RwPath}");
                Write31ConfigIni(normals, vertexColor, uvCoords, triStrip, ignoreMatColor, noMaterials, flipUV, collTree);
            }
            else if (game == HipHopFile.Game.BFBB)
            {
                if (!IsRw35Installed)
                    throw new Exception($"Make sure \"RenderWare35.exe\" and \"assimp-vc143-mtd.dll\" exist in {RwPath}");
                Write35ConfigIni(normals, vertexColor, uvCoords, triStrip, ignoreMatColor, noMaterials, flipUV, collTree);
            }
            else
            {
                throw new Exception(game.ToString() + " does not support BSPs");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = game == HipHopFile.Game.Scooby ? Rw31ExePath : Rw35ExePath,
                WorkingDirectory = RwPath,
                Arguments = $"-i \"{modelPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            RwProcess = new Process() { StartInfo = startInfo };

            try
            {
                RwProcess.Start();
#if DEBUG
                string output = RwProcess.StandardOutput.ReadToEnd();
                string err = RwProcess.StandardError.ReadToEnd();
                File.WriteAllText(LogOutputPath, output + err);
#endif
                RwProcess.WaitForExit(5000);

                bsp = File.ReadAllBytes(BSPPath);
            }
            finally
            {
                if (RwProcess != null)
                    RwProcess.Dispose();
#if !DEBUG
                if (File.Exists(IniPath))
                    File.Delete(IniPath);
                if (File.Exists(BSPPath))
                    File.Delete(BSPPath);
#endif
            }
            return bsp;
        }
    }
}
