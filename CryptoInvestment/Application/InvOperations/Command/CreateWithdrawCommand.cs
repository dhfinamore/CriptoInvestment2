using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Command;

public record CreateWithdrawCommand(
    int BalanceId,
    decimal Amount,
    int WalletId) : IRequest<ErrorOr<Success>>;