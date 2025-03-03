using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvOperations;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Command;

public class CreateWithdrawCommandHandler : IRequestHandler<CreateWithdrawCommand, ErrorOr<Success>>
{
    private readonly IInvOperationRepository _invOperationRepository;
    private readonly IInvAssetsRepository _invAssetsRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWithdrawCommandHandler(IInvOperationRepository invOperationRepository, IInvAssetsRepository invAssetsRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _invOperationRepository = invOperationRepository;
        _invAssetsRepository = invAssetsRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(CreateWithdrawCommand request, CancellationToken cancellationToken)
    {
        var balance = await _invAssetsRepository.GetBalanceByIdAsync(request.BalanceId);
        
        if (balance is null)
            return Error.NotFound(description: "Balance not found");
        
        var customer = await _customerRepository.GetCustomerByIdAsync(balance.IdCustomer);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        if (balance.Balance < request.Amount)
            return Error.Failure(description: "Insufficient balance");
        
        balance.Balance -= request.Amount;
        
        var operation = new InvOperation
        {
            IdCustomer = customer.IdCustomer,
            IdCurrency = (int)balance.IdCurrency!,
            Amount = request.Amount,
            CustomerWalletId = request.WalletId,
            Status = 1,
            IdInvAction = 4
        };
        
        await _invAssetsRepository.UpdateInvBalance(balance);
        await _invOperationRepository.CreateInvOperationAsync(operation);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Success;
    }
}