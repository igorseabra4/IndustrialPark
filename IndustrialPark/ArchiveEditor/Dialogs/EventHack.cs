using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IndustrialPark
{
    /// <summary>
    /// Form allowing user to use the UpgradePowerUp exploit.
    /// </summary>
    public partial class EventHack : Form
    {
        public EventHack()
        {
            InitializeComponent();

            // Find the player asset
            // FIXME: Only finds one Player asset, ignores others?
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                foreach (Asset a in ae.archive.GetAllAssets())
                {
                    if (a is IndustrialPark.AssetPLYR)
                        textBoxPlayerAssetID.Text = a.assetName;
                }

            // Adds the list of events to the Source Event ComboBox.
            foreach (var o in Enum.GetValues(typeof(EventTSSM)))
            {
                comboBoxSourceEvent.Items.Add(o);
                comboBoxSourceEvent.SelectedIndex = 0;
            }

            Links = new List<Link>();
        }

        /// <summary>
        /// The list of generated links.
        /// </summary>
        internal List<Link> Links { get; private set; }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Generates the list of <see cref="Link"/>s from the provided information.
        /// </summary>
        private void generateLinks()
        {
            var dataType = radioButtonByte.Checked ? MemoryHackDataType.UINT8
               : radioButtonInt.Checked ? MemoryHackDataType.INT32
               : MemoryHackDataType.FLOAT32;

            byte[] oldValue;
            byte[] newValue;
            uint memoryAddress;

            try
            {
                // Convert values into bytes.
                oldValue = MemoryHack.GetBytes(textBoxOldValue.Text, dataType);
                newValue = MemoryHack.GetBytes(textBoxNewValue.Text, dataType);

                memoryAddress = Convert.ToUInt32(textBoxMemoryAddress.Text, 16);
            } catch
            {
                // Stop if input cannot be converted to numerical values
                return;
            }

            var modification = new MemoryModification(memoryAddress, oldValue, newValue, dataType);

            // Get the difference between each corresponding byte in the old/new values.
            byte[] byteDifference = MemoryHack.GetByteDifference(modification);

            // Calculate the arguments for every event.
            // This list will be the same length as the number of links.
            List<float> eventArguments = MemoryHack.GetEventOffsetsAsFloats(modification, byteDifference);

            // Finally, get the list of links.
            // This might be greater than 255 (the max. number of links per asset, according to Skyweiss)
            Game currentGame = Game.Incredibles; // Used for TSSM
            ushort eventReceiveId = (ushort)comboBoxSourceEvent.SelectedItem;
            AssetID playerAssetID = new AssetID(textBoxPlayerAssetID.Text);

            Links = MemoryHack.GetLinks(eventArguments, currentGame,
                eventReceiveId, playerAssetID);

            // Optionally, prepend the three GivePowerUp events which are needed to use the UpgradePowerUp exploit.
            if (checkBoxPrependHack.Checked)
            {
                List<Link> enableHack = MemoryHack.GetUpgradePowerUpHackEvents(playerAssetID, eventReceiveId);
                enableHack.AddRange(Links);
                Links = enableHack;
            }
        }

        private void buttonAddEvents_Click(object sender, EventArgs e)
        {
            generateLinks();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxMemoryAddress_TextChanged(object sender, EventArgs e)
        {
            string offsetString;
            int hexadecimalRadix = 16;

            try
            {
               offsetString = MemoryHack.GetOffset(
                    Convert.ToUInt32(textBoxMemoryAddress.Text, hexadecimalRadix)).ToString();
            }
            catch (Exception ex)
            {
                offsetString = "";
            }

            labelOffset.Text = "Offset: " + offsetString;
            buttonAddEvents.Enabled = buttonCopyLinks.Enabled = canGenerateLinks();
        }

        /// <summary>
        /// Whether the user has entered enough information for <see cref="Link"/>s to be generated.
        /// </summary>
        /// <returns>true if Links can be generated, otherwise false</returns>
        private bool canGenerateLinks()
        {
            return textBoxMemoryAddress.Text.Length > 0
                && textBoxOldValue.Text.Length > 0
                && textBoxNewValue.Text.Length > 0
                && !textBoxOldValue.Text.Equals(textBoxNewValue.Text);
        }

        private void textBoxOldValue_TextChanged(object sender, EventArgs e)
        {
            buttonAddEvents.Enabled = buttonCopyLinks.Enabled = canGenerateLinks();
        }

        private void textBoxNewValue_TextChanged(object sender, EventArgs e)
        {
            buttonAddEvents.Enabled = buttonCopyLinks.Enabled = canGenerateLinks();
        }

        private void buttonCopyLinks_Click(object sender, EventArgs e)
        {
            generateLinks();
            Clipboard.SetText(JsonConvert.SerializeObject(Links, Formatting.Indented));
        }
    }
}
