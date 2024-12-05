using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESDynamicIVEncrypt
{
    //public string Main(string jsonString)
    //{
    //    // Key for Encryption
    //    string hexaKey = "77616d706c65496467134145536b659123616d706c65496468634145536b8022";
    //    //string data = "{\"jti\":\"1623863001\",\"sub\":\"9e7df2d3-79a2-46bf-b4b2-1b5129b9596c\",\"iss\":\"9e7df2d3-79a2-46bf-b4b2-1b5129b9596c\",\"aud\":\"https://app.uat-opt.idfcfirstbank.com/platform/oauth/oauth2/token\",\"exp\":8054080528}";
    //    string data = jsonString;
    //    return DynamicIVEncrypt(data, hexaKey);
    //    // Sample Output: XV5jc0t7L3pwN2hLfWM8OUBdhjmD2qNHGOoKq0/EJPc=
    //}

    /*
    * Method to generate random 16 byte IV
    */
    public string GenerateIV()
    {
        // Starting Character limit
        int asciiCharStart = 47;
        // Ending Character limit
        int asciiCharEnd = 126;
        int charCount = 16;

        Random random = new Random();
        StringBuilder builder = new StringBuilder();
        // Iterate, get random int between the ASCII limits and then convert to char, finally append to builder
        for (int i = 0; i < charCount; i++)
            builder.Append((char)(random.Next(asciiCharStart, asciiCharEnd)));

        return builder.ToString();
    }

    // Method to convert Hexadecimal key into bytes
    public byte[] HexStringToHexBytes(string inputHex)
    {
        byte[] resultantArray = new byte[inputHex.Length / 2];
        for (int i = 0; i < resultantArray.Length; i++)
        {
            resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
        }
        return resultantArray;
    }

    // Actual logic for Encryption
    public string EncryptDataWithAes(byte[] cipher, byte[] key, byte[] iv)
    {
        using (Aes aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = key;
            aesAlgorithm.IV = iv;
            aesAlgorithm.Mode = CipherMode.CBC;
            // PKCS7 padding is compatible with PKCS5
            aesAlgorithm.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aesAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /*
    * Method to encrypt the data using dynamic IV encryption
    * Appending the IV to the front of the payload
    */
    public string DynamicIVEncrypt(string dataToEncrypt)
    {
        string secretKey = "77616d706c65496467134145536b659123616d706c65496468634145536b8022"; //secretKey for UAT
        //string secretKey = "77616d706c65496467134145536b659123616d706c65496468634145536b6629";//secretKey for Production
        
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
        // Getting the key bytes from Hexadecimal Key
        byte[] keyBytes = HexStringToHexBytes(secretKey);

        // Generate the IV
        string iv = GenerateIV();
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

        // Performing the encryption
        string encryptedString = EncryptDataWithAes(dataBytes, keyBytes, ivBytes);
        // Combine IV and encrypted data
        byte[] combinedBytes = new byte[ivBytes.Length + Convert.FromBase64String(encryptedString).Length];
        Buffer.BlockCopy(ivBytes, 0, combinedBytes, 0, ivBytes.Length);
        Buffer.BlockCopy(Convert.FromBase64String(encryptedString), 0, combinedBytes, ivBytes.Length, Convert.FromBase64String(encryptedString).Length);
        return Convert.ToBase64String(combinedBytes);
    }
}
