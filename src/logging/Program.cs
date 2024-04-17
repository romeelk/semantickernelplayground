using logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

const string CompletionModel = "gpt-3.5-turbo";

Console.WriteLine("SK demo showing how to use logging");

Console.WriteLine("Using Open API ...");

Console.WriteLine("Fetching OpenAI Key ");

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

var skAppSettings = configuration.GetSection("SkAppSettings").Get<SkAppSettings>();

try
{
    ValidateAPIKeys(skAppSettings);

    IKernelBuilder builder = Kernel.CreateBuilder();

    LogLevel loggingLevel;

    if (!Enum.TryParse(skAppSettings.LoggingLevel, out loggingLevel))
    {
        Console.WriteLine("Could not get log level from appSettings.json. Setting to Information");
        loggingLevel = LogLevel.Information;
    }
    Console.WriteLine($"Current Semantic Kernel logging level {loggingLevel}");
    ConfigureChatCompletion(CompletionModel, skAppSettings, builder, loggingLevel);

    var kernel = builder.Build();
    var questionFunction = kernel.CreateFunctionFromPrompt(
    "When was {{$language}} {{$version}} released?"
    );
    var result = await questionFunction.InvokeAsync(kernel, new KernelArguments
    {
        ["language"] = "C#",
        ["version"] = "5.0",
    });
    Console.WriteLine("Answer: {0}", result.GetValue<string>());
}
catch (Exception exception)
{
    Console.WriteLine($"Oops an error was encountered: {exception.ToString()}");
}

static void ValidateAPIKeys(SkAppSettings? skAppSettings)
{
    if (skAppSettings.UseAzureOpenAI && string.IsNullOrEmpty(skAppSettings.AzureOpenAIKey))
    {
        throw new InvalidOperationException("The AzureOpenAIKey is not set in the appsettings.json file.");
    }

    if (!skAppSettings.UseAzureOpenAI && string.IsNullOrEmpty(skAppSettings.OpenAIKey))
    {
        throw new InvalidOperationException("The OpenAIKey is not set in the appsettings.json file.");
    }
}

static void ConfigureChatCompletion(string CompletionModel, SkAppSettings? skAppSettings, IKernelBuilder builder, LogLevel loggingLevel)
{
    if (skAppSettings.UseAzureOpenAI)
    {
        Console.WriteLine("Configuring Azure Open AI Chat completion");
        builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(loggingLevel))
                    .AddAzureOpenAIChatCompletion(deploymentName:skAppSettings.AzureOpenAIDeploymentName,endpoint:skAppSettings.AzureOpenAIEndPoint,apiKey:skAppSettings.AzureOpenAIKey,modelId:skAppSettings.AzureOpenAIModelId);
    }
    else
    {
        Console.WriteLine("Configuring Open AI Chat completion");
        builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(loggingLevel))
                    .AddOpenAIChatCompletion(CompletionModel, skAppSettings.OpenAIKey);
    }
}