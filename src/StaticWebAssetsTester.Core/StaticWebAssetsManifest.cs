using System.Text.Json.Serialization;

namespace Hwoodiwiss.StaticWebAssetTester.Core;

public sealed record StaticWebAssetsManifest
{
    [JsonPropertyName("Version")]
    public int Version { get; set; }

    [JsonPropertyName("Endpoints")] 
    public StaticWebAssetsManifestEndpoint[] Endpoints { get; set; } = [];
}
