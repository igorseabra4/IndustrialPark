namespace IndustrialPark
{
    public class CollWrapper
    {
        public EntryCOLL Entry;

        public CollWrapper(EntryCOLL entry)
        {
            Entry = entry;
        }

        public AssetID CollisionModel
        {
            get => Entry.CollisionModel;
            set => Entry.CollisionModel = value;
        }

        public AssetID CameraCollisionModel
        {
            get => Entry.CameraCollisionModel;
            set => Entry.CameraCollisionModel = value;
        }
    }
}
