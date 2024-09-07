using Hwoodiwiss.StaticWebAssetTester.Cli;
using Spectre.Console.Cli;

var app = new CommandApp<TestCommand>();
return await app.RunAsync(args);
