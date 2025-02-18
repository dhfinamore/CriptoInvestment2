using CryptoInvestment.Domain.Customers;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.CustomersPic.GetCustomerPicQuery;

public record GetCustomerPicQuery(int CustomerId) : IRequest<ErrorOr<CustomerPic>>;