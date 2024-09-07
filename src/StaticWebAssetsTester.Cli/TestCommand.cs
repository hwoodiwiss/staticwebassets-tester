using System.ComponentModel;
using Hwoodiwiss.StaticWebAssetTester.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Hwoodiwiss.StaticWebAssetTester.Cli;

internal sealed class TestCommand : AsyncCommand<TestCommand.TestCommandSettings>
{
    public sealed class TestCommandSettings : CommandSettings
    {
        [Description("The path to the staticwebassets.endpoints.json to verify")]
        [CommandArgument(0, "[manifest-path]")]
        public required string ManifestPath { get; init; }

        [Description("The root URI of the web running application to test the manifest agains")]
        [CommandArgument(1, "[app-uri]")]
        public required string AppUri { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, TestCommandSettings settings)
    {
        if (!Uri.TryCreate(settings.AppUri, UriKind.Absolute, out var uri))
        {
            AnsiConsole.MarkupLine($"[red]Could not parse the provided Uri: {settings.AppUri}[/]");
            return 1;
        }

        using var httpClient = new HttpClient();
        var tester = new StaticWebAssetsTester(httpClient,
            settings.ManifestPath,
            uri);

        await foreach (var result in tester.TestApplication())
        {
            var resultSymbol = result.Success ? '✅' : '❌';
            var colour = result.Success ? "[green]" : "[red]";
            var resultText = $"{result.Path} - {resultSymbol} - {result.StatusCode}";
            AnsiConsole.MarkupLine($"{colour}{resultText}[/]");
        }

        return 0;
    }
}
