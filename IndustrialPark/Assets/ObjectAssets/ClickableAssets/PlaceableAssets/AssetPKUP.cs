using HipHopFile;
using SharpDX;
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

        protected override float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (dontRender || isInvisible)
                return null;

            uint _modelAssetId;
            try { _modelAssetId = AssetPICK.pickEntries[_pickEntryID]; }
            catch { return initialDistance; }

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetId))
            {
                RenderWareModelFile rwmf;

                if (ArchiveEditorFunctions.renderingDictionary[_modelAssetId] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        rwmf = ArchiveEditorFunctions.renderingDictionary[_modelAssetId].GetRenderWareModelFile();
                    else return initialDistance;
                }
                else rwmf = ArchiveEditorFunctions.renderingDictionary[_modelAssetId].GetRenderWareModelFile();

                foreach (RenderWareFile.Triangle t in rwmf.triangleList)
                {
                    Vector3 v1 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex1], world);
                    Vector3 v2 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex2], world);
                    Vector3 v3 = (Vector3)Vector3.Transform(rwmf.vertexListG[t.vertex3], world);

                    if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                    {
                        hasIntersected = true;

                        if (distance < smallestDistance)
                            smallestDistance = distance;
                    }
                }

                if (hasIntersected)
                    return smallestDistance;
                else return null;
            }

            return initialDistance;
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible) return;
                if (AssetPICK.pickEntries.ContainsKey(_pickEntryID))
                    if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(AssetPICK.pickEntries[_pickEntryID]))
                    {
                        ArchiveEditorFunctions.renderingDictionary[AssetPICK.pickEntries[_pickEntryID]].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color);
                        return;
                    }
            renderer.DrawCube(world, isSelected);
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