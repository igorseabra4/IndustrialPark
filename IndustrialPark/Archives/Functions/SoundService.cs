using HipHopFile;
using IndustrialPark.Assets;
using IndustrialPark.SoundUtility;
using System;
using System.Collections.Generic;
using System.IO;

namespace IndustrialPark.Archives;

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

    public static IEnumerable<AssetSound> CreateSounds(Game game, Platform platform, AssetType assetType, bool raw, string[] fileNames, Action<string> step = null)
    {
        var assets = new List<AssetSound>();

        foreach (var fileName in fileNames)
        {
            var assetName = Path.GetFileNameWithoutExtension(fileName);

            byte[] data = raw ? File.ReadAllBytes(fileName) : CreateSoundFile(game, platform, fileName);
            if (data == null)
                continue;

            var sound = new AssetSound(assetName, assetType, game, platform, data);
            assets.Add(sound);
            step?.Invoke(fileName);
        }

        return assets;
    }
}
