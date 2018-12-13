using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPKUP : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x5C + Offset;

        public AssetPKUP(Section_AHDR AHDR) : base(AHDR) { }

        public override void Setup()
        {
            _pickEntryID = ReadUInt(0x54 + Offset);

            base.Setup();
        }

        public override bool HasReference(uint assetID)
        {
            if (PickReferenceID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        protected override void CreateBoundingBox()
        {
            if (AssetPICK.pickEntries.ContainsKey(_pickEntryID) &&
                ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]) &&
                ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile() != null)
            {
                List<Vector3> vertexList = ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().vertexListG;

                vertices = new Vector3[vertexList.Count];
                for (int i = 0; i < vertexList.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);
                boundingBox = BoundingBox.FromPoints(vertices);

                if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]))
                {
                    if (ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]] is AssetMINF MINF)
                    {
                        if (MINF.HasRenderWareModelFile())
                            triangles = ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().triangleList.ToArray();
                        else
                            triangles = null;
                    }
                    else
                        triangles = ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().triangleList.ToArray();
                }
                else
                    triangles = null;
            }
            else
            {
                vertices = new Vector3[SharpRenderer.cubeVertices.Count];
                for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);
                boundingBox = BoundingBox.FromPoints(vertices);
            }
        }
        
        public override void Draw(SharpRenderer renderer)
        {
            if (DontRender || isInvisible)
                return;

            if (AssetPICK.pickEntries.ContainsKey(_pickEntryID))
                if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]))
                {
                    ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color);
                    return;
                }

            renderer.DrawCube(LocalWorld(), isSelected);
        }
        
        [TypeConverter(typeof(HexByteTypeConverter))]
        [Category("Pickup")]
        public byte Shape
        {
            get => ReadByte(0x09);
            set => Write(0x09, value);
        }

        private uint _pickEntryID;
        [Category("Pickup")]
        public AssetID PickReferenceID
        {
            get => _pickEntryID;
            set
            {
                _pickEntryID = value;
                Write(0x54 + Offset, value);
            }
        }

        [Category("Pickup")]
        public short UnknownShort58
        {
            get => ReadShort(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Pickup")]
        public short UnknownShort5A
        {
            get => ReadShort(0x5A + Offset);
            set => Write(0x5A + Offset, value);
        }
    }
}