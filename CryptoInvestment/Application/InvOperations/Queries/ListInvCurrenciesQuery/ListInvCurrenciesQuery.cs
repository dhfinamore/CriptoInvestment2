using CryptoInvestment.Domain.InvOperations;
using MediatR;
using ErrorOr;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvCurrenciesQuery;

public record ListInvCurrenciesQuery() : IRequest<ErrorOr<List<InvCurrency>>>;
