using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;

namespace IndustrialPark.Archive;

public static class SoundService
{
    public static byte[] CreateSoundFile(Game game, Platform platform, string fileName, bool ps2Looping = false, bool compress = true, bool forcemono = false, int samplerate = 0) => platform switch
    {
        Platform.Xbox => SoundUtility_XboxADPCM.ConvertSoundToXboxADPCM(fileName, compress, forcemono, samplerate),
        Platform.PS2 => SoundUtility_PS2VAG.ConvertSoundToPS2VAG(fileName, ps2Looping, samplerate),
        Platform.GameCube when game >= Game.Incredibles => SoundUtility_FMOD.ConvertSoundToFSB3(fileName, samplerate, forcemono, compress),
        Platform.GameCube => SoundUtility_DSP.ConvertSoundToDSP(fileName, samplerate),
        _ => null
    };

    public static IEnumerable<Section_AHDR> CreateSounds(Game game, Platform platform, AssetType assetType, bool raw, string[] fileNames, Action<string> step = null)
    {
        var assets = new List<Section_AHDR>();

        foreach (var fileName in fileNames)
        {
            var assetName = Path.GetFileNameWithoutExtension(fileName);
            var assetID = Functions.BKDRHash(assetName);

            byte[] data = raw ? File.ReadAllBytes(fileName) : CreateSoundFile(game, platform, fileName);
            if (data == null)
                continue;

            var AHDR = new Section_AHDR(assetID, assetType, AssetService.AHDRFlagsFromAssetType(assetType), new Section_ADBG(0, assetName, "", 0), data);

            assets.Add(AHDR);
            step?.Invoke(fileName);
        }

        return assets;
    }
}
