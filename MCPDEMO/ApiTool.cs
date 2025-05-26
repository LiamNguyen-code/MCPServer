using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFirstMCP;

[McpServerToolType]
public static class ApiTool
{
    private static readonly HttpClient httpClient = new HttpClient();

    [McpServerTool, Description("Call a public API and return the JSON result.")]
    public static async Task<string> CallPublicApiAsync(string url)
    {
        try
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return $"Error: {(int)response.StatusCode} - {response.ReasonPhrase}";

            string responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (HttpRequestException ex)
        {
            return $"Error calling API: {ex.Message}";
        }
    }

    [McpServerTool, Description("Send a POST request to the API.")]
    public static async Task<string> PostToApiAsync(string url, string jsonBody)
    {
        try
        {
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? $"Response: {responseBody}"
                : $"Error: {(int)response.StatusCode} - {responseBody}";
        }
        catch (Exception ex)
        {
            return $"Error sending POST request: {ex.Message}";
        }
    }

    [McpServerTool, Description("Call an API with an access token and return the result.")]
    public static async Task<string> CallApiWithTokenAsync(string url, string accessToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? JsonSerializer.Serialize(JsonDocument.Parse(responseBody), new JsonSerializerOptions { WriteIndented = true })
                : $"Error: {(int)response.StatusCode} - {responseBody}";
        }
        catch (HttpRequestException ex)
        {
            return $"Error calling API: {ex.Message}";
        }
    }

    [McpServerTool, Description("Sends a POST request with an access token and JSON body.")]
    public static async Task<string> PostApiWithTokenAsync(string url, string accessToken, string jsonBody)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? $"✅ Success:\n{responseContent}"
                : $"❌ Error {(int)response.StatusCode} - {response.ReasonPhrase}\n{responseContent}";
        }
        catch (HttpRequestException ex)
        {
            return $"❗ Error sending POST request: {ex.Message}";
        }
    }
}
