using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomerWithdrawalWallets.Queries;

public record ListCustomerWithdrawalWalletsQuery(int CustomerId) : IRequest<ErrorOr<List<CustomerWithdrawalWallet>>>;