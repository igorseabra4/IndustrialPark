using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class LinkEditor : Form
    {
        private readonly uint thisAssetID;
        private Game game;

        private LinkEditor(Game game, Link[] events, LinkType linkType, uint thisAssetID)
        {
            InitializeComponent();
            TopMost = true;

            this.thisAssetID = thisAssetID;

            bgColor = textBoxTargetAsset.BackColor;

            groupBoxEventData.Enabled = false;

            AutoCompleteStringCollection sourceObjects = new AutoCompleteStringCollection();
            AutoCompleteStringCollection sourceAll = new AutoCompleteStringCollection();

            if (!HexUIntTypeConverter.Legacy)
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    foreach (Asset a in ae.archive.GetAllAssets())
                    {
                        sourceAll.Add(a.assetName);

                        if (a is BaseAsset oa)
                            sourceObjects.Add(oa.assetName);
                    }

            textBoxTargetAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxTargetAsset.AutoCompleteCustomSource = sourceObjects;
            textBoxArgumentAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxArgumentAsset.AutoCompleteCustomSource = sourceAll;
            textBoxSourceCheckOrFlags.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxSourceCheckOrFlags.AutoCompleteCustomSource = sourceObjects;

            numericUpDownTime.Minimum = decimal.MinValue;
            numericUpDownTime.Maximum = decimal.MaxValue;

            if (linkType != LinkType.Normal)
            {
                if (linkType == LinkType.Timed)
                {
                    groupBoxSourceEvent.Text = "Time";
                    groupBoxSourceCheckOrFlags.Visible = false;
                }
                else if (linkType == LinkType.TimedRotu)
                {
                    groupBoxSourceEvent.Text = "Time";
                    groupBoxSourceCheckOrFlags.Visible = false;
                    enabledcheckBox.Visible = true;
                }
                else if (linkType == LinkType.Progress)
                {
                    groupBoxSourceEvent.Text = "Percent";
                    groupBoxSourceCheckOrFlags.Text = "Flags";
                }

                numericUpDownTime.Visible = true;
                comboRecieveEvent.Visible = false;
            }

            this.game = game;

            var eventAutoCompleteCollection = new AutoCompleteStringCollection();

            foreach (var o in Enum.GetValues(game == Game.Scooby ? typeof(EventScooby) : game == Game.BFBB ? typeof(EventBFBB) : game == Game.Incredibles ? typeof(EventTSSM) : game == Game.ROTU ? typeof(EventROTU) : typeof(EventRatProto)))
            {
                eventAutoCompleteCollection.Add(o.ToString());
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            comboRecieveEvent.AutoCompleteCustomSource = eventAutoCompleteCollection;
            comboSendEvent.AutoCompleteCustomSource = eventAutoCompleteCollection;

            foreach (var assetEvent in events)
                listBoxLinks.Items.Add(assetEvent);
        }

        public static Link[] GetLinks(Game game, Link[] links, LinkType linkType, uint thisAssetID)
        {
            LinkEditor linkEditor = new LinkEditor(game, links, linkType, thisAssetID);
            linkEditor.ShowDialog();

            if (linkEditor.OK)
            {
                List<Link> newLinks = new List<Link>();
                foreach (Link l in linkEditor.listBoxLinks.Items)
                    newLinks.Add(l);

                if (linkType == LinkType.Timed || linkType == LinkType.TimedRotu)
                    return newLinks.OrderBy(link=>link.Time).ToArray();
                return newLinks.ToArray();
            }

            return null;
        }

        private bool OK = false;

        private readonly Color bgColor;
        private bool ProgramIsChangingStuff = false;
        private bool ListBoxShouldUpdate = true;

        private string GetAssetName(AssetID assetID) => HexUIntTypeConverter.StringFromAssetID(assetID);

        private AssetID GetAssetID(string assetName)
        {
            if (HexUIntTypeConverter.Legacy)
                return Convert.ToUInt32(assetName, 16);
            return HexUIntTypeConverter.AssetIDFromString(assetName);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            listBoxLinks.Items.Add(new Link(game));
            listBoxLinks.SelectedIndices.Clear();
            listBoxLinks.SelectedIndex = listBoxLinks.Items.Count - 1;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int prev = listBoxLinks.SelectedIndex;
            for (int i = 0; i < listBoxLinks.Items.Count; i++)
                if (listBoxLinks.SelectedItems.Contains(listBoxLinks.Items[i]))
                {
                    listBoxLinks.Items.RemoveAt(i);
                    i--;
                }

            listBoxLinks.SelectedIndices.Clear();
            listBoxLinks.SelectedIndex = Math.Max(-1, prev - 1);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            var selectedLinks = new List<Link>();
            foreach (var l in listBoxLinks.SelectedItems)
                selectedLinks.Add((Link)l);
            Clipboard.SetText(JsonConvert.SerializeObject(selectedLinks, Formatting.Indented));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                var links = JsonConvert.DeserializeObject<List<Link>>(Clipboard.GetText());

                listBoxLinks.SelectedIndices.Clear();
                foreach (var link in links)
                {
                    if (LinkListEditor.LinkType == LinkType.Normal)
                    {
                        link.Time = 0;
                        link.Flags = 0;
                    }
                    else
                    {
                        link.EventReceiveID = 0;
                        if (LinkListEditor.LinkType == LinkType.Progress)
                            link.Flags = 0;
                        else if (LinkListEditor.LinkType == LinkType.TimedRotu)
                            link.Enabled = true;
                    }

                    listBoxLinks.Items.Add(link);
                    listBoxLinks.SetSelected(listBoxLinks.Items.Count - 1, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to paste links: " + ex.Message + ". Are you sure you have links copied?");
            }
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBoxShouldUpdate)
            {
                if (listBoxLinks.SelectedItems.Count == 1)
                {
                    if (!groupBoxEventData.Enabled)
                        groupBoxEventData.Enabled = true;
                    ProgramIsChangingStuff = true;

                    Link assetEvent = (Link)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                    comboRecieveEvent.SelectedIndex = assetEvent.EventReceiveID;
                    comboSendEvent.SelectedIndex = assetEvent.EventSendID;

                    textBoxTargetAsset.Text = GetAssetName(assetEvent.TargetAsset);
                    textBoxArgumentAsset.Text = GetAssetName(assetEvent.ArgumentAsset);

                    if (LinkListEditor.LinkType == LinkType.Progress)
                        textBoxSourceCheckOrFlags.Text = assetEvent.Flags.ToString();
                    else if (LinkListEditor.LinkType == LinkType.TimedRotu)
                        enabledcheckBox.Checked = assetEvent.Enabled;
                    else if (LinkListEditor.LinkType == LinkType.Normal)
                        textBoxSourceCheckOrFlags.Text = GetAssetName(assetEvent.SourceCheckAsset);
                    else
                        textBoxSourceCheckOrFlags.Text = "";

                    numericUpDownTime.Value = (decimal)assetEvent.Time;

                    if (GetAssetName(assetEvent.Parameter1).StartsWith("0x") &&
                        GetAssetName(assetEvent.Parameter2).StartsWith("0x") &&
                        GetAssetName(assetEvent.Parameter3).StartsWith("0x") &&
                        GetAssetName(assetEvent.Parameter4).StartsWith("0x"))
                        checkBoxHex.Checked = false;
                    else
                        checkBoxHex.Checked = true;

                    checkBoxHex_CheckedChanged(null, null);

                    ProgramIsChangingStuff = false;
                }
                else if (listBoxLinks.SelectedItems.Count == 0)
                {
                    groupBoxEventData.Enabled = false;
                }
            }
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                int previndex = listBoxLinks.SelectedIndex;

                if (previndex > 0)
                {
                    Link previous = (Link)listBoxLinks.Items[previndex - 1];
                    listBoxLinks.Items[previndex - 1] = (Link)listBoxLinks.Items[previndex];
                    listBoxLinks.Items[previndex] = previous;
                }

                listBoxLinks.SelectedIndices.Clear();
                listBoxLinks.SelectedIndex = Math.Max(previndex - 1, 0);
            }
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                int previndex = listBoxLinks.SelectedIndex;

                if (previndex < listBoxLinks.Items.Count - 1)
                {
                    Link post = (Link)listBoxLinks.Items[previndex + 1];
                    listBoxLinks.Items[previndex + 1] = (Link)listBoxLinks.Items[previndex];
                    listBoxLinks.Items[previndex] = post;
                }

                listBoxLinks.SelectedIndices.Clear();
                listBoxLinks.SelectedIndex = Math.Min(previndex + 1, listBoxLinks.Items.Count - 1);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void comboReceiveEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                foreach (int i in listBoxLinks.SelectedIndices)
                    ((Link)listBoxLinks.Items[i]).EventReceiveID = (ushort)comboRecieveEvent.SelectedIndex;

                SetListBoxUpdate();
            }
        }

        private void comboSendEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                foreach (int i in listBoxLinks.SelectedIndices)
                    ((Link)listBoxLinks.Items[i]).EventSendID = (ushort)comboSendEvent.SelectedIndex;

                SetListBoxUpdate();
            }
        }

        private void textBoxTargetAsset_TextChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                textBoxTargetAsset.BackColor = bgColor;

                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        ((Link)listBoxLinks.Items[i]).TargetAsset = GetAssetID(textBoxTargetAsset.Text);
                    SetListBoxUpdate();
                }
                catch
                {
                    textBoxTargetAsset.BackColor = Color.Red;
                }
            }
        }

        private void textBoxArgumentAsset_TextChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                textBoxArgumentAsset.BackColor = bgColor;

                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        ((Link)listBoxLinks.Items[i]).ArgumentAsset = GetAssetID(textBoxArgumentAsset.Text);
                }
                catch
                {
                    textBoxArgumentAsset.BackColor = Color.Red;
                }
            }
        }

        private void textBoxSourceCheck_TextChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                textBoxSourceCheckOrFlags.BackColor = bgColor;

                try
                {
                    if (LinkListEditor.LinkType == LinkType.Progress)
                        foreach (int i in listBoxLinks.SelectedIndices)
                            ((Link)listBoxLinks.Items[i]).Flags = Convert.ToInt32(textBoxSourceCheckOrFlags.Text);
                    else if (LinkListEditor.LinkType == LinkType.Normal)
                        foreach (int i in listBoxLinks.SelectedIndices)
                            ((Link)listBoxLinks.Items[i]).SourceCheckAsset = GetAssetID(textBoxSourceCheckOrFlags.Text);
                }
                catch
                {
                    textBoxSourceCheckOrFlags.BackColor = Color.Red;
                }
            }
        }

        private void numericUpDownTime_ValueChanged(object sender, EventArgs e)
        {
            if (!ProgramIsChangingStuff)
            {
                numericUpDownTime.BackColor = bgColor;

                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        ((Link)listBoxLinks.Items[i]).Time = (float)numericUpDownTime.Value;
                    SetListBoxUpdate();
                }
                catch
                {
                    numericUpDownTime.BackColor = Color.Red;
                }
            }
        }

        private void SetListBoxUpdate()
        {
            ListBoxShouldUpdate = false;

            var indices = new List<int>(listBoxLinks.SelectedIndices.Count);
            foreach (int i in listBoxLinks.SelectedIndices)
                indices.Add(i);
            indices.Sort();

            listBoxLinks.SelectedIndices.Clear();

            for (int i = 0; i < listBoxLinks.Items.Count; i++)
                listBoxLinks.Items[i] = (Link)listBoxLinks.Items[i];

            foreach (int i in indices)
                listBoxLinks.SetSelected(i, true);

            ListBoxShouldUpdate = true;
        }

        private void textBoxTargetAsset_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxTargetAsset.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).TargetAsset);
                ProgramIsChangingStuff = false;
            }
        }

        private void textBoxArgumentAsset_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxArgumentAsset.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).ArgumentAsset);
                ProgramIsChangingStuff = false;
            }
        }

        private void textBoxSourceCheck_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxSourceCheckOrFlags.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).SourceCheckAsset);
                ProgramIsChangingStuff = false;
            }
        }

        private void checkBoxHex_CheckedChanged(object sender, EventArgs e)
        {
            ProgramIsChangingStuff = true;

            Link assetEvent = (Link)listBoxLinks.Items[listBoxLinks.SelectedIndex];
            if (checkBoxHex.Checked)
            {
                textBox1.Text = GetAssetName(assetEvent.Parameter1);
                textBox2.Text = GetAssetName(assetEvent.Parameter2);
                textBox3.Text = GetAssetName(assetEvent.Parameter3);
                textBox4.Text = GetAssetName(assetEvent.Parameter4);
            }
            else
            {
                textBox1.Text = assetEvent.FloatParameter1.ToString("0.0000");
                textBox2.Text = assetEvent.FloatParameter2.ToString("0.0000");
                textBox3.Text = assetEvent.FloatParameter3.ToString("0.0000");
                textBox4.Text = assetEvent.FloatParameter4.ToString("0.0000");
            }

            ProgramIsChangingStuff = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = bgColor;

            if (!ProgramIsChangingStuff)
            {
                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        if (checkBoxHex.Checked)
                            ((Link)listBoxLinks.Items[i]).Parameter1 = GetAssetID(textBox1.Text);
                        else
                            ((Link)listBoxLinks.Items[i]).FloatParameter1 = float.Parse(textBox1.Text);

                    SetListBoxUpdate();
                }
                catch
                {
                    textBox1.BackColor = Color.Red;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.BackColor = bgColor;

            if (!ProgramIsChangingStuff)
            {
                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        if (checkBoxHex.Checked)
                            ((Link)listBoxLinks.Items[i]).Parameter2 = GetAssetID(textBox2.Text);
                        else
                            ((Link)listBoxLinks.Items[i]).FloatParameter2 = float.Parse(textBox2.Text);

                    SetListBoxUpdate();
                }
                catch
                {
                    textBox2.BackColor = Color.Red;
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.BackColor = bgColor;

            if (!ProgramIsChangingStuff)
            {
                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        if (checkBoxHex.Checked)
                            ((Link)listBoxLinks.Items[i]).Parameter3 = GetAssetID(textBox3.Text);
                        else
                            ((Link)listBoxLinks.Items[i]).FloatParameter3 = float.Parse(textBox3.Text);

                    SetListBoxUpdate();
                }
                catch
                {
                    textBox3.BackColor = Color.Red;
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.BackColor = bgColor;

            if (!ProgramIsChangingStuff)
            {
                try
                {
                    foreach (int i in listBoxLinks.SelectedIndices)
                        if (checkBoxHex.Checked)
                            ((Link)listBoxLinks.Items[i]).Parameter4 = GetAssetID(textBox4.Text);
                        else
                            ((Link)listBoxLinks.Items[i]).FloatParameter4 = float.Parse(textBox4.Text);

                    SetListBoxUpdate();
                }
                catch
                {
                    textBox4.BackColor = Color.Red;
                }
            }
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            checkBoxHex_CheckedChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = AboutBox.WikiLink + "Events", UseShellExecute = true});
        }

        private void buttonTargetSelf_Click(object sender, EventArgs e)
        {
            textBoxTargetAsset.Text = GetAssetName(thisAssetID);
        }

        private void buttonTargetPlus1_Click(object sender, EventArgs e)
        {
            try
            {
                int num = Convert.ToInt32(textBoxTargetAsset.Text.Split('_').Last());
                textBoxTargetAsset.Text = textBoxTargetAsset.Text.Substring(0, textBoxTargetAsset.Text.LastIndexOf('_')) + '_' + (num + 1).ToString("D2");
            }
            catch
            {
                MessageBox.Show("Unable to find sequence number in asset name");
            }
        }

        private void enabledcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ProgramIsChangingStuff = true;

            foreach (int i in listBoxLinks.SelectedIndices)
            {
                if (enabledcheckBox.Checked)
                    ((Link)listBoxLinks.Items[i]).Enabled = true;
                else
                    ((Link)listBoxLinks.Items[i]).Enabled = false;
            }

            SetListBoxUpdate();
            ProgramIsChangingStuff = false;

        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Prevent buggy behaviour where autocomplete box and 
            // dropdown appear at the same time (Thanks Double H)
            var cmb = (ComboBox)sender;
            cmb.DroppedDown = false;
        }
    }
}