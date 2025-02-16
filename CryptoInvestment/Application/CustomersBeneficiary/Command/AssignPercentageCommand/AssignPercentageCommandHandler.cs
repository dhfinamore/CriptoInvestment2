using CryptoInvestment.Application.Common.Interface;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.AssignPercentageCommand;

public class AssignPercentageCommandHandler : IRequestHandler<AssignPercentageCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerBeneficiaryRepository _beneficiaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPercentageCommandHandler(ICustomerRepository customerRepository, ICustomerBeneficiaryRepository beneficiaryRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _beneficiaryRepository = beneficiaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(AssignPercentageCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound("Customer not found");
        
        if (request.CustomerBeneficiaries.Sum(x => x.Porcent) != 100)
            return Error.Validation("The sum of the percentages must be 100");
        
        await _beneficiaryRepository.UpdateCustomerBeneficiaryAsync(request.CustomerBeneficiaries);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Success;
    }
}