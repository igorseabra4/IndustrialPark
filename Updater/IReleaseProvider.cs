namespace IndustrialPark.Updater;

public interface IReleaseProvider
{
    ReleaseInfo GetLatestRelease();

    void DownloadRelease(ReleaseInfo release, string destinationZipPath);
}
