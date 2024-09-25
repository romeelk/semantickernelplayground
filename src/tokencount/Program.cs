using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Kernel = Microsoft.SemanticKernel.Kernel;
using System.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Semantic Kernel token count example");

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

var env = Environment.GetEnvironmentVariables()
                     .Cast<DictionaryEntry>()
                     .Where(t => t.Key.ToString()
                     .StartsWith("azoa"));

if (env.Count() == 4)
{
    Console.WriteLine("Azure Open AI env vars set...");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("No Azure Open AI env variables set. Exiting demo");
    Environment.Exit(-1);
}
// Setup kernel
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName: azoaDeployedModel, endpoint: azoaEndpoint, apiKey: azoaApikey, modelId: azoaModel);
kernelBuilder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));

var kernel = kernelBuilder.Build();

string skPrompt = """
{{$input}}

Summarize the content above.
""";


var input = """
The Olympic torch has begun its mega journey to the Paris 2024 Olympic Games.

Greek actress Mary Mina, playing the role of a high priestess, lit the flame in a dramatic ceremony in ancient Olympia, Greece, where the first Olympic games were held in 776 BC. The flame was meant to be lit by the sun's rays - as is tradition - but cloudy skies meant a back-up flame was used.

She handed the torch to Greece's Olympic rowing champion Stefanos Ntouskos, who started the relay which will see it travel across Greece and over the Mediterranean on a ship.
""";

var result = await kernel.InvokePromptAsync(skPrompt, new() { ["input"] = input });

Console.WriteLine(result);

// Filter on usage metadata

var usage = result?.Metadata?.Where(t => t.Key.Equals("Usage")).FirstOrDefault().Value as CompletionsUsage;

Console.WriteLine($"The following prompt tokens were consumed in the last execution: {usage?.PromptTokens}");
Console.WriteLine($"The following Total tokens are available in last execution: {usage?.TotalTokens}");
Console.WriteLine($"The following completion tokens are consumed in last execution: {usage?.CompletionTokens}");