using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoHelper
{
  private static readonly byte[] KEY;
  // Your IV Initialization Vector
  private static readonly byte[] ivx = Encoding.UTF8.GetBytes("7Q0da9DvmT63Z4Sw");

  static CryptoHelper()
  {
    // Your Secret Key
    KEY = Encoding.UTF8.GetBytes("MSTCz4EES5gvqGKi");
  }

  public static string Encrypt(string message)
  {
    using (var aes = Aes.Create())
    {
      aes.Key = KEY;
      aes.IV = ivx;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
      byte[] srcBuff = Encoding.UTF8.GetBytes(message);
      byte[] dstBuff;

      using (var ms = new System.IO.MemoryStream())
      {
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
          cs.Write(srcBuff, 0, srcBuff.Length);
          cs.FlushFinalBlock();
          dstBuff = ms.ToArray();
        }
      }

      return Convert.ToBase64String(dstBuff);
    }
  }

  public static string Decrypt(string encrypted)
  {
    using (var aes = Aes.Create())
    {
      aes.Key = KEY;
      aes.IV = ivx;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
      byte[] raw = Convert.FromBase64String(encrypted);
      byte[] originalBytes;

      using (var ms = new System.IO.MemoryStream(raw))
      {
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        {
          using (var reader = new System.IO.StreamReader(cs, Encoding.UTF8))
          {
            return reader.ReadToEnd();
          }
        }
      }
    }
  }
  public static string ComputeHash(string data)
  {
    using (SHA256 sha256 = SHA256.Create())
    {
      byte[] dataBytes = Encoding.UTF8.GetBytes(data);
      byte[] hashBytes = sha256.ComputeHash(dataBytes);
      return Convert.ToBase64String(hashBytes);
    }
  }

}