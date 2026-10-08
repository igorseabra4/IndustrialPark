using HipHopFile;
using System.IO;

namespace IndustrialPark
{
    public class LayerAddedAction : IReversibleAction
    {
        private ArchiveEditorFunctions archive;
        private LayerType type;
        private int index;

        public LayerAddedAction(ArchiveEditorFunctions archive, LayerType type, int index)
        {
            this.archive = archive;
            this.type = type;
            this.index = index;
        }

        public void Undo()
        {
            archive.RemoveLayer(index);
        }

        public void Redo()
        {
            archive.AddLayer(type, index);
        }

        public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

        public override string ToString()
        {
            return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] Layer {type} added at {index}";
        }
    }
}