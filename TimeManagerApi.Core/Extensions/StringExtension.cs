using System.Security.Cryptography;
using System.Text;

namespace TimeManagerApi.Core.Extensions;

public static class StringExtension
{
    public static string GetHashSha256(this string text)
    {
        if (String.IsNullOrWhiteSpace(text)) throw new ArgumentNullException("Нет данных для шифрования");

        using(var sha256 = SHA256.Create())  
        {  
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();  
        }  
    }
}