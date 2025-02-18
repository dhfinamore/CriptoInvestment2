using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersPic.GetCustomerPicQuery;

public class GetCustomerPicQueryHandler : IRequestHandler<GetCustomerPicQuery, ErrorOr<CustomerPic>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerPicQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerPic>> Handle(GetCustomerPicQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        var customerPic = await _customerRepository.GetCustomerPic(request.CustomerId);
        
        if (customerPic is null)
            return Error.NotFound(description: "Customer pic not found");
        
        return customerPic;
    }
}