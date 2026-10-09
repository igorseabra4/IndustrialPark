using HipHopFile;
using System.IO;

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
            Program.MainForm.RefreshLayerList(archive, index);
        }

        public void Redo()
        {
            archive.RemoveLayer(index);
            Program.MainForm.RefreshLayerList(archive);
        }

        public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

        public override string ToString()
        {
            return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] Layer {type} removed at {index}";
        }
    }
}