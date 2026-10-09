using HipHopFile;
using System.IO;

namespace IndustrialPark
{
    public class LayerTypeChangedAction : IReversibleAction
    {
        private ArchiveEditorFunctions archive;
        private int index;
        private LayerType oldValue;
        private LayerType newValue;

        public LayerTypeChangedAction(ArchiveEditorFunctions archive, int index, LayerType oldValue, LayerType newValue)
        {
            this.archive = archive;
            this.index = index;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public void Undo()
        {
            archive.SetLayerTypeGeneric(index, oldValue);
            Program.MainForm.RefreshLayerList(archive, index);
        }

        public void Redo()
        {
            archive.SetLayerTypeGeneric(index, newValue);
            Program.MainForm.RefreshLayerList(archive, index);
        }

        public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

        public override string ToString()
        {
            return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] Layer {index} changed type from {oldValue} to {newValue}";
        }
    }
}