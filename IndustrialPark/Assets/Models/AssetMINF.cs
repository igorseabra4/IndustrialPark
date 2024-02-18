using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class MinfReference : GenericAssetDataContainer
    {
        public static int Size => 56;
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
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

        public MinfReference() { }
        public MinfReference(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
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

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Model);
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
        }
    }

    public class MinfParam : GenericAssetDataContainer
    {
        public AssetID Type_Hex { get; set; }
        public EMinfParamType Type_Enum
        {
            get => Enum.GetValues(typeof(EMinfParamType)).Cast<EMinfParamType>().DefaultIfEmpty(EMinfParamType.Unknown).FirstOrDefault(p => Type_Hex.Equals((uint)p));
            set
            {
                Type_Hex = (uint)value;
            }
        }
        public string Value { get; set; }

        public string Value_Note => "Value is a string (in this case, representing one or more numbers), but it's not an actual number - so be careful with the formatting! Anything you type will be accepted";

        public MinfParam()
        {
            Type_Hex = 0;
            Value = "0.0";
        }

        public MinfParam(EndianBinaryReader reader)
        {
            Type_Hex = reader.ReadUInt32();
            var count = reader.ReadByte();
            Value = reader.ReadString(count * 4 + 3);
            Value = Value.Trim('\0');
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Type_Hex);

            var pDataLen = Value.Length + 2;
            while (pDataLen % 4 != 0)
                pDataLen++;

            writer.Write((byte)(pDataLen / 4 - 1));
            writer.Write(Value);

            do
                writer.Write((byte)0);
            while (writer.BaseStream.Length % 4 != 0);
        }

        public override string ToString() => $"[{Type_Enum}] " + Value;
    }

    public class AssetMINF : Asset, IAssetWithModel
    {
        private const string categoryName = "Model Info";
        public override string AssetInfo => string.Join(", ", References.Select(r => HexUIntTypeConverter.StringFromAssetID(r.Model)));

        [Category(categoryName), ValidReferenceRequired]
        public AssetID AnimationTable { get; set; }
        [Category(categoryName)]
        public AssetID CombatID { get; set; }

        private const string categoryNameBrain = categoryName + ": Brain ID";

        [Category(categoryNameBrain)]
        public AssetID BrainID { get; set; }
        [Category(categoryNameBrain)]
        public EBrainID_Movie BrainID_Movie
        {
            get => Enum.GetValues(typeof(EBrainID_Movie)).Cast<EBrainID_Movie>().DefaultIfEmpty(EBrainID_Movie.Unknown).FirstOrDefault(p => BrainID.Equals((uint)p));
            set { BrainID = (uint)value; }
        }
        [Category(categoryNameBrain)]
        public EBrainID_Incredibles BrainID_Incredibles
        {
            get => Enum.GetValues(typeof(EBrainID_Incredibles)).Cast<EBrainID_Incredibles>().DefaultIfEmpty(EBrainID_Incredibles.Unknown).FirstOrDefault(p => BrainID.Equals((uint)p));
            set { BrainID = (uint)value; }
        }
        [Category(categoryNameBrain)]
        public EBrainID_ROTU BrainID_ROTU
        {
            get => Enum.GetValues(typeof(EBrainID_ROTU)).Cast<EBrainID_ROTU>().DefaultIfEmpty(EBrainID_ROTU.Unknown).FirstOrDefault(p => BrainID.Equals((uint)p));
            set { BrainID = (uint)value; }
        }
        [Category(categoryNameBrain)]
        public EBrainID_RatProto BrainID_RatProto
        {
            get => Enum.GetValues(typeof(EBrainID_RatProto)).Cast<EBrainID_RatProto>().DefaultIfEmpty(EBrainID_RatProto.Unknown).FirstOrDefault(p => BrainID.Equals((uint)p));
            set { BrainID = (uint)value; }
        }

        [Category(categoryName)]
        public MinfReference[] References { get; set; }

        private MinfParam[] _parameters;
        [Category(categoryName)]
        public MinfParam[] Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                CreateTransformMatrix();
            }
        }

        public AssetMINF(string assetName) : base(assetName, AssetType.ModelInfo)
        {
            References = new MinfReference[0];
            _parameters = new MinfParam[0];

            AddToDictionary();
        }

        public AssetMINF(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                int amountOfReferences = reader.ReadInt32();
                AnimationTable = reader.ReadUInt32();
                if (game != Game.Scooby)
                {
                    CombatID = reader.ReadUInt32();
                    BrainID = reader.ReadUInt32();
                }

                References = new MinfReference[amountOfReferences];
                for (int i = 0; i < References.Length; i++)
                    References[i] = new MinfReference(reader);

                var mParams = new List<MinfParam>();
                while (!reader.EndOfStream)
                    mParams.Add(new MinfParam(reader));
                _parameters = mParams.ToArray();

                AddToDictionary();
                CreateTransformMatrix();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.WriteMagic("MINF");
            writer.Write(References.Length);
            writer.Write(AnimationTable);

            if (game != Game.Scooby)
            {
                writer.Write(CombatID);
                writer.Write(BrainID);
            }

            foreach (var r in References)
                r.Serialize(writer);
            foreach (var b in _parameters)
                b.Serialize(writer);
        }

        private string newName => assetName.Replace(".MINF", "");

        public void AddToDictionary()
        {
            AddToRenderingDictionary(assetID, this);
            if (game >= Game.Incredibles)
            {
                AddToRenderingDictionary(Functions.BKDRHash(newName), this);
                AddToNameDictionary(Functions.BKDRHash(newName), newName);
            }
        }

        public void RemoveFromDictionary()
        {
            RemoveFromRenderingDictionary(Functions.BKDRHash(newName));
            RemoveFromNameDictionary(Functions.BKDRHash(newName));
        }

        public static bool drawOnlyFirst = false;

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset)
        {
            bool noneDrawn = true;
            foreach (var reference in References)
            {
                uint _model = reference.Model;
                if (renderingDictionary.ContainsKey(_model))
                {
                    renderingDictionary[_model].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, uvAnimOffset);
                    noneDrawn = false;
                }
                if (drawOnlyFirst)
                    break;
            }
            if (noneDrawn)
                renderer.DrawCube(world, isSelected | isSelected);
        }

        private uint _model => References.Length > 0 ? References[0].Model : 0;

        [Browsable(false)]
        public bool SpecialBlendMode => renderingDictionary.ContainsKey(_model) ? renderingDictionary[_model].SpecialBlendMode : true;

        public RenderWareModelFile GetRenderWareModelFile() => GetFromRenderingDictionary(_model);

        [Browsable(false)]
        public Matrix TransformMatrix { get; private set; }

        public void CreateTransformMatrix()
        {
            TransformMatrix = Matrix.Scaling(GetScaleParameter());
        }

        private Vector3 GetScaleParameter()
        {
            foreach (var param in _parameters)
                if (param.Type_Enum == EMinfParamType.ScaleModel)
                    return ParamToVector(param.Value);
            return Vector3.One;
        }

        private static Vector3 ParamToVector(string paramValue)
        {
            var tokens = paramValue.Trim('{', '}', ' ').Replace(" ", "").Split(',');
            var prevScale = new Vector3(Convert.ToSingle(tokens[0]), Convert.ToSingle(tokens[1]), Convert.ToSingle(tokens[2]));
            return prevScale;
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("CombatID");
                dt.RemoveProperty("BrainID");
            }
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("BrainID_Movie");
                dt.RemoveProperty("BrainID_Incredibles");
            }
            if (game != Game.ROTU)
                dt.RemoveProperty("BrainID_ROTU");
            if (game != Game.RatProto)
                dt.RemoveProperty("BrainID_RatProto");

            base.SetDynamicProperties(dt);
        }

        public void ApplyScale(Vector3 scale)
        {
            string VectorToParam(Vector3 v) => $"{{ {v.X:.0###########}, {v.Y:.0###########}, {v.Z:.0###########} }}";

            var mparams = _parameters.ToList();

            var sparamIndex = -1;
            for (int i = 0; i < mparams.Count; i++)
                if (mparams[i].Type_Enum == EMinfParamType.ScaleModel)
                {
                    sparamIndex = i;
                    break;
                }

            if (sparamIndex == -1)
            {
                mparams.Add(new MinfParam() { Type_Enum = EMinfParamType.ScaleModel, Value = VectorToParam(scale) });
            }
            else
            {
                var scaleParam = mparams[sparamIndex];
                Vector3 prevScale = ParamToVector(scaleParam.Value);
                mparams[sparamIndex].Value = VectorToParam(new Vector3(prevScale.X * scale.X, prevScale.Y * scale.Y, prevScale.Z * scale.Z));
            }

            Parameters = mparams.ToArray();
        }
    }
}