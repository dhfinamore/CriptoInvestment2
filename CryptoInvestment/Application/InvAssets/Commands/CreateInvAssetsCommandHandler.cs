using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Domain.InvOperations;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvAssets.Commands;

public class CreateInvAssetsCommandHandler : IRequestHandler<CreateInvAssetsCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IInvAssetsRepository _invAssetsRepository;
    private readonly IInvOperationRepository _invOperationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvAssetsCommandHandler(
        ICustomerRepository customerRepository,
        IInvAssetsRepository invAssetsRepository,
        IInvOperationRepository invOperationRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _invAssetsRepository = invAssetsRepository;
        _invOperationRepository = invOperationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(CreateInvAssetsCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(command.CustomerId);

        if (customer is null)
            return Error.NotFound(description: "Customer not found");

        var invAssets = new InvAsset()
        {
            IdCustomer = command.CustomerId,
            IdInvPlans = command.InvPlanId,
            IdCurrency = command.CurrencyId,
            Amount = command.Amount,
            ReinvestAmount = command.ReinvestAmount,
            EndType = command.EndType,
            ReinvestPercent = command.ReinvestPercent
        };

        if (command.ReinvestAmount < command.Amount)
        {
            var invOperation = new InvOperation()
            {
                IdCustomer = command.CustomerId,
                IdCurrency = command.CurrencyId,
                Amount = command.Amount - command.ReinvestAmount,
                IdInvPlans = command.InvPlanId,
                IdInvAction = 1,
                Status = 1
            };

            await _invOperationRepository.CreateInvOperationAsync(invOperation);
        }

        if (command.ReinvestAmount > 0)
        {
            var invOperation = new InvOperation()
            {
                IdCustomer = command.CustomerId,
                IdCurrency = command.CurrencyId,
                Amount = command.ReinvestAmount,
                IdInvPlans = command.InvPlanId,
                IdInvAction = 3,
                Status = 0
            };
            
            var invBalances = await _invAssetsRepository.GetInvBalances(command.CustomerId);
            var balance = invBalances.FirstOrDefault(b => b.IdCurrency == command.CurrencyId);
            
            if (balance is null)
                return Error.NotFound(description: "Balance not found");
            
            balance.Balance -= command.ReinvestAmount;
            
            await _invAssetsRepository.UpdateInvBalance(balance);
            await _invOperationRepository.CreateInvOperationAsync(invOperation);
        }

        if (command.ReinvestAmount == command.Amount)
        {
            invAssets.DateStart = DateTime.Now;
            invAssets.ExpectedProfit = command.ExpectedProfit;
        }

        await _invAssetsRepository.CreateInvAssetsAsync(invAssets);
        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}