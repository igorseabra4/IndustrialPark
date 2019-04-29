using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class ModelReference
    {
        public AssetID Model_AssetID { get; set; }
        public AssetID UnknownAssetID { get; set; }
        public float UnknownFloat01 { get; set; }
        public float UnknownFloat02 { get; set; }
        public float UnknownFloat03 { get; set; }
        public float UnknownFloat04 { get; set; }
        public float UnknownFloat05 { get; set; }
        public float UnknownFloat06 { get; set; }
        public float UnknownFloat07 { get; set; }
        public float UnknownFloat08 { get; set; }
        public float UnknownFloat09 { get; set; }
        public float UnknownFloat10 { get; set; }
        public float UnknownFloat11 { get; set; }
        public float UnknownFloat12 { get; set; }

        public ModelReference()
        {
            Model_AssetID = 0;
            UnknownAssetID = 0;
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

                ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);

                if (Functions.currentGame == Game.Incredibles)
                {
                    ArchiveEditorFunctions.AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                    ArchiveEditorFunctions.AddToNameDictionary(Functions.BKDRHash(newName), newName);
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
            ArchiveEditorFunctions.renderingDictionary.Remove(Functions.BKDRHash(newName));
            ArchiveEditorFunctions.nameDictionary.Remove(Functions.BKDRHash(newName));
        }

        public override bool HasReference(uint assetID)
        {
            if (ATBL_AssetID == assetID)
                return true;

            foreach (ModelReference m in ModelReferences)
            {
                if (m.Model_AssetID == assetID)
                    return true;
                if (m.UnknownAssetID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(ATBL_AssetID, ref result);
            foreach (ModelReference m in ModelReferences)
            {
                if (m.Model_AssetID == 0)
                    result.Add("MINF model reference with Model_AssetID set to 0");
                Verify(m.Model_AssetID, ref result);
            }
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color);
            }
            else
            {
                renderer.DrawCube(world, isSelected |isSelected);
            }
        }

        public RenderWareModelFile GetRenderWareModelFile()
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                return ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile();
            }
            else
            {
                throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its RenderWareModelFile");
            }
        }

        public bool HasRenderWareModelFile()
        {
            return ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) && ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null;
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
        public int UnknownInt0C
        {
            get
            {
                if (Functions.currentGame != Game.Scooby)
                    return ReadInt(0x0C);
                return 0;
            }
            set
            {
                if (Functions.currentGame != Game.Scooby)
                    Write(0x0C, value);
            }
        }

        [Category("Model Info")]
        public int UnknownInt10
        {
            get
            {
                if (Functions.currentGame != Game.Scooby)
                    return ReadInt(0x10);
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
        public ModelReference[] ModelReferences
        {
            get
            {
                List<ModelReference> references = new List<ModelReference>();

                for (int i = 0; i < AmountOfReferences; i++)
                {
                    references.Add(new ModelReference()
                    {
                        Model_AssetID = ReadUInt(ModelReferencesStart + i * 56),
                        UnknownAssetID = ReadUInt(ModelReferencesStart + i * 56 + 0x04),
                        UnknownFloat01 = ReadFloat(ModelReferencesStart + i * 56 + 0x08),
                        UnknownFloat02 = ReadFloat(ModelReferencesStart + i * 56 + 0x0C),
                        UnknownFloat03 = ReadFloat(ModelReferencesStart + i * 56 + 0x10),
                        UnknownFloat04 = ReadFloat(ModelReferencesStart + i * 56 + 0x14),
                        UnknownFloat05 = ReadFloat(ModelReferencesStart + i * 56 + 0x18),
                        UnknownFloat06 = ReadFloat(ModelReferencesStart + i * 56 + 0x1C),
                        UnknownFloat07 = ReadFloat(ModelReferencesStart + i * 56 + 0x20),
                        UnknownFloat08 = ReadFloat(ModelReferencesStart + i * 56 + 0x24),
                        UnknownFloat09 = ReadFloat(ModelReferencesStart + i * 56 + 0x28),
                        UnknownFloat10 = ReadFloat(ModelReferencesStart + i * 56 + 0x2C),
                        UnknownFloat11 = ReadFloat(ModelReferencesStart + i * 56 + 0x30),
                        UnknownFloat12 = ReadFloat(ModelReferencesStart + i * 56 + 0x34),
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

                foreach (ModelReference m in value)
                {
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.Model_AssetID)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownAssetID)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat01)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat02)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat03)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat04)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat05)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat06)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat07)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat08)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat09)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat10)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat11)));
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.UnknownFloat12)));
                }

                before.AddRange(Data.Skip(EndOfModelReferenceData));

                Data = before.ToArray();

                AmountOfReferences = value.Length;
            }
        }

        private int EndOfModelReferenceData => ModelReferencesStart + 56 * AmountOfReferences;

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
                List<byte> before = new List<byte>();

                before.AddRange(Data.Take(EndOfModelReferenceData));

                foreach (AssetID a in value)
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(a)));

                Data = before.ToArray();
            }
        }
    }
}