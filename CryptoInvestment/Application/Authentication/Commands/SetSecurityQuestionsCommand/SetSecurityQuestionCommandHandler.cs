using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetSecurityQuestionsCommand;

public class SetSecurityQuestionCommandHandler : IRequestHandler<SetSecurityQuestionCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetSecurityQuestionCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Customer>> Handle(SetSecurityQuestionCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(command.CustomerId);

        if (customer is null)
            return Error.NotFound(description: "Customer not found");

        List<CustomerQuestion> customerQuestions = [];

        for (int i = 0; i < command.CustomerSecurityQuestions.Count; i++)
        {
            var customerQuestion = new CustomerQuestion()
            {
                IdCustomer = command.CustomerId,
                IdQuestion = command.CustomerSecurityQuestions[i].Item1,
                Response = command.CustomerSecurityQuestions[i].Item2,
                Order = i + 1
            };
            
            customerQuestions.Add(customerQuestion);
        }
        
        if (command.IsUpdate)
            await _customerRepository.UpdateSecurityQuestions(customer.IdCustomer, customerQuestions);
        else
        {
            await _customerRepository.AddSecurityQuestions(customerQuestions);
        }
        
        await _unitOfWork.CommitChangesAsync();

        return customer;
    }
}