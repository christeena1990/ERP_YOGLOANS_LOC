using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class IDFC_singlePayment
{
    private readonly HttpClient _httpClient;
    public IDFC_singlePayment()
    {
        _httpClient = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public async Task<string> SendSinglePaymentRequest(string bearerToken, string requestBody, string tran_id)
    {
        // Define the request URL
        string requestUrl = "https://apiext.uat.idfcfirstbank.com/paymenttxns/v1/fundTransfer"; //UAT link
        //string requestUrl = "https://apiext.payments.idfcfirstbank.com/paymenttxns/v1/fundTransfer"; //Production link
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

        // Define the request headers
        request.Headers.Add("source", "YOG");
        request.Headers.Add("correlationId", tran_id);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        // Define the request content as ByteArrayContent with application/octet-stream
        byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
        request.Content = new ByteArrayContent(byteArray);
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        // Send the request and get the response
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        string reponsejson = JsonConvert.SerializeObject(response); //Json response
        // Read and return the response content
        return await response.Content.ReadAsStringAsync();
    }
}
