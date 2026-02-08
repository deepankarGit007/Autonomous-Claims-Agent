using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SynapxAgent
{
    public class ClaimsAgent
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        // Updated to use the specific model from your list
        private const string ModelUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public ClaimsAgent(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<ExtractedFields> ExtractDataAsync(string rawPdfText)
        {
            // 1. Construct the Prompt
            // We give the AI the raw text and specific instructions to map it to our class structure.
            var prompt = $@"
You are an expert autonomous insurance agent. Your job is to extract data from the following FNOL (First Notice of Loss) document text.

INSTRUCTIONS:
1. Extract the data into the exact JSON structure defined below.
2. If a field is not found in the text, set it to null.
3. For 'estimatedDamage', extract only the numeric value (e.g., 5000). If not found, use null.
4. For 'claimType', infer if it is 'Injury', 'Property Damage', or 'Theft' based on the description.
5. Output ONLY the JSON object. Do not add markdown formatting like ```json.

JSON STRUCTURE:
{{
  ""policyDetails"": {{
    ""policyNumber"": ""string"",
    ""policyHolderName"": ""string"",
    ""effectiveDate"": ""string"",
    ""expirationDate"": ""string""
  }},
  ""incidentDetails"": {{
    ""date"": ""string"",
    ""time"": ""string"",
    ""location"": ""string"",
    ""description"": ""string""
  }},
  ""involvedParties"": {{
    ""claimantName"": ""string"",
    ""thirdPartyName"": ""string"",
    ""contactPhone"": ""string"",
    ""contactEmail"": ""string""
  }},
  ""assetDetails"": {{
    ""assetType"": ""string"",
    ""assetId"": ""string"",
    ""estimatedDamage"": 0
  }},
  ""mandatoryData"": {{
    ""claimType"": ""string"",
    ""hasAttachments"": false,
    ""hasInitialEstimate"": false
  }}
}}

DOCUMENT TEXT:
{rawPdfText}";

            // 2. Prepare the Request Payload for Gemini
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            // 3. Call the API
            try
            {
                var response = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}", jsonContent);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                // 4. Parse the Response
                // Gemini returns a complex JSON. We need to dig into candidates -> content -> parts -> text
                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var textResponse = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    // Clean up any Markdown formatting if the AI adds it despite instructions
                    textResponse = textResponse.Replace("```json", "").Replace("```", "").Trim();

                    // Deserialize the AI's answer into our C# Object
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ExtractedFields>(textResponse, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Extraction Failed: {ex.Message}");
                return new ExtractedFields(); // Return empty object on failure to prevent crash
            }
        }
    }
}