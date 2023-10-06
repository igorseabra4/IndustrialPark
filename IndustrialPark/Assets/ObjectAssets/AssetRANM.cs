using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class ReactiveAnimation : GenericAssetDataContainer
    {
        public AssetID ModelStatic { get; set; }
        public AssetID ModelBound { get; set; }
        public AssetSingle LevelOfDetailDistance { get; set; }
        public AssetID AnimationIdle { get; set; }
        public AssetID AnimationMoveThrough { get; set; }
        public AssetID AnimationHit { get; set; }
        public AssetID SoundGroupIdle { get; set; }
        public AssetID SoundGroupMoveThrough { get; set; }
        public AssetID SoundGroupHit { get; set; }
        public AssetID ModelBurn { get; set; }
        public AssetID AnimationBurn { get; set; }
        public AssetSingle BurnFuel { get; set; }
        public AssetSingle BurnFlameSize { get; set; }
        public AssetSingle BurnEmitScale { get; set; }
        public AssetSingle MoveThroughRadius { get; set; }

        public ReactiveAnimation() { }
        public ReactiveAnimation(EndianBinaryReader reader)
        {
            ModelStatic = reader.ReadUInt32();
            ModelBound = reader.ReadUInt32();
            LevelOfDetailDistance = reader.ReadSingle();
            AnimationIdle = reader.ReadUInt32();
            AnimationMoveThrough = reader.ReadUInt32();
            AnimationHit = reader.ReadUInt32();
            SoundGroupIdle = reader.ReadUInt32();
            SoundGroupMoveThrough = reader.ReadUInt32();
            SoundGroupHit = reader.ReadUInt32();
            ModelBurn = reader.ReadUInt32();
            AnimationBurn = reader.ReadUInt32();
            BurnFuel = reader.ReadSingle();
            BurnFlameSize = reader.ReadSingle();
            BurnEmitScale = reader.ReadSingle();
            MoveThroughRadius = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(ModelStatic);
            writer.Write(ModelBound);
            writer.Write(LevelOfDetailDistance);
            writer.Write(AnimationIdle);
            writer.Write(AnimationMoveThrough);
            writer.Write(AnimationHit);
            writer.Write(SoundGroupIdle);
            writer.Write(SoundGroupMoveThrough);
            writer.Write(SoundGroupHit);
            writer.Write(ModelBurn);
            writer.Write(AnimationBurn);
            writer.Write(BurnFuel);
            writer.Write(BurnFlameSize);
            writer.Write(BurnEmitScale);
            writer.Write(MoveThroughRadius);
        }
    }

    public class AssetRANM : BaseAsset
    {
        private const string categoryName = "Reactive Animation";
        public override string AssetInfo => ItemsString(ReactiveAnimations.Length, "animation");

        [Category(categoryName)]
        public int Version { get; set; }
        [Category(categoryName)]
        public ReactiveAnimation[] ReactiveAnimations { get; set; }

        public AssetRANM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Version = reader.ReadInt32();
                var count = reader.ReadInt32();
                ReactiveAnimations = new ReactiveAnimation[count];
                for (int i = 0; i < ReactiveAnimations.Length; i++)
                    ReactiveAnimations[i] = new ReactiveAnimation(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
            writer.Write(ReactiveAnimations.Length);
            foreach (var ra in ReactiveAnimations)
                ra.Serialize(writer);
            SerializeLinks(writer);
        }
    }
}