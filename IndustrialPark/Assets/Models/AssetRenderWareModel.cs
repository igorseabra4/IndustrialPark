﻿using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetRenderWareModel : AssetWithData
    {
        protected RenderWareModelFile model;

        public override string AssetInfo => $"{RwVersion(renderWareVersion)} {(IsNativeData ? "native" : "")} {base.AssetInfo}";

        public AssetRenderWareModel(string assetName, AssetType assetType, byte[] data, SharpRenderer renderer) : base(assetName, assetType, data)
        {
            Setup(renderer);
        }

        public AssetRenderWareModel(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game)
        {
            Setup(renderer);
        }

        public virtual void Setup(SharpRenderer renderer)
        {
            if (model != null)
                model.Dispose();
            if (renderer == null)
                return;

#if !DEBUG
            try
            {
#endif
            ReadFileMethods.treatStuffAsByteArray = false;
            var rwSecArray = ReadFileMethods.ReadRenderWareFile(Data);
            model = new RenderWareModelFile(renderer.device, rwSecArray);
            if (rwSecArray.Length > 0)
                renderWareVersion = rwSecArray[0].renderWareVersion;
            SetupAtomicFlagsForRender();
#if !DEBUG
            }
            catch (Exception ex)
            {
                if (model != null)
                    model.Dispose();
                model = null;
                throw new Exception("Error: " + ToString() + " has an unsupported format and cannot be rendered. " + ex.Message);
            }
#endif
        }

        public RenderWareModelFile GetRenderWareModelFile() => model;

        [Browsable(false)]
        public bool IsNativeData => model != null && model.isNativeData;

        [Browsable(false)]
        public string[] Textures
        {
            get
            {
                List<string> names = new List<string>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                    {
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            if (geo.materialList != null)
                                if (geo.materialList.materialList != null)
                                    foreach (Material_0007 mat in geo.materialList.materialList)
                                        if (mat.texture != null)
                                            if (mat.texture.diffuseTextureName != null)
                                                if (!names.Contains(mat.texture.diffuseTextureName.stringString))
                                                    names.Add(mat.texture.diffuseTextureName.stringString);
                    }
                    else if (rws is World_000B world)
                    {
                        if (world.materialList != null)
                            if (world.materialList.materialList != null)
                                foreach (Material_0007 mat in world.materialList.materialList)
                                    if (mat.texture != null)
                                        if (mat.texture.diffuseTextureName != null)
                                            if (!names.Contains(mat.texture.diffuseTextureName.stringString))
                                                names.Add(mat.texture.diffuseTextureName.stringString);
                    }

                return names.ToArray();
            }
        }

        public override bool HasReference(uint assetID) =>
            Textures.Any(s => Functions.BKDRHash(s + ".RW3") == assetID || Functions.BKDRHash(s) == assetID) ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (ModelAsRWSections.Length == 0)
                result.Add("Failed to read MODL asset. This might be just a library error and does not necessarily mean the model is broken.");

            foreach (string s in Textures)
                if (!(Program.MainForm.AssetExists(Functions.BKDRHash(s + ".RW3")) || Program.MainForm.AssetExists(Functions.BKDRHash(s))))
                    result.Add($"I haven't found texture {s}, used by the model. This might just mean I haven't looked properly for it, though.");

            if (Program.MainForm.WhoTargets(assetID).Count == 0)
                result.Add("Model appears to be unused, as no other asset references it. This might just mean I haven't looked properly for an asset which does does, though.");
        }

        private int renderWareVersion;

        protected RWSection[] ModelAsRWSections
        {
            get
            {
                try
                {
                    ReadFileMethods.treatStuffAsByteArray = true;
                    RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
                    renderWareVersion = sections[0].renderWareVersion;
                    ReadFileMethods.treatStuffAsByteArray = false;
                    return sections;
                }
                catch
                {
                    return new RWSection[0];
                }
            }
            set
            {
                ReadFileMethods.treatStuffAsByteArray = true;
                Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
                ReadFileMethods.treatStuffAsByteArray = false;

                model.Dispose();
                Setup(Program.MainForm.renderer);
            }
        }

        [Browsable(false)]
        public Material_0007[] Materials
        {
            get
            {
                var materials = new List<Material_0007>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            materials.AddRange(geo.materialList.materialList);

                return materials.ToArray();
            }
            set
            {
                RWSection[] sections = ModelAsRWSections;

                int k = 0;
                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                    {
                        for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
                        {
                            bool hasMaterialEffects = false;
                            for (int j = 0; j < clump.geometryList.geometryList[i].materialList.materialList.Length; j++)
                            {
                                if (k >= value.Length)
                                    break;
                                else
                                    clump.geometryList.geometryList[i].materialList.materialList[j] = value[k];

                                foreach (var rwss in value[k].materialExtension.extensionSectionList)
                                    if (rwss is MaterialEffectsPLG_0120)
                                        hasMaterialEffects = true;
                                k++;
                            }

                            // giving the atomic the material effects
                            if (hasMaterialEffects)
                            {
                                var plg = new MaterialEffectsPLG_0120() { value = MaterialEffectType.BumpMap, isAtomicExtension = true };

                                bool newMatEffsFound = false;
                                for (int j = 0; j < clump.atomicList[i].atomicExtension.extensionSectionList.Count; j++)
                                    if (clump.atomicList[i].atomicExtension.extensionSectionList[j] is MaterialEffectsPLG_0120)
                                    {
                                        clump.atomicList[i].atomicExtension.extensionSectionList[j] = plg;
                                        newMatEffsFound = true;
                                        break;
                                    }

                                if (!newMatEffsFound)
                                    clump.atomicList[i].atomicExtension.extensionSectionList.Add(plg);
                            }
                            else
                            {
                                for (int j = 0; j < clump.atomicList[i].atomicExtension.extensionSectionList.Count; j++)
                                    if (clump.atomicList[i].atomicExtension.extensionSectionList[j] is MaterialEffectsPLG_0120 plg)
                                        clump.atomicList[i].atomicExtension.extensionSectionList.RemoveAt(j--);
                            }
                        }
                    }

                ModelAsRWSections = sections;
                Setup(Program.MainForm.renderer);
            }
        }

        protected bool[] _dontDrawMeshNumber;

        [Category("Model Data")]
        public AtomicFlags[] AtomicFlags
        {
            get
            {
                List<AtomicFlags> flags = new List<AtomicFlags>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (var atomic in clump.atomicList)
                            flags.Add(atomic.atomicStruct.flags);

                return flags.ToArray();
            }

            set
            {
                int i = 0;
                RWSection[] sections = ModelAsRWSections;

                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                        foreach (var atomic in clump.atomicList)
                        {
                            if (i >= value.Length)
                                continue;

                            atomic.atomicStruct.flags = value[i];
                            i++;
                        }

                ModelAsRWSections = sections;
                SetupAtomicFlagsForRender();
            }
        }

        private void SetupAtomicFlagsForRender()
        {
            var value = AtomicFlags;
            if (value.Length == 0)
            {
                _dontDrawMeshNumber = new bool[model.meshList.Count];
                for (int j = 0; j < value.Length; j++)
                    _dontDrawMeshNumber[j] = false;
            }
            else
            {
                _dontDrawMeshNumber = new bool[value.Length];
                for (int j = 0; j < value.Length; j++)
                    _dontDrawMeshNumber[j] = !((value[j] & RenderWareFile.Sections.AtomicFlags.Render) != 0);
            }
        }

        public void ApplyVertexColors(Func<Vector4, Vector4> getColor)
        {
            RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
            renderWareVersion = sections[0].renderWareVersion;

            foreach (RWSection rws in sections)
                if (rws is Clump_0010 clump)
                    for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
                        if (clump.geometryList.geometryList[i].geometryStruct.vertexColors != null)
                            ApplyVertexColors(clump.geometryList.geometryList[i], getColor);
                        else
                            foreach (var ex in clump.geometryList.geometryList[i].geometryExtension.extensionSectionList)
                                if (ex is NativeDataPLG_0510 nativeData)
                                    if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                                        ApplyVertexColors(nativeData.nativeDataStruct.nativeData, getColor);

            Data = ReadFileMethods.ExportRenderWareFile(sections, renderWareVersion);
            if (Program.MainForm != null)
                Setup(Program.MainForm.renderer);
        }

        private void ApplyVertexColors(NativeDataGC nativeData, Func<Vector4, Vector4> getColor)
        {
            for (int j = 0; j < nativeData.declarations.Length; j++)
                if (nativeData.declarations[j].declarationType == Declarations.Color)
                {
                    var vd = (ColorDeclaration)nativeData.declarations[j];
                    for (int k = 0; k < vd.entryList.Count; k++)
                    {
                        var oldColor = vd.entryList[k];

                        var newColor = getColor(
                            new Vector4(oldColor.R / 255f, oldColor.G / 255f, oldColor.B / 255f, oldColor.A / 255f));

                        vd.entryList[k] = new RenderWareFile.Color(
                            (byte)(newColor.X * 255),
                            (byte)(newColor.Y * 255),
                            (byte)(newColor.Z * 255),
                            (byte)(newColor.W * 255));
                    }
                }
        }

        private void ApplyVertexColors(Geometry_000F geometry, Func<Vector4, Vector4> getColor)
        {
            for (int i = 0; i < geometry.geometryStruct.vertexColors.Length; i++)
            {
                var oldColor = geometry.geometryStruct.vertexColors[i];
                var newColor = getColor(new Vector4(oldColor.R / 255.0f, oldColor.G / 255.0f, oldColor.B / 255.0f, oldColor.A / 255.0f));
                geometry.geometryStruct.vertexColors[i] = new RenderWareFile.Color(
                    (byte)(newColor.X * 255f),
                    (byte)(newColor.Y * 255f),
                    (byte)(newColor.Z * 255f),
                    (byte)(newColor.W * 255f));
            }
        }

        public virtual void ApplyScale(Vector3 factor)
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
            Setup(Program.Renderer);
        }

        private static void ApplyScale(Vector3 factor, Geometry_000F geo)
        {
            var singleFactor = Math.Max(factor.X, Math.Max(factor.Y, factor.Z));
            geo.geometryStruct.sphereCenterX *= factor.X;
            geo.geometryStruct.sphereCenterY *= factor.Y;
            geo.geometryStruct.sphereCenterZ *= factor.Z;
            geo.geometryStruct.sphereRadius *= singleFactor;

            if ((geo.geometryStruct.geometryFlags2 & GeometryFlags2.isNativeGeometry) == 0)
                ApplyScale(factor, geo.geometryStruct, singleFactor);
            else
                foreach (var ex in geo.geometryExtension.extensionSectionList)
                    if (ex is NativeDataPLG_0510 nativeData)
                        if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                            ApplyScale(factor, nativeData);
        }

        private static void ApplyScale(Vector3 factor, GeometryStruct_0001 geometryStruct, float singleFactor)
        {
            for (int i = 0; i < geometryStruct.morphTargets.Length; i++)
            {
                geometryStruct.morphTargets[i].sphereCenter.X *= factor.X;
                geometryStruct.morphTargets[i].sphereCenter.Y *= factor.Y;
                geometryStruct.morphTargets[i].sphereCenter.Z *= factor.Z;
                geometryStruct.morphTargets[i].radius *= singleFactor;

                if (geometryStruct.morphTargets[i].hasVertices != 0)
                    for (int j = 0; j < geometryStruct.morphTargets[i].vertices.Length; j++)
                    {
                        geometryStruct.morphTargets[i].vertices[j].X *= factor.X;
                        geometryStruct.morphTargets[i].vertices[j].Y *= factor.Y;
                        geometryStruct.morphTargets[i].vertices[j].Z *= factor.Z;
                    }
            }
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

        public virtual void ApplyRotation(float yaw, float pitch, float roll)
        {
            var matrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);
            var transform = new Func<float, float, float, Vertex3>((float x, float y, float z) =>
            {
                var vector = Vector3.Transform(new Vector3(x, y, z), matrix);
                return new Vertex3(vector.X, vector.Y, vector.Z);
            });
            ApplyTransformation(transform);
        }

        private void ApplyTransformation(Func<float, float, float, Vertex3> transform)
        {
            RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
            var renderWareVersion = sections[0].renderWareVersion;
            foreach (RWSection rws in sections)
                if (rws is Clump_0010 clump)
                {
                    StripClump(clump);
                    foreach (Geometry_000F geo in clump.geometryList.geometryList)
                        ApplyTransformation(transform, geo);
                }
                else if (rws is World_000B world)
                {
                    if (world.firstWorldChunk is AtomicSector_0009 atomic)
                        ApplyTransformation(transform, atomic);
                    else if (world.firstWorldChunk is PlaneSector_000A plane)
                        ApplyTransformation(transform, plane);
                }

            Data = ReadFileMethods.ExportRenderWareFile(sections, renderWareVersion);
            Setup(Program.Renderer);
        }

        private void StripClump(Clump_0010 clump)
        {
            foreach (var extension in clump.frameList.extensionList)
                for (int i = 0; i < extension.extensionSectionList.Count; i++)
                    if (extension.extensionSectionList[i].sectionIdentifier == RenderWareFile.Section.HAnimPLG)
                        extension.extensionSectionList.RemoveAt(i--);
            foreach (var geo in clump.geometryList.geometryList)
                for (int i = 0; i < geo.geometryExtension.extensionSectionList.Count; i++)
                    if (geo.geometryExtension.extensionSectionList[i].sectionIdentifier == RenderWareFile.Section.MorphPLG || geo.geometryExtension.extensionSectionList[i].sectionIdentifier == RenderWareFile.Section.CollisionPLG)
                        geo.geometryExtension.extensionSectionList.RemoveAt(i--);
        }

        private static void ApplyTransformation(Func<float, float, float, Vertex3> transform, Geometry_000F geo)
        {
            if ((geo.geometryStruct.geometryFlags2 & GeometryFlags2.isNativeGeometry) == 0)
            {
                ApplyTransformation(transform, geo.geometryStruct, out _);
                geo.geometryStruct.sphereCenterX = 0;
                geo.geometryStruct.sphereCenterY = 0;
                geo.geometryStruct.sphereCenterZ = 0;
                geo.geometryStruct.sphereRadius = 0;
            }
            else
            {
                BoundingSphere? bounds = null;
                foreach (var ex in geo.geometryExtension.extensionSectionList)
                    if (ex is NativeDataPLG_0510 nativeData)
                        if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                        {
                            ApplyTransformation(transform, nativeData, out BoundingSphere? localBounds);
                            bounds = bounds.HasValue ? BoundingSphere.Merge(bounds.Value, localBounds.Value) : localBounds;
                        }
                if (bounds.HasValue)
                {
                    geo.geometryStruct.sphereCenterX = bounds.Value.Center.X;
                    geo.geometryStruct.sphereCenterY = bounds.Value.Center.Y;
                    geo.geometryStruct.sphereCenterZ = bounds.Value.Center.Z;
                    geo.geometryStruct.sphereRadius = bounds.Value.Radius;
                }
            }
        }

        private static void ApplyTransformation(Func<float, float, float, Vertex3> transform, GeometryStruct_0001 geometryStruct, out BoundingSphere? bounds)
        {
            bounds = null;
            for (int i = 0; i < geometryStruct.morphTargets.Length; i++)
            {
                BoundingSphere localBounds = new BoundingSphere();
                if (geometryStruct.morphTargets[i].hasVertices != 0)
                {
                    for (int j = 0; j < geometryStruct.morphTargets[i].vertices.Length; j++)
                        geometryStruct.morphTargets[i].vertices[j] = transform(geometryStruct.morphTargets[i].vertices[j].X, geometryStruct.morphTargets[i].vertices[j].Y, geometryStruct.morphTargets[i].vertices[j].Z);
                    localBounds = GetBoundingSphere(geometryStruct.morphTargets[i].vertices);
                    bounds = bounds.HasValue ? BoundingSphere.Merge(bounds.Value, localBounds) : localBounds;
                }
                geometryStruct.morphTargets[i].sphereCenter.X = localBounds.Center.X;
                geometryStruct.morphTargets[i].sphereCenter.Y = localBounds.Center.Y;
                geometryStruct.morphTargets[i].sphereCenter.Z = localBounds.Center.Z;
                geometryStruct.morphTargets[i].radius = localBounds.Radius;
            }
        }

        private static void ApplyTransformation(Func<float, float, float, Vertex3> transform, NativeDataPLG_0510 nativeData, out BoundingSphere? bounds)
        {
            bounds = null;
            for (int i = 0; i < nativeData.nativeDataStruct.nativeData.declarations.Length; i++)
                if (nativeData.nativeDataStruct.nativeData.declarations[i].declarationType == Declarations.Vertex)
                {
                    var vd = (Vertex3Declaration)nativeData.nativeDataStruct.nativeData.declarations[i];
                    for (int j = 0; j < vd.entryList.Count; j++)
                        vd.entryList[j] = transform(vd.entryList[j].X, vd.entryList[j].Y, vd.entryList[j].Z);
                    var newBounds = GetBoundingSphere(vd.entryList);
                    bounds = bounds.HasValue ? BoundingSphere.Merge(bounds.Value, newBounds) : newBounds;
                }
        }

        private static void ApplyTransformation(Func<float, float, float, Vertex3> transform, AtomicSector_0009 atomic)
        {
            BoundingSphere? bounds = null;
            if (atomic.atomicSectorStruct.isNativeData)
            {
                foreach (var ex in atomic.atomicSectorExtension.extensionSectionList)
                    if (ex is NativeDataPLG_0510 nativeData)
                        if (nativeData.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                            ApplyTransformation(transform, nativeData, out bounds);
            }
            else
            {
                for (int i = 0; i < atomic.atomicSectorStruct.vertexArray.Length; i++)
                    atomic.atomicSectorStruct.vertexArray[i] = transform(atomic.atomicSectorStruct.vertexArray[i].X, atomic.atomicSectorStruct.vertexArray[i].Y, atomic.atomicSectorStruct.vertexArray[i].Z);
                bounds = GetBoundingSphere(atomic.atomicSectorStruct.vertexArray);
            }

            if (bounds.HasValue)
            {
                var bb = BoundingBox.FromSphere(bounds.Value);
                atomic.atomicSectorStruct.boxMaximum = new Vertex3(bb.Maximum.X, bb.Maximum.Y, bb.Maximum.Z);
                atomic.atomicSectorStruct.boxMinimum = new Vertex3(bb.Minimum.X, bb.Minimum.Y, bb.Minimum.Z);
            }
        }

        private static void ApplyTransformation(Func<float, float, float, Vertex3> transform, PlaneSector_000A plane)
        {
            if (plane.leftSection is AtomicSector_0009 atomicL)
                ApplyTransformation(transform, atomicL);
            else if (plane.leftSection is PlaneSector_000A planeL)
                ApplyTransformation(transform, planeL);
            if (plane.rightSection is AtomicSector_0009 atomicR)
                ApplyTransformation(transform, atomicR);
            else if (plane.rightSection is PlaneSector_000A planeR)
                ApplyTransformation(transform, planeR);
        }

        private static BoundingSphere GetBoundingSphere(IEnumerable<Vertex3> vertices)
        {
            var first = vertices.FirstOrDefault();
            var min = new Vector3(first.X, first.Y, first.Z);
            var max = new Vector3(first.X, first.Y, first.Z);

            foreach (var v in vertices)
            {
                if (v.X < min.X)
                    min.X = v.X;
                if (v.Y < min.Y)
                    min.Y = v.Y;
                if (v.Z < min.Z)
                    min.Z = v.Z;
                if (v.X > max.X)
                    max.X = v.X;
                if (v.Y > max.Y)
                    max.Y = v.Y;
                if (v.Z > max.Z)
                    max.Z = v.Z;
            }

            return new BoundingSphere(max + min / 2f, (max - min).Length());
        }
    }
}