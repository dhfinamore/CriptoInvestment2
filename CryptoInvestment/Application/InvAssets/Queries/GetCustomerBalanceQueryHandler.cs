using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvAssets;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvAssets.Queries;

public class GetCustomerBalanceQueryHandler : IRequestHandler<GetCustomerBalanceQuery, ErrorOr<List<InvBalance>>>
{
    private readonly IInvAssetsRepository _invAssetsRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerBalanceQueryHandler(IInvAssetsRepository invAssetsRepository, ICustomerRepository customerRepository)
    {
        _invAssetsRepository = invAssetsRepository;
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<InvBalance>>> Handle(GetCustomerBalanceQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

        if (customer is null)
        {
            return Error.NotFound(description: "Customer not found");
        }
        var balances = await _invAssetsRepository.GetInvBalances(request.CustomerId);
        
        return balances.Count > 0 
            ? balances 
            : [];
    }
}