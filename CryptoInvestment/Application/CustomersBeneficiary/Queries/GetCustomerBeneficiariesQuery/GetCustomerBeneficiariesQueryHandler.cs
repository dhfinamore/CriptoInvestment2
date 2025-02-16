using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerBeneficiariesQuery;

public class GetCustomerBeneficiariesQueryHandler : IRequestHandler<GetCustomerBeneficiariesQuery, ErrorOr<List<CustomerBeneficiary>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerBeneficiaryRepository _beneficiaryRepository;

    public GetCustomerBeneficiariesQueryHandler(ICustomerRepository customerRepository, ICustomerBeneficiaryRepository beneficiaryRepository)
    {
        _customerRepository = customerRepository;
        _beneficiaryRepository = beneficiaryRepository;
    }

    public async Task<ErrorOr<List<CustomerBeneficiary>>> Handle(GetCustomerBeneficiariesQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");

        return await _beneficiaryRepository.GetBeneficiaryByCustomerIdAsync(request.CustomerId);
    }
}