using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
public class Leegality_docStatusApi
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://sandbox.leegality.com/api/v3.3/document/details"; //UAT link
    private const string AuthToken = "5Q93w8u563MlB0vspe32uroSGyq3I5y2"; //UAT
    public Leegality_docStatusApi()
    {
        _httpClient = new HttpClient();
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }
    public async Task<string> LeegalityDocStatusRequest(string documentId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
        _httpClient.DefaultRequestHeaders.Add("Cookie", "_lee=Leegality");

        HttpResponseMessage response = null;

        string requestUrl = string.Format("{0}?documentId={1}", ApiUrl, documentId);

        try
        {
            response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (HttpRequestException e)
        {
            // Handle exception
            string msg = e.ToString();
            return null;
        }
    }
}