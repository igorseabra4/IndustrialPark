using System;
using System.Collections.Generic;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetTEXT : Asset
    {
        public AssetTEXT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (string s in AssetNamesInText)
            {
                if (Functions.BKDRHash(s) == assetID)
                    return true;
                if (Functions.BKDRHash(s + ".RW3") == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

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
        
        public string Text
        {
            get => System.Text.Encoding.ASCII.GetString(Data, 4, ReadInt(0));

            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));
                foreach (char c in value)
                    newData.Add((byte)c);
                newData.Add(0);

                while (newData.Count % 4 != 0)
                    newData.Add(0);

                Data = newData.ToArray();
            }
        }
    }
}