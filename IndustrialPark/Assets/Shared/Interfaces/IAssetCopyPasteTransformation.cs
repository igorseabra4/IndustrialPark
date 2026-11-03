namespace IndustrialPark
{
    public interface IAssetCopyPasteTransformation
    {
        void CopyTransformation();
        void PasteTransformation();
        void CreateTransformMatrix(SharpRenderer renderer);
    }
}
