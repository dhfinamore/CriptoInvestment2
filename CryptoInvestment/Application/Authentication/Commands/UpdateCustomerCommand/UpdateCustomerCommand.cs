using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.UpdateCustomerCommand;

public record UpdateCustomerCommand(
    int CustomerId,
    string Name,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string? Phone,
    string? Country,
    string? State,
    string? City,
    string? BirthDate) : IRequest<ErrorOr<Success>>;