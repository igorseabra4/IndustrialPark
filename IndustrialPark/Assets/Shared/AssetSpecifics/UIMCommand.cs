using IndustrialPark.AssetEditorColors;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum UIMCommandType
    {
        Move = 0,
        Scale = 1,
        Rotate = 2,
        Opacity = 3,
        AbsoluteScale = 4,
        Brightness = 5,
        Color = 6,
        UVScroll = 7
    }

    public abstract class UIMCommand : GenericAssetDataContainer
    {
        protected const string categoryName = "UIM Command";

        [Category(categoryName)]
        public AssetSingle StartTime { get; set; }
        [Category(categoryName)]
        public AssetSingle EndTime { get; set; }
        [Category(categoryName)]
        public AssetSingle AccelTime { get; set; }
        [Category(categoryName)]
        public AssetSingle DecelTime { get; set; }
        [Category(categoryName)]
        public bool Enabled { get; set; }

        public UIMCommand()
        {
            Enabled = true;
        }

        public UIMCommand(EndianBinaryReader reader)
        {
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            AccelTime = reader.ReadSingle();
            DecelTime = reader.ReadSingle();
            Enabled = reader.ReadByte() != 0;
            reader.BaseStream.Position += 3;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write(AccelTime);
            writer.Write(DecelTime);
            writer.Write((byte)(Enabled ? 1 : 0));
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class UIMCommand_Move : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Move";

        [Category(categoryNameS)]
        public AssetSingle DistanceX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle DistanceY { get; set; }

        public UIMCommand_Move() : base() { }
        public UIMCommand_Move(EndianBinaryReader reader) : base(reader)
        {
            DistanceX = reader.ReadSingle();
            DistanceY = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Move);
            base.Serialize(writer);
            writer.Write(DistanceX);
            writer.Write(DistanceY);
        }
    }

    public class UIMCommand_Scale : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Scale";

        [Category(categoryNameS)]
        public AssetSingle AmountX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle AmountY { get; set; }
        [Category(categoryNameS)]
        public bool CenterPivot { get; set; }
        [Category(categoryNameS)]
        public AssetSingle CenterOffsetX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle CenterOffsetY { get; set; }

        public UIMCommand_Scale() : base() { }
        public UIMCommand_Scale(EndianBinaryReader reader) : base(reader)
        {
            AmountX = reader.ReadSingle();
            AmountY = reader.ReadSingle();
            CenterPivot = reader.ReadByte() != 0;
            reader.BaseStream.Position += 3;
            CenterOffsetX = reader.ReadSingle();
            CenterOffsetY = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Scale);
            base.Serialize(writer);
            writer.Write(AmountX);
            writer.Write(AmountY);
            writer.Write((byte)(CenterPivot ? 1 : 0));
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(CenterOffsetX);
            writer.Write(CenterOffsetY);
        }
    }

    public class UIMCommand_Rotate : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Rotate";

        [Category(categoryNameS)]
        public AssetSingle Rotation { get; set; }
        [Category(categoryNameS)]
        public AssetSingle CenterOffsetX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle CenterOffsetY { get; set; }

        public UIMCommand_Rotate() : base() { }
        public UIMCommand_Rotate(EndianBinaryReader reader) : base(reader)
        {
            Rotation = reader.ReadSingle();
            CenterOffsetX = reader.ReadSingle();
            CenterOffsetY = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Rotate);
            base.Serialize(writer);
            writer.Write(Rotation);
            writer.Write(CenterOffsetX);
            writer.Write(CenterOffsetY);
        }
    }

    public class UIMCommand_Opacity : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Opacity";

        [Category(categoryNameS)]
        public byte StartOpacity { get; set; }
        [Category(categoryNameS)]
        public byte EndOpacity { get; set; }

        public UIMCommand_Opacity() : base() { }
        public UIMCommand_Opacity(EndianBinaryReader reader) : base(reader)
        {
            StartOpacity = reader.ReadByte();
            EndOpacity = reader.ReadByte();
            reader.BaseStream.Position += 2;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Opacity);
            base.Serialize(writer);
            writer.Write(StartOpacity);
            writer.Write(EndOpacity);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class UIMCommand_AbsoluteScale : UIMCommand
    {
        private const string categoryNameS = categoryName + ": AbsoluteScale";

        [Category(categoryNameS)]
        public AssetSingle StartX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle StartY { get; set; }
        [Category(categoryNameS)]
        public AssetSingle EndX { get; set; }
        [Category(categoryNameS)]
        public AssetSingle EndY { get; set; }
        [Category(categoryNameS)]
        public bool CenterPivot { get; set; }
        [Category(categoryNameS)]
        public AssetByte TextScale { get; set; }

        public UIMCommand_AbsoluteScale() : base() { }
        public UIMCommand_AbsoluteScale(EndianBinaryReader reader) : base(reader)
        {
            StartX = reader.ReadSingle();
            StartY = reader.ReadSingle();
            EndX = reader.ReadSingle();
            EndY = reader.ReadSingle();
            CenterPivot = reader.ReadByte() != 0;
            TextScale = reader.ReadByte();
            reader.BaseStream.Position += 2;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.AbsoluteScale);
            base.Serialize(writer);
            writer.Write(StartX);
            writer.Write(StartY);
            writer.Write(EndX);
            writer.Write(EndY);
            writer.Write((byte)(Enabled ? 1 : 0));
            writer.Write(TextScale);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class UIMCommand_Brightness : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Brightness";

        [Category(categoryNameS)]
        public byte StartBrightness { get; set; }
        [Category(categoryNameS)]
        public byte EndBrightness { get; set; }

        public UIMCommand_Brightness() : base() { }
        public UIMCommand_Brightness(EndianBinaryReader reader) : base(reader)
        {
            StartBrightness = reader.ReadByte();
            EndBrightness = reader.ReadByte();
            reader.BaseStream.Position += 2;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Brightness);
            base.Serialize(writer);
            writer.Write(StartBrightness);
            writer.Write(EndBrightness);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class UIMCommand_Color : UIMCommand
    {
        private const string categoryNameS = categoryName + ": Color";

        private byte _startColorR;
        private byte _startColorG;
        private byte _startColorB;
        private byte _endColorR;
        private byte _endColorG;
        private byte _endColorB;
        [Category(categoryNameS)]
        public AssetColor StartColor
        {
            get => new AssetColor(_startColorR, _startColorG, _startColorB, 0xFF);
            set
            {
                _startColorR = value.R;
                _startColorG = value.G;
                _startColorB = value.B;
            }
        }
        [Category(categoryNameS)]
        public AssetColor EndColor
        {
            get => new AssetColor(_endColorR, _endColorG, _endColorB, 0xFF);
            set
            {
                _endColorR = value.R;
                _endColorG = value.G;
                _endColorB = value.B;
            }
        }

        public UIMCommand_Color() : base()
        {
            StartColor = new AssetColor(-1);
            EndColor = new AssetColor(-1);
        }

        public UIMCommand_Color(EndianBinaryReader reader) : base(reader)
        {
            _startColorR = reader.ReadByte();
            _startColorG = reader.ReadByte();
            _startColorB = reader.ReadByte();
            _endColorR = reader.ReadByte();
            _endColorG = reader.ReadByte();
            _endColorB = reader.ReadByte();
            reader.BaseStream.Position += 2;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.Color);
            base.Serialize(writer);
            writer.Write(_startColorR);
            writer.Write(_startColorG);
            writer.Write(_startColorB);
            writer.Write(_endColorR);
            writer.Write(_endColorG);
            writer.Write(_endColorB);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class UIMCommand_UVScroll : UIMCommand
    {
        private const string categoryNameS = categoryName + ": UVScroll";

        [Category(categoryNameS)]
        public AssetSingle AmountU { get; set; }
        [Category(categoryNameS)]
        public AssetSingle AmountV { get; set; }

        public UIMCommand_UVScroll() : base() { }
        public UIMCommand_UVScroll(EndianBinaryReader reader) : base(reader)
        {
            AmountU = reader.ReadSingle();
            AmountV = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)UIMCommandType.UVScroll);
            base.Serialize(writer);
            writer.Write(AmountU);
            writer.Write(AmountV);
        }
    }
}
