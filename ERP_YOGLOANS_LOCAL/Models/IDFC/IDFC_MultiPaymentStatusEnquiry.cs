using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
public class IDFC_MultiPaymentStatusEnquiry
{
    private readonly HttpClient _httpClient;
    public IDFC_MultiPaymentStatusEnquiry()
    {
        _httpClient = new HttpClient();
       // _httpClient.Timeout = TimeSpan.FromSeconds(30);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    public async Task<string> SendMultiPaymentEnquiry(string bearerToken, string requestBody, string tran_id)
    {
        // Define the request URL
         string requestUrl = "https://apiext.uat.idfcfirstbank.com/cmsbridge-payment/v1/multiPaymentStatusEnquiry ";
        //string requestUrl = "https://apiext.payments.idfcfirstbank.com/cmsbridge-payment/v1/multiPaymentStatusEnquiry ";
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        var id = DateTime.Now.AddMinutes(4).ToString("yyyy-MM-dd HH:mm:ss");// DateTime.Today.AddMinutes(4);
        // Define the request content as StringContent
        var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/octet-stream");

        // Define the request headers
        request.Headers.Add("source", "YOGLOAN");
        request.Headers.Add("correlationId", tran_id);
        request.Headers.Add("corp_id", "YOGLOAN");
        request.Headers.Add("Authorization", "Bearer " + bearerToken);

        // Set the request content
        request.Content = requestContent;

        // Send the request and get the response
        HttpResponseMessage response = await _httpClient.SendAsync(request);


        // Read and return the response content
        return await response.Content.ReadAsStringAsync();
    }

}