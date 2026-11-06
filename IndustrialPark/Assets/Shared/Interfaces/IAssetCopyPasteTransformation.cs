namespace IndustrialPark
{
    public interface IAssetCopyPasteTransformation
    {
        Transformation GetTransformation();
        void SetTransformation(Transformation transformation);
        void CreateTransformMatrix(SharpRenderer renderer);
    }
}
