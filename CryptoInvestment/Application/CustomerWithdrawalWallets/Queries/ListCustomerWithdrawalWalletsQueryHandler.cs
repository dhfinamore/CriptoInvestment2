using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomerWithdrawalWallets.Queries;

public class ListCustomerWithdrawalWalletsQueryHandler : IRequestHandler<ListCustomerWithdrawalWalletsQuery, ErrorOr<List<CustomerWithdrawalWallet>>>
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomerWithdrawalWalletsQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<CustomerWithdrawalWallet>>> Handle(ListCustomerWithdrawalWalletsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

        if (customer is null)
            return Error.NotFound(description: "Customer not found");

        var wallets = await _customerRepository.GetCustomerWithdrawalWallets(request.CustomerId);
        return wallets.Count > 0 ? wallets : [];
    }
}
