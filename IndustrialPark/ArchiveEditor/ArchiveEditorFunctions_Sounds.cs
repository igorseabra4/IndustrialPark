using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public void AddSoundToSNDI(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            if (!ContainsAssetWithType(AssetType.SoundInfo))
            {
                var prevIndex = SelectedLayerIndex;

                if (!NoLayers)
                    SelectedLayerIndex = IndexOfLayerOfType(LayerType.SNDTOC);

                var list = new List<uint>();
                PlaceTemplate(new SharpDX.Vector3(), ref list, "sound_info", AssetTemplate.Sound_Info);

                if (!NoLayers)
                    SelectedLayerIndex = prevIndex;
            }

            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    SNDI_G2.AddEntry(soundData, assetID, assetType);
                    finalData = new byte[0];
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    InternalSoundEditor.VagHeaderToLittleEndian(ref soundData);
                    SNDI_P.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
            }
            throw new Exception("Unable to add sound: SNDI asset not found");
        }

        public void RemoveSoundFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        SNDI_G1.RemoveEntry(assetID, GetFromAssetID(assetID).assetType);
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    if (SNDI_G2.HasReference(assetID))
                        SNDI_G2.RemoveEntry(assetID);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        SNDI_X.RemoveEntry(assetID, GetFromAssetID(assetID).assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        SNDI_P.RemoveEntry(assetID, GetFromAssetID(assetID).assetType);
                }
            }
        }

        public byte[] GetHeaderFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        return SNDI_G1.GetHeader(assetID, GetFromAssetID(assetID).assetType);
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    if (SNDI_G2.HasReference(assetID))
                        return SNDI_G2.GetHeader(assetID);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        return SNDI_X.GetHeader(assetID, GetFromAssetID(assetID).assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        return SNDI_P.GetHeader(assetID, GetFromAssetID(assetID).assetType);
                }
            }

            throw new Exception("Error: could not find SNDI asset which contains this sound in this archive.");
        }

        public byte[] GetSoundData(uint assetID, byte[] previousData)
        {
            List<byte> file = new List<byte>();
            file.AddRange(GetHeaderFromSNDI(assetID));
            file.AddRange(previousData);

            if (new string(new char[] { (char)file[0], (char)file[1], (char)file[2], (char)file[3] }) == "RIFF")
            {
                byte[] chunkSizeArr = BitConverter.GetBytes(file.Count - 8);

                file[4] = chunkSizeArr[0];
                file[5] = chunkSizeArr[1];
                file[6] = chunkSizeArr[2];
                file[7] = chunkSizeArr[3];
            }

            return file.ToArray();
        }

        public int GetDefaultSampleRate()
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V2 SNDI_G2)
                    if (SNDI_G2.Entry_Sounds != null)
                        return SNDI_G2.Entry_Sounds.SampleHeader.Frequency;

            return 32000;
        }

        public byte[] CreateSoundFile(AssetType assetType, string fileName, bool ps2Looping = false, bool xboxcompress = true, bool forcemono = false, int samplerate = 0)
        {
            if (platform == Platform.Xbox)
                return SoundUtility_XboxADPCM.ConvertSoundToXboxADPCM(fileName, xboxcompress, forcemono, samplerate);
            if (platform == Platform.PS2)
                return SoundUtility_PS2VAG.ConvertSoundToPS2VAG(fileName, ps2Looping, samplerate);
            if (platform == Platform.GameCube && game >= Game.Incredibles)
                return SoundUtility_FMOD.ConvertSoundToFSB3(fileName, assetType == AssetType.Sound ? GetDefaultSampleRate() : samplerate, forcemono);
            if (platform == Platform.GameCube)
                return SoundUtility_DSP.ConvertSoundToDSP(fileName, samplerate);

            MessageBox.Show("Cannot import sound: unsupported platform.");
            return null;
        }

        public void ImportSounds(bool raw, string[] fileNames, AssetType assetType, bool forceOverwrite, out List<uint> assetIDs)
        {
            assetIDs = new List<uint>();
            ProgressBar progressBar = new ProgressBar("Import Sounds");
            progressBar.SetProgressBar(0, fileNames.Count(), 1);
            progressBar.Show();

            foreach (var fileName in fileNames)
            {
                var assetName = Path.GetFileNameWithoutExtension(fileName);
                var assetID = Functions.BKDRHash(assetName);

                if (ContainsAsset(assetID))
                {
                    var result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{assetID:X8}] {assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                        RemoveAsset(assetID, true);
                    else
                        continue;
                }

                byte[] data = raw ? File.ReadAllBytes(fileName) : CreateSoundFile(assetType, fileName);
                if (data == null)
                    continue;

                var sramLayer = IndexOfLayerOfType(LayerType.SRAM);
                GetSNDI(true);

                var AHDR = new Section_AHDR(assetID, assetType, AHDRFlagsFromAssetType(assetType),
                        new Section_ADBG(0, assetName, "", 0), data);

                try
                {
                    AddSoundToSNDI(data, AHDR.assetID, AHDR.assetType, out byte[] soundData);
                    AHDR.data = soundData;
                    AddAsset(AHDR, game, platform.Endianness(), false, sramLayer);
                    assetIDs.Add(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                UnsavedChanges = true;
                progressBar.PerformStep(fileName);
            }
            progressBar.Close();
        }
    }
}
