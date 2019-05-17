using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class ModelInst
    {
        public static int Size => 56;
        public AssetID Model_AssetID { get; set; }
        public ushort Flags { get; set; }
        public byte Parent { get; set; }
        public byte Bone { get; set; }
        public float RightX { get; set; }
        public float RightY { get; set; }
        public float RightZ { get; set; }
        public float UpX { get; set; }
        public float UpY { get; set; }
        public float UpZ { get; set; }
        public float AtX { get; set; }
        public float AtY { get; set; }
        public float AtZ { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public ModelInst()
        {
            Model_AssetID = 0;
        }
    }

    public class AssetMINF : Asset, IAssetWithModel
    {
        public AssetMINF(Section_AHDR AHDR) : base(AHDR)
        {
            try
            {
                if (Functions.currentGame == Game.Scooby)
                    _modelAssetID = ReadUInt(0x0C);
                else
                    _modelAssetID = ReadUInt(0x14);

                AddToRenderingDictionary(AHDR.assetID, this);

                if (Functions.currentGame == Game.Incredibles)
                {
                    AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                    AddToNameDictionary(Functions.BKDRHash(newName), newName);
                }
            }
            catch
            {
                _modelAssetID = 0;
            }
        }

        private string newName => AHDR.ADBG.assetName.Replace(".MINF", "");

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
            {
                if (m.Model_AssetID == assetID)
                    return true;
            }

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
                renderer.DrawCube(world, isSelected |isSelected);
        }

        public RenderWareModelFile GetRenderWareModelFile()
        {
            if (renderingDictionary.ContainsKey(_modelAssetID))
                return renderingDictionary[_modelAssetID].GetRenderWareModelFile();

            throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its RenderWareModelFile");
        }

        public bool HasRenderWareModelFile()
        {
            return renderingDictionary.ContainsKey(_modelAssetID) && renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null;
        }

        private uint _modelAssetID;

        [Category("Model Info"), ReadOnly(true)]
        public int AmountOfReferences
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Model Info")]
        public AssetID ATBL_AssetID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Model Info")]
        public AssetID CombatID
        {
            get
            {
                if (Functions.currentGame != Game.Scooby)
                    return ReadUInt(0x0C);
                return 0;
            }
            set
            {
                if (Functions.currentGame != Game.Scooby)
                    Write(0x0C, value);
            }
        }

        [Category("Model Info")]
        public AssetID BrainID
        {
            get
            {
                if (Functions.currentGame != Game.Scooby)
                    return ReadUInt(0x10);
                return 0;
            }
            set
            {
                if (Functions.currentGame != Game.Scooby)
                    Write(0x10, value);
            }
        }

        private int ModelReferencesStart => Functions.currentGame == Game.Scooby ? 0xC : 0x14;

        [Category("Model Info")]
        public ModelInst[] ModelReferences
        {
            get
            {
                List<ModelInst> references = new List<ModelInst>();

                for (int i = 0; i < AmountOfReferences; i++)
                {
                    references.Add(new ModelInst()
                    {
                        Model_AssetID = ReadUInt(ModelReferencesStart + i * ModelInst.Size),
                        Flags = ReadUShort(ModelReferencesStart + i * ModelInst.Size + 0x04),
                        Parent = ReadByte(ModelReferencesStart + i * ModelInst.Size + 0x06),
                        Bone = ReadByte(ModelReferencesStart + i * ModelInst.Size + 0x07),
                        RightX = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x08),
                        RightY = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x0C),
                        RightZ = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x10),
                        UpX = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x14),
                        UpY = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x18),
                        UpZ = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x1C),
                        AtX = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x20),
                        AtY = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x24),
                        AtZ = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x28),
                        PosX = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x2C),
                        PosY = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x30),
                        PosZ = ReadFloat(ModelReferencesStart + i * ModelInst.Size + 0x34),
                    });
                }

                return references.ToArray();
            }
            set
            {
                List<byte> before = new List<byte>();

                before.AddRange(Data.Take(ModelReferencesStart));

                if (value.Length > 0)
                    _modelAssetID = value[0].Model_AssetID;

                foreach (ModelInst m in value)
                {
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.Model_AssetID)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.Flags)));
                    before.Add(m.Parent);
                    before.Add(m.Bone);
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.RightX)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.RightY)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.RightZ)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UpX)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UpY)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UpZ)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.AtX)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.AtY)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.AtZ)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.PosX)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.PosY)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.PosZ)));
                }

                before.AddRange(Data.Skip(EndOfModelReferenceData));

                Data = before.ToArray();

                AmountOfReferences = value.Length;
            }
        }

        private int EndOfModelReferenceData => ModelReferencesStart + ModelInst.Size * AmountOfReferences;

        [Category("Model Info")]
        public AssetID[] RestOfData
        {
            get
            {
                List<AssetID> list = new List<AssetID>();

                for (int i = EndOfModelReferenceData; i < Data.Length; i += 4)
                    list.Add(ReadUInt(i));

                return list.ToArray();
            }
            set
            {
                List<byte> before = Data.Take(EndOfModelReferenceData).ToList();

                foreach (AssetID a in value)
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(a)));

                Data = before.ToArray();
            }
        }
    }
}