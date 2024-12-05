using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;


// This class is for IDFC single payment.
public class IDFC_directCustomer
{
    public async Task<string> ProcessPayment(string jsonRequest, string updateId)
    {
        //reponsejson = JsonConvert.SerializeObject(initiateAPIReq); //Json request
        AESDynamicIVEncrypt aES = new AESDynamicIVEncrypt();
        string encData = aES.DynamicIVEncrypt(jsonRequest); // Encrypt the Json request

        ////create tran_id
        var ss = updateId.Trim();



        //string client_id = "b2373565-22cf-47f0-b416-e69851030f26"; //ClientId for Production
       
        string client_id = "9e7df2d3-79a2-46bf-b4b2-1b5129b9596c"; //ClientId for UAT

        string audience = "https://app.uat-opt.idfcfirstbank.com/platform/oauth/oauth2/token"; //UAT Link

        //string audience = "https://app.my.idfcfirstbank.com/platform/oauth/oauth2/token";//Production Link

        string privateKeyFilePath = HostingEnvironment.MapPath("~") + "Cerificate\\wildcard_yogloans_com.pem";
        IDFC_RSA_Class iDFC_RSA_Class = new IDFC_RSA_Class();
        RSA rsa = iDFC_RSA_Class.LoadRSAFromPemFile(privateKeyFilePath);


        JWTTokenGenerator jWTTokenGenerator = new JWTTokenGenerator();
        string jwtToken = jWTTokenGenerator.GenerateJwtToken(client_id, rsa, audience);

        //Steps for generating Bearer Token using the JWT token
        BearerTokenRetriever bearerTokenRetriever = new BearerTokenRetriever();
        // string bearerToken = await bearerTokenRetriever.GetBearerToken(jwtToken);
        string scope = "paymenttxn-v1fundTransfer"; // UAT Scope        
        string bearerToken = await bearerTokenRetriever.GetBearerToken(jwtToken, scope);

        // Steps for sending request to SinglePayment API
        IDFC_singlePayment iDFC_SinglePayment = new IDFC_singlePayment();
        string responseEncrypted = await iDFC_SinglePayment.SendSinglePaymentRequest(bearerToken, encData, ss);

        //Code for decrypt the encrypted response
        AESDynamicIVDecrypt aESDynamicIVDecrypt = new AESDynamicIVDecrypt();
        string api_response = aESDynamicIVDecrypt.DynamicIVDecrypt(responseEncrypted);

        return api_response;
    }
}