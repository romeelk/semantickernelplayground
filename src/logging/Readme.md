# Logging example

This sample demonstrates the use of Logging. Logging verbosity is important to understand
your interaction with the underlying LLM.

## Set Azure OpenAI settings

You can either set Azure OpenAI via the appSettings.json (For demo purposes this is checked in - you should gitignore these files).

```
{
    "SkAppSettings" : {
        "LoggingLevel": "Information",
        "UseAzureOpenAI": true,
        "AzureOpenAIKey": "YourAzureOpenAIKey",
        "AzureOpenAIDeploymentName": "gpt-35-turbo",
        "AzureOpenAIEndPoint": "Your Azure endpoint",
        "AzureOpenAIModelId": "Your selected modelID",
        "OpenAIKey": "OpenAIKey"
    }
}
```
## Setup logging

Logging can be setup by adding the logging component to the IServiceCollection as shown below:

```
    builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));

```

This setups console logging level to Information.

For more detailed LLM logs set your logging level to Trace.