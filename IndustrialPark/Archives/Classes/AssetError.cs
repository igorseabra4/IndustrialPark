using HipHopFile;

namespace IndustrialPark.Archives;

public struct AssetError
{
    public ErrorReason reason;
    public uint assetID;
    public string assetName;
    public AssetType assetType;
    public string exception;

    public override string ToString() =>
        $"[{assetID:X8}] {assetName} ({AssetTypeContainer.AssetTypeToString(assetType)}) {string.Format("({0})", ErrorMessage) ?? ""}";

    public readonly string ErrorMessage => reason switch
    {
        ErrorReason.ParseException => $"Parse error: {exception}",
        ErrorReason.CannotBuild => "Failed build testing",
        ErrorReason.NotImplemented => "Unsupported",
        ErrorReason.NoData => "Contains no data",
        _ => "No error"
    };
}
