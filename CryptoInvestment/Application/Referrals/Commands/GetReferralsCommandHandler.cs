using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Referrals.Commands;

public class GetReferralsCommandHandler : IRequestHandler<GetReferralsCommand, ErrorOr<List<Customer>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetReferralsCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<Customer>>> Handle(GetReferralsCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);

        if (customer is null)
            return Error.NotFound(description: "customer not found");
        
        return await _customerRepository.GetCustomerReferrals(request.CustomerId);
    }
}