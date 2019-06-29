using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public void Shuffle()
        {
            {
                // shuffle pkup positions
                List<AssetPKUP> assets = new List<AssetPKUP>();

                foreach (Asset a in assetDictionary.Values)
                    if (a is AssetPKUP pkup)
                        assets.Add(pkup);

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
            {
                // shuffle tiki types
                List<AssetVIL> assets = new List<AssetVIL>();

                foreach (Asset a in assetDictionary.Values)
                    if (a is AssetVIL tiki && (
                        tiki.VilType == VilType.tiki_shhhh_bind ||
                        tiki.VilType == VilType.tiki_stone_bind ||
                        tiki.VilType == VilType.tiki_thunder_bind ||
                        tiki.VilType == VilType.tiki_wooden_bind))
                        assets.Add(tiki);

                List<VilType> viltypes = (from asset in assets where true select asset.VilType).ToList();
                List<AssetID> models = (from asset in assets where true select asset.Model_AssetID).ToList();

                Random r = new Random((int)DateTime.Now.ToBinary());
                
                foreach (AssetVIL a in assets)
                {
                    int value = r.Next(0, viltypes.Count);
                    int value2 = r.Next(0, viltypes.Count);

                    a.VilType = viltypes[value];
                    a.Model_AssetID = models[value2];

                    viltypes.RemoveAt(value);
                    models.RemoveAt(value2);
                }
            }
            {
                // shuffle mrkr positions
                List<IClickableAsset> assets = new List<IClickableAsset>();

                foreach (Asset a in assetDictionary.Values)
                    if (a is AssetMRKR tiki)
                    {
                        string name = tiki.AHDR.ADBG.assetName.ToLower();

                        if (name.Contains("checkp") || name.Contains("from") || name.Contains("spat") || name.Contains("mk") || name.Contains("tbox") || name.Contains("tele"))
                            assets.Add(tiki);
                    }
                    else if (a is AssetDYNA dyna && dyna.Type_BFBB == DynaType_BFBB.pointer)
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
            {
                Random r = new Random((int)DateTime.Now.ToBinary());

                float minMulti = 0.5f;
                float maxMulti = 1f;

                // shuffle plat speed
                    foreach (Asset a in assetDictionary.Values)
                        if (a is AssetPLAT plat)
                        {
                            if (plat.PlatSpecific is PlatSpecific_ConveryorBelt p)
                            {
                                p.Speed *= r.NextFloat(minMulti, maxMulti);
                                plat.PlatSpecific = p;
                            }
                            else if (plat.PlatSpecific is PlatSpecific_FallingPlatform pf)
                            {
                                pf.Speed *= r.NextFloat(minMulti, maxMulti);
                                plat.PlatSpecific = pf;
                            }
                            else if (plat.PlatSpecific is PlatSpecific_BreakawayPlatform b)
                            {
                                b.BreakawayDelay *= r.NextFloat(minMulti, maxMulti);
                                b.ResetDelay *= r.NextFloat(minMulti, maxMulti);
                                plat.PlatSpecific = b;
                            }
                            else if (plat.PlatSpecific is PlatSpecific_TeeterTotter tt)
                            {
                                tt.InverseMass *= r.NextFloat(minMulti, maxMulti);
                                plat.PlatSpecific = tt;
                            }

                            if (plat.Motion is Motion_MovePoint mp)
                            {
                                mp.Speed *= r.NextFloat(minMulti, maxMulti);
                                plat.Motion = mp;
                            }
                            else if (plat.Motion is Motion_Mechanism mc)
                            {
                                mc.PostRetractDelay *= r.NextFloat(minMulti, maxMulti);
                                mc.RetractDelay *= r.NextFloat(minMulti, maxMulti);
                                mc.RotateAccelTime *= r.NextFloat(minMulti, maxMulti);
                                mc.RotateDecelTime *= r.NextFloat(minMulti, maxMulti);
                                mc.RotateTime *= r.NextFloat(minMulti, maxMulti);
                                mc.SlideAccelTime *= r.NextFloat(minMulti, maxMulti);
                                mc.SlideDecelTime *= r.NextFloat(minMulti, maxMulti);
                                mc.SlideTime *= r.NextFloat(minMulti, maxMulti);
                                plat.Motion = mc;
                            }
                        }
            }
        }
    }
}