using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class EventSearch : Form
    {
        public EventSearch()
        {
            InitializeComponent();
        }

        private void EventSearch_Load(object sender, EventArgs e)
        {
            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                comboSenderAsset.Items.Add(o);
                comboTargetAsset.Items.Add(o);
            }
            foreach (EventTypeBFBB o in Enum.GetValues(typeof(EventTypeBFBB)))
            {
                comboRecieveEvent.Items.Add(o);
                comboTargetEvent.Items.Add(o);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private string rootDir;

        private void buttonChooseRoot_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            {
                rootDir = openFile.FileName;
                labelRootDir.Text = "Root Directory: " + rootDir;
            }
        }

        AssetType senderAssetType;
        AssetType recieverAssetType;
        EventTypeBFBB recieveEventType;
        EventTypeBFBB targetEventType;

        HashSet<AssetType> senders = new HashSet<AssetType>();
        HashSet<AssetType> recievers = new HashSet<AssetType>();
        HashSet<EventTypeBFBB> recievedEvents = new HashSet<EventTypeBFBB>();
        HashSet<EventTypeBFBB> sentEvents = new HashSet<EventTypeBFBB>();

        int total = 0;

        private void buttonPerform_Click(object sender, EventArgs e)
        {
            senderAssetType = (AssetType)comboSenderAsset.SelectedItem;
            recieverAssetType = (AssetType)comboTargetAsset.SelectedItem;
            recieveEventType = (EventTypeBFBB)comboRecieveEvent.SelectedItem;
            targetEventType = (EventTypeBFBB)comboTargetEvent.SelectedItem;

            senders = new HashSet<AssetType>();
            recievers = new HashSet<AssetType>();
            recievedEvents = new HashSet<EventTypeBFBB>();
            sentEvents = new HashSet<EventTypeBFBB>();

            total = 0;
            richTextBox1.Clear();

            AddFolder(rootDir);

            richTextBox2.Text = $"Found a total of {total} events sent by {senders.Count} different asset types: ";
            foreach (AssetType type in senders)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"to {recievers.Count} different asset types: ";
            foreach (AssetType type in recievers)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"the recieved events are of {recievedEvents.Count} different event types: ";
            foreach (EventTypeBFBB type in recievedEvents)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"and the sent events are of {sentEvents.Count} different event types: ";
            foreach (EventTypeBFBB type in sentEvents)
                richTextBox2.Text += type.ToString() + ", ";
        }

        private void AddFolder(string folderPath)
        {
            foreach (string s in Directory.GetFiles(folderPath))
            {
                if (Path.GetExtension(s).ToLower() == ".hip" || Path.GetExtension(s).ToLower() == ".hop")
                {
                    ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                    archive.OpenFile(s);
                    WriteWhatIFound(archive);
                    archive.Dispose();
                }
            }
            foreach (string s in Directory.GetDirectories(folderPath))
                AddFolder(s);
        }

        private void WriteWhatIFound(ArchiveEditorFunctions archive)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = archive.GetAllAssets().Count;
            progressBar1.Step = 1;

            foreach (Asset asset in archive.GetAllAssets())
            {
                progressBar1.PerformStep();

                if (senderAssetType != AssetType.Null && asset.AHDR.assetType != senderAssetType)
                    continue;

                if (asset is ObjectAsset objectAsset)
                    foreach (AssetEventBFBB assetEvent in objectAsset.EventsBFBB)
                    {
                        if (recieveEventType != EventTypeBFBB.Unknown && assetEvent.EventReceiveID != recieveEventType)
                            continue;
                        if (targetEventType != EventTypeBFBB.Unknown && assetEvent.EventSendID != targetEventType)
                            continue;

                        Asset targetAsset = null;

                        if (archive.ContainsAsset(assetEvent.TargetAssetID))
                            targetAsset = archive.GetFromAssetID(assetEvent.TargetAssetID);

                        if (targetAsset != null && recieverAssetType != AssetType.Null && targetAsset.AHDR.assetType != recieverAssetType)
                            continue;

                        if (targetAsset == null && recieverAssetType != AssetType.Null)
                            continue;

                        string eventName;
                        if (targetAsset == null)
                            eventName = $"{objectAsset.AHDR.ADBG.assetName} ({assetEvent.EventReceiveID.ToString()}) => {assetEvent.EventSendID.ToString()} => {assetEvent.TargetAssetID.ToString()} [{assetEvent.Arguments_Float[0]}, {assetEvent.Arguments_Float[1]}, {assetEvent.Arguments_Float[2]}, {assetEvent.Arguments_Float[3]}, {assetEvent.Arguments_Hex[4].ToString()}, {assetEvent.Arguments_Hex[5].ToString()}]";
                        else
                            eventName = $"{objectAsset.AHDR.ADBG.assetName} ({assetEvent.EventReceiveID.ToString()}) => {assetEvent.EventSendID.ToString()} => {targetAsset.AHDR.ADBG.assetName} [{ assetEvent.Arguments_Float[0]}, { assetEvent.Arguments_Float[1]}, { assetEvent.Arguments_Float[2]}, { assetEvent.Arguments_Float[3]}, { assetEvent.Arguments_Hex[4].ToString()}, { assetEvent.Arguments_Hex[5].ToString()}]";

                        richTextBox1.AppendText(eventName + "\n");
                        senders.Add(objectAsset.AHDR.assetType);
                        if (targetAsset != null)
                            recievers.Add(targetAsset.AHDR.assetType);
                        recievedEvents.Add(assetEvent.EventReceiveID);
                        sentEvents.Add(assetEvent.EventSendID);
                        total++;
                    }
            }
        }
    }
}
