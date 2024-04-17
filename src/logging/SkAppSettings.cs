using Microsoft.Extensions.Logging;

namespace logging;
public class SkAppSettings 
{
    public string? LoggingLevel {get;set;} = LogLevel.Trace.ToString();
    public bool UseAzureOpenAI {get;set;}
    public string? AzureOpenAIKey {get;set;}
    public string? AzureOpenAIDeploymentName {get;set;}
    public string? AzureOpenAIEndPoint {get;set;}
    public string? AzureOpenAIModelId {get;set;}
    public string? OpenAIKey {get;set;}
}