using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;


namespace IndustrialPark
{
    public class ModelInst : GenericAssetDataContainer
    {
        public static int Size => 56;
        public AssetID Model_AssetID { get; set; }
        public ushort Flags { get; set; }
        public byte Parent { get; set; }
        public byte Bone { get; set; }
        public AssetSingle RightX { get; set; }
        public AssetSingle RightY { get; set; }
        public AssetSingle RightZ { get; set; }
        public AssetSingle UpX { get; set; }
        public AssetSingle UpY { get; set; }
        public AssetSingle UpZ { get; set; }
        public AssetSingle AtX { get; set; }
        public AssetSingle AtY { get; set; }
        public AssetSingle AtZ { get; set; }
        public AssetSingle PosX { get; set; }
        public AssetSingle PosY { get; set; }
        public AssetSingle PosZ { get; set; }

        public ModelInst()
        {
            Model_AssetID = 0;
        }

        public ModelInst(EndianBinaryReader reader)
        {
            Model_AssetID = reader.ReadUInt32();
            Flags = reader.ReadUInt16();
            Parent = reader.ReadByte();
            Bone = reader.ReadByte();
            RightX = reader.ReadSingle();
            RightY = reader.ReadSingle();
            RightZ = reader.ReadSingle();
            UpX = reader.ReadSingle();
            UpY = reader.ReadSingle();
            UpZ = reader.ReadSingle();
            AtX = reader.ReadSingle();
            AtY = reader.ReadSingle();
            AtZ = reader.ReadSingle();
            PosX = reader.ReadSingle();
            PosY = reader.ReadSingle();
            PosZ = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Model_AssetID);
            writer.Write(Flags);
            writer.Write(Parent);
            writer.Write(Bone);
            writer.Write(RightX);
            writer.Write(RightY);
            writer.Write(RightZ);
            writer.Write(UpX);
            writer.Write(UpY);
            writer.Write(UpZ);
            writer.Write(AtX);
            writer.Write(AtY);
            writer.Write(AtZ);
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write(PosZ);

            return writer.ToArray();
        }
    }

    public class AssetMINF : Asset, IAssetWithModel
    {
        private const string categoryName = "Model Info";

        private char[] _magic;
        [Category(categoryName)]
        public char[] Magic
        {
            get => _magic;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _magic = value;
            }
        }
        [Category(categoryName)]
        public AssetID ATBL_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID CombatID { get; set; }
        [Category(categoryName)]
        public AssetID BrainID { get; set; }
        [Category(categoryName)]
        public ModelInst[] ModelReferences { get; set; }
        [Category(categoryName)]
        public byte[] UnknownData { get; set; }

        public AssetMINF(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            var chars = reader.ReadChars(4);
            Magic = reader.endianness == Endianness.Big ? chars : chars.Reverse().ToArray();

            int amountOfReferences = reader.ReadInt32();
            ATBL_AssetID = reader.ReadUInt32();
            if (game != Game.Scooby)
            {
                CombatID = reader.ReadUInt32();
                BrainID = reader.ReadUInt32();
            }

            ModelReferences = new ModelInst[amountOfReferences];
            for (int i = 0; i < ModelReferences.Length; i++)
                ModelReferences[i] = new ModelInst(reader);

            var unknownData = new List<byte>();

            while (!reader.EndOfStream)
                unknownData.Add(reader.ReadByte());
            UnknownData = unknownData.ToArray();

            if (ModelReferences.Length > 0)
                _modelAssetID = ModelReferences[0].Model_AssetID;
            else _modelAssetID = 0;

            if (_modelAssetID != 0)
            {
                AddToRenderingDictionary(AHDR.assetID, this);

                if (game == Game.Incredibles)
                {
                    AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                    AddToNameDictionary(Functions.BKDRHash(newName), newName);
                }
            }

        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(writer.endianness == Endianness.Big ? Magic : Magic.Reverse().ToArray());
            writer.Write(ModelReferences.Length);
            writer.Write(ATBL_AssetID);

            if (game != Game.Scooby)
            {
                writer.Write(CombatID);
                writer.Write(BrainID);
            }

            foreach (var r in ModelReferences)
                writer.Write(r.Serialize(game, platform));
            foreach (var b in UnknownData)
                writer.Write(b);

            return writer.ToArray();
        }

        private string newName => assetName.Replace(".MINF", "");

        private uint _modelAssetID;

        public void MovieRemoveFromDictionary()
        {
            renderingDictionary.Remove(Functions.BKDRHash(newName));
            nameDictionary.Remove(Functions.BKDRHash(newName));
        }

        public override bool HasReference(uint assetID)
        {
            if (ATBL_AssetID == assetID)
                return true;

            foreach (ModelInst m in ModelReferences)
                if (m.Model_AssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(ATBL_AssetID, ref result);
            foreach (ModelInst m in ModelReferences)
            {
                if (m.Model_AssetID == 0)
                    result.Add("MINF model reference with Model_AssetID set to 0");
                Verify(m.Model_AssetID, ref result);
            }
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset)
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset);
            else
                renderer.DrawCube(world, isSelected | isSelected);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => renderingDictionary.ContainsKey(_modelAssetID) ? renderingDictionary[_modelAssetID].SpecialBlendMode : true;

        public RenderWareModelFile GetRenderWareModelFile() => GetFromRenderingDictionary(_modelAssetID);

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("CombatID");
                dt.RemoveProperty("BrainID");
            }

            base.SetDynamicProperties(dt);
        }
    }
}