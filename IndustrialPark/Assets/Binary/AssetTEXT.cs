using HipHopFile;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class AssetTEXT : Asset
    {
        public override string AssetInfo => Text;

        public string Text { get; set; }

        public AssetTEXT(string assetName) : base(assetName, AssetType.Text)
        {
            Text = "";
        }

        public AssetTEXT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                var length = reader.ReadInt32();
                Text = reader.ReadString(length);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Text.Length);
                writer.Write(Text);

                if (Text.Length == 0 || writer.BaseStream.Length % 4 != 0 || (writer.BaseStream.Length % 4 == 0 && Text[Text.Length - 1] != '\0'))
                    do writer.Write((byte)0);
                    while (writer.BaseStream.Length % 4 != 0);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) =>
            AssetNamesInText.Any(s => Functions.BKDRHash(s) == assetID || Functions.BKDRHash(s + ".RW3") == assetID) ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            foreach (string s in AssetNamesInText)
                if (!(Program.MainForm.AssetExists(Functions.BKDRHash(s)) || Program.MainForm.AssetExists(Functions.BKDRHash(s + ".RW3"))))
                    result.Add("No open archive contains asset " + s + ", which is referenced in this asset.");
        }

        private List<string> AssetNamesInText
        {
            get
            {
                string text = Text;
                List<string> assetNames = new List<string>();

                for (int i = 0; i < text.Length; i++)
                    if (text[i] == '{' || text[i] == ';')
                    {
                        string assetName = "";

                        if (i + 8 < text.Length && text.Substring(i, 8) == "{insert:")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}')
                                {
                                    assetName = text.Substring(i + 8, j - (i + 8));
                                    break;
                                }
                        }
                        else if (i + 3 < text.Length && text.Substring(i, 3) == "{i:")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}')
                                {
                                    assetName = text.Substring(i + 3, j - (i + 3));
                                    break;
                                }
                        }
                        else if (i + 5 < text.Length && text.Substring(i, 5) == "{tex:")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}' || text[j] == ';')
                                {
                                    assetName = text.Substring(i + 5, j - (i + 5));
                                    break;
                                }
                        }
                        else if (i + 7 < text.Length && text.Substring(i, 7) == "{sound:")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}' || text[j] == ';')
                                {
                                    assetName = text.Substring(i + 7, j - (i + 7));
                                    break;
                                }
                        }
                        else if (i + 7 < text.Length && text.Substring(i, 7) == "{timer:")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}')
                                {
                                    assetName = text.Substring(i + 7, j - (i + 7));
                                    break;
                                }
                        }
                        else if (i + 9 < text.Length && text.Substring(i, 9) == ";speaker=")
                        {
                            for (int j = i; j < text.Length; j++)
                                if (text[j] == '}')
                                {
                                    assetName = text.Substring(i + 9, j - (i + 9));
                                    break;
                                }
                        }

                        if (assetName != "")
                            assetNames.Add(assetName);
                    }

                return assetNames;
            }
        }
    }
}