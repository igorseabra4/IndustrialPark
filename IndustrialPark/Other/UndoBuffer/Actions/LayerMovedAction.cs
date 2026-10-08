using System.IO;

namespace IndustrialPark
{
    public class LayerMovedAction : IReversibleAction
    {
        private ArchiveEditorFunctions archive;
        private int index;
        private bool isDown;

        public LayerMovedAction(ArchiveEditorFunctions archive, int index, bool isDown)
        {
            this.archive = archive;
            this.index = index;
            this.isDown = isDown;
        }

        public void Undo()
        {
            if (isDown)
                archive.MoveLayerUp(index);
            else
                archive.MoveLayerDown(index);
        }

        public void Redo()
        {
            if (isDown)
                archive.MoveLayerDown(index - 1);
            else
                archive.MoveLayerUp(index + 1);
        }

        public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

        public override string ToString()
        {
            return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] Layer {index} moved {(isDown ? "down" : "up")}";
        }
    }
}