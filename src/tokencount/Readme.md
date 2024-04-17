# Token example

This sample prints out the token count metadata. Understanding your token consumption 
is important when designing your prompts.

## Set Azure OpenAI settings

You can either set Azure OpenAI via the appSettings.json (For demo purposes this is checked in - you should gitignore these files)

```
{
    "aoaiApiKey": "81264c9f87fb442c9dca1c682e2eeeb3",
    "azoaEndpoint": "https://airk.openai.azure.com/",
    "aoaiModel": "gpt-35-turbo"
}
```

OR

Set them as env var as follows

```
export azoaApiKey=asadsdasb442casdsadasdas
export azoaModelId="gpt-35-turbo"
export azoaDeployedModel="gpt-35-turbo"
azoaEndpoint": "Endpoint url"
```

## Token counts

LLM's use Token counts to partiton the request and completion response. Tokens are calculated on each request
each request.

In this demo you see the following token count metadata:

```
The following prompt tokens were consumed in the last execution: 137
The following Total tokens are available in last execution: 208
The following completion tokens are consumed in last execution: 71
```

These values can be obtained from the "Usage" metadata:

```
var usage = result?.Metadata?.Where(t => t.Key.Equals("Usage")).FirstOrDefault().Value as CompletionsUsage;

Console.WriteLine($"The following prompt tokens were consumed in the last execution: {usage?.PromptTokens}");
Console.WriteLine($"The following Total tokens are available in last execution: {usage?.TotalTokens}");
Console.WriteLine($"The following completion tokens are consumed in last execution: {usage?.CompletionTokens}");
```