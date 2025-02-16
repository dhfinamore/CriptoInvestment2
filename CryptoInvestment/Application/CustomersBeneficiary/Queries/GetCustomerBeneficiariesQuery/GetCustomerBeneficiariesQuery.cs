using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;

public record GetCustomerBeneficiariesQuery(int CustomerId) : IRequest<ErrorOr<List<CustomerBeneficiary>>>;