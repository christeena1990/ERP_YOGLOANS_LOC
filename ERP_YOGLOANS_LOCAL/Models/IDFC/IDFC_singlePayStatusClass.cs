using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
public class IDFC_singlePayStatusClass
{
    private readonly HttpClient _httpClient;
    public IDFC_singlePayStatusClass()
    {
        _httpClient = new HttpClient();
        // _httpClient.Timeout = TimeSpan.FromSeconds(30);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public async Task<string> SendPaymentTransactionStatusRequest(string bearerToken, string requestBody, string correlationId)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            string requestUrl = "https://apiext.uat.idfcfirstbank.com/paymentenqs/v1/paymentTransactionStatus"; //UAT link
           // string requestUrl = "https://apiext.payments.idfcfirstbank.com/paymentenqs/v1/paymentTransactionStatus"; //Production link
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Add("source", "YOG"); // Replace with actual source value
            request.Headers.Add("correlationId", correlationId);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(requestBody));
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}