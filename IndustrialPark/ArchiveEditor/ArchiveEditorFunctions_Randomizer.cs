using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using HipHopFile;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public bool Shuffle(RandomizerFlags flags, float minSpeed, float maxSpeed, float minTime, float maxTime)
        {
            bool shuffled = false;
            
            if (ShouldShuffle(flags, RandomizerFlags.Textures, AssetType.RWTX))
            {
                ShuffleData(AssetType.RWTX);
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Models, AssetType.MODL))
            {
                ShuffleData(AssetType.MODL);
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Boulder_Settings, AssetType.BOUL))
            {
                ShuffleBoulders();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Sounds, AssetType.SNDI))
            {
                ShuffleSounds(ShouldShuffle(flags, RandomizerFlags.Mix_SND_SNDS));
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Pickup_Positions, AssetType.PKUP))
            {
                ShufflePKUPPositions();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.MVPT_Radius, AssetType.MVPT))
            {
                ShuffleMVPT();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Tiki_Types, AssetType.VIL))
            {
                ShuffleVilTypes(new VilType[] {
                    VilType.tiki_shhhh_bind,
                    VilType.tiki_stone_bind,
                    VilType.tiki_thunder_bind,
                    VilType.tiki_wooden_bind },

                    ShouldShuffle(flags, RandomizerFlags.Tiki_Models),
                    ShouldShuffle(flags, RandomizerFlags.Tiki_Allow_Any_Type));

                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Enemy_Types, AssetType.VIL))
            {
                ShuffleVilTypes(new VilType[] {
                    VilType.g_love_bind,
                    VilType.ham_bind,
                    VilType.robot_0a_bomb_bind,
                    VilType.robot_0a_bzzt_bind,
                    VilType.robot_0a_chomper_bind,
                    VilType.robot_0a_fodder_bind,
                    VilType.robot_4a_monsoon_bind,
                    VilType.robot_9a_bind,
                    VilType.robot_chuck_bind,
                    VilType.robot_sleepytime_bind,
                    VilType.robot_tar_bind },

                    ShouldShuffle(flags, RandomizerFlags.Enemy_Models), 
                    ShouldShuffle(flags, RandomizerFlags.Enemies_Allow_Any_Type));

                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.MRKR_Positions, AssetType.MRKR))
            {
                ShuffleMRKRPositions(ShouldShuffle(flags, RandomizerFlags.DYNA_Pointers));
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.SIMP_Positions, AssetType.SIMP))
            {
                ShuffleSIMPPositions();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.PLAT_Speeds, AssetType.PLAT))
            {
                ShufflePlatSpeeds(minSpeed, maxSpeed, minTime, maxTime);
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Timers, AssetType.TIMR))
            {
                ShuffleTimers();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Cameras, AssetType.CAM))
            {
                ShuffleCameras();
                shuffled = true;
            }

            if (ShouldShuffle(flags, RandomizerFlags.Double_LODT_Distances, AssetType.LODT))
            {
                DoubleLODT();
                shuffled = true;
            }

            return shuffled;
        }

        private bool ShouldShuffle(RandomizerFlags flags, RandomizerFlags flag)
            => (flags & flag) != 0;

        private bool ShouldShuffle(RandomizerFlags flags, RandomizerFlags flag, AssetType assetType)
            => (flags & flag) != 0 && GetAssetsOfType(assetType).Any();

        private void ShuffleData(AssetType assetType)
        {
            List<Asset> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == assetType select asset).ToList();

            List<byte[]> datas = (from asset in assets where true select asset.Data).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (Asset a in assets)
            {
                int value = r.Next(0, datas.Count);

                a.Data = datas[value];

                datas.RemoveAt(value);
            }
        }

        private void ShufflePKUPPositions()
        {
            List<AssetPKUP> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.PKUP select asset).Cast<AssetPKUP>().ToList();

            List<Vector3> positions = (from asset in assets where true select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (AssetPKUP a in assets)
            {
                int value = r.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);
            }
        }

        private void ShuffleCameras()
        {
            List<AssetCAM> assets = (from asset in assetDictionary.Values where asset.AHDR.assetType == AssetType.CAM select asset).Cast<AssetCAM>().ToList();

            List<Vector3> positions = (from asset in assets where true select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();
            List<Vector3[]> angles = (from asset in assets where true select (new Vector3[] {
                new Vector3(asset.NormalizedForwardX, asset.NormalizedForwardY, asset.NormalizedForwardZ),
                new Vector3(asset.NormalizedUpX, asset.NormalizedUpY, asset.NormalizedUpZ),
                new Vector3(asset.NormalizedLeftX, asset.NormalizedLeftY, asset.NormalizedLeftZ),
                new Vector3(asset.ViewOffsetX, asset.ViewOffsetY, asset.ViewOffsetZ)
            })).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (AssetCAM a in assets)
            {
                int value1 = r.Next(0, positions.Count);
                int value2 = r.Next(0, angles.Count);

                a.PositionX = positions[value1].X;
                a.PositionY = positions[value1].Y;
                a.PositionZ = positions[value1].Z;

                a.NormalizedForwardX = angles[value2][0].X;
                a.NormalizedForwardY = angles[value2][0].Y;
                a.NormalizedForwardZ = angles[value2][0].Z;

                a.NormalizedUpX = angles[value2][1].X;
                a.NormalizedUpY = angles[value2][1].Y;
                a.NormalizedUpZ = angles[value2][1].Z;

                a.NormalizedLeftX = angles[value2][2].X;
                a.NormalizedLeftY = angles[value2][2].Y;
                a.NormalizedLeftZ = angles[value2][2].Z;

                a.ViewOffsetX = angles[value2][3].X;
                a.ViewOffsetY = angles[value2][3].Y;
                a.ViewOffsetZ = angles[value2][3].Z;

                positions.RemoveAt(value1);
                angles.RemoveAt(value2);
            }
        }

        private void ShuffleSIMPPositions()
        {
            List<AssetSIMP> assets = (from asset in assetDictionary.Values where
                                      asset is AssetSIMP simp && FindWhoTargets(simp.AssetID).Count == 0
                                      select asset).Cast<AssetSIMP>().ToList();

            List<Vector3> positions = (from asset in assets where true select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (AssetSIMP a in assets)
            {
                int value = r.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);
            }
        }
        
        private void ShuffleVilTypes(VilType[] allowed, bool mixModels, bool veryRandom)
        {
            List<AssetVIL> assets = (from asset in assetDictionary.Values where asset is AssetVIL tiki && allowed.Contains(tiki.VilType) select asset).Cast<AssetVIL>().ToList();
            List<VilType> viltypes = (from asset in assets where true select asset.VilType).ToList();
            List<AssetID> models = (from asset in assets where true select asset.Model_AssetID).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (AssetVIL a in assets)
            {
                int viltypes_value = r.Next(0, viltypes.Count);
                int model_value = mixModels ? r.Next(0, viltypes.Count) : viltypes_value;

                a.VilType = veryRandom ? allowed[r.Next(0, allowed.Length)] : viltypes[viltypes_value];
                a.Model_AssetID = models[model_value];
                
                viltypes.RemoveAt(viltypes_value);
                models.RemoveAt(model_value);
            }
        }

        private void ShuffleMRKRPositions(bool includeDynaPointers)
        {
            List<IClickableAsset> assets = new List<IClickableAsset>();

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetMRKR mrkr)
                {
                    List<uint> whoTargets = FindWhoTargets(mrkr.AHDR.assetID);

                    if (whoTargets.Count > 0 && GetFromAssetID(whoTargets[0]) is AssetDYNA)
                        continue;
                    assets.Add(mrkr);
                    //string name = mrkr.AHDR.ADBG.assetName.ToLower();
                }
                else if (includeDynaPointers && a is AssetDYNA dyna && dyna.Type_BFBB == DynaType_BFBB.pointer)
                    assets.Add(dyna);

            List<Vector3> positions = (from asset in assets where true select (new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ))).ToList();

            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (IClickableAsset a in assets)
            {
                int value = r.Next(0, positions.Count);

                a.PositionX = positions[value].X;
                a.PositionY = positions[value].Y;
                a.PositionZ = positions[value].Z;

                positions.RemoveAt(value);
            }
        }

        private void ShufflePlatSpeeds(float minMultiSpeed, float maxMultiSpeed, float minMultiTime, float maxMultiTime)
        {
            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPLAT plat)
                {
                    if (plat.PlatSpecific is PlatSpecific_ConveryorBelt p)
                    {
                        p.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = p;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_FallingPlatform pf)
                    {
                        pf.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = pf;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_BreakawayPlatform b)
                    {
                        b.BreakawayDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        b.ResetDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        plat.PlatSpecific = b;
                    }
                    else if (plat.PlatSpecific is PlatSpecific_TeeterTotter tt)
                    {
                        tt.InverseMass *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.PlatSpecific = tt;
                    }

                    if (plat.Motion is Motion_MovePoint mp)
                    {
                        mp.Speed *= r.NextFloat(minMultiSpeed, maxMultiSpeed);
                        plat.Motion = mp;
                    }
                    else if (plat.Motion is Motion_Mechanism mc)
                    {
                        mc.PostRetractDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RetractDelay *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateAccelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateDecelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.RotateTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideAccelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideDecelTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        mc.SlideTime *= r.NextFloat(minMultiTime, maxMultiTime);
                        plat.Motion = mc;
                    }
                }
        }

        private void ShuffleBoulders()
        {
            Random r = new Random((int)DateTime.Now.ToBinary());

            float min = 0.5f;
            float max = 2f;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetBOUL boul)
                {
                    boul.Gravity *= r.NextFloat(min, max);
                    boul.Mass *= r.NextFloat(min, max);
                    boul.BounceFactor *= r.NextFloat(min, max);
                    boul.Friction *= r.NextFloat(min, max);
                    boul.StartFriction *= r.NextFloat(min, max); 
                    boul.MaxLinearVelocity *= r.NextFloat(min, max);
                    boul.MaxAngularVelocity *= r.NextFloat(min, max);
                    boul.Stickiness *= r.NextFloat(min, max);
                    boul.BounceDamp *= r.NextFloat(min, max);
                    boul.KillTimer *= r.NextFloat(min, max);
                    boul.InnerRadius *= r.NextFloat(min, max);
                    boul.OuterRadius *= r.NextFloat(min, max);
                }
        }

        private void ShuffleSounds(bool mixTypes)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V1 sndi)
                {
                    List<EntrySoundInfo_GCN_V1> snd = sndi.Entries_SND.ToList();
                    List<EntrySoundInfo_GCN_V1> snds = sndi.Entries_SNDS.ToList();

                    Random r = new Random((int)DateTime.Now.ToBinary());

                    if (mixTypes)
                    {
                        List<(byte[], byte[])> sounds = new List<(byte[], byte[])>();

                        foreach (var v in snd)
                            sounds.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snds)
                            sounds.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snd)
                        {
                            int index = r.Next(0, sounds.Count);
                            v.SoundHeader = sounds[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = sounds[index].Item2;
                            sounds.RemoveAt(index);
                        }

                        foreach (var v in snds)
                        {
                            int index = r.Next(0, sounds.Count);
                            v.SoundHeader = sounds[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = sounds[index].Item2;
                            sounds.RemoveAt(index);
                        }
                    }
                    else
                    {
                        List<(byte[], byte[])> soundsSND = new List<(byte[], byte[])>();

                        foreach (var v in snd)
                            soundsSND.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snd)
                        {
                            int index = r.Next(0, soundsSND.Count);
                            v.SoundHeader = soundsSND[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = soundsSND[index].Item2;
                            soundsSND.RemoveAt(index);
                        }

                        List<(byte[], byte[])> soundsSNDS = new List<(byte[], byte[])>();

                        foreach (var v in snds)
                            soundsSNDS.Add((v.SoundHeader, GetFromAssetID(v.SoundAssetID).Data));

                        foreach (var v in snds)
                        {
                            int index = r.Next(0, soundsSNDS.Count);
                            v.SoundHeader = soundsSNDS[index].Item1;
                            GetFromAssetID(v.SoundAssetID).Data = soundsSNDS[index].Item2;
                            soundsSNDS.RemoveAt(index);
                        }
                    }

                    sndi.Entries_SND = snd.ToArray();
                    sndi.Entries_SNDS = snds.ToArray();
                }
        }

        private void DoubleLODT()
        {
            float value = 2f;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetLODT lodt)
                {
                    EntryLODT[] entries = lodt.LODT_Entries;
                    
                    foreach (var v in entries)
                    {
                        v.MaxDistance *= value;
                        v.LOD1_Distance *= value;
                        v.LOD2_Distance *= value;
                        v.LOD3_Distance *= value;
                    }

                    lodt.LODT_Entries = entries;
                }
        }

        private void ShuffleMVPT()
        {
            float min = 0.9f;
            float max = 1.8f;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetMVPT_Scooby mvpt)
                {
                    Random r = new Random((int)DateTime.Now.ToBinary());

                    if (mvpt.ArenaRadius != -1)
                        mvpt.ArenaRadius *= r.NextFloat(min, max);
                    if (mvpt is AssetMVPT mvpts)
                    {
                        if (mvpts.ZoneRadius != -1)
                            mvpts.ZoneRadius *= r.NextFloat(min, max);
                        if (mvpts.Delay != -1)
                            mvpts.Delay *= r.NextFloat(min, max);
                    }
                }
        }

        private void ShuffleTimers()
        {
            Random r = new Random((int)DateTime.Now.ToBinary());

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetTIMR timr)
                    timr.Time *= r.NextFloat(0.75f, 1.75f);
        }
    }
}