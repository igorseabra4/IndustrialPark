using HipHopFile;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public void AddSoundToSNDI(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            if (!ContainsAssetWithType(AssetType.SoundInfo))
            {
                int layerType = (int)LayerType_BFBB.SNDTOC;
                if (game == Game.Incredibles)
                    layerType = (int)LayerType_TSSM.SNDTOC;

                var list = new List<uint>();
                PlaceTemplate(new SharpDX.Vector3(), IndexOfLayerOfType(layerType), ref list, "sound_info", AssetTemplate.SoundInfo);
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
                    SNDI_G2.AddEntry(soundData, assetID);
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

        public void AddJawDataToJAW(byte[] jawData, uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.AddEntry(jawData, assetID);
                    return;
                }
            }

            throw new Exception("Unable to add jaw data: JAW asset not found");
        }

    }
}
