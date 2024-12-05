
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class BearerTokenRetriever
{
    private readonly HttpClient _httpClient;

    public BearerTokenRetriever()
    {
        try
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
        catch(Exception ex)
        {
            string message = ex.ToString();
        }
        
    }

    public async Task<string> GetBearerToken(string jwtToken, string scope= "cmsapibridge-payments-MultiPayment cmsapibridge-payments-MultiPaymentStatusEnquiry")
    {
        var tokenEndpoint = "https://apiext.uat.idfcfirstbank.com/authorization/oauth2/token"; // UAT Link
       //var tokenEndpoint = "https://apiext.idfcfirstbank.com/authorization/oauth2/token"; // PRODUCTION Link
    

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", scope),            
            new KeyValuePair<string, string>("client_id", "9e7df2d3-79a2-46bf-b4b2-1b5129b9596c"), // UAT client id
            //new KeyValuePair<string, string>("client_id", "b2373565-22cf-47f0-b416-e69851030f26"),  //Production client id            
            new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new KeyValuePair<string, string>("client_assertion", jwtToken)
        });

        // Set the Content-Type header on the content object
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        try
        {           

            // Send the request using PostAsync
            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Extract and return the access token from the response
                // Note: In a real-world scenario, you should handle the response according to the OAuth 2.0 specifications
                var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(responseContent);
                return tokenResponse.access_token;
            }
            else
            {
                // Handle error cases (e.g., log or throw an exception)
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("HTTP request failed with status code"+ response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException("HTTP request failed.", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException("JSON serialization/deserialization error.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

}
