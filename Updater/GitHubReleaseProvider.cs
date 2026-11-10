using Octokit;
using System.Net;

namespace IndustrialPark.Updater;

internal sealed class GitHubReleaseProvider(
    string owner = "igorseabra4",
    string repo = "IndustrialPark"
) : IReleaseProvider
{
    public ReleaseInfo GetLatestRelease()
    {
        var client = new GitHubClient(new ProductHeaderValue("IP"));
        Release latest = client.Repository.Release.GetLatest(owner, repo).GetAwaiter().GetResult();

        return new ReleaseInfo
        {
            TagName = latest.TagName,
            Name = latest.Name,
            Body = latest.Body,
            DownloadUrl = latest.Assets[0].BrowserDownloadUrl
        };
    }

    public void DownloadRelease(ReleaseInfo release, string destinationZipPath)
    {
        using var webClient = new WebClient();
        webClient.Headers.Add(HttpRequestHeader.UserAgent, "Anything");
        webClient.DownloadFile(release.DownloadUrl, destinationZipPath);
    }
}
