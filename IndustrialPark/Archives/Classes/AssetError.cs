namespace IndustrialPark.Archives;

public struct AssetError
{
    public ErrorReason reason;
    public uint assetID;
    public string assetName;
    public string exception;

    public override string ToString() =>
        $"-{reason}: [{assetID:X8}] {assetName} {string.Format("({0})", ErrorMessage) ?? ""}";

    public readonly string ErrorMessage => reason switch
    {
        ErrorReason.ParseException => $"Cannot parse: {exception}",
        ErrorReason.CannotBuild => "Cannot build",
        ErrorReason.NotImplemented => "Unsupported by Industrial Park",
        ErrorReason.NoData => "Asset is empty and does not contain any data",
        _ => "No error"
    };
}
