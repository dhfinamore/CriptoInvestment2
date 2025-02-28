using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.InvAssets.Commands;

public record CreateInvAssetsCommand(
    int CustomerId,
    int CurrencyId,
    int InvPlanId,
    decimal Amount,
    decimal ReinvestAmount,
    int EndType,
    decimal ReinvestPercent) : IRequest<ErrorOr<Success>>;