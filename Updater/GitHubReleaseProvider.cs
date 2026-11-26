using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace IndustrialPark.Updater;

internal sealed class GitHubReleaseProvider(
    string owner = "igorseabra4",
    string repo = "IndustrialPark",
    Uri apiUrl = null
) : IReleaseProvider
{
    public ReleaseInfo GetLatestRelease()
    {
        var client = apiUrl == null
            ? new GitHubClient(new ProductHeaderValue("IP"))
            : new GitHubClient(new ProductHeaderValue("IP"), apiUrl);
        Release latest = client.Repository.Release.GetLatest(owner, repo).GetAwaiter().GetResult();
        string expectedName = $"IndustrialPark_{latest.TagName}.zip";
        ReleaseAsset asset = latest.Assets.SingleOrDefault(a => string.Equals(a.Name, expectedName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidDataException($"Release {latest.TagName} is missing the expected asset {expectedName}.");

        return new ReleaseInfo
        {
            TagName = latest.TagName,
            Name = latest.Name,
            Body = latest.Body,
            DownloadUrl = asset.BrowserDownloadUrl
        };
    }

    public void DownloadRelease(ReleaseInfo release, string destinationZipPath)
    {
        using var webClient = new HttpClient();
        webClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
        var data = webClient.GetByteArrayAsync(release.DownloadUrl).GetAwaiter().GetResult();
        File.WriteAllBytes(destinationZipPath, data);
    }
}
