using CryptoInvestment.Application.Common.Interface;

using ErrorOr;

using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.SetWithdrawalsPasswordCommand;

public class SetWithdrawalsPasswordCommandHandler : IRequestHandler<SetWithdrawalsPasswordCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetWithdrawalsPasswordCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(SetWithdrawalsPasswordCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
        {
            return Error.NotFound(description: "Customer not found");
        }
        
        customer.PasswdWithdrawal = BCrypt.Net.BCrypt.HashPassword(request.WithdrawalsPassword);
        
        await _customerRepository.UpdateCustomerAsync(customer);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Success;
    }
}