using CryptoInvestment.Domain.SecurityQuestions;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.SecurityQuestions.Queries.ListSecurityQuestions;

public record ListSecurityQuestionsQuery() : IRequest<ErrorOr<List<SecurityQuestion>>>;