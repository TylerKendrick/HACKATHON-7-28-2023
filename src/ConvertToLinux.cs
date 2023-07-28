using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.AI.OpenAI;
using Azure;

namespace src;
public static class ConvertToLinux
{
    private const string
        openaiUri = "https://your-azure-openai-resource.com",
        azureKeyCredential = "your-azure-openai-resource-api-key";


    [FunctionName("ConvertToLinux")]
    public static async Task<IActionResult> RunLinux(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        string responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        return new OkObjectResult(responseMessage);
    }

    [FunctionName("ConvertToWindows")]
    public static async Task<IActionResult> RunWindows(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        OpenAIClient client = new(
                new Uri(openaiUri),
                new AzureKeyCredential(azureKeyCredential));

        string prompt = $"Convert the following: {req.Body}";
        var prompts = new CompletionsOptions()
            {
                Prompts = 
                {
                    "Convert Windows Batch script into Linux Bash Scripts.",
                    "Exit Status: https://www.geeksforgeeks.org/exit-status-variable-in-linux/",
                    "Save all call exit status,  $?,  into a unique variable before If testing .",
                    "Save exit status into a variable after all sqlplus calls in script.",
                    "Use unique variable in all output processing.",
                    prompt
                }
            };
        Response<Completions> response = await client.GetCompletionsAsync(
            "gpt-35-turbo", // assumes a matching model deployment or model name
            prompts);

        var responseBody = string.Join(Environment.NewLine, response.Value.Choices);
        return new OkObjectResult(responseBody);
    }
}
