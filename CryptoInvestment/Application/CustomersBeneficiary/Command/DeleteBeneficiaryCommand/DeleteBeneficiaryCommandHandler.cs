using CryptoInvestment.Application.Common.Interface;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.CustomersBeneficiary.Command.DeleteBeneficiaryCommand;

public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, ErrorOr<Deleted>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerBeneficiaryRepository _customerBeneficiaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBeneficiaryCommandHandler(
        ICustomerRepository customerRepository, 
        ICustomerBeneficiaryRepository customerBeneficiaryRepository, 
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _customerBeneficiaryRepository = customerBeneficiaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description : "Customer not found");
        
        var beneficiary = await _customerBeneficiaryRepository.GetBeneficiaryByIdAsync(request.BeneficiaryId);
        
        if (beneficiary is null)
            return Error.NotFound(description : "Beneficiary not found");
        
        if (beneficiary.IdCustomer != request.CustomerId)
            return Error.Forbidden(description : "Beneficiary does not belong to the customer");
        
        await _customerBeneficiaryRepository.DeleteBeneficiaryAsync(beneficiary);
        await _unitOfWork.CommitChangesAsync();
        
        return Result.Deleted;
    }
}