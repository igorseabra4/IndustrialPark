namespace IndustrialPark.Updater;

public sealed class ReleaseInfo
{
    public string TagName { get; init; }
    public string Name { get; init; }
    public string Body { get; init; }
    public string DownloadUrl { get; init; }
}
