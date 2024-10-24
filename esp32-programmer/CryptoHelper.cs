using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoHelper
{
  public static string Encode(string text)
  {
    var plainTextBytes = Encoding.UTF8.GetBytes(text);
    return Convert.ToBase64String(plainTextBytes);
  }

  public static string Decode(string encodedText)
  {
    var base64EncodedBytes = Convert.FromBase64String(encodedText);
    return Encoding.UTF8.GetString(base64EncodedBytes);
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