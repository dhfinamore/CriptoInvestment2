using CryptoInvestment.Application.Common.Interface;
using System.Security.Cryptography;
using System.Text;

namespace CryptoInvestment.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService()
    {
        _key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes para AES-128
        _iv  = Encoding.UTF8.GetBytes("abcdefghijklmnop"); // 16 bytes IV
    }

    private string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV  = _iv;
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    private string Decrypt(string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV  = _iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    public string EncryptEmail(string email)
    {
        return Encrypt(email);
    }

    public string DecryptEmail(string cipherText)
    {
        return Decrypt(cipherText);
    }

    public string EncryptId(string id)
    {
        return Encrypt(id);
    }

    public string DecryptId(string cipherText)
    {
        return Decrypt(cipherText);
    }

    public string EncryptDate(DateTime date)
    {
        return Encrypt(date.ToString("o")); // "o" es el formato round-trip
    }

    public DateTime? DecryptDate(string cipherText)
    {
        try
        {
            string decrypted = Decrypt(cipherText);
            if (DateTime.TryParse(decrypted, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime result))
            {
                return result;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
