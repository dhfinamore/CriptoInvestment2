using CryptoInvestment.Domain.Customers;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;

public record SetPasswordCommand(
    string Email,
    string Password) : IRequest<ErrorOr<Customer>>;