using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvOperations;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvOperationsQuery;

public class ListInvOperationsQueryHandler : IRequestHandler<ListInvOperationsQuery, ErrorOr<List<InvOperation>>>
{
    private readonly IInvOperationRepository _invOperationRepository;
    private readonly ICustomerRepository _customerRepository;

    public ListInvOperationsQueryHandler(IInvOperationRepository invOperationRepository, ICustomerRepository customerRepository)
    {
        _invOperationRepository = invOperationRepository;
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<InvOperation>>> Handle(ListInvOperationsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            Error.NotFound(description: "Customer not found");
        
        return await _invOperationRepository.GetInvOperationsAsync(request.CustomerId);
    }
}