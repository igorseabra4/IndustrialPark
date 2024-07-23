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

        private void CreateDirectoryNodeTree(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Not a valid directory path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!gameDirBox.Items.Contains(path))
                gameDirBox.Items.Add(path);
            while (gameDirBox.Items.Count > 5)
                gameDirBox.Items.RemoveAt(0);
            recentGameDirPaths = gameDirBox.Items.Cast<string>().ToArray();

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(CreateDirectoryNode(new DirectoryInfo(path)));
            treeView1.TopNode.Expand();
            treeView1.EndUpdate();

            CalculateTotalISOSize();
        }

        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name) { Checked = true, ImageIndex = 0, SelectedImageIndex = 0 };
         
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            foreach (var file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name) { Checked = true };
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

            foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes))
                if (node.ImageIndex != 0)
                    totalISOSize += new FileInfo(GetAbsoluteFilePathFromNode(node)).Length;

            toolStripStatusLabel2.Text = ArchiveEditor.ConvertSize((int)totalISOSize);
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

        private List<TreeNode> GetCheckedNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    checkedNodes.Add(node);

                if (node.Nodes.Count > 0)
                    checkedNodes.AddRange(GetCheckedNodes(node.Nodes));
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
                builder.VolumeIdentifier = Path.GetDirectoryName(gameDirBox.Text);

                foreach (TreeNode node in GetCheckedNodes(treeView1.Nodes))
                {
                    string fullpath = GetAbsoluteFilePathFromNode(node);
                    string relativePath = GetRelativePath(gameDirBox.Text, fullpath);

                    FileAttributes attr = File.GetAttributes(fullpath);
                    if ((attr & FileAttributes.Directory) != 0)
                        builder.AddDirectory(relativePath);
                    else
                        builder.AddFile(relativePath, fullpath);
                }
                builder.Build(outputDir);
                toolStripStatusLabel1.Text = "ISO successfully built";
            }
            catch
            {
                toolStripStatusLabel1.Text = "ISO building failed!";
                return false;
            }

            Task.Delay(5000).ContinueWith(i => { toolStripStatusLabel1.Text = ""; });
            return true;
        }

        private void AddDirectory(UdfBuilder builder, string srcDir, string rootDir)
        {

            foreach (string filepath in Directory.GetFiles(srcDir))
            {
                string relativePath = GetRelativePath(rootDir, filepath);
                Console.WriteLine(relativePath);
                builder.AddFile(relativePath, filepath);
            }

            foreach (string dirPath in Directory.GetDirectories(srcDir))
            {
                string relativePath = GetRelativePath(srcDir, dirPath);
                Console.WriteLine(relativePath);
                AddDirectory(builder, dirPath, rootDir);
                builder.AddDirectory(relativePath);
            }
        }

        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme) { return toPath; }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
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
    }

}
