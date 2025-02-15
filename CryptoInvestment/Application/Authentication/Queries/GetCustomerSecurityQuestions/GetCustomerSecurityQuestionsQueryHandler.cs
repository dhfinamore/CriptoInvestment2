using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.GetCustomerSecurityQuestions;

public class GetCustomerSecurityQuestionsQueryHandler : IRequestHandler<GetCustomerSecurityQuestionsQuery, ErrorOr<List<CustomerQuestion>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerSecurityQuestionsQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<CustomerQuestion>>> Handle(GetCustomerSecurityQuestionsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        var customerQuestions = await _customerRepository.GetSecurityQuestions(customer.IdCustomer);
        return customerQuestions.Count > 0 ? 
            customerQuestions : 
            Error.NotFound(description: "No security questions found for this customer");
    }
}