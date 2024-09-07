using System.Text.Json.Serialization;

namespace Hwoodiwiss.StaticWebAssetTester.Core;

public sealed record StaticWebAssetsManifestEndpoint
{
    [JsonPropertyName("Route")] 
    public string Route { get; set; } = string.Empty;
}
