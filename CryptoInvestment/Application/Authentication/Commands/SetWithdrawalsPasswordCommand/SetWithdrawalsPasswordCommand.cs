using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetWithdrawalsPasswordCommand;

public record SetWithdrawalsPasswordCommand(
    int CustomerId,
    string WithdrawalsPassword) : IRequest<ErrorOr<Success>>;