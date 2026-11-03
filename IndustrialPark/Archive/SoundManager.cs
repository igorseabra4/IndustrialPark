using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public class SoundManager
{
    private readonly ArchiveSession session;
    private readonly AssetRepository assetRepository;
    private readonly LayerRepository layerRepository;

    public SoundManager(
        ArchiveSession session,
        AssetRepository assetRepository,
        LayerRepository layerRepository)
    {
        this.session = session;
        this.assetRepository = assetRepository;
        this.layerRepository = layerRepository;
    }

    private IEnumerable<Asset> SoundInfos => assetRepository.Assets.Where(asset => asset.assetType == AssetType.SoundInfo);

    public void AddToSoundInfo(AssetSound sound)
    {
        Asset soundInfo;

        if (assetRepository.ContainsType(AssetType.SoundInfo))
        {
            soundInfo = SoundInfos.First();
        }
        else
        {
            soundInfo = AssetFactory.GenerateTemplate(session.Game, session.Platform, assetRepository.Contains, AssetTemplate.Sound_Info)[0];
            assetRepository.Add(soundInfo);
            var layerIndex = layerRepository.GetOrCreateLayerOfType(LayerType.SNDTOC);
            layerRepository.AddAsset(layerIndex, soundInfo.assetID);
        }

        byte[] finalData;

        switch (soundInfo)
        {
            case AssetSNDI_GCN_V1 SNDI_G1:
                SNDI_G1.AddEntry(sound.Data, sound.assetID, sound.assetType, out finalData);
                sound.Data = finalData;

                SNDI_G1.InvokePropertyChanged(false);
                sound.InvokePropertyChanged(false);
                break;
            case AssetSNDI_GCN_V2 SNDI_G2:
                SNDI_G2.AddEntry(sound.Data, sound.assetID, sound.assetType);
                sound.Data = [];

                SNDI_G2.InvokePropertyChanged(false);
                sound.InvokePropertyChanged(false);
                break;
            case AssetSNDI_XBOX SNDI_X:
                SNDI_X.AddEntry(sound.Data, sound.assetID, sound.assetType, out finalData);
                sound.Data = finalData;

                SNDI_X.InvokePropertyChanged(false);
                sound.InvokePropertyChanged(false);
                break;
            case AssetSNDI_PS2 SNDI_P:
                var data = sound.Data;
                InternalSoundEditor.VagHeaderToLittleEndian(ref data);
                SNDI_P.AddEntry(data, sound.assetID, sound.assetType, out finalData);
                sound.Data = finalData;

                SNDI_P.InvokePropertyChanged(false);
                sound.InvokePropertyChanged(false);
                break;
            default:
                throw new Exception("Unable to add sound: Sound Info asset not found");
        }
    }

    public void RemoveFromSoundInfo(uint assetID, AssetType assetType)
    {
        if (assetType != AssetType.Sound && assetType != AssetType.SoundStream)
            return;

        var soundInfos = SoundInfos.Where(asset => asset.HasReference(assetID));
        foreach (Asset a in soundInfos)
            switch (a)
            {
                case AssetSNDI_GCN_V1 SNDI_G1:
                    SNDI_G1.RemoveEntry(assetID, assetType);
                    break;
                case AssetSNDI_GCN_V2 SNDI_G2:
                    SNDI_G2.RemoveEntry(assetID);
                    break;
                case AssetSNDI_XBOX SNDI_X:
                    SNDI_X.RemoveEntry(assetID, assetType);
                    break;
                case AssetSNDI_PS2 SNDI_P:
                    SNDI_P.RemoveEntry(assetID, assetType);
                    break;
            }
        foreach (Asset a in soundInfos)
            a.InvokePropertyChanged(false);
        var jawData = assetRepository.Assets.OfType<AssetJAW>().Where(asset => asset.HasReference(assetID));
        foreach (var jaw in jawData)
        {
            jaw.RemoveEntry(assetID);
            jaw.InvokePropertyChanged(false);
        }
    }

    public byte[] GetHeaderFromSoundInfo(uint assetID, AssetType assetType)
    {
        var soundInfos = SoundInfos.Where(asset => asset.HasReference(assetID));
        foreach (Asset a in soundInfos)
        {
            switch (a)
            {
                case AssetSNDI_GCN_V1 SNDI_G1:
                    if (SNDI_G1.HasReference(assetID))
                        return SNDI_G1.GetHeader(assetID, assetType);
                    break;
                case AssetSNDI_GCN_V2 SNDI_G2:
                    if (SNDI_G2.HasReference(assetID))
                        return SNDI_G2.GetHeader(assetID);
                    break;
                case AssetSNDI_XBOX SNDI_X:
                    if (SNDI_X.HasReference(assetID))
                        return SNDI_X.GetHeader(assetID, assetType);
                    break;
                case AssetSNDI_PS2 SNDI_P:
                    if (SNDI_P.HasReference(assetID))
                        return SNDI_P.GetHeader(assetID, assetType);
                    break;
            }
        }

        throw new Exception("Error: could not find SNDI asset which contains this sound in this archive.");
    }

    public byte[] GetCompleteSoundData(uint assetID, AssetType assetType, byte[] previousData)
    {
        List<byte> file = [.. GetHeaderFromSoundInfo(assetID, assetType), .. previousData];

        if (new string([(char)file[0], (char)file[1], (char)file[2], (char)file[3]]) == "RIFF")
        {
            byte[] chunkSizeArr = BitConverter.GetBytes(file.Count - 8);

            file[4] = chunkSizeArr[0];
            file[5] = chunkSizeArr[1];
            file[6] = chunkSizeArr[2];
            file[7] = chunkSizeArr[3];
        }

        return file.ToArray();
    }
}