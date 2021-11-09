using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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
            foreach (EventBFBB o in Enum.GetValues(typeof(EventBFBB)))
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
        EventBFBB recieveEventType;
        EventBFBB targetEventType;

        HashSet<AssetType> senders = new HashSet<AssetType>();
        HashSet<AssetType> recievers = new HashSet<AssetType>();
        HashSet<EventBFBB> recievedEvents = new HashSet<EventBFBB>();
        HashSet<EventBFBB> sentEvents = new HashSet<EventBFBB>();

        int total = 0;

        private void buttonPerform_Click(object sender, EventArgs e)
        {
            senderAssetType = (AssetType)comboSenderAsset.SelectedItem;
            recieverAssetType = (AssetType)comboTargetAsset.SelectedItem;
            recieveEventType = (EventBFBB)comboRecieveEvent.SelectedItem;
            targetEventType = (EventBFBB)comboTargetEvent.SelectedItem;

            senders = new HashSet<AssetType>();
            recievers = new HashSet<AssetType>();
            recievedEvents = new HashSet<EventBFBB>();
            sentEvents = new HashSet<EventBFBB>();

            total = 0;
            richTextBox1.Clear();

            Platform scoobyPlatform = Platform.Unknown;
            AddFolder(rootDir, ref scoobyPlatform);

            richTextBox2.Text = $"Found a total of {total} events sent by {senders.Count} different asset types: ";
            foreach (AssetType type in senders)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"to {recievers.Count} different asset types: ";
            foreach (AssetType type in recievers)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"the recieved events are of {recievedEvents.Count} different event types: ";
            foreach (EventBFBB type in recievedEvents)
                richTextBox2.Text += type.ToString() + ", ";
            richTextBox2.Text += $"and the sent events are of {sentEvents.Count} different event types: ";
            foreach (EventBFBB type in sentEvents)
                richTextBox2.Text += type.ToString() + ", ";
        }

        private void AddFolder(string folderPath, ref Platform scoobyPlatform)
        {
            foreach (string s in Directory.GetFiles(folderPath))
            {
                if (Path.GetExtension(s).ToLower() == ".hip" || Path.GetExtension(s).ToLower() == ".hop")
                {
                    ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                    archive.OpenFile(s, false, scoobyPlatform, out _, true);
                    if (scoobyPlatform == Platform.Unknown)
                        scoobyPlatform = archive.platform;
                    WriteWhatIFound(archive);
                    archive.Dispose(false);
                }
            }
            foreach (string s in Directory.GetDirectories(folderPath))
                AddFolder(s, ref scoobyPlatform);
        }

        private void WriteWhatIFound(ArchiveEditorFunctions archive)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = archive.AssetCount;
            progressBar1.Step = 1;

            foreach (Asset asset in archive.GetAllAssets())
            {
                progressBar1.PerformStep();

                if (senderAssetType != AssetType.Null && asset.assetType != senderAssetType)
                    continue;

                if (asset is BaseAsset objectAsset)
                    try
                    {
                        foreach (Link assetEvent in objectAsset.Links)
                        {
                            if (recieveEventType != EventBFBB.Unknown && (EventBFBB)assetEvent.EventReceiveID != recieveEventType)
                                continue;
                            if (targetEventType != EventBFBB.Unknown && (EventBFBB)assetEvent.EventSendID != targetEventType)
                                continue;

                            Asset targetAsset = null;
                            if (archive.ContainsAsset(assetEvent.TargetAssetID))
                                targetAsset = archive.GetFromAssetID(assetEvent.TargetAssetID);

                            if (recieverAssetType != AssetType.Null)
                            {
                                if (targetAsset != null && targetAsset.assetType != recieverAssetType)
                                    continue;
                                if (targetAsset == null)
                                    continue;
                            }

                            string eventName = $"{objectAsset.assetName} ({assetEvent.EventReceiveID}) => {assetEvent.EventSendID} => ";

                            if (targetAsset == null)
                                eventName += $"0x{assetEvent.TargetAssetID.ToString("X8")}";
                            else
                                eventName += $"{targetAsset.assetName}";

                            eventName += $" [{assetEvent.FloatParameter1}, {assetEvent.FloatParameter2}, {assetEvent.FloatParameter3}, {assetEvent.FloatParameter4}";

                            if (assetEvent.ArgumentAssetID != 0)
                            {
                                if (archive.ContainsAsset(assetEvent.ArgumentAssetID))
                                    eventName += $", {archive.GetFromAssetID(assetEvent.ArgumentAssetID).assetName}";
                                else
                                    eventName += $", 0x{assetEvent.ArgumentAssetID.ToString("X8")}";
                            }
                            if (assetEvent.SourceCheckAssetID != 0)
                            {
                                if (archive.ContainsAsset(assetEvent.SourceCheckAssetID))
                                    eventName += $", {archive.GetFromAssetID(assetEvent.SourceCheckAssetID).assetName}";
                                else
                                    eventName += $", 0x{assetEvent.SourceCheckAssetID.ToString("X8")}";
                            }

                            eventName += "]";

                            richTextBox1.AppendText(eventName + "\n");
                            senders.Add(objectAsset.assetType);
                            if (targetAsset != null)
                                recievers.Add(targetAsset.assetType);
                            recievedEvents.Add((EventBFBB)assetEvent.EventReceiveID);
                            sentEvents.Add((EventBFBB)assetEvent.EventSendID);
                            total++;
                        }
                    }
                    catch
                    {

                    }
            }
        }
    }
}
