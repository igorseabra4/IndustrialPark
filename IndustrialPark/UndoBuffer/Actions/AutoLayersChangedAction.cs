namespace IndustrialPark.Archive
{
    public class AutoLayersChangedAction : IReversibleAction
    {
        private LayerRepository layerRepository;
        private bool value;

        public AutoLayersChangedAction(LayerRepository layerRepository, bool value)
        {
            this.layerRepository = layerRepository;
            this.value = value;
        }

        public void Redo()
        {
            layerRepository.AutoLayers = value;
        }

        public void Undo()
        {
            layerRepository.AutoLayers = !value;
        }

        public override string ToString()
        {
            return $"Auto Layers {(value ? "enabled" : "disabled")}";
        }
    }
}