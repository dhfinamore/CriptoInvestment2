using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.RegisterCommand;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Customer>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _customerRepository.ExistEmailAsync(command.Email))
            return RegisterCommandErrors.EmailAlreadyRegistered;

        if (await _customerRepository.ExistPhoneAsync(command.Phone))
            return RegisterCommandErrors.PhoneAlreadyRegistered;

        var customer = new Customer()
        {
            Email = command.Email,
            Nombre = command.Name,
            ApellidoPaterno = command.FirstFamilyName,
            ApellidoMaterno = command.SecondFamilyName,
            Phone = command.Phone,
            AcceptPromoEmail = command.AcceptPromotions
        };

        await _customerRepository.CreateCustomerAsync(customer);
        await _unitOfWork.CommitChangesAsync();

        return customer;
    }
}
