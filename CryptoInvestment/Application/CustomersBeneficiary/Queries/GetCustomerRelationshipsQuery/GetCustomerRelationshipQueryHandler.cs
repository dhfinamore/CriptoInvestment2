using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Queries.GetCustomerRelationshipsQuery;

public class GetCustomerRelationshipQueryHandler : IRequestHandler<GetCustomerRelationshipQuery, ErrorOr<List<CustomerRelationship>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerRelationshipQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<CustomerRelationship>>> Handle(GetCustomerRelationshipQuery request, CancellationToken cancellationToken)
    {
        var customerRelationships = await _customerRepository.GetCustomerRelationships();
        return customerRelationships.Count > 0 ? 
            customerRelationships : 
            Error.NotFound(description: "No security questions found");
    }
}