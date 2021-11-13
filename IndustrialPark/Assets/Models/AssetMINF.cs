using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public enum MinfAssetParam : uint
    {
        Unknown = 0,
        AlertTime = 0xE1F06C25,
        AttackFOV = 0x542DEDB9,
        AttackFrames01 = 0x08DD3607,
        AttackFrames01a = 0x8932A5D6,
        AttackFrames01b = 0x8932A5D7,
        AttackFrames02 = 0x08DD3608,
        AttackFrames02a = 0x8932A659,
        AttackFrames02b = 0x8932A65A,
        AttackFrames03 = 0x08DD3609,
        AttackFrames03a = 0x8932A6DC,
        AttackFrames03b = 0x8932A6DD,
        AttackPeriod = 0x7E0D772D,
        AttackRadius = 0x2CFEF976,
        AttackSize01 = 0xB7E2838E,
        Bogus_Share = 0x21471CD4,
        BoundMainCenter = 0x7FC3A6D8,
        BoundMainExtent = 0xC51AAE79,
        BoundMainIsBox = 0x897929FA,
        DelayFidget = 0xCE24D474,
        DetectHeight = 0x3527643A,
        DetectOffset = 0x26EF64C2,
        DetectRadius = 0xC1708625,
        DistShadowCast = 0xAD40A895,
        Empty = 0xC5989999,
        EndTag_INIOnly = 0x4E7E7FA0,
        EndTag_PropsOnly = 0xFAC5F4FC,
        EndTag_Shared = 0xADC60725,
        EsteemSlotA = 0xE41EF9D0,
        EsteemSlotB = 0xE41EF9D1,
        EsteemSlotC = 0xE41EF9D2,
        EsteemSlotD = 0xE41EF9D3,
        EsteemSlotE = 0xE41EF9D4,
        FactorAccel = 0x760DAEFD,
        FactorDrift = 0xACBAF0C6,
        FactorElasticity = 0x5E65A9CC,
        FactorGravKnoc = 0x6BB17A76,
        FactorMass = 0x1BE96B2B,
        FirstMovepoint = 0x10DB9FC3,
        HitPoints = 0x716A0BCE,
        IsBucketHead = 0x5505FA6C,
        MoveSpeed = 0x9CFD776A,
        NonRandomTalkAnims = 0x870AB6C2,
        ScaleModel = 0x50E7A4A3,
        ShadowCacheRadius = 0x22822228,
        ShadowRasterRadius = 0xDF02CEFF,
        Shrapnel = 0x98615EC9,
        SoundRadius = 0x77E1B6C1,
        StunTime = 0xE807BCBB,
        TestCount = 0x282BF925,
        TurnSpeed = 0x1DB516BA,
        VtxAttack = 0x60FF93CA,
        VtxAttack1 = 0xA2C8A08F,
        VtxAttack2 = 0xA2C8A090,
        VtxAttack3 = 0xA2C8A091,
        VtxAttack4 = 0xA2C8A092,
        VtxAttackBase = 0x3D1A90E7,
        VtxDmgFlameA = 0x9BAE0B2E,
        VtxDmgFlameB = 0x9BAE0B2F,
        VtxDmgFlameC = 0x9BAE0B30,
        VtxDmgSmokeA = 0x74D34EBE,
        VtxDmgSmokeB = 0x74D34EBF,
        VtxDmgSmokeC = 0x74D34EC0,
        VtxExhaust = 0x91046CD0,
        VtxEyeball = 0x57FCA29A,
        VtxGen01 = 0xC3E4972B,
        VtxGen02 = 0xC3E4972C,
        VtxGen03 = 0xC3E4972D,
        VtxGen04 = 0xC3E4972E,
        VtxGen05 = 0xC3E4972F,
        VtxPropel = 0xF9FCFD6C,
    }

    public class MinfReference : GenericAssetDataContainer
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

        public MinfReference() { }
        public MinfReference(EndianBinaryReader reader)
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
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
    }

    public class MinfParam
    {
        public AssetID Type_Hex { get; set; }
        public MinfAssetParam Type_Enum
        {
            get
            {
                foreach (MinfAssetParam map in Enum.GetValues(typeof(MinfAssetParam)))
                    if ((uint)map == Type_Hex)
                        return map;
                return MinfAssetParam.Unknown;
            }
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

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Type_Hex);

                var pDataLen = Value.Length + 2;
                while (pDataLen % 4 != 0)
                    pDataLen++;

                writer.Write((byte)(pDataLen / 4 - 1));
                writer.Write(Value);

                do writer.Write((byte)0);
                while (writer.BaseStream.Length % 4 != 0);

                return writer.ToArray();
            }
        }

        public override string ToString() => $"[{Type_Enum}] " + Value;
    }

    public class AssetMINF : Asset, IAssetWithModel
    {
        private const string categoryName = "Model Info";

        [Category(categoryName)]
        public AssetID ATBL_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID CombatID { get; set; }
        [Category(categoryName)]
        public AssetID BrainID { get; set; }
        [Category(categoryName)]
        public MinfReference[] MinfReferences { get; set; }
        [Category(categoryName)]
        public MinfParam[] MinfParams { get; set; }

        public AssetMINF(string assetName) : base(assetName, AssetType.MINF)
        {
            MinfReferences = new MinfReference[0];
            MinfParams = new MinfParam[0];
        }

        public AssetMINF(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                int amountOfReferences = reader.ReadInt32();
                ATBL_AssetID = reader.ReadUInt32();
                if (game != Game.Scooby)
                {
                    CombatID = reader.ReadUInt32();
                    BrainID = reader.ReadUInt32();
                }

                MinfReferences = new MinfReference[amountOfReferences];
                for (int i = 0; i < MinfReferences.Length; i++)
                    MinfReferences[i] = new MinfReference(reader);

                var mParams = new List<MinfParam>();
                while (!reader.EndOfStream)
                    mParams.Add(new MinfParam(reader));
                MinfParams = mParams.ToArray();

                if (MinfReferences.Length > 0)
                    _modelAssetID = MinfReferences[0].Model_AssetID;
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
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.WriteMagic("MINF");
                writer.Write(MinfReferences.Length);
                writer.Write(ATBL_AssetID);

                if (game != Game.Scooby)
                {
                    writer.Write(CombatID);
                    writer.Write(BrainID);
                }

                foreach (var r in MinfReferences)
                    writer.Write(r.Serialize(game, endianness));
                foreach (var b in MinfParams)
                    writer.Write(b.Serialize(endianness));

                return writer.ToArray();
            }
        }

        private string newName => assetName.Replace(".MINF", "");

        private uint _modelAssetID;

        public void MovieRemoveFromDictionary()
        {
            RemoveFromRenderingDictionary(Functions.BKDRHash(newName));
            RemoveFromNameDictionary(Functions.BKDRHash(newName));
        }

        public override bool HasReference(uint assetID)
        {
            if (ATBL_AssetID == assetID)
                return true;

            foreach (MinfReference m in MinfReferences)
                if (m.Model_AssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(ATBL_AssetID, ref result);
            foreach (MinfReference m in MinfReferences)
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