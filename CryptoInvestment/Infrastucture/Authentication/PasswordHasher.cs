using CryptoInvestment.Application.Common.Interface;

namespace CryptoInvestment.Infrastucture.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}