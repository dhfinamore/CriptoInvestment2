using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetPasswordCommand;

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPasswordCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Customer>> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(command.Email);
        
        if (customer is null)
            return Error.NotFound(description: "No existe un usuario con ese email.");
        
        if (customer.PasswdLogin is not null && !BCrypt.Net.BCrypt.Verify(command.CurrentPassword, customer.PasswdLogin))
            return Error.Forbidden(description: "La contraseña actual no es correcta.");
        
        customer.PasswdLogin = BCrypt.Net.BCrypt.HashPassword(command.Password);
        customer.EmailValidated = true;
        
        await _customerRepository.UpdateCustomerAsync(customer);
        await _unitOfWork.CommitChangesAsync();
        
        return customer;
    }
}