using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

public class IDFC_RSA_Class
{
    public RSA LoadRSAFromPemFile(string privateKeyFilePath)
    {
        try
        {
            string privateKeyString = File.ReadAllText(privateKeyFilePath);
            var pemReader = new PemReader(new StringReader(privateKeyString));
            var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;

            if (keyPair != null)
            {
                var rsaParams = DotNetUtilities.ToRSAParameters(keyPair.Private as RsaPrivateCrtKeyParameters);
                var rsa = RSA.Create();
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
            else
            {
                throw new InvalidOperationException("Private key could not be loaded.");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error reading file:" + ex.Message);
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine("Invalid key format:" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:" + ex.Message);
        }

        return null;
    }
}