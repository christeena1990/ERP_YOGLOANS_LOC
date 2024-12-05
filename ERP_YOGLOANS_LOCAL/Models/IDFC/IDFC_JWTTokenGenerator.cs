using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

public class JWTTokenGenerator
{
    public string GenerateJwtToken(string clientId, RSA privateKey, string audience, int expirationMinutes = 60)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var rsaSecurityKey = new RsaSecurityKey(privateKey);

        var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);
        //var expirationTime1 = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + expirationMinutes * 60).ToString();
        var expirationTime1 = ((long)(expirationTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();


        var claims = new List<Claim>
        {
            new Claim("jti", Guid.NewGuid().ToString()),
            new Claim("sub", clientId),
            new Claim("iss", clientId),
            new Claim("aud", audience),
            new Claim("exp", expirationTime1)
        };

        var header = new JwtHeader(new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationTime,
            SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256),
        };

        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
       
        token.Header.Add("kid", "b8ae206a-d30d-4175-ba77-a2ba8f50f7d8"); // Kid for UAT

        //token.Header.Add("kid", "705ea2eb-f8b4-496d-8e64-c9560093bf00"); // Kid for Production
        
        if (token.Payload.ContainsKey("iss"))
        {
            token.Payload.Remove("iss");
        }
        token.Payload.Add("iss", clientId);

        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

}

