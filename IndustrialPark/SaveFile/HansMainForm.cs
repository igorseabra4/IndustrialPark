using HipHopFile;
using IndustrialPark.SaveFile;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.SaveFile
{
    public partial class HansMainForm : Form
    {
        public HansMainForm()
        {
            InitializeComponent();
            saveFileManager = new SaveFileManager();
#if !DEBUG
            editToolStripMenuItem.Enabled = false;            
#endif
        }

        private SaveFileManager saveFileManager;
        private string currentFile;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFile.FileName;
                saveFileManager.ReadFile(openFile.FileName, currentGame, currentPlatform);
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                groupBox1.Enabled = true;
                FillBlocksListBox();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileManager.WriteFile(currentFile, currentGame, currentPlatform, out string comment);

            if (!string.IsNullOrEmpty(comment))
                MessageBox.Show(comment);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = saveFile.FileName;
                saveToolStripMenuItem_Click(sender, e);
            }
        }

        private void FillBlocksListBox()
        {
            listBoxBlocks.Items.Clear();
            foreach (Block b in saveFileManager.Blocks)
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
        }
        
        private void listBoxBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1 && listBoxBlocks.SelectedIndex < saveFileManager.Blocks.Count)
                propertyGridSectionEditor.SelectedObject = saveFileManager.Blocks[listBoxBlocks.SelectedIndex];
            else
                propertyGridSectionEditor.SelectedObject = null;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddSectionDialog addSectionDialog = new AddSectionDialog();
            addSectionDialog.ShowDialog();
            if (addSectionDialog.OKed)
            {
                saveFileManager.AddNew(addSectionDialog.section, currentGame);

                Block b = saveFileManager.Blocks.Last();
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int removeIndex = listBoxBlocks.SelectedIndex;
                listBoxBlocks.SelectedIndex = -1;
                saveFileManager.Blocks.RemoveAt(removeIndex);
                listBoxBlocks.Items.RemoveAt(removeIndex);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                Clipboard.SetText(JsonConvert.SerializeObject(
                (
                    saveFileManager.Blocks[listBoxBlocks.SelectedIndex].sectionIdentifier,
                    JsonConvert.SerializeObject(saveFileManager.Blocks[listBoxBlocks.SelectedIndex], Formatting.Indented)
                ), Formatting.Indented));
            }
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            try
            {
                (SaveFileSection, string) container = JsonConvert.DeserializeObject<(SaveFileSection, string)>(Clipboard.GetText());
                
                switch (container.Item1)
                {
                    case SaveFileSection.CNTR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_CNTR>(container.Item2));
                        break;
                    case SaveFileSection.GDAT:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_GDAT>(container.Item2));
                        break;
                    case SaveFileSection.LEDR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_LEDR>(container.Item2));
                        break;
                    case SaveFileSection.PLYR:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PLYR>(container.Item2));
                        break;
                    case SaveFileSection.PREF:
                        switch (currentGame)
                        {
                            case SaveFileGame.Scooby:
                                saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PREF_Scoo>(container.Item2));
                                break;
                            case SaveFileGame.Movie:
                            case SaveFileGame.Incredibles:
                                saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PREF_TSSM>(container.Item2));
                                break;
                            case SaveFileGame.BFBB:
                                saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_PREF_BFBB>(container.Item2));
                                break;
                        }
                        break;
                    case SaveFileSection.ROOM:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_ROOM>(container.Item2));
                        break;
                    case SaveFileSection.SFIL:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_SFIL>(container.Item2));
                        break;
                    case SaveFileSection.SVID:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_SVID>(container.Item2));
                        break;
                    case SaveFileSection.Scene:
                        saveFileManager.Blocks.Add(JsonConvert.DeserializeObject<Section_Scene>(container.Item2));
                        break;
                    default:
                        throw new Exception("Unknown section type");
                }

                Block b = saveFileManager.Blocks.Last();
                if (b is Section_Scene scene)
                    listBoxBlocks.Items.Add(scene.SceneID);
                else
                    listBoxBlocks.Items.Add(b.sectionIdentifier.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting section: " + ex.Message + "\nAre you sure you have a section copied?");
            }
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int previndex = listBoxBlocks.SelectedIndex;

                if (previndex > 0)
                {
                    Block previous = saveFileManager.Blocks[previndex - 1];
                    saveFileManager.Blocks[previndex - 1] = saveFileManager.Blocks[previndex];
                    saveFileManager.Blocks[previndex] = previous;
                }

                FillBlocksListBox();
                listBoxBlocks.SelectedIndex = Math.Max(previndex - 1, 0);
            }
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex > -1)
            {
                int previndex = listBoxBlocks.SelectedIndex;

                if (previndex < listBoxBlocks.Items.Count - 1)
                {
                    Block previous = saveFileManager.Blocks[previndex + 1];
                    saveFileManager.Blocks[previndex + 1] = saveFileManager.Blocks[previndex];
                    saveFileManager.Blocks[previndex] = previous;
                }

                FillBlocksListBox();
                listBoxBlocks.SelectedIndex = Math.Min(previndex + 1, listBoxBlocks.Items.Count - 1);
            }
        }

        private void propertyGridSectionEditor_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridSectionEditor.SelectedObject is Section_Scene scene)
                listBoxBlocks.Items[listBoxBlocks.SelectedIndex] = scene.SceneID;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hans v0.2 is a save file editor for Heavy Iron Studios games by igorseabra4; additional credits go to Seil for figuring out the format in the first place!");
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private SaveFileGame currentGame = SaveFileGame.BFBB;
        private Platform currentPlatform = Platform.GameCube;

        private void scoobyDooToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentGame = SaveFileGame.Scooby;
            scoobyDooToolStripMenuItem.Checked = true;
            battleForBikiniBottomToolStripMenuItem.Checked = false;
            movieGameToolStripMenuItem.Checked = false;
            theIncrediblesToolStripMenuItem.Checked = false;
        }

        private void battleForBikiniBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentGame = SaveFileGame.BFBB;
            scoobyDooToolStripMenuItem.Checked = false;
            battleForBikiniBottomToolStripMenuItem.Checked = true;
            movieGameToolStripMenuItem.Checked = false;
            theIncrediblesToolStripMenuItem.Checked = false;
        }

        private void movieGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentGame = SaveFileGame.Movie;
            scoobyDooToolStripMenuItem.Checked = false;
            battleForBikiniBottomToolStripMenuItem.Checked = false;
            movieGameToolStripMenuItem.Checked = true;
            theIncrediblesToolStripMenuItem.Checked = false;
        }

        private void theIncrediblesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentGame = SaveFileGame.Incredibles;
            scoobyDooToolStripMenuItem.Checked = false;
            battleForBikiniBottomToolStripMenuItem.Checked = false;
            movieGameToolStripMenuItem.Checked = false;
            theIncrediblesToolStripMenuItem.Checked = true;
        }

        private void gameCubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPlatform = Platform.GameCube;
            gameCubeToolStripMenuItem.Checked = true;
            playstation2ToolStripMenuItem.Checked = false;
            xboxToolStripMenuItem.Checked = false;
        }

        private void playstation2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPlatform = Platform.PS2;
            gameCubeToolStripMenuItem.Checked = false;
            playstation2ToolStripMenuItem.Checked = true;
            xboxToolStripMenuItem.Checked = false;
        }

        private void xboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPlatform = Platform.Xbox;
            gameCubeToolStripMenuItem.Checked = false;
            playstation2ToolStripMenuItem.Checked = false;
            xboxToolStripMenuItem.Checked = true;
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxBlocks.SelectedIndex < 0 || listBoxBlocks.SelectedIndex >= saveFileManager.Blocks.Count)
            {
                MessageBox.Show("Please select a block to generate a report.");
                return;
            }
            if (saveFileManager.Blocks[listBoxBlocks.SelectedIndex] is Section_Scene scene)
            {
                var archive = Program.MainForm.archiveEditors.First(ae => scene.SceneID.ToUpper().Equals(Path.GetFileNameWithoutExtension(ae.archive.currentlyOpenFilePath).ToUpper())).archive;
                if (archive != null)
                {
                    var result = SaveFileScene.GenerateReport(archive, currentPlatform, scene.Data);
                    new ScrollableMessageBox(scene.SceneID + " Report", result).Show();
                }
                else
                {
                    MessageBox.Show("Archive not found for the selected scene.");
                }
            }
            else
            {
                MessageBox.Show("Selected block is not a scene.");
            }
        }
    }
}
