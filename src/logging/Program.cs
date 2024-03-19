using System;
using System.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

const string CompletionModel = "gpt-3.5-turbo";

Console.WriteLine("SK demo showing how to use logging");

Console.WriteLine("Using Open API ...");

Console.WriteLine("Fetching OpenAI Key ");


// Addg logging 
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();


string GetOpenAIApiKey() => configuration["OpenAIiKey"] ?? throw new ArgumentException("Could not fetch OpenAPI key!. Make sure it is set either as env var or in appsettings.json");

string GetAzureOpenAIKey() => configuration["AzureOpenAIKey"] ?? throw new ArgumentException("Could not fetch Azure OpenAPI key!. Make sure it is set either as env var or in appsettings.json");

var skAppSettings = configuration.GetSection("SkAppSettings").Get<SkAppSettings>();

try
{
    bool useAzureOpenAI = true;

    IKernelBuilder builder = Kernel.CreateBuilder();

    LogLevel loggingLevel;

    if(!Enum.TryParse(skAppSettings.LoggingLevel, out loggingLevel))
    {
        Console.WriteLine("Could not get log level from appSettings. Setting to Information");
        loggingLevel = LogLevel.Information;
    }
    Console.WriteLine($"Current Semantic Kernel logging level {loggingLevel}");   

    if(useAzureOpenAI) 
    {
        Console.WriteLine("Configuring Azure Open AI");
        builder.Services.AddLogging(c=> c.AddConsole().SetMinimumLevel(loggingLevel))
                    .AddOpenAIChatCompletion(CompletionModel,GetAzureOpenAIKey());

    }
    else
    {
        builder.Services.AddLogging(c=> c.AddConsole().SetMinimumLevel(loggingLevel))
                    .AddOpenAIChatCompletion(CompletionModel,GetOpenAIApiKey());
    }        
    
   
    var kernel =  builder.Build();
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