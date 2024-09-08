using System.Text.Json;
using System.Threading.Channels;

namespace Hwoodiwiss.StaticWebAssetTester.Core;

public sealed class StaticWebAssetsTester(HttpClient httpClient, string manifestPath, Uri uri)
{
    private Thread? _workerThread;

    public IAsyncEnumerable<ResultDetails> TestApplication()
    {
        var manifest = ReadManifest(manifestPath);
        var channel = Channel.CreateUnbounded<ResultDetails>();

        _workerThread = new Thread(() => ThreadWorker(manifest, channel));
        _workerThread.Start();

        return channel.Reader.ReadAllAsync();
    }

    private async void ThreadWorker(StaticWebAssetsManifest manifest, Channel<ResultDetails> channel)
        => await CreateExceptionSafeTask(async () =>
        {
            await Parallel.ForEachAsync(manifest.Endpoints, async (endpoint, _) =>
            {
                await TestPathAsync(endpoint.Route, channel);
            });
            channel.Writer.Complete();
        },
        (ex) =>
        {
            Console.WriteLine("An error occured, closing the channel");
            Console.WriteLine($"Type - {ex.GetType().Name}");
            Console.WriteLine(ex.Message);
            channel.Writer.Complete();
        });

    private static async Task CreateExceptionSafeTask(Func<Task> asyncProc, Action<Exception> failureHandler)
    {
        try
        {
            await asyncProc();
        }
        catch(Exception ex)
        {
            failureHandler(ex);
        }
    }
    private async Task TestPathAsync(string path, ChannelWriter<ResultDetails> resultWriter)
    {
        var uriBuilder = new UriBuilder(uri)
        {
            Path = path
        };
        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = uriBuilder.Uri;

        using var response = await httpClient.SendAsync(request);

        await resultWriter.WriteAsync(new (path, response.IsSuccessStatusCode, (int)response.StatusCode));
    }

    private static StaticWebAssetsManifest ReadManifest(string manifestPath)
    {
        if (!File.Exists(manifestPath))
        {
            throw new FileNotFoundException($"Could not find manifest file {manifestPath}");
        }

        using var manifestStream = File.OpenRead(manifestPath);
        var result = JsonSerializer.Deserialize(manifestStream, StaticWebAssetsTesterJsonSerializerContext.Default.StaticWebAssetsManifest);
        return result ?? throw new FileLoadException($"{manifestPath} was not a valid Static Web Assets manifest file.");
    }
}
