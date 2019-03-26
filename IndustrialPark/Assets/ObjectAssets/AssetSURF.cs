using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSURF : ObjectAsset
    {
        public AssetSURF(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x12C;

        public override bool HasReference(uint assetID)
        {
            if (BumpMapTexture_AssetID == assetID)
                return true;
            if (EnvMapTexture_AssetID == assetID)
                return true;
            if (DualMapTexture_AssetID == assetID)
                return true;
            if (TextureAnim1_GroupAssetID == assetID)
                return true;
            if (TextureAnim2_GroupAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte DamageType
        {
            get => ReadByte(0x08);
            set => Write(0x08, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Sticky
        {
            get => ReadByte(0x09);
            set => Write(0x09, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte DamageFlags
        {
            get => ReadByte(0x0A);
            set => Write(0x0A, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SurfaceType
        {
            get => ReadByte(0x0B);
            set => Write(0x0B, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Phys_Pad
        {
            get => ReadByte(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SlideStart
        {
            get => ReadByte(0x0D);
            set => Write(0x0D, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SlideStop
        {
            get => ReadByte(0x0E);
            set => Write(0x0E, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte PhysicsFlags
        {
            get => ReadByte(0x0F);
            set => Write(0x0F, value);
        }

        [Category("Surface: Base"), TypeConverter(typeof(FloatTypeConverter))]
        public float Friction
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        
        [Category("Surface: Material Effects")]
        public int MaterialEffectFlags
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Surface: Material Effects")]
        public AssetID BumpMapTexture_AssetID
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Surface: Material Effects")]
        public AssetID EnvMapTexture_AssetID
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Surface: Material Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float Shininess
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Surface: Material Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float Bumpiness
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        [Category("Surface: Material Effects")]
        public AssetID DualMapTexture_AssetID
        {
            get => ReadUInt(0x28);
            set => Write(0x28, value);
        }
        
        [Category("Surface: Color Effects")]
        public short ColorEffectFlags
        {
            get => ReadShort(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Surface: Color Effects")]
        public short ColorEffectMode
        {
            get => ReadShort(0x2E);
            set => Write(0x2E, value);
        }

        [Category("Surface: Color Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float Speed
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Surface: Texture Animation")]
        public int TextureAnimFlags
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Surface: Texture Animation")]
        public short TextureAnim1_Pad38
        {
            get => ReadShort(0x38);
            set => Write(0x38, value);
        }

        [Category("Surface: Texture Animation")]
        public short TextureAnim1_Mode
        {
            get => ReadShort(0x3A);
            set => Write(0x3A, value);
        }

        [Category("Surface: Texture Animation")]
        public AssetID TextureAnim1_GroupAssetID
        {
            get => ReadUInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Surface: Texture Animation"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextureAnim1_Speed
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category("Surface: Texture Animation")]
        public short TextureAnim2_Pad44
        {
            get => ReadShort(0x44);
            set => Write(0x44, value);
        }

        [Category("Surface: Texture Animation")]
        public short TextureAnim2_Mode
        {
            get => ReadShort(0x46);
            set => Write(0x46, value);
        }

        [Category("Surface: Texture Animation")]
        public AssetID TextureAnim2_GroupAssetID
        {
            get => ReadUInt(0x48);
            set => Write(0x48, value);
        }

        [Category("Surface: Texture Animation"), TypeConverter(typeof(FloatTypeConverter))]
        public float TextureAnim2_Speed
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Surface: UV Effects")]
        public int UVEffects_Flags
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Surface: UV Effects")]
        public int UVEffects1_Mode
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Rot
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_RotSpd
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Trans_X
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Trans_Y
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Trans_Z
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_TransSpeed_X
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_TransSpeed_Y
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_TransSpeed_Z
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
               
        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Scale_X
        {
            get => ReadFloat(0x78);
            set => Write(0x78, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Scale_Y
        {
            get => ReadFloat(0x7C);
            set => Write(0x7C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Scale_Z
        {
            get => ReadFloat(0x80);
            set => Write(0x80, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_ScaleSpeed_X
        {
            get => ReadFloat(0x84);
            set => Write(0x84, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_ScaleSpeed_Y
        {
            get => ReadFloat(0x88);
            set => Write(0x88, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_ScaleSpeed_Z
        {
            get => ReadFloat(0x8C);
            set => Write(0x8C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Min_X
        {
            get => ReadFloat(0x90);
            set => Write(0x90, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Min_Y
        {
            get => ReadFloat(0x94);
            set => Write(0x94, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Min_Z
        {
            get => ReadFloat(0x98);
            set => Write(0x98, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Max_X
        {
            get => ReadFloat(0x9C);
            set => Write(0x9C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Max_Y
        {
            get => ReadFloat(0xA0);
            set => Write(0xA0, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_Max_Z
        {
            get => ReadFloat(0xA4);
            set => Write(0xA4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_MinMaxSpeed_X
        {
            get => ReadFloat(0xA8);
            set => Write(0xA8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_MinMaxSpeed_Y
        {
            get => ReadFloat(0xAC);
            set => Write(0xAC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects1_MinMaxSpeed_Z
        {
            get => ReadFloat(0xB0);
            set => Write(0xB0, value);
        }

        [Category("Surface: UV Effects")]
        public int UVEffects2_Mode
        {
            get => ReadInt(0xB4);
            set => Write(0xB4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Rot
        {
            get => ReadFloat(0xB8);
            set => Write(0xB8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_RotSpd
        {
            get => ReadFloat(0xBC);
            set => Write(0xBC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Trans_X
        {
            get => ReadFloat(0xC0);
            set => Write(0xC0, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Trans_Y
        {
            get => ReadFloat(0xC4);
            set => Write(0xC4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Trans_Z
        {
            get => ReadFloat(0xC8);
            set => Write(0xC8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_TransSpeed_X
        {
            get => ReadFloat(0xCC);
            set => Write(0xCC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_TransSpeed_Y
        {
            get => ReadFloat(0xD0);
            set => Write(0xD0, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_TransSpeed_Z
        {
            get => ReadFloat(0xD4);
            set => Write(0xD4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Scale_X
        {
            get => ReadFloat(0xD8);
            set => Write(0xD8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Scale_Y
        {
            get => ReadFloat(0xDC);
            set => Write(0xDC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Scale_Z
        {
            get => ReadFloat(0xE0);
            set => Write(0xE0, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_ScaleSpeed_X
        {
            get => ReadFloat(0xE4);
            set => Write(0xE4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_ScaleSpeed_Y
        {
            get => ReadFloat(0xE8);
            set => Write(0xE8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_ScaleSpeed_Z
        {
            get => ReadFloat(0xEC);
            set => Write(0xEC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Min_X
        {
            get => ReadFloat(0xF0);
            set => Write(0xF0, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Min_Y
        {
            get => ReadFloat(0xF4);
            set => Write(0xF4, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Min_Z
        {
            get => ReadFloat(0xF8);
            set => Write(0xF8, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Max_X
        {
            get => ReadFloat(0xFC);
            set => Write(0xFC, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Max_Y
        {
            get => ReadFloat(0x100);
            set => Write(0x100, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_Max_Z
        {
            get => ReadFloat(0x104);
            set => Write(0x104, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_MinMaxSpeed_X
        {
            get => ReadFloat(0x108);
            set => Write(0x108, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_MinMaxSpeed_Y
        {
            get => ReadFloat(0x10C);
            set => Write(0x10C, value);
        }

        [Category("Surface: UV Effects"), TypeConverter(typeof(FloatTypeConverter))]
        public float UVEffects2_MinMaxSpeed_Z
        {
            get => ReadFloat(0x110);
            set => Write(0x110, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte On
        {
            get => ReadByte(0x114);
            set => Write(0x114, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding115
        {
            get => ReadByte(0x115);
            set => Write(0x115, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding116
        {
            get => ReadByte(0x116);
            set => Write(0x116, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding117
        {
            get => ReadByte(0x117);
            set => Write(0x117, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(FloatTypeConverter))]
        public float OutOfBoundsDelay
        {
            get => ReadFloat(0x118);
            set => Write(0x118, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(FloatTypeConverter))]
        public float WalljumpScaleXZ
        {
            get => ReadFloat(0x11C);
            set => Write(0x11C, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(FloatTypeConverter))]
        public float WalljumpScaleY
        {
            get => ReadFloat(0x120);
            set => Write(0x120, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(FloatTypeConverter))]
        public float DamageTimer
        {
            get => ReadFloat(0x124);
            set => Write(0x124, value);
        }

        [Category("Surface: Other"), TypeConverter(typeof(FloatTypeConverter))]
        public float DamageBounce
        {
            get => ReadFloat(0x128);
            set => Write(0x128, value);
        }
    }
}