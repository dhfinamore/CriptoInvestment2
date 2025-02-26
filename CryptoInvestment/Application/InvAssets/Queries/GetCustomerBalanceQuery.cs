using CryptoInvestment.Domain.InvAssets;
using MediatR;
using ErrorOr;

namespace CryptoInvestment.Application.InvAssets.Queries;

public record GetCustomerBalanceQuery(int CustomerId) : IRequest<ErrorOr<List<InvBalance>>>;