using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.RegisterCommand;

public record RegisterCommand(
    string Email,
    string Name,
    string FirstFamilyName,
    string SecondFamilyName,
    string Phone,
    bool TermsAndConditions,
    bool AcceptPromotions) : IRequest<ErrorOr<Customer>>;
