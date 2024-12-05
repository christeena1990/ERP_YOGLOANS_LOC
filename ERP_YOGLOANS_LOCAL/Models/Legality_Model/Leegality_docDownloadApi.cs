using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
public class Leegality_docDownloadApi
{
    private readonly HttpClient _httpClient;
    private const string AuthToken = "5Q93w8u563MlB0vspe32uroSGyq3I5y2"; // UAT
    private static readonly string apiUrl = "https://sandbox.leegality.com/api/v3.1/document/fetchDocument";

    public Leegality_docDownloadApi()
    {
        _httpClient = new HttpClient();
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public async Task<byte[]> FetchDocumentAsync(string documentId, string documentDownloadType = "DOCUMENT")
    {
        using (var client = new HttpClient())
        {
            // Construct the full URL with query parameters
            var url = $"{apiUrl}?documentId={documentId}&documentDownloadType={documentDownloadType}";

            // Set the headers
            client.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

            try
            {
                // Make the GET request
                var response = await client.GetAsync(url);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the content as a byte array
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
        }
    }

}