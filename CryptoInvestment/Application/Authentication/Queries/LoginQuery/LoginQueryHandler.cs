using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Queries.LoginQuery;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public LoginQueryHandler(ICustomerRepository customerRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Customer>> Handle(Queries.LoginQuery.LoginQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(request.Email);
        
        if (customer is null)
            return Error.NotFound(description: "El usuario no fue encontrado.");

        if (customer.EmailValidated != true)
            return LoginQueryErrors.EmailNotVerified;
        
        if (!_passwordHasher.VerifyHashedPassword(customer.PasswdLogin!, request.Password))
        {
            if (customer.LockedUp.HasValue && customer.LockedUp.Value.AddMinutes(15) < DateTime.Now)
            {
                customer.FailedLoginAttempts = 0;
                customer.LockedUp = null;
                await _customerRepository.UpdateCustomerAsync(customer);
                await _unitOfWork.CommitChangesAsync();
            }
            
            if (customer.LockedUp.HasValue && customer.LockedUp.Value.AddMinutes(15) > DateTime.Now)
            {
                var minutesLeft = (int) (customer.LockedUp.Value.AddMinutes(15) - DateTime.Now).TotalMinutes;
                if (minutesLeft == 1)
                {
                    customer.FailedLoginAttempts = 0;
                    customer.LockedUp = null;
                    await _customerRepository.UpdateCustomerAsync(customer);
                    await _unitOfWork.CommitChangesAsync();
                }
                return LoginQueryErrors.AccountLocked;
            }
            
            customer.FailedLoginAttempts++;
            await _customerRepository.UpdateCustomerAsync(customer);
            
            if (customer.FailedLoginAttempts > 3)
            {
                customer.FailedLoginAttempts = 3;
                customer.LockedUp = DateTime.Now;
                await _customerRepository.UpdateCustomerAsync(customer);
                await _unitOfWork.CommitChangesAsync();
                return LoginQueryErrors.AccountLocked;
            }
            
            await _unitOfWork.CommitChangesAsync();
            return Error.Unauthorized(description: "Contraseña incorrecta.");
        }
        
        if (customer.FailedLoginAttempts > 0)
        {
            customer.FailedLoginAttempts = 0;
            customer.LockedUp = null;
            await _customerRepository.UpdateCustomerAsync(customer);
            await _unitOfWork.CommitChangesAsync();
        }
        
        if (!await _customerRepository.HasSecurityQuestionsAsync(customer.IdCustomer))
        {
            return LoginQueryErrors.SecurityQuestionsNotConfigured;
        }
        
        return customer;
    }
}