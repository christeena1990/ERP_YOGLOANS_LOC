using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LeegalityApiClient
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://sandbox.leegality.com/api/v3.0/sign/request"; //UAT link
    private const string AuthToken = "5Q93w8u563MlB0vspe32uroSGyq3I5y2"; //UAT

    public LeegalityApiClient()
    {
        _httpClient = new HttpClient();
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public async Task<string> SendLeegalityRequestAsync(ERP_YOGLOANS_LOCAL.Models.Legality_Model.Leegality_Business_request request)
    {
        var jsonContent = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
        _httpClient.DefaultRequestHeaders.Add("Cookie", "_lee=Leegality");

        HttpResponseMessage response = null;

        try
        {
            response = await _httpClient.PostAsync(ApiUrl, content);
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


