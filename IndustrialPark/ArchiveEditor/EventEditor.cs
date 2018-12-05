using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class EventEditor : Form
    {
        private enum EventType
        {
            BFBB,
            TSSM,
            Incredibles
        }

        private EventType eventType;

        private EventEditor()
        {
            InitializeComponent();
            TopMost = true;

            bgColor = textBoxTargetAsset.BackColor;
            
            groupBoxEventData.Enabled = false;

            AutoCompleteStringCollection source = new AutoCompleteStringCollection();

            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                foreach (Asset a in ae.archive.GetAllAssets())
                    if (a is ObjectAsset oa)
                        source.Add(oa.AHDR.ADBG.assetName);

            textBoxTargetAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxTargetAsset.AutoCompleteCustomSource = source;
        }

        private EventEditor(AssetEventBFBB[] events) : this()
        {
            eventType = EventType.BFBB;
            foreach (EventTypeBFBB o in Enum.GetValues(typeof(EventTypeBFBB)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (AssetEventBFBB assetEvent in events)
                listBoxEvents.Items.Add(assetEvent);
        }

        private EventEditor(AssetEventTSSM[] events) : this()
        {
            eventType = EventType.TSSM;
            foreach (EventTypeTSSM o in Enum.GetValues(typeof(EventTypeTSSM)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (AssetEventTSSM assetEvent in events)
                listBoxEvents.Items.Add(assetEvent);
        }

        private EventEditor(AssetEventIncredibles[] events) : this()
        {
            eventType = EventType.Incredibles;
            foreach (EventTypeIncredibles o in Enum.GetValues(typeof(EventTypeIncredibles)))
            {
                comboRecieveEvent.Items.Add(o);
                comboSendEvent.Items.Add(o);
            }

            foreach (AssetEventIncredibles assetEvent in events)
                listBoxEvents.Items.Add(assetEvent);
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

        public static AssetEventBFBB[] GetEvents(AssetEventBFBB[] events, out bool success)
        {
            EventEditor eventEditor = new EventEditor(events);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<AssetEventBFBB> assetEventBFBBs = new List<AssetEventBFBB>();
            foreach (AssetEventBFBB assetEvent in eventEditor.listBoxEvents.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        public static AssetEventTSSM[] GetEvents(AssetEventTSSM[] events, out bool success)
        {
            EventEditor eventEditor = new EventEditor(events);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<AssetEventTSSM> assetEventBFBBs = new List<AssetEventTSSM>();
            foreach (AssetEventTSSM assetEvent in eventEditor.listBoxEvents.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        public static AssetEventIncredibles[] GetEvents(AssetEventIncredibles[] events, out bool success)
        {
            EventEditor eventEditor = new EventEditor(events);
            eventEditor.ShowDialog();

            success = eventEditor.OK;

            List<AssetEventIncredibles> assetEventBFBBs = new List<AssetEventIncredibles>();
            foreach (AssetEventIncredibles assetEvent in eventEditor.listBoxEvents.Items)
                assetEventBFBBs.Add(assetEvent);

            return assetEventBFBBs.ToArray();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            switch (eventType)
            {
                case EventType.BFBB:
                    listBoxEvents.Items.Add(new AssetEventBFBB());
                    break;
                case EventType.TSSM:
                    listBoxEvents.Items.Add(new AssetEventTSSM());
                    break;
                case EventType.Incredibles:
                    listBoxEvents.Items.Add(new AssetEventIncredibles());
                    break;
            }
            listBoxEvents.SelectedIndices.Clear();
            listBoxEvents.SelectedIndex = listBoxEvents.Items.Count - 1;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int prev = listBoxEvents.SelectedIndex;
            for (int i = 0; i < listBoxEvents.Items.Count; i++)
                if (listBoxEvents.SelectedItems.Contains(listBoxEvents.Items[i]))
                {
                    listBoxEvents.Items.RemoveAt(i);
                    i--;
                }

            listBoxEvents.SelectedIndices.Clear();
            listBoxEvents.SelectedIndex = Math.Max(-1, prev - 1);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            List<byte[]> assetEventBFBBs = new List<byte[]>();
            foreach (AssetEvent assetEvent in listBoxEvents.SelectedItems)
                assetEventBFBBs.Add(assetEvent.ToByteArray());

            Clipboard.SetText(JsonConvert.SerializeObject(assetEventBFBBs));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            List<byte[]> assetEvents;
            try
            {
                assetEvents = JsonConvert.DeserializeObject<List<byte[]>>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to paste events: " + ex.Message + ". Are you sure you have events copied?");
                return;
            }

            listBoxEvents.SelectedIndices.Clear();
            foreach (byte[] data in assetEvents)
            {
                switch (eventType)
                {
                    case EventType.BFBB:
                        listBoxEvents.Items.Add(new AssetEventBFBB(data, 0));
                        break;
                    case EventType.TSSM:
                        listBoxEvents.Items.Add(new AssetEventTSSM(data, 0));
                        break;
                    case EventType.Incredibles:
                        listBoxEvents.Items.Add(new AssetEventIncredibles(data, 0));
                        break;
                }
                listBoxEvents.SetSelected(listBoxEvents.Items.Count - 1, true);
            }
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBoxShouldUpdate)
            {
                if (listBoxEvents.SelectedItems.Count == 1)
                {
                    if (!groupBoxEventData.Enabled)
                        groupBoxEventData.Enabled = true;
                    ProgramIsChangingStuff = true;

                    AssetEvent assetEvent = null;

                    switch (eventType)
                    {
                        case EventType.BFBB:
                            assetEvent = (AssetEventBFBB)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((AssetEventBFBB)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((AssetEventBFBB)assetEvent).EventSendID;
                            break;
                        case EventType.TSSM:
                            assetEvent = (AssetEventTSSM)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((AssetEventTSSM)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((AssetEventTSSM)assetEvent).EventSendID;
                            break;
                        case EventType.Incredibles:
                            assetEvent = (AssetEventIncredibles)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                            comboRecieveEvent.SelectedItem = ((AssetEventIncredibles)assetEvent).EventReceiveID;
                            comboSendEvent.SelectedItem = ((AssetEventIncredibles)assetEvent).EventSendID;
                            break;
                    }

                    textBoxTargetAsset.Text = GetAssetName(assetEvent.TargetAssetID);

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
            if (listBoxEvents.SelectedItems.Count == 1)
            {
                int previndex = listBoxEvents.SelectedIndex;

                if (previndex > 0)
                {
                    switch (eventType)
                    {
                        case EventType.BFBB:
                            {
                                AssetEventBFBB previous = (AssetEventBFBB)listBoxEvents.Items[previndex - 1];
                                listBoxEvents.Items[previndex - 1] = (AssetEventBFBB)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = previous;
                                break;
                            }
                        case EventType.TSSM:
                            {
                                AssetEventTSSM previous = (AssetEventTSSM)listBoxEvents.Items[previndex - 1];
                                listBoxEvents.Items[previndex - 1] = (AssetEventTSSM)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = previous;
                                break;
                            }
                        case EventType.Incredibles:
                            {
                                AssetEventIncredibles previous = (AssetEventIncredibles)listBoxEvents.Items[previndex - 1];
                                listBoxEvents.Items[previndex - 1] = (AssetEventIncredibles)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = previous;
                                break;
                            }
                    }
                }

                listBoxEvents.SelectedIndices.Clear();
                listBoxEvents.SelectedIndex = Math.Max(previndex - 1, 0);
            }
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItems.Count == 1)
            {
                int previndex = listBoxEvents.SelectedIndex;
                
                if (previndex < listBoxEvents.Items.Count - 1)
                {
                    switch (eventType)
                    {
                        case EventType.BFBB:
                            {
                                AssetEventBFBB post = (AssetEventBFBB)listBoxEvents.Items[previndex + 1];
                                listBoxEvents.Items[previndex + 1] = (AssetEventBFBB)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = post;
                                break;
                            }
                        case EventType.TSSM:
                            {
                                AssetEventTSSM post = (AssetEventTSSM)listBoxEvents.Items[previndex + 1];
                                listBoxEvents.Items[previndex + 1] = (AssetEventTSSM)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = post;
                                break;
                            }
                        case EventType.Incredibles:
                            {
                                AssetEventIncredibles post = (AssetEventIncredibles)listBoxEvents.Items[previndex + 1];
                                listBoxEvents.Items[previndex + 1] = (AssetEventIncredibles)listBoxEvents.Items[previndex];
                                listBoxEvents.Items[previndex] = post;
                                break;
                            }
                    }
                }

                listBoxEvents.SelectedIndices.Clear();
                listBoxEvents.SelectedIndex = Math.Min(previndex + 1, listBoxEvents.Items.Count - 1);
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
            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                switch (eventType)
                {
                    case EventType.BFBB:
                        ((AssetEventBFBB)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventReceiveID = (EventTypeBFBB)comboRecieveEvent.SelectedItem;
                        break;
                    case EventType.TSSM:
                        ((AssetEventTSSM)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventReceiveID = (EventTypeTSSM)comboRecieveEvent.SelectedItem;
                        break;
                    case EventType.Incredibles:
                        ((AssetEventIncredibles)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventReceiveID = (EventTypeIncredibles)comboRecieveEvent.SelectedItem;
                        break;
                }
                SetListBoxUpdate();
            }
        }

        private void comboSendEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                switch (eventType)
                {
                    case EventType.BFBB:
                        ((AssetEventBFBB)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventSendID = (EventTypeBFBB)comboSendEvent.SelectedItem;
                        break;
                    case EventType.TSSM:
                        ((AssetEventTSSM)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventSendID = (EventTypeTSSM)comboSendEvent.SelectedItem;
                        break;
                    case EventType.Incredibles:
                        ((AssetEventIncredibles)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventSendID = (EventTypeIncredibles)comboSendEvent.SelectedItem;
                        break;
                }
                SetListBoxUpdate();
            }
        }

        private void textBoxTargetAsset_TextChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                textBoxTargetAsset.BackColor = bgColor;

                try
                {
                    ((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).TargetAssetID = GetAssetID(textBoxTargetAsset.Text);
                    SetListBoxUpdate();
                }
                catch
                {
                    textBoxTargetAsset.BackColor = Color.Red;
                }
            }
        }

        private void SetListBoxUpdate()
        {
            ListBoxShouldUpdate = false;

            switch (eventType)
            {
                case EventType.BFBB:
                    listBoxEvents.Items[listBoxEvents.SelectedIndex] = (AssetEventBFBB)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                    break;
                case EventType.TSSM:
                    listBoxEvents.Items[listBoxEvents.SelectedIndex] = (AssetEventTSSM)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                    break;
                case EventType.Incredibles:
                    listBoxEvents.Items[listBoxEvents.SelectedIndex] = (AssetEventIncredibles)listBoxEvents.Items[listBoxEvents.SelectedIndex];
                    ((AssetEventIncredibles)listBoxEvents.Items[listBoxEvents.SelectedIndex]).EventSendID = (EventTypeIncredibles)comboSendEvent.SelectedItem;
                    break;
            }

            ListBoxShouldUpdate = true;
        }

        private void textBoxTargetAsset_Leave(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItems.Count == 1)
            {
                ProgramIsChangingStuff = true;
                textBoxTargetAsset.Text = GetAssetName(((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).TargetAssetID);
                ProgramIsChangingStuff = false;
            }
        }

        private void checkBoxHex_CheckedChanged(object sender, EventArgs e)
        {
            ProgramIsChangingStuff = true;

            AssetEvent assetEvent = (AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex];
            if (checkBoxHex.Checked)
            {
                textBox1.Text = GetAssetName(assetEvent.Arguments_Hex[0]);
                textBox2.Text = GetAssetName(assetEvent.Arguments_Hex[1]);
                textBox3.Text = GetAssetName(assetEvent.Arguments_Hex[2]);
                textBox4.Text = GetAssetName(assetEvent.Arguments_Hex[3]);
                textBox5.Text = GetAssetName(assetEvent.Arguments_Hex[4]);
                textBox6.Text = GetAssetName(assetEvent.Arguments_Hex[5]);
            }
            else
            {
                textBox1.Text = assetEvent.Arguments_Float[0].ToString("0.0000");
                textBox2.Text = assetEvent.Arguments_Float[1].ToString("0.0000");
                textBox3.Text = assetEvent.Arguments_Float[2].ToString("0.0000");
                textBox4.Text = assetEvent.Arguments_Float[3].ToString("0.0000");
                textBox5.Text = assetEvent.Arguments_Float[4].ToString("0.0000");
                textBox6.Text = assetEvent.Arguments_Float[5].ToString("0.0000");
            }

            ProgramIsChangingStuff = false;
        }

        private void SetArgument(int index, string text)
        {
            if (checkBoxHex.Checked)
            {
                AssetID[] Arguments_Hex = ((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).Arguments_Hex;
                Arguments_Hex[index] = GetAssetID(text);
                ((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).Arguments_Hex = Arguments_Hex;
            }
            else
            {
                float[] Arguments_Float = ((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).Arguments_Float;
                Arguments_Float[index] = float.Parse(text);
                ((AssetEvent)listBoxEvents.Items[listBoxEvents.SelectedIndex]).Arguments_Float = Arguments_Float;
            }

            SetListBoxUpdate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = bgColor;

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
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

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
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

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
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

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
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

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.BackColor = bgColor;

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(4, textBox5.Text);
                    SetListBoxUpdate();
                }
                catch
                {
                    textBox5.BackColor = Color.Red;
                }
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox5.BackColor = bgColor;

            if (listBoxEvents.SelectedItems.Count == 1 && !ProgramIsChangingStuff)
            {
                try
                {
                    SetArgument(5, textBox6.Text);
                    SetListBoxUpdate();
                }
                catch
                {
                    textBox6.BackColor = Color.Red;
                }
            }
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            checkBoxHex_CheckedChanged(null, null);
        }
    }
}