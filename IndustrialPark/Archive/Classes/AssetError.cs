namespace IndustrialPark.Archive;

public struct AssetError
{
    public ErrorReason reason;
    public uint assetID;
    public string assetName;
    public string errorMessage;

    public override string ToString()
    {
        return $"-{reason}: [{assetID:X8}] {assetName} {string.Format("({0})", errorMessage) ?? ""}";
    }
}
