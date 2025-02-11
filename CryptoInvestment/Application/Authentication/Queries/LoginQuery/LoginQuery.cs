using CryptoInvestment.Domain.Customers;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.LoginQuery;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<Customer>>;