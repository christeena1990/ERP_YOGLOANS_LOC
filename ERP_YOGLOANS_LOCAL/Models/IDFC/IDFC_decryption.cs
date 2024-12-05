using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class AESDynamicIVDecrypt
{
    public string DynamicIVDecrypt(string encryptedData)
    {
        string hexaKey = "77616d706c65496467134145536b659123616d706c65496468634145536b8022"; //secret key for UAT
        //string hexaKey = "77616d706c65496467134145536b659123616d706c65496468634145536b6629"; //secret Key for Production
        byte[] combinedBytes = Convert.FromBase64String(encryptedData);
        byte[] ivBytes = new byte[16];
        Buffer.BlockCopy(combinedBytes, 0, ivBytes, 0, 16);
        int lengthOfCipherText = combinedBytes.Length - 16;
        byte[] cipherTextBytes = new byte[lengthOfCipherText];
        Buffer.BlockCopy(combinedBytes, 16, cipherTextBytes, 0, lengthOfCipherText);
        byte[] keyBytes = HexStringToByteArray(hexaKey);
        string decryptedString = DecryptDataWithAes(cipherTextBytes, keyBytes, ivBytes);
        return decryptedString;
    }

    public string DecryptDataWithAes(byte[] cipher, byte[] key, byte[] iv)
    {
        using (Aes aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = key;
            aesAlgorithm.IV = iv;
            aesAlgorithm.Mode = CipherMode.CBC;
            aesAlgorithm.Padding = PaddingMode.PKCS7;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aesAlgorithm.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }
                return Encoding.ASCII.GetString(ms.ToArray());
            }
        }
    }

    public byte[] HexStringToByteArray(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}
