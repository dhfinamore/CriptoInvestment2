using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.GetCustomerByEmailQuery;

public record GetCustomerByEmailQuery(string Email) : IRequest<ErrorOr<Customer>>;