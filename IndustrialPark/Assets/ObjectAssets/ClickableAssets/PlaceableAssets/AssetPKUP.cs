using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetPKUP : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x5C + Offset;

        public AssetPKUP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _pickEntryID = ReadUInt(0x54 + Offset);
        }

        public override bool HasReference(uint assetID) => PickReferenceID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (_pickEntryID == 0)
                result.Add("PKUP with PickReferenceID set to 0");
        }

        protected override void CreateBoundingBox()
        {
            if (AssetPICK.pickEntries.ContainsKey(_pickEntryID) &&
                renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]) &&
                renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].HasRenderWareModelFile() &&
                renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile() != null)
            {
                List<Vector3> vertexList = renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().vertexListG;

                vertices = new Vector3[vertexList.Count];
                for (int i = 0; i < vertexList.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);
                boundingBox = BoundingBox.FromPoints(vertices);

                if (renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]))
                {
                    if (renderingDictionary[AssetPICK.pickEntries[_pickEntryID]] is AssetMINF MINF)
                    {
                        if (MINF.HasRenderWareModelFile())
                            triangles = renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().triangleList.ToArray();
                        else
                            triangles = null;
                    }
                    else
                        triangles = renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].GetRenderWareModelFile().triangleList.ToArray();
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
            if (!isSelected && (dontRender || isInvisible))
                return;

            if (AssetPICK.pickEntries.ContainsKey(_pickEntryID))
                if (renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]))
                {
                    renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
                    return;
                }

            renderer.DrawCube(LocalWorld(), isSelected);
        }

        private const string categoryName = "Pickup";

        [TypeConverter(typeof(HexByteTypeConverter))]
        [Category(categoryName)]
        public byte Shape
        {
            get => ReadByte(0x09);
            set => Write(0x09, value);
        }

        private uint _pickEntryID;
        [Category(categoryName)]
        public AssetID PickReferenceID
        {
            get => _pickEntryID;
            set
            {
                _pickEntryID = value;
                Write(0x54 + Offset, value);
            }
        }

        [Category(categoryName)]        
        public DynamicTypeDescriptor PickupFlags => ShortFlagsDescriptor(0x58 + Offset,
            "Reappear right after collect",
            "Initially visible");

        [Browsable(false)]
        public short PickupFlagsShort
        {
            get => ReadShort(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName)]
        public short PickupValue
        {
            get => ReadShort(0x5A + Offset);
            set => Write(0x5A + Offset, value);
        }
    }
}