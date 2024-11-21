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

static (string? azoaEndpoint, string? azoaApiKey, string? aoaiModelId, string ? azoaDeployedModel) GetConfig(IConfiguration config)
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
    endpoint:azoaEndpoint,
    apiKey:azoaApikey,
    modelId:azoaModel);

builder.Plugins.AddFromType<TimePlugin>();
builder.Plugins.AddFromType<ConversationSummaryPlugin>();

var kernel = builder.Build();

try {
    await PromptWithHistory(kernel);
    await PromptTravelPersona(kernel);
    await PromptSystemMessage(kernel);
}
catch(Exception exception)
{
    System.Console.WriteLine($"Oops an error occured {exception.ToString()}");
}

static async Task PromptWithHistory(Kernel kernel)
{
    string language = "Python";
    string history = @"I am keen programming student wanting to know more about python";
    // Assign a persona to the prompt
    string prompt = @$"
    You are AI Python programming assistant. You are helpful, creative, 
    and very friendly. Consider the prograammers's background:
    ${history}

    Create a list of helpful phrases and words in 
    ${language} a programmer would find useful.

    Group phrases by category. Include common Python 
    statementss. Display the statements in the following format: 
    Python:

    Begin with: 'Here are some statments in ${language} 
    you may find helpful:' 
    and end with: 'I hope this helps you Python Learning'";
    
    Console.WriteLine($"Prompt history: {history}");
    Console.WriteLine($"Prompt:{prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompt response: {result}");
}

static async Task PromptTravelPersona(Kernel kernel)
{
    string input = @"I'm planning an anniversary trip with my 
    spouse. We like hiking, mountains, and beaches. Our 
    travel budget is $15000";

    string prompt = @$"
    The following is a conversation with an AI travel 
    assistant. The assistant is helpful, creative, and 
    very friendly.

    <message role=""user"">Can you give me some travel 
    destination suggestions?</message>

    <message role=""assistant"">Of course! Do you have a 
    budget or any specific activities in mind?</message>

    <message role=""user"">${input}</message>";

    Console.WriteLine($"Prompt :{prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompt response with Travel Persona Role: {result}");
}

static async Task PromptSystemMessage(Kernel kernel)
{
    var input = @$"I want to learn the Javascript language. Please summarise 
    for me a learning path";

    string prompt = @$"
    <message role=""system"">The following is a conversation with an AI Programming 
    assistant. The assistant is helpful
    very friendly.</message>

    <message role=""user"">${input}</message>";

    Console.WriteLine($"Prompt {prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompot response: {result}");
}