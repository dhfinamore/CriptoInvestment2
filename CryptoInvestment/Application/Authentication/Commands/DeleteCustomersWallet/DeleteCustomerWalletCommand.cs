using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.DeleteCustomersWallet;

public record DeleteCustomerWalletCommand(
    int CustomerId, 
    int WalletId) : IRequest<ErrorOr<Success>>;
