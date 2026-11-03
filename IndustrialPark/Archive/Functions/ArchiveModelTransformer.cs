using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public static class ArchiveModelTransformer
{
    public static IEnumerable<AssetType> ScalableAssetTypesOnArchive(Archive archive) => archive.Assets.Where(asset =>
        asset is IClickableAsset ||
        (archive.Game == Game.BFBB && asset is AssetJSP) ||
        (archive.Game == Game.BFBB && asset is AssetJSP_INFO) ||
        asset is AssetLODT ||
        asset is AssetFLY).Select(asset => asset.assetType).Distinct();

    public static void ApplyScale(Archive archive, Vector3 factor, IEnumerable<AssetType> assetTypes = null, bool bakeEntityUnproportionalScales = true, bool bakeNpcsVilScales = false)
    {
        if (factor.X == 1f && factor.Y == 1f && factor.Z == 1f)
        {
            MessageBox.Show("Scale not applied as the scale vector is (1, 1, 1).");
            return;
        }

        var actions = new List<IReversibleAction>();

        float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

        var assets = archive.Assets.Where(a => assetTypes == null || assetTypes.Contains(a.assetType)).ToArray();

        foreach (Asset a in assets)
        {
            if (a is IVolumeAsset volume)
            {
                volume.ApplyScale(factor, singleFactor);
            }
            else if (a is AssetVOLU volu)
            {
                volu.ApplyScale(factor, singleFactor);
            }
            else if (a is AssetMVPT MVPT)
            {
                MVPT.PositionX *= factor.X;
                MVPT.PositionY *= factor.Y;
                MVPT.PositionZ *= factor.Z;

                if (MVPT.ZoneRadius != -1)
                    MVPT.ZoneRadius *= singleFactor;
                if (MVPT.ArenaRadius != -1)
                    MVPT.ArenaRadius *= singleFactor;
            }
            else if (a is AssetSFX SFX)
            {
                SFX.PositionX *= factor.X;
                SFX.PositionY *= factor.Y;
                SFX.PositionZ *= factor.Z;

                SFX.OuterRadius *= singleFactor;
                SFX.InnerRadius *= singleFactor;
            }
            else if (a is AssetBOUL BOUL)
            {
                BOUL.PositionX *= factor.X;
                BOUL.PositionY *= factor.Y;
                BOUL.PositionZ *= factor.Z;

                BOUL.ScaleX *= factor.X;
                BOUL.ScaleY *= factor.Y;
                BOUL.ScaleZ *= factor.Z;

                BOUL.OuterRadius *= singleFactor;
                BOUL.InnerRadius *= singleFactor;
            }
            else if (a is AssetSGRP SGRP)
            {
                SGRP.OuterRadius *= singleFactor;
                SGRP.InnerRadius *= singleFactor;
            }
            else if (a is AssetPKUP PKUP)
            {
                PKUP.PositionX *= factor.X;
                PKUP.PositionY *= factor.Y;
                PKUP.PositionZ *= factor.Z;
            }
            else if (a is EntityAsset placeable && !(a is AssetPLYR || a is AssetUI || a is AssetUIFT))
            {
                placeable.PositionX *= factor.X;
                placeable.PositionY *= factor.Y;
                placeable.PositionZ *= factor.Z;

                if (placeable is AssetNPC || placeable is AssetVIL)
                {
                    if (bakeNpcsVilScales)
                        placeable.Model = ApplyBakeScale(placeable.assetName, placeable.Model, factor);
                }
                else if (factor.X != factor.Y || factor.X != factor.Z || factor.Y != factor.Z)
                {
                    if (bakeEntityUnproportionalScales)
                        placeable.Model = ApplyBakeScale(placeable.assetName, placeable.Model, factor);
                }
                else
                {
                    placeable.ScaleX *= factor.X;
                    placeable.ScaleY *= factor.Y;
                    placeable.ScaleZ *= factor.Z;
                }
            }
            else if (a is DynaEnemy enemysb)
            {
                enemysb.PositionX *= factor.X;
                enemysb.PositionY *= factor.Y;
                enemysb.PositionZ *= factor.Z;

                //if (bakeNpcsVilScales)
                //    enemysb.Model = ApplyBakeScale(enemysb.Model, factor);
            }
            else if (a is DynaGObjectTrainCar tcar)
            {
                tcar.PositionX *= factor.X;
                tcar.PositionY *= factor.Y;
                tcar.PositionZ *= factor.Z;

                //if (bakeEntityUnproportionalScales)
                //    tcar.Model = ApplyBakeScale(tcar.Model, factor);
            }
            else if (a is IClickableAsset ica && a is not DynaGObjectTeleport)
            {
                ica.PositionX *= factor.X;
                ica.PositionY *= factor.Y;
                ica.PositionZ *= factor.Z;
            }
            else if (a is AssetJSP jsp)
            {
                jsp.ApplyScale(factor);
            }
            else if (a is AssetJSP_INFO jspinfo)
            {
                jspinfo.ApplyScale(factor);
            }
            else if (a is AssetLODT lodt && singleFactor > 1.0)
            {
                var entries = lodt.Entries;
                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i].MaxDistance *= singleFactor;
                    entries[i].LOD1_MinDistance *= singleFactor;
                    entries[i].LOD2_MinDistance *= singleFactor;
                    entries[i].LOD3_MinDistance *= singleFactor;
                }
                lodt.Entries = entries;
            }
            else if (a is AssetFLY fly)
            {
                var entries = fly.Frames;
                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i].CameraPosition.X *= factor.X;
                    entries[i].CameraPosition.Y *= factor.Y;
                    entries[i].CameraPosition.Z *= factor.Z;
                }
                fly.Frames = entries;
            }
            else
            {
                continue;
            }
            a.InvokePropertyChanged(false);
        }
    }

    public static uint ApplyBakeScale(string assetName, uint modelAssetId, Vector3 scale)
    {
        if (scale.X == 1f && scale.Y == 1f && scale.Z == 1f)
        {
            MessageBox.Show("Bake scale not applied as the scale vector is (1, 1, 1).");
            return modelAssetId;
        }

        (Archive archive, Asset model) bsmc = (null, null);
        int count = 0;

        foreach (var ae in Program.ArchiveEditors)
            if (ae.archive.TryGetAsset(modelAssetId, out Asset asset) && asset is IAssetWithModel model)
            {
                bsmc = (ae.archive, asset);
                count++;
                if (count > 1)
                {
                    MessageBox.Show($"Unable to bake scale for asset {assetName}: model 0x{modelAssetId:X8} found in more than one open archive.");
                    return modelAssetId;
                }
            }

        if (count == 0)
            MessageBox.Show($"Unable to bake scale for asset {assetName}: model 0x{modelAssetId:X8} not found in open archives.");
        else if (count == 1)
            return ApplyBakeScaleLocal(bsmc.archive, bsmc.model, scale);

        return modelAssetId;
    }

    public static uint ApplyBakeScaleLocal(Archive archive, Asset model, Vector3 scale)
    {
        var AHDR = model.BuildAHDR(archive.Platform.Endianness());
        var newAssetName = AHDR.ADBG.assetName + $"_{scale.X:.0###########}_{scale.Y:.0###########}_{scale.Z:.0###########}";
        var newAssetId = Functions.BKDRHash(newAssetName);

        if (!archive.ContainsAsset(newAssetId))
        {
            AHDR.ADBG.assetName = newAssetName;
            AHDR.assetID = newAssetId;

            var layerIndex = archive.GetLayerFromAssetID(model.assetID);

            var newModel = archive.AddNewAsset(AHDR, archive.Game, archive.Platform, layerIndex);
            ((IAssetWithModel)newModel).ApplyScale(scale);
        }

        return newAssetId;
    }

    public static uint ApplyBakeRotation(uint modelAssetId, float yaw, float pitch, float roll)
    {
        if (yaw == 0f && pitch == 0f && roll == 0f)
        {
            MessageBox.Show("Bake rotation not applied as the rotation vector is (0, 0, 0).");
            return modelAssetId;
        }

        (Archive archive, AssetMODL model) bsmc = (null, null);
        int count = 0;

        foreach (var ae in Program.ArchiveEditors)
            if (ae.archive.TryGetAsset(modelAssetId, out Asset asset) && asset is AssetMODL model)
            {
                bsmc = (ae.archive, model);
                count++;
                if (count > 1)
                {
                    MessageBox.Show("Unable to bake rotation: model found in more than one open archive.");
                    return modelAssetId;
                }
            }

        if (count == 0)
            MessageBox.Show("Unable bake rotation: model not found in open archives.");
        else if (count == 1)
            return ApplyBakeRotationLocal(bsmc.archive, bsmc.model, yaw, pitch, roll);

        return modelAssetId;
    }

    public static uint ApplyBakeRotationLocal(Archive archive, AssetMODL model, float yaw, float pitch, float roll)
    {
        var AHDR = model.BuildAHDR(archive.Platform.Endianness());
        var newAssetName = AHDR.ADBG.assetName + $"_{MathUtil.RadiansToDegrees(yaw):.0###########}_{MathUtil.RadiansToDegrees(pitch):.0###########}_{MathUtil.RadiansToDegrees(roll):.0###########}";
        var newAssetId = Functions.BKDRHash(newAssetName);

        if (!archive.ContainsAsset(newAssetId))
        {
            AHDR.ADBG.assetName = newAssetName;
            AHDR.assetID = newAssetId;

            var layerIndex = archive.GetLayerFromAssetID(model.assetID);

            var newModel = (AssetMODL)archive.AddNewAsset(AHDR, archive.Game, archive.Platform, layerIndex);
            newModel.ApplyRotation(yaw, pitch, roll);
        }

        return newAssetId;
    }
}
