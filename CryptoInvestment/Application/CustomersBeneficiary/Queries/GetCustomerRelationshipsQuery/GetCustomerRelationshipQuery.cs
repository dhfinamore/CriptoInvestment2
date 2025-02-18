using CryptoInvestment.Domain.Customers;
using MediatR;
using ErrorOr;

namespace CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerRelationshipsQuery;

public record GetCustomerRelationshipQuery() : IRequest<ErrorOr<List<CustomerRelationship>>>;