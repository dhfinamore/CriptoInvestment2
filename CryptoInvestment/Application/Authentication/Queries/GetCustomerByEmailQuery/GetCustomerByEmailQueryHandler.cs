using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.GetCustomerByEmailQuery;

public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByEmailQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<Customer>> Handle(GetCustomerByEmailQuery query, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(query.Email);
        
        return customer == null 
            ? Error.NotFound(description: "Customer not found.")
            : customer;
    }
}