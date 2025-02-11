using ErrorOr;

namespace CryptoInvestment.Application.Common.Interface;

public interface IEmailService
{
    public Task<ErrorOr<Success>> SendVerificationEmailAsync(string email, string subject, string body);
}