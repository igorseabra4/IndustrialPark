using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyCritter : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaEnemyCritter() : base()
        {
        }

        public DynaEnemyCritter(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Unknown00 = Switch(BitConverter.ToInt32(Data, 0x0));
            Unknown04 = Data[0x04];
            Unknown05 = Data[0x05];
            Flags06 = Switch(BitConverter.ToInt16(Data, 0x06));
            VisibilityFlag = Data[0x08];
            TypeFlag = Data[0x08];
            UnknownFlag0A = Data[0x08];
            SolidityFlag = Data[0x08];
            Surface_AssetID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            _yaw = Switch(BitConverter.ToSingle(Data, 0x10));
            _pitch = Switch(BitConverter.ToSingle(Data, 0x14));
            _roll = Switch(BitConverter.ToSingle(Data, 0x18));
            _position.X = Switch(BitConverter.ToSingle(Data, 0x1C));
            _position.Y = Switch(BitConverter.ToSingle(Data, 0x20));
            _position.Z = Switch(BitConverter.ToSingle(Data, 0x24));
            _scale.X = Switch(BitConverter.ToSingle(Data, 0x28));
            _scale.Y = Switch(BitConverter.ToSingle(Data, 0x2C));
            _scale.Z = Switch(BitConverter.ToSingle(Data, 0x30));
            ColorRed = Switch(BitConverter.ToSingle(Data, 0x34));
            ColorGreen = Switch(BitConverter.ToSingle(Data, 0x38));
            ColorBlue = Switch(BitConverter.ToSingle(Data, 0x3C));
            ColorAlpha = Switch(BitConverter.ToSingle(Data, 0x40));
            Unknown44 = Switch(BitConverter.ToUInt32(Data, 0x44));
            Model_AssetID = Switch(BitConverter.ToUInt32(Data, 0x48));
            Unknown4C = Switch(BitConverter.ToUInt32(Data, 0x4C));
            MVPT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            Unknown54 = Switch(BitConverter.ToUInt32(Data, 0x54));
            //Unknown54 = 0;

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (Surface_AssetID == assetID)
                return true;
            if (Unknown44 == assetID)
                return true;
            if (Model_AssetID == assetID)
                return true;
            if (Unknown4C == assetID)
                return true;
            if (MVPT_AssetID == assetID)
                return true;
            if (Unknown54 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Unknown00)));
            list.Add(Unknown04);
            list.Add(Unknown05);
            list.AddRange(BitConverter.GetBytes(Switch(Flags06)));
            list.Add(VisibilityFlag);
            list.Add(TypeFlag);
            list.Add(UnknownFlag0A);
            list.Add(SolidityFlag);
            list.AddRange(BitConverter.GetBytes(Switch(Surface_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(Pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(Roll)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleX)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleY)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorRed)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorGreen)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorBlue)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorAlpha)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
            list.AddRange(BitConverter.GetBytes(Switch(Model_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown54)));
            return list.ToArray();
        }

        [Category("Dyna Enemy Critter")]
        public AssetID Unknown44 { get; set; }
        
        [Category("Dyna Enemy Critter")]
        public AssetID Unknown4C { get; set; }

        [Category("Dyna Enemy Critter")]
        public AssetID MVPT_AssetID { get; set; }

        [Category("Dyna Enemy Critter")]
        public AssetID Unknown54 { get; set; }
    }
}