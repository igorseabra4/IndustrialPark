using AssetEditorColors;
using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace IndustrialPark
{
    public class AssetUIFT : AssetUI
    {
        private const string categoryName = "User Interface Font";

        [Category(categoryName)]
        public FlagBitmask UIFontFlags { get; set; } = ShortFlagsDescriptor(
              "Align Left",
              "Align Right",
              "Align Center",
              "Show Background");
        [Category(categoryName)]
        public byte UIFontMode { get; set; }
        [Category(categoryName)]
        public FontEnum FontID { get; set; }
        [Category(categoryName)]
        public AssetID Text { get; set; }
        [Category(categoryName)]
        public AssetColor BackgroundColor { get; set; }
        [Category(categoryName)]
        public AssetColor FontColor { get; set; }
        [Category(categoryName)]
        public short Padding_Top { get; set; }
        [Category(categoryName)]
        public short Padding_Bottom { get; set; }
        [Category(categoryName)]
        public short Padding_Left { get; set; }
        [Category(categoryName)]
        public short Padding_Right { get; set; }
        [Category(categoryName)]
        public short Spacing_Horizontal { get; set; }
        [Category(categoryName)]
        public short Spacing_Vertical { get; set; }
        [Category(categoryName)]
        public short Char_Width { get; set; }
        [Category(categoryName)]
        public short Char_Height { get; set; }
        [Category(categoryName)]
        public int MaxHeight { get; set; } // not in scooby

        public AssetUIFT(string assetName, Vector3 position) : base(assetName, AssetType.UserInterfaceFont, BaseAssetType.UIFont, position)
        {
            BackgroundColor = new AssetColor(128, 128, 128, 128);
            FontColor = new AssetColor(255, 255, 255, 255);
            Padding_Top = 2;
            Padding_Bottom = 2;
            Padding_Left = 2;
            Padding_Right = 2;
            Spacing_Horizontal = 24;
            Spacing_Vertical = 24;
            Char_Width = 24;
            Char_Height = 24;
        }

        public AssetUIFT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = game == Game.BFBB ? 0x80 : 0x7C;

                UIFontFlags.FlagValueShort = reader.ReadUInt16();
                UIFontMode = reader.ReadByte();
                FontID = (FontEnum)reader.ReadByte();
                Text = reader.ReadUInt32();
                BackgroundColor = reader.ReadColor();
                FontColor = reader.ReadColor();
                Padding_Top = reader.ReadInt16();
                Padding_Bottom = reader.ReadInt16();
                Padding_Left = reader.ReadInt16();
                Padding_Right = reader.ReadInt16();
                Spacing_Horizontal = reader.ReadInt16();
                Spacing_Vertical = reader.ReadInt16();
                Char_Width = reader.ReadInt16();
                Char_Height = reader.ReadInt16();
                if (game != Game.Scooby)
                    MaxHeight = reader.ReadInt32();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(SerializeUIData(endianness));
                writer.Write(UIFontFlags.FlagValueShort);
                writer.Write(UIFontMode);
                writer.Write((byte)FontID);
                writer.Write(Text);
                writer.Write(BackgroundColor);
                writer.Write(FontColor);
                writer.Write(Padding_Top);
                writer.Write(Padding_Bottom);
                writer.Write(Padding_Left);
                writer.Write(Padding_Right);
                writer.Write(Spacing_Horizontal);
                writer.Write(Spacing_Vertical);
                writer.Write(Char_Width);
                writer.Write(Char_Height);
                if (game != Game.Scooby)
                    writer.Write(MaxHeight);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static new bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Text, ref result);
        }

        protected override void CreateBoundingBox()
        {
            CreateBoundingBox(SharpRenderer.planeVertices);
        }

        public override float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.planeTriangles, SharpRenderer.planeVertices, world);
            return null;
        }

        public override void Draw(SharpRenderer renderer)
        {
            bool found = false;
            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(Text) && ae.archive.GetFromAssetID(Text) is AssetTEXT assetText)
                {
                    var newText = assetText.Text;
                    if (newText != text)
                    {
                        text = newText;
                        if (image != null)
                        {
                            image.Dispose();
                            image = null;
                        }
                        image = TextureUtilities.CreateTextureFromBitmap(renderer.device.Device, DrawBitmapFromText());
                        found = true;
                    }
                }
            if (!found)
                image = null;

            renderer.DrawPlaneText(world, isSelected, UvAnimOffset, image);
        }

        private ShaderResourceView image;
        private string text = "";

        private Bitmap DrawBitmapFromText()
        {
            var font = new Font(FontFamily.GenericMonospace, Char_Width, FontStyle.Regular, GraphicsUnit.Pixel);            
            
            var img = new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(img);

            if ((UIFontFlags.FlagValueInt & 8) != 0)
                graphics.Clear(System.Drawing.Color.FromArgb(BackgroundColor.GetARGB()));
            else
                graphics.Clear(System.Drawing.Color.FromArgb(0));

            Brush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(FontColor.GetARGB()));

            var fmt = new StringFormat();
            //fmt.Alignment =
            //    ((UIFontFlags.FlagValueInt & 1) != 0) ? StringAlignment.Near :
            //    ((UIFontFlags.FlagValueInt & 2) != 0) ? StringAlignment.Center :
            //    ((UIFontFlags.FlagValueInt & 4) != 0) ? StringAlignment.Far : 0;

            var x = 0;
            var y = 0;

            foreach (var c in text)
            {
                graphics.DrawString(c.ToString(), font, textBrush, x, y, fmt);
                x += Spacing_Horizontal/2;
            }

            graphics.Save();

            textBrush.Dispose();
            graphics.Dispose();
            fmt.Dispose();

            return img;
        }
    }
}