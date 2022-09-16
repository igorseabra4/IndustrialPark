using System.IO;

namespace IndustrialPark
{
    public class BakeScaleModelContainer
    {
        public ArchiveEditorFunctions archive;
        public IAssetWithModel model;

        public override string ToString() =>
            $"{Path.GetFileName(archive.currentlyOpenFilePath)}";
    }
}