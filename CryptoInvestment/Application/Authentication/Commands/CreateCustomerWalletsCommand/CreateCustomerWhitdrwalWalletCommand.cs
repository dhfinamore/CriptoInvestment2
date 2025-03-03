using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.CreateCustomerWalletsCommand;

public record CreateCustomerWalletCommand(
    int CustomerId,
    string WalletName,
    int InvCurrency,
    string Account,
    bool Used,
    int? WalletId = null) : IRequest<ErrorOr<Success>>;