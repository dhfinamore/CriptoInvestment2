using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
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
        Customer? customer = await _customerRepository.GetCustomerByIdAsync(command.CustomerId);
        if (customer is null)
        {
            return Error.NotFound("Customer not found");
        }

        InvAsset invAsset = new InvAsset()
        {
            IdCustomer = command.CustomerId,
            IdInvPlans = command.InvPlanId,
            IdCurrency = command.CurrencyId,
            Amount = command.Amount,
            ReinvestAmount = command.ReinvestAmount,
            EndType = command.EndType,
            ReinvestPercent = command.ReinvestPercent
        };
        
        List<InvOperation> operationsNeedingAssetId = [];

        if (command.ReinvestAmount < command.Amount)
        {
            InvOperation opDiff = new InvOperation()
            {
                IdCustomer = command.CustomerId,
                IdCurrency = command.CurrencyId,
                Amount = command.Amount - command.ReinvestAmount,
                IdInvPlans = command.InvPlanId,
                IdInvAction = 1,
                Status = 1
            };
            operationsNeedingAssetId.Add(opDiff);
        }

        if (command.ReinvestAmount > 0)
        {
            InvOperation opReinvest = new InvOperation()
            {
                IdCustomer = command.CustomerId,
                IdCurrency = command.CurrencyId,
                Amount = command.ReinvestAmount,
                IdInvPlans = command.InvPlanId,
                IdInvAction = 3,
                Status = 0
            };
            operationsNeedingAssetId.Add(opReinvest);

            List<InvBalance> invBalances = await _invAssetsRepository.GetInvBalances(command.CustomerId);
            InvBalance? balance = invBalances.FirstOrDefault(b => b.IdCurrency == command.CurrencyId);
            if (balance is null)
            {
                return Error.NotFound("Balance not found");
            }

            balance.Balance -= command.ReinvestAmount;
            await _invAssetsRepository.UpdateInvBalance(balance);
        }

        if (command.ReinvestAmount == command.Amount)
        {
            invAsset.DateStart = DateTime.Now;
            invAsset.ExpectedProfit = command.ExpectedProfit;
        }

        await _invAssetsRepository.CreateInvAssetsAsync(invAsset);
        await _unitOfWork.CommitChangesAsync();
        
        foreach (InvOperation op in operationsNeedingAssetId)
        {
            op.IdInvAssets = invAsset.IdInvAssets;
            await _invOperationRepository.CreateInvOperationAsync(op);
        }

        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}