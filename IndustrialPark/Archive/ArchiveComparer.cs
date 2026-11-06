using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public readonly record struct ArchiveAssetKey(uint AssetID, AssetType AssetType);

public class ArchiveComparer
{
    private readonly Archive source;
    private readonly Archive destination;

    public ArchiveComparer(Archive source, Archive destination)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
    }

    public static ArchiveComparisonResult CompareArchives(Archive source, Archive destination) =>
        new ArchiveComparer(source, destination).Compare();

    public static Archive CreateDeltaArchive(Archive source, Archive destination) =>
        new ArchiveComparer(source, destination).CreateDeltaArchive();

    public ArchiveComparisonResult Compare()
    {
        var sourceAssets = source.Assets.ToDictionary(GetAssetKey);
        var destinationAssets = destination.Assets.ToDictionary(GetAssetKey);
        var sourceEndianness = source.Platform.Endianness();
        var destinationEndianness = destination.Platform.Endianness();

        var sourceOnly = new List<ArchiveAssetComparison>();
        var destinationOnly = new List<ArchiveAssetComparison>();
        var identical = new List<ArchiveAssetComparison>();
        var modified = new List<ArchiveAssetComparison>();

        foreach (var key in sourceAssets.Keys.Concat(destinationAssets.Keys).Distinct())
        {
            sourceAssets.TryGetValue(key, out var sourceAsset);
            destinationAssets.TryGetValue(key, out var destinationAsset);

            if (sourceAsset == null)
            {
                destinationOnly.Add(new ArchiveAssetComparison(null, destinationAsset, null,
                    destinationAsset.Serialize(destinationEndianness)));
                continue;
            }

            if (destinationAsset == null)
            {
                sourceOnly.Add(new ArchiveAssetComparison(sourceAsset, null,
                    sourceAsset.Serialize(sourceEndianness), null));
                continue;
            }

            var sourceData = sourceAsset.Serialize(sourceEndianness);
            var destinationData = destinationAsset.Serialize(destinationEndianness);
            var comparison = new ArchiveAssetComparison(sourceAsset, destinationAsset, sourceData, destinationData);

            if (sourceData.SequenceEqual(destinationData))
                identical.Add(comparison);
            else
                modified.Add(comparison);
        }

        return new ArchiveComparisonResult(sourceOnly, destinationOnly, identical, modified);
    }

    public Archive CreateDeltaArchive()
    {
        var comparison = Compare();
        var delta = new Archive(
            destination.Platform,
            destination.Game,
            destination.FileDate,
            destination.DateString,
            autoLayers: true,
            addDefaultAssets: false,
            subscriptions: false);

        foreach (var assetComparison in comparison.AssetsOnlyInDestination.Concat(comparison.ModifiedAssets))
            delta.AddNewAsset(assetComparison.DestinationAsset, -1);

        foreach (var assetComparison in comparison.AssetsOnlyInSource)
        {
            var sourceAsset = assetComparison.SourceAsset;
            var removedAsset = new AssetGeneric(sourceAsset.assetName, sourceAsset.assetType, []) { assetID = sourceAsset.assetID };
            delta.AddNewAsset(removedAsset, -1);
        }
        
        // identical assets: skip

        return delta;
    }

    private static ArchiveAssetKey GetAssetKey(Asset asset) => new(asset.assetID, asset.assetType);
}

public class ArchiveComparisonResult
{
    public IReadOnlyList<ArchiveAssetComparison> AssetsOnlyInSource { get; }
    public IReadOnlyList<ArchiveAssetComparison> AssetsOnlyInDestination { get; }
    public IReadOnlyList<ArchiveAssetComparison> IdenticalAssets { get; }
    public IReadOnlyList<ArchiveAssetComparison> ModifiedAssets { get; }

    public ArchiveComparisonResult(
        IReadOnlyList<ArchiveAssetComparison> assetsOnlyInSource,
        IReadOnlyList<ArchiveAssetComparison> assetsOnlyInDestination,
        IReadOnlyList<ArchiveAssetComparison> identicalAssets,
        IReadOnlyList<ArchiveAssetComparison> modifiedAssets)
    {
        AssetsOnlyInSource = assetsOnlyInSource;
        AssetsOnlyInDestination = assetsOnlyInDestination;
        IdenticalAssets = identicalAssets;
        ModifiedAssets = modifiedAssets;
    }
}

public class ArchiveAssetComparison
{
    public Asset SourceAsset { get; }
    public Asset DestinationAsset { get; }
    public byte[] SourceData { get; }
    public byte[] DestinationData { get; }

    public ArchiveAssetComparison(Asset sourceAsset, Asset destinationAsset, byte[] sourceData, byte[] destinationData)
    {
        SourceAsset = sourceAsset;
        DestinationAsset = destinationAsset;
        SourceData = sourceData;
        DestinationData = destinationData;
    }
}
