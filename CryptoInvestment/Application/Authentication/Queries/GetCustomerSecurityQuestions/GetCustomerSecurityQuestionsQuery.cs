using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;

public record GetCustomerSecurityQuestionsQuery(
    int CustomerId) : IRequest<ErrorOr<List<CustomerQuestion>>>;