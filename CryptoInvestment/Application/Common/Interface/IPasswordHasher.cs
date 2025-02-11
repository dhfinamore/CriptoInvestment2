namespace CryptoInvestment.Application.Common.Interface;

public interface IPasswordHasher
{
    public bool VerifyHashedPassword(string hashedPassword, string password);
}