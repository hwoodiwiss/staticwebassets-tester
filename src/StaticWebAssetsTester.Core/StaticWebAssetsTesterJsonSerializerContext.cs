using System.Text.Json.Serialization;

namespace Hwoodiwiss.StaticWebAssetTester.Core;

[JsonSerializable(typeof(StaticWebAssetsManifest))]
public partial class StaticWebAssetsTesterJsonSerializerContext : JsonSerializerContext;
