using HipHopFile;

namespace IndustrialPark
{
    public class LayerRemovedAction : IReversibleAction
    {
        private ArchiveEditorFunctions archive;
        private LayerType type;
        private int index;

        public LayerRemovedAction(ArchiveEditorFunctions archive, LayerType type, int index)
        {
            this.archive = archive;
            this.type = type;
            this.index = index;
        }

        public void Undo()
        {
            archive.AddLayer(type, index);
        }

        public void Redo()
        {
            archive.RemoveLayer(index);
        }

        public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;
    }
}