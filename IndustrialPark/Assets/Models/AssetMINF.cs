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
        public AssetID ModelAssetID { get; set; }
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
            ModelAssetID = 0;
            UnknownAssetID = 0;
        }
    }

    public class AssetMINF : Asset, IAssetWithModel
    {
        public AssetMINF(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup()
        {
            try
            {
                _modelAssetID = ReadUInt(0x14);
                ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
            }
            catch
            {
                _modelAssetID = 0;
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (ATBL_AssetID == assetID)
                return true;

            foreach (ModelReference m in ModelReferences)
            {
                if (m.ModelAssetID == assetID)
                    return true;
                if (m.UnknownAssetID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
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
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Model Info")]
        public int UnknownInt10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

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
                        ModelAssetID = ReadUInt(0x14 + i * 56),
                        UnknownAssetID = ReadUInt(0x14 + i * 56 + 0x04),
                        UnknownFloat01 = ReadFloat(0x14 + i * 56 + 0x08),
                        UnknownFloat02 = ReadFloat(0x14 + i * 56 + 0x0C),
                        UnknownFloat03 = ReadFloat(0x14 + i * 56 + 0x10),
                        UnknownFloat04 = ReadFloat(0x14 + i * 56 + 0x14),
                        UnknownFloat05 = ReadFloat(0x14 + i * 56 + 0x18),
                        UnknownFloat06 = ReadFloat(0x14 + i * 56 + 0x1C),
                        UnknownFloat07 = ReadFloat(0x14 + i * 56 + 0x20),
                        UnknownFloat08 = ReadFloat(0x14 + i * 56 + 0x24),
                        UnknownFloat09 = ReadFloat(0x14 + i * 56 + 0x28),
                        UnknownFloat10 = ReadFloat(0x14 + i * 56 + 0x2C),
                        UnknownFloat11 = ReadFloat(0x14 + i * 56 + 0x30),
                        UnknownFloat12 = ReadFloat(0x14 + i * 56 + 0x34),
                    });
                }

                return references.ToArray();
            }
            set
            {
                List<byte> before = new List<byte>();

                before.AddRange(Data.Take(0x14));

                if (value.Length > 0)
                    _modelAssetID = value[0].ModelAssetID;

                foreach (ModelReference m in value)
                {
                    before.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(m.ModelAssetID)));
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

        private int EndOfModelReferenceData => 0x14 + 56 * AmountOfReferences;

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