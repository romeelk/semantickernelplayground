using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;


IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

static (string? azoaEndpoint, string? azoaApiKey, string? aoaiModelId, string? azoaDeployedModel) GetConfig(IConfiguration config)
{
    var azoaEndpoint = config["azoaEndpoint"];
    var azoaApikey = config["azoaApiKey"];
    var azoaModelId = config["azoaModelId"];
    var azoaDeployedModel = config["azoaDeployedModel"];
    return (azoaEndpoint, azoaApikey, azoaModelId, azoaDeployedModel);
}
var (azoaEndpoint, azoaApikey, azoaModel, azoaDeployedModel) = GetConfig(configuration);


var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: azoaDeployedModel,
    endpoint: azoaEndpoint,
    apiKey: azoaApikey,
    modelId: azoaModel);

// Load some in built plugins
builder.Plugins.AddFromType<TimePlugin>();
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
builder.Plugins.AddFromType<FileIOPlugin>();

var kernel = builder.Build();

await GetDateTimeInfo(kernel);
await GetConversationTopics(kernel);

var pluginFileToRead = Path.Combine(Environment.CurrentDirectory, "File.txt");

await GetFileAndPassToPrompt(kernel, pluginFileToRead);
// Invoke Time Plugin
static async Task GetDateTimeInfo(Kernel kernel)
{
    Console.WriteLine("SK TimePlugin --> Tell me the current Day of week");
    var currentDay = await kernel.InvokeAsync(nameof(TimePlugin), "DayOfWeek");
    Console.WriteLine(currentDay);

    Console.WriteLine("SK TimePlugin --> Tell me the current time");
    var currentTime = await kernel.InvokeAsync(nameof(TimePlugin), "Time");
    Console.WriteLine(currentTime);
}
static async Task GetConversationTopics(Kernel kernel)
{
    string input = @"I am a graduate seeking to learn Python. Provide me a list of learning materials which will assist me";

    Console.WriteLine(input);
    var result = await kernel.InvokeAsync(
    "ConversationSummaryPlugin",
    "GetConversationTopics",
    new() { { "input", input } });

    Console.WriteLine(result);
}

static async Task GetFileAndPassToPrompt(Kernel kernel, string filename)
{
    Console.WriteLine("---> Using FilePlugin to read a prompt from a file");

    var fileContents = await kernel.InvokeAsync(nameof(FileIOPlugin), "Read", new() { { "path", filename } });

    Console.WriteLine($"File contents: {fileContents}");

    var result = await kernel.InvokePromptAsync(fileContents.ToString());
    Console.WriteLine($"Result; {result}");
}