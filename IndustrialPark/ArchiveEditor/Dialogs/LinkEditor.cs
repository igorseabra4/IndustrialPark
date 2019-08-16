using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class LinkEditor : Form
    {
        private enum EventType
        {
            BFBB,
            TSSM,
            Incredibles
        }

        private Endianness endianness;
        private EventType eventType;
        private bool isTimed = false;

        private LinkEditor(bool isTimed, Endianness endianness)
        {
            InitializeComponent();
            TopMost = true;

            this.endianness = endianness;

            bgColor = textBoxTargetAsset.BackColor;
            
            groupBoxEventData.Enabled = false;

            AutoCompleteStringCollection sourceObjects = new AutoCompleteStringCollection();
            AutoCompleteStringCollection sourceAll = new AutoCompleteStringCollection();

            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                foreach (Asset a in ae.archive.GetAllAssets())
                {
                    sourceAll.Add(a.AHDR.ADBG.assetName);

                    if (a is ObjectAsset oa)
                        sourceObjects.Add(oa.AHDR.ADBG.assetName);
                }

            textBoxTargetAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxTargetAsset.AutoCompleteCustomSource = sourceObjects;
            textBoxArgumentAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxArgumentAsset.AutoCompleteCustomSource = sourceAll;
            textBoxSourceCheck.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxSourceCheck.AutoCompleteCustomSource = sourceObjects;

            numericUpDownTime.Minimum = decimal.MinValue;
            numericUpDownTime.Maximum = decimal.MaxValue;

            this.isTimed = isTimed;
            if (isTimed)
            {
                groupBox3.Text = "Time";
                numericUpDownTime.Visible = true;
                comboRecieveEvent.Visible = false;
                groupBox7.Visible = false;
                textBoxSourceCheck.Visible = false;
            }
        }

        private LinkEditor(LinkBFBB[] events, bool isTimed, Endianness endianness) : this(isTimed, endianness)
        {
            eventType = EventType.BFBB;
            foreach (EventBFBB o in Enum.GetValues(typeof(EventBFBB)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (LinkBFBB assetEvent in events)
                listBoxLinks.Items.Add(assetEvent);
        }

        private LinkEditor(LinkTSSM[] events, bool isTimed, Endianness endianness) : this(isTimed, endianness)
        {
            eventType = EventType.TSSM;
            foreach (EventTSSM o in Enum.GetValues(typeof(EventTSSM)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (LinkTSSM assetEvent in events)
                listBoxLinks.Items.Add(assetEvent);
        }

        private LinkEditor(LinkIncredibles[] events, bool isTimed, Endianness endianness) : this(isTimed, endianness)
        {
            eventType = EventType.Incredibles;
            foreach (EventIncredibles o in Enum.GetValues(typeof(EventIncredibles)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (LinkIncredibles assetEvent in events)
                listBoxLinks.Items.Add(assetEvent);
        }

        private bool OK = false;

        private Color bgColor;
        private bool ProgramIsChangingStuff = false;
        private bool ListBoxShouldUpdate = true;

        private string GetAssetName(AssetID assetID)
        {
            if (AssetIDTypeConverter.Legacy)
                return assetID.ToString("X8");
            return Program.MainForm.GetAssetNameFromID(assetID);
        }

        private AssetID GetAssetID(string assetName)
        {
            if (AssetIDTypeConverter.Legacy)
                return Convert.ToUInt32(assetName, 16);
            return AssetIDTypeConverter.AssetIDFromString(assetName);
        }

        public static LinkBFBB[] GetEvents(LinkBFBB[] links, Endianness endianness, out bool success, bool isTimed)
        {
            LinkEditor eventEditor = new LinkEditor(links, isTimed, endianness);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<LinkBFBB> assetEventBFBBs = new List<LinkBFBB>();
            foreach (LinkBFBB assetEvent in eventEditor.listBoxLinks.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        public static LinkTSSM[] GetEvents(LinkTSSM[] links, Endianness endianness, out bool success, bool isTimed)
        {
            LinkEditor eventEditor = new LinkEditor(links, isTimed, endianness);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<LinkTSSM> assetEventBFBBs = new List<LinkTSSM>();
            foreach (LinkTSSM assetEvent in eventEditor.listBoxLinks.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        public static LinkIncredibles[] GetEvents(LinkIncredibles[] links, Endianness endianness, out bool success, bool isTimed)
        {
            LinkEditor eventEditor = new LinkEditor(links, isTimed, endianness);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<LinkIncredibles> assetEventBFBBs = new List<LinkIncredibles>();
            foreach (LinkIncredibles assetEvent in eventEditor.listBoxLinks.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            switch (eventType)
            {
                case EventType.BFBB:
                    listBoxLinks.Items.Add(new LinkBFBB(endianness, isTimed));
                    break;
                case EventType.TSSM:
                    listBoxLinks.Items.Add(new LinkTSSM(endianness, isTimed));
                    break;
                case EventType.Incredibles:
                    listBoxLinks.Items.Add(new LinkIncredibles(endianness, isTimed));
                    break;
            }
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
            List<byte[]> assetEventBFBBs = new List<byte[]>();
            foreach (Link assetEvent in listBoxLinks.SelectedItems)
                assetEventBFBBs.Add(assetEvent.ToByteArray());

            Clipboard.SetText(JsonConvert.SerializeObject(new LinkClipboard(endianness, assetEventBFBBs), Formatting.Indented));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            LinkClipboard linkClipboard;
            try
            {
                linkClipboard = JsonConvert.DeserializeObject<LinkClipboard>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to paste links: " + ex.Message + ". Are you sure you have links copied?");
                return;
            }

            listBoxLinks.SelectedIndices.Clear();
            foreach (byte[] data in linkClipboard.links)
            {
                byte[] newData = isTimed ?
                    (linkClipboard.endianness != endianness ?
                    EndianConverter.GetTimedLinksReversedEndian(data) : data) :
                    (linkClipboard.endianness != endianness ?
                    EndianConverter.GetLinksReversedEndian(data) : data);

                switch (eventType)
                {
                    case EventType.BFBB:
                        listBoxLinks.Items.Add(new LinkBFBB(newData, 0, isTimed, endianness));
                        break;
                    case EventType.TSSM:
                        listBoxLinks.Items.Add(new LinkTSSM(newData, 0, isTimed, endianness));
                        break;
                    case EventType.Incredibles:
                        listBoxLinks.Items.Add(new LinkIncredibles(newData, 0, isTimed, endianness));
                        break;
                }
                listBoxLinks.SetSelected(listBoxLinks.Items.Count - 1, true);
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

                    Link assetEvent = null;

                    switch (eventType)
                    {
                        case EventType.BFBB:
                            assetEvent = (LinkBFBB)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((LinkBFBB)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((LinkBFBB)assetEvent).EventSendID;
                            break;
                        case EventType.TSSM:
                            assetEvent = (LinkTSSM)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((LinkTSSM)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((LinkTSSM)assetEvent).EventSendID;
                            break;
                        case EventType.Incredibles:
                            assetEvent = (LinkIncredibles)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((LinkIncredibles)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((LinkIncredibles)assetEvent).EventSendID;
                            break;
                    }

                    textBoxTargetAsset.Text = GetAssetName(assetEvent.TargetAssetID);
                    textBoxArgumentAsset.Text = GetAssetName(assetEvent.ArgumentAssetID);
                    textBoxSourceCheck.Text = GetAssetName(assetEvent.SourceCheckAssetID);
                    numericUpDownTime.Value = (decimal)assetEvent.Time;

                    checkBoxHex_CheckedChanged(null, null);

                    ProgramIsChangingStuff = false;
                }
                else
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
                    switch (eventType)
                    {
                        case EventType.BFBB:
                            {
                                LinkBFBB previous = (LinkBFBB)listBoxLinks.Items[previndex - 1];
                                listBoxLinks.Items[previndex - 1] = (LinkBFBB)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = previous;
                                break;
                            }
                        case EventType.TSSM:
                            {
                                LinkTSSM previous = (LinkTSSM)listBoxLinks.Items[previndex - 1];
                                listBoxLinks.Items[previndex - 1] = (LinkTSSM)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = previous;
                                break;
                            }
                        case EventType.Incredibles:
                            {
                                LinkIncredibles previous = (LinkIncredibles)listBoxLinks.Items[previndex - 1];
                                listBoxLinks.Items[previndex - 1] = (LinkIncredibles)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = previous;
                                break;
                            }
                    }
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
                    switch (eventType)
                    {
                        case EventType.BFBB:
                            {
                                LinkBFBB post = (LinkBFBB)listBoxLinks.Items[previndex + 1];
                                listBoxLinks.Items[previndex + 1] = (LinkBFBB)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = post;
                                break;
                            }
                        case EventType.TSSM:
                            {
                                LinkTSSM post = (LinkTSSM)listBoxLinks.Items[previndex + 1];
                                listBoxLinks.Items[previndex + 1] = (LinkTSSM)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = post;
                                break;
                            }
                        case EventType.Incredibles:
                            {
                                LinkIncredibles post = (LinkIncredibles)listBoxLinks.Items[previndex + 1];
                                listBoxLinks.Items[previndex + 1] = (LinkIncredibles)listBoxLinks.Items[previndex];
                                listBoxLinks.Items[previndex] = post;
                                break;
                            }
                    }
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
        
        private void comboRecieveEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                switch (eventType)
                {
                    case EventType.BFBB:
                        ((LinkBFBB)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventReceiveID = (EventBFBB)comboRecieveEvent.SelectedItem;
                        break;
                    case EventType.TSSM:
                        ((LinkTSSM)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventReceiveID = (EventTSSM)comboRecieveEvent.SelectedItem;
                        break;
                    case EventType.Incredibles:
                        ((LinkIncredibles)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventReceiveID = (EventIncredibles)comboRecieveEvent.SelectedItem;
                        break;
                }
                SetListBoxUpdate();
            }
        }

        private void comboSendEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                switch (eventType)
                {
                    case EventType.BFBB:
                        ((LinkBFBB)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventSendID = (EventBFBB)comboSendEvent.SelectedItem;
                        break;
                    case EventType.TSSM:
                        ((LinkTSSM)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventSendID = (EventTSSM)comboSendEvent.SelectedItem;
                        break;
                    case EventType.Incredibles:
                        ((LinkIncredibles)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventSendID = (EventIncredibles)comboSendEvent.SelectedItem;
                        break;
                }
                SetListBoxUpdate();
            }
        }

        private void textBoxTargetAsset_TextChanged(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                textBoxTargetAsset.BackColor = bgColor;

                try
                {
                    ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).TargetAssetID = GetAssetID(textBoxTargetAsset.Text);
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
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                textBoxArgumentAsset.BackColor = bgColor;

                try
                {
                    ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).ArgumentAssetID = GetAssetID(textBoxArgumentAsset.Text);
                }
                catch
                {
                    textBoxArgumentAsset.BackColor = Color.Red;
                }
            }
        }

        private void textBoxSourceCheck_TextChanged(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                textBoxSourceCheck.BackColor = bgColor;

                try
                {
                    ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).SourceCheckAssetID = GetAssetID(textBoxSourceCheck.Text);
                }
                catch
                {
                    textBoxSourceCheck.BackColor = Color.Red;
                }
            }
        }

        private void numericUpDownTime_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                numericUpDownTime.BackColor = bgColor;

                try
                {
                    ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).Time = (float)numericUpDownTime.Value;
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

            switch (eventType)
            {
                case EventType.BFBB:
                    listBoxLinks.Items[listBoxLinks.SelectedIndex] = (LinkBFBB)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                    break;
                case EventType.TSSM:
                    listBoxLinks.Items[listBoxLinks.SelectedIndex] = (LinkTSSM)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                    break;
                case EventType.Incredibles:
                    listBoxLinks.Items[listBoxLinks.SelectedIndex] = (LinkIncredibles)listBoxLinks.Items[listBoxLinks.SelectedIndex];
                    ((LinkIncredibles)listBoxLinks.Items[listBoxLinks.SelectedIndex]).EventSendID = (EventIncredibles)comboSendEvent.SelectedItem;
                    break;
            }

            ListBoxShouldUpdate = true;
        }

        private void textBoxTargetAsset_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxTargetAsset.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).TargetAssetID);
                ProgramIsChangingStuff = false;
            }
        }

        private void textBoxArgumentAsset_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxArgumentAsset.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).ArgumentAssetID);
                ProgramIsChangingStuff = false;
            }
        }

        private void textBoxSourceCheck_Leave(object sender, EventArgs e)
        {
            if (listBoxLinks.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxSourceCheck.Text = GetAssetName(((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).SourceCheckAssetID);
                ProgramIsChangingStuff = false;
            }
        }

        private void checkBoxHex_CheckedChanged(object sender, EventArgs e)
        {
            ProgramIsChangingStuff = true;

            Link assetEvent = (Link)listBoxLinks.Items[listBoxLinks.SelectedIndex];
            if (checkBoxHex.Checked)
            {
                textBox1.Text = GetAssetName(assetEvent.Arguments_Hex[0]);
                textBox2.Text = GetAssetName(assetEvent.Arguments_Hex[1]);
                textBox3.Text = GetAssetName(assetEvent.Arguments_Hex[2]);
                textBox4.Text = GetAssetName(assetEvent.Arguments_Hex[3]);
            }
            else
            {
                textBox1.Text = assetEvent.Arguments_Float[0].ToString("0.0000");
                textBox2.Text = assetEvent.Arguments_Float[1].ToString("0.0000");
                textBox3.Text = assetEvent.Arguments_Float[2].ToString("0.0000");
                textBox4.Text = assetEvent.Arguments_Float[3].ToString("0.0000");
            }

            ProgramIsChangingStuff = false;
        }

        private void SetArgument(int index, string text)
        {
            if (checkBoxHex.Checked)
            {
                AssetID[] Arguments_Hex = ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).Arguments_Hex;
                Arguments_Hex[index] = GetAssetID(text);
                ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).Arguments_Hex = Arguments_Hex;
            }
            else
            {
                float[] Arguments_Float = ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).Arguments_Float;
                Arguments_Float[index] = float.Parse(text);
                ((Link)listBoxLinks.Items[listBoxLinks.SelectedIndex]).Arguments_Float = Arguments_Float;
            }

            SetListBoxUpdate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = bgColor;

            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(0, textBox1.Text);
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

            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(1, textBox2.Text);
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

            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(2, textBox3.Text);
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

            if (listBoxLinks.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(3, textBox4.Text);
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
            System.Diagnostics.Process.Start(AboutBox.WikiLink + "Events");
        }
    }
}