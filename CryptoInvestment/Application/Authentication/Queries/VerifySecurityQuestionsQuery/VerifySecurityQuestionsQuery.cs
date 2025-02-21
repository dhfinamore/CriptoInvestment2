using MediatR;
using ErrorOr;

namespace CryptoInvestment.Application.Authentication.Queries.VerifySecurityQuestionsQuery;

public record VerifySecurityQuestionsQuery(
    string Email,
    string FirstAnswer,
    string SecondAnswer,
    string ThirdAnswer) : IRequest<ErrorOr<bool>>;