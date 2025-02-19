using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Referrals.Commands;

public record GetReferralsCommand(int CustomerId) : IRequest<ErrorOr<List<Customer>>>;