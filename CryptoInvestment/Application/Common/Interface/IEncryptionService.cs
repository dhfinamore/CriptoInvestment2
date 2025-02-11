namespace CryptoInvestment.Application.Common.Interface;

public interface IEncryptionService
{
    string EncryptEmail(string email);

    string DecryptEmail(string cipherText);

    string EncryptId(string id);

    string DecryptId(string cipherText);

    string EncryptDate(DateTime date);

    DateTime? DecryptDate(string cipherText);
}
