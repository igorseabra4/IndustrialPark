using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Ps2IsoTools;
using Ps2IsoTools.UDF;

namespace IndustrialPark
{
    public partial class BuildISO : Form
    {
        public static string PCSX2Path;
        public static string[] recentGameDirPaths;
        private FileSystemWatcher fileSystemWatcher;
        private string[] lastCheckedNodes = [];
        private DirectoryInfo currentDirectory;

        public BuildISO()
        {
            InitializeComponent();

            pcsx2PathTextBox.Text = PCSX2Path;
            if (recentGameDirPaths != null)
                gameDirBox.Items.AddRange(recentGameDirPaths);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
            Hide();
        }

        private string GetAbsoluteFilePathFromNode(TreeNode node) => Path.Combine(Directory.GetParent(gameDirBox.Text).FullName, node.FullPath);

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "EXE Files|*.exe",
                Title = "Please select the PCSX2 exe"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                pcsx2PathTextBox.Text = dialog.FileName;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gameDirBox.Text = dialog.SelectedPath;
                CreateDirectoryNodeTree(dialog.SelectedPath);
            }
        }

        private void InitializeFileSystemWatcher(string path)
        {
            fileSystemWatcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size,
                SynchronizingObject = this
            };
            fileSystemWatcher.Renamed += OnFileRenamed;
            fileSystemWatcher.Changed += (s, e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Renamed)
                    CalculateTotalISOSize();
            };
            fileSystemWatcher.Deleted += OnFileDeleted;
            fileSystemWatcher.Created += (s, e) => CreateDirectoryNodeTree(gameDirBox.Text);
            fileSystemWatcher.Error += OnFileSystemError;

        }

        private void CreateDirectoryNodeTree(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Not a valid directory path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            InitializeFileSystemWatcher(path);

            if (!gameDirBox.Items.Contains(path))
                gameDirBox.Items.Add(path);
            while (gameDirBox.Items.Count > 5)
                gameDirBox.Items.RemoveAt(0);
            recentGameDirPaths = gameDirBox.Items.Cast<string>().ToArray();

            if (path == currentDirectory?.FullName)
                lastCheckedNodes = GetCheckedNodes(treeView1.Nodes).Select(node => node.Text).ToArray();
            else
                lastCheckedNodes = [];
            currentDirectory = new DirectoryInfo(path);

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(CreateDirectoryNode(currentDirectory));
            treeView1.TopNode.Expand();
            treeView1.EndUpdate();

            CalculateTotalISOSize();
        }

        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name)
            { 
                Checked = lastCheckedNodes.Contains(directoryInfo.Name) || !lastCheckedNodes.Any(), 
                ImageIndex = 0, 
                SelectedImageIndex = 0 
            };

            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name) { Checked = lastCheckedNodes.Contains(file.Name) || !lastCheckedNodes.Any() };
                if (!imageList1.Images.ContainsKey(file.Extension))
                    imageList1.Images.Add(file.Extension, Icon.ExtractAssociatedIcon(file.FullName));
                fileNode.ImageKey = file.Extension;
                fileNode.SelectedImageKey = file.Extension;
                directoryNode.Nodes.Add(fileNode);
            }
            return directoryNode;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            CheckAllChildNodes(e.Node);
            CheckAllParentNodes(e.Node);
            CalculateTotalISOSize();
        }

        private void CheckAllChildNodes(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = node.Checked;
                CheckAllChildNodes(child);
            }
        }

        private void CheckAllParentNodes(TreeNode node)
        {
            if (node.Parent == null)
                return;

            node.Parent.Checked = node.Parent.Nodes.Cast<TreeNode>().Any(i => i.Checked);
            CheckAllParentNodes(node.Parent);
        }

        private void CalculateTotalISOSize()
        {
            long totalISOSize = 0;
            long totalISOSizeChecked = 0;

            foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes, true))
                if (node.ImageIndex != 0)
                {
                    long fileSize = new FileInfo(GetAbsoluteFilePathFromNode(node)).Length;
                    totalISOSize += fileSize;
                    if (node.Checked)
                        totalISOSizeChecked += fileSize;
                }

            toolStripStatusLabel2.Text = $"{ArchiveEditor.ConvertSize((int)totalISOSizeChecked)}/{ArchiveEditor.ConvertSize((int)totalISOSize)}";
        }

        private void buttonRunButton_Click(object sender, EventArgs e)
        {
            if (!BuildPS2ISO(outputIsoPath.Text))
                return;
            if (!File.Exists(pcsx2PathTextBox.Text))
            {
                MessageBox.Show("Invalid PCSX2 path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = pcsx2PathTextBox.Text,
                Arguments = $"\"{outputIsoPath.Text}\" {pcsx2ArgumentsTextBox.Text}",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = true,
            };

            Process process = new Process() { StartInfo = startInfo };
            process.Start();
            process.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "ISO Files|*.ISO",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                outputIsoPath.Text = Path.GetFullPath(dialog.FileName);
        }

        private List<TreeNode> GetCheckedNodes(TreeNodeCollection nodes, bool allNodes = false)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked || allNodes)
                    checkedNodes.Add(node);

                if (node.Nodes.Count > 0)
                    checkedNodes.AddRange(GetCheckedNodes(node.Nodes, allNodes));
            }

            return checkedNodes;
        }

        private bool BuildPS2ISO(string outputDir)
        {
            if (string.IsNullOrEmpty(outputIsoPath.Text) || !Directory.Exists(Path.GetDirectoryName(outputIsoPath.Text)) || string.IsNullOrEmpty(gameDirBox.Text) || !Directory.Exists(gameDirBox.Text))
            {
                MessageBox.Show($"\"{label4.Text}\" and/or \"{label3.Text}\" not specified or not a valid directory path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            toolStripStatusLabel1.Text = "Build in progress...";
            statusStrip1.Update();

            try
            {
                UdfBuilder builder = new UdfBuilder();
                builder.VolumeIdentifier = new DirectoryInfo(gameDirBox.Text).Name;

                foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes))
                {
                    if (node.Parent == null)
                        continue;

                    string fullpath = GetAbsoluteFilePathFromNode(node);
                    string relativePath = Path.GetRelativePath(gameDirBox.Text, fullpath);

                    FileAttributes attr = File.GetAttributes(fullpath);
                    if ((attr & FileAttributes.Directory) != 0)
                        builder.AddDirectory(relativePath);
                    else
                        builder.AddFile(relativePath, fullpath);
                }
                builder.Build(outputDir);
                toolStripStatusLabel1.Text = "ISO successfully built";
            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = "ISO building failed!";
                MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Task.Delay(5000).ContinueWith(i => { toolStripStatusLabel1.Text = ""; });
            return true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BuildPS2ISO(outputIsoPath.Text);
        }

        private void pcsx2PathTextBox_TextChanged(object sender, EventArgs e)
        {
            PCSX2Path = pcsx2PathTextBox.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateDirectoryNodeTree(gameDirBox.Text);
        }

        private void gameDirBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                CreateDirectoryNodeTree(gameDirBox.Text);
        }

        private void gameDirBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDirectoryNodeTree(gameDirBox.Text);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes, true))
                if (node.Text == e.OldName)
                    node.Text = e.Name;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes, true))
                if (node.Text == e.Name)
                    node.Remove();
        }
        private void OnFileSystemError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show($"Failed to watch for filesystem changes ({e.GetException()})\n.FileSystemWatcher is now disabled", "FileSystemWatcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            fileSystemWatcher.EnableRaisingEvents = false;
        }
    }

}
