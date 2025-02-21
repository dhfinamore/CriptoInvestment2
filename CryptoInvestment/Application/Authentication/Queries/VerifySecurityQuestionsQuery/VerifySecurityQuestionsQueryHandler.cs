using CryptoInvestment.Application.Common.Interface;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.VerifySecurityQuestionsQuery;

public class VerifySecurityQuestionsQueryHandler : IRequestHandler<VerifySecurityQuestionsQuery, ErrorOr<bool>>
{
    private readonly ICustomerRepository _customerRepository;

    public VerifySecurityQuestionsQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<bool>> Handle(VerifySecurityQuestionsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(request.Email);

        if (customer is null)
            return Error.NotFound(description: "Customer not Found");

        var customerQuestions = await _customerRepository.GetSecurityQuestions(customer.IdCustomer);

        if (customerQuestions.Count <= 0)
            return Error.NotFound(description: "Customer Questions not Found");

        var verifyQuestion1 = BCrypt.Net.BCrypt.Verify(request.FirstAnswer, customerQuestions[0].Response);
        var verifyQuestion2 = BCrypt.Net.BCrypt.Verify(request.SecondAnswer, customerQuestions[1].Response);
        var verifyQuestion3 = BCrypt.Net.BCrypt.Verify(request.ThirdAnswer, customerQuestions[2].Response);

        if (!verifyQuestion1 || !verifyQuestion2 || !verifyQuestion3)
            return Error.Unauthorized(description: "Invalid Security Questions");

        return true;
    }
}