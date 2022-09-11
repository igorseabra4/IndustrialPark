using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;

namespace IndustrialPark
{
    public class AssetJSP : AssetRenderWareModel, IRenderableAsset
    {
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public AssetJSP(string assetName, AssetType assetType, byte[] data, SharpRenderer renderer) : base(assetName, assetType, data, renderer) { }

        public AssetJSP(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness, renderer) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableJSPs(this);
        }

        public void CreateTransformMatrix() => boundingBox = BoundingBox.FromPoints(model.vertexListG.ToArray());

        public float GetDistanceFrom(Vector3 cameraPosition) => 0;

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            model.Render(renderer, Matrix.Identity, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero, _dontDrawMeshNumber);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            float? smallestDistance = null;

            foreach (Triangle t in model.triangleList)
            {
                Vector3 v1 = model.vertexListG[t.vertex1];
                Vector3 v2 = model.vertexListG[t.vertex2];
                Vector3 v3 = model.vertexListG[t.vertex3];

                if (ray.Intersects(ref v1, ref v2, ref v3, out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
            }

            return smallestDistance;
        }

        public void ApplyScale(Vector3 factor)
        {
            RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
            var renderWareVersion = sections[0].renderWareVersion;
            foreach (RWSection rws in sections)
                if (rws is Clump_0010 clump)
                    foreach (Geometry_000F geo in clump.geometryList.geometryList)
                        ApplyScale(factor, geo);
                else if (rws is World_000B world)
                {
                    if (world.firstWorldChunk is AtomicSector_0009 atomic)
                        ApplyScale(factor, atomic);
                    else if (world.firstWorldChunk is PlaneSector_000A plane)
                        ApplyScale(factor, plane);
                }

            Data = ReadFileMethods.ExportRenderWareFile(sections, renderWareVersion);

            if (model != null)
                model.Dispose();
            if (Program.MainForm != null)
            {
                Setup(Program.MainForm.renderer);
                CreateTransformMatrix();
            }
        }

        private static void ApplyScale(Vector3 factor, Geometry_000F geo)
        {
            geo.geometryStruct.sphereCenterX *= factor.X;
            geo.geometryStruct.sphereCenterY *= factor.X;
            geo.geometryStruct.sphereCenterZ *= factor.X;
            geo.geometryStruct.sphereRadius *= Math.Max(factor.X, Math.Max(factor.Y, factor.Z));

            if ((geo.geometryStruct.geometryFlags2 & GeometryFlags2.isNativeGeometry) == 0)
                for (int i = 0; i < geo.geometryStruct.morphTargets.Length; i++)
                {
                    if (geo.geometryStruct.morphTargets[i].hasVertices != 0)
                    {
                        for (int j = 0; j < geo.geometryStruct.morphTargets[i].vertices.Length; j++)
                        {
                            geo.geometryStruct.morphTargets[i].vertices[j].X *= factor.X;
                            geo.geometryStruct.morphTargets[i].vertices[j].Y *= factor.Y;
                            geo.geometryStruct.morphTargets[i].vertices[j].Z *= factor.Z;
                            geo.geometryStruct.morphTargets[i].sphereCenter.X *= factor.X;
                            geo.geometryStruct.morphTargets[i].sphereCenter.Y *= factor.X;
                            geo.geometryStruct.morphTargets[i].sphereCenter.X *= factor.X;
                            geo.geometryStruct.morphTargets[i].radius *= Math.Max(factor.X, Math.Max(factor.Y, factor.Z));
                        }
                    }
                }
            else
                foreach (var ex in geo.geometryExtension.extensionSectionList)
                    if (ex is NativeDataPLG_0510 nativeData)
                        if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                            ApplyScale(factor, nativeData);
        }

        private static void ApplyScale(Vector3 factor, NativeDataPLG_0510 nativeData)
        {
            for (int i = 0; i < nativeData.nativeDataStruct.nativeData.declarations.Length; i++)
                if (nativeData.nativeDataStruct.nativeData.declarations[i].declarationType == Declarations.Vertex)
                {
                    var vd = (Vertex3Declaration)nativeData.nativeDataStruct.nativeData.declarations[i];
                    for (int j = 0; j < vd.entryList.Count; j++)
                        vd.entryList[j] = new Vertex3(
                            vd.entryList[j].X * factor.X,
                            vd.entryList[j].Y * factor.Y,
                            vd.entryList[j].Z * factor.Z);
                }
        }

        private static void ApplyScale(Vector3 factor, AtomicSector_0009 atomic)
        {
            if (atomic.atomicSectorStruct.isNativeData)
            {
                foreach (var ex in atomic.atomicSectorExtension.extensionSectionList)
                    if (ex is NativeDataPLG_0510 nativeData)
                        if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                            ApplyScale(factor, nativeData);
            }
            else
            {
                atomic.atomicSectorStruct.boxMaximum.X *= factor.X;
                atomic.atomicSectorStruct.boxMaximum.Y *= factor.Y;
                atomic.atomicSectorStruct.boxMaximum.Z *= factor.Z;

                atomic.atomicSectorStruct.boxMinimum.X *= factor.X;
                atomic.atomicSectorStruct.boxMinimum.Y *= factor.Y;
                atomic.atomicSectorStruct.boxMinimum.Z *= factor.Z;

                for (int i = 0; i < atomic.atomicSectorStruct.vertexArray.Length; i++)
                {
                    atomic.atomicSectorStruct.vertexArray[i].X *= factor.X;
                    atomic.atomicSectorStruct.vertexArray[i].Y *= factor.Y;
                    atomic.atomicSectorStruct.vertexArray[i].Z *= factor.Z;
                }
            }
        }

        private static void ApplyScale(Vector3 factor, PlaneSector_000A plane)
        {
            if (plane.leftSection is AtomicSector_0009 atomicL)
                ApplyScale(factor, atomicL);
            else if (plane.leftSection is PlaneSector_000A planeL)
                ApplyScale(factor, planeL);
            if (plane.rightSection is AtomicSector_0009 atomicR)
                ApplyScale(factor, atomicR);
            else if (plane.rightSection is PlaneSector_000A planeR)
                ApplyScale(factor, planeR);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => false;
    }
}