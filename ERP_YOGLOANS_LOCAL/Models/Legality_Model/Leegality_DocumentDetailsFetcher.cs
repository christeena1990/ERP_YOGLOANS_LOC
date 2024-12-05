using System;
using System.Net.Http;
using System.Threading.Tasks;

public class Leegality_DocumentDetailsFetcher
{
    // Define constants for the URL and token
    private const string AuthToken = "5Q93w8u563MlB0vspe32uroSGyq3I5y2";
    private const string ApiUrl = "https://sandbox.leegality.com/api/v3.2/document/details";

    public async Task<string> GetDocumentDetailsAsync(string documentId)
    {
        using (var client = new HttpClient())
        {
            // Construct the URL with the document ID
            var url = $"{ApiUrl}?documentId={documentId}";

            // Set headers
            client.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
            client.DefaultRequestHeaders.Add("Cookie", "_lee=Leegality; _lee=Leegality");

            try
            {
                // Make the GET request
                var response = await client.GetAsync(url);

                // Check if the response is successful
                response.EnsureSuccessStatusCode();

                // Return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
        }
    }
}

